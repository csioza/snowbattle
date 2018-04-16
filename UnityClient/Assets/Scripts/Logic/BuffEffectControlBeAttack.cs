using UnityEngine;

//控制受击
class BuffEffectControlBeAttack : IBuffEffect
{
    ActorAction m_action = null;
    public BuffEffectControlBeAttack()
        : base(ENBuff.ControlBeAttack)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectControlBeAttack();
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
        m_action = actor.ActionControl.AddAction(ControlBeAttackAction.SGetActionType());
        if (null == m_action)
        {
            Debug.LogWarning("add ControlBeAttackAction failed, buff id is " + BuffID);
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