using System;
using System.Collections.Generic;

public class FloorRankInfo : IDataBase
{
    public int m_rankID { get; protected set; }
    public string m_rank { get; protected set; }
    public float m_coinParam { get; protected set; }
    public float m_expParam { get; protected set; }
    public float m_skillExpParam { get; protected set; }
}

public class FloorRankTable
{
    public Dictionary<string, FloorRankInfo> m_map;
    public FloorRankInfo LookUp(string rank)
    {
        FloorRankInfo info = null;
        if (m_map.TryGetValue(rank, out info))
        {
            return info;
        }

        return null;
    }

    public void Load(byte[] bytes)
    {
        m_map = new Dictionary<string, FloorRankInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount      = helper.ReadInt();

        for (int index = 0; index < sceneCount; ++index)
        {
            FloorRankInfo info = new FloorRankInfo();
            
            info.Load(helper);

            m_map.Add(info.m_rank, info);
        }
    }
}