using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct FrientActorInfo
{
    public int m_actorID;
    public string m_actorName;
    public int m_level;
    public int m_choiceCount;
    public long m_beforLoadTime;
    CSItem cardInfo;
}

public class FriendList : IPropertyObject
{

    // 显示类型
    public enum EDITTYPE
    {
        enFriendList,
        enApplyList,
        enDeputy,
        enSupport,
        enALL,
        enNone,
    }
    public enum ENPropertyChanged
    {
        enShow,
        enHide,
        enSortUpdateInfo,
        enOnServerSocialityList,
        enAddFriend,
        enDeleteFriend,
    }


    public EDITTYPE m_friendListType = EDITTYPE.enFriendList;
    public int m_friendCapcity { get { return User.Singleton.GetFriendCount(); } }
    public int m_applyCapcity { get { return GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enExpandFriendsNum).IntTypeValue; } }
    Dictionary<int, SortableItem> m_friendInfoDic = null;
    public List<SortableItem> m_friendInfoList = null;          //好友列表数据
    public List<SortableItem> m_sortFriendList = null;          //排序过后的数据
    public int m_curDisposeFriendItemID = -1;
    public ENSortType m_sortType = ENSortType.enDefault;

    public FriendList()
    {
        SetPropertyObjectID((int)MVCPropertyID.enFriendList);
        m_friendInfoDic = new Dictionary<int, SortableItem>();
        m_friendInfoList = new List<SortableItem>();
//         CreateFriendList();
//         CreateApplyInfoList();
    }

    #region Singleton
    static FriendList m_singleton;
    static public FriendList Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new FriendList();
            }
            return m_singleton;
        }
    }
    #endregion
    public void SortCards(ENSortType sortType, bool reverse)
	{
		if ( sortType == ENSortType.enDefault)
		{
            sortType = ENSortType.enLoadTime;
		}
        m_sortType = sortType;
        //m_lastDirReverse = reverse;
        m_sortFriendList = CardBag.Singleton.SortByList(sortType, ENSortClassType.enFriend, reverse, m_friendInfoList);
        SortCardUpdateInfo(); 
	}
    //测试，创建好友数据
    public void CreateFriendList()
    {
        for (int i = 9; i >= 0;i-- )
        {
            FriendItem tmpItem = CreateFriendItemData(i, FriendItemType.enFriend);
            m_friendInfoDic.Add(tmpItem.GetID(), tmpItem);
        }
    }
    //测试，创建确认好友列表数据
    public void CreateApplyInfoList()
    {
        for (int i = 20; i >= 15; i--)
        {
            FriendItem tmpItem = CreateFriendItemData(i, FriendItemType.enUntreatedRequest);
            m_friendInfoDic.Add(tmpItem.GetID(), tmpItem);
        }
    }
    public FriendItem CreateFriendItemData(int index, FriendItemType relation)
    {
        FriendItem tmpFriendItem = new FriendItem();
        tmpFriendItem.m_id = index;
        tmpFriendItem.m_level = index;
        tmpFriendItem.m_choiceCount = index;
        tmpFriendItem.m_beforLoadTime = 0;
        tmpFriendItem.m_actorName = index.ToString("00");
        tmpFriendItem.m_relation = (int)relation;
        CSItem tmpCSItem = new CSItem();
        int tmpIndex = 0;
//        int TableCount = GameTable.HeroInfoTableAsset.m_list.Count;
//        int randIndex = UnityEngine.Random.Range(0, TableCount);
        foreach (HeroInfo item in GameTable.HeroInfoTableAsset.m_list.Values)
        {
            if (index == tmpIndex)
            {
                tmpCSItem.IDInTable = (short)item.ID;
                break;
            }
            tmpIndex++;
        }
        // = (short)randIndex;
        tmpCSItem.Level        = index;
        tmpFriendItem.m_itemData = tmpCSItem;
        return tmpFriendItem;
    }
    //------------------------------------------------------
    public bool IsSureToAddFriend(int count)
    {
        return true;// m_curFriendCount + count <= m_friendCapcity;
    }
    public void RemoveAllFriendInfoDic(int type)
    {
        m_friendInfoDic.Clear();
    }

	public void RemoveFriendInfoByRelation(int relation)
	{
		List<int> removeList = new List<int>();
		foreach( var item in m_friendInfoDic )
		{
			FriendItem friendItem = (FriendItem)item.Value;
			if((friendItem.m_relation& relation) > 0 )
			{
				removeList.Add(item.Key);
			}
		}
		foreach(var removeID in removeList )
		{
			m_friendInfoDic.Remove(removeID);
		}
	}
    public void InitFriendInfoDic(FriendItem item)
    {
		if (!m_friendInfoDic.ContainsKey(item.GetID() ))
		{
			m_friendInfoDic.Add(item.GetID(), item);
		}
    }
    public void InitFriendInfoList()
    {
        m_friendInfoList.Clear();
        m_friendInfoList.AddRange(m_friendInfoDic.Values);
//         foreach (FriendItem item in m_friendInfoDic.Values)
//         {
// 
//         }
    }

    public void DeleteFriend(int friendItemID)
    {
        FriendItem tmpItem = m_friendInfoDic[friendItemID] as FriendItem;
        m_friendInfoDic.Remove(friendItemID);
        if (null != m_sortFriendList)
        {
            m_sortFriendList.Remove(tmpItem);
        }
        m_friendInfoList.Remove(tmpItem);
        SortCardUpdateInfo();
    }
    public void AddFriendFunc(FriendItem friendItem)
    {
        if (m_friendInfoDic.ContainsKey(friendItem.GetID()))
        {
            FriendItem tmpFriendItem = (FriendItem)m_friendInfoDic[friendItem.GetID()];

            tmpFriendItem.m_relation = friendItem.m_relation;/* (int)FriendItemType.enFriend;*/
        }
        else
        {
            m_friendInfoDic.Add(friendItem.GetID(), friendItem);
        }
        InitFriendInfoList();
        //SortCardUpdateInfo();
    }
    public List<SortableItem> GetFriendInfoList()
    {
        List<SortableItem> tmpFriendInfoList = new List<SortableItem>();
        tmpFriendInfoList.AddRange(m_friendInfoDic.Values);
        return tmpFriendInfoList;
    }
    public int GetFriendListCount()
    {
        return m_friendInfoList.Count;
    }
    public void OnShow()
    {
        NotifyChanged((int)ENPropertyChanged.enShow, null);
    }
    public void OnHide()
    {
        NotifyChanged((int)ENPropertyChanged.enHide, null);
    }
    public void SortCardUpdateInfo()
    {
        NotifyChanged((int)ENPropertyChanged.enSortUpdateInfo, null);
    }
    public void OnShowCardDetail(CSItem card)
    {
        CardBag.Singleton.OnShowCardDetail(card, true);
    }
    public void OnAddFriend(int friendItemID)
    {
        m_curDisposeFriendItemID = friendItemID;
        NotifyChanged((int)ENPropertyChanged.enAddFriend, null);
    }
    public void OnDeleteFriend(int friendItemID)
    {
        m_curDisposeFriendItemID = friendItemID;
        NotifyChanged((int)ENPropertyChanged.enDeleteFriend, null);
    }
    //     public void ShowUpdateTeamInfo(bool isCondition = true)
    //     {
    //         NotifyChanged((int)ENPropertyChanged.enUpdateTeam, isCondition);
    //     }
	
	public FriendItem LookupFriendItem(int guid)
	{
		SortableItem item = null;
		if(m_friendInfoDic.TryGetValue(guid,out item) )
		{
			return item as FriendItem;
		}
		return null;
	}
}
