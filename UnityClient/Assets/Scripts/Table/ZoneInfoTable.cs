using System;
using System.Collections.Generic;

public class ZoneTableInfo : IDataBase
{
    public int m_id { get; protected set; }
    public string m_name { get; protected set; }
    public int m_iconId { get; protected set; }
    public string m_tips { get; protected set; }
    public List<int> m_stageList { get; protected set; }
}

public class ZoneInfoTable
{
    public Dictionary<int, ZoneTableInfo> m_map { get; protected set; }
    public ZoneTableInfo LookUp(int id)
    {
        ZoneTableInfo info = null;
        if (m_map.TryGetValue(id, out info))
        {
            return info;
        }

        return null;
    }

    public void Load(byte[] bytes)
    {
        m_map               = new Dictionary<int, ZoneTableInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount      = helper.ReadInt();

        for (int index = 0; index < sceneCount; ++index)
        {
            ZoneTableInfo info   = new ZoneTableInfo();
            
            info.Load(helper);

            m_map.Add(info.m_id, info);
        }
    }
}