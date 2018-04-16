using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneGateInfo : IDataBase
{
    public int ID { get; private set; }
    public string prefabName { get; private set; }
}
public class SceneGateTable
{
    public SceneGateTable()
    {

    }
    public SortedList<int, SceneGateInfo> GateInfoList { get; protected set; }
    public SceneGateInfo Lookup(int id)
    {
        SceneGateInfo info;
        GateInfoList.TryGetValue(id, out info);
        return info;
    }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        GateInfoList = new SortedList<int, SceneGateInfo>(length);
        for (int index = 0; index < length; index++ )
        {
            SceneGateInfo info = new SceneGateInfo();
            info.Load(helper);
            GateInfoList.Add(info.ID, info);
        }

    }
}
