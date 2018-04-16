using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(LE_Action_Gate)), CanEditMultipleObjects]

public class LE_Action_GateEditor : Editor
{


    public override void OnInspectorGUI()
    {
        LE_Action_Gate cam = target as LE_Action_Gate;

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
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("gateType"));
                    EditorGUILayout.EndHorizontal();



                    switch (list.GetArrayElementAtIndex(i).FindPropertyRelative("gateType").intValue)
                    {
                        case (int)GateOpResult.ENGateType.specifiedGate:
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("targetGateObj"));
                                EditorGUILayout.EndHorizontal();
                                break;
                            }
                        case (int)GateOpResult.ENGateType.CustomBlackboardStr:
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("gateBlackboardStr"));
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




