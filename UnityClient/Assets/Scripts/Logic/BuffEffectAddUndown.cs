using System;
using System.Collections;
using UnityEngine;

class BuffEffectAddUndown : IBuffEffect
{
    ActorAction m_action = null;
    public BuffEffectAddUndown()
        : base(ENBuff.AddUndown)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectAddUndown();
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
        m_action = actor.ActionControl.AddAction(UndownAction.SGetActionType());
        if (null == m_action)
        {
            Debug.LogWarning("add UndownAction failed, buff id is " + BuffID);
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