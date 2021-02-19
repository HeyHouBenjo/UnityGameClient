using Networking.Architecture;
using Networking.UI;
using UnityEngine;

namespace Networking {
    public static class ClientHandle {
        public static void Welcome(Packet packet) {
            var msg = packet.ReadString();
            var id = packet.ReadInt();

            Client.Instance.OnConnected(id, msg);
        }
        
        public static void CreateFailed(Packet packet) {
            string message = packet.ReadString();

            UIManager.Instance.roomListCreateMenu.createRoomButton.interactable = true;
            Debug.Log(message);
        }

        public static void JoinFailed(Packet packet) {
            string roomId = packet.ReadString();
            string message = packet.ReadString();
            
            UIManager.Instance.roomListCreateMenu.OnJoinFailed(roomId);
            Debug.Log(message);
        }

        public static void LeaveFailed(Packet packet) {
            string message = packet.ReadString();
            
            Debug.Log(message);
        }

        public static void KickFailed(Packet packet) {
            string message = packet.ReadString();
            
            Debug.Log(message);
        }
    }
}
