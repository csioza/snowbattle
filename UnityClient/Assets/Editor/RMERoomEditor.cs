using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
[CanEditMultipleObjects]
[CustomEditor(typeof(RMERoomEdit))]
public class RMERoomEditor : Editor
{

    static public RoomTagTable asset { get; private set; }
    static public Dictionary<int, List<RoomTagInfo>> RoomTagInfoList { get; private set; }

    static public List<int> IdList { get; private set; }


    public override void OnInspectorGUI()
    {
        RMERoomEdit cam = target as RMERoomEdit;
        DrawDefaultInspector();

        if (null == asset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("RoomTagTable");
            asset = new RoomTagTable();
            asset.Load(obj.bytes);
        }

        if (null == RoomTagInfoList)
        {
            RoomTagInfoList = new Dictionary<int, List<RoomTagInfo>>();
            int index   = 1;
            int key     = 0;
            foreach (KeyValuePair<int, RoomTagInfo> item in asset.list)
            {
                if (RoomTagInfoList.ContainsKey(key))
                {
                    RoomTagInfoList[key].Add(item.Value);
                  //  Debug.Log("index:" + index + ",key:" + key + "gengxin");
                }
                else
                {
                    List<RoomTagInfo> list = new List<RoomTagInfo>();
                    list.Add(item.Value);
                    RoomTagInfoList.Add(key, list);
                  //  Debug.Log("index:" + index + ",key:" + key + "xinjian");
                }
               // Debug.Log("index:" + index + ",key:" + key + "");
                if ( (index%2) == 0 )
                {
                    key++;
                }
                index++;
            }
 
        }

        if (NGUIEditorTools.DrawHeader("Tag"))
        {
            NGUIEditorTools.BeginContents();
            {

                foreach (KeyValuePair<int, List<RoomTagInfo>> item in RoomTagInfoList)
                {
                    GUILayout.BeginHorizontal();
                    foreach (RoomTagInfo info in item.Value)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(info.name), new GUIContent(info.name), GUILayout.MinWidth(20f));

                    }
                    GUILayout.EndHorizontal();
                }

            }
            NGUIEditorTools.EndContents();
        }

        serializedObject.ApplyModifiedProperties();


        if ( null == IdList )
        {
            IdList = new List<int>();    
        }

        IdList.Clear();

        foreach (KeyValuePair<int, RoomTagInfo> item in asset.list)
        {
            if (serializedObject.FindProperty(item.Value.name).boolValue)
            {
                IdList.Add(item.Key); ;
                //Debug.Log("item.Value.name:" + item.Value.name);
            }
        }
        cam.SaveData(IdList);

        //EditorGUILayout.
        if (GUILayout.Button("Update"))
        {
            asset.list.Clear();

            TextAsset obj = GameData.LoadConfig<TextAsset>("RoomTagTable");

            asset.Load(obj.bytes);
        
            int index = 1;
            int key = 0;
            RoomTagInfoList.Clear();

            foreach (KeyValuePair<int, RoomTagInfo> item in asset.list)
            {
                if (RoomTagInfoList.ContainsKey(key))
                {
                    RoomTagInfoList[key].Add(item.Value);
                    //  Debug.Log("index:" + index + ",key:" + key + "gengxin");
                }
                else
                {
                    List<RoomTagInfo> list = new List<RoomTagInfo>();
                    list.Add(item.Value);
                    RoomTagInfoList.Add(key, list);
                    //  Debug.Log("index:" + index + ",key:" + key + "xinjian");
                }
                // Debug.Log("index:" + index + ",key:" + key + "");
                if ((index % 2) == 0)
                {
                    key++;
                }
                index++;
            } 
        }
    }
}




