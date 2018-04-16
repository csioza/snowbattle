using System;
using System.Collections.Generic;

// 二次确认
public class RUSure : IPropertyObject
{
    public enum ENPropertyChanged
    {
        enShow = 1,
        enShowBase = 2,
    }

    public string m_text = "";
    public string m_titelText = "";
    public int m_iconId = 0;
    public string m_useText = "";//还有XXX的XXX可用

    public RUSure()
    {
        SetPropertyObjectID((int)MVCPropertyID.enRUSure);

    }

    #region Singleton
    static RUSure m_singleton;
    static public RUSure Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new RUSure();
            }
            return m_singleton;
        }
    }
    #endregion

    public void Show(string str, UIRUSure.OnButtonCallbacked callback)
    {
        m_text = str;
        NotifyChanged((int)ENPropertyChanged.enShow, callback);
    }

    // 联网联失败
    public void ClientNetConnectFailed()
    {
        Show(Localization.Get("ConnectionFailed"), Reconnect);
    }

    // 重联
    void Reconnect()
    {
        User.Singleton.UserLogin();
    }

    //显示二次确认框
    public void ShowPanelText(string text,string titelText, string useText,int iconId) 
    {
        m_text = text;
        m_titelText = titelText;
        m_useText = useText;
        m_iconId = iconId;
        NotifyChanged((int)ENPropertyChanged.enShowBase,null);
    }
}