using System;
using UnityEngine;
using System.Collections.Generic;

//主控角色退场
public class ActorExitAction : ActorAction
{
    public override ENType GetActionType() { return ENType.enActorExitAction; }
    public static ENType SGetActionType() { return ENType.enActorExitAction; }


    public override void OnEnter()
    {
        if (CurrentActor.m_isDeposited)
        {//托管中，发送beattack action的消息
			Vector3 pos = CurrentActor.MainPos;
            IMiniServer.Singleton.SendAction_ActorExit_C2BS(CurrentActor.ID, pos);
        }
        RefreshActionRef();
        //effect
        CurrentActor.PlayEffect("ef-e-switchout-E01", 0, "", false, Vector3.zero);
        if (!CurrentActor.MainRigidBody.isKinematic)
        {
            CurrentActor.MainRigidBody.velocity = Vector3.zero;
        }
        if (CurrentActor.IsDead)
        {//角色死亡
            CurrentActor.HideMe();
        }
        //MainGame.FaceToCameraWithoutY(CurrentActor.MainObj);
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
        CurrentActor.HideMe();
        CurrentActor.EnableCollider(true);
    }

    public override bool OnUpdate()
    {
        return Time.time - AnimStartTime > AnimLength;
    }
};