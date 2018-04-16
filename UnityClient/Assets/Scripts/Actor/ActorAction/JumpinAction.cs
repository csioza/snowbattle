using System;
using UnityEngine;
using System.Collections.Generic;

//入场
public class JumpinAction : ActorAction
{
    public override ENType GetActionType() { return ENType.enJumpinAction; }
    public static ENType SGetActionType() { return ENType.enJumpinAction; }

    //停留的位置
    Vector3 m_stayPos = Vector3.zero;
	public override void OnEnter()
    {
        if (CurrentActor.m_isDeposited)
        {//托管中，发送beattack action的消息
            //Vector3 pos = CurrentActor.MainPos;
			IMiniServer.Singleton.SendAction_JumpIn_C2BS(CurrentActor.ID, CurrentActor.TargetManager.CurrentTarget.ID);
        }
        if (CurrentActor.TargetManager.CurrentTarget != null && !CurrentActor.TargetManager.CurrentTarget.IsDead)
        {
            Vector3 forward = CurrentActor.TargetManager.CurrentTarget.MainObj.transform.forward;
            m_stayPos = CurrentActor.TargetManager.CurrentTarget.RealPos + forward.normalized;
            CurrentActor.UnhideMe(m_stayPos);
        }
        else
        {
            IsEnable = true;
            return;
        }

        MainGame.FaceToCameraWithoutY(CurrentActor.MainObj);

        CurrentActor.MainAnim.cullingType = AnimationCullingType.AlwaysAnimate;
        RefreshActionRef();
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
	}

	public override bool OnUpdate()
    {
        if (!CurrentActor.MainRigidBody.isKinematic)
        {
            CurrentActor.MainRigidBody.velocity = Vector3.zero;
        }
        //PlayAnimation时会更改MainPos的位置为RealPos，所以，要一直设置MainPos的位置，不让其改变（因为RealPos有可能不对）
        CurrentActor.UnhideMe(m_stayPos);
        return Time.time - AnimStartTime > AnimLength;
	}
};