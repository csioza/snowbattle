using System;
using System.Collections.Generic;
using UnityEngine;


//Info.
public class EquipExpMoneyInfo : IDataBase
{
    public int level{ get; protected set; }
    public int maxExp{ get; protected set; }
    public int needMoney{ get; protected set; }
}

//装备经验Info表.   
public class EquipExpMoneyTable
{
    public List<EquipExpMoneyInfo> m_equipExpMoneyInfoList { get; protected set; }
    public EquipExpMoneyInfo Lookup(int level)
    {
        return m_equipExpMoneyInfoList.Find(item => item.level == level);
    }

    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        m_equipExpMoneyInfoList = new List<EquipExpMoneyInfo>(length);
        for (int index = 0; index < length; ++index)
        {
            EquipExpMoneyInfo info = new EquipExpMoneyInfo();
            info.Load(helper);
            m_equipExpMoneyInfoList.Add(info);
        }
    }
}
