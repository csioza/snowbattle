using System;
using System.Collections.Generic;
using UnityEngine;

public class ActorBlendAnim
{
    Actor CurrentActor { get; set; }
    public ActorAction.ENType CurActionType { get; set; }
    ActorAction.ENType LastActionType { get; set; }
    public ActorBlendAnim(Actor actor)
    {
        CurrentActor = actor;
        m_fHeadCurAngle = 0f;
        m_fUpperCurAngle = 0f;
    }
    //!当前头部旋转的角度（用于点击移动头部注视旋转）
    float m_fHeadCurAngle = 0f;
    float m_fUpperCurAngle = 0f;
    //!角色移动的方向向量
    Vector3 mTargetDirection = Vector3.zero;
    Vector3 mMoveTargetPos = Vector3.zero;
    //!一次移动过程是否需要再次改变移动动画
    //bool mIsSwitchOn = false;
    //!最大阀值
    float mMaxAngle = 95f;
    //!腰部旋转最小阀值
    float mMinAngle = 60f;
    
    bool mIsRotateUpBodyWithoutTarget = false;
    //!是否使用移动旋转
    bool m_isRawRotation = false;
    //!是否旋转上半身
    bool m_isRotateUpbody = true;
    //!tick
    public void LateUpdate()
    {
        if (GameSettings.Singleton.m_closeActorBlendAnim)
        {
            return;
        }
        if (CurrentActor.Type == ActorType.enMain && CurrentActor.IsActorExit)
        {
            return;
        }
        if (CurrentActor.MainPlayerConfig == null || CurrentActor.MainPlayerConfig.m_headTransform == null)
        {
            return;
        }
        ActorAction.ENType actionType = LastActionType;
        LastActionType = CurActionType;
        if (CurActionType != ActorAction.ENType.enStandAction && CurActionType != ActorAction.ENType.enMoveAction)
        {
            Reset();
            return;
        }
        if (CurrentActor.IsDead)
        {
            Reset();
            return;
        }
        if (actionType == ActorAction.ENType.enRollAction && CurActionType == ActorAction.ENType.enStandAction)
        {
            m_isRotateUpbody = false;
            Reset();
            return;
        }
        else
        {
            if (CurActionType != ActorAction.ENType.enStandAction)
            {
                m_isRotateUpbody = true;
            }
        }
        bool isHaveTarget = (null != CurrentActor.TargetManager.CurrentTarget);
        if (isHaveTarget && IsRawRotation())
        {
            MoveAction action = CurrentActor.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
            if (null != action)
            {
                action.AnimName = "run";
            }
            return;
        }
        if (isHaveTarget)
        {
            RotateMainBody();
            if (m_isRotateUpbody)
            {
                RotateUpBody();
            }
        }
        else
        {
            if (mIsRotateUpBodyWithoutTarget)
            {
                RotateUpBodyWithoutTarget();
            }
        }  
    }
    void ResetGazingData()
    {
        if (CurActionType != ActorAction.ENType.enMoveAction)
        {
            m_fUpperCurAngle = 0f;
            m_fHeadCurAngle = 0f;
        }
    }
    //!点击移动头部注视的旋转
    void RotateUpBodyWithoutTarget()
    {
        if (CurrentActor.MainPlayerConfig == null || CurrentActor.MainPlayerConfig.m_headTransform == null)
        {
            return;
        }
        Vector3 vDirection = mMoveTargetPos - CurrentActor.MainPos;
        vDirection.y = 0f;
        //Debug.Log("vDirection=" + vDirection.ToString() + " mitude=" + vDirection.sqrMagnitude.ToString() + " targetPos=" + mMoveTargetPos + " mainPos=" + CurrentActor.MainPos);
        if (vDirection.sqrMagnitude <= 0.01f)
        {
            mIsRotateUpBodyWithoutTarget = false;
            return;
        }
        Vector3 vForward = new Vector3(CurrentActor.MainObj.transform.forward.x, 0f, CurrentActor.MainObj.transform.forward.z);
        float fCurForwardAngle = HorizontalAngle(vForward);
        float fTargetAngle = HorizontalAngle(vDirection);
        //计算面向与移动方向的夹角
        float deltaAngle = Mathf.DeltaAngle(fCurForwardAngle, fTargetAngle);
        //上半身所占比重0.3
        float upBodyAngle = deltaAngle * 0.3f;
        //头部所占比重0.7
        float headAngle = deltaAngle * 0.7f;
        //上半身允许旋转的最大角度40
        if (Mathf.Abs(upBodyAngle) > 40f)
        {
            upBodyAngle = (upBodyAngle / Mathf.Abs(upBodyAngle)) * 40f;
        }
        //头部允许旋转的最大角度60
        if (Mathf.Abs(headAngle) > 60f)
        {
            headAngle = (headAngle / Mathf.Abs(headAngle)) * 60f;
        }
        //Debug.Log("curForwardAngle=" + fCurForwardAngle + " targetAngle=" + fTargetAngle + " upbodyAngle=" + upBodyAngle + " headAngle=" + headAngle+" deltaAngle="+deltaAngle);
        float upBodyLerp = Mathf.LerpAngle(m_fUpperCurAngle, upBodyAngle, Time.deltaTime * GameSettings.Singleton.m_moveUpperRotateSpeed);
        m_fUpperCurAngle = upBodyLerp;
        float fHeadLerp = Mathf.LerpAngle(m_fHeadCurAngle, headAngle, Time.deltaTime * GameSettings.Singleton.m_moveHeadRotateSpeed);
        m_fHeadCurAngle = fHeadLerp;
        
        Quaternion quater = Quaternion.Euler(0f, fHeadLerp, 0f);
        CurrentActor.MainPlayerConfig.m_headTransform.rotation = quater * CurrentActor.MainPlayerConfig.m_headTransform.rotation;
        
        quater = Quaternion.Euler(0f, upBodyLerp, 0f);
        CurrentActor.MainPlayerConfig.m_upperBodyTransform.rotation = quater * CurrentActor.MainPlayerConfig.m_upperBodyTransform.rotation;
        //Debug.Log("upBodyLerp=" + upBodyLerp + " fHeadLerp=" + fHeadLerp + " deltaAngle=" + deltaAngle);
    }
    public void FixedUpdate()
    {
        if (GameSettings.Singleton.m_closeActorBlendAnim)
        {
            return;
        }
    }
    //!叉积
    Vector3 vCross(Vector3 lt, Vector3 rt)
    {
        return new Vector3(lt.y * rt.z - lt.z * rt.y,
            lt.z * rt.x - lt.x * rt.z,
            lt.x * rt.y - lt.y * rt.x);

    }
    //
    public bool IsRawRotation()
    {
        if (GameSettings.Singleton.m_closeActorBlendAnim)
        {
            return true;
        }
        Player player = CurrentActor as Player;
        float fDist = Vector3.Distance(CurrentActor.RealPos, CurrentActor.TargetManager.CurrentTarget.RealPos);
        if (fDist > player.GetUnlockGazingRange + 0.5f)
        {
            return true;
        }
        else if (fDist < player.GetUnlockGazingRange - 0.5f)
        {
            return false;
        }
        return m_isRawRotation;
    }
    //!手势按下操作
    public void OnPressDown(Vector3 targetPos)
    {
        if (GameSettings.Singleton.m_closeActorBlendAnim)
        {
            return;
        }
        if (null == CurrentActor.TargetManager.CurrentTarget)
        {
            mMoveTargetPos = targetPos;
            mIsRotateUpBodyWithoutTarget = true;
            ResetGazingData();
        }
        //Debug.Log("OnPressDown");
    }
    //!重置相关数据
    public void Reset()
    {
        m_fHeadCurAngle = 0f;
        m_fUpperCurAngle = 0f;
        mMoveTargetPos = Vector3.zero;
        mIsRotateUpBodyWithoutTarget = false;
    }
    public void OnPressUp()
    {

    }
    //!改变移动目标点
    public void Retarget(Vector3 vTargetPos)
    {
        m_isRawRotation = false;
        mTargetDirection = vTargetPos - CurrentActor.MainPos;
        mTargetDirection.y = 0f;
        mMoveTargetPos = vTargetPos;
        if (GameSettings.Singleton.m_closeActorBlendAnim)
        {
            return ;
        }
        bool isTrue = (CurActionType == ActorAction.ENType.enMoveAction &&
            null != CurrentActor.TargetManager.CurrentTarget);
        if (isTrue)
        {
            MoveAction action = CurrentActor.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
            if (null == action)
            {
                return;
            }
//            mIsSwitchOn = false;
            Vector3 vNPCForward = CurrentActor.TargetManager.CurrentTarget.MainPos - CurrentActor.MainPos;
            vNPCForward.y = 0f;
            //Vector3 vMainForward = new Vector3(CurrentActor.MainObj.transform.forward.x, 0f, CurrentActor.MainObj.transform.forward.z);
            float fForwardAngle = Vector3.Angle(vNPCForward.normalized, mTargetDirection.normalized);
            //插值，避免临界值的问题(左右旋转)
            if (fForwardAngle >= (mMaxAngle + 10f))
            {
                action.AnimName = "backward";
            }
            else if (fForwardAngle <= (mMaxAngle - 10f))
            {
                action.AnimName = "run";
            }
            if (fForwardAngle >= - float.Epsilon && fForwardAngle <= float.Epsilon)
            {
                m_isRawRotation = true;

            }
            //Debug.Log("forwardangle=" + fForwardAngle+" animationname="+action.AnimName);
            if (IsRawRotation())
            {
                action.AnimName = "run";
                m_fHeadCurAngle = 0f;
                m_fUpperCurAngle = 0f;
                //Debug.Log("actionType=" + CurActionType);
                return;
            }
            //Debug.Log("mLastAnimName=" + mLastAnimName);
        }
    }
    //!角色整体旋转
    void RotateMainBody()
    {
        if (CurActionType == ActorAction.ENType.enMoveAction)
        {
            MoveAction action = CurrentActor.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
            if (null == action)
            {
                return;
            }
            Vector3 vMainForward = new Vector3(CurrentActor.MainObj.transform.forward.x, 0f, CurrentActor.MainObj.transform.forward.z);
            //Vector3 vMainForward = mMoveTargetPos - CurrentActor.MainPos;
            vMainForward.y = 0f;
            Vector3 vNPCDirection = CurrentActor.TargetManager.CurrentTarget.MainPos - CurrentActor.MainPos;
            vNPCDirection.y = 0f;
            float fForwardAngle = Vector3.Angle(vNPCDirection.normalized, mTargetDirection.normalized);
            //if (!mIsSwitchOn)
            {
                if (fForwardAngle >= (mMaxAngle + 10f))
                {
                    if (action.AnimName != "backward")
                    {
                        action.AnimName = "backward";
//                        mIsSwitchOn = true;
                        //Debug.Log("backward");
                    }
                }
                else if (fForwardAngle <= (mMaxAngle - 10f))
                {
                    if (action.AnimName != "run")
                    {
                        action.AnimName = "run";
//                        mIsSwitchOn = true;
                        //Debug.Log("run");
                    }
                }
                //Debug.Log("action.AnimName=" + action.AnimName + " fForwardAngle=" + fForwardAngle + " mIsSwitchOn=" + mIsSwitchOn);
            }
            Vector3 vDirection = new Vector3(mTargetDirection.x, 0, mTargetDirection.z);
            Vector3 vbodyTurnDirection = Vector3.zero;
            if (action.AnimName == "run")
            {
                vbodyTurnDirection = vDirection;
            }
            else
            {
                vbodyTurnDirection = -vDirection;
            }
            float fBodyTurnAngle = Vector3.Angle(vbodyTurnDirection.normalized, vMainForward.normalized);
            if (fBodyTurnAngle >= -float.Epsilon && fBodyTurnAngle <= float.Epsilon)
            {
                return;
            }
            float rotateSpeed = Time.deltaTime * GameSettings.Singleton.m_bodyRotateSpeed * 360f;
            //Debug.Log("bodyTurnAngle=" + fBodyTurnAngle+" rotateSpeed="+rotateSpeed);
            if (fBodyTurnAngle < rotateSpeed)
            {
                CurrentActor.MoveRotation(Quaternion.LookRotation(vbodyTurnDirection.normalized) * Quaternion.LookRotation(Vector3.forward));
                return;
            }
            //int nRote = 0;
            float fRote = 0f;
            if (action.AnimName == "run")
            {
                Vector3 verticalVec = Vector3.Cross(vbodyTurnDirection.normalized, new Vector3(0f, 1.0f, 0f));
                fRote = Vector3.Angle(verticalVec.normalized, vMainForward.normalized);
                //nRote = Mathf.RoundToInt(fRote);
                if (fRote >= 90f)
                {
                    rotateSpeed = -rotateSpeed;
                }
            }
            else
            {
                float fNPCTurnAngle = Vector3.Angle(vbodyTurnDirection.normalized, vNPCDirection.normalized);
                if (fNPCTurnAngle >= fBodyTurnAngle)
                {
                    Vector3 verticalVec = Vector3.Cross(vbodyTurnDirection.normalized, new Vector3(0f, 1.0f, 0f));
                    fRote = Vector3.Angle(verticalVec.normalized, vMainForward.normalized);
                    if (fRote >= 90f)
                    {
                        rotateSpeed = -rotateSpeed;
                    }
                }
                else
                {
                    Vector3 verticalVec = Vector3.Cross(vNPCDirection.normalized, new Vector3(0f, 1.0f, 0f));
                    fRote = Vector3.Angle(verticalVec.normalized, vMainForward.normalized);
                    if (fRote >= 90f)
                    {
                        rotateSpeed = -rotateSpeed;
                    }
                }
                //Debug.Log("fBodyTurnAngle=" + fBodyTurnAngle + " rotateSpeed=" + rotateSpeed + " action.AnimName=" + action.AnimName + " fRote=" + fRote);
            }
            //Debug.Log("moveaction");
            CurrentActor.MainObj.transform.Rotate(0.0f, rotateSpeed, 0.0f);
        }
        else if (CurActionType == ActorAction.ENType.enStandAction)
        {
            StandAction action = CurrentActor.ActionControl.LookupAction(ActorAction.ENType.enStandAction) as StandAction;
            if (null == action)
            {
                return;
            }
            float fBodyTurnAngle = 0f;
            Vector3 vDirection = CurrentActor.TargetManager.CurrentTarget.RealPos - CurrentActor.RealPos;
            vDirection.y = 0f;
            vDirection.Normalize();
            Vector3 vForward = new Vector3(CurrentActor.MainObj.transform.forward.x, 0f, CurrentActor.MainObj.transform.forward.z);
            vForward.Normalize();
            fBodyTurnAngle = Vector3.Angle(vForward.normalized, vDirection.normalized);
            //m_fCurAngle = 0f;
            //action.AnimName = "readytofight";
            float rotateSpeed = Time.deltaTime * GameSettings.Singleton.m_standRotateSpeed * 360f;
            if (fBodyTurnAngle < rotateSpeed)
            {
                CurrentActor.MoveRotation(Quaternion.LookRotation(vDirection.normalized) * Quaternion.LookRotation(Vector3.forward));
                return;
            }
            fBodyTurnAngle = rotateSpeed;
            Vector3 verticalVec = Vector3.Cross(vForward.normalized, new Vector3(0, 1.0f, 0));
            verticalVec.Normalize();
            float fOtherRot = Vector3.Angle(verticalVec.normalized, vDirection.normalized);
            if (fOtherRot <= 90f)
            {
                fBodyTurnAngle = -fBodyTurnAngle;
            }
            Quaternion bodyRotation = Quaternion.Euler(0, fBodyTurnAngle, 0);
            CurrentActor.MainObj.transform.rotation = bodyRotation * CurrentActor.MainObj.transform.rotation;
            //CurrentActor.MainObj.transform.Rotate(0.0f, fBodyTurnAngle, 0.0f);
            //Debug.Log("enStandAction body turn angle="+fBodyTurnAngle);
        }
    }
    //!旋转角色上半身(头、胸)
    void RotateUpBody()
    {
        //!如果待机状态(standaction)不做上半身的旋转的话，那么从移动到待机动画转换时就会出现动画抖动的问题
        //if (CurActionType == ActorAction.ENType.enMoveAction)
        {
            Vector3 mainForward = CurrentActor.MainObj.transform.forward;
            Vector3 rotateDirection = CurrentActor.TargetManager.CurrentTarget.MainPos - CurrentActor.MainPos;
            rotateDirection.y = 0f;

//            float fAngle = Vector3.Angle(mainForward, rotateDirection);
            float fCurForwardAngle = HorizontalAngle(mainForward);
            float fTargetAngle = HorizontalAngle(rotateDirection);
            float deltaAngle = Mathf.DeltaAngle(fCurForwardAngle, fTargetAngle);
            if (Mathf.Abs(deltaAngle) >= mMaxAngle + 20f)
            {
                return;
            }
            float upBodyPercent = 0.0f;
            if (Mathf.Abs(deltaAngle) >= mMinAngle)
            {
                upBodyPercent = 0.7f;
            }
            float fUpperAngle = upBodyPercent * deltaAngle;
            float upBodyLerp = Mathf.LerpAngle(m_fUpperCurAngle, fUpperAngle, Time.deltaTime * GameSettings.Singleton.m_upperBodyRotateSpeed);

            float fHeadAngle = deltaAngle - upBodyLerp;
            float fHeadLerp = Mathf.LerpAngle(m_fHeadCurAngle, fHeadAngle, Time.deltaTime * GameSettings.Singleton.m_headRotateSpeed);
            //Debug.Log("deltaAngle=" + deltaAngle + " m_fUpperCurAngle=" + m_fUpperCurAngle + " upperAngle=" + fUpperAngle + " upBodyLerp=" + upBodyLerp + " m_fHeadCurAngle=" + m_fHeadCurAngle + " headAngle=" + fHeadAngle + " fHeadLerp=" + fHeadLerp);


            m_fUpperCurAngle = upBodyLerp;
            m_fHeadCurAngle = fHeadLerp;

            Quaternion quater = Quaternion.Euler(0f, upBodyLerp, 0f);
            CurrentActor.MainPlayerConfig.m_upperBodyTransform.rotation = quater * CurrentActor.MainPlayerConfig.m_upperBodyTransform.rotation;

            quater = Quaternion.Euler(0f, fHeadLerp, 0f);
            CurrentActor.MainPlayerConfig.m_headTransform.rotation = quater * CurrentActor.MainPlayerConfig.m_headTransform.rotation;
        }
    }
    public static float HorizontalAngle(Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }
	// The angle between dirA and dirB around axis
	static float AngleAroundAxis (Vector3 dirA, Vector3 dirB, Vector3 axis) 
    {
	    // Project A and B onto the plane orthogonal target axis
	    dirA = dirA - Vector3.Project (dirA, axis);
	    dirB = dirB - Vector3.Project (dirB, axis);
	   
	    // Find (positive) angle between A and B
        float angle = Vector3.Angle(dirA, dirB);
	   
	    // Return angle multiplied with 1 or -1
	    return angle * (Vector3.Dot (axis, Vector3.Cross (dirA, dirB)) < 0 ? -1 : 1);
	}
	
}