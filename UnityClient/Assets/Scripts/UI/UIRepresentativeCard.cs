using UnityEngine;
using System.Collections;
using System;

public class UIRepresentativeCard : UIWindow 
{
	int m_userGUID;
    public enum ENOptType
    {
        enRepreType,
        enFriendType,
    }

    //ENOptType m_curOptType = ENOptType.enRepreType;
	//玩家信息
	UILabel m_userName;		//玩家名字
	UILabel m_userLevel;	//玩家等级
	UILabel m_labelGUID;	//玩家GUID

	//代表卡信息
	UITexture m_cardPortrait;//代表卡图片
	UILabel m_cardName;		//卡牌名
	UILabel m_cardLevel;	//等级
	GameObject m_rarityObj;	//星级
	UITexture m_raritySprite;
	UILabel m_occAndRace;	//职业种族
	UILabel m_phyAttack;	//物理攻击
	UILabel m_magAttack;	//魔法攻击
	UILabel m_hp;			//血
	UILabel m_breakthroughTimes;	//突破次数


	GameObject m_btnTeamLeaderModel;		//使用队长按钮
	GameObject m_btnCancelTeamLeaderMode;	//取消使用队长
	GameObject m_btnDetail;			//查看详情
	GameObject m_btnChangeCard;		//更换代表卡
	GameObject m_btnCancel;			//取消
	GameObject m_btnSelectThis;		//选择按钮

    GameObject m_detailBtn;         //好友界面，查看详情
    GameObject m_emailBtn;          //好友界面， 邮箱
    GameObject m_deleteFriendBtn;    //好友界面，删除好友
    GameObject m_cancelBtn;         //好友界面，取消按钮
    FriendItem m_frienditem;        //当前好友信息

	static public UIRepresentativeCard GetInstance()
	{
		UIRepresentativeCard self = UIManager.Singleton.GetUIWithoutLoad<UIRepresentativeCard>();
		if (self != null)
		{
			return self;
		}
		self = UIManager.Singleton.LoadUI<UIRepresentativeCard>("UI/UIRepresentativeCard", UIManager.Anchor.Center);
		return self;
	}

	public override void OnInit()
	{
		base.OnInit();
		GameObject userInfoObj = FindChild("UserInfo");
		//玩家信息
		m_userName = FindChild("LabelUserName", userInfoObj).GetComponent<UILabel>();
		m_userLevel = FindChild("LabelUserLevel", userInfoObj).GetComponent<UILabel>();
		m_labelGUID = FindChild("LabelGUID", userInfoObj).GetComponent<UILabel>();

		GameObject cardInfoObj = FindChild("CardInfo");
		//代表卡信息
		m_cardPortrait = FindChild("SpritePortrait", cardInfoObj).GetComponent<UITexture>();
		GameObject infoObj = FindChild("Info", cardInfoObj);
		m_cardName = FindChild("LabelCardName", infoObj).GetComponent<UILabel>();
		m_cardLevel = FindChild("LabelCardLevel", infoObj).GetComponent<UILabel>();
		m_raritySprite = FindChildComponent<UITexture>("RaritySprite", infoObj);
		m_occAndRace = FindChild("LabelRaceOcc", infoObj).GetComponent<UILabel>();
		m_phyAttack = FindChild("LabelPhyAttackVal", infoObj).GetComponent<UILabel>();
		m_magAttack = FindChild("LabelMagAttackVal", infoObj).GetComponent<UILabel>();
		m_hp = FindChild("LabeHPVal", infoObj).GetComponent<UILabel>();
		m_breakthroughTimes = FindChild("LabeBreakthroughVal", infoObj).GetComponent<UILabel>();
		m_btnTeamLeaderModel = FindChild("BtnTeamLeader", cardInfoObj);
		m_btnCancelTeamLeaderMode = FindChild("BtnCancelTeamLeader", cardInfoObj);
		m_btnDetail = FindChild("BtnDetail");
		m_btnChangeCard = FindChild("BtnChangeCard");
		m_btnCancel = FindChild("BtnCancel");
		m_btnSelectThis = FindChild("BtnSelectThis");

        m_detailBtn = FindChild("DetailBtn");        //好友界面， 查看详情
        m_emailBtn = FindChild("EmailBtn");          //好友界面， 邮箱
        m_deleteFriendBtn = FindChild("FriendDeleteBtn");    //好友界面，删除好友
        m_cancelBtn = FindChild("CancelBtn");         //好友界面，取消按钮
	}

