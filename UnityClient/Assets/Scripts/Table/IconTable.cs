//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Table
//	created:	2013-6-27
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEngine;
// 此表用于 以前加载图片
public class IconInfo : IDataBase
{
	//图标ID
	public int ID { get; protected set; }
	//
	public string AtlasName { get; protected set; }
	//
	public string SpriteName { get; protected set; }
}

public class IconTable
{
	public Dictionary<int, IconInfo> m_list { get; protected set; }
	public IconInfo Lookup(int id)
	{
        IconInfo info = null;
        m_list.TryGetValue(id, out info);
        return info;
	}
	public void Load(byte[] bytes)
	{
        m_list = new Dictionary<int, IconInfo>();
		BinaryHelper helper = new BinaryHelper(bytes);
		int length = helper.ReadInt();
		for (int index = 0; index < length; ++index)
		{
			IconInfo info = new IconInfo();
			info.Load(helper);
			m_list.Add(info.ID, info);
		}
	}
};