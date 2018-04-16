using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
[CanEditMultipleObjects]
[CustomEditor(typeof(RMELevelRouteHead))]
public class RMELevelRouteHeadEditor : Editor
{

    public override void OnInspectorGUI()
    {
        RMELevelRouteHead cam = target as RMELevelRouteHead;
        DrawDefaultInspector();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("viewId"));
        if (GUILayout.Button("ViewScene"))
        {
            LevelDataOut.BuildLevelRouteData(cam.head, cam);
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
        if (GUILayout.Button("SetMonsters"))
        {
            LevelDataOut.SetMonsters(cam.gameObject);
        }
    }
}




