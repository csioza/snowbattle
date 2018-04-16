using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerMsgAnalyzer
{
	#region singleton
	static private ServerMsgAnalyzer m_singleton;
	static public ServerMsgAnalyzer Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new ServerMsgAnalyzer();
				m_singleton.Init();
			}
			return m_singleton;
		}
	}
	#endregion


	delegate ServerMsgStruct OnMsgAnalized(int response, object data);
	//Dictionary<int, OnMsgAnalized> m_analizerDic = new Dictionary<int, OnMsgAnalized>();

	//默认解析函数的列表.
	delegate ServerMsgStruct MessageAnalizer(object data);
	Dictionary<string, MessageAnalizer> m_defaultAnalizerDic = new Dictionary<string, MessageAnalizer>();



	/// <summary>
	/// 这里是注册解析的回调.
	/// </summary>
	void Init()
	{
		//注册默认解析函数
		RegisterDefaultAnalyzer(ServerMessageKeyWords.KEYWORD_GLOBAL_CONFIGS, DefaulatAnalyzeGlobalConfigs);
		RegisterDefaultAnalyzer(ServerMessageKeyWords.KEYWORD_SERVER_CONFIGS,DefaulatAnalyzeServerConfigs);
		RegisterDefaultAnalyzer(ServerMessageKeyWords.KEYWORD_ZONES, DefaulatAnalyzeZones);
		RegisterDefaultAnalyzer(ServerMessageKeyWords.KEYWORD_SESSION, DefaulatAnalyzeSession);
		RegisterDefaultAnalyzer(ServerMessageKeyWords.KEYWORD_ACCOUNTINFO, DefaulatAnalyzeAccountInfo);
		RegisterDefaultAnalyzer(ServerMessageKeyWords.KEYWORD_USERINFO, DefaulatAnalyzeUserInfo);
		RegisterDefaultAnalyzer(ServerMessageKeyWords.KEYWORD_HEROCARD_BAG, DefaulatAnalyzeHerocardBag);
		RegisterDefaultAnalyzer(ServerMessageKeyWords.KEYWORD_HEROCARD_GAIN, DefaulatAnalyzeHerocardGain);
		RegisterDefaultAnalyzer(ServerMessageKeyWords.KEYWORD_HEROCARD_LOSE, DefaulatAnalyzeHerocardLose);
		RegisterDefaultAnalyzer(ServerMessageKeyWords.KEYWORD_HEROCARD_UPDATE, DefaulatAnalyzeHerocardUpdate);
	}
	private void RegisterDefaultAnalyzer(string keyWord, MessageAnalizer analyzer)
	{
		m_defaultAnalizerDic.Add(keyWord, analyzer);
	}

	private MessageAnalizer GetAnalizer(string keyWord)
	{
		MessageAnalizer analyzer;
		if (m_defaultAnalizerDic.TryGetValue(keyWord, out analyzer))
		{
			return analyzer;
		}
		return null;
	}

	public int AnalyzeServerMessage(string msgStr,out Dictionary<string,ServerMsgStruct> msgStructs)
	{
		if (!msgStr.StartsWith("{"))
		{
			msgStr = msgStr.Substring(msgStr.IndexOf('{'));
		}
		Dictionary<string, object> serverMsg = MiniJSON.Json.Deserialize(msgStr) as Dictionary<string, object>;
		int respond = int.Parse(serverMsg["respond"].ToString());


		object dataObj = null;		
		if (serverMsg.TryGetValue("data", out dataObj))
		{
			//解析得到的msg结构
			msgStructs = DefaulatAnalyzeCallback(dataObj);
		}
		else
		{
			msgStructs = null;
			Debug.Log("no data in server respond");
		}
		return respond;		
	}

	private Dictionary<string, ServerMsgStruct> DefaulatAnalyzeCallback(object data)
	{
		//解析完之后得到的Dictionary
		Dictionary<string, ServerMsgStruct>  ServerMsgList = new Dictionary<string, ServerMsgStruct>();
		Dictionary<string, object> reponseData = (Dictionary<string, object>)data;		
		foreach (KeyValuePair<string, object> respondStructData in reponseData)
		{
			MessageAnalizer analizer = GetAnalizer(respondStructData.Key);
			if (analizer == null)
			{
				Debug.Log("no default analizer, key = " + respondStructData.Key);
				continue;
			}
			//调用默认解析
			ServerMsgStruct msgStruct = analizer(respondStructData.Value);
			if (msgStruct == null)
			{
				Debug.Log("msgStruct is null, key = " + respondStructData.Key);
			}
			else
			{
				ServerMsgList.Add(respondStructData.Key, msgStruct);
			}
		}
		return ServerMsgList;
	}

	private string GetString(Dictionary<string, object> objDic, string key)
	{
		object obj;
		if (objDic.TryGetValue(key,out obj) && obj != null)
		{
			return obj.ToString();
		}
		return "";
	}

	private int GetInt(Dictionary<string, object> objDic, string key)
	{
		string intStr = GetString(objDic, key);

		int outInt = -1;
		int.TryParse(intStr, out outInt);
		return outInt;
	}

	//-------------------------------------------------------------------------------------------
	private ServerMsgStruct DefaulatAnalyzeGlobalConfigs(object obj)
	{
		ServermsgStructGlobalConfig msgStruct = new ServermsgStructGlobalConfig();
		msgStruct.analyzeData(obj);
		return msgStruct;
	}

	private ServerMsgStruct DefaulatAnalyzeServerConfigs(object obj)
	{
		ServermsgStructServerConfig msgStruct = new ServermsgStructServerConfig();
		msgStruct.analyzeData(obj);
		return msgStruct;
	}

	private ServerMsgStruct DefaulatAnalyzeZones(object obj)
	{
		ServerMsgStructZone msgStruct = new ServerMsgStructZone();
		msgStruct.analyzeData(obj);
		return msgStruct;
	}

	//session  只发送字符串过来.
	private ServerMsgStruct DefaulatAnalyzeSession(object obj)
	{
		ServerMsgStructSession sessionSturct = new ServerMsgStructSession();
		sessionSturct.analyzeData(obj);
		return sessionSturct;

	}

	//account_info
	private ServerMsgStruct DefaulatAnalyzeAccountInfo(object obj)
	{
		ServerMsgStructAccountInfo msgStruct = new ServerMsgStructAccountInfo();
		msgStruct.analyzeData(obj);
		return msgStruct;
	}

	//user_info
	private ServerMsgStruct DefaulatAnalyzeUserInfo(object obj)
	{
		ServerMsgStructUserInfo userinfoStruct = new ServerMsgStructUserInfo();
		userinfoStruct.analyzeData(obj);
	
		return userinfoStruct;
	}

	//herocard_bag
	private ServerMsgStruct DefaulatAnalyzeHerocardBag(object obj)
	{
		ServerMsgHerocardBag msgStruct = new ServerMsgHerocardBag();
		msgStruct.analyzeData(obj);

		return msgStruct;
	}
	private ServerMsgStruct DefaulatAnalyzeHerocardGain(object obj)
	{
		ServerMsgStructHeroCardGain msgStruct = new ServerMsgStructHeroCardGain();
		msgStruct.analyzeData(obj);

		return msgStruct;
	}
	private ServerMsgStruct DefaulatAnalyzeHerocardLose(object obj)
	{
		ServerMsgStructHeroCardLose msgStruct = new ServerMsgStructHeroCardLose();
		msgStruct.analyzeData(obj);

		return msgStruct;
	}
	private ServerMsgStruct DefaulatAnalyzeHerocardUpdate(object obj)
	{
		ServerMsgStructHeroCardUpdate msgStruct = new ServerMsgStructHeroCardUpdate();
		msgStruct.analyzeData(obj);
		return msgStruct;
	}
}
