using UnityEngine;
using System;
using System.Collections;

public enum ENCameraTargetType
{
    enNone,
    enActor,
    enGameObject,
    enPostion,
    enBackToActor,
}
public class CameraController
{
	public GameObject MainCamera { get; private set; }
    Vector3 m_lastCacheCameraPos = Vector3.zero;
	private float m_baseFieldOfView { get { return GameSettings.Singleton.CameraFieldOfView; } }

    ENCameraTargetType m_curTargetType = ENCameraTargetType.enActor;
    GameObject m_shakeObj = null;
    GameObject m_darkerObj = null;
	public CameraController()
	{

	}

	public void Init(GameObject cameraObj)
	{
		MainCamera = cameraObj;
        m_shakeObj = MainCamera.transform.Find("CameraShake").gameObject;
        m_darkerObj = MainCamera.transform.Find("CameraMaskDark").gameObject;
        m_darkerObj.SetActive(true);
        //m_baseFieldOfView = MainCamera.camera.fieldOfView;
	}

    enum ENCameraLightType
    {
        enNone,
        enL2D,//亮变暗
        enIng,//持续中
        enD2L,//暗变亮
    }
    ENCameraLightType m_cameraLightType = ENCameraLightType.enNone;
    string m_cameraLightL2D, m_cameraLightD2L;
    float m_cameraLightDuration = 0, m_cameraLightStartTime = 0;
    public void CameraCoverLightChange(string lightToDark, string darkToLight, float duration)
    {
        m_cameraLightL2D = lightToDark;
        m_cameraLightD2L = darkToLight;
        m_cameraLightDuration = duration;
        m_cameraLightStartTime = Time.time;
        m_cameraLightType = ENCameraLightType.enL2D;
        m_cameraLightIndex = -1;
    }

