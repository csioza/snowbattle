using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIPopWnd : UIWindow
{
    UILabel m_cardSize  = null;
    UILabel m_maxSize   = null;

    static public UIPopWnd GetInstance()
    {
        UIPopWnd self = UIManager.Singleton.GetUIWithoutLoad<UIPopWnd>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UIPopWnd>("UI/UIPopWnd", UIManager.Anchor.Center);
        }
        return self;
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enPopWnd, OnPropertyChanged);


        m_cardSize  = FindChildComponent<UILabel>("CardSize");
        m_maxSize   = FindChildComponent<UILabel>("MaxSize");
    }
	void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)PopWnd.ENPropertyChanged.enShow)
        {
            ShowWindow();
            UpdateInfo();
        }
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("OK", OnOk);
        AddChildMouseClickEvent("Bag", OnBag);
    }

    public void OnOk( GameObject obj )
    {
        HideWindow();
        ExpandBag.Singleton.ShowExpandBag();
    }

    public void OnBag(GameObject obj)
    {
        HideWindow();
        MainMenu.Singleton.ShowMainMenu();
        MainUIManager.Singleton.HideCurWindow();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardBag);
        //CardBag.Singleton.OnShowCardBag();
        StageMenu.Singleton.OnHide();
    }

    void UpdateInfo()
    {
        m_cardSize.text = "" + CardBag.Singleton.m_cardNum;
        m_maxSize.text  = "/" + CardBag.Singleton.GetBagCapcity();
    }
}