using UnityEngine;
using System;
using System.Collections.Generic;


public class StateBattle : State
{
	//private GameObject m_terrain = null;
    private GameObject m_sceneLight;
    private Light m_light;
    public CameraController SelfCamera { get { return MainGame.Singleton.MainCamera; } }
    public void InitGUI()
    {
        MainUIManager.Singleton.OnLoadStateUI("StateBattle");
         // 如果是长连接
        if (ClientNet.Singleton.IsLongConnecting)
        {
            UISwordSoul.GetInstance().HideWindow();
            UIReward.GetInstance().HideWindow();
            UIBattleBtn_D.GetInstance().HideAutoAttack();
            UIMainHead.GetInstance().HideSwordSouls();
        }
    }
    //隐藏ui，清除ui数据
    public void ClearGUI()
    {
        MainUIManager.Singleton.OnExitDestroy("StateBattle");
    }
	public override void OnEnter()
	{
		base.OnEnter();
        if (ForwardDirectionArrow.Singleton == null)
        {
            new ForwardDirectionArrow();
        }
        //主角色
        {
            MainPlayer chief = ActorManager.Singleton.Chief;
            if (chief == null)
            {
                chief = ActorManager.Singleton.CreatePureActor(ActorType.enMain, (int)EnMyPlayers.enChief, Team.Singleton.Chief.Guid, Team.Singleton.Chief.IDInTable) as MainPlayer;
                ActorManager.Singleton.SetChief(chief);
                chief.IsActorExit = false;
            }
            chief.Props.SetProperty_Int32(ENProperty.vocationid, 1);
            chief.CreateNeedModels();
            if (!chief.IsActorExit)
            {
                chief.UnhideMe(new Vector3(chief.PositionX, 0, chief.PositionZ));
            }
        }
        if(!ClientNet.Singleton.IsLongConnecting)
        {//长连接，不创建角色
            //副角色
            if (null != Team.Singleton.Deputy)
            {
                MainPlayer deputy = ActorManager.Singleton.Deputy;
                if (deputy == null)
                {
                    deputy = ActorManager.Singleton.CreatePureActor(ActorType.enMain, (int)EnMyPlayers.enDeputy, Team.Singleton.Deputy.Guid, Team.Singleton.Deputy.IDInTable) as MainPlayer;
                    ActorManager.Singleton.SetDeputy(deputy);
                    deputy.IsActorExit = true;
                }
                deputy.Props.SetProperty_Int32(ENProperty.vocationid, 1);
                deputy.CreateNeedModels();
                //副角色退场
                deputy.ActorExit();
                deputy.HideMe();
            }
            //支援角色
//             if (null != Team.Singleton.Support)
//             {
//                 Player support = ActorManager.Singleton.Support;
//                 if (support == null)
//                 {
//                     support = ActorManager.Singleton.CreatePureActor(ActorType.enSwitch, (int)EnMyPlayers.enSupport, Team.Singleton.Support.Guid, Team.Singleton.Support.IDInTable) as Player;
//                     ActorManager.Singleton.SetSupport(support);
//                 }
//                 support.Props.SetProperty_Int32(ENProperty.vocationid, 5);
//                 support.CreateNeedModels();
//                 support.InitSwitchSkillUI();
//                 //支援角色隐藏
//                 support.HideMe();
//                 //停止tick
//                 ActorManager.Singleton.ReleaseActor(support.ID, false);
//             }
//             if (!GameSettings.Singleton.m_isHideChangeAndFollow)
//             {//战友角色
//                 if (null != Team.Singleton.Comrade)
//                 {
//                     Player comrade = ActorManager.Singleton.Comrade;
//                     if (comrade == null)
//                     {
//                         comrade = ActorManager.Singleton.CreatePureActor(ActorType.enFollow, (int)EnMyPlayers.enComrade, Team.Singleton.Comrade.Guid, Team.Singleton.Comrade.IDInTable) as Player;
//                         ActorManager.Singleton.SetComrade(comrade);
//                     }
//                     comrade.CreateNeedModels();
//                 }
//             }
        }
        InitGUI();
		GameTable.SwitchAnimationTable("AnimationFight");
        
        //摄像机就位后，再隐藏界面
       // UIStageMenu.GetInstance().HideWindow();
        //!加载场景
        SM.RandomRoomLevel.Singleton.LoadNeedResource();
        // 初始化升级数据
        LevelUp.Singleton.InitData();
        //战斗stage开始
        BattleArena.Singleton.StageStart();

        Loading.Singleton.m_conFailedNum = 0;

        Reward.Singleton.Reset();

        // 一进入战斗状态时 记录一些数据
        BattleSummary.Singleton.RecordData();

        Map.Singleton.SetMapSize();

        KillTips.Singleton.Clear();
	}
	public override void OnExit()
	{
        ClearGUI();
        RemoteAttackManager.Singleton.Clear();
        ActorManager.Singleton.ClearActor();
        SM.RandomRoomLevel.Singleton.OnQuit();
        BattleArena.Singleton.Clear();
        PoolManager.Singleton.DestroyAll();
        base.OnExit();
	}
	public override void OnUpdate()
	{
        if (IsGamePause())
        {
            return;
        }
        ClientNet.Singleton.Update();
        ActorManager.Singleton.Update();
        RemoteAttackManager.Singleton.Update();
		User.Singleton.Update();
	}
	public override void OnLateUpdate()
	{
        if (IsGamePause())
        {
            return;
        }
		if (null != mainPlayer)
		{
            SelfCamera.Follow(mainPlayer);
            SelfCamera.Update();
		}
		ActorManager.Singleton.LateUpdate();
        CursorEffectFunc.Singleton.OnLateUpdate();
	}
	public override void OnFixedUpdate()
	{
        if (IsGamePause())
        {
            return;
        }
        ActorManager.Singleton.FixedUpdate();
        SM.RandomRoomLevel.Singleton.FixedUpdate();
        EventManager.Singleton.FixedUpdate();
        EResultManager.Singleton.FixedUpdate();
        BattleArena.Singleton.FixedUpdate();
        CursorEffectFunc.Singleton.OnFixedUpdate();
        IMiniServer.Singleton.Tick();
	}
	public override void OnPause()
	{

	}
	public override void OnResume()
	{
		base.OnResume();
	}
	public override void OnTap(Vector2 fingerPos)
	{
        if (IsGamePause())
        {
            return;
        }
        CursorEffectFunc.Singleton.OnTriggerEvent(CursorEffectFunc.ENFingerType.enOnTapType, fingerPos);
	}
	public override void OnLongPress(Vector2 fingerPos)
	{
        if (IsGamePause())
        {
            return;
        }
	}
	public override void OnDoubleTap(Vector2 fingerPos)
	{
        if (!BattleArena.Singleton.m_roomIsRolling)
        {
            return;
        }

        if (IsGamePause())
        {
            return;
        }
        //Debug.Log("--------------OnDoubleTap------------------");
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(fingerPos);
        if (!Physics.Raycast(ray, out hitInfo, 1500.0f))
        {
            return;
        }
        Transform transform = hitInfo.collider.gameObject.transform;
        Transform mayActorTransform = transform.parent;
        while (mayActorTransform != null)
        {
            ActorProp cfg = mayActorTransform.GetComponent<ActorProp>();
            if (cfg != null && cfg.ActorLogicObj != mainPlayer && cfg.ActorLogicObj.Props.GetProperty_Int32(ENProperty.islive) == 1)
            {
                return;
            }
            mayActorTransform = mayActorTransform.parent;
        }
        //mainPlayer.CurrentCmd = new MainPlayer.Cmd(hitInfo.point, Player.ENCmdType.enRoll);
	}
	public override void OnDragMove(Vector2 fingerPos, Vector2 delta, Vector2 TotalMove)
	{
        if (IsGamePause())
        {
            return;
        }
        //Debug.Log("--------------OnDragMove------------------");
        if (GameSettings.Singleton.FingerOpen == true)
        {
            m_sceneLight = GameObject.Find("Directional light").gameObject;
            m_light = m_sceneLight.GetComponent<Light>();
            m_light.intensity = Mathf.Clamp(m_light.intensity + delta.x / 100, 0.0f, 8.0f);
        }
        Vector3 curPos = Vector3.zero;
        curPos.x = fingerPos.x + TotalMove.x;
        curPos.y = fingerPos.y + TotalMove.y;
        CursorEffectFunc.Singleton.OnTriggerEvent(CursorEffectFunc.ENFingerType.enOnDragMoveType, curPos);
	}
    
