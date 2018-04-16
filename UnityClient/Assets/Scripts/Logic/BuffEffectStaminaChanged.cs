using System;
using System.Collections;
using UnityEngine;
//强韧度改变
class BuffEffectStaminaChanged : IBuffEffect
{
    public BuffEffectStaminaChanged()
        : base(ENBuff.StaminaChanged)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectStaminaChanged();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (actor == null)
        {
            Debug.LogWarning("actor is not exist, actor id is " + TargetID);
            return;
        }
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
        if (result.ClassID == (int)ENResult.StaminaChanged)
        {//强韧度改变的目标是自己，所以在OnGetResult
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            Actor actor = ActorManager.Singleton.Lookup(TargetID);
            if (actor.Type != ActorType.enNPC)
            {
                Debug.LogWarning("StaminaChanged OnGetResult failed! actor is not npc! actor type=" + actor.Type.ToString());
                return;
            }
            foreach (var item in info.BuffResultList)
            {//破韧
                if (item.ID == (int)ClassID)
                {//第一个参数为添加buff的id
                    IResult r = BattleFactory.Singleton.CreateResult(ENResult.AddBuff, TargetID, TargetID, 0, 0, new float[1] { item.ParamList[0] });
                    if (r != null)
                    {
                        r.ResultExpr(new float[1] { item.ParamList[0] });
                        BattleFactory.Singleton.DispatchResult(r);
                    }
                }
            }
        }
    }
}