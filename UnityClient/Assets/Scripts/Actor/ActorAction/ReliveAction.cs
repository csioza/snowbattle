//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Actor\ActorAction
//	created:	2013-4-18
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

//复活属于瞬时行为，所以OnUpdate返回true
public class ReliveAction : ActorAction
{
	public override ENType GetActionType() { return ENType.enReliveAction; }
	public static ENType SGetActionType() { return ENType.enReliveAction; }

	public override void OnEnter()
	{
		RefreshActionRef();
		CurrentActor.PlayEffect("ef-demo-reborn");
	}
	public override bool OnUpdate()
    {
        return Time.time - AnimStartTime > AnimLength;
	}

    public override void OnExit()
    {
        base.OnExit();
        CurrentActor.EnableCollider(true);
		CurrentActor.Props.SetProperty_Int32(ENProperty.islive, 1);
        CurrentActor.SetCurHP(CurrentActor.MaxHP);
    }
};