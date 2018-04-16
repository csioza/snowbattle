using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIRUSure : UIWindow
{
    UILabel m_titelText = null;
    UILabel m_text = null;
    UILabel m_useText = null;//还有多少XX可以用
    UITexture m_icon = null;

    public delegate void OnButtonCallbacked();

    OnButtonCallbacked m_yesCallbacked;

    static public UIRUSure GetInstance()
    {
        UIRUSure self = UIManager.Singleton.GetUIWithoutLoad<UIRUSure>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UIRUSure>("UI/UIRUSure", UIManager.Anchor.Center);
        }
        return self;
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enRUSure, OnPropertyChanged);

        m_text   = FindChildComponent<UILabel>("Text");
        m_titelText = FindChildComponent<UILabel>("Title");
        m_icon = FindChildComponent<UITexture>("Logo");
        m_useText = FindChildComponent<UILabel>("hint");
    }
	void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)//heizTest
    {
        if (eventType == (int)RUSure.ENPropertyChanged.enShow)
        {
            m_yesCallbacked = (OnButtonCallbacked)eventObj;
            ShowWindow();
            UpdateInfo();
        }
        else if (eventType == (int)RUSure.ENPropertyChanged.enShowBase)
        {
            ShowWindow();
            UpdateBaseInfo();
            //关闭转菊花loading界面
            Loading.Singleton.Hide();
        }
    }

    void OnPropertyClinetNetChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
       
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("OK", OnOk);
    }

    public void OnOk( GameObject obj )
    {
        if (null != m_yesCallbacked)
        {
            m_yesCallbacked();
        }
       
        HideWindow();
    }

    void UpdateInfo()
    {
        m_text.text = RUSure.Singleton.m_text;
    }

    void UpdateBaseInfo() 
    {
        m_text.text = RUSure.Singleton.m_text;
        m_titelText.text = RUSure.Singleton.m_titelText;
        IconInfomation info = GameTable.IconInfoTableAsset.Lookup(RUSure.Singleton.m_iconId);
        m_icon.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(info.dirName);
        m_useText.text = RUSure.Singleton.m_useText;
    }
}