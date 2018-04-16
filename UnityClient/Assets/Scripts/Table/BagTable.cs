using System;
using System.Collections.Generic;

public enum BAGTYPEENUM
{
    enCardBagType =1, //  卡牌背包
}
public class BagInfo : IDataBase
{
    public int m_id { get; protected set; }
    public string m_bagType { get; protected set; }
    public int m_initalSize { get; protected set; }
    public int m_maxSize { get; protected set; }
    public int m_expandCost1 { get; protected set; }
    public int m_expandSize1 { get; protected set; }
    public int m_expandCost2 { get; protected set; }
    public int m_expandSize2 { get; protected set; }
    public int m_expandCost3 { get; protected set; }
    public int m_expandSize3 { get; protected set; }
}

public class BagTable
{
    public Dictionary<int, BagInfo> m_map;
    public BagInfo LookUp(int id)
    {
        BagInfo info = null;
        if (m_map.TryGetValue(id, out info))
        {
            return info;
        }

        return null;
    }

    public void Load(byte[] bytes)
    {
        m_map               = new Dictionary<int, BagInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount      = helper.ReadInt();

        for (int index = 0; index < sceneCount; ++index)
        {
            BagInfo info    = new BagInfo();
            
            info.Load(helper);

            m_map.Add(info.m_id, info);
        }
    }
}