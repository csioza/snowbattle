using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NpcShop : IDataBase
{
    public int ShopId { get; protected set; }
    public string ShopName { get; protected set; }
    public List<int> ShopItems { get; protected set; }
}

public class ShopTable
{
    public Dictionary<int, NpcShop > NpcShopList { get; protected set; }     // NPC…ÃµÍ
	public NpcShop Lookup(int id)
	{
        NpcShop vShop = null;
        NpcShopList.TryGetValue(id, out vShop);
		return vShop;
	}
	public void Load(byte[] bytes)
	{
		BinaryHelper helper = new BinaryHelper(bytes);
		int length = helper.ReadInt();
        NpcShopList = new Dictionary<int, NpcShop >();
		for (int index = 0; index < length; ++index)
		{
			NpcShop info = new NpcShop();
            info.Load(helper);
            NpcShopList.Add(info.ShopId, info);
		}
	}
}
