//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Table
//	created:	2013-5-14
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEngine;


public class VocationInfo : IDataBase
{
	public int ID { get; private set; }
	public string Name { get; private set; }
	public int ModelID { get; private set; }
	public int MapID { get; private set; }
	public float MapXPos { get; private set; }
	public float MapYPos { get; private set; }
}

//职业表
public class VocationTable
{
	public List<VocationInfo> VocationInfoList { get; protected set; }
	public VocationInfo Lookup(int id)
	{
		return VocationInfoList.Find(item => item.ID == id);
	}
	public void Load(byte[] bytes)
	{
		BinaryHelper helper = new BinaryHelper(bytes);
		int length = helper.ReadInt();
		VocationInfoList = new List<VocationInfo>(length);
		for (int index = 0; index < length; ++index)
		{
			VocationInfo info = new VocationInfo();
			info.Load(helper);
			VocationInfoList.Add(info);
		}
	}
};