using System;
using System.Collections;
using Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Global {
    public class Loader : PersistentSingleton<Loader> {

        public Slider progressSlider;

        private void Start() {
            ResetUI();
        }

        private void ResetUI() {
            progressSlider.gameObject.SetActive(false);
            progressSlider.value = 0;
        }

        public void StartLoad() {
            progressSlider.gameObject.SetActive(true);
            StartCoroutine(Load("Game"));
        }

        private IEnumerator Load(string sceneName) {
            var operation = SceneManager.LoadSceneAsync(sceneName);

            while (!operation.isDone) {
                float progress = Mathf.Clamp01(operation.progress / .9f);
                progressSlider.value = progress;
                
                yield return null;
            }
        }
        
    }
}
