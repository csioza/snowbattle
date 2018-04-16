using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIOperateSell : UIWindow
{

    UILabel m_price         = null;

    UILabel m_ring          = null;

    UILabel m_sellBtnText   = null;

    UIButton m_sellBtn      = null;

    UILabel m_capacity = null;



    static public UIOperateSell Create()
    {
        UIOperateSell self = UIManager.Singleton.GetUIWithoutLoad<UIOperateSell>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIOperateSell>("UI/UIOperateSell", UIManager.Anchor.Center);
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();


        m_sellBtnText   = FindChildComponent<UILabel>("SellBtnText");

        m_sellBtn       = FindChildComponent<UIButton>("SellBtn");

        m_price         = FindChildComponent<UILabel>("Price");

        m_ring          = FindChildComponent<UILabel>("Ring");

        m_capacity      = FindChildComponent<UILabel>("Capacity");

    }

    public override void AttachEvent()
    {
        base.AttachEvent();


        AddChildMouseClickEvent("SellCancel", OnSellCancel);

        AddChildMouseClickEvent("SellBtn", OnSell);

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

        OperateCardList.Singleton.m_chosenCardNum = OperateCardList.Singleton.m_sellList.Count;

        m_capacity.text = capacityText;

        if (OperateCardList.Singleton.m_chosenCardNum == 0)
        {
            m_sellBtnText.text = Localization.Get("Sell");
            // 当没有选择任何时 确定按钮 灰掉
            m_sellBtn.isEnabled = false;
        }
        else
        {
            m_sellBtnText.text = string.Format(Localization.Get("SellNum"), OperateCardList.Singleton.m_chosenCardNum);
            // 当有选择时 确定按钮 激活
            m_sellBtn.isEnabled = true;
        }
        int price   = 0;
        int ring    = 0;

        for (int i = 0; i < OperateCardList.Singleton.m_sellList.Count;i++ )
        {
            CSItem item = CardBag.Singleton.GetCardByGuid(OperateCardList.Singleton.m_sellList[i]);
            if (null == item)
            {
                continue;
            }

            int cardID      = item.IDInTable;

            HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(cardID);

            if (null == heroInfo)
            {
                continue;
            }

            price   = price + heroInfo.Price;
            ring    = ring + heroInfo.Ring;
        }

        m_price.text    = "" + price;
        m_ring.text     = "" + ring;
    }

    // 出售 的返回
    void OnSellCancel(GameObject obj)
    {
        OperateCardList.Singleton.m_sellList.Clear();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardBag);
    }

    // 出售 响应
    void OnSell(GameObject obj)
    {
        UIOperaterCardList.GetInstance().OnShowRUSure();
    }

     //  点击响应
    public void OnClickItem(UICardItem item, ENSortType sortType)
    {
        OperateCardList.Singleton.SellItemOperation(item.m_param.m_guid);
    }

    public bool OnSortCondion(CSItem item)
    {

        // 收藏或者在编队中 则不显示出来
        if (item.Love || Team.Singleton.IsCardInTeam(item.Guid))
        {
            return true;
        }

        return false;
    }
}
