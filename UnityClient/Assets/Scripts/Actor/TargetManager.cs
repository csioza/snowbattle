//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Actor
//	created:	2013-6-5
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActorTargetManager
{
    public ActorTargetManager(Actor self)
    {
        Owner = self;
    }
    Actor Owner { get; set; }
    #region TargetList
    public class ActorValue
	{
		public Actor 	m_target;
		public int		m_value;//0为未切换到的目标，1为切换过的目标
		public ActorValue(Actor target, int value)
		{
			m_target = target;
			m_value = value;
		}
	}
	private List<ActorValue> m_targetList = null;
	public List<ActorValue> TargetList
    {
        get
        {
            if (m_targetList == null)
            {
                m_targetList = new List<ActorValue>();
            }
            return m_targetList;
        }
    }
    #endregion
    //是否显示目标脚底特效
    public bool m_showSelectedMark;
    //目标脚底的特效
    GameObject m_selectedParticle;
    //开始获得目标的时间
    public float m_getTagetTimer = 0;
	public void AddTarget(Actor target)
	{
        ActorValue actorValue = TargetList.Find(item => item.m_target == target);
        if (actorValue == null)
		{
            TargetList.Add(new ActorValue(target, 0));
		}
	}
	public void RemoveTarget(Actor target)
	{
        TargetList.RemoveAll(item => item.m_target == target);
	}
	public void ClearTarget()
	{
        if (null != CurrentTarget)
        {
            CurrentTarget = null;
        }
        TargetList.Clear();
        if (m_selectedParticle != null)
        {
            //GameObject.Destroy(m_selectedParticle);
            PoolManager.Singleton.ReleaseObj(m_selectedParticle);
            m_selectedParticle = null;
        }
	}
    //修改目标列表、不修改其中的target
	public void ModifyTargetListValue(int value, Actor target)
	{
        ActorValue actorValue = TargetList.Find(item => item.m_target == target);
        if (actorValue != null)
        {
            actorValue.m_value = value;
        }
	}
	//将除lastActor之外的所有Value清零
	public void ClearTargetListValue(Actor lastActor)
	{
        foreach (var item in TargetList)
        {
            if (item.m_target == lastActor && TargetList.Count != 1)
            {
                continue;
            }
            item.m_value = 0;
        }
	}
	public int GetTargetListValue(Actor target)
	{
        ActorValue actorValue = TargetList.Find(item => item.m_target == target);
        if (actorValue != null)
        {
            return actorValue.m_value;
        }
		return -1;
	}
    public void OnActorRelease(int actorid)
    {
        if (CurrentTarget != null && CurrentTarget.ID == actorid)
        {
            TargetList.RemoveAll(item => item.m_target == CurrentTarget);
            CurrentTarget = null;
        }
    }
    Actor m_currentTarget;
    //只提供给主控角色的接口
    public void SetCurrentTarget(Actor actor)
    {
        if (null != m_selectedParticle)
        {
            //GameObject.Destroy(m_selectedParticle);
            PoolManager.Singleton.ReleaseObj(m_selectedParticle);
            m_selectedParticle = null;
        }
        if (m_currentTarget != actor)
        {//new target
            if (m_currentTarget != null)
            {//老目标被标记为目标的数量减1
                --m_currentTarget.BetargetedNumber;
            }
            if (actor != null)
            {//新目标被标记为目标的数量加1
                ++actor.BetargetedNumber;
            }
        }
        m_currentTarget = actor;
        if (m_currentTarget != null)
        {
            m_getTagetTimer = Time.time;
            if (m_selectedParticle == null)
            {
                MainGame.Singleton.StartCoroutine(Coroutine_LoadEffectObj());
            }
            else
            {
                SetEffectObjPos();
            }
        }
        else
        {//清除ai中的当前目标
            Owner.SelfAI.m_curTargetID = 0;
        }
        ActorManager.Singleton.MainActor.NotifyChangedButton(actor);
    }
    public Actor CurrentTarget
    {
        get
        {
            return m_currentTarget;
        }
        set
        {
            if (m_currentTarget != value)
            {//new target
                if (m_currentTarget != null)
                {//老目标被标记为目标的数量减1
                    --m_currentTarget.BetargetedNumber;
                }
                if (value != null)
                {//新目标被标记为目标的数量加1
                    ++value.BetargetedNumber;
                }
            }
            if (null != m_currentTarget)
            {
                if (m_currentTarget == value)
                {
                    return;
                }
            }
            if (null != m_selectedParticle)
            {
                //GameObject.Destroy(m_selectedParticle);
                PoolManager.Singleton.ReleaseObj(m_selectedParticle);
                m_selectedParticle = null;
            }
            m_currentTarget = value;
            if (m_currentTarget != null)
            {//被标记为目标的数量加1
                //++m_currentTarget.BetargetedNumber;
                m_getTagetTimer = Time.time;
            }
            else
            {//清除ai中的当前目标
                Owner.SelfAI.m_curTargetID = 0;
            }
            if (m_showSelectedMark)
            {//只有主控角色的目标才设置目标特效
                ActorManager.Singleton.MainActor.NotifyChangedButton(value);
                if (m_currentTarget != null)
                {
                    if (m_selectedParticle == null)
                    {
                        MainGame.Singleton.StartCoroutine(Coroutine_LoadEffectObj());
                    }
                    else
                    {
                        SetEffectObjPos();
                    }
                }
                else
                {
                    if (m_selectedParticle != null)
                    {
                        //GameObject.Destroy(m_selectedParticle);
                        PoolManager.Singleton.ReleaseObj(m_selectedParticle);
                        m_selectedParticle = null;
                    }
                }
            }
        }
    }
    IEnumerator Coroutine_LoadEffectObj()
    {
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath("ef-e-choosetarget-E01"), data);
        while (true)
        {
            e.MoveNext();
            if (data.m_isFinish)
            {
                break;
            }
            yield return e.Current;
        }
        if (data.m_obj != null)
        {
            m_selectedParticle = data.m_obj as GameObject;
            SetEffectObjPos();
        }
    }
    void SetEffectObjPos()
    {
        if (m_selectedParticle != null)
        {
            Transform t = m_currentTarget.GetObject_AdherentPoints().transform.Find("zeroPoint");
            if (t == null)
            {
                Debug.LogError("not zeroPoint, actor id:" + m_currentTarget.ID);
                t = m_currentTarget.GetObject_AdherentPoints().transform;
            }
            m_selectedParticle.transform.parent = t;
            m_selectedParticle.transform.localPosition = Vector3.zero;
            m_selectedParticle.transform.rotation = new Quaternion(0, 0, 0, 0);
            m_selectedParticle.transform.localScale = Vector3.one;
            m_selectedParticle.SetActive(true);
        }
    }
    //设置选中特效的隐藏、显示m_selectedParticle
    public void SetSelectedParticleActive(bool isShow)
    {
        if (null != m_selectedParticle)
        {
            m_selectedParticle.SetActive(isShow);
        }
    }

	static public bool IsEnemy(Actor self, Actor target)
    {
        if (self == target)
        {
            return false;
        }
        ENCamp selfCamp = (ENCamp)self.Camp;
        if (selfCamp != self.TempCamp)
        {
            selfCamp = self.TempCamp;
        }
        ActorType selfType = self.Type;
        if (selfType != self.TempType)
        {
            selfType = self.TempType;
        }
        return ActorTargetManager.IsEnemy(selfCamp, selfType, target);
	}
    static public bool IsFriendly(Actor self, Actor target)
    {
        if (self == target)
        {
            return false;
        }
        ENCamp selfCamp = (ENCamp)self.Camp;
        if (selfCamp != self.TempCamp)
        {
            selfCamp = self.TempCamp;
        }
        ActorType selfType = self.Type;
        if (selfType != self.TempType)
        {
            selfType = self.TempType;
        }
        return ActorTargetManager.IsFriendly(selfCamp, selfType, target);
    }
    static public bool IsInteractionally(Actor self, Actor target)
    {
        if (self == target)
        {
            return false;
        }
        if (self.Type != ActorType.enMain)
        {//自己不是主控角色，不能打开互动物件
            return false;
        }
        if (target.Type != ActorType.enNPC)
        {//目标不是npc，不能是互动物件
            return false;
        }
        else
        {
            NPC npc = target as NPC;
            if (npc.GetNpcType() != ENNpcType.enBoxNPC)
            {//目标不是互动物件
                return false;
            }
        }
        return true;
    }
    static public bool IsEnemy(ENCamp selfCamp, ActorType selfType, Actor target)
    {
        if (selfCamp != ENCamp.enNone)
        {
            ENCamp targetCamp = (ENCamp)target.TempCamp;
            switch (selfCamp)
            {
                case ENCamp.enRed:
                    {
                        if (targetCamp == ENCamp.enRed ||
                            targetCamp == ENCamp.enNeutralFriend)
                        {
                            return false;
                        }
                    }
                    break;
                case ENCamp.enBlue:
                    {
                        if (targetCamp == ENCamp.enBlue ||
                            targetCamp == ENCamp.enNeutralFriend)
                        {
                            return false;
                        }
                    }
                    break;
                case ENCamp.enYellow:
                    {
                        if (targetCamp == ENCamp.enYellow ||
                            targetCamp == ENCamp.enNeutralFriend)
                        {
                            return false;
                        }
                    }
                    break;
                case ENCamp.enNeutralHostile:
                    {
                        return true;
                    }
                    //break;
                case ENCamp.enNeutralFriend:
                    {
                        return false;
                    }
                    //break;
            }
            return true;
        }
        switch (selfType)
        {
            case ActorType.enMain:
            case ActorType.enSwitch:
            case ActorType.enPlayer:
            case ActorType.enFollow:
            case ActorType.enPlayer_Sneak:
                if (target.Type == ActorType.enNPC)
                {//npc
                    NPC npc = target as NPC;
                    ENNpcType npcType = npc.GetNpcType();
                    if (npcType == ENNpcType.enFunctionNPC ||
                        npcType == ENNpcType.enBlockNPC ||
                        npcType == ENNpcType.enBoxNPC) //todo,区别NPC类型,2暂定为不能被攻击的
                    {
                    }
                    else
                    {
                        return true;
                    }
                }
                if (target.Type == ActorType.enNPCTrap)
                {//机关
                    return false;
                }
                break;
            case ActorType.enNPC_Friend:
                {
                    if (target.Type == ActorType.enNPC)
                    {
                        NPC npc = target as NPC;
                        ENNpcType npcType = npc.GetNpcType();
                        if (npcType == ENNpcType.enFunctionNPC ||
                            npcType == ENNpcType.enBlockNPC ||
                            npcType == ENNpcType.enBoxNPC) //todo,区别NPC类型,2暂定为不能被攻击的
                        {
                        }
                        else
                        {
                            if (target.TempType == ActorType.enNPC_Friend)
                            {//友方npc不能互相攻击
                                ;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
                break;
            case ActorType.enNPC:
                if (ActorType.enMain == target.Type || ActorType.enFollow == target.Type)
                {
                    if (target.TempType == ActorType.enPlayer_Sneak)
                    {
                        ;
                    }
                    else
                    {
                        return true;
                    }
                }
                break;
            case ActorType.enNPC_AllEnemy:
                if (target.Type == ActorType.enNPC)
                {
                    NPC npc = target as NPC;
                    ENNpcType npcType = npc.GetNpcType();
                    if (npcType == ENNpcType.enFunctionNPC ||
                        npcType == ENNpcType.enBlockNPC ||
                        npcType == ENNpcType.enBoxNPC) //todo,区别NPC类型,2暂定为不能被攻击的
                    {
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (target.Type == ActorType.enNPCTrap)
                {
                    return false;
                }
                else
                {
                    return true;
                }
                break;
            case ActorType.enNPCTrap:
                {
                    return true;
                }
//                break;
            default:
                break;
        }
        return false;
    }
    static public bool IsFriendly(ENCamp selfCamp, ActorType selfType, Actor target)
    {
        if (selfCamp != ENCamp.enNone)
        {
            ENCamp targetCamp = (ENCamp)target.TempCamp;
            switch (selfCamp)
            {
                case ENCamp.enRed:
                    {
                        if (targetCamp == ENCamp.enRed ||
                            targetCamp == ENCamp.enNeutralFriend)
                        {
                            return true;
                        }
                    }
                    break;
                case ENCamp.enBlue:
                    {
                        if (targetCamp == ENCamp.enBlue ||
                            targetCamp == ENCamp.enNeutralFriend)
                        {
                            return true;
                        }
                    }
                    break;
                case ENCamp.enYellow:
                    {
                        if (targetCamp == ENCamp.enYellow ||
                            targetCamp == ENCamp.enNeutralFriend)
                        {
                            return true;
                        }
                    }
                    break;
                case ENCamp.enNeutralHostile:
                    {
                        return false;
                    }
//                    break;
                case ENCamp.enNeutralFriend:
                    {
                        return true;
                    }
//                    break;
            }
            return false;
        }
        switch (selfType)
        {
            case ActorType.enMain:
            case ActorType.enSwitch:
            case ActorType.enPlayer:
            case ActorType.enFollow:
            case ActorType.enNPC_Friend:
            case ActorType.enPlayer_Sneak:
                if (target.Type == ActorType.enNPC)
                {
                    NPC npc = target as NPC;
                    if (npc.GetNpcType() == ENNpcType.enBoxNPC) //todo,区别NPC类型,2暂定为不能被攻击的
                    {
                        return true;
                    }
                    if (target.TempType == ActorType.enNPC_Friend)
                    {//友方npc
                        return true;
                    }
                }
                else if (ActorType.enMold == target.Type)
                {
                    return false;
                }
                else
                {
                    return true;
                }
                break;
            case ActorType.enNPC:
                {
                    if (target.Type == ActorType.enNPC)
                    {
                        NPC npc = target as NPC;
                        ENNpcType type = npc.GetNpcType();
                        if (type == ENNpcType.enCommonNPC || type == ENNpcType.enBOSSNPC || type == ENNpcType.enEliteNPC
                            || type == ENNpcType.enMobaNpc || type == ENNpcType.enMobaTower)
                        {
                            return true;
                        }
                    }
                }
                break;
            case ActorType.enNPC_AllEnemy:
                break;
            default:
                break;
        }
        return false;
    }
    //获得两个目标之间的距离
    static public UnityEngine.AI.NavMeshPath m_targetPath = new UnityEngine.AI.NavMeshPath();
    static public float GetTargetDistance(Vector3 selfPos, Vector3 targetPos)
    {
        if (targetPos == selfPos)
        {//是自己
            return 0;
        }
        float distance = 0;
        if (UnityEngine.AI.NavMesh.CalculatePath(selfPos, targetPos, -1, ActorTargetManager.m_targetPath))
        {
            int count = ActorTargetManager.m_targetPath.corners.Length;
            for (int i = 0; i < count; ++i)
            {
                if (i + 1 < count)
                {
                    Vector3 d = ActorTargetManager.m_targetPath.corners[i + 1] - ActorTargetManager.m_targetPath.corners[i];
                    d.y = 0.0f;
                    distance += d.magnitude;
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            Vector3 targetDirection = targetPos - selfPos;
            targetDirection.y = 0.0f;
            distance = targetDirection.magnitude;
        }
        return (distance == 0.0f ? float.MaxValue : distance);
    }
    //trigger是否可以生效
    static public bool IsTrigger(Collider other)
    {
        if (other.isTrigger)
        {
            return false;
        }
        return true;
    }
};