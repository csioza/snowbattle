using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class UIApplyList : UIWindow
{
    GameObject m_item = null;
    UILabel m_curFrdNumLab = null;      //当前朋友个数Lab
    UILabel m_capacityNumLab = null;      //当前朋友个数Lab
    //UIScrollBar m_scrollBar = null;         //滑动条
    FriendItem m_foundFriendItem = null;    
    //GirdLis
    Dictionary<int, UIWindow> m_friendList = null;
    UIGrid m_gridList = null;
    GameObject m_optBtnList = null;         //批处理按钮
    GameObject m_optBtn = null;             //操作按钮
    //Dictionary<in>
    static public UIApplyList GetInstance()
    {
        UIApplyList self = UIManager.Singleton.GetUIWithoutLoad<UIApplyList>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIApplyList>("UI/UIApplyList", UIManager.Anchor.Center);
        return self;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        GameObject.Destroy(m_item);
        UIGrid.Destroy(m_gridList);
        foreach (UIWindow item in m_friendList.Values)
        {
            item.Destroy();
        }
        m_friendList.Clear();
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enApplyList, OnPropertyChanged);

        m_item = FindChild("ItemPlace");
        m_gridList = FindChildComponent<UIGrid>("GridList");
        m_curFrdNumLab = FindChildComponent<UILabel>("Capacity");
        m_capacityNumLab = FindChildComponent<UILabel>("MaxCapacity");
//        m_scrollBar = FindChildComponent<UIScrollBar>("List");
        m_optBtnList = FindChild("UIFriendConfirmOption");
        m_optBtnList.SetActive(false);
        m_optBtn = FindChild("OptBtn");
        m_friendList = new Dictionary<int, UIWindow>();
    }
    //对UICardSort的数据进行操作
    //=============================================
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {

        if (eventType == (int)ApplyList.ENPropertyChanged.enShow)
        {
//             if (MainUIManager.Singleton.ChangeUI(this))
//             {
//                 UpdateInfo();
//                 ShowWindow();
//                 int msgID = MiniServer.Singleton.SendGetSocialityList_C2CH((int)FriendItemType.enAll);
//                 RegisterRespondFuncByMessageID(msgID, OnServerMsgCallBack);
//                 //int msgID = MiniServer.Singleton.SendGetSocialityList_C2CH();
//                 //RegisterRespondFuncByMessageID(msgID, OnServerMsgCallBack);
//             }
        }
        else if (eventType == (int)ApplyList.ENPropertyChanged.enHide)
        {
            HideWindow();
        }
        if (eventType == (int)ApplyList.ENPropertyChanged.enSortUpdateInfo)
        {
            UpdateInfo();
        }
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        //AddChildMouseClickEvent("Button_Enter", OnEnterBtn);
        AddMouseClickEvent(m_optBtn, OnClickOptBtn);

        AddMouseClickEvent(FindChild("AcceptAllBtn"), OnClickAcceptAllBtn);
        AddMouseClickEvent(FindChild("RefuseAllBtn"), OnClickRefuseAllBtnAllBtn);
    }

    public override void OnShowWindow()
    {
 	     base.OnShowWindow();
         UpdateInfo();
         int msgID = MiniServer.Singleton.SendGetSocialityList_C2CH((int)FriendItemType.enAll);
         RegisterRespondFuncByMessageID(msgID, OnServerMsgCallBack);
    }
    public override void OnHideWindow()
    {

        base.OnHideWindow();
        m_optBtnList.SetActive(false);
    }
    //------------------------点击事件回调函数------------------------------------
    //点击更多操作按钮
    public void OnClickOptBtn(object sender, EventArgs e)
    {
        m_optBtnList.SetActive(!m_optBtnList.activeSelf);
    }
    //点击接受全部请求按钮
    public void OnClickAcceptAllBtn(object sender, EventArgs e)
    {
        UICommonMsgBoxCfg boxCfg = FindChild("AcceptAllBtn").GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnAcceptAllBtnYes, OnAcceptAllBtnNo, boxCfg); 
    }
    //接受全部申请二次确认框的确定按钮
    public void OnAcceptAllBtnYes(object sender, EventArgs e)
    {
        ApplyList.Singleton.OnAcceptAll();
    }
    //接受全部申请二次确认框的取消按钮
    public void OnAcceptAllBtnNo(object sender, EventArgs e)
    {

    }
    //拒绝全部申请按钮
    public void OnClickRefuseAllBtnAllBtn(object sender, EventArgs e)
    {
        UICommonMsgBoxCfg boxCfg = FindChild("RefuseAllBtn").GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnRefuseAllBtnYes, OnRefuseAllBtnNo, boxCfg); 
    }
    //拒绝全部申请二次确认框的确定按钮
    public void OnRefuseAllBtnYes(object sender, EventArgs e)
    {
        ApplyList.Singleton.OnRefuseAll();
    }
    //拒绝全部申请二次确认框的取消按钮
    public void OnRefuseAllBtnNo(object sender, EventArgs e)
    {

    }
    //点击FriendItem人物信息回调
    public void OnClickPlayer(object sender, EventArgs e)
    {
        GameObject obj = (GameObject)sender;
        Parma parma = obj.transform.parent.GetComponent<Parma>();
        Debug.Log("OnClickIPlayer---------------------" + parma.m_id);
        UIFriendItem friendItem = CreateFriendItemPrefab((FriendItem)FriendList.Singleton.m_friendInfoList[parma.m_id]);
        UIFriendMessageBox.GetInstance().InsertUIWnd(friendItem);
        UIFriendMsgBoxCfg FriendboxCfg = FindChild("InnerFrame").GetComponent<UIFriendMsgBoxCfg>();
        UIFriendCfgItem friendCfgItem = FriendboxCfg.ItemList[0];
        UIFriendMessageBox.GetInstance().ShowMsgBox(OnButtonYes, OnButtonNo, OnButtonRefuserBtn, friendCfgItem);
    }
    //点击FriendItem头像卡牌回调
    void OnClickCard(object sender, EventArgs e)
    {
        GameObject obj = (GameObject)sender;
        Parma parma = obj.transform.parent.GetComponent<Parma>();
        Debug.Log("OnClickIPlayer---------------------" + parma.m_id);

        UIFriendItem friendItem = CreateFriendItemPrefab((FriendItem)FriendList.Singleton.m_friendInfoList[parma.m_id]);
        UIFriendMessageBox.GetInstance().InsertUIWnd(friendItem);
        UIFriendMsgBoxCfg FriendboxCfg = FindChild("InnerFrame").GetComponent<UIFriendMsgBoxCfg>();
        UIFriendCfgItem friendCfgItem = FriendboxCfg.ItemList[0];
        UIFriendMessageBox.GetInstance().ShowMsgBox(OnButtonYes, OnButtonNo, OnButtonRefuserBtn, friendCfgItem);
    }
    //长按FriendItem人物信息回调
    void OnClickLongPressPlayer(object sender, EventArgs e)
    {
//        GameObject gameObj = (GameObject)sender;
//        Parma param = gameObj.transform.parent.GetComponent<Parma>();
    }
    //长按FriendItem头像卡牌回调
    void OnClickLongPressCard(object sender, EventArgs e)
    {
//        GameObject obj = (GameObject)sender;
//        Parma parma = obj.transform.parent.GetComponent<Parma>();
    }
    //对单个FriendItem操作的确认按钮
    public void OnButtonYes(object sender, EventArgs e)
    {
        if (FriendList.Singleton.IsSureToAddFriend(1))
        {
            int msgID = MiniServer.Singleton.SendPassFriendRequest(m_foundFriendItem.m_id);
            RegisterRespondFuncByMessageID(msgID, OnPassFriendServerMsgCallBack);
        }
        else
        {
            //二次确认框，好友人数达到上限；
        }

    }
    //对单个FriendItem操作的取消按钮
    public void OnButtonNo(object sender, EventArgs e)
    {

    }
    //对单个FriendItem操作的拒绝按钮
    public void OnButtonRefuserBtn(object sender, EventArgs e)
    {
        ApplyList.Singleton.OnDeleteFriend(m_foundFriendItem, false);
    }
    //-----------------------------------------------------------------------------------------

    // 增加格子 index 格子唯一索引 从1开始，x,y为位置
    void AddGrid(int index, FriendItem FrienditemInfo)
    {
        // 如果存在 则返回
        if (m_friendList.ContainsKey(index))
        {
            return;
        }

        //创建FriendItem窗口
        UIFriendItem friendItemcc = new UIFriendItem();
        friendItemcc.SetClickFuncHead(OnClickPlayer, OnClickCard, OnClickLongPressPlayer, OnClickLongPressCard);
        friendItemcc.SetFriendItemClickType(UIFriendItem.ENClickFriendItemType.enApplyList);
        friendItemcc.Load("UIFriendItem", FindChildComponent<UIGrid>("GridList").transform);
        friendItemcc.SetFriendItemParamID(index);
        //UpdateUIInfo(FrienditemInfo, friendItemcc);
        
//        UIPanel panel = friendItemcc.WindowRoot.GetComponent<UIPanel>();
        m_friendList.Add(index, friendItemcc);
    }

    public UIFriendItem CreateFriendItemPrefab(FriendItem item)
    {
        m_foundFriendItem = item;// FoundFriendByID.Singleton.m_findFriendItem;//CreateFriendItemData(0);
        UIFriendItem friendItem = new UIFriendItem();
        friendItem.Load("UIFriendItem", null);
        int tmpIndex = FriendList.Singleton.GetFriendListCount();
        friendItem.SetFriendItemParamID(tmpIndex + 1);
        UpdateUIInfo(m_foundFriendItem, friendItem, tmpIndex + 1);
        friendItem.HideWindow();
        return friendItem;
    }
    
    public void UpdateInfo()
    {
        int tmpIndex = 0;
        int friendListIndex = -1;
        List<SortableItem> m_infoList = GetFriendListByType();
        m_capacityNumLab.text = "/" + FriendList.Singleton.m_applyCapcity;
        
        foreach (FriendItem itemInfo in m_infoList)
        {
            // 职业刷选
            friendListIndex++;
            if ((itemInfo.m_relation & (int)FriendItemType.enUntreatedRequest) == 0)
            {
                continue;
            }
            if (!m_friendList.ContainsKey(tmpIndex))
            {
                AddGrid(tmpIndex, itemInfo);
            }
            UpdateUIInfo(itemInfo, (UIFriendItem)m_friendList[tmpIndex], friendListIndex);
            tmpIndex++;
        }
        m_curFrdNumLab.text = "" + tmpIndex;
        for (; tmpIndex < m_friendList.Count; tmpIndex++)
        {
            m_friendList[tmpIndex].HideWindow();
            m_friendList[tmpIndex].WindowRoot.GetComponent<Parma>().m_id = -1;
        }
        m_gridList.Reposition();
    }
    public List<SortableItem> GetFriendListByType()
    {
        return FriendList.Singleton.GetFriendInfoList();
    }
    public void UpdateUIInfo(FriendItem itemInfo, UIFriendItem friendItem, int tmpIndex)
    {
        friendItem.UpdatePlayerInfo(itemInfo);
        friendItem.UpDateRepresentativeCard(itemInfo.GetItem());
        friendItem.SetFriendItemParamID(tmpIndex);
        friendItem.WindowRoot.SetActive(true);
    }
    public void OnServerMsgCallBack(MessageRespond respond)
    {
        /*Loading.Singleton.Hide();*/
        if (respond.IsSuccess)
        {
            UpdateInfo();
        }
        Debug.Log("UIApplyList:OnServerMsgCallBack");
    }
    public void OnPassFriendServerMsgCallBack(MessageRespond respond)
    {
        if (respond.IsSuccess)
        {
            UpdateInfo();
        }
        
    }
}


