using System;
using System.Collections.Generic;

public class RaceInfo : IDataBase
{
    public int m_id { get; protected set; }
    public string m_name { get; protected set; }
    public string m_describe { get; protected set; }
    public int m_iconId { get; protected set; }
}

public class RaceInfoTable
{
    public Dictionary<int, RaceInfo> m_map { get; protected set; }
    public RaceInfo LookUp(int id)
    {
        RaceInfo info = null;
        if (m_map.TryGetValue(id, out info))
        {
            return info;
        }

        return null;
    }
    public void Load(byte[] bytes)
    {
        m_map = new Dictionary<int, RaceInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount = helper.ReadInt();
        for (int index = 0; index < sceneCount; ++index)
        {
            RaceInfo info = new RaceInfo();
            
            info.Load(helper);

            m_map.Add(info.m_id, info);
        }
    }
}