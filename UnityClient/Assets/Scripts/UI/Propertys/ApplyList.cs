using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ApplyList : IPropertyObject
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
        enShowCardDetail,
    }

    public int m_friendCapcity { get { return 50; } }
    public List<SortableItem> m_applyInfoList = null;           //确认列表数据
    public CSItem m_cardForDetail = null;
    public int m_curApplyCount = 0;//m_friendInfoList.Count;

    public ApplyList()
    {
        SetPropertyObjectID((int)MVCPropertyID.enApplyList);
        m_applyInfoList = new List<SortableItem>();
        CreateApplyInfoList();
    }

    #region Singleton
    static ApplyList m_singleton;
    static public ApplyList Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new ApplyList();
            }
            return m_singleton;
        }
    }
    #endregion
    //获取服务器数据
    public void SetApplyInfoList(List<SortableItem> applyInfoList)
    {
        m_applyInfoList = applyInfoList;
    }
    public int GetCurApplyListCount()
    {
        return m_applyInfoList.Count;
    }

    //测试，创建确认好友列表数据
    public void CreateApplyInfoList()
    {
        for (int i = 20; i >= 15; i--)
        {
            m_applyInfoList.Add(CreateFriendItemData(i));
        }
    }

    public FriendItem CreateFriendItemData(int index)
    {
        FriendItem tmpFriendItem = new FriendItem();
        tmpFriendItem.m_id = index;
        tmpFriendItem.m_level = index;
        tmpFriendItem.m_choiceCount = index;
        tmpFriendItem.m_beforLoadTime = 0;
        tmpFriendItem.m_actorName = index.ToString("00");
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
        tmpCSItem.Level = index;
        tmpFriendItem.m_itemData = tmpCSItem;
        return tmpFriendItem;
    }

    public void InitApplyInfoList(FriendItem item)
    {
        m_applyInfoList.Add(item);
    }
    public void OnDeleteFriend(FriendItem friendItem, bool passResult)
    {
//        SortableItem tmp = (SortableItem)friendItem;
        m_applyInfoList.Remove(friendItem);
        if (passResult)
        {
            MiniServer.Singleton.SendPassFriendRequest(friendItem.m_id);
        }
        else
        {
            MiniServer.Singleton.SendDelFriend_C2CH(friendItem.m_id);
        }
       
        SortCardUpdateInfo();
    }
    public void OnAddFriend()
    {

    }
    public void OnAcceptAll()
    {
        foreach (FriendItem item in m_applyInfoList)
        {
            //向服务器发送消息
            FriendList.Singleton.OnAddFriend(item.GetID());
            //m_applyInfoList.Remove(item);
        }
        m_applyInfoList.Clear();
        //向服务器发送清空消息
        SortCardUpdateInfo();

    }
    public void OnRefuseAll()
    {
        m_applyInfoList.Clear();
        //向服务器发送清空消息
        SortCardUpdateInfo();

    }
    public int GetFriendListCount()
    {
        return m_applyInfoList.Count;
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
        m_cardForDetail = card;
        NotifyChanged((int)ENPropertyChanged.enShowCardDetail, null);
    }
    //     public void ShowUpdateTeamInfo(bool isCondition = true)
    //     {
    //         NotifyChanged((int)ENPropertyChanged.enUpdateTeam, isCondition);
    //     }

}
