using System;
using System.Collections;
using UnityEngine;

//改变恢复
class BuffEffectChangeRestore : IBuffEffect
{
    enum ENType
    {
        enNone,
        enGetRestore,//受到恢复
        enProduceRestore,//造成恢复
    }
    public BuffEffectChangeRestore()
        : base(ENBuff.ChangeRestore)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectChangeRestore();
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
        if (result.ClassID == (int)ENResult.Health)
        {
            ResultHealth r = result as ResultHealth;
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    switch ((ENType)item.ParamList[0])
                    {
                        case ENType.enGetRestore:
                            {
                                if ((int)item.ParamList[4] == (int)r.HealthResultType)
                                {//改变某类型的恢复
                                    float percent = item.ParamList[1];
                                    if (percent != 0)
                                    {
                                        r.HealthValue *= percent;
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
        if (result.ClassID == (int)ENResult.Health)
        {
            ResultHealth r = result as ResultHealth;
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    switch ((ENType)item.ParamList[0])
                    {
                        case ENType.enProduceRestore:
                            {
                                if ((int)item.ParamList[4] == (int)r.HealthResultType)
                                {//改变某类型的恢复
                                    float percent = item.ParamList[1];
                                    if (percent != 0)
                                    {
                                        r.HealthValue *= percent;
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