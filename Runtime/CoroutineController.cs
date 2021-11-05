using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace Mek.Coroutines
{
    public static class CoroutineController
    {
        public static event Action<string> RoutineFinished; 
        
        private static Dictionary<string, IEnumerator> _coroutineDictionary = new Dictionary<string, IEnumerator>();
        
        private static readonly Random _random = new Random();

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

        public static void DoAfterGivenTime(float time, Action onComplete, string key = "")
        {
            if (key == "")
            {
                key = $"TimeRoutine{GenerateRandomKey()}";
            }
            StartCoroutine(key, TimeRoutine(time, onComplete));
        }

        public static void DoAfterFixedUpdate(Action onComplete, string key = "")
        {
            if (key == "")
            {
                key = $"FixedUpdateRoutine{GenerateRandomKey()}";
            }
            StartCoroutine(key, FixedUpdateRoutine(onComplete));
        }
        public static void DoAfterCondition(Func<bool> condition, Action onComplete, string key = "")
        {
            if (key == "")
            {
                key = $"ConditionRoutine_{GenerateRandomKey()}";
            }
            StartCoroutine(key, ConditionRoutine(condition, onComplete));
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

        private static IEnumerator FixedUpdateRoutine(Action onComplete)
        {
            yield return new WaitForFixedUpdate();
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
        
        public static string GenerateRandomKey(int size = 20, bool lowerCase = false)  
        {  
            var builder = new StringBuilder(size);  
  
            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';  
            const int lettersOffset = 26; // A...Z or a..z: length=26  
  
            for (var i = 0; i < size; i++)  
            {  
                var @char = (char)_random.Next(offset, offset + lettersOffset);  
                builder.Append(@char);  
            }  
  
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();  
        }

        #endregion
    }
}
