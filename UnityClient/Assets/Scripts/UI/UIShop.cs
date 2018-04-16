using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIShop : UIWindow
{

    public enum RecruitmentType 
    {
        enMagicStone = 1,
        enFriendShipOnce = 2,
        enFriendShipMore = 3,
        enGetCardById = 4,
    }
    public enum ShopType 
    {
        enMagicStone = 1,
        enRingOfHonor = 2,
        enRecoverStamina = 3,
        enRecoverEnergy = 4,
        enExpandFriends = 5,
    }

    public enum succType 
    {
        enRecoverStamina = 1,
        enRecoverEnergy = 2,
        enBuyRingOfHonorCard = 3,
        enFriendShipOnceRecruitment = 4,
        enMagicStoneRecruitment = 5,
        enExpandFriends = 6,
    }
    //播放开启界面动画
    //UIPanel m_PlayEnter     = null;

//购买魔法石所需变量
    //购买魔法石商店面板
    UIPanel m_MagicStoe     = null;
    //队友招募面板
    UIPanel m_Recruitment   = null;
    //GirdLis
    UIGrid m_stoneGridList = null;

    UIDragScrollView m_item = null;

    //购买魔法石商品ID
    int m_stoneId           = 0;
    // 格子列表 ID，格子UI
    Dictionary<int, UIDragScrollView> m_gridList = null;

//友情招募
    UILabel m_friendShipText = null;

    UIButton m_friendRecruitmentMoreButton = null;
    UIButton m_friendRecruitmentOnceButton = null;

    UIButton m_stoneRecruitmentButton = null;
    UISprite m_stoneRecruitmentGrid = null;//魔法石招募面板

    UILabel m_friendShipNum = null;//友情点数数量显示

    int m_onceFriendRecruitment = 1;
    int m_moreFriendRecruitment = 0;

//恢复功能
    UIButton m_recoverStaminaButton = null;//恢复体力按钮
    UIButton m_recoverEnergyButton = null;//恢复能量按钮
    UILabel m_recoverStaminaButtonLable = null;//恢复体力按钮Lable
    UILabel m_recoverEnergyButtonLable = null;//恢能量力按钮Lable
    UISprite m_recoverEnergyBG = null;//恢复体力背景
    UISprite m_recoverStaminaBG = null;//恢复能量背景

//荣誉戒指
    UIPanel m_ringOdHonorPanel = null;//荣誉戒指面板
    UIPanel m_cardListPanel = null;
    UIButton m_ringOfHonorButtonLeft = null;//荣誉戒指界面按钮左
    UIButton m_ringOfHonorButtonRight = null;//按钮右
    //gridList
    UIGrid m_cardGridList = null;
    // 格子列表 ID，格子UI
    Dictionary<int, UIDragScrollView> m_ringHonorGridList = null;
    bool m_ringHonorLeft = false;//点击左按钮
    bool m_ringHonorRight = false;//点击右按钮
    float m_ringHonorPos = 0.0f;//位移大小
    float m_ringHonorOldPos = 0.0f;//上次荣誉戒指滑动界面位置
    int m_honorCardCurrtIndex = 1;//当前显示卡牌的ID
    //UIPanel m_ringHonorCardPanel = null;

    UIProgressBar m_ringHonorBar = null;

    UIDragScrollView m_ringItem = null;

    UITexture m_skillItem = null;
    UIGrid m_skillGirdList = null;
    UISprite m_tips = null;
    int m_ringOfHonorId = 0;
    int m_ringOfHonorCardId = 0;
    UILabel m_skillName = null;
    UILabel m_skillDes = null;
    UILabel m_skillCD = null;
//扩展好友格子
    UILabel m_expandFriendsLabel;
    UISprite m_expandFriendsBG;
    UIButton m_expandFriendsButton;
//直接获得卡牌
    //UIButton m_enterIdButton = null;
    UIInput m_enterCardIdInput = null;


    static public UIShop GetInstance()
    {
        UIShop self = UIManager.Singleton.GetUIWithoutLoad<UIShop>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIShop>("UI/UIShop", UIManager.Anchor.Center);
        return self;
    }

    public override void OnInit() 
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enShopProps, OnPropertyChanged);
        m_gridList = new Dictionary<int, UIDragScrollView>();
        m_ringHonorGridList = new Dictionary<int, UIDragScrollView>();

