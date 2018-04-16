using System;
using UnityEngine;

//翻滚
public class RollAction : ActorAction
{
	public override ENType GetActionType() { return ENType.enRollAction; }
    public static ENType SGetActionType() { return ENType.enRollAction; }

    Vector3 m_curPos { get; set; }
    Vector3 m_targetPos { get; set; }
    public void Init(Vector3 targetPos)
    {
        IsSyncPosition = false;
        m_curPos = CurrentActor.RealPos;
        m_targetPos = targetPos;
        if (CurrentActor.m_isDeposited)
        {//托管中，发送roll action的消息
            IMiniServer.Singleton.SendAction_Roll_C2BS(CurrentActor.ID, m_curPos, m_targetPos);
        }
    }
    public void Init(Vector3 curPos, Vector3 targetPos)
    {
        IsSyncPosition = true;
        SyncPosition = curPos;
        m_curPos = curPos;
        m_targetPos = targetPos;
        if (CurrentActor.m_isDeposited)
        {//托管中，发送roll action的消息
            IMiniServer.Singleton.SendAction_Roll_C2BS(CurrentActor.ID, m_curPos, m_targetPos);
        }
    }
	public override void OnEnter()
    {
        SyncPositionIfNeed();
        RefreshActionRef();
        CurrentActor.MainAnim.cullingType = AnimationCullingType.AlwaysAnimate;

        CurrentActor.StartCaution();

        Vector3 direction = m_targetPos - m_curPos;
        direction.y = 0.0f;
        direction.Normalize();
        CurrentActor.MoveRotation(Quaternion.LookRotation(direction.normalized) * Quaternion.LookRotation(Vector3.forward));
	}
	public override void OnInterupt()
    {
        OnExit();
        if (CurrentActor.m_isDeposited)
        {//托管中，发送action被打断的消息
            IMiniServer.Singleton.SendActionInterupt_C2BS(CurrentActor.ID, (int)GetActionType());
        }
	}
	public override void OnExit()
    {
        CurrentActor.MainAnim.cullingType = AnimationCullingType.BasedOnRenderers;
        if (CurrentActor.CenterCollider != null)
        {
            CurrentActor.CenterCollider.gameObject.layer = LayerMask.NameToLayer("Actor");
        }
        //将位移动画的位移回馈到角色，并且播放下一个动画，下一个动画不能Fade [8/11/2015 tgame]
        CurrentActor.ApplyAnimationOffset();
	}
	public override bool OnUpdate()
    {
        if (!CurrentActor.MainRigidBody.isKinematic)
        {
            CurrentActor.MainRigidBody.velocity = Vector3.zero;
        }
        return Time.time - AnimStartTime > AnimLength;
    }
};