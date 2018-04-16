using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIBattleSummary : UIWindow
{
    public enum ENSTAGE
    {
        enStage1 = 1,   // 动画阶段1
        enStage2,       // 动画阶段2
        enStage3,       // 动画阶段3
        enStage4,       // 动画阶段4
    }

    UILabel m_floorName     = null;
    //UITexture m_item = null;
    UILabel m_coin          = null;
    UILabel m_exp           = null;
    UILabel m_totalScore    = null;
    UILabel m_bossKilled    = null;
    UILabel m_elapsedTime   = null;
    UILabel m_maxCombo      = null;
    UILabel m_reliveCount   = null;
    UILabel m_bossKilledScore   = null;
    UILabel m_elapsedTimeScore  = null;
    UILabel m_maxComboScore     = null;
    UILabel m_reliveCountScore  = null;
    UILabel m_coinMultiply      = null;
    UILabel m_expMultiply       = null;
    UILabel m_level             = null;
    UISlider m_expBar           = null;
    UILabel m_expPercent        = null;
    UILabel m_expDetail         = null;

    GameObject m_prizeCard      = null;

    GameObject m_cardGrid       = null;

    GameObject m_quitBox        = null;
    GameObject m_fastBox        = null;

    Dictionary<GameObject, UICardHead> m_prizeCardList      = new Dictionary<GameObject, UICardHead>();// 此列表是管理资源
    Dictionary<GameObject, UICardHead> m_prizeCardDataList  = new Dictionary<GameObject, UICardHead>();// 次列表用来显示 可以随意删除 其中项而不用担心 资源没有被释放


    //int m_itemIndex             = 0;

    List<UITexture> m_itemList   = null;

    int m_currentCorroutineIndex= 0;

    

    static public UIBattleSummary GetInstance()
	{
        UIBattleSummary self = UIManager.Singleton.GetUIWithoutLoad<UIBattleSummary>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIBattleSummary>("UI/UISummary", UIManager.Anchor.Center);
		return self;
	}

	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enBattelSummary, OnPropertyChanged);

        m_floorName         = FindChildComponent<UILabel>("FloorName");
