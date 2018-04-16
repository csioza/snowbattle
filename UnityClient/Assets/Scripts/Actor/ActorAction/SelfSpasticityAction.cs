//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Actor\ActorAction
//	created:	2013-4-18
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

//主动僵直
public class SelfSpasticityAction : ActorAction
{
	public override ENType GetActionType() { return ENType.enSelfSpasticityAction; }
	public static ENType SGetActionType() { return ENType.enSelfSpasticityAction; }

    private float m_changeDuration = 0;
    bool m_isChangeDuration = false;
	public override void OnEnter()
	{
        RefreshActionRef();
        AnimStartTime = Time.time;
	}

	public override bool OnUpdate()
	{
        if (Time.time - AnimStartTime > (m_isChangeDuration ? m_changeDuration : AnimLength))
        {
            return true;
        }
        return false;
	}

	public void ChangeDurationTime(float duration)
	{
        m_changeDuration = duration;
        m_isChangeDuration = true;
	}
	public override void Reset()
	{
		base.Reset();
        m_changeDuration = 0;
        m_isChangeDuration = false;
	}
};