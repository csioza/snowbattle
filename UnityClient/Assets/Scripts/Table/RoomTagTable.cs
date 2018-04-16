using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;


public class RoomTagInfo : IDataBase
{
    public int ID { get; protected set; }
    
    public string name { get; protected set; }   
}


public class RoomTagTable
{
    public RoomTagInfo Lookup(int id)
	{
        RoomTagInfo info;
		list.TryGetValue(id, out info);
		return info;
	}
    public SortedList<int, RoomTagInfo> list { get; protected set; }

	public void Load(byte[] bytes)
	{
		BinaryHelper helper = new BinaryHelper(bytes);
		int length = helper.ReadInt();
        list = new SortedList<int, RoomTagInfo>(length);
		for (int index = 0; index < length; ++index)
		{
            RoomTagInfo info = new RoomTagInfo();
			info.Load(helper);
            list.Add(info.ID, info);
		}
	}
}