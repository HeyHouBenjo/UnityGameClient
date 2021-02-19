using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking.UI {
    public class RoomListCreateMenu : MonoBehaviour {

        [Header("Room List Menu")]
        public Transform roomListContainer;
        public RoomListItem roomListItemPrefab;
        public GameObject listLoadIndicator;

        [Header("Room Create Menu")]
        public TMP_InputField roomNameInputField;
        public TMP_InputField roomPasswordInputField;
        public Slider maxPlayersSlider;
        public TextMeshProUGUI maxPlayersText;
        public Button createRoomButton;
        public GameObject createLoadIndicator;
        

        public void OnRoomListRequest() {
            listLoadIndicator.SetActive(true);
        }

        public void OnRoomListReceived(IEnumerable<Room> rooms) {
            listLoadIndicator.SetActive(false);

            for (var i = 0; i < roomListContainer.childCount; i++)
                Destroy(roomListContainer.GetChild(i).gameObject);

            foreach (var room in rooms) {
                var roomListItem = Instantiate(roomListItemPrefab, roomListContainer);

                roomListItem.Name = room.Name;
                roomListItem.Id = room.Id;
                roomListItem.IsLocked = room.IsLocked;
                roomListItem.MaxPlayers = room.MaxPlayers;
                roomListItem.CurrentPlayers = room.Players.Count;
            }
        }

        public void OnMaxPlayersSliderChanged(float value) {
            maxPlayersText.text = ((int)value).ToString();
        }

        public void OnCreateButtonClicked() {
            createRoomButton.interactable = false;
            createLoadIndicator.SetActive(true);
            RoomSend.Create(
                roomNameInputField.text,
                roomPasswordInputField.text,
                (int)maxPlayersSlider.value
            );
        }

        public void OnTryJoinRoom(string id, string password) {
            listLoadIndicator.SetActive(true);
            RoomSend.Join(id, password);
        }

        public void OnJoinFailed(string roomId) {
            listLoadIndicator.SetActive(false);
            
            roomListContainer.GetComponentsInChildren<RoomListItem>().
                Single(item => item.Id.Equals(roomId)).
                GetComponent<Button>().interactable = true;
        }

        public void ResetUI() {
            for (var i = 0; i < roomListContainer.childCount; i++)
                Destroy(roomListContainer.GetChild(i).gameObject);
            
            roomNameInputField.text = $"{Client.Instance.Name}'s room";
            roomPasswordInputField.text = "";
            maxPlayersSlider.value = 4;
            maxPlayersText.text = "4";
            createRoomButton.interactable = true;
            createLoadIndicator.SetActive(false);
            listLoadIndicator.SetActive(false);
        }
    }
}
