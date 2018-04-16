using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIDebugLog : UIWindow
{

    UIToggle m_toggle        = null;
    UILabel m_content = null;

    static public UIDebugLog Singleton
    {
        get
        {
            UIDebugLog self = UIManager.Singleton.GetUIWithoutLoad<UIDebugLog>();
            if (self == null)
            {
                self = UIManager.Singleton.LoadUI<UIDebugLog>("UI/UIDebugLog", UIManager.Anchor.Center);
            }
            return self;
        }
    }

    static public UIDebugLog GetInstance()
    {
        UIDebugLog self = UIManager.Singleton.GetUIWithoutLoad<UIDebugLog>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UIDebugLog>("UI/UIDebugLog", UIManager.Anchor.Center);
        }
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();

        m_toggle    = FindChildComponent<UIToggle>("Toggle");
        m_content   = FindChildComponent<UILabel>("content"); 
        AddPropChangedNotify((int)MVCPropertyID.enDebugLog, OnPropertyChanged);
        EventDelegate.Add(m_toggle.onChange, OnChange);

	}

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)DebugLog.ENPropertyChanged.enShow)
        {
            m_content.text = m_content.text + DebugLog.Singleton.m_log;

            if (PlayerPrefs.GetInt("ForbidenDebugLogTag") == 0)
            {
                ShowWindow();
            }
           
        }
    }

	public override void AttachEvent()
	{
        base.AttachEvent();
        AddChildMouseClickEvent("Close", OnButtonCloseClicked);
		
	}

    public void OnButtonCloseClicked(object sender, EventArgs e)
    {
        HideWindow();
    }

    void OnChange()
    {
        if (UIToggle.current.value )
        {
            PlayerPrefs.SetInt("ForbidenDebugLogTag", 1);
        }
        else
        {
            PlayerPrefs.SetInt("ForbidenDebugLogTag", 0);
        }
        PlayerPrefs.Save();
    }
   
}
