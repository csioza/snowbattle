//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Actor\ActorAction
//	created:	2013-4-18
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;
using System.Collections;

//死亡
public class DeadAction : ActorAction
{
    bool m_isKinematic = false;
    public bool IsPlayOver = false;
    public override ENType GetActionType() { return ENType.enDeadAction; }
	public static ENType SGetActionType() { return ENType.enDeadAction; }
    public Vector3 mTargetPos = Vector3.zero;
    public void Init(bool isRelive)
    {
        IsPlayOver = false;
        if (CurrentActor.Type == ActorType.enNPC)
        {
            NPC npc = CurrentActor as NPC;
            if (npc.GetNpcType() == ENNpcType.enBOSSNPC)
            {//boss死亡后会移动，加此代码，防止移动
                m_isKinematic = CurrentActor.MainRigidBody.isKinematic;
                CurrentActor.MainRigidBody.isKinematic = true;
            }
            else
            {
                if (!isRelive)
                {//不复活，隐藏collider
                    if (CurrentActor.CenterCollider != null)
                    {
                        CurrentActor.CenterCollider.gameObject.layer = LayerMask.NameToLayer("DisableCollider");
                    }
                }
            }
            if (npc.GetNpcType() != ENNpcType.enBoxNPC)
            {
                float fAngle = Vector3.Angle(CurrentActor.MainObj.transform.forward, CurrentActor.mBlastForward.normalized);
                if (fAngle <= 60f)
                {
                    Vector3 vForward = CurrentActor.mBlastPoint - CurrentActor.RealPos;
                    vForward.y = 0f;
                    vForward.Normalize();
                    float fLerp = Vector3.Angle(vForward, CurrentActor.MainObj.transform.forward);
                    CurrentActor.MainObj.transform.Rotate(Vector3.up, fLerp);

                }
            }
        }
        AnimStartTime = Time.time;
    }

	public override void OnEnter()
	{
        RefreshActionRef();
//         float fAngle = Vector3.Angle(CurrentActor.MainObj.transform.forward, CurrentActor.mBlastForward.normalized);
//         //Debug.Log("fAngle=" + fAngle);
//         if (fAngle <= 60f)
//         {
//             Vector3 vForward = CurrentActor.mBlastPoint - CurrentActor.RealPos;
//             vForward.y = 0f;
//             vForward.Normalize();
//             float fLerp = Vector3.Angle(vForward, CurrentActor.MainObj.transform.forward);
//             CurrentActor.MainObj.transform.Rotate(Vector3.up, fLerp);
//             //CurrentActor.MainObj.transform.rotation = Quaternion.LookRotation(vForward);
//             Debug.Log("vforward=" + fLerp);
// //             vForward = vForward * 0.5f;
// //             mTargetPos = CurrentActor.RealPos + vForward;
// //             CurrentActor.ForceMoveToPosition(mTargetPos);
// 
//         }
	}

	public override void OnInterupt()
	{
        OnExit();
	}

    public override void OnExit()
    {
        CurrentActor.MainRigidBody.isKinematic = m_isKinematic;
    }
	public override bool OnUpdate()
    {
        if (ActorType.enNPC == CurrentActor.Type)
        {
            NPC npc = CurrentActor as NPC;
            float fDuration = 3.0f;
            if (npc.GetNpcType() == ENNpcType.enBoxNPC)
            {//宝箱永远不消失
                fDuration = float.MaxValue;
            }
            else
            {
                if (npc.NpcBehaviour)
                {
                    fDuration = npc.NpcBehaviour.DeadDuration;
                }
                fDuration += AnimLength;
            }
            if (Time.time - AnimStartTime > fDuration)
            {
                CurrentActor.MainRigidBody.isKinematic = m_isKinematic;
                ActorManager.Singleton.ReleaseActor(CurrentActor.ID);
            }
            if (!IsPlayOver && Time.time - AnimStartTime > AnimLength)
            {
                IsPlayOver = true;
                if (npc.GetNpcType() != ENNpcType.enBOSSNPC)
                {
                    m_isKinematic = CurrentActor.MainRigidBody.isKinematic;
                    CurrentActor.MainRigidBody.isKinematic = true;
                }
                CurrentActor.EnableCollider(false);
            }
        }
        else
        {
            float fDuration = AnimLength + 0.3f;
            if (Time.time - AnimStartTime > fDuration)
            {
                CurrentActor.MainRigidBody.isKinematic = m_isKinematic;
            }
            if (!IsPlayOver && Time.time - AnimStartTime > AnimLength)
            {
                IsPlayOver = true;

                m_isKinematic = CurrentActor.MainRigidBody.isKinematic;
                CurrentActor.MainRigidBody.isKinematic = true;

                CurrentActor.EnableCollider(false);
            }
        }
		return false;
	}
};