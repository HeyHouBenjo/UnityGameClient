using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking.UI {
    public class PlayerSlotItem : MonoBehaviour {

        public TextMeshProUGUI nameText;
        
        public Sprite readySprite;
        public Sprite notReadySprite;
        public Image readyButtonImage;
        
        public Button leaveButton;
        public Button readyButton;
        public Button promoteButton;
        
        public Image colorImage;

        public bool IsReady { get; set; }

        public int ClientId { get; set; }

        public bool IsLeader { get; set; }
        
        public int ColorId { get; set; }
        

        public void OnLeaveButtonClicked() {
            if (ClientId.Equals(Client.Instance.Id))
                RoomSend.Leave();
            else
                RoomSend.Kick(ClientId);
        }

        public void OnReadyButtonClicked() {
            RoomSend.Ready(!IsReady);
        }

        public void OnPromoteButtonClicked() {
            RoomSend.Leader(ClientId);
        }

        public void UpdateUI(int leaderId) {
            int myClientId = Client.Instance.Id;
            readyButtonImage.sprite = IsReady ? readySprite : notReadySprite;
            readyButton.interactable = ClientId.Equals(myClientId);
            promoteButton.interactable = leaderId.Equals(myClientId);
            promoteButton.gameObject.SetActive(!ClientId.Equals(leaderId));
            leaveButton.interactable = ClientId.Equals(myClientId) || leaderId.Equals(myClientId);
            colorImage.color = Client.Instance.ColorFromId(ColorId);
        }
    }
}
