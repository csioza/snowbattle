using System;
using UnityEngine;

//被击
public class BeAttackAction : ActorAction
{
	public override ENType GetActionType() { return ActorAction.ENType.enBeAttackAction; }
	public static ENType SGetActionType() { return ENType.enBeAttackAction; }

    public Actor m_srcActor { get; set; }
    public bool m_isBack = false;//击退
    public bool m_isFly = false;//击飞
    public string mStrAnim = "";
    public string mAniBlend = "";
    public float fBlendFactor = 0f;
    public void Init(Actor srcActor, bool isBack, bool isFly)
	{
		m_srcActor = srcActor;
        m_isBack = isBack;
        m_isFly = isFly;
        if (CurrentActor.m_isDeposited)
        {//托管中，发送beattack action的消息
            //Vector3 pos = CurrentActor.MainPos;
			IMiniServer.Singleton.SendAction_BeHit_C2BS(CurrentActor.ID, m_srcActor.ID, m_isBack, m_isFly);
        }
	}

	public override void OnEnter()
    {
        SyncPositionIfNeed();
		if (null == m_srcActor)
		{
			IsEnable = false;
			return;
        }
        //CurrentActor.PlayEffect("ef-daji", 1.0f, "strikePoint", false, Vector3.zero);
        return;
        string attackedType = "attacked";
        float yRot = 0f;
        float fOtherRot = 0f;
        if (m_isBack)
        {
            attackedType = "repulse";
        }
        Vector3 vForward = Vector3.zero;
        if (m_srcActor.mCurAttackBoneStartPos == Vector3.zero)
        {
            //Debug.Log("CurrentActor.mBlastPoint=" + CurrentActor.mBlastPoint);
            if (CurrentActor.mBlastPoint != Vector3.zero)
            {
                vForward = CurrentActor.RealPos - CurrentActor.mBlastPoint;
            }
            else
            {
                vForward = m_srcActor.MainObj.transform.forward;
            }
        }
        else
        {
            vForward = m_srcActor.mCurAttackBoneEndPos - m_srcActor.mCurAttackBoneStartPos;
        }
        vForward.y = 0f;
        yRot = Vector3.Angle(CurrentActor.MainObj.transform.forward, vForward);
        Vector3 verticalDirction = Vector3.Cross(CurrentActor.MainObj.transform.forward, new Vector3(0, 1.0f, 0));
        verticalDirction.y = 0f;
        fOtherRot = Vector3.Angle(vForward, verticalDirction);
        //Debug.Log("yRot=" + yRot + " fotherRot=" + fOtherRot);
        float fX = yRot;
        if (yRot > 90)
        {
            fX = fX - 90;
            mStrAnim = (attackedType + "-s");
        }
        else
        {
            fX = 90 - fX;
            mStrAnim = (attackedType + "-n");
        }
        float fY = fOtherRot;
        if (fOtherRot > 90.0f)
        {
            fY = fY - 90;
            mAniBlend = (attackedType + "-w");
        }
        else
        {
            fY = 90 - fY;
            mAniBlend = (attackedType + "-e");
        }
        if (fY > 0f && fX > 0f)
        {
            fBlendFactor = fY / fX;
        }
        if (fBlendFactor >= -0.001f && fBlendFactor <= 0.001f)
        {
            fBlendFactor = 0f;
        }
        //Debug.Log("mAniBlend = " + mAniBlend+ " strAnim="+mStrAnim+" blendfactor="+fBlendFactor);
        m_srcActor.mCurAttackBoneStartPos = Vector3.zero;
        m_srcActor.mCurAttackBoneEndPos = Vector3.zero;
        RefreshActionRef();

        if (CurrentActor.Type == ActorType.enMain && m_isBack)
        {
            MainPlayer player = CurrentActor as MainPlayer;
            player.CurrentCmd = null;
        }
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
        //回馈位移动画
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