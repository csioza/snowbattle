using UnityEngine;

//光环
public class BuffEffectHalo : IBuffEffect
{
    public enum HaloType
    {
        enNull,
        enHaloAddBuff,
    }
    Actor m_selfActor = null;
    BuffInfo m_buffInfo = null;
    public BuffEffectHalo()
        : base(ENBuff.Halo)
    {
        IsNeedResultPass = true;
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectHalo();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
        m_selfActor = ActorManager.Singleton.Lookup(TargetID);
        if (m_selfActor == null)
        {
            Debug.LogWarning("actor is null, actor id is " + TargetID);
            IsNeedResultPass = false;
            return;
        }
        m_buffInfo = GameTable.BuffTableAsset.Lookup(BuffID);
        if (m_buffInfo == null)
        {
            Debug.LogWarning("buff is null, buff id is " + BuffID);
            IsNeedResultPass = false;
            return;
        }
        foreach (var item in m_buffInfo.BuffResultList)
        {
            if (item.ID == (int)ClassID)
            {
                switch ((HaloType)item.ParamList[0])
                {
                    case HaloType.enNull:
                        break;
                    case HaloType.enHaloAddBuff:
                        {
                            IsNeedTick = true;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public override void Exec(IResultControl control)
    {
        foreach (var item in m_buffInfo.BuffResultList)
        {
            if (item.ID == (int)ClassID)
            {
                switch ((HaloType)item.ParamList[0])//选择光环类型
                {
                    case HaloType.enNull:
                        break;
                    case HaloType.enHaloAddBuff:

                        ActorManager.Singleton.ForEach_buff(CheckActorDistance, item);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    void CheckActorDistance(Actor target, BuffResultInfo info)
    {
        if (target.Type == ActorType.enNPC)
        {
            NPC npc = target as NPC;
            if (npc.GetNpcType() == ENNpcType.enBlockNPC ||
                npc.GetNpcType() == ENNpcType.enFunctionNPC)
            {
                return;
            }
        }
        else if (target.Type == ActorType.enMain)
        {
            if (target.IsActorExit)
            {
                return;
            }
        }
        if (target.IsDead)
        {
            return;
        }

        switch ((ENTargetType)info.ParamList[2])//判断技能目标类型
        {
            case ENTargetType.enEnemy:
                {
                    if (!ActorTargetManager.IsEnemy(m_selfActor, target))
                    {
                        return;
                    }
                }
                break;
            case ENTargetType.enFriendly:
                {
                    if (!ActorTargetManager.IsFriendly(m_selfActor, target))
                    {
                        return;
                    }
                }
                break;
            case ENTargetType.enSelf:
                {
                    if (m_selfActor != target)
                    {
                        return;
                    }
                }
                break;
            case ENTargetType.enNullTarget:
                break;
            case ENTargetType.enFriendlyAndSelf:
                {
                    if (!ActorTargetManager.IsFriendly(m_selfActor, target) && m_selfActor != target)
                    {
                        return;
                    }
                }
                break;
            default:
                break;
        }
        float distance = ActorTargetManager.GetTargetDistance(m_selfActor.RealPos, target.RealPos);
        bool tmpFlag = true;
        if (distance <= info.ParamList[1])//判断光环半径
        {
            foreach (Buff tmpBuf in target.MyBuffControl.BuffList)
            {
                if (tmpBuf.BuffID == (int)info.ParamList[2])
                {
                    tmpFlag = false;
                }
            }
            if (tmpFlag)
            {
                IResult addBuffResult = BattleFactory.Singleton.CreateResult(ENResult.AddBuff, m_selfActor.ID, target.ID, 0, 0, new float[1] { info.ParamList[3] });
                if (addBuffResult != null)
                {
                    addBuffResult.ResultExpr(new float[1] { info.ParamList[3] });//添加技能ID
                    BattleFactory.Singleton.DispatchResult(addBuffResult);
                }
            }
        }
        else
        {
            tmpFlag = false;
            foreach (Buff tmpBuf in target.MyBuffControl.BuffList)
            {
                if (tmpBuf.BuffID == info.ParamList[2])
                {
                    tmpFlag = true;
                }
            }
            if (tmpFlag)
            {
                IResult rbResult = BattleFactory.Singleton.CreateResult(ENResult.RemoveBuff, m_selfActor.ID, target.ID, 0, 0, new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffID, info.ParamList[3] });
                if (rbResult != null)
                {
                    rbResult.ResultExpr(new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffID, info.ParamList[3] });
                    BattleFactory.Singleton.DispatchResult(rbResult);
                }
            }
        }
               
        
    }
}