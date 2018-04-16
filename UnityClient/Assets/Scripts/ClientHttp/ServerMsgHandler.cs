using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerMsgHandler
{
	#region singleton
	static private ServerMsgHandler m_singleton;
	static public ServerMsgHandler Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new ServerMsgHandler();
				m_singleton.Init();
			}
			return m_singleton;
		}
	}
	#endregion

	public delegate void OnServerMsgReceived(int respond,Dictionary<string,ServerMsgStruct> msgList);
	//自定义列表
	private Dictionary<short, OnServerMsgReceived> m_HandlerDic = new Dictionary<short, OnServerMsgReceived>();

	//按类型注册的处理函数	
	public void Init()
	{
		RegisterHandler(HttpReqMsgID.POST_GATESERVER_GLOBALCONFIG, OnMsgGlobalConfigs);
		RegisterHandler(HttpReqMsgID.POST_ACCOUNT_SERVERCONFIG, OnMsgServerConfigs);
		RegisterHandler(HttpReqMsgID.POST_ACCOUNT_GET_ZONE, OnMsgGetZone);
		RegisterHandler(HttpReqMsgID.POST_ACCOUNT_REGISTER, OnMsgAccRegister);
		RegisterHandler(HttpReqMsgID.POST_ACCOUNT_CHECK, OnMsgAccCheck);
		RegisterHandler(HttpReqMsgID.POST_ACCOUNT_LOGIN, OnMsgLogin);
		RegisterHandler(HttpReqMsgID.POST_USER_CREATE,OnMsgUserCreate);
		RegisterHandler(HttpReqMsgID.POST_ACCOUNT_CHANGEPWD, OnMsgChangePwd);
		RegisterHandler(HttpReqMsgID.POST_HEROCARD_GET, OnMsgGetHeroCard);
		RegisterHandler(HttpReqMsgID.POST_HEROCARD_MERGE, OnMsgHeroCardMerge);
		RegisterHandler(HttpReqMsgID.POST_HEROCARD_EVOLVE, OnMsgHeroCardEvolve);
	}

	private void RegisterHandler(short msgID,OnServerMsgReceived handler)
	{
		m_HandlerDic.Add(msgID, handler);
	}

	private OnServerMsgReceived GetHandler(short msgID)
	{
		OnServerMsgReceived handler;
		if (m_HandlerDic.TryGetValue(msgID,out handler))
		{
			return handler;
		}
		return null;
	}

	public void HandleServerMessage(short msgID, int respond, Dictionary<string, ServerMsgStruct> serverMsgList)
	{
		OnServerMsgReceived handler = GetHandler(msgID);
		if (handler != null)
		{
			handler(respond, serverMsgList);
		}
		
	}

	//华丽的分割线--------------------------------------------------------------------------------------------
	public void OnMsgGlobalConfigs(int respond, Dictionary<string, ServerMsgStruct> msgList)
	{
		ServerMsgStruct msgStruct;
		if (msgList.TryGetValue(ServerMessageKeyWords.KEYWORD_GLOBAL_CONFIGS, out msgStruct) == false)
		{
			return;
		}

//		ServermsgStructGlobalConfig config = (ServermsgStructGlobalConfig)msgStruct;

		Debug.Log("zones count = " + GlobalServerSetting.Singleton.ZoneList.Count);
	}

	public void OnMsgServerConfigs(int respond, Dictionary<string, ServerMsgStruct> msgList)
	{
		ServerMsgStruct msgStruct;
		if (msgList.TryGetValue(ServerMessageKeyWords.KEYWORD_SERVER_CONFIGS, out msgStruct) == false)
		{
			return;
		}
//		ServermsgStructServerConfig configs = (ServermsgStructServerConfig)msgStruct;
		Debug.Log("cards count = " + ZoneServerSetting.Singleton.initHerocards.Count);
	}

	public void OnMsgGetZone(int respond, Dictionary<string, ServerMsgStruct> msgList)
	{
		ServerMsgStruct msgStruct;
		if (msgList.TryGetValue(ServerMessageKeyWords.KEYWORD_ZONES, out msgStruct) == false)
		{
			return;
		}

		ServerMsgStructZone msg = (ServerMsgStructZone)msgStruct;
//		foreach (ZoneInfo info in msg.zoneInfoList)
//		{
			//Debug.Log(info.zoneName + info.zoneUrl);
//		}

       // UIServerList.GetInstance().UpdateList(msg);
        Login.Singleton.OnGetZone(msg);

	}

	public void OnMsgAccRegister(int respond, Dictionary<string, ServerMsgStruct> msgList)
	{
		ServerMsgStruct msgStruct;
		if (msgList.TryGetValue(ServerMessageKeyWords.KEYWORD_ACCOUNTINFO, out msgStruct) == false)
		{
			return;
		}
		Debug.Log("OnMsgAccRegister");
//		ServerMsgStructAccountInfo msg = (ServerMsgStructAccountInfo)msgStruct;
        //UIRegist.GetInstance().OnRegisteredResult(respond,msg);
       // Login.Singleton.OnAccRegister(respond, msg);
	}

	public void OnMsgAccCheck(int respond, Dictionary<string, ServerMsgStruct> msgList)
	{
		ServerMsgStruct msgStruct;
		if (msgList.TryGetValue(ServerMessageKeyWords.KEYWORD_ACCOUNTINFO, out msgStruct) == true)
		{
//            ServerMsgStructAccountInfo msg = (ServerMsgStructAccountInfo)msgStruct;

           // Login.Singleton.OnAccCheck(respond, msg);
		}
		

       
	}


	public void OnMsgLogin(int respond, Dictionary<string, ServerMsgStruct> msgList)
	{
		ServerMsgStruct msgStruct;
		if (msgList.TryGetValue(ServerMessageKeyWords.KEYWORD_USERINFO, out msgStruct) == true)
		{
//            ServerMsgStructUserInfo msg = (ServerMsgStructUserInfo)msgStruct;
		}

       
	}

	public void OnMsgUserCreate(int respond, Dictionary<string, ServerMsgStruct> msgList)
	{
        Debug.Log("OnMsgUserCreate " + respond);
        ServerMsgStruct msgStruct;

        if (msgList.TryGetValue(ServerMessageKeyWords.KEYWORD_USERINFO, out msgStruct) == false)
        {
            return;
        }

//        ServerMsgStructUserInfo msg = (ServerMsgStructUserInfo)msgStruct;

      //  Login.Singleton.OnMsgUserCreate(respond, msg);

	}

	//改密码
	public void OnMsgChangePwd(int respond, Dictionary<string, ServerMsgStruct> msgList)
	{
		ServerMsgStruct msgStruct;
		if (msgList.TryGetValue(ServerMessageKeyWords.KEYWORD_ACCOUNTINFO, out msgStruct) == false)
		{
			return;
		}
		ServerMsgStructAccountInfo msg = (ServerMsgStructAccountInfo)msgStruct;

		Debug.Log(msg.accname + "  ..  " + msg.accpwd);
        //UIEditPassword.GetInstance().OnEditResult(respond,msg);
        Login.Singleton.OnChangePwd(respond, msg);
	}

	//自己获取卡牌
	public void OnMsgGetHeroCard(int respond, Dictionary<string, ServerMsgStruct> msgList)
	{
		//
		Debug.Log("OnMsgGetHeroCard");
	}
	//卡牌合成
	public void OnMsgHeroCardMerge(int respond, Dictionary<string, ServerMsgStruct> msgList)
	{
		//
		Debug.Log("OnMsgHeroCardMerge");
        // 刷新 升级卡牌相关界面
        CardUpdateProp.Singleton.FlushUpdateCardData();
	}

	//卡牌进化
	public void OnMsgHeroCardEvolve(int respond, Dictionary<string, ServerMsgStruct> msgList)
	{
		//
		Debug.Log("OnMsgHeroCardEvolve");

        ServerMsgStruct msgStruct;
        if (msgList.TryGetValue(ServerMessageKeyWords.KEYWORD_HEROCARD_GAIN, out msgStruct) == false)
        {
            return;
        }

//        ServerMsgStructHeroCardGain msg = (ServerMsgStructHeroCardGain)msgStruct;

       // CardEvolution.Singleton.OnHeroCardEvolve(respond, msg);
	}

}