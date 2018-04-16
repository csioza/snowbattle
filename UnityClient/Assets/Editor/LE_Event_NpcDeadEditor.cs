using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(LE_Event_NpcDead)), CanEditMultipleObjects]

public class LE_Event_NpcDeadEditor : Editor
{


    public override void OnInspectorGUI()
    {
        LE_Event_NpcDead cam = target as LE_Event_NpcDead;

        GUILayout.Label(new GUIContent("The Serialize No.:" + cam.SerializeNo));

        

        serializedObject.Update();

        DrawDefaultInspector();

        ShowList(serializedObject.FindProperty("npcList"));

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
       // EditorGUI.indentLevel += 1;

        if (NGUIEditorTools.DrawMinimalisticHeader("Npc List"))
        {
            NGUIEditorTools.BeginContents(true);

            SerializedProperty size = list.FindPropertyRelative("Array.size");
            EditorGUILayout.PropertyField(size);

            EditorGUI.indentLevel += 1;
            for (int i = 0; i < list.arraySize; i++)
            {
               
                if (NGUIEditorTools.DrawMinimalisticHeader("Npc" + i))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("npcType"));
                    EditorGUILayout.EndHorizontal();



                    if (list.GetArrayElementAtIndex(i).FindPropertyRelative("npcType").intValue == (int)DeadEvent.ENDeadActorType.enSpicalNpc)
                    {
                         EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("targetNpc"));
                    }
                    else if (list.GetArrayElementAtIndex(i).FindPropertyRelative("npcType").intValue == (int)DeadEvent.ENDeadActorType.enOther)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("blackboardActorName"));
                        EditorGUILayout.EndHorizontal();
                    }

                    
                }
               
            }

            EditorGUI.indentLevel -= 1;
           
            NGUIEditorTools.EndContents();

        }

    }

}




