using System;
using System.Collections;
using UnityEngine;

//改变伤害
class BuffEffectChangeDamage : IBuffEffect
{
    enum ENType
    {
        enNone,
        enGetDamage,//受到伤害
        enProduceDamage,//造成伤害
    }
    public BuffEffectChangeDamage()
        : base(ENBuff.ChangeDamage)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectChangeDamage();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        if (info == null)
        {
            Debug.LogWarning("buff is not exist, buff id is " + BuffID);
            return;
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
                    switch ((ENType)item.ParamList[0])
                    {
                        case ENType.enGetDamage:
                            {
                                if ((int)item.ParamList[4] == (int)r.DamageResultType)
                                {//改变某类型的伤害
                                    float percent = item.ParamList[1];
                                    if (percent != 0)
                                    {
                                        r.DamageValue *= percent;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
    public override void OnProduceResult(IResult result, IResultControl control)
    {
        base.OnProduceResult(result, control);
        if (result.ClassID == (int)ENResult.Damage)
        {
            ResultDamage r = result as ResultDamage;
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    switch ((ENType)item.ParamList[0])
                    {
                        case ENType.enProduceDamage:
                            {
                                if ((int)item.ParamList[4] == (int)r.DamageResultType)
                                {//改变某类型的伤害
                                    float percent = item.ParamList[1];
                                    if (percent != 0)
                                    {
                                        r.DamageValue *= percent;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}