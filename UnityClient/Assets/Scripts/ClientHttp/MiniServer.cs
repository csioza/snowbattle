using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniServer : IMiniServer
{
	public override int gateserver_globalconfig()
	{
        MessageBlock msg = MessageBlock.CreateMessage(HttpReqMsgID.POST_GATESERVER_GLOBALCONFIG);
		msg.AddString("test", "test");
		MessageTransfer.Singleton.SendMsg( msg);
        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
	public override int account_serverconfig()
	{
        MessageBlock msg = MessageBlock.CreateMessage(HttpReqMsgID.POST_ACCOUNT_SERVERCONFIG);
		msg.AddString("test", "test");
        MessageTransfer.Singleton.SendMsg( msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
	public override int account_get_zone()
	{
		MessageBlock msg = MessageBlock.CreateMessage(HttpReqMsgID.POST_ACCOUNT_GET_ZONE);
		msg.AddString("test", "test");
        MessageTransfer.Singleton.SendMsg( msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
	public override int account_register(string accname, string accpwd)
	{
		MessageBlock msg = MessageBlock.CreateMessage(HttpReqMsgID.POST_ACCOUNT_REGISTER);
		msg.AddString("accname", accname);
		msg.AddString("accpwd", accpwd);
        MessageTransfer.Singleton.SendMsg( msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}

	public override int account_check_local(string accname, string accpwd)
	{
		MessageBlock msg = MessageBlock.CreateMessage(HttpReqMsgID.POST_ACCOUNT_CHECK);
		msg.AddString("accname", accname);
		msg.AddString("accpwd", accpwd);
        MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}

	public override int account_login(string accname, string token)
	{
		MessageBlock msg = MessageBlock.CreateMessage(HttpReqMsgID.POST_ACCOUNT_LOGIN);
		msg.AddString("login_type", "local");
		msg.AddString("accname", accname);
		msg.AddString("token", token);
        MessageTransfer.Singleton.SendMsg( msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
	public override int account_change_pwd(string accname, string oldpwd, string newpwd)
	{
		MessageBlock msg = MessageBlock.CreateMessage(HttpReqMsgID.POST_ACCOUNT_CHANGEPWD);
		msg.AddString("accname", accname);
		msg.AddString("oldpwd", oldpwd);
		msg.AddString("newpwd", newpwd);
        MessageTransfer.Singleton.SendMsg( msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
    //TODO
	public override int user_create(string name, int portrait,int herocard_id)
	{
		MessageBlock msg = MessageBlock.CreateMessage(HttpReqMsgID.POST_USER_CREATE);
		msg.AddString("name", name);
		msg.AddParam("portrait", portrait);
		msg.AddParam("herocard_id", herocard_id);
        MessageTransfer.Singleton.SendMsg( msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
    //TODO
	public override int herocard_carete(int card_id)
	{
		MessageBlock msg = MessageBlock.CreateMessage(HttpReqMsgID.POST_HEROCARD_GET);
		msg.AddParam("card_id", card_id);
        MessageTransfer.Singleton.SendMsg( msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
    //TODO
	public override int herocard_merge(int maincard, string material_cards)
	{
		MessageBlock msg = MessageBlock.CreateMessage(HttpReqMsgID.POST_HEROCARD_MERGE);
		msg.AddParam("main_card", maincard);
        msg.AddString("material_cards", material_cards);

        MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
    //TODO
	public override int herocard_evolve(int maincard, string material_cards)
	{
		MessageBlock msg = MessageBlock.CreateMessage(HttpReqMsgID.POST_HEROCARD_EVOLVE);
		msg.AddParam("main_card", maincard);
        msg.AddString("material_cards", material_cards);

        MessageTransfer.Singleton.SendMsg( msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}


    //
    public override int user_login(string accname, string accpwd)
    {
        Debug.Log("user_login accname:" + accname + ",accpwd:" + accpwd);
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgAccountLogin_C2S);
        msg.AddString("accname", accname, 64);
        msg.AddString("token", accpwd, 32);
		msg.AddParam("protocolVersion", ServerSetting.Singleton.ProtocolVersion);
        MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }

    public override int user_LogOut()
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgKickSelf_C2S);
        MessageTransfer.Singleton.SendMsg(msg);
        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }

    public override int user_create_char(string name)
    {
        Debug.Log("user_create_char:" + name);
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgAccountOperateChar_C2S);
        msg.AddString("name", name, 16);
        MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
        
	public override int user_ask_playerData()
	{
        Debug.Log("user_ask_playerData");
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgAskPlayerData_C2S);

		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
	public override int player_get_card(int cardID,int level)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgPlayerGetCard_C2S);
		msg.AddParam("cardID", cardID);
		msg.AddParam("cardLevel", level);
		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
	public override int user_ask_bagData()
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgAskItemBag_C2S);
		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}


	public override int herocard_addExp(CSItemGuid cardGUID, int exp)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgHerocardAddexp_C2S);
		msg.AddParam("heroCardGuid_lowPart",cardGUID.m_lowPart);
		msg.AddParam("heroCardGuid_highPart", cardGUID.m_highPart);
		msg.AddParam("exp",exp);

		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}

	public override int req_herocardMerge(CSItemGuid cardGUID, List<CSItemGuid> foodCards)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgHerocardMerge_C2S);
		msg.AddParam("heroCardGuid_lowPart",cardGUID.m_lowPart);
		msg.AddParam("heroCardGuid_highPart", cardGUID.m_highPart);

		
		for (int index = 0; index < 10; index++)
		{
			if (index >= foodCards.Count)
			{
				msg.AddParam("lowPart", 0);
				msg.AddParam("highPart", 0);
			}
			else
			{
				msg.AddParam("lowPart", foodCards[index].m_lowPart);
				msg.AddParam("highPart", foodCards[index].m_highPart);
			}
		}

		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}

	public override int req_herocardEvolve(CSItemGuid cardGUID)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgHerocardEvolve_C2S);
		msg.AddParam("heroCardGuid_lowPart", cardGUID.m_lowPart);
		msg.AddParam("heroCardGuid_highPart", cardGUID.m_highPart);

		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}

	public override int req_setRepresentativeCard(CSItemGuid cardGUID)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgSetRepresentativeCard_C2S);
		msg.AddParam("heroCardGuid_lowPart", cardGUID.m_lowPart);
		msg.AddParam("heroCardGuid_highPart", cardGUID.m_highPart);
		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
	public override int req_battleHelperList()
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgReqBattleHelperList_C2CH);
		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}

	//扩充背包
	public override int req_expandBagCapacity(int cost)
	{
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgExpandCardBagCapacity_C2S);
		msg.AddParam("cost", cost);
		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
    //发送确认购买魔法石
    public override int SendBuyMagicStone_C2S(int magicStoneId) 
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgSendBuyMagicStone_C2S);
        msg.AddParam("magicStoneId", magicStoneId);
        MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    //发送请求商店信息
    public override int SendShopItemInfo_C2S(int shopType) 
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgSendShopItemInfo_C2S);
        msg.AddParam("shopType", shopType);
        MessageTransfer.Singleton.SendMsg(msg);
        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }

    //发送购买荣誉戒指卡片
    public override int SendBuyRingOfHonorCard_C2S(int ringInfo,int cardId) 
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgSendBuyRingOfHonorCard_C2S);
        msg.AddParam("ringInfo", ringInfo);
        msg.AddParam("cardId", cardId);
        MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    //发送更改名称消息
    public override int SendChangeName_C2S(string changeName) 
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgSendChangeName_C2S);
        msg.AddString("changeName", changeName,16);
        MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }

    public override int SendEnterDungeon(int dungeonID, int myStamina, int camp, string key,bool isRolling)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgEnterDugeon_C2S);
		msg.AddParam("dungeonID", dungeonID);
		msg.AddParam("stamina", myStamina);
        msg.AddParam("camp", camp);
        msg.AddString("key", key, 32);
        if (isRolling)
        {
            msg.AddParam("isRolling", 1);
        }
        else 
        {
            msg.AddParam("isRolling", 0);
        }
        
		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
	public override int SendDungeonSettlement(int dungeonID, int time, int combo, int killBoss, int reSpawn, int score, int exp, int money, int awardID ,List<int> dropList = null)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgDungeonSettlement_C2S);
		msg.AddParam("dungeonID", dungeonID);
		msg.AddParam("time", time);
		msg.AddParam("combo", combo);
		msg.AddParam("killBoss", killBoss);
		msg.AddParam("reSpawn", reSpawn);
		msg.AddParam("score", score);
		msg.AddParam("exp", exp);
		msg.AddParam("money", money);
		msg.AddParam("awardID", awardID);
        int nSize = 0;
        nSize = (dropList == null) ? 0 : (dropList.Count>255 ? 255 : dropList.Count);
        msg.AddParam("dropListSize", nSize);
        msg.AddArray("dropList", dropList, nSize);
        Debug.Log("nSize:" + nSize);
        MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }

	public override int SendExitDungeon(int dungeonID)
	{
        MessageBlock msg = null;
        if (ClientNet.Singleton.IsLongConnecting)
        {
            msg = MessageBlock.CreateMessage(GamePacketID.ENMsgLeaveDugeon_C2BS, true);
            LCMsgSender.Singleton.SendMsg(msg);
        }
        else
        {
            msg = MessageBlock.CreateMessage(GamePacketID.ENMsgExitDungeon_C2S);
            msg.AddParam("dungeonID", dungeonID);
            MessageTransfer.Singleton.SendMsg(msg);
        }
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
    //发送到服务器卡牌升段
    public override int SendCardDivisionUpdate(CSItem card, int formulaId, List<CSItem> cardList) 
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgCardDivisionUpdate_C2S);
        msg.AddParam("cardId",(int) card.IDInTable);
        msg.AddParam("cardlowPart", card.m_guid.m_lowPart);
        msg.AddParam("cardhighPart", card.m_guid.m_highPart);
        msg.AddParam("formulaId", formulaId);
        msg.AddParam("cardNum", cardList.Count);
        foreach (CSItem item in cardList)
        {
            msg.AddParam("lowPart", item.m_guid.m_lowPart);
            msg.AddParam("highPart", item.m_guid.m_highPart);
        }

        MessageTransfer.Singleton.SendMsg(msg);
        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }

    //发送到服务器招募信息
    public override int SendRecruitment_C2S(int RecruitmentType ) 
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgRecruitment_C2S);
        msg.AddParam("recruitmentType", RecruitmentType);
        msg.AddParam("cardId", 0);
        MessageTransfer.Singleton.SendMsg(msg);
        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    //发送到服务器直接获得卡牌

    public override int SendGetCardById_C2S(int RecruitmentType,int cardId) 
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgRecruitment_C2S);
        msg.AddParam("recruitmentType", RecruitmentType);
        msg.AddParam("cardId", cardId);
        MessageTransfer.Singleton.SendMsg(msg);
        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }

    //扩充卡牌背包
    public override int SendExpandCardBag_C2S(int expandType) 
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgExpandCardBagCapacity_C2S);
        msg.AddParam("expandType", expandType);
        MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }

	public override int SendSelectTeamIndex(int teamIndex)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgSelectTeamIndex);
		msg.AddParam("taemIndex", teamIndex);
		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
    
    //todo 列表发送php待改
	public override int SendSaveOneTeamData(int teamIndex)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgSaveOneTeamData_C2S);
		msg.AddParam("taemIndex", teamIndex);

        TeamItem item = Team.Singleton.GetTeamItemByIndex(teamIndex);

        // 如果为空则代表 删除队伍中的 所有队员
        if ( null == item )
        {
            for (int index = 0; index < 4; index++)
            {
                msg.AddParam("lowPart", 0);
                msg.AddParam("highPart", 0);
            }
        }
        else
        {
            for (int index = 0; index < item.m_memberList.Length; index++)
            {
                msg.AddParam("lowPart", item.m_memberList[index].m_lowPart);
                msg.AddParam("highPart", item.m_memberList[index].m_highPart);
            }
        }
        
		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}


	public override int SendSellHeroCard_C2S(List<CSItemGuid> cardList)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgSellHeroCard_C2S);

		for (int index = 0; index < 10; index++)
		{
			if (index >= cardList.Count)
			{
				msg.AddParam("lowPart", 0);
				msg.AddParam("highPart", 0);
			}
			else
			{
				msg.AddParam("lowPart", cardList[index].m_lowPart);
				msg.AddParam("highPart", cardList[index].m_highPart);
			}
		}

		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}

    //查找
	public override int SendLookupPlayer_C2CE(int charID)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgLookupPlayer_C2CE);
		msg.AddParam("charID",charID);
		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}

    //获取社交列表
	public override int SendGetSocialityList_C2CH(int type) 
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgGetSocialityList_C2CH);
        msg.AddParam("type", type);
		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}

    //加好友
	public override int SendAddFriend_C2CH(int charID)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgAddFriend_C2CH);
		msg.AddParam("charID", charID);
		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
    //删好友
	public override int SendDelFriend_C2CH(int charID)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgDelFriend_C2CH);
		msg.AddParam("charID", charID);
		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}


    public override int InitCard_C2S()
    {
        if (Login.Singleton.m_curCardId <= 0)
        {
            return 0;
        }
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgAskInitCard_C2S);
        msg.AddParam("initCardId", Login.Singleton.m_curCardId);
        MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }

    //通过好友请求
	public override int SendPassFriendRequest(int targetID)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgPassFriendRequest_C2CH);
		msg.AddParam("targetID", targetID);
		MessageTransfer.Singleton.SendMsg(msg);

		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}

	public override int SendFloorReliveOption(int floorID, int option)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgFloorReliveOption_C2S);
		msg.AddParam("floorID", floorID);
		msg.AddParam("option", option);
		MessageTransfer.Singleton.SendMsg(msg);

		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}

	public override int SendSelectBattleHelper(int charID)
	{
		//charID，选择的战友的角色charID
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgSelectBattleHelper_C2CH);
		msg.AddParam("charID", charID);
		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}
    public override int SendGMCmd(string cmd)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgGMCmd_C2S);
        msg.AddString("cmd", cmd);
        MessageTransfer.Singleton.SendMsg(msg);
        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }

	public override int MsgSendMail_C2S(MailItem mail)
	{
		MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgSendMail_C2S);
		msg.AddParam("mail", mail);
		MessageTransfer.Singleton.SendMsg(msg);
		int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
	}

    //-------------------长连接消息----------------
    public override int SendLogin_C2BS(string tokenID)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgLogin_C2BS, true);
        msg.AddString("tokenID", tokenID, tokenID.Length);
        LCMsgSender.Singleton.SendMsg(msg);
        Debug.LogWarning("tokenID:" + tokenID);
        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    public override int SendTest_C2BS()
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgTest_C2BS, true);
        LCMsgSender.Singleton.SendMsg(msg);
        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }

    // 创建reuslt的消息
    public override int SendCreateResult_C2BS(int resultId, int source, int target, int skillResultID = 0, int skillID = 0, float[] param = null)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgCreateResultC2BS, true);
        msg.AddParam("resultId", resultId);
        msg.AddParam("source", source);
        msg.AddParam("target ", target);
        msg.AddParam("skillResultID", skillResultID);
        msg.AddParam("skillID", skillID);

        Actor targetActor   = ActorManager.Singleton.Lookup(target);
        Actor sourceActor   = ActorManager.Singleton.Lookup(source);

        float d             = 0.0f;
        if (null != targetActor && null != sourceActor)
        {
            d = ActorTargetManager.GetTargetDistance(sourceActor.RealPos, targetActor.RealPos);
        }

        msg.AddParam("distanace", d);

        if (param != null)
        {
            msg.AddParam("paramLength", param.Length);
            foreach (float item in param)
            {
                msg.AddParam("param", item);
            }
        }
        else
        { 
            msg.AddParam("paramLength",0);

        }

       
        LCMsgSender.Singleton.SendMsg(msg);
        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    float m_lastTickTime = 0;
    public override int Tick()
    {
        if (ClientNet.Singleton.IsLongConnecting)
        {
            if (Time.time - m_lastTickTime > GameSettings.Singleton.m_longConnectTickDuration)
            {
                m_lastTickTime = Time.time;
                MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgTick_C2BS, true);
                LCMsgSender.Singleton.SendMsg(msg);
                int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
            }
        }
        return 0;
    }
    public override int SendSyncPosition_C2BS(int id, float x, float z)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgSyncPosition_C2BS, true);
        msg.AddParam("sobID", id);
        msg.AddParam("x", x);
        msg.AddParam("z", z);
        LCMsgSender.Singleton.SendMsg(msg);
        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    //move action
    public override int SendAction_Move_C2BS(int sobID,float startx,float startz,float x, float z)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgAction_Move_C2BS, true);
        msg.AddParam("sobID", sobID);
        msg.AddParam("x", x);
        msg.AddParam("z", z);
        msg.AddParam("startx", startx);
        msg.AddParam("startz", startz);

        LCMsgSender.Singleton.SendMsg(msg);

        //Debug.Log("SendMoveBegin_C2BS");
        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    //attack action
    public override int SendAction_Attack_C2BS(int sobID, int skillID, int targetID, float x, float z)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgAction_Attack_C2BS, true);
        msg.AddParam("sobID", sobID);
        msg.AddParam("skillID", skillID);
        msg.AddParam("targetID", targetID);
        msg.AddParam("x", x);
        msg.AddParam("z", z);

        LCMsgSender.Singleton.SendMsg(msg);

        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    //attacking move action
    public override int SendAction_AttackingMove_C2BS(int sobID, int skillID, int targetID, float x, float z)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgAction_AttackingMove_C2BS, true);
        msg.AddParam("sobID", sobID);
        msg.AddParam("skillID", skillID);
        msg.AddParam("targetID", targetID);
        msg.AddParam("x", x);
        msg.AddParam("z", z);

        LCMsgSender.Singleton.SendMsg(msg);

        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    //behit action
	public override int SendAction_BeHit_C2BS(int sobID, int srcActorID, bool isBack, bool isFly)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgAction_BeHit_C2BS, true);
        msg.AddParam("sobID", sobID);
		msg.AddParam("srcActorID", srcActorID);
		msg.AddParam("isBack", isBack);
		msg.AddParam("isFly", isFly);

        LCMsgSender.Singleton.SendMsg(msg);

        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    //roll action
	public override int SendAction_Roll_C2BS(int sobID, Vector3 curPos, Vector3 targetPos)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgAction_Roll_C2BS, true);
        msg.AddParam("sobID", sobID);
		msg.AddParam("curPosX", curPos.x);
		msg.AddParam("curPosZ", curPos.z);
		msg.AddParam("targetPosX", targetPos.x);
		msg.AddParam("targetPosZ", targetPos.z);
        LCMsgSender.Singleton.SendMsg(msg);

        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    //actorEnter action
	public override int SendAction_ActorEnter_C2BS(int sobID, Vector3 curPos, Vector3 forward)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgAction_ActorEnter_C2BS, true);
        msg.AddParam("sobID", sobID);
		msg.AddParam("curPosX", curPos.x);
		msg.AddParam("curPosY", curPos.z);
		msg.AddParam("forwardX", forward.x);
		msg.AddParam("forwardY", forward.z);

        LCMsgSender.Singleton.SendMsg(msg);

        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    //actorExit action
	public override int SendAction_ActorExit_C2BS(int sobID, Vector3 curPos)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgAction_ActorExit_C2BS, true);
        msg.AddParam("sobID", sobID);
		msg.AddParam("curPosX", curPos.x);
		msg.AddParam("curPosY", curPos.y);

        LCMsgSender.Singleton.SendMsg(msg);

        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    //jumpin action
	public override int SendAction_JumpIn_C2BS(int sobID, int targetID)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgAction_JumpIn_C2BS, true);
        msg.AddParam("sobID", sobID);
		msg.AddParam("targetID", targetID);

        LCMsgSender.Singleton.SendMsg(msg);

        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    //jumpout action
    public override int SendAction_JumpOut_C2BS(int sobID, float x, float z)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgAction_JumpOut_C2BS, true);
        msg.AddParam("sobID", sobID);
        msg.AddParam("x", x);
        msg.AddParam("z", z);

        LCMsgSender.Singleton.SendMsg(msg);

        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    //action被打断
    public override int SendActionInterupt_C2BS(int sobID, int actionType)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgActionInterupt_C2BS, true);
        msg.AddParam("sobID", sobID);
        msg.AddParam("actionType", actionType);

        LCMsgSender.Singleton.SendMsg(msg);

        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    
    // 更新COMbo数量
    public override int SendComboNum_C2BS(int type,int skillResultId=0)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgComboNum_BS2C, true);
        msg.AddParam("combotype", type);
        msg.AddParam("skillResultId", skillResultId);
        LCMsgSender.Singleton.SendMsg(msg);
       
        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }

    //
    public override int SendTestAllSkill_C2BS(int sobID, List<int> skillIDList)
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgTestAllSkill_C2BS, true);
        msg.AddParam("sobID", sobID);
        msg.AddArray("skillIDList", skillIDList, 8);
        LCMsgSender.Singleton.SendMsg(msg);

        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;
    }
    public override int SendLevelUpSkill_C2BS(int sobID, int skillIndex,bool isSkill) 
    {
        MessageBlock msg = MessageBlock.CreateMessage(GamePacketID.ENMsgLevelUpSkill_C2BS, true);
        msg.AddParam("sobID", sobID);
        msg.AddParam("skillIndex", skillIndex);
        if (isSkill)
        {
            msg.AddParam("isSkill", 1);
        }
        else 
        {
            msg.AddParam("isSkill", 0);
        }
        LCMsgSender.Singleton.SendMsg(msg);
        int msgID = msg.MessageID;
        MessageBlock.ReleaseMessage(msg);
        return msgID;

    }
}

