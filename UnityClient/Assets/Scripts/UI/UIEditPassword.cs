using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIEditPassword : UIWindow
{

    UIInput m_userName          = null;
    UIInput m_oldPassword       = null;
    UIInput m_newPassword       = null;
    UIInput m_confirmNewPassword= null;

    UILabel m_tips              = null;

    static public UIEditPassword GetInstance()
	{
        UIEditPassword self = UIManager.Singleton.GetUIWithoutLoad<UIEditPassword>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIEditPassword>("UI/UIEditPassword", UIManager.Anchor.Center);
		return self;
	}
	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enMainPlayer, OnPropertyChanged);

        m_userName              = FindChildComponent<UIInput>("UserName");
        m_oldPassword           = FindChildComponent<UIInput>("OldPassword");
        m_newPassword           = FindChildComponent<UIInput>("NewPassword");
        m_confirmNewPassword    = FindChildComponent<UIInput>("ConfirmNewPassword");

        m_tips                  = FindChildComponent<UILabel>("Tips");
	}
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)Login.ENPropertyChanged.enChangePWD)
        {
            int respond = (int)eventObj;
            OnEditResult(respond);
        }
    }

    public override void AttachEvent()
    {
        base.AttachEvent();

        AddChildMouseClickEvent("Enter", OnEdit);
        AddChildMouseClickEvent("Return", OnReturn);
    }

    // 确认修改密码
    public void OnEdit(object sender, EventArgs e)
    {
        if (m_userName.value == "" || m_oldPassword.value == "" || m_newPassword.value == "" || m_confirmNewPassword.value == "")
        {
            Debug.Log("您输入的账号或者密码为空，请重新输入");
            m_tips.text = "您输入的账号或者密码为空，请重新输入";
            return;
        }

        if ( m_newPassword.value != m_confirmNewPassword.value )
        {
            Debug.Log("新密码 两次输入不一样 ，请重新输入");
            m_tips.text                 = "新密码 两次输入不一样 ，请重新输入";
            m_newPassword.value         = "";
            m_confirmNewPassword.value  = "";
            return;
        }

        if ( m_oldPassword.value ==  m_newPassword.value  )
        {
            Debug.Log("新密码和老密码不能一样，请重新输入");
            m_tips.text                 = "新密码和老密码不能一样，请重新输入";
            m_newPassword.value         = "";
            m_confirmNewPassword.value  = "";
            m_oldPassword.value         = "";
            return;
        }

        // 发送服务器 校验
        IMiniServer.Singleton.account_change_pwd(m_userName.value, m_oldPassword.value, m_newPassword.value);
    }

    // 返回
    public void OnReturn(object sender, EventArgs e)
    {
        HideWindow();
        UIGameStart.GetInstance().ShowWindow();
    }

    public void OnShow()
    {
        m_newPassword.value         = "";
        m_confirmNewPassword.value  = "";
        m_userName.value            = "";
        m_oldPassword.value         = "";  
        m_tips.text                 = "";
        ShowWindow();

    }

    // 发送服务器校验后的回调
    public void OnEditResult(int respond )
    {
		if (respond == 1)
        {
            Debug.Log("修改密码成功");
            m_tips.text                     = "修改密码成功";
            m_newPassword.value             = "";
            m_confirmNewPassword.value      = "";
            m_oldPassword.value             = "";

        }
        else
        {
            Debug.Log("修改密码失败，无效的账户名或密码");
            m_tips.text                     = "修改密码失败，无效的账户名或密码";
        }

    }


}
