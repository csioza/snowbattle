using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;


public enum ENShakeParamIndex
{
    enNone = 0,
    
    Count,
}

public class ShakeParamInfo : IDataBase
{
    public int ID { get; protected set; }
    //Camera 1
    public float CameraParam1 { get; protected set; }
    public float CameraParam2 { get; protected set; }
    public float CameraParam3 { get; protected set; }
    //Õð¶¯´ÎÊý
    public int ShakeNum { get; protected set; }
}

public class ShakeTable
{
    public ShakeParamInfo Lookup(int id)
    {
        ShakeParamInfo info;
        ShakeParamInfoList.TryGetValue(id, out info);
        return info;
    }
    public SortedList<int, ShakeParamInfo> ShakeParamInfoList { get; protected set; }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        ShakeParamInfoList = new SortedList<int, ShakeParamInfo>(length);
        for (int index = 0; index < length; ++index)
        {
            ShakeParamInfo info = new ShakeParamInfo();
            info.Load(helper);
            ShakeParamInfoList.Add(info.ID, info);
        }
    }
}