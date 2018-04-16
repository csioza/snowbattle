using System;
using UnityEngine;
//伤害叠加
public class BuffEffectDamageSuperimposition : IBuffEffect
{
    enum ENType
    {
        enNone,
        enDamagedBuffEffect,//受到伤害时，检测自身buff效果类型
        enDamagedBuffID,//受到伤害时，检测自身buffid
        enDamagedHpPercent,//受到伤害时，检测自身hp百分比
        enDamageAddBuff,//造成伤害时，自身添加buff
        enDamageAddSkillResult,//造成伤害时，添加技能result
    }
    public BuffEffectDamageSuperimposition()
        : base(ENBuff.DamageSuperimposition)
    {
    }
    static public IBuffEffect CreateNew()
    {
        return new BuffEffectDamageSuperimposition();
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
    public override void OnGetResult(IResult result, IResultControl control)
    {
        base.OnGetResult(result, control);
        if (result.ClassID == (int)ENResult.Damage)
        {
            ResultDamage rd = result as ResultDamage;
            Actor self = ActorManager.Singleton.Lookup(TargetID);
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            if (self == null || info == null) return;
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    switch ((ENType)item.ParamList[0])
                    {
                        case ENType.enDamagedBuffEffect:
                            {
                                bool flag = false;
                                foreach (var buff in self.MyBuffControl.BuffList)
                                {
                                    BuffInfo buffInfo = GameTable.BuffTableAsset.Lookup(buff.BuffID);
                                    if (buffInfo == null) continue;
                                    if (buffInfo.BuffEffectType == (int)item.ParamList[1])
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    if (item.ParamList[3] > UnityEngine.Random.Range(0.0f, 1.0f))
                                    {
                                        rd.DamageValue *= (1 + item.ParamList[2]);
                                    }
                                }
                            }
                            break;
                        case ENType.enDamagedBuffID:
                            {
                                if (null != self.MyBuffControl.BuffList.Find(buff => buff.BuffID == (int)item.ParamList[1]))
                                {
                                    if (item.ParamList[3] > UnityEngine.Random.Range(0.0f, 1.0f))
                                    {
                                        rd.DamageValue *= (1 + item.ParamList[2]);
                                    }
                                }
                            }
                            break;
                        case ENType.enDamagedHpPercent:
                            {
                                if (self.HP < item.ParamList[1])
                                {
                                    if (item.ParamList[3] > UnityEngine.Random.Range(0.0f, 1.0f))
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
    public override void OnProduceResult(IResult result, IResultControl control)
    {
        base.OnProduceResult(result, control);
        if (result.ClassID == (int)ENResult.Damage)
        {
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    switch ((ENType)item.ParamList[0])
                    {
                        case ENType.enDamageAddBuff:
                            {
                                if (item.ParamList[1] > UnityEngine.Random.Range(0.0f, 1.0f))
                                {
                                    IResult r = BattleFactory.Singleton.CreateResult(ENResult.AddBuff, TargetID, TargetID,0,0, new float[1] { item.ParamList[2] });
                                    if (r != null)
                                    {
                                        r.ResultExpr(new float[1] { item.ParamList[2] });
                                        BattleFactory.Singleton.DispatchResult(r);
                                    }
                                }
                            }
                            break;
                        case ENType.enDamageAddSkillResult:
                            {
                                if (item.ParamList[1] > UnityEngine.Random.Range(0.0f, 1.0f))
                                {
                                    BattleFactory.Singleton.CreateSkillResult(TargetID, (int)item.ParamList[2], 0,0,item.ParamList);
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}