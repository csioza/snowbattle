using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WWWHelper 
{
	#region singleton
	static private WWWHelper m_singleton;
	static public WWWHelper Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new WWWHelper();
			}
			return m_singleton;
		}
	}
	#endregion


	public void WWWPost(short msgID,MessageBlock msg)
	{
		string realUrl = GetRealPostUrl(msgID);
		if (realUrl == "")
		{
			return;
		}

		MainGame.Singleton.StartCoroutine(BeginPost(msgID, realUrl, ((HttpMessageBlock)msg).SerializeToForm()));	
	}


	public IEnumerator BeginPost(short msgID, string url, WWWForm sendFrom)
	{
		WWW getData = new WWW(url, sendFrom);
		yield return getData;
		if (getData.error != null)
		{
			Debug.Log("msgID = " + msgID + "url = " + url);
			Debug.Log(getData.error);
			if (MainGame.Singleton.CurrentState is StateInitApp)
			{
				GlobalServerSetting.Singleton.GotSetting = 2;
			}
		}
		else
		{
			Debug.Log("send msgID = " + msgID.ToString() + ",serverMsg = " + getData.text);

			//解析出一个列表,然后handle
			Dictionary<string, ServerMsgStruct> msgList;
			int respond = ServerMsgAnalyzer.Singleton.AnalyzeServerMessage(getData.text, out msgList);
			if (msgList != null)
			{
				//默认的处理函数
				ServerMsgDefaultHandler.Singleton.HandleServerMessage(respond, msgList);
				//自定义处理函数
				ServerMsgHandler.Singleton.HandleServerMessage(msgID, respond, msgList);

			}
		}
	}

	private string GetRealPostUrl(short msgID)
	{
		string postfixUrl = MessageIDTranslate.Singleton.GetMsgUrl(msgID);
		if (postfixUrl == "")
		{
			Debug.Log("no this msg . id= " + msgID);
			return "";
		}

		//todo hash 待定
		string hash = "abcde";
		return HTTPSetting.REQ_Website + postfixUrl + "&hash=" + hash;
	}

}
