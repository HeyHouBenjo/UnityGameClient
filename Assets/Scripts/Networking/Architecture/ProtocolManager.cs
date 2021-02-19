using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Networking.Architecture {

    public class TcpManager {
        private const int DataBufferSize = 4096;
        private byte[] _receiveBuffer;
        private Packet _receivedData;

        private NetworkStream _stream;
        public TcpClient Socket;

        public bool IsConnected => Socket != null;

        public void Connect() {
            Socket = new TcpClient {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };

            _receiveBuffer = new byte[DataBufferSize];
            Socket.BeginConnect(Client.Instance.ip, Client.Instance.port, ConnectCallback, Socket);
        }

        private void ConnectCallback(IAsyncResult result) {
            Socket.EndConnect(result);

            if (!Socket.Connected)
                return;

            _stream = Socket.GetStream();

            _receivedData = new Packet();

            _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
        } // ReSharper disable Unity.PerformanceAnalysis
        public void SendData(Packet packet) {
            try {
                if (Socket != null)
                    _stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            } catch (Exception ex) {
                Debug.Log($"Error sending data to server via TCP: {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result) {
            try {
                var byteLength = _stream.EndRead(result);
                if (byteLength <= 0) {
                    Client.Instance.Disconnect();
                    return;
                }

                var data = new byte[byteLength];
                Array.Copy(_receiveBuffer, data, byteLength);

                _receivedData.Reset(HandleData(data));
                _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
            } catch {
                Disconnect();
            }
        }

        private bool HandleData(byte[] data) {
            var packetLength = 0;

            _receivedData.SetBytes(data);

            if (_receivedData.UnreadLength() >= 4) {
                packetLength = _receivedData.ReadInt();
                if (packetLength <= 0)
                    return true;
            }

            while (packetLength > 0 && packetLength <= _receivedData.UnreadLength()) {
                var packetBytes = _receivedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() => ReadPacket.Read(packetBytes));

                packetLength = 0;
                if (_receivedData.UnreadLength() < 4)
                    continue;
                packetLength = _receivedData.ReadInt();
                if (packetLength <= 0)
                    return true;
            }

            return packetLength <= 1;
        }

        private void Disconnect() {
            Client.Instance.Disconnect();
            
            _stream = null;
            _receivedData = null;
            _receiveBuffer = null;
            Socket = null;
        }
    }

    public class UdpManager {
        private IPEndPoint _endPoint;
        public UdpClient Socket;

        public UdpManager() {
            _endPoint = new IPEndPoint(IPAddress.Parse(Client.Instance.ip), Client.Instance.port);
        }

        public void Connect(int localPort) {
            Socket = new UdpClient(localPort);

            Socket.Connect(_endPoint);
            Socket.BeginReceive(ReceiveCallback, null);

            using var packet = new Packet();
            SendData(packet);
        }

        public void SendData(Packet packet) {
            try {
                packet.InsertInt(Client.Instance.Id);
                Socket?.BeginSend(packet.ToArray(), packet.Length(), null, null);
            } catch (Exception ex) {
                Debug.Log($"Error sending data to server via UDP: {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result) {
            try {
                var data = Socket.EndReceive(result, ref _endPoint);
                Socket.BeginReceive(ReceiveCallback, null);

                if (data.Length < 4) {
                    Client.Instance.Disconnect();
                    return;
                }

                HandleData(data);
            } catch {
                Disconnect();
            }
        }

        private static void HandleData(byte[] data) {
            using (var packet = new Packet(data)) {
                var packetLength = packet.ReadInt();
                data = packet.ReadBytes(packetLength);
            }
            ThreadManager.ExecuteOnMainThread(() => ReadPacket.Read(data));
        }

        private void Disconnect() {
            Client.Instance.Disconnect();
            
            Socket = null;
            _endPoint = null;
        }
    }


    internal static class ReadPacket {
        public static void Read(byte[] data){
            using var packet = new Packet(data);
            var packetTypeId = packet.ReadInt();
            var packetActionId = packet.ReadInt();

            Client.PacketHandlers[packetTypeId][packetActionId](packet);
        }
    }
    
}
