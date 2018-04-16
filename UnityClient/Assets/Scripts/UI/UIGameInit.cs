using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIGameInit : UIWindow
{
    UILabel m_guid              = null;
    UILabel m_version           = null;


    static public UIGameInit GetInstance()
	{
        UIGameInit self = UIManager.Singleton.GetUIWithoutLoad<UIGameInit>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIGameInit>("UI/UIGameInit", UIManager.Anchor.Center);
		return self;
	}
	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enMainPlayer, OnPropertyChanged);
        AddPropChangedNotify((int)MVCPropertyID.enUserProps, OnPropertyClientNetChanged);

        m_guid                  = FindChildComponent<UILabel>("Guid");
        m_version               = FindChildComponent<UILabel>("Version");

        this.WindowRoot.GetComponent<Animation>().Play("ui-gameinit-00");

        m_guid.text     = PlayerPrefs.GetInt("User.Singleton.Guid").ToString();
        m_version.text  = GameSettings.Singleton.m_version;

	}
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {

    }

    void OnPropertyClientNetChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        // 人物属性接受完毕
        if (eventType == (int)User.ENPropertyChanged.enOnServerProps)
        {
            MainGame.Singleton.TranslateTo(new StateMainUI());
            HideWindow();
        }
    }

    public override void AttachEvent()
    {
        base.AttachEvent();
        //AddChildMouseClickEvent("Enter", OnEnter);
       // AddChildMouseClickEvent("ChangeAccout", OnChangeAccout);
       // AddChildMouseClickEvent("Server", OnChooseServer);
        AddChildMouseClickEvent("GameStart", OnEnter);
    }

    // 进入游戏
    public void OnEnter(GameObject obj)
    {
        //PlayerPrefs.GetString("UserName");

        User.Singleton.UserLogin();
        HideWindow();
        return;

        // 如果有记录
        if (PlayerPrefs.HasKey("XUserName"))
        {
            // 登陆
            //User.Singleton.UserLogin();

            MiniServer.Singleton.user_login(PlayerPrefs.GetString("XUserName"), PlayerPrefs.GetString("XPassword"));

            Loading.Singleton.SetLoadingTips();
        }
        // 否则 则进入注册或者用账号登陆界面
        else
        {
            UIGameStart.GetInstance().ShowWindow();
        }

        HideWindow();
        
    }

    // 切换账号
    public void OnChangeAccout(object sender, EventArgs e)
    {
        HideWindow();
    }

    // 选择服务器 
    public void OnChooseServer(object sender, EventArgs e)
    {
     
    }

    public void UpdateServerInfo( ZoneInfo info )
    {
        HTTPSetting.ChangeWebsite(info.zoneUrl);
        ShowWindow();
    }
}
