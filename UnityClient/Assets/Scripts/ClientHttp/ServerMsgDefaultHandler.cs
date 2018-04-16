using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerMsgDefaultHandler
{

	#region singleton
	static private ServerMsgDefaultHandler m_singleton;
	static public ServerMsgDefaultHandler Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new ServerMsgDefaultHandler();
				m_singleton.Init();
			}
			return m_singleton;
		}
	}
	#endregion

	public delegate void ServerMsgHandler(int respond , ServerMsgStruct data);
	//默认解析列表
	private Dictionary<string, ServerMsgHandler> m_DefaultHandler = new Dictionary<string, ServerMsgHandler>();


	//依据关键字的默认解析
	private void Init()
	{
		RegisterDefaultHandler(ServerMessageKeyWords.KEYWORD_GLOBAL_CONFIGS, OnServerMsgGlobalConfig);
		RegisterDefaultHandler(ServerMessageKeyWords.KEYWORD_SERVER_CONFIGS, OnServerMsgServerConfig);
		RegisterDefaultHandler(ServerMessageKeyWords.KEYWORD_ZONES, OnServerMsgZones);
		RegisterDefaultHandler(ServerMessageKeyWords.KEYWORD_SESSION, OnServerMsgSession);
		RegisterDefaultHandler(ServerMessageKeyWords.KEYWORD_ACCOUNTINFO, OnServerMsgAccountInfo);
		RegisterDefaultHandler(ServerMessageKeyWords.KEYWORD_USERINFO, OnServerMsgUserInfo);
		RegisterDefaultHandler(ServerMessageKeyWords.KEYWORD_HEROCARD_BAG, OnServerMsgHerocardBag);
		RegisterDefaultHandler(ServerMessageKeyWords.KEYWORD_HEROCARD_GAIN, OnServerMsgHerocardGain);
		RegisterDefaultHandler(ServerMessageKeyWords.KEYWORD_HEROCARD_LOSE, OnServerMsgHerocardLose);
		RegisterDefaultHandler(ServerMessageKeyWords.KEYWORD_HEROCARD_UPDATE, OnServerMsgHerocardUpdate);

	}
	private void RegisterDefaultHandler(string keyWord, ServerMsgHandler handler)
	{
		m_DefaultHandler.Add(keyWord, handler);
	}

	private ServerMsgHandler GetHandler(string keyWord)
	{
		ServerMsgHandler handler = null;
		if (m_DefaultHandler.TryGetValue(keyWord,out handler))
		{
			return handler;
		}
		return null;
	}

	public void HandleServerMessage(int respond , Dictionary<string, ServerMsgStruct> serverMsgs)
	{
		foreach (KeyValuePair<string, ServerMsgStruct> serverMsg in serverMsgs)
		{
			ServerMsgHandler handler = GetHandler(serverMsg.Key);
			if (handler != null)
			{
				handler(respond, serverMsg.Value);
			}
			else
			{
				continue;
			}
		}
	}

	private void OnServerMsgGlobalConfig(int respond, ServerMsgStruct msgStruct)
	{
		ServermsgStructGlobalConfig globalSetting = (ServermsgStructGlobalConfig)msgStruct;
		if (globalSetting == null)
		{
			return;
		}

		Debug.Log("OnServerMsgGlobalConfig");
		GlobalServerSetting.Singleton.GotSetting = 1;
		GlobalServerSetting.Singleton.ZoneList = new List<ZoneInfo>(globalSetting.zoneInfoList);
	}

	private void OnServerMsgServerConfig(int respond, ServerMsgStruct msgStruct)
	{
		Debug.Log("OnServerMsgServerConfig");
		ServermsgStructServerConfig serverSetting = (ServermsgStructServerConfig)msgStruct;
		if (serverSetting == null)
		{
			return;
		}

		ZoneServerSetting.Singleton.initHerocards = new List<int>(serverSetting.initHerocards);
	}

	private void OnServerMsgZones(int respond, ServerMsgStruct msgStruct)
	{
		Debug.Log("OnServerMsgZones");
	}

	private void OnServerMsgSession(int respond, ServerMsgStruct msgStruct)
	{
		ServerMsgStructSession sessionInfo = (ServerMsgStructSession)msgStruct;
		MessageTransfer.Singleton.Session = sessionInfo.session;
		Debug.Log("OnServerMsgSession");
	}
	private void OnServerMsgAccountInfo(int respond, ServerMsgStruct msgStruct)
	{
		
		Debug.Log("OnServerMsgAccountInfo");
		//①把结构 给客户端数据
		//②通知.

	}
	private void OnServerMsgUserInfo(int respond, ServerMsgStruct msgStruct)
	{
		//
		ServerMsgStructUserInfo userInfo = (ServerMsgStructUserInfo)msgStruct;
		MessageTransfer.Singleton.UID = userInfo.uid;

		Debug.Log("OnServerMsgUserInfo");
	}
	private void OnServerMsgHerocardBag(int respond, ServerMsgStruct msgStruct)
	{
		Debug.Log("OnServerMsgHerocardBag");


        ServerMsgHerocardBag msg = (ServerMsgHerocardBag)msgStruct;

        CardBag.Singleton.InitCard(msg);
     
	}


	private void OnServerMsgHerocardGain(int respond, ServerMsgStruct msgStruct)
	{
		Debug.Log("ServerMsgStructHeroCardGain");
//		ServerMsgStructHeroCardGain msg = (ServerMsgStructHeroCardGain)msgStruct;

        //CardBag.Singleton.OnHeroCardGain(respond, msg);
	}

	private void OnServerMsgHerocardLose(int respond, ServerMsgStruct msgStruct)
	{
		Debug.Log("OnServerMsgHerocardLose");
//		ServerMsgStructHeroCardLose msg = (ServerMsgStructHeroCardLose)msgStruct;

        //CardBag.Singleton.OnHeroCardLose(respond, msg);
	}

	private void OnServerMsgHerocardUpdate(int respond, ServerMsgStruct msgStruct)
	{
		Debug.Log("OnServerMsgHerocardUpdate");
//		ServerMsgStructHeroCardUpdate msg = (ServerMsgStructHeroCardUpdate)msgStruct;

        //CardBag.Singleton.OnHeroCardUpdate(respond, msg);
     }


}