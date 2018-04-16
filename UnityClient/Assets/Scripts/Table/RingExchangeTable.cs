using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RingExchangeTableInfo:IDataBase
{
    public int id { get; protected set; }
    public string startDate { get; protected set; }
    public string startTime { get; protected set; }
    public string endDate { get; protected set; }
    public string endTime { get; protected set; }
}

public class RingExchangeTable
{
    public List<RingExchangeTableInfo> m_ringExchangeTableInfoList { get; protected set; }
    public RingExchangeTableInfo Lookup(int id)
    {
        return m_ringExchangeTableInfoList.Find(item => item.id == id);
    }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        m_ringExchangeTableInfoList = new List<RingExchangeTableInfo>(length);
        for (int index = 0; index < length; ++index)
        {
            RingExchangeTableInfo info = new RingExchangeTableInfo();
            info.Load(helper);
            m_ringExchangeTableInfoList.Add(info);
        }
    }
}
