using System;
using System.Collections.Generic;

public class CardTypeInfo : IDataBase
{
    public int m_id { get; protected set; }
    public int m_hp { get; protected set; }
    public int m_phyAtt { get; protected set; }
    public int m_phyDef { get; protected set; }
    public int m_magAtt { get; protected set; }
    public int m_magDef { get; protected set; }
    public float m_critRate { get; protected set; }
    public float m_critParam { get; protected set; }
    
}

public class CardTypeVariationTable
{
    public Dictionary<int, CardTypeInfo> m_map { get; protected set; }
    public CardTypeInfo LookUp(int quality)
    {
        CardTypeInfo info = null;
        if (m_map.TryGetValue(quality, out info))
        {
            return info;
        }
        return null;
    }
    public void Load(byte[] bytes)
    {
        m_map = new Dictionary<int, CardTypeInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount = helper.ReadInt();
        for (int index = 0; index < sceneCount; ++index)
        {
            CardTypeInfo info = new CardTypeInfo();
            info.Load(helper);
            m_map.Add(info.m_id, info);
        }
    }
}