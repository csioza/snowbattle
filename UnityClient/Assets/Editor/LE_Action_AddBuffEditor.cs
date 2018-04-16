using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(LE_Action_AddBuff)), CanEditMultipleObjects]

public class LE_Action_AddBuffEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LE_Action_AddBuff cam = target as LE_Action_AddBuff;

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
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("actorType"));
                    EditorGUILayout.EndHorizontal();



                    switch (list.GetArrayElementAtIndex(i).FindPropertyRelative("actorType").intValue)
                    {
                        case (int)LE_Action_AddBuffInfo.Type.specifiedNPC:
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("targetObj"));
                                EditorGUILayout.EndHorizontal();
                                break;
                            }
                        case (int)LE_Action_AddBuffInfo.Type.CustomBlackboardStr:
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("buffBlackboardStr"));
                                EditorGUILayout.EndHorizontal();
                                break;
                            }
                       
                    }


                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("actionType"));
                    EditorGUILayout.EndHorizontal();
                   // EditorGUI.indentLevel += 1;
                    if (NGUIEditorTools.DrawHeader("BuffList"+i))
                     {
                         SerializedProperty size1 = list.GetArrayElementAtIndex(i).FindPropertyRelative("buffList").FindPropertyRelative("Array.size");
                         EditorGUILayout.PropertyField(size1);

                         NGUIEditorTools.BeginContents(true);
                         for (int j = 0; j < list.GetArrayElementAtIndex(i).FindPropertyRelative("buffList").arraySize; j++)
                         {
                             EditorGUILayout.BeginHorizontal();
                             EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("buffList").GetArrayElementAtIndex(j));
                             EditorGUILayout.EndHorizontal();
                         }
                         NGUIEditorTools.EndContents();
                     }

                    //EditorGUI.indentLevel -= 1;
                }

            }
            EditorGUI.indentLevel -= 1;
            NGUIEditorTools.EndContents();
    
        }
       
    }
}




