using System;
using System.Collections;

namespace Mek.Coroutines
{
    public static class Extensions
    {
        public static void StartCoroutine(this IEnumerator coroutine, string key)
        {
            if (coroutine == null)
            {
                throw new System.ArgumentNullException("Null routine");
            }

            CoroutineController.StartCoroutine(key, coroutine);
        }
        public static void StartCoroutine(this IEnumerator coroutine)
        {
            if (coroutine == null)
            {
                throw new System.ArgumentNullException("Null routine");
            }

            CoroutineController.StartCoroutine(coroutine);
        }

        public static void OnComplete(this IEnumerator routine, Action onComplete)
        {
            CoroutineController.RoutineFinished += OnRoutineFinished;
            void OnRoutineFinished(string obj)
            {
                CoroutineController.RoutineFinished -= OnRoutineFinished;
                onComplete?.Invoke();
            }
        }

        public static void OnRoutineWithKeyCompleted(this string key, Action onComplete)
        {
            CoroutineController.RoutineFinished += OnRoutineFinished;
            void OnRoutineFinished(string obj)
            {
                if (obj != key) return;
                CoroutineController.RoutineFinished -= OnRoutineFinished;
                onComplete?.Invoke();
            }
        }

        public static string GenerateUniqueString()
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=","");
            GuidString = GuidString.Replace("+","");

            return GuidString;
        }
    }
}
