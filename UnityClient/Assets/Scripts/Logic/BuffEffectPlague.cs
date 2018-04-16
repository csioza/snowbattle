using UnityEngine;

//瘟疫
class BuffEffectPlague : IBuffEffect
{
    enum ENPlagueType
    {
        enNone,
        enInfect,//传染
    }
    public BuffEffectPlague()
        : base(ENBuff.Plague)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectPlague();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        if (info == null)
        {
            Debug.LogWarning("buff is not exist! buff id=" + BuffID.ToString());
            return;
        }
        foreach (var item in info.BuffResultList)
        {
            if (item.ID == (int)ClassID)
            {
                switch ((ENPlagueType)item.ParamList[0])
                {
                    case ENPlagueType.enInfect:
                        {
                            //给自己添加buff
                            IResult r = BattleFactory.Singleton.CreateResult(ENResult.AddBuff, TargetID, TargetID,0, 0, new float[1] { item.ParamList[1] });
                            if (r != null)
                            {
                                r.ResultExpr(new float[1] { item.ParamList[1] });
                                BattleFactory.Singleton.DispatchResult(r);
                            }
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
        if (result.ClassID == (int)ENResult.Dead)
        {
            m_self = ActorManager.Singleton.Lookup(TargetID);
            if (m_self == null)
            {
                Debug.LogWarning("actor is not exist, actor id is " + TargetID);
                return;
            }
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            if (info == null)
            {
                Debug.LogWarning("buff is not exist! buff id=" + BuffID.ToString());
                return;
            }
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    switch ((ENPlagueType)item.ParamList[0])
                    {
                        case ENPlagueType.enInfect:
                            {
                                m_target = null;
                                ActorManager.Singleton.ForEach(FindFriend);
                                if (m_target != null)
                                {
                                    //给友方添加buff
                                    IResult r = BattleFactory.Singleton.CreateResult(ENResult.AddBuff, TargetID, m_target.ID, 0, 0, new float[1] { item.ParamList[2] });
                                    if (r != null)
                                    {
                                        r.ResultExpr(new float[1] { item.ParamList[2] });
                                        BattleFactory.Singleton.DispatchResult(r);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
    Actor m_self = null;
    Actor m_target = null;
    void FindFriend(Actor target)
    {
        if (m_target != null)
        {
            return;
        }
        if (target.IsDead)
        {
            return;
        }
        if (ActorTargetManager.IsFriendly(m_self, target))
        {
            float d = ActorTargetManager.GetTargetDistance(m_self.MainPos, target.MainPos);
            float attackRange = 0;
            if (m_self.Type == ActorType.enNPC)
            {
                attackRange = (m_self as NPC).CurrentTableInfo.AttackRange;
            }
            else
            {
                attackRange = (m_self as Player).CurrentTableInfo.AttackRange;
            }
            if (d < attackRange)
            {//攻击范围内
                m_target = target;
            }
        }
    }
}