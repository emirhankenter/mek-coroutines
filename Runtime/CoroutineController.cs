using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mek.Coroutines
{
    public static class CoroutineController
    {
        public static event Action<string> RoutineFinished; 
        
        private static Dictionary<string, IEnumerator> _coroutineDictionary = new Dictionary<string, IEnumerator>();

        public static List<string> RoutineKeys => _coroutineDictionary.Keys.ToList();

        public static void StartCoroutine(string key, IEnumerator coroutine)
        {
            StartMyCoroutine(key, coroutine);
        }
        
        public static void StartCoroutine(IEnumerator coroutine)
        {
            StartCoroutine(GenerateKey(coroutine), coroutine);
        }
        
        public static bool IsCoroutineRunning(string key)
        {
            return _coroutineDictionary.ContainsKey(key);
        }

        public static bool IsCoroutineRunning(IEnumerator coroutine)
        {
            return IsCoroutineRunning(GenerateKey(coroutine));
        }

        public static void StopCoroutine(string key)
        {
            if (_coroutineDictionary.TryGetValue(key, out IEnumerator value))
            {
                CoroutineWorker.Instance.StopCoroutine(value);
                _coroutineDictionary.Remove(key);
                RoutineFinished?.Invoke(key);
            }
        }

        private static Coroutine StartMyCoroutine(string key, IEnumerator coroutine)
        {
            return CoroutineWorker.Instance.StartCoroutine(GenericCoroutine(key, coroutine));
        }

        private static IEnumerator GenericCoroutine(string key, IEnumerator coroutine)
        {
            if (_coroutineDictionary.ContainsKey(key))
            {
                Debug.LogError($"There is a routine running with key {key}!");
                yield break;
            }
            _coroutineDictionary.Add(key, coroutine);
            yield return CoroutineWorker.Instance.StartCoroutine(coroutine);
            _coroutineDictionary.Remove(key);
            RoutineFinished?.Invoke(key);
        }

        public static void ToggleRoutine(bool state, string routineKey, IEnumerator routine)
        {
            if (state && !IsCoroutineRunning(routineKey))
            {
                StartCoroutine(routineKey, routine);
            }

            if (!state && IsCoroutineRunning(routineKey))
            {
                StopCoroutine(routineKey);
            }
        }

        #region Helpers

        public static void DoAfterGivenTime(float time, Action onComplete)
        {
            DoAfterTime(time, onComplete);
        }

        public static void DoAfterFixedUpdate(Action onComplete)
        {
            CoroutineWorker.Instance.StartCoroutine(FixedUpdateRoutine());

            IEnumerator FixedUpdateRoutine()
            {
                yield return new WaitForFixedUpdate();
                onComplete?.Invoke();
            }
        }
        public static void DoAfterCondition(Func<bool> condition, Action onComplete)
        {
            CoroutineWorker.Instance.StartCoroutine(ConditionRoutine(condition, () => { onComplete?.Invoke(); }));
        }

        private static void DoAfterTime(float time, Action onComplete)
        {
            CoroutineWorker.Instance.StartCoroutine(TimeRoutine(time, () => { onComplete?.Invoke();}));
        }

        private static IEnumerator TimeRoutine(float timer, Action onComplete)
        {
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null;
            }

            onComplete?.Invoke();
        }

        private static IEnumerator ConditionRoutine(Func<bool> condition, Action onComplete)
        {
            yield return new WaitUntil(condition);

            onComplete?.Invoke();
        }

        private static string GenerateKey(IEnumerator iEnumerator)
        {
            if (iEnumerator != null)
            {
                var type = iEnumerator.GetType();
                return $"{type.DeclaringType?.Name}{type.Name}";
            }

            return "";
        }

        #endregion
    }
}
