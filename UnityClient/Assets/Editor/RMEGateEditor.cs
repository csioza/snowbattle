using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(RMEGateEdit)), CanEditMultipleObjects]

public class RMEGateEditor : Editor
{


    public override void OnInspectorGUI()
    {
        RMEGateEdit cam = target as RMEGateEdit;

        serializedObject.Update();

        DrawDefaultInspector();



        switch ( cam.passtype )
        {
            case SM.ENGateOpenCondType.enKillSpecialMonster:
                {
                    ShowList(serializedObject.FindProperty("monsterList"));
                    break;
                }
            case SM.ENGateOpenCondType.enOpenBox:
                {
                    ShowList(serializedObject.FindProperty("boxList"));
                    break;
                }
            case SM.ENGateOpenCondType.enOpenSheme:
                {
                    ShowList(serializedObject.FindProperty("shemeList"));
                    break;
                }
            default:
                break;
           
        }

       // GUILayout.Label(new GUIContent(cam.objId.ToString()));

        serializedObject.ApplyModifiedProperties();
    }

    static void ShowList(SerializedProperty list)
    {
        if (!list.isArray)
        {
            return;
        }
        EditorGUI.indentLevel += 1;
        EditorGUILayout.PropertyField(list);
        EditorGUI.indentLevel += 1;
        SerializedProperty size = list.FindPropertyRelative("Array.size");
        EditorGUILayout.PropertyField(size);

        for (int i = 0; i < list.arraySize; i++) {

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel -= 1;
        EditorGUI.indentLevel -= 1;
    }
}




