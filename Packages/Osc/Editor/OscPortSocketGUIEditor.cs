using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Osc
{
    [CustomEditor(typeof(OscPortSocketGUI))]
    public class OscPortSocketGUIEditor : Editor
    {
        SerializedProperty onReceive;
        SerializedProperty onError;
        SerializedProperty receiveMode;

        private void OnEnable()
        {
            onReceive = serializedObject.FindProperty("OnReceive");
            onError = serializedObject.FindProperty("OnError");
            receiveMode = serializedObject.FindProperty("receiveMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(onReceive);
            EditorGUILayout.PropertyField(onError);
            EditorGUILayout.PropertyField(receiveMode);
        }
    }
}
