using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIGuild : UIWindow
{
    static public UIGuild GetInstance()
    {
        UIGuild self = UIManager.Singleton.GetUIWithoutLoad<UIGuild>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIGuild>("UI/UIGuild", UIManager.Anchor.Center);
        return self;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enGuild, OnPropertyChanged);
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {

        if (eventType == (int)Guild.ENPropertyChanged.enShow)
        {
//             if (MainUIManager.Singleton.ChangeUI(this))
//             {
//                 MainButtonList.Singleton.m_curShowType = MainButtonList.SHOWWNDTYPE.ENGroup;
//                 ShowWindow();
//             }
        }

        else if (eventType == (int)Guild.ENPropertyChanged.enHide)
        {
            HideWindow();
        }

    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("Button_Friend", OnFriendBtn);
        AddChildMouseClickEvent("Button_Back", OnBackBtn);
    }

    public override void OnHideWindow()
    {
        base.OnHideWindow();
    }
    public override void OnShowWindow()
    {
        base.OnShowWindow();
        MainButtonList.Singleton.m_curShowType = MainButtonList.SHOWWNDTYPE.ENGroup;
    }

    // 点击好友按钮
    public void OnFriendBtn(GameObject obj)
    {
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enFriend);
        //Friend.Singleton.OnShow();
    }
    public void OnBackBtn(GameObject obj)
    {
        //Zone.Singleton.ShowZone();
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enZone);
    }
}
