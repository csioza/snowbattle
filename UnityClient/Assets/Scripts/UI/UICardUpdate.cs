using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UICardUpdate : UIWindow
{
    UITexture m_cardBG           = null;
    UILabel m_tupoCounts        = null;
    UILabel m_levelTips             = null;
    List<UISprite> m_spriteList = null;
    List<UIButton> m_buttonList = null;
    UIGrid m_grid               = null;
    UISlider m_processBar       = null;
    UILabel m_cost              = null;
    UILabel m_hp                = null;
    UILabel m_magAttack         = null;
    UILabel m_phyAttack         = null;
    UILabel m_occuption         = null;
    UILabel m_type              = null;
    UITexture m_rarity           = null;
    UILabel m_name              = null;
    UILabel m_break             = null;
    //UILabel m_baseLevel         = null;
    //UIScrollView m_cardList     = null;
    UIButton m_evolution        = null;
    UILabel m_levelAfter        = null;
    UILabel m_levelBefore       = null;
    UILabel m_expPercent        = null;
    UILabel m_hold              = null;
    UILabel m_chosen            = null;
    UIPanel m_success           = null;
    UILabel m_successBreak      = null;
    UILabel m_oldLv             = null;
    UILabel m_oldHp             = null;
    UILabel m_oldPhyAttack      = null;
    UILabel m_oldMagAttack      = null;

    UILabel m_newLv             = null;
    UILabel m_newHp             = null;
    UILabel m_newPhyAttack      = null;
    UILabel m_newMagAttack      = null;
    UITexture m_successCardBG    = null;
    UILabel m_warning           = null;
    UILabel m_costMoney         = null;
    UILabel m_myMoney           = null;
    UISlider m_attribute        = null;
    UIButton m_levelUpBtn       = null;
    UIPanel m_listPanel         = null;
        
    int m_listMax               = 10; // 材料列表的最大个数

    // 索引， slotID
    //Dictionary<int,int> m_dataSlotList    = null;

    static public UICardUpdate GetInstance()
	{
        UICardUpdate self = UIManager.Singleton.GetUIWithoutLoad<UICardUpdate>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UICardUpdate>("UI/UICardUpdate", UIManager.Anchor.Center);
		return self;
	}

	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enCardUpdate, OnPropertyChanged);

        m_cardBG        = FindChildComponent<UITexture>("Card");
        m_grid          = FindChildComponent<UIGrid>("Grid");
        m_tupoCounts    = FindChildComponent<UILabel>("TupoCounts");
        m_levelTips     = FindChildComponent<UILabel>("LevelTips");
        m_processBar    = FindChildComponent<UISlider>("Exp");
        m_cost          = FindChildComponent<UILabel>("Cost");
        m_hp            = FindChildComponent<UILabel>("HP");
        m_magAttack     = FindChildComponent<UILabel>("MagAttack");
        m_phyAttack     = FindChildComponent<UILabel>("PhyAttack");
        m_occuption     = FindChildComponent<UILabel>("Occupation");
        m_type          = FindChildComponent<UILabel>("Type");
        m_rarity        = FindChildComponent<UITexture>("Rarity");
        m_name          = FindChildComponent<UILabel>("Name");

        m_break         = FindChildComponent<UILabel>("Tupo");
//        m_baseLevel     = FindChildComponent<UILabel>("Level");
//        m_cardList      = FindChildComponent<UIScrollView>("CardList");
        m_evolution     = FindChildComponent<UIButton>("Evolution");
        m_levelAfter    = FindChildComponent<UILabel>("LevelAfter");
        m_levelBefore   = FindChildComponent<UILabel>("Level");
        m_expPercent    = FindChildComponent<UILabel>("LevelPercent");
        m_chosen        = FindChildComponent<UILabel>("ChosenNum");
        m_hold          = FindChildComponent<UILabel>("HoldNum");
        m_success       = FindChildComponent<UIPanel>("Success");
        m_successBreak  = FindChildComponent<UILabel>("Break");
        m_oldLv         = FindChildComponent<UILabel>("OldLV");
        m_oldHp         = FindChildComponent<UILabel>("OldHP");
        m_oldPhyAttack  = FindChildComponent<UILabel>("OldPhyAttack");
        m_oldMagAttack  = FindChildComponent<UILabel>("OldMagAttack");

        m_newLv         = FindChildComponent<UILabel>("NewLV");
        m_newHp         = FindChildComponent<UILabel>("NewHP");
        m_newMagAttack  = FindChildComponent<UILabel>("NewMagAttack");
        m_newPhyAttack  = FindChildComponent<UILabel>("NewPhyAttack");
        m_successCardBG = FindChildComponent<UITexture>("SuccessCardBG");
        m_warning       = FindChildComponent<UILabel>("Warning");
        m_costMoney     = FindChildComponent<UILabel>("CostMoney");
        m_myMoney       = FindChildComponent<UILabel>("MyMoney");
        m_attribute     = FindChildComponent<UISlider>("Attribute");

        m_levelUpBtn    = FindChildComponent<UIButton>("Update");
        m_listPanel     = FindChildComponent<UIPanel>("CardList");

        m_spriteList    = new List<UISprite>();
        m_buttonList    = new List<UIButton>();
