
using System.Collections.Generic;
using System.IO;
using System;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 同步地形数据
/// </summary>
public class SynchronousTerrain
{
    static GameObject m_terrainObj = null;
    static string PrefabPath = "Scene/";

    [@MenuItem("SceneEditor/Synchronous Terrain Data")]
    static void LevelRouteDataRead()
    {
        GameObject[] selection = Selection.gameObjects;;//Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab | SelectionMode.Deep);

        if (selection.Length <= 0)
        {
            return;
        }

        string name             = "";
        string terrainName      = "";
        terrainName = selection[0].name;
        terrainName = terrainName.Replace("lep", "");
        terrainName = terrainName.Remove(terrainName.LastIndexOf("-"));
        name        = terrainName;
        terrainName = terrainName.Replace("s-d-", "");
        terrainName = terrainName.Remove(terrainName.IndexOf("-"));
       

        string prefabFile   = PrefabPath + terrainName + "/Room/" + name;
        m_terrainObj        = GameData.LoadPrefab<GameObject>(prefabFile);

        if (null == m_terrainObj)
        {
            Debug.LogError("null == m_terrainObj file "+ prefabFile+" not exsit !!");
            return;
        }

        Debug.Log("name:" + name + ",terrainName:" + terrainName + ",prefabFile:" + prefabFile);
        for (int i = 0; i < selection.Length; i++)
        {
            GameObject obj = selection[i];

            // 如果包含 leps则表明是 房间
            if ( obj.name.IndexOf("leps") >= 0 )
            {
                Debug.Log("obj.name:" + obj.name);
                SetData(obj);
            }

        }

    }


    static void SetData(GameObject obj)
    {
        GameObject copy = PrefabUtility.InstantiatePrefab(obj) as GameObject;

        SetAllAttr("activeArea",        copy);
        SetAllAttr("Collider",          copy);
        SetAllAttr("LocatorRT",         copy);
        SetAllAttr("RemovableGate_E",   copy);
        SetAllAttr("RemovableGate_N",   copy);
        SetAllAttr("RemovableGate_S",   copy);
        SetAllAttr("RemovableGate_W",   copy);
        SetAllAttr("activeArea",        copy);

        // 复制地形文件
        CopyTerrainData(copy);

        SetPostionAttr("GateLocate_E",copy);
        SetPostionAttr("GateLocate_N", copy);
        SetPostionAttr("GateLocate_S", copy);
        SetPostionAttr("GateLocate_W", copy);

        // 设置位置属性
        SetLocatorAttr(copy);

        PrefabUtility.ReplacePrefab(copy, PrefabUtility.GetPrefabParent(copy));

        GameObject.DestroyImmediate(copy,true);


    }

    static void SetAllAttr(string strName, GameObject desObj)
    {
        Transform src               = m_terrainObj.transform.Find(strName);
        Transform des               = desObj.transform.Find(strName);

        if ( src == null )
        {
            return;
        }

        if ( des != null )
        {
            GameObject.DestroyImmediate(des.gameObject,true);
        }

        GameObject copy                 = GameObject.Instantiate(src.gameObject) as GameObject;
        copy.transform.parent           = desObj.transform;

        copy.transform.localPosition    = src.localPosition;
        copy.transform.localRotation    = src.localRotation;
        copy.transform.localScale       = src.localScale;

        copy.name = copy.name.Replace("(Clone)", "");

        //Debug.Log("src.transform.localPosition:" + src.localPosition + ",src.transform:" + src.position + ",src.transform.localScale:" + src.localScale);
    }

    static void CopyTerrainData(GameObject copy)
    {


        SetAllAttr(m_terrainObj.name, copy);

    }

    // 设置位置属性
    static void SetPostionAttr( string name ,GameObject desObj )
    {
        Transform src = m_terrainObj.transform.Find(name);
        Transform des = desObj.transform.Find(name);

        if (src == null)
        {
            return;
        }

        des.localPosition = src.localPosition;
    }


    // 设置 LOCATOR 属性
    static void SetLocatorAttr( GameObject desObj )
    {
        Transform src = m_terrainObj.transform.Find("Locator");
        Transform des = desObj.transform.Find("Locator");

        if (src == null)
        {
            return;
        }

        Dictionary<GameObject, Vector3> list = new Dictionary<GameObject, Vector3>();

        Component[] children = des.GetComponentsInChildren(typeof(RMEAdjustPosInScene), true);

        // 记录原来的世界坐标位置
        foreach (Component child in children)
        {
            list.Add(child.gameObject, child.transform.position);

            Debug.Log("1child.gameObject:" + child.gameObject.name + ",child.transform.position:" + child.transform.position);
        }

        des.localPosition = src.localPosition;

        foreach (KeyValuePair<GameObject,Vector3> item in list)
        {
            item.Key.transform.position = item.Value;

            Debug.Log("2child.gameObject:" + item.Key.gameObject.name + ",child.transform.position:" + item.Value);
        }
    }
}