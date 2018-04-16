using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 任务表
public class MissionInfo : IDataBase
{
    public int MissionID { get; protected set; }
    public int MissionType { get; protected set; }
    public string MissionName { get; protected set; }
    public int NpcID { get; protected set; }
    public int FinishNpcID { get; protected set; }
    public string MissionDepict { get; protected set; }
    public string GetTalk { get; protected set; }
    public string FinishTalk { get; protected set; }
    public int ParentID { get; protected set; }
    public int Prof { get; protected set; }
    public int Level { get; protected set; }
    public int MaxLevel { get; protected set; }
    public int Exp { get; protected set; }
    public int Money { get; protected set; }
    public int RMBMoney { get; protected set; }
    public int Award1 { get; protected set; }
    public int Award2 { get; protected set; }
    public int Award3 { get; protected set; }
    public int Award4 { get; protected set; }
    public int Team { get; protected set; }
    public short condition1 { get; protected set; }
    public short condition2 { get; protected set; }
    public short condition3 { get; protected set; }
//     public int condition4 { get; protected set; }
    
}

public class MissionTable
{
    public Dictionary<int,MissionInfo> MissionInfoList { get; protected set; }
    public MissionInfo Lookup(int id)
    {
        MissionInfo info = null;
        if (MissionInfoList.TryGetValue(id, out info))
        {
            return info;
        }
        return null;
    }
    public List<MissionInfo> LookupIDByNpcID(int id)
    {
        List<MissionInfo> list = new List<MissionInfo>();
        foreach (KeyValuePair<int, MissionInfo> pair in MissionInfoList)
        {
            if (pair.Value.NpcID == id)
            {
                list.Add(pair.Value);
            }
        }
        return list;
    }
    public List<MissionInfo> LookupFinishIDByNpcID(int id)
    {
        List<MissionInfo> list = new List<MissionInfo>();
        foreach (KeyValuePair<int, MissionInfo> pair in MissionInfoList)
        {
            if (pair.Value.FinishNpcID == id)
            {
                list.Add(pair.Value);
            }
        }
        return list;
    }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        MissionInfoList = new Dictionary<int,MissionInfo>();
        for (int index = 0; index < length; ++index)
        {
            MissionInfo info = new MissionInfo();
            info.Load(helper);
            MissionInfoList.Add(info.MissionID,info);
        }
    }
}