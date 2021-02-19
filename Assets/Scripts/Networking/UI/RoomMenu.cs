using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking.UI {
    public class RoomMenu : MonoBehaviour {

        public TextMeshProUGUI roomNameText;
        public GameObject roomPlayerList;
        public PlayerSlotItem playerSlotItemPrefab;
        public RectTransform emptySlotItemPrefab;
        public Button startGameButton;
        public TextMeshProUGUI countdownText;
        public Transform colorList;
        public Button colorButtonPrefab;

        private IEnumerable<PlayerSlotItem> PlayerItems => GetComponentsInChildren<PlayerSlotItem>();

        private PlayerSlotItem GetPlayerItem(int clientId) {
            return PlayerItems.Single(item => item.ClientId.Equals(clientId));
        }

        private bool IsCountingDown { get; set; }

        private DateTime StartTime { get; set; }
        public void OnStartGameButtonClicked() {
            startGameButton.interactable = false;
            RoomSend.Start();
        }

        private void Start() {
            foreach (var color in Client.Instance.colors) {
                var button = Instantiate(colorButtonPrefab, colorList);
                button.onClick.AddListener(
                    () => RoomSend.Color(button.transform.GetSiblingIndex())
                );
                button.GetComponent<Image>().color = color;
            }
        }

        private void Update() {
            if (!IsCountingDown)
                return;

            var timeLeft = StartTime - DateTime.Now;
            var totalSeconds = (int)timeLeft.TotalSeconds;
            var deciSeconds = timeLeft.Milliseconds / 100;
            countdownText.text = $"{totalSeconds}.{deciSeconds}";
            
            if (timeLeft.Ticks > 0)
                return;
            
            StopCountdown();

            UIManager.Instance.StartGame();
        }

        public void StartCountdown(long startTimeTicks) {
            IsCountingDown = true;
            StartTime = new DateTime(startTimeTicks);
            countdownText.gameObject.SetActive(true);
        }

        private void StopCountdown() {
            IsCountingDown = false;
            countdownText.gameObject.SetActive(false);
        }
        
        public void ResetUI() {
            StopCountdown();
            startGameButton.interactable = false;
            for (var i = 0; i < roomPlayerList.transform.childCount; i++)
                Destroy(roomPlayerList.transform.GetChild(i).gameObject);
            //TODO Apply default Game settings
        }

        public void OnSelfJoined(Room room) {
            roomNameText.text = room.Name;
            foreach (var kvp in room.Players) {
                var playerItem = Instantiate(playerSlotItemPrefab, roomPlayerList.transform);
                playerItem.nameText.text = kvp.Value;
                playerItem.ClientId = kvp.Key;
                if (playerItem.ClientId.Equals(Client.Instance.Id))
                    playerItem.nameText.fontStyle = FontStyles.Bold;
            }

            for (var i = 0; i < room.MaxPlayers - room.Players.Count; i++)
                Instantiate(emptySlotItemPrefab, roomPlayerList.transform);

            //TODO Display Game settings
        }

        public void OnOtherJoined(int clientId, Room room) {
            StopCountdown();
            var slotIndex = room.Players.Count - 1;
            Destroy(roomPlayerList.transform.GetChild(slotIndex).gameObject);
            var playerItem = Instantiate(playerSlotItemPrefab, roomPlayerList.transform);
            playerItem.transform.SetSiblingIndex(slotIndex);
            playerItem.nameText.text = room.Players[clientId];
            playerItem.ClientId = clientId;
        }

        public void OnOtherLeft(int clientId) {
            StopCountdown();
            var slotIndex = GetPlayerItem(clientId).transform.GetSiblingIndex();
            Destroy(roomPlayerList.transform.GetChild(slotIndex).gameObject);
            Instantiate(emptySlotItemPrefab, roomPlayerList.transform);
        }

        public void OnClientPropertiesUpdate(Dictionary<int, Room.ClientProperties> props, int leaderId) {

            var usedColorIds = new List<int>();
            
            foreach (var kvp in props) {
                var item = GetPlayerItem(kvp.Key);
                item.IsReady = kvp.Value.IsReady;
                item.IsLeader = kvp.Value.IsLeader;
                item.ColorId = kvp.Value.ColorId;
                item.UpdateUI(leaderId);
                usedColorIds.Add(item.ColorId);
            }

            for (int i = 0; i < colorList.childCount; i++) {
                colorList.GetChild(i).GetComponent<Button>().interactable = !usedColorIds.Contains(i);
            }
            
            startGameButton.interactable = 
                Client.Instance.Id.Equals(leaderId) && 
                PlayerItems.All(item => item.IsReady);

        }

        public void OnGameSettingsUpdate() {
            
        }
    }
}
