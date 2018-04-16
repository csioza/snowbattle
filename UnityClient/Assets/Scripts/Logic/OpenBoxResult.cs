using UnityEngine;
using System.Collections;
using NGE.Network;

public class OpenBoxResult : IResult
{
    public OpenBoxResult()
        : base((int)ENResult.OpenBox)
    {
    }
    public static IResult CreateNew()
    {
        return new OpenBoxResult();
    }
    public override void Exec(IResultControl control)
    {
        base.Exec(control);
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            return;
        }
        if (actor.Type == ActorType.enNPCTrap)
        {//机关
            ControlTrap tmpActor = actor as ControlTrap;
            tmpActor.OperateOpenAnim();
            ActorManager.Singleton.Lookup(SourceID).TargetManager.CurrentTarget = null;
        }
        else if (actor.Type == ActorType.enNPC)
        {
            NPC npc = actor as NPC;
            if (npc.GetNpcType() == ENNpcType.enBoxNPC)
            {//宝箱
                IResult r = BattleFactory.Singleton.CreateResult(ENResult.Dead, SourceID, TargetID);
                if (r != null)
                {
                    r.ResultExpr(null);
                    BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
                }
            }
        }
        
    }

    public override void ResultServerExec(IResultControl control)
    {
        base.ResultServerExec(control);
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            return;
        }
        if (actor.Type == ActorType.enNPCTrap)
        {//机关
            ControlTrap tmpActor = actor as ControlTrap;
            tmpActor.OperateOpenAnim();
            ActorManager.Singleton.Lookup(SourceID).TargetManager.CurrentTarget = null;
        }
        else if (actor.Type == ActorType.enNPC)
        {
        }

    }
}