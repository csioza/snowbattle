using System;
using UnityEngine;
using System.Collections.Generic;

public class AINpcBoss : AINpc
{
    //愤怒次数
    public int m_angerCount = 0;
    //愤怒技能id
    int m_angerSkillID = 1021;
    //战斗开始的时间
    float m_battleStartTime = 0;
    public AINode m_bossAI = null;
    public AINpcBoss(string fileName, string bossStr)
    {
        mIsInitBaseData = false;
        InitBaseData();
        m_bossAI = AINodeManager.CreateBossAI(fileName, bossStr);
    }
    public void InitBaseData()
    {
        
    }
    public override void Update()
    {
        base.BaseUpdate();
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
        m_bossAI.Exec(Owner);
//         if (BreakStamina())
//         {
//             ;
//         }
//         else if (Anger())
//         {
//             ;
//         }
//         else if (GetTarget())
//         {
//         }
//         else if (Battle())
//         {
//             ;
//         }
//         else
//         {//free
//             ActionForwardTo(m_curTargetID);
//         }
    }
    //破韧
    public bool BreakStamina()
    {
        int skillID = 0;
        if (Self.CurrentCmd != null)
        {//攻击命令，直接释放
            skillID = Self.CurrentCmd.m_skillID;
            if (Self.CurrentCmd.m_targetID != 0)
            {
                m_curTargetID = Self.CurrentCmd.m_targetID;
            }
            if (ActionTryFireSkill(skillID))
            {
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
            return true;
        }
        return false;
    }
    //愤怒
    bool Anger()
    {
        float hpPercent = Self.HP / Self.MaxHP;
        if (hpPercent <= 0.5f)
        {//hp<=50%
            if (m_angerCount == 0)
            {//愤怒次数为0
                SkillInfo info = GameTable.SkillTableAsset.Lookup(m_angerSkillID);
                GetRangeTargetList((ENTargetType)info.TargetType, info.AttackDistance);
                m_curTargetID = 0;
                //释放愤怒技能
                if (ActionTryFireSkill(m_angerSkillID))
                {//愤怒次数+1
                    m_angerCount++;
                    return true;
                }
            }
        }
        return false;
    }
    //获得攻击目标
    bool GetTarget()
    {
        if (!Self.CurrentTargetIsDead)
        {//有目标
            m_curTargetID = Self.CurrentTarget.ID;
            Self.ShowBossBloodBar();
        }
        else
        {
            GetRangeTargetList(ENTargetType.enEnemy, Self.CurrentTableInfo.AttackRange);
            if (m_targetIDList.Count > 0)
            {//攻击范围内有敌人
                Self.CurrentTarget = m_minActor;
                return true;
            }
        }
        return false;
    }
    //战斗
    bool Battle()
    {
        if (HighFarAttack())
        {
            ;
        }
        else if (HighNearAttack())
        {
            ;
        }
        else if (FarAttack())
        {
            ;
        }
        else if (NearAttack())
        {
            ;
        }
        else
        {
            return false;
        }
        return true;
    }
    //高级远程攻击
    bool HighFarAttack()
    {
        if (!Self.CurrentTargetIsDead)
        {//有目标
            float hpPercent = Self.HP / Self.MaxHP;
            if (hpPercent <= 0.5f)
            {//hp<=50%
                float d = ActorTargetManager.GetTargetDistance(Self.RealPos, Self.CurrentTarget.RealPos);
                if (d >= 2)
                {
                    if (Time.time - m_battleStartTime > 4)
                    {//
                        //释放“远程”&&“高级”技能
                        int skillID = GetSkillID(ENSkillDistanceType.enFar, ENSkillLevelType.enHigh);
                        if (skillID != 0 && ActionTryFireSkill(skillID))
                        {
                            m_battleStartTime = Time.time;
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    //高级近战攻击
    bool HighNearAttack()
    {
        if (!Self.CurrentTargetIsDead)
        {//有目标
            float hpPercent = Self.HP / Self.MaxHP;
            if (hpPercent <= 0.5f)
            {//hp<=50%
                float d = ActorTargetManager.GetTargetDistance(Self.RealPos, Self.CurrentTarget.RealPos);
                if (d < 2)
                {
                    if (Time.time - m_battleStartTime > 4)
                    {//
                        //释放“近战”&&“高级”技能
                        int skillID = GetSkillID(ENSkillDistanceType.enNear, ENSkillLevelType.enHigh);
                        if (skillID != 0 && ActionTryFireSkill(skillID))
                        {
                            m_battleStartTime = Time.time;
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    //远程攻击
    bool FarAttack()
    {
        if (!Self.CurrentTargetIsDead)
        {//有目标
            float d = ActorTargetManager.GetTargetDistance(Self.RealPos, Self.CurrentTarget.RealPos);
            if (d >= 2)
            {
                if (Time.time - m_battleStartTime > 4)
                {//
                    //释放“远程”技能
					int skillID = GetSkillID(ENSkillDistanceType.enFar,ENSkillLevelType.enBase);
                    if (skillID != 0 && ActionTryFireSkill(skillID))
                    {
                        m_battleStartTime = Time.time;
                        return true;
                    }
                }
            }
        }
        return false;
    }
    //近战攻击
    bool NearAttack()
    {
        if (!Self.CurrentTargetIsDead)
        {//有目标
            float d = ActorTargetManager.GetTargetDistance(Self.RealPos, Self.CurrentTarget.RealPos);
            if (d < 2)
            {
                if (Time.time - m_battleStartTime > 4)
                {//
                    //释放“近战”技能
					int skillID = GetSkillID(ENSkillDistanceType.enNear,ENSkillLevelType.enBase);
                    if (skillID != 0 && ActionTryFireSkill(skillID))
                    {
                        m_battleStartTime = Time.time;
                        return true;
                    }
                }
            }
        }
        return false;
    }
    //获得技能id
    public int GetSkillID(ENSkillDistanceType distanceType, ENSkillLevelType levelType = ENSkillLevelType.enNone)
    {
        int skillID = 0;
        List<int> skillIDList = new List<int>();
        foreach (var item in Self.SkillBag)
        {
            if (distanceType != ENSkillDistanceType.enNone && item.SkillTableInfo.SkillDistanceType != (int)distanceType)
            {
                continue;
            }
            if (levelType != ENSkillLevelType.enNone && item.SkillTableInfo.SkillLevelType != (int)levelType)
            {
                continue;
            }
            if (!item.IsCanFire(Self))
            {
                continue;
            }
            skillIDList.Add(item.SkillTableInfo.ID);
        }
        if (skillIDList.Count != 0)
        {
            skillID = skillIDList[UnityEngine.Random.Range(0, skillIDList.Count)];
        }
        return skillID;
    }
    public bool GetRangeAndSetCurTarget()
    {
        GetRangeTargetList(ENTargetType.enEnemy, Self.CurrentTableInfo.AttackRange);
        if (m_targetIDList.Count > 0)
        {//攻击范围内有敌人
            Self.CurrentTarget = m_minActor;
            return true;
        }
        return false;
    }
    public bool SetBossBloodBar()
    {
        m_curTargetID = Self.CurrentTarget.ID;
        Self.ShowBossBloodBar();
        return true;
    }
}