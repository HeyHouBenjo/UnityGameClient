using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking.UI {
    public class RoomListItem : MonoBehaviour {

        public TextMeshProUGUI nameText;
        public TextMeshProUGUI slotsText;
        public TMP_InputField passwordField;

        public Button mainItemButton;

        public string Id { get; set; }

        public string Name {
            set => nameText.text = value;
        }
        
        private int _maxPlayers;
        public int MaxPlayers {
            get => _maxPlayers;
            set {
                _maxPlayers = value;
                UpdateSlotsText();
            }
        }
        
        private int _currentPlayers;
        public int CurrentPlayers {
            get => _currentPlayers;
            set {
                _currentPlayers = value;
                UpdateSlotsText();
            }
        }
        
        public bool IsLocked {
            set => mainItemButton.interactable = !value;
        }
        
        private void UpdateSlotsText() {
            slotsText.text = $"{CurrentPlayers} / {MaxPlayers}";
        }

        public void OnClicked() {
            mainItemButton.interactable = false;
            UIManager.Instance.roomListCreateMenu.OnTryJoinRoom(Id, passwordField.text);
        }
    }
}
