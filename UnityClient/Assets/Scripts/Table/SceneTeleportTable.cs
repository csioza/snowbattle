using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneTeleportInfo : IDataBase
{
    public int ID { get; private set; }
    public string prefabName { get; private set; }
}
public class SceneTeleportTable
{
    public SceneTeleportTable()
    {

    }
    public SortedList<int, SceneTeleportInfo> TeleportInfoList { get; protected set; }
    public SceneTeleportInfo Lookup(int id)
    {
        SceneTeleportInfo info;
        TeleportInfoList.TryGetValue(id, out info);
        return info;
    }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        TeleportInfoList = new SortedList<int, SceneTeleportInfo>(length);
        for (int index = 0; index < length; index++ )
        {
            SceneTeleportInfo info = new SceneTeleportInfo();
            info.Load(helper);
            TeleportInfoList.Add(info.ID, info);
        }

    }
}
