using System;
using System.Collections.Generic;

namespace MMORPG_Server.Package {
    public class ThreadManager {
        private static readonly List<Action> _ExecuteOnMainThread = new List<Action>();
        private static readonly List<Action> _ExecuteCopiedOnMainThread = new List<Action>();
        private static bool _actionToExecuteOnMainThread = false;
        public static void UpdateMain() {
            if (_actionToExecuteOnMainThread) {
                _ExecuteCopiedOnMainThread.Clear();
                lock (_ExecuteOnMainThread) {
                    _ExecuteCopiedOnMainThread.AddRange(_ExecuteOnMainThread);
                    _ExecuteOnMainThread.Clear();
                    _actionToExecuteOnMainThread = false;
                }

                for (int i = 0; i < _ExecuteCopiedOnMainThread.Count; i++) {
                    _ExecuteCopiedOnMainThread[i]();
                }
            }
        }

        public static void ExecuteOnMainThread(Action action) {
            if (action == null) {
                Console.WriteLine("No action to execute on main thread!");
                return;
            }

            lock (_ExecuteOnMainThread) {
                _ExecuteOnMainThread.Add(action);
                _actionToExecuteOnMainThread = true;
            }
        }
    }
}