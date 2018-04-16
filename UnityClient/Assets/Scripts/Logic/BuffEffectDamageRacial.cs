using System;
using UnityEngine;
//种族伤害
public class BuffEffectDamageRacial : IBuffEffect
{
    enum ENType
    {
        enNone,
        enRacialType,//种族类型
    }
    public BuffEffectDamageRacial()
        : base(ENBuff.DamageRacial)
    {
    }
    static public IBuffEffect CreateNew()
    {
        return new BuffEffectDamageRacial();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
        Actor self = ActorManager.Singleton.Lookup(TargetID);
        if (self == null)
        {
            Debug.LogWarning("actor is null, id:" + TargetID);
            return;
        }
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        if (info == null)
        {
            Debug.LogWarning("buff is null, id:" + BuffID);
            return;
        }
        IsNeedResultPass = true;
    }
    public override void OnProduceResult(IResult result, IResultControl control)
    {
        base.OnProduceResult(result, control);
        if (result.ClassID == (int)ENResult.Damage)
        {
            ResultDamage rd = result as ResultDamage;
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    switch ((ENType)item.ParamList[0])
                    {
                        case ENType.enRacialType:
                            {
                                Actor target = ActorManager.Singleton.Lookup(rd.TargetID);
                                if (target != null)
                                {
                                    if (target.OnGetRaceType() == (int)item.ParamList[1])
                                    {
                                        rd.DamageValue *= (1 + item.ParamList[2]);
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