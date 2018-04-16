using System;
using System.Collections.Generic;

public class CardLevelInfo : IDataBase
{
    public int m_id { get; protected set; }
    public float m_hpParam { get; protected set; }
    public float m_phyAttParam { get; protected set; }
    public float m_phyDefParam { get; protected set; }
    public float m_magAttParam { get; protected set; }
    public float m_magDefParam { get; protected set; }
}

public class CardLevelVariationTable
{
    public Dictionary<int, CardLevelInfo> m_map { get; protected set; }
    public CardLevelInfo LookUp(int id)
    {
        CardLevelInfo info = null;
        if (m_map.TryGetValue(id, out info))
        {
            return info;
        }
        return null;
    }
    public void Load(byte[] bytes)
    {
        m_map = new Dictionary<int, CardLevelInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount = helper.ReadInt();
        for (int index = 0; index < sceneCount; ++index)
        {
            CardLevelInfo info = new CardLevelInfo();
            info.Load(helper);
            m_map.Add(info.m_id, info);
        }
    }
}