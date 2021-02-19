using System.Collections;
using Global;
using UnityEngine;

namespace Networking.UI {
    public class UIManager : Singleton<UIManager> {

        public StartMenu startMenu;

        public RoomListCreateMenu roomListCreateMenu;

        public RoomMenu roomMenu;

        public CanvasGroup canvasGroup;
        public float fadeOutDuration;

        private void Start() {
            startMenu.ResetUI();
            roomListCreateMenu.ResetUI();
            roomMenu.ResetUI();
        
            roomListCreateMenu.gameObject.SetActive(false);
            roomMenu.gameObject.SetActive(false);
            startMenu.gameObject.SetActive(true);
        }

        public void OnConnectSuccess() {
            startMenu.gameObject.SetActive(false);
            startMenu.ResetUI();
        
            roomListCreateMenu.roomNameInputField.text = $"{Client.Instance.Name}'s room";
            roomListCreateMenu.gameObject.SetActive(true);
        }

        public void OnSelfRoomJoined(Room room) {
            roomListCreateMenu.gameObject.SetActive(false);
            roomListCreateMenu.ResetUI();
        
            roomMenu.OnSelfJoined(room);
            roomMenu.gameObject.SetActive(true);
        }

        public void OnSelfRoomLeft() {
            roomMenu.gameObject.SetActive(false);
            roomMenu.ResetUI();
        
            roomListCreateMenu.gameObject.SetActive(true);
        }

        public void OnDisconnect() {
            roomMenu.gameObject.SetActive(false);
            roomListCreateMenu.gameObject.SetActive(false);
            roomMenu.ResetUI();
            roomListCreateMenu.ResetUI();
        
            startMenu.gameObject.SetActive(true);
        }

        public void StartGame() {
            canvasGroup.interactable = false;
            StartCoroutine(FadeOut(fadeOutDuration));
        }

        private IEnumerator FadeOut(float secondsDuration) {

            const float updatesPerSecond = 60;
        
            while (canvasGroup.alpha > 0) {
                canvasGroup.alpha -= 1 / (secondsDuration * updatesPerSecond);
                yield return new WaitForSeconds(1 / updatesPerSecond);
            }

            Loader.Instance.StartLoad();
        }
    }
}
