using System;
using System.Collections.Generic;

public class YellowPointInfo : IDataBase
{
    public int m_id { get; protected set; }
    public float m_hpParam { get; protected set; }
    public float m_phyAttParam { get; protected set; }
    public float m_phyDefParam { get; protected set; }
    public float m_magAttParam { get; protected set; }
    public float m_magDefParam { get; protected set; }
}

public class YellowPointParamTable
{
    public Dictionary<int, YellowPointInfo> m_map { get; protected set; }
    public YellowPointInfo LookUp(int id)
    {
        YellowPointInfo info = null;
        if (m_map.TryGetValue(id, out info))
        {
            return info;
        }
        return null;
    }
    public void Load(byte[] bytes)
    {
        m_map = new Dictionary<int, YellowPointInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount = helper.ReadInt();
        for (int index = 0; index < sceneCount; ++index)
        {
            YellowPointInfo info = new YellowPointInfo();
            info.Load(helper);
            m_map.Add(info.m_id, info);
        }
    }
}