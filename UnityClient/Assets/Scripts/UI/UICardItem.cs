using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UICardItem : UIWindow
{

    UITexture m_headImage   = null;
    public Parma m_param           = null;
    UILabel m_level         = null;

    GameObject m_chosen     = null;

    UISprite m_love         = null;
    UISprite m_defaultHead  = null;
    UITexture m_occ         = null;
    UITexture m_rarity      = null;
    UISprite m_tag          = null;
    GameObject m_shadow     = null;
    UILabel m_break         = null;
    GameObject m_tips       = null;
    GameObject m_mask       = null;


    public delegate void OnClickCallbacked(object sender, EventArgs e);
    public delegate void OnPressCallbacked(object sender, EventArgs e);

    OnClickCallbacked m_clickCallBack = null;
    OnPressCallbacked m_pressCallBack = null;


    GameObject m_bg = null;

    public int m_index = 0; // 用来分辨在显示列表中的索引位置 用来判断是否是可以显示卡牌详情的标志 >0为 可以显示卡牌详情

    static public UICardItem Create()
    {
        UICardItem self = UIManager.Singleton.LoadUI<UICardItem>("UI/UICardItem", UIManager.Anchor.Center);
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();

        m_headImage = FindChildComponent<UITexture>("CardHead");

        m_param     = FindChildComponent<Parma>("DragCardItemBG");

        m_level     = FindChildComponent<UILabel>("Level");

        m_chosen = FindChild("Chosen");

        m_love = FindChildComponent<UISprite>("Love");

        m_defaultHead = FindChildComponent<UISprite>("defaultHead");

        m_occ = FindChildComponent<UITexture>("Occupation");

        m_rarity = FindChildComponent<UITexture>("Rarity");

        m_bg = FindChild("DragCardItemBG");

        m_tag = FindChildComponent<UISprite>("tag");

        m_shadow = FindChild("Shadow");

        m_break = FindChildComponent<UILabel>("Break");

        m_tips = FindChild("Tips");

        m_mask = FindChild("mask");
    }

    public override void AttachEvent()
    {
        base.AttachEvent();

        AddChildMouseClickEvent("DragCardItemBG", OnClickItem);

        AddChildMouseLongPressEvent("DragCardItemBG", OnClickLongPressItem);
    }
    public override void OnShowWindow()
    {
        base.OnShowWindow();
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (null == WindowRoot)
        {
            return;
        }
    }

    public void SetParentWnd(GameObject parent)
    {
        WindowRoot.transform.parent = parent.transform;
    }

    public void SetClickCallback(OnClickCallbacked action)
    {
        m_clickCallBack = action;
    }

    public void SetPressCallback(OnPressCallbacked action)
    {
        m_pressCallBack = action;
    }


    // 长按 响应
    void OnClickLongPressItem(object sender, EventArgs e)
    {
        CardBag.Singleton.m_curOptinGuid    = m_param.m_guid;
        CardBag.Singleton.m_curOptinIndex   = m_index;

        if (m_pressCallBack != null)
        {
            m_pressCallBack(sender,e);
            return;
        }

        if (m_param.m_guid.Equals(CSItemGuid.Zero))
        {
            return;
        }

        // 扩容
        if (-1 == m_param.m_id)
        {
        }
        else
        {
            CardBag.Singleton.m_curOptinGuid = m_param.m_guid;
            CardBag.Singleton.OnShowCardDetail();
        }
    }

    // 点击卡牌响应
    void OnClickItem(object sender, EventArgs e)
    {
        CardBag.Singleton.m_curOptinGuid    = m_param.m_guid;
        CardBag.Singleton.m_curOptinIndex   = m_index;

        Debug.Log("uicarditem OnClickItem:" + m_index+","+WindowRoot.name);
        if (m_clickCallBack != null)
        {
            m_clickCallBack(sender,e);
            return;
        }
        // 扩容
        if (-1 == m_param.m_id)
        {
            ExpandBag.Singleton.ShowExpandBag();
        }
        else
        {
            if (!m_param.m_guid.Equals(CSItemGuid.Zero))
            {
                //m_seleteCardObj = obj;
//                 Transform seleteItem = obj.transform.FindChild("Item");
//                 if (null != seleteItem)
//                 {
//                     Transform seleteTra = seleteItem.transform.FindChild("cardSelect");
//                     if (null != seleteTra)
//                     {
//                         seleteTra.gameObject.SetActive(true);
//                     }
//                 }
                CardBag.Singleton.OnShowCardOption(m_param.m_guid);
            }
        }

    }


    public void Update(CSItem card, ENSortType sortType,int index =0)
    {
        if (null == card)
        {
            return;
        }

        ShowWindow();

        m_index     = index; // 表示 用显示卡牌详情
        int cardID  = card.IDInTable;
        m_bg.gameObject.SetActive(true);

        HeroInfo heroInfo   = GameTable.HeroInfoTableAsset.Lookup(cardID);

        if ( heroInfo != null )
        {
            // 道具ID
            m_param.m_guid = card.Guid; 

            // 头像
            IconInfomation iconInfo = GameTable.IconInfoTableAsset.Lookup(heroInfo.headImageId);
            if (null != iconInfo)
            {
                m_headImage.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
            }

            // 等级
            m_level.gameObject.SetActive(true);

            // 排序后的 应显示的 属性
            switch(sortType)
            {
                case ENSortType.enByRarity:
                    {
                        m_level.text    = heroInfo.Rarity.ToString();
                        break;
                    }
                case ENSortType.enByPhyAttack:
                    {   
                        m_level.text    = card.GetPhyAttack().ToString();
                        break;
                    }
                case ENSortType.enByMagAttack:
                    {
                        m_level.text    = card.GetMagAttack().ToString();
                        break;
                    }
                case ENSortType.enByHp:
                    {
                        m_level.text    = card.GetHp().ToString();
                        break;
                    }
                default:
                    int levelMax        = card.GetMaxLevel();

                    if (card.Level >= levelMax)
                    {
                        m_level.text = Localization.Get("MaxCardLevel"); 
                    }
                    else
                    {
                        m_level.text = Localization.Get("CardLevel") + card.Level;
                    }
                    break;
                     
            }
            // 文字闪现 播放一致
            m_chosen.GetComponent<TweenAlpha>().ResetToBeginning();

            // 是否在编队中 
            m_chosen.SetActive(Team.Singleton.IsCardInTeam(card.Guid));
           
            // 是否是最爱
            m_love.gameObject.SetActive(card.Love);

            m_defaultHead.gameObject.SetActive(false);

            // 职业
            OccupationInfo occupationInfo = GameTable.OccupationInfoAsset.LookUp(heroInfo.Occupation);
            if ( null != occupationInfo )
            {
                iconInfo = GameTable.IconInfoTableAsset.Lookup(occupationInfo.m_iconId);

                if (null != iconInfo)
                {
                    m_occ.gameObject.SetActive(true);

                    m_occ.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
                }
            }

           
            RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(heroInfo.Rarity);
            if ( null != rarityInfo )
            {
                iconInfo = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);

                m_rarity.gameObject.SetActive(true);

                m_rarity.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
            }
        } 
    }

    // 设置空
    public void SetEmpty()
    {
        m_bg.gameObject.SetActive(true);

        UITexture spritet   = m_bg.GetComponent<UITexture>();
        spritet.mainTexture = UICardBag.GetInstance().m_gridBgTexture;

        m_headImage.mainTexture = null;
        m_level.text        = "";
        m_level.gameObject.SetActive(false);

        m_chosen.gameObject.SetActive(false);

        // 道具ID
        m_param.m_id            = 0; // slotId
        m_param.m_guid          = CSItemGuid.Zero;

        m_index = 0; // 表示 不用显示卡牌详情

        m_love.gameObject.SetActive(false);

        m_defaultHead.gameObject.SetActive(true);

        // 职业 
        m_occ.gameObject.SetActive(false);

        // 星级
        m_rarity.gameObject.SetActive(false);

        m_tag.spriteName = "";

        m_break.gameObject.SetActive(false);
    }

    // 设置扩展
    public void SetExpand()
    {

        UITexture spritet           = m_bg.GetComponent<UITexture>();
        spritet.mainTexture         = UICardBag.GetInstance().m_extendBagTexture;

        m_bg.SetActive(true);

        m_headImage.gameObject.SetActive(false);

        m_level.text             = "";

        m_defaultHead.gameObject.SetActive(false);

        // 职业 
        m_occ.gameObject.SetActive(false);

        // 星级
        m_rarity.gameObject.SetActive(false);

        //扩容
        m_param.m_id             = -1; 
        m_param.m_guid.Reset();

        m_index = 0; // 表示 不用显示卡牌详情
    }


    public void ShowContent()
    {
        m_bg.SetActive(true);
    }

    public void HideContent()
    {
        m_bg.SetActive(false);
    }

    // 设置编辑队伍 相关
    public void UpdateOperateTeam(CSItem card,  ENSortType sortType,Team.EDITTYPE type = Team.EDITTYPE.enALL,int index = 0)
    {
        if (card ==null)
        {
            return;
        }


        Update(card, sortType, index);

        m_param.m_id    = (int)Team.EDITTYPE.enNone;
        m_param.m_guid  = card.Guid;

        int cardID = card.IDInTable;

        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(cardID);

        if ( heroInfo == null )
        {
            return;
        }

        string strShadow        = "";
        // 编辑全部队员类型
        if (Team.Singleton.m_curEditType == Team.EDITTYPE.enALL)
        {

            string coverName    = "";
            CSItemGuid guid     = Team.Singleton.m_bagMainSlotId;
            switch (type)
            {
                case Team.EDITTYPE.enMain:
                    {
                        coverName   = "cardCover-team-1";
                        guid        = Team.Singleton.m_bagMainSlotId;
                        break;
                    }
                case Team.EDITTYPE.enDeputy:
                    {
                        coverName   = "cardCover-team-2";
                        guid        = Team.Singleton.m_bagDeputySlotId;
                        break;
                    }
                case Team.EDITTYPE.enSupport:
                    {
                        coverName   = "cardCover-team-3";
                        guid        = Team.Singleton.m_bagSupportSlotId;
                        break;
                    }
                case Team.EDITTYPE.enNone:
                    {
                        coverName   = "";
                        guid        = card.Guid;
                        break;
                    }
                case Team.EDITTYPE.enALL:
                {
                     // 队伍索引图标
                    if (Team.Singleton.m_bagMainSlotId.Equals(card.Guid))
                    {
                        coverName   = "cardCover-team-1";
                        type        = Team.EDITTYPE.enMain;
                       
                    }
                    else if (Team.Singleton.m_bagDeputySlotId.Equals(card.Guid))
                    {
                        coverName   = "cardCover-team-2";
                        type        = Team.EDITTYPE.enDeputy;
                    }
                    else if (Team.Singleton.m_bagSupportSlotId.Equals(card.Guid))
                    {
                        coverName   = "cardCover-team-3";
                        type        = Team.EDITTYPE.enSupport;
                    }
                    guid        = card.Guid;
                    break;
                }

            }

            //  如果已选 则显示相应的 已选的图片
            if (guid.Equals(card.Guid))
            {
                m_tips.GetComponent<UISprite>().spriteName  = coverName;
                m_param.m_type                              = (int)Team.Singleton.m_curEditType;
                m_param.m_id                                = (int)type;
            }

            m_chosen.gameObject.SetActive(m_param.m_id != (int)Team.EDITTYPE.enNone);

            //获取世界参数表提取队伍最大人数
            WorldParamInfo info = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enTeamRoleMaxNum);
            int roleMax = 0;
            if (null != info)
            {
                roleMax = info.IntTypeValue;
            }
            else
            {
                roleMax = 3;
            }
            // 如果超过了 玩家的领导力
            if (Team.Singleton.GetTheTeamCostExcept(card.Guid) + heroInfo.Cost > User.Singleton.GetLeadership())
            {
                strShadow = "cost_Not_Enough";
            }
            else if (OperateCardList.Singleton.m_hadItemList.Count >= roleMax)         //队伍内人数是否大于等于队伍最大人数
            {
                strShadow = "mask_card";
            }
           
        }
        // 编辑单个队伍类型
        else
        {
            // 突破
            m_break.text                                = Localization.Get("LevelBreak") + card.BreakCounts;

            m_tips.GetComponent<UISprite>().spriteName = "cardCover-choose";

            // 将要被替换角色的领导力
            CSItem replaceCard = Team.Singleton.GetCard(Team.Singleton.m_curTeamIndex, Team.Singleton.m_curEditType);
            int replaceCost = 0;
            if (null != replaceCard)
            {
                replaceCost = GameTable.HeroInfoTableAsset.Lookup(replaceCard.IDInTable).Cost;
            }
            // 如果超过了 玩家的领导力()
            if (OperateCardList.Singleton.m_leadShipCost - replaceCost + heroInfo.Cost > User.Singleton.GetLeadership())
            {
                strShadow = "cost_Not_Enough";
            }
        }

        m_tips.SetActive(true);
        m_mask.SetActive(true);
        m_defaultHead.gameObject.SetActive(false);
        m_tag.spriteName = "";
        m_shadow.GetComponent<UISprite>().spriteName = strShadow;
        m_love.gameObject.SetActive(false);
    
    }

    // 更新从operatecardList 中用的格子
    public void UpdateOperate(CSItem item,ENSortType sortType,int index= 0)
    {
        if (null == item)
        {
            return;
        }

        int cardID = item.IDInTable;

        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(cardID);

        if (null == heroInfo)
        {
            return;
        }

        m_index = index;

        Update(item, sortType, m_index);

        OperateCardList.Singleton.m_curChosenIndex = -1; 

        // 已选择索引
        if (OperateCardList.Singleton.m_curType == OperateCardList.TYPE.enCardLevelUpDataCardType)
        {
            OperateCardList.Singleton.m_curChosenIndex  = OperateCardList.Singleton.LevelUpList.IndexOf(item.Guid) + 1;
        }
        else if (OperateCardList.Singleton.m_curType == OperateCardList.TYPE.enSellType)
        {
            OperateCardList.Singleton.m_curChosenIndex  = OperateCardList.Singleton.m_sellList.IndexOf(item.Guid) + 1;
        }

        
        m_tag.spriteName                                = "" + OperateCardList.Singleton.m_curChosenIndex;
        m_param.m_id                                    = (int)Team.EDITTYPE.enNone;

        // 使用中
        m_chosen.SetActive(Team.Singleton.IsCardInTeam(item.Guid));


        bool bShowShadow = false;

        if (OperateCardList.Singleton.m_curChosenIndex == 0)
        {
            if (OperateCardList.Singleton.m_chosenCardNum >= 10)
            {
               bShowShadow = true;
            }
        }

        m_shadow.SetActive(bShowShadow);
        m_break.text = Localization.Get("LevelBreak") + item.BreakCounts;
        m_break.gameObject.SetActive(true);
        m_tips.SetActive(false);
        m_mask.SetActive(false);
        m_shadow.SetActive(false);
        m_love.gameObject.SetActive(false);
        m_tag.gameObject.SetActive(true);

       
    }

    // 设置 单个队员编辑时的 删除
    public void SetXForOperateTeam()
    {
        m_headImage.mainTexture     = null;
        m_tips.GetComponent<UISprite>().spriteName       = "cardCover-cancel";
        m_tips.SetActive(true);

        m_param.m_id            = -1;// 移除标志位
        m_param.m_type          = (int)Team.Singleton.m_curEditType;
        m_tag.spriteName        = "";
        m_level.text            = "";
        m_break.gameObject.SetActive(false);
        m_chosen.SetActive(false);
        m_mask.SetActive(false);
        m_love.gameObject.SetActive(false);
        m_occ.gameObject.SetActive(false);
        m_rarity.gameObject.SetActive(false);
    }

    // 本格子 是否可以显示出 卡牌详情
    public bool IsHaveCardDetail()
    {
        return m_index != 0;
    }
}
