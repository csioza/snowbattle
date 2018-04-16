using System;
using System.Collections;
using UnityEngine;

//吸血
class BuffEffectVampire : IBuffEffect
{
    public BuffEffectVampire()
        : base(ENBuff.Vampire)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectVampire();
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
    public override void OnProduceResult(IResult result, IResultControl control)
    {
        base.OnProduceResult(result, control);
        if (result.ClassID == (int)ENResult.Damage)
        {
            ResultDamage damage = result as ResultDamage;
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    if (item.ParamList[1] < UnityEngine.Random.Range(0.0f, 1.0f))
                    {//几率吸血
                        continue;
                    }
                    float value = item.ParamList[0] * damage.DamageValue;
                    IResult r = BattleFactory.Singleton.CreateResult(ENResult.Health, SourceID, TargetID, 0, 0, new float[5] { (float)ResultHealth.HealthType.enFixed, value, 0, 0, 0 });
                    if (r != null)
                    {
                        r.ResultExpr(new float[5] { (float)ResultHealth.HealthType.enFixed, value, 0, 0, 0 });
                        BattleFactory.Singleton.DispatchResult(r);
                    }
                }
            }
        }
    }
}