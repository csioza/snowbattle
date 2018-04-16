using System;
using UnityEngine;

//假受击
public class FakeBeAttackAction : ActorAction
{
    public override ENType GetActionType() { return ActorAction.ENType.enFakeBeAttackAction; }
	public static ENType SGetActionType() { return ENType.enFakeBeAttackAction; }

    public string mStrAnim = "";
    public string mAniBlend = "";
    public float fBlendFactor = 0f;
    private Actor m_srcActor = null;
    public void Init(Actor srcActor)
    {
        m_srcActor = srcActor;
    }
	public override void OnEnter()
	{
        if (m_srcActor == null)
        {
            IsEnable = false;
            return;
        }
        //CurrentActor.PlayEffect("ef-daji", 1.0f, "strikePoint", false, Vector3.zero);
        return;
        fBlendFactor = 0f;
        string attackedType = "attacked";
        float yRot = 0f;
        float fOtherRot = 0f;
        Vector3 vForward = Vector3.zero;
        if (m_srcActor.mCurAttackBoneStartPos == Vector3.zero)
        {
            vForward = m_srcActor.MainObj.transform.forward;
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
            fBlendFactor = 0.01f;
        }
        m_srcActor.mCurAttackBoneStartPos = Vector3.zero;
        m_srcActor.mCurAttackBoneEndPos = Vector3.zero;
        RefreshActionRef();
	}

	public override void OnInterupt()
    {
        OnExit();
	}

	public override void OnExit()
    {
        CurrentActor.MainAnim.cullingType = AnimationCullingType.BasedOnRenderers;
	}

	public override bool OnUpdate()
	{
        return Time.time - AnimStartTime > AnimLength;
	}
};