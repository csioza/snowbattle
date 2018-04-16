using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIDebugStartUp : UIWindow
{

    // 网址输入框
    UIInput m_website   = null;
    // 下载按钮
    //UIButton m_downLoad = null;
    // 进入游戏按钮
    //UIButton m_enter    = null;
    // 离线进入游戏按钮
    //UIButton m_offLineEnter = null;

    UIButton m_onlineEnter = null;

    UIInput m_ip        = null;

    UIPopupList m_popupListSelect = null;
    //!表示是否可以进入正式游戏状态了
    public bool CanProcessMainGame = false;

    static public UIDebugStartUp GetInstance()
	{
        UIDebugStartUp self = UIManager.Singleton.GetUIWithoutLoad<UIDebugStartUp>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIDebugStartUp>("UI/UIDebugStartUp", UIManager.Anchor.Center);
		return self;
	}

	public override void OnInit ()
	{
		base.OnInit ();

        m_website       = FindChildComponent<UIInput>("WebSiteInput");
//        m_downLoad      = FindChildComponent<UIButton>("DownLoad");
//        m_enter         = FindChildComponent<UIButton>("Enter");
//        m_offLineEnter  = FindChildComponent<UIButton>("OfflineEnter");
        m_ip            = FindChildComponent<UIInput>("IPInput");
        m_onlineEnter   = FindChildComponent<UIButton>("EnterOnLine");

        m_website.defaultText = "请输入下载地址网址";

        if (PlayerPrefs.HasKey("DownloadWebSite"))
        {
            m_website.value = PlayerPrefs.GetString("DownloadWebSite");
        }
        else
        {
            m_website.value = "http://115.28.220.134:8083/xproject/Publish/";
        }

        if (PlayerPrefs.HasKey("OnLineIP"))
        {
            m_ip.value = PlayerPrefs.GetString("OnLineIP");
        }
        else
        {
            m_ip.value = "192.168.11.143";
        }

        bool bSingle = GameSettings.Singleton.m_isSingle;
    
        
        //m_ip.gameObject.SetActive(!bSingle);

        //m_onlineEnter.gameObject.SetActive(!bSingle);

        m_popupListSelect = FindChild("Popup List").GetComponent<UIPopupList>();

        EventDelegate.Add(m_popupListSelect.onChange, OnPopupListChanged);
	}
          
    
	
    public void OnPopupListChanged()
    {
        //Debug.Log("UIPopupList.current.value:" + UIPopupList.current.value);
        m_ip.value = UIPopupList.current.value;

        string ipStr = UIPopupList.current.value;
        ipStr =   ipStr.Replace("(外网)", "");
        ipStr =  ipStr.Replace("(内网)", "");
        m_ip.value = ipStr;
        
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {

    }

    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("Enter", OnEnter);
        AddChildMouseClickEvent("DownLoad", OnDownLoad);
        AddChildMouseClickEvent("OfflineEnter", OnOffLineEnter);
        AddChildMouseClickEvent("EnterOnLine", OnEnterOnLine);
    }

    // 进入游戏
    public void OnEnter(object sender)
    {
        //User.Singleton.UserLogin();
        GameSettings.Singleton.ServerIP = m_ip.value;
        GameResManager.Singleton.UsePackage = true;
        GameResManager.Singleton.BuildGameResMap();
        GameResManager.Singleton.PreLoad();

        GameSettings.Singleton.m_isSingle = false;
        CanProcessMainGame = true;
        //MainGame.Singleton.TranslateTo(new StateLogin());
        HideWindow();
    }

    // 连网进入游戏
    public void OnEnterOnLine(object sender)
    {
        GameSettings.Singleton.ServerIP     = m_ip.value;

        PlayerPrefs.SetString("OnLineIP", m_ip.value);
        PlayerPrefs.Save();


        GameSettings.Singleton.m_isSingle   = false;
        CanProcessMainGame = true;
        GameResManager.Singleton.m_isPreLoadFinish = true;
        //MainGame.Singleton.TranslateTo(new StateLogin());
        HideWindow();
    }
    void OnCheckUpdateResult(bool haveUpdate)
    {
        Debug.Log("Update:" + haveUpdate);
        if (haveUpdate)
        {
            GameResManager.Singleton.DownloadUpdatePackage();
        }
    }
    // 下载
    public void OnDownLoad(object sender)
    {
        if (m_website.value !="")
        {
            PlayerPrefs.SetString("DownloadWebSite", m_website.value);
            PlayerPrefs.Save();
            GameResManager.Singleton.CheckUpdate(m_website.value, OnCheckUpdateResult);
            //ResPath.ServerResourceUrl = m_website.value;
            //GameResMng.ForcePackage = true;
            //GameResMng.GetResMng().StartWWW(false);
        }
    }

    public override void OnUpdate()
    {
    }

    // 离线进入游戏
    public void OnOffLineEnter(object sender)
    {
        GameSettings.Singleton.m_isSingle = true;
        CanProcessMainGame = true;
        GameResManager.Singleton.m_isPreLoadFinish = true;

        MainGame.Singleton.TranslateTo(new StateMainUI());

        //User.Singleton.HelperList.Test();
        HideWindow();
    }
}
