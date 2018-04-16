using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
[CanEditMultipleObjects]
[CustomEditor(typeof(RMEArrayEdit))]
public class RMEArraryEditor : Editor
{

    public override void OnInspectorGUI()
    {
        RMEArrayEdit cam = target as RMEArrayEdit;
        DrawDefaultInspector();

        if (GUILayout.Button("FillInTheFormation"))
        {
            cam.FillTheFormation();
        }
    }
}




