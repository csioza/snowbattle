using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

public class TeleportInfo
{
    public float[] ParamList { get; set; }
    public TeleportInfo()
    {
        ParamList = new float[2];
    }
}

public class SceneMapNumericInfo
{
    public int ID { get; protected set; }
    public float SceneMapX { get; protected set; }
    public float SceneMapY { get; protected set; }
    public float IconMapX { get; protected set; }
    public float IconMapY { get; protected set; }
    public float DeviationX { get; protected set; }
    public float DeviationY { get; protected set; }
    public string IconPath { get; protected set; }
    public TeleportInfo[] TeleporterList { get; protected set; }

    public virtual void Load(BinaryHelper helper)
    {
        ID = helper.ReadInt();
        SceneMapX = helper.ReadFloat();
        SceneMapY = helper.ReadFloat();
        IconMapX = helper.ReadFloat();
        IconMapY = helper.ReadFloat();
        DeviationX = helper.ReadFloat();
        DeviationY = helper.ReadFloat();
        IconPath = helper.ReadString();
        int resultCount = helper.ReadInt();
        TeleporterList = new TeleportInfo[resultCount];
        for (int index = 0; index < resultCount; ++index )
        {
            TeleporterList[index] = new TeleportInfo();
            for (int innerIndex = 0; innerIndex < TeleporterList[index].ParamList.Length; ++innerIndex)
            {
                TeleporterList[index].ParamList[innerIndex] = helper.ReadFloat();
            }
        }
    }

#if UNITY_EDITOR
    public virtual void Save(BinaryHelper helper)
    {
        helper.Write(ID);
        helper.Write(SceneMapX);
        helper.Write(SceneMapY);
        helper.Write(IconMapX);
        helper.Write(IconMapY);
        helper.Write(DeviationX);
        helper.Write(DeviationY);
        helper.Write(IconPath);
        helper.Write(TeleporterList.Length);
        foreach (var info in TeleporterList)
        {
            foreach (var innerInfo in info.ParamList)
            {
                helper.Write(innerInfo);
            }
        }
    }
#endif
}

public class SceneMapNumericTable
{
    public SceneMapNumericInfo Lookup(int id)
    {
        SceneMapNumericInfo info;
        SceneMapNumericInfoList.TryGetValue(id, out info);
        return info;
    }
    public Dictionary<int, SceneMapNumericInfo> SceneMapNumericInfoList { get; protected set; }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        SceneMapNumericInfoList = new Dictionary<int, SceneMapNumericInfo>(length);
        for (int index = 0; index < length; ++index)
        {
            SceneMapNumericInfo info = new SceneMapNumericInfo();
            info.Load(helper);
            SceneMapNumericInfoList.Add(info.ID, info);
        }
    }
}