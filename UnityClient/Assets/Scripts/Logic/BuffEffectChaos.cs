using UnityEngine;

//混乱
class BuffEffectChaos : IBuffEffect
{
    public BuffEffectChaos()
        : base(ENBuff.Chaos)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectChaos();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            Debug.LogWarning("OnGetBuffEffect failed! actor is not exist! actor id=" + TargetID.ToString());
            return;
        }
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        if (info == null)
        {
            Debug.LogWarning("OnGetBuffEffect failed! buff is not exist! buff id=" + BuffID.ToString());
            return;
        }
        if (actor.Type == ActorType.enNPC)
        {//此buff效果只对npc生效
            actor.CurrentTarget = null;//重新选择目标
            actor.TempType = ActorType.enNPC_AllEnemy;
        }
    }
    public override void OnRemoved(IResultControl control)
    {
        base.OnRemoved(control);
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            Debug.LogWarning("actor is not exist! actor id=" + TargetID.ToString());
            return;
        }
        actor.CurrentTarget = null;//重新选择目标
        actor.TempType = actor.Type;
    }
}
