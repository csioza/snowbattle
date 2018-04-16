using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEventInfo : IDataBase
{
    public int EventID { get; protected set; }
    public int EventType { get; protected set; }
    public int TargetID { get; protected set; }
    public int TargetCount { get; protected set; }
}

public class DungeonEventTable
{
    public Dictionary<int, DungeonEventInfo> m_dungeonEventMap;
    public DungeonEventInfo LookUp(int eventID)
    {
        DungeonEventInfo info = null;
        if (m_dungeonEventMap.TryGetValue(eventID, out info))
        {
            return info;
        }
        return null;
    }
    public void Load(byte[] bytes)
    {
        m_dungeonEventMap = new Dictionary<int, DungeonEventInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int count = helper.ReadInt();
        for (int index = 0; index < count; ++index)
        {
            DungeonEventInfo info = new DungeonEventInfo();
            info.Load(helper);
            m_dungeonEventMap.Add(info.EventID, info);
        }
    }
}
