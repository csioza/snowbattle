using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UICardBag : UIWindow
{

    //GameObject m_item = null;
    UILabel m_capacity          = null;
    UILabel m_maxCapacity       = null;
    
    UIPanel m_selectLayout      = null;
    UIPanel m_option            = null;
    UILabel m_optionName        = null;
    UILabel m_optionLevel       = null;
    UILabel m_optionHp          = null;
    UILabel m_optionMagAttack   = null;
    UILabel m_optionPhyAttack   = null;
    UILabel m_optionRace        = null;
    //UIPanel m_itemPanel         = null;

    UILabel m_opInfoOp          = null;
    //int m_curLine               = 0;// 当前包行数

    UIGrid m_grid               = null;

    UICardSort m_cardSortUI;

    UILabel m_cardSortLable     = null;

    UIProgressBar m_bagBar      = null;

    // 格子列表 ID，格子UI
    Dictionary<int, UICardItem> m_gridList = null;

    public Texture m_gridBgTexture         = null; // 道具格子的背景图
    public Texture m_extendBagTexture      = null; // 扩充背包的图片

    GameObject m_seleteCardObj      = null;

    //营地界面 卡牌详细信息 added by wangxin 
    //UIButton m_infoReturn = null;//卡牌详细信息界面 返回按钮
    //UIButton m_joinLoveButton = null;//加入最爱按钮
    UITexture m_cardInfoOccupation = null;//职业图标
    UITexture m_cardInfoRace = null;//种族图标
    UITexture m_cardInfoMain = null;//头像图标
    UITexture m_rarityTexture = null;//星级图标
    UISprite m_cardInfoLove = null;//加入最爱图片
    UISprite m_joinLoveSprite = null;//加入最爱按钮图片
    UIButton m_stengThen = null;//强化卡牌按钮
    UIButton m_evlotion = null;//段位升级按钮
    UILabel m_deputyLabel = null;//代表显示标签
    UILabel m_chosenLable = null;//使用中标签

    GameObject m_dragBox = null;

    static public UICardBag GetInstance()
	{
        UICardBag self = UIManager.Singleton.GetUIWithoutLoad<UICardBag>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UICardBag>("UI/UICardBag", UIManager.Anchor.Center);
		return self;
	}

    public override void OnDestroy()
    {
        base.OnDestroy();


        foreach (KeyValuePair<int, UICardItem> item in m_gridList)
        {
            item.Value.Destroy();
        }

        m_cardSortUI.Destroy();

        m_gridBgTexture     = null;

        m_extendBagTexture  = null;


    }
	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enCardBag, OnPropertyChanged);

        m_gridList      = new Dictionary<int, UICardItem>();

        m_capacity  = FindChildComponent<UILabel>("Capacity");
        m_maxCapacity = FindChildComponent<UILabel>("MaxCapacity");

        m_selectLayout  = FindChildComponent<UIPanel>("SelectLayout");

        m_option        = FindChildComponent<UIPanel>("Option");
        m_optionName = FindChildComponent<UILabel>("CardInfoname");
        m_optionLevel = FindChildComponent<UILabel>("CardInfoLevel");
        m_optionHp      = FindChildComponent<UILabel>("CardInfoRhp");
        m_optionMagAttack = FindChildComponent<UILabel>("CardInfoRmagAttack");
        m_optionPhyAttack = FindChildComponent<UILabel>("CardInfoRphyAttack");
        m_optionRace = FindChildComponent<UILabel>("CardInfoRace_name");


        m_opInfoOp = FindChildComponent<UILabel>("CardInfoOccupation_name");
        m_grid          = FindChildComponent<UIGrid>("GridList");
        m_cardSortLable = FindChildComponent<UILabel>("CardSortLabel");
