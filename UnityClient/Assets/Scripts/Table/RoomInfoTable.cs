//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEngine;


public class RoomEditInfo : IDataBase
{
    public int id { get; private set; }		
    public string name { get; private set; }
	
}

public class RoomEditInfoTable
{
    public Dictionary<int, RoomEditInfo> m_list { get; protected set; }
    public RoomEditInfo Lookup(int id)
	{
        RoomEditInfo info = null;
        m_list.TryGetValue(id, out info);
        return info;
	}
	public void Load(byte[] bytes)
	{
        m_list = new Dictionary<int, RoomEditInfo>();
		BinaryHelper helper = new BinaryHelper(bytes);
		int length = helper.ReadInt();
		for (int index = 0; index < length; ++index)
		{
            RoomEditInfo info = new RoomEditInfo();
            info.Load(helper);
			m_list.Add(info.id, info);
		}
	}
};