    public void StopCameraCoverLightChange()
    {
        if (m_cameraLightType != ENCameraLightType.enNone)
        {
            if (m_cameraLightType != ENCameraLightType.enD2L)
            {
                m_cameraLightIndex = -1;
            }
            m_cameraLightType = ENCameraLightType.enD2L;
        }
    }
    int m_cameraLightIndex = -1;
    void UpdateCameraLight()
    {
        switch (m_cameraLightType)
        {
            case ENCameraLightType.enL2D:
                {
                    AnimCurveData data = AnimCurveDataManager.Singleton.GetAnimCurveData("Anim/" + m_cameraLightL2D + ".bytes");
                    float now = Time.time;
                    if (now - m_cameraLightStartTime > data.m_interval || m_cameraLightIndex == -1)
                    {
                        m_cameraLightStartTime = now;
                        ++m_cameraLightIndex;
                        if (m_cameraLightIndex >= data.AnimCurveInfoList.Count)
                        {
                            m_cameraLightType = ENCameraLightType.enIng;
                            m_cameraLightStartTime = now;
                            m_cameraLightIndex = -1;
                            return;
                        }
                        Renderer[] renders = m_darkerObj.GetComponentsInChildren<Renderer>();
                        foreach (Renderer r in renders)
                        {
                            KeyframeData info = data.AnimCurveInfoList[m_cameraLightIndex % data.AnimCurveInfoList.Count];
                            foreach (var propItem in info.PropertyInfoList)
                            {
                                if (r.material.HasProperty(propItem.m_name))
                                {
                                    //if (propItem.m_color != Color.white)
                                    //{
                                    //    r.material.SetColor(propItem.m_name, propItem.m_color);
                                    //}
                                    if (propItem.m_value != float.MinValue)
                                    {
                                        r.material.SetFloat(propItem.m_name, propItem.m_value);
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            case ENCameraLightType.enIng:
                if (Time.time - m_cameraLightStartTime > m_cameraLightDuration)
                {
                    m_cameraLightType = ENCameraLightType.enD2L;
                    m_cameraLightStartTime = Time.time;
                    m_cameraLightIndex = -1;
                }
                break;
            case ENCameraLightType.enD2L:
                {
                    AnimCurveData data = AnimCurveDataManager.Singleton.GetAnimCurveData("Anim/" + m_cameraLightD2L + ".bytes");
                    float now = Time.time;
                    if (now - m_cameraLightStartTime > data.m_interval || m_cameraLightIndex == -1)
                    {
                        m_cameraLightStartTime = now;
                        ++m_cameraLightIndex;
                        if (m_cameraLightIndex >= data.AnimCurveInfoList.Count)
                        {
                            m_cameraLightType = ENCameraLightType.enNone;
                            return;
                        }
                        Renderer[] renders = m_darkerObj.GetComponentsInChildren<Renderer>();
                        foreach (Renderer r in renders)
                        {
                            KeyframeData info = data.AnimCurveInfoList[m_cameraLightIndex % data.AnimCurveInfoList.Count];
                            foreach (var propItem in info.PropertyInfoList)
                            {
                                if (r.material.HasProperty(propItem.m_name))
                                {
                                    //if (propItem.m_color != Color.white)
                                    //{
                                    //    r.material.SetColor(propItem.m_name, propItem.m_color);
                                    //}
                                    if (propItem.m_value != float.MinValue)
                                    {
                                        r.material.SetFloat(propItem.m_name, propItem.m_value);
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

	public void OnDestroy()
	{
		MainCamera = null;
	}
    float touchScreenHeight;
    float touchScreenWidth;
    public bool IsOutOfScreen(Vector3 vpos)
    {
        touchScreenHeight = Screen.height;
        touchScreenWidth = Screen.width;
        Vector3 m_vpos = MainCamera.GetComponent<Camera>().WorldToScreenPoint(vpos);
        if (m_vpos.x > 0 && m_vpos.x < touchScreenWidth && m_vpos.y > 0 && m_vpos.y < touchScreenHeight)
        {
            return true; 
        }
        return false;
    }
    #region CameraFollow
    Actor m_targetActor = null;
    GameObject m_targetObj = null;
    Vector3 m_targetPos = Vector3.zero;
    float m_changeTime = 0;
    float m_backTime = 0;
    float m_autoBackTime = 0;
    float m_startTime = 0;
    bool m_isChangeY = false;
	public void MoveAtOnce(Actor target)
	{
        m_targetActor = target;
        m_curTargetType = ENCameraTargetType.enActor;
        Vector3 cameraPos = m_targetActor.MainObj.transform.position;
        cameraPos.y = 0.35f;
        MainCamera.transform.position = cameraPos;
        MainCamera.transform.Translate(Vector3.back * GameSettings.Singleton.CameraDistance);
        MainCamera.transform.rotation = Quaternion.Euler(GameSettings.Singleton.CameraRotate);
        m_lastCacheCameraPos = MainCamera.transform.position;
        xVelocity = 0;
        yVelocity = 0;
        zVelocity = 0;
        m_isChangeY = false;
        UpdateImpl(false);
	}
	public void Follow(Actor target)
	{
        m_targetActor = target;
	}
    public void ChangeFollowTarget(GameObject obj, float changeTime, float backTime, bool isChangeY, float autoBackTime = 0)
    {
        if (obj == null || !obj.activeSelf)
        {
            return;
        }
        m_curTargetType = ENCameraTargetType.enGameObject;
        m_targetObj = obj;
        m_changeTime = changeTime;
        m_backTime = backTime;
        m_isChangeY = isChangeY;
        m_autoBackTime = autoBackTime;
        m_startTime = Time.time;
    }
    public void ChangeFollowTarget(Vector3 pos, float changeTime, float backTime, bool isChangeY, float autoBackTime = 0)
    {
        m_curTargetType = ENCameraTargetType.enPostion;
        m_targetPos = pos;
        m_changeTime = changeTime;
        m_backTime = backTime;
        m_isChangeY = isChangeY;
        m_autoBackTime = autoBackTime;
        m_startTime = Time.time;
    }
    //飞行道具用，只接受摄像机跟随的obj的命令
    public void BackToActor(GameObject obj, float backTime)
    {
        if (m_targetObj != obj)
        {
            return;
        }
        BackToActor(backTime);
    }
    public void BackToActor(float backTime)
    {
        m_curTargetType = ENCameraTargetType.enBackToActor;
        m_changeTime = backTime;
        m_startTime = Time.time;
        m_isChangeY = false;
    }
    #endregion
    //private float m_shakeStartTime = 0.0f;
    //private float m_shakeRange = 0.0f;
    //private float m_shakeTime = 0.0f;
    //private int m_shakeType = 0;
    //private float m_nextShakeCameraTime = 0;
    //Vector3 m_shakeOffset = Vector3.zero;
    //public void Shake(float shakeRange, float shakeTime, float fieldOfView, int shakeType)
    //{
    //    m_shakeRange = shakeRange*0.4f;
    //    m_shakeTime = shakeTime;
    //    m_shakeStartTime = Time.realtimeSinceStartup;
    //    if (fieldOfView > 0.0f)
    //    {
    //        MainCamera.camera.fieldOfView = fieldOfView;
    //    }
    //    m_shakeType = shakeType;
    //}

    public void Shake(string animName)
	{
        AnimationClip clip = MainCamera.GetComponent<Animation>().GetClip(animName);
        if (null != clip)
        {
            MainCamera.GetComponent<Animation>().Play(clip.name, PlayMode.StopSameLayer);
        }
        //Shake(GameSettings.Singleton.CameraShackRange, GameSettings.Singleton.CameraShackTime, MainCamera.camera.fieldOfView, 0);
	}
    private bool IsNormalAttack()
    {
        if (m_targetActor.ActionControl.IsActionRunning(ActorAction.ENType.enAttackAction))
        {
            AttackAction action = m_targetActor.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
            return action.IsNormalAttack();
        }
        return false;
    }
    float xVelocity = 0, yVelocity = 0, zVelocity = 0;
    Vector3 m_lastCameraPos = Vector3.zero;
    float m_currentSmoothTime = GameSettings.Singleton.m_cameraSmoothTimeForStart;
    public void Update()
    {
        UpdateImpl(true);
        UpdateCameraLight();
    }
	void UpdateImpl(bool damping)
    {
        //camera跟随目标的位置
        Vector3 cameraPos = Vector3.zero;
        //
        float smoothTime = 0;
        switch (m_curTargetType)
        {
            case ENCameraTargetType.enActor:
                {
                    if (m_targetActor == null)
                    {
                        return;
                    }
                    GameObject obj = m_targetActor.CenterPart;
                    if (m_targetActor.ActionControl.IsActionEmpty() ||
                        m_targetActor.ActionControl.IsActionRunning(ActorAction.ENType.enStandAction) ||
                        m_targetActor.ActionControl.IsActionRunning(ActorAction.ENType.enActorEnterAction) ||
                        IsNormalAttack())
                    {
                        obj = m_targetActor.MainObj;
                    }
                    cameraPos = null != obj ? obj.transform.position : Vector3.zero;

                    if (!m_targetActor.IsDead)
                    {//主控角色未死亡时，对准主控角色跟目标之间
                        Actor target = m_targetActor.CurrentTarget;
                        if (target != null && !target.IsDead)
                        {
                            Vector3 selfPos = cameraPos;
                            Vector3 targetPos = target.MainPos;
                            selfPos.y = 0;
                            targetPos.y = 0;
                            cameraPos = selfPos + (targetPos - selfPos) * 0.5f;
                        }
                    }
                    //计算smooth时间
                    smoothTime = GameSettings.Singleton.m_cameraSmoothTimeForStart;
                    MoveAction action = m_targetActor.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
                    if (action != null && CursorEffectFunc.Singleton.CurFingerType != CursorEffectFunc.ENFingerType.enOnDragMoveType)
                    {
                        float d = ActorTargetManager.GetTargetDistance(m_targetActor.RealPos, action.m_realTargetPos);
                        if (d < GameSettings.Singleton.m_cameraStopDistance)
                        {
                            smoothTime = GameSettings.Singleton.m_cameraSmoothTimeForStop;
                        }
                    }
                    else if (m_lastCameraPos == cameraPos)
                    {//目标停止运动
                        smoothTime = GameSettings.Singleton.m_cameraSmoothTimeForStop;
                    }
                    float velocity = 0;
                    m_currentSmoothTime = Mathf.SmoothDamp(m_currentSmoothTime, smoothTime, ref velocity, GameSettings.Singleton.m_timeForCameraChangeTime);
                }
                break;
            case ENCameraTargetType.enGameObject:
                {
                    cameraPos = m_targetObj.transform.position;
                    smoothTime = m_changeTime;
                    if (m_autoBackTime != 0)
                    {
                        if (Time.time - m_startTime > m_autoBackTime)
                        {
                            m_changeTime = m_backTime;
                            m_startTime = Time.time;
                            m_curTargetType = ENCameraTargetType.enBackToActor;
                        }
                    }
                    m_currentSmoothTime = smoothTime;
                }
                break;
            case ENCameraTargetType.enPostion:
                {
                    cameraPos = m_targetPos;
                    smoothTime = m_changeTime;
                    if (m_autoBackTime != 0)
                    {
                        if (Time.time - m_startTime > m_autoBackTime)
                        {
                            m_changeTime = m_backTime;
                            m_startTime = Time.time;
                            m_curTargetType = ENCameraTargetType.enBackToActor;
                        }
                    }
                    m_currentSmoothTime = smoothTime;
                }
                break;
            case ENCameraTargetType.enBackToActor:
                {
                    GameObject obj = m_targetActor.CenterPart;
                    if (m_targetActor.ActionControl.IsActionEmpty() ||
                        m_targetActor.ActionControl.IsActionRunning(ActorAction.ENType.enStandAction) ||
                        m_targetActor.ActionControl.IsActionRunning(ActorAction.ENType.enActorEnterAction) ||
                        IsNormalAttack())
                    {
                        obj = m_targetActor.MainObj;
                    }
                    cameraPos = null != obj ? obj.transform.position : Vector3.zero;

                    if (!m_targetActor.IsDead)
                    {//主控角色未死亡时，对准主控角色跟目标之间
                        Actor target = m_targetActor.CurrentTarget;
                        if (target != null && !target.IsDead)
                        {
                            Vector3 selfPos = cameraPos;
                            Vector3 targetPos = target.MainPos;
                            selfPos.y = 0;
                            targetPos.y = 0;
                            cameraPos = selfPos + (targetPos - selfPos) * 0.5f;
                        }
                    }

                    smoothTime = m_changeTime;
                    if (Time.time - m_startTime > m_changeTime)
                    {
                        m_curTargetType = ENCameraTargetType.enActor;
                    }
                    m_currentSmoothTime = smoothTime;
                }
                break;
            default:
                {
                    m_curTargetType = ENCameraTargetType.enActor;
                    UpdateImpl(damping);
                    return;
                }
//                break;
        }
        m_lastCameraPos = cameraPos;
        if (!m_isChangeY)
        {
            cameraPos.y = 0.35f;
        }
        
        //拉远
        MainCamera.transform.position = cameraPos;
        MainCamera.transform.Translate(Vector3.back * GameSettings.Singleton.CameraDistance);
        MainCamera.transform.rotation = Quaternion.Euler(GameSettings.Singleton.CameraRotate);
        cameraPos = MainCamera.transform.position;

        //damp
        if (damping)
        {
            float x = Mathf.SmoothDamp(m_lastCacheCameraPos.x, cameraPos.x, ref xVelocity, m_currentSmoothTime, GameSettings.Singleton.m_smoothMaxSpeed);
            float y = Mathf.SmoothDamp(m_lastCacheCameraPos.y, cameraPos.y, ref yVelocity, m_currentSmoothTime, GameSettings.Singleton.m_smoothMaxSpeed);
            float z = Mathf.SmoothDamp(m_lastCacheCameraPos.z, cameraPos.z, ref zVelocity, m_currentSmoothTime, GameSettings.Singleton.m_smoothMaxSpeed);
            cameraPos = new Vector3(x, y, z);
        }
        Vector3 pos = CheckFloat(m_shakeObj.transform.localPosition);
        //shake
        cameraPos += pos;
        MainCamera.transform.position = cameraPos;
        m_lastCacheCameraPos = cameraPos;
    }

    Vector3 CheckFloat(Vector3 pos)
    {
        Vector3 returnPos = pos;
        if (float.IsNaN(pos.x) || float.IsInfinity(pos.x))
        {
            returnPos.x = 0;
        }
        if (float.IsNaN(pos.y) || float.IsInfinity(pos.y))
        {
            returnPos.y = 0;
        }
        if (float.IsNaN(pos.z) || float.IsInfinity(pos.z))
        {
            returnPos.z = 0;
        }
        return returnPos;
    }

	public virtual void OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta)
	{
	}
	public virtual void OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta)
	{
	}
	public virtual void OnTwoFingerDragMoveEnd(Vector2 fingerPos)
	{
	}
	public virtual void OnRotate(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta)
	{
	}
	public virtual void OnRotateEnd(Vector2 fingerPos1, Vector2 fingerPos2, float totalRotationAngle)
	{
	}
}
