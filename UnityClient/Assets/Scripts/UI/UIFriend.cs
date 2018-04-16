using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIFriend : UIWindow
{
    static public UIFriend GetInstance()
    {
        UIFriend self = UIManager.Singleton.GetUIWithoutLoad<UIFriend>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIFriend>("UI/UIFriend", UIManager.Anchor.Center);
        return self;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enFriend, OnPropertyChanged);
    }
    public override void OnShowWindow()
    {

    }
    public override void OnHideWindow()
    {

    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {

        if (eventType == (int)Guild.ENPropertyChanged.enShow)
        {
//             if (MainUIManager.Singleton.ChangeUI(this))
//             {
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
        AddChildMouseClickEvent("Button_Back", OnBackBtn);
        AddChildMouseClickEvent("Button_FoundID", OnFoundByIDBtn);
        AddChildMouseClickEvent("Button_FriendList", OnFriendListBtn);
        AddChildMouseClickEvent("Button_Sure", OnFriendSureBtn);
    }

    public void OnFriendListBtn(GameObject obj)
    {
        FriendList.Singleton.m_friendListType = FriendList.EDITTYPE.enFriendList;
        FriendList.Singleton.OnShow();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enFriendList);
    }
    public void OnFoundByIDBtn(GameObject obj)
    {
        FoundFriendByID.Singleton.OnShow();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enFoundFriendByID);
    }
    public void OnFriendSureBtn(GameObject obj)
    {
        ApplyList.Singleton.OnShow();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enApplyList);
    }
    // 点击返回按钮
    public void OnBackBtn(GameObject obj)
    {
        //Guild.Singleton.OnShow();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enGuild);
    }
}
