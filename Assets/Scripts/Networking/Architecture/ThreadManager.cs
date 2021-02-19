using System;
using System.Collections.Generic;
using UnityEngine;

namespace Networking.Architecture {
    public static class ThreadManager {
        private static readonly List<Action> ToExecuteOnMainThread = new List<Action>();
        private static readonly List<Action> ExecuteCopiedOnMainThread = new List<Action>();
        private static bool _actionToExecuteOnMainThread;

        /// <summary>Sets an action to be executed on the main thread.</summary>
        /// <param name="action">The action to be executed on the main thread.</param>
        public static void ExecuteOnMainThread(Action action) {
            if (action == null) {
                Debug.Log("No action to execute on main thread!");
                return;
            }

            lock (ToExecuteOnMainThread) {
                ToExecuteOnMainThread.Add(action);
                _actionToExecuteOnMainThread = true;
            }
        }

        /// <summary>Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread.</summary>
        public static void UpdateMain() {
            if (!_actionToExecuteOnMainThread)
                return;

            ExecuteCopiedOnMainThread.Clear();
            lock (ToExecuteOnMainThread) {
                ExecuteCopiedOnMainThread.AddRange(ToExecuteOnMainThread);
                ToExecuteOnMainThread.Clear();
                _actionToExecuteOnMainThread = false;
            }

            foreach (var t in ExecuteCopiedOnMainThread)
                t();
        }
    }
}