//        m_PlayEnter     = FindChildComponent<UIPanel>("Enter");
        m_MagicStoe     = FindChildComponent<UIPanel>("StoneShopPanel");
        m_Recruitment   = FindChildComponent<UIPanel>("RecruitmentPanel");
        m_item          = FindChildComponent<UIDragScrollView>("StoneGrid");
        m_stoneGridList = FindChildComponent<UIGrid>("StoneGridList");
        m_friendShipText = FindChildComponent<UILabel>("RecruitmentMoreText");
        m_friendRecruitmentMoreButton = FindChildComponent<UIButton>("FriendRecruitmentMore");
        m_friendRecruitmentOnceButton = FindChildComponent<UIButton>("FriendRecruitmentOnce");
        m_stoneRecruitmentButton = FindChildComponent<UIButton>("StoneRecruitmentButton");
        m_stoneRecruitmentGrid = FindChildComponent<UISprite>("StoneRecruitmentGrid");
        m_recoverStaminaButton = FindChildComponent<UIButton>("RecoverStamina");
        m_recoverEnergyButton = FindChildComponent<UIButton>("RecoverEnergy");
        m_recoverStaminaButtonLable = FindChildComponent<UILabel>("StaminaLabel");
        m_recoverEnergyButtonLable = FindChildComponent<UILabel>("EnergyLabel");
        m_friendShipNum = FindChildComponent<UILabel>("FriendShipNum");
        m_ringOdHonorPanel = FindChildComponent<UIPanel>("RingOfHonorPanel");
        m_cardGridList = FindChildComponent<UIGrid>("CardGridList");

        m_ringHonorPos = m_cardGridList.GetComponent<Parma>().m_id * 0.01f;

        m_ringItem = FindChildComponent<UIDragScrollView>("CardInfoPanel");
        m_cardListPanel = FindChildComponent<UIPanel>("ExcangeCardList");
        m_cardListPanel.transform.GetComponent<UIScrollView>().onDragFinished = OnScrollDragFinish;


        m_skillItem = FindChildComponent<UITexture>("SkillItem");
        m_skillGirdList = FindChildComponent<UIGrid>("Skill");
        m_tips = FindChildComponent<UISprite>("Tips");
        m_skillName = FindChildComponent<UILabel>("SkillName");
        m_skillDes = FindChildComponent<UILabel>("SkillDes");
        m_skillCD = FindChildComponent<UILabel>("SkillCD");
        m_expandFriendsLabel = FindChildComponent<UILabel>("ExpandFriendsLabel");
        m_expandFriendsBG = FindChildComponent<UISprite>("ExpandFriendsBG");
        m_expandFriendsButton = FindChildComponent<UIButton>("ExpandFriends");
        m_recoverEnergyBG = FindChildComponent<UISprite>("RecoverEnergyBG");
        m_recoverStaminaBG = FindChildComponent<UISprite>("RecoverStaminaBG");
