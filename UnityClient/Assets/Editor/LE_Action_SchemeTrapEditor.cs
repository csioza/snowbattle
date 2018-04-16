using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(LE_Action_SchemeTrap)), CanEditMultipleObjects]

public class LE_Action_SchemeTrapEditor : Editor
{


    public override void OnInspectorGUI()
    {
        LE_Action_SchemeTrap cam = target as LE_Action_SchemeTrap;

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
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("trapType"));
                    EditorGUILayout.EndHorizontal();

                   

                    switch (list.GetArrayElementAtIndex(i).FindPropertyRelative("trapType").intValue)
                    {
                        case (int)TrapResult.ENTrapType.enSpecial:
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("schemeObj"));
                                EditorGUILayout.EndHorizontal();
                                break;
                            }
                        case (int)TrapResult.ENTrapType.CustomBlackboardStr:
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("trapBlackboardStr"));
                                EditorGUILayout.EndHorizontal();
                                break;
                            }
                    }
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("actionType"));
                    EditorGUILayout.EndHorizontal();     
                }

            }
            EditorGUI.indentLevel -= 1;
            NGUIEditorTools.EndContents();
    
        }
       
    }
}




