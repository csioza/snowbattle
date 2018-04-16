using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class UIFriendList : UIWindow
{
    GameObject m_item = null;
    GameObject m_sortBtn = null;            //      排序按钮
    UICardSort m_cardSortUI;                //排序 UIWindow
    UILabel m_cardSortLable = null;         //排序按钮文字
    UILabel m_curFrdNumLab = null;          //当前朋友个数Lab
    UILabel m_capacityNumLab = null;        //当前朋友个数Lab
    //UIScrollBar m_scrollBar = null;         //滑动条
    //GirdLis
    Dictionary<int, UIWindow> m_friendList = null;
    UIGrid m_friendGridList = null;
    //Dictionary<in>
    static public UIFriendList GetInstance()
    {
        UIFriendList self = UIManager.Singleton.GetUIWithoutLoad<UIFriendList>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIFriendList>("UI/UIFriendList", UIManager.Anchor.Center);
        return self;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        GameObject.Destroy(m_item);
        UIGrid.Destroy(m_friendGridList);
        foreach (UIWindow item in m_friendList.Values)
        {
            item.Destroy();
        }
        m_friendList.Clear();

        m_cardSortUI.Destroy();
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enFriendList, OnPropertyChanged);
        m_item = FindChild("ItemPlace");
        m_sortBtn = FindChild("CardSort");
        m_friendGridList = FindChildComponent<UIGrid>("GridList");
        m_cardSortLable = FindChildComponent<UILabel>("CardSortLabel");
        m_curFrdNumLab = FindChildComponent<UILabel>("Capacity");
        m_capacityNumLab = FindChildComponent<UILabel>("MaxCapacity");
//        m_scrollBar = FindChildComponent<UIScrollBar>("List");
        m_friendList = new Dictionary<int, UIWindow>();
        m_cardSortUI = UICardSort.Load();
        m_cardSortUI.SetParentWnd(this);

        InitBaseDate();


    }
    //对UICardSort的数据进行操作

    //=============================================
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {

        if (eventType == (int)Guild.ENPropertyChanged.enShow)
        {
//             if (MainUIManager.Singleton.ChangeUI(this))
//             {
//                 InitFriendList();
//                 ShowWindow();
//                 UpdateInfo();
//                 int msgID = MiniServer.Singleton.SendGetSocialityList_C2CH((int)FriendItemType.enAll);
//                 RegisterRespondFuncByMessageID(msgID, OnServerMsgCallBack);
//             }
        }

        else if (eventType == (int)FriendList.ENPropertyChanged.enHide)
        {
            HideWindow();
        }
        else if (eventType == (int)FriendList.ENPropertyChanged.enSortUpdateInfo)
        {
            //FriendList.Singleton.SortCards(ENSortType.enDefault, true);
            UpdateInfo();
        }
        else if (eventType == (int)FriendList.ENPropertyChanged.enAddFriend)
        {
            OnAddFriend();
        }
        else if (eventType == (int)FriendList.ENPropertyChanged.enDeleteFriend)
        {
            OnDeleteFriend();
        }
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        //AddChildMouseClickEvent("Button_Enter", OnEnterBtn);
        AddMouseClickEvent(m_sortBtn, OnClickSortBtn);
    }

    public override void OnShowWindow()
    {
        base.OnShowWindow();
        InitFriendList();
        UpdateInfo();
        int msgID = MiniServer.Singleton.SendGetSocialityList_C2CH((int)FriendItemType.enAll);
        RegisterRespondFuncByMessageID(msgID, OnServerMsgCallBack);
    }

    public override void OnHideWindow()
    {
        base.OnHideWindow();
        FriendList.Singleton.m_sortFriendList = null;
        FriendList.Singleton.m_sortType = ENSortType.enDefault;
    }

    // 初始化UI内数据信息
    public void InitBaseDate()
    {

    }

    // 初始化背包
    public void InitFriendList()
    {
        FriendList.Singleton.m_sortFriendList = null;
    }

     // 增加格子 index 格子唯一索引 从1开始，x,y为位置
    void AddGrid(int index, FriendItem FrienditemInfo)
    {

        // 如果存在 则返回
        if (m_friendList.ContainsKey(index))
        {
            return;
        }
        
        // 设置 父物体
        //friendItem.transform.parent = 

        // 设置大小
        //friendItem.transform.localScale = m_item.transform.localScale;

        //friendItem.gameObject.SetActive(true);

        // String.Format("{0:00000}", index);
        //friendItem.gameObject.name = showNumStr;

        UIFriendItem friendItemcc = new UIFriendItem();
        friendItemcc.SetClickFuncHead(OnClickPlayer, OnClickCard, OnClickLongPressPlayer, OnClickLongPressCard);
        friendItemcc.SetFriendItemClickType(UIFriendItem.ENClickFriendItemType.enFriendList);
        friendItemcc.Load("UIFriendItem", FindChildComponent<UIGrid>("GridList").transform);
        friendItemcc.SetFriendItemParamID(index);
        //UpdateUIInfo(FrienditemInfo, friendItemcc);
        
//        UIPanel panel = friendItemcc.WindowRoot.GetComponent<UIPanel>();
        
        m_friendList.Add(index, friendItemcc);
    }

    void OnClickPlayer(object sender, EventArgs e)
    {
         GameObject gameObj = (GameObject)sender;
         Parma param = gameObj.transform.parent.GetComponent<Parma>();
         //UIRepresentativeCard.GetInstance().ShowGUID(param.m_id, UIRepresentativeCard.ENOptType.enFriendType);
         FriendItem tmpFriendItem = null;
         if (FriendList.Singleton.m_sortFriendList != null)
         {
             tmpFriendItem = (FriendItem)FriendList.Singleton.m_sortFriendList[param.m_id];
         }
         else
         {
             tmpFriendItem = (FriendItem)FriendList.Singleton.m_friendInfoList[param.m_id];
         }
         UIRepresentativeCard.GetInstance().ShowFriendPlayerInfo(tmpFriendItem);
    }
    void OnClickCard(object sender, EventArgs e)
    {
        GameObject obj = (GameObject)sender;
        Parma parma = obj.transform.parent.GetComponent<Parma>();
        Debug.Log("OnClickCard---------------------" + parma.m_id);

    }
    void OnClickLongPressPlayer(object sender, EventArgs e)
    {
        GameObject gameObj = (GameObject)sender;
        Parma param = gameObj.transform.parent.GetComponent<Parma>();
        UIRepresentativeCard.GetInstance().ShowGUID(param.m_id, UIRepresentativeCard.ENOptType.enFriendType);
    }
    void OnClickLongPressCard(object sender, EventArgs e)
    {
        GameObject obj = (GameObject)sender;
        Parma parma = obj.transform.parent.GetComponent<Parma>();
        CSItem card = new CSItem();
        FriendItem tmpFriendItem;
        if (FriendList.Singleton.m_sortFriendList != null)
        {
            tmpFriendItem = (FriendItem)FriendList.Singleton.m_sortFriendList[parma.m_id];
            card = tmpFriendItem.GetItem();
        }
        else
        {
            tmpFriendItem = (FriendItem)FriendList.Singleton.m_friendInfoList[parma.m_id];
            card = tmpFriendItem.GetItem();
        }

        FriendList.Singleton.OnShowCardDetail(card);
    }
    public void UpdateInfo()
    {
//         UILabel popupList = FindChild("PopupList").transform.FindChild("Label").GetComponent<UILabel>();
//         Debug.Log("-------------" + popupList.text);
        int tmpIndex = 0;
        m_cardSortLable.text = CardBag.Singleton.GetSrotString(FriendList.Singleton.m_sortType);
        
        List<SortableItem> m_infoList = GetFriendListByType();
        m_capacityNumLab.text = "/" + FriendList.Singleton.m_friendCapcity;
        
        foreach (FriendItem itemInfo in m_infoList)
        {
            int cardID = itemInfo.GetItem().IDInTable;
            HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(cardID);
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
            if ((itemInfo.m_relation & (int)FriendItemType.enFriend) == 0)
            {
                continue;
            }
            if (!m_friendList.ContainsKey(tmpIndex))
            {
                AddGrid(tmpIndex, itemInfo);
            }
            UpdateUIInfo(itemInfo, (UIFriendItem)m_friendList[tmpIndex],tmpIndex);
            tmpIndex++;
        }
        m_curFrdNumLab.text = "" + tmpIndex;
        for (; tmpIndex < m_friendList.Count; tmpIndex++)
        {
            m_friendList[tmpIndex].HideWindow();
            m_friendList[tmpIndex].WindowRoot.GetComponent<Parma>().m_id = -1;
        }
        m_friendGridList.Reposition();
    }
    public List<SortableItem> GetFriendListByType()
    {

        if (FriendList.Singleton.m_sortFriendList != null)
        {
            return FriendList.Singleton.m_sortFriendList;
        }
        else
        {
            return FriendList.Singleton.GetFriendInfoList();
        }
    }
    public void UpdateUIInfo(FriendItem itemInfo, UIFriendItem friendItem, int index)
    {
        friendItem.UpdatePlayerInfo(itemInfo);
        friendItem.UpDateRepresentativeCard(itemInfo.GetItem());
        friendItem.SetFriendItemParamID(index);
        friendItem.ShowWindow();
    }
