using System.Collections;
using System.Collections.Generic;
using BrainDesigner.Scripts.Editor;
using UnityEditor;
using UnityEngine;

namespace BrainDesigner.Scripts
{
    [CustomEditor(typeof(BrainDesigner))]
    public class BrainDesignerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            BrainDesigner brainDesigner = (BrainDesigner)target;
            serializedObject.Update();

            DrawDefaultInspector();

            EditorGUILayout.Space();

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold
            };


            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Design the brain", buttonStyle, GUILayout.Height(22),
                   GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.6f)))
                BrainDesignerEditorWindow.OpenWindow();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
