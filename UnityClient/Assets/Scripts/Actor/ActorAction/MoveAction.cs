//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Actor\ActorAction
//	created:	2013-4-16
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//移动
public class MoveAction : ActorAction
{
	public Action OnMoveEnd;
	public Action OnInterupted;
	public override ENType GetActionType() { return ENType.enMoveAction; }
	public static ENType SGetActionType() { return ENType.enMoveAction; }
	public Vector3 TargetPos
	{
		get { return m_targetPos; }
		set { m_targetPos = value; m_targetPos.y = 0.0f; }
	}
	private Vector3 m_targetPos = Vector3.zero;
	private Vector3 m_startPos = Vector3.zero;
    public Vector3 m_targetForward=Vector3.zero;
    bool m_isMakeSureForward = false;
    private UnityEngine.AI.NavMeshPath m_pathList = new UnityEngine.AI.NavMeshPath();
//    private int m_curNodeIndx = 0;
    public float m_currentSpeed = 0.0f;
    public Vector3 mLastMovePos = Vector3.zero;
    public float mStartTime = 0;
    public float mRealSpeed = 0f;
    public Vector3 m_realTargetPos = Vector3.zero;
    //是否后退
    public bool m_isBack = false;
    //!是否不寻路 tgame
    public bool m_isNotAStar = false;
    public string AnimName { get; set; }
    //是否停止移动
    public bool IsStopMove { get { return m_currentSpeed == 0 || mRealSpeed / m_currentSpeed <= 0.1f; } }
    //是否在旋转
    public bool mIsRotating = false;
    /// <summary>
    /// ////////////////////////////////////////////////////////////
    /// </summary>
    public List<Vector3> PathNodeList = new List<Vector3>();
    //
    List<GameObject> objList = new List<GameObject>();
    IEnumerator Coroutine_LoadEffect(List<Vector3> posNodeList)
    {
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath("ef-e-choosetarget-E01"), data);
        while (true)
        {
            e.MoveNext();
            if (data.m_isFinish)
            {
                break;
            }
            yield return e.Current;
        }
        if (data.m_obj != null)
        {
            foreach (Vector3 pos in posNodeList)
            {
                GameObject obj = GameObject.Instantiate(data.m_obj) as GameObject;
                obj.transform.position = new Vector3(pos.x, 0.1f, pos.z);
                obj.SetActive(true);
                objList.Add(obj);
            }
        }
    }
    //做为移动Action的外部接口，所有移动，都只使用此接口
    public void Retarget(Vector3 pos)
    {
        SyncPositionIfNeed();
        float fDist = Vector3.Distance(m_realTargetPos, pos);
        if (fDist <= 0.2f)
        {
            //Debug.Log("FDist:" + fDist);
            return;
        }
        if (objList.Count > 0)
        {
            foreach (GameObject obj in objList)
            {
                GameObject.DestroyImmediate(obj);
            }
            objList.Clear();
        }
        //if (CurrentActor.Type == ActorType.enMain/* || CurrentActor.Type == ActorType.enFollow*/)
        {
            bool isPath = true;
            if (CurrentActor.Type == ActorType.enMain)
            {
                if (CursorEffectFunc.Singleton.CurFingerType == CursorEffectFunc.ENFingerType.enOnDragMoveType
                    || m_isNotAStar)
                {
                    PathNodeList.Add(CurrentActor.MainPos);
                    PathNodeList.Add(pos);
                }
                else
                {
                    PathNodeList = SM.RandomRoomLevel.Singleton.FindPath(CurrentActor.MainPos, pos, ref isPath);
                }
            }
            else
            {
                PathNodeList = SM.RandomRoomLevel.Singleton.FindPath(CurrentActor.MainPos, pos, ref isPath);
            }
            if (PathNodeList.Count > 0)
            {
                PathNodeList.RemoveAt(0);
            }
            if (CurrentActor.Type == ActorType.enMain)
            {
                if (isPath)
                {
                    if (PathNodeList.Count > 0)
                    {
                        Vector3 targetPos = PathNodeList[PathNodeList.Count - 1];
                        fDist = Vector3.Distance(targetPos, pos);
                        if (fDist <= 0.5f)
                        {
                            CursorEffectFunc.Singleton.SetClickGroundEffect01(pos);
                        }
                    }
                }
            }
            if (GameSettings.Singleton.m_isOpenFathfinderDebug)
            {
                MainGame.Singleton.StartCoroutine(Coroutine_LoadEffect(PathNodeList));
            }
        }
        
        if (PathNodeList.Count <= 0)
        {
            TargetPos = pos;
        }
        else
        {
            TargetPos = PathNodeList[0];
            PathNodeList.RemoveAt(0);
        }
        m_realTargetPos = pos;
        mStartTime = Time.time;
        mLastMovePos = CurrentActor.MainPos;
        mRealSpeed = m_isBack ? CurrentActor.MovebackSpeed : CurrentActor.MoveSpeed;
        RefreshData();
        if (null != CurrentActor.mActorBlendAnim)
        {
            CurrentActor.mActorBlendAnim.Retarget(TargetPos);
        }
        if (CurrentActor.m_isDeposited)
        {//托管中，发送move action的消息
            Vector3 curPos = CurrentActor.MainPos;
            IMiniServer.Singleton.SendAction_Move_C2BS(CurrentActor.ID,curPos.x,curPos.z,pos.x, pos.z);
        }
    }
    float m_fCurLerpSpeed = 0f;
    void SetVelocity(Vector3 velo, float fSpeed)
    {
        //m_currentVelocityNormalize = velo.normalized;
        //m_currentSpeed = velo.magnitude;
        //if (CurrentActor.MainRigidBody.isKinematic)
        //{
        //    m_currentVelocityNormalize = velo.normalized;
        //    m_currentSpeed = velo.magnitude;
        //}
        //else
        //{
        //    CurrentActor.MainRigidBody.velocity = velo;
        //}
        float fVel = fSpeed;
        //if (CurrentActor.Type == ActorType.enMain)
        {
            if (velo != Vector3.zero && m_fCurLerpSpeed < fSpeed && GameSettings.Singleton.m_moveAcceleration > 0)
            {
                fVel = Mathf.Lerp(m_fCurLerpSpeed, fSpeed, Time.deltaTime * GameSettings.Singleton.m_moveAcceleration);
                m_fCurLerpSpeed = fVel;
            }
        }
        //Debug.Log("fspeed =" + fVel);
        velo = fVel * velo;
        if (!CurrentActor.MainRigidBody.isKinematic)
        {
            CurrentActor.MainRigidBody.velocity = velo;
//             if (CurrentActor.Type == ActorType.enMain)
//             {
//                 Debug.Log("velo:" + velo.ToString());
//             }
        }
        m_currentSpeed = CurrentActor.MainRigidBody.velocity.magnitude;
        //CurrentActor.MainPos = new Vector3(CurrentActor.MainPos.x, 0f, CurrentActor.MainPos.z);
        ///m_currentVelocityNormalize = velo;
        //CurrentActor.MainCharController.velocity = velo;
    }
    public void RefreshData()
    {
        m_startPos = CurrentActor.MainPos;
        m_startPos.y = 0.0f;
        m_targetForward = TargetPos - CurrentActor.MainPos;
        m_targetForward.y = 0.0f;
        m_targetForward.Normalize();
        m_isMakeSureForward = false;
        //CurrentActor.MainRigidBody.isKinematic = false;
        SetVelocity(m_targetForward.normalized, (m_isBack ? CurrentActor.MovebackSpeed : CurrentActor.MoveSpeed));
	}