//        m_dataSlotList  = new Dictionary<int, int>();

        for (int i = 0; i < 10; i++)
        {
            UISprite spirte = FindChildComponent<UISprite>("Image"+i);
            m_spriteList.Add(spirte);

            UIButton button  = FindChildComponent<UIButton>("Button" + i );
            m_buttonList.Add(button);
        }

        m_grid.Reposition();
            
	}

    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("Card", OnChooseBaseCard);

        AddChildMouseClickEvent("Update", OnLevelUp);

        AddChildMouseClickEvent("Return", OnReturn);

        for (int i = 0; i < 10; i++ )
        {
            AddChildMouseClickEvent("" + i, OnChooseDataCard);

            AddChildMouseClickEvent("Button" + i, OnCancelDataCard);
        }

        AddChildMouseClickEvent("CardListBg", OnChooseDataCard);

        AddChildMouseClickEvent("Evolution", OnShowEvolution);

        AddChildMouseClickEvent("LevelUpAgain", OnLevelUpAgain);

        AddChildMouseClickEvent("SuccessCancel", OnSuccessCancel);

    }



    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if ( null == WindowRoot )
        {
            return;
        }
        if (eventType == (int)CardUpdateProp.ENPropertyChanged.enShowCardUpdate)
        {
//             if (MainUIManager.Singleton.ChangeUI(this))
//             {
//                 ShowWindow();
//                 UpdateInfo();
//             }
        }
        else if (eventType == (int)CardUpdateProp.ENPropertyChanged.enFlushCardUpdate)
        {
            UpdateInfo();
        }
        else if (eventType == (int)CardUpdateProp.ENPropertyChanged.enSuccess)
        {
            UpdateSuccessInfo();
        }
  
    }

    public override void OnShowWindow()
    {
        base.OnShowWindow();
        UpdateInfo();
    }
    public override void OnHideWindow()
    {
        base.OnHideWindow();
        UpdateInfo();
    }

    // 选择 强化的基本卡牌
    void OnChooseBaseCard(object sender)
    {
        OperateCardList.Singleton.OnShowUpdateBaseCardList();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enOperaterCardList);
        HideWindow();
    }

    // 选择 强化的 材料卡牌
    void OnChooseDataCard(object sender)
    {
        OperateCardList.Singleton.OnShowLevelUpDataCardList();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enOperaterCardList);
        HideWindow();
    }

    // 返回
    void OnReturn(object sender)
    {
        CardUpdateProp.Singleton.ClearUpdateCardData();
        //HideWindow();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardBag);
        //CardBag.Singleton.OnShowCardBag();
        UICardBag.GetInstance().HideSeleteCardEffect();
    }

    // 显示进化合成界面
    void OnShowEvolution(object sender)
    {
        CardEvolution.Singleton.OnShowCardEvolution(CardUpdateProp.Singleton.m_curLevelGuid);
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardEvolution);
        HideWindow();
    }

    // 继续强化
    void OnLevelUpAgain(object sender)
    {
        m_success.gameObject.SetActive(false);
        UpdateInfo();
    }

    // 强化成功返回
    void OnSuccessCancel(object sender)
    {
        m_success.gameObject.SetActive(false);
        CardUpdateProp.Singleton.m_curLevelGuid = CSItemGuid.Zero;
        UpdateInfo();
        //HideWindow();

    }

    // 卡牌上的详细信息更新
    void UpdateCardInfo()
    {
        // 清空
        if (CardUpdateProp.Singleton.m_curLevelGuid.Equals(CSItemGuid.Zero))
        {
            m_cost.text         = "" ;
            m_name.text         = "";
            m_phyAttack.text    = "" ;
            m_magAttack.text    = "" ;
            m_hp.text           = "" ;
            m_occuption.text    = "";
            m_type.text         = "";
            m_rarity.mainTexture = null;
            m_levelTips.text    = "" ;

            // 清空下
            m_cardBG.mainTexture = null;
            // 隐藏突破信息
            m_break.gameObject.SetActive(false);
            // 隐藏提示信息
            m_warning.gameObject.SetActive(false);

            // 隐藏 等级进度条
            m_levelBefore.gameObject.SetActive(false);
            // 隐藏 升级后的进度条
            m_levelAfter.gameObject.SetActive(false);

            m_attribute.gameObject.SetActive(false);
        }
       CSItem card           = CardBag.Singleton.GetCardByGuid(CardUpdateProp.Singleton.m_curLevelGuid);
       if ( null == card)
       {
           return;
       }

       HeroInfo heroInfo     = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
       if (null == heroInfo)
        {
            return;
        }
        OccupationInfo occupationInfo   = GameTable.OccupationInfoAsset.LookUp(heroInfo.Occupation);

        RaceInfo raceInfo               = GameTable.RaceInfoTableAsset.LookUp(heroInfo.Type);

        m_cost.text         = "" + heroInfo.Cost;
        m_name.text         = heroInfo.StrName;
        m_phyAttack.text    = "" + card.GetPhyAttack();
        m_magAttack.text    = "" + card.GetMagAttack();
        m_hp.text           = "" + card.GetHp();
        m_occuption.text    = occupationInfo.m_name;
        m_type.text         = raceInfo.m_name;
       
        m_levelTips.text        = "" + card.Level;

        RarityRelativeInfo rarityInfo       = GameTable.RarityRelativeAsset.LookUp(heroInfo.Rarity);
        IconInfomation icon                 = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);

        m_rarity.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);
        m_attribute.gameObject.SetActive(true);
    }

    void UpdateInfo()
    {
        UpdateCardInfo();

        // 如果 升级卡牌  无效则 清空相关UI
        if (CardUpdateProp.Singleton.m_curLevelGuid.Equals(CSItemGuid.Zero))
        {
            // 隐藏材料 列表
            for (int i = 0; i < m_listMax; i++)
            {
                Parma parma = FindChildComponent<Parma>("" + i);
                if (null == parma)
                {
                    continue;
                }
                parma.m_guid = CSItemGuid.Zero;
                parma.gameObject.SetActive(false);
            }
        }

        // 选择数
        m_chosen.text   = string.Format(Localization.Get("ChooeseNum"), OperateCardList.Singleton.LevelUpList.Count);
        // 持有数
        m_hold.text     = CardBag.Singleton.itemList.Count + "/" + CardBag.Singleton.GetBagCapcity();
        // 升级的 基本卡牌
        CSItem card     = CardBag.Singleton.GetCardByGuid(CardUpdateProp.Singleton.m_curLevelGuid);
        if ( null ==  card )
        {
            return;
        }
        HeroInfo info          = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
        if (null == info)
        {
            return;
        }

        IconInfomation imageInfo     = GameTable.IconInfoTableAsset.Lookup(info.ImageId);

        if ( null == imageInfo )
        {
            return;
        }

        LevelUpInfo levInfo    = GameTable.LevelUpTableAsset.LookUp(card.Level);

        if ( null == levInfo )
        {
            return;
        }

        // 记录升级前的各种数据 用于升级后的显示
        CardUpdateProp.Singleton.SetOldData();

        float process       = (float)card.Exp / (float)levInfo.Monster_Exp;
        m_processBar.value  = process;

        m_expPercent.text   = card.Exp*100/levInfo.Monster_Exp+"%";
        m_cardBG.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(imageInfo.dirName);

        m_levelTips.text        = "LV" + card.Level;
        m_tupoCounts.text   = "" + (info.BreakThroughCount - card.BreakCounts);

        // 需消耗的金币
        int money           = card.Level * OperateCardList.Singleton.LevelUpList.Count;
        m_costMoney.text    = "" + money;
        // 持有金币
        m_myMoney.text      = "" + User.Singleton.GetMoney();

        if (money > User.Singleton.GetMoney())
        {
            m_costMoney.color = Color.red;
        }
        else
        {
            m_costMoney.color = Color.white;
        }

        // 基本卡牌的
        int baseCardId      = card.IDInTable;

        int index           = 0;
        bool bBreak         = false; // 是否显示突破信息
        bool bWarning       = false; // 是否显示提示有三星以上卡牌 作为材料
        // 材料所提供的经验
        int supplyExp       = 0;

        // 将要突破的次数
        int willBreakCounts = 0;

        // 材料卡牌列表
        foreach (CSItemGuid item in OperateCardList.Singleton.LevelUpList)
        {
            //
            Parma parma    = FindChildComponent<Parma>("" + index);
            if (null == parma)
            {
                continue;
            }

            GameObject obj      = parma.gameObject;
            obj.SetActive(true);
            card                = CardBag.Singleton.GetCardByGuid(item);
            if (null == card)
            {
                continue;
            }

            // 如果材料卡牌和 基本卡牌一样的 则 显示界限突破 信息
            if (card.IDInTable == baseCardId)
            {
                bBreak = true;
                willBreakCounts++;
            }
            info                = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);

            if (null == info)
            {
                continue;
            }

            // 如果是三星以上卡牌
            if(info.Rarity >=3)
            {
                bWarning = true;
            }

            // guid
            parma.m_guid        = item;

            imageInfo           = GameTable.IconInfoTableAsset.Lookup(info.headImageId);

            // 提供的经验
            supplyExp           = supplyExp + CardUpdateProp.Singleton.GetCardExpByLevel((int)card.IDInTable, card.Level);

            if (null == imageInfo)
            {
                continue;
            }

            //  头像
            obj.transform.Find("Image").GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(imageInfo.dirName);

            RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(info.Rarity);

            if (null != rarityInfo)
            {
                imageInfo = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);
                //  星级
                obj.transform.Find("Rank").GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(imageInfo.dirName);
            }
           
            //  名称
            obj.transform.Find("Name").GetComponent<UILabel>().text            = info.StrName;
            //  等级
            obj.transform.Find("Level").GetComponent<UILabel>().text           = "" + card.Level;      

            index++;
        }

        // 三星材料卡提示
        m_warning.gameObject.SetActive(bWarning);
 
        // 升级后的 等级
        int afterLV     = 0;
        // 升级后的经验
        int afterExp    = 0;

        card            = CardBag.Singleton.GetCardByGuid(CardUpdateProp.Singleton.m_curLevelGuid);

        CardUpdateProp.Singleton.GetLevelUpLVExp(card, supplyExp,willBreakCounts, out afterLV, out afterExp);

        // 经验
        // 只显示 当前经验 并不播放动画
        if (afterLV == 0 )
        {
            m_levelBefore.gameObject.SetActive(true);
            m_levelBefore.GetComponent<Animation>().Stop();
            Color color         = Color.white;
            color.a             = 1;
            m_levelBefore.color = color;
            m_levelBefore.GetComponent<TweenAlpha>().enabled = false;
            m_levelAfter.gameObject.SetActive(false);
        }

        // 当前经验和 升级后的经验 同时播放动画
        else
        {
           // m_levelBefore.animation.Play("ui-cardstate-00");
           // m_levelAfter.animation.Play("ui-cardstate-01");
            m_levelBefore.GetComponent<TweenAlpha>().enabled = true;
            levInfo             = GameTable.LevelUpTableAsset.LookUp(afterLV);
            int afterMaxExp     = levInfo.Monster_Exp;
            int per             = afterExp * 100 / afterMaxExp;
            m_levelAfter.gameObject.transform.Find("LevelTipsAfter").GetComponent<UILabel>().text      = Localization.Get("CardLevel") + afterLV;
            if (per>=100)
            {
                per = 100;
            }
            m_levelAfter.gameObject.transform.Find("LevelPercentAfter").GetComponent<UILabel>().text   =  per +"%";

            m_levelAfter.gameObject.transform.Find("ExpAfter").GetComponent<UISlider>().value          = ((float)per)/100f;

            m_levelAfter.gameObject.SetActive(true);
      
        }

        // 突破信息显示
        m_break.gameObject.SetActive(bBreak);

        // 隐藏其余的 列表
        for (; index < m_listMax; index++)
        {
            Parma parma = FindChildComponent<Parma>("" + index);
            if (null == parma)
            {
                continue;
            }
            parma.m_guid = CSItemGuid.Zero;
            parma.gameObject.SetActive(false);
        }

        // 是否激活合成按钮 条件 是 基本卡可强化 有材料 够金钱
        m_levelUpBtn.isEnabled = (OperateCardList.Singleton.LevelUpList.Count > 0) /*&&(money <= User.Singleton.GetMoney())*/;

        // 进化合成按钮是否激活
        m_evolution.isEnabled = card.IsEvlotion(); 
   
        m_grid.Reposition();
        // 计算消耗金钱

        // 把列表位置 提到最上面
        m_listPanel.transform.Find("Progress").GetComponent<UIProgressBar>().value = 0;

    }

    // 材料列表中的 取消按钮
    void OnCancelDataCard(object sender, EventArgs e)
    {
        GameObject obj      = (GameObject)sender;

        GameObject parent   = obj.transform.parent.gameObject;

        Parma param    = parent.GetComponent<Parma>();
        if (null == param)
        {
            return;
        }

        OperateCardList.Singleton.RemoveLevelUpDataItem(param.m_guid);

        UpdateInfo();
    }

    // 合成
    void OnLevelUp(object sender, EventArgs e)
    {

       if (CardUpdateProp.Singleton.m_curLevelGuid.Equals(CSItemGuid.Zero))
        {
            Debug.Log("UICardUpdate: 当前没有选择基本卡牌");
            return;
        }

        if (OperateCardList.Singleton.LevelUpList.Count == 0)
        {
            Debug.Log("UICardUpdate: 当前没有选择升级所用的材料卡牌");
            return;
        }

        // loading 
        Loading.Singleton.SetLoadingTips((int)LOADINGTIPSENUM.enJumpToBag);

        // 发送服务器
		IMiniServer.Singleton.req_herocardMerge(CardUpdateProp.Singleton.m_curLevelGuid, OperateCardList.Singleton.LevelUpList);

        Debug.Log("OnLevelUp m_curLevelGuid:" + CardUpdateProp.Singleton.m_curLevelGuid.m_lowPart + "," + CardUpdateProp.Singleton.m_curLevelGuid.m_highPart);
        //CardUpdateProp.Singleton.OnLevelUp();
    }

    // 更新 强化成功信息 并显示出来
    void UpdateSuccessInfo()
    {
        m_success.gameObject.SetActive(true);

        CSItem card         = CardBag.Singleton.GetCardByGuid(CardUpdateProp.Singleton.m_levelUpAfterGuid);
        if ( null == card )
        {
            return;
        }

        HeroInfo info       = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
        if (null == info)
        {
            return;
        }

        IconInfomation imageInfo  = GameTable.IconInfoTableAsset.Lookup(info.ImageId);

        if (null == imageInfo)
        {
            return;
        }

        // 突破次数
        int breakNum        = card.BreakCounts - CardUpdateProp.Singleton.m_oldBreak ;
        if (breakNum>0)
        {
            m_successBreak.text = string.Format(Localization.Get("BreakNum"), breakNum); 
        }
        else
        {
            m_successBreak.text = "";
        }

        m_successCardBG.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(imageInfo.dirName);

        // 升级前的数据
        m_oldLv.text        = CardUpdateProp.Singleton.m_oldLevelTips;
        m_oldHp.text        = "" + CardUpdateProp.Singleton.m_oldHP;
        m_oldMagAttack.text = "" + CardUpdateProp.Singleton.m_oldMagAtk;
        m_oldPhyAttack.text = "" + CardUpdateProp.Singleton.m_oldPhyAtk;

        // 升级后的数据
        m_newLv.text        = card.Level + "/" + card.GetMaxLevel();
        m_newHp.text        = ""+(int)card.GetHp();
        m_newMagAttack.text = "" + card.GetMagAttack();
        m_newPhyAttack.text = "" + card.GetPhyAttack();
    }


    // 显示解锁技能信息
    void OnShowUnlockSkill()
    {
       if (CardUpdateProp.Singleton.m_unlockSkillList.Count == 0)
       {
           return;
       }

      int skillId =  CardUpdateProp.Singleton.m_unlockSkillList[0];

        // 显示

        // 移除
      CardUpdateProp.Singleton.m_unlockSkillList.Remove(skillId);
    }
}
