using System;
using UnityEngine;
public class TrapAttackActionCallback : MonoBehaviour
{
    void ChangeResultID(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp)
        {
            return;
        }
        TrapAttackAction action = (selfProp.ActorLogicObj as Trap).mActionControl.LookupAction(TrapAction.ENType.enAttackAction) as TrapAttackAction;
        if (null != action)
        {
            action.mSkillResultID = animEvent.intParameter;
        }
    }
}