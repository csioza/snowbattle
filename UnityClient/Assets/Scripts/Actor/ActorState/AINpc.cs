using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AINpc : AIBase
{
    public class TargetInfo
    {
        public int m_targetID = 0;
        public bool m_isRemove = false;
        public TargetInfo(int targetID)
        {
            m_targetID = targetID;
        }
    }
    private List<TargetInfo> m_alertTargetList = new List<TargetInfo>();
    //下一次ai的间隔
    protected float m_nextAITime = 0.5f;
    //当前巡逻位置
    int m_patrolIndex = 0;
    protected NPC Self { get { return Owner as NPC; } }
    //是否反隐形
    public bool IsFindSneak = false;

	public bool IsFinishPathList = false;
	public Vector3 m_curPathNode;
    public virtual void BaseUpdate()
    {
        base.Update();
    }
	public override void Update ()
    {
        base.Update();
        m_nextAITime -= Time.deltaTime;
        if (m_nextAITime > 0.0f)
        {
            return;
        }
        m_nextAITime = UnityEngine.Random.Range(0.1f, 0.5f);
        if (Self.IsDead)
        {//死亡
            return;
        }
        
        FindSneak();
        if (IsClearTargetWithRange())
        {//清除目标列表
            Self.TargetManager.ClearTarget();
        }
        else if (!Owner.m_isDeposited && IsGoHome())
        {//无敌返回
        }
        else if (IsHealth())
        {//治疗
        }
        else if (IsCancelAlert())
        {//取消警戒
            Self.ActionControl.RemoveAction(ActorAction.ENType.enAlertAction);
        }
        else if (GetDamagedTarget())
        {//获得伤害我的目标
            ;
        }
        else if (IsAttack())
        {//战斗
			IsCalled = false;
            LoadPreCD();

            Self.ShowBossBloodBar();

            Self.AttackRangeChangeType = NPC.ENARChangeType.enLarge;
            m_patrolIndex = 0;
            //警戒
            AlertAction action = Self.ActionControl.LookupAction(ActorAction.ENType.enAlertAction) as AlertAction;
            if (action != null)
            {//播放开始战斗前动画
                action.Refresh();
                return;
            }
            //开始战斗
            GetRangeTargetList(ENTargetType.enEnemy);
            //添加警戒目标
            for (int i = 0; i < m_alertTargetList.Count; ++i)
            {
                int targetID = m_alertTargetList[i].m_targetID;
                if (!m_targetIDList.Contains(targetID))
                {
                    m_targetIDList.Add(targetID);
                }
            }
            //添加当前目标
            if (Self.TargetManager.CurrentTarget != null && !Self.TargetManager.CurrentTarget.IsDead)
            {
                m_curTargetID = Self.TargetManager.CurrentTarget.ID;
                if (!m_targetIDList.Contains(m_curTargetID))
                {
                    m_targetIDList.Add(m_curTargetID);
                }
            }
            int skillID = 0;
            if (Self.CurrentCmd != null)
            {//攻击命令，直接释放
                skillID = Self.CurrentCmd.m_skillID;
                if (Self.CurrentCmd.m_targetID != 0)
                {
                    m_curTargetID = Self.CurrentCmd.m_targetID;
                }
            }
            if (ActionTryFireSkill(skillID))
            {
                Self.CurrentCmd = null;
                if (skillID == Self.CurrentTableInfo.StaminaSkillID)
                {//触怒技
                    IResult r = BattleFactory.Singleton.CreateResult(ENResult.StaminaChanged, Self.ID, Self.ID);
                    if (r != null)
                    {
                        r.ResultExpr(null);
                        BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
                    }
                }
                Actor target = ActorManager.Singleton.Lookup(m_curTargetID);
                if (target != null && !target.IsDead)
                {
                    Self.TargetManager.CurrentTarget = target;
                }
                Self.CurrentCmd = null;
            }
            else
            {
                ActionForwardTo(m_curTargetID);
            }
            //呼叫同伴
            GetRangeTargetList(ENTargetType.enFriendly, Self.CurrentTableInfo.CallNpcRange);
            Actor curTarget = ActorManager.Singleton.Lookup(m_curTargetID);
            for (int i = 0; i < m_targetIDList.Count; ++i)
            {
                Actor friend = ActorManager.Singleton.Lookup(m_targetIDList[i]);
                if (friend.Type == Owner.Type)
                {//只呼叫类型相同的友方
                    friend.SelfAI.IsCalled = true;
                    if (friend.TargetManager.CurrentTarget == null || friend.TargetManager.CurrentTarget.IsDead)
                    {
                        friend.TargetManager.CurrentTarget = curTarget;
                    }
                }
            }
        }
        else if (IsAlert())
        {//警戒
            Self.ShowBossBloodBar();

            AlertAction action = Self.ActionControl.LookupAction(ActorAction.ENType.enAlertAction) as AlertAction;
            if (action == null)
            {
                action = Self.ActionControl.AddAction(ActorAction.ENType.enAlertAction) as AlertAction;
                //action.Init(Self.CurrentTableInfo.AlertPeriod);
            }
        }
        else
        {//空闲
            if (Self.PatrolList.Count != 0)
            {//巡逻
                Vector3 targetPos = Self.PatrolList[m_patrolIndex];
                Vector3 d = targetPos - Self.RealPos;
                d.y = 0;
                if (d.magnitude <= 0.4f)
                {
                    ++m_patrolIndex;
                    if (m_patrolIndex >= Self.PatrolList.Count)
                    {
                        m_patrolIndex = 0;
                    }
                }
                else
                {
                    ActionMoveTo(targetPos);
                }
            }
            else
            {
                if (!Owner.m_isDeposited)
                {
                    Vector3 d = Self.RealPos - Self.m_startAttackPos;
                    d.y = 0;
                    if (d.magnitude <= 0.4f)
                    {
                    }
                    else
                    {//返回出生点
                        ActionMoveTo(Self.m_startAttackPos);
                    }
                }
                else
                {
                    if (!IsFinishPathList)
                    {
                        IsFinishPathList = true;
                        int pathIndex = Self.Props.GetProperty_Int32(ENProperty.camp);
                        if (SM.RandomRoomLevel.Singleton.m_scenePathNodeDic.Count != 0)
						{
                            if (SM.RandomRoomLevel.Singleton.m_scenePathNodeDic[pathIndex].Count != 0)
							{
                                Self.m_pathNodeList = new List<Vector3>(SM.RandomRoomLevel.Singleton.m_scenePathNodeDic[pathIndex].ToArray());
							}
						}
						
                        if (Self.m_pathNodeList.Count > 0)
                        {
                            Vector3 tmpPos = Self.m_pathNodeList[0];
                            m_curPathNode = tmpPos;
                            Self.m_pathNodeList.Remove(tmpPos);
                        }
                    }
                    if ((Self.MainPos - m_curPathNode).magnitude < 0.5)
                    {
                        if (Self.m_pathNodeList.Count > 0)
                        {
                            Vector3 tmpPos = Self.m_pathNodeList[0];
                            m_curPathNode = tmpPos;
                            Self.m_pathNodeList.Remove(tmpPos);
                        }
                    }
                    ActionMoveTo(m_curPathNode);
                }
            }
        }
	}
    #region DamagedCounterParam //受伤反击参数
    float m_damagedCounterParam = 0;
    float DamagedCounterParam
    {
        get
        {
            if (m_damagedCounterParam == 0)
            {
                m_damagedCounterParam = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enDamagedCounterParam).FloatTypeValue;
            }
            return m_damagedCounterParam;
        }
    }
    #endregion
    //获得伤害我的目标
    bool GetDamagedTarget()
    {
        float param = Self.MaxHP * DamagedCounterParam;
        foreach (var item in Self.m_damagedRecordMap)
        {
            if (item.Value > param)
            {
                Actor target = ActorManager.Singleton.Lookup(item.Key);
                if (target == null || target.IsDead)
                {
                    continue;
                }
                m_curTargetID = target.ID;
                Self.CurrentTarget = target;
                //清除
                Self.m_damagedRecordMap[item.Key] = 0;
                return true;
            }
        }
        return false;
    }
    //加载预CD
    protected void LoadPreCD()
    {
        if (m_isLoadPreCD) return;
        m_isLoadPreCD = true;
        foreach (var item in Owner.SkillBag)
        {
            Owner.SkillControl.AddSkillPreCD(item.SkillTableInfo.ID, Owner);
        }
    }
    //是否警戒
    private bool IsAlert()
    {
        GetRangeTargetList(ENTargetType.enEnemy, Self.CurrentTableInfo.AlertRange);
        for (int i = 0; i < m_targetIDList.Count; ++i)
        {
            int targetID = m_targetIDList[i];
            if (!m_alertTargetList.Exists(item => item.m_targetID == targetID))
            {
                TargetInfo info = new TargetInfo(targetID);
                m_alertTargetList.Add(info);
            }
        }
        //清除警戒列表中的已死、不存在的目标
        for (int j = 0; j < m_alertTargetList.Count; ++j)
        {
            TargetInfo info = m_alertTargetList[j];
            Actor target = ActorManager.Singleton.Lookup(info.m_targetID);
            if (target == null || target.IsDead)
            {
                info.m_isRemove = true;
            }
        }
        m_alertTargetList.RemoveAll(item => item.m_isRemove);
        if (m_alertTargetList.Count == 0)
        {
            Self.AttackRangeChangeType = NPC.ENARChangeType.enSmall;
        }
        else
        {
            Self.AttackRangeChangeType = NPC.ENARChangeType.enLarge;
        }
        return m_alertTargetList.Count != 0;
    }
    //是否攻击
    private bool IsAttack()
    {
        Actor target = ActorManager.Singleton.Lookup(m_curTargetID);
        if (target != null && !target.IsDead)
        {//有目标
            MainPlayer mainActor = target as MainPlayer;
            if (mainActor != null && mainActor.IsActorExit)
            {
                m_curTargetID = 0;
            }
            else
            {
                return true;
            }
        }
        if (IsCalled)
        {//被同伴呼叫
            return true;
        }
        GetRangeTargetList(ENTargetType.enEnemy);
        if (Self.AttackRange > m_minDistance)
        {//进入攻击范围
            m_curTargetID = m_minActor.ID;
            return true;
        }
        if (Self.DamageSource != null && !Self.DamageSource.IsDead)
        {//受到伤害
            if (ActorTargetManager.IsEnemy(Self, Self.DamageSource))
            {//伤害来源是敌人
                m_curTargetID = Self.DamageSource.ID;
                return true;
            }
        }
        //AlertAction action = Self.ActionControl.LookupAction(ActorAction.ENType.enAlertAction) as AlertAction;
        //if (action != null && action.IsTimeout)
        //{//敌人在警戒范围内停留超过警戒时间
        //    float minDistance = float.MaxValue;
        //    for (int i = 0; i < m_alertTargetList.Count; ++i)
        //    {
        //        int targetID = m_alertTargetList[i].m_targetID;
        //        target = ActorManager.Singleton.Lookup(targetID);
        //        if (target != null && !target.IsDead)
        //        {
        //            Vector3 d = target.RealPos - Self.RealPos;
        //            if (Mathf.Abs(d.magnitude) < minDistance)
        //            {
        //                m_curTargetID = targetID;
        //            }
        //        }
        //    }
        //    return true;
        //}
        return false;
    }
    //是否取消警戒
    private bool IsCancelAlert()
    {
        AlertAction action = Self.ActionControl.LookupAction(ActorAction.ENType.enAlertAction) as AlertAction;
        if (action != null)
        {
            for (int i = 0; i < m_alertTargetList.Count; ++i)
            {
                TargetInfo info = m_alertTargetList[i];
                Actor target = ActorManager.Singleton.Lookup(info.m_targetID);
                if (target == null || target.IsDead)
                {
                    info.m_isRemove = true;
                    continue;
                }
                Vector3 d = target.RealPos - Self.RealPos;
                d.y = 0;
                if (Mathf.Abs(d.magnitude) > Self.CurrentTableInfo.CancelAlertRange)
                {//超过“取消警戒的范围”
                    info.m_isRemove = true;
                    continue;
                }
            }
            m_alertTargetList.RemoveAll(item => item.m_isRemove);
            if (m_alertTargetList.Count == 0)
            {
                return true;
            }
        }
        return false;
    }
    //是否治疗
    protected bool IsHealth()
    {
        if (IsHealthSelf() || IsHealthFriendly())
        {
            return true;
        }
        return false;
    }
    //检查自己与目标的距离是否超过设定距离
    private bool IsClearTargetWithRange()
    {
        Actor target = ActorManager.Singleton.Lookup(m_curTargetID);
        if (target != null)
        {
            Vector3 visionRange = Self.RealPos - target.RealPos;
            visionRange.y = 0;
            if (visionRange.magnitude >= Self.CurrentTableInfo.VisionRange)
            {//小于追击间距
                m_isGoHome = true;
                return true;
            }
        }
        return false;
    }
    //无敌返回
    bool m_isGoHome = false;
    //是否返回出生点
    private bool IsGoHome()
    {
        if (!m_isGoHome)
        {
			m_isGoHome = IsClearTargetWithRange();
			Vector3 chaseRange = Self.RealPos - Self.m_startAttackPos;
			chaseRange.y = 0;
			if (chaseRange.magnitude >= Self.CurrentTableInfo.MaxChaseRange)
			{//小于追击距离
				m_isGoHome = true;
			}
        }
        if (m_isGoHome)
        {
            //清除命令
            Self.CurrentCmd = null;
            Vector3 d = Self.RealPos - Self.m_startAttackPos;
            d.y = 0;
            if (d.magnitude <= 0.1f)
            {
                m_isGoHome = false;
            }
            else
            {//返回出生点
                ActionMoveTo(Self.m_startAttackPos);
            }
        }
        return m_isGoHome;
    }
    protected void FindSneak()
    {
        if (IsFindSneak)
        {
            ActorManager.Singleton.ForEach(CheckSneak);
        }
    }
    void CheckSneak(Actor target)
    {
        if (target == null || target.IsDead)
        {
            return;
        }
        if (target.TempType == ActorType.enPlayer_Sneak)
        {//潜行中
            float distance = ActorTargetManager.GetTargetDistance(Self.RealPos, target.RealPos);
            if (Self.CurrentTableInfo.AlertRange > distance)
            {//警戒范围内
                target.StopSneak();
            }
        }
    }
}
