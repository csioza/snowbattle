using UnityEngine;

//抵消伤害
public class BuffEffectOffsetDamage : IBuffEffect
{
    enum ENOffsetDamageType
    {
        enNone,
        enTimes,//次数
        enProbability,//几率
    }
    int m_offsetDamageTimes = 0;
    public BuffEffectOffsetDamage()
        : base(ENBuff.OffsetDamage)
    {
        m_offsetDamageTimes = 0;
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectOffsetDamage();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        if (info == null)
        {
            Debug.LogWarning("buff is null, buff id is " + BuffID);
            return;
        }
        foreach (var item in info.BuffResultList)
        {
            if (item.ID == (int)ClassID)
            {
                switch ((ENOffsetDamageType)item.ParamList[0])
                {
                    case ENOffsetDamageType.enTimes:
                        {
                            m_offsetDamageTimes += (int)item.ParamList[1];
                        }
                        break;
                }
            }
        }
        IsNeedResultPass = true;
    }
    public override void OnGetResult(IResult result, IResultControl control)
    {
        base.OnGetResult(result, control);
        if (result.ClassID == (int)ENResult.Damage)
        {
            ResultDamage r = result as ResultDamage;
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    switch ((ENOffsetDamageType)item.ParamList[0])
                    {
                        case ENOffsetDamageType.enTimes:
                            {
                                if (item.ParamList[4] == 0 || (int)item.ParamList[4] == (int)r.DamageResultType)
                                {
                                    r.DamageValue = 0;
                                    --m_offsetDamageTimes;
                                }
                                if (m_offsetDamageTimes <= 0)
                                {//抵消伤害效果所在的buff里只有这一个效果
                                    //移除自身
                                    IResult rbResult = BattleFactory.Singleton.CreateResult(ENResult.RemoveBuff, TargetID, TargetID, 0, 0, new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffID, BuffID });
                                    if (rbResult != null)
                                    {
                                        rbResult.ResultExpr(new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffID, BuffID });
                                        BattleFactory.Singleton.DispatchResult(rbResult);
                                    }
                                    continue;
                                }
                                
                            }
                            break;
                        case ENOffsetDamageType.enProbability:
                            {
                                if (item.ParamList[4] == 0 || (int)item.ParamList[4] == (int)r.DamageResultType)
                                {
                                    if (item.ParamList[1] < UnityEngine.Random.Range(0.0f, 1.0f))
                                    {//几率
                                        continue;
                                    }
                                    r.DamageValue = 0;
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}