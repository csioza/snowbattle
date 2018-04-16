//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Actor\ActorAction
//	created:	2013-4-16
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

//站立
public class StandAction : ActorAction
{
	public override ENType GetActionType() { return ENType.enStandAction; }
	public static ENType SGetActionType() { return ENType.enStandAction; }

    public string AnimName = "standby";
    Vector3 m_targetForward = Vector3.zero;
    float m_rotateSpeed = 0;
	public override void OnEnter()
	{
        RefreshActionRef();
        //
        if (CurrentActor.Type == ActorType.enNPC) 
		{
			NPC npc = CurrentActor as NPC;
			m_rotateSpeed = npc.CurrentTableInfo.RotateSpeed;
		}
		else if (CurrentActor.Type == ActorType.enNPCTrap) 
		{
			m_rotateSpeed = 0;
		}
        else
        {
            m_rotateSpeed = CurrentActor.PropConfig.RotateSpeed;
        }
        if ((CurrentActor.Type == ActorType.enNPC && CurrentActor.CurrentTarget != null) ||
           (CurrentActor.Type == ActorType.enFollow && CurrentActor.CurrentTargetIsDead))
        {
            Vector3 targetPos = Vector3.zero;
            if (CurrentActor.Type == ActorType.enNPC)
            {
                targetPos = CurrentActor.CurrentTarget.RealPos;
            }
            else
            {
                targetPos = ActorManager.Singleton.MainActor.RealPos;
            }
            ForwardTarget(targetPos);
        }
	}
    public void ForwardTarget(Vector3 targetPos)
    {
        m_targetForward = targetPos - CurrentActor.RealPos;
        m_targetForward.y = 0.0f;
        m_targetForward.Normalize();
    }

	public override bool OnUpdate()
	{
		if (!CurrentActor.MainRigidBody.isKinematic)
		{
			CurrentActor.MainRigidBody.velocity = Vector3.zero;
        }
        
        ///////////////////////////////////////////////////////////////////////
        if ((CurrentActor.Type == ActorType.enNPC && CurrentActor.CurrentTarget != null) ||
            (CurrentActor.Type == ActorType.enFollow && CurrentActor.CurrentTargetIsDead))
        {
            Vector3 vForward = new Vector3(CurrentActor.MainObj.transform.forward.x, 0f, CurrentActor.MainObj.transform.forward.z);
            float fCurForwardAngle = ActorBlendAnim.HorizontalAngle(vForward);
            float fTargetAngle = ActorBlendAnim.HorizontalAngle(m_targetForward);
            float fInterval = fTargetAngle - fCurForwardAngle;
            if (Mathf.Abs(fInterval) > 10.0f)
            {
                float fLerp = Mathf.LerpAngle(fCurForwardAngle, fTargetAngle, Time.deltaTime * m_rotateSpeed);
                Quaternion quater = Quaternion.Euler(0f, fLerp - fCurForwardAngle, 0f);
                CurrentActor.MainObj.transform.rotation = quater * CurrentActor.MainObj.transform.rotation;
            }
        }
		return false;
	}
    public override void Reset()
    {
        base.Reset();
        AnimName = "standby";
    }
};