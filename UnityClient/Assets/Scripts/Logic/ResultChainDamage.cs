using System;
using System.Collections.Generic;
using UnityEngine;

//连锁伤害
public class ResultChainDamage : IResult
{
    enum ENChainDamageType
    {
        enNone,
        enMagicMulti,//魔法倍数
        enPercent,//百分比
    }
    float[] m_paramList = new float[] { };
    public ResultChainDamage()
        : base((int)ENResult.ChainDamage)
    {
    }
    static public IResult CreateNew()
    {
        return new ResultChainDamage();
    }
    public override void ResultExpr(float[] param)
    {
        base.ResultExpr(param);
        m_paramList = param;
    }
    public override void Exec(IResultControl control)
    {
        base.Exec(control);
        if (m_paramList == null)
        {
            Debug.LogWarning("param is null");
            return;
        }
        if (m_paramList.Length == 0)
        {
            Debug.LogWarning("param length is 0");
            return;
        }
        m_self = ActorManager.Singleton.Lookup(SourceID);
        if (m_self == null)
        {
            Debug.LogWarning("actor is null, id is " + SourceID);
            return;
        }
        if (m_self.Type == ActorType.enNPC)
        {
            m_range = (m_self as NPC).CurrentTableInfo.AttackRange;
        }
        else
        {
            m_range = (m_self as Player).CurrentTableInfo.AttackRange;
        }
        SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(SkillResultID);
        if (info == null)
        {
            Debug.LogWarning("SkillResultInfo is null, id is " + SkillResultID);
            return;
        }
        switch ((ENChainDamageType)m_paramList[0])
        {
            case ENChainDamageType.enMagicMulti:
                {
                    float attackValue = m_self.Props.GetProperty_Float(ENProperty.magattack);
                    float multi = m_paramList[1];
                    int count = (int)m_paramList[2];
                    float percent = m_paramList[3];

                    m_targetList = new List<Actor>();
                    Actor target = ActorManager.Singleton.Lookup(TargetID);
                    if (target != null && !target.IsDead)
                    {
                        m_targetList.Add(target);
                    }
                    ActorManager.Singleton.ForEach(CheckTarget);
                    for (int i = 0; i < count; ++i)
                    {
                        if (i >= m_targetList.Count)
                        {
                            return;
                        }
                        float defend = m_targetList[i].Props.GetProperty_Float(ENProperty.magdefend);
                        float value = (attackValue * multi - defend) * (float)Math.Pow(percent, i);
                        float min = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enMagAttackMinValue).FloatTypeValue;
                        if (value < attackValue * min)
                        {
                            value = attackValue * min;
                        }
                        
                        {//添加特效
                            m_targetList[i].PlayEffect(info.EffectName, info.EffectDuration, info.EffectPos, info.IsAheadBone, Vector3.zero);
                        }
                        {//添加combo
                            float modifyValue = GetComboSkillDamageModify(SkillResultID);
                            m_self.OnComboChanged(SkillResultID, m_targetList[i].ID, modifyValue);
                        }
                        IResult r = BattleFactory.Singleton.CreateResult(ENResult.Damage, SourceID, m_targetList[i].ID, 0, 0, new float[5] { (float)ResultDamage.ENDamageType.enDamageFixed, value, 0, 0, m_paramList[4] });
                        if (r != null)
                        {
                            r.ResultExpr(new float[5] { (float)ResultDamage.ENDamageType.enDamageFixed, value, 0, 0, m_paramList[4] });
                            BattleFactory.Singleton.DispatchResult(r);
                        }
                    }
                }
                break;
            case ENChainDamageType.enPercent:
                {
                    int count = (int)m_paramList[2];
                    float percent = m_paramList[3];

                    m_targetList = new List<Actor>();
                    Actor target = ActorManager.Singleton.Lookup(TargetID);
                    if (target != null && !target.IsDead)
                    {
                        m_targetList.Add(target);
                    }
                    ActorManager.Singleton.ForEach(CheckTarget);
                    for (int i = 0; i < count; ++i)
                    {
                        if (i >= m_targetList.Count)
                        {
                            return;
                        }
                        float maxHP = m_targetList[i].MaxHP;
                        float value = (maxHP * m_paramList[1]) * (float)Math.Pow(percent, i);

                        {//添加特效
                            m_targetList[i].PlayEffect(info.EffectName, info.EffectDuration, info.EffectPos, info.IsAheadBone, Vector3.zero);
                        }
                        {//添加combo
                            float modifyValue = GetComboSkillDamageModify(SkillResultID);
                            m_self.OnComboChanged(SkillResultID, m_targetList[i].ID, modifyValue);
                        }
                        IResult r = BattleFactory.Singleton.CreateResult(ENResult.Damage, SourceID, m_targetList[i].ID, 0, 0, new float[5] { (float)ResultDamage.ENDamageType.enDamageFixed, value, 0, 0, m_paramList[4] });
                        if (r != null)
                        {
                            r.ResultExpr(new float[5] { (float)ResultDamage.ENDamageType.enDamageFixed, value, 0, 0, m_paramList[4] });
                            BattleFactory.Singleton.DispatchResult(r);
                        }
                    }
                }
                break;
        }
    }
    Actor m_self = null;
    float m_range = 0;
    List<Actor> m_targetList = null;
    void CheckTarget(Actor target)
    {
        if (target.IsDead)
        {
            return;
        }
        if (ActorTargetManager.IsEnemy(m_self, target))
        {
            float distance = ActorTargetManager.GetTargetDistance(m_self.RealPos, target.RealPos);
            if (distance < m_range)
            {
                if (!m_targetList.Contains(target))
                {
                    m_targetList.Add(target);
                }
            }
        }
    }
    //公式:取整((连击数-最小生效连击数)/修正最小combo步长)*多段轻攻击提升百分比/100+1
    #region 最小生效连击数
    int m_minComboNumber = 0;
    int MinComboNumber
    {
        get
        {
            if (m_minComboNumber == 0)
            {
                m_minComboNumber = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enMinComboNumber).IntTypeValue;
            }
            return m_minComboNumber;
        }
    }
    #endregion
    #region 修正最小combo步长
    int m_minComboStepModify = 0;
    int MinComboStepModify
    {
        get
        {
            if (m_minComboStepModify == 0)
            {
                m_minComboStepModify = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enMinComboStepModify).IntTypeValue;
            }
            return m_minComboStepModify;
        }
    }
    #endregion
    #region 多段轻攻击提升百分比
    float m_morePercent = 0;
    float MorePercent
    {
        get
        {
            if (m_morePercent == 0)
            {
                m_morePercent = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enMorePercent).FloatTypeValue;
            }
            return m_morePercent;
        }
    }
    #endregion
    #region 中段中攻击提升百分比
    float m_somePercent = 0;
    float SomePercent
    {
        get
        {
            if (m_somePercent == 0)
            {
                m_somePercent = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSomePercent).FloatTypeValue;
            }
            return m_somePercent;
        }
    }
    #endregion
    #region 少段重攻击提升百分比
    float m_littlePercent = 0;
    float LittlePercent
    {
        get
        {
            if (m_littlePercent == 0)
            {
                m_littlePercent = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enLittlePercent).FloatTypeValue;
            }
            return m_littlePercent;
        }
    }
    #endregion
    private float GetComboSkillDamageModify(int resultID)
    {
        float modify = 1.0f;
        SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(resultID);
        Actor source = ActorManager.Singleton.Lookup(SourceID);
        if (info != null && null != source)
        {
            if (source.Combo != null)
            {
                int count = source.Combo.TotalComboNumber;
                if (count <= MinComboNumber)
                {
                    modify = 0;
                }
                else
                {
                    switch ((ENSkillComboType)info.ComboType)
                    {
                        case ENSkillComboType.enMore:
                            {
                                modify = (float)((int)(((count - MinComboNumber) / MinComboStepModify) + 1) - 1) * MorePercent + 1;
                            }
                            break;
                        case ENSkillComboType.enSome:
                            {
                                modify = (float)((int)(((count - MinComboNumber) / MinComboStepModify) + 1) - 1) * SomePercent + 1;
                            }
                            break;
                        case ENSkillComboType.enLittle:
                            {
                                modify = (float)((int)(((count - MinComboNumber) / MinComboStepModify) + 1) - 1) * LittlePercent + 1;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        return modify;
    }
}