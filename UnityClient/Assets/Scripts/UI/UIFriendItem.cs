using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class UIFriendItem : UIWindow
{
    public enum ENClickFriendItemType
    {
        enFriendList,
        enApplyList,
    }
    public delegate void OnButtonCallbacked(object sender, EventArgs e);
    EventHandler<EventArgs> m_clickPlayerCallbacked;
    EventHandler<EventArgs> m_clickCardCallbacked;
    EventHandler<EventArgs> m_clickLongPlayerCallbacked;
    EventHandler<EventArgs> m_clickLongCardCallbacked;

    public ENClickFriendItemType m_clickFriendItemType = ENClickFriendItemType.enFriendList;
    public bool m_clickAble = true;
    public void Load(string strPrefab, Transform parent = null)
    {
        //base.Load("UI/" + strPrefab);
        LoadAsync("UI/" + strPrefab);
        SetParent(parent);
        //AttachEvent();
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        if (!m_clickAble)
        {
            return;
        }
        AddMouseClickEvent(this.WindowRoot.transform.Find("Player").gameObject, m_clickPlayerCallbacked);
        AddMouseClickEvent(this.WindowRoot.transform.Find("RepresentativeCard").gameObject, m_clickCardCallbacked);
        AddMouseLongPressEvent(this.WindowRoot.transform.Find("Player").gameObject, m_clickLongPlayerCallbacked);
        AddMouseLongPressEvent(this.WindowRoot.transform.Find("RepresentativeCard").gameObject, m_clickLongCardCallbacked);
//         if (ENClickFriendItemType.enFriendList == m_clickFriendItemType)
//         {
//             
//         }
//         else if (ENClickFriendItemType.enApplyList == m_clickFriendItemType)
//         {
//             AddMouseClickEvent(this.WindowRoot.transform.FindChild("Player").gameObject, OnApplyListClickPlayer);
//             AddMouseClickEvent(this.WindowRoot.transform.FindChild("RepresentativeCard").gameObject, OnApplyListClickPlayer);
//             AddMouseLongPressEvent(this.WindowRoot.transform.FindChild("Player").gameObject, OnApplyListClickLongPressPlayer);
//             AddMouseLongPressEvent(this.WindowRoot.transform.FindChild("RepresentativeCard").gameObject, OnApplyListClickLongPressCard);
//         }
        
    }

    public void SetClickFuncHead(EventHandler<EventArgs> clickPlayerCallbacked, EventHandler<EventArgs> clickCardCallbacked, EventHandler<EventArgs> clickLongPlayerCallbacked, EventHandler<EventArgs> clickLongCardCallbacked)
    {
        m_clickPlayerCallbacked = clickPlayerCallbacked;
        m_clickCardCallbacked = clickCardCallbacked;
        m_clickLongPlayerCallbacked = clickLongPlayerCallbacked;
        m_clickLongCardCallbacked = clickLongCardCallbacked;
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        m_clickPlayerCallbacked = null;
        m_clickCardCallbacked = null;
        m_clickLongPlayerCallbacked = null;
        m_clickLongCardCallbacked = null;
    }
    //----------------设置FriendItem预设信息-----Star--------------------------
    public void SetFriendItemParent(Transform objTransForm)
    {
        this.WindowRoot.transform.parent = objTransForm;
    }

    public void SetFriendItemName(string friendItemName)
    {
        this.WindowRoot.gameObject.name = friendItemName;
    }
    public void SetFriendItemParamID(int index)
    {
        Parma param = this.WindowRoot.transform.GetComponent<Parma>();
        param.m_id = index;
        string showNumStr = index.ToString("00000");
        SetFriendItemName(showNumStr);
        SetLocalScale(Vector3.one);
    }
    public void SetFriendItemClickType(ENClickFriendItemType type)
    {
        m_clickFriendItemType = type;
    }
    //--------------设置FriendItem预设信息-----Over--------------------------

    //--------------添加FriendItem预设点击响应函数----------------------------
    void OnApplyListClickPlayer(object sender, EventArgs e)
    {
        
    }
    void OnFriendListClickPlayer(object sender, EventArgs e)
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
    void OnApplyListClickCard(object sender, EventArgs e)
    {

    }
    void OnFriendListClickCard(object sender, EventArgs e)
    {
        GameObject obj = (GameObject)sender;
        Parma parma = obj.transform.parent.GetComponent<Parma>();
        Debug.Log("OnClickCard---------------------" + parma.m_id);
    }
    void OnApplyListClickLongPressPlayer(object sender, EventArgs e)
    {

    }

    void OnFriendListClickLongPressPlayer(object sender, EventArgs e)
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

    void OnApplyListClickLongPressCard(object sender, EventArgs e)
    {

    }
    void OnFriendListClickLongPressCard(object sender, EventArgs e)
    {
        GameObject obj = (GameObject)sender;
        Parma parma = obj.transform.parent.GetComponent<Parma>();
        CSItem card = new CSItem();
        FriendItem tmpFriendItem;
        if (FriendList.Singleton.m_friendListType == FriendList.EDITTYPE.enFriendList)
        {
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

        }
        else if (FriendList.Singleton.m_friendListType == FriendList.EDITTYPE.enApplyList)
        {
            ;
        }
        FriendList.Singleton.OnShowCardDetail(card);
    }
    //---------------------------------------------------------------------------------
    //--------刷新FriendItem显示信息-----------------------------------------
    public void UpdatePlayerInfo(FriendItem itemInfo)
    {

        Transform transform = this.WindowRoot.transform;
        Transform playerTransForm = transform.Find("Player");
        UILabel playerLevel = playerTransForm.Find("PlayerLevel").GetComponent<UILabel>();
        playerLevel.text = "Lv." + itemInfo.m_level;
        UILabel playerName = playerTransForm.Find("PlayerName").GetComponent<UILabel>();
        playerName.text = itemInfo.m_actorName;
		int serverTime = (int)TimeUtil.GetServerTimeStampNow();
		int playerLastLoginTime = (int)itemInfo.beforLoadTime();
		int diff = (serverTime - playerLastLoginTime)/60; //分钟
		string showText = "";
		if (diff <= 0)
		{
			diff = 1;
		}
		if (diff < 60)
		{
			showText = string.Format(Localization.Get("LastLoginTimeMin"), diff.ToString());
		}
		else
		{
			diff = diff / 60; //小时
			if(diff < 24)
			{
				showText = string.Format(Localization.Get("LastLoginTimeHour"), diff.ToString());
			}
			else
			{
				diff = diff / 24; //天
				showText = string.Format(Localization.Get("LastLoginTimeDay"), diff.ToString());
			}
		}
		UILabel lastLoginTime = playerTransForm.Find("LastLoginTime").GetComponent<UILabel>();
		lastLoginTime.text = showText;

		UILabel chooseNum = playerTransForm.Find("ChosenNum").GetComponent<UILabel>();
		chooseNum.text = string.Format(Localization.Get("selecttime"), itemInfo.m_choiceCount);

    }

    public void UpDateRepresentativeCard(CSItem item)
    {
        Transform transform = this.WindowRoot.transform;
        Transform playerTransForm = transform.Find("RepresentativeCard");
        UILabel cardLevel = playerTransForm.Find("Level").GetComponent<UILabel>();
        cardLevel.text = "" + item.Level;
        UITexture cardHeadSprite = playerTransForm.Find("headPortrait").GetComponent<UITexture>();
        int cardID =  item.IDInTable;
        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(cardID);

        if ( null == heroInfo )
        {
            return;
        }

        IconInfomation iconInfo     = GameTable.IconInfoTableAsset.Lookup(heroInfo.headImageId);
        if ( null == iconInfo )
        {
            return;
        }
        cardHeadSprite.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);

    }


	//--------------------Add by Keith------------------------
	FriendItem m_friendItem = null;
	public void SetFriendItem (FriendItem item)
	{
		m_friendItem = item;
	}
	public FriendItem GetFriendItem()
	{
		return m_friendItem;
	}
	public static UIFriendItem CreateFriendItemPrefab(FriendItem item)
	{
		UIFriendItem ui = new UIFriendItem();
		ui.SetFriendItem(item);
		ui.Load("UIFriendItem", null);
		ui.HideWindow();
		return ui;
	}

	public void UpdateUIInfo( )
	{
		UpdatePlayerInfo(m_friendItem);
		WindowRoot.SetActive(true);
	}
    //----------------------------------------------------------
}