//        m_enterIdButton = FindChildComponent<UIButton>("EnterIdBtn");
        m_enterCardIdInput = FindChildComponent<UIInput>("Input");
        m_ringOfHonorButtonLeft = FindChildComponent<UIButton>("Selectturn_left");
        m_ringOfHonorButtonRight = FindChildComponent<UIButton>("Selectturn_right");
        m_ringHonorBar = FindChildComponent<UIProgressBar>("RingOfHonorCardBar");
        

        //m_ringHonorBar.onDragFinished = OnScrollBar;
        //EventDelegate.Add(m_ringHonorBar.onChange, OnScrollBar);
    }

    public override void OnShowWindow() 
    {
        base.OnShowWindow();
        //播放 打开商城开门动画
        //m_PlayEnter.animation.Play("ui-entergamestore-00");
        //if (MainUIManager.Singleton.ChangeUI(this))
        {
            MainButtonList.Singleton.m_curShowType = MainButtonList.SHOWWNDTYPE.ENStore;
        }
    } 



    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj) 
    {
        if (eventType == (int)ShopProp.ENPropertyChanged.enUpdateMagicStoneShop)
        {
            UpDateMagicStoneShopUI();
            //关闭转菊花loading界面
            Loading.Singleton.Hide();
        }
        else if (eventType == (int)ShopProp.ENPropertyChanged.enUpdateRecruitmentPanel)
        {
            //关闭转菊花loading界面
            Loading.Singleton.Hide();
            SetFriendShipText();
        }
        else if (eventType == (int)ShopProp.ENPropertyChanged.enUpdateRingOfHonorShop)
        {
            //重置荣誉戒指商店
            ResetRingOfHonor();
            //更新荣誉戒指商店信息
            UpDateRingOfHonorShopUI();
            //关闭转菊花loading界面
            Loading.Singleton.Hide();
        }
        else if (eventType == (int)ShopProp.ENPropertyChanged.enCloseOtherChildPanel)
        {
            CloseUIShopChildPanel();
        }
        else if (eventType == (int)ShopProp.ENPropertyChanged.enHide)
        {
            HideWindow();
        }
    }

    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("CloseShop", OnClose);
        AddChildMouseClickEvent("MagicStone_shop", OnOpenMagicStoneShop);
        AddChildMouseClickEvent("CloseStone", OnCloseStone);
        AddChildMouseClickEvent("Recruitment_team", OnRecruitmentTeam);
        AddChildMouseClickEvent("CloseRecruitment", OnCloseRecruitment);
        AddChildMouseClickEvent("StoneButton", OnBuyStoneButton);
        AddChildMouseClickEvent("StoneRecruitmentButton", OnStoneRecruitmentButton);
        AddChildMouseClickEvent("Bag_Expand", OnExpandBag);
        AddChildMouseClickEvent("Energy_Recover", OnRecoverEnergy);
        AddChildMouseClickEvent("Stamina_Recover", OnRecoverStamina);
        AddChildMouseClickEvent("FriendRecruitmentOnce", OnFriendRecruitmentOnce);
        AddChildMouseClickEvent("FriendRecruitmentMore", OnFriendRecruitmentMore);
        AddChildMouseClickEvent("Ring_Honor", OnRingOfHonor);
        AddChildMouseClickEvent("CloseRingOfHonor", OnCloseRingOfHonor);
        AddChildMouseClickEvent("Friends_Expand", OnExpandFriends);
        AddChildMouseClickEvent("EnterIdBtn", OnEnterIdBtn);
        AddChildMouseClickEvent("Selectturn_left",OnRingOfHonorButtonLeft);
        AddChildMouseClickEvent("Selectturn_right", OnRingOfHonorButtonRight);
    }

    public override void OnHideWindow()
    {
        base.OnHideWindow();
        //MainButtonList.Singleton.Show();
        //HideWindow();
        //MainMenu.Singleton.ShowMainMenu();
        //
    }


    void OnClose(GameObject obj)
    {
//        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enZone);
       //HideWindow();
    }
    void OnOpenMagicStoneShop(GameObject obj) 
    {
        m_MagicStoe.gameObject.SetActive(true);
        //进入转菊花loading界面
        Loading.Singleton.SetLoadingTips((int)(LOADINGTIPSENUM.enJumpToStore));
        ShopProp.Singleton.SendMagicStoneShopInfo();
    }
    //更新购买魔法石界面
    void UpDateMagicStoneShopUI() 
    {
        int index = 1;
        foreach (MagicStoneInfo item in ShopProp.Singleton.m_magicStoneList)
        {
            string numText = string.Format(Localization.Get("BuyMagicStone"), item.m_magicStoneNum);
            string priceText = string.Format(Localization.Get("MagciStonePrice"), item.m_magicStonePrice);
            if (m_gridList.ContainsKey(index))
            {
                UIDragScrollView scrollView = m_gridList[index];
                scrollView.GetComponent<Parma>().m_id = item.m_magicStoneId;
                scrollView.transform.Find("Text").GetComponent<UILabel>().text = numText;
                scrollView.transform.Find("Price").GetComponent<UILabel>().text = priceText;
                scrollView.transform.parent = FindChildComponent<UIGrid>("StoneGridList").transform;
                scrollView.gameObject.SetActive(true);
            }
            else 
            {
                UIDragScrollView copy = GameObject.Instantiate(m_item) as UIDragScrollView;
                copy.GetComponent<Parma>().m_id = item.m_magicStoneId;
                copy.transform.Find("Text").GetComponent<UILabel>().text = numText;
                copy.transform.Find("Price").GetComponent<UILabel>().text = priceText;
                copy.transform.parent = FindChildComponent<UIGrid>("StoneGridList").transform;
                // 设置大小
                copy.transform.localScale = m_item.transform.localScale;
                copy.gameObject.SetActive(true);
                m_gridList.Add(index, copy);
                string buttonName = copy.transform.Find("StoneButton").name + index;
                copy.transform.Find("StoneButton").name = buttonName;
                AddChildMouseClickEvent(buttonName, OnBuyStoneButton);
            }
            index++;
        }

        for (;index < m_gridList.Count; index++)
        {
            m_gridList[index].gameObject.SetActive(false);
            m_gridList[index].GetComponent<Parma>().m_id = 0;
        }
        m_stoneGridList.Reposition();
    }

    //关闭商店全部子界面
    void CloseUIShopChildPanel() 
    {
        m_MagicStoe.gameObject.SetActive(false);
        m_Recruitment.gameObject.SetActive(false);
        m_ringOdHonorPanel.gameObject.SetActive(false);
        ShopProp.Singleton.m_magicStoneList.Clear();
    }
    

    //重置荣誉戒指商店
    void ResetRingOfHonor() 
    {
        foreach (KeyValuePair<int, UIDragScrollView> item in m_ringHonorGridList)
        {
            GameObject.Destroy(item.Value.gameObject);
        }
        m_ringHonorGridList.Clear();
        m_ringHonorLeft = false;
        m_ringHonorRight = false;
        m_ringHonorOldPos = 0.0f;
        m_honorCardCurrtIndex = 1;
        m_ringHonorBar.value = 0.0f;
    }
    //更新荣誉戒指商店界面
    void UpDateRingOfHonorShopUI()
    {
        int ring = User.Singleton.UserProps.GetProperty_Int32(UserProperty.ring);

        int index = 1;
        foreach (RingOfHonorInfo item in ShopProp.Singleton.m_ringOfHonorList) 
        {
            UIDragScrollView copy = GameObject.Instantiate(m_ringItem) as UIDragScrollView;
            copy.GetComponent<Parma>().m_id = item.m_infoId;
            copy.GetComponent<Parma>().m_type = item.m_cardId;
            copy.transform.Find("Exchange").GetComponent<UIButton>().transform.Find("RingNum").GetComponent<UILabel>().text = item.m_price.ToString();
            //如果戒指数量不足 按钮为灰色
            if (ring < item.m_price)
            {
                copy.transform.Find("Exchange").GetComponent<UIButton>().enabled = false;
            }


            //设置卡牌显示 兑换时间
            RingExchangeTableInfo info = GameTable.RingExchangeTableAsset.Lookup(item.m_infoId);
            UISlider timeSlider = copy.transform.Find("CardDetail").GetComponent<UISlider>().transform.Find("ExchangeTime").GetComponent<UISlider>();
            timeSlider.transform.Find("StartTime").GetComponent<UILabel>().text = info.startDate + "~";
            timeSlider.transform.Find("EndTime").GetComponent<UILabel>().text = info.endDate;

            //获得卡牌信息
            HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(item.m_cardId);
            //设置卡牌名字
            copy.transform.Find("Attribute").GetComponent<UISlider>().transform.Find("Name").GetComponent<UILabel>().text = heroInfo.StrName;
            copy.transform.Find("CardDetail").GetComponent<UISlider>().transform.Find("CardNamePanel").transform.Find("Name").GetComponent<UILabel>().text = heroInfo.StrName;
            //设置种族职业
            UISlider typeSlider = copy.transform.Find("Attribute").GetComponent<UISlider>().transform.Find("TypePanel").GetComponent<UISlider>();
            typeSlider.transform.Find("Type").GetComponent<UILabel>().text = GameTable.RaceInfoTableAsset.LookUp(heroInfo.Type).m_name;
            UISlider occupationSlider = copy.transform.Find("Attribute").GetComponent<UISlider>().transform.Find("OccupationPanel").GetComponent<UISlider>();
            occupationSlider.transform.Find("Occupation").GetComponent<UILabel>().text = GameTable.OccupationInfoAsset.LookUp(heroInfo.Occupation).m_name;
            UITexture occupationSprite = occupationSlider.transform.Find("Texture").GetComponent<UITexture>();
            //设置职业图标
            OccupationInfo occInfo = GameTable.OccupationInfoAsset.LookUp(heroInfo.Occupation);
            IconInfomation occicon = GameTable.IconInfoTableAsset.Lookup(occInfo.m_iconId);
            occupationSprite.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(occicon.dirName);
            //设置星级
            UISlider raritySlider = copy.transform.Find("Attribute").GetComponent<UISlider>().transform.Find("RarityPanel").GetComponent<UISlider>();

            RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(heroInfo.Rarity);
            IconInfomation icon = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);
            raritySlider.transform.Find("Rarity").GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);

            icon = GameTable.IconInfoTableAsset.Lookup(heroInfo.ImageId);
            //设置卡牌图片
            copy.transform.Find("Attribute").GetComponent<UISlider>().transform.Find("Card").GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);

    
            //设置魔法攻击力
            UISlider magicSlider = copy.transform.Find("CardDetail").GetComponent<UISlider>().transform.Find("CardNamePanel").transform.Find("MaxMagAttackPanel").GetComponent<UISlider>();
