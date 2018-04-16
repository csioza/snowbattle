using System;
using UnityEngine;
using System.Collections.Generic;

//退场
public class JumpoutAction : ActorAction
{
    public override ENType GetActionType() { return ENType.enJumpoutAction; }
    public static ENType SGetActionType() { return ENType.enJumpoutAction; }

    public override void OnEnter()
    {
        if (CurrentActor.m_isDeposited)
        {//托管中，发送beattack action的消息
            Vector3 pos = CurrentActor.MainPos;
            IMiniServer.Singleton.SendAction_JumpOut_C2BS(CurrentActor.ID, pos.x, pos.z);
        }
        CurrentActor.MainAnim.cullingType = AnimationCullingType.AlwaysAnimate;
        RefreshActionRef();

        if (!CurrentActor.MainRigidBody.isKinematic)
        {
            CurrentActor.MainRigidBody.velocity = Vector3.zero;
        }
        MainGame.FaceToCameraWithoutY(CurrentActor.MainObj);
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
        CurrentActor.GetBodyObject().transform.localPosition = Vector3.zero;
        CurrentActor.MainAnim.cullingType = AnimationCullingType.BasedOnRenderers;
        CurrentActor.HideMe();
    }

    public override bool OnUpdate()
    {
        return Time.time - AnimStartTime > AnimLength;
    }
};