using System;
using System.Collections.Generic;
using NGE.Network;

public class SResultDead : ResultDead
{
    public SResultDead()
    {

    }
    public override void Serialize(PacketWriter stream)
    {
        base.Serialize(stream);
    }
    public override void Exec(IResultControl control)
    {
        base.Exec(control);
        //IBattleObject actorSrc = control.LookupGameObject(SourceID);
        //IBattleObject actorTarget = control.LookupGameObject(TargetID);
        //if (null == actorTarget)
        //{
        //    return;
        //}
        //if (ActorType.enNPC == actorTarget.Type)
        //{
        //    //TryDropItemHolder();
        //}
        //if (ActorType.enMain == actorTarget.Type)
        //{
        //    //ReturnToTown();
        //}
        //else if (ActorType.enNPC == actorTarget.Type)
        //{
            
        //}
    }
}