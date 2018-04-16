using System;
using System.Collections;
using UnityEngine;

//持续回血
class BuffEffectContinuedRestoreHP : IBuffEffect
{
    public BuffEffectContinuedRestoreHP()
        : base(ENBuff.ContinuedRestoreHP)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectContinuedRestoreHP();
    }
    public override void Exec(IResultControl control)
    {
        base.Exec(control);
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        foreach (var item in info.BuffResultList)
        {
            if (item.ID == (int)ClassID)
            {
                IResult r = BattleFactory.Singleton.CreateResult(ENResult.Health, SourceID, TargetID, 0, 0, item.ParamList);
                if (r != null)
                {
                    r.ResultExpr(item.ParamList);
                    BattleFactory.Singleton.DispatchResult(r);
                }
            }
        }
    }
}