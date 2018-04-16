using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
// 宝箱编辑
public class RMERoomEdit : RMEBaseInfoEdit
{
    [HideInInspector]
    [SerializeField]
    public bool NeedKey = false;
    [HideInInspector]
    [SerializeField]
    public bool DropKey = false;
    [HideInInspector]
    [SerializeField]
    public bool BOSS = false;
    [HideInInspector]
    [SerializeField]
    public bool Begin = false;
    [HideInInspector]
    [SerializeField]
    public bool End = false;
    [HideInInspector]
    [SerializeField]
    public bool TriggerOnly = false;
    [HideInInspector]
    [SerializeField]
    public bool SSS = false;

    [HideInInspector]
    [SerializeField]
    public bool E = false;
    [HideInInspector]
    [SerializeField]
    public bool N = false;
    [HideInInspector]
    [SerializeField]
    public List<int> idList = new List<int>();

    public int terrainId   = 0;  //  地形ID


    public RMERoomEdit()
        : base(Type.enRoom)
    {

    }
    public void SaveData(List<int> list)
    {
        idList.Clear();

        foreach (int id in list)
        {
            idList.Add(id);
           // Debug.Log("id:" + id);
        }
    }

    
}




