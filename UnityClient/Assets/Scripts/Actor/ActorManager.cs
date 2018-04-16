using System;
using System.Collections.Generic;
using UnityEngine;

public enum ActorType
{
    enNone,
	enMain,
	enPlayer,//其它玩家
	enNPC,
    enMold,//沙盘模型
    enSwitch,//切入技角色
    enFollow,//战友角色（跟随角色）
    enNPC_Friend,//友方npc
    enNPC_AllEnemy,//视所有人为敌人的npc
    enPlayer_Sneak,//Player潜行
    enNPCTrap,//机关
}
public enum EnMyPlayers
{
    enNone,
    enChief,
    enDeputy,
    enSupport,
    enComrade,
}
public enum VocationType
{
    enNpc = 0,
}
//阵营的枚举
public enum ENCamp
{
    enNone,
    enRed,//阵营-红
    enBlue,//阵营-蓝
    enYellow,//阵营-黄
    enNeutralHostile,//中立-敌对（视红、蓝、黄阵营为敌对）
    enNeutralFriend,//中立-友好（视所有阵营为友好）
}
public class ActorManager
{
	#region Singleton
    static ActorManager m_singleton;
    static public ActorManager Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new ActorManager();
            }
            return m_singleton;
        }
    }
	#endregion
    
    public Dictionary<int, Actor> m_actorMap = new Dictionary<int, Actor>();
    public Dictionary<int, Actor> m_delayReleaseActorDict = new Dictionary<int, Actor>();
    #region MainActor
    private MainPlayer m_chiefActor = null;
    public MainPlayer Chief
    {
        get
        {
            //if (m_chiefActor == null)
            //{
            //    m_chiefActor = Lookup((int)EnMyPlayers.enChief) as MainPlayer;
            //}
            return m_chiefActor;
        }
    }
    public void SetChief(Actor actor) { m_chiefActor = actor as MainPlayer; }
    private MainPlayer m_deputyActor = null;
    public MainPlayer Deputy
    {
        get
        {
            //if (m_deputyActor == null)
            //{
            //    m_deputyActor = Lookup((int)EnMyPlayers.enDeputy) as MainPlayer;
            //}
            return m_deputyActor;
        }
    }
    public void SetDeputy(Actor actor) { m_deputyActor = actor as MainPlayer; }
    private Player m_supportActor = null;
    public Player Support//支援角色
    {
        get
        {
            //if (m_supportActor == null)
            //{
            //    m_supportActor = Lookup((int)EnMyPlayers.enSupport) as Player;
            //}
            return m_supportActor;
        }
    }
    public void SetSupport(Actor actor) { m_supportActor = actor as Player; }
    private Player m_comradeActor = null;
    public Player Comrade//同伴
    {
        get
        {
            //if (m_comradeActor == null)
            //{
            //    m_comradeActor = Lookup((int)EnMyPlayers.enComrade) as Player;
            //}
            return m_comradeActor;
        }
    }
    public void SetComrade(Actor actor) { m_comradeActor = actor as Player; }
    public MainPlayer MainActor
    {
        get
        {
            if (Chief == null || Chief.IsActorExit)
            {
                return Deputy;
            }
            return Chief;
        }
    }
    #endregion
    public Actor MoldActor { get; private set; }
	public Actor Lookup(int id)
	{
		Actor actor = null;
		m_actorMap.TryGetValue(id, out actor);
		return actor;
	}
    //获取所有npc表里ID为id的npc列表
    public List<Actor> LookupNPC(int id)
    {
        List<Actor> actorList = new List<Actor>();
        foreach (KeyValuePair<int, Actor> pair in m_actorMap)
        {
            if (pair.Value.IDInTable == id)
            {
                actorList.Add(pair.Value);
            }
        }
        return actorList;
    }
    public Actor CreatePureActor(ActorType type, int actorID, CSItemGuid guid, int staticID, bool isAddInMap = true)
    {
        Actor actor = null;
        m_actorMap.TryGetValue(actorID, out actor);
        if (actor != null)
        {
            return actor;
        }
        switch (type)
        {
            case ActorType.enMain:
            {
                actor = new MainPlayer(type, actorID, staticID, guid);
                ConfigMainActor(actor);
            }
            break;
            case ActorType.enNPC:
            {
                actor = new NPC(type, actorID, staticID, guid);
				ConfigNPC(actor);
            }
            break;
            case ActorType.enPlayer:
            {
                actor = new Player(type, actorID, staticID, guid);
                ConfigPlayer(actor);
            }
            break;
            case ActorType.enFollow:
            {
                actor = new Player(type, actorID, staticID, guid);
                ConfigPlayer(actor);
                m_comradeActor = actor as Player;
            }
            break;
            case ActorType.enSwitch:
            {
                actor = new Player(type, actorID, staticID, guid);
                ConfigPlayer(actor);
                m_supportActor = actor as Player;
            }
            break;
//             case ActorType.enMold:
//             {
//                 actor = new SandTablePlayer(type, actorID);
//                 ConfigMainActor(actor);
//             }
            //break;
            case ActorType.enNPCTrap:
                actor = Trap.CreateTrapByType(actorID, staticID, guid);
                ConfigTrap(actor);
                break;
            default:
            break;
        }
        if (null != actor && isAddInMap)
        {
            m_actorMap.Add(actorID, actor);
            Actor delayActor;
            m_delayReleaseActorDict.TryGetValue(actorID, out delayActor);
            if (null != delayActor)
            {
                m_delayReleaseActorDict.Remove(actorID);
            }
        }
        return actor;
    }
    public int GetValidActorID(int id)
    {
        while (m_actorMap.ContainsKey(id))
        {
            ++id;
        }
        return id;
    }
	private List<int> m_releaseList = new List<int>();
	public void ReleaseActor(int actorID, bool isDestroy = true)
	{
		Actor actor = null;
		if (!m_actorMap.TryGetValue(actorID, out actor))
		{
			return;
		}
		if (!m_releaseList.Contains(actorID))
		{
            if (isDestroy)
            {
                actor.Destroy();
            }
            else
            {
                Actor delayActor;
                m_delayReleaseActorDict.TryGetValue(actorID, out delayActor);
                if (null == delayActor)
                {
                    m_delayReleaseActorDict.Add(actorID, actor);
                }
            }
			m_releaseList.Add(actorID);
		}
	}
    public void AddActor(int actorID, Actor actor)
    {
        if (null != actor)
        {
            Actor value = null;
            m_actorMap.TryGetValue(actorID, out value);
            if (value != null)
            {
                m_actorMap[actorID] = actor;
            }
            else
            {
                m_actorMap.Add(actorID, actor);
            }
            Actor delayActor;
            m_delayReleaseActorDict.TryGetValue(actorID, out delayActor);
            if (null != delayActor)
            {
                m_delayReleaseActorDict.Remove(actorID);
            }
        }
    }
	public void ClearActor()
	{
		foreach (var pair in m_actorMap)
		{
			Actor actor = pair.Value;
			actor.Destroy();
            actor = null;
		}
		m_actorMap.Clear();
        m_chiefActor = null;
        m_deputyActor = null;
        m_supportActor = null;
        m_comradeActor = null;
        m_switchCDTotal = 0;
	}
    public void ClearActorWithoutMain()
    {
        foreach (var pair in m_actorMap)
        {
            Actor actor = pair.Value;
            if (actor.Type != ActorType.enMain)
            {
                ReleaseActor(actor.ID);
            }
        }
    }
	public void Update()
	{
		foreach (var actorPair in m_actorMap)
		{
			actorPair.Value.Update();
		}
        foreach (KeyValuePair<int, Actor> pair in m_delayReleaseActorDict)
        {
            pair.Value.Update();
        }
	}
	public void LateUpdate()
	{
		foreach (var actorPair in m_actorMap)
		{
			actorPair.Value.LateUpdate();
		}
        foreach (KeyValuePair<int, Actor> pair in m_delayReleaseActorDict)
        {
            pair.Value.LateUpdate();
        }
	}
	public void FixedUpdate()
	{
		foreach (int actorID in m_releaseList)
		{
			m_actorMap.Remove(actorID);
		}
        m_releaseList.Clear();
        foreach (var actorPair in m_actorMap)
        {
            actorPair.Value.FixedUpdate();
        }
        foreach (KeyValuePair<int, Actor> pair in m_delayReleaseActorDict)
        {
			if (pair.Value != null)
			{
				pair.Value.FixedUpdate();
			}
        }
	}

	private void ConfigMainActor(Actor actor)
	{
        actor.TargetManager = new ActorTargetManager(actor);
        actor.TargetManager.m_showSelectedMark = true;
	}

    private void ConfigSandTableActor(Actor actor)
    {
        actor.TargetManager = new ActorTargetManager(actor);
    }

	public void ForEach(Action<Actor> action)
	{
		foreach (var item in m_actorMap)
		{
			action(item.Value);
		}
	}
    public void ForEach_buff(Action<Actor, BuffResultInfo> action, BuffResultInfo buffResultInfo)
	{
		foreach (var item in m_actorMap)
		{
            action(item.Value, buffResultInfo);
		}
	}
    public void ForEach_result(Action<Actor, float[]> action, float[] param)
    {
        foreach (var item in m_actorMap)
        {
            action(item.Value, param);
        }
    }
    public Actor ForEach_Bound(Bounds bound)
    {
        Actor selectActor = null;
        float fMinDist = 1000f;
        foreach (var item in m_actorMap)
        {
            Actor actor = item.Value;
            if (null != actor.ActorCfgData && actor.ActorCfgData.MeshRender)
            {
                if (!bound.Intersects(actor.ActorCfgData.MeshRender.bounds))
                {
                    continue;
                }
            }
            else if (null != actor.CenterCollider)
            {
                if (!bound.Intersects(actor.CenterCollider.bounds))
                {
                    continue;
                }
            }
            else
            {
                continue;
            }
            if (/*actor != MainActor && */!actor.IsDead)
            {
                float fDist = bound.SqrDistance(actor.MainPos);
                if (fDist < fMinDist)
                {
                    selectActor = actor;
                    fMinDist = fDist;
                }
            }
        }
        return selectActor;
    }
	private void ConfigNPC(Actor actor)
	{
// 		if (!ClientNet.Singleton.MainConnect.IsConnected)
// 		{
        actor.TargetManager = new ActorTargetManager(actor);
//		}
	}
	private void ConfigPlayer(Actor actor)
    {
        actor.TargetManager = new ActorTargetManager(actor);
	}
    private void ConfigTrap(Actor actor)
    {
        actor.TargetManager = new ActorTargetManager(actor);
    }
    //通知所有人清除目标
    public void NotifyAll_ClearTarget(int actorID)
    {
        foreach (Actor actor in m_actorMap.Values)
        {
            actor.TargetManager.OnActorRelease(actorID);
        }
    }
    public float m_switchCDTimer = 0;
    public int m_switchCDTotal = 0;
    public bool m_isSwitchForDead = false;//死亡后切换
    //private int m_switchBuffID = 0;
    //切换主控角色
    public bool SwitchMainActor(bool isSwitchForDead, bool isAttacking = false)
    {
        if (m_switchCDTotal > 0 && !MainActor.IsDead)
        {//cd中，并且当前角色未死
            //msg("XXX尚未休整完毕，请稍后再试！");
            return false;
        }
        if (Deputy == null)
        {//没有可切换的角色
            return false;
        }
        int curID,lastID;
        if (MainActor == Chief)
        {
            curID = Deputy.ID;
            lastID = Chief.ID;
        }
        else
        {
            curID = Chief.ID;
            lastID = Deputy.ID;
        }
        MainPlayer curActor = Lookup(curID) as MainPlayer;
        if (curActor.IsDead)
        {//要切换的角色已经死亡
            return false;
        }
        MainPlayer lastActor = Lookup(lastID) as MainPlayer;

        // 小地图的点管理 把原来的角色移除 
        Map.Singleton.RomvePoint(lastActor);
        

        //退场角色的信息 begin
        Vector3 lastPos = lastActor.MainPos;
        Vector3 lastForward = lastActor.MainObj.transform.forward;
        Vector3 targetPos = Vector3.zero;
        if (!isAttacking)
        {
            if (lastActor.ActionControl.IsActionRunning(ActorAction.ENType.enMoveAction))
            {//move action
                MoveAction action = lastActor.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
                if (action.mRealSpeed / action.m_currentSpeed <= 0.1f)
                {
                }
                else
                {
                    targetPos = action.m_realTargetPos;
                }
            }
        }
        int targetID = 0;
        if (lastActor.TargetManager.CurrentTarget != null)
        {
            targetID = lastActor.TargetManager.CurrentTarget.ID;
        }
        //退场角色的信息 end
        ActorExitAction exitAction = lastActor.ActionControl.AddAction(ActorAction.ENType.enActorExitAction) as ActorExitAction;
        if (exitAction == null)
        {//退场失败，不能切换
            return false;
        }
        lastActor.ActorExit();

        //开始CD
        m_switchCDTimer = Time.time;
        m_switchCDTotal = (curActor.CurrentTableInfo.SwitchCD + lastActor.CurrentTableInfo.SwitchCD) / 2;
        m_isSwitchForDead = isSwitchForDead;

        //当前角色上场
        if (lastActor.IsDead)
        {
            float percent = (float)m_switchCDTotal / ((curActor.CurrentTableInfo.SwitchCD + lastActor.CurrentTableInfo.SwitchCD) / 2);
            float hp = (float)curActor.HP;
            if (percent != 0)
            {
                hp *= percent;
                curActor.SetCurHP((int)hp);
            }
        }
        curActor.ActorEnter(lastPos, lastForward, targetID, targetPos, isAttacking);

        // 小地图新进入的角色 加入
        Map.Singleton.AddPoint(curActor);
        return true;
    }
    //修改托管npc
    public void ModifyDepositNpc(int id)
    {
        foreach (var item in m_actorMap.Values)
        {
            if (item.Type == ActorType.enNPC)
            {
                if (id == item.ID)
                {//托管
                    //Debug.LogWarning("deposited npc id:" + item.ID);
                    item.m_isDeposited = true;
                    if (item.SelfAI.GetType() != typeof(AINpcLong))
                    {
                        item.SelfAI = new AINpcLong();
                        item.SelfAI.Owner = item;
                    }
                }
                //else
                //{
                //    item.m_isDeposited = false;
                //    if (item.SelfAI.GetType() != typeof(AIServerActor))
                //    {
                //        item.SelfAI = new AIServerActor();
                //        item.SelfAI.Owner = item;
                //    }
                //}
            }
        }
    }
    //修改actor的id
    public void ModifyActorID(int oldID, int newID)
    {
        Actor actor = null;
        m_actorMap.TryGetValue(oldID, out actor);
        if (actor != null)
        {
            m_actorMap.Remove(oldID);
            m_actorMap.Add(newID, actor);
        }
		m_delayReleaseActorDict.TryGetValue(oldID, out actor);
		if (actor != null)
		{
			m_delayReleaseActorDict.Remove(oldID);
			m_delayReleaseActorDict.Add(newID, actor);
		}
    }
	public void RefreshAllName()
	{
		foreach (var item in m_actorMap)
		{
			item.Value.InitName();
		}
	}
}