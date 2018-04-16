using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIServerList : UIWindow
{

    //UIPanel m_serverPanel                           = null;

    Dictionary<string, ZoneInfo> m_serverList       = null;
    Dictionary<string, ZoneInfo> m_allServerList    = null;



    static public UIServerList GetInstance()
	{
        UIServerList self = UIManager.Singleton.GetUIWithoutLoad<UIServerList>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIServerList>("UI/UIServerList", UIManager.Anchor.Center);
		return self;
	}

	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enLogin, OnPropertyChanged);

        //m_serverPanel       = FindChildComponent<UIPanel>("TheServers");

        m_serverList        = new Dictionary<string, ZoneInfo>();
        m_allServerList     = new Dictionary<string, ZoneInfo>();

		UpdateList();

	}
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)Login.ENPropertyChanged.enServerListUpdate)
        {
            ServerMsgStructZone msg = (ServerMsgStructZone)eventObj;
            UpdateList(msg);
        }
    }

    public override void AttachEvent()
    {
        base.AttachEvent();
     
    }

    // 进入游戏
    public void OnEnter(object sender, EventArgs e)
    {
      
    }

	public void UpdateList()
	{
		List<ZoneInfo> zoneInfo = GlobalServerSetting.Singleton.ZoneList;
		if (zoneInfo == null || zoneInfo.Count == 0)
		{
			return;
		}
		UIImageButton btn = FindChildComponent<UIImageButton>("Server");
		if (zoneInfo.Count >= 1)
		{
			btn.transform.Find("ServerTxt").name = "Txt" + 0;
			// 设置位置
			btn.transform.localPosition = Vector3.zero;
			FindChildComponent<UILabel>("Txt" + 0).text = zoneInfo[0].zoneName;

			btn.name = "" + 0;
			AddChildMouseClickEvent(btn.name, OnClickServer);

			m_serverList.Add(btn.name, zoneInfo[0]);

			Debug.Log("1UpdateList:" + zoneInfo[0].zoneName);
		}

		if (zoneInfo.Count <= 1)
		{
			return;
		}

		// 显示全部服务器
		for (int i = 0; i < zoneInfo.Count; i++)
		{
			UIImageButton btn1 = GameObject.Instantiate(btn) as UIImageButton;

			// 设置 父物体
			btn1.transform.parent = FindChildComponent<UIPanel>("AllServers").transform;

			// 设置位置
			btn1.transform.localPosition = new Vector3(0, i * (-25), 0);

			// 设置大小
			btn1.transform.localScale = btn.transform.localScale;

			// 修改按钮名称
			btn1.name = "all" + i;

			// 修改名称
			btn1.transform.Find("Txt0").name = "Txt" + i;

			FindChildComponent<UILabel>("Txt" + i).text = zoneInfo[i].zoneName;

			// 关联事件
			AddChildMouseClickEvent(btn1.name, OnClickServer2);

			if (false == m_allServerList.ContainsKey(btn1.name))
			{
				m_allServerList.Add(btn1.name, zoneInfo[i]);

			}

			Debug.Log("2UpdateList: i =" + i + "," + zoneInfo[i].zoneName + ",label.text:" + FindChildComponent<UILabel>("Txt" + i).text + ",btn1.name:" + btn1.name);


		}
	}

    public void UpdateList(ServerMsgStructZone msg)
    {
        // 暂时以列表不空 时不做处理 需修改 如果 要强制更新的时候 要跳过此判断 做额外处理 todo
        if (m_serverList.Count != 0 && m_serverList.Count != 0 )
        {
            return;
        }
        
        // 显示推荐服务器

        UIImageButton btn = FindChildComponent<UIImageButton>("Server");
        if (msg.zoneInfoList.Count >= 1)
        {
            btn.transform.Find("ServerTxt").name = "Txt" + 0;
            // 设置位置
            btn.transform.localPosition                     = Vector3.zero;
            FindChildComponent<UILabel>("Txt" + 0).text = msg.zoneInfoList[0].zoneName;

            btn.name = "" + 0;
            AddChildMouseClickEvent(btn.name, OnClickServer);

            m_serverList.Add(btn.name, msg.zoneInfoList[0]);

            //Debug.Log("1UpdateList:" + msg.zoneInfoList[0].zoneName);
        }

        if (msg.zoneInfoList.Count <= 1 )
        {
            return;
        }

        // 显示全部服务器
        for (int i = 0; i < msg.zoneInfoList.Count; i++)
        {
            UIImageButton btn1              = GameObject.Instantiate(btn) as UIImageButton;

            // 设置 父物体
            btn1.transform.parent           =  FindChildComponent<UIPanel>("AllServers").transform;

            // 设置位置
            btn1.transform.localPosition    = new Vector3(0, i * (-25), 0);

            // 设置大小
            btn1.transform.localScale       = btn.transform.localScale;

            // 修改按钮名称
            btn1.name                       = "all" + i;

            // 修改名称
            btn1.transform.Find("Txt0").name = "Txt" + i;

            FindChildComponent<UILabel>("Txt" + i).text = msg.zoneInfoList[i].zoneName;  

            // 关联事件
            AddChildMouseClickEvent(btn1.name, OnClickServer2);

            if (false == m_allServerList.ContainsKey(btn1.name))
            {
                m_allServerList.Add(btn1.name, msg.zoneInfoList[i]);

            }

            //Debug.Log("2UpdateList: i =" + i + "," + msg.zoneInfoList[i].zoneName + ",label.text:" + FindChildComponent<UILabel>("Txt" + i).text + ",btn1.name:" + btn1.name);


        }
    }

    public void OnClickServer2(object sender, EventArgs e)
    {
        GameObject obj = (GameObject)sender;

        if (m_allServerList.ContainsKey(obj.name))
        {
            UIGameInit.GetInstance().UpdateServerInfo(m_allServerList[obj.name]);

            //记录
            PlayerPrefs.SetString("XServerName", m_allServerList[obj.name].zoneName);
            PlayerPrefs.SetString("XServerAddress", m_allServerList[obj.name].zoneUrl);
        }

        HideWindow();

        
    }
    
    public void OnClickServer(object sender, EventArgs e)
    {

        GameObject obj = (GameObject)sender;
      
        if (m_serverList.ContainsKey(obj.name))
        {
            UIGameInit.GetInstance().UpdateServerInfo(m_serverList[obj.name]);

            //记录
            PlayerPrefs.SetString("XServerName", m_serverList[obj.name].zoneName);
            PlayerPrefs.SetString("XServerAddress", m_serverList[obj.name].zoneUrl);
        }

        HideWindow();

    }



}
