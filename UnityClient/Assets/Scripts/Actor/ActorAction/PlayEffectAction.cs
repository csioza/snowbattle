//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Actor\ActorAction
//	created:	2013-4-18
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

//播放特效
public class PlayEffectAction : ActorAction
{
	public override ENType GetActionType() { return ENType.enPlayEffectAction; }
	public static ENType SGetActionType() { return ENType.enPlayEffectAction; }

	public override void OnEnter()
	{
        RefreshActionRef();
	}

	public override bool OnUpdate()
	{
		return true;
	}

};