//            RarityRelativeInfo rarityRelative = GameTable.RarityRelativeAsset.LookUp(heroInfo.Rarity);
            magicSlider.transform.Find("MagAttackNum").GetComponent<UILabel>().text = BattleFormula.GetMagAttack(item.m_cardId, 1) + ""; 

            UISlider phySlider = copy.transform.Find("CardDetail").GetComponent<UISlider>().transform.Find("CardNamePanel").transform.Find("MaxPhyAttackPanel").GetComponent<UISlider>();
            phySlider.transform.Find("PhyAttackNum").GetComponent<UILabel>().text = BattleFormula.GetPhyAttack(item.m_cardId, 1) + ""; 
            //设置生命值
            UISlider hpSlider = copy.transform.Find("CardDetail").GetComponent<UISlider>().transform.Find("CardNamePanel").transform.Find("MaxHpPanel").GetComponent<UISlider>();
            hpSlider.transform.Find("MaxHpNum").GetComponent<UILabel>().text = BattleFormula.GetHp(item.m_cardId, 1) + ""; 

            //设置卡牌编号
            UILabel cardNum = copy.transform.Find("Attribute").GetComponent<UISlider>().transform.Find("CardNumText").GetComponent<UILabel>();
            cardNum.text = string.Format(Localization.Get("MercenaryNum"), heroInfo.CardId);


            copy.transform.parent = FindChildComponent<UIGrid>("CardGridList").transform;
            // 设置大小
            copy.transform.localScale = m_ringItem.transform.localScale;
            copy.gameObject.SetActive(true);

            //设置主动技能
            IconInfomation iconInfo = null;
            SkillInfo skillInfo = null;
            int skillIndex = 1;
            List<int> skillIDList = heroInfo.GetAllSkillIDList();
            foreach (int skillId in skillIDList)
            {
                skillInfo = GameTable.SkillTableAsset.Lookup(skillId);
                if (0 == skillInfo.SkillType)
                {
                    UITexture copySkillItem = GameObject.Instantiate(m_skillItem) as UITexture;
                    copySkillItem.GetComponent<Parma>().m_id = skillId;
                    copySkillItem.transform.localScale = m_skillItem.transform.lossyScale;
                    copySkillItem.gameObject.SetActive(true);
                    iconInfo = GameTable.IconInfoTableAsset.Lookup(skillInfo.Icon);
                    copySkillItem.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
                    copySkillItem.name = copySkillItem.name + skillIndex;

                    copySkillItem.transform.parent = copy.transform.Find("CardDetail").transform.Find("Skill").transform;
                    EventDelegate.Add(copySkillItem.GetComponent<UIEventTrigger>().onPress, OnShowTips);
                    EventDelegate.Add(copySkillItem.GetComponent<UIEventTrigger>().onClick, OnHideTips);
                    //AddChildMouseClickEvent(copySkillItem.name, OnHideTips);
                }
                skillIndex++;
            }
            m_skillGirdList.Reposition();
            //设置被动技能
            int passiveIndex = 1;
            foreach (int skillId in heroInfo.PassiveSkillIDList)
            {
                skillInfo = GameTable.SkillTableAsset.Lookup(skillId);
                if (0 == skillInfo.SkillType)
                {
                    UISprite copySkillItem = m_skillItem.gameObject.AddComponent<UISprite>();
                    copySkillItem.GetComponent<Parma>().m_id = skillId;
                    copySkillItem.transform.localScale = m_skillItem.transform.lossyScale;
                    copySkillItem.gameObject.SetActive(true);
                    iconInfo = GameTable.IconInfoTableAsset.Lookup(skillInfo.Icon);
                    copySkillItem.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
                    copySkillItem.name = copySkillItem.name + skillIndex;

                    copySkillItem.transform.parent = copy.transform.Find("CardDetail").transform.Find("PassiveSkill").transform;
                    EventDelegate.Add(copySkillItem.GetComponent<UIEventTrigger>().onPress, OnShowTips);
                    EventDelegate.Add(copySkillItem.GetComponent<UIEventTrigger>().onClick, OnHideTips);
                    //AddChildMouseClickEvent(copySkillItem.name, OnHideTips);
                }
                passiveIndex++;
            }


           
            m_ringHonorGridList.Add(index, copy);
            AddMouseClickEvent(copy.transform.Find("Exchange").gameObject, OnBuyRingCardButton);
            index++;
        }
        for (; index < m_ringHonorGridList.Count; index++)
        {
            m_ringHonorGridList[index].gameObject.SetActive(false);
            m_ringHonorGridList[index].GetComponent<Parma>().m_id = 0;
            m_ringHonorGridList[index].GetComponent<Parma>().m_type = 0;
        }
        m_cardGridList.Reposition();
    }

    // 显示Tips
    void OnShowTips()
    {
        Parma param = UIEventTrigger.current.gameObject.GetComponent<Parma>();
        if (null == param)
        {
            return;
        }

        // 更新
        UpdateTipsInfo(param.m_id);

        m_tips.gameObject.SetActive(true);
    }

    // 隐藏TIPS
    void OnHideTips()
    {
        m_tips.gameObject.SetActive(false);
    }

    // 更新TIPS 信息
    void UpdateTipsInfo(int skillId)
    {
        SkillInfo info = GameTable.SkillTableAsset.Lookup(skillId);
        if (null == info)
        {
            return;
        }

        CDInfo cdInfo = GameTable.CDTableAsset.Lookup(info.CoolDown);

        m_skillName.text = info.Name;
        m_skillDes.text = info.Description;
        m_skillCD.text = UICardDetail.GetInstance().GetTimeString(cdInfo.CDTime);

//        int needLevel = 0;
        // tips 位置
        float postionY = 98f;
        // 主动技能类型
        if (info.SkillType == (int)ENSkillType.enSkill)
        {
            postionY = 98f;
        }
        else
        {
            postionY = -17f;
        }
       
        // 设置位置
        m_tips.LocalPositionY(postionY);
    }

    //点击购买荣誉戒指卡牌按钮
    void OnBuyRingCardButton(object sender, EventArgs e) 
    {
        GameObject obj = (GameObject)sender;
        m_ringOfHonorId = obj.transform.parent.GetComponent<Parma>().m_id;
        m_ringOfHonorCardId = obj.transform.parent.GetComponent<Parma>().m_type;

        UICommonMsgBoxCfg boxCfg = m_cardListPanel.transform.GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnBuyRingCardButtonYes, OnBuyRingCardButtonNo, boxCfg);
        string cardName = GameTable.HeroInfoTableAsset.Lookup(m_ringOfHonorCardId).StrName;
        string buyText = string.Format(Localization.Get("EnsureExchangeCard"), cardName);
        UICommonMsgBox.GetInstance().GetMainText().SetText(buyText);

    }

    //确定购买荣誉戒指
    void OnBuyRingCardButtonYes(object sender, EventArgs e) 
    {
        //进入转菊花loading界面
        Loading.Singleton.SetLoadingTips((int)(LOADINGTIPSENUM.enJumpToStore));
        ShopProp.Singleton.SendBuyRingOfHonor(m_ringOfHonorId, m_ringOfHonorCardId);
    }
    void OnBuyRingCardButtonNo(object sender, EventArgs e)
    {

    }



    void OnCloseStone(GameObject obj) 
    {
        m_MagicStoe.gameObject.SetActive(false);
        ShopProp.Singleton.m_magicStoneList.Clear();
    }
    //设置友情点数按钮显示
    void SetFriendShipText() 
    {
        int friendPoint =  User.Singleton.UserProps.GetProperty_Int32(UserProperty.friendship_point);
        m_friendShipNum.text = string.Format(Localization.Get("CurrentFriendshipPoints"), friendPoint);
        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enFrinendShipRecruitmentNum);

        if (friendPoint < param.IntTypeValue)
        {
            m_friendRecruitmentMoreButton.gameObject.SetActive(false);
            m_friendRecruitmentOnceButton.isEnabled = false;
        }
        else if(friendPoint >= param.IntTypeValue && (friendPoint < (param.IntTypeValue * 2)))
        {
            m_friendRecruitmentOnceButton.isEnabled = true;
            m_friendRecruitmentMoreButton.gameObject.SetActive(false);
        }
        else
        {
            m_friendRecruitmentOnceButton.isEnabled = true;
            m_friendRecruitmentMoreButton.gameObject.SetActive(true);
        }
        //显示多次招募正确文字
        int friendCount = friendPoint/param.IntTypeValue;
        param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enRecruitTimesMax);
        friendCount = (friendCount > param.IntTypeValue) ? param.IntTypeValue : friendCount;
        m_moreFriendRecruitment = friendCount;
        m_friendShipText.text = string.Format(Localization.Get("RecruitTimes"), friendCount);
    }
    void OnRecruitmentTeam(GameObject obj) 
    {
        ShowRecruitment();
    }
    void OnCloseRecruitment(GameObject obj) 
    {
        m_Recruitment.gameObject.SetActive(false);
    }

    public void ShowRecruitment() 
    {
        m_Recruitment.gameObject.SetActive(true);
        SetFriendShipText();
    }

    //点击购买按钮
    void OnBuyStoneButton(GameObject obj) 
    {
        m_stoneId = obj.transform.parent.GetComponent<Parma>().m_id;
        int magicStoneNum = 0;
        int magicStonePrice = 0;
        foreach (MagicStoneInfo item in ShopProp.Singleton.m_magicStoneList)
        {
            if (item.m_magicStoneId == m_stoneId)
            {
                magicStoneNum = item.m_magicStoneNum;
                magicStonePrice = item.m_magicStonePrice;
            }
        }
        UICommonMsgBoxCfg boxCfg = m_stoneGridList.transform.GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnBuyStoneButtonYes, OnBuyStoneButtonNo, boxCfg);
        string buyText = string.Format(Localization.Get("EnsureBuyMagicStone"), magicStonePrice, magicStoneNum);
        UICommonMsgBox.GetInstance().GetMainText().SetText(buyText);
    }
    //购买二次确认框
    void OnBuyStoneButtonYes(object sender, EventArgs e) 
    {
        ShopProp.Singleton.SendBuyMagicStone(m_stoneId);
    }
    void OnBuyStoneButtonNo(object sender, EventArgs e)
    {

    }
    //友情招募一次按钮
    void OnFriendRecruitmentOnce(GameObject obj) 
    {
        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enFrinendShipRecruitmentNum);
        UICommonMsgBoxCfg boxCfg = m_friendRecruitmentOnceButton.transform.GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnFriendRecruitmentOnceYes, OnStoneRecruitmentButtonNo, boxCfg);
        string buyText = string.Format(Localization.Get("EnsureFriendRecruit"), param.IntTypeValue,m_onceFriendRecruitment);
        UICommonMsgBox.GetInstance().GetMainText().SetText(buyText);
        int friendPoint = User.Singleton.UserProps.GetProperty_Int32(UserProperty.friendship_point);
        buyText = string.Format(Localization.Get("RemainFriendPoints"), (friendPoint - param.IntTypeValue));
        UICommonMsgBox.GetInstance().GetMainText().SetHintText(buyText);

    }
    void OnFriendRecruitmentOnceYes(object sender, EventArgs e) 
    {
        //进入转菊花loading界面
        Loading.Singleton.SetLoadingTips((int)(LOADINGTIPSENUM.enJumpToStore));
        ShopProp.Singleton.SendFriendRecruitmentOnce();
        ////关闭抽卡界面
        //m_Recruitment.gameObject.SetActive(false);
        HideWindow();
    }

    void OnFriendRecruitmentOnceNO(object sender, EventArgs e) 
    {

    }

    //友情招募多次按钮
    void OnFriendRecruitmentMore(GameObject obj) 
    {
        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enFrinendShipRecruitmentNum);
        UICommonMsgBoxCfg boxCfg = m_friendRecruitmentMoreButton.transform.GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnFriendRecruitmentMoreYes, OnFriendRecruitmentMoreNo, boxCfg);
        string buyText = string.Format(Localization.Get("EnsureFriendRecruit"), (m_moreFriendRecruitment * param.IntTypeValue), m_moreFriendRecruitment);
        UICommonMsgBox.GetInstance().GetMainText().SetText(buyText);
        int friendPoint = User.Singleton.UserProps.GetProperty_Int32(UserProperty.friendship_point);
        buyText = string.Format(Localization.Get("RemainFriendPoints"), (friendPoint - (m_moreFriendRecruitment * param.IntTypeValue)));
        UICommonMsgBox.GetInstance().GetMainText().SetHintText(buyText);
    }

    void OnFriendRecruitmentMoreYes(object sender, EventArgs e) 
    {
        //进入转菊花loading界面
        Loading.Singleton.SetLoadingTips((int)(LOADINGTIPSENUM.enJumpToStore));
        ShopProp.Singleton.SendFriendRecruitmentMore();
        ////关闭抽卡界面
        //m_Recruitment.gameObject.SetActive(false);
        HideWindow();
    }
    void OnFriendRecruitmentMoreNo(object sender, EventArgs e)
    {

    }


    //魔法石招募
    void OnStoneRecruitmentButton(GameObject obj) 
    {
        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enMagicStoneRecruitmentNum);
        UICommonMsgBoxCfg boxCfg = m_stoneRecruitmentButton.transform.GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnStoneRecruitmentButtonYes, OnStoneRecruitmentButtonNo, boxCfg);
        string buyText = string.Format(Localization.Get("EnsureStoneRecruit"), param.IntTypeValue);
        UICommonMsgBox.GetInstance().GetMainText().SetText(buyText);
    }

    void OnStoneRecruitmentButtonYes(object sender, EventArgs e) 
    {
        //确认购买后判断魔法石是否足够 不够 二次确认框 魔法石不足
        int bindMoney = User.Singleton.UserProps.GetProperty_Int32(UserProperty.bind_money);
        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enMagicStoneRecruitmentNum);
        if (param.IntTypeValue > bindMoney)
        {
            UICommonMsgBoxCfg boxCfg = m_stoneRecruitmentGrid.transform.GetComponent<UICommonMsgBoxCfg>();
            UICommonMsgBox.GetInstance().ShowMsgBox(null, null, boxCfg);
            string text = Localization.Get("NotEnoughMagicStone");
            UICommonMsgBox.GetInstance().GetMainText().SetText(text);
            return;
        }
        //进入转菊花loading界面
        Loading.Singleton.SetLoadingTips((int)(LOADINGTIPSENUM.enJumpToStore));
        //如果足够 发送消息 到服务器魔法石招募
        ShopProp.Singleton.SendMagicStoneRecruitment();
        ////关闭抽卡界面
        //m_Recruitment.gameObject.SetActive(false);
        HideWindow();
    }
    void OnStoneRecruitmentButtonNo(object sender, EventArgs e) 
    {

    }



    //扩充背包
    void OnExpandBag(GameObject obj) 
    {
        ExpandBag.Singleton.ShowExpandBag();
    }

    //点击恢复体力按钮
    void OnRecoverStamina(GameObject obj) 
    {
        //恢复体力值数量
        int staminaNum = 0;
        //计算恢复体力所需魔法石
        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enRecoverStaminaMagicStoneNum);
        int needStone = param.IntTypeValue;

        int money = User.Singleton.GetMoney();
        if (needStone > money)
        {
            //弹出二次确认 魔法石不足
            UICommonMsgBoxCfg boxCfg = m_recoverStaminaBG.transform.GetComponent<UICommonMsgBoxCfg>();
            UICommonMsgBox.GetInstance().ShowMsgBox(null, null, boxCfg);
            string text = Localization.Get("NotEnoughMagicStone");
            UICommonMsgBox.GetInstance().GetMainText().SetText(text);
            return;
        }
        //最大体力值上限
        int maxStamina = User.Singleton.GetMaxStamina();
        int stamina = User.Singleton.UserProps.GetProperty_Int32(UserProperty.stamina);//当前体力值
        //判断是否为最大体力值
        if (stamina >= maxStamina)
        {
            //弹出二次确认 体力值已满
            UICommonMsgBoxCfg boxCfg = m_recoverStaminaButtonLable.transform.GetComponent<UICommonMsgBoxCfg>();
            UICommonMsgBox.GetInstance().ShowMsgBox(null, null, boxCfg);
            string text = Localization.Get("StaminaIsMax");
            UICommonMsgBox.GetInstance().GetMainText().SetText(text);
            return;
        }
        param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enRecoverStaminaNum);
        //直接回满
        if (0 > param.IntTypeValue)
        {
            //最大体力值上限
            staminaNum = maxStamina - stamina;

        }
        else 
        {
            //恢复固定值
            param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enRecoverStaminaNum);
            staminaNum = param.IntTypeValue;//恢复多少体力
        }

        //弹出二次确认 是否恢复体力
        UICommonMsgBoxCfg boxCfg2 = m_recoverStaminaButton.transform.GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnRecoverStaminaYes, OnRecoverStaminaNo, boxCfg2);
        string text2 = string.Format(Localization.Get("EnsureRecoverStamina"), needStone, staminaNum);
        UICommonMsgBox.GetInstance().GetMainText().SetText(text2);
    }

    void OnRecoverStaminaYes(object sender, EventArgs e) 
    {
        //发送确认恢复体力的消息
        ShopProp.Singleton.SendRecoverStamina();
        //进入转菊花loading界面
        Loading.Singleton.SetLoadingTips((int)(LOADINGTIPSENUM.enJumpToStore));
    }
    void OnRecoverStaminaNo(object sender, EventArgs e) 
    {

    }


    //恢复能量
    void OnRecoverEnergy(GameObject obj) 
    {
        //恢复能量值数量
        int energyNum = 0;
        //计算恢能量所需魔法石
        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enRecoverEnergyMagicStoneNum);
        int needStone = param.IntTypeValue;
        //最大军资上限
        int maxEnergy = User.Singleton.GetMaxEnergy();
        int energy = User.Singleton.UserProps.GetProperty_Int32(UserProperty.energy);//当前军资

        int money = User.Singleton.GetMoney();
        if (needStone > money)
        {
            //弹出二次确认 魔法石不足
            UICommonMsgBoxCfg boxCfg = m_recoverEnergyBG.transform.GetComponent<UICommonMsgBoxCfg>();
            UICommonMsgBox.GetInstance().ShowMsgBox(null, null, boxCfg);
            string text = Localization.Get("NotEnoughMagicStone");
            UICommonMsgBox.GetInstance().GetMainText().SetText(text);
            return;
        }

        //判断是否为最大军资
        if (energy >= maxEnergy)
        {
            //弹出二次确认 军资已满
            UICommonMsgBoxCfg boxCfg = m_recoverEnergyButtonLable.transform.GetComponent<UICommonMsgBoxCfg>();
            UICommonMsgBox.GetInstance().ShowMsgBox(null, null, boxCfg);
            string text = Localization.Get("EnergyIsMax");
            UICommonMsgBox.GetInstance().GetMainText().SetText(text);
            return;
        }
        param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enRecoverEnergyNum);
        //直接回满
        if (0 > param.IntTypeValue)
        {
            //最大军资上限
            energyNum = maxEnergy - energy;

        }
        else
        {
            //恢复固定值
            param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enRecoverEnergyNum);
            energyNum = param.IntTypeValue;//恢复多少军资
        }
        //弹出二次确认 是否恢复军资
        UICommonMsgBoxCfg boxCfg2 = m_recoverEnergyButton.transform.GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnRecoverEnergyYes, OnRecoverEnergyNo, boxCfg2);
        string text2 = string.Format(Localization.Get("EnsureRecoverEnergy"), needStone, energyNum);
        UICommonMsgBox.GetInstance().GetMainText().SetText(text2);
    }
    void OnRecoverEnergyYes(object sender, EventArgs e) 
    {
        //发送确认恢复体力的消息
        ShopProp.Singleton.SendRecoverEnergy();
        //进入转菊花loading界面
        Loading.Singleton.SetLoadingTips((int)(LOADINGTIPSENUM.enJumpToStore));
    }
    void OnRecoverEnergyNo(object sender, EventArgs e) 
    {

    }

    //荣誉戒指
    void OnRingOfHonor(GameObject obj) 
    {
        m_ringOdHonorPanel.gameObject.SetActive(true);
        //进入转菊花loading界面
        Loading.Singleton.SetLoadingTips((int)(LOADINGTIPSENUM.enJumpToStore));
        //发送到服务器 请求荣誉戒指商店信息
        ShopProp.Singleton.SendRingOfHonorShopInfo();
    }
    void OnCloseRingOfHonor(GameObject obj) 
    {
        m_ringOdHonorPanel.gameObject.SetActive(false);
    }

    //购买好友格子
    void OnExpandFriends(GameObject obj) 
    {
        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enMaxFriendCount);
        int friendCount = User.Singleton.GetFriendCount();
        if (friendCount >= param.IntTypeValue)
        {
            //弹出二次确认 好友格子已经达到上限
            UICommonMsgBoxCfg boxCfg = m_expandFriendsLabel.transform.GetComponent<UICommonMsgBoxCfg>();
            UICommonMsgBox.GetInstance().ShowMsgBox(null, null, boxCfg);
            string text = Localization.Get("FriendNumMax");
            UICommonMsgBox.GetInstance().GetMainText().SetText(text);
            return;
        }
        param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enExpandFriendsNeedMagicStone);
        int needMoney = param.IntTypeValue;
        int money = User.Singleton.GetMoney();
        if (needMoney > money)
        {
            //弹出二次确认 魔法石不足
            UICommonMsgBoxCfg boxCfg = m_expandFriendsBG.transform.GetComponent<UICommonMsgBoxCfg>();
            UICommonMsgBox.GetInstance().ShowMsgBox(null, null, boxCfg);
            string text = Localization.Get("NotEnoughMagicStone");
            UICommonMsgBox.GetInstance().GetMainText().SetText(text);
            return;
        }
        param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enExpandFriendsNum);
        //弹出二次确认 是否恢复军资
        UICommonMsgBoxCfg boxCfg2 = m_expandFriendsButton.transform.GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnExpandFriendsYes, OnExpandFriendsNo, boxCfg2);
        string text2 = string.Format(Localization.Get("EnsureExpandFriends"), needMoney, param.IntTypeValue);
        UICommonMsgBox.GetInstance().GetMainText().SetText(text2);        
    }
    void OnExpandFriendsYes(object sender, EventArgs e)
    {
        //发送购买好友格子的消息
        ShopProp.Singleton.SendExpandFriends();
        //进入转菊花loading界面
        Loading.Singleton.SetLoadingTips((int)(LOADINGTIPSENUM.enJumpToStore));
    }
    void OnExpandFriendsNo(object sender, EventArgs e)
    {

    }

    //点击获得卡牌按钮
    void OnEnterIdBtn(GameObject obj) 
    {
        if (m_enterCardIdInput.value.Length > 0)
        {
            //发送到服务器 获得卡牌
            ShopProp.Singleton.SendGetCardBuyId(int.Parse(m_enterCardIdInput.value));
        }
    }

    //荣誉戒指兑换卡牌界面按钮响应
    void OnRingOfHonorButtonLeft(GameObject obj) 
    {
        m_ringHonorLeft = true;
        if (m_honorCardCurrtIndex > 1)
        {
            m_honorCardCurrtIndex = m_honorCardCurrtIndex - 1;
        }
    }
    void OnRingOfHonorButtonRight(GameObject obj)
    {
        m_ringHonorRight = true;
        if (m_honorCardCurrtIndex < ShopProp.Singleton.m_ringOfHonorList.Count)
        {
            m_honorCardCurrtIndex = m_honorCardCurrtIndex + 1;
        }
    }

    void OnScrollDragFinish() 
    {
        if (m_ringHonorOldPos < m_ringHonorBar.value)
        {
            m_ringHonorRight = true;
            if (m_honorCardCurrtIndex < ShopProp.Singleton.m_ringOfHonorList.Count)
            {
                m_honorCardCurrtIndex = m_honorCardCurrtIndex + 1;
            }
        }
        if (m_ringHonorOldPos > m_ringHonorBar.value)
        {
            m_ringHonorLeft = true;
            if (m_honorCardCurrtIndex > 1)
            {
                m_honorCardCurrtIndex = m_honorCardCurrtIndex - 1;
            }
        }
        m_ringHonorOldPos = m_ringHonorBar.value;
    }
    

    public override void OnUpdate()
    {
        base.OnUpdate();
        if ((m_ringHonorBar.value - 0.0f) < 0.01f)
        {
            m_ringOfHonorButtonLeft.gameObject.SetActive(false);
        }
        else
        {
            m_ringOfHonorButtonLeft.gameObject.SetActive(true);
        }
        if ((1.0f - m_ringHonorBar.value) < 0.01f)
        {
            m_ringOfHonorButtonRight.gameObject.SetActive(false);
        }
        else
        {
            m_ringOfHonorButtonRight.gameObject.SetActive(true);
        }
        if (m_ringHonorLeft)
        {
            m_ringHonorBar.value = m_ringHonorBar.value - m_ringHonorPos;
            if (m_ringHonorBar.value <= (ShopProp.Singleton.m_cardPre * (m_honorCardCurrtIndex - 1)))
            {
                m_ringHonorOldPos = m_ringHonorBar.value;
                m_ringHonorLeft = false;
                if (m_honorCardCurrtIndex == 1)
                {
                    m_ringHonorBar.value = 0.0f;
                }
            }
        }
        if (m_ringHonorRight)
        {
            m_ringHonorBar.value = m_ringHonorBar.value + m_ringHonorPos;
            float temp = (ShopProp.Singleton.m_cardPre * (m_honorCardCurrtIndex - 1));
            if (temp > 0.9f)
            {
                temp = 1.0f;
            }
            if (m_ringHonorBar.value >= temp)
            {
                m_ringHonorRight = false;
                m_ringHonorOldPos = m_ringHonorBar.value;
                if (m_honorCardCurrtIndex == ShopProp.Singleton.m_ringOfHonorList.Count)
                {
                    m_ringHonorBar.value = 1.0f;
                    m_ringHonorOldPos = 1.0f;
                }
            }
        }
    }
}