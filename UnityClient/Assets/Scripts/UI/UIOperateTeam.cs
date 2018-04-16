using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIOperateTeam : UIWindow
{

    UILabel m_teamCost  = null;

    static public UIOperateTeam Create()
    {
        UIOperateTeam self = UIManager.Singleton.GetUIWithoutLoad<UIOperateTeam>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIOperateTeam>("UI/UIOperateTeam", UIManager.Anchor.Center);
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();


        m_teamCost = FindChildComponent<UILabel>("TeamCost");

    }

    public override void AttachEvent()
    {
        base.AttachEvent();

        AddChildMouseClickEvent("TeamOK", OnTeamOK);

        AddChildMouseClickEvent("TeamCancel", OnTeamCancel);

    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (null == WindowRoot)
        {
            return;
        }
    }

    // 编辑队伍的 的确定
    void OnTeamOK(GameObject obj)
    {
        Team.Singleton.RemoveEmpty();

        Team.Singleton.UpdateTeamMember();

        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enTeam);
    }

    // 编辑队伍的 的 返回
    void OnTeamCancel(GameObject obj)
    {
        // 如果不是编辑全队
        if (Team.Singleton.m_curEditType == Team.EDITTYPE.enALL)
        {

        }
        else
        {
            // 删除
            if (OperateCardList.Singleton.m_oldTeamMember == CSItemGuid.Zero)
            {
                Team.Singleton.RemoveTeamMember(Team.Singleton.m_curTeamIndex, Team.Singleton.m_curEditType);
                Team.Singleton.RemoveEmpty();
            }
            // 更换
            else
            {
                Team.Singleton.AddTeamMember(Team.Singleton.m_curTeamIndex, Team.Singleton.m_curEditType, OperateCardList.Singleton.m_oldTeamMember);
            }
        }
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enTeam);

    }


    // 队伍编辑 更新
    public override void OnShowWindow()
    {
        base.OnShowWindow();

        // 更新 operatecardlist中的格子
        OperateCardList.Singleton.UpdateItemForTeam();

        // 编辑整个队伍
        if (Team.Singleton.m_curEditType == Team.EDITTYPE.enALL)
        {
            m_teamCost.text = Team.Singleton.GetAllCost() + "/" + User.Singleton.GetLeadership();
        }
        // 编辑队伍中的单个角色
        else
        {
            m_teamCost.text = OperateCardList.Singleton.m_leadShipCost + "/" + User.Singleton.GetLeadership();
        }
    }


    //  点击响应
    public void OnClickTeamItem(UICardItem item, ENSortType sortType)
    {
        int leadship    = 0;
        CSItem card     = null;

        // 编辑全队
        if (Team.Singleton.m_curEditType == Team.EDITTYPE.enALL)
        {
            // 要判断领导力
            // 不在编队中
            if ( item.m_param.m_id == (int)Team.EDITTYPE.enNone )
            {
                // 点选角色
                card = CardBag.Singleton.GetCardByGuid(item.m_param.m_guid);

                if (null != card)
                {
                    leadship = Team.Singleton.GetAllCost() + GameTable.HeroInfoTableAsset.Lookup(card.IDInTable).Cost;
                }
                // 如果领导力不够 则返回
                if (leadship > User.Singleton.GetLeadership())
                {
                    return;
                }

                Team.EDITTYPE type                      = Team.EDITTYPE.enNone;

                if (Team.Singleton.m_bagMainSlotId.Equals(CSItemGuid.Zero))
                {
                    Team.Singleton.m_bagMainSlotId      = item.m_param.m_guid;
                    type                                = Team.EDITTYPE.enMain;
                   
                }

                else if (Team.Singleton.m_bagDeputySlotId == CSItemGuid.Zero)
                {
                    Team.Singleton.m_bagDeputySlotId    = item.m_param.m_guid;
                    type                                = Team.EDITTYPE.enDeputy;
                }

                else if (Team.Singleton.m_bagSupportSlotId == CSItemGuid.Zero)
                {
                    Team.Singleton.m_bagSupportSlotId   = item.m_param.m_guid;
                    type                                = Team.EDITTYPE.enSupport;
                }

                item.m_param.m_id                       = (int)type;

                item.UpdateOperateTeam(card, sortType, type, item.m_index);
            }
            // 在编队中
            else
            {
                if (item.m_param.m_id == (int)Team.EDITTYPE.enMain)
                {
                    Team.Singleton.m_bagMainSlotId      = CSItemGuid.Zero;
                    item.m_param.m_id                   = (int)Team.EDITTYPE.enNone;
                }
                else if (item.m_param.m_id == (int)Team.EDITTYPE.enDeputy)
                {
                    Team.Singleton.m_bagDeputySlotId    = CSItemGuid.Zero;
                    item.m_param.m_id                   = (int)Team.EDITTYPE.enNone;
                }
                else if (item.m_param.m_id == (int)Team.EDITTYPE.enSupport)
                {
                    Team.Singleton.m_bagSupportSlotId   = CSItemGuid.Zero;
                    item.m_param.m_id                   = (int)Team.EDITTYPE.enNone;
                }

                item.UpdateOperateTeam(card, sortType,Team.EDITTYPE.enNone, item.m_index);
            }

        }
        // 编辑单个队伍
        else
        {
            // 如果是移除标志位 则移除当前 队伍成员
            if (item.m_param.m_id == -1)
            {
                Team.Singleton.RemoveTeamMember(Team.Singleton.m_curTeamIndex, Team.Singleton.m_curEditType);
                Team.Singleton.UpdateTeamBagAllSlotId();
                return;
            }

            // 如果是当前队伍中的队伍成员 则返回
            if (Team.Singleton.IsCardInTheTeam(Team.Singleton.m_curTeamIndex, item.m_param.m_guid))
            {
                return;
            }

            card = CardBag.Singleton.GetCardByGuid(item.m_param.m_guid);
            HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);

            if (null == heroInfo)
            {
                Debug.LogWarning("null == heroInfo card.IDInTable:" + card.IDInTable);
                return;
            }


            // 将要被替换角色的领导力
            CSItem replaceCard = Team.Singleton.GetCard(Team.Singleton.m_curTeamIndex, Team.Singleton.m_curEditType);
            int replaceCost = 0;
            if (null != replaceCard)
            {
                replaceCost = GameTable.HeroInfoTableAsset.Lookup(replaceCard.IDInTable).Cost;
            }
            // 要判断领导力

            // 如果领导力不够 则返回
            if (OperateCardList.Singleton.m_leadShipCost - replaceCost + heroInfo.Cost > User.Singleton.GetLeadership())
            {
                return;
            }

            // 添加
            if (item.m_param.m_id != -1)
            {
                Team.Singleton.AddTeamMember(Team.Singleton.m_curTeamIndex, Team.Singleton.m_curEditType, item.m_param.m_guid);
            }

            Team.Singleton.UpdateTeamBagAllSlotId();
        }


    }

    // 指定卡牌是否 被 排除 true 排除， false 不排除
    public bool OnSortCondion(CSItem item)
    {
        // 编辑全队伍 类型
        if (Team.Singleton.m_curEditType == Team.EDITTYPE.enALL)
        {
            // 已在队伍中 则不显示
            if (OperateCardList.Singleton.m_hadItemList.Contains(item.Guid))
            {
                return true;
            }
        }
        // 编辑队伍单个成员
        else
        {
            // 已在队伍中 则不显示
            if (OperateCardList.Singleton.m_hadItemList.Contains(item.Guid))
            {
                return true;
            }
        }

        return false;
    }
}
