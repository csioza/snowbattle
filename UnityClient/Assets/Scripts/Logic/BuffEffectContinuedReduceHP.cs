using System;
using System.Collections;
using UnityEngine;

//持续减血
class BuffEffectContinuedReduceHP : IBuffEffect
{
    public BuffEffectContinuedReduceHP()
        : base(ENBuff.ContinuedReduceHP)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectContinuedReduceHP();
    }
    public override void Exec(IResultControl control)
    {
        base.Exec(control);
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        foreach (var item in info.BuffResultList)
        {
            if (item.ID == (int)ClassID)
            {
                IResult r = BattleFactory.Singleton.CreateResult(ENResult.Damage, SourceID, TargetID, 0,0,item.ParamList);
                if (r != null)
                {
                    Actor targetActor = ActorManager.Singleton.Lookup(TargetID);
                    targetActor.SetBlastPos(TargetID);
                    r.ResultExpr(item.ParamList);
                    BattleFactory.Singleton.DispatchResult(r);
                }
            }
        }
    }
}