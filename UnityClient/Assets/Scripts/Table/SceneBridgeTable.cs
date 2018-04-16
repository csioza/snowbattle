using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneBridgeInfo : IDataBase
{
    public int ID { get; private set; }
    public string bridgeHeadPrefab { get; private set; }
    public string bridgeBodyPrefab { get; private set; }
    public string bridgeTailPrefab { get; private set; }
}
public class SceneBridgeTable
{
    public SceneBridgeTable()
    {

    }
    public SortedList<int, SceneBridgeInfo> BridgeInfoList { get; protected set; }
    public SceneBridgeInfo Lookup(int id)
    {
        SceneBridgeInfo info;
        BridgeInfoList.TryGetValue(id, out info);
        return info;
    }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        BridgeInfoList = new SortedList<int, SceneBridgeInfo>(length);
        for (int index = 0; index < length; index++ )
        {
            SceneBridgeInfo info = new SceneBridgeInfo();
            info.Load(helper);
            BridgeInfoList.Add(info.ID, info);
        }

    }
}