	public void ShowGUID(int userGuid,ENOptType optType)
	{
		m_userGUID = userGuid;

        FindChild("RepresentativeBtnList").SetActive(true);
        FindChild("FriendBtnList").SetActive(false);
        RefreshInfo();
        ShowWindow();
	}

	void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
	{

	}

	public override void AttachEvent()
	{
		AddMouseClickEvent(m_btnTeamLeaderModel, OnTeamLeaderModelClicked);
		AddMouseClickEvent(m_btnCancelTeamLeaderMode, OnCancelTeamLeaderModelClicked);
		AddMouseClickEvent(m_btnDetail, OnDetailClicked);
		AddMouseClickEvent(m_btnChangeCard, OnChangeCardClicked);
		AddMouseClickEvent(m_btnCancel, OnCancelClicked);
		AddMouseClickEvent(m_btnSelectThis, OnSelectThisClicked);

        AddMouseClickEvent(m_detailBtn, OnFriendDeailBtn);
        AddMouseClickEvent(m_emailBtn, OnFriendEmailBtn);
        AddMouseClickEvent(m_deleteFriendBtn, OnFriendDeleteBtn);
        AddMouseClickEvent(m_cancelBtn, OnFriendCancelBtn);
	}

	public override void OnDestroy()
	{

	}

	public void RefreshInfo()
	{
		if (User.Singleton.Guid == m_userGUID)
		{
			ShowSelf();
		}
		else
		{
			ShowBattleHelper(m_userGUID);
		}	
	}

