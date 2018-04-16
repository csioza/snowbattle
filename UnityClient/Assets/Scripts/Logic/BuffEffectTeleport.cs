using System;
using UnityEngine;

//瞬移
public class BuffEffectTeleport : IBuffEffect
{
    public BuffEffectTeleport()
        : base(ENBuff.Teleport)
    {
        ;
    }
    static public IBuffEffect CreateNew()
    {
        return new BuffEffectTeleport();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
        Actor self = ActorManager.Singleton.Lookup(TargetID);
        if (self == null)
        {
            Debug.LogWarning("actor is null, id is " + TargetID);
            return;
        }
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        if (info == null)
        {
            Debug.LogWarning("buff is null, id is " + BuffID);
            return;
        }
        foreach (var item in info.BuffResultList)
        {
            if (item.ID == (int)ClassID)
            {
                switch ((Teleport.ENTeleportType)item.ParamList[0])
                {
                    case Teleport.ENTeleportType.enTimes:
                        {
                            self.SelfTeleport = new Teleport((int)item.ParamList[1]);
                            return;
                        }
//                        break;
                }
            }
        }
    }
    public override void OnRemoved(IResultControl control)
    {
        base.OnRemoved(control);
        Actor self = ActorManager.Singleton.Lookup(TargetID);
        if (self == null)
        {
            Debug.LogWarning("actor is null, id is " + TargetID);
            return;
        }
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        if (info == null)
        {
            Debug.LogWarning("buff is null, id is " + BuffID);
            return;
        }
        foreach (var item in info.BuffResultList)
        {
            if (item.ID == (int)ClassID)
            {
                self.SelfTeleport.Remove((Teleport.ENTeleportType)item.ParamList[0]);
                return;
            }
        }
    }
}