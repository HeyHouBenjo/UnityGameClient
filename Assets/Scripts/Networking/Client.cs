using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Global;
using Networking.Architecture;
using Networking.PacketTypes;
using Networking.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking {

    public class Client : PersistentSingleton<Client> {
        public delegate void PacketHandler(Packet packet);
        public static Dictionary<int, Dictionary<int, PacketHandler>> PacketHandlers;

        public string ip;
        public int port;

        public List<Color> colors;

        public Color ColorFromId(int id) {
            return colors[id];
        }
        
        public Room GameRoom { get; set; }
        public Dictionary<int, Room.ClientProperties> GameProperties { get; set; }

        private bool _isConnected;
        public TcpManager Tcp;
        public UdpManager Udp;

        public int Id { get; private set; }

        public string Name { get; private set; }

        private void Start() {
            SceneManager.sceneLoaded += (scene, mode) => {
                Debug.Log($"Loaded Scene {scene} with mode {mode}.");
            };
        }

        private void Update() {
            ThreadManager.UpdateMain();
        }

        private void OnApplicationQuit() {
            Disconnect();
        }

        public void ConnectToServer() {
            UIManager.Instance.startMenu.ConnectToServer();

            Tcp = new TcpManager();
            Udp = new UdpManager();
            InitializeClientData();
            
            Tcp.Connect();
        }

        public void OnConnected(int clientId, string message) {
            _isConnected = true;
            Id = clientId;
            Name = UIManager.Instance.startMenu.usernameField.text;
            Udp.Connect(((IPEndPoint)Tcp.Socket.Client.LocalEndPoint).Port);

            ClientSend.WelcomeReceived();
            
            RoomSend.List();
            UIManager.Instance.OnConnectSuccess();
            UIManager.Instance.roomListCreateMenu.OnRoomListRequest();

            Debug.Log("Connected successfully to " +
                      $"{Instance.Tcp.Socket.Client.RemoteEndPoint} : " +
                      $"{message}"
            );
        }

        private static void InitializeClientData() {
            PacketHandlers = new Dictionary<int, Dictionary<int, PacketHandler>> {
                {(int)PacketType.Default, new Dictionary<int, PacketHandler> {
                    {(int)ServerDefaultPacket.Welcome, ClientHandle.Welcome}
                }},
                {(int)PacketType.Room, new Dictionary<int, PacketHandler> {
                    {(int)ServerRoomPacket.RList, RoomHandle.List},
                    {(int)ServerRoomPacket.RCreated, RoomHandle.Created},
                    {(int)ServerRoomPacket.RJoined, RoomHandle.Joined},
                    {(int)ServerRoomPacket.RLeft, RoomHandle.Left},
                    {(int)ServerRoomPacket.RCreateFailed, ClientHandle.CreateFailed},
                    {(int)ServerRoomPacket.RJoinFailed, ClientHandle.JoinFailed},
                    {(int)ServerRoomPacket.RLeaveFailed, ClientHandle.LeaveFailed},
                    {(int)ServerRoomPacket.RKickFailed, ClientHandle.KickFailed},
                    {(int)ServerRoomPacket.RStart, RoomHandle.Start},
                    {(int)ServerRoomPacket.RProperties, RoomHandle.Properties},
                }}
            };
        }

        public void Disconnect() {
            if (!_isConnected)
                return;
            
            _isConnected = false;

            ThreadManager.ExecuteOnMainThread(UIManager.Instance.OnDisconnect);

            Tcp.Socket.Close();
            Udp.Socket.Close();

            Debug.Log("Disconnected from server.");
        }
    }
}
