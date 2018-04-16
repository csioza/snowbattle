using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Collections;


public class ConditionInfo
{
    public int[] ParamList { get; set; }
    public ConditionInfo()
    {
        ParamList = new int[2];
    }
}

public class IllumeInfo 
{
    public int ID { get; protected set; }
    public int DungeonID { get; protected set; }
    public int DungeonGroupID { get; protected set; }
    public ConditionInfo[] ConditionList { get; protected set; }
    public virtual void Load(BinaryHelper helper)
    {
        ID = helper.ReadInt();
        DungeonID = helper.ReadInt();
        DungeonGroupID = helper.ReadInt();
        int resultCount = helper.ReadInt();
        ConditionList = new ConditionInfo[resultCount];
        for (int index = 0; index < resultCount; ++index)
        {
            ConditionList[index] = new ConditionInfo();
            for (int innerIndex = 0; innerIndex < ConditionList[index].ParamList.Length; ++innerIndex)
            {
                ConditionList[index].ParamList[innerIndex] = helper.ReadInt();
            }
        }
    }
#if UNITY_EDITOR
    public virtual void Save(BinaryHelper helper)
    {
        helper.Write(ID);
        helper.Write(DungeonID);
        helper.Write(DungeonGroupID);
        helper.Write(ConditionList.Length);
        foreach (var info in ConditionList)
        {
            foreach (var innerInfo in info.ParamList)
            {
                helper.Write(innerInfo);
            }
        }
    }
#endif
}

public class IllumeTable
{
    public IllumeInfo Lookup(int id)
    {
        IllumeInfo info;
        IllumeInfoList.TryGetValue(id, out info);
        return info;
    }
    public Dictionary<int, IllumeInfo> IllumeInfoList { get; protected set; }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        IllumeInfoList = new Dictionary<int, IllumeInfo>(length);
        for (int index = 0; index < length; ++index)
        {
            IllumeInfo info = new IllumeInfo();
            info.Load(helper);
            IllumeInfoList.Add(info.ID, info);
        }
    }
}