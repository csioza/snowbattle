using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UILogin : UIWindow
{
    UIInput m_userName = null;
    UIInput m_password = null;

    static public UILogin GetInstance()
	{
        UILogin self = UIManager.Singleton.GetUIWithoutLoad<UILogin>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UILogin>("UI/UILogin", UIManager.Anchor.Center);
		return self;
	}

	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enLogin, OnPropertyChanged);

        m_userName = FindChildComponent<UIInput>("UserName");
        m_password = FindChildComponent<UIInput>("Password");

       
	}
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
       
             
    }

    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("OK", OnEnter);
        AddChildMouseClickEvent("Cancel", OnCancel);
       
    }

    public void OnEnter(GameObject obj)
    {
       
       //检查用户名与密码
        Login.Singleton.CheckLogin(m_userName.value, m_password.value);
    }

    public void OnCancel(GameObject obj)
    {
        MainGame.Singleton.TranslateTo(new StateLogin());
    }

}
