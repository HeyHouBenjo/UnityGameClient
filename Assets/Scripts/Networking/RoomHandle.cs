using System.Collections.Generic;
using System.Linq;
using Networking.Architecture;
using Networking.UI;

namespace Networking {
    public static class RoomHandle {
        public static void List(Packet packet) {
            var rooms = new List<Room>();

            var roomCount = packet.ReadInt();
            for (var i = 0; i < roomCount; i++)
                rooms.Add(packet.ReadRoom());

            UIManager.Instance.roomListCreateMenu.OnRoomListReceived(rooms);
        }

        public static void Created(Packet packet) {
            var room = packet.ReadRoom();

            UIManager.Instance.OnSelfRoomJoined(room);
            
            Properties(packet);
        }

        public static void Joined(Packet packet) {
            var room = packet.ReadRoom();
            var clientId = packet.ReadInt();

            if (clientId.Equals(Client.Instance.Id))
                UIManager.Instance.OnSelfRoomJoined(room);
            else
                UIManager.Instance.roomMenu.OnOtherJoined(clientId, room);
            
            Properties(packet);
        }

        public static void Left(Packet packet) {
            int clientId = packet.ReadInt();

            if (clientId.Equals(Client.Instance.Id)) {
                UIManager.Instance.OnSelfRoomLeft();
            } else {
                UIManager.Instance.roomMenu.OnOtherLeft(clientId);
                Properties(packet);
            }
        }

        public static void Properties(Packet packet) {
            var clientPropertiesMap = packet.ReadClientProperties();
            var leaderId = clientPropertiesMap.Single(pair => pair.Value.IsLeader).Key;
            
            UIManager.Instance.roomMenu.OnClientPropertiesUpdate(clientPropertiesMap, leaderId);
        }

        public static void Start(Packet packet) {
            var startTimeTicks = packet.ReadLong();
            var room = packet.ReadRoom();
            var clientPropertiesMap = packet.ReadClientProperties();
            UIManager.Instance.roomMenu.StartCountdown(startTimeTicks);
            Client.Instance.GameRoom = room;
            Client.Instance.GameProperties = clientPropertiesMap;
        }
    }
}
