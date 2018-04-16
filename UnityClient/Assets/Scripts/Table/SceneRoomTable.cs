using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneRoomInfo : IDataBase
{
    public int ID { get; private set; }
    public string prefabName { get; private set; }
}
public class SceneRoomTable
{
    public SceneRoomTable()
    {

    }
    public SortedList<int, SceneRoomInfo> RoomInfoList { get; protected set; }
    public SceneRoomInfo Lookup(int id)
    {
        SceneRoomInfo info;
        RoomInfoList.TryGetValue(id, out info);
        return info;
    }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        RoomInfoList = new SortedList<int, SceneRoomInfo>(length);
        for (int index = 0; index < length; index++ )
        {
            SceneRoomInfo info = new SceneRoomInfo();
            info.Load(helper);
            RoomInfoList.Add(info.ID, info);
        }

    }
}