//        m_itemPanel     = FindChildComponent<UIPanel>("List");
        m_bagBar        = FindChildComponent<UIProgressBar>("BagBar");

        m_dragBox       = FindChild("DragBox");

        EventDelegate.Add(m_bagBar.onChange, OnScrollBar);

        m_cardSortUI    = UICardSort.Load();
        m_cardSortUI.SetParentWnd(this);
		m_cardSortUI.SortCards(ENSortType.enDefault, true);

        InitBag();
        UpdateInfo();
     

        WorldParamInfo worldParamInfo   = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enBagGridBgIconId);
        IconInfomation iconInfo         = GameTable.IconInfoTableAsset.Lookup(worldParamInfo.IntTypeValue);
        m_gridBgTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);

        worldParamInfo                  = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enBagExpandIconId);
        iconInfo                        = GameTable.IconInfoTableAsset.Lookup(worldParamInfo.IntTypeValue);
        m_extendBagTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
	    //营地卡牌详细信息界面
//        m_infoReturn = FindChildComponent<UIButton>("Button_return");
        m_cardInfoRace = FindChildComponent<UITexture>("CardInfoRace");
        m_cardInfoOccupation = FindChildComponent<UITexture>("CardInfoOccupation");
        m_rarityTexture = FindChildComponent<UITexture>("rarityIcon");
        m_cardInfoMain = FindChildComponent<UITexture>("CardInfoMain");
        m_cardInfoLove = FindChildComponent<UISprite>("cardInfoLove");
