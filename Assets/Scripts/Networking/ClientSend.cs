using Networking.Architecture;
using Networking.PacketTypes;
using static Networking.Architecture.SendData;
using static Networking.PacketTypes.ClientDefaultPacket;

namespace Networking {
    public static class ClientSend {
        
        private static Packet CreatePacket(ClientDefaultPacket type) {
            return new Packet((int)PacketType.Default, (int)type);
        }

        public static void WelcomeReceived() {
            using var packet = CreatePacket(DWelcomeReceived);

            packet.Write(Client.Instance.Name);

            SendTcpData(packet);
        }
    }
}
