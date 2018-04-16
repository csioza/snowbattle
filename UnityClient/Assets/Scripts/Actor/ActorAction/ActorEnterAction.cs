using System;
using UnityEngine;
using System.Collections.Generic;

//主控角色入场
public class ActorEnterAction : ActorAction
{
    public override ENType GetActionType() { return ENType.enActorEnterAction; }
    public static ENType SGetActionType() { return ENType.enActorEnterAction; }

    public override void OnEnter()
    {
        if (CurrentActor.m_isDeposited)
        {//托管中，发送beattack action的消息
			Vector3 pos = CurrentActor.MainPos;
			Vector3 forward = CurrentActor.MainObj.transform.forward;
            IMiniServer.Singleton.SendAction_ActorEnter_C2BS(CurrentActor.ID, pos, forward);
        }
        SyncPositionIfNeed();
        RefreshActionRef();

        //effect
        //CurrentActor.PlayEffect("ef-e-switchout-E01", 0, "", false, Vector3.zero);
        //朝向camera
        //MainGame.FaceToCameraWithoutY(CurrentActor.MainObj);
        CurrentActor.MainAnim.cullingType = AnimationCullingType.AlwaysAnimate;
    }

    public override void OnInterupt()
    {
        if (CurrentActor.m_isDeposited)
        {//托管中，发送action被打断的消息
            IMiniServer.Singleton.SendActionInterupt_C2BS(CurrentActor.ID, (int)GetActionType());
        }
        OnExit();
    }

    public override void OnExit()
    {
        CurrentActor.EnableCollider(true);
        CurrentActor.MainAnim.cullingType = AnimationCullingType.BasedOnRenderers;
    }

    public override bool OnUpdate()
    {
        if (!CurrentActor.MainRigidBody.isKinematic)
        {
            CurrentActor.MainRigidBody.velocity = Vector3.zero;
        }
        //PlayAnimation时会更改MainPos的位置为RealPos，所以，要一直设置MainPos的位置，不让其改变（因为RealPos有可能不对）
        CurrentActor.UnhideMe(CurrentActor.MainPos);
        return Time.time - AnimStartTime > AnimLength;
    }
};