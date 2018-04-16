using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(RMEBoxEdit)), CanEditMultipleObjects]

public class RMEBoxEditor : Editor
{


    public override void OnInspectorGUI()
    {
        RMEBoxEdit cam = target as RMEBoxEdit;

        GUILayout.Label(new GUIContent("The Serialize No.:" + cam.SerializeNo));

        serializedObject.Update();

        DrawDefaultInspector();


        

        serializedObject.ApplyModifiedProperties();
    }

  
}