//     public void UpdatePlayerInfo(FriendItem itemInfo, Transform transform)
//     {
//         
//         
//         Transform playerTransForm = transform.FindChild("Player");
//         UILabel playerLevel = playerTransForm.FindChild("PlayerLevel").GetComponent<UILabel>();
//         playerLevel.text = "Lv." + itemInfo.m_level;
//         UILabel playerName = playerTransForm.FindChild("PlayerName").GetComponent<UILabel>();
//         playerName.text = itemInfo.m_actorName;
//     }
// 
//     public void UpDateRepresentativeCard(CSItem item, Transform transform)
//     {
//         Transform playerTransForm = transform.FindChild("RepresentativeCard");
//         UILabel cardLevel = playerTransForm.FindChild("Level").GetComponent<UILabel>();
//         cardLevel.text = ""+item.Level;
//         UISprite cardHeadSprite = playerTransForm.FindChild("headPortrait").GetComponent<UISprite>();
//         int cardID = item.IDInTable;
//         HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(cardID);
//         IconInfo iconInfo = GameTable.IconTableAsset.Lookup(heroInfo.headImageId);
//         cardHeadSprite.spriteName = iconInfo.SpriteName;
// 
//     }

    // 点击返回按钮
    public void OnClickSortBtn(object sender, EventArgs e)
    {
        m_cardSortUI.m_curSortClassType = ENSortClassType.enFriend;
        m_cardSortUI.ShowOrHide();
    }

    public void OnAddFriend()
    {
        int msgID = MiniServer.Singleton.SendAddFriend_C2CH(FriendList.Singleton.m_curDisposeFriendItemID);
        RegisterRespondFuncByMessageID(msgID, OnServerMsgCallBack);
    }

    public void OnDeleteFriend()
    {
        int msgID = MiniServer.Singleton.SendDelFriend_C2CH(FriendList.Singleton.m_curDisposeFriendItemID);
        RegisterRespondFuncByMessageID(msgID, OnDelFriendCallBack);
    }

    public void OnServerMsgCallBack(MessageRespond respond)
    {
        /*Loading.Singleton.Hide();*/
        if (respond.IsSuccess)
        {
            UpdateInfo();
        }
        Debug.Log("UIFriendList:OnServerMsgCallBack");
    }
    public void OnAddFriendCallBack(MessageRespond respond)
    {

    }
    public void OnDelFriendCallBack(MessageRespond respond)
    {
        if (respond.IsSuccess)
        {
            return;
        }
    }
}


