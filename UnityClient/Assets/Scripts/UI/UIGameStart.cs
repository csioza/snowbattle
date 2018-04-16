using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIGameStart : UIWindow
{
    UILogin m_login     = null;
    UIRegist m_regist   = null;
    GameObject m_gameStart = null;

    static public UIGameStart GetInstance()
	{
        UIGameStart self = UIManager.Singleton.GetUIWithoutLoad<UIGameStart>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIGameStart>("UI/UIGameStart", UIManager.Anchor.Center);
		return self;
	}

	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enLogin, OnPropertyChanged);

        m_gameStart = FindChild("GameStart");
        m_login     = UILogin.GetInstance();
        m_regist    = UIRegist.GetInstance();

        m_login.SetParent(WindowRoot);
        m_regist.SetParent(WindowRoot);

        m_login.HideWindow();
        m_regist.HideWindow();
       
	}
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)Login.ENPropertyChanged.enLoginHide)
        {
            HideWindow();
        }
        else if (eventType == (int)Login.ENPropertyChanged.enShowLogin)
        {
            ShowWindow();
        }
             
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_login.Destroy();
        m_regist.Destroy();
            
    }

    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("Registe", OnRegiste);
        AddChildMouseClickEvent("Login", OnLogin);
       
    }

    public override void OnShowWindow()
    {
        base.OnShowWindow();
        m_regist.HideWindow();
        m_login.HideWindow();
        m_gameStart.SetActive(true);
    }


    // 注册
    public void OnRegiste(GameObject obj)
    {
        m_regist.ShowWindow();
        m_gameStart.SetActive(false);

    }

    public void OnLogin(GameObject obj)
    {

        m_login.ShowWindow();
        m_gameStart.SetActive(false);
    }

    
}
