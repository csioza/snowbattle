using UnityEngine;

//改变buff
public class BuffEffectChangeBuff : IBuffEffect
{
    enum ENChangeBuffType
    {
        enNone,
        enSelfLength,//改变自己产生的buff时长
        enTargetLength,//改变目标产生的buff时长
    }
    public BuffEffectChangeBuff()
        : base(ENBuff.ChangeBuff)
    {
    }
    static public IBuffEffect CreateNew()
    {
        return new BuffEffectChangeBuff();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
        Actor target = ActorManager.Singleton.Lookup(TargetID);
        if (target == null)
        {
            Debug.LogWarning("target is not exist, id is " + TargetID);
            return;
        }
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        if (info == null)
        {
            Debug.LogWarning("buff is not exist, id is " + BuffID);
            return;
        }
        //foreach (var item in info.BuffResultList)
        //{
        //    if (item.ID == (int)ClassID)
        //    {
        //        switch ((ENChangeBuffType)item.ParamList[0])
        //        {
        //            case ENChangeBuffType.enLength:
        //                {
        //                    foreach (var buff in target.MyBuffControl.BuffList)
        //                    {
        //                        BuffInfo buffInfo = GameTable.BuffTableAsset.Lookup(buff.BuffID);
        //                        if (buffInfo == null)
        //                        {
        //                            continue;
        //                        }
        //                        if (buffInfo.BuffType != (int)item.ParamList[1])
        //                        {
        //                            continue;
        //                        }
        //                        buff.BuffTimer += item.ParamList[2];
        //                    }
        //                }
        //                break;
        //        }
        //    }
        //}
        IsNeedResultPass = true;
    }
    public override void OnProduceResult(IResult result, IResultControl control)
    {
        base.OnProduceResult(result, control);
        if (result.ClassID == (int)ENResult.AddBuff)
        {
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            if (info == null)
            {
                Debug.LogWarning("buff is not exist, id is " + BuffID);
                return;
            }
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    switch ((ENChangeBuffType)item.ParamList[0])
                    {
                        case ENChangeBuffType.enSelfLength:
                            {
                                ResultAddBuff abResult = result as ResultAddBuff;
                                for (int i = 0; i < abResult.BuffIDList.Length; ++i)
                                {
                                    int buffID = (int)abResult.BuffIDList[i];
                                    BuffInfo buffInfo = GameTable.BuffTableAsset.Lookup(buffID);
                                    if (buffInfo == null)
                                    {
                                        continue;
                                    }
                                    if (buffInfo.BuffType != (int)item.ParamList[1])
                                    {
                                        continue;
                                    }
                                    abResult.BuffModifyTimeList[i] += item.ParamList[2];
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
    public override void OnGetResult(IResult result, IResultControl control)
    {
        base.OnGetResult(result, control);
        if (result.ClassID == (int)ENResult.AddBuff)
        {
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            if (info == null)
            {
                Debug.LogWarning("buff is not exist, id is " + BuffID);
                return;
            }
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    switch ((ENChangeBuffType)item.ParamList[0])
                    {
                        case ENChangeBuffType.enTargetLength:
                            {
                                ResultAddBuff abResult = result as ResultAddBuff;
                                for (int i = 0; i < abResult.BuffIDList.Length; ++i)
                                {
                                    int buffID = (int)abResult.BuffIDList[i];
                                    BuffInfo buffInfo = GameTable.BuffTableAsset.Lookup(buffID);
                                    if (buffInfo == null)
                                    {
                                        continue;
                                    }
                                    if (buffInfo.BuffType != (int)item.ParamList[1])
                                    {
                                        continue;
                                    }
                                    abResult.BuffModifyTimeList[i] += item.ParamList[2];
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}