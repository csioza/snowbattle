using System;
using UnityEngine;

//不能受击
public class ControlBeAttackAction : ActorAction
{
    public override ENType GetActionType() { return ENType.enControlBeAttackAction; }
    public static ENType SGetActionType() { return ENType.enControlBeAttackAction; }

    public override void OnEnter()
    {
        RefreshActionRef();
    }

    public override bool OnUpdate()
    {
        return false;
    }
}