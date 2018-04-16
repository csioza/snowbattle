using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NGE.Network;

#region disableWarning
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#endregion

public class ResultSkill : IResult
{
    float m_damageModify = 0;

    bool m_isHit        = false;
    bool m_isFakeBreak  = false;
    bool m_isBreak      = false;
    bool m_isBack       = false;
    bool m_isFly        = false;


    public ResultSkill()
		:base((int)ENResult.Skill)
    {
        
    }
    public static IResult CreateNew()
    {
        return new ResultSkill();
    }
    public override void Deserialize(PacketReader stream)
    {
        base.Deserialize(stream);

        SkillID         = stream.ReadInt32();
        SkillResultID   = stream.ReadInt32();

        m_isHit         = (stream.ReadInt32() == 1) ? true : false;
        m_isFly         = (stream.ReadInt32() == 1) ? true : false;
        m_isFakeBreak   = (stream.ReadInt32() == 1) ? true : false;
        m_isBreak       = (stream.ReadInt32() == 1) ? true : false;
        m_isBack        = (stream.ReadInt32() == 1) ? true : false;
       


        //Debug.Log("SkillID:" + SkillID + ",SkillResultID:" + SkillResultID + ",TargetID:" + TargetID + ",SourceID:" + SourceID + ",m_isHit:" + m_isHit + ",m_isFakeBreak:" + m_isFakeBreak + ",m_isBreak:" + m_isBreak + ",m_isBack:" + m_isBack + ",m_isFly:" + m_isFly);
    }
    public override void ResultExpr(float[] param)
    {
        base.ResultExpr(param);
        if (param != null && param.Length > 0)
        {
            SkillResultID = (int)param[0];
        }
        m_damageModify = 0;
    }
    public override void Exec(IResultControl control)
    {
        SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(SkillResultID);
        if (null == info)
        {
            return;
        }
        Actor target = ActorManager.Singleton.Lookup(TargetID);
        Actor source = ActorManager.Singleton.Lookup(SourceID);
        if (null != target && source != null)
        {
            float r = UnityEngine.Random.Range(0.0f, 1.0f);
            if (r >= source.Props.GetProperty_Float(ENProperty.hit) - target.Props.GetProperty_Float(ENProperty.avoid))
            {//没有命中
                return;
            }
            r = UnityEngine.Random.Range(0.0f, 1.0f);
            bool isFakeBreak = false, isBreak = false, isBack = false, isFly = false;
            if (r <= info.InterferePersent - target.Props.GetProperty_Float(ENProperty.AnitInterfere))
            {//普通受击
                isFakeBreak = true;
                r = UnityEngine.Random.Range(0.0f, 1.0f);
                if (r < info.BreakPersent - target.Props.GetProperty_Float(ENProperty.AnitInterrupt))
                {//强受击
                    isBreak = true;
                    //Debug.LogWarning("hit 打断 target id=" + target.ID.ToString());
                    r = UnityEngine.Random.Range(0.0f, 1.0f);
                    if (r <= info.HitBackPersent - target.Props.GetProperty_Float(ENProperty.AnitRepel))
                    {//击退
                        //Debug.LogWarning("hit 击退 target id=" + target.ID.ToString());
                        isBack = true;
                        r = UnityEngine.Random.Range(0.0f, 1.0f);
                        if (r < info.HitFlyPersent - target.Props.GetProperty_Float(ENProperty.AnitLauncher))
                        {//击飞
                            //Debug.LogWarning("hit 击飞 target id=" + target.ID.ToString());
                            isFly = true;
                        }
                    }
                }
            }
            if (info.ComboNum > 0)
            {
                m_damageModify = GetComboSkillDamageModify(SkillResultID);
                source.OnComboChanged(SkillResultID, TargetID, m_damageModify);
            }
            {//目标强韧结算
                float curStamina = target.Props.GetProperty_Float(ENProperty.stamina);
                if (curStamina <= 0.0f)
                {
                    target.OnStaminaEvent(source, out isFly);
                    isBack = isFly ? isFly : isBack;
                }
            }
            if (isBreak || isFly)
            {
                target.BeHited(source, isBack, isFly, info.AnimGauge);
            }
            else if (isFakeBreak)
            {
                target.FakeBeHited(source);
            }
        }
        else
        {
            return;
        }
        if (info.IsPlayEffect)
        {
            target.PlayEffect(info.EffectName, info.EffectDuration, info.EffectPos, info.IsAheadBone, Vector3.zero);
        }
        if (info.IsChangeShader)
        {
            AnimationShaderParamCallback callback = target.GetBodyParentObject().GetComponent<AnimationShaderParamCallback>();
            callback.ChangeShader(info.ChangeShaderAnimName);
        }
        foreach (var item in info.ParamList)
        {
            if (item.ID == (int)ENResult.Skill)
            {
                BattleFactory.Singleton.CreateSkillResult(SourceID, (int)item.Param[0], target.ID, SkillID, item.Param);
                continue;
            }
            IResult r = BattleFactory.Singleton.CreateResult((ENResult)item.ID, SourceID, TargetID, SkillResultID, SkillID, item.Param);
            if (r != null)
            {
                if (item.ID == (int)ENResult.Damage)
                {
                    ResultDamage damage = r as ResultDamage;
                    damage.DamageModify = m_damageModify / 100 + 1;
                }
                r.ResultExpr(item.Param);
                BattleFactory.Singleton.DispatchResult(r);
            }
        }
        if (source.Combo != null)
        {//combo附加效果
            int comboResultID = 0;
            foreach (var item in info.ExtraParamList)
            {
                if (item.ComboJudgeCount <= 0)
                {
                    continue;
                }
                int comboValue = source.Combo.TotalComboNumber;
                if (comboValue >= item.ComboJudgeCount)
                {
                    comboResultID = item.SkillResultID;
                }
            }
            if (comboResultID != 0)
            {
                BattleFactory.Singleton.CreateSkillResult(SourceID, comboResultID, target.ID, SkillID);
            }
        }
        //战斗意志
        source.OnAddWill(WillToFight.ENAddWillType.enSkillResult, new float[2] { info.SFightWillGet, info.NFightWillGet });
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


    //根据服务器发回的数据进行结算
    public override void ResultServerExec(IResultControl control)
    {
        SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(SkillResultID);
        if (null == info)
        {
            return;
        }
        Actor target = ActorManager.Singleton.Lookup(TargetID);
        Actor source = ActorManager.Singleton.Lookup(SourceID);
        if (null == target || source == null)
        {
            return;
        }
     
        //没有命中
        if ( !m_isHit )
        {
            return;
        }

        // 如果产生combo
        if (info.ComboNum >0)
        {
           //m_damageModify = GetComboSkillDamageModify(SkillResultID);
            source.OnComboChanged(SkillResultID, TargetID, m_damageModify);
        }

        //目标强韧
        {
            target.OnStaminaEvent(source, out m_isFly);
        }
        if (m_isBreak || m_isFly)
        {
            target.BeHited(source, m_isBack, m_isFly, info.AnimGauge);
        }
        else if (m_isFakeBreak)
        {
            target.FakeBeHited(source);
        }
        if (info.IsPlayEffect)
        {
            target.PlayEffect(info.EffectName, info.EffectDuration, info.EffectPos, info.IsAheadBone, Vector3.zero);
        }
        if (info.IsChangeShader)
        {
            AnimationShaderParamCallback callback = target.GetBodyParentObject().GetComponent<AnimationShaderParamCallback>();
            callback.ChangeShader(info.ChangeShaderAnimName);
        }

        //战斗意志
        source.OnAddWill(WillToFight.ENAddWillType.enSkillResult, new float[2] { info.SFightWillGet, info.NFightWillGet });

        // 设置爆破点
        target.SetBlastPos(source.RealPos, source.GetBodyObject().transform.forward);
        
    }
}
