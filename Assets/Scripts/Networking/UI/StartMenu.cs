using UnityEngine;
using UnityEngine.UI;

namespace Networking.UI {
    public class StartMenu : MonoBehaviour {

        [Header("Start Menu")]
        public InputField usernameField;
        public Button connectButton;
        public GameObject connectingIndicator;

        public void ConnectToServer() {
            usernameField.interactable = false;
            connectButton.interactable = false;
            connectingIndicator.SetActive(true);
        }

        public void ResetUI() {
            usernameField.interactable = true;
            connectButton.interactable = true;
            connectingIndicator.SetActive(false);
        }
    }
}
