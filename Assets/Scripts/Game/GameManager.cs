using System;
using System.Collections.Generic;
using Global;
using Networking;

namespace Game {
    public class GameManager : Singleton<GameManager> {
        
        public PlayerManager playerPrefab;
        
        private Dictionary<int, PlayerManager> Players { get; } = new Dictionary<int, PlayerManager>();

        private void Start() {
            Loader.Instance.gameObject.SetActive(false);
            CreatePlayers();
        }

        private void CreatePlayers() {
            var room = Client.Instance.GameRoom;
            var clientPropertiesMap = Client.Instance.GameProperties;
            var colors = Client.Instance.colors;

            foreach (var kvp in room.Players) {
                var player = Instantiate(playerPrefab);
                player.Id = kvp.Key;
                player.Name = kvp.Value;
                player.Color = colors[clientPropertiesMap[player.Id].ColorId];
                Players.Add(player.Id, player);
            }
        }
        
        private void Update() { }
    }
}
