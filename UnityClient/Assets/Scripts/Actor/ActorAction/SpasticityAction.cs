//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Actor\ActorAction
//	created:	2013-4-18
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

//被动僵直
public class SpasticityAction : ActorAction
{
	public override ENType GetActionType() { return ENType.enSpasticityAction; }
	public static ENType SGetActionType() { return ENType.enSpasticityAction; }
	public float LastTime { get; set; }
	private float m_startTime;
	public override void OnEnter()
	{
		m_startTime = Time.time;
        RefreshActionRef();
	}

	public override bool OnUpdate()
	{
		return Time.time - m_startTime > LastTime;
	}


	public override void Reset()
	{
		base.Reset();
		m_startTime = 0.0f;
		LastTime = 0.0f;
	}
};