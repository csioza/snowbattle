using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(RMESchemeEdit)), CanEditMultipleObjects]

public class RMEShemeEditor : Editor
{


    public override void OnInspectorGUI()
    {
        RMESchemeEdit cam = target as RMESchemeEdit;

        GUILayout.Label(new GUIContent("The Serialize No.:" + cam.SerializeNo));

        serializedObject.Update();

        DrawDefaultInspector();


        

        serializedObject.ApplyModifiedProperties();
    }

  
}




