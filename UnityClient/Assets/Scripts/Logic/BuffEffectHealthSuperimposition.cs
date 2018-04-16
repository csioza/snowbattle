using System;
using UnityEngine;
//给予叠加
public class BuffEffectHealthSuperimposition : IBuffEffect
{
    enum ENType
    {
        enNone,
        enAddBuff,//给自身添加buff
        enAddSkillResult,//添加skillresult
    }
    public BuffEffectHealthSuperimposition()
        : base(ENBuff.HealthSuperimposition)
    {
    }
    static public IBuffEffect CreateNew()
    {
        return new BuffEffectHealthSuperimposition();
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
        if (result.ClassID == (int)ENResult.Health)
        {
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    switch ((ENType)item.ParamList[0])
                    {
                        case ENType.enAddBuff:
                            {
                                if (item.ParamList[1] > UnityEngine.Random.Range(0.0f, 1.0f))
                                {
                                    IResult r = BattleFactory.Singleton.CreateResult(ENResult.AddBuff, TargetID, TargetID, 0, 0, new float[1] { item.ParamList[2] });
                                    if (r != null)
                                    {
                                        r.ResultExpr(new float[1] { item.ParamList[2] });
                                        BattleFactory.Singleton.DispatchResult(r);
                                    }
                                }
                            }
                            break;
                        case ENType.enAddSkillResult:
                            {
                                if (item.ParamList[1] > UnityEngine.Random.Range(0.0f, 1.0f))
                                {
                                    BattleFactory.Singleton.CreateSkillResult(TargetID, (int)item.ParamList[2], 0, 0, item.ParamList);
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}