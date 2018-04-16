using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPartner : AIBase
{
    MainPlayer MainActor { get { return ActorManager.Singleton.MainActor; } }
    Player Self { get { return Owner as Player; } }
    float m_nextAITime = 0;
    //是否开始跟随
    bool m_isStartFollow = false;
    //开始休息的时间
    bool m_isStartFree = false;
    float m_startFreeTime = 0;
    //避让判断距离
    float m_evadingValue = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enEvadingValueId).FloatTypeValue;
    //避让移动距离
    float m_evadingDistance = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enEvadingDistanceId).FloatTypeValue;
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
    public override void Update()
    {
        base.Update();
        m_nextAITime -= Time.deltaTime;
        if (m_nextAITime > 0.0f)
        {
            return;
        }
        m_nextAITime = UnityEngine.Random.Range(0.1f, 0.5f);
        if (Owner.IsDead)
        {
            return;
        }
        if (StartFollow())
        {//开始跟随主控角色
            m_isStartFollow = true;
            m_isStartFree = false;
            //清除同伴目标
            Self.CurrentTarget = null;
        }
        else if (StopFollow())
        {//停止跟随主控角色
            m_isStartFollow = false;
            m_isStartFree = false;
        }
        else if (GetTarget())
        {//获得目标
            m_isStartFree = false;
        }
        else if (ChangeOtherPos())
        {
            ;
        }
        else if (AttackTarget())
        {//攻击目标
            m_isStartFree = false;
        }
        else if (GetTargetFromDamage())
        {//获得攻击我的目标
            ;
        }
        else if (Evading())
        {
            ;
        }
        else
        {//free
            if (Self.CurrentTargetIsDead)
            {//没有目标时，面向主控角色
               ActionForwardTo(MainActor.ID);
            }
            m_startFreeTime = Time.time;
            m_isStartFree = true;
        }
    }
    bool Evading()
    {
        if (Self.CurrentTargetIsDead)
        {
            MoveAction moveAction = MainActor.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
            if (moveAction != null)
            {//如果主控角色正在移动
                float PosDistance = ActorTargetManager.GetTargetDistance(Self.MainPos, MainActor.MainPos);
                if (PosDistance < m_evadingValue)
                {//获去主控角色与同伴位置的距离
                    Vector3 mainMoveForward = moveAction.TargetPos - MainActor.MainPos;
                    Vector3 moveForward = Self.MainPos - MainActor.MainPos;
                    moveForward.y = 0;
                    mainMoveForward.y = 0;
                    moveForward.Normalize();
                    mainMoveForward.Normalize();
                    float angle = Vector3.Angle(moveForward, mainMoveForward);
                    if (angle < 90)
                    {
                        Vector3 verticalMoveForward = Vector3.Cross(mainMoveForward, new Vector3(0, 1, 0));
                        verticalMoveForward.Normalize();
                        int tmpIntX = (mainMoveForward.x > moveForward.x ? -1 : 1);
                        int tmpIntZ = (mainMoveForward.z > moveForward.z ? -1 : 1);
                        verticalMoveForward.x = Mathf.Abs(verticalMoveForward.x) * tmpIntX;
                        verticalMoveForward.z = Mathf.Abs(verticalMoveForward.z) * tmpIntZ;
                        Vector3 targetPos = Self.MainPos + (verticalMoveForward * m_evadingDistance);
                        if (!SM.RandomRoomLevel.Singleton.QuickFindPath(Self.MainPos, targetPos))
                        {
                            moveForward.x = moveForward.x * -1;
                            moveForward.z = moveForward.z * -1;
                            targetPos = Self.MainPos + (verticalMoveForward * m_evadingDistance);
                        }
                        ActionMoveTo(targetPos);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool ChangeOtherPos()
    {
        MoveAction moveAction = Self.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
        if (moveAction != null)
        {//如果主控角色正在移动
            float PosDistance = ActorTargetManager.GetTargetDistance(Self.MainPos, MainActor.MainPos);
            if (PosDistance < 1)
            {//获去主控角色与同伴位置的距离
                Vector3 selfMoveForward = moveAction.TargetPos - Self.MainPos;
                Vector3 moveForward =  MainActor.MainPos - Self.MainPos;
                moveForward.y = 0;
                selfMoveForward.y = 0;
                moveForward.Normalize();
                selfMoveForward.Normalize();
                float angle = Vector3.Angle(moveForward, selfMoveForward);
                if (angle < 45)
                {
                    Vector3 verticalMoveForward = Vector3.Cross(selfMoveForward, new Vector3(0, 1, 0));
                    verticalMoveForward.Normalize();
                    int tmpIntX = (selfMoveForward.x > moveForward.x ? 1 : -1);
                    int tmpIntZ = (selfMoveForward.z > moveForward.z ? 1 : -1);
                    verticalMoveForward.x = Mathf.Abs(verticalMoveForward.x) * tmpIntX;
                    verticalMoveForward.z = Mathf.Abs(verticalMoveForward.z) * tmpIntZ;
                    Vector3 targetPos = Self.MainPos + (verticalMoveForward * 3);
//                     if (!SDungeonsMgr.Singleton.QuickFindPath(Self.MainPos, targetPos))
//                     {
//                         moveForward.x = moveForward.x * -1;
//                         moveForward.z = moveForward.z * -1;
//                         targetPos = Self.MainPos + (verticalMoveForward * 3);
//                     }
                    ActionMoveTo(targetPos);
                    return true;
                }
            }
        }
        return false;
    }

    #region StartFollowDistance//开始跟随的距离
    float m_startFollowDistance = 0;
    float StartFollowDistance
    {
        get
        {
            if (m_startFollowDistance == 0)
            {
                m_startFollowDistance = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enPartnerFollowDistance).FloatTypeValue;
            }
            return m_startFollowDistance;
        }
    }
    #endregion
    #region StopFollowDistance//停止跟随的距离
    float m_stopFollowDistance = 0;
    float StopFollowDistance
    {
        get
        {
            if (m_stopFollowDistance == 0)
            {
                m_stopFollowDistance = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enPartnerStopFollowDistance).FloatTypeValue;
            }
            return m_stopFollowDistance;
        }
    }
    #endregion
    #region ForceMoveDistance//瞬移距离
    float m_forceMoveDistance = 0;
    float ForceMoveDistance
    {
        get
        {
            if (m_forceMoveDistance == 0)
            {
                m_forceMoveDistance = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enPartnerForceMoveDistance).FloatTypeValue;
            }
            return m_forceMoveDistance;
        }
    }
    #endregion
    #region FindPathTime//无法寻路的最大时间
    float m_maxFindPathTime = 0;
    float MaxFindPathTime
    {
        get
        {
            if (m_maxFindPathTime == 0)
            {
                m_maxFindPathTime = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enPartnerMaxFindPathTime).FloatTypeValue;
            }
            return m_maxFindPathTime;
        }
    }
    float mDisAbleFindPathTime = 0;
    #endregion
    Vector3 mForceMovePos = Vector3.zero;
    //开始跟随主控角色
    Vector3 GetPosGroundPos(Vector3 pos)
    {
        List<Vector3> mCurPosGroundPosList = new List<Vector3>();
        mCurPosGroundPosList.Add(new Vector3(pos.x + 1,pos.y,pos.z));
        mCurPosGroundPosList.Add(new Vector3(pos.x, pos.y, pos.z + 1));
        mCurPosGroundPosList.Add(new Vector3(pos.x - 1, pos.y, pos.z));
        mCurPosGroundPosList.Add(new Vector3(pos.x, pos.y, pos.z - 1));
        Vector3 tmpPos = MainActor.MainPos;
        for (int i = 0; i < mCurPosGroundPosList.Count; i++ )
        {
            if (SM.RandomRoomLevel.Singleton.QuickFindPath(MainActor.MainPos, mCurPosGroundPosList[i]))
            {
                tmpPos = mCurPosGroundPosList[i];
                break;
            }
        }
        return tmpPos;
    }
    bool StartFollow()
    {
        mForceMovePos = Vector3.zero;
        float distance = ActorTargetManager.GetTargetDistance(Self.RealPos, MainActor.RealPos);
        if (distance > StartFollowDistance)
        {
//             MoveAction action = MainActor.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
//             if (action != null && action.mRealSpeed != 0)
//             {
//                 mForceMovePos = action.mLastMovePos;
//             }
//             else
//             {
//                 if (mForceMovePos == Vector3.zero)
//                 {
//                     mForceMovePos = MainActor.MainPos;
//                 }
//             }
            mForceMovePos = GetPosGroundPos(MainActor.MainPos);
            if (NormalForceMove(distance, mForceMovePos))
            {
                ActionForwardTo(MainActor.ID);
                return true;
            }
            else if(forceMove(mForceMovePos))
            {
                ActionForwardTo(MainActor.ID);
                return true;
            }
            else
            {
                
                if (MainActor.CurrentTargetIsDead || Self.CurrentTargetIsDead)
                {//主控角色没有目标 或 同伴没有目标
                    ActionMoveTo(MainActor.RealPos);
                    return true;
                }
            }
//             if (!SDungeonsMgr.Singleton.QuickFindPath(Self.MainPos, MainActor.MainPos) || distance > ForceMoveDistance)
//             {//跟主控角色之间不能寻路或者距离过大
//                 if (mDisAbleFindPathTime == 0)
//                 {//如果不能寻路的时间为0说明这次是第一次不能寻路，为不能寻路的时间赋值
//                     mDisAbleFindPathTime = Time.time;
//                 }
//                 if (Time.time - mDisAbleFindPathTime > MaxFindPathTime)
//                 {
//                     TeleportAction teleportAction = Self.ActionControl.AddAction(ActorAction.ENType.enTeleportAction) as TeleportAction;
//                     if (teleportAction != null)
//                     {
//                         teleportAction.Init(mForceMovePos);
//                     }
//                     //Self.ForceMoveToPosition(mForceMovePos);
//                     mDisAbleFindPathTime = 0;
//                     return true;
//                 }
//             }
//             else
//             {
//                 mDisAbleFindPathTime = 0;
//                 if (MainActor.CurrentTargetIsDead || Self.CurrentTargetIsDead)
//                 {//主控角色没有目标 或 同伴没有目标
//                     ActionMoveTo(MainActor.RealPos);
//                     return true;
//                 }
//             }
        }
//         if (ForceMove())
//         {//瞬移
//             return true;
//         }
//         else if (FollowMainActor())
//         {//跟随
//             return true;
//         }
        return false;
    }
    bool NormalForceMove(float distance, Vector3 mForceMovePos)
    {//瞬移
        bool isCanFindPath = SM.RandomRoomLevel.Singleton.QuickFindPath(Self.MainPos, mForceMovePos);
        if (!isCanFindPath || distance > ForceMoveDistance)
        {//跟主控角色之间不能寻路或者距离过大
            if (mDisAbleFindPathTime == 0)
            {//如果不能寻路的时间为0说明这次是第一次不能寻路，为不能寻路的时间赋值
                mDisAbleFindPathTime = Time.time;
            }
            if (Time.time - mDisAbleFindPathTime > MaxFindPathTime)
            {
                TeleportAction teleportAction = Self.ActionControl.AddAction(ActorAction.ENType.enTeleportAction) as TeleportAction;
                if (teleportAction != null)
                {
                    teleportAction.Init(mForceMovePos);
                }
                //Self.ForceMoveToPosition(mForceMovePos);
                mDisAbleFindPathTime = 0;
                return true;
            }
        }
        return false;
//         float distance = ActorTargetManager.GetTargetDistance(Self.RealPos, MainActor.RealPos);
//         distance = Mathf.Abs(distance);
//         if (distance > ForceMoveDistance)
//         {//距离大于瞬移距离
//             if (!SDungeonsMgr.Singleton.QuickFindPath(Self.MainPos, ActorManager.Singleton.MainActor.MainPos))
//             {//跟主控角色之间不能寻路
//                 if (mDisAbleFindPathTime == 0)
//                 {//如果不能寻路的时间为0说明这次是第一次不能寻路，为不能寻路的时间赋值
//                     mDisAbleFindPathTime = Time.time;
//                 }
//                 if (Time.time - mDisAbleFindPathTime > MaxFindPathTime)
//                 {
//                     Self.ForceMoveToPosition(ActorManager.Singleton.MainActor.MainPos);
//                     mDisAbleFindPathTime = 0;
//                     return true;
//                 } 
//             }
//         }
//         return false;
    }
    bool forceMove(Vector3 mForceMovePos)
    {//强制瞬移
        if (Self.mCurRoomGUID != -1 && Self.mCurRoomGUID != MainActor.mCurRoomGUID)
        {
            if (!SM.RandomRoomLevel.Singleton.QuickFindPath(Self.MainPos, mForceMovePos))
            {
                TeleportAction teleportAction = Self.ActionControl.AddAction(ActorAction.ENType.enTeleportAction) as TeleportAction;
                if (teleportAction != null)
                {
                    teleportAction.Init(mForceMovePos);
                }
                mDisAbleFindPathTime = 0;
                return true;
            }
        }
        return false;
    }
    bool FollowMainActor()
    {//跟随
        float distance = ActorTargetManager.GetTargetDistance(Self.RealPos, MainActor.RealPos);
        distance = Mathf.Abs(distance);
        if (distance > StartFollowDistance)
        {
            if (MainActor.CurrentTargetIsDead || Self.CurrentTargetIsDead)
            {//主控角色没有目标 或 同伴没有目标
                ActionMoveTo(MainActor.RealPos);
                mDisAbleFindPathTime = 0;
                return true;
            }
        }
        return false;
    }
    //停止跟随主控角色
    bool StopFollow()
    {
        if (m_isStartFollow)
        {
            float distance = ActorTargetManager.GetTargetDistance(Self.RealPos, MainActor.RealPos);
            distance = Mathf.Abs(distance);
            if (distance < StopFollowDistance)
            {
                Self.ActionControl.RemoveAction(ActorAction.ENType.enMoveAction);
                return true;
            }
        }
        return false;
    }
    #region GetTargetDuration//主角选中目标的时长
    float m_getTargetDuration = 0;
    float GetTargetDuration
    {
        get
        {
            if (m_getTargetDuration == 0)
            {
                m_getTargetDuration = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enGetTargetDuration).FloatTypeValue;
            }
            return m_getTargetDuration;
        }
    }
    #endregion
    //获得目标
    bool GetTarget()
    {
        if (GameSettings.Singleton.m_partnerGetTargetFromSelf)
        {//自己获得目标
            if (Self.CurrentTargetIsDead)
            {//当前没有目标
                //获得主动攻击范围内的敌人
                GetRangeTargetList(ENTargetType.enEnemy, Self.CurrentTableInfo.AutoAttackRange);
                if (m_targetIDList.Count != 0)
                {//有敌人
                    //将最近的敌人设置为目标
                    Self.CurrentTarget = m_minActor;
                    m_curTargetID = m_minActor.ID;
                    return true;
                }
            }
        }
        else
        {//从主控角色处获得
            if (PassiveSwitchTarget())
            {
                return true;
            }
            else if (SwitchTargetToMainTarget())
            {
                return true;
            }
            else if (IndependentAccessToTarget())
            {
                return true;
            }
        }
        return false;
    }

    bool PassiveSwitchTarget()
    {//被动切换目标
        float param = Self.MaxHP * DamagedCounterParam;
        foreach (var item in Self.m_damagedRecordMap)
        {
            if (m_curTargetID == item.Key)
            {
                continue;
            }
            if (item.Value > param)
            {
                Actor target = ActorManager.Singleton.Lookup(item.Key);
                if (target == null || target.IsDead)
                {
                    continue;
                }
                if (!ActorTargetManager.IsEnemy(Self, target))
                {
                    continue;
                }
                m_curTargetID = target.ID;
                Self.CurrentTarget = target;
                //清除
                Self.m_damagedRecordMap[item.Key] = 0;
                (target as NPC).m_priorityAttack = true;
                return true;
            }
        }
        return false;
    }
    bool SwitchTargetToMainTarget()
    {//切换到主控角色的目标
        if (Self.CurrentTargetIsDead || !(Self.CurrentTarget as NPC).m_priorityAttack)
        {       
            if (!MainActor.CurrentTargetIsDead)
            {//主控角色有目标
                if (!ActorTargetManager.IsEnemy(Self, MainActor.CurrentTarget))
                {
                    return false;
                }
                if (Time.time - MainActor.TargetManager.m_getTagetTimer > GetTargetDuration)
                {//超过获得目标时长
                    if (MainActor.CurrentTarget != Self.CurrentTarget)
                    {//主控角色的目标与同伴的目标不一致
                        //设置目标
                        Self.CurrentTarget = MainActor.CurrentTarget;
                        m_curTargetID = MainActor.CurrentTarget.ID;
                        return true;
                    }
                }
            }
        }
        return false;
    }
    bool IndependentAccessToTarget()
    {//自主获取目标
        if (Self.CurrentTargetIsDead)
        {//当前没有目标
            //获得主动攻击范围内的敌人
            GetRangeTargetList(ENTargetType.enEnemy, Self.CurrentTableInfo.AutoAttackRange);
            if (m_targetIDList.Count != 0)
            {//有敌人
                //将最近的敌人设置为目标
                Self.CurrentTarget = m_minActor;
                m_curTargetID = m_minActor.ID;
                return true;
            }
        }
        return false;
    }
    //攻击目标
    bool AttackTarget()
    {
        if (Self.ActionControl.IsActionRunning(AttackAction.SGetActionType()))
        {
            return false;
        }
        if (!Self.CurrentTargetIsDead)
        {
            m_curTargetID = Self.CurrentTarget.ID;
            if (!m_targetIDList.Contains(m_curTargetID))
            {
                m_targetIDList.Add(m_curTargetID);
            }
            if (FireFinalSkill())
            {//释放终结技
                ;
            }
            else if (FireConnectOrSpecialSkill())
            {//释放连接技或特殊技
                ;
            }
            else
            {//释放普攻
                FireNormalSkill();
            }
            return true;
        }
        return false;
    }
    //释放终结技
    bool FireFinalSkill()
    {
        if (Self.WTF != null && Self.WTF.IsWillBurning)
        {//意志燃烧中
            bool isCanFire = true;
            int skillID = 0;
            foreach (var item in Self.SkillBag)
            {
                if ((int)ENSkillConnectionType.enConnect == item.SkillTableInfo.SkillConnectionType ||
                    (int)ENSkillConnectionType.enSpecial == item.SkillTableInfo.SkillConnectionType)
                {//连接技和特殊技
                    if (!Self.IsSkillCDRunning(item.SkillTableInfo.ID))
                    {//没在cd中
                        isCanFire = false;
                        break;
                    }
                }
                else if ((int)ENSkillConnectionType.enFinal == item.SkillTableInfo.SkillConnectionType)
                {//终结技
                    if (Self.IsSkillCDRunning(item.SkillTableInfo.ID))
                    {//cd中
                        isCanFire = false;
                        break;
                    }
                    skillID = item.SkillTableInfo.ID;
                }
            }
            if (isCanFire && skillID != 0)
            {//有终结技并且可以释放终结技
                float percent = Self.WTF.WillValue / 100;
                if (UnityEngine.Random.Range(0.0f, 1f) < percent)
                {//几率释放
                    if (ActionTryFireSkill(skillID))
                    {//释放技能成功
                        return true;
                    }
                }
            }
        }
        return false;
    }
    enum ENRandType
    {
        enHaveTargetSkill,//指定目标的技能
        enNullTargetSkill,//无目标的技能
        enBuffSkill,//buff技能
        enRestoreSkill,//恢复技能
        enCount,
    }
    //释放连接技或特殊技
    bool FireConnectOrSpecialSkill()
    {
        if (Self.WTF != null && Self.WTF.IsWillBurning)
        {//意志燃烧中
            int randValue = UnityEngine.Random.Range(0, (int)ENRandType.enCount);
            switch ((ENRandType)randValue)
            {
                case ENRandType.enHaveTargetSkill:
                    {//指定目标的技能
                        foreach (var item in Self.SkillBag)
                        {
                            if (!item.IsCanFire(Self))
                            {
                                continue;
                            }
                            if (item.SkillTableInfo.TargetType == (int)ENTargetType.enNullTarget)
                            {//无目标技能
                                continue;
                            }
                            if ((int)ENSkillConnectionType.enConnect == item.SkillTableInfo.SkillConnectionType ||
                                (int)ENSkillConnectionType.enSpecial == item.SkillTableInfo.SkillConnectionType)
                            {//连接技和特殊技
                                if (ActionTryFireSkill(item.SkillTableInfo.ID))
                                {//释放技能成功
                                    return true;
                                }
                            }

                        }
                    }
                    break;
                case ENRandType.enNullTargetSkill:
                    {//无目标的技能
                        float d = ActorTargetManager.GetTargetDistance(Self.RealPos, Self.CurrentTarget.RealPos);
                        if (Self.CurrentTableInfo.AutoAttackRange >= d)
                        {//自动攻击范围内
                            foreach (var item in Self.SkillBag)
                            {
                                if (!item.IsCanFire(Self))
                                {
                                    continue;
                                }
                                if (item.SkillTableInfo.TargetType != (int)ENTargetType.enNullTarget)
                                {//不是无目标技能
                                    continue;
                                }
                                if (ActionTryFireSkill(item.SkillTableInfo.ID))
                                {//释放技能成功
                                    return true;
                                }
                            }
                        }
                    }
                    break;
                case ENRandType.enBuffSkill:
                    {//buff技能
                        Actor friendActor = null;
                        int r = UnityEngine.Random.Range(0, 2);
                        if (r == 0)
                        {
                            friendActor = Self;
                        }
                        else
                        {
                            friendActor = MainActor;
                        }
                        foreach (var item in Self.SkillBag)
                        {
                            if (!item.IsCanFire(Self))
                            {
                                continue;
                            }
                            if (item.SkillTableInfo.SkillEffectType != (int)ENSkillEffectType.enBuff)
                            {//不是buff技能
                                continue;
                            }
                            if (ActionTryAttack(item.SkillTableInfo.ID, friendActor))
                            {//释放技能成功
                                return true;
                            }
                        }
                    }
                    break;
                case ENRandType.enRestoreSkill:
                    {//恢复技能
                        Actor friendActor = null;
                        int r = UnityEngine.Random.Range(0, 2);
                        if (r == 0)
                        {
                            friendActor = Self;
                        }
                        else
                        {
                            friendActor = MainActor;
                        }
                        foreach (var item in Self.SkillBag)
                        {
                            if (!item.IsCanFire(Self))
                            {
                                continue;
                            }
                            if (item.SkillTableInfo.SkillEffectType != (int)ENSkillEffectType.enRestore)
                            {//不是恢复技能
                                continue;
                            }
                            if (ActionTryAttack(item.SkillTableInfo.ID, friendActor))
                            {//释放技能成功
                                return true;
                            }
                        }
                    }
                    break;
            }
        }
        return false;
    }
    //释放普攻
    bool FireNormalSkill()
    {
        foreach (var item in Self.SkillBag)
        {
            if (!item.IsCanFire(Self))
            {
                continue;
            }
            if (item.SkillTableInfo.SkillType != (int)ENSkillType.enSkillNormalType)
            {//不是普攻
                continue;
            }
            if (ActionTryFireSkill(item.SkillTableInfo.ID))
            {//释放技能成功
                return true;
            }
        }
        return false;
    }
    //获取反击目标
    bool GetTargetFromDamage()
    {
        if (m_isStartFree)
        {//开始休息
            if (Self.DamageSource != null)
            {//有伤害来源
                if (Time.time - Self.DamageTime > m_startFreeTime + GameSettings.Singleton.m_autoCounterattack)
                {//受伤一定时间之后
                    //设置目标
                    Self.CurrentTarget = Self.DamageSource;
                    m_curTargetID = Self.DamageSource.ID;
                }
            }
        }
        return false;
    }
}
