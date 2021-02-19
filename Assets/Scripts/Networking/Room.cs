using System.Collections.Generic;

namespace Networking {
    public class Room {
        
        public struct ClientProperties { 
            public bool IsLeader;
            public bool IsReady;
            public int ColorId;
        }

        public Room(string id, string name, int leaderId, int maxPlayers) {
            Id = id;
            Name = name;
            LeaderId = leaderId;
            MaxPlayers = maxPlayers;
        }

        public string Id { get; }

        public string Name { get; }
        
        public bool IsLocked { get; set; }

        public int LeaderId { get; }

        public int MaxPlayers { get; }

        public Dictionary<int, string> Players { get; set; }
    }
}
