using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ComponentShare : MonoBehaviour
{
    public List<UnityEngine.Object> ShareObjectList = new List<UnityEngine.Object>();
}

public class CreatePrefab
{
    #region Singleton
    static CreatePrefab m_singleton;
    static public CreatePrefab Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new CreatePrefab();
            }
            return m_singleton;
        }
    }
    #endregion

    public void Create(string filepath, List<UnityEngine.Object> objList)
    {
        if (System.IO.File.Exists(filepath))
        {
            System.IO.File.Delete(filepath);
        }
        GameObject emptyObj = new GameObject();
        GameObject prefab = PrefabUtility.CreatePrefab(filepath, emptyObj);
        prefab = GameObject.Instantiate(prefab) as GameObject;
        ComponentShare callback = prefab.AddComponent<ComponentShare>();
        callback.ShareObjectList.AddRange(objList);

        GameObject.DestroyImmediate(emptyObj);
        GameObject.DestroyImmediate(prefab);
    }
}