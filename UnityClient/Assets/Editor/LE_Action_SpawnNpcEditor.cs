using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(LE_Action_SpawnNpc)), CanEditMultipleObjects]

public class LE_Action_SpawnNpcEditor : Editor
{


    public override void OnInspectorGUI()
    {
        LE_Action_SpawnNpc cam = target as LE_Action_SpawnNpc;

        GUILayout.Label(new GUIContent("The Serialize No.:" + cam.SerializeNo));

        serializedObject.Update();

        DrawDefaultInspector();

        ShowList(serializedObject.FindProperty("spawnNpcList"));
            

        serializedObject.ApplyModifiedProperties();
    }

    static void ShowList(SerializedProperty list)
    {
        if (!list.isArray)
        {
            return;
        }
        EditorGUI.indentLevel += 1;
        if (NGUIEditorTools.DrawMinimalisticHeader("Spawn Npc List"))
        {
            //NGUIEditorTools.BeginContents();
            EditorGUI.indentLevel += 1;
            SerializedProperty size = list.FindPropertyRelative("Array.size");
            EditorGUILayout.PropertyField(size);

            for (int i = 0; i < list.arraySize; i++)
            {

                if (NGUIEditorTools.DrawMinimalisticHeader("Element" + i))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("targetNpc"));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("spawnPosition"));
                    EditorGUILayout.EndHorizontal();

                    switch (list.GetArrayElementAtIndex(i).FindPropertyRelative("spawnPosition").intValue)
                    {
                        case (int)RefreshMonsterResult.ENSwapPosType.enLocationPosition:
                            {
                                break;
                            }
                        case (int)RefreshMonsterResult.ENSwapPosType.PlayerPositionOffset:
                        case (int)RefreshMonsterResult.ENSwapPosType.CustomBlackboardStr:
                        case (int)RefreshMonsterResult.ENSwapPosType.AffectedGate:
                        case (int)RefreshMonsterResult.ENSwapPosType.TriggeringGate:
                        case (int)RefreshMonsterResult.ENSwapPosType.TriggeringTrap:
                        case (int)RefreshMonsterResult.ENSwapPosType.TargetPositionOfSkillBeingCast:
                        case (int)RefreshMonsterResult.ENSwapPosType.TargetActorOfTriggeringActor:
                        case (int)RefreshMonsterResult.ENSwapPosType.DeadActor:
                        case (int)RefreshMonsterResult.ENSwapPosType.CastingActor:
                        case (int)RefreshMonsterResult.ENSwapPosType.LastCreatedActor:
                        case (int)RefreshMonsterResult.ENSwapPosType.TargetActorOfSkill:
                        case (int)RefreshMonsterResult.ENSwapPosType.RebornActor:
                        case (int)RefreshMonsterResult.ENSwapPosType.AttackedActor:
                        case (int)RefreshMonsterResult.ENSwapPosType.TransportingActor:
                        case (int)RefreshMonsterResult.ENSwapPosType.TriggerEventTrap:
                        case (int)RefreshMonsterResult.ENSwapPosType.triggerPositionOffset:
                            {
                                EditorGUILayout.BeginHorizontal();
                                Vector2 soft = EditorGUILayout.Vector2Field("Off Set", list.GetArrayElementAtIndex(i).FindPropertyRelative("offset").vector2Value, GUILayout.MinWidth(30f));
                                EditorGUILayout.EndHorizontal();

                                list.GetArrayElementAtIndex(i).FindPropertyRelative("offset").vector2Value = soft;
                                break;
                            }
                        case (int)RefreshMonsterResult.ENSwapPosType.ObjectPositionOffset:
                            {
                                EditorGUILayout.BeginHorizontal();
                                Vector2 soft = EditorGUILayout.Vector2Field("Off Set", list.GetArrayElementAtIndex(i).FindPropertyRelative("offset").vector2Value, GUILayout.MinWidth(30f));
                                EditorGUILayout.EndHorizontal();

                                list.GetArrayElementAtIndex(i).FindPropertyRelative("offset").vector2Value = soft;
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("targetObject"));
                                EditorGUILayout.EndHorizontal();
                                break;
                            }
                    }


                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("blackboardStr"));
                    EditorGUILayout.EndHorizontal();

                    
                }

            }
            EditorGUI.indentLevel -= 1;
            EditorGUI.indentLevel -= 1;
    
        }
       
    }
}




