using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIOperaterCardList : UIWindow
{
    // 格子列表 ID，格子UI
    Dictionary<int, UICardItem> m_gridList    = null;

    //int m_curLine                                   = 0;// 当前包行数

    //UIDragScrollView m_item = null;

    UIPanel m_ruSure        = null;

    Dictionary<int,GameObject> m_sellImageList = null;

    UITexture m_sellItem     = null;

    UIGrid m_grid           = null;

    UILabel m_tips          = null;

    UIGrid m_grid2          = null;


    UIGrid m_gridItemList   = null;

    UICardSort m_cardSortUI;

    public  int m_tempIndex = 1;// 用于卡牌索引

    public delegate void OnClickCallbacked(UICardItem item, ENSortType sortType);

    public delegate bool OnSortCallbacked(CSItem item);


    Dictionary<OperateCardList.TYPE, OnClickCallbacked> m_clickList = new Dictionary<OperateCardList.TYPE, OnClickCallbacked>(); // 点击格子的回调列表

    Dictionary<OperateCardList.TYPE, OnSortCallbacked> m_sortList = new Dictionary<OperateCardList.TYPE, OnSortCallbacked>(); // 更新时 哪些卡牌可以显示的排列 回调列表

    Dictionary<OperateCardList.TYPE, UIWindow> m_wndList = new Dictionary<OperateCardList.TYPE, UIWindow>();//各种分类列表

    static public UIOperaterCardList GetInstance()
    {
        UIOperaterCardList self = UIManager.Singleton.GetUIWithoutLoad<UIOperaterCardList>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIOperaterCardList>("UI/UIOperateCardList", UIManager.Anchor.Center);
        return self;
    }

    public UIOperaterCardList()
    {

    }

	public override void OnInit ()
	{
        base.OnInit();

        AddPropChangedNotify((int)MVCPropertyID.enOperateCardList, OnPropertyChanged);
        AddPropChangedNotify((int)MVCPropertyID.enCardBag, OnCardBagPropertyChanged);

        m_gridList  = new Dictionary<int, UICardItem>();

//        m_item      =   FindChildComponent<UIDragScrollView>("Grid");

       

        m_ruSure    =   FindChildComponent<UIPanel>("RUSure");

        m_sellItem  =   FindChildComponent<UITexture>("SellItem");


        m_sellImageList =   new Dictionary<int,GameObject>();

        m_grid          = FindChildComponent<UIGrid>("ItemList");

        m_grid2         = FindChildComponent<UIGrid>("ItemList2");

        m_tips          = FindChildComponent<UILabel>("RUSureTips");



        m_gridItemList = FindChildComponent<UIGrid>("GridList");
		
        // 队伍编辑
        UIOperateTeam teamItem  = UIOperateTeam.Create();
        AddNewWindow(       OperateCardList.TYPE.enTeamCardListType, teamItem);
        // 点击的回调
        AddClickCallback(   OperateCardList.TYPE.enTeamCardListType, teamItem.OnClickTeamItem);
        // 排列的回调
        AddSortCallback(    OperateCardList.TYPE.enTeamCardListType, teamItem.OnSortCondion);

        // 出售编辑
        UIOperateSell sellItem = UIOperateSell.Create();
        AddNewWindow(OperateCardList.TYPE.enSellType, sellItem);
        AddClickCallback(OperateCardList.TYPE.enSellType, sellItem.OnClickItem);
        AddSortCallback(OperateCardList.TYPE.enSellType, sellItem.OnSortCondion);

        // 升级基本卡编辑
        UIOperateLevelupBaseCard levelupBaseItem = UIOperateLevelupBaseCard.Create();
        AddNewWindow(OperateCardList.TYPE.enCardLevelUpBaseCardType, levelupBaseItem);
        AddClickCallback(OperateCardList.TYPE.enCardLevelUpBaseCardType, levelupBaseItem.OnClickItem);
        AddSortCallback(OperateCardList.TYPE.enCardLevelUpBaseCardType, levelupBaseItem.OnSortCondion);

        // 强化合成编辑
        UIOperateLevelupDataCard levelupDataItem = UIOperateLevelupDataCard.Create();
        AddNewWindow(OperateCardList.TYPE.enCardLevelUpDataCardType, levelupDataItem);
        AddClickCallback(OperateCardList.TYPE.enCardLevelUpDataCardType, levelupDataItem.OnClickItem);
        AddSortCallback(OperateCardList.TYPE.enCardLevelUpDataCardType, levelupDataItem.OnSortCondion);

        //  进化 的基本卡牌列表
        UIOperateEvolution evolutionItem = UIOperateEvolution.Create();
        AddNewWindow(OperateCardList.TYPE.enCardEvolutionBaseCardType, evolutionItem);
        AddClickCallback(OperateCardList.TYPE.enCardEvolutionBaseCardType, evolutionItem.OnClickItem);
        AddSortCallback(OperateCardList.TYPE.enCardEvolutionBaseCardType, evolutionItem.OnSortCondion);

        //  
        UIOperateCardDivision divsionItem = UIOperateCardDivision.Create();
        AddNewWindow(OperateCardList.TYPE.enCardDivisionUpdate, divsionItem);
        AddClickCallback(OperateCardList.TYPE.enCardDivisionUpdate, divsionItem.OnClickItem);
        AddSortCallback(OperateCardList.TYPE.enCardDivisionUpdate, divsionItem.OnSortCondion);

        //  
        UIOperateRepresentativeCard representativeItem = UIOperateRepresentativeCard.Create();
        AddNewWindow(OperateCardList.TYPE.enSelectRepresentativeCard, representativeItem);
        AddClickCallback(OperateCardList.TYPE.enSelectRepresentativeCard, representativeItem.OnClickItem);
        AddSortCallback(OperateCardList.TYPE.enSelectRepresentativeCard, representativeItem.OnSortCondion);

   
        m_cardSortUI = UICardSort.Load();
        m_cardSortUI.SetParentWnd(this);
        m_cardSortUI.SortCards(ENSortType.enDefault, true);

		HideWindow ();
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
		if (false == WindowRoot.activeSelf) 
		{
			return;
		}

        if (eventType == (int)OperateCardList.ENPropertyChanged.enUpdate)
        {
            UpdateInfo();
        }
        else if (eventType == (int)OperateCardList.ENPropertyChanged.enUpdateItemForTeam)
        {
            OnUpdateItemForTeam();
        }
  
    }

    void OnCardBagPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if ( !WindowRoot.activeSelf )
        {
            return;
        }

        if (eventType == (int)CardBag.ENPropertyChanged.enCardSort)
        {
            OperateCardList.Singleton.UpdateCardList();
        }
        else if (eventType == (int)CardBag.ENPropertyChanged.enCardSelect)
        {
            OperateCardList.Singleton.UpdateCardList();
        }
    }


    public override void AttachEvent()
    {
        base.AttachEvent();
    

        AddChildMouseClickEvent("RUSureOK", OnRUSureOK);

        AddChildMouseClickEvent("RUSureCancel", OnRUSureCancel);

		

        AddChildMouseClickEvent("CardSort", OnShowCardSort);
    }
    public override void OnShowWindow()
    {
        base.OnShowWindow();
        m_cardSortUI.SetVisable(false);
        m_cardSortUI.SortCards(ENSortType.enDefault, true);
    }

    public override void OnHideWindow()
    {

    }

    // 添加子窗口
    void AddNewWindow(OperateCardList.TYPE type,UIWindow wnd)
    {
        if (m_wndList.ContainsKey(type))
        {
            return;
        }
        m_wndList.Add(type, wnd);
        wnd.SetParent(WindowRoot);
        m_clickList.Add(type, null);
        m_sortList.Add(type, null);
    }

    // 添加 点击回调函数
    void AddClickCallback(OperateCardList.TYPE type, OnClickCallbacked callback)
    {
        if (m_clickList.ContainsKey(type))
        {
            m_clickList[type] = callback;
        }
        else
        {
            m_clickList.Add(type, callback);
        } 
    }

    // 添加 排列 回调函数
    void AddSortCallback(OperateCardList.TYPE type, OnSortCallbacked callback)
    {
        if (m_sortList.ContainsKey(type))
        {
            m_sortList[type] = callback;
        }
        else
        {
            m_sortList.Add(type, callback);
        }
    }


    // 卡牌排序
    void OnShowCardSort(object obj, EventArgs e)
    {
        m_cardSortUI.ShowOrHide();
    }

    // 二次确认
    void OnRUSureOK(GameObject obj)
    {

        m_ruSure.gameObject.SetActive(false);

        // 向服务器发送卖消息
        IMiniServer.Singleton.SendSellHeroCard_C2S(OperateCardList.Singleton.m_sellList);
        OperateCardList.Singleton.m_sellList.Clear();

        // 显示Loading
        Loading.Singleton.SetLoadingTips((int)LOADINGTIPSENUM.enLogin);
    }

    // 二次确认 的取消
    void OnRUSureCancel(GameObject obj)
    {
        m_ruSure.gameObject.SetActive(false);

    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        foreach ( KeyValuePair<int,UICardItem> item in m_gridList)
        {
            item.Value.Destroy();
        }

        foreach (KeyValuePair<OperateCardList.TYPE, UIWindow> item in m_wndList)
        {
            item.Value.Destroy();
        }



        foreach (KeyValuePair<int, GameObject> item in m_sellImageList)
        {
            GameObject.Destroy(item.Value);
        }

        m_clickList.Clear();
        m_sortList.Clear();

        m_cardSortUI.Destroy();
    }

    // 扩充一个格子
    void ExpandOneGrid()
    {
        int index = m_gridList.Count + 1;
        AddGrid(index);
    }

    // 更新
    void UpdateInfo()
    {
        m_tempIndex = 1;

        // 不同类型显示内容不同 
        ShowCurTypeLayout();

        // 排序好的列表
        List<CSItemGuid> cardList   = m_cardSortUI.GetSortReslut();
        ENSortType sortType         = m_cardSortUI.GetLastSortType();

        // 显示卡牌列表
        foreach (CSItemGuid itemGuid in cardList)
        {
            CSItem item = CardBag.Singleton.GetCardByGuid(itemGuid);
            if (null == item)
            {
                continue;
            }

            int cardID                                  = item.IDInTable;

            HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(cardID);

            if (null == heroInfo)
            {
                Debug.LogWarning("heroInfo == NULL heroInfo cardID:" + cardID);
                continue;
            }

            // 筛选
            if ( SortConditon(item) )
            {
                continue;
            }

            // 格子不够增加格子
            if (false == m_gridList.ContainsKey(m_tempIndex))
            {
                ExpandOneGrid();
            }

            // 更新格子
            UICardItem cardItem = m_gridList[m_tempIndex];

            cardItem.UpdateOperate(item, sortType, m_tempIndex);

            m_tempIndex++;
        }


        // 把剩余的空格子图片置空
        for (; m_tempIndex <= m_gridList.Count; m_tempIndex++)
        {
            UICardItem cardItem = m_gridList[m_tempIndex];
            cardItem.SetEmpty();
            cardItem.HideWindow();
        }

        m_gridItemList.Reposition();
    }

    // 显示指定当前类型的界面
    void ShowCurTypeLayout()
    {
//        string capacityText = "" + CardBag.Singleton.m_cardNum + "/" + CardBag.Singleton.GetBagCapcity();
        // 用来去除 此列表中的卡牌 排列


        // 显示当前类型的 界面
        foreach (KeyValuePair<OperateCardList.TYPE, UIWindow> typeItem in m_wndList)
        {
			if (typeItem.Value == null)
			{
				continue;
			}
            if (OperateCardList.Singleton.m_curType == typeItem.Key)
            {
                typeItem.Value.ShowWindow();
            }
            else
            {
                typeItem.Value.HideWindow();
            }
        } 
    }

    // 指定卡牌是否 被 排除 true 排除， false 不排除
    bool SortConditon( CSItem item  )
    {
        if ( null == item )
        {
            return true;
        }

        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(item.IDInTable);

        if (null == heroInfo)
        {
            Debug.LogWarning("null == heroInfo item.IDInTable:" + item.IDInTable);
            return true;
        }

        // 职业和种族刷选
        if (SortByOccRace(item.IDInTable))
        {
            return true;
        }
        
          
        // 回调当前类型的 筛选
        foreach (KeyValuePair<OperateCardList.TYPE, OnSortCallbacked> typeItem in m_sortList)
        {
			if (typeItem.Value == null)
			{
				continue;
			}
            if (OperateCardList.Singleton.m_curType == typeItem.Key)
            {
                return typeItem.Value(item);
            }  
        }
        return false;
    }


    // 刷选职业和种族 TRUE 排除 FALSE 不排除
    public bool SortByOccRace(int cardId)
    {
        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(cardId);
        if (null == heroInfo)
        {
            Debug.LogWarning("null == heroInfo item.IDInTable:" + cardId);
            return true;
        }

        // 职业刷选
        if (m_cardSortUI.GetSelectedOcc().Count != 0)
        {
            // 如果不是是选中的职业 则不显示
            if (!m_cardSortUI.GetSelectedOcc().Contains(heroInfo.Occupation))
            {
                return true;
            }
        }

        // 种族筛选
        if (m_cardSortUI.GetSelectedRace().Count != 0)
        {
            // 如果不是是选中的职业 则不显示
            if (!m_cardSortUI.GetSelectedRace().Contains(heroInfo.Type))
            {
                return true;
            }
        }
        return false;
    }

    // 为队伍相关编辑 更新格子
    void OnUpdateItemForTeam()
    {
        // 领导力消耗
        OperateCardList.Singleton.m_leadShipCost = 0;

        m_tempIndex                 = 1;
        // 先显示满足条件的卡牌
        int teamIndex               = Team.Singleton.m_curTeamIndex;
        CSItem card                 = null;

        OperateCardList.Singleton.m_hadItemList.Clear();

        // 队伍特殊 顺序特别设置

        // 编辑整个队伍
        if (Team.Singleton.m_curEditType == Team.EDITTYPE.enALL)
        {

            // 主角色
            card = CardBag.Singleton.GetCardByGuid(Team.Singleton.m_bagMainSlotId);
            UpdateTeamItem(card, Team.EDITTYPE.enMain);

            // 副角色
            card = CardBag.Singleton.GetCardByGuid(Team.Singleton.m_bagDeputySlotId);
            UpdateTeamItem(card, Team.EDITTYPE.enDeputy);
    
            // 支持角色
            card = CardBag.Singleton.GetCardByGuid(Team.Singleton.m_bagSupportSlotId);
            UpdateTeamItem(card, Team.EDITTYPE.enSupport);
        }
        // 编辑队伍中的单个角色
        else
        {
            // 格子不够增加格子
            if (false == m_gridList.ContainsKey(m_tempIndex))
            {
                ExpandOneGrid();
            }

            // 第一个格子 是个X
            UICardItem item = m_gridList[m_tempIndex];
            item.SetXForOperateTeam();
            m_tempIndex++;

            // 已选 主角色
            card                = Team.Singleton.GetCard(teamIndex, Team.EDITTYPE.enMain);
            UpdateTeamItem(card, Team.EDITTYPE.enMain);

            // 已选 副角色
            card                    = Team.Singleton.GetCard(teamIndex, Team.EDITTYPE.enDeputy);
            UpdateTeamItem(card, Team.EDITTYPE.enDeputy);

            // 已选 支持角色
            card = Team.Singleton.GetCard(teamIndex, Team.EDITTYPE.enSupport);
            UpdateTeamItem(card, Team.EDITTYPE.enSupport);
         }
    }

    // 更新单个格子 为队伍编辑
    void UpdateTeamItem(CSItem card,Team.EDITTYPE type)
    {
        if (null != card && !SortByOccRace(card.IDInTable))
        {
            // 格子不够增加格子
            if (false == m_gridList.ContainsKey(m_tempIndex))
            {
                ExpandOneGrid();
            }

            UICardItem item = m_gridList[m_tempIndex];

            HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
            if (null == heroInfo)
            {
                Debug.LogWarning("UpdateTeamItem null == heroInfo item.IDInTable:" + card.IDInTable);
            }
            else
            {
                OperateCardList.Singleton.m_leadShipCost = OperateCardList.Singleton.m_leadShipCost + heroInfo.Cost;
                item.UpdateOperateTeam(card, m_cardSortUI.GetLastSortType(), type, m_tempIndex);
                OperateCardList.Singleton.m_hadItemList.Add(card.Guid);
            }
            m_tempIndex++;
        }
    }

    // 初始化背包
    public void InitBag()
    {
        for (int i = 1; i <= CardBag.Singleton.m_capacity; i++)
        {
            AddGrid(i);
        }

    }
    // 增加格子 index 格子唯一索引
    void AddGrid(int index)
    {
        // 如果存在 则返回
        if (m_gridList.ContainsKey(index))
        {
            return;
        }

        UICardItem item = UICardItem.Create();
        // 设置 父物体
        item.SetParentWnd(m_gridItemList.gameObject);
        item.SetRootName(index.ToString());

        // 设置回调函数
        item.SetClickCallback(OnClickItem);
        item.SetPressCallback(OnClickLongPressItem);
       
        m_gridList.Add(index, item);
    }

    // 长按 显示卡牌详细界面
    void OnClickLongPressItem(object sender, EventArgs e)
    {
        GameObject obj = (GameObject)sender;
        Parma param = obj.GetComponent<Parma>();

        if (param.m_guid.Equals(CSItemGuid.Zero))
        {
            return;
        }

        UICardDetail.GetInstance().SetPreCallback(PreCardDetail);
        UICardDetail.GetInstance().SetNextCallback(NextCardDetail);
        // 显示只显示返回按钮的 卡牌详细界面
        CardBag.Singleton.OnShowCardDetail(CardBag.Singleton.GetCardByGuid(param.m_guid),true);

    }
    // 点击 道具的响应
    void OnClickItem(object sender, EventArgs e)
    {
        GameObject obj  = (GameObject)sender;
        int index       = int.Parse(obj.transform.parent.name);

        if (!m_gridList.ContainsKey(index))
        {
            return;
        }

        Parma parma                                     = obj.GetComponent<Parma>();
        OperateCardList.Singleton.m_curChooseItemGuid   = parma.m_guid;

        // 掉相对应的点击回调函数
        if (m_clickList.ContainsKey(OperateCardList.Singleton.m_curType) && m_clickList[OperateCardList.Singleton.m_curType] != null)
        {
            m_clickList[OperateCardList.Singleton.m_curType](m_gridList[index],m_cardSortUI.GetLastSortType());
        }

        UpdateInfo();
    }


	

    // 显示二次确认
    public  void OnShowRUSure()
    {
        m_ruSure.gameObject.SetActive(true);
       

        int i = 0;
        bool bShowTips = false;
        foreach (CSItemGuid slotId in OperateCardList.Singleton.m_sellList)
        {
            CSItem card = CardBag.Singleton.GetCardByGuid(slotId);
            if ( null == card )
            {
                continue;
            }
            HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);

            if (null == info)
            {
                Debug.LogWarning("null == hero card.IDInTable:" + card.IDInTable);
                continue;
            }

            IconInfomation iconInfo = GameTable.IconInfoTableAsset.Lookup(info.headImageId);

            // 有三星的卡
            if ( info.Rarity >= 3 )
            {
                bShowTips = true;
            }
            Transform parent = m_grid.transform;

            if ( i>= 5 )
            {
                parent = m_grid2.transform;
            }

            if (false == m_sellImageList.ContainsKey(i))
            {
                // 创建
                GameObject copy             = GameObject.Instantiate(m_sellItem.gameObject) as GameObject;
                copy.transform.parent       = parent;
                copy.transform.localScale   = m_sellItem.transform.localScale;
                copy.gameObject.SetActive(true);
                copy.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);

                m_sellImageList.Add(i, copy);
            }
            else
            {
                m_sellImageList[i].GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
                m_sellImageList[i].transform.parent = parent;
            }

            i++;
        }

        List<int> temp = new List<int>();

        for (; i < m_sellImageList.Count; i++ )
        {
            if (m_sellImageList.ContainsKey(i))
            {
                GameObject.Destroy(m_sellImageList[i]);
                temp.Add(i);
            }
            
        }

        foreach (int index in temp)
        {
            m_sellImageList.Remove(index);
        }

        m_grid.Reposition();
        m_grid2.Reposition();

        // 出售三星以上卡牌提示
        m_tips.gameObject.SetActive(bShowTips);
       
    }

    public void PreCardDetail()
    {
        int index = CardBag.Singleton.m_curOptinIndex - 1;
        UpdateCardDetail(index);
        index = CardBag.Singleton.m_curOptinIndex - 1;
        UICardDetail.GetInstance().ShowPre(IsHaveCardDetail(index));
    }

    public void NextCardDetail()
    {
        int index = CardBag.Singleton.m_curOptinIndex + 1;
        UpdateCardDetail(index);
        index = CardBag.Singleton.m_curOptinIndex + 1;
        UICardDetail.GetInstance().ShowNext(IsHaveCardDetail(index));
    }

    bool IsHaveCardDetail(int index)
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
                CardBag.Singleton.m_curOptinIndex = index;
                CardBag.Singleton.m_curOptinGuid = item.m_param.m_guid;
                CardBag.Singleton.OnShowCardDetail();
            }
        }

    }

}
