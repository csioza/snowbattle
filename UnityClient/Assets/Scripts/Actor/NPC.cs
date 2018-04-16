using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region disableWarning
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#endregion

public class NPC : Actor
{
    GameObject m_objAttackRange    = null;
    GameObject m_objAlertRange     = null;

    NPCInfo m_npcInfo = null;
    public NPCInfo CurrentTableInfo { get { return m_npcInfo; } private set { m_npcInfo = value; } }
    UIBossBloodBar m_uiBossBloodBar = null;
    List<Vector3> m_patrolList = new List<Vector3>();
    public List<Vector3> PatrolList { get { return m_patrolList; } set { m_patrolList = value; } }

    public NPCBehaviour NpcBehaviour { get; private set; }
    //public ActorConfigData NpcConfigData { get; private set; }
    public Transform HPBarTrans { get; private set; }
    public Transform WarningTipTrans { get; private set; }
    //!是否是激活
    public bool IsActive { get; set; }
    //!当前所属的ROOM
    public RoomBase CurRoom { set; get; }
    //优先攻击标志
    public bool m_priorityAttack = false;
    #region AttackRange//攻击范围
    private float m_attackRange = 0;
    public float AttackRange
    {
        get
        {
            if (m_arChangeType == ENARChangeType.enLarge)
            {
                float now = Time.time;
                m_attackRange = m_attackRange + (now - m_arChangeStartTime) * CurrentTableInfo.LargeRate;
                m_arChangeStartTime = now;
                if (m_attackRange > CurrentTableInfo.AlertRange)
                {
                    m_attackRange = CurrentTableInfo.AlertRange;
                    m_arChangeType = ENARChangeType.enNone;
                }
            }
            else if (m_arChangeType == ENARChangeType.enSmall)
            {
                float now = Time.time;
                m_attackRange = m_attackRange - (now - m_arChangeStartTime) * CurrentTableInfo.SmallRate;
                m_arChangeStartTime = now;
                if (m_attackRange < CurrentTableInfo.AttackRange)
                {
                    m_attackRange = CurrentTableInfo.AttackRange;
                    m_arChangeType = ENARChangeType.enNone;
                }
            }
            else
            {
                if (m_attackRange == 0)
                {
                    m_attackRange = CurrentTableInfo.AttackRange;
                }
            }
            return m_attackRange;
        }
        set
        {
            m_attackRange = value;
        }
    }
    //攻击范围改变类型
    public enum ENARChangeType
    {
        enNone,
        enLarge,//变大
        enSmall,//变小
    }
    private ENARChangeType m_arChangeType = ENARChangeType.enNone;
    public ENARChangeType AttackRangeChangeType
    {
        get
        {
            return m_arChangeType;
        }
        set
        {
            if (m_arChangeType != value)
            {
                if (value == ENARChangeType.enSmall && m_attackRange == CurrentTableInfo.AttackRange)
                {//已经最大
                    ;
                }
                else if (value == ENARChangeType.enLarge && m_attackRange == CurrentTableInfo.AlertRange)
                {//已经最小
                    ;
                }
                else
                {
                    m_arChangeStartTime = Time.time;
                    m_arChangeType = value;
                }
            }
        }
    }
    //攻击范围改变类型开始的时间
    private float m_arChangeStartTime = 0;
    #endregion

