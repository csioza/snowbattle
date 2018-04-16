using UnityEngine;

//禁止死亡
public class BuffEffectOutlawDeath : IBuffEffect
{
    enum ENOutlawDeathType
    {
        enNone,
        enDead,
    }
    ENOutlawDeathType m_type = ENOutlawDeathType.enNone;
    public BuffEffectOutlawDeath()
        : base(ENBuff.OutlawDeath)
    {
        IsNeedResultPass = true;
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectOutlawDeath();
    }
    public override void OnGetBuffEffect()
    {
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        if (null == info)
        {
            Debug.LogWarning("OutlawDeath buff is null, buff id is " + BuffID);
            IsNeedResultPass = false;
            return;
        }
        foreach (var item in info.BuffResultList)
        {
            if (item.ID == (int)ClassID)
            {
                m_type = (ENOutlawDeathType)item.ParamList[0];
                break;
            }
        }
    }
    public override void OnGetResult(IResult result, IResultControl control)
    {
        base.OnGetResult(result, control);
        switch (m_type)
        {
            case ENOutlawDeathType.enDead:
                {
                    if (result.ClassID == (int)ENResult.Dead)
                    {
                        //设置死亡Result返回标志位
                        ResultDead rd = result as ResultDead;
                        rd.m_offExec = true;
                        //设置当前人物的血量
                        ActorManager.Singleton.Lookup(TargetID).Props.SetProperty_Int32(ENProperty.hp, 1);
                    }
                }
                break;
            default:
                break;
        }
    }
}