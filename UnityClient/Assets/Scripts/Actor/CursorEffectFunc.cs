using UnityEngine;
using System.Collections;

public class CursorEffectFunc 
{
    #region Singleton
    static CursorEffectFunc m_singleton;
    static public CursorEffectFunc Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new CursorEffectFunc();
            }
            return m_singleton;
        }
    }
    #endregion
    public enum ENFingerType
    {
        enNoneType,
        enOnTapType,
        enOnDragMoveType,
        enOnPressDownType,
        enOnPressUpType,
        enOnDragMoveEndType,
    }
    enum ENCursorType
    {
        enNoneType,
        enChooseTarget_E00,
        enClickGround_E00,
        enClickGround_E01,
    }
    public void Destroy()
    {
        if (null != mCursorEffect)
        {
            //GameObject.Destroy(mCursorEffect);
            PoolManager.Singleton.ReleaseObj(mCursorEffect);
            mCursorEffect = null;
        }
        if (mFlagMoveDirection != null)
        {
            //GameObject.Destroy(mFlagMoveDirection);
            PoolManager.Singleton.ReleaseObj(mFlagMoveDirection);
            mFlagMoveDirection = null;
        }
        mIsTrigger = false;
    }
    public void Active()
    {
        mIsTrigger = true;
    }
    MainPlayer mainPlayer { get { return ActorManager.Singleton.MainActor; } }
    GameObject mCursorEffect = null;
    GameObject mFlagMoveDirection = null;
    public ENFingerType CurFingerType { get; protected set; }
    ENCursorType CurCursorType { get; set; }
    Vector3 LastMovePos { get; set; }
    float mMoveEffectSpeed = 0.08f;
    bool mIsTrigger = true;
    //////////////////////////////////////////////////////////////////////////
    public void OnFixedUpdate()
    {
        if (null != mCursorEffect && CurCursorType == ENCursorType.enClickGround_E01)
        {
            if (mainPlayer.IsMoveAfterSwitch)
            {
                return;
            }
            if (!mainPlayer.ActionControl.IsActionRunning(ActorAction.ENType.enMoveAction))
            {
                //GameObject.Destroy(mCursorEffect);
                PoolManager.Singleton.ReleaseObj(mCursorEffect);
                mCursorEffect = null;
            }
            else
            {
                MoveAction action = mainPlayer.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
                float fDiff = action.mRealSpeed / action.m_currentSpeed;
                if (fDiff <= 0.1f)
                {
                    //GameObject.Destroy(mCursorEffect);
                    PoolManager.Singleton.ReleaseObj(mCursorEffect);
                    mCursorEffect = null;
                }
            }
        }
    }
    public void OnLateUpdate()
    {
        if (null != mFlagMoveDirection)
        {
            mFlagMoveDirection.transform.localPosition = ActorManager.Singleton.MainActor.CenterPartWorldPos;
        }
    }
    /// <summary>
    /// /////////////////////
    /// </summary>
    /// <param name="fingerType"></param>
    public void OnTriggerEvent(ENFingerType fingerType, Vector3 curPos)
    {
        if (!mIsTrigger)
        {
            return;
        }
        //ENFingerType lastType = CurFingerType;
        CurFingerType = fingerType;
        if (fingerType == ENFingerType.enOnPressDownType || fingerType == ENFingerType.enOnTapType
            || fingerType == ENFingerType.enOnDragMoveEndType)
        {
            if (mCursorEffect != null)
            {
                //GameObject.Destroy(mCursorEffect);
                PoolManager.Singleton.ReleaseObj(mCursorEffect);
                mCursorEffect = null;
            }
        }
        else if (fingerType == ENFingerType.enOnPressUpType)
        {
            if (mCursorEffect != null)
            {
                //GameObject.Destroy(mCursorEffect);
                PoolManager.Singleton.ReleaseObj(mCursorEffect);
                mCursorEffect = null;
            }
            if (mFlagMoveDirection != null)
            {
                //GameObject.Destroy(mFlagMoveDirection);
                PoolManager.Singleton.ReleaseObj(mFlagMoveDirection);
                mFlagMoveDirection = null;
            }
            if (null != mainPlayer.mActorBlendAnim)
            {
                mainPlayer.mActorBlendAnim.OnPressUp();
            }
            
            return;
        }
        else if (fingerType == ENFingerType.enOnDragMoveType)
        {
            if (CurCursorType != ENCursorType.enClickGround_E00)
            {
                if (mCursorEffect != null)
                {
                    //GameObject.Destroy(mCursorEffect);
                    PoolManager.Singleton.ReleaseObj(mCursorEffect);
                    mCursorEffect = null;
                }
            }
        }
        Ray ray = Camera.main.ScreenPointToRay(curPos);
        RaycastHit[] rayHitArray = Physics.RaycastAll(ray, 1500.0f);
        if (rayHitArray.Length == 0)
        {
            return;
        }
        int i = 0;
        for (; i < rayHitArray.Length; i++)
        {
            if (rayHitArray[i].collider.gameObject.name == "GlobalGround")
            {
                break;
            }
        }
        if (i >= rayHitArray.Length)
        {
            return;
        }
        RaycastHit hitInfo = rayHitArray[i];
        Vector3 worldPos = new Vector3(hitInfo.point.x, 0.2f, hitInfo.point.z);
        MainGame.Singleton.mSelectBoxObj.transform.localPosition = worldPos;
        Collider collider = MainGame.Singleton.mSelectBoxObj.gameObject.GetComponent<Collider>();
        Actor actor = ActorManager.Singleton.ForEach_Bound(collider.bounds);
        if (null != actor && mainPlayer.IsCanSelectTarget(actor))
        {
            Actor lastActor = mainPlayer.TargetManager.CurrentTarget;
            if (actor != mainPlayer)
            {
                if (fingerType == ENFingerType.enOnPressDownType)
                {
                    SetChooseTargetEffect00(actor);
                }
                else if (fingerType == ENFingerType.enOnTapType ||
                    fingerType == ENFingerType.enOnDragMoveEndType)
                {
                    mainPlayer.TargetManager.ClearTarget();
                    mainPlayer.TargetManager.SetCurrentTarget(actor);
                    if (actor.Type == ActorType.enNPC || actor.Type == ActorType.enNPCTrap)
                    {
                        mainPlayer.NotifyChangedButton(actor);
                    }
                }
                else if (fingerType == ENFingerType.enOnDragMoveType)
                {
                    SetChooseTargetEffect00(actor);
                    MainPlayer.Cmd cmd = new MainPlayer.Cmd(hitInfo.point);
                    cmd.m_isMoveByNoAStar = true;
                    mainPlayer.CurrentCmd = cmd;
                }
            }
            else
            {
                if (fingerType == ENFingerType.enOnPressDownType)
                {
                    SetClickGroundEffect00(hitInfo.point);
                }
                else if (fingerType == ENFingerType.enOnDragMoveType)
                {
                    SetDragMoveEffect(hitInfo.point);
                }
            }
            if (fingerType == ENFingerType.enOnTapType)
            {
                if (lastActor != actor && actor != mainPlayer)
                {
                    AttackAction action = mainPlayer.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
                    if (action != null && action.IsNormalAttack())
                    {//普攻，移除AttackAction
                        mainPlayer.ActionControl.RemoveAction(ActorAction.ENType.enAttackAction);
                    }
                }
                if (actor.Type == ActorType.enNPC || actor.Type == ActorType.enNPCTrap)
                {
                    mainPlayer.FireNormalSkill();
                }
            }
            return;
        }
        if (fingerType == ENFingerType.enOnPressDownType)
        {
            SetClickGroundEffect00(hitInfo.point);
            if (null != mainPlayer.mActorBlendAnim)
            {
                mainPlayer.mActorBlendAnim.OnPressDown(hitInfo.point);
            }
        }
        else if (fingerType == ENFingerType.enOnTapType ||
            fingerType == ENFingerType.enOnDragMoveEndType)
        {
            if (fingerType == ENFingerType.enOnTapType)
            {
                StartMove(hitInfo.point);
            }
            else
            {
                //mainPlayer.CurrentCmd = new MainPlayer.Cmd(hitInfo.point);
            }
            //
        }
        else if (fingerType == ENFingerType.enOnDragMoveType)
        {
            MainPlayer.Cmd cmd = new MainPlayer.Cmd(hitInfo.point);
            cmd.m_isMoveByNoAStar = true;
            mainPlayer.CurrentCmd = cmd;
            OnDragMoveEffectOp(curPos, hitInfo.point);
            if (null != mainPlayer.mActorBlendAnim)
            {
                mainPlayer.mActorBlendAnim.OnPressDown(hitInfo.point);
            }
        }
    }
    void StartMove(Vector3 vTarget)
    {
        MainGame.Singleton.StartCoroutine(StartMove(GameSettings.Singleton.m_upperRotateDuration, vTarget));
    }
    private IEnumerator StartMove(float duration, Vector3 vTarget)
    {
        yield return new WaitForSeconds(duration);
        mainPlayer.CurrentCmd = new MainPlayer.Cmd(vTarget);
    }
    string cursorEffectName = "ef-e-clickground-E00";
    void CheckCursorEffectName(Vector3 curPos)
    {
        string lastName = cursorEffectName;
        if (!SM.RandomRoomLevel.Singleton.QuickFindPath(ActorManager.Singleton.MainActor.RealPos, curPos))
        {
            cursorEffectName = "ef-e-unabletoreach-E01";
        }
        else
        {
            cursorEffectName = "ef-e-clickground-E00";
        }
        if (null != mCursorEffect && lastName != cursorEffectName)
        {
            //GameObject.Destroy(mCursorEffect);
            PoolManager.Singleton.ReleaseObj(mCursorEffect);
            mCursorEffect = null;
        }
    }
    void SetDragMoveEffect(Vector3 targetPos)
    {
        if (null == mFlagMoveDirection)
        {
            MainGame.Singleton.StartCoroutine(Coroutine_LoadPressGroudEffect(targetPos));
        }
        else
        {
            SetPressGroudEffectPos(targetPos);
        }
        //判断是否显示禁止移动符
        //CheckCursorEffectName(targetPos);
        if (cursorEffectName != "ef-e-clickground-E00")
        {
            cursorEffectName = "ef-e-clickground-E00";
            PoolManager.Singleton.ReleaseObj(mCursorEffect);
            mCursorEffect = null;
        }
        //点击地面特效
        if (null == mCursorEffect)
        {
            CurCursorType = ENCursorType.enClickGround_E00;
            MainGame.Singleton.StartCoroutine(Coroutine_LoadCursorEffect(targetPos));
        }
        else
        {
            mCursorEffect.transform.localPosition = new Vector3(targetPos.x, 0f, targetPos.z);
        }
    }

    IEnumerator Coroutine_LoadCursorEffect(Vector3 targetPos, Transform t = null)
    {
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath(cursorEffectName), data);
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
            mCursorEffect = data.m_obj as GameObject;
            if (t == null)
            {
                t = MainGame.Singleton.MainObject.transform;
            }
            mCursorEffect.transform.parent = t;
            mCursorEffect.transform.localPosition = new Vector3(targetPos.x, 0f, targetPos.z);
            mCursorEffect.transform.rotation = new Quaternion(0, 0, 0, 0);
            mCursorEffect.transform.localScale = Vector3.one;
            mCursorEffect.SetActive(true);
        }
    }
    IEnumerator Coroutine_LoadPressGroudEffect(Vector3 targetPos)
    {
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath("ef-e-pressground-E01"), data);
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
            mFlagMoveDirection = data.m_obj as GameObject;
            SetPressGroudEffectPos(targetPos);
        }
    }
    void SetPressGroudEffectPos(Vector3 targetPos)
    {
        if (mFlagMoveDirection != null)
        {
            mFlagMoveDirection.transform.parent = MainGame.Singleton.MainObject.transform;
            Vector3 vDirection = targetPos - ActorManager.Singleton.MainActor.CenterPartWorldPos;
            vDirection.y = 0.0f;
            vDirection.Normalize();
            mFlagMoveDirection.transform.localPosition = ActorManager.Singleton.MainActor.CenterPartWorldPos;
            mFlagMoveDirection.transform.rotation = Quaternion.LookRotation(vDirection.normalized) * Quaternion.LookRotation(Vector3.forward);
            mFlagMoveDirection.transform.localScale = Vector3.one;
            mFlagMoveDirection.SetActive(true);
        }
    }
    void OnDragMoveEffectOp(Vector3 curPos, Vector3 targetPos)
    {
        if (LastMovePos.x == curPos.x && LastMovePos.y == curPos.y)
        {
            mMoveEffectSpeed += 0.1f;
            if (mMoveEffectSpeed > 1.0f)
            {
                mMoveEffectSpeed = 1.0f;
            }
        }
        else
        {
            mMoveEffectSpeed = 0.08f;
        }
        LastMovePos = curPos;
        if (null == mFlagMoveDirection)
        {
            MainGame.Singleton.StartCoroutine(Coroutine_LoadPressGroudEffect(targetPos));
        }
        else
        {
            SetPressGroudEffectPos(targetPos);
        }
        //判断是否显示禁止移动符
        //CheckCursorEffectName(targetPos);
        if (cursorEffectName != "ef-e-clickground-E00")
        {
            cursorEffectName = "ef-e-clickground-E00";
            PoolManager.Singleton.ReleaseObj(mCursorEffect);
            mCursorEffect = null;
        }
        //点击地面特效
        if (null == mCursorEffect)
        {
            CurCursorType = ENCursorType.enClickGround_E00;
            MainGame.Singleton.StartCoroutine(Coroutine_LoadCursorEffect(targetPos));
        }
        else
        {
            Vector3 selfPos = mCursorEffect.transform.localPosition;
            float xVelocity = 0, zVelocity = 0;
            float x = Mathf.SmoothDamp(selfPos.x, targetPos.x, ref xVelocity, 0.05f);
            float z = Mathf.SmoothDamp(selfPos.z, targetPos.z, ref zVelocity, 0.05f);
            mCursorEffect.transform.localPosition = new Vector3(x, 0f, z);
            //Debug.LogWarning("time " + Time.deltaTime.ToString());
        }
    }
    public void SetClickGroundEffect01(Vector3 curPos)
    {
        //点击地面特效
        if (null == mCursorEffect)
        {
            CurCursorType = ENCursorType.enClickGround_E01;
            cursorEffectName = "ef-e-clickground-E01";
            MainGame.Singleton.StartCoroutine(Coroutine_LoadCursorEffect(curPos));
        }
    }
    void SetClickGroundEffect00(Vector3 curPos)
    {
        //点击地面特效
        CheckCursorEffectName(curPos);
        if (null == mCursorEffect)
        {
            CurCursorType = ENCursorType.enClickGround_E00;
            MainGame.Singleton.StartCoroutine(Coroutine_LoadCursorEffect(curPos));
        }
        else
        {
            mCursorEffect.transform.localPosition = new Vector3(curPos.x, 0f, curPos.z);
        }
    }
    void SetChooseTargetEffect00(Actor actor)
    {
        if (CurCursorType != ENCursorType.enChooseTarget_E00)
        {
            if (null != mCursorEffect)
            {
                //GameObject.Destroy(mCursorEffect);
                PoolManager.Singleton.ReleaseObj(mCursorEffect);
                mCursorEffect = null;
            }
        }
        if (actor.MainObj != null)
        {
            if (mCursorEffect == null)
            {
                CurCursorType = ENCursorType.enChooseTarget_E00;
                cursorEffectName = "ef-e-choosetarget-E00";
                MainGame.Singleton.StartCoroutine(Coroutine_LoadCursorEffect(Vector3.zero, actor.MainObj.transform));
            }
            else
            {
                mCursorEffect.transform.parent = actor.MainObj.transform;
                mCursorEffect.transform.localPosition = Vector3.zero;
                mCursorEffect.transform.rotation = new Quaternion(0, 0, 0, 0);
                mCursorEffect.transform.localScale = Vector3.one;
            }
        }
    }
}