	void ShowSelf()
	{

		m_btnTeamLeaderModel.SetActive(User.Singleton.IsTeamLeaderModel);
		m_btnCancelTeamLeaderMode.SetActive(!User.Singleton.IsTeamLeaderModel);
		m_btnSelectThis.SetActive(false);
		m_btnChangeCard.SetActive(true);

		//userinfo
		m_userName.text = User.Singleton.UserProps.GetProperty_String(UserProperty.name);
        m_userLevel.text = Localization.Get("CardLevel") + User.Singleton.UserProps.GetProperty_Int32(UserProperty.level).ToString();
		m_labelGUID.text = "ID:" + User.Singleton.Guid.ToString();

		//代表卡info
		CSItemGuid cardGuid = User.Singleton.RepresentativeCard;
		CSItem card = CardBag.Singleton.GetCardByGuid(cardGuid);
		if (card != null)
		{
			Debug.Log("RefreshInfo");
			HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
			if (info == null)
			{
				return;
			}
			int headImageID = info.headImageId;
			IconInfomation icon = GameTable.IconInfoTableAsset.Lookup(headImageID);
            m_cardPortrait.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);
			m_cardName.text = info.StrName;
			m_cardLevel.text = card.Level.ToString();

            RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(info.Rarity);
            icon = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);
            m_raritySprite.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);
			int occ = info.Occupation;
			int race = info.Type;
			string occName = GameTable.OccupationInfoAsset.LookUp(occ).m_name;
			string raceName = GameTable.RaceInfoTableAsset.LookUp(race).m_name;
			m_occAndRace.text = occName + " " + raceName;
			m_phyAttack.text = ((int)card.GetPhyAttack()).ToString();
			m_magAttack.text = ((int)card.GetMagAttack()).ToString();
			m_hp.text = ((int)card.GetHp()).ToString();
			m_breakthroughTimes.text = card.BreakCounts.ToString();
		}
	}

	void ShowBattleHelper(int userGUID)
	{
		Helper helper = User.Singleton.HelperList.LookupHelper(userGUID);
		if (helper == null)
		{
			Debug.Log("No this helper, id = " + userGUID.ToString());
			ShowSelf();
			return;
		}
		m_btnTeamLeaderModel.SetActive(false);
		m_btnCancelTeamLeaderMode.SetActive(false);
		m_btnSelectThis.SetActive(true);
		m_btnChangeCard.SetActive(false);
		//userinfo
		m_userName.text = helper.m_userName;
        m_userLevel.text = Localization.Get("CardLevel") + helper.m_userLevel.ToString();
		m_labelGUID.text = "ID:" + helper.m_userGuid.ToString();

		CSItem card = new CSItem();
		card.Guid = helper.m_cardGuid;
		card.m_id = (short)helper.m_cardId;
		card.Level = helper.m_cardLevel;
		card.BreakCounts = helper.m_cardBreakCounts;
		card.m_segment.m_heroCard.m_skill = helper.m_cardSkill;

		//代表卡info
// 		CSItemGuid cardGuid = User.Singleton.RepresentativeCard;
// 		CSItem card = CardBag.Singleton.GetCardByGuid(cardGuid);
		if (card != null)
		{
			Debug.Log("RefreshInfo");
			HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
			if (info == null)
			{
				return;
			}
			int headImageID = info.headImageId;
            IconInfomation icon = GameTable.IconInfoTableAsset.Lookup(headImageID);
            m_cardPortrait.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);
			
			m_cardName.text = info.StrName;
			m_cardLevel.text = card.Level.ToString();
            RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(info.Rarity);
            icon = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);
            m_raritySprite.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);
			int occ = info.Occupation;
			int race = info.Type;
			string occName = GameTable.OccupationInfoAsset.LookUp(occ).m_name;
			string raceName = GameTable.RaceInfoTableAsset.LookUp(race).m_name;
			m_occAndRace.text = occName + " " + raceName;
			m_phyAttack.text = ((int)card.GetPhyAttack()).ToString();
			m_magAttack.text = ((int)card.GetMagAttack()).ToString();
			m_hp.text = ((int)card.GetHp()).ToString();
			m_breakthroughTimes.text = card.BreakCounts.ToString();
		}
	}
    public void ShowFriendPlayerInfo(FriendItem friendItem)
    {
		m_btnTeamLeaderModel.SetActive(false);
		m_btnCancelTeamLeaderMode.SetActive(false);

        FindChild("FriendBtnList").SetActive(true);
        FindChild("RepresentativeBtnList").SetActive(false);
        m_frienditem = friendItem;
        m_userName.text = friendItem.m_actorName;
        m_userLevel.text = Localization.Get("CardLevel") + friendItem.m_level;
        m_labelGUID.text = "ID:" + "000000";
        CSItem card = friendItem.GetItem();
        if (card != null)
		{
			Debug.Log("RefreshInfo");
			HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
			if (info == null)
			{
				return;
			}
			int headImageID = info.headImageId;
            IconInfomation icon = GameTable.IconInfoTableAsset.Lookup(headImageID);
            m_cardPortrait.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);
			m_cardName.text = info.StrName;
			m_cardLevel.text = card.Level.ToString();

            RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(info.Rarity);
            icon = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);
            m_raritySprite.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);

			int occ = info.Occupation;
			int race = info.Type;
			string occName = GameTable.OccupationInfoAsset.LookUp(occ).m_name;
			string raceName = GameTable.RaceInfoTableAsset.LookUp(race).m_name;
			m_occAndRace.text = occName + " " + raceName;
			m_phyAttack.text = ((int)card.GetPhyAttack()).ToString();
			m_magAttack.text = ((int)card.GetMagAttack()).ToString();
			m_hp.text = ((int)card.GetHp()).ToString();
			m_breakthroughTimes.text = card.BreakCounts.ToString();
		}
        ShowWindow();
    }
	public override void OnShowWindow()
	{
		//RefreshInfo();
	}

	//是否使用队长模式
	public void OnTeamLeaderModelClicked(object obj, EventArgs e)
	{
		GameObject gameObject = obj as GameObject;
		string prefabStr = "";
        UICommonMsgBoxCfg boxCfg = gameObject.GetComponent<UICommonMsgBoxCfg>();
		prefabStr = "UITeamLeaderModel";
		boxCfg.mainTextPrefab = prefabStr;
		UICommonMsgBox.GetInstance().ShowMsgBox(OnButtonYes, OnButtonNo, boxCfg);
		Debug.Log("OnTeamLeaderClicked");
	}
	public void OnCancelTeamLeaderModelClicked(object obj, EventArgs e)
	{
		GameObject gameObject = obj as GameObject;
		string prefabStr = "";
		UICommonMsgBoxCfg boxCfg = gameObject.GetComponent<UICommonMsgBoxCfg>();
		prefabStr = "UICancelTeamLeaderModeLabel";
		boxCfg.mainTextPrefab = prefabStr;
		UICommonMsgBox.GetInstance().ShowMsgBox(OnButtonYes, OnButtonNo, boxCfg);
		Debug.Log("OnCancelTeamLeaderModelClicked");
	}
	public void OnButtonYes(object sender, EventArgs e)
	{
		User.Singleton.IsTeamLeaderModel = !User.Singleton.IsTeamLeaderModel;
		m_btnTeamLeaderModel.SetActive(User.Singleton.IsTeamLeaderModel);
		m_btnCancelTeamLeaderMode.SetActive(!User.Singleton.IsTeamLeaderModel);
	}
	public void OnButtonNo(object sender, EventArgs e)
	{

	}

	public void OnDetailClicked(object obj, EventArgs e)
	{
		if (m_userGUID == User.Singleton.Guid)
		{
			// 显示卡牌详细界面 只带返回按钮
            CardBag.Singleton.OnShowCardDetail(CardBag.Singleton.GetCardByGuid(User.Singleton.RepresentativeCard),true);
		}
		else
		{
			Helper helper = User.Singleton.HelperList.LookupHelper(m_userGUID);
			CSItem card = new CSItem();
			card.Guid = helper.m_cardGuid;
			card.m_id = (short)helper.m_cardId;
			card.Level = helper.m_cardLevel;
			card.BreakCounts = helper.m_cardBreakCounts;
			card.m_segment.m_heroCard.m_skill = helper.m_cardSkill;
            CardBag.Singleton.OnShowCardDetail(card, true);
		}
	}
	//更换代表卡
	public void OnChangeCardClicked(object obj, EventArgs e)
	{
		Debug.Log("OnChangeCardClicked");

		OperateCardList.Singleton.ShowSelectRepresentiveCard(true);
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enOperaterCardList);
		//UIMain.GetInstance().HideWindow();

		HideWindow();
	}
	//取消
	public void OnCancelClicked(object obj, EventArgs e)
	{
		Debug.Log("OnCancelClicked");
		HideWindow();
	}
	//选择战友卡
	public void OnSelectThisClicked(object obj, EventArgs e)
	{
		StageMenu.Singleton.SetCurHelperGuid(m_userGUID);
		HideWindow();
	}

    public void OnFriendDeailBtn(object obj, EventArgs e)
    {
        FriendList.Singleton.OnShowCardDetail(m_frienditem.GetItem());
    }
    public void OnFriendDeleteBtn(object obj, EventArgs e)
    {
        UICommonMsgBoxCfg boxCfg = FindChild("FriendBtnList").transform.Find("FriendDeleteBtn").GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnFriendDelBtnYes, OnFriendBtnNo, boxCfg);
    }
    public void OnFriendCancelBtn(object obj, EventArgs e)
    {
        HideWindow();
    }
    public void OnFriendEmailBtn(object obj, EventArgs e)
    {

    }
    public void OnFriendDelBtnYes(object sender, EventArgs e)
    {
        FriendList.Singleton.OnDeleteFriend(m_frienditem.GetID());
        HideWindow();
    }
    public void OnFriendBtnNo(object sender, EventArgs e)
    {

    }
}
