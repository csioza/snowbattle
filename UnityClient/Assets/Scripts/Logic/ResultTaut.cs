using UnityEngine;
using NGE.Network;

//嘲讽
public class ResultTaut : IResult
{
    Player m_self = null;
    public ResultTaut()
		:base((int)ENResult.Taut)
    {
    }

    public static IResult CreateNew()
    {
        return new ResultTaut();
    }

    public override void ResultExpr(float[] param)
    {
        base.ResultExpr(param);
    }

    public override void Exec(IResultControl control)
    {
        base.Exec(control);
        Actor source = ActorManager.Singleton.Lookup(SourceID);
        if (source == null)
        {
            Debug.LogWarning("actor is not exist, id is " + SourceID);
            return;
        }
        if (source.Type == ActorType.enNPC)
        {
            Debug.LogWarning("npc can not taut");
            return;
        }
        m_self = source as Player;
        ActorManager.Singleton.ForEach(ChangeTarget);
    }
    void ChangeTarget(Actor target)
    {
        if (target.IsDead)
        {
            return;
        }
        if (ActorTargetManager.IsEnemy(m_self, target))
        {
            float d = ActorTargetManager.GetTargetDistance(m_self.MainPos, target.MainPos);
            if (m_self.CurrentTableInfo.AttackRange > d)
            {//改变目标
                target.CurrentTarget = m_self;
            }
        }
    }

    public override void ResultServerExec(IResultControl control)
    {
        base.ResultServerExec(control);

        Actor source = ActorManager.Singleton.Lookup(SourceID);
        if (source == null)
        {
            Debug.LogWarning("actor is not exist, id is " + SourceID);
            return;
        }
        if (source.Type == ActorType.enNPC)
        {
            Debug.LogWarning("npc can not taut");
            return;
        }
        m_self = source as Player;
        ActorManager.Singleton.ForEach(ChangeTarget);
    }
}