//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Actor\ActorAction
//	created:	2013-4-18
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

//霸体
public class UndownAction : ActorAction
{
	public override ENType GetActionType() { return ENType.enUndownAction; }
	public static ENType SGetActionType() { return ENType.enUndownAction; }

    public float ActionLength { get; private set; }

    public void Init(float length)
    {
        AnimStartTime = Time.time;
        ActionLength = length;
    }

	public override void OnEnter()
	{
        RefreshActionRef();
	}

	public override bool OnUpdate()
    {
        return Time.time - AnimStartTime > ActionLength;
	}

};