//        m_joinLoveButton = FindChildComponent<UIButton>("Button_join_love");
        m_joinLoveSprite = FindChildComponent<UISprite>("join_love");
        m_stengThen = FindChildComponent<UIButton>("Button_strengthen_synthesis");
        m_evlotion = FindChildComponent<UIButton>("Button_dan_upgrade");
        m_deputyLabel = FindChildComponent<UILabel>("deputyLabel");
        m_chosenLable = FindChildComponent<UILabel>("ChosenLable");
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        
        if (eventType == (int)CardBag.ENPropertyChanged.enInitCardBag)
        {
            InitBag();

            UpdateInfo();
        }
        else if (eventType == (int)CardBag.ENPropertyChanged.enUpdateCardInBag)
        {
            if (WindowRoot.gameObject.activeSelf)
            {
                m_cardSortUI.SortCards(m_cardSortUI.GetLastSortType(), true);

                UpdateInfo();
            }
            
        }

        else if (eventType == (int)CardBag.ENPropertyChanged.enResetSelectedList)
        {
            UpdateInfo();
        }
        else if (eventType == (int)CardBag.ENPropertyChanged.enShowOption)
        {
            UpdateOption();
        }

        else if (eventType == (int)CardBag.ENPropertyChanged.enExpandCardBag)
        {
            ExpandCardBagGrid();
        }

		else if (eventType == (int)CardBag.ENPropertyChanged.enCardSort)
		{
            UpdateInfo();
		}
		else if (eventType == (int)CardBag.ENPropertyChanged.enCardSelect)
		{
            UpdateInfo();
		}		
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("Close", OnClose);
        AddChildMouseClickEvent("Expand", OnExpand);
        AddChildMouseClickEvent("GetNewCard", OnGetNewCard);
        AddChildMouseClickEvent("CardSort", OnCardSort);

        AddChildMouseClickEvent("OK", OnSelectOK);

        AddChildMouseClickEvent("Sell", OnSellCard);

        AddChildMouseClickEvent("Button_return", OnInfoReturn);//返回按钮

        AddChildMouseClickEvent("Button_quety_details", OnShowCardDetail);//卡牌详情

        AddChildMouseClickEvent("Button_join_love", OnAddLove);//加入最爱

        AddChildMouseClickEvent("Button_share", OnShare);//分享

        AddChildMouseClickEvent("Button_strengthen_synthesis", OnShowLevelUpCard);//强化卡牌

        AddChildMouseClickEvent("Button_promotion_synthesis", OnShowEvlotionCard);//进化合成

        AddChildMouseClickEvent("Button_dan_upgrade", OnUpgrade);//段位升级
    }

    void OnClose( GameObject obj )
    {

    
    }

    void OnExpand(GameObject obj)
    {
        ExpandBag.Singleton.ShowExpandBag();
    }

    void OnGetNewCard(GameObject obj)
    {
        CardBag.Singleton.RandomAddCard();
        m_cardSortUI.SortCards(ENSortType.enDefault, true);
    }
    void OnCardSort(GameObject obj)
    {
        m_cardSortUI.ShowOrHide();
    }

    void OnSelectConditions(GameObject obj)
    {
      m_selectLayout.gameObject.SetActive(!m_selectLayout.gameObject.activeSelf);
    }

    void OnSelectOK(GameObject obj)
    {
        m_selectLayout.gameObject.SetActive(!m_selectLayout.gameObject.activeSelf);
    }

    // 初始化背包
    public void InitBag()
    {
        for (int i = 1; i <= CardBag.Singleton.m_capacity; i++)
        {
            AddGrid(i); 
        }
        
    }

    // 增加格子 index 格子唯一索引 从1开始，x,y为位置
    void AddGrid( int index )
    {
        // 如果存在 则返回
        if (m_gridList.ContainsKey(index))
        {
            return;
        }

        UICardItem item             =  UICardItem.Create();

        // 设置 父物体
        item.SetParentWnd(FindChildComponent<UIGrid>("GridList").gameObject);

        string showNumStr           = index.ToString("00000");// String.Format("{0:00000}", index);
        
        item.SetRootName(showNumStr);

        m_gridList.Add(index, item);
    }

    void UpdateInfo()
    {
		// if hide then not update
		if(WindowRoot.activeSelf == false)
		{
			return;
		}
        m_capacity.text     = "" + CardBag.Singleton.m_cardNum;
        m_maxCapacity.text  = "/" + CardBag.Singleton.m_capacity;

        if (CardBag.Singleton.m_cardNum > CardBag.Singleton.m_capacity)
        {
            m_capacity.color = Color.red;  
        }
        else
        {
            m_capacity.color = Color.black;  
        }

        m_cardSortLable.text        = CardBag.Singleton.GetSrotString(m_cardSortUI.GetLastSortType());
        // 排序好的列表
        List<CSItemGuid> cardList   = m_cardSortUI.GetSortReslut();// CardBag.Singleton.GetSortedCardList();
        // 排序类型
        ENSortType sortType         = m_cardSortUI.GetLastSortType();

        int i = 1;
        foreach (CSItemGuid item in cardList)
        {
            CSItem card = CardBag.Singleton.GetCardByGuid(item);
            if (null == card)
            {
                continue;
            }

            int cardID          = card.IDInTable;
            HeroInfo heroInfo   = GameTable.HeroInfoTableAsset.Lookup(cardID);

            if (null == heroInfo)
            {
                Debug.LogWarning("heroInfo == NULL heroInfo cardID:" + cardID);
                continue;
            }
            // 职业刷选
            if (m_cardSortUI.GetSelectedOcc().Count != 0)
            {
                // 如果不是是选中的职业 则不显示
                if (!m_cardSortUI.GetSelectedOcc().Contains(heroInfo.Occupation))
                {
                    continue;
                }
            }

            // 种族筛选
            if (m_cardSortUI.GetSelectedRace().Count != 0)
            {
                // 如果不是是选中的职业 则不显示
                if (!m_cardSortUI.GetSelectedRace().Contains(heroInfo.Type))
                {
                    continue;
                }
            }


            if ( false == m_gridList.ContainsKey(i) )
            {
                CreateOneGird(i);
            }

            m_gridList[i].Update(card, sortType, i);

            i++;
        }

        // 把剩余的空格子图片置空
        for (; i <= m_gridList.Count; i++)
        {
            if (!m_gridList.ContainsKey(i))
            {
                continue;
            }

            m_gridList[i].SetEmpty();

        }
       
        // 最后一个格子 是扩充按钮
        int tempInt = CardBag.Singleton.m_capacity + 1;

        if (CardBag.Singleton.m_cardNum > CardBag.Singleton.m_capacity)
        {
            tempInt = CardBag.Singleton.m_cardNum + 1;
        }
      

        CreateOneGird(tempInt);

        m_gridList[tempInt].SetExpand();
    
        m_grid.Reposition();

        // 设置背包 中卡牌的空隙 中用来拖拽的 BOX大小
        BoxCollider boxCollider = m_dragBox.GetComponent<BoxCollider>();
        int totalCol            = User.Singleton.GetBagCapcity() / m_grid.maxPerLine;
        boxCollider.size        = new Vector3(boxCollider.size.x, totalCol * m_grid.cellHeight * 2, 0);

        boxCollider.center      = new Vector3(boxCollider.center.x, (-1) * totalCol * m_grid.cellHeight / 2, 0);
    }

    // 扩充背包
    void ExpandCardBagGrid( )
    {

        for (int i = 1; i <= CardBag.Singleton.m_capacity; i++)
        {
            CreateOneGird(i);
        }

        UpdateInfo();
    }

    // 创建一个空格子
    void CreateOneGird( int index )
    {
        if (m_gridList.ContainsKey(index))
        {
            return;
        }

        AddGrid(index);

    }

    // 长按 响应
    void OnClickLongPressItem(object sender, EventArgs e)
    {
        GameObject obj  = (GameObject)sender;
        Parma param     = obj.GetComponent<Parma>();

        if (param.m_guid.Equals(CSItemGuid.Zero))
        {
            return;
        }

        Debug.Log("OnClickLongPressItem点击的道具的格子ID 为：" + param.m_id);

        // 扩容
        if (-1 == param.m_id)
        {
        }
        else
        {
            CardBag.Singleton.m_curOptinGuid = param.m_guid;

            UICardDetail.GetInstance().SetPreCallback(PreCardDetail);
            UICardDetail.GetInstance().SetNextCallback(NextCardDetail);
            CardBag.Singleton.OnShowCardDetail();
        }

       

    }

    // 点击卡牌响应
    void OnClickItem(object sender, EventArgs e)
    {
       GameObject obj   = (GameObject)sender;
       Parma param      = obj.GetComponent<Parma>();
       Debug.Log("点击的道具的格子ID 为："+param.m_id);

       // 扩容
       if ( -1 == param.m_id)
       {
           ExpandBag.Singleton.ShowExpandBag();
       }
       else
       {
           if (!param.m_guid.Equals(CSItemGuid.Zero))
           {
               m_seleteCardObj = obj;
               Transform seleteItem = obj.transform.Find("Item");
               if (null != seleteItem)
               {
                   Transform seleteTra = seleteItem.transform.Find("cardSelect");
                   if (null != seleteTra)
                   {
                       seleteTra.gameObject.SetActive(true);
                   }
               }
               CardBag.Singleton.OnShowCardOption(param.m_guid);
           }
       }
      
    }


    // 添加或取消收藏
    void OnAddLove(GameObject obj)
    {
        CardBag.Singleton.AddLove();

        // m_option.gameObject.SetActive(false);
        UpdateOption();
    }
    //分享
    void OnShare(GameObject obj) 
    {

    }

    // 显示强化界面
    void OnShowLevelUpCard(GameObject obj)
    {
        m_option.gameObject.SetActive(false);
        HideWindow();

        CardUpdateProp.Singleton.OnShowCardUpdate(CardBag.Singleton.m_curOptinGuid);
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardUpdate);
    }

    // 显示进化界面
    void OnShowEvlotionCard(GameObject obj)
    {
        m_option.gameObject.SetActive(false);
        HideWindow();
        CardEvolution.Singleton.OnShowCardEvolution(CardBag.Singleton.m_curOptinGuid);
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardEvolution);
    }

    //显示段位升级界面
    void OnUpgrade(GameObject obj) 
    {
        m_option.gameObject.SetActive(false);
        HideWindow();
        CardDivisionUpdateProp.Singleton.SetCardCurt(CardBag.Singleton.m_curOptinGuid);
        //UICardDivisionUpdate.GetInstance().ShowWindow();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardDivisionUpdate);
    }

    // 显示卡牌详细界面
    void OnShowCardDetail(GameObject obj)
    {
        m_option.gameObject.SetActive(false);

        UICardDetail.GetInstance().SetPreCallback(PreCardDetail);
        UICardDetail.GetInstance().SetNextCallback(NextCardDetail);

        CardBag.Singleton.OnShowCardDetail();

    }

    // 显示出售界面
   void OnSellCard(GameObject obj)
   {
       HideWindow();

       OperateCardList.Singleton.OnSellCard();
       MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enOperaterCardList);
   }

    // 更新选项界面
    void UpdateOption()
   {
       m_option.gameObject.SetActive(true);

        CSItem card     = CardBag.Singleton.GetCardByGuid(CardBag.Singleton.m_curOptinGuid);
        
        HeroInfo hero   = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);

        IconInfomation icon   = GameTable.IconInfoTableAsset.Lookup(hero.headImageId);

        RaceInfo race   = GameTable.RaceInfoTableAsset.LookUp(hero.Type);

        OccupationInfo occupationInfo = GameTable.OccupationInfoAsset.LookUp(hero.Occupation);

        RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(hero.Rarity);

        IconInfomation rarityIconInfo = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);
        //设置角色头像图片
        m_cardInfoMain.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);

        //设置星级图标
        m_rarityTexture.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(rarityIconInfo.dirName);
        //设置种族图标
        IconInfomation raceIcon = GameTable.IconInfoTableAsset.Lookup(race.m_iconId);
        m_cardInfoRace.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(raceIcon.dirName);
        //设置职业图标
        IconInfomation occIcon = GameTable.IconInfoTableAsset.Lookup(occupationInfo.m_iconId);
        m_cardInfoOccupation.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(occIcon.dirName);

        m_optionName.text       = hero.StrName;
        m_optionLevel.text      = Localization.Get("CardLevel") + card.Level;
        m_opInfoOp.text         = occupationInfo.m_name;
        m_optionHp.text         = "" + card.GetHp();
        m_optionMagAttack.text  = "" + card.GetMagAttack();
        m_optionPhyAttack.text  = "" + card.GetPhyAttack();
        m_optionRace.text       = race.m_name;

        //根据是否为最爱 决定按钮显示 图片和头像图标显示
        if (card.Love)
        {
            //button 按钮换图片
            WorldParamInfo worldInfo =  GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enCancelLoveImageName);
            m_joinLoveSprite.spriteName = worldInfo.StringTypeValue;
            //显示最爱图标
            m_cardInfoLove.gameObject.SetActive(true);
        }
        else 
        {
            //button 按钮换图片
            WorldParamInfo worldInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enJoinLoveImageName);
            m_joinLoveSprite.spriteName = worldInfo.StringTypeValue;
            //隐藏最爱图标
            m_cardInfoLove.gameObject.SetActive(false);
        }

        // 可强化
        if (card.IsStengthen())
        {
            m_stengThen.isEnabled = true;
        }
        else
        {
            m_stengThen.isEnabled = false;
        }

        //设置素材卡牌
        GradeUpRequireInfo gradeInfo = GameTable.gradeUpRequireAsset.Lookup(card.m_id);
        if (null == gradeInfo)
        {
            return;
        }

        // 是否达到进化所需等级
        if (card.IsEvlotion() && card.BreakCounts < gradeInfo.GradeUpTime)
        {
            m_evlotion.isEnabled = true;
        }
        else
        {
            m_evlotion.isEnabled = false;
        }

        
        //是否是代表卡
        m_deputyLabel.gameObject.SetActive(false);
        m_chosenLable.gameObject.SetActive(false);
        if (User.Singleton.RepresentativeCard == CardBag.Singleton.m_curOptinGuid)
        {
            m_deputyLabel.gameObject.SetActive(true);
        }
        else 
        {
            if (Team.Singleton.IsCardInTeam(CardBag.Singleton.m_curOptinGuid))
            {
                m_chosenLable.gameObject.SetActive(true);
            }
        }
        

        //判断是否可以段位升级（待设定）
	}

	void OnCardSortReslut()
	{
//		List<CSItemGuid> reslutList = m_cardSortUI.GetSortReslut();
//		List<int> occList = m_cardSortUI.GetSelectedOcc();
//		List<int> raceList = m_cardSortUI.GetSelectedRace();
//		ENSortType sortType = m_cardSortUI.GetLastSortType();
	}

	void OnCardSelect()
	{
        // 如果为空 则全部都显示
//		List<int> occList = m_cardSortUI.GetSelectedOcc();
//		List<int> raceList = m_cardSortUI.GetSelectedRace();
	}

    // 永远只显示 一定量的格子 已减少渲染
    void OnScrollBar()
    {
        int totalCol            = User.Singleton.GetBagCapcity()/ m_grid.maxPerLine;
       
        // 当前所显示的 起始行
        int col                 = (int)(totalCol * m_bagBar.value);

        //Debug.Log("totalCol:" + totalCol + ",m_bagBar.value:" + m_bagBar.value + ",col:" + col);

        col = col - 2;
        if (col < 0)
        {
            col = 0;
        }
        // 显示格子的起始索引
        int startIndex = col * m_grid.maxPerLine + 1;


        ShowTheGird(startIndex);
    }

    // 显示指定索引上下的 格子 隐藏其他所有
    void ShowTheGird(int startIndex)
    {

       
        // 显示格子的结束索引
        int endIndex    = startIndex +  GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enBagShowItemNum).IntTypeValue;
        int bagCapcity  =  User.Singleton.GetBagCapcity();

       // Debug.Log("ShowTheGird;" + startIndex + ",endIndex:" + endIndex + ",bagCapcity:" + bagCapcity);

        // 显示该显示的
        for (int i = startIndex; i < endIndex; i++)
        {
            if (m_gridList.ContainsKey(i))
            {
                m_gridList[i].ShowContent();
            }
        }

        // 隐藏其余 前面的
        for (int i = 0; i < startIndex ; i++)
        {
            if (m_gridList.ContainsKey(i))
            {
                m_gridList[i].HideContent();
            }
        }

        // 隐藏其余 后面的
        for (int i = endIndex; i < bagCapcity; i++)
        {
            if (m_gridList.ContainsKey(i))
            {
                m_gridList[i].HideContent();
                
            }
        }
    }
    public override bool OnLeave()
    {
        m_option.gameObject.SetActive(false);
        return true;
    }

	public override void OnShowWindow()
	{
		base.OnShowWindow();

        //m_cardSortUI.SetVisable(false);
        m_cardSortUI.ShowWindow();
        m_cardSortUI.SortCards(ENSortType.enDefault, true);

        MainButtonList.Singleton.m_curShowType = MainButtonList.SHOWWNDTYPE.ENBag;

        m_bagBar.value = 0f;
       
        // 等待动画播完后 重置格子位置
        float length = this.WindowRoot.GetComponent<Animation>().GetClip("ui-windowsCutIn").length;

        MainGame.Singleton.StartCoroutine(CoroutineAnimationEnd(length));

	}

    //
    IEnumerator CoroutineAnimationEnd(float timeLength)
    {
        yield return new WaitForSeconds(timeLength+0.1f);
        m_grid.Reposition();
    }


    //营地卡牌详细信息界面

    void OnInfoReturn(GameObject obj)
    {
        m_option.gameObject.SetActive(false);
        HideSeleteCardEffect();
    }
    //隐藏选中显示高亮
    public void HideSeleteCardEffect() 
    {
        if (null != m_seleteCardObj)
        {
            Transform seleteItem = m_seleteCardObj.transform.Find("Item");
            if (null != seleteItem)
            {
                Transform seleteTra = seleteItem.transform.Find("cardSelect");
                if (null != seleteTra)
                {
                    seleteTra.gameObject.SetActive(false);
                }
            }
        }
    }


    public void PreCardDetail()
    {
        int index   = CardBag.Singleton.m_curOptinIndex - 1;
        UpdateCardDetail(index);
        index       = CardBag.Singleton.m_curOptinIndex - 1;
        UICardDetail.GetInstance().ShowPre(IsHaveCardDetail(index));
    }

    public void NextCardDetail()
    {
        int index   = CardBag.Singleton.m_curOptinIndex + 1;
        UpdateCardDetail(index);
        index       = CardBag.Singleton.m_curOptinIndex + 1;
        UICardDetail.GetInstance().ShowNext(IsHaveCardDetail(index));
    }

    bool IsHaveCardDetail( int index )
    {
        if (!m_gridList.ContainsKey(index))
        {
            return false;
        }
        UICardItem item = m_gridList[index];
        if (null == item)
        {
            return false;
        }
        return item.IsHaveCardDetail();
    }

    public void UpdateCardDetail(int index)
    {
        if (m_gridList.ContainsKey(index))
        {
            UICardItem item = m_gridList[index];
            if (item.IsHaveCardDetail())
            {
                //更新卡牌详情
                CardBag.Singleton.m_curOptinIndex   = index;
                CardBag.Singleton.m_curOptinGuid    = item.m_param.m_guid;
                CardBag.Singleton.OnShowCardDetail();
            }
        }   

    }
}
