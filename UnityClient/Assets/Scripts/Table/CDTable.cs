using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

public class CDInfo : IDataBase
{
    public int ID { get; protected set; }
    public float CDTime { get; protected set; }
    public float CDTimeAddLevelUp { get; protected set; }
}

public class CDTable
{
    public CDInfo Lookup(int id)
    {
        CDInfo info;
        CDInfoList.TryGetValue(id, out info);
        return info;
    }
    public SortedList<int, CDInfo> CDInfoList { get; protected set; }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        CDInfoList = new SortedList<int, CDInfo>(length);
        for (int index = 0; index < length; ++index)
        {
            CDInfo info = new CDInfo();
            info.Load(helper);
            CDInfoList.Add(info.ID, info);
        }
    }
}