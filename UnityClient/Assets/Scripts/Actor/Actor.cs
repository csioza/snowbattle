using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : IPropertyObject
{
    #region CMD
    public enum ENCmdType
    {
        enMove,
        enSkill,
        enLoopNormalAttack,
        enStop,
        enRoll,
        enSwitchActor,
		enActorEnter,
		enActorExit,
		enInteruptAction,
		enBeHitAction,
		enJumpInAction,
		enJumpOutAction,
        enAttackingMoveAction,
        enStopMove,
    }
    public class Cmd
    {
        public ENCmdType m_type;
        public int m_skillID;
        public int m_targetID;
        public Vector3 m_moveTargetPos;
        //!同步位置,服务器发送信息过来时，会设置这个值，表示部分action的其实坐标
		public Vector3 m_syncPos;
        //!同步位置是否有效
        public bool IsSyncPosValidate;
		public Vector3 m_forward;
		//打断命令中被打断的Action的类型
		public int m_interruptedActionType;
		//受击命令的参数
		public int m_srcActorID;
		public bool m_isBack;
		public bool m_isFly;
        public bool m_isMoveByNoAStar = false;

        public Cmd(int skillID)
        {
            m_type = ENCmdType.enSkill;
            m_skillID = skillID;
            IsSyncPosValidate = false;
        }
        public Cmd(int skillID, int targetID)
        {
            m_type = ENCmdType.enSkill;
            m_skillID = skillID;
            m_targetID = targetID;
            IsSyncPosValidate = false;
        }
        public Cmd(int skillID, int targetID,Vector3 syncPos)
        {
            m_type = ENCmdType.enSkill;
            m_skillID = skillID;
            m_targetID = targetID;
            m_syncPos = syncPos;
            IsSyncPosValidate = true;
        }
        static public Cmd CreateMovingAttack(int skillID, int targetID, Vector3 syncPos)
        {
            Cmd cmd = new Cmd(skillID,targetID,syncPos);
            cmd.m_type = ENCmdType.enAttackingMoveAction;
            return cmd;
        }
        static public Cmd CreateSyncMove(Vector3 targetPos, Vector3 startPos)
        {
            Cmd cmd = new Cmd(targetPos);
            cmd.IsSyncPosValidate = true;
            cmd.m_syncPos = startPos;
            return cmd;
        }
        public Cmd(Vector3 pos, ENCmdType type = ENCmdType.enMove)
        {
            m_type = type;
            m_moveTargetPos = pos;
            IsSyncPosValidate = false;
        }
        public Cmd(ENCmdType type)
        {
            m_type = type;
            m_interruptedActionType = 0;
            IsSyncPosValidate = false;
        }
		public Cmd(ENCmdType type, Vector3 curPos, Vector3 forward)
		{
			m_type = type;
            m_syncPos = curPos;
            IsSyncPosValidate = true;
			m_forward = forward;
		}
        public Cmd(Vector3 curPos, Vector3 targetPos)
        {
            m_type = ENCmdType.enRoll;
            m_syncPos = curPos;
            IsSyncPosValidate = true;
            m_moveTargetPos = targetPos;
        }
        public Cmd()
        {
            m_type = ENCmdType.enStop;
            IsSyncPosValidate = false;
        }
        public void SetInterruptParam(int interruptedActionType)
        {
            m_interruptedActionType = interruptedActionType;
        }
		public void SetBeHitParam(int srcActorID, bool isBack, bool isFly)
		{
			m_srcActorID = srcActorID;
			m_isBack = isBack;
			m_isFly = isFly;
		}
		public void SetJumpInParam(int targetID)
		{
			m_targetID = targetID;
		}
    }
    private Cmd m_currentCmd = null;
    public Cmd CurrentCmd
    {
        get { return m_currentCmd; }
        set
        {
            m_currentCmd = value;
            //if (m_currentCmd == null)
            //{
            //    Debug.LogWarning("cmd is null");
            //}
            //else
            //{
            //    Debug.LogWarning("cmd type is " + m_currentCmd.m_type.ToString());
            //}
        }
    }
    #endregion
    //是否托管
    public bool m_isDeposited = false;
    public enum ENPropertyChanged
    {
        /// <summary>
        /// 1-55 skillID
        /// </summary>
        enBossBloodBar = 10001,
        enMainHead = 10002,
        enRelive = 10003,
		enDirectRelive = 10004,
		enCountDown = 10005,
        enRestart       = 10006,
        enSwitchSkill,
        enSetCamera,//heizTest
        enBattleBtn_L,
        enBattleBtn,
        enBattleBtn_D,
        enPauseBtn,
        enMainPlayerDead,
        //enSoulCharge,
    }
    //临时的actor类型
    public ActorType TempType;
    //临时阵营
    private ENCamp m_tempCamp = ENCamp.enNone;
    public ENCamp TempCamp
    {
        get
        {
            if (m_tempCamp == ENCamp.enNone)
            {
                m_tempCamp = (ENCamp)this.Camp;
            }
            return m_tempCamp;
        }
        set
        {
            m_tempCamp = value;
        }
    }
    public int ID { get; protected set; }
    public int SobTag { get; protected set; }
    public ActorType Type { get; private set; }
    public bool IsDead { get { return Props.GetProperty_Int32(ENProperty.islive) == 0; } }
    //!是否真正死亡，不包含复活BUFF
    public bool IsRealDead { get; set; }

    public PropertySet Props { get; set; }
	public CSItemBag ItemBag { get; set; }

	public ActorPropConfig PropConfig { get; private set; }

    public CSItemGuid GUID { get; private set; }

	public GameObject MainObj { get; private set; }
    public Transform MainTransform { get; private set; }
	public GameObject ShadowObj { get; private set; }
	public Vector3 MainPos 
	{
		get
        {
            if (MainTransform != null)
            {
                return MainTransform.localPosition;
            }
            else
            {
                return Vector3.zero;
            }
        }
        set { MainTransform.localPosition = value; }
	}

	//血条
	public Transform HPBarTrans { get; private set; }
	public GameObject mHPBarObj;
	public UIHPBar m_uiHPBar = null;
	public float m_hpBarShowStartTime = 0;
	public bool m_isHPBarNeedShow = true;

    Transform m_textTrans;
	public Transform TextTrans
    {
        get
        {
            if (m_textTrans == null)
            {
                GameObject objAP = GetObject_AdherentPoints();
                if (null != objAP)
                {
                    m_textTrans = objAP.transform.Find("TextPoint");
                    if (m_textTrans == null)
                    {
                        Debug.LogWarning("Get TextTrans fail,id:" + ID);
                    }
                }
            }
            return m_textTrans;
        }
        private set
        {
            m_textTrans = null;
        }
    }

    //!真实的碰撞体所跟随的对象,只跟随位置，不跟随旋转
    Transform m_colliderFollowObj;
    protected float mCapsulYSize = 0.0f;
    public float mCapsulRangeSize { get; protected set; }
    //起始位置 added by luozj
    public Vector3 m_startAttackPos = Vector3.zero;
    //摄像机跟随位置,使用时判断其安全性
    public GameObject CenterPart { get; private set; }
    //使用时判断其安全性
    public Collider CenterCollider { get; private set; }
    public Collider[] ColliderArray { get; private set; }
    bool RealPosIsValidate = false;
    public Vector3 RealPos { get; private set; }
    public Animation MainAnim { get; private set; }
    //public SkinnedMeshRenderer MeshRender { get; private set; }
    public Rigidbody MainRigidBody { get; private set; }
    public mainPlayerCfg MainPlayerConfig { get; private set; }
    //AdherentPoints
    public ActorAdherentPoints AdherentPoints { get; private set; }
    //public CharacterController MainCharController { get; private set; }
    public int mRedFlashTriggerPercent { get; set; }
    //攻击者方向的起始点与结束点
    public Vector3 mCurAttackBoneStartPos = Vector3.zero;
    public Vector3 mCurAttackBoneEndPos = Vector3.zero;
    //爆破点
    public Vector3 mBlastPoint { get; set; }
    public Vector3 mBlastForward { get; set; }

    public Vector3 mLastMovePos = Vector3.zero;
    public float mStartTime = 0;
    public float mRealSpeed = 0f;
    public Vector3 CenterPartWorldPos { get; private set; }
    public ActorBlendAnim mActorBlendAnim = null;
    //public NavMeshAgent MainNavMeshAgent { get; private set; }
    //actor的配置数据
    public ActorConfigData ActorCfgData { get; private set; }
    public CSItemGuid equipHeadID ;
    public CSItemGuid equipWeapenID;
    public CSItemGuid equipBodyID;
    public CSItemGuid equipFootID;
    //!
    public float BaseMass = 1.0f;

    public int LastFinishDungeonID;
    public int LastFinishDungeonDiff;
    public int CurrentDungeonID;
    public int CurrentDungeonDiffID;
    public uint[] mActionRefCountDict = new uint[(int)ActorAction.ENType.Count];
	//默认速度
    protected float m_defaultVelocity = 4.0f;
    //角色所在room GUID
    public int mCurRoomGUID
    {
        get { return SM.RandomRoomLevel.Singleton.LookupRoomGUID(MainPos); }
    }
    //在表里的id
    public int IDInTable
    {
        get { return Props.GetProperty_Int32(ENProperty.IDInTable); }
        set { Props.SetProperty_Int32(ENProperty.IDInTable, value); }
    }
    public float HP
    {
        get { return Props.GetProperty_Float(ENProperty.hp); }
        set { Props.SetProperty_Float(ENProperty.hp, value); }
    }
    public float MaxHP
    {
        get { return Props.GetProperty_Float(ENProperty.maxhp); }
        set { Props.SetProperty_Float(ENProperty.maxhp, value); }
    }
    public float MovebackSpeed
    {
        get { return Props.GetProperty_Float(ENProperty.MovebackSpeed); }
        set { Props.SetProperty_Float(ENProperty.MovebackSpeed, value); }
    }
	public float MoveSpeed
	{
		get { return Props.GetProperty_Float(ENProperty.runSpeed); }
        set { Props.SetProperty_Float(ENProperty.runSpeed, value); }
    }
    public float AnimationSpeed
    {
        get { return Props.GetProperty_Float(ENProperty.AnimationSpeed); }
        set { Props.SetProperty_Float(ENProperty.AnimationSpeed, value); }
    }
    public int Level
    {
        get { return Props.GetProperty_Int32(ENProperty.level); }
        set { Props.SetProperty_Int32(ENProperty.level, value); }
    }
    public float SkillCDModifyValue
    {
        get { return Props.GetProperty_Float(ENProperty.SkillCDModifyValue); }
        set { Props.SetProperty_Float(ENProperty.SkillCDModifyValue, value); }
    }
    public float SkillCDModifyPercent
    {
        get { return Props.GetProperty_Float(ENProperty.SkillCDModifyPercent); }
        set { Props.SetProperty_Float(ENProperty.SkillCDModifyPercent, value); }
    }
    public int WeaponType
    {
        get { return Props.GetProperty_Int32(ENProperty.WeaponID); }
        set { Props.SetProperty_Int32(ENProperty.WeaponID, value); }
    }
    public float PositionX
    {
        get { return Props.GetProperty_Float(ENProperty.positionX); }
        set { Props.SetProperty_Float(ENProperty.positionX, value); }
    }
    public float PositionZ
    {
        get { return Props.GetProperty_Float(ENProperty.positionZ); }
        set { Props.SetProperty_Float(ENProperty.positionZ, value); }
    }
    public int Camp
    {
        get { return Props.GetProperty_Int32(ENProperty.camp); }
        set { Props.SetProperty_Int32(ENProperty.camp, value); }
    }

	public float ModelScale
	{
        get { return Props.GetProperty_Float(ENProperty.ModelScale); }
        set { Props.SetProperty_Float(ENProperty.ModelScale, value); }
	}
    public string Name
    {
        get { return Props.GetProperty_String(ENProperty.name); }
        set { Props.SetProperty_String(ENProperty.name, value); }
    }

    //
    public int KillEnemy
    {
        get { return Props.GetProperty_Int32(ENProperty.KillEnemy); }
        set { Props.SetProperty_Int32(ENProperty.KillEnemy, value); }
    }
    //在表里的id
    public int BeKilled
    {
        get { return Props.GetProperty_Int32(ENProperty.BeKilled); }
        set { Props.SetProperty_Int32(ENProperty.BeKilled, value); }
    }
    //在表里的id
    public float ReliveTime
    {
        get { return Props.GetProperty_Float(ENProperty.ReliveTime); }
        set { Props.SetProperty_Float(ENProperty.ReliveTime, value); }
    }
    //攻击动作播放速度
    public float AttackAnimSpeed
    {
        get { return Props.GetProperty_Float(ENProperty.AttackAnimSpeed); }
        set { Props.SetProperty_Float(ENProperty.AttackAnimSpeed, value); }
    }
    //角色是否退场（主控角色使用）
    public bool IsActorExit
    {
        get { return Props.GetProperty_Int32(ENProperty.IsActorExit) == 1; }
        set { Props.SetProperty_Int32(ENProperty.IsActorExit, value ? 1 : 0); }
    }

    //当前模型ID
    public int CurModelID = 0;
    //当前模型对应的武器ID
    public int CurModel2WeaponID = 0;

    #region SkillBag
    //actor的技能信息
    public class ActorSkillInfo
    {
        //技能信息
        public SkillInfo SkillTableInfo { get; set; }
        //技能等级
        public int SkillLevel { get; set; }
        //是否沉默
        public bool IsSilence { get; set; }
        //最小combo需求
        public int MinComboRequir
        {
            get
            {
                return SkillTableInfo.MinComboRequir + (int)(SkillTableInfo.ComboRequirementParam * SkillLevel);
            }
        }

        public ActorSkillInfo()
        {
            SkillTableInfo = new SkillInfo();
            SkillLevel = 0;
            IsSilence = false;
        }
        public ActorSkillInfo(SkillInfo info, int skillLevel)
        {
            SkillTableInfo = new SkillInfo();
            SkillTableInfo = info;
            SkillLevel = skillLevel;
            IsSilence = false;
        }
        //是否可以释放
        public bool IsCanFire(Actor self, bool isCMD = false)
        {
            if (!isCMD)
            {//不是命令技能
                if (SkillTableInfo.SkillType == (int)ENSkillType.enSwordSoul ||
                    SkillTableInfo.SkillType == (int)ENSkillType.enBreakStamina)
                {//剑魂技或破韧技
                    return false;
                }
            }
            if (IsSilence)
            {//沉默
                return false;
            }
            if (self.IsSkillCDRunning(SkillTableInfo.ID))
            {//cd中
                return false;
            }
            if (self.CurrentRage < SkillTableInfo.CostRagePoint)
            {//怒气值不足
                return false;
            }
            if (self.Combo != null)
            {
                if (MinComboRequir > self.Combo.TotalComboNumber)
                {//combo值不足
                    return false;
                }
            }
            return true;
        }
    }
    //技能背包
    public List<ActorSkillInfo> SkillBag { get; protected set; }
    public Dictionary<int, int> m_usedSkillCountBySkillId = new Dictionary<int, int>();
    #endregion
    //瞬移
    public Teleport SelfTeleport { get; set; }
    public bool IsCanTeleport
    {
        get
        {
            if (SelfTeleport != null)
            {
                return SelfTeleport.IsCanTeleport();
            }
            return false;
        }
    }
    //combo props
    protected ComboProps m_combo = null;
    public ComboProps Combo { get { return m_combo; } set { m_combo = value; } }
    //怒气值
    public float CurrentRage = 0;
    //AI内用到的所有数据是否初始化标志
    public bool IsInitAIDataList = false;



    #region CurrentState
    //战斗状态持续时间
    float m_fightDuration = 0;
    float FightDuration
    {
        get
        {
            if (m_fightDuration == 0)
            {
                m_fightDuration = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enActroState_FightDuration).FloatTypeValue;
            }
            return m_fightDuration;
        }
    }
    //被标记为目标的数量
    int m_beTargetedNumber = 0;
    public int BetargetedNumber
    {
        get { return m_beTargetedNumber; }
        set
        {
            m_beTargetedNumber = value;
            if (m_beTargetedNumber == 0)
            {
                StartCaution();
            }
        }
    }
    //状态
    public enum ENActorState
    {
        enStateNone,
        enStateFight,//战斗状态
        enStateCaution,//警惕状态，现在有翻滚后、释放无目标技能后
    }
    //警惕状态开始时间
    private float m_cautionStartTime = 0;
    public void StartCaution()
    {
        m_cautionStartTime = Time.realtimeSinceStartup;
    }
    public ENActorState CurrentState
    {
        get
        {
            if (BetargetedNumber > 0)
            {//被标记为目标
                return ENActorState.enStateFight;
            }
            if (CurrentTarget != null)
            {//有攻击目标
                return ENActorState.enStateFight;
            }
            if (Time.realtimeSinceStartup - m_cautionStartTime > FightDuration)
            {
                return ENActorState.enStateNone;
            }
            else
            {//警惕中
                return ENActorState.enStateCaution;
            }
        }
    }
    public bool IsState_Fight { get { return CurrentState == ENActorState.enStateFight; } }
    public bool IsState_ReadyToFight
    {
        get
        {
            if (CurrentState == ENActorState.enStateFight ||
                CurrentState == ENActorState.enStateCaution)
            {
                return true;
            }
            return false;
        }
    }
    #endregion
    #region Constructor
    public Actor(int id, int staticID, CSItemGuid guid)
    {
        IsRealDead = false;
        Type = ActorType.enNPCTrap;
        TempType = Type;
        ID = id;
        GUID = guid;
        
        MyBuffControl = new BuffControl(this);
        CDControl = new ActorCDControl();
        SkillControl = new ActorSkillCDControl();
//         ActionControl = new ActorActionControl(this);
//         mAnimControl = new AnimationControl(this);
    }
    public Actor(ActorType type, int id, int staticID, CSItemGuid guid)
	{
        IsRealDead = false;
		Type = type;
        TempType = Type;
		ID = id;
		ActionControl = new ActorActionControl(this);
		//StateControl = new ActorStateControl();
        MyBuffControl = new BuffControl(this);
        CDControl = new ActorCDControl();
        SkillControl = new ActorSkillCDControl();
        mAnimControl = new AnimationControl(this);

        GUID = guid;
	}
	#endregion
    public void SetSobTag(int sobTag)
    {
        SobTag = sobTag;
    }
    public void SetID(int id)
    {
        ID = id;
        if (MainObj != null)
        {
            MainObj.name = "Actor_" + Type.ToString() + "_" + ID.ToString();
        }
    }
	#region Resource
    public void ChangeLayer(Int32 theLayer,GameObject obj)
    {
        obj.layer = theLayer;
        for(int i=0;i<obj.transform.childCount;i++)
        {
            ChangeLayer(theLayer,obj.transform.GetChild(i).gameObject);
        }
    }
	//加载body
    public virtual IEnumerator Coroutine_LoadBody(string modelFile, int modelID, float modelScale, GameResPackage.AsyncLoadObjectData loadData)
    {
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetPrefabPath(modelFile), data, true);
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
            (data.m_obj as GameObject).transform.localScale = new Vector3(modelScale, modelScale, modelScale);
        }
        loadData.m_obj = data.m_obj;
        loadData.m_isFinish = true;
    }
    public enum ENSkillStepType
    {
        enNone,
        enPrepare,//准备
        enSpell,//吟唱
        enSlash,//冲锋
        enSlashBlend,//冲锋融合
        enRelease,//释放
        enReleaseBlend,//释放融合
        enConduct,//引导
        enEndConduct,//引导结束
    }
    public List<string> GetAnimationList(string modelType, string weaponType, List<int> skillIDList)
    {
        List<string> animList = new List<string>();
        foreach (var item in skillIDList)
        {
            SkillInfo info = GameTable.SkillTableAsset.Lookup(item);
            if (info == null)
            {
                continue;
            }
            string prefix = modelType + "-" + weaponType + "-" + info.ID.ToString() + "-";
            if (info.IsPrepareExist)
            {
                string name = info.PrepareMotion;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + ((int)ENSkillStepType.enPrepare).ToString() + "-" + name;
                    animList.Add(animName);
                }
                else
                {
                    Debug.LogWarning("animation is null, skill id:" + info.ID + ",准备");
                }
            }
            if (info.IsSpellExist)
            {
                string name = info.SpellMotion;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + ((int)ENSkillStepType.enSpell).ToString() + "-" + name;
                    animList.Add(animName);
                }
                else
                {
                    Debug.LogWarning("animation is null, skill id:" + info.ID + ",吟唱");
                }
            }
            if (info.IsSlashExist)
            {
                string name = info.SlashMotion;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + ((int)ENSkillStepType.enSlash).ToString() + "-" + name;
                    animList.Add(animName);
                }
                else
                {
                    Debug.LogWarning("animation is null, skill id:" + info.ID + ",冲锋");
                }
                name = info.SlashBlendMotionName;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + ((int)ENSkillStepType.enSlashBlend).ToString() + "-" + name;
                    animList.Add(animName);
                }
                else
                {
                    Debug.LogWarning("animation is null, skill id:" + info.ID + ",冲锋融合");
                }
            }
            if (info.IsReleaseExist)
            {
                string name = info.ReleaseMotion;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + ((int)ENSkillStepType.enRelease).ToString() + "-" + name;
                    animList.Add(animName);
                }
                else
                {
                    Debug.LogWarning("animation is null, skill id:" + info.ID + ",释放");
                }
                name = info.ReleaseBlendMotionName;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + ((int)ENSkillStepType.enReleaseBlend).ToString() + "-" + name;
                    animList.Add(animName);
                }
            }
            if (info.IsConductExist)
            {
                string name = info.ConductMotion;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + ((int)ENSkillStepType.enConduct).ToString() + "-" + name;
                    animList.Add(animName);
                }
                else
                {
                    Debug.LogWarning("animation is null, skill id:" + info.ID + ",引导");
                }
            }
            if (info.IsEndConductExist)
            {
                string name = info.EndConductMotion;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + ((int)ENSkillStepType.enEndConduct).ToString() + "-" + name;
                    animList.Add(animName);
                }
                else
                {
                    Debug.LogWarning("animation is null, skill id:" + info.ID + ",引导结束");
                }
            }
        }
        return animList;
    }
    public void Load(string prefabName, int modelID, float modelScale, int weaponId = 0)
    {
        MainObj = PoolManager.Singleton.Load(GameData.GetPrefabPath(prefabName));
        MainObj.SetActive(true);
        MainObj.name = "Actor_" + Type.ToString() + "_" + ID.ToString();
        MainTransform = MainObj.transform;
        MainObj.transform.parent = MainGame.Singleton.MainObject.transform;
        NotifyChangeModel();
    }

    public GameObject[] m_ghostObjs = new GameObject[2];
    int m_nextGhostIndex = 0;
    public void TryLoadGhost()
    {
        if (!SystemInfo.deviceModel.Contains("iPhone6"))
        {
            //return;
        }
        //加载残影
        mainPlayerCfg cfg = GetBodyObject().GetComponent<mainPlayerCfg>();
        if (cfg != null && cfg.m_ghostResource != null)
        {
			
			for (int i = 0; i < m_ghostObjs.Length; i++)
			{
				m_ghostObjs[i] = GameObject.Instantiate(cfg.m_ghostResource) as GameObject;
			}
            for (int i = 0; i < m_ghostObjs.Length; i++)
            {
                GhostFollow f = m_ghostObjs[i].GetComponent<GhostFollow>();
				GhostFollow fnext = null;
				if(i+1<m_ghostObjs.Length)
				{
					fnext=m_ghostObjs[i+1].GetComponent<GhostFollow>();
				}
                f.Init(cfg.m_rootTransform, MainObj.transform, i,fnext);
                m_ghostObjs[i].transform.localPosition = Vector3.zero;
                m_ghostObjs[i].transform.localScale = Vector3.one;
            }
        }
    }

    //显示残影
    public void ShowGhost(float loopSecond)
    {
		int nextIndex = 0;
			//m_nextGhostIndex % m_ghostObjs.Length;
        if(m_ghostObjs[nextIndex] == null)
        {
            return;
        }
        GhostFollow f = m_ghostObjs[nextIndex].GetComponent<GhostFollow>();
        //if(f.IsDispeard
        f.CaptureGhost(loopSecond);
        m_nextGhostIndex++;
    }

	//加载资源后的处理
    protected virtual void OnLoaded(float modelScale, int modelID)
    {
        {
             ActorProp prop = MainObj.GetComponent<ActorProp>();
             if (prop == null)
             {
                 prop = MainObj.AddComponent<ActorProp>();
                 prop.Type = Type;
                 prop.ID = ID;
                 prop.ActorLogicObj = this;
             }
           
            //MainNavMeshAgent = MainObj.AddComponent<NavMeshAgent>();

        }
        PropConfig = MainObj.GetComponent<ActorPropConfig>();
        MainRigidBody = MainObj.GetComponent<Rigidbody>();
        //MainCharController = MainObj.GetComponent<CharacterController>();
        RefreshBodyElement(modelScale, modelID);
        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enUrgentEventParam);
        mRedFlashTriggerPercent = param.IntTypeValue;
        ChangeLayer(LayerMask.NameToLayer("Actor"), MainObj);
        GetBodyObject().layer = LayerMask.NameToLayer("ActorDamage");
        if (CenterPart != null)
        {
            CfgFollowPosition f = CenterPart.GetComponent<CfgFollowPosition>();
            if (f != null)
            {
                m_colliderFollowObj = f.m_follow;
            }
        }
        MainRigidBody.mass = 1000 * BaseMass;
        {//记录Renderer
            AnimationShaderParamCallback callback = GetBodyParentObject().GetComponent<AnimationShaderParamCallback>();
            if (callback != null)
            {
                callback.BackupAllSharedMaterial();
            }
        }
        if (CfgShader.Instance != null)
        {
            CfgShader.Instance.ReplaceActorShader(GetBodyObject(), this);
        }
        //删除body的脚本
        if (GetBodyObject().GetComponent<AnimationCameraCallback>())
        {
            GameObject.Destroy(GetBodyObject().GetComponent<AnimationCameraCallback>());
        }
        if (GetBodyObject().GetComponent<AnimationEffectCallback>())
        {
            GameObject.Destroy(GetBodyObject().GetComponent<AnimationEffectCallback>());
        }
        if (GetBodyObject().GetComponent<AnimationEndCallback>())
        {
            GameObject.Destroy(GetBodyObject().GetComponent<AnimationEndCallback>());
        }
        //if (GetBodyObject().GetComponent<AnimationShaderParamCallback>())
        //{
        //    GameObject.Destroy(GetBodyObject().GetComponent<AnimationShaderParamCallback>());
        //}
        if (GetBodyObject().GetComponent<AnimationSoundCallback>())
        {
            GameObject.Destroy(GetBodyObject().GetComponent<AnimationSoundCallback>());
        }
        if (GetBodyObject().GetComponent<BreakPointCallback>())
        {
            GameObject.Destroy(GetBodyObject().GetComponent<BreakPointCallback>());
        }
        if (GetBodyObject().GetComponent<AttackActionCallback>())
        {
            GameObject.Destroy(GetBodyObject().GetComponent<AttackActionCallback>());
        }
        m_isLoaded = true;

		AddHPBar();
    }
    //返回值为当前血值
    public float ReduceHp(float hp)
    {
        float cHp = HP;
        cHp -= hp;
        SetCurHP(cHp);
        return HP;
    }
    //返回值为实际加血
    public float AddHp(float hp)
    {
        if (hp < 0)
        {
            return 0;
        }
        float cHp = HP;
        float value = MaxHP - HP;
        if (hp > value)
        {
            hp = value;
        }
        cHp += hp;
        SetCurHP(cHp);
        return hp;
    }
    
    public virtual void UpdateHPBar()
    {
		if (IsDead)
		{
			if (mHPBarObj != null && mHPBarObj.activeSelf)
			{
				SetHPBar();
				mHPBarObj.SetActive(false);
			}
			return;
		}
		if (mHPBarObj != null)
		{
			if (!mHPBarObj.activeSelf)
			{
				mHPBarObj.SetActive(true);
				m_uiHPBar = null;
			}
			SetHPBar();
		}		
    }
	public void SetHPBar()
	{
		if (m_uiHPBar == null)
		{
			m_uiHPBar = mHPBarObj.GetComponentInChildren<UIHPBar>();
			m_uiHPBar.PlayShowAnim();
			//m_uiHPBar.m_ownerID = this.ID;
		}
		if (IsDead != m_uiHPBar.m_actorIsDead)
		{
			m_uiHPBar.m_actorIsDead = IsDead;
		}
		//if (m_isHPBarNeedShow)
		//{
		//    mHPBarObj.SetActive(true);
		//    m_uiHPBar.PlayShowAnim();
		//    //m_isHPBarNeedShow = false;
		//}
		m_hpBarShowStartTime = Time.time;
		float process = HP / MaxHP;
		m_uiHPBar.HP = process;
		

		Transform nameLabel = mHPBarObj.transform.Find("CharacterName");
		if (nameLabel != null)
		{
			UILabel label = nameLabel.gameObject.GetComponent<UILabel>();
			string name = Name;
			name += "<" + Camp.ToString() + ">";
			int hp = (int)HP;
			int maxHP = (int)MaxHP;
			name += "(" + hp.ToString() + "/" + maxHP.ToString() + ")";
			label.text = name;
		}
	}
	public void RefreshAllName()
	{
		foreach(var item in ActorManager.Singleton.m_actorMap)
		{
			item.Value.InitName();
		}
	}
    public void InitName()
    {
		if (mHPBarObj == null)
		{
			Debug.LogError("InitName The mHPBarObj is null:"+ ID);
			return;
		}
		Transform nameLabel = mHPBarObj.transform.Find("CharacterName");
		if (nameLabel != null && ActorManager.Singleton.MainActor != null)
		{
			UILabel label = nameLabel.gameObject.GetComponent<UILabel>();
            label.color = GetColorValueByCamp(ActorManager.Singleton.MainActor.Camp);
            string name = Name;
			name += "<" + Camp.ToString() + ">";
			int hp = (int)HP;
			int maxHP = (int)MaxHP;
			name += "(" + hp.ToString() + "/" + maxHP.ToString() + ")";
			label.text = name;
		}

		Transform hpTrans = mHPBarObj.transform.Find("BarHP").Find("hp");
		if (hpTrans != null && ActorManager.Singleton.MainActor != null)
		{
            string hpSprint = GetHPSprintByCamp(ActorManager.Singleton.MainActor.Camp);
			UISprite sprite = hpTrans.gameObject.GetComponent<UISprite>();
			sprite.spriteName = hpSprint;
		}

        float modelScal = ModelScale;
        GameObject body = GetBodyObject();
        if (body != null)
        {
            body.transform.localScale = new Vector3(modelScal, modelScal, modelScal);
        }
    }
	public string GetHPSprintByCamp(int camp)
	{
		if (camp != Camp)
		{
			return "blue";
		}
		return "red_surface";
	}

	public Color GetColorValueByCamp(int camp)
	{
		if (Camp != camp)
		{
            return Color.red;
		}
        return Color.yellow;
	}
	//销毁资源
	public virtual void Destroy()
    {
		GameObject.Destroy(MainObj);
        MainObj = null;
        MainTransform = null;
        if (null != ShadowObj)
        {
            PoolManager.Singleton.ReleaseObj(ShadowObj);
            ShadowObj = null;
            //GameObject.Destroy(ShadowObj);
        }

        string name = "Bar_" + Type.ToString() + "_" + ID.ToString();
        GameObject grp = GameObject.Find(name);
        if (null != grp)
        {
            GameObject.Destroy(grp);
        }
        //删除所有的buff
		MyBuffControl.RemoveAll(BattleFactory.Singleton.GetBattleContext().CreateResultControl());
        //删除特效
        //PoolManager.Singleton.ReleaseAll();
        //删除ghost
        for (int i=0;i<m_ghostObjs.Length;i++)
        {
            if (m_ghostObjs[i] != null)
            {
                GameObject.Destroy(m_ghostObjs[i]);
            }
            m_ghostObjs[i] = null;
        }
		ActionControl.ReleaseAll ();
        MyBuffControl.ReleaseAll();
        //Resources.UnloadUnusedAssets();
	}
	#endregion
	//行为控制
	public ActorActionControl ActionControl { get; private set; }
	//状态控制
	//public ActorStateControl StateControl { get; private set; }
    //目标管理
    public ActorTargetManager TargetManager { get; set; }
    public Actor CurrentTarget { get { return TargetManager.CurrentTarget; } set { TargetManager.CurrentTarget = value; } }
    public bool CurrentTargetIsDead { get { return CurrentTarget == null ? true : CurrentTarget.IsDead; } }
    //Buff管理
    public BuffControl MyBuffControl { get; private set; }

    public ActorCDControl CDControl { get; private set; }
    public ActorSkillCDControl SkillControl { get; private set; }
    public AIBase SelfAI;
    //动画控制器
    public AnimationControl mAnimControl { get; private set; }
    AnimationConfig m_aniCfg;
	#region 角色行为
	//默认行为
	public virtual void DefaultAction()
	{

    }

	//开始搜索目标
	public virtual void StartSearch()
	{

	}
    //受击
    public virtual void BeHited(Actor srcActor, bool isBack, bool isFly, float animGauge)
    {
        BeAttackAction action = ActionControl.AddAction(ActorAction.ENType.enBeAttackAction) as BeAttackAction;
        if (null != action)
        {
            action.Init(srcActor, isBack, isFly);
        }
	}
    public virtual void FakeBeHited(Actor srcActor)
    {
        FakeBeAttackAction action = ActionControl.AddAction(ActorAction.ENType.enFakeBeAttackAction) as FakeBeAttackAction;
        if (null != action)
        {
            action.Init(srcActor);
        }
    }
    //释放技能
    public virtual bool OnFireSkill(int skillID)
    {
        return true;
    }
	#endregion
    

    // 角色当前所经历的ZONEID 未突破 
    public int m_curZoneId 
    {
        set
        {
            PlayerPrefs.SetInt("XprojectZoneID", value);
        }
        get
        {
            if (  PlayerPrefs.HasKey("XprojectZoneID") )
            {
                return PlayerPrefs.GetInt("XprojectZoneID");
            }
            return 0;
        }
    }

    // 角色当前所经历的关卡StageID 未突破-1 代表 从第一关开始 0 代表已完全突破当前经历Zone 进入下一Zone
    public int m_curStageId
    {
        set
        {
            PlayerPrefs.SetInt("XprojectStageID", value);
        }
        get
        {
            if (PlayerPrefs.HasKey("XprojectStageID"))
            {
                return PlayerPrefs.GetInt("XprojectStageID");
            }
            return -1;
        }
    }

    // 角色当前所所经历的关卡FloorId 未突破-1 代表 从第一关开始 0 代表已完全突破当前Stage 进入下一Stage
    public int m_curFloorId
    {
        set
        {
            PlayerPrefs.SetInt("XprojectFloorID", value);
        }
        get
        {
            if (PlayerPrefs.HasKey("XprojectFloorID"))
            {
                return PlayerPrefs.GetInt("XprojectFloorID");
            }
            return -1;
        }
    }
    
    public RoomElement roomElement { get; set; }
	//创建模型
    public virtual bool CreateNeedModels()
    {
		return false;
    }
    #region Update
    public virtual void Update()
    {
		if ((mHPBarObj != null) && (mHPBarObj.activeSelf))
		{
			MainGame.FaceToCamera(mHPBarObj);
		}
	}
	public virtual void LateUpdate()
	{
        if (MainAnim != null)
        {
            if (m_aniCfg == null)
            {
                m_aniCfg = MainAnim.GetComponent<AnimationConfig>();
            }
            if (null != m_aniCfg && null != m_aniCfg.Trail)
            {
                m_aniCfg.Trail.Itterate(Time.time - Time.deltaTime);
                m_aniCfg.Trail.UpdateTrail(Time.time, Time.deltaTime);
            }
        }
        if (null != CenterPart)
        {
            CenterPartWorldPos = CenterPart.transform.position;
            CenterPartWorldPos = new Vector3(CenterPartWorldPos.x, 0.0f, CenterPartWorldPos.z);
        }
        if (Type == ActorType.enMain)
        {
            ForwardDirectionArrow.Singleton.LateUpdate();
        }
        if (null != mActorBlendAnim)
        {
            mActorBlendAnim.LateUpdate();
        }
        if (m_colliderFollowObj != null && CenterPart != null)
        {
            CenterPart.transform.localPosition = m_colliderFollowObj.localPosition;
        }
        if (CenterPart != null)
        {
            RealPos = CenterPart.transform.position;
        }
        RealPos = new Vector3(RealPos.x, MainPos.y, RealPos.z);
        RealPosIsValidate = true;
    }
    //是否死亡倒计时
    bool IsDeadCD { get; set; }
    //存活时间
    float LiveDuration { get; set; }
    //开始死亡倒计时
    public void StartDeadCD(float duration)
    {
        IsDeadCD = true;
        LiveDuration = duration;
    }
    protected bool m_isLoaded = false;
	public virtual void FixedUpdate()
    {
        if (!m_isLoaded)
        {
            return;
        }
        if (IsDeadCD)
        {//死亡倒计时
            if (LiveDuration < 0)
            {
                if (!ActionControl.IsActionRunning(AttackAction.SGetActionType()))
                {
                    HideMe();
                    IResult r = BattleFactory.Singleton.CreateResult(ENResult.Dead, ID, ID);
                    if (r != null)
                    {
                        BattleFactory.Singleton.DispatchResult(r);
                    }
                    ActorManager.Singleton.ReleaseActor(ID, true);
                    return;
                }
            }
            LiveDuration -= Time.deltaTime;
        }
        if (Time.time - mStartTime >= 0.2f)
        {
            Vector3 d = MainPos - mLastMovePos;
            d.y = 0;
            mRealSpeed = d.magnitude / (Time.time - mStartTime);
            mStartTime = Time.time;
            mLastMovePos = MainPos;
        }
        //StateControl.FixedUpdate(this);
        MyBuffControl.Tick(Time.deltaTime, BattleFactory.Singleton.GetBattleContext().CreateResultControl());
        CDControl.Tick(this);
        SelfAI.Update();
        ActionControl.FixedUpdate();
        if (null != mActorBlendAnim)
        {
            mActorBlendAnim.FixedUpdate();
        }
        mAnimControl.Tick();

        //由于刚体在速度低于某个值得时候，会自动进入sleep的状态，所以这里需要手动唤醒一下
        //不准删 tgame 2014.5.3
        if (MainRigidBody != null)
        {
            MainRigidBody.WakeUp();
        }

// 		if (ActorManager.Singleton.MainActor.TargetManager.CurrentTarget != this)
// 		{//自己不是目标
// 			if (mHPBarObj != null)
// 			{
// 				if (!m_isHPBarNeedShow)
// 				{
// 					if (Time.time - m_hpBarShowStartTime > GameSettings.Singleton.m_npcHPBloodDuration)
// 					{
// 						if (HP > 0)
// 						{//等于0时，UIHPBar播放
// 							m_uiHPBar.PlayHideAnim();
// 						}
// 						m_isHPBarNeedShow = true;
// 					}
// 				}
// 			}
// 		}
// 		else
// 		{
// 			if (mHPBarObj != null)
// 			{
// 				if (m_isHPBarNeedShow || !mHPBarObj.activeSelf)
// 				{
// 					mHPBarObj.SetActive(true);
// 					if (m_uiHPBar == null)
// 					{
// 						m_uiHPBar = mHPBarObj.GetComponentInChildren<UIHPBar>();
// 						//m_uiHPBar.m_ownerID = this.ID;
// 						m_uiHPBar.HP = HP / MaxHP;
// 					}
// 					m_uiHPBar.PlayShowAnim();
// 					m_hpBarShowStartTime = Time.time;
// 					//m_isHPBarNeedShow = false;
// 				}
// 			}
// 		}
//		UpdateHPBar();
    }
	#endregion

    public void ForceMoveToPosition(Vector3 pos, Quaternion quater)
    {
        MainObj.transform.rotation = quater;
        ForceMoveToPosition(pos);
    }
    public void ForceMoveToPosition(Vector3 pos)
    {
        MainPos = pos;
        RealPos = pos;
        mLastMovePos = pos;
        RealPosIsValidate = false;
        if (null != mActorBlendAnim)
        {
            mActorBlendAnim.Reset();
        }
    }

    public void MoveRotation(Quaternion rot)
    {
        if (MainRigidBody != null)
        {
            MainRigidBody.MoveRotation(rot);
        }
        //else if(MainCharController != null)
        //{
        //    MainCharController.transform.rotation = rot;
        //}
        else
        {
            MainObj.transform.localRotation = rot;
        }
    }
    public void EnableCollider(bool enable)
    {
        //if (MainCharController != null)
        //{
        //    MainCharController.enabled = enable;
        //}
        //if (MainRigidBody != null)
        //{
        //    if (enable)
        //    {
        //        MainRigidBody.WakeUp();
        //    }
        //    else
        //    {
        //        MainRigidBody.Sleep();
        //    }
        //}
        MainGame.Singleton.StartCoroutine(Coroutine_EnableCollider(enable));
        //CenterPart.GetComponent<Collider>().enabled = enable;
    }
    IEnumerator Coroutine_EnableCollider(bool enable)
    {
        while (true)
        {
            if (CenterPart != null && CenterPart.transform != null)
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        CenterPart.GetComponent<Collider>().enabled = enable;
    }
    
    public float GetCurrentSpeedFromRigid()
    {
        //if (MainCharController != null)
        //{
        //    return MainCharController.velocity.magnitude;
        //}
        //else 
        if (MainRigidBody != null)
        {
            return MainRigidBody.velocity.magnitude;
        }
        return 0.0f;
    }
    #region Animation

    public float BlendAnimation(string animName, float targetWeight, float fFadeTime = 0.3f, AnimationBlendMode blendMode = AnimationBlendMode.Blend)
	{
        string animFullName = m_animPrefix + animName;
        AnimInfo tempAnimData = GameTable.AnimationTableAsset.Lookup(animName);
        if (null != tempAnimData)
        {
            animName = tempAnimData.AnimName[UnityEngine.Random.Range(0, tempAnimData.AnimName.Length)];
            animFullName = m_animPrefix + animName;
        }
        AnimationClip clip = MainAnim.GetClip(animFullName);
        if (null == clip)
		{
            return 0.0f;
		}
        if (MainAnim.IsPlaying(animFullName))
        {
            MainAnim.Stop(animFullName);
        }
        MainAnim[animFullName].blendMode = blendMode;
        MainAnim.Blend(animFullName, targetWeight, fFadeTime);
        AnimationSpeedChanged(animFullName);
        return clip.length;
	}
    bool m_isNextAnimationDisableFade = false;
    //将位移动画的位移回馈到角色，并且播放下一个动画，下一个动画不能Fade [8/11/2015 tgame]
    public void ApplyAnimationOffset()
    {
        if (RealPosIsValidate && (RealPos - MainPos).magnitude > 0.5f)
        {
            MainPos = new Vector3(RealPos.x, MainPos.y, RealPos.z);
            if (CenterPart != null)
            {
                CenterPart.transform.position = RealPos;
            }
            m_isNextAnimationDisableFade = true;
        }
    }
	public float PlayAnimation(string animName)
	{
        string str;
		return PlayAnimation(animName, Vector3.zero, out str, Vector3.zero);
	}
	public float PlayAnimation(string animName, Vector3 direction, out string animFullName, Vector3 syncPos)
	{
        animFullName = "";
        if (MainAnim == null)
        {
            return 0;
        }
		if (Vector3.zero != direction)
		{
			direction.y = 0.0f;
            direction.Normalize();
            MoveRotation(Quaternion.LookRotation(direction.normalized) * Quaternion.LookRotation(Vector3.forward));
		}
        animFullName = animName;
        AnimInfo tempAnimData = GameTable.AnimationTableAsset.Lookup(animFullName);
		AnimationClip clip = null;
        if (null != tempAnimData)
        {
            animName = tempAnimData.AnimName[UnityEngine.Random.Range(0, tempAnimData.AnimName.Length)];
            animFullName = m_animPrefix + animName;
            clip = MainAnim.GetClip(animFullName);
            if (null == clip)
            {
                //Debug.LogWarning("Miss animation error: " + animName);
                return 0.0f;
            }
        }
        else
        {
            animFullName = m_animPrefix + animName;
        }
		if (null == clip)
		{
            clip = MainAnim.GetClip(animFullName);
			if (null == clip)
			{
				//Debug.LogWarning("Miss animation2 error: " + animName);
				return 0.0f;
			}
		}
        ChangeAdherentPoints(animName);
		switch (clip.wrapMode)
		{
			case WrapMode.Loop:
                if (MainAnim.IsPlaying(animFullName))
                {
                    //Debug.LogWarning("The animation isPlaying: " + animName);
                    MainAnim.CrossFade(animFullName);
                    //animaiton的播放速度
                    //MainAnim[animFullName].speed = AnimationSpeed;
					return 0.0f;
				}
				break;
            default:
                {
                    MainAnim.Stop();
                    MainGame.Singleton.StartCoroutine(CoroutineAnimationEnd(++m_currentCorroutineIndex, clip.length));
                }
                break;
		}
        //Debug.Log(MainObj.name + " Play Animation:" + ",name=" + animName);
        if (m_isNextAnimationDisableFade)
		{
            MainAnim.Play(animFullName);
            m_isNextAnimationDisableFade = false;
		}
		else
        {
            NPC npc = this as NPC;
            if (this.Type == ActorType.enNPC && npc.GetNpcType() == ENNpcType.enBoxNPC)
            {
                MainAnim.Play(animFullName);
            }
            else
            {
                MainAnim.CrossFade(animFullName);
            }
        }
        AnimationSpeedChanged(animFullName);
        return clip.length;
	}
    public enum ENAnimType
    {
        enPlay,
        enCrossFade,
        enBlend,
    }
    public void PlaySkillEventAnimation(int skillID, int step, string animName, ENAnimType type, float[] param = null)
    {
        ModelInfo mInfo = GameTable.ModelInfoTableAsset.Lookup(CurModelID);
        if (mInfo == null)
        {
            Debug.LogWarning("model id is null,id:"+CurModelID);
            return;
        }
        WeaponInfo wInfo = GameTable.WeaponInfoTableAsset.Lookup(WeaponType);
        if (wInfo == null)
        {
            Debug.LogWarning("weapon id is null,id:" + WeaponType);
            return;
        }
        Animation anim = GetBodyParentObject().GetComponent<Animation>();
        if (anim == null)
        {
            Debug.LogWarning("body parent animation is null");
            return;
        }
        string name = mInfo.modelType.ToString() + "-" + ((int)wInfo.WeaponType).ToString() + "-" +
            skillID.ToString() + "-" + step.ToString() + "-" + animName;
        AnimationClip clip = anim.GetClip(name);
        if (clip == null)
        {
            Debug.LogError("skill animation is null,name:" + name + ",actor type:" + Type + ",tabelID:" + IDInTable);
            DebugLog.Singleton.OnShowLog("skill animation is null,name:" + name + ",actor type:" + Type + ",tabelID:" + IDInTable);
            return;
        }
        switch (type)
        {
            case ENAnimType.enPlay:
                {
                    anim.Stop();
                    anim.Play(name);
                }
                break;
            case ENAnimType.enCrossFade:
                {
                    anim.Stop();
                    anim.CrossFade(name);
                }
                break;
            case ENAnimType.enBlend:
                {
                    if (anim.IsPlaying(name))
                    {
                        anim.Stop(name);
                    }
                    anim[name].blendMode = (AnimationBlendMode)param[0];
                    anim.Blend(name, param[1], param[2]);
                }
                break;
        }
    }
    public void StopPlaySkillEventAnimation()
    {
        Animation anim = GetBodyParentObject().GetComponent<Animation>();
        if (anim != null)
        {
            anim.Stop();
        }
        else
        {
            Debug.LogWarning("body parent animation is null");
        }
    }
	private int m_currentCorroutineIndex = 0;

	private IEnumerator CoroutineAnimationEnd(int animCoroutineIndex, float timeLength)
	{
		yield return new WaitForSeconds(timeLength);
		if (null != MainAnim && animCoroutineIndex == m_currentCorroutineIndex)
		{
			MainAnim.SendMessage("OnAnimationEnd", SendMessageOptions.DontRequireReceiver);
		}
	}
	//改变AdherentPoints
    public void ChangeAdherentPoints(string animName)
    {
        if (AdherentPoints == null)
        {
            return;
        }
        ModelInfo info = GameTable.ModelInfoTableAsset.Lookup(CurModelID);
        if (info != null)
        {
            if (info.AnimationList.Contains(animName))
            {
                AdherentPoints.FollowedT = GetObject_Bip01().transform;
            }
            else
            {
                AdherentPoints.FollowedT = null;
            }
        }
    }
	#endregion
    #region Effect
    public void DestroyEffect(GameObject obj)
    {
        if (false == GameSettings.Singleton.m_playEffect)
        {
            return ;
        }

        if (null != obj)
        {
            PoolManager.Singleton.ReleaseObj(obj);
        }
    }
    public void PlayEffect(string effectName)
    {
        PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetEffectPath(effectName), LoadEffectCallback);
        //GameObject effect = PoolManager.Singleton.LoadEffectWithPool(effectName);
        //PlayEffect(effect, 0.0f, false);
    }
    void LoadEffectCallback(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            PlayEffect(obj as GameObject, 0, false);
        }
    }
    //查找actor的骨骼
    public Transform LookupBone(Transform parent, string name)
    {
        Transform c = LookupBone2(parent, name);
        if (c == null)
        {
            name = name.Replace(" ", "_");
            c = LookupBone2(parent, name);
        }
        return c;
    }
	Transform LookupBone2(Transform parent,string name)
	{
		Transform c = parent.Find (name);
		if (c != null) 
		{
			return c;
		}
		for (int i=0; i<parent.childCount; i++) 
        {
            Transform cc = parent.GetChild(i);
            c = LookupBone2(cc, name);
            if(c!= null)
            {
                return c;
            }
        }
        return c;
	}
    //重构一个静态函数的LookupBone2
    public static Transform S_LookupBone2(Transform parent, string name)
    {
        Transform c = parent.Find(name);
        if (c != null)
        {
            return c;
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform cc = parent.GetChild(i);
            c = S_LookupBone2(cc, name);
            if (c != null)
            {
                return c;
            }
        }
        return c;
    }
    public void PlayEffectInPosition(string effectName, Vector3 pos)
    {
        MainGame.Singleton.StartCoroutine(Coroutine_PlayEffect(effectName, pos));
        //GameObject effect = PoolManager.Singleton.LoadEffectWithPool(effectName);
        //PlayEffectInPosition(effect, pos);
    }
    public IEnumerator Coroutine_PlayEffect(string effectName, Vector3 pos)
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
            PlayEffectInPosition(data.m_obj as GameObject, pos);
        }
    }
    public void PlayEffectInPosition(GameObject obj, Vector3 pos)
    {
        GameObject sceneEffect = PoolManager.Singleton.CreateObj(obj, false);
        sceneEffect.transform.localPosition = pos;
        sceneEffect.SetActive(true);
    }
    private GameObject PlayEffect(GameObject obj, float effectTime, bool isParentBone, string posByBone="")
    {
        if (null == obj || MainObj == null)
        {
			return null;
		}

        if ( false == GameSettings.Singleton.m_playEffect )
        {
            return null;
        }
        GameObject sceneEffect = PoolManager.Singleton.CreateObj(obj,false);
		sceneEffect.transform.localPosition = new Vector3(RealPos.x, MainPos.y, RealPos.z);
		Vector3 currentForward = sceneEffect.transform.eulerAngles;
		currentForward.y = MainObj.transform.eulerAngles.y;
		sceneEffect.transform.eulerAngles = currentForward;
        Transform boneT;
        if (!string.IsNullOrEmpty(posByBone))
        {
            boneT = LookupBone(MainObj.transform,posByBone);
			if(boneT)
			{
                if (isParentBone)
                {
                    sceneEffect.transform.parent = boneT;
                }
                Vector3 pos = boneT.position;
                sceneEffect.transform.position = pos;
	            //Vector3 v = MainGame.Singleton.MainCamera.MainCamera.transform.position - pos;
	            //v.x = v.z = 0.0f;
	            //sceneEffect.transform.LookAt(pos - v);
	            //sceneEffect.transform.position = pos;
            }
            else
            {
                Debug.LogWarning("PlayEffect miss bone=" + posByBone + ",actor=" + ID.ToString());
                sceneEffect.transform.position = MainPos;
            }
        }
        ParticleSystem partSys = sceneEffect.GetComponent<ParticleSystem>();
        if (null != partSys && !partSys.loop)
        {
            ParticleConfig config = sceneEffect.GetComponent<ParticleConfig>();
            if (null != config && config.RotateParticle)
            {
				partSys.startRotation = MainObj.transform.eulerAngles.y * Mathf.PI / 180.0f;
            }
            MainGame.Singleton.StartCoroutine(RemoveEffect(++m_effectIndex, sceneEffect, partSys.duration));
        }
        else
        {
            MainGame.Singleton.StartCoroutine(RemoveEffect(++m_effectIndex, sceneEffect, effectTime));
        }
        sceneEffect.SetActive(true);
        return sceneEffect;
    }
    public void OnPlayEffect(GameObject obj, AnimationEvent animEvent)
    {
        GameObject effectObj = PlayEffect(animEvent.objectReferenceParameter as GameObject,
            animEvent.floatParameter, animEvent.intParameter != 0, animEvent.stringParameter);
        if (effectObj != null && this.ActionControl.IsActionRunning(ActorAction.ENType.enAttackAction))
        {
            AttackAction action = this.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
            action.AddEffectObj(effectObj);
        }
    }
    //连接技特效
    public void OnPlayConnectEffect(GameObject obj, AnimationEvent animEvent)
    {
        if (string.IsNullOrEmpty(animEvent.stringParameter) || this.Combo == null)
        {
            return;
        }
        string[] temp = animEvent.stringParameter.Split(new char[1] { ';' });
        string bone = temp[0];
        string effectList = temp[1];
        string[] param = effectList.Split(new char[1] { ',' });
        //普通特效
        string effect0 = param[0];
        //连接技特效
        string effect1 = param[1];
        string effectName = effect0;
        if (this.Combo.IsPlayConnectEffect())
        {//播放连接技特效
            effectName = effect1;
        }
        PlayEffect(effectName, animEvent.floatParameter, bone, animEvent.intParameter != 0, Vector3.zero, true);
        //GameObject effectObj = PlayEffect(effectName, animEvent.floatParameter, bone, animEvent.intParameter != 0, Vector3.zero);
        //if (effectObj != null && this.ActionControl.IsActionRunning(ActorAction.ENType.enAttackAction))
        //{
        //    AttackAction action = this.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
        //    action.AddEffectObj(effectObj);
        //}
    }
    public virtual void PlayEffect(string effectName, float effectTime, string posByBone, bool isAdhered, Vector3 offset, bool isAddToAttackAction = false)
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
                    AttackAction action = this.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
                    action.AddEffectObj(data.m_obj as GameObject);
                }
            }
        }
        else
        {
            AttackAction action = ActionControl.LookupAction(AttackAction.SGetActionType()) as AttackAction;
            if (action != null)
            {
                Debug.LogWarning("Play effect fail, actorID:" + ID + ", skillID:" + action.m_skillID + ",effectName:" + effectName);
            }
            else
            {
                Debug.LogWarning("Play effect fail, name:" + effectName);
            }
        }
    }
    public void PlayEffect(GameObject obj, float effectTime, string posByBone, bool isAdhered, Vector3 offset)
    {
        if (false == GameSettings.Singleton.m_playEffect)
        {
            return;
        }
        if (obj != null)
        {
            obj.transform.localPosition = new Vector3(RealPos.x, MainPos.y, RealPos.z);
            Vector3 currentForward = obj.transform.eulerAngles;
            currentForward.y = MainObj.transform.eulerAngles.y;
            obj.transform.eulerAngles = currentForward;
            Transform boneT;
            if (!string.IsNullOrEmpty(posByBone))
            {
                boneT = LookupBone(MainObj.transform, posByBone);
                if (boneT)
                {
                    Vector3 pos = boneT.position;
                    if (isAdhered)
                    {
                        obj.transform.parent = boneT.transform;
                        pos.x += offset.x;
                        pos.y += offset.y;
                        pos.z += offset.z;
                    }
                    else
                    {
                        pos = pos + boneT.rotation * offset;
                    }
                    obj.transform.position = pos;
                }
                else
                {
                    Debug.Log("PlayEffect miss bone=" + posByBone + ",actor=" + ID.ToString());
                    obj.transform.position = MainPos;
                }
            }
            else
            {

            }
            if (effectTime != 0)
            {
                MainGame.Singleton.StartCoroutine(RemoveEffect(++m_effectIndex, obj, effectTime));
            }
            obj.SetActive(true);
        }
    }
    //终结技特效
    public void OnPlayComboEffect(GameObject obj, AnimationEvent animEvent)
    {
        if (this.Combo == null || Combo.TotalComboNumber == 0)
        {
            return;
        }
        if (string.IsNullOrEmpty(animEvent.stringParameter))
        {
            return;
        }
        string[] temp = animEvent.stringParameter.Split(new char[1] { ':' });
        if (temp.Length < 2)
        {
            return;
        }
        string bone = temp[0];
        string[] array = temp[1].Split(new char[1] { ';' });
        int length = array.Length;
        if (length == 0)
        {
            return;
        }
        int index = 0, number = 0;
        string name = "", effectName = "";
        while (GetComboEffect(array, index, out number, out name))
        {
            if (Combo.TotalComboNumber <= number)
            {
                break;
            }
            effectName = name;
            index++;
        }
        PlayEffect(effectName, animEvent.floatParameter, bone, animEvent.intParameter != 0, Vector3.zero);
    }
    bool GetComboEffect(string[] array, int index, out int number, out string name)
    {
        number = 0;
        name = "";
        if (array.Length <= index)
        {
            return false;
        }
        string[] effectList = array[index].Split(new char[1] { ',' });
        if (effectList.Length != 2)
        {
            return false;
        }
        number = int.Parse(effectList[0]);
        name = effectList[1];
        return true;
    }

	private int m_effectIndex = 0;
	private IEnumerator RemoveEffect(int effectIndex, GameObject obj, float duration)
	{
		yield return new WaitForSeconds(duration);
		//if (m_effectIndex == effectIndex)
		{
			if (null != obj)
			{
				PoolManager.Singleton.ReleaseObj(obj);
			}
		}
	}
	#endregion

	#region 这些是与换装操作相关的
	//------------------------------------------------------------------------------------------------
    //
    // 这些是与换装操作相关的
    //
    //----------------------------------------------------------------------------
    // 根据需要来创建保存列表
    public void NewChangeDic()
    {
       // mChanges = new Dictionary<string, MeshMtrGrp>();
    }
    
    //--------------------------------------------------------------------------
    protected GameObject m_objBodyParent = null;
    public GameObject GetBodyParentObject()
    {
        //return GetBodyObject();
        if (m_objBodyParent == null)
        {
            Transform chBody = MainObj.transform.Find("parent");
            if (chBody != null)
            {
                m_objBodyParent = chBody.gameObject;
            }
        }
        return m_objBodyParent;
    }
    protected GameObject m_objBody = null;
    public GameObject GetBodyObject()
    {
        if (m_objBody == null)
        {
            Transform chBody = GetBodyParentObject().transform.Find("body");
            if (chBody != null)
            {
                m_objBody = chBody.gameObject;
            }
        }
        return m_objBody;
    }
    GameObject m_objBip01 = null;
    public GameObject GetObject_Bip01(bool isLoad = false)
    {
        if (m_objBip01 == null || isLoad)
        {
            mainPlayerCfg cfg = GetBodyObject().GetComponent<mainPlayerCfg>();
            Transform t = cfg.m_rootTransform;
            if (t != null)
            {
                m_objBip01 = t.gameObject;
            }
        }
        return m_objBip01;
    }
    GameObject m_objAdherentPoints = null;
    public GameObject GetObject_AdherentPoints(bool isLoad = false)
    {
        if (m_objAdherentPoints == null || isLoad)
        {
            Transform t = GetBodyObject().transform.Find("AdherentPoints");
            if (t != null)
            {
                m_objAdherentPoints = t.gameObject;
            }
            else
            {
                m_objAdherentPoints = GetBodyObject();
            }
        }
        return m_objAdherentPoints;
    }
    //----------------------------------------------------------------------------
    //// 切换指定部位: (部位名, Mesh文件, 材质文件)
    //public bool NtfChangePart(string szPart, string szMshFile, string szMtrFile)
    //{
    //    GameObject obj = GetBodyObject();
    //    return ActorDressUtil.NtfChangePart(ref obj, szPart, szMshFile, szMtrFile);
    //}

    ////----------------------------------------------------------------------------
    //// 切换Mesh, 材质使用 szMshFile 里的
    //public bool NtfChangePartMesh(string szPart, string szMshFile)
    //{
    //    GameObject obj = GetBodyObject();
    //    return ActorDressUtil.NtfChangePartMesh(ref obj, szPart, szMshFile);
    //}
    
    ////----------------------------------------------------------------------------
    //// 仅切换材质
    //public bool NtfChangePartMaterial(string szPart, string szMtrFile)
    //{
    //    GameObject obj = GetBodyObject();
    //    return ActorDressUtil.NtfChangePartMaterial(ref obj, szPart, szMtrFile);
    //}

	//------------------------------------------------------------------------------------------------


	#endregion
    #region combo
    public virtual void OnStaminaEvent(Actor source, out bool isFly)
    {//npc实现
        isFly = false;
    }
    //combo改变
    public virtual void OnComboChanged(int resultID, int targetID, float damageModify)
    {
    }
    //削减combo连击的时间间隔
    public virtual void OnWeakenComboTime(float time)
    {
        ;
    }
    public void ComboClear()
    {
        if (Combo != null)
        {
            Combo.ComboClear();
        }
    }
    #endregion
    public List<IWidget> m_reduceHPList = new List<IWidget>();
    public virtual void OnHpChanged(int hp, bool isCrit, float multiple, bool isHeal)
	{
		if (hp > 0)
		{
			NpcReduceHP widget = MainGame.Singleton.CurrentWidgets.AddWidget<NpcReduceHP>();
			widget.SetValue(hp, isCrit, multiple, isHeal);
			widget.m_obj.transform.position = TextTrans.position;

			m_reduceHPList.Add(widget as IWidget);
		}
        m_reduceHPList.RemoveAll(item => !item.IsEnable);
        for (int i = 0; i < m_reduceHPList.Count; ++i)
        {
            if (i + 1 == m_reduceHPList.Count)
            {//最后一个是自己，不通知
                continue;
            }
            m_reduceHPList[i].Notify();
        }
    }
    public float SetCurHP(float curHP)
    {
        if (curHP > MaxHP)
        {
            curHP = MaxHP;
        }
        else if (curHP < 0)
        {
            curHP = 0;
        }
        HP = curHP;
        this.UpdateHPBar();
        return curHP;
    }
    public void ClearData()
    {
        CDControl.RemoveAll();
    }
    public void HideMe(bool isHide = false)
    {
        if (!isHide)
        {
            MainPos = new Vector3(10000.0f*Camp, 0.0f, 10000.0f);
        }
        else
        {
            MainObj.SetActive(false);
            if (ShadowObj != null)
            {
                ShadowObj.SetActive(false);
            }
        }
        RealPosIsValidate = false;
		IsActorExit = true;
    }
    public void MoveTo(Vector3 pos)
    {
        if (MainObj == null)
        {
            return;
        }
        MainPos = pos;
        RealPos = pos;
        RealPosIsValidate = false;
        IsActorExit = false;
    }
    public void UnhideMe(Vector3 pos)
    {
        if (MainObj == null)
        {
            return;
        }
        MainPos = pos;
        RealPos = pos;
        MainObj.SetActive(true);
        if (ShadowObj != null)
        {
            ShadowObj.SetActive(true);
        }
        RealPosIsValidate = false;
		IsActorExit = false;
    }
    public virtual bool IsCanSelected()
    {
        return true;
    }
    public void AnimationSpeedChanged(string animName = "")
    {
        MoveAction action = ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
        if (action != null)
        {
            if (!string.IsNullOrEmpty(action.AnimFullName))
            {
                animName = action.AnimFullName;
            }
            AnimationState state = MainAnim[animName];
            if (state != null)
            {
                state.speed = AnimationSpeed;
            }
        }
        AttackAction aAction = ActionControl.LookupAction(AttackAction.SGetActionType()) as AttackAction;
        if (aAction != null)
        {
            if (!string.IsNullOrEmpty(aAction.AnimFullName))
            {
                animName = aAction.AnimFullName;
            }
            AnimationState state = MainAnim[animName];
            if (state != null)
            {
                state.speed = AttackAnimSpeed;
            }
        }
    }

    public bool IsSkillCDRunning(int skillID)
    {
        return SkillControl.IsSkillCDRunning(skillID, this);
    }
    public enum ENSkillSilenceType
    {
        enNone,
        enSkill,//对应技能表中的SkillType
        enSkillSpecial,//对应技能表中的SkillSpecialType
    }
    //通知技能沉默
    public void NotifySkillSilence(int silenceType, int type, bool isSilence)
    {
        foreach (var item in SkillBag)
        {
            switch ((ENSkillSilenceType)silenceType)
            {
                case ENSkillSilenceType.enSkill:
                    {
                        if (item.SkillTableInfo.SkillType == type)
                        {
                            item.IsSilence = isSilence;
                        }
                    }
                    break;
                case ENSkillSilenceType.enSkillSpecial:
                    {
                        if (item.SkillTableInfo.SkillSpecialType == type)
                        {
                            item.IsSilence = isSilence;
                        }
                    }
                    break;
            }
        }
        OnSkillSilence(silenceType, type, isSilence);
    }
    public virtual void OnSkillSilence(int silenceType, int type, bool isSilence)
    {
    }

    //通知Actor变身
    //buff变身时的id
    int m_changeModelBuffID = 0;
    //获取初始Actor的ModelID
    public virtual int GetActorInitModelID()
    {
        return 0;
    }
    //获取初始Actor的WeaponID
    public virtual int GetActorInitWeaponID()
    {
        return 0;
    }
    //检测变身是否是BUFF触发的
    public bool ChangeModeByBuff(out int modelId, out int weaponid)
    {
        int changeModelBuffID = 0;
        modelId = 0;
        weaponid = 0;
        foreach (var item in this.MyBuffControl.BuffList)
        {
            if (null != item && !item.IsNeedRemove)
            {
                foreach (var effectItem in item.EffectList)
                {//查找改变模型的buffid
                    if (effectItem.ClassID == ENBuff.ChangeModel)
                    {
                        changeModelBuffID = item.BuffID;
                    }
                }
            }
        }
        if (changeModelBuffID != 0)
        {
            if (m_changeModelBuffID == changeModelBuffID)
            {
                return true;
            }
        }

        m_changeModelBuffID = changeModelBuffID;
        if (changeModelBuffID != 0)
        {
            BuffInfo info = GameTable.BuffTableAsset.Lookup(changeModelBuffID);
            if (info == null)
            {
                Debug.LogWarning("buff is null, buff id is " + changeModelBuffID);
                return false;
            }
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ENBuff.ChangeModel)
                {
                    switch ((BuffEffectChangeMode.ENChangeModelType)item.ParamList[0])
                    {
                        case BuffEffectChangeMode.ENChangeModelType.enChangeModel:
                            {
                                modelId = (int)item.ParamList[1];
                                weaponid = (int)item.ParamList[2];
                            }
                            break;
                    }
                }
            }
        }
        else
        {
            return false;
        }
        return true;
    }

    //临时的模型ID
    int tmp_modelId = 0;
    //临时的武器ID
    int tmp_weaponId = 0;
    public void NotifyChangeModel()
    {
        tmp_modelId = CurModelID;
        tmp_weaponId = CurModel2WeaponID;
        int tmpModelId = 0;
        int tmpWeaponId = 0;
        if (ChangeModeByBuff(out tmpModelId, out tmpWeaponId))
        {
            if (tmpModelId != 0)
            {//认为不是同一个Buff的变身
                CurModelID = tmpModelId;
                CurModel2WeaponID = tmpWeaponId;
            }
            else
            {//认为是同一个BUff的变身
                return;
            }
        }
        else
        {//Buff时间终止，还原
            CurModelID = GetActorInitModelID();
            CurModel2WeaponID = GetActorInitWeaponID();
        }
        MainGame.Singleton.StartCoroutine(Coroutine_ChangeModel());
    }
    IEnumerator Coroutine_ChangeModel()
    {
        //改变BODY
        if (CurModelID != tmp_modelId)
        {
            GameResPackage.AsyncLoadPackageData data = new GameResPackage.AsyncLoadPackageData();
            IEnumerator e = Coroutine_ChangeModelBody(CurModelID, CurModel2WeaponID, data);
            while (true)
            {
                e.MoveNext();
                if (data.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
        }
        else
        {
            //加载武器
            if (CurModel2WeaponID != tmp_weaponId)
            {
                GameResPackage.AsyncLoadPackageData data2 = new GameResPackage.AsyncLoadPackageData();
                IEnumerator e = UIManager.Singleton.Coroutine_LoadWeapon(GetBodyObject(), CurModel2WeaponID, data2);
                while (true)
                {
                    e.MoveNext();
                    if (data2.m_isFinish)
                    {
                        break;
                    }
                    yield return e.Current;
                }
            }
        }
    }

    void CreateParentObj()
    {
        
        GameObject parentObj = new GameObject();
        parentObj.name = "parent";
        parentObj.transform.parent = MainObj.transform;
        //加载模型的父的所有事件脚本
        parentObj.AddComponent<AnimationCameraCallback>();
        parentObj.AddComponent<AnimationEffectCallback>();
        parentObj.AddComponent<AnimationEndCallback>();
        parentObj.AddComponent<AnimationShaderParamCallback>();
        parentObj.AddComponent<AnimationSoundCallback>();
        parentObj.AddComponent<BreakPointCallback>();
        parentObj.AddComponent<AttackActionCallback>();

        parentObj.AddComponent<Animation>();
        //
        ActorProp prop = parentObj.AddComponent<ActorProp>();
        prop.Type = Type;
        prop.ID = ID;
        prop.ActorLogicObj = this;
    }
    string m_animPrefix { get; set; }
    IEnumerator Coroutine_LoadBodyParent(int modelId, int weaponId, GameResPackage.AsyncLoadPackageData loadData)
    {
        if (GetBodyParentObject() == null)
        {
            CreateParentObj();
        }
        //加载技能事件
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        GameObject parentObj = GetBodyParentObject();
        ModelInfo info = GameTable.ModelInfoTableAsset.Lookup(modelId);
        WeaponInfo wInfo = GameTable.WeaponInfoTableAsset.Lookup(weaponId);

        Animation parentAnim = parentObj.GetComponent<Animation>();

        OnInitSkillBag();
        List<int> skillIDList = new List<int>();
        if (null != SkillBag)
        {
            foreach (var item in SkillBag)
            {
                skillIDList.Add(item.SkillTableInfo.ID);
            }
            if (info != null && wInfo != null)
            {
                m_animPrefix = "a-" + info.modelType.ToString() + "-w" + ((int)wInfo.WeaponType).ToString() + "-";
                List<string> animList = GetAnimationList(info.modelType.ToString(), ((int)wInfo.WeaponType).ToString(), skillIDList);
                foreach (var item in animList)
                {
                    data.Reset();
                    IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetSkillAnimPath(item), data, false, false);
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
                        AnimationClip clip = data.m_obj as AnimationClip;
                        if (null == parentAnim.GetClip(clip.name))
                        {
                            parentAnim.AddClip(clip, clip.name);
                        }
                    }
                }
            }
        }
        
        loadData.m_isFinish = true;
    }

    IEnumerator Coroutine_ChangeModelBody(int modelID, int weaponId, GameResPackage.AsyncLoadPackageData loadData)
    {
        {//加载body的父，用来播放技能动作事件
            GameResPackage.AsyncLoadPackageData data = new GameResPackage.AsyncLoadPackageData();
            IEnumerator e = Coroutine_LoadBodyParent(modelID, weaponId, data);
            while (true)
            {
                e.MoveNext();
                if (data.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
        }
        //加载要变成的body预设
        GameObject curBody = GetBodyObject();
        GameObject body = null;
        ModelInfo info = GameTable.ModelInfoTableAsset.Lookup(modelID);
        if (null == info)
        {
            Debug.LogWarning("Error ModelInfo is null" + modelID);
            CurModelID = tmp_modelId;
            CurModel2WeaponID = tmp_weaponId;
        }
        else
        {
            GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
            IEnumerator e = Coroutine_LoadBody(info.ModelFile, modelID, ModelScale, data);// PoolManager.Singleton.Coroutine_Load(GameData.GetPrefabPath(info.ModelFile), data, true);
            while (true)
            {
                e.MoveNext();
                if (data.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
            if (data.m_obj == null)
            {
                CurModelID = tmp_modelId;
                CurModel2WeaponID = tmp_weaponId;
            }
            else
            {
                body = data.m_obj as GameObject;
                body.name = "body";
                body.transform.parent = GetBodyParentObject().transform;
                body.transform.localPosition = Vector3.zero;
                body.transform.localRotation = Quaternion.identity;
                //
                CurModelID = modelID;
                //加载武器
                GameResPackage.AsyncLoadPackageData data2 = new GameResPackage.AsyncLoadPackageData();
                e = UIManager.Singleton.Coroutine_LoadWeapon(body, CurModel2WeaponID, data2);
                while (true)
                {
                    e.MoveNext();
                    if (data2.m_isFinish)
                    {
                        break;
                    }
                    yield return e.Current;
                }

                //是否加载影子
                if (ShadowObj == null)
                {
                    bool isLoadShadowObj = true;
                    if (this.Type == ActorType.enNPC)
                    {
                        NPCInfo npcInfo = GameTable.NPCInfoTableAsset.Lookup(this.IDInTable);
                        if (npcInfo != null)
                        {
                            if (npcInfo.Type == (int)ENNpcType.enBlockNPC || npcInfo.Type == (int)ENNpcType.enBoxNPC)
                            {
                                isLoadShadowObj = false;
                            }
                        }
                    }
                    if (isLoadShadowObj)
                    {
                        data.Reset();
                        e = PoolManager.Singleton.Coroutine_Load("Prefabs/Player/shadow.fbx", data, true);
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
                            ShadowObj = data.m_obj as GameObject;
                            Transform tShadow = GetObject_AdherentPoints().transform.Find("shadowPoint");
                            if (tShadow != null)
                            {
                                ShadowObj.transform.parent = tShadow;
                            }
                            else
                            {
                                Debug.LogWarning("ShadowPoint is null");
                            }
                            ShadowObj.transform.localPosition = Vector3.zero;
                            ShadowObj.transform.localScale = Vector3.one;
                        }
                    }
                }
                //加载动画
                if (Type != ActorType.enNPCTrap)
                {
                    PreAnimationLoad.Singleton.LoadAnimation(body, modelID, weaponId);
                }
                if (Type == ActorType.enMain)
                {
                    TryLoadGhost();
                }
            }
        }
        
        if (curBody != null)
        {
            ChangeOldBody2NewBody(curBody, body);
            body.SetActive(true);

            m_objBodyParent = null;
            m_objBody = null;

            //修改用到的模型下的挂载点的更新
            GetObject_Bip01(true);
            GetObject_AdherentPoints(true);
            //GameObject.Destroy(curBody);
            PoolManager.Singleton.ReleaseObj(curBody);
            //PreAnimationLoad.Singleton.LoadAnimation(body.gameObject, modelID, weaponId);
            RefreshBodyElement(info.Scale, modelID);
        }
        else
        {
            OnLoaded(info.Scale, modelID);
        }
        loadData.m_isFinish = true;
    }
    public void ChangeOldBody2NewBody(GameObject oldBody, GameObject newBody)
    {
        oldBody.name = "preBody";
        Transform oldT = oldBody.transform.Find("AdherentPoints");
        Transform newT = newBody.transform.Find("AdherentPoints");
        if (null != oldT || null != newT)
        {
            for (int i = 0; i < oldT.childCount; ++i)
            {
                Transform oldChildT = oldT.GetChild(i);
                if (null == oldChildT)
                {
                    continue;
                }
                Transform newChildT = newT.Find(oldChildT.name);
                if (null == newChildT)
                {
                    continue;
                }
                for (int j = 0; j < oldChildT.childCount;)
                {
                    Debug.Log("count:" + oldChildT.childCount + ", cur index:" + j);
                    Transform t = oldChildT.GetChild(j);
                    if (null == t)
                    {
                        ++j;
                        continue;
                    }
                    t.parent = newChildT;
                    t.localPosition = Vector3.zero;
                }
            }
        }
        oldBody.SetActive(false);
    }
    //修改用到的模型及模型下边的TransForm中的脚本更新
//         ChangeDataAction(modelScale, modelID);
//         mAnimControl.Reset();
    //更新Actor身上预设数据
    public virtual void LoadedByChangeBodyElement()
    {
		GameObject objAP = GetObject_AdherentPoints();
		if (null != objAP)
		{
			Transform tAdherentPoints = objAP.transform;
			HPBarTrans = tAdherentPoints.Find("HPbarPoint");
			TextTrans = tAdherentPoints.Find("TextPoint");
		}
    }

	//加载血条
	public virtual void AddHPBar()
	{
		string uiName = "UIBloodBar";
		PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetUIPath(uiName), LoadBloodCallback);
	}
	//设置血条
	public void LoadBloodCallback(UnityEngine.Object obj)
	{
        if (obj != null && null != HPBarTrans)
		{
			mHPBarObj = obj as GameObject;
			mHPBarObj.name = "Bar_" + Type.ToString() + "_" + ID.ToString();
			mHPBarObj.transform.parent = HPBarTrans.transform;
			mHPBarObj.transform.localPosition = Vector3.zero;
		}
		else 
		{
			Debug.Log("The HPBarUI Error");
		}
	}
    //刷新模型Body上的各种元素
    public void RefreshBodyElement(float modelScale, int modelID)
    {
        Transform bodyTrans = GetBodyObject().transform;
        for (int i = 0; i < bodyTrans.childCount; i++)
        {
            Transform child = bodyTrans.GetChild(i);
            CenterCollider = child.GetComponent<Collider>();
            if (null != CenterCollider)
            {
                CenterPart = child.gameObject;
                break;
            }
        }
        ActorCfgData = bodyTrans.gameObject.GetComponent<ActorConfigData>();
        if (null == ActorCfgData)
        {
            ActorCfgData = bodyTrans.gameObject.AddComponent<ActorConfigData>();
        }
        //bodyTrans.transform.localPosition = Vector3.zero;
        if (null != ActorCfgData && null == ActorCfgData.MeshRender)
        {
            for (int i = 0; i < bodyTrans.childCount; i++)
            {
                Transform child = bodyTrans.GetChild(i);
                ActorCfgData.MeshRender = child.GetComponent<SkinnedMeshRenderer>();
                if (null != ActorCfgData.MeshRender)
                {
                    break;
                }
            }
        }
        //!锁定目标功能,勿添加其它功能逻辑 added by guo
        if (Type == ActorType.enMain)
        {
            MainPlayerConfig = bodyTrans.GetComponent<mainPlayerCfg>();
            if (mActorBlendAnim == null)
            {
                //mActorBlendAnim = new ActorBlendAnim(this);
            }
        }
        //////////////////////////////////////////////////////////////////////////////////
        if (CenterPart != null)
        {
            CenterPart.layer = LayerMask.NameToLayer("Actor");
        }
        //CenterPart.transform.localScale = new Vector3(modelScale, modelScale, modelScale);
        //CenterCollider = CenterPart.GetComponent<Collider>();
        CapsuleCollider collider = CenterCollider as CapsuleCollider;
        if (null != collider)
        {
            if (this.Type == ActorType.enSwitch)
            {
                collider.enabled = false;
            }

            mCapsulYSize = modelScale * collider.height + collider.center.z;
            mCapsulRangeSize = collider.radius;
        }

        //自身播放特效
        AnimationEffectCallback effectCallback = GetBodyParentObject().GetComponent<AnimationEffectCallback>();
        if (null == effectCallback)
        {
            effectCallback = GetBodyParentObject().AddComponent<AnimationEffectCallback>();
        }
        effectCallback.Callback = OnPlayEffect;
        effectCallback.ConnectEffectCallback = OnPlayConnectEffect;
        effectCallback.ComboEffectCallback = OnPlayComboEffect;

        //添加Trigger的回调脚本
        TriggerCallback triggerCallback = GetBodyObject().GetComponent<TriggerCallback>();
        if (triggerCallback == null)
        {
            triggerCallback = GetBodyObject().AddComponent<TriggerCallback>();
        }
        triggerCallback.EnterCallback = OnTriggerEnter;

        //更改Animation脚本
        MainAnim = bodyTrans.GetComponent<Animation>();
        if (MainAnim == null)
        {
            MainAnim = bodyTrans.gameObject.AddComponent<Animation>();
        }
        if (MainAnim != null)
        {
            AnimationConfig animConfig = MainAnim.GetComponent<AnimationConfig>();
            if (null != animConfig && null != animConfig.Trail)
            {
                animConfig.Trail.ClearTrail();
            }
        }

        //ColliderArray
        ColliderArray = GetBodyObject().transform.GetComponents<Collider>();
        //更改挂载点的脚本
        if (GetObject_AdherentPoints() != null)
        {
            AdherentPoints = GetObject_AdherentPoints().GetComponent<ActorAdherentPoints>();
            if (AdherentPoints == null)
            {
                AdherentPoints = GetObject_AdherentPoints().AddComponent<ActorAdherentPoints>();
            }
        }
        LoadedByChangeBodyElement();
    }

//     public void ChangeDataAction(float modelScale, int modelID)
//     {
//         Transform bodyTrans = GetBodyObject().transform;
//         if (Type != ActorType.enNPCTrap)
//         {
//             PreAnimationLoad.Singleton.LoadAnimation(bodyTrans.gameObject, modelID, WeaponType);
//         }
//         for (int i = 0; i < bodyTrans.childCount; i++)
//         {
//             Transform child = bodyTrans.GetChild(i);
//             CenterCollider = child.GetComponent<Collider>();
//             if (null != CenterCollider)
//             {
//                 CenterPart = child.gameObject;
//                 break;
//             }
//         }
//         ActorCfgData = bodyTrans.gameObject.GetComponent<ActorConfigData>();
//         if (null == ActorCfgData)
//         {
//             ActorCfgData = bodyTrans.gameObject.AddComponent<ActorConfigData>();
//         }
//         //bodyTrans.transform.localPosition = Vector3.zero;
//         if (null != ActorCfgData && null == ActorCfgData.MeshRender)
//         {
//             for (int i = 0; i < bodyTrans.childCount; i++)
//             {
//                 Transform child = bodyTrans.GetChild(i);
//                 ActorCfgData.MeshRender = child.GetComponent<SkinnedMeshRenderer>();
//                 if (null != ActorCfgData.MeshRender)
//                 {
//                     break;
//                 }
//             }
//         }
//         //!锁定目标功能,勿添加其它功能逻辑 added by guo
//         if (Type == ActorType.enMain)
//         {
//             MainPlayerConfig = bodyTrans.GetComponent<mainPlayerCfg>();
//             if (mActorBlendAnim == null)
//             {
//                 mActorBlendAnim = new ActorBlendAnim(this);
//             }
//         }
//         //////////////////////////////////////////////////////////////////////////////////
//         //CenterPart = bodyTrans.FindChild("Bip01").gameObject;
//         CenterPart.layer = LayerMask.NameToLayer("Actor");
//         //CenterPart.transform.localScale = new Vector3(modelScale, modelScale, modelScale);
//         //CenterCollider = CenterPart.GetComponent<Collider>();
//         CapsuleCollider collider = CenterCollider as CapsuleCollider;
//         if (null != collider)
//         {
//             if (this.Type == ActorType.enSwitch)
//             {
//                 collider.enabled = false;
//             }
// 
//             mCapsulYSize = modelScale * collider.height + collider.center.z;
//             mCapsulRangeSize = collider.radius;
//         }
// 
//         //自身播放特效
//         AnimationEffectCallback effectCallback = GetBodyParentObject().GetComponent<AnimationEffectCallback>();
//         if (null == effectCallback)
//         {
//             effectCallback = GetBodyParentObject().AddComponent<AnimationEffectCallback>();
//         }
//         effectCallback.Callback = OnPlayEffect;
//         effectCallback.ConnectEffectCallback = OnPlayConnectEffect;
//         effectCallback.ComboEffectCallback = OnPlayComboEffect;
// 
//         //更改Animation脚本
//         MainAnim = bodyTrans.GetComponent<Animation>();
//         AnimationConfig animConfig = MainAnim.GetComponent<AnimationConfig>();
//         if (null != animConfig && null != animConfig.Trail)
//         {
//             animConfig.Trail.ClearTrail();
//         }
// 
//         //ColliderArray
//         ColliderArray = GetBodyObject().transform.GetComponents<Collider>();
//         //更改挂载点的脚本
//         if (GetObject_AdherentPoints() != null)
//         {
//             AdherentPoints = GetObject_AdherentPoints().GetComponent<ActorAdherentPoints>();
//             if (AdherentPoints == null)
//             {
//                 AdherentPoints = GetObject_AdherentPoints().AddComponent<ActorAdherentPoints>();
//             }
//         }
//     }

    public void SetBlastPos(Vector3 tmpBlastPos, Vector3 forward)
    {
        if (this.RealPos == tmpBlastPos)
        {
            mBlastPoint = MainPos + this.GetBodyObject().transform.forward * 5;
            mBlastForward = -this.GetBodyObject().transform.forward; 
        }
        else
        {
            //爆破点是攻击者坐标，因受击者与攻击者过近，所以爆破点是
            //攻击者身后5米处
            mBlastPoint = tmpBlastPos;
            mBlastForward =forward;
        }
       
    }
    public void SetBlastPos(int sourActorID)
    {
        Actor sourActor = ActorManager.Singleton.Lookup(sourActorID);
        if (sourActor == null || sourActor == this)
        {
            //如果源Actor 不存在或者是自己 那么爆破点在自己前方+1
            Vector3 tmpPos = sourActor.RealPos + this.GetBodyObject().transform.forward * 5;
            SetBlastPos(tmpPos, this.GetBodyObject().transform.forward);
        }
        else
        {
            SetBlastPos(sourActor.RealPos, sourActor.GetBodyObject().transform.forward);
        }
        
    }
    //潜行buffid
    int m_sneakBuffID = 0;
    //开始潜行
    public void StartSneak(int buffID)
    {
        TempType = ActorType.enPlayer_Sneak;
        m_sneakBuffID = buffID;
        //通知所有人清除目标
        ActorManager.Singleton.NotifyAll_ClearTarget(ID);
    }
    //停止潜行
    public void StopSneak()
    {
        TempType = Type;
        IResult r = BattleFactory.Singleton.CreateResult(ENResult.RemoveBuff, ID, ID, 0, 0, new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffID, m_sneakBuffID });
        if (r != null)
        {
            r.ResultExpr(new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffID, m_sneakBuffID });
            BattleFactory.Singleton.DispatchResult(r);
        }
    }
    //获取种族类型
    public virtual int OnGetRaceType()
    {
        return 0;
    }
    public bool IsHaveDropItem()
    {
        if (null == roomElement)
        {
            return false;
        }
        if (roomElement.MonsterData.dropList.Count <= 0)
        {
            return false;
        }
        return true;
    }
    public virtual void OnDead()
    {
        IsRealDead = true;
        if (null == roomElement)
        {
            return;
        }
        if (roomElement.MonsterData.dropList.Count <= 0)
        {
            return;
        }
        BattleArena.Singleton.DropCardsIdList.AddRange(roomElement.MonsterData.dropList);
    }
    //添加意志
    public virtual void OnAddWill(WillToFight.ENAddWillType type, float[] paramList)
    {
        ;
    }

    #region DamageInfo//受到伤害时的信息
    //伤害来源
    public Actor DamageSource = null;
    //伤害时间
    public float DamageTime = 0;
    #endregion
    public void Damgaed(Actor source)
    {
        if (source != null && !source.IsDead)
        {
            DamageSource = source;
            DamageTime = Time.time;
        }
    }
    //隐藏时调用
    public virtual void OnBecameInvisible()
    {
        ;
    }
    //被伤害记录
    public Dictionary<int, int> m_damagedRecordMap = new Dictionary<int, int>();
    public void DamagedRecord(int actorID, int damageValue)
    {
        if (m_damagedRecordMap.ContainsKey(actorID))
        {
            m_damagedRecordMap[actorID] += damageValue;
        }
        else
        {
            m_damagedRecordMap.Add(actorID, damageValue);
        }
    }
    //攻击collider的回调函数
    void OnTriggerEnter(GameObject gameObject, Collider other)
    {
        if (!ActorTargetManager.IsTrigger(other))
        {
            return;
        }
        GlobalEnvironment.Singleton.IsInCallbackOrTrigger = true;
        try
        {
            AttackAction action = ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
            if (null != action)
            {
                action.OnTriggerEnter(gameObject, other);
            }
            else
            {
                AttackingMoveAction amAction = ActionControl.LookupAction(AttackingMoveAction.SGetActionType()) as AttackingMoveAction;
                if (amAction != null)
                {
                    amAction.OnTriggerEnter(gameObject, other);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error On OnTriggerEnter" + e.Message + ",,,,Stack:" + e.StackTrace.ToString());
            DebugLog.Singleton.OnShowLog("[AttackActionCallback Error On OnTriggerEnter] " + e.Message + " " + e.StackTrace.ToString());
        }
    }
    //隐藏body下的所有istrigger状态为true的collider
    public void HideTriggerCollider()
    {
        if (ColliderArray != null)
        {
            foreach (var item in ColliderArray)
            {
                if (item.isTrigger)
                {
                    item.enabled = false;
                }
            }
        }
    }
    //播放武器动作(isRArm：false为左手，true为右手，animName：动画名称)
    public void PlayWeaponsAnimation(bool isRArm, string animName)
    {
        string arm = "LeftArmWeapon";
        if (isRArm)
        {
            arm = "RightArmWeapon";
        }
        GameObject body = GetBodyObject();
        if (body == null)
        {
            Debug.LogWarning("PlayWeaponsAnimation fail, body is null,ID:" + ID);
            return;
        }
        Transform t = LookupBone(body.transform, arm);
        if (t == null)
        {
            Debug.LogWarning("PlayWeaponsAnimation fail, look bone fail,bone:" + arm);
            return;
        }
        if (t.GetComponent<Animation>() == null)
        {
            Debug.LogWarning("PlayWeaponsAnimation fail, bone's animation is null,bone:" + arm);
            return;
        }
        AnimationClip clip = t.GetComponent<Animation>().GetClip(animName);
        if (clip == null)
        {
            Debug.LogWarning("PlayWeaponsAnimation fail, clip is null, animName:"+animName);
            return;
        }
        t.GetComponent<Animation>().Stop();
        t.GetComponent<Animation>().Play(clip.name);
    }
    //显示或隐藏武器表中隐藏的武器
    public void ShowWeaponModelWithTable(bool isShow = true)
    {
        WeaponInfo info = GameTable.WeaponInfoTableAsset.Lookup(WeaponType);
        if (info != null)
        {
            if (info.IsHideRightModel)
            {
                Transform t = LookupBone(GetObject_Bip01().transform, "RightArmWeapon");
                if (t != null)
                {
                    t.gameObject.SetActive(isShow);
                }
            }
            if (info.IsHideLeftModel)
            {
                Transform t = LookupBone(GetObject_Bip01().transform, "LeftArmWeapon");
                if (t != null)
                {
                    t.gameObject.SetActive(isShow);
                }
            }
        }
    }
    //停止播放所有武器动作
    public void StopPlayWeaponsAnimation()
    {
        GameObject body = GetBodyObject();
        if (body == null)
        {
            Debug.LogWarning("StopPlayWeaponsAnimation fail, body is null,ID:" + ID);
            return;
        }
        Transform t = LookupBone(body.transform, "LeftArmWeapon");
        if (t != null && t.GetComponent<Animation>() != null)
        {
            t.GetComponent<Animation>().Stop();
            //播放默认动画
            t.GetComponent<Animation>().Play(t.GetComponent<Animation>().clip.name);
        }
        t = LookupBone(body.transform, "RightArmWeapon");
        if (t != null && t.GetComponent<Animation>() != null)
        {
            t.GetComponent<Animation>().Stop();
            //播放默认动画
            t.GetComponent<Animation>().Play(t.GetComponent<Animation>().clip.name);
        }
    }
    //初始化技能背包
    public virtual void OnInitSkillBag(bool isNotifyUI = false)
    {
        //技能列表
		PropertyValueIntListView skillIDList = Props.GetProperty_Custom(ENProperty.SkillIDList) as PropertyValueIntListView;
        PropertyValueIntListView skillLevelList = Props.GetProperty_Custom(ENProperty.SkillLevelList) as PropertyValueIntListView;
		if (null != skillIDList)
		{
            //被动技能，清除buff
            foreach (var item in skillIDList.m_list)
            {
                SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(item);
                if (skillInfo == null) continue;
                if (skillInfo.SkillType == (int)ENSkillType.enPassive)
                {
                    List<float> paramList = new List<float>(1 + skillInfo.BuffIDList.Count);
                    paramList.Add((float)ResultRemoveBuff.ENRemoveBuffType.enBuffID);
                    paramList.AddRange(skillInfo.BuffIDList);

                    IResult r = BattleFactory.Singleton.CreateResult(ENResult.RemoveBuff, ID, ID, 0, 0, paramList.ToArray());
                    if (r != null)
                    {
                        r.ResultExpr(paramList.ToArray());
                        BattleFactory.Singleton.DispatchResult(r);
                    }
                }
            }
            //添加主动技能
            for (int i = 0; i < skillIDList.m_list.Length; ++i)
            {
                int skillID = skillIDList.m_list[i];
                int skillLevel = skillLevelList.m_list[i];
                SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(skillID);
                if (skillInfo == null) continue;
                if (skillInfo.SkillType == (int)ENSkillType.enPassive)
                {//被动技能
                    IResult r = BattleFactory.Singleton.CreateResult(ENResult.AddBuff, ID, ID, 0, 0, skillInfo.BuffIDList.ToArray());
                    if (r != null)
                    {
                        r.ResultExpr(skillInfo.BuffIDList.ToArray());
                        BattleFactory.Singleton.DispatchResult(r);
                    }
                    continue;
                }
                if (null == SkillBag.Find(info => info.SkillTableInfo.ID == skillID))
                {
                    SkillBag.Add(new ActorSkillInfo(skillInfo, skillLevel));
                }
            }
		}
        
    }
    //朝向
    public void Forward(Vector3 targetPos)
    {
        Vector3 forward = targetPos - MainPos;
        forward.y = 0.0f;
        forward.Normalize();
        MainObj.transform.forward = forward;
    }
    //初始化属性
    public virtual void OnInitProps()
    {
    }

	//切换技能
    public void ChangeSkill(Dictionary<int, int> skillList)
	{
		MainGame.Singleton.StartCoroutine(Coroutine_ChangeSkill(skillList));
	}

    IEnumerator Coroutine_ChangeSkill(Dictionary<int, int> skillList)
	{
        //修改属性
        PropertyValueIntListView propSkillIDList = Props.GetProperty_Custom(ENProperty.SkillIDList) as PropertyValueIntListView;
        foreach (var item in skillList)
        {
            propSkillIDList.m_list[item.Key] = item.Value;
        }
        Props.SetProperty_Custom(ENProperty.SkillIDList, propSkillIDList);
        GameResPackage.AsyncLoadPackageData data = new GameResPackage.AsyncLoadPackageData();
        IEnumerator e = Coroutine_LoadBodyParent(CurModelID, WeaponType, data);
        while (true)
        {
            e.MoveNext();
            if (data.m_isFinish)
            {
                break;
            }
            yield return e.Current;
        }
        NotifyChanged((int)ENPropertyChanged.enBattleBtn_D, MainPlayer.ENBattleBtnNotifyType.enSkillChange);
        if (m_isDeposited)
        {
            IMiniServer.Singleton.SendTestAllSkill_C2BS(ID, new List<int>(propSkillIDList.m_list));
        }
	}
    //更新技能事件animation
    public void UpdateSkillEventAnimation()
    {
        GameResPackage.AsyncLoadPackageData data = new GameResPackage.AsyncLoadPackageData();
        MainGame.Singleton.StartCoroutine(Coroutine_LoadBodyParent(CurModelID, WeaponType, data));
    }
}