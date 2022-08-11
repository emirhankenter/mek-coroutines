using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mek.Coroutines
{
    [CustomEditor(typeof(CoroutineWorker))]
    public class CoroutineWorkerEditor : Editor
    {
        private Object scriptObject;

        private CoroutineWorker _instance;
    
        void OnEnable()
        {
            string[] guids = AssetDatabase.FindAssets($"{nameof(CoroutineWorker)} t:Script");
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            scriptObject = AssetDatabase.LoadAssetAtPath<Object>(path);
            
            _instance = serializedObject.targetObject as CoroutineWorker;
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script:", scriptObject, typeof(Object), false);
            
            serializedObject.Update();
            
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Routines only be shown in runtime!", MessageType.Info);
                return;
            }
            
            EditorGUI.EndDisabledGroup();

            
            for (int i = 0; i < _instance.RunningRoutines.Count; i++)
            {
                EditorGUILayout.BeginVertical("HelpBox");
                EditorGUILayout.BeginHorizontal();
                var value = _instance.RunningRoutines[i];

                EditorGUILayout.LabelField(value);

                if (GUILayout.Button("X", GUILayout.MaxWidth(50)))
                {
                    CoroutineController.StopCoroutine(value);
                    Debug.LogWarning($"Routine \"{value}\" stopped!");
                }
                    
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            
            if (_instance.RunningRoutines.Count > 0 && GUILayout.Button("Stop All"))
            {
                var keys = new List<string>(_instance.RunningRoutines);
                var anyStoppedRoutine = false;
                foreach (var key in keys)
                {
                    CoroutineController.StopCoroutine(key);
                    if (!anyStoppedRoutine)
                    {
                        anyStoppedRoutine = true;
                    }
                }
                
                Debug.LogWarning("All routines have been stopped!");
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
