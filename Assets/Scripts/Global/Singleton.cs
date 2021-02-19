using UnityEngine;

namespace Global {
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

        public static T Instance;

        private void Awake() {
            if (Instance == null) {
                Instance = this as T;
            } else if (Instance != this) {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }
    }
    
    public class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour {

        public static T Instance;

        private void Awake() {
            if (Instance == null) {
                Instance = this as T;
                DontDestroyOnLoad(this);
            } else if (Instance != this) {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }
    }
}
