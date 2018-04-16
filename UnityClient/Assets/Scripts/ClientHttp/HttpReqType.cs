using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 添加这里注意:需要添加一个消息的ID (HttpReqMsgID), 一个对应处理的URL(HttpReqUrl) 
/// 然后在MessageIDTranslate:Init中 把他们关联起来.才可以
/// </summary>
public struct HttpReqMsgID
{
	public const short POST_GATESERVER_GLOBALCONFIG = 0;	//网关配置
	public const short POST_ACCOUNT_SERVERCONFIG= 1;
	public const short POST_ACCOUNT_GET_ZONE	= 2;
	public const short POST_ACCOUNT_REGISTER	= 3;
	public const short POST_ACCOUNT_CHECK		= 4;
	public const short POST_ACCOUNT_LOGIN		= 5;
	public const short POST_ACCOUNT_CHANGEPWD	= 6;
	public const short POST_USER_CREATE			= 7;
	public const short POST_HEROCARD_GET		= 8;
	public const short POST_HEROCARD_MERGE		= 9;
	public const short POST_HEROCARD_EVOLVE		= 10;
}
public struct HttpReqUrl
{
	public const string POST_GATESERVER_GLOBALCONFIG = "api/gateserver/globalconfig";
	public const string POST_SERVER_CONFIG = "api/account/serverconfig";
	public const string POST_ACCOUNT_GET_ZONE = "api/account/zone";
	public const string POST_ACCOUNT_REGISTER = "api/account/register";
	public const string POST_ACCOUNT_CHECK = "api/account/check";
	public const string POST_ACCOUNT_LOGIN = "api/user/login";
	public const string POST_ACCOUNT_CHANGEPWD = "api/account/changepwd";
	public const string POST_USER_CREATE = "api/user/create";

	public const string POST_HEROCARD_GET = "api/herocard/get";
	public const string POST_HEROCARD_MERGE = "api/herocard/merge";
	public const string POST_HEROCARD_EVOLVE = "api/herocard/evolve";
}

public class MessageIDTranslate
{
	#region singleton
	private static MessageIDTranslate m_transfer;
	public static MessageIDTranslate Singleton
	{
		get
		{
			if (m_transfer == null)
			{
				m_transfer = new MessageIDTranslate();
				m_transfer.Init();
			}
			return m_transfer;
		}
	}
	#endregion
	Dictionary<short, string> m_MsgUrlDic = new Dictionary<short, string>();

	private void Init()
	{
		m_MsgUrlDic.Add(HttpReqMsgID.POST_GATESERVER_GLOBALCONFIG, HttpReqUrl.POST_GATESERVER_GLOBALCONFIG);
		m_MsgUrlDic.Add(HttpReqMsgID.POST_ACCOUNT_SERVERCONFIG, HttpReqUrl.POST_SERVER_CONFIG);		
		m_MsgUrlDic.Add(HttpReqMsgID.POST_ACCOUNT_GET_ZONE, HttpReqUrl.POST_ACCOUNT_GET_ZONE);
		m_MsgUrlDic.Add(HttpReqMsgID.POST_ACCOUNT_REGISTER, HttpReqUrl.POST_ACCOUNT_REGISTER);
		m_MsgUrlDic.Add(HttpReqMsgID.POST_ACCOUNT_CHECK, HttpReqUrl.POST_ACCOUNT_CHECK);
		m_MsgUrlDic.Add(HttpReqMsgID.POST_ACCOUNT_LOGIN, HttpReqUrl.POST_ACCOUNT_LOGIN);
		m_MsgUrlDic.Add(HttpReqMsgID.POST_ACCOUNT_CHANGEPWD, HttpReqUrl.POST_ACCOUNT_CHANGEPWD);
		m_MsgUrlDic.Add(HttpReqMsgID.POST_USER_CREATE, HttpReqUrl.POST_USER_CREATE);
		m_MsgUrlDic.Add(HttpReqMsgID.POST_HEROCARD_GET, HttpReqUrl.POST_HEROCARD_GET);
		m_MsgUrlDic.Add(HttpReqMsgID.POST_HEROCARD_MERGE, HttpReqUrl.POST_HEROCARD_MERGE);
		m_MsgUrlDic.Add(HttpReqMsgID.POST_HEROCARD_EVOLVE, HttpReqUrl.POST_HEROCARD_EVOLVE);
	}

	public string GetMsgUrl(short msgID)
	{
		string url = "";
		if (m_MsgUrlDic.TryGetValue(msgID, out url))
		{
			return url;
		}
		return "";
	}
}