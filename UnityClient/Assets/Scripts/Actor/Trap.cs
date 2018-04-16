using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region disableWarning
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#endregion

public enum TrapType
{
    enNone = 0,
    enTouchType,
    enControl,
    enAuto,
}
public class Trap : Actor
{

    public enum TrapState
    {
        enClose = 0,
        enOpen,
        enLock,
        enDisable,
    }
    public enum TrapEffectType
    {
        enNone,
    }
    public enum TrapGenreType
    {
        enNone,
    }
    public enum TrapTouchType
    {
        enNone,
    }
    public enum TrapCampType
    {
        enNone,
        enMainControl,
        enFriend,
        enNeutrality,
        
    }
    //base Info
    public TrapActionControl mActionControl;
    public TrapAnimationControl mAnimationControl;

    #region   曾经的变量

    //机关信息
    TrapInfo m_trapInfo = null;
    public TrapInfo CurrentTableInfo { get { return m_trapInfo; } private set { m_trapInfo = value; } }
    //机关的激活/未激活/打开/锁定状态的动画名称
    public string mOpenTrapAnim = "", mLocalTrapAnim = "", mCloseTrapAnim = "", mDisableAnim = "";
    //public ActorConfigData NpcConfigData { get; private set; }
    //!是否是激活
    public bool IsActive { get; set; }
    public RoomElement roomElement { get; set; }
    public TrapType mTrapType = TrapType.enNone;
    //!当前所属的ROOM
    public RoomBase CurRoom { set; get; }
    //当前机关状态(开启状态：可触发，关闭状态：不可触发)
    public bool m_trapAble = false;
    public bool TrapAble { set { m_trapAble = value; } get { return m_trapAble; } }
    //机关激活状态
    public bool m_trapActivate = false;
    public bool TrapActive { set { m_trapActivate = value; } get { return m_trapActivate; } }
    //机关状态
    public TrapState mTrapState = TrapState.enClose;
    #endregion

    //机关是否需要钥匙
    public bool m_IsNeedKey = false;
    //机关需要的钥匙的ID
    public int m_needKeyId = -1;

    //最小攻击间隔
    public float mMinAttackTime = 0f;
    //最大攻击次数
    public int mMaxAttackCount = 0;

    //攻击动作的动画名称列表
    public List<string> mAttackActionAnimList;
    //持续伤害的动作名称列表
    public List<string> mContinueDiamgeActionAnimList;
    public List<string> mStandByAnimList;
    //在机关范围内的ActorID
    public List<int> mEnterActorIDList = new List<int>();
    //
    public TrapCampType mTrapCampType = TrapCampType.enNone;
    public Trap(int id, int staticID, CSItemGuid guid)
        : base(id, staticID, guid)
    {
        //重新设置ActionControl
        mActionControl = new TrapActionControl(this);
        //重新设置AinmationControl
        mAnimationControl = new TrapAnimationControl(this);
        //读取机关信息
        CurrentTableInfo = GameTable.TrapInfoTableAsset.Lookup(staticID);
        mTrapCampType = (TrapCampType)CurrentTableInfo.CampType;
        m_IsNeedKey = CurrentTableInfo.NeedKey;
        //创建TrapAI
        IsInitAIDataList = false;
        
        //设置Props信息
        Props = ENProperty.Singleton.CreatePropertySet("SOBTrap");
        SetPropertyObjectID((int)MVCPropertyID.enActorStartID + id);
        //存储表ID
        IDInTable = staticID;
        InitProps();
    }
    public static Trap CreateTrapByType(int id, int staticID, CSItemGuid guid)
    {
        TrapInfo trapInfo = GameTable.TrapInfoTableAsset.Lookup(staticID);
        Trap curTrap = null;
        switch ((TrapType)trapInfo.TrapType)
        {
            case TrapType.enAuto:
                curTrap = new AutoTrap(id, staticID, guid);
                break;
            case TrapType.enControl:
                curTrap = new ControlTrap(id, staticID, guid);
                break;
            case TrapType.enTouchType:
                curTrap = new TouchTrap( id, staticID, guid);
                break;
        }
        return curTrap;
    }
    public int GetTrapCamp()
    {
        return (int)CurrentTableInfo.CampType;
    }
    public override void Destroy()
    {
        base.Destroy();
    }

    private GameObject BodyObject { get; set; }

    public void SetBodyObject(GameObject obj)
    {
        BodyObject = obj;
    }

    public override bool CreateNeedModels()
    {
        string npcPrefab = "Model/EnemyA";
        Load(npcPrefab, CurrentTableInfo.ModelID, CurrentTableInfo.ModelScale, WeaponType);
        MainObj.name = "Trap_" + CurrentTableInfo.TrapType.ToString() + "_" + ID.ToString();
        GameObject body = GetBodyObject();
        body.AddComponent<AnimationCameraCallback>();
        body.AddComponent<AnimationEffectCallback>();
        body.AddComponent<AnimationEndCallback>();
        body.AddComponent<AnimationShaderParamCallback>();
        body.AddComponent<AnimationSoundCallback>();
        body.AddComponent<BreakPointCallback>();

        if (mTrapType == TrapType.enAuto)
        {
            CreateEnterCallBack();
        }
        else if (mTrapType == TrapType.enTouchType)
        {
            CreateEnterCallBack();
            createExitCallBack();
        }
        return true;
    }
    void CreateEnterCallBack()
    {
        //攻击盒子添加回调
        GameObject body = GetBodyObject();
        TrapAttackActionCallback attackActionCallback = body.GetComponent<TrapAttackActionCallback>();
        if (attackActionCallback == null)
        {
            body.AddComponent<TrapAttackActionCallback>();
        }
        TriggerCallback enterCallBack = body.GetComponent<TriggerCallback>();
        if (enterCallBack == null)
        {
            enterCallBack = body.AddComponent<TriggerCallback>();
        }
        enterCallBack.EnterCallback = OnAttackEnterCallBack;
        enterCallBack.ExitCallback = OnAttackExitCallBack;
    }
    void createExitCallBack()
    {
        //为我的检测盒子添加回调
        GameObject body = GetBodyObject().transform.Find("Collider").gameObject;
        TriggerCallback DetectionCallback = body.GetComponent<TriggerCallback>();
        if (DetectionCallback == null)
        {
            DetectionCallback = body.AddComponent<TriggerCallback>();
        }
        DetectionCallback.EnterCallback = OnCheckEnterCallBack;
        DetectionCallback.ExitCallback = OnCheckExitCallBack;
    }
    public void InitProps()
    {
        SM.MonsterData mCurRoomMonsterData = null;
        if (roomElement != null && roomElement.MonsterData != null)
        {
            mCurRoomMonsterData = SM.RandomRoomLevel.Singleton.LookupMonsterData(roomElement.MonsterData.monsterObjId);
        }
        Props.SetProperty_Int32(ENProperty.islive, 1);
        Props.SetProperty_Float(ENProperty.phyattack, 30);
        Props.SetProperty_Float(ENProperty.hit, 50);
    }
    //心跳
    public override void FixedUpdate()
    {
        //base.FixedUpdate();
        mActionControl.FixedUpdate();
        SelfAI.Update();
        mAnimationControl.Tick();
    }

    public override int GetActorInitModelID()
    {
        return CurrentTableInfo.ModelID;
    }
    public override int GetActorInitWeaponID()
    {
        return 0;
    }
    void OnCheckEnterCallBack(GameObject gameObject, Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        Transform targetObj = other.transform;
        while (null != targetObj && targetObj.name != "body")
        {
            targetObj = targetObj.parent;
        }
        if (targetObj == null)
        {
            return;
        }

        ActorProp prop = targetObj.parent.GetComponent<ActorProp>();
        Actor targetActor = prop.ActorLogicObj;
        if (targetActor.Type == ActorType.enNPCTrap)
        {
            return;
        }
        if (!CheckActorTouchResult(targetActor))
        {
            return;
        }
        if (!mEnterActorIDList.Contains(targetActor.ID))
        {
            mEnterActorIDList.Add(targetActor.ID);
        }
        
        /*(SelfAI as AINpcTrap).mEnterActorIDList.Add(targetActor.ID);*/
    }
    void OnCheckExitCallBack(GameObject gameObject, Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        Transform targetObj = other.transform;
        while (null != targetObj && targetObj.name != "body")
        {
            targetObj = targetObj.parent;
        }
        if (targetObj == null)
        {
            return;
        }

        ActorProp prop = targetObj.parent.GetComponent<ActorProp>();
        Actor targetActor = prop.ActorLogicObj;
        if (targetActor.Type == ActorType.enNPCTrap)
        {
            return;
        }
        if (mEnterActorIDList.Contains(targetActor.ID))
        {
            mEnterActorIDList.Remove(targetActor.ID);
        }
        
        /*(SelfAI as AINpcTrap).mEnterActorIDList.Remove(targetActor.ID);*/
    }

    void OnAttackEnterCallBack(GameObject gameObject, Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        Transform targetObj = other.transform;
        while (null != targetObj && targetObj.name != "body")
        {
            targetObj = targetObj.parent;
        }
        if (targetObj == null)
        {
            return;
        }
        TrapAttackAction action = mActionControl.LookupAction(TrapAction.ENType.enAttackAction) as TrapAttackAction;
        if (action != null)
        {
            action.OnTriggerEnter(gameObject, other);
        }
    }
    void OnAttackExitCallBack(GameObject gameObject, Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        Transform targetObj = other.transform;
        while (null != targetObj && targetObj.name != "body")
        {
            targetObj = targetObj.parent;
        }
        if (targetObj == null)
        {
            return;
        }
    }
    public virtual bool OnAttackAction()
    {
        TrapAttackAction attackAction = mActionControl.AddAction(TrapAction.ENType.enAttackAction) as TrapAttackAction;
        if (attackAction != null)
        {
            attackAction.Init(mAttackActionAnimList);
            return true;
        }
        return false;
    }
    public virtual bool OnBackAction()
    {
        return false;
    }
    public virtual bool OnContinueDamageAction()
    {
        return false;
    }
    public virtual bool OnStandByAction()
    {
        return false;
    }
    public virtual void OperateTrap()
    {

    }

    //Get/SET TrapActive
    public bool GetTrapActive()
    {
        return m_trapActivate;
    }
    public virtual void SetTrapActive(bool val)
    {
        m_trapActivate = val;
    }
    //Get/SET TrapAble
    public bool GetTrapAble()
    {
        return m_trapAble;
    }
    public virtual void SetTrapAble(bool val)
    {
        m_trapAble = val;
    }

    public virtual void SetTrapState(TrapState state)
    {
        mTrapState = state;
    }
    public TrapState GetTrapState()
    {
        return mTrapState;
    }

    public override void PlayEffect(string effectName, float effectTime, string posByBone, bool isAdhered, Vector3 offset, bool isAddToAttackAction = false)
    {
        if (false == GameSettings.Singleton.m_playEffect)
        {
            return;
        }
        MainGame.Singleton.StartCoroutine(Coroutine_PlayEffect(effectName, effectTime, posByBone, isAdhered, offset, isAddToAttackAction));
    }
    IEnumerator Coroutine_PlayEffect(string effectName, float effectTime, string posByBone, bool isAdhered, Vector3 offset, bool isAddToAttackAction = false)
    {
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath(effectName), data);
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
            PlayEffect(data.m_obj as GameObject, effectTime, posByBone, isAdhered, offset);
            if (isAddToAttackAction)
            {
                if (ActionControl.IsActionRunning(ActorAction.ENType.enAttackAction))
                {
                    TrapAttackAction action = this.mActionControl.LookupAction(TrapAction.ENType.enAttackAction) as TrapAttackAction;
                    action.AddEffectObj(data.m_obj as GameObject);
                }
            }
        }
        else
        {
            TrapAttackAction action = mActionControl.LookupAction(TrapAttackAction.SGetActionType()) as TrapAttackAction;
            if (action != null)
            {
                Debug.LogWarning("Play effect fail, actorID:" + ID + ",effectName:" + effectName);
            }
            else
            {
                Debug.LogWarning("Play effect fail, name:" + effectName);
            }
        }
    }

    public bool CheckActorAttackResult(Actor targetActor)
    {
        switch (mTrapCampType)
        {
            case TrapCampType.enFriend:
                switch (targetActor.Type)
                {
                    case ActorType.enMain:
                    case ActorType.enPlayer:
                    case ActorType.enFollow:
                    case ActorType.enNPC_Friend:
                    case ActorType.enNPC_AllEnemy:
                    case ActorType.enPlayer_Sneak:
                        return true;
                    case ActorType.enNPC:
                    case ActorType.enMold:
                    case ActorType.enSwitch:
                    case ActorType.enNPCTrap:
                        return false;
                    default:
                        return false;
                }
            case TrapCampType.enNeutrality:
                switch (targetActor.Type)
                {
                    case ActorType.enMain:
                    case ActorType.enPlayer:
                    case ActorType.enNPC:
                    case ActorType.enMold:
                    
                    case ActorType.enFollow:
                    case ActorType.enNPC_Friend:
                    case ActorType.enNPC_AllEnemy:
                    case ActorType.enPlayer_Sneak:
                        return true;
                    case ActorType.enSwitch:
                    case ActorType.enNPCTrap:
                        return false;
                    default:
                        return false;
                }
            case TrapCampType.enMainControl:
                switch (targetActor.Type)
                {
                    case ActorType.enMain:
                    case ActorType.enPlayer:
                    case ActorType.enFollow:
                    case ActorType.enNPC_Friend:
                        return true;
                    default:
                        return false;
                }
        }
        return false;
    }

    public bool CheckActorTouchResult(Actor targetActor)
    {
        switch (mTrapCampType)
        {
            case TrapCampType.enFriend:
                switch (targetActor.Type)
                {
                    case ActorType.enMain:
                    case ActorType.enFollow:
                        return true;
//                        break;
                    default:
                        return false;
//                        break;
                }
//                break;
            case TrapCampType.enNeutrality:
                if (targetActor.Type == ActorType.enNPC)
                {
                    NPC tmpNpc = targetActor as NPC;
                    if (tmpNpc.GetNpcType() == ENNpcType.enBoxNPC)
                    {
                        return false;
                    }
                }
                return true;
//                break;
            case TrapCampType.enMainControl:
                if (targetActor.Type == ActorType.enMain)
                {
                    return true;
                }
                return false;
//                break;
        }
        return false;
    }
    public float PlayAnimation(string animName)
    {
        if (MainAnim == null)
        {
            return 0;
        }
        AnimationClip clip = MainAnim.GetClip(animName);
        if (null == clip)
        {
            //Debug.LogWarning("Miss animation error: " + animName);
            return 0.0f;
        }
        MainAnim.Play(animName);
        return clip.length;
    }
}