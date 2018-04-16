using NGE.Network;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#region disableWarning
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#endregion

public enum ENPlayerCardOptTypen
{
    //  强化
    enCardMerge =1,
    // 进化
    enCardEvolve=2,
	enSellCard =3,
}
public partial class MessageProcess
{
    #region singleton
    static private MessageProcess m_singleton;
	static public MessageProcess Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new MessageProcess();
			}
			return m_singleton;
		}
	}
    #endregion
    public MessageProcess()
	{
	}
	public void Init()
	{
        Debug.Log("--------------------------");
		// 成功连接时执行OnMainConnected函数
		ClientNet.Singleton.ShortConnect.Connected += new ConnectionCallback(OnMainConnected);
		// 连接失败时执行OnConnectFailed函数
        ClientNet.Singleton.ShortConnect.ConnectionFailed += new ConnectionFailedCallback(OnConnectFailed);

        ClientNet.Singleton.ShortConnect.ConnectionLost += new ConnectionFailedCallback(OnConnectFailed);
        //////////////////////////////////////////////////////////////////////////
        // 所有自定义消息
        // 登录
        ClientNet.Singleton.RegisterHandler(GamePacketID.msgLogin_S2C, new OnPacketReceive(OnMsgLogin_S2C));
        //  创建角色
        ClientNet.Singleton.RegisterHandler(GamePacketID.msgPlayerCreate_S2C, new OnPacketReceive(OnMsgPlayerCreate_S2C));
        // 朋友相关
        ClientNet.Singleton.RegisterHandler(GamePacketID.msgFriendList_S2C, new OnPacketReceive(OnMsgFriendList_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.msgFriendAdd_S2C, new OnPacketReceive(OnMsgFriendAdd_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.msgFriendAdd_Agree_S2C, new OnPacketReceive(OnMsgFriendAdd_Agree_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.msgFriendAdd_Refuse_S2C, new OnPacketReceive(OnMsgFriendAdd_Refuse_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.msgFriendDel_S2C, new OnPacketReceive(OnMsgFriendDel_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.msgFriendBeDel_S2C, new OnPacketReceive(OnMsgFriendBeDel_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.msgFriendChat_S2C, new OnPacketReceive(OnMsgFriendChat_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.msgFriendLogin_S2C, new OnPacketReceive(OnMsgFriendLogin_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.msgFriendLogout_S2C, new OnPacketReceive(OnMsgFriendLogout_S2C));

		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgMessageRespond_S2C, new OnPacketReceive(OnMsgMessageRespond_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgAccountOperateChar_S2C, new OnPacketReceive(OnMsgOperateChar_C2S));
        ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgAccountLoginResult_S2F2C,new OnPacketReceive(OnMsgAccountLoginResult_S2F2C) );
		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgPlayerData_S2D2C, new OnPacketReceive(OnMsgPlayerData_S2D2C));
		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSyncBagItem_S2C, new OnPacketReceive(OnMsgSyncBagItem_S2C));
		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgDelBagItem_S2C, new OnPacketReceive(OnMsgDelBagItem_S2C));
		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSyncItemBag_S2C, new OnPacketReceive(OnMsgSyncItemBag_S2C));
		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgServerPlayerGetReady_S2Ce2Fe2C, new OnPacketReceive(OnMsgServerPlayerGetReady_S2Ce2Fe2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSessionError_S2C, new OnPacketReceive(OnMsgSessionError_S2C));
		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSyncServerTime_S2C, new OnPacketReceive(OnMsgSyncServerTime_S2C));
		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSetRepresentativeCard_S2C, new OnPacketReceive(OnMsgSetRepresentativeCard_S2C));
		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSyncBattleHalper_CH2C, new OnPacketReceive(OnMsgSyncBattleHalper_CH2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSendMagicStoneInfo_S2C, new OnPacketReceive(OnMsgSendMagicStoneInfo_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgEnterDugeon_S2C, new OnPacketReceive(OnMsgEnterDugeon_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgDungeonEnd_S2C, new OnPacketReceive(OnMsgDungeonEnd_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgExpandCardBagCapacity_S2C, new OnPacketReceive(OnMsgExpandCardBagCapacity_S2C));    
// 		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSyncLevel_S2C, new OnPacketReceive(OnMsgSyncLevel_S2C));
// 		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSyncMoney_S2C, new OnPacketReceive(OnMsgSyncMoney_S2C));
// 		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSyncBindMoney_S2C, new OnPacketReceive(OnMsgSyncBindMoney_S2C));
// 		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSyncRing_S2C, new OnPacketReceive(OnMsgSyncRing_S2C));
// 		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSyncFriendshipPoint_S2C, new OnPacketReceive(OnMsgSyncFriendshipPoint_S2C));		
// 		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSyncStamina_S2C, new OnPacketReceive(OnMsgSyncStamina_S2C));		
// 		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgOneTeamData_S2C, new OnPacketReceive(OnMsgOneTeamData_S2C));
 //       ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgRecoverEnergySucc_S2C, new OnPacketReceive(OnMsgRecoverEnergySucc_S2C));
//		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSyncEnergy_S2C, new OnPacketReceive(OnMsgSyncEnergy_S2C));
        //ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgRecoverStaminaSucc_S2C, new OnPacketReceive(OnMsgRecoverStaminaSucc_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgRecoverSucc_S2C, new OnPacketReceive(OnMsgRecoverSucc_S2C));
        //ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgMagicStoneRecruitmentSucc_S2C, new OnPacketReceive(OnMsgMagicStoneRecruitmentSucc_S2C));
        //ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgFriendShipOnceRecruitmentSucc_S2C, new OnPacketReceive(OnMsgFriendShipOnceRecruitmentSucc_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgFriendShipMoreRecruitmentSucc_S2C, new OnPacketReceive(OnMsgFriendShipMoreRecruitmentSucc_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSendRingOfHonorInfo_S2C, new OnPacketReceive(OnMsgSendRingOfHonorInfo_S2C));
        //ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgBuyRingOfHonorCardSucc_S2C, new OnPacketReceive(OnMsgBuyRingOfHonorCardSucc_S2C));
		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSocialityItem_CH2C, new OnPacketReceive(OnMsgSocialityItem_S2C));
		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSocialityItemList_CH2C, new OnPacketReceive(OnMsgSocialityItemList_S2C));
//		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgLookupPlayerResult_CH2C, new OnPacketReceive(OnMsgLookupPlayerResult_CH2C));
		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgRemoveSocialityItem_CH2C, new OnPacketReceive(OnMsgRemoveSocialityItem_CH2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSendChangeNameSucc_S2C, new OnPacketReceive(OnMsgSendChangeNameSucc_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgInitCard_S2C, new OnPacketReceive(OnMsgInitCard_S2C));
		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSyncDirtyPropertys_S2C, new OnPacketReceive(OnMsgSyncDirtyPropertys_S2C));
		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSyncFloorInst_S2C, new OnPacketReceive(OnENMsgSyncFloorInst_S2C));
		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgFloorReliveReslut_S2C, new OnPacketReceive(OnMsgFloorReliveReslut_S2C));
		ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgServerLog_S2C, new OnPacketReceive(OnMsgServerLog_S2C));
        ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgSendUpdateCardSucc_S2C, new OnPacketReceive(OnMsgSendUpdateCardSucc_S2C));

        ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsgEnterLCDugeon_S2C, new OnPacketReceive(OnMsgEnterLCDugeon_S2C));

        if (ClientNet.Singleton.ShortConnect.Reactor.DefaultHandler == null)
		{
            ClientNet.Singleton.ShortConnect.Reactor.RegisterDefaultHandler(new OnPacketReceive(OnDefaultMessage));
		}
	}
	// 连接成功时执行的函数
	void OnMainConnected(ConnectionArgs args, object state)
	{
		MessageTransfer.Singleton.IsTryingConn = false;
		ClientNetProperty.Singleton.NotifyChanged((int)ClientNetProperty.ENPropertyChanged.enConnected, null);

        MessageTransfer.Singleton.SendCacheCachedMessage();
	}
	// 连接失败时执行的函数
	void OnConnectFailed(ConnectionArgs args, ErrorArgs error, object state)
	{
		Debug.Log("OnConnectFailed !!!");
		MessageTransfer.Singleton.IsTryingConn = false;
		ClientNetProperty.Singleton.NotifyChanged((int)ClientNetProperty.ENPropertyChanged.enConnectionFailed, error.ErrorType);

        // 如果没有得到SESSION才弹提示框
        if ( false == MessageTransfer.Singleton.GotSession )
        {
            if (ClientNet.Singleton.IsLongConnecting)
            {//长连接中，直接重连
                ReConnect();
            }
            else
            {
                UICommonMsgBoxCfg boxCfg = new UICommonMsgBoxCfg();
                boxCfg.mainTextPrefab = "UILostConnectionLabel";
                boxCfg.buttonNum = 1;
                UICommonMsgBox.GetInstance().ShowMsgBox(OnConnectFailedButtonClick, null, boxCfg);
            }
        }
	}

    // 连接失败 确认后 重联
    public void OnConnectFailedButtonClick(object sender, EventArgs e)
    {
        ReConnect();
    }
    //重联
    private void ReConnect()
    {
        User.Singleton.UserLogin();
    }

    // 默认消息接收
	public void OnDefaultMessage(PacketReader p, object state)
	{
		int id = p.PacketID;
		Debug.LogWarning("Missed Process of message:" + id);
	}
    // 登录
    public void OnMsgLogin_S2C(PacketReader p, object state)
    {
        //
    }
    // 创建角色
    public void OnMsgPlayerCreate_S2C(PacketReader p, object state)
    {
        //
    }
    // 朋友相关
    public void OnMsgFriendList_S2C(PacketReader p, object state)
    {
        //
    }
    public void OnMsgFriendAdd_S2C(PacketReader p, object state)
    {
        string friendName = p.ReadUTF8(64);
    }
    public void OnMsgFriendAdd_Agree_S2C(PacketReader p, object state)
    {
        string friendName = p.ReadUTF8(64);
    }
    public void OnMsgFriendAdd_Refuse_S2C(PacketReader p, object state)
    {
        string friendName = p.ReadUTF8(64);
        string reason = p.ReadUTF8(256);
    }
    public void OnMsgFriendDel_S2C(PacketReader p, object state)
    {
        string friendName = p.ReadUTF8(64);
    }
    public void OnMsgFriendBeDel_S2C(PacketReader p, object state)
    {
        string friendName = p.ReadUTF8(64);
    }
    public void OnMsgFriendChat_S2C(PacketReader p, object state)
    {
        string friendName = p.ReadUTF8(64);
        string info = p.ReadUTF8(256);
    }
    public void OnMsgFriendLogin_S2C(PacketReader p, object state)
    {
        string friendName = p.ReadUTF8(64);
    }
    public void OnMsgFriendLogout_S2C(PacketReader p, object state)
    {
        string friendName = p.ReadUTF8(64);
    }

	public void OnMsgMessageRespond_S2C(PacketReader p, object state)
	{
		MessageRespond respond;
		respond.MessageID = p.ReadInt16();
		respond.IsSuccess = (p.ReadInt16() == 0? false:true);
		respond.RespondCode = p.ReadInt32();
		MessageRespondProperty.Singleton.OnMessageRespond(respond);

	}

    public void OnMsgOperateChar_C2S(PacketReader p, object state)
    {
        short operateType = p.ReadInt16();
        short charIndex = p.ReadInt16();
        int charCount = p.ReadInt32();
        string name = p.ReadUTF8(16);

        Debug.Log("operateType = " + operateType + ", " + charIndex + "," + charCount + "," + name);

        if (operateType == 1) //1 create 重新创建角色
        {
            //ClientNet.Singleton.NeedCreateChar = true;
//             OperateCharPacket login = new OperateCharPacket(1, 1, 1, "aaaa");
//             ClientNet.Singleton.SendPacket(login);

            //关闭转菊花loading界面
            Loading.Singleton.Hide();

            // 显示登录
            Login.Singleton.ShowEnterName();

        }
    }

    enum ENLoginResult
    {
        enSuccess = 1,
        enFailed = 2,
		enProtocolVerErr = 3,  //协议版本对不上
    }
    public void OnMsgAccountLoginResult_S2F2C(PacketReader p, object state)
    {
        byte reslut = p.ReadByte();
        string session = p.ReadAscii(32);

        uint sobID = p.ReadUInt32();
        uint zoneID = p.ReadUInt32();

        Debug.Log("session = " + session);

		if (reslut == (byte)ENLoginResult.enSuccess)
        {
            MessageTransfer.Singleton.Session = session;
        }
		else if(reslut == (byte)ENLoginResult.enProtocolVerErr)
		{
            UICommonMsgBoxCfg boxCfg = new UICommonMsgBoxCfg();
            boxCfg.isUsePrefab = false;
            boxCfg.mainTextPrefab = "协议版本号与服务端不同，请更新客户端";
            boxCfg.buttonNum = 1;
            UICommonMsgBox.GetInstance().ShowMsgBox(null, null, boxCfg);
			//Debug.LogError("协议版本号与服务端不同，请更新客户端");
		}

    }

	public void OnMsgPlayerData_S2D2C(PacketReader p, object state)
	{
		Debug.Log("OnMsgPlayerData_S2D2C");

		int operatorType = p.ReadInt32();
		int feID = p.ReadInt32();
		uint accountUID = p.ReadUInt32();
		string accountName = p.ReadUTF8(64);
		int charUID = p.ReadInt32();
		uint dataLength = p.ReadUInt32();
		byte version = p.ReadByte();
		int serilizeType = p.ReadInt32();

		PropertySet props = User.Singleton.UserProps;
		CSItemBag bag = User.Singleton.ItemBag;
		PlayerFloorInsts floorInsts = User.Singleton.floorInsts;
		//todo test Data
		//CardBag.Singleton.GetTestCard();
		User.Singleton.Guid = charUID;
		if ((serilizeType & (int)ENSerilizeType.enSerilizeProps) > 0)
		{
			props.Deserialize(p);
		}
		if ((serilizeType & (int)ENSerilizeType.enSerilizeItemBag) > 0)
		{
			bag.Deserialize(p);
		}
		if ((serilizeType & (int)ENSerilizeType.enSerilizeFloorInsts) > 0)
		{
			floorInsts.Deserialize(p);
		}
		User.Singleton.OnServerProps();
		UnityEngine.Debug.Log("count = "+bag.GetItemCount());

		//MiniServer.Singleton.req_battleHelperList();

        // 记录数据
        User.Singleton.RecordData();
	}

	public void OnMsgSyncBagItem_S2C(PacketReader p, object state)
	{
		Debug.Log("OnMsgSyncBagItem_S2C");
		int optType = p.ReadInt32();
        CSItem item = new CSItem();
        item.Read(p);
        item.Init();
        CSItem userItem = User.Singleton.ItemBag.GetItemByGuid(item.m_guid);
        if (userItem == null)
        {
            userItem = item;
            User.Singleton.ItemBag.AddItem(item);

            BattleSummary.Singleton.SummaryGetNewCard(item.m_id);
            // 更新背包提示 获得新卡的数量
            MainButtonList.Singleton.GotNewCard();
            User.Singleton.GetCardChangeIllustrated(item.m_id);
        }
        else
        {
            userItem.ReplaceSegmentData(item);
        }

        // 背包更新
        CardBag.Singleton.OnUpdateCardBag();
        switch (optType)
        {
                // 进化
            case (int)ENPlayerCardOptTypen.enCardEvolve:
                {
                    Loading.Singleton.Hide();
                    CardEvolution.Singleton.OnHeroCardEvolve(item.Guid);
                    break;
                }
                // 强化
            case (int)ENPlayerCardOptTypen.enCardMerge:
                {
                    Loading.Singleton.Hide();
                    Debug.Log("强化成功后的 卡牌  item.Exp:" + userItem.Exp + ",item.Level:" + userItem.Level + ",userItem.IDInTable:" + userItem.IDInTable + ",userItem:" + userItem.Guid.m_lowPart+","+userItem.Guid.m_highPart);
                    CardUpdateProp.Singleton.OnS2CLevelUpSuccess(item.Guid);
                    break;
                }     
        }


	}
	public void OnMsgDelBagItem_S2C(PacketReader p, object state)
	{
		Debug.Log("OnMsgDelBagItem_S2C");
        int optType = p.ReadInt32();
		CSItemGuid guid;
		guid.m_lowPart = p.ReadInt32();
		guid.m_highPart = p.ReadInt32();

		int idx = User.Singleton.ItemBag.GetItemSlotByGuid(guid);
		User.Singleton.ItemBag.RemoveItem(idx);
		Debug.Log("remove, guid = " + guid.m_lowPart + " " + guid.m_highPart + " index = " + idx);

        // 背包更新
        CardBag.Singleton.OnUpdateCardBag();

        switch (optType)
        {         
            // 出售
            case (int)ENPlayerCardOptTypen.enSellCard:
                {
                    Debug.Log("ENPlayerCardOptTypen.enSellCard");
                    OperateCardList.Singleton.UpdateCardList();

                    // 隐藏LOADING
                    Loading.Singleton.Hide();

                    break;
                }
        }


	}
	public void OnMsgSyncItemBag_S2C(PacketReader p, object state)
	{
// 		Debug.Log("OnMsgSyncItemBag_S2C");
// 		int operatorType = p.ReadInt32();
// 		int bagType = p.ReadInt32();
// 		CSItem item = new CSItem();
// 		item.Read(p);
// 		CSItem userItem = User.Singleton.ItemBag.GetItemByGuid(item.m_guid);
// 		if (userItem == null)
// 		{
// 			User.Singleton.ItemBag.AddItem(item);
// 		}
// 		else
// 		{
// 			userItem.ReplaceSegmentData(item);
// 		}
	}

	public void OnMsgServerPlayerGetReady_S2Ce2Fe2C(PacketReader p, object state)
	{
		// MiniServer.Singleton.user_ask_playerData();
      
       int sobId        = p.ReadInt32();
       int newPlayer    = p.ReadInt32();
       Debug.Log("OnMsgServerPlayerGetReady_S2Ce2Fe2C:" + newPlayer);

       
       Login.Singleton.OnAskInitCard(newPlayer) ;
     
	}

    public void OnMsgSessionError_S2C(PacketReader p, object state)
    {
        Debug.Log("OnMsgSessionError_S2C");

        User.Singleton.UserLogin();
    }

	public void OnMsgSyncServerTime_S2C(PacketReader p, object state)
	{
        // 获取角色信息的 反馈函数
        Loading.Singleton.Hide();

		Debug.Log("OnMsgSyncServerTime_S2C");
		uint serverTime = p.ReadUInt32();
		TimeUtil.SetServerTimeStamp(serverTime);
	}

	public void OnMsgSetRepresentativeCard_S2C(PacketReader p, object state)
	{
		Debug.Log("OnMsgSetRepresentativeCard_S2C");
		CSItemGuid guid;
		guid.m_lowPart = p.ReadInt32();
		guid.m_highPart = p.ReadInt32();

		User.Singleton.RepresentativeCard = guid;
	}

	public void OnMsgSyncBattleHalper_CH2C(PacketReader p, object state)
	{
		Debug.Log("OnMsgSyncBattleHalper_CH2C");
		int charID = p.ReadInt32();
		string charName = p.ReadUTF8(16);
		int charLevel = p.ReadInt32();
		int selectTimes = p.ReadInt32();
		int helperType = p.ReadInt32();
		int lastLoginTime = p.ReadInt32();
		CSItem item = new CSItem();
		item.Read(p);

		if (helperType == 0)
		{
			User.Singleton.HelperList.RemoveHelperCard(charID);
		}
		else
		{
			Helper helper = new Helper();
			helper.m_userGuid = charID;
			helper.m_userName = charName;
			helper.m_userLevel = charLevel;
			helper.m_cardGuid = item.m_guid;
			helper.m_cardId = item.m_id;
			helper.m_cardLevel = item.Level;
			helper.m_cardBreakCounts = item.BreakCounts;
			helper.m_type = helperType;
			helper.m_chosenNum = selectTimes;
			User.Singleton.HelperList.AddHelperCard(helper);
		}
	}
    public void OnMsgSendMagicStoneInfo_S2C(PacketReader p, object state) 
    {
        Debug.Log("OnMsgSendMagicStoneInfo_S2C");
        MagicStoneTableInfo tableInfo = null;
        int magicStoneListSize = p.ReadInt32();
        for (int i = 1; i <= magicStoneListSize; i++)
        {
            int magicStoneId = p.ReadInt32();
            bool isDiscount =  (p.ReadInt32() == 0 ) ? false : true;
            MagicStoneInfo stoneInfo = new MagicStoneInfo();
            tableInfo = GameTable.MagicStoneTableAsset.Lookup(magicStoneId);
            if (null != tableInfo)
            {
                stoneInfo.m_magicStoneId = magicStoneId;
                stoneInfo.m_magicStoneNum = tableInfo.magciStoneNumber;
                stoneInfo.m_magicStonePrice =  (isDiscount) ? tableInfo.discountPrice : tableInfo.price;
                ShopProp.Singleton.m_magicStoneList.Add(stoneInfo);
            }
        }
        ShopProp.Singleton.OnUpdateMagicStoneShop();
    }

    public void OnMsgExpandCardBagCapacity_S2C(PacketReader p, object state) 
    {
        Debug.Log("OnMsgExpandCardBagCapacity_S2C");
        int expandNum = p.ReadInt32();
        CardBag.Singleton.AddCadBag(expandNum);
    }


    public void OnMsgRecoverSucc_S2C(PacketReader p, object state) 
    {
        Debug.Log("OnMsgRecoverSucc_S2C");
        int type = p.ReadInt32();
        int num = p.ReadInt32();
        
        switch (type)
        {
            case (int)UIShop.succType.enRecoverStamina:
                OnMsgRecoverStaminaSucc_S2C(num);
                break;
            case (int)UIShop.succType.enRecoverEnergy:
                OnMsgRecoverEnergySucc_S2C(num);
                break;
            case (int)UIShop.succType.enBuyRingOfHonorCard:
                OnMsgBuyRingOfHonorCardSucc_S2C(num);
                break;
            case (int)UIShop.succType.enFriendShipOnceRecruitment:
                OnMsgFriendShipOnceRecruitmentSucc_S2C(num);
                break;
            case (int)UIShop.succType.enMagicStoneRecruitment:
                OnMsgMagicStoneRecruitmentSucc_S2C(num);
                break;
            case (int)UIShop.succType.enExpandFriends:
                OnMsgExpandFriendsSucc_S2C(num);
                break;
            default:
                break;
        }
    }
    void OnMsgExpandFriendsSucc_S2C(int friendCount) 
    {
        ShopProp.Singleton.OnShowExpandFriendsSucc(friendCount);
    }
    void OnMsgRecoverStaminaSucc_S2C(int staminaNum) 
    {
        ShopProp.Singleton.OnShowRecoverStaminaSucc(staminaNum);
    }
    void OnMsgRecoverEnergySucc_S2C(int energyNum)
    {
        ShopProp.Singleton.OnShowRecoverEnergySucc(energyNum);
    }

    void OnMsgMagicStoneRecruitmentSucc_S2C(int slot) 
    {
        Debug.Log("OnMsgMagicStoneRecruitmentSucc_S2C");

        //关闭转菊花loading界面
        Loading.Singleton.Hide();
        //清空格子列表
        GachaPanelProp.Singleton.ResetSlotList();
        GachaPanelProp.Singleton.AddCardSlot(slot);
        MainUIManager.Singleton.HideAllNeedHideWnd();
        UIGachaPanel.GetInstance().ShowWindow();
        UIGachaPanel.GetInstance().ResetPanelPos();
    }
    void OnMsgFriendShipOnceRecruitmentSucc_S2C(int slot) 
    {
        Debug.Log("OnMsgFriendShipOnceRecruitmentSucc_S2C");
        //清空格子列表
        GachaPanelProp.Singleton.ResetSlotList();
        GachaPanelProp.Singleton.AddCardSlot(slot);
        MainUIManager.Singleton.HideAllNeedHideWnd();
        UIGachaPanel.GetInstance().ShowWindow();
        UIGachaPanel.GetInstance().ResetPanelPos();
    }

    public void OnMsgFriendShipMoreRecruitmentSucc_S2C(PacketReader p, object state) 
    {
        Debug.Log("OnMsgFriendShipMoreRecruitmentSucc_S2C");
        int cardNum = p.ReadInt32();
        //清空格子列表
        GachaPanelProp.Singleton.ResetSlotList();
        for (int i = 1; i <= cardNum; i++ )
        {
            int slot = p.ReadInt32();
            GachaPanelProp.Singleton.AddCardSlot(slot);
        }
        MainUIManager.Singleton.HideAllNeedHideWnd();
        UIGachaPanel.GetInstance().ShowWindow();
        UIGachaPanel.GetInstance().ResetPanelPos();
        //获得卡牌特效
        //UIGetCard.GetInstance().SetName(cardName);
        //UIGetCard.GetInstance().ShowWindow();
        //ShopProp.Singleton.OnUpdateRecruitmentPanel();
    }
    public void OnMsgSendUpdateCardSucc_S2C(PacketReader p, object state) 
    {
        Debug.Log("OnMsgSendUpdateCardSucc_S2C");
        int succType = p.ReadInt32();
        int slot = p.ReadInt32();
    }

    public void OnMsgSendRingOfHonorInfo_S2C(PacketReader p, object state) 
    {
        Debug.Log("OnMsgSendRingOfHonorInfo_S2C");
        ShopProp.Singleton.m_ringOfHonorList.Clear();
        int ringOfHonorListSize = p.ReadInt32();
        for (int i = 1; i <= ringOfHonorListSize; i++)
        {
            int infoId = p.ReadInt32();
            int cardId = p.ReadInt32();
            int price = p.ReadInt32();
            RingOfHonorInfo ringInfo = new RingOfHonorInfo();
            ringInfo.m_infoId = infoId;
            ringInfo.m_cardId = cardId;
            ringInfo.m_price = price;
            ShopProp.Singleton.m_ringOfHonorList.Add(ringInfo);
            User.Singleton.SeeCardChangeIllustrated(cardId);
        }
        ShopProp.Singleton.m_cardPre = (1.0f / (ringOfHonorListSize - 1.0f));
        ShopProp.Singleton.OnUpdateRingOfHonorShop();
    }

    void OnMsgBuyRingOfHonorCardSucc_S2C(int cardId) 
    {
        Debug.Log("OnMsgBuyRingOfHonorCardSucc_S2C");
        string cardName = GameTable.HeroInfoTableAsset.Lookup(cardId).StrName;
        //获得卡牌特效
        UIGetCard.GetInstance().SetName(cardName);
        UIGetCard.GetInstance().ShowWindow();
        ShopProp.Singleton.OnUpdateRingOfHonorShop();
    }

    public void OnMsgSendChangeNameSucc_S2C(PacketReader p, object state) 
    {
        Debug.Log("OnMsgSendChangeNameSucc_S2C");
        string name = p.ReadUTF8(16);

        User.Singleton.SetUserName(name);
        GameSetingProp.Singleton.OnChangeName();
    }

	public void OnMsgEnterDugeon_S2C(PacketReader p, object state)
	{
		Debug.Log("OnMsgEnterDugeon_S2C");
		int dungeonID = p.ReadInt32();
		int dataSize = p.ReadInt32();
		string dungonData = p.ReadUTF8(dataSize);
		//Debug.Log(dungonData);
        ClientNet.Singleton.m_connectionType = ClientNet.ENConnectionType.enShortConnect;
        BattleArena.Singleton.ReceiveBattleArenaInfo(dungonData, dungeonID);
	}

    //进入长连接副本的回复
    public void OnMsgEnterLCDugeon_S2C(PacketReader p, object state)
    {
        string ip = p.ReadUTF8(16);
        int port = p.ReadInt32();
        Debug.LogWarning("LC_Server_IP:" + ip + ",port:"+port);
        string tokenID = p.ReadUTF8(32);
        Debug.LogWarning("tokenID:" + tokenID);

        ClientNet.Singleton.m_connectionType = ClientNet.ENConnectionType.enLongConnect;
        LCMsgSender.Singleton.Init(ip, port);

        IMiniServer.Singleton.SendLogin_C2BS(tokenID);
    }

	public void OnMsgDungeonEnd_S2C(PacketReader p, object state)
	{
        Debug.Log("OnMsgDungeonEnd_S2C");

		int dungeonID   = p.ReadInt32();
        int score       = p.ReadInt32();
		int exp         = p.ReadInt32();
		int money       = p.ReadInt32();
		int awardID     = p.ReadInt32();
		int dropListCount = p.ReadInt32();
		//List<int> dropList = new List<int>();
		for (int index = 0; index < dropListCount; index ++ )
		{
			BattleSummary.Singleton.m_battleRewardItemList.Add(p.ReadInt32());
		}

		BattleSummary.Singleton.m_desExp        = exp;
        BattleSummary.Singleton.m_desMoney      = money;
        BattleSummary.Singleton.m_curRank       = BattleSummary.Singleton.GetRank(score, dungeonID);

        Debug.Log("战斗结束后 从服务器发回的消息 dungeonID:" + dungeonID + ",exp:" + exp + ",money:" + money + ",score:" + score + ", BattleSummary.Singleton.m_curRank:" + BattleSummary.Singleton.m_curRank);
		//SceneManger.Singleton.NotifyChanged((int)SceneManger.ENPropertyChanged.enDungeonEnd, null);
      
        //score == 0  为失败不为0 为胜利
        BattleSummary.Singleton.BattleSummaryDungeonEnd(score != 0);
	}

