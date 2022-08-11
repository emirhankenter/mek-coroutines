using System.Collections.Generic;
using UnityEngine;

namespace Mek.Coroutines
{
    public class CoroutineWorker : MonoBehaviour
    {
        public List<string> RunningRoutines => CoroutineController.RoutineKeys;
        
        private static CoroutineWorker _instance;
        public static CoroutineWorker Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new GameObject("CoroutineWorker").AddComponent<CoroutineWorker>();
                DontDestroyOnLoad(_instance);
                return _instance;
            }
        }
    }
}