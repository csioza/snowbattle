using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public class UIFoundFriendByID : UIWindow
{
    FriendItem m_foundFriendItem = null;

	GameObject myGuidObj = null;
    static public UIFoundFriendByID GetInstance()
    {
        UIFoundFriendByID self = UIManager.Singleton.GetUIWithoutLoad<UIFoundFriendByID>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIFoundFriendByID>("UI/UIFoundFriendByID", UIManager.Anchor.Center);
        return self;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enFoundFriendByID, OnPropertyChanged);
        UILabel userNameTxt = FindChildComponent<UILabel>("UserNameTxt");
        userNameTxt.text = "";

		myGuidObj = FindChild("MyGuid");
		RegisterRespondFuncByRespondCode((int)ENMessageRespond.enIDNotFound, OnPlayerIDNotFound);
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {

        if (eventType == (int)FoundFriendByID.ENPropertyChanged.enShow)
        {
//             if (MainUIManager.Singleton.ChangeUI(this))
//             {
//                 UpdateInfo();
//                 ShowWindow();
//             }
        }

        else if (eventType == (int)FoundFriendByID.ENPropertyChanged.enHide)
        {
            HideWindow();
        }
        else if (eventType == (int)FoundFriendByID.ENPropertyChanged.enLookupPlayer)
        {
            OnShowMsgBox(FoundFriendByID.ENMsgInfoType.enSuccess);
        }

    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("Button_Enter", OnEnterBtn);
    }

    public override void OnShowWindow()
    {
        base.OnShowWindow();
        UpdateInfo();
    }
    public override void OnHideWindow()
    {
        base.OnHideWindow();
    }

    public void UpdateInfo()
    {
        GameObject inputFriendID = FindChild("WebSiteInputFriendID");
        UIInput inputComponet = inputFriendID.transform.GetComponent<UIInput>();
        inputComponet.value = "";

		myGuidObj.GetComponent<UIInput>().value = User.Singleton.Guid.ToString();
    }

    public UIFriendItem CreateFriendItemPrefab()
    {
        m_foundFriendItem = FoundFriendByID.Singleton.m_findFriendItem;//CreateFriendItemData(0);
        UIFriendItem friendItem = new UIFriendItem();
        friendItem.Load("UIFriendItem", null);
        int tmpIndex = FriendList.Singleton.GetFriendListCount();
        friendItem.SetFriendItemParamID(tmpIndex+1);
        UpdateFriendItemInfo(m_foundFriendItem, friendItem);
        friendItem.HideWindow();
        return friendItem;
    }
    public UIFriendItem CreateFindFriendItemPrefab()
    {
        return null;
    }

    public void UpdateFriendItemInfo(FriendItem itemInfo, UIFriendItem friendItem)
    {
        friendItem.UpdatePlayerInfo(itemInfo);
        friendItem.UpDateRepresentativeCard(itemInfo.GetItem());
        friendItem.WindowRoot.SetActive(true);
    }

    // 点击确认按钮
    public void OnEnterBtn(GameObject obj)
    {
        GameObject inputFriendID = FindChild("WebSiteInputFriendID");
        UIInput inputComponet = inputFriendID.transform.GetComponent<UIInput>();
        //Debug.Log(("uifoundFriendByID-----------------:" + inputComponet.value));
        //MiniServer.Singleton.SendLookupPlayer_C2CE((int)inputComponet.value);
        if (inputComponet.value == "")
        {
            Debug.Log("------------------------Shu123321123");
            return;
        }
        int tmpFriendID = int.Parse(inputComponet.value);
        if (tmpFriendID == User.Singleton.GetUserID())
        {
            //弹出二次提示框Error Self
            UIFriendMsgBoxCfg boxCfg = FindChild("Button_Enter").GetComponent<UIFriendMsgBoxCfg>();
            UIFriendCfgItem friendCfgItem = boxCfg.ItemList[(int)FoundFriendByID.ENMsgInfoType.enErrorSelf];
            UIFriendMessageBox.GetInstance().ShowMsgBox(OnButtonYes, OnButtonNo, OnButtonRefuserBtn, friendCfgItem);
            return;
        }
        //OnShowMsgBox(FoundFriendByID.ENMsgInfoType.enErrorID);
        //向服务器发送带查找好友ID
        int msgID = MiniServer.Singleton.SendLookupPlayer_C2CE(tmpFriendID);
		RegisterRespondFuncByMessageID(msgID,OnPlayerIDNotFound);

        //加载loading界面
        //Loading.Singleton.Show();
    }
    public void OnButtonAccept(object sender, EventArgs e)
    {

    }

    public void OnButtonRefuserBtn(object sender, EventArgs e)
    {

    }

    public void OnButtonYes(object sender, EventArgs e)
    {
        int msgID = MiniServer.Singleton.SendAddFriend_C2CH(m_foundFriendItem.GetID());
        RegisterRespondFuncByMessageID(msgID, OnServerMsgCallBack);
        /*FriendList.Singleton.OnAddFriend(m_foundFriendItem.GetID());*/
    }
    public void OnButtonNo(object sender, EventArgs e)
    {

    }

    public void OnFriendBtnYes(object sender, EventArgs e)
    {

    }
    public void OnFriendBtnNo(object sender, EventArgs e)
    {

    }
    public void OnShowMsgBox(FoundFriendByID.ENMsgInfoType msgType)
    {
        
        switch (msgType)
        {
            case FoundFriendByID.ENMsgInfoType.enErrorID:
                break;
            case FoundFriendByID.ENMsgInfoType.enSuccess:
                /*Loading.Singleton.Hide();*/
                UIFriendItem friendItem = CreateFriendItemPrefab();
                UIFriendMessageBox.GetInstance().InsertUIWnd(friendItem);
                UIFriendMsgBoxCfg FriendboxCfg = FindChild("Button_Enter").GetComponent<UIFriendMsgBoxCfg>();
                UIFriendCfgItem friendCfgItem = FriendboxCfg.ItemList[(int)FoundFriendByID.ENMsgInfoType.enSuccess];
                UIFriendMessageBox.GetInstance().ShowMsgBox(OnButtonYes, OnButtonNo, OnButtonRefuserBtn, friendCfgItem);
                break;
            default:
                break;
        }
    }

	//好友查找失败
	public void OnPlayerIDNotFound(MessageRespond respond)
	{
        /*Loading.Singleton.Hide();*/
        if (respond.IsSuccess)
        {
            return;
        }
        Debug.Log("OnPlayerIDNotFound");
        UICommonMsgBoxCfg boxCfg = FindChild("Button_Enter").GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnFriendBtnYes, OnFriendBtnNo, boxCfg);   
	}
    public void OnServerMsgCallBack(MessageRespond respond)
    {
        if (respond.IsSuccess)
        {
            return;
        }
    }
}