// 	public void OnMsgOneTeamData_S2C(PacketReader p, object state)
// 	{
// 		Debug.Log("OnMsgOneTeamData_S2C");
// 		int teamIndex   = p.ReadInt32();
// 		TeamItem item   = new TeamItem();
// 
//         item.m_index    = teamIndex;
// 
// 		for (int index = 0; index < 3; index ++ )
// 		{
// 			item.m_memberList[index].m_lowPart = p.ReadInt32();
// 			item.m_memberList[index].m_highPart = p.ReadInt32();
// 		}
// 		item.m_memberList[3].m_lowPart = 0;
// 		item.m_memberList[3].m_highPart = 0;
// 
// 		PropertyValueAccoutTeamView teamValue = User.Singleton.UserProps.GetProperty_Custom(UserProperty.team_data) as PropertyValueAccoutTeamView;
// 		if (teamIndex < 0 || teamIndex >= teamValue.m_teamList.Length)
// 		{
// 			return;
// 		}
// 		teamValue.m_teamList[teamIndex] = item;
// 
//         User.Singleton.DeserializeTeamData();
// 
// 	}

// 	public void OnMsgSyncLevel_S2C(PacketReader p, object state)
// 	{
// 		Debug.Log("OnMsgSyncLevel_S2C");
// 		int level = p.ReadInt32();
// 		int exp = p.ReadInt32();
// 		User.Singleton.SetLevelExp(level, exp);
// 	}
// 	public void OnMsgSyncMoney_S2C(PacketReader p, object state)
// 	{	
// 		Debug.Log("OnMsgSyncMoney_S2C");
// 		int changeVal = p.ReadInt32();
// 		int nowVal = p.ReadInt32();
// 		User.Singleton.SetMoney(nowVal);
// 	}
// 	public void OnMsgSyncBindMoney_S2C(PacketReader p, object state)
// 	{
// 		Debug.Log("OnMsgSyncBindMoney_S2C");
// 		int changeVal = p.ReadInt32();
// 		int nowVal = p.ReadInt32();
// 		User.Singleton.SetBindMoney(nowVal);
// 	}
// 	public void OnMsgSyncRing_S2C(PacketReader p, object state)
// 	{
// 		Debug.Log("OnMsgSyncRing_S2C");
// 		int changeVal = p.ReadInt32();
// 		int nowVal = p.ReadInt32();
// 		User.Singleton.SetRing(nowVal);
// 	}
// 	public void OnMsgSyncFriendshipPoint_S2C(PacketReader p, object state)
// 	{
// 		Debug.Log("OnMsgSyncFriendshipPoint_S2C");
// 		int changeVal = p.ReadInt32();
// 		int nowVal = p.ReadInt32();
// 		User.Singleton.SetFriendshpiPoint(nowVal);
// 	}
// 	public void OnMsgSyncStamina_S2C(PacketReader p, object state)
// 	{
// 		Debug.Log("OnMsgSyncStamina_S2C");
// 		int stamina = p.ReadInt32();
// 		int staminaSaveTime = p.ReadInt32();
// 		User.Singleton.SetStamina(stamina, staminaSaveTime);
// 	}
// 
// 	public void OnMsgSyncEnergy_S2C(PacketReader p, object state)
// 	{
// 		Debug.Log("OnMsgSyncEnergy_S2C");
// 		int energy = p.ReadInt32();
// 		int energySaveTime = p.ReadInt32();
// 		User.Singleton.SetEnergy(energy, energySaveTime);
// 	}

	public void OnMsgSocialityItem_S2C(PacketReader p, object state)
	{
        FriendItem friendItem = new FriendItem();
		friendItem.Read(p);

        int relation = friendItem.m_relation;
        if (relation > 0)
        {
            FriendList.Singleton.AddFriendFunc(friendItem);
        }
        else if (relation == 0)
        {
            FoundFriendByID.Singleton.OnShowSuccessMsgBox(friendItem);
            Debug.Log("OnMsgSocialityItem_S2C CharID = ");// + charID);
        }

        
        //friendItem.m_id;
//         int id = friendItem.GetID();
// 
//         FriendList.Singleton.AddFriendFunc(friendItem);


	}
	public void OnMsgSocialityItemList_S2C(PacketReader p, object state)
	{
		int relation = p.ReadInt32();
		int itemCount = p.ReadInt32();
		Debug.Log("OnMsgSocialityItem_S2C ,count = " + itemCount);
		FriendList.Singleton.RemoveFriendInfoByRelation(relation);
		for (int index = 0; index < itemCount; index++ )
		{
			FriendItem item = new FriendItem();
			item.Read(p);
            FriendList.Singleton.InitFriendInfoDic(item);
		}
        FriendList.Singleton.InitFriendInfoList();
	}