//        m_item              = FindChildComponent<UITexture>("Item");
        //m_clearImage        = FindChildComponent<UISprite>("Clear");

        m_coin              = FindChildComponent<UILabel>("Coin");
        m_exp               = FindChildComponent<UILabel>("Exp");
        m_totalScore        = FindChildComponent<UILabel>("TotalScore");
        m_bossKilled        = FindChildComponent<UILabel>("BossKilledNumber");
        m_elapsedTime       = FindChildComponent<UILabel>("ElapsedTimeNumber");
        m_maxCombo          = FindChildComponent<UILabel>("MaxComboNumber");
        m_reliveCount       = FindChildComponent<UILabel>("ReliveCountNumber");
        m_bossKilledScore   = FindChildComponent<UILabel>("BossKilledScore");
        m_elapsedTimeScore  = FindChildComponent<UILabel>("ElapsedTimeScore");
        m_maxComboScore     = FindChildComponent<UILabel>("MaxComboScore");
        m_reliveCountScore  = FindChildComponent<UILabel>("ReliveCountScore");
        m_coinMultiply      = FindChildComponent<UILabel>("CoinMultiply");
        m_expMultiply       = FindChildComponent<UILabel>("ExpMultiply");
        m_level             = FindChildComponent<UILabel>("PlayerLevel");
        m_expBar            = FindChildComponent<UISlider>("ExpBar");
        m_expPercent        = FindChildComponent<UILabel>("ExpPercent");
        m_expDetail         = FindChildComponent<UILabel>("ExpDetail");

        m_quitBox           = FindChild("UIBattleDetailSummaryBG3");

        m_fastBox           = FindChild("UIBattleDetailSummaryBG2");

        m_prizeCard         = FindChild("Card");

        m_cardGrid          = FindChild("Grid");

        m_itemList          = new List<UITexture>();

       
	}

    public override void AttachEvent()
    {
        base.AttachEvent();
        //AddChildMouseClickEvent("Confirm", OnReturn);
        AddChildMouseClickEvent("UIBattleDataSummaryBG", OnQuickOne);
        AddChildMouseClickEvent("UIBattleDetailSummaryBG1", OnQuickTwo);
        AddChildMouseClickEvent("UIBattleDetailSummaryBG2", OnQuickThree);

        AddChildMouseClickEvent("UIBattleDetailSummaryBG3", OnReturn);
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (null == WindowRoot)
        {
            return;
        }

        if (eventType == (int)BattleSummary.ENPropertyChanged.enShow)
        {
            ShowWindow();
            UpdateInfo();
        }
        else if (eventType == (int)BattleSummary.ENPropertyChanged.enTick)
        {
            m_tick = true;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        foreach (UITexture item in m_itemList)
        {
            GameObject.Destroy(item.gameObject);
        }

        foreach (KeyValuePair<GameObject,UICardHead> item in m_prizeCardList)
        {
            GameObject.Destroy(item.Key);

            item.Value.Destroy();
        }

        m_prizeCardList.Clear();

        m_prizeCardDataList.Clear();

    }
    void OnReturn(GameObject obj)
    {
        UILabel rank = FindChildComponent<UILabel>(BattleSummary.Singleton.m_curRank);
        if (rank != null)
        {
            rank.gameObject.SetActive(false);
        }

        m_tick  = false;

        //  进入floor前 如果背包 获得卡牌 大于背包容量 则 跳转
        if (CardBag.Singleton.IsPopSizeMax())
        {
            PopWnd.Singleton.Show();
        }

        m_quitBox.SetActive(false);
        BattleSummary.Singleton.m_fastPrizeCard = false;

        // 退出结算界面后才转到 主界面
        StateMainUI mainState           = new StateMainUI();
        MainGame.Singleton.TranslateTo(mainState);

        HideWindow();
    }

    // 结算界面 一开始 点击加速或者播放下一段动画
    void OnQuickOne(GameObject obj)
    {
        // 加速
        SetAnimationSpeed("ui-summary-00", 10.0f);
    }

    // 结算界面 第二段 点击加速或者播放下一段动画
    void OnQuickTwo(GameObject obj)
    {
        SetAnimationSpeed("ui-summary-01", 10.0f);

    }

    // 结算界面 第三阶段 点击加速或者播放下一段动画
    void OnQuickThree(GameObject obj)
    {
        BattleSummary.Singleton.m_fastPrizeCard = true;
    }

    // 播放动画
    public void PlayStageAnimation(ENSTAGE stage)
    {
        string animationName = "";
        switch (stage)
        {
            case ENSTAGE.enStage1:
                animationName = "ui-summary-00";
                break;
            case ENSTAGE.enStage2:
                animationName = "ui-summary-01";
                break;
            case ENSTAGE.enStage3:
                animationName = "ui-summary-02";
                m_tick = true;
                break;
        }

        SetAnimationSpeed(animationName, 1.0f);
        if (null != WindowRoot)
        {
            this.WindowRoot.GetComponent<Animation>().Play(animationName);
        }

  
        AnimationClip clip = WindowRoot.GetComponent<Animation>().GetClip(animationName);
        MainGame.Singleton.StartCoroutine(CoroutineAnimationEnd(++m_currentCorroutineIndex, clip.length, (ENSTAGE)(stage+1)));
    }
  

    void UpdateInfo()
    {
        FloorInfo info = GameTable.FloorInfoTableAsset.LookUp(StageMenu.Singleton.m_curFloorId);
        if ( null == info )
        {
            Debug.Log("UIBattleSummary UpdateInfo FloorInfo info==null BattleSu mmary.Singleton.m_floorId=" + StageMenu.Singleton.m_curFloorId);
            return;
        }
        m_floorName.text    = info.m_name;
       

        m_maxCombo.text     = BattleSummary.Singleton.m_maxComboCount.ToString();
        m_reliveCount.text  = BattleSummary.Singleton.m_reliveCount.ToString();
        m_bossKilled.text   = BattleSummary.Singleton.m_killBossCount.ToString();
        m_elapsedTime.text  = BattleSummary.Singleton.GetPassStageTime();


        int killedBossScore     = BattleSummary.Singleton.GetKilledBossScore();
        int elapsedTimeScore    = BattleSummary.Singleton.GetElapsedTimeScore();
        int maxComboScore       = BattleSummary.Singleton.GetMaxComboScore();
        int reliveCountScore    = BattleSummary.Singleton.GetReliveScore();
        int totalScore          = killedBossScore+elapsedTimeScore+maxComboScore+reliveCountScore;

        //Debug.Log("killedBossScore:" + killedBossScore + ",elapsedTimeScore:" + elapsedTimeScore + ",maxComboScore:" + maxComboScore + ",reliveCountScore:" + reliveCountScore + ",reliveCountScore:" + reliveCountScore);

        m_bossKilledScore.text  = killedBossScore.ToString();
        m_elapsedTimeScore.text = elapsedTimeScore.ToString();
        m_maxComboScore.text    = maxComboScore.ToString();
        m_reliveCountScore.text = reliveCountScore.ToString();

        m_totalScore.text       = totalScore.ToString();
        string strRank = BattleSummary.Singleton.GetRank(totalScore, StageMenu.Singleton.m_curFloorId);


        BattleSummary.Singleton.m_curRank   = strRank;
        UILabel rank = FindChildComponent<UILabel>(strRank);
        if ( rank != null )
        {
            rank.gameObject.SetActive(true);
        }

       m_coinMultiply.text  =  BattleSummary.Singleton.GetMoneyParam().ToString();
       m_coin.text          =  BattleSummary.Singleton.GetBaseMoney().ToString();
       m_exp.text           =  BattleSummary.Singleton.GetBaseExp().ToString();
       m_expMultiply.text   =  BattleSummary.Singleton.GetExpParam().ToString();

        SetAnimationSpeed("ui-battlevictory-00",1.0f);
        AnimationClip clip = WindowRoot.GetComponent<Animation>().GetClip("ui-battlevictory-00");

        // 当 第一段动画播完后 没有任何操作则 播放第二段动画
        MainGame.Singleton.StartCoroutine(CoroutineAnimationEnd(++m_currentCorroutineIndex, clip.length, ENSTAGE.enStage1));
                        

        foreach (UITexture item in m_itemList)
        {
            GameObject.Destroy(item.gameObject);
        }
        m_itemList.Clear();

        PlayerAttrInfo playerAttrInfo = GameTable.playerAttrTableAsset.LookUp(User.Singleton.GetLevel());
        if ( null == playerAttrInfo )
        {
            return;
        }     

        int needExp         = playerAttrInfo.m_needExp; 
        int exp             = User.Singleton.GetExp();
        m_expDetail.text    = exp.ToString() + "/" + needExp.ToString();

        m_expPercent.text   = exp * 100 / needExp + "%";

    }

    //动画播完后的操作 stage 为下一阶段要播放的动画
    IEnumerator CoroutineAnimationEnd(int animCoroutineIndex, float timeLength,ENSTAGE stage)
    {
         yield return new WaitForSeconds(timeLength+1);
        if ( animCoroutineIndex == m_currentCorroutineIndex)
		{
            if (stage == ENSTAGE.enStage4)
            {
                ShowPrizeCard();
            }
            else
            {
                PlayStageAnimation(stage);
            }
        }
    }

    //设置动画的速度
    void SetAnimationSpeed( string name,float speed )
    {
        if (null == WindowRoot)
        {
            return;
        }
        
        AnimationState state = this.WindowRoot.GetComponent<Animation>()[name];
        state.speed = speed;
    }
    
    // 经验数值上涨的 变化
    void ExpUp()
    {
        if ( false == BattleSummary.Singleton.m_expChange )
        {
            return;
        }

        int exp     = BattleSummary.Singleton.m_desExp;
        int oldExp  = BattleSummary.Singleton.m_oldExpNum;

        if (oldExp >= exp)
        {
            BattleSummary.Singleton.m_expChange = false;
            oldExp = exp;
        }

        m_exp.text = oldExp.ToString();
        BattleSummary.Singleton.m_oldExpNum = oldExp + BattleSummary.Singleton.m_expChangePerFPS;

    }

    // 金钱数值上涨的 变化
    void MoneyUp()
    {
        if (false == BattleSummary.Singleton.m_moneyChange)
        {
            return;
        }

        int money       = BattleSummary.Singleton.m_desMoney;
        int oldMoney    = (int)BattleSummary.Singleton.m_oldMoney;

        if (oldMoney >= money)
        {
            BattleSummary.Singleton.m_moneyChange = false;
            oldMoney = money;
        }

        m_coin.text     = oldMoney.ToString();

        BattleSummary.Singleton.m_oldMoney = oldMoney + BattleSummary.Singleton.m_moneyChangePerFPS;
    }

    // 经验条 上涨的 变化
    void ExpBarUp()
    {
        if (false == BattleSummary.Singleton.m_expBarChange)
        {
            return;
        }
       
        int level   =  User.Singleton.GetLevel();
        int exp     =  User.Singleton.GetExp();

       
       
        int oldExp  =  BattleSummary.Singleton.m_oldExp;
        int oldLevel= BattleSummary.Singleton.m_oldLevel;

        // 是否显示 技能升级界面
        bool bShowSkillLevelUp = true;
        
        // 等级没有变化则 直接涨到目地 数值即可
        if ( level == BattleSummary.Singleton.m_oldLevel )
        {
            if (oldExp >= exp)
            {
                BattleSummary.Singleton.m_expBarChange = false;
                oldExp = exp;
            }

            BattleSummary.Singleton.m_oldExp = oldExp + 20;

            PlayerAttrInfo playerAttrInfo = GameTable.playerAttrTableAsset.LookUp(oldLevel);
            if (null == playerAttrInfo)
            {
                return;
            }

            int needExp = playerAttrInfo.m_needExp;

            m_expBar.value = (float)oldExp  / (float)needExp;
            m_level.text    = Localization.Get("CardLevel") + level.ToString();

            m_expDetail.text    = exp.ToString() + "/" + needExp.ToString();
            m_expPercent.text   = exp * 100 / needExp + "%";

           //Debug.LogError("oldExp:" + oldExp + ",needExp:" + needExp + " m_expBar.value:" + m_expBar.value + ",exp:" + exp);
           
        }
        // 如果等级变化了 则先 涨满 相应的 等级经验 然后再涨到相应目标数据
        else
        {
            oldExp = 0;
            if (BattleSummary.Singleton.m_oldLevel < level)
            {
              
                if (m_expBar.value >= 0.9)
                {
                    m_expBar.value                      = 0f;
                    BattleSummary.Singleton.m_oldLevel  = BattleSummary.Singleton.m_oldLevel + 1;
                    m_level.text                        = Localization.Get("CardLevel") + BattleSummary.Singleton.m_oldLevel.ToString();
                }
                else
                {
                    m_expBar.value = m_expBar.value + 0.1f;
                }
                //Debug.Log("ExpBarUp m_expBar.value:" + m_expBar.value);
            }

            // 如果升级了

            if (BattleSummary.Singleton.m_oldLevel >= level)
            {
                // 让下一阶段动画停止播放
                m_currentCorroutineIndex++;

                // 经验涨完 后显示升级界面
                LevelUp.Singleton.Show();

                bShowSkillLevelUp = false;

            }
            //Debug.LogError(BattleSummary.Singleton.m_oldLevel + ",level:" + level);
        }

        if(bShowSkillLevelUp)
        {
            // 显示技能升级
            if ( LevelUp.Singleton.m_skillList.Count >0 )
            {
                // 让下一阶段动画停止播放
                m_currentCorroutineIndex++;

                LevelUp.Singleton.ShowSkill();
            }
        }
    }

    override public void OnUpdate()
    {
        ExpUp();

        MoneyUp();

        ExpBarUp();
    }

    void ShowPrizeCard()
    {
       foreach ( KeyValuePair<GameObject,UICardHead> item in m_prizeCardList )
       {
           GameObject.Destroy(item.Key);
           item.Value.Destroy();
       }
       m_prizeCardList.Clear();
       m_prizeCardDataList.Clear();

       MainGame.Singleton.StartCoroutine(CoroutinePrizeCard());

    }

     // 
    IEnumerator CoroutinePrizeCard()
    {
        yield return new WaitForSeconds(0);
//         BattleArena.Singleton.DropCardsIdList.Add(2);
//         BattleArena.Singleton.DropCardsIdList.Add(4);
//         BattleArena.Singleton.DropCardsIdList.Add(5);
//         BattleArena.Singleton.DropCardsIdList.Add(5);
// 
//         BattleSummary.Singleton.SummaryGetNewCard(2);
//         BattleSummary.Singleton.SummaryGetNewCard(4);
//         BattleSummary.Singleton.SummaryGetNewCard(5);
//         BattleSummary.Singleton.SummaryGetNewCard(5);

        foreach (int cardId in BattleArena.Singleton.DropCardsIdList)
        {
            GameObject item                 = GameObject.Instantiate(m_prizeCard) as GameObject;

            item.transform.parent           = m_cardGrid.transform;

            item.transform.localPosition    = Vector3.zero;
            item.transform.localScale       = Vector3.one;

            int id = cardId;

            // 容错卡牌ID
            if ( GameTable.HeroInfoTableAsset.Lookup(id) == null )
            {
                id = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enDefaultCardId).IntTypeValue;
            }

            bool bNew = BattleSummary.Singleton.IsNewCard(id);

            // 如果是新的
            item.transform.Find("New").gameObject.SetActive(bNew);

            Debug.Log(bNew);
        
            UICardHead tempHead = UICardHead.Create();

            tempHead.SetParent(item.transform);
            tempHead.SetLocalPosition(Vector3.zero);
            tempHead.RegisterCallBack(PressCardHead, null);
            CSItem tempCard         = new CSItem();
            tempCard.m_id           = (short)id;
            tempCard.Level          = 1;
            tempHead.SetCardInfo(tempCard);
            tempHead.SetCardLoveShow(false);
            tempHead.SetCardInfoShow(false);
            tempHead.SetCardNew(bNew);
            tempHead.HideWindow();


            item.SetActive(false);

            m_prizeCardList.Add(item, tempHead);
            m_prizeCardDataList.Add(item, tempHead);
        }

        m_cardGrid.GetComponent<UIGrid>().Reposition();


        // 先显示
        foreach (KeyValuePair<GameObject,UICardHead> item in m_prizeCardList)
        {
            item.Key.SetActive(true);

            // 如果快速
            if ( BattleSummary.Singleton.m_fastPrizeCard )
            {
                yield return new WaitForSeconds(0f); 
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
           
        }

        // 再一个一个播动画
        MainGame.Singleton.StartCoroutine(CoroutinePlayAnmiationPrizeCard());
    }

    //长按显示卡牌详情界面 只带返回按钮
    void PressCardHead(CSItem card, int materialId)
    {
        // 下面代码的意思是 在显示的时候才创建UICardDetail界面
        UICardDetail.GetInstance();
        CardBag.Singleton.OnShowCardDetail(card,true);

    }    

    // 播放每个奖励的动画
    IEnumerator CoroutinePlayAnmiationPrizeCard()
    {
        if (m_prizeCardDataList.Count == 0 )
        {
            yield return new WaitForSeconds(0);

            // 
            m_quitBox.SetActive(true);
            m_fastBox.SetActive(false);
        }
        else
        {
            GameObject obj  = null;
            // 是否继续显示 
            bool bShowNext  = true;
            foreach (KeyValuePair<GameObject, UICardHead> item in m_prizeCardDataList)
            {

                // 如果 平常速度
                if ( !BattleSummary.Singleton.m_fastPrizeCard )
                {
                    item.Key.GetComponent<Animation>().Play("ui-battledatasummarycontract-00");

                    AnimationClip clip = item.Key.GetComponent<Animation>().GetClip("ui-battledatasummarycontract-00");

                    // 
                    yield return new WaitForSeconds(clip.length);

                    // 播完动画 显示头像
                    item.Value.ShowWindow();
                    item.Value.ShowCard();

                    item.Value.SetCardLoveShow(false);
                    item.Value.SetCardInfoShow(false);

                    

                    if (item.Value.m_showCard != null)
                    {
                        // 如果 是新卡 则要 显示卡牌详情
                        if ( item.Value.m_bNew )
                        {
                            UICardDetail.GetInstance().RegisterCallBack(CardDetailCallBack);
                            CardBag.Singleton.OnShowCardDetail(item.Value.m_showCard,true);
                            bShowNext = false;
                        }

                    }
                }
                 // 否则快速
                else
                { 
                    //  显示头像
                    item.Value.ShowWindow();
                    item.Value.ShowCard();

                    item.Value.SetCardLoveShow(false);
                    item.Value.SetCardInfoShow(false);

                }
                obj = item.Key;

                obj.transform.Find("Contract").gameObject.SetActive(false);
                break;
            }

            if (null != obj)
            {

                m_prizeCardDataList.Remove(obj);
            }
            if (bShowNext)
            {
                MainGame.Singleton.StartCoroutine(CoroutinePlayAnmiationPrizeCard());
            }
            
        }
        
    }

    // 卡牌详情 返回的回调函数
    void CardDetailCallBack()
    {
        // 再一个一个播动画
        MainGame.Singleton.StartCoroutine(CoroutinePlayAnmiationPrizeCard());
    }
}
