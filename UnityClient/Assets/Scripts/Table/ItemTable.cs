using System;
using System.Collections.Generic;

public class ItemInfo : IDataBase
{
    public int m_id { get; protected set; }
    public string m_name { get; protected set; }
    public int m_type { get; protected set; }
    public int m_iconId { get; protected set; }
    public string m_description { get; protected set; }
    public int m_quality { get; protected set; }
    public int m_maxNumber { get; protected set; }
    public int m_isCanSale { get; protected set; }
    public int m_salePrice1 { get; protected set; }
    public int m_saleType1 { get; protected set; }
    public int m_salePrice2 { get; protected set; }
    public int m_saleType2 { get; protected set; }
    public int m_salePrice3 { get; protected set; }
    public int m_saleType3 { get; protected set; }  
}

public class ItemTable
{
    public Dictionary<int, ItemInfo> m_map { get; protected set; }
    public ItemInfo LookUp(int id)
    {
        ItemInfo info = null;
        if (m_map.TryGetValue(id, out info))
        {
            return info;
        }

        return null;
    }

    public void Load(byte[] bytes)
    {
        m_map = new Dictionary<int, ItemInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount      = helper.ReadInt();

        for (int index = 0; index < sceneCount; ++index)
        {
            ItemInfo info = new ItemInfo();
            
            info.Load(helper);

            m_map.Add(info.m_id, info);
        }
    }
}