// 	public void OnMsgLookupPlayerResult_CH2C(PacketReader p, object state)
// 	{
// 		Debug.Log("OnMsgLookupPlayerResult_CH2C");
// 		int charID = p.ReadInt32();
// 		int result = p.ReadInt32();
// 	}

	public void OnMsgRemoveSocialityItem_CH2C(PacketReader p, object state)
	{
		Debug.Log("OnMsgRemoveSocialityItem_CH2C");
		int charID = p.ReadInt32();
        FriendList.Singleton.DeleteFriend(charID);
	}

    public void OnMsgInitCard_S2C(PacketReader p, object state)
    {
        Debug.Log("OnMsgInitCard_S2C");

        MiniServer.Singleton.user_ask_playerData();
    }

	public void OnMsgSyncDirtyPropertys_S2C(PacketReader p, object state)
	{
		Debug.Log("OnMsgSyncDirtyPropertys_S2C");

		uint dataLength = p.ReadUInt32();
		User.Singleton.UserProps.Deserialize(p);
		User.Singleton.OnServerProps();
	}

	public void OnENMsgSyncFloorInst_S2C(PacketReader p, object state)
	{
		Debug.Log("OnENMsgSyncFloorInst_S2C");
		int floorID = p.ReadInt32();
		FloorInstData data = new FloorInstData();
		data.Read(p);
		User.Singleton.floorInsts.ReplaceFloorInstData(floorID, data);
	}

	public void OnMsgFloorReliveReslut_S2C(PacketReader p, object state)
	{
		Debug.Log("OnENMsgSyncFloorInst_S2C");
		int floorID = p.ReadInt32();
		int reslut = p.ReadInt32();
		ENReliveReslut enReslut = (ENReliveReslut)reslut;
        UIRelive.GetInstance().OnBuyReliveMsgCallBack(floorID, enReslut);
	}
	public void OnMsgServerLog_S2C(PacketReader p, object state)
	{
		int dataLength = p.ReadInt32();
		string str = p.ReadUTF8(dataLength);
		Debug.Log(str);
		Loading.Singleton.NotifyChanged((int)Loading.ENPropertyChanged.enServerLog,(object)str);
    }
}


