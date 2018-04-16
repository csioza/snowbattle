using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIRegist : UIWindow
{
    UIInput m_userName          = null;
    UIInput m_password          = null;
    UIInput m_confirmOPssword   = null;

    bool m_bUpdate              = false;

    GameObject m_checkUserNameOk = null;

    static public UIRegist GetInstance()
	{
        UIRegist self = UIManager.Singleton.GetUIWithoutLoad<UIRegist>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIRegist>("UI/UIRegiste", UIManager.Anchor.Center);
		return self;
       
	}

	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enLogin, OnPropertyChanged);

        m_userName          = FindChildComponent<UIInput>("UserName");
        m_password          = FindChildComponent<UIInput>("Password");
        m_confirmOPssword   = FindChildComponent<UIInput>("ConfirmPassword");

        m_checkUserNameOk   = FindChild("CheckOk");
        
	}
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)Login.ENPropertyChanged.enCheckUserNameOk)
        {
            bool tag = (bool)eventObj;
            m_checkUserNameOk.SetActive(tag);
        }

    }

    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("CheckUserName", OnCheck);
        AddChildMouseClickEvent("OK", OnEnter);
        AddChildMouseClickEvent("Cancel", OnReturn);

    }

    // 检查用户名是否可用
    void OnCheck(object sender, EventArgs e)
    {
        Login.Singleton.CheckUserName(m_userName.value,m_password.value);
    }

    // 进入游戏
    public void OnEnter(object sender, EventArgs e)
    {
        // 设置密码框位 显示星星状态
        //SetPassWordShowType(false);

        string username = m_userName.value;
        string password = m_password.value;

        // 检查账号相关
        Login.Singleton.CheckUserName(username, password, 1);  
    }



    // 返回 
    public void OnReturn(object sender, EventArgs e)
    {
        MainGame.Singleton.TranslateTo(new StateInitApp());
    }



    // 设置密码显示类型
    public void SetPassWordShowType(bool bShow)
    {
        // 显示密码 
        if ( bShow )
        {
            m_password.inputType        = UIInput.InputType.Standard;
            m_confirmOPssword.inputType = UIInput.InputType.Standard;
        }
        // 显示星星            
        else
        {
            m_password.inputType        = UIInput.InputType.Password;
            m_confirmOPssword.inputType = UIInput.InputType.Password;
        }
    }

    public override void OnUpdate()
    {
        if ( false == m_bUpdate )
        {
            return;
        }
 
        if ( m_password.value == m_confirmOPssword.value )
        {
            return;
        }

        m_bUpdate                   = false;
        m_password.value            = "";
        m_confirmOPssword.value     = "";

        SetPassWordShowType(false);

        
    }
}
