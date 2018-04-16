using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIGMPanel : UIWindow
{

    UIInput m_uiInput = null;
    string m_strCmd = "";
    static public UIGMPanel GetInstance()
	{
        UIGMPanel self = UIManager.Singleton.GetUIWithoutLoad<UIGMPanel>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIGMPanel>("UI/UIGMPanel", UIManager.Anchor.Center);
		return self;
	}
	public override void OnInit ()
	{
		base.OnInit ();
        m_uiInput = FindChildComponent<UIInput>("InputCmd");
        EventDelegate.Add(m_uiInput.onChange, OnInputChange);
	}

    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("SendBtn", OnSend);
    }
    public void OnInputChange()
    {
        m_strCmd = m_uiInput.value;
    }
    // 进入游戏
    public void OnSend(GameObject obj)
    {
        //
        MiniServer.Singleton.SendGMCmd(m_strCmd);
        HideWindow();
        
    }
}
