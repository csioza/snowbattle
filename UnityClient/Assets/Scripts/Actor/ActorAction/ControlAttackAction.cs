using System;
using UnityEngine;

//不能攻击
public class ControlAttackAction : ActorAction
{
    public override ENType GetActionType() { return ENType.enControlAttackAction; }
    public static ENType SGetActionType() { return ENType.enControlAttackAction; }
    
    public override void OnEnter()
    {
        RefreshActionRef();
    }

    public override bool OnUpdate()
    {
        return false;
    }
}