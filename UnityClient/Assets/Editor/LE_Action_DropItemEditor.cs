using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(LE_Action_DropItem)), CanEditMultipleObjects]

public class LE_Action_DropItemEditor : Editor
{


    public override void OnInspectorGUI()
    {
        LE_Action_DropItem cam = target as LE_Action_DropItem;

        GUILayout.Label(new GUIContent("The Serialize No.:" + cam.SerializeNo));

        serializedObject.Update();

        DrawDefaultInspector();

        ShowList(serializedObject.FindProperty("List"));
            

        serializedObject.ApplyModifiedProperties();
    }

    static void ShowList(SerializedProperty list)
    {
        if (!list.isArray)
        {
            return;
        }
   
        if (NGUIEditorTools.DrawMinimalisticHeader("List"))
        {
            NGUIEditorTools.BeginContents(true);
            SerializedProperty size = list.FindPropertyRelative("Array.size");
            EditorGUILayout.PropertyField(size);
            EditorGUI.indentLevel += 1;
            for (int i = 0; i < list.arraySize; i++)
            {

                if (NGUIEditorTools.DrawMinimalisticHeader("Element" + i))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("type"));
                    EditorGUILayout.EndHorizontal();



                    switch (list.GetArrayElementAtIndex(i).FindPropertyRelative("type").intValue)
                    {
                        case (int)DropItemResult.ID.key:
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("keyId"));
                                EditorGUILayout.EndHorizontal();
                                break;
                            }                     
                    }

                }

            }
            EditorGUI.indentLevel -= 1;
            NGUIEditorTools.EndContents();
    
        }
       
    }
}




