using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//Info.
public class MagicStoneTableInfo : IDataBase
{
    public int id { get; protected set; }
    public int magciStoneNumber { get; protected set; }
    public int price { get; protected set; }
    public int discountPrice { get; protected set; }
}

public class MagicStoneTable
{
    public List<MagicStoneTableInfo> m_magicStoneTableInfoList { get; protected set; }
    public MagicStoneTableInfo Lookup(int id)
    {
        return m_magicStoneTableInfoList.Find(item => item.id == id);
    }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        m_magicStoneTableInfoList = new List<MagicStoneTableInfo>(length);
        for (int index = 0; index < length; ++index)
        {
            MagicStoneTableInfo info = new MagicStoneTableInfo();
            info.Load(helper);
            m_magicStoneTableInfoList.Add(info);
        }
    }
}