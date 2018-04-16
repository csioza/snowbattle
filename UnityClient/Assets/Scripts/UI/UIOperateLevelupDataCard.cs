using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIOperateLevelupDataCard : UIWindow
{

    UILabel m_dataCapacity  = null;

    UILabel m_dataOKText    = null;

    UIButton m_dataOKBtn    = null;
    
    static public UIOperateLevelupDataCard Create()
    {
        UIOperateLevelupDataCard self = UIManager.Singleton.GetUIWithoutLoad<UIOperateLevelupDataCard>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIOperateLevelupDataCard>("UI/UILevelUpDataCard", UIManager.Anchor.Center);
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();

        m_dataCapacity  = FindChildComponent<UILabel>("DataCapacity");

        m_dataOKText    = FindChildComponent<UILabel>("UpdateDataOKText");

        m_dataOKBtn     = FindChildComponent<UIButton>("UpdateDataOK");
    }

    public override void AttachEvent()
    {
        base.AttachEvent();


        AddChildMouseClickEvent("UpdateDataOK", OnUpdateDataOK);

        AddChildMouseClickEvent("UpdateDataCancel", OnUpdateDataCancel);

    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (null == WindowRoot)
        {
            return;
        }
    }

    // 升级的 材料卡 确认
    void OnUpdateDataOK(GameObject obj)
    {
        CardUpdateProp.Singleton.OnShowCardUpdate(CardUpdateProp.Singleton.m_curLevelGuid);
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardUpdate);
    }

    // 升级的 材料卡 取消
    void OnUpdateDataCancel(GameObject obj)
    {
        CardUpdateProp.Singleton.OnShowCardUpdate(CardUpdateProp.Singleton.m_curLevelGuid);
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardUpdate);
    }



    // 队伍编辑 更新
    public override void OnShowWindow()
    {
        base.OnShowWindow();

        OperateCardList.Singleton.m_chosenCardNum = 0;

        string capacityText                         = "" + CardBag.Singleton.m_cardNum + "/" + CardBag.Singleton.GetBagCapcity();
        OperateCardList.Singleton.m_chosenCardNum   = OperateCardList.Singleton.LevelUpList.Count;
        m_dataCapacity.text                         = capacityText;

        if (OperateCardList.Singleton.m_chosenCardNum == 0)
        {
            m_dataOKText.text = Localization.Get("Confirm");
            // 当没有选择任何时 确定按钮 灰掉
            m_dataOKBtn.isEnabled = false;
        }
        else
        {
            m_dataOKText.text = string.Format(Localization.Get("ConfirmNum"), OperateCardList.Singleton.m_chosenCardNum);
            // 当有选择时 确定按钮 激活
            m_dataOKBtn.isEnabled = true;
        }

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
        OperateCardList.Singleton.LevelUpItemOperation(item.m_param.m_guid);
    }

    public bool OnSortCondion(CSItem item)
    {

        // 收藏或者在编队中 则不显示出来
        if (item.Love || Team.Singleton.IsCardInTeam(item.Guid))
        {
            return true;
        }
        // 基本卡牌自己不会出现
        if (item.Guid.Equals(CardUpdateProp.Singleton.m_curLevelGuid))
        {
            return true;
        }

        return false;
    }
}
