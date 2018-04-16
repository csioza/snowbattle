using UnityEngine;

//冻结
class BuffEffectControlAttack : IBuffEffect
{
    ActorAction m_action = null;
    public BuffEffectControlAttack()
        : base(ENBuff.ControlAttack)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectControlAttack();
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
        m_action = actor.ActionControl.AddAction(ControlAttackAction.SGetActionType());
        if (null == m_action)
        {
            Debug.LogWarning("add ControlAttackAction failed, buff id is " + BuffID);
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
        actor.ActionControl.RemoveAction(m_action);
    }
}