using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIOperateLevelupBaseCard : UIWindow
{

    UILabel m_baseCapacity = null;

    static public UIOperateLevelupBaseCard Create()
    {
        UIOperateLevelupBaseCard self = UIManager.Singleton.GetUIWithoutLoad<UIOperateLevelupBaseCard>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIOperateLevelupBaseCard>("UI/UILevelUpBaseCard", UIManager.Anchor.Center);
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();

        m_baseCapacity = FindChildComponent<UILabel>("BaseCapacity");
      

    }

    public override void AttachEvent()
    {
        base.AttachEvent();

        AddChildMouseClickEvent("Cancel", OnUpdateBaseCancel);

    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (null == WindowRoot)
        {
            return;
        }
    }


    // 队伍编辑 更新
    public override void OnShowWindow()
    {
        base.OnShowWindow();

        string capacityText = "" + CardBag.Singleton.m_cardNum + "/" + CardBag.Singleton.GetBagCapcity();

        m_baseCapacity.text = capacityText;
    }
    // 选择升级 基本卡牌的 返回
    void OnUpdateBaseCancel(GameObject obj)
    {
        CardUpdateProp.Singleton.OnShowCardUpdate(CardUpdateProp.Singleton.m_curLevelGuid);
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardUpdate);
    }

     //  点击响应
    public void OnClickItem(UICardItem item, ENSortType sortType)
    {
        CardUpdateProp.Singleton.OnShowCardUpdate(item.m_param.m_guid);
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardUpdate);
    }

    public bool OnSortCondion(CSItem item)
    {

        // 如果不可强化 则不显示出来
        if (!item.IsStengthen())
        {
            return true;
        }

        return false;
    }
}
