
using System;
using System.Collections.Generic;
using UnityEngine;

public class IconInfomation : IDataBase
{
	//图标ID
	public int ID { get; protected set; }
	//
	public string dirName { get; protected set; }

}

public class IconInfoTable
{
    public Dictionary<int, IconInfomation> m_list { get; protected set; }
    public IconInfomation Lookup(int id)
	{
        IconInfomation info = null;
        m_list.TryGetValue(id, out info);
        return info;
	}
	public void Load(byte[] bytes)
	{
        m_list = new Dictionary<int, IconInfomation>();
		BinaryHelper helper = new BinaryHelper(bytes);
		int length = helper.ReadInt();
		for (int index = 0; index < length; ++index)
		{
            IconInfomation info = new IconInfomation();
			info.Load(helper);
			m_list.Add(info.ID, info);
		}
	}
};