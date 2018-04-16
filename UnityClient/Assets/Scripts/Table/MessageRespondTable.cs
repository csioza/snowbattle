using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

public enum ENMessageRespond
{
	enNone = 0,
	enLoginSuccess			= 1,	//登录成功
	enConnFailed			= 2,	//连接失败
	enIDNotFound			= 3,	//ID不正确，搜寻失败
	enAddFriendReqSent		= 4,	//交友申请已送出
	enDelFriendSuccess		= 5,	//删除成功
	enRenameSuccess			= 6,	//已更改名字
	enRenameInlegal			= 7,	//名字内含有敏感词汇，请重新输入
	enMergeFailed			= 8,		//强化失败，卡牌不合法
	enEvolutionFailed		= 9,	//进化失败，卡牌不合法
	enDungeonIDNotFound		= 10,	//副本ID查找失败
	enNotEnoughStamina		= 11,	//体力不足

	enAlreadyFriends        = 12,		//你们已经是好友了，无需再次发送
	enRepeatedAddFriendRequest= 13,		//你已请求过添加该好友
	enTooManyUntreatedRequest= 14,		//对方未处理的请求过多
	enInvalidFriendRequest  = 15,		//失效的请求
	enFriendFull            = 16,		//好友已满
	enTargetFriendFull      = 17,		//对方好友已满

	enFloorStatusError		= 18,		//当前本地和服务端的副本状态不匹配
	enNotEnoughMoney		= 19,		//金钱不够
	enNotEnoughBMoney		= 20,		//绑定钱不够
	enNotEnoughRing			= 21,		//戒指不够
	enNotEnoughFriendPoint	= 22,		//友情点不足
	enMax,
}

public class MessageRespondInfo : IDataBase
{
    public int ID { get; protected set; }
    //描述
    public string Describe { get; protected set; }   
}


public class MessageRespondTable
{
	public MessageRespondInfo Lookup(int id)
	{
		MessageRespondInfo info;
		MessageRespondInfoList.TryGetValue(id, out info);
		return info;
	}
	public SortedList<int, MessageRespondInfo> MessageRespondInfoList { get; protected set; }
	public void Load(byte[] bytes)
	{
		BinaryHelper helper = new BinaryHelper(bytes);
		int length = helper.ReadInt();
		MessageRespondInfoList = new SortedList<int, MessageRespondInfo>(length);
		for (int index = 0; index < length; ++index)
		{
			MessageRespondInfo info = new MessageRespondInfo();
			info.Load(helper);
			MessageRespondInfoList.Add(info.ID, info);
		}
	}
}