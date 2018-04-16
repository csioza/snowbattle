using System;
using System.Collections.Generic;
using UnityEngine;


public class RoomAttrInfo : IDataBase
{
    public int ID { get; private set; }
    public int PassageType { get; private set; }
    public string ArtRes { get; private set; }
    public List<string> DesignResList { get; private set; }
    public int BranchFlag { get; private set; }
}
public class RoomAttrTable
{
    public RoomAttrTable()
    {

    }
    public SortedList<int, RoomAttrInfo> RoomAttrInfoList { get; protected set; }
    public RoomAttrInfo Lookup(int id)
    {
        RoomAttrInfo info;
        RoomAttrInfoList.TryGetValue(id, out info);
        return info;
    }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        RoomAttrInfoList = new SortedList<int, RoomAttrInfo>(length);
        for (int index = 0; index < length; index++ )
        {
            RoomAttrInfo info = new RoomAttrInfo();
            info.Load(helper);
            RoomAttrInfoList.Add(info.ID, info);
        }
    }
}
