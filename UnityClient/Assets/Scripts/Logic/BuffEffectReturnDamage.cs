using UnityEngine;

//反弹伤害（可以和吸血效果合并）
public class BuffEffectReturnDamage : IBuffEffect
{
    enum ENReturnDamageType
    {
        enNone,
        enPercent,//按百分比反弹
    }
    public BuffEffectReturnDamage()
        : base(ENBuff.ReturnDamage)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectReturnDamage();
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
                    if (r.DamageResultType == ResultDamage.ENDamageResultType.enReturn)
                    {//伤害类型为反弹伤害，不再反弹
                        continue;
                    }
                    switch ((ENReturnDamageType)item.ParamList[0])
                    {
                        case ENReturnDamageType.enPercent:
                            {
                                if (item.ParamList[4] == 0 || (int)item.ParamList[4] == (int)r.DamageResultType)
                                {
                                    if (item.ParamList[2] < UnityEngine.Random.Range(0.0f, 1.0f))
                                    {//几率
                                        continue;
                                    }
                                    float value = r.DamageValue * item.ParamList[1];
                                    //反弹伤害给伤害来源
                                    IResult returnDamage = BattleFactory.Singleton.CreateResult(ENResult.Damage, TargetID, r.SourceID, 0, 0, new float[5] { (float)ResultDamage.ENDamageType.enDamageFixed, value, 0, 0, (float)ResultDamage.ENDamageResultType.enReturn });
                                    if (returnDamage != null)
                                    {//伤害类型为enDamageFixed
                                        Actor targetActor = ActorManager.Singleton.Lookup(r.SourceID);
                                        targetActor.SetBlastPos(TargetID);
                                        returnDamage.ResultExpr(new float[5] { (float)ResultDamage.ENDamageType.enDamageFixed, value, 0, 0, (float)ResultDamage.ENDamageResultType.enReturn });
                                        BattleFactory.Singleton.DispatchResult(returnDamage);
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