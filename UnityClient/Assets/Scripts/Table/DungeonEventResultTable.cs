using System;
using System.Collections.Generic;

public struct EventResultParam
{
    public int paramInt { get; set; }
    public string paramString { get; set; }
}

public class DungeonEventResultInfo
{
    public int ResultID { get; set; }
    public string ResultName { get; set; }
    public EventResultParam[] ResultParamList { get; set; }

    public DungeonEventResultInfo()
    {
        ResultParamList = new EventResultParam[4];
    }
}

public class DungeonEventResultTable
{
    public Dictionary<int, DungeonEventResultInfo> m_dungeonEventResultMap;
    public DungeonEventResultInfo LookUp(int resultID)
    {
        DungeonEventResultInfo info = null;
        if (m_dungeonEventResultMap.TryGetValue(resultID, out info))
        {
            return info;
        }
        return null;
    }
    public void Load(byte[] bytes)
    {
        m_dungeonEventResultMap = new Dictionary<int, DungeonEventResultInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int count = helper.ReadInt();
        for (int index = 0; index < count; ++index)
        {
            DungeonEventResultInfo info = new DungeonEventResultInfo();
            info.ResultID = helper.ReadInt();
            info.ResultName = helper.ReadString();
            for (int i = 0; i < 4; i++)
            {
                info.ResultParamList[i].paramInt = helper.ReadInt();
                info.ResultParamList[i].paramString = helper.ReadString();
            }
            m_dungeonEventResultMap.Add(info.ResultID, info);
        }
    }
}
