//长连接消息接收器 added by luozj
using System;
using UnityEngine;
using NGE.Network;
using System.Collections.Generic;

public class LCMsgReceiver
{
    enum EnBattleServerErrorCode
    {
        enNone,
        enCreateSceneFail,//创建地图失败
        enCreateSceneNpcFail,//创建地图的npc失败
        enNotFoundPlayerWithTokenID,//没有找到对应令牌id的player
    }
    public LCMsgReceiver()
    {
        ;
    }

    public void Init()
    {
        // 成功连接时执行OnMainConnected函数
        ClientNet.Singleton.LongConnect.Connected += new ConnectionCallback(OnMainConnected);
        // 连接失败时执行OnConnectFailed函数
        ClientNet.Singleton.LongConnect.ConnectionFailed += new ConnectionFailedCallback(OnConnectFailed);
        ClientNet.Singleton.LongConnect.ConnectionLost += new ConnectionFailedCallback(OnConnectFailed);

        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgMessageRespond_BS2C, new OnPacketReceive(OnMsgMessageRespond_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgEnterDugeon_BS2C, new OnPacketReceive(OnMsgEnterDugeon_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgSyncActor_BS2C, new OnPacketReceive(OnMsgSyncActor_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgSyncPropertySet_S2C, new OnPacketReceive(OnMsgSyncPropertySet_S2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgSyncResultTree_S2C, new OnPacketReceive(OnMsgSyncResultTree_S2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgDepositNpcAi_BS2C, new OnPacketReceive(OnMsgDepositNpcAi_BS2C));
        //ClientNet.Singleton.RegisterHandler(GamePacketID.ENMsg_BS2C, new OnPacketReceive(null), ClientNet.ENConnectionType.enLongConnect);
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgDotaMapInfo_BS2C, new OnPacketReceive(OnMsgDotaMapInfo_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgSyncAction_Move_BS2C, new OnPacketReceive(OnMsgSyncAction_Move_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgSyncAction_Attack_BS2C, new OnPacketReceive(OnMsgSyncAction_Attack_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgSyncAction_AttackingMove_BS2C, new OnPacketReceive(OnMsgSyncAction_AttackingMove_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgSyncAction_BeHit_BS2C, new OnPacketReceive(OnMsgSyncAction_BeHit_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgSyncAction_Roll_BS2C, new OnPacketReceive(OnMsgSyncAction_Roll_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgSyncAction_ActorEnter_BS2C, new OnPacketReceive(OnMsgSyncAction_ActorEnter_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgSyncAction_ActorExit_BS2C, new OnPacketReceive(OnMsgSyncAction_ActorExit_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgSyncAction_JumpIn_BS2C, new OnPacketReceive(OnMsgSyncAction_JumpIn_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgSyncAction_JumpOut_BS2C, new OnPacketReceive(OnMsgSyncAction_JumpOut_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgSyncActionInterupt_BS2C, new OnPacketReceive(OnMsgSyncActionInterupt_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgSceneFinished_BS2C, new OnPacketReceive(OnMsgSceneFinished_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgPlayerLeaveScene_BS2C, new OnPacketReceive(OnMsgPlayerLeaveScene_BS2C));
        ClientNet.Singleton.LCRegisterHandler(GamePacketID.ENMsgRecoverLevelUpSucc_BS2C, new OnPacketReceive(OnMsgRecoverLevelUpSucc_BS2C));
        
        
        if (ClientNet.Singleton.LongConnect.Reactor.DefaultHandler == null)
        {
            ClientNet.Singleton.LongConnect.Reactor.RegisterDefaultHandler(new OnPacketReceive(OnDefaultMessage));
        }
    }
    // 连接成功时执行的函数
    void OnMainConnected(ConnectionArgs args, object state)
    {
        LCMsgSender.Singleton.IsTryingConn = false;
        //ClientNetProperty.Singleton.NotifyChanged((int)ClientNetProperty.ENPropertyChanged.enConnected, null);

        LCMsgSender.Singleton.SendCacheMsg();
    }
    // 连接失败时执行的函数
    void OnConnectFailed(ConnectionArgs args, ErrorArgs error, object state)
    {
        Debug.Log("OnConnectFailed !!!");
        LCMsgSender.Singleton.IsTryingConn = false;
        //ClientNetProperty.Singleton.NotifyChanged((int)ClientNetProperty.ENPropertyChanged.enConnectionFailed, error.ErrorType);

        // 如果没有得到SESSION才弹提示框
        //if (false == LCMsgSender.Singleton.GotSession)
        {
            UICommonMsgBoxCfg boxCfg = new UICommonMsgBoxCfg();
            boxCfg.mainTextPrefab = "UILostConnectionLabel";
            boxCfg.buttonNum = 1;
            UICommonMsgBox.GetInstance().ShowMsgBox(OnConnectFailedButtonClick, null, boxCfg);
        }
    }
    // 连接失败 确认后 重联
    void OnConnectFailedButtonClick(object sender, EventArgs e)
    {
        LCMsgSender.Singleton.ReConnect();
    }
    // 默认消息接收
    void OnDefaultMessage(PacketReader p, object state)
    {
        int id = p.PacketID;
        Debug.LogWarning("Missed Process of message:" + id);
    }

    //统一回复的消息
    void OnMsgMessageRespond_BS2C(PacketReader p, object state)
    {
        //Debug.Log("OnMsgMessageRespond_BS2C");
        MessageRespond respond;
        respond.MessageID = p.ReadInt16();
        respond.IsSuccess = (p.ReadInt16() == 0 ? false : true);
        respond.RespondCode = p.ReadInt32();

        if (!respond.IsSuccess)
        {
            if (respond.MessageID == GamePacketID.ENMsgError_BS2S)
            {//battleServer发给zoneServer的错误信息
                Debug.Log("battleServer发给zoneServer的错误信息");
            }
            switch ((EnBattleServerErrorCode)respond.RespondCode)
            {
                case EnBattleServerErrorCode.enCreateSceneFail:
                    {
                        Debug.LogWarning("battleServer创建地图失败");
                    }
                    break;
                case EnBattleServerErrorCode.enCreateSceneNpcFail:
                    break;
                case EnBattleServerErrorCode.enNotFoundPlayerWithTokenID:
                    break;
            }
        }
        MessageRespondProperty.Singleton.OnMessageRespond(respond);
    }

    //进入副本
    void OnMsgEnterDugeon_BS2C(PacketReader p, object state)
    {
        Debug.Log("OnMsgEnterDugeon_BS2C");
        int dungeonID = p.ReadInt32();
        int dataSize = p.ReadInt32();
        string dungonData = p.ReadUTF8(dataSize);
        BattleArena.Singleton.ReceiveBattleArenaInfo(dungonData, dungeonID);
        //test
        //IMiniServer.Singleton.SendTest_C2BS();
    }
    //同步服务器脏属性
    void OnMsgSyncPropertySet_S2C(PacketReader p, object state) 
    {
        //Debug.Log("OnMsgSyncPropertySet_S2C");
        p.ReadInt32();
        p.ReadInt32();
        int id = p.ReadInt32();
        int length = p.ReadInt32();
        Actor syncActor = ActorManager.Singleton.Lookup(id);
        if (syncActor == null)
        {
            Debug.LogError("can not find actor,id:"+id);
            return;
        }
        syncActor.Props.Deserialize(p);
        syncActor.UpdateHPBar();
        if (syncActor.Props.DirtyPropertys.Contains(ENProperty.SkillIDList))
        {
            syncActor.UpdateSkillEventAnimation();
        }

        BattleArena.Singleton.UpdateKillInfo(syncActor);

    }
    //广播消息
    void OnMsgSyncActor_BS2C(PacketReader p, object state) 
    {
        //Debug.Log("OnMsgPlayerEnterScene_BS2C");
        int sobID = p.ReadInt32();
        int syncType = p.ReadInt32();
        int sobClassType = p.ReadInt32();
        int sobTag = p.ReadInt32();
        int dataLen = p.ReadInt32();
        //Debug.LogWarning("OnMsgSyncActor_BS2C,sobID:" + sobID + ",syncType:" + syncType + ",sobTag:" + sobTag);

        switch ((ENSyncActorType)syncType)
        {
            case ENSyncActorType.enSyncViewable:
                {//别人-可见
                    ActorType type = ActorType.enPlayer;
                    if ((int)ENSobTag.enSobNpc == sobTag)
                    {//npc
                        type = ActorType.enNPC;
                    }

					Actor selfActor = ActorManager.Singleton.CreatePureActor(type, sobID, CSItemGuid.Zero, 0);
                    if (selfActor != null)
                    {
                        int index = p.Index;
                        selfActor.Props.Deserialize(p);
                        selfActor.OnInitProps();
                        //再次Deserialize，是因为在OnInitProps时props被覆盖了
                        p.Seek(index, System.IO.SeekOrigin.Begin);
                        selfActor.Props.Deserialize(p);

                        
                        selfActor.OnInitSkillBag();
                        selfActor.CreateNeedModels();
						selfActor.InitName();

                        if (selfActor.IsActorExit)
                        {
                            selfActor.HideMe();
                        }
                        else
                        {
                            selfActor.UnhideMe(new Vector3(selfActor.PositionX, 0, selfActor.PositionZ));
                        }

                        selfActor.SelfAI = new AIServerActor();
                        selfActor.SelfAI.Owner = selfActor;

                        if ((int)ENSobTag.enSobNpc != sobTag)
                        {
                            Debug.Log("player enter scene,id:" + selfActor.ID + ",name:" + selfActor.Name);
                            // 添加到小地图中
                            UIMap.GetInstance().ShowWindow();
                            Map.Singleton.AddPoint(selfActor);
                        }
						selfActor.UpdateHPBar();
                    }
                }
                break;
            case ENSyncActorType.enSyncRemoveView:
                {//别人-不可见
                    ;
                }
                break;
            case ENSyncActorType.enSyncSelf:
                {//自己
                    ActorType type = ActorType.enMain;
                    CSItemGuid guid = CSItemGuid.Zero;
                    Actor self = null;
					bool isNotifyUI = false;
                    if (sobTag == (int)ENSobTag.enChiefPlayer)
                    {
                        guid = Team.Singleton.Chief.Guid;
                        self = ActorManager.Singleton.Chief;
						isNotifyUI = true;
                    }
                    else if (sobTag == (int)ENSobTag.enDeputyPlayer)
                    {
                        guid = Team.Singleton.Deputy.Guid;
                        self = ActorManager.Singleton.Deputy;
                    }
                    else if (sobTag == (int)ENSobTag.enSupportPlayer)
                    {
                        type = ActorType.enSwitch;
                        guid = Team.Singleton.Support.Guid;
                        self = ActorManager.Singleton.Support;
                    }
                    else if (sobTag == (int)ENSobTag.enComradePlayer)
                    {
                        type = ActorType.enFollow;
                        guid = Team.Singleton.Comrade.Guid;
                        self = ActorManager.Singleton.Comrade;
                    }
                    if (self == null)
                    {
                        self = ActorManager.Singleton.CreatePureActor(type, sobID, guid, 0);
                    }
                    else
                    {
                        if (sobTag != (int)ENSobTag.enChiefPlayer)
                        {
                            ActorManager.Singleton.ModifyActorID(self.ID, sobID);
                            self.SetID(sobID);
                        }
                    }
                    if (self != null)
                    {
                        int idInTable = self.IDInTable;

                        self.m_isDeposited = true;
                        self.SetSobTag(sobTag);

                        int index = p.Index;
                        self.Props.Deserialize(p);
                        self.OnInitProps();
                        if (sobTag == (int)ENSobTag.enChiefPlayer)
                        {
                            if (idInTable != 0 && idInTable != self.IDInTable)
                            {
                                ActorManager.Singleton.ReleaseActor(self.ID);
                                self = ActorManager.Singleton.CreatePureActor(type, sobID, guid, self.IDInTable);
                                if (self == null)
                                {
                                    Debug.LogError("create chief player fail");
                                    return;
                                }

                                self.m_isDeposited = true;
                                self.SetSobTag(sobTag);
                            }
                            else
                            {
                                ActorManager.Singleton.ModifyActorID(self.ID, sobID);
                                self.SetID(sobID);
                            }
                        }
                        //再次Deserialize，是因为在OnInitProps时props被覆盖了
                        p.Seek(index, System.IO.SeekOrigin.Begin);
                        self.Props.Deserialize(p);

                        self.OnInitSkillBag(isNotifyUI);
                        self.CreateNeedModels();

                        if (sobTag == (int)ENSobTag.enChiefPlayer)
                        {
                            ActorManager.Singleton.SetChief(self);
							ActorManager.Singleton.RefreshAllName();

                            // 添加到小地图中
                            UIMap.GetInstance().ShowWindow();
                            Map.Singleton.AddPoint(self);

                            if (self.IsActorExit)
                            {
                                self.HideMe();
                                (self as MainPlayer).ActorExit();
                            }
                            else
                            {
                                self.UnhideMe(new Vector3(self.PositionX, 0, self.PositionZ));
                                MainGame.Singleton.MainCamera.MoveAtOnce(self);
                            }
                            self.NotifyChanged((int)Actor.ENPropertyChanged.enMainHead, EnMainHeadType.enInitActor);

                            //设置优先选择的目标类型
                            (self as MainPlayer).m_firstTargetType = ActorType.enPlayer;
                        }
                        else if (sobTag == (int)ENSobTag.enDeputyPlayer)
                        {
                            ActorManager.Singleton.SetDeputy(self);

                            // 添加到小地图中
                            UIMap.GetInstance().ShowWindow();
                            Map.Singleton.AddPoint(self);

                            if (self.IsActorExit)
                            {
                                self.HideMe();
                                (self as MainPlayer).ActorExit();
                            }
                            else
                            {
                                self.UnhideMe(new Vector3(self.PositionX, 0, self.PositionZ));
                                MainGame.Singleton.MainCamera.MoveAtOnce(self);
                                //self.NotifyChanged((int)Actor.ENPropertyChanged.enMainHead, EnMainHeadType.enSwitchActor);
                            }
                            self.NotifyChanged((int)Actor.ENPropertyChanged.enMainHead, EnMainHeadType.enInitActor);

                            //设置优先选择的目标类型
                            (self as MainPlayer).m_firstTargetType = ActorType.enPlayer;
                        }
                        else if (sobTag == (int)ENSobTag.enSupportPlayer)
                        {
                            ActorManager.Singleton.SetSupport(self);
                            self.HideMe();
                        }
                        else if (sobTag == (int)ENSobTag.enComradePlayer)
                        {
                            ActorManager.Singleton.SetComrade(self);
                            self.HideMe();//同伴先隐藏自己
                        }
                        self.InitName();
						self.UpdateHPBar();
                    }
                }
                break;
        }
    }

    // 服务器发送回 resultTree
    public void OnMsgSyncResultTree_S2C(PacketReader p, object state)
    {
        int dataSize        = p.ReadInt32();
         IResultControl resultControl = BattleFactory.Singleton.GetBattleContext().CreateResultControl();
         resultControl.GetResultTree().Deserialize(p);
         resultControl.GetResultTree().ExecAllServerResult(resultControl);
         resultControl.GetResultTree().ReleaseAllResult();
    }

    //托管npcai
    public void OnMsgDepositNpcAi_BS2C(PacketReader p, object state)
    {
        int sobID = p.ReadInt32();
        ActorManager.Singleton.ModifyDepositNpc(sobID);
    }

	//接收服务器发来的阵营寻路节点坐标
	public void OnMsgDotaMapInfo_BS2C(PacketReader p, object state)
	{
		//Debug.LogWarning("OnMsgDotaMapInfo_BS2C");
		SM.RandomRoomLevel.Singleton.m_scenePathNodeDic.Clear();
		SM.RandomRoomLevel.Singleton.m_sceneCampReliveNode.Clear();
		//float reliveTime = p.ReadFloat();
		//SM.RandomRoomLevel.Singleton.m_sceneHeroReliveTime = reliveTime;
		//Debug.LogWarning("relive time:" + reliveTime);
        int isRolling = p.ReadInt32();
        if (1 == isRolling)
        {
            BattleArena.Singleton.m_roomIsRolling = true;
        }
        else 
        {
            BattleArena.Singleton.m_roomIsRolling = false;
        }

		int num = p.ReadInt32();
		Debug.LogWarning("num:" + num);
		for (int i = 0; i < 5; i++)
		{
			int camp = p.ReadInt32();
			//Debug.LogWarning("camp:" + camp);
			int count = p.ReadInt32();
			//Debug.LogWarning("count:" + count);
			List<Vector3> posList = new List<Vector3>();
			for (int j = 0; j < 10; j++)
			{
				if (j < count)
				{
					float posX = p.ReadFloat();
					float posZ = p.ReadFloat();
					posList.Add(new Vector3(posX, 0, posZ));
					//Debug.LogWarning("pos,x:" + posX + ",z" + posZ);
				}
				else
				{
					//Debug.LogWarning("null,j:"+j+",count:"+count);
					p.ReadFloat();
					p.ReadFloat();
				}
			}
			//Debug.LogWarning("list.count:" + posList.Count);
            if (!SM.RandomRoomLevel.Singleton.m_scenePathNodeDic.ContainsKey(camp))
            {
                SM.RandomRoomLevel.Singleton.m_scenePathNodeDic.Add(camp, posList);
            }
			float relivePosX = p.ReadFloat();
			float relivePosZ = p.ReadFloat();
            if (!SM.RandomRoomLevel.Singleton.m_sceneCampReliveNode.ContainsKey(camp))
            {
                SM.RandomRoomLevel.Singleton.m_sceneCampReliveNode.Add(camp, new Vector3(relivePosX, 0, relivePosZ));
            }
			//Debug.LogWarning("relivePos,x:" + relivePosX + ",z" + relivePosZ);
		}

    }

    //同步move action
    public void OnMsgSyncAction_Move_BS2C(PacketReader p, object state)
    {
        int id = p.ReadInt32();
        float x = p.ReadFloat();
        float z = p.ReadFloat();
        float startx = p.ReadFloat();
        float startz = p.ReadFloat();

        Actor actor = ActorManager.Singleton.Lookup(id);
        if (actor != null)
        {
            actor.CurrentCmd = Actor.Cmd.CreateSyncMove(new Vector3(x, actor.MainPos.y, z), new Vector3(startx, actor.MainPos.y, startz));
        }
        else
        {
            Debug.LogWarning("actor is not exist, id:" + id);
        }
    }

    //同步attack action
    public void OnMsgSyncAction_Attack_BS2C(PacketReader p, object state)
    {
        int id = p.ReadInt32();
        int skillID = p.ReadInt32();
        int targetID = p.ReadInt32();
        float x = p.ReadFloat();
        float z = p.ReadFloat();
        //Debug.LogWarning("OnMsgSyncAction_Attack_BS2C,id:" + id + ",skillID:" + skillID + ",targetID:" + targetID);
        Actor actor = ActorManager.Singleton.Lookup(id);
        if (actor != null)
        {
            actor.CurrentCmd = new Actor.Cmd(skillID, targetID,new Vector3(x, actor.MainPos.y, z));
        }
        else
        {
            Debug.LogError("actor is not exist, id:" + id);
        }
    }
    //接收到服务器发送的 技能升级
    public void OnMsgRecoverLevelUpSucc_BS2C(PacketReader p, object state) 
    {
        int levelPointNum = p.ReadInt32();
        bool isSkill = (p.ReadInt32() == 1) ? true : false;
        int skillId = p.ReadInt32();
        int yellowPointNum = p.ReadInt32();
        MainPlayer mainPlayer = ActorManager.Singleton.MainActor;
        mainPlayer.Props.SetProperty_Int32(ENProperty.LevelUpPoint, levelPointNum);
        if (isSkill)
        {
            int skillIndex = UIBattleBtn_D.Singleton.GetSkillIndexBySkillId(skillId);
            PropertyValueIntListView skillLevelList = mainPlayer.Props.GetProperty_Custom(ENProperty.SkillLevelList) as PropertyValueIntListView;
            skillLevelList.m_list[skillIndex]++;
            mainPlayer.Props.SetProperty_Custom(ENProperty.SkillLevelList, skillLevelList);
            Actor.ActorSkillInfo info = mainPlayer.SkillBag.Find(item => item.SkillTableInfo.ID == skillId);
            if (null != info)
            {
                info.SkillLevel++;
            }
        }
        else 
        {
            mainPlayer.Props.SetProperty_Int32(ENProperty.YellowPointLevel, yellowPointNum);
        }

        
        mainPlayer.UpDataSkillLevelUp();
    }

    //同步attackingMove action
    public void OnMsgSyncAction_AttackingMove_BS2C(PacketReader p, object state)
    {
        int id = p.ReadInt32();
        int skillID = p.ReadInt32();
        int targetID = p.ReadInt32();
        float x = p.ReadFloat();
        float z = p.ReadFloat();
        Actor actor = ActorManager.Singleton.Lookup(id);
        if (actor != null)
        {
            actor.CurrentCmd = Actor.Cmd.CreateMovingAttack(skillID, targetID,new Vector3(x, actor.MainPos.y, z));
        }
        else
        {
            Debug.LogError("actor is not exist, id:" + id);
        }
    }

    //同步BeHit action
    public void OnMsgSyncAction_BeHit_BS2C(PacketReader p, object state)
    {
        int id = p.ReadInt32();
		int srcActorId = p.ReadInt32();
        bool isBack = p.ReadBoolean();
		bool isFly = p.ReadBoolean();

        Actor actor = ActorManager.Singleton.Lookup(id);
        if (actor != null)
        {
			actor.CurrentCmd = new Actor.Cmd(Actor.ENCmdType.enBeHitAction);
			actor.CurrentCmd.SetBeHitParam(srcActorId, isBack, isFly);
        }
        else
        {
            Debug.LogWarning("actor is not exist, id:" + id);
        }
    }

    //同步Roll action
    public void OnMsgSyncAction_Roll_BS2C(PacketReader p, object state)
    {
		//Debug.Log("OnMsgSyncAction_Roll_BS2C");
        int id = p.ReadInt32();
        float curX = p.ReadFloat();
		float curZ = p.ReadFloat();
		float targetX = p.ReadFloat();
		float targetZ = p.ReadFloat();
		//Debug.LogError("id="+id+" x:"+x + " y:"+z);
        Actor actor = ActorManager.Singleton.Lookup(id);
        if (actor != null)
        {
            actor.CurrentCmd = new Actor.Cmd(new Vector3(curX, actor.MainPos.y, curZ), new Vector3(targetX, actor.MainPos.y, targetZ));
        }
        else
        {
            Debug.LogWarning("actor is not exist, id:" + id);
        }
    }

    //同步ActorEnter action
    public void OnMsgSyncAction_ActorEnter_BS2C(PacketReader p, object state)
    {
		//Debug.LogError("OnMsgSyncAction_ActorEnter_BS2C");
		int id = p.ReadInt32();
		float curPosX = p.ReadFloat();
		float curPosZ = p.ReadFloat();
		float forwardX = p.ReadFloat();
		float forwardZ = p.ReadFloat();

        Actor actor = ActorManager.Singleton.Lookup(id);
        if (actor != null)
        {
			Vector3 curPos = new Vector3(curPosX, actor.MainPos.y, curPosZ);
            Vector3 forward = new Vector3(forwardX, actor.MainPos.y, forwardZ);
            actor.CurrentCmd = new Actor.Cmd(Actor.ENCmdType.enActorEnter, curPos, forward);
        }
        else
        {
            Debug.LogWarning("actor is not exist, id:" + id);
        }
    }

    //同步ActorExit action
    public void OnMsgSyncAction_ActorExit_BS2C(PacketReader p, object state)
    {
		//Debug.LogError("OnMsgSyncAction_ActorExit_BS2C");
		int id = p.ReadInt32();

        Actor actor = ActorManager.Singleton.Lookup(id);
        if (actor != null)
        {
            actor.CurrentCmd = new Actor.Cmd(Actor.ENCmdType.enActorExit);
        }
        else
        {
            Debug.LogWarning("actor is not exist, id:" + id);
        }
    }

    //同步JumpIn action
    public void OnMsgSyncAction_JumpIn_BS2C(PacketReader p, object state)
    {
        int id = p.ReadInt32();
		int targetID = p.ReadInt32();

        Actor actor = ActorManager.Singleton.Lookup(id);
        if (actor != null)
        {
            actor.CurrentCmd = new Actor.Cmd(Actor.ENCmdType.enJumpInAction);
			actor.CurrentCmd.SetJumpInParam(targetID);
        }
        else
        {
            Debug.LogWarning("actor is not exist, id:" + id);
        }
    }

    //同步JumpOut action
    public void OnMsgSyncAction_JumpOut_BS2C(PacketReader p, object state)
    {
        int id = p.ReadInt32();
        float x = p.ReadFloat();
        float z = p.ReadFloat();

        Actor actor = ActorManager.Singleton.Lookup(id);
        if (actor != null)
        {
			actor.CurrentCmd = new Actor.Cmd(Actor.ENCmdType.enJumpOutAction);
        }
        else
        {
            Debug.LogWarning("actor is not exist, id:" + id);
        }
    }

    //同步action被打断
    public void OnMsgSyncActionInterupt_BS2C(PacketReader p, object state)
    {
		//Debug.LogError("OnMsgSyncActionInterupt_BS2C");
        int id = p.ReadInt32();
        int interruptedActionType = p.ReadInt32();

		Actor actor = ActorManager.Singleton.Lookup(id);
		if (actor != null)
		{
			actor.CurrentCmd = new Actor.Cmd(Actor.ENCmdType.enInteruptAction);
            actor.CurrentCmd.SetInterruptParam(interruptedActionType);
		}
		else
		{
			Debug.LogWarning("actor is not exist, id:" + id);
		}
    }

    public void OnMsgSceneFinished_BS2C(PacketReader p, object state)
    {
        int failCamp = p.ReadInt32();

        Actor self = ActorManager.Singleton.MainActor;
        int myCamp = 0;
        if (self != null)
        {
            myCamp = self.Camp;
        }
        ClientNet.Singleton.m_connectionType = ClientNet.ENConnectionType.enShortConnect;
        BattleArena.Singleton.StageStop(false, myCamp != failCamp);
    }

    public void OnMsgPlayerLeaveScene_BS2C(PacketReader p, object state)
    {
        int sobID = p.ReadInt32();

        Actor player = ActorManager.Singleton.Lookup(sobID);
        if (player != null)
        {
            Debug.Log("player leave scene,id:" + sobID + ",name:" + player.Name);
        }
        ActorManager.Singleton.ReleaseActor(sobID);
    }
}