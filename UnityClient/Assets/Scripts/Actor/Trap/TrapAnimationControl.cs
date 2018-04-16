using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class TrapAnimationControl
{
    private Trap mActor;
    private string mLastAnimName = "";

    public uint[] mLastActionRefCountDict = new uint[(int)ActorAction.ENType.Count];
    public TrapAnimationControl(Trap actor)
    {
        mActor = actor;
    }
    public void Reset()
    {
        mLastAnimName = "";
    }
    bool IsNewAction(TrapAction.ENType actionType)
    {
        TrapAction action = mActor.mActionControl.LookupAction(actionType);
        if (null == action)
        {
            return true;
        }
        uint curRefCount = mActor.mActionRefCountDict[(int)action.GetActionType()];
        uint lastRefCount = mLastActionRefCountDict[(int)action.GetActionType()];
        mLastActionRefCountDict[(int)action.GetActionType()] = curRefCount;
        return (curRefCount != lastRefCount);
    }
    public void Tick()
    {
        string strAnim = "standby";
//        TrapAction.ENType curActionType = TrapAction.ENType.enNone;
//        Vector3 mDirection = Vector3.zero;
        if (mActor.mActionControl.IsActionRunning(TrapAction.ENType.enAttackAction))
        {
            TrapAttackAction action = mActor.mActionControl.LookupAction(TrapAction.ENType.enAttackAction) as TrapAttackAction;
//            bool isNewAction = IsNewAction(TrapAttackAction.ENType.enAttackAction);
            string curActionAnimName = action.GetAnimationName();
            if (curActionAnimName != mLastAnimName)
            {
                mLastAnimName = curActionAnimName;
                float animLength = mActor.PlayAnimation(curActionAnimName);
                action.AnimStartTime = Time.time;
                action.AnimLength = animLength;
            }
            return;
        }
        else if (mActor.mActionControl.IsActionRunning(TrapAction.ENType.enStandAction))
        {
            TrapStandAction action = mActor.mActionControl.LookupAction(TrapAction.ENType.enStandAction) as TrapStandAction;
            string curActionAnimName = action.GetAnimationName();
            if (curActionAnimName != mLastAnimName)
            {
                mLastAnimName = curActionAnimName;
                float animLength = mActor.PlayAnimation(curActionAnimName);
                action.AnimStartTime = Time.time;
                action.AnimLength = animLength;
            }
            return;
        }
        if (strAnim == "")
        {
            return;
        }
        if (strAnim == "standby")
        {
            if (mActor.IsState_ReadyToFight)
            {
                strAnim = "readytofight";
            }
        }

    }
}