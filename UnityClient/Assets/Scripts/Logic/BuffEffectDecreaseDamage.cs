using System;
using UnityEngine;

//递减伤害
public class BuffEffectDecreaseDamage : IBuffEffect
{
    enum ENDecreaseDamageType
    {
        enNone,
        enPercent,//按百分比递减
    }
    //递减百分比
    float m_decreasePercent = 0;
    public BuffEffectDecreaseDamage()
        : base(ENBuff.DecreaseDamage)
    {
        m_decreasePercent = 0;
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectDecreaseDamage();
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
        foreach (var item in info.BuffResultList)
        {
            if (item.ID == (int)ClassID)
            {
                switch ((ENDecreaseDamageType)item.ParamList[0])
                {
                    case ENDecreaseDamageType.enPercent:
                        {
                            m_decreasePercent = item.ParamList[1];
                        }
                        break;
                }
            }
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
                    switch ((ENDecreaseDamageType)item.ParamList[0])
                    {
                        case ENDecreaseDamageType.enPercent:
                            {
                                if (item.ParamList[4] == 0 || (int)item.ParamList[4] == (int)r.DamageResultType)
                                {
                                    r.DamageValue *= m_decreasePercent;
                                    m_decreasePercent *= item.ParamList[3];
                                    if (m_decreasePercent < item.ParamList[2])
                                    {//最小值
                                        m_decreasePercent = item.ParamList[2];
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