	public List<Vector3> m_pathNodeList = new List<Vector3>();
	public NPC(ActorType type, int id, int staticID,CSItemGuid guid)
		: base(type, id, staticID,guid)
	{
        IsActive = true;
		m_defaultVelocity = 4.0f;
        Props = ENProperty.Singleton.CreatePropertySet("SOBNpc");
        SetPropertyObjectID((int)MVCPropertyID.enActorStartID + id);

        IDInTable = staticID;
        OnInitProps();
	}
    public override void OnInitProps()
	{
        base.OnInitProps();
		CurrentTableInfo = GameTable.NPCInfoTableAsset.Lookup(IDInTable);
		if (CurrentTableInfo == null) return;

		if (GetNpcType() == ENNpcType.enBOSSNPC)
		{//boss
			SelfAI = new AINpcBoss(m_npcInfo.BossAIXmlName, m_npcInfo.BossAIXmlSubName);
		}
		else
		{
			if ((ENNPCAIType)m_npcInfo.AIType == ENNPCAIType.enAITypeNormal)
			{
				SelfAI = new AINpcLong();
			}
			else if ((ENNPCAIType)m_npcInfo.AIType == ENNPCAIType.enAITypeBlock)
			{
				SelfAI = new AINpcBlock();
			}
		}
		SelfAI.Owner = this;

		if (GetNpcType() == ENNpcType.enBOSSNPC)
		{//显示boss硬直，测试用
			UIPauseBtn.Singleton.Register(ID);
		}

		//添加buff
		if (m_npcInfo.GiftBuffIDList != null && m_npcInfo.GiftBuffIDList.Count != 0)
		{
            IResult r = BattleFactory.Singleton.CreateResult(ENResult.AddBuff, ID, ID, 0, 0, m_npcInfo.GiftBuffIDList.ToArray());
			if (r != null)
			{
				r.ResultExpr(m_npcInfo.GiftBuffIDList.ToArray());
				BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
			}

		}

        Props.SetProperty_Int32(ENProperty.islive, 1);
        SM.MonsterData mCurRoomMonsterData = null;
        if (roomElement != null && roomElement.MonsterData != null)
        {
            mCurRoomMonsterData = SM.RandomRoomLevel.Singleton.LookupMonsterData(roomElement.MonsterData.monsterObjId);
        }

        if (mCurRoomMonsterData != null && mCurRoomMonsterData.HP != -1)
        {
            Props.SetProperty_Float(ENProperty.maxhp, mCurRoomMonsterData.HP);
            Props.SetProperty_Float(ENProperty.hp, mCurRoomMonsterData.HP);
        }
        else
        {
            Props.SetProperty_Float(ENProperty.maxhp, m_npcInfo.HPMax);
            Props.SetProperty_Float(ENProperty.hp, m_npcInfo.HPMax);
        }
        if (mCurRoomMonsterData != null && mCurRoomMonsterData.PhyAtk != -1)
        {
            Props.SetProperty_Float(ENProperty.phyattack, mCurRoomMonsterData.PhyAtk);
        }
        else
        {
            Props.SetProperty_Float(ENProperty.phyattack, m_npcInfo.PhyAttack);
        }
        if (mCurRoomMonsterData != null && mCurRoomMonsterData.PhyDef != -1)
        {
            Props.SetProperty_Float(ENProperty.phydefend, mCurRoomMonsterData.PhyDef);
        }
        else
        {
            Props.SetProperty_Float(ENProperty.phydefend, m_npcInfo.PhyDefend);
        }
        if (mCurRoomMonsterData != null && mCurRoomMonsterData.MagAtk != -1)
        {
            Props.SetProperty_Float(ENProperty.magattack, mCurRoomMonsterData.MagAtk);
        }
        else
        {
            Props.SetProperty_Float(ENProperty.magattack, m_npcInfo.MagAttack);
        }
        if (mCurRoomMonsterData != null && mCurRoomMonsterData.MagDef != -1)
        {
            Props.SetProperty_Float(ENProperty.magdefend, mCurRoomMonsterData.MagDef);
        }
        else
        {
            Props.SetProperty_Float(ENProperty.magdefend, m_npcInfo.MagDefend);
        }
        PropertyValueIntListView skillIDList = Props.GetProperty_Custom(ENProperty.SkillIDList) as PropertyValueIntListView;
        //if (mCurRoomMonsterData != null)
        {
            int count = 0;
            if (mCurRoomMonsterData != null && mCurRoomMonsterData.SkillList != null)
            {
                for (int i = 0; i < mCurRoomMonsterData.SkillList.Count; ++i)
                {
                    skillIDList.m_list[i] = mCurRoomMonsterData.SkillList[i];
                    ++count;
                }
            }
            else
            {
                for (int i = 0; i < m_npcInfo.SkillList.Count; i++)
                {
                    skillIDList.m_list[i] = m_npcInfo.SkillList[i];
                    ++count;
                }
            }
            if (mCurRoomMonsterData != null && mCurRoomMonsterData.PassiveSkillList != null)
            {
                for (int i = 0; i < mCurRoomMonsterData.PassiveSkillList.Count; ++i)
                {
                    skillIDList.m_list[i + count] = mCurRoomMonsterData.PassiveSkillList[i];
                }
            }
            else
            {
                for (int i = 0; i < m_npcInfo.PassiveSkillList.Count; i++)
                {
                    skillIDList.m_list[i + count] = m_npcInfo.PassiveSkillList[i];
                }
            }
        }
        Props.SetProperty_Custom(ENProperty.SkillIDList, skillIDList);

        //设置NPCType
        if (mCurRoomMonsterData != null && mCurRoomMonsterData.MonsterType != -1)
        {
            Props.SetProperty_Int32(ENProperty.NpcType, (int)mCurRoomMonsterData.MonsterType);
        }
        else
        {
            Props.SetProperty_Int32(ENProperty.NpcType, (int)m_npcInfo.Type);
        }

        //设置模型比例
        if (mCurRoomMonsterData != null && mCurRoomMonsterData.ChangeScale != -1)
        {
            ModelScale = mCurRoomMonsterData.ChangeScale;
        }
        else
        {
            ModelScale = m_npcInfo.ModelScale;
        }

        Props.SetProperty_Float(ENProperty.avoid, m_npcInfo.Avoid);
        Props.SetProperty_Float(ENProperty.hit, m_npcInfo.HitRate);
        Props.SetProperty_Float(ENProperty.crit, m_npcInfo.CritRate);
        Props.SetProperty_Float(ENProperty.critParam, m_npcInfo.CritParam);
        Props.SetProperty_Float(ENProperty.runSpeed, m_npcInfo.MoveSpeed);
        Props.SetProperty_Float(ENProperty.stamina, (float)m_npcInfo.StaminaMax);
        Props.SetProperty_Float(ENProperty.maxStamina, (float)m_npcInfo.StaminaMax);
        Props.SetProperty_Float(ENProperty.FResist, m_npcInfo.Resist);
        Props.SetProperty_Float(ENProperty.AnitInterfere, m_npcInfo.AnitInterfereRate);
        Props.SetProperty_Float(ENProperty.AnitInterrupt, m_npcInfo.AnitInterruptRate);
        Props.SetProperty_Float(ENProperty.AnitRepel, m_npcInfo.AnitRepelRate);
        Props.SetProperty_Float(ENProperty.AnitLauncher, m_npcInfo.AnitLauncherRate);
        Props.SetProperty_Float(ENProperty.WoundParam, m_npcInfo.WoundParam);
        Props.SetProperty_Float(ENProperty.MovebackSpeed, m_npcInfo.RetreatSpeed);
        Props.SetProperty_Float(ENProperty.AnimationSpeed, m_npcInfo.AnimationSpeed);
        Props.SetProperty_Int32(ENProperty.WeaponID, m_npcInfo.WeaponID);
        AttackAnimSpeed = 1;
    }
    private GameObject BodyObject {get; set;}
    public void SetBodyObject(GameObject obj)
    {
        BodyObject = obj;
    }
    // true: 不可以被攻击
    public bool CanNotBeAttack()
    {
        if (m_npcInfo.Type == (int)ENNpcType.enBlockNPC || m_npcInfo.Type == (int)ENNpcType.enFunctionNPC)
        {
            return true;
        }
        return false;
    }
    public ENNpcType GetNpcType()
    {
        return (ENNpcType)Props.GetProperty_Int32(ENProperty.NpcType);
    }

