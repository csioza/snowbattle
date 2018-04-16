using System;
using System.Collections.Generic;

public class RarityRelativeInfo : IDataBase
{
    public int m_rarity { get; protected set; }
    public int m_iconId { get; protected set; }
    public int m_levelMax { get; protected set; }
    public float m_mainAttrMutiply { get; protected set; }
    
    public int m_dropItemModelID { get; protected set; }
    public int m_dropContractIconID { get; protected set; }
   
}

public class RarityRelativeTable
{
    public Dictionary<int, RarityRelativeInfo> m_rarityRelativeMap { get; protected set; }
    public RarityRelativeInfo LookUp(int id)
    {
        RarityRelativeInfo info = null;
        if (m_rarityRelativeMap.TryGetValue(id, out info))
        {
            return info;
        }

        return null;
    }
    public void Load(byte[] bytes)
    {
        m_rarityRelativeMap = new Dictionary<int, RarityRelativeInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount = helper.ReadInt();
        for (int index = 0; index < sceneCount; ++index)
        {
            RarityRelativeInfo info = new RarityRelativeInfo();
            
            info.Load(helper);

            m_rarityRelativeMap.Add(info.m_rarity, info);
        }
    }
}