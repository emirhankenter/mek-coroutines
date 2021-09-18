using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif

namespace Mek.Coroutines
{
    public class CoroutineWorker : MonoBehaviour
    {
        #if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ShowInInspector, ReadOnly]
        #endif
        private List<string> RunningRoutines => CoroutineController.RoutineKeys;
        
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