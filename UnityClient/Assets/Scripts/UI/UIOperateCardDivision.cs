using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIOperateCardDivision : UIWindow
{
    static public UIOperateCardDivision Create()
    {
        UIOperateCardDivision self = UIManager.Singleton.GetUIWithoutLoad<UIOperateCardDivision>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIOperateCardDivision>("UI/UIOperateCardDivision", UIManager.Anchor.Center);
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();
    }

    public override void AttachEvent()
    {
        base.AttachEvent();


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
    }

  
     //  点击响应
    public void OnClickItem(UICardItem item, ENSortType sortType)
    {
       
    }

    public bool OnSortCondion(CSItem item)
    {

        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(item.IDInTable);

        if (null == heroInfo)
        {
            Debug.LogWarning("null == heroInfo item.IDInTable:" + item.IDInTable);
            return true;
        }

        // 收藏或者在编队中或者是代表卡 则不显示出来
        if (item.Love || Team.Singleton.IsCardInTeam(item.Guid)
            || User.Singleton.RepresentativeCard == item.Guid)
        {
            return true;
        }
        int occ = OperateCardList.Singleton.m_cardDivisionOcc;
        int level = OperateCardList.Singleton.m_cardDivisionLevel;
        int star = OperateCardList.Singleton.m_cardDivisionStar;
        if (heroInfo.Occupation == occ && item.Level == level && star == heroInfo.Rarity)
        {
            return false;
        }
        return true;
    }

    // 进化的基本卡牌 取消
    void OnEvolutionDataCancel(GameObject obj)
    {
        CardEvolution.Singleton.OnShowCardEvolution(CardEvolution.Singleton.m_curEvolutionGuid);
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardEvolution);
    }


    //选择升段素材的确定
    void OnDicisionOKButton(GameObject obj)
    {
        HideWindow();
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardDivisionUpdate);
        CSItem card = CardBag.Singleton.GetCardByGuid(OperateCardList.Singleton.m_curChooseItemGuid);
        if (null == card)
        {
            return;
        }
        CardDivisionUpdateProp.Singleton.ChooseCard(card, OperateCardList.Singleton.m_cardDivisionMateriaId);
    }
    //选择升段素材的取消
    void OnDivisionExitButton(GameObject obj)
    {
        HideWindow();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardDivisionUpdate);
    }
}
