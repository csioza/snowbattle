using System;
using System.Collections.Generic;

public class AttrRatioInfo : IDataBase
{
    public int m_id { get; protected set; }
    public int m_occupation{ get; protected set; }
    public int m_rarity { get; protected set; }
    public float m_hpMutiply { get; protected set; }
    public float m_phyAttackMutiply { get; protected set; }
    public float m_magAttackMutiply { get; protected set; }
    public float m_phyDefendMutiply { get; protected set; }
    public float m_magDefendMutiply { get; protected set; }

    public int m_hPMaxAdd { get; protected set; }
    public int m_phyAttackAdd { get; protected set; }
    public int m_magAttackAdd { get; protected set; }
    public int m_phyDefendAdd { get; protected set; }
    public int m_magDefendAdd { get; protected set; }

   
}

public class AttrRatioTable
{
    public Dictionary<int, AttrRatioInfo> m_map { get; protected set; }
    public AttrRatioInfo LookUp(int occ,int rarity)
    {
        foreach (KeyValuePair<int, AttrRatioInfo> item in m_map)
        {
            if (item.Value.m_occupation == occ && item.Value.m_rarity == rarity)
            {
                AttrRatioInfo info = null;
                if (m_map.TryGetValue(item.Key, out info))
                {
                    return info;
                }
            }
        }

        return null;
    }
    public void Load(byte[] bytes)
    {
        m_map = new Dictionary<int, AttrRatioInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount = helper.ReadInt();
        for (int index = 0; index < sceneCount; ++index)
        {
            AttrRatioInfo info = new AttrRatioInfo();
            
            info.Load(helper);

            m_map.Add(info.m_id, info);
        }
    }
}