using System;
using UnityEngine;

//debuff免疫
public class BuffEffectIgnoreDebuff : IBuffEffect
{
    enum IgnoreDebuffType
    {
        enNull,
        enControl,
    }
    public BuffEffectIgnoreDebuff()
        :base(ENBuff.IgnoreDebuff)
    {
        IsNeedResultPass = true;
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectIgnoreDebuff();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
    }
    public override void OnGetResult(IResult result, IResultControl control)
    {
 	    base.OnGetResult(result, control);
        if (result.ClassID == (int)ENResult.AddBuff)
        {
            ResultAddBuff r = result as ResultAddBuff;
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    switch ((IgnoreDebuffType)item.ParamList[0])
                    {
                        case IgnoreDebuffType.enControl:
                            {
                                if (item.ParamList[2] < UnityEngine.Random.Range(0.0f, 1.0f))
                                {//几率
                                    continue;
                                }
                                for (int i = 0; i < r.BuffIDList.Length; ++i)
                                {
                                    BuffInfo buffInfo = GameTable.BuffTableAsset.Lookup((int)r.BuffIDList[i]);
                                    if (buffInfo != null)
                                    {
                                        if (buffInfo.BuffEffectType == item.ParamList[1])
                                        {
                                            r.BuffIDIsRun[i] = true;
                                        }
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
