using System;
using System.Collections.Generic;

public enum ENSortType
{
    enDefault = 0,
    enGotTime,
    enByRarity,
    enByPhyAttack,
    enByMagAttack,
    enByHp,
    enByOccupation,
    enByType,
    enByLevel,
    enByFavorite,
    enCanEvolve,
    enGold,
    enRing,
    enLoadTime,
}

public enum ENSortClassType
{
    enFriend,
    enCSItem,
}

// 卡牌背包
public class CardBag : IPropertyObject  
{
    public enum ENPropertyChanged
    {
        enShowCard = 1,
        enShowCardUpdate ,
        enFlushCardUpdate,
        enShowBaseCardList ,
        enShowDataCardList,
        enShowGetNewCard,
        enShowCardBag,
        enInitCardBag,
        enUpdateCardInBag,
        enResetSelectedList,
        enShowOption,
        enShowCardDetail,
        enSellCard,
        enExpandCardBag,
		enCardSort,
		enCardSelect,
    }

	Dictionary<int, string> m_sortStringByType;

    public enum enCardShowType
    {
        enBaseUpdateType  = 1,
        enBaseEvolutionType ,
    }

    public CSItemBag itemBag { get { return User.Singleton.ItemBag; } }

    public List<CSItem> itemList { get { return itemBag.itemList; } }

    // 背包容量
    public int m_capacity  { get { return GetBagCapcity();}  }

    // 实际 背包卡牌个数
    public int m_cardNum { get { return itemBag.GetItemCount(); } }

    CSItemGuid m_curCardId                  = new CSItemGuid();

    public BagInfo m_bagTableInfo    = null;

    // 种族筛选列表 int = 种族ID 
    //List<int> m_raceSelected        = null;

    // 职业筛选列表 int = 职业ID 
    //List<int> m_occSelected         = null;

    // 卡牌详细界面 不是背包中的 卡牌
    public CSItem m_cardForDetail = null;

    // 当前显示详细选项的格子ID
    public CSItemGuid m_curOptinGuid  = new CSItemGuid();
    // 当前显示详细选项的格子所以和你
    public int m_curOptinIndex = 0;

    // 添加格子用到的guid累加
    public int m_GuidIndex = 1;

    public CardBag()
    {
        SetPropertyObjectID((int)MVCPropertyID.enCardBag);

        //m_raceSelected                  = new List<int>();

        //m_occSelected                   = new List<int>();

        WorldParamInfo worldParamInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enCardBagID);
        if ( null == worldParamInfo )
        {
            return;
        }

        BagInfo bagInfo                 = GameTable.BagTableAsset.LookUp(worldParamInfo.IntTypeValue);

        if ( null == bagInfo )
        {
            return;
        }


        m_bagTableInfo                  = bagInfo;

		InitSortString();
    }

    #region Singleton
    static CardBag m_singleton;
    static public CardBag Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new CardBag();
            }
            return m_singleton;
        }
    }
    #endregion

    // 获得背包容量
    public int GetBagCapcity()
    {
		return User.Singleton.GetBagCapcity();
    }


    // 增加背包容量
    public void AddBagCapcity( int expandSize )
    {
//         int capcity =  User.Singleton.UserProps.GetProperty_Int32(UserProperty.bagCapcity_boughtTimes);
//         capcity     = capcity + expandSize;
//         User.Singleton.UserProps.SetProperty_Int32(UserProperty.bagCapcity_boughtTimes, capcity);
// 
//         UnityEngine.Debug.Log("AddBagCapcity 增加" + expandSize + "新格子，现背包容量为:" + capcity);

		User.Singleton.AddBagCapcity();
		UnityEngine.Debug.Log("AddBagCapcity 增加" + 5 + "新格子，现背包容量为:" + CardBag.Singleton.GetBagCapcity());
	}

    // 按默认排序方式 显示背包
    public void OnShowCardBag()
    {

        NotifyChanged((int)ENPropertyChanged.enShowCardBag, null);
    }

    // 按默认排序方式 更新背包
    public void OnUpdateCardBag()
    {
        NotifyChanged((int)ENPropertyChanged.enUpdateCardInBag, null);
    }
    
    public void InitCard(ServerMsgHerocardBag msg)
    {
        UnityEngine.Debug.Log("InitCard " + msg.capacity + "," + msg.herocardList.Count);
    }

    public void ShowCard( CSItemGuid guid )
    {
        CSItemGuid temp = new CSItemGuid();
        if (guid.Equals(temp))
        {
            guid = m_curCardId;
        }

        if (null == itemBag.GetItemByGuid(guid))
        {
            return;
        }

        NotifyChanged((int)ENPropertyChanged.enShowCard, guid);
    }

   // 通过GUID 来获得卡牌
    public CSItem GetCardByGuid(CSItemGuid guid)
    {
        return itemBag.GetItemByGuid(guid);
    }

    // 获得id卡牌的个数
    public int GetCardNumById( int id )
    {
        return itemBag.GetCardNumById(id);
    }

    // 显示 获得新卡牌
    public void ShowGetNewCard(CSItemGuid guid)
    {
        NotifyChanged((int)ENPropertyChanged.enShowGetNewCard, guid);
    }

    // 测试用 随机获得一张卡牌
    public void RandomAddCard()
    {
        if (m_cardNum >= m_capacity)
        {
            //return;
        }

        int index   = GameTable.HeroInfoTableAsset.m_list.Count;

        index       = UnityEngine.Random.Range(0, index);

        int random  = 0;
        int cardId  = 5;
        foreach (HeroInfo item in GameTable.HeroInfoTableAsset.m_list.Values)
        {
            if (random == index)
            {
                cardId = item.ID;
            }
            random++;
        }
        if ( GameSettings.Singleton.m_isSingle)
        {
            AddCardOffLine(cardId);
           
            NotifyChanged((int)ENPropertyChanged.enUpdateCardInBag, null);
        }
        else
        {
            AddCard(cardId);
        }
       
    }

    // 测试用 向服务器发消息 添加一个指定id的卡牌
    public void AddCard( int cardId )
    {
        IMiniServer.Singleton.player_get_card(cardId,1);
        UnityEngine.Debug.Log("AddCard:" + cardId);      
    }

    // 离线状态的 增加卡牌 纯客户端处理 测试用
    public void AddCardOffLine(int cardId)
    {
        int index = m_GuidIndex++;

        CSItemGuid guid = new CSItemGuid();
        guid.m_lowPart = index;

        CSItem item = new CSItem();
        item.Level = 1;
        item.Love = false;
        item.Exp = 2;
        item.GotTime = (uint)UnityEngine.Time.time;
        item.IDInTable = (short)cardId;
        item.Guid = guid;

        item.Init();

        AddCard(item);

    }

    // 向背包中添加一张指定卡
    public void AddCard(CSItem item)
    {
        itemBag.AddItemInBag(item);
    }

    // 从背包中移除一张指定卡
    public void RemoveCard(CSItemGuid guid)
    {

        itemBag.RemoveItem(guid);

    }

    static int Compare(CSItemGuid a, CSItemGuid b)
    {
        if (a.m_lowPart > b.m_lowPart )
        {
            return 1;
        }
        else if (a.m_lowPart < b.m_lowPart)
        {
            return -1;
        }
        return 0;
    }

    // 添加收藏或者 解除收藏
    public void AddLove()
    {
       CSItem card = GetCardByGuid(m_curOptinGuid);
       if (null == card)
        {
            return;
        }

       card.Love = !card.Love;

        NotifyChanged((int)ENPropertyChanged.enUpdateCardInBag, null);
    }

    // 显示选项
    public void OnShowCardOption(CSItemGuid guid)
    {
        m_curOptinGuid = guid;

        NotifyChanged((int)ENPropertyChanged.enShowOption, null);
    }

    // 显示卡牌详细界面 带所有选项按钮  bOnlyCancel是否只显示 返回按钮
    public void OnShowCardDetail(CSItem card = null, bool bOnlyCancel = false)
    {
        if (null == card)
        {
            SetShowCardData(itemBag.GetItemByGuid(m_curOptinGuid), bOnlyCancel);
        }
        else
        {
            SetShowCardData(card, bOnlyCancel);
        }

        NotifyChanged((int)ENPropertyChanged.enShowCardDetail, null);
    }

    // 设置要显示的 卡牌详情的卡牌数据   bOnlyCancel是否只显示 返回按钮
    public void SetShowCardData(CSItem card,bool bOnlyCancel = false)
    {
        // 只带 返回按钮
        if (bOnlyCancel)
        {
            m_curOptinGuid = CSItemGuid.Zero;
        } 
        m_cardForDetail      = card;
    }

    //扩容
    public void ExpandCardBag( int type )
    {
        // 当前背包容量超过最大
        if (m_capacity >= m_bagTableInfo.m_maxSize)
        {
            RUSure.Singleton.Show(Localization.Get("ExpandMax"), null);
            return;
        }
        int expandCost = 0;
//        int expandSize = 0;
        switch (type)
        {
            case (int)ExpandBag.ExpandType.enExpand1:
                {
                    expandCost  = m_bagTableInfo.m_expandCost1;
//                    expandSize  = m_bagTableInfo.m_expandSize1;
                }
                break;
            case (int)ExpandBag.ExpandType.enExpand2:
                {
                    expandCost  = m_bagTableInfo.m_expandCost2;
//                    expandSize  = m_bagTableInfo.m_expandSize2;
                }
                break;
            case (int)ExpandBag.ExpandType.enExpand3:
                {
                    expandCost  = m_bagTableInfo.m_expandCost3;
//                    expandSize  = m_bagTableInfo.m_expandSize3;
                }
                break;
        }
        //弹出二次确认框 魔法石不足
        if (User.Singleton.GetDiamond() < expandCost)
        {
            UICommonMsgBoxCfg boxCfg = UIExpandBag.GetInstance().FindChild("OK").transform.GetComponent<UICommonMsgBoxCfg>();
            UICommonMsgBox.GetInstance().ShowMsgBox(null, null, boxCfg);
            string text = Localization.Get("NotEnoughMagicStone");
            UICommonMsgBox.GetInstance().GetMainText().SetText(text);
            return;
        }
        
        //向服务器发送扩展背包消息
//        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enCardBagID);
        MiniServer.Singleton.SendExpandCardBag_C2S(type);
        //进入转菊花loading界面
        Loading.Singleton.SetLoadingTips();


        //NotifyChanged((int)ENPropertyChanged.enExpandCardBag, type);
        //string str = string.Format(Localization.Get("ExpandSuccess"), expandSize, m_capacity);
        //RUSure.Singleton.Show(str, null);
    }

    //增加 背包格子
    public void AddCadBag(int expandNum) 
    {
		int oldNum = User.Singleton.UserProps.GetProperty_Int32(UserProperty.bagCapcity_expandedSize);
        int addNum = expandNum - oldNum;
		User.Singleton.UserProps.SetProperty_Int32(UserProperty.bagCapcity_expandedSize, expandNum);
        //显示成功扩展背包确认框
        ShowAddCardBagPanel(addNum, expandNum);
        NotifyChanged((int)ENPropertyChanged.enExpandCardBag, null);
    }
    public void ShowAddCardBagPanel(int addNum, int expandNum) 
    {
        string text = string.Format(Localization.Get("ExpandBagSpace"), addNum);
        string titelText = Localization.Get("ExpandBagSuccess");
        BagInfo info = GameTable.BagTableAsset.LookUp((int)BAGTYPEENUM.enCardBagType);
        string useText = string.Format(Localization.Get("BagSpaceStatus"), (expandNum + info.m_initalSize));
        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enExtendBagTipsIconId);
        int iconId = param.IntTypeValue;
        RUSure.Singleton.ShowPanelText(text, titelText, useText,iconId);
    }



    // 是否要弹出 背包已满
    public bool IsPopSizeMax()
    {
        if (m_cardNum > GetBagCapcity())
        {
            return true;
        }
        return false;
    }

	public bool CanCardEvolve(CSItemGuid itemGuid)
	{
		CSItem item = GetCardByGuid(itemGuid);
		if (item == null)
		{
			return false;
		}
		HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(item.IDInTable);
		if ( info == null)
		{
			return false;
		}
		if (item.Level < info.EvolveNeedLevel)
		{
			return false;
		}

		//看素材是否足够
		return CardEvolutionFoodEnough(itemGuid);
	}

	//进化素材手否足够
	public bool CardEvolutionFoodEnough(CSItemGuid itemGuid)
	{
		CSItem item = GetCardByGuid(itemGuid);
		if (item == null)
		{
			return false;
		}
		HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(item.IDInTable);
		if (info == null)
		{
			return false;
		}
		//进化需要的卡牌id和数量
		Dictionary<int, int> evolveCardList = new Dictionary<int, int>();
        foreach (var cardID in info.EvolveCardList)
        {
            if (evolveCardList.ContainsKey(cardID))
            {
                ++evolveCardList[cardID];
            }
            else
            {
                evolveCardList[cardID] = 0;
            }
        }
		foreach (CSItem card in itemList)
		{
			if (item.Guid == card.Guid)
			{
				continue;
			}
			int cardID = card.IDInTable;
			if (evolveCardList.ContainsKey(cardID))
			{
				evolveCardList[cardID]--;
			}
		}

		foreach (KeyValuePair<int, int> resultCount in evolveCardList)
		{
			int result = resultCount.Value;
			if (result > 0)
			{
				return false;
			}
		}
		return true;
	}

	#region 各种排序
	//①默认排序规则， 入手时间
	public List<CSItemGuid> SortCardByGotTime(ENSortType sortType, bool reverse)
	{
		//队伍
		List<int> tempTeam = new List<int>();
		Dictionary<int, List<CSItem>> dicTeam = new Dictionary<int, List<CSItem>>();
		//其他
		List<int> temp = new List<int>();
		Dictionary<int, List<CSItem>> dic = new Dictionary<int, List<CSItem>>();

		// 先是 队伍 
		// 再是 其他 
		foreach (CSItem item in itemList)
		{
			HeroInfo heroInfo = item.GetHeroInfo();
			if (heroInfo == null)
			{
				UnityEngine.Debug.Log("SortCardByGotTime  heroInfo == null , id = " + item.IDInTable);
				continue;
			}
			if (item.Guid == User.Singleton.RepresentativeCard)
			{
				continue;
			}

			int time = (int)item.GotTime;
			// 队伍中的
			if (Team.Singleton.IsCardInTeam(item.Guid))
			{
				if (dicTeam.ContainsKey(time))
				{
					dicTeam[time].Add(item);
				}
				else
				{
					List<CSItem> tmpItemList = new List<CSItem>();
					tmpItemList.Add(item);
					dicTeam.Add(time, tmpItemList);
					tempTeam.Add(time);
				}
			}
			else
			{
				if (dic.ContainsKey(time))
				{
					dic[time].Add(item);
				}
				else
				{
					List<CSItem> tmpItemList = new List<CSItem>();
					tmpItemList.Add(item);
					dic.Add(time, tmpItemList);
					temp.Add(time);
				}
			}
		}
		tempTeam.Sort();
		temp.Sort();
		if (!reverse)
		{
			tempTeam.Reverse();
			temp.Reverse();
		}

		List<CSItem> resultList = new List<CSItem>();
		resultList.Clear();
		foreach (int time in tempTeam)
		{
			dicTeam[time].Sort(new ItemIdCompare());
			foreach (CSItem item in dicTeam[time])
			{
				resultList.Add(item);
			}
		}
		foreach (int time in temp)
		{
			dic[time].Sort(new ItemIdCompare());
			foreach (CSItem item in dic[time])
			{
				resultList.Add(item);
			}
		}
		//全部统计好了，将ID返回出去
		List<CSItemGuid> guidList = new List<CSItemGuid>();
		guidList.Add(User.Singleton.RepresentativeCard);
		foreach (CSItem item in resultList)
		{
			guidList.Add(item.Guid);
		}
		return guidList;
	}

	//星级排序
	public List<CSItemGuid> SortByRarity(ENSortType sortType, bool reverse)
	{
		//队伍
		List<int> tempTeam = new List<int>();
		Dictionary<int, List<CSItem>> dicTeam = new Dictionary<int, List<CSItem>>();
		//其他
		List<int> temp = new List<int>();
		Dictionary<int, List<CSItem>> dic = new Dictionary<int, List<CSItem>>();

		// 先是 队伍
		// 再是 其他
		foreach (CSItem item in itemList)
		{
			HeroInfo heroInfo = item.GetHeroInfo();
			if (heroInfo == null)
			{
				UnityEngine.Debug.Log("SortCardByGotTime  heroInfo == null , id = " + item.IDInTable);
				continue;
			}
			if (item.Guid == User.Singleton.RepresentativeCard)
			{
				continue;
			}

			int rarity = heroInfo.Rarity;
			// 队伍中的
			if (Team.Singleton.IsCardInTeam(item.Guid))
			{
				if (dicTeam.ContainsKey(rarity))
				{
					dicTeam[rarity].Add(item);
				}
				else
				{
					List<CSItem> tmpItemList = new List<CSItem>();
					tmpItemList.Add(item);
					dicTeam.Add(rarity, tmpItemList);
					tempTeam.Add(rarity);
				}
			}
			else
			{
				if (dic.ContainsKey(rarity))
				{
					dic[rarity].Add(item);
				}
				else
				{
					List<CSItem> tmpItemList = new List<CSItem>();
					tmpItemList.Add(item);
					dic.Add(rarity, tmpItemList);
					temp.Add(rarity);
				}
			}
		}
		tempTeam.Sort();
		temp.Sort();
		if (!reverse)
		{
			tempTeam.Reverse();
			temp.Reverse();
		}

		List<CSItem> resultList = new List<CSItem>();
		resultList.Clear();
		foreach (int rarity in tempTeam)
		{
			dicTeam[rarity].Sort(new ItemIdCompare());
			foreach (CSItem item in dicTeam[rarity])
			{
				resultList.Add(item);
			}
		}
		foreach (int rarity in temp)
		{
			dic[rarity].Sort(new ItemIdCompare());
			foreach (CSItem item in dic[rarity])
			{
				resultList.Add(item);
			}
		}
		//全部统计好了，将ID返回出去
		List<CSItemGuid> guidList = new List<CSItemGuid>();
		guidList.Add(User.Singleton.RepresentativeCard);
			
		foreach (CSItem item in resultList)
		{
			guidList.Add(item.Guid);
		}
		return guidList;
	}

	//属性排序（物理攻击， 魔法攻击， hp）
	public List<CSItemGuid> SortByFlotProperty(ENSortType sortType, bool reverse)
	{
		//队伍
		List<float> tempTeam = new List<float>();
        Dictionary<float, List<CSItem>> dicTeam = new Dictionary<float, List<CSItem>>();
		//其他
		List<float> temp = new List<float>();
        Dictionary<float, List<CSItem>> dic = new Dictionary<float, List<CSItem>>();


		// 先是 队伍 
		// 再是 其他

		foreach (CSItem item in itemList)
		{
			HeroInfo heroInfo = item.GetHeroInfo();
			if (heroInfo == null)
			{
				UnityEngine.Debug.Log("SortCardByGotTime  heroInfo == null , id = " + item.IDInTable);
				continue;
			}
			if (item.Guid == User.Singleton.RepresentativeCard)
			{
				continue;
			}

			float propVal = 0f;
			switch (sortType)
			{
				case ENSortType.enByPhyAttack:
					propVal = item.GetPhyAttack();
					break;
				case ENSortType.enByMagAttack:
					propVal = item.GetMagAttack();
					break;
				case ENSortType.enByHp:
					propVal = item.GetHp();
					break;
				default:
					UnityEngine.Debug.Log("SortByProperty err, type = " + sortType);
					return null;
			}
			// 队伍中的
			if (Team.Singleton.IsCardInTeam(item.Guid))
			{
				if (dicTeam.ContainsKey(propVal))
				{
					dicTeam[propVal].Add(item);
				}
				else
				{
                    List<CSItem> tmpItemList = new List<CSItem>();
					tmpItemList.Add(item);
					dicTeam.Add(propVal, tmpItemList);
					tempTeam.Add(propVal);
				}
			}
			else
			{
				if (dic.ContainsKey(propVal))
				{
					dic[propVal].Add(item);
				}
				else
				{
                    List<CSItem> tmpItemList = new List<CSItem>();
                    tmpItemList.Add(item);
					dic.Add(propVal, tmpItemList);
					temp.Add(propVal);
				}
			}
		}
		tempTeam.Sort();
		temp.Sort();
		if (!reverse)
		{
			tempTeam.Reverse();
			temp.Reverse();
		}

        List<CSItem> resultList = new List<CSItem>();
		resultList.Clear();
		foreach (float propVal in tempTeam)
		{
			dicTeam[propVal].Sort(new ItemGotTimeCompare());
			foreach (CSItem item in dicTeam[propVal])
			{
				resultList.Add(item);
			}
		}
		foreach (float propVal in temp)
		{
			dic[propVal].Sort(new ItemGotTimeCompare());
			foreach (CSItem item in dic[propVal])
			{
				resultList.Add(item);
			}
		}
		//全部统计好了，将ID返回出去
		List<CSItemGuid> guidList = new List<CSItemGuid>();
		guidList.Add(User.Singleton.RepresentativeCard);
			
		foreach (CSItem item in resultList)
		{
			guidList.Add(item.Guid);
		}
		return guidList;
	}

    public List<SortableItem> SortByList(ENSortType sortType,ENSortClassType classType, bool reverse, List<SortableItem> sortList)
    {
        List<float> tempTeam = new List<float>();
        Dictionary<float, List<CSItem>> dicTeam = new Dictionary<float, List<CSItem>>();

        Dictionary<float, List<SortableItem>> dic = new Dictionary<float, List<SortableItem>>();
        List<float> tmpKeyList = new List<float>();
        foreach (SortableItem item in sortList)
        {
            float propVal = 0f;
            switch (classType)
            {
                case ENSortClassType.enCSItem:
                    CSItem itemData = (CSItem)item;
                    propVal = GetSortValue(itemData, sortType, out dicTeam, out tempTeam);
                    break;
                case ENSortClassType.enFriend:
                    FriendItem tmpFriendItem = (FriendItem)item;
                    propVal = GetSortValue(tmpFriendItem, sortType, out dicTeam, out tempTeam);
                    break;
                default:
                    UnityEngine.Debug.Log("SortByProperty err, type = " + sortType);
                    return null;
            }
            if (dic.ContainsKey(propVal))
            {
                dic[propVal].Add(item);
            }
            else
            {
                List<SortableItem> tmpItemList = new List<SortableItem>();
                tmpItemList.Add(item);
                dic.Add(propVal, tmpItemList);
                tmpKeyList.Add(propVal);
            }
        }
        tmpKeyList.Sort();
        tempTeam.Reverse();
        if (!reverse)
        {
            tempTeam.Reverse();
            tmpKeyList.Reverse();
        }
        List<SortableItem> resultList = new List<SortableItem>();
        resultList.Clear();
        foreach (float propVal in tempTeam)
        {
            dicTeam[propVal].Sort(new ItemGotTimeCompare());
            foreach (CSItem item in dicTeam[propVal])
            {
                resultList.Add(item);
            }
        }
        foreach (float propVal in tmpKeyList)
        {
            
            if (dic.ContainsKey(propVal))
            {
                if (ENSortClassType.enCSItem == classType)
                {
                    dic[propVal].Sort(new ItemGotTimeCompareMy());
                }

                foreach (SortableItem item in dic[propVal])
                {
                    resultList.Add(item);
                }
            }
            
        }
//         //全部统计好了，将ID返回出去
//         List<CSItemGuid> guidList = new List<CSItemGuid>();
//         guidList.Add(User.Singleton.RepresentativeCard);
// 
//         foreach (CSItem item in resultList)
//         {
//             guidList.Add(item.Guid);
//         }
        return resultList;

//         List<int> tmp = new List<int>();
//         return tmp;
    }

    public float GetSortValue(SortableItem item, ENSortType sortType, out Dictionary<float, List<CSItem>> dicTeam, out List<float> tempTeam)
    {
        tempTeam = new List<float>();
        dicTeam = new Dictionary<float, List<CSItem>>();
        FriendItem friendItem = (FriendItem)item;
        if (sortType != ENSortType.enLoadTime)
        {
            CSItem itemData = friendItem.GetItem();
            return GetSortValue(itemData, sortType, out dicTeam, out tempTeam);
        }
        

        return friendItem.beforLoadTime();
    }
    public float GetSortValue(CSItem item, ENSortType sortType, out Dictionary<float, List<CSItem>> dicTeam, out List<float> tempTeam)
    {
        tempTeam = new List<float>();
        dicTeam = new Dictionary<float, List<CSItem>>();
        float propVal = 0f;
        switch (sortType)
        {
            case ENSortType.enByPhyAttack:
                propVal = item.GetPhyAttack();
                break;
            case ENSortType.enByMagAttack:
                propVal = item.GetMagAttack();
                break;
            case ENSortType.enByHp:
                propVal = item.GetHp();
                break;
            default:
                UnityEngine.Debug.Log("SortByProperty err, type = " + sortType);
                break;
               // return 0;
        }
        if (Team.Singleton.IsCardInTeam(item.Guid))
        {
            if (dicTeam.ContainsKey(propVal))
            {
                dicTeam[propVal].Add(item);
            }
            else
            {
                List<CSItem> tmpItemList = new List<CSItem>();
                tmpItemList.Add(item);
                dicTeam.Add(propVal, tmpItemList);
                tempTeam.Add(propVal);
            }
        }
        return propVal;
    }

	//int属性排序（level,favorite）
	public List<CSItemGuid> SortByIntProperty(ENSortType sortType, bool reverse)
	{
		//队伍
		List<int> tempTeam = new List<int>();
        Dictionary<int, List<CSItem>> dicTeam = new Dictionary<int, List<CSItem>>();
		//其他
		List<int> temp = new List<int>();
        Dictionary<int, List<CSItem>> dic = new Dictionary<int, List<CSItem>>();

		// 先是 队伍 
		// 再是 其他
		foreach (CSItem item in itemList)
		{
			HeroInfo heroInfo = item.GetHeroInfo();
			if (heroInfo == null)
			{
				UnityEngine.Debug.Log("SortCardByGotTime  heroInfo == null , id = " + item.IDInTable);
				continue;
			}
			if (item.Guid == User.Singleton.RepresentativeCard)
			{
				continue;
			}

			int propVal = 0;
			switch (sortType)
			{
				case ENSortType.enByLevel:
					propVal = item.Level;
					break;
				case ENSortType.enByFavorite:
					propVal = item.GetHeroCardPart().m_favorite == true?1:0;
					break;
				default:
					UnityEngine.Debug.Log("SortByProperty err, type = " + sortType);
					return null;
			}
			// 队伍中的
			if (Team.Singleton.IsCardInTeam(item.Guid))
			{
				if (dicTeam.ContainsKey(propVal))
				{
					dicTeam[propVal].Add(item);
				}
				else
				{
                    List<CSItem> tmpItemList = new List<CSItem>();
					tmpItemList.Add(item);
					dicTeam.Add(propVal, tmpItemList);
					tempTeam.Add(propVal);
				}
			}
			else
			{
				if (dic.ContainsKey(propVal))
				{
					dic[propVal].Add(item);
				}
				else
				{
                    List<CSItem> tmpItemList = new List<CSItem>();
					tmpItemList.Add(item);
					dic.Add(propVal, tmpItemList);
					temp.Add(propVal);
				}
			}
		}
		tempTeam.Sort();
		temp.Sort();
		if (!reverse)
		{
			tempTeam.Reverse();
			temp.Reverse();
		}

		List<CSItem> resultList = new List<CSItem>();
		resultList.Clear();
		foreach (int propVal in tempTeam)
		{
			dicTeam[propVal].Sort(new ItemGotTimeCompare());
			foreach (CSItem item in dicTeam[propVal])
			{
				resultList.Add(item);
			}
		}
		foreach (int propVal in temp)
		{
			dic[propVal].Sort(new ItemGotTimeCompare());
			foreach (CSItem item in dic[propVal])
			{
				resultList.Add(item);
			}
		}
		//全部统计好了，将ID返回出去
		List<CSItemGuid> guidList = new List<CSItemGuid>();
		guidList.Add(User.Singleton.RepresentativeCard);
		foreach (CSItem item in resultList)
		{
			guidList.Add(item.Guid);
		}
		return guidList;
	}

	//职业， 种族
	public List<CSItemGuid> SortCardByOccOrRace(ENSortType sortType, bool reverse)
	{
		//队伍
		List<int> tempTeam = new List<int>();
        Dictionary<int, List<CSItem>> dicTeam = new Dictionary<int, List<CSItem>>();
		//其他
		List<int> temp = new List<int>();
        Dictionary<int, List<CSItem>> dic = new Dictionary<int, List<CSItem>>();

		// 先是 队伍
		// 再是 其他
		foreach (CSItem item in itemList)
		{
			HeroInfo heroInfo = item.GetHeroInfo();
			if (heroInfo == null)
			{
				UnityEngine.Debug.Log("SortCardByGotTime  heroInfo == null , id = " + item.IDInTable);
				continue;
			}
			if (item.Guid == User.Singleton.RepresentativeCard)
			{
				continue;
			}

			int intVal = 0;
			switch (sortType)
			{
				case ENSortType.enByOccupation:
					intVal = heroInfo.Occupation;
					break;
				case ENSortType.enByType:
					intVal = heroInfo.Type;
					break;
				default:
					UnityEngine.Debug.Log("SortCardByOccOrRace err, type = " + sortType);
					return null;
			}
			// 队伍中的
			if (Team.Singleton.IsCardInTeam(item.Guid))
			{
				if (dicTeam.ContainsKey(intVal))
				{
					dicTeam[intVal].Add(item);
				}
				else
				{
                    List<CSItem> tmpItemList = new List<CSItem>();
					tmpItemList.Add(item);
					dicTeam.Add(intVal, tmpItemList);
					tempTeam.Add(intVal);
				}
			}
			else
			{
				if (dic.ContainsKey(intVal))
				{
					dic[intVal].Add(item);
				}
				else
				{
                    List<CSItem> tmpItemList = new List<CSItem>();
					tmpItemList.Add(item);
					dic.Add(intVal, tmpItemList);
					temp.Add(intVal);
				}
			}
		}
		tempTeam.Sort();
		temp.Sort();
		//职业默认从小到大
		if (reverse)
		{
			tempTeam.Reverse();
			temp.Reverse();
		}

		List<CSItem> resultList = new List<CSItem>();
		resultList.Clear();
		foreach (int intVal in tempTeam)
		{
			dicTeam[intVal].Sort(new ItemGotTimeCompare());
			foreach (CSItem item in dicTeam[intVal])
			{
				resultList.Add(item);
			}
		}
		foreach (int intVal in temp)
		{
			dic[intVal].Sort(new ItemGotTimeCompare());
			foreach (CSItem item in dic[intVal])
			{
				resultList.Add(item);
			}
		}
		//全部统计好了，将ID返回出去
		List<CSItemGuid> guidList = new List<CSItemGuid>();
		guidList.Add(User.Singleton.RepresentativeCard);
		foreach (CSItem item in resultList)
		{
			guidList.Add(item.Guid);
		}
		return guidList;
	}

	//////////////////////////////特殊的排序/////////////////////////////////////
	//出售
	public List<CSItemGuid> SortCardBySellPrice(ENSortType sortType, bool reverse)
	{
		//不包含队伍，和最爱
		List<int> temp = new List<int>();
        Dictionary<int, List<CSItem>> dic = new Dictionary<int, List<CSItem>>();

		foreach (CSItem item in itemList)
		{
			HeroInfo heroInfo = item.GetHeroInfo();
			if (heroInfo == null)
			{
				UnityEngine.Debug.Log("SortCardByGotTime  heroInfo == null , id = " + item.IDInTable);
				return null;
			}
			if (item.Guid == User.Singleton.RepresentativeCard)
			{
				continue;
			}

			int intVal = 0;
			switch (sortType)
			{
				case ENSortType.enGold:
					intVal = heroInfo.Price;
					break;
				case ENSortType.enRing:
					intVal = heroInfo.Ring;
					break;
				default:
					UnityEngine.Debug.Log("SortCardByOccOrRace err, type = " + sortType);
					return null;
			}

			// 队伍中的 和最爱的不参与排序
			if (Team.Singleton.IsCardInTeam(item.Guid))
			{
				continue;
			}
			if (item.GetHeroCardPart().m_favorite)
			{
				continue;
			}

			if (dic.ContainsKey(intVal))
			{
				dic[intVal].Add(item);
			}
			else
			{
                List<CSItem> tmpItemList = new List<CSItem>();
				tmpItemList.Add(item);
				dic.Add(intVal, tmpItemList);
				temp.Add(intVal);
			}

		}
		temp.Sort();
		if (!reverse)
		{
			temp.Reverse();
		}

        List<CSItem> resultList = new List<CSItem>();
		resultList.Clear();
		foreach (int intVal in temp)
		{
			dic[intVal].Sort(new ItemGotTimeCompare());
			foreach (CSItem item in dic[intVal])
			{
				resultList.Add(item);
			}
		}
		//全部统计好了，将ID返回出去
		List<CSItemGuid> guidList = new List<CSItemGuid>();
		foreach (CSItem item in resultList)
		{
			guidList.Add(item.Guid);
		}
		return guidList;
	}

	//是否可进化（最特殊）
	public List<CSItemGuid> SortCardByEvolve(ENSortType sortType, bool reverse)
	{
		//该序列不做逆序排列
		List<CSItem> list1 = new List<CSItem>();	//队伍中 	等级满足		素材满足		亮
		List<CSItem> list2 = new List<CSItem>();	//非队中 	等级满足		素材满足		亮
		List<CSItem> list3 = new List<CSItem>();	//队伍中 	等级不满		素材满足		亮
		List<CSItem> list4 = new List<CSItem>();	//非队中 	等级不满		素材满足		亮
		List<CSItem> list5 = new List<CSItem>();	//队伍中 	等级满足		素材不满		暗
		List<CSItem> list6 = new List<CSItem>();	//非队中		等级满足		素材不满		暗
		List<CSItem> list7 = new List<CSItem>();	//队伍中		等级不满		素材不满		暗
		List<CSItem> list8 = new List<CSItem>();	//非队中		等级不满		素材不满		暗

		int	inTeam = 1<<2;
		int	levelEnough = 1<<1;
		int foodCardEnough = 1 << 0;
		
		Dictionary<int,List<CSItem> > listDic = new Dictionary<int, List<CSItem> >();
		{
			listDic.Add((inTeam | levelEnough | foodCardEnough), list1);  //111	7
			listDic.Add((		  levelEnough | foodCardEnough), list2);	//011	3
			listDic.Add((inTeam |			    foodCardEnough), list3);	//101	5
			listDic.Add((				        foodCardEnough ) ,list4 );	//001	1
			listDic.Add((inTeam | levelEnough			       ), list5);	//110	6
			listDic.Add( (		   levelEnough					) ,list6 );	//010	2
			listDic.Add( (inTeam            					) ,list7 );	//100	4
			listDic.Add( (0										) ,list8 );	//000	0
		}

		foreach (CSItem item in itemList)
		{
			HeroInfo heroInfo = item.GetHeroInfo();
			if (heroInfo == null)
			{
				UnityEngine.Debug.Log("SortCardByGotTime  heroInfo == null , id = " + item.IDInTable);
				continue;
			}
			if (item.Guid == User.Singleton.RepresentativeCard)
			{
				continue;
			}
			int resultFlag = 0;
			//是否在队伍里
			if (Team.Singleton.IsCardInTeam(item.Guid))
			{
				resultFlag = resultFlag | inTeam;
			}
			if (item.ReachEvlotionLevel())
			{
				resultFlag = resultFlag | levelEnough;
			}

			if (CardBag.Singleton.CardEvolutionFoodEnough(item.Guid))
			{
				resultFlag = resultFlag | foodCardEnough;
			}
			//加入到对应的list
			listDic[resultFlag].Add(item);
		}
        foreach (KeyValuePair<int, List<CSItem>> listItem in listDic)
		{
			listItem.Value.Sort(new ItemGotTimeCompare());
		}
		
		//特殊处理代表卡,只考虑等级和材料
		int RepresentativeCardFlag = 0;
		CSItem repCard = CardBag.Singleton.GetCardByGuid(User.Singleton.RepresentativeCard);
		if (repCard != null)
		{
			if (repCard.ReachEvlotionLevel())
			{
				RepresentativeCardFlag = RepresentativeCardFlag | levelEnough;
			}

			if (CardBag.Singleton.CardEvolutionFoodEnough(repCard.Guid))
			{
				RepresentativeCardFlag = RepresentativeCardFlag | foodCardEnough;
			}
		}

		if (RepresentativeCardFlag == (levelEnough | foodCardEnough))
		{
			list1.Insert(0, repCard);
		}
		else if (RepresentativeCardFlag == foodCardEnough)
		{
			list3.Insert(0, repCard);
		}
		else if (RepresentativeCardFlag == levelEnough)
		{
			list5.Insert(0, repCard);
		}
		else if (RepresentativeCardFlag == 0)
		{
			list7.Insert(0, repCard);
		}

		List<CSItem> resultList = new List<CSItem>();
		resultList.Clear();
		{
			resultList.AddRange(list1);
			resultList.AddRange(list2);
			resultList.AddRange(list3);
			resultList.AddRange(list4);
			resultList.AddRange(list5);
			resultList.AddRange(list6);
			resultList.AddRange(list7);
			resultList.AddRange(list8);
		}
				
		//全部统计好了，将ID返回出去
		List<CSItemGuid> guidList = new List<CSItemGuid>();
		foreach (CSItem item in resultList)
		{
			guidList.Add(item.Guid);
		}
		return guidList;
	}
	#endregion

	//通知从外部调用
	public void NotifySortResult()
	{
		NotifyChanged((int)ENPropertyChanged.enCardSort, null);
	}

	public void NotifySelect()
	{
		NotifyChanged((int)ENPropertyChanged.enCardSelect, null);
	}

	public void GetTestCard()
	{
		if (Team.Singleton.GetCard(1, Team.EDITTYPE.enMain) == null)
		{
			AddCard(5);

		}

		if (Team.Singleton.GetCard(1, Team.EDITTYPE.enDeputy) == null)
		{
			AddCard(25);

		}

		if (Team.Singleton.GetCard(1, Team.EDITTYPE.enSupport) == null)
		{
			AddCard(6);

		}
	}

	void InitSortString()
	{
		m_sortStringByType = new Dictionary<int, string>();
        m_sortStringByType.Add((int)ENSortType.enDefault, Localization.Get("DefaultSort"));
        m_sortStringByType.Add((int)ENSortType.enGotTime, Localization.Get("GotTime"));
        m_sortStringByType.Add((int)ENSortType.enByRarity, Localization.Get("Rarity"));
        m_sortStringByType.Add((int)ENSortType.enByPhyAttack, Localization.Get("PhyAtk"));
        m_sortStringByType.Add((int)ENSortType.enByMagAttack, Localization.Get("MagAtk"));
        m_sortStringByType.Add((int)ENSortType.enByHp, Localization.Get("Hp"));
        m_sortStringByType.Add((int)ENSortType.enByOccupation, Localization.Get("Occ"));
        m_sortStringByType.Add((int)ENSortType.enByType, Localization.Get("Race"));
        m_sortStringByType.Add((int)ENSortType.enByLevel, Localization.Get("Level"));
        m_sortStringByType.Add((int)ENSortType.enByFavorite, Localization.Get("Favorite"));
        m_sortStringByType.Add((int)ENSortType.enCanEvolve, Localization.Get("CanEvolve"));
        m_sortStringByType.Add((int)ENSortType.enGold, Localization.Get("SellGold"));
        m_sortStringByType.Add((int)ENSortType.enRing, Localization.Get("SellRing"));
	}

	public string GetSrotString(ENSortType sortType)
	{
		string retStr = "";
		if (m_sortStringByType.TryGetValue((int)sortType, out retStr))
		{
			return retStr;
		}
		return "";
	}
}


public class ItemIdCompare : IComparer<CSItem>
{
    //按ID排序
    public int Compare(CSItem x, CSItem y)
    {
		return x.IDInTable.CompareTo(y.IDInTable);
    }
}

public class ItemGotTimeCompare : IComparer<CSItem>
{
	//按ID排序
	public int Compare(CSItem x, CSItem y)
	{
		return x.GotTime.CompareTo(y.GotTime);
	}
}

public class ItemGotTimeCompareMy : IComparer<SortableItem>
{
    //按ID排序
    public int Compare(SortableItem tmpx, SortableItem tmpy)
    {
        CSItem x = (CSItem)tmpx;
        CSItem y = (CSItem)tmpy;
        return x.GotTime.CompareTo(y.GotTime);
    }
}
//FriendItemList 根据种族或职业排序
public class FriendItemGetOccCompare : IComparer<SortableItem>
{
    public int Compare(SortableItem tmpx, SortableItem tmpy)
    {
        FriendItem friendItemx = (FriendItem)tmpx;
//        FriendItem friendItemy = (FriendItem)tmpy;
        CSItem x = friendItemx.GetItem();
        CSItem y = friendItemx.GetItem();
        return x.GotTime.CompareTo(y.GotTime);
    }
}