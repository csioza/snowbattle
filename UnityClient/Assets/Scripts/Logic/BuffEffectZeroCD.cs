using UnityEngine;

//技能CD跳过
public class BuffEffectZeroCD : IBuffEffect
{
    enum OverLeapType
    {
        enNull,
        enOverLeapTimes,//次数
        enProbability,//几率
    }
    int m_overLeapSkillCDTimes = 0;
    public BuffEffectZeroCD()
        : base(ENBuff.ZeroCD)
    {
        IsNeedResultPass = true;
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectZeroCD();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
        Actor targetActor = ActorManager.Singleton.Lookup(TargetID);
        if (targetActor == null)
        {
            Debug.LogWarning("actor is null, actor id is " + TargetID);
            IsNeedResultPass = false;
            return;
        }
        BuffInfo buffInfo = GameTable.BuffTableAsset.Lookup(BuffID);
        if (buffInfo == null)
        {
            Debug.LogWarning("buff is null, buff id is " + BuffID);
            IsNeedResultPass = false;
            return;
        }
        foreach (var item in buffInfo.BuffResultList)
        {
            if (item.ID == (int)ClassID)
            {
                switch ((OverLeapType)item.ParamList[0])
                {
                    case OverLeapType.enOverLeapTimes:
                        {
                            m_overLeapSkillCDTimes = (int)item.ParamList[1];
                        }
                        break;
                }
            }
        }
    }
    public override void OnProduceResult(IResult result, IResultControl control)
    {
        base.OnGetResult(result, control);
        if (result.ClassID == (int)ENResult.SkillCD)
        {
            ResultSkillCD r = result as ResultSkillCD;
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    switch ((OverLeapType)item.ParamList[0])
                    {
                        case OverLeapType.enOverLeapTimes:
                            {
                                if (m_overLeapSkillCDTimes <= 0)
                                {
                                    //移除自身
                                    IResult rbResult = BattleFactory.Singleton.CreateResult(ENResult.RemoveBuff, TargetID, TargetID,0,0, new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffID, BuffID });
                                    if (rbResult != null)
                                    {
                                        rbResult.ResultExpr(new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffID, BuffID });
                                        BattleFactory.Singleton.DispatchResult(rbResult);
                                    }
                                    return;
                                }
                                if (m_overLeapSkillCDTimes > 0)
                                {
                                    r.CDTime = 0;
                                    --m_overLeapSkillCDTimes;
                                }
                            }
                            break;
                        case OverLeapType.enProbability:
                            {
                                if (item.ParamList[1] < UnityEngine.Random.Range(0.0f, 1.0f))
                                {//几率
                                    continue;
                                }
                                r.CDTime = 0;
                            }
                            break;
                    }
                }
            }
        }
    }
}