using UnityEngine;
using NGE.Network;
//嘲讽
public class ResultVampire : IResult
{
    enum ENType
    {
        enNone,
        enDamagePhysic,//物理伤害
        enDamageMagic,//魔法伤害
        enDamagePercent,//百分比伤害
    }
    //伤害值
    float m_damageValue = 0;
    public float DamageValue { get { return m_damageValue; } set { m_damageValue = value; } }
    //吸血百分比
    float m_vampirePercent = 0;
    //伤害result类型
    private float m_damageResultType = 0;

    //吸血值
    int m_vampireValue = 0;

    public ResultVampire()
        : base((int)ENResult.Vampire)
    {
    }

    public static IResult CreateNew()
    {
        return new ResultVampire();
    }

    public override void Deserialize(PacketReader stream)
    {
        base.Deserialize(stream);
        m_vampireValue =  stream.ReadInt32();

        Debug.Log(" ResultVampire Deserialize:" + ",m_vampireValue:" + m_vampireValue + ",SourceID:" + SourceID);
    }


    public override void ResultExpr(float[] param)
    {
        base.ResultExpr(param);
        if (param.Length == 0) return;
        
        Actor source = ActorManager.Singleton.Lookup(SourceID);
        if (source == null)
        {
            Debug.LogWarning("source is not exist, id is " + SourceID);
            return;
        }
        Actor target = ActorManager.Singleton.Lookup(TargetID);
        if (target == null)
        {
            Debug.LogWarning("target is not exist, id is " + TargetID);
            return;
        }
        float damageValue = 0;
        switch ((ENType)param[0])
        {
            case ENType.enDamagePhysic:
                {
                    if (param.Length < 5)
                    {
                        return;
                    }
                    float attackValue = source.Props.GetProperty_Float(ENProperty.phyattack);
                    damageValue = attackValue * param[1] - target.Props.GetProperty_Float(ENProperty.phydefend);
                    float min = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enPhyAttackMinValue).FloatTypeValue;
                    if (damageValue < attackValue * min)
                    {
                        damageValue = attackValue * min;
                    }
                    else
                    {
                        damageValue *= UnityEngine.Random.Range(1 - param[2], 1 + param[2]);
                    }
                    m_vampirePercent = param[3];
                    m_damageResultType = param[4];
                }
                break;
            case ENType.enDamageMagic:
                {
                    if (param.Length < 5)
                    {
                        return;
                    }
                    float attackValue = source.Props.GetProperty_Float(ENProperty.magattack);
                    damageValue = attackValue * param[1] - target.Props.GetProperty_Float(ENProperty.magdefend);
                    float min = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enMagAttackMinValue).FloatTypeValue;
                    if (damageValue < attackValue * min)
                    {
                        damageValue = attackValue * min;
                    }
                    else
                    {
                        damageValue *= UnityEngine.Random.Range(1 - param[2], 1 + param[2]);
                    }
                    m_vampirePercent = param[3];
                    m_damageResultType = param[4];
                }
                break;
            case ENType.enDamagePercent:
                {
                    damageValue = target.HP * param[1] * param[2];
                    m_vampirePercent = param[3];
                    m_damageResultType = param[4];
                }
                break;
        }
        if (damageValue > 0)
        {
            DamageValue = damageValue;
        }
    }

    public override void Exec(IResultControl control)
    {
        base.Exec(control);
        int value = (int)DamageValue;
        if (value <= 0)
        {
            return;
        }
        Actor source = ActorManager.Singleton.Lookup(SourceID);
        if (source == null)
        {
            Debug.LogWarning("actor is not exist, id is " + SourceID);
            return;
        }
        Actor target = ActorManager.Singleton.Lookup(TargetID);
        if (target == null)
        {
            Debug.LogWarning("target is not exist, id is " + TargetID);
            return;
        }
        IResult r = BattleFactory.Singleton.CreateResult(ENResult.Damage, SourceID, TargetID, 0, 0, new float[5] { (float)ResultDamage.ENDamageType.enDamageFixed, value, 0, 0, m_damageResultType });
        if (r != null)
        {
            r.ResultExpr(new float[5] { (float)ResultDamage.ENDamageType.enDamageFixed, value, 0, 0, m_damageResultType });
            BattleFactory.Singleton.DispatchResult(r);

            ResultDamage rd = r as ResultDamage;
            value = (int)rd.DamageValue;
            if (value > 0)
            {
                float vampireValue = value * m_vampirePercent;
                if (!source.IsDead)
                {
                    source.AddHp((int)vampireValue);
                    source.OnHpChanged((int)vampireValue, false, 0, true);
                }
            }
        }
    }

    public override void ResultServerExec(IResultControl control)
    {
        base.ResultServerExec(control);

        Actor source = ActorManager.Singleton.Lookup(SourceID);
        if (source == null)
        {
            Debug.LogWarning("actor is not exist, id is " + SourceID);
            return;
        }
        if (m_vampireValue<= 0 )
        {
            return;
        }

        if (!source.IsDead)
        {
            source.AddHp(m_vampireValue);
            source.OnHpChanged(m_vampireValue, false, 0, true);
        }

    }
}