using Networking.Architecture;
using Networking.PacketTypes;
using static Networking.Architecture.SendData;
using static Networking.PacketTypes.ClientRoomPacket;

namespace Networking {
    public static class RoomSend {

        private static Packet CreatePacket(ClientRoomPacket type) {
            return new Packet((int)PacketType.Room, (int)type);
        }

        public static void List() {
            using var packet = CreatePacket(RList);

            SendTcpData(packet);
        }

        public static void Create(string name, string password, int maxPlayers) {
            using var packet = CreatePacket(RCreate);

            packet.Write(name);
            packet.Write(password);
            packet.Write(maxPlayers);

            SendTcpData(packet);
        }

        public static void Join(string roomId, string password) {
            using var packet = CreatePacket(RJoin);

            packet.Write(roomId);
            packet.Write(password);

            SendTcpData(packet);
        }

        public static void Leave() {
            using var packet = CreatePacket(RLeave);

            SendTcpData(packet);
        }

        public static void Kick(int clientId) {
            using var packet = CreatePacket(RKick);

            packet.Write(clientId);

            SendTcpData(packet);
        }

        public static void Leader(int nextLeaderId) {
            using var packet = CreatePacket(RLeader);
            
            packet.Write(nextLeaderId);
            
            SendTcpData(packet);
        }
        
        public static void Color(int colorId) {
            using var packet = CreatePacket(RColor);
            
            packet.Write(colorId);
            
            SendTcpData(packet);
        }

        public static void Ready(bool isReady) {
            using var packet = CreatePacket(RReady);
            
            packet.Write(isReady);
            
            SendTcpData(packet);
        }

        public static void Start() {
            using var packet = CreatePacket(RStart);
            
            SendTcpData(packet);
        }
    }
}
