using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(LE_Event_SchemeTrap)), CanEditMultipleObjects]

public class LE_Event_SchemeTrapEditor : Editor
{


    public override void OnInspectorGUI()
    {
        LE_Event_SchemeTrap cam = target as LE_Event_SchemeTrap;

        GUILayout.Label(new GUIContent("The Serialize No.:" + cam.SerializeNo));

        serializedObject.Update();

        DrawDefaultInspector();

        ShowList(serializedObject.FindProperty("schemeList"));

        SerializedProperty list = serializedObject.FindProperty("ActionList");
        if (NGUIEditorTools.DrawMinimalisticHeader("ActionList"))
        {
            NGUIEditorTools.BeginContents(true);

            SerializedProperty size = list.FindPropertyRelative("Array.size");
            EditorGUILayout.PropertyField(size);

            EditorGUI.indentLevel += 1;
            for (int i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel -= 1;

            NGUIEditorTools.EndContents();

        }
        serializedObject.ApplyModifiedProperties();
    }

    static void ShowList(SerializedProperty list)
    {
        if (!list.isArray)
        {
            return;
        }
        
        if (NGUIEditorTools.DrawMinimalisticHeader("SchemeList"))
        {
            NGUIEditorTools.BeginContents(true);
            SerializedProperty size = list.FindPropertyRelative("Array.size");
            EditorGUILayout.PropertyField(size);

            EditorGUI.indentLevel += 1;
            for (int i = 0; i < list.arraySize; i++)
            {
               
                if (NGUIEditorTools.DrawMinimalisticHeader("Scheme" + i))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("type"));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("target"));
                    EditorGUILayout.EndHorizontal(); 
                }

            }
            EditorGUI.indentLevel -= 1;
            NGUIEditorTools.EndContents();
    
        }
       
    }
}