//     public void Reforward(Vector3 targetPos)
//     {
//         m_targetForward = targetPos - CurrentActor.MainPos;
//         m_targetForward.y = 0.0f;
//         m_targetForward.Normalize();
//         m_isMakeSureForward = true;
//         m_isBack = false;
// 
//         if (null != CurrentActor.mActorBlendAnim)
//         {
//             CurrentActor.mActorBlendAnim.Retarget(targetPos);
//         }
//     }
    float m_rotateSpeed = 0;
	public override void OnEnter()
	{
        //CurrentActor.MainRigidBody.isKinematic = false;
        RefreshActionRef();
        if (!m_isMakeSureForward)
        {
            CurrentActor.MainRigidBody.mass = CurrentActor.BaseMass;
        }
        if (CurrentActor.Type == ActorType.enNPC)
        {
            NPC npc = CurrentActor as NPC;
            m_rotateSpeed = npc.CurrentTableInfo.RotateSpeed;
        }
        else
        {
            m_rotateSpeed = CurrentActor.PropConfig.RotateSpeed;
        }
        AnimName = "standby";
	}
	public override void OnInterupt()
    {
        if (CurrentActor.m_isDeposited)
        {//托管中，发送action被打断的消息
            IMiniServer.Singleton.SendActionInterupt_C2BS(CurrentActor.ID, (int)GetActionType());
        }
        CurrentActor.MainRigidBody.mass = 1000 * CurrentActor.BaseMass;
        SetVelocity(Vector3.zero, 0f);
        //CurrentActor.MainRigidBody.isKinematic = true;
        m_pathList.ClearCorners();
		if (null != OnInterupted)
		{
			OnInterupted();
		}
        if (null != CurrentActor.mActorBlendAnim)
        {
            CurrentActor.mActorBlendAnim.Reset();
        }
	}
	public override void OnExit()
    {
        CurrentActor.MainRigidBody.mass = 1000 * CurrentActor.BaseMass;
		//CurrentActor.PlayAnimation("standby");
        SetVelocity(Vector3.zero, 0f);
        //CurrentActor.MainRigidBody.isKinematic = true;
        m_pathList.ClearCorners();
		if (null != OnMoveEnd)
		{
			OnMoveEnd();
		}
        if (objList.Count > 0)
        {
            foreach (GameObject obj in objList)
            {
                if (obj != null)
                {
                    GameObject.Destroy(obj);
                }
                //GameObject.DestroyImmediate(obj);
            }
            objList.Clear();
        }
	}
    //上次同步的时间
    float m_lastSyncTime = 0;
    Vector3 m_lastSyncPos = Vector3.zero;
	public override bool OnUpdate()
    {
        if (CurrentActor.m_isDeposited)
        {//被托管，同步位置
            if (Time.time - m_lastSyncTime > GameSettings.Singleton.m_longConnectTickDuration)
            {
                m_lastSyncTime = Time.time;
                if (CurrentActor.MainPos != m_lastSyncPos)
                {
                    m_lastSyncPos = CurrentActor.MainPos;
                    IMiniServer.Singleton.SendSyncPosition_C2BS(CurrentActor.ID, CurrentActor.MainPos.x, CurrentActor.MainPos.z);
                }
            }
        }
        if (CurrentActor.IsCanTeleport)
        {//瞬移
            CurrentActor.SelfTeleport.TeleportOnce(CurrentActor, m_realTargetPos);
            return true;
        }
        bool isForwardArrived=false;
        Vector3 curForward=CurrentActor.MainObj.transform.forward;
        Vector3 v = curForward-m_targetForward;
        if (v.magnitude>0.01f)
        {
            Vector3 vForward = new Vector3(CurrentActor.MainObj.transform.forward.x, 0f, CurrentActor.MainObj.transform.forward.z);
            float fCurForwardAngle = ActorBlendAnim.HorizontalAngle(vForward);
            float fTargetAngle = ActorBlendAnim.HorizontalAngle(m_targetForward);
            if (null == CurrentActor.mActorBlendAnim || CurrentActor.TargetManager.CurrentTarget == null ||
                CurrentActor.mActorBlendAnim.IsRawRotation())//!锁定目标功能,勿添加其它功能逻辑 added by guo
            {
                float fLerp = Mathf.LerpAngle(fCurForwardAngle, fTargetAngle, Time.deltaTime * m_rotateSpeed);
                float fInterval = fLerp - fCurForwardAngle;
                Quaternion quater = Quaternion.Euler(0f, fInterval, 0f);
                CurrentActor.MainObj.transform.rotation = quater * CurrentActor.MainObj.transform.rotation;
                //旋转角度大于定值则不移动,(防止旋转过程中出现滑步的现象)
                float maxRotate = GameSettings.Singleton.m_stopMoveMaxRotationValS;
                if (AnimName == "run")
                {
                    maxRotate = GameSettings.Singleton.m_stopMoveMaxRotationValR;
                }
                if (v.magnitude > maxRotate)
                {
                    SetVelocity(Vector3.zero, 0f);
                    mStartTime = Time.time;
                    mIsRotating = true;
                    return false;
                }
            }
        }
        else
        {
            isForwardArrived = true;
        }
        if (mIsRotating)
        {
            mIsRotating = false;
        }
        //!锁定目标功能,勿添加其它功能逻辑 added by guo
        if (null == CurrentActor.mActorBlendAnim || CurrentActor.TargetManager.CurrentTarget == null)
        {
            AnimName = "run";
        }
        //获得真实的移动速度,(解决角色碰到障碍原地跑动的现象)
        if (Time.time - mStartTime >= 0.2f)
        {
            Vector3 distance = CurrentActor.MainPos - mLastMovePos;
            distance.y = 0;
            mRealSpeed = distance.magnitude / (Time.time - mStartTime);
            mStartTime = Time.time;
            mLastMovePos = CurrentActor.MainPos;
        }
        Vector3 d = CurrentActor.MainPos - m_startPos;
        d.y = 0;
        Vector3 d2 = TargetPos - m_startPos;
        d2.y = 0;
        bool isPosArrived = d.magnitude >= d2.magnitude;
        if (!isPosArrived)
        {
            Vector3 direction = TargetPos - CurrentActor.MainPos;
            direction.y = 0.0f;
            direction.Normalize();
            m_targetForward = m_isBack ? -direction : direction;
            Vector3 velocity = direction;
            float fSpeed = (m_isBack ? CurrentActor.MovebackSpeed : CurrentActor.MoveSpeed);
            if (AnimName == "backward")
            {
                fSpeed = CurrentActor.MovebackSpeed;
            }
            SetVelocity(velocity, fSpeed);
        }
        if (isPosArrived && (!m_isMakeSureForward || isForwardArrived))
		{
            if (PathNodeList.Count > 0)
            {
                TargetPos = PathNodeList[0];
                PathNodeList.RemoveAt(0);
                m_targetForward = TargetPos - CurrentActor.MainPos;
                m_targetForward.y = 0.0f;
                m_targetForward.Normalize();
                m_isMakeSureForward = false;
                m_startPos = CurrentActor.MainPos;
                m_startPos.y = 0.0f;
                return false;
            }
            return true;
		}
        return false;
	}

	public override void Reset()
	{	
		m_isBack = false;
		//!是否不寻路 tgame
		m_isNotAStar = false;
		if (ActorType.enNPC == CurrentActor.Type) {
// 			NPC npc = CurrentActor as NPC;
// 			AINpc aiNpc = npc.SelfAI as AINpc;
// 			aiNpc.m_isGoHome = false;
		}
		base.Reset();
		m_targetPos = Vector3.zero;
		m_startPos = Vector3.zero;
        //m_isSlowDown = false;
        //m_moveSpeedScale = 1.0f;
        m_isBack = false;
        m_fCurLerpSpeed = 0f;
        m_realTargetPos = Vector3.zero;
	}
};