using UnityEngine;
using System.Collections;
using NGE.Network;

#region disableWarning
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#endregion

public class ResultDamage : IResult
{
    //伤害result类型
    public enum ENDamageResultType
    {
        enNone,
        enFixed, //固定伤害类型
        enPhysic, //物理伤害类型
        enMagic, //魔法伤害类型
        enDot, //dot伤害类型
        enReturn,//反弹伤害类型
    }
    //伤害类型
    public enum ENDamageType
    {
        enNone,
        enDamageFixed,//固定伤害
        enPhyDamageTimes,//物理攻击倍数伤害
        enMagDamageTimes,//魔法攻击倍数伤害
        enDamagePercent,//百分比伤害
        enDamageSunder,//破甲伤害
        enPhyDamageDistance,//物理距离比例伤害
        enMagDamageDistance,//魔法距离比例伤害
    }
    //伤害result类型
    private ENDamageResultType m_damageResultType = ENDamageResultType.enNone;
    public ENDamageResultType DamageResultType { get { return m_damageResultType; } protected set { m_damageResultType = value; } }
    //伤害数值
    private float m_damageValue = 0;
    public float DamageValue { get { return m_damageValue; } set { m_damageValue = value; } }
    //伤害修正
    public float DamageModify { get; set; }
    public ResultDamage()
		: base((int)ENResult.Damage)
    {
        DamageResultType = ENDamageResultType.enNone;
        DamageValue = 0;
    }

    int m_param1 { get; set; }
    float m_param2 { get; set; }
    float m_param3 { get; set; }
    float m_param4 { get; set; }

    public static IResult CreateNew()
    {
        return new ResultDamage();
    }

    public override void Deserialize(PacketReader stream)
    {
        base.Deserialize(stream);
        DamageValue     = stream.ReadInt32();
        m_attackValue   = stream.ReadInt32();
        m_isCrit        = (stream.ReadInt32() == 1) ? true : false;

        m_param1 = stream.ReadInt32();

        if ( m_param1!=0 )
        {
            m_param2    =   stream.ReadFloat();
            m_param3    =   stream.ReadFloat();
            m_param4    =   stream.ReadFloat();
            SkillID     =   stream.ReadInt32();
        }
        //Debug.Log("m_param1:" + m_param1 + ",m_param2:" + m_param2);

        //Debug.Log("DamageValue:" + DamageValue + ",SourceID:" + SourceID + ",TargetID:" + TargetID);
    }

    public override void ResultExpr(float[] param)
    {
        base.ResultExpr(param);
        if (param == null || param.Length < 5)
        {
            return;
        }
        Actor target = ActorManager.Singleton.Lookup(TargetID);
        if (null == target)
        {
            return;
        }
        Actor source = ActorManager.Singleton.Lookup(SourceID);
        if (null == source)
        {
            return;
        }
        //物理、魔法加成
        float phyAdd = 0, magAdd = 0;
        //攻击力倍数的修正
        float modifyValue = 0;
        //伤害修正
        float damageModify = 1;
        SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(SkillResultID);
        if (null != info)
        {
            if (info.MinComboCount > 0)
            {
                if (source.Combo != null && source.Combo.TotalComboNumber >= info.MinComboCount)
                {
                    phyAdd = info.PhyAttackAdd;
                    magAdd = info.MagAttackAdd;
                }
            }
            if (source.Combo != null)
            {
                int comboNumber = source.Combo.TotalComboNumber;
                if (comboNumber > 44)
                {
                    comboNumber = 44;
                }
                modifyValue = (int)(1.0f / 15.0f * comboNumber) * info.AttackModify;
            }
            if (info.ComboNum >0)
            {
                damageModify = DamageModify;
            }
            if (info.IsWeakenComboTime)
            {
                target.OnWeakenComboTime(info.WeakenComboTime);
            }
        }
        {//结算
            switch ((ENDamageType)param[0])
            {
                #region enDamageFixed
                case ENDamageType.enDamageFixed://固定伤害
                    {
                        float damage = 0.0f;
                        if (param[1] != 0)
                        {//固定伤害
                            damage = param[1];
                        }
                        if (param[2] != 0)
                        {//百分比上下浮动
                            //目标的伤害系数
                            float woundParam = target.Props.GetProperty_Float(ENProperty.WoundParam);
                            damage *= (woundParam != 0 ? woundParam : 1);
                            damage *= UnityEngine.Random.Range(1 - param[2], 1 + param[2]);
                        }
                        DamageValue = damage;
                    }
                    break;
                #endregion
                #region enPhyDamageTimes
                case ENDamageType.enPhyDamageTimes://物理攻击倍数伤害
                    {
                        float srcAttackValue = source.Props.GetProperty_Float(ENProperty.phyattack);
                        if (param[1] != 0)
                        {//物理攻击力的倍数
                            srcAttackValue *= (param[1] + modifyValue);
                        }
                        srcAttackValue += phyAdd;
                        float damage = srcAttackValue - target.Props.GetProperty_Float(ENProperty.phydefend);
                        float min = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enPhyAttackMinValue).FloatTypeValue;
                        if (damage < srcAttackValue * min)
                        {//伤害最低为物理攻击*物理攻击力的最小值
                            damage = srcAttackValue * min;
                        }
                        else
                        {
                            if (param[2] != 0)
                            {//百分比上下浮动
                                //目标的伤害系数
                                float woundParam = target.Props.GetProperty_Float(ENProperty.WoundParam);
                                damage *= (woundParam != 0 ? woundParam : 1);
                                damage *= UnityEngine.Random.Range(1 - param[2], 1 + param[2]);
                            }
                        }
                        DamageValue = damage;
                        m_attackValue = source.Props.GetProperty_Float(ENProperty.phyattack);
                    }
                    break;
                #endregion
                #region enMagDamageTimes
                case ENDamageType.enMagDamageTimes://魔法攻击倍数伤害
                    {
                        float srcAttackValue = source.Props.GetProperty_Float(ENProperty.magattack);
                        if (param[1] != 0)
                        {//魔法攻击力的倍数
                            srcAttackValue *= (param[1] + modifyValue);
                        }
                        srcAttackValue += magAdd;
                        float damage = srcAttackValue - target.Props.GetProperty_Float(ENProperty.magdefend);
                        float min = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enMagAttackMinValue).FloatTypeValue;
                        if (damage < srcAttackValue * min)
                        {//伤害最低为魔法攻击的10%
                            damage = srcAttackValue * min;
                        }
                        else
                        {
                            if (param[2] != 0)
                            {//百分比上下浮动
                                //目标的伤害系数
                                float woundParam = target.Props.GetProperty_Float(ENProperty.WoundParam);
                                damage *= (woundParam != 0 ? woundParam : 1);
                                damage *= UnityEngine.Random.Range(1 - param[2], 1 + param[2]);
                            }
                        }
                        DamageValue = damage;
                        m_attackValue = source.Props.GetProperty_Float(ENProperty.magattack);
                    }
                    break;
                #endregion
                #region enDamagePercent
                case ENDamageType.enDamagePercent:
                    {
                        float percent = param[1];
                        if (percent > 0 && percent < 1)
                        {
                            float damage = target.MaxHP * percent;
                            float random = param[2];
                            if (random != 0)
                            {//百分比上下浮动
                                //目标的伤害系数
                                float woundParam = target.Props.GetProperty_Float(ENProperty.WoundParam);
                                damage *= (woundParam != 0 ? woundParam : 1);
                                damage *= UnityEngine.Random.Range(1 - param[2], 1 + param[2]);
                            }
                            DamageValue = damage;
                        }
                    }
                    break;
                #endregion
                #region enDamageSunder
                case ENDamageType.enDamageSunder:
                    {
                        float srcAttackValue = source.Props.GetProperty_Float(ENProperty.phyattack);
                        srcAttackValue += phyAdd;
                        float damage = srcAttackValue - target.Props.GetProperty_Float(ENProperty.phydefend) * param[1];
                        if (damage < srcAttackValue * 0.1f)
                        {//伤害最低为物理攻击的10%
                            damage = srcAttackValue * 0.1f;
                        }
                        else
                        {
                            if (param[2] != 0)
                            {//百分比上下浮动
                                //目标的伤害系数
                                float woundParam = target.Props.GetProperty_Float(ENProperty.WoundParam);
                                damage *= (woundParam != 0 ? woundParam : 1);
                                damage *= UnityEngine.Random.Range(1 - param[2], 1 + param[2]);
                            }
                        }
                        DamageValue = damage;
                        m_attackValue = source.Props.GetProperty_Float(ENProperty.phyattack);
                    }
                    break;
                #endregion
                #region enDistancePhyDamageTimes
                case ENDamageType.enPhyDamageDistance:
                    {
                        SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(SkillID);
                        if (skillInfo == null)
                        {
                            return;
                        }
                        float range = 0;
                        float d = ActorTargetManager.GetTargetDistance(source.RealPos, target.RealPos);
                        if (d < param[1])
                        {
                            range = param[1];
                        }
                        else if (d > param[2])
                        {
                            range = param[2];
                        }
                        else
                        {
                            range = d;
                        }
                        float attackValue = source.Props.GetProperty_Float(ENProperty.phyattack);
                        float defend = target.Props.GetProperty_Float(ENProperty.phydefend);
                        DamageValue = attackValue * (range / skillInfo.AttackDistance) * param[3] - defend;
                        float min = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enPhyAttackMinValue).FloatTypeValue;
                        if (DamageValue < attackValue * min)
                        {
                            DamageValue = attackValue * min;
                        }
                    }
                    break;
                #endregion
                #region enDistanceMagDamageTimes
                case ENDamageType.enMagDamageDistance:
                    {
                        SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(SkillID);
                        if (skillInfo == null)
                        {
                            return;
                        }
                        float range = 0;
                        float d = ActorTargetManager.GetTargetDistance(source.RealPos, target.RealPos);
                        if (d < param[1])
                        {
                            range = param[1];
                        }
                        else if (d > param[2])
                        {
                            range = param[2];
                        }
                        else
                        {
                            range = d;
                        }
                        float attackValue = source.Props.GetProperty_Float(ENProperty.magattack);
                        float defend = target.Props.GetProperty_Float(ENProperty.magdefend);
                        DamageValue = attackValue * (range / skillInfo.AttackDistance) * param[3] - defend;
                        float min = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enMagAttackMinValue).FloatTypeValue;
                        if (DamageValue < attackValue * min)
                        {
                            DamageValue = attackValue * min;
                        }
                    }
                    break;
                #endregion
                default:
                    break;
            }
            DamageResultType = (ENDamageResultType)param[4];
            if (DamageValue != 0)
            {
                DamageValue *= damageModify;
                if (UnityEngine.Random.Range(0.0f, 1f) < source.Props.GetProperty_Float(ENProperty.crit))
                {//暴击
                    m_isCrit = true;
                    DamageValue *= (source.Props.GetProperty_Float(ENProperty.critParam));
                }
            }
            else
            {
                Debug.LogWarning("damage result is error, damage is " + DamageValue.ToString()
                    + ", source actor id is " + source.ID.ToString());
            }
        }
    }
    float m_attackValue = 0;
    public override void Exec(IResultControl control)
    {
        int value = (int)DamageValue;
        if (value <= 0)
        {
            return;
        }
        Actor target = ActorManager.Singleton.Lookup(TargetID);
        if (null == target)
        {
            return;
        }
        Actor source = ActorManager.Singleton.Lookup(SourceID);
        float curHP = target.ReduceHp(value);
        float multiple = 0;
        if (m_attackValue != 0)
        {
            multiple = value / m_attackValue;
        }
        target.OnHpChanged(value, m_isCrit, multiple, false);
        if (curHP <= 0)
        {//dead
            if (source != null)
            {
                source.TargetManager.ClearTarget();
            }
            {//result-dead
                IResult r = BattleFactory.Singleton.CreateResult(ENResult.Dead, SourceID, target.ID);
                if (r != null)
                {
                    r.ResultExpr(null);
                    BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
                }
            }
        }
        {//伤害来源
            target.Damgaed(source);
        }
        {//战斗意志
            target.OnAddWill(WillToFight.ENAddWillType.enBeAttacked, new float[1] { value });
        }
        if (target.Type == ActorType.enNPC)
        {//npc记录被伤害数据
            (target as NPC).DamagedRecord(source.ID, value);
        }
        if (target.Type == ActorType.enFollow)
        {//同伴记录被伤害 数据
            target.DamagedRecord(source.ID, value);
        }
    }

    public override void ResultServerExec(IResultControl control)
    {

   //     ClientExec();

        int value = (int)DamageValue;
        if (value <= 0)
        {
            return;
        }
        Actor target = ActorManager.Singleton.Lookup(TargetID);
        if (null == target)
        {
            return;
        }
        Actor source = ActorManager.Singleton.Lookup(SourceID);


       // float curHP = target.ReduceHp(value);

        float multiple = 0;
        if (m_attackValue != 0)
        {
            multiple = value / m_attackValue;
        }
        target.OnHpChanged(value, m_isCrit, multiple, false);
        
        {//伤害来源
            target.Damgaed(source);
        }
        {//战斗意志
            target.OnAddWill(WillToFight.ENAddWillType.enBeAttacked, new float[1] { value });
        }
        if (target.Type == ActorType.enNPC)
        {//npc记录被伤害数据
            (target as NPC).DamagedRecord(source.ID, value);
        }
        if (target.Type == ActorType.enFollow)
        {//同伴记录被伤害 数据
            target.DamagedRecord(source.ID, value);
        }
    }

    // 部分客户端结算
    void ClientExec()
    {

        if ( m_param1 == 0)
        {
            return;
        }
        Actor target = ActorManager.Singleton.Lookup(TargetID);
      
        Actor source = ActorManager.Singleton.Lookup(SourceID);

        if ( null == target || null == source )
        {
            return;
        }

        switch(m_param1)
        {
            #region enPhyDamageDistance
            case (int)ENDamageType.enPhyDamageDistance:
                {
                    SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(SkillID);
                    if (skillInfo == null)
                    {
                        return;
                    }
                    float range = 0;
                    float d = ActorTargetManager.GetTargetDistance(source.RealPos, target.RealPos);
                    if (d < m_param2)
                    {
                        range = m_param2;
                    }
                    else if (d > m_param3)
                    {
                        range = m_param3;
                    }
                    else
                    {
                        range = d;
                    }
                    float attackValue   = source.Props.GetProperty_Float(ENProperty.phyattack);
					float critRate      = source.Props.GetProperty_Int32(ENProperty.crit);
					float critParam     = source.Props.GetProperty_Int32(ENProperty.critParam);
					float srcLevel      = source.Props.GetProperty_Int32(ENProperty.level);
                    float defend        = target.Props.GetProperty_Float(ENProperty.phydefend);
					float dstLevel      = target.Props.GetProperty_Int32(ENProperty.level);


					float damageRandParam = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enDamageRandomParam).FloatTypeValue;
					float DRParam = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enDamageReduceParam).FloatTypeValue;
					float hightestDRParam = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enHighestDRParam).FloatTypeValue;

					float levelGap = dstLevel - srcLevel;

					if (levelGap <= 0){
						levelGap =1;
					}

					float DRRate = defend/(defend + levelGap * DRParam);

					if (DRRate >= hightestDRParam){
						DRRate = hightestDRParam ;
					}

					DamageValue = attackValue * (range / skillInfo.AttackDistance) * m_param4 * (1 - DRRate);

                }
                break;
                #endregion
            #region enDistanceMagDamageTimes
            case (int)ENDamageType.enMagDamageDistance:
                {
                    SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(SkillID);
                    if (skillInfo == null)
                    {
                        return;
                    }
                    float range     = 0;
                    float d         = ActorTargetManager.GetTargetDistance(source.RealPos, target.RealPos);
                    if (d < m_param2)
                    {
                        range       = m_param2;
                    }
                    else if (d > m_param3)
                    {
                        range       = m_param3;
                    }
                    else
                    {
                        range       = d;
                    }
					float attackValue   = source.Props.GetProperty_Float(ENProperty.magattack);
					float critRate      = source.Props.GetProperty_Int32(ENProperty.crit);
					float critParam     = source.Props.GetProperty_Int32(ENProperty.critParam);
					float srcLevel      = source.Props.GetProperty_Int32(ENProperty.level);
					float defend        = target.Props.GetProperty_Float(ENProperty.magdefend);
					float dstLevel      = target.Props.GetProperty_Int32(ENProperty.level);
					
					
					float damageRandParam = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enDamageRandomParam).FloatTypeValue;
					float DRParam = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enDamageReduceParam).FloatTypeValue;
					float hightestDRParam = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enHighestDRParam).FloatTypeValue;
					
					float levelGap = dstLevel - srcLevel;
					
					if (levelGap <= 0){
						levelGap =1;
					}
					
					float DRRate = defend/(defend + levelGap * DRParam);
					
					if (DRRate >= hightestDRParam){
						DRRate = hightestDRParam ;
					}
					
					DamageValue = attackValue * (range / skillInfo.AttackDistance) * m_param4 * (1 - DRRate);
		}
			break;
			#endregion
		}
       
    }
}