using UnityEngine;

//控制移动
class BuffEffectControlMove : IBuffEffect
{
    ActorAction m_action = null;
    public BuffEffectControlMove()
        : base(ENBuff.ControlMove)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectControlMove();
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
            Debug.LogWarning("buff is not exist, id is " + BuffID);
            return;
        }
        foreach (var item in info.BuffResultList)
        {
            if (item.ID == (int)ClassID)
            {
                m_action = actor.ActionControl.AddAction(ControlMoveAction.SGetActionType()) as ControlMoveAction;
                if (null != m_action)
                {
                    ControlMoveAction cmAction = m_action as ControlMoveAction;
                    cmAction.Init((ControlMoveAction.ENAnimType)item.ParamList[0]);
                }
                else
                {
                    Debug.LogWarning("add ControlMoveAction failed");
                }
            }
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