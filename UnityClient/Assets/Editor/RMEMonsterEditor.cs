using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(RMEMonsterEdit)), CanEditMultipleObjects]

public class RMEMonsterEditor : Editor
{


    public override void OnInspectorGUI()
    {
        RMEMonsterEdit cam = target as RMEMonsterEdit;

        GUILayout.Label(new GUIContent("The Serialize No.:" + cam.SerializeNo));

        serializedObject.Update();

        DrawDefaultInspector();


        

        serializedObject.ApplyModifiedProperties();
    }

  
}




