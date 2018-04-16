using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPlayer : AIBase
{
    MainPlayer Self { get { return Owner as MainPlayer; } }
    #region AutoAttack
    //是否自动攻击
    bool m_isAutoAttack = false;
    //自动攻击的间隔
    float m_autoAttackInterval = 0;
    #endregion
    //最后一次位移动作的时间
    float m_lastActionTime = 0;
    //是否正在进行攻击
    public bool m_isAttacking = false;
    //是否可以反击
    bool m_isCounterattack = false;
    //反击技能的攻击范围
    float m_counterSkillRange = 0;
    public void BaseUpdate()
    {
        base.Update();
        if (Self.IsActorExit) return;//当前角色离场
        ReliveUI();
        #region Unlock
        if (!Self.CurrentTargetIsDead)
        {//判断距离 清除目标
            float distance = ActorTargetManager.GetTargetDistance(Self.MainPos, Self.CurrentTarget.MainPos);
            if (distance > Self.CurrentTableInfo.UnlockRange)
            {
                Self.CurrentTarget = null;
                Self.DamageSource = null;
            }
        }
        #endregion
        CheckAllSkill();
    }
    void ReliveUI()
    {
        #region 复活ui
        DeadAction dead = Owner.ActionControl.LookupAction(ActorAction.ENType.enDeadAction) as DeadAction;
        if (dead != null)
        {
            if (dead.IsPlayOver && !Self.m_isDeadNotify)
            {//死亡动画已播放完毕
                if (ActorManager.Singleton.Chief.IsDead &&
                    (ActorManager.Singleton.Deputy == null || ActorManager.Singleton.Deputy.IsDead))
                {//主、副角色都死亡
					if (Owner.m_isDeposited)
					{
						Owner.NotifyChanged((int)Actor.ENPropertyChanged.enCountDown, null);
					}
					else
					{
                        Owner.NotifyChanged((int)Actor.ENPropertyChanged.enRelive, null);
					}
					
                }
                else
                {
                    //是否攻击
                    bool isAttacking = false;
                    if (Self.CurrentCmd != null)
                    {
                        if (Self.CurrentCmd.m_type == Player.ENCmdType.enLoopNormalAttack || Self.CurrentCmd.m_type == Player.ENCmdType.enSkill)
                        {
                            isAttacking = true;
                        }
                    }
                    ActorManager.Singleton.SwitchMainActor(true, isAttacking);
                }
                Self.m_isDeadNotify = true;
            }
            return;
        }
        else
        {
            Self.m_isDeadNotify = false;
        }
        #endregion
    }
    public override void Update()
    {
        base.Update();
        if (Self.IsActorExit) return;//当前角色离场
		//TimeToRelive();
        ReliveUI();
        #region Unlock
        if (!Self.CurrentTargetIsDead)
        {//判断距离 清除目标
            float distance = ActorTargetManager.GetTargetDistance(Self.MainPos, Self.CurrentTarget.MainPos);
            if (distance > Self.CurrentTableInfo.UnlockRange)
            {
                Self.CurrentTarget = null;
                Self.DamageSource = null;
            }
        }
        #endregion
        CheckAllSkill();//检测可释放技能
        if (Self.CurrentCmd == null)
        {//当前没有命令
            MoveAction action = Self.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
            if ((action == null || action.IsStopMove) &&
                !Self.ActionControl.IsActionRunning(ActorAction.ENType.enAttackAction) &&
                !Self.ActionControl.IsActionRunning(ActorAction.ENType.enRollAction))
            {//不在移动、攻击、翻滚等位移动作中
                if (!Self.CurrentTargetIsDead)
                {//当前目标没死
                    float distance = ActorTargetManager.GetTargetDistance(Self.RealPos, Self.CurrentTarget.RealPos);
                    if (Self.CurrentTableInfo.AutoAttackRange > distance)
                    {//当前目标在自动攻击范围内
                        //Self.FireNormalSkill();
                        return;
                    }
                }
                else
                {
                    if (m_isAutoAttack)
                    {
                        m_autoAttackInterval -= Time.deltaTime;
                        if (m_autoAttackInterval < 0)
                        {
                            GetRangeTargetList(ENTargetType.enEnemy, Self.CurrentTableInfo.AutoAttackRange);
                            if (m_targetIDList.Count > 0)
                            {
                                Self.TargetManager.CurrentTarget = m_minActor;
                                Self.CurrentCmd = new Player.Cmd(Player.ENCmdType.enLoopNormalAttack);
                                Self.CurrentCmd.m_skillID = Self.NormalSkillList[0];
                                return;
                            }
                        }
                    }
                }
                //自动反击
                if (m_lastActionTime == 0)
                {
                    m_lastActionTime = Time.time;
                }
                if (!m_isCounterattack && Time.time - m_lastActionTime > GameSettings.Singleton.m_autoCounterattack)
                {//距离最后一次位移动作 已超过3秒 可以反击
                    m_isCounterattack = false;
                }
            }
            if (m_isCounterattack)
            {//可以反击
                if (Self.DamageTime > m_lastActionTime + GameSettings.Singleton.m_autoCounterattack)
                {
                    if (Self.DamageSource != null && !Self.DamageSource.IsDead && ActorTargetManager.IsEnemy(Self, Self.DamageSource))
                    {
                        if (m_counterSkillRange == 0)
                        {
                            SkillInfo info = GameTable.SkillTableAsset.Lookup(Self.NormalSkillList[0]);
                            m_counterSkillRange = info.AttackDistance;
                        }
                        Vector3 distance = Owner.RealPos - Self.DamageSource.RealPos;
                        distance.y = 0;
                        if (distance.magnitude > m_counterSkillRange)
                        {//不在攻击范围内
                            //向技能目标移动
                            ActionMoveTo(Self.DamageSource.RealPos);
                        }
                        else
                        {//在攻击范围内
                            //释放技能
                            if (ActionTryAttack(Self.NormalSkillList[0], Self.DamageSource))
                            {
                                m_curTargetID = Self.DamageSource.ID;
                                Self.CurrentTarget = ActorManager.Singleton.Lookup(m_curTargetID);
                                m_lastActionTime = 0;
                                m_isCounterattack = false;
                                return;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            MainPlayer.Cmd cmd = Self.CurrentCmd;
            if (cmd.m_type != Player.ENCmdType.enSkill && cmd.m_type != Player.ENCmdType.enLoopNormalAttack)
            {//取消自动攻击
                m_isAutoAttack = false;
            }
            //最后一次动作的时间
            m_lastActionTime = 0;
            //清除自动反击
            m_isCounterattack = false;
            if (cmd.m_type != Player.ENCmdType.enSkill)
            {//取消技能高亮
                DelSkillHighlight();
            }

            m_autoAttackInterval = GameSettings.Singleton.m_autoAttackInterval;
            if (!IsCmdExecute())
            {//命令不允许执行
                Self.CurrentCmd = null;
                return;
            }

            m_curTargetID = 0;
            if (!Self.CurrentTargetIsDead)
            {
                m_curTargetID = Self.CurrentTarget.ID;
            }
            switch (cmd.m_type)
            {
                case Player.ENCmdType.enMove:
                    {
                        //  [8/3/2015 tgame]
                        if (cmd.m_isMoveByNoAStar )
                        {
                            if (ActionMoveToNotAStar(cmd.m_moveTargetPos))
                            {
                                //点击地面特效
                                Self.IsMoveAfterSwitch = false;
                                Self.CurrentCmd = null;
                            }
                        }
                        else
                        {
                            if (ActionMoveTo(cmd.m_moveTargetPos))
                            {
                                //点击地面特效
                                Self.IsMoveAfterSwitch = false;
                                Self.CurrentCmd = null;
                            }
                        }
                    }
                    break;
                case Player.ENCmdType.enStopMove:
                    {
                        MoveAction ac = Self.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
                        if (ac!=null)
                        {
                            Self.ActionControl.RemoveAction(ActorAction.ENType.enMoveAction);
                        }
                    }
                    break;
                case Player.ENCmdType.enRoll:
                    {
                        RollAction rollAction = Self.ActionControl.AddAction(ActorAction.ENType.enRollAction) as RollAction;
                        if (rollAction != null)
                        {
                            rollAction.Init(cmd.m_moveTargetPos);
                            Self.CurrentCmd = null;
                        }
                    }
                    break;
                case Player.ENCmdType.enSwitchActor:
                    {
                        if (!m_isAttacking)
                        {
                            if (Self.ActionControl.IsActionRunning(ActorAction.ENType.enAttackAction))
                            {
                                if (ActorManager.Singleton.m_switchCDTotal > 0)
                                {//cd中，继续攻击
                                    Self.FireNormalSkill();
                                    return;
                                }
                                m_isAttacking = true;
                            }
                        }
                        if (ActorManager.Singleton.SwitchMainActor(false, m_isAttacking))
                        {
                            Self.CurrentCmd = null;
                            m_isAttacking = false;
                        }
                    }
                    break;
                case Player.ENCmdType.enSkill:
                    {
                        int skillID = cmd.m_skillID;
                        Actor.ActorSkillInfo info = Self.SkillBag.Find(item => item.SkillTableInfo.ID == skillID);
                        if (IsFireSkill(skillID))
                        {
                            GetRangeTargetList((ENTargetType)info.SkillTableInfo.TargetType, Self.CurrentTableInfo.AttackRange, Self.m_firstTargetType);
                            if (ActionTryFireSkill(skillID))
                            {
                                m_isAutoAttack = false;
                                m_lastSkillID = skillID;
                                DelSkillHighlight();

                                Actor target = ActorManager.Singleton.Lookup(m_curTargetID);
                                if (target != null && ActorTargetManager.IsEnemy(Self, target))
                                {//只对仇人进行循环普攻
                                    Self.CurrentTarget = target;
                                    Self.CurrentCmd.m_skillID = Self.NormalSkillList[0];
                                    Self.CurrentCmd.m_type = Player.ENCmdType.enStop;//enLoopNormalAttack;
                                }
                                else
                                {
                                    Self.CurrentCmd = null;
                                }
                            }
                            else
                            {
                                if (m_targetIDList.Count == 0)
                                {
                                    DelSkillHighlight();
                                    Self.CurrentCmd = null;
                                }
                            }
                        }
                        else
                        {
                            GetRangeTargetList((ENTargetType)info.SkillTableInfo.TargetType, Self.CurrentTableInfo.AttackRange);
                            if (m_targetIDList.Count != 0 && skillID != m_highlightSkillID)
                            {//技能高亮的通知
                                AddSkillHighlight();
                                m_highlightSkillID = skillID;
                            }
                        }
                    }
                    break;
                case Player.ENCmdType.enLoopNormalAttack:
                    {
                        int skillID = Self.CurrentCmd.m_skillID;
                        if (Self.CurrentTargetIsDead)
                        {//循环攻击时 必须要有目标 目标不能死亡
                            Self.CurrentCmd = null;
                            return;
                        }
                        if (IsFireSkill(skillID))
                        {
                            if (ActionTryFireSkill(skillID))
                            {
                                m_lastSkillID = skillID;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
    //最后一次释放的技能ID
    protected int m_lastSkillID = 0;
    //是否可以释放该技能
    protected bool IsFireSkill(int skillID)
    {
        if (Self.ActionControl.IsActionRunning(ActorAction.ENType.enAttackAction))
        {//攻击中
            if (Self.NormalSkillList.Contains(skillID))
            {//当前技能是普攻
                return false;
            }
            else
            {//当前技能不是普攻
                if (Self.NormalSkillList.Contains(m_lastSkillID))
                {//上个技能是普攻
                    return true;
                }
                else
                {//上个技能不是普攻
                    return false;
                }
            }
        }
        return true;
    }
    protected bool IsCmdExecute()
    {
        DeadAction deadAction = Self.ActionControl.LookupAction(ActorAction.ENType.enDeadAction) as DeadAction;
        if (deadAction != null)
        {
            return false;
        }
        ReliveAction reliveAction = Self.ActionControl.LookupAction(ActorAction.ENType.enReliveAction) as ReliveAction;
        if (reliveAction != null)
        {
            return false;
        }
        return true;
    }
    //高亮的技能id
    protected int m_highlightSkillID = 0;
    //取消技能高亮
    protected void DelSkillHighlight()
    {
        if (m_highlightSkillID != 0)
        {
            Self.NotifyChanged((int)Actor.ENPropertyChanged.enBattleBtn_D, MainPlayer.ENBattleBtnNotifyType.enDelSkillHighlight);
            m_highlightSkillID = 0;
        }
    }
    //添加技能高亮
    protected void AddSkillHighlight()
    {
        Self.NotifyChanged((int)Actor.ENPropertyChanged.enBattleBtn_D, MainPlayer.ENBattleBtnNotifyType.enAddSkillHighlight);
    }
    #region CheckAllSkill
    #region SkillIDListForFire//可释放技能列表
    List<int> m_skillIDListForFire;
    public List<int> SkillIDListForFire
    {
        get
        {
            if (m_skillIDListForFire == null)
            {
                m_skillIDListForFire = new List<int>();
            }
            return m_skillIDListForFire;
        }
        protected set { m_skillIDListForFire = value; }
    }
    #endregion
    bool m_isSkillUpdateForFire = false;
    List<int> m_newSkillIDListForFire = new List<int>();
    //可释放技能列表(不包含普攻技能)
    public List<int> FireSkillIDListWithoutNormal = new List<int>();
    void CheckTarget(Actor target)
    {
        if (target == null || target.IsDead)
        {
            return;
        }
        Vector3 d = target.RealPos - Self.RealPos;
        d.y = 0;
        float distance = d.magnitude;
        foreach (var item in Self.SkillBag)
        {
            if (item.IsSilence) continue;
            if (item.MinComboRequir > Self.Combo.TotalComboNumber)
            {//combo值不足，不能释放技能
                continue;
            }
            if (Self.CurrentRage < item.SkillTableInfo.CostRagePoint)
            {//怒气值不足
                continue;
            }
            if (distance < item.SkillTableInfo.LeastAttackDistance)
            {//最小攻击距离内
                continue;
            }
            float attackRange = Self.CurrentTableInfo.AttackRange;
            if (!Self.CurrentTargetIsDead)
            {
                attackRange = item.SkillTableInfo.AttackDistance > attackRange ? item.SkillTableInfo.AttackDistance : attackRange;
            }
            if (distance < attackRange)
            {//攻击距离内
                switch ((ENTargetType)item.SkillTableInfo.TargetType)
                {
                    case ENTargetType.enEnemy:
                        if (!ActorTargetManager.IsEnemy(Self, target))
                        {
                            continue;
                        }
                        break;
                    case ENTargetType.enFriendly:
                        if (!ActorTargetManager.IsFriendly(Self, target))
                        {
                            continue;
                        }
                        break;
                    case ENTargetType.enSelf:
                        if (Self != target)
                        {
                            continue;
                        }
                        break;
                    case ENTargetType.enNullTarget:
                        break;
                    case ENTargetType.enFriendlyAndSelf:
                        {
                            if (!ActorTargetManager.IsFriendly(Self, target) && Self != target)
                            {
                                continue;
                            }
                        }
                        break;
                    default:
                        continue;
                        //break;
                }
                if (!m_newSkillIDListForFire.Contains(item.SkillTableInfo.ID))
                {
                    if (Self.NormalSkillList.Contains(item.SkillTableInfo.ID))
                    {
                        m_newSkillIDListForFire.AddRange(Self.NormalSkillList);
                    }
                    else
                    {
                        m_newSkillIDListForFire.Add(item.SkillTableInfo.ID);
                    }
                }
                if (!SkillIDListForFire.Contains(item.SkillTableInfo.ID))
                {
                    m_isSkillUpdateForFire = true;
                }

                if (item.SkillTableInfo.SkillType == (int)ENSkillType.enSkill)
                {//skill
                    FireSkillIDListWithoutNormal.Add(item.SkillTableInfo.ID);
                }
            }
        }
    }
    void CheckAllSkill()
    {
        m_newSkillIDListForFire.Clear();
        FireSkillIDListWithoutNormal.Clear();
        ActorManager.Singleton.ForEach(CheckTarget);
        if (m_isSkillUpdateForFire || SkillIDListForFire.Count != m_newSkillIDListForFire.Count)
        {//可释放技能列表改变，通知ui
            SkillIDListForFire.Clear();
            SkillIDListForFire.AddRange(m_newSkillIDListForFire);
            Self.NotifyChanged((int)Actor.ENPropertyChanged.enBattleBtn_D, MainPlayer.ENBattleBtnNotifyType.enSkillEnabled);
            m_isSkillUpdateForFire = false;
        }
    }
    #endregion//检测可释放技能
}