	public override void OnDragMoveEnd(Vector2 fingerPos, Vector2 delta, Vector2 TotalMove)
	{
        //Debug.Log("OnDragMoveEnd");
        if (IsGamePause())
        {
            return;
        }
        Vector3 curPos = Vector3.zero;
        curPos.x = fingerPos.x + TotalMove.x;
        curPos.y = fingerPos.y + TotalMove.y;
        CursorEffectFunc.Singleton.OnTriggerEvent(CursorEffectFunc.ENFingerType.enOnDragMoveEndType, curPos);
	}
	public override void OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta)
	{
        if (GameSettings.Singleton.FingerOpen == true)
        {
            //Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - delta/20, 1.0f, 179.0f);
        }
	}
	public override void OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta)
	{
        if (GameSettings.Singleton.FingerOpen == true)
        {
            SelfCamera.OnTwoFingerDragMove(fingerPos, delta);
        }
	}
    public override void OnTwoFingerDragMoveEnd(Vector2 fingerPos)
	{
        SelfCamera.OnTwoFingerDragMoveEnd(fingerPos);
	}
	public override void OnRotate(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta)
	{
        SelfCamera.OnRotate(fingerPos1, fingerPos2, rotationAngleDelta);
	}
	public override void OnRotateEnd(Vector2 fingerPos1, Vector2 fingerPos2, float totalRotationAngle)
	{
        SelfCamera.OnRotateEnd(fingerPos1, fingerPos2, totalRotationAngle);
	}
	public override void OnDrawGraph(string graphName)
	{
	}
    MainPlayer mainPlayer { get { return ActorManager.Singleton.MainActor; } }
	public GameObject m_moveTarPosParticle = null;
    public override void OnPressDown(int fingerIndex, Vector2 fingerPos)
    {
        if (IsGamePause())
        {
            return;
        }
        CursorEffectFunc.Singleton.OnTriggerEvent(CursorEffectFunc.ENFingerType.enOnPressDownType, fingerPos);
    }
    public override void OnPressUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown)
    {
        if (IsGamePause())
        {
            return;
        }
        CursorEffectFunc.Singleton.OnTriggerEvent(CursorEffectFunc.ENFingerType.enOnPressUpType, fingerPos);
    }
    public override void OnSwipe(Vector2 total, FingerGestures.SwipeDirection direction)
    {
        if (!BattleArena.Singleton.m_roomIsRolling)
        {
            return;
        }
        if (IsGamePause())
        {
            return;
        }
        Vector3 rolePosToScreenPos = Camera.main.WorldToScreenPoint(mainPlayer.RealPos);
        Vector3 targetPosOnScreen = new Vector3(total.x + rolePosToScreenPos.x, total.y + rolePosToScreenPos.y, rolePosToScreenPos.z);
        Vector3 targetRealPos = Camera.main.ScreenToWorldPoint(targetPosOnScreen);

        mainPlayer.CurrentCmd = new MainPlayer.Cmd(targetRealPos, Player.ENCmdType.enRoll);

    }
}