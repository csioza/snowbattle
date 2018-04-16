using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class AnimationControl
{
    private Actor   mActor;
    private string mLastAnimName = "";
    
    public uint[] mLastActionRefCountDict = new uint[(int)ActorAction.ENType.Count];
    public AnimationControl(Actor actor)
    {
        mActor = actor;
    }
    public void Reset()
    {
        mLastAnimName = "";
    }
    bool IsNewAction(ActorAction.ENType actionType)
    {
        ActorAction action = mActor.ActionControl.LookupAction(actionType);
        if (null == action)
        {
            return true;
        }
        uint curRefCount = mActor.mActionRefCountDict[(int)action.GetActionType()];
        //mActor.mActionRefCountDict.TryGetValue(action.GetActionType(), out curRefCount);
        uint lastRefCount = mLastActionRefCountDict[(int)action.GetActionType()];
        mLastActionRefCountDict[(int)action.GetActionType()] = curRefCount;
//         if (mLastActionRefCountDict.TryGetValue(action.GetActionType(), out lastRefCount))
//         {
//             mLastActionRefCountDict[action.GetActionType()] = curRefCount;
//         }
//         else
//         {
//             mLastActionRefCountDict.Add(action.GetActionType(), curRefCount);
//         }
        return (curRefCount != lastRefCount);
    }
    public void Tick()
    {
        string strAnim = "standby";
        ActorAction.ENType curActionType = ActorAction.ENType.enNone;
		Vector3 mDirection = Vector3.zero;
        Vector3 syncPos = Vector3.zero;
        if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enBeAttackAction))
        {
            BeAttackAction action = mActor.ActionControl.LookupAction(ActorAction.ENType.enBeAttackAction) as BeAttackAction;
            bool isNewAction = IsNewAction(ActorAction.ENType.enBeAttackAction);
            if (action.mStrAnim != mLastAnimName || isNewAction)
            {
                bool isHitedFly = false;
                if (action.m_isBack)
                {
                    isHitedFly = action.m_isFly;
                }
                mLastAnimName = action.mStrAnim;
                string animFullName = "";
                float animLength = mActor.PlayAnimation(action.mStrAnim, mDirection, out animFullName, syncPos);
                if (action.fBlendFactor > 0f)
                {
                    mActor.BlendAnimation(action.mAniBlend, action.fBlendFactor);
                }
                if (isHitedFly)
                {
                    mActor.BlendAnimation("knock", 1.0f);
                }
                if (isNewAction)
                {
                    action.AnimStartTime = Time.time;
                    action.AnimLength = animLength;
                    action.AnimFullName = animFullName;
                }
            }
            return;
        }
        else if (mActor.ActionControl.IsActionRunning(AttackingMoveAction.SGetActionType()))
        {
            AttackingMoveAction action = mActor.ActionControl.LookupAction(AttackingMoveAction.SGetActionType()) as AttackingMoveAction;
            strAnim = action.AnimName;

            curActionType = AttackingMoveAction.SGetActionType();
        }
        else if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enAttackAction))
        {
            AttackAction action = mActor.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
            bool isNewAction = IsNewAction(ActorAction.ENType.enAttackAction);
            if (isNewAction)
            {
                if (!action.m_isPlay)
                {
                    strAnim = "standby";
                }
                else
                {
                    if (null != action.m_skillTarget)
                    {
                        mDirection = action.m_skillTarget.RealPos - mActor.RealPos;
                    }
                    strAnim = action.GetAnimationName();
                }
                if (strAnim == "standby")
                {
//                     if (mActor.IsState_ReadyToFight)
//                     {
//                         strAnim = "readytofight";
//                     }
                }
                mLastAnimName = strAnim;
                string animFullName = "";
                if (action.IsSyncPosition)
                {
                    syncPos = action.SyncPosition;
                }
                float animLength = mActor.PlayAnimation(strAnim, mDirection, out animFullName, syncPos);
                if (isNewAction)
                {
                    action.AnimStartTime = Time.time;
                    action.AnimLength = animLength;
                    action.AnimFullName = animFullName;
                }
                AttackAction aAction = action as AttackAction;// mActor.ActionControl.LookupAction(curActionType) as AttackAction;
//                AttackAction.AttackStepInfo info = aAction.GetAttackStepInfo();
                if (aAction.m_isPlay)
                {//攻击中
                    if (aAction.m_skillTarget != null)
                    {//有技能目标
                        float targetWeight = 0;
                        string blendAnimName = "";
                        Vector3 direction = aAction.m_skillTarget.RealPos - mActor.RealPos;
                        direction.y = 0f;
                        if (aAction.m_skillInfo.SlashMotion == strAnim)
                        {//计算冲刺的权重
                            if (aAction.m_skillInfo.SlashMotionDistance > 0)
                            {
                                targetWeight = (direction.magnitude + aAction.m_skillInfo.SlashTargetPosOffset) / aAction.m_skillInfo.SlashMotionDistance;
                                blendAnimName = aAction.m_skillInfo.SlashBlendMotionName;
                            }
                        }
                        else if (aAction.m_skillInfo.ReleaseMotion == strAnim)
                        {//计算释放的权重
                            if (aAction.m_skillInfo.ReleaseMotionDistance > 0)
                            {
                                targetWeight = (direction.magnitude + aAction.m_skillInfo.ReleaseTargetPosOffset) / aAction.m_skillInfo.ReleaseMotionDistance;
                                blendAnimName = aAction.m_skillInfo.ReleaseBlendMotionName;
                            }
                        }

                        if (!string.IsNullOrEmpty(blendAnimName))
                        {//动作融合
                            if (targetWeight < 0)
                            {
                                targetWeight = 0;
                            }
                            mActor.BlendAnimation(strAnim, targetWeight, 0);
                            mActor.BlendAnimation(blendAnimName, 1 - targetWeight, 0);
                        }
                    }
                }
                SelfSpasticityAction spAction = mActor.ActionControl.AddAction(ActorAction.ENType.enSelfSpasticityAction) as SelfSpasticityAction;
                if (null != spAction)
                {
                    spAction.AnimLength = animLength;
                }
            }
            FakeBeAttackAction beFakeAction = mActor.ActionControl.LookupAction(ActorAction.ENType.enFakeBeAttackAction) as FakeBeAttackAction;
            if (null != beFakeAction && IsNewAction(ActorAction.ENType.enFakeBeAttackAction))
            {
                if ((action.IsNormalAttack()))
                {
                    mActor.BlendAnimation(beFakeAction.mStrAnim, GameSettings.Singleton.m_fakeBlendWeightValue, GameSettings.Singleton.m_fakeBlendFadeTime, AnimationBlendMode.Additive);
                }
            }
            return;
        }
        else if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enFakeBeAttackAction))
        {
            FakeBeAttackAction action = mActor.ActionControl.LookupAction(ActorAction.ENType.enFakeBeAttackAction) as FakeBeAttackAction;
            bool isNewAction = IsNewAction(ActorAction.ENType.enFakeBeAttackAction);
            if (action.mStrAnim != mLastAnimName || isNewAction)
            {
                mLastAnimName = action.mStrAnim;
                if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enMoveAction))
                {
                    mActor.BlendAnimation(action.mStrAnim, GameSettings.Singleton.m_fakeBlendWeightValue, GameSettings.Singleton.m_fakeBlendFadeTime, AnimationBlendMode.Additive);
                }
                else
                {
                    string animFullName = "";
                    float animLength = mActor.PlayAnimation(action.mStrAnim, mDirection, out animFullName, syncPos);
                    if (action.fBlendFactor > 0f)
                    {
                        mActor.BlendAnimation(action.mAniBlend, action.fBlendFactor);
                    }
                    action.AnimStartTime = Time.time;
                    action.AnimLength = animLength;
                    action.AnimFullName = animFullName;
                }
            }
            return;
        }
        else if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enMoveAction))
        {
            MoveAction action = mActor.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
            strAnim = action.AnimName;
            float fDiff = action.mRealSpeed / action.m_currentSpeed;
            if (action.mIsRotating)
            {
                //strAnim = "standby";
            }
            else if ((fDiff <= 0.1f || action.m_currentSpeed == 0))
            {
                strAnim = "standby";
            }
            bool isNewAction = IsNewAction(ActorAction.ENType.enMoveAction);
            if (strAnim != mLastAnimName || isNewAction)
            {
//                 if (strAnim == "run")
//                 {
//                     float fBase = mActor.PropConfig.RunBaseSpeed - mActor.PropConfig.WalkBaseSpeed;
//                     float fPercent = (mActor.MoveSpeed - mActor.PropConfig.WalkBaseSpeed) / fBase;
//                     if (fPercent <= 0.0f || fPercent <= 1 / fBase)
//                     {
//                         strAnim = "walk";
//                     }
//                 }
//                 if (strAnim == "standby")
//                 {
//                     if (mActor.IsState_ReadyToFight)
//                     {
//                         strAnim = "readytofight";
//                     }
//                 }
                mLastAnimName = strAnim;
                string animFullName = "";
                float animLength = mActor.PlayAnimation(strAnim, mDirection, out animFullName, syncPos);
                if (isNewAction)
                {
                    action.AnimStartTime = Time.time;
                    action.AnimLength = animLength;
                    action.AnimFullName = animFullName;
                }
            }
            return;
            //Debug.Log("strAnim=" + strAnim + " realSpeed=" + action.mRealSpeed + " action.m_currentSpeed=" + action.m_currentSpeed);
        }
        else if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enStandAction))
        {
            StandAction action = mActor.ActionControl.LookupAction(StandAction.SGetActionType()) as StandAction;
            strAnim = action.AnimName;
            curActionType = ActorAction.ENType.enStandAction;
        }
        else if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enDeadAction))
        {
            strAnim = "dead";
            curActionType = ActorAction.ENType.enDeadAction;
        }
        else if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enReliveAction))
        {
            strAnim = "reborn";
            curActionType = ActorAction.ENType.enReliveAction;
        }
        else if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enAlertAction))
        {
            strAnim = "alert";
            curActionType = ActorAction.ENType.enAlertAction;
        }
        else if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enControlMoveAction))
        {//控制移动
            ControlMoveAction action = mActor.ActionControl.LookupAction(ControlMoveAction.SGetActionType()) as ControlMoveAction;
            strAnim = action.AnimName;
            curActionType = ActorAction.ENType.enControlMoveAction;
        }
        else if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enJumpinAction))
        {
            strAnim = "jumpin";
            curActionType = ActorAction.ENType.enJumpinAction;
        }
        else if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enJumpoutAction))
        {
            strAnim = "jumpout";
            curActionType = ActorAction.ENType.enJumpoutAction;
        }
        else if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enRollAction))
        {
            strAnim = "rolling";
            curActionType = ActorAction.ENType.enRollAction;
        }
        else if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enActorExitAction))
        {
            strAnim = "jumpout";
            curActionType = ActorAction.ENType.enActorExitAction;
        }
        else if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enActorEnterAction))
        {
            strAnim = "jumpin";
            curActionType = ActorAction.ENType.enActorEnterAction;
        }
        else if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enControlAttackAction))
        {//控制攻击
            strAnim = "standby";
            curActionType = ActorAction.ENType.enControlAttackAction;
        }
        else if (mActor.ActionControl.IsActionRunning(ActorAction.ENType.enControlBeAttackAction))
        {//控制受击
            strAnim = "standby";
            curActionType = ActorAction.ENType.enControlBeAttackAction;
        }
        if (strAnim == "")
        {
            return;
        }
        if (strAnim == "standby")
        {
//             if (mActor.IsState_ReadyToFight)
//             {
//                 strAnim = "readytofight";
//             }
        }
        bool isNewAction1 = IsNewAction(curActionType);
        if (strAnim != mLastAnimName || isNewAction1)
        {
            mLastAnimName = strAnim;
            ActorAction action = mActor.ActionControl.LookupAction(curActionType);
            if (action != null)
            {
                if (action.IsSyncPosition)
                {
                    syncPos = action.SyncPosition;
                }
            }

            string animFullName = "";
            float animLength = mActor.PlayAnimation(strAnim, mDirection, out animFullName, syncPos);

            if (action != null)
            {//设置animation播放的时间
                if (isNewAction1)
                {
                    action.AnimStartTime = Time.time;
                    action.AnimLength = animLength;
                    action.AnimFullName = animFullName;
                }
            }
        }
    }
}