    public ENNpcInterSubType GetNpcInterSubType()
    {
        return (ENNpcInterSubType)m_npcInfo.InterSubType;
    }
    public override IEnumerator Coroutine_LoadBody(string modelFile, int modelID, float modelScale, GameResPackage.AsyncLoadObjectData loadData)
    {
        if (null != roomElement)
        {
            if (roomElement.IsUsePrefabLoad)
            {
                BodyObject.transform.localScale = new Vector3(modelScale, modelScale, modelScale);
                loadData.m_obj = BodyObject;
                loadData.m_isFinish = true;
            }
        }
        if (!loadData.m_isFinish)
        {
            IEnumerator e = base.Coroutine_LoadBody(modelFile, modelID, modelScale, loadData);
            while (true)
            {
                e.MoveNext();
                if (loadData.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
        }
    }
	protected override void OnLoaded(float modelScale, int modelID)
    {
		base.OnLoaded(modelScale, modelID);

        MainObj.GetComponent<Rigidbody>().useGravity = !m_npcInfo.IsPush;
        MainObj.GetComponent<Rigidbody>().isKinematic = !m_npcInfo.IsPush;

        LoadedByChangeBodyElement();
        //AddHPBar();         // 只有副本里的NPC才会显示血条
        //UpdateHeadIcon();   // 头顶图标
        //InitHeadWarnTip();
        MainGame.Singleton.StartCoroutine(InitHeadWarnTip());
        //test
        TestBossStamina();

        MainGame.Singleton.StartCoroutine(Coroutine_LoadTestRange());
    }
    public override void LoadedByChangeBodyElement()
    {
        Transform bodyTrans = GetBodyObject().transform;
        for (int i = 0; i < bodyTrans.childCount; i++)
        {
            Transform child = bodyTrans.GetChild(i);
            if (null != child.GetComponent<Renderer>())
            {
                NpcBehaviour = child.gameObject.GetComponent<NPCBehaviour>();
                if (NpcBehaviour == null)
                {
                    NpcBehaviour = child.gameObject.AddComponent<NPCBehaviour>();
                }
                NpcBehaviour.CurrentActor = this;
                break;
            }
        }
        GameObject objAP = GetObject_AdherentPoints();
        if (null != objAP)
        {
            Transform tAdherentPoints = objAP.transform;
            HPBarTrans = tAdherentPoints.Find("HPbarPoint");
            
            WarningTipTrans = tAdherentPoints.Find("WarningPoint");
        }
        if (m_npcInfo.Type == (int)ENNpcType.enBlockNPC)
        {
            if (CenterPart != null)
            {
                CenterPart.layer = LayerMask.NameToLayer("Default");
            }
        }
        if (m_npcInfo.Type == (int)ENNpcType.enBOSSNPC)
        {//boss血条
            m_uiBossBloodBar = UIBossBloodBar.Singleton;
            m_uiBossBloodBar.Register(this.ID);
            m_uiBossBloodBar.HideWindow();
        }
    }
    IEnumerator Coroutine_LoadTestRange()
    {// 显示范围资源
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath("ef-EA"), data);
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
            m_objAttackRange = data.m_obj as GameObject;
            m_objAttackRange.transform.parent = MainObj.transform;
            m_objAttackRange.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            m_objAttackRange.transform.localScale = new Vector3(AttackRange * 2, 0f, AttackRange * 2);
        }

        data.Reset();
        e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath("ef-EW"), data);
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
            m_objAlertRange = data.m_obj as GameObject;
            m_objAlertRange.transform.parent = MainObj.transform;
            m_objAlertRange.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            m_objAlertRange.transform.localScale = new Vector3(m_npcInfo.AlertRange * 2, 0f, m_npcInfo.AlertRange * 2);
        }
	}

    public void SetMonsterProps(int propType, int val)
    {
        if (val != -1)
        {
            Props.SetProperty_Int32(propType, val);
        }
        else
        {
            Props.SetProperty_Float(ENProperty.maxhp, m_npcInfo.HPMax);
        }
        int cc = Props.GetProperty_Int32(propType);
        //Debug.Log("MonsterID:" + m_dynamicIDSeed + "  " + propType + "   " + cc);
    }

    public override bool CreateNeedModels()
	{
        if (MainObj != null)
        {
            return true;
        }
		if (m_npcInfo == null)
		{
			Debug.LogError("npc CreateNeedModels fail, id=" + ID + ",tableid=" + IDInTable);
			return false;
		}
        ENNpcType npcType = (ENNpcType)m_npcInfo.Type;
        string npcPrefab = "Model/EnemyB";
        switch (npcType)
        {
            case ENNpcType.enBlockNPC:
            case ENNpcType.enFunctionNPC:
            case ENNpcType.enBoxNPC:
                npcPrefab = "Model/EnemyA";
                break;
            case ENNpcType.enBOSSNPC:
                npcPrefab = "Model/EnemyBoss";
                BaseMass = 100;
                break;
            case ENNpcType.enCommonNPC:
            break;
        }

		if (CanNotBeAttack())
		{
            Load(npcPrefab, m_npcInfo.ModelId, m_npcInfo.ModelScale, WeaponType);
			ActionControl.AddDisableCount(ActorAction.ENType.enSpasticityAction);
		}
		else
		{
            Load(npcPrefab, m_npcInfo.ModelId, m_npcInfo.ModelScale, WeaponType);
        }
		return true;
	}

	#region 行为
	public override void DefaultAction()
	{
        ActionControl.AddAction(ActorAction.ENType.enStandAction);
	}
    public override void BeHited(Actor srcActor, bool isBack, bool isFly, float animGauge)
	{
        base.BeHited(srcActor, isBack, isFly, animGauge);
		{
			SpasticityAction action = ActionControl.AddAction(ActorAction.ENType.enSpasticityAction) as SpasticityAction;
			if (null != action)
			{
				action.LastTime = 0.50f;
			}
		}
	}
	#endregion

    static void NtfDestroy(ref GameObject obj)
    {
        if (obj != null)
            GameObject.Destroy(obj);
        obj = null;
    }
    public override void Destroy()
    {
        NpcBehaviour.CurrentActor = null;
        MainGame.Singleton.StartCoroutine(BeginDisappear());
    }
    public void DestroyImpl()
    {
        PoolManager.Singleton.ReleaseObj(mHPBarObj);
        //NtfDestroy(ref mHPBarObj);
        //NtfDestroy(ref mHeadIcon);
        PoolManager.Singleton.ReleaseObj(mHeadWarningTip);
        PoolManager.Singleton.ReleaseObj(mHeadDoubleWarnTip);
        PoolManager.Singleton.ReleaseObj(m_headWarningChangeTip);
        //NtfDestroy(ref mHeadWarningTip);
        //NtfDestroy(ref mHeadDoubleWarnTip);
        //NtfDestroy(ref m_headWarningChangeTip);
        if (m_uiBossBloodBar != null)
        {
            m_uiBossBloodBar.Destroy();
            m_uiBossBloodBar = null;
        }
        PoolManager.Singleton.ReleaseObj(m_objAttackRange);
        PoolManager.Singleton.ReleaseObj(m_objAlertRange);
        base.Destroy();
    }
    public IEnumerator BeginDisappear()
    {
        if (null != this)
        {
            float disTime = 1;
//             if(MainObj.active)
//             {
//                 MainObj.SetActive(false);
//             }
            GameObject shadowObj = ShadowObj;
            if (mHPBarObj != null)
            {
                //UIHPBar hpBar = mHPBarObj.GetComponentInChildren<UIHPBar>();
                //disTime = hpBar.StartDisappear();
            }
            float t = disTime;

            while (t >= 0.0f && shadowObj != null)
            {
                float s = t / disTime;
                //float distance = Time.deltaTime* 1.50f / disTime;
                //MainObj.transform.localPosition = new Vector3(MainObj.transform.localPosition.x
                //    , MainObj.transform.localPosition.y - distance
                //    , MainObj.transform.localPosition.z);
                shadowObj.transform.localScale = new UnityEngine.Vector3(s, s, s);
                yield return 0;
                t = t - Time.deltaTime;
            }
            DestroyImpl();
        }
    }

    public override void AddHPBar()
    {
        if (m_npcInfo.Type == (int)ENNpcType.enBoxNPC)
        {
            return;
        }
        if(!CanNotBeAttack())
        {
            if (m_npcInfo.Type != (int)ENNpcType.enBOSSNPC)
            {
                string uiName = "UIBloodBar";
                if (m_npcInfo.Type == (int)ENNpcType.enEliteNPC)
                {
                    uiName = "UIEliteBloodBar";
                }
                PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetUIPath(uiName), LoadBloodCallback);
                 //mHPBarObj = GameObject.Instantiate(GameData.LoadPrefab<GameObject>(uiName)) as GameObject;
                //mHPBarObj.name = "Bar_" + Type.ToString() + "_" + ID.ToString();
                //mHPBarObj.transform.parent = HPBarTrans.transform;
                //mHPBarObj.transform.localPosition = Vector3.zero;
            }
        }
    }
    void LoadBloodCallback(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            mHPBarObj = obj as GameObject;
            mHPBarObj.name = "Bar_" + Type.ToString() + "_" + ID.ToString();
            mHPBarObj.transform.parent = HPBarTrans.transform;
            mHPBarObj.transform.localPosition = Vector3.zero;
        }
    }
    public override void UpdateHPBar()
    {
		base.UpdateHPBar();
    }

    //GameObject mHeadIcon = null;
    //void AddHeadIcon()
    //{
    //    if ((null == mHeadIcon) && CanNotBeAttack())
    //    {
    //        GameObject grp = GameObject.Find("FlagGrp");
    //        if (grp == null)
    //        {
    //            grp = new GameObject("FlagGrp");
    //            grp.transform.position = Vector3.zero;
    //            grp.transform.parent = MainGame.Singleton.MainObject.transform;
    //        }

    //        float fsl = 0.01f;
    //        Vector3 vpos = this.RealPos;
    //        vpos.y += 1.0f;
    //        mHeadIcon = GameObject.Instantiate(GameData.LoadPrefab<GameObject>("UI/UINpcHead")) as GameObject;
    //        mHeadIcon.name = "Icn_" + Type.ToString() + "_" + ID.ToString();
    //        mHeadIcon.transform.parent = grp.transform;
    //        mHeadIcon.transform.position = vpos;
    //        mHeadIcon.transform.localScale = new Vector3(fsl, fsl, fsl);
    //    }
    //}

    // 更新NPC头顶标记
    //public void UpdateHeadIcon()
    //{
    //    string sAtlas = "";

    //    // 没有任务的情况下处理其它标记
    //    if (string.IsNullOrEmpty(sAtlas))
    //    {
    //        if (m_npcInfo.HasFunc((int)NpcFuncType.NPC_SHOP))
    //        {
    //            sAtlas = "i-interface-symbol-shangdian-D-0";
    //        }
    //    }
    //    // 装备强化
    //    if (string.IsNullOrEmpty(sAtlas))
    //    {
    //        if (m_npcInfo.HasFunc((int)NpcFuncType.NPC_EQUIP))
    //        {
    //            sAtlas = "qianghua";
    //        }
    //    }

    //    if (!string.IsNullOrEmpty(sAtlas))
    //    {
    //        AddHeadIcon();
    //        UIAtlas ua = GameData.LoadUIAtlas("symbol");
    //        if (ua != null)
    //        {
    //            Transform ch = mHeadIcon.transform.FindChild("HeadIcon");
    //            if (ch != null)
    //            {
    //                UISprite spt = ch.gameObject.GetComponent<UISprite>();
    //                spt.atlas = ua;
    //                spt.spriteName = sAtlas;
    //                return;
    //            }
    //        }
    //     }
    //    if (mHeadIcon != null)
    //    {
    //        GameObject.Destroy(mHeadIcon);
    //        mHeadIcon = null;
    //    }
    //}
    public override void Update()
    {
        base.Update();
        //if (mHeadIcon != null)
        //{
        //    Vector3 vpos = this.RealPos;
        //    vpos.y += 1.0f;
        //    vpos.x += 0.3f;
        //    mHeadIcon.transform.position = vpos;
        //    MainGame.FaceToCamera(mHeadIcon);
        //}
		if ((mHeadWarningTip != null) && (mHeadWarningTip.activeSelf))
        {
            MainGame.FaceToCamera(mHeadWarningTip);
		}
        if ((mHeadDoubleWarnTip != null) && (mHeadDoubleWarnTip.activeSelf))
        {
            MainGame.FaceToCamera(mHeadDoubleWarnTip);
        }
        if ((m_headWarningChangeTip != null) && (m_headWarningChangeTip.activeSelf))
        {
            MainGame.FaceToCamera(m_headWarningChangeTip);
        }
    }
    GameObject mHeadWarningTip;
	GameObject mHeadDoubleWarnTip;
    GameObject m_headWarningChangeTip = null;
    IEnumerator InitHeadWarnTip()
    {
        float fScale = 1.0f;
        if (null != ActorCfgData)
        {
            fScale = ActorCfgData.m_warningTipsScale;
        }
        if (null != WarningTipTrans)
        {
            {
                GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
                IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetUIPath("ef-e-monsteralert-E03"), data);
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
                    mHeadWarningTip = data.m_obj as GameObject;//GameObject.Instantiate(GameData.LoadPrefab<GameObject>("UI/ef-e-monsteralert-E03")) as GameObject;
                    mHeadWarningTip.name = "Tip_" + Type.ToString() + "_" + ID.ToString();
                    mHeadWarningTip.transform.parent = WarningTipTrans.transform;
                    mHeadWarningTip.transform.localScale = new Vector3(fScale, fScale, fScale);
                    mHeadWarningTip.transform.localPosition = Vector3.zero;
                }
            }

            {
                GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
                IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetUIPath("ef-e-monsteralert-E04"), data);
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
                    mHeadDoubleWarnTip = data.m_obj as GameObject;//GameObject.Instantiate(GameData.LoadPrefab<GameObject>("UI/ef-e-monsteralert-E04")) as GameObject;
                    mHeadDoubleWarnTip.name = "TipDouble_" + Type.ToString() + "_" + ID.ToString();
                    mHeadDoubleWarnTip.transform.parent = WarningTipTrans.transform;
                    mHeadDoubleWarnTip.transform.localScale = new Vector3(fScale, fScale, fScale);
                    mHeadDoubleWarnTip.transform.localPosition = Vector3.zero;

                    //警告动画时长
                    mHeadDoubleWarnTip.SetActive(true);
                    Animation anim = mHeadDoubleWarnTip.GetComponentInChildren<Animation>();
                    if (anim != null)
                    {
                        if (anim.clip != null)
                        {
                            m_doubleWarnAnimLength = anim.clip.length;
                        }
                    }
                    mHeadDoubleWarnTip.SetActive(false);
                }
            }

            {
                GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
                IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetUIPath("ef-e-monsteralert-E05"), data);
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
                    m_headWarningChangeTip = data.m_obj as GameObject;//GameObject.Instantiate(GameData.LoadPrefab<GameObject>("UI/ef-e-monsteralert-E05")) as GameObject;
                    m_headWarningChangeTip.name = "TipChange_" + Type.ToString() + "_" + ID.ToString();
                    m_headWarningChangeTip.transform.parent = WarningTipTrans.transform;
                    m_headWarningChangeTip.transform.localScale = new Vector3(fScale, fScale, fScale);
                    m_headWarningChangeTip.transform.localPosition = Vector3.zero;
                    m_changeMaterial = m_headWarningChangeTip.GetComponent<ChangeMaterial>();
                    if (m_changeMaterial == null)
                    {
                        m_headWarningChangeTip.SetActive(true);
                        m_changeMaterial = m_headWarningChangeTip.GetComponentInChildren<ChangeMaterial>();
                        m_headWarningChangeTip.SetActive(false);
                    }
                }
            }
        }
    }
    float m_warnAnimLength = 0;
    float m_doubleWarnAnimLength = 0;
    ChangeMaterial m_changeMaterial = null;
	public float ShowHeadWarnTip()
	{
		mHeadWarningTip.SetActive(true);
        return m_warnAnimLength;
	}
    public float ShowDoubleWarnTip()
    {
        mHeadWarningTip.SetActive(false);
        m_headWarningChangeTip.SetActive(false);
        mHeadDoubleWarnTip.SetActive(true);
        return m_doubleWarnAnimLength;
    }
	public void HideWarnTip()
	{
		mHeadWarningTip.SetActive(false);
		mHeadDoubleWarnTip.SetActive(false);
        m_headWarningChangeTip.SetActive(false);
	}
    //改变头顶叹号颜色
    public void ChangeHeadWarnTip()
    {
        float change = AttackRange - CurrentTableInfo.AttackRange;
        if (change == 0)
        {
            return;
        }
        float percent = (change) / (CurrentTableInfo.AlertRange - CurrentTableInfo.AttackRange);

        mHeadWarningTip.SetActive(false);
        mHeadDoubleWarnTip.SetActive(false);
        m_headWarningChangeTip.SetActive(true);

        if (m_changeMaterial != null)
        {
            m_changeMaterial.Percent = percent;
        }
    }
    bool m_isShowBossBloodBar = false;
    //isRealShow：主控角色目标是自己，一直NotifyChanged
    public void ShowBossBloodBar(bool isRealShow = false)
    {
        if (this.GetNpcType() != ENNpcType.enBOSSNPC)
        {
            return;
        }
        Actor actor = ActorManager.Singleton.MainActor.TargetManager.CurrentTarget;
        if (actor != null)
        {//主控角色有目标
            if (actor.Type == ActorType.enNPC)
            {//主控角色的目标是NPC
                NPC npc = actor as NPC;
                if (npc.CurrentTableInfo.Type == (int)ENNpcType.enBOSSNPC)
                {//主控角色的目标是boss
                    if (this == npc && m_isShowBossBloodBar)
                    {//角色目标是自己，自己已经显示
                        if (!isRealShow)
                        {
                            return;
                        }
                    }
                    else if (this != npc && !m_isShowBossBloodBar)
                    {//角色目标不是自己，自己没有显示
                        return;
                    }
                    m_isShowBossBloodBar = (this == npc);
                }
            }
        }
        else
        {//主控角色没有目标
            m_isShowBossBloodBar = true;
        }
        NotifyChanged((int)ENPropertyChanged.enBossBloodBar, m_isShowBossBloodBar);
    }
    public override void OnStaminaEvent(Actor source, out bool isFly)
    {
        isFly = false;
        //小于0时恢复百分比
        float curStamina = (float)m_npcInfo.StaminaMax * m_npcInfo.StaminaReset;
        this.Props.SetProperty_Float(ENProperty.stamina, curStamina);
        TestBossStamina();
        //Debug.LogWarning("破韧小于0时恢复百分比 cur value=" + curStamina.ToString() + "。 target id==" + target.ID.ToString());
        switch ((ENStaminaEventType)m_npcInfo.StaminaEvent)
        {
            case ENStaminaEventType.enBeHitFly:
                {//被击飞
                    isFly = true;
                }
                break;
            case ENStaminaEventType.enFireSkill:
                {//释放技能
                    //this.OnFireSkill(m_npcInfo.StaminaSkillID);
                    this.SelfAI.GetBaseData("StaminaEvent", AIBaseData.DataType.enBool).SetValue(true);
                }
                break;
        }
    }
    public override void OnHpChanged(int hp, bool isCrit, float multiple, bool isHeal)
    {

        this.ShowBossBloodBar(true);
        base.OnHpChanged(hp, isCrit, multiple, isHeal);
    }

    public override bool OnFireSkill(int skillID)
    {
        if (IsDead)
        {
            return false;
        }
        //npc的技能不是都在技能背包中（比如：触怒技）
        SkillInfo info = GameTable.SkillTableAsset.Lookup(skillID);
        if (info == null)
        {
            return false;
        }
        if (IsSkillCDRunning(skillID))
        {//cd中
            return false;
        }
        int targetID = 0;
        if (null != TargetManager.CurrentTarget)
        {
            targetID = TargetManager.CurrentTarget.ID;
        }
        this.CurrentCmd = new NPC.Cmd(skillID, targetID);
        return true;
    }

    public void ShowAttackRange()
    {
        if (m_objAttackRange == null)
        {
            return;
        }

        if (m_objAttackRange.activeSelf)
        {
            m_objAttackRange.SetActive(false);
        }
        else
        {
            m_objAttackRange.SetActive(true);
            m_objAttackRange.transform.localScale = new Vector3(AttackRange * 2, 0f, AttackRange * 2);
        }
    }

    public void ShowAlertRange()
    {
        if (m_objAlertRange == null)
        {
            return;
        }

        if (m_objAlertRange.activeSelf)
        {
            m_objAlertRange.SetActive(false);
        }
        else
        {
            m_objAlertRange.SetActive(true);
        }
    }
    private float m_staminaRestoreTime = Time.time;
    public override void FixedUpdate()
    {
        if (!m_isLoaded)
        {
            return;
        }
        if (MainAnim != null)
        {
            if (IsState_Fight)
            {//战斗时和非战斗时的animation的cullingType是不一样的
                MainAnim.cullingType = AnimationCullingType.AlwaysAnimate;
            }
            else
            {
                MainAnim.cullingType = AnimationCullingType.BasedOnRenderers;
            }
        }
        base.FixedUpdate();
        {//强韧度回复
            float curStamina = this.Props.GetProperty_Float(ENProperty.stamina);
            float maxStamina = this.Props.GetProperty_Float(ENProperty.maxStamina);
            if (curStamina < maxStamina)
            {
                float now = Time.time;
                if (now - m_staminaRestoreTime >= CurrentTableInfo.StaminaRestorePeriod)
                {//恢复
                    m_staminaRestoreTime = now;
                    curStamina += CurrentTableInfo.StaminaRestoreValue;
                    if (curStamina > maxStamina)
                    {
                        curStamina = maxStamina;
                    }
                    if (curStamina < 0)
                    {
                        curStamina = 0;
                    }
                }
                this.Props.SetProperty_Float(ENProperty.stamina, curStamina);
                TestBossStamina();
            }
        }
        if (m_objAttackRange.activeSelf)
        {
            m_objAttackRange.transform.localScale = new Vector3(AttackRange * 2, 0f, AttackRange * 2);
        }
        
    }
    public void TestBossStamina()
    {
        if (GetNpcType() == ENNpcType.enBOSSNPC)
        {//显示boss硬直，测试用
            this.NotifyChanged((int)ENPropertyChanged.enPauseBtn, null);
        }
    }
    public override bool IsCanSelected()
    {
        return IsActive;
    }
    //获取种族类型
    public override int OnGetRaceType()
    {
        return CurrentTableInfo.RaceType;
    }

    public override void OnBecameInvisible()
    {
        base.OnBecameInvisible();
        if (this.IsRealDead)
        {
            ActorManager.Singleton.ReleaseActor(ID);
        }
    }

    public override int GetActorInitModelID()
    {
        return CurrentTableInfo.ModelId;
    }
    public override int GetActorInitWeaponID()
    {
        return CurrentTableInfo.WeaponID;
    }
    public override void OnInitSkillBag(bool isNotifyUI = false)
	{
		if (SkillBag == null)
		{
			SkillBag = new List<ActorSkillInfo>();
		}
		SkillBag.Clear();
        base.OnInitSkillBag(isNotifyUI);
        //luozhijia test
        //skill bag
        //PropertyValueIntListView tmpPropVal = Props.GetProperty_Custom(ENProperty.SkillList) as PropertyValueIntListView;

        //foreach (var item in tmpPropVal.m_list)
        //{
        //    SkillInfo info = GameTable.SkillTableAsset.Lookup(item);
        //    if (info != null)
        //    {
        //        SkillBag.Add(new ActorSkillInfo(info));
        //    }
        //}
        
        ////被动技能
        //tmpPropVal = Props.GetProperty_Custom(ENProperty.PassiveSkillList) as PropertyValueIntListView;
        //foreach (var item in tmpPropVal.m_list)
        //{
        //    SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(item);
        //    if (skillInfo == null) continue;
        //    if (skillInfo.SkillType == (int)ENSkillType.enPassive)
        //    {
        //        IResult r = BattleFactory.Singleton.CreateResult(ENResult.AddBuff, ID, ID);
        //        if (r != null)
        //        {
        //            r.ResultExpr(skillInfo.BuffIDList.ToArray());
        //            BattleFactory.Singleton.DispatchResult(r);
        //        }
        //    }
        //}
    }
}