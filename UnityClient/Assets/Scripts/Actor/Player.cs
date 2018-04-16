using System;
using System.Collections.Generic;
using System.Collections;	// IEnumerator
using UnityEngine;


public class Player : Actor
{
    //战斗意志
    public WillToFight WTF { get; private set; }
    //
    HeroInfo m_heroInfo = null;
    public HeroInfo CurrentTableInfo { get { return m_heroInfo; } private set { m_heroInfo = value; } }
	//普攻列表
	public List<int> NormalSkillList { get { return CurrentTableInfo.NormalSkillIDList; } }
    public float GetUnlockGazingRange { get { return m_heroInfo.UnlockGazingRange; } private set{} }
	public Player(ActorType type, int id, int staticID, CSItemGuid guid)
        : base(type, id, staticID, guid)
    {
        if (type == ActorType.enSwitch)
        {
            SelfAI = new AISwitchPlayer();
            SelfAI.Owner = this;
            Combo = BattleArena.Singleton.Combo;
        }
        else if (type == ActorType.enFollow)
        {
            SelfAI = new AIPartner();
            SelfAI.Owner = this;

            Combo = null;//BattleArena.Singleton.Combo;

            WTF = new WillToFight(this);
        }
        else
        {
            SelfAI = new AIServerActor();
            SelfAI.Owner = this;
        }
        m_defaultVelocity = 5.0f;
        Props = ENProperty.Singleton.CreatePropertySet("SOBPlayer");
        SetPropertyObjectID((int)(MVCPropertyID.enActorStartID) + id);

        IDInTable = staticID;
        OnInitProps();
	}
    public override void OnInitProps()
    {
 	    base.OnInitProps();
        CurrentTableInfo = GameTable.HeroInfoTableAsset.Lookup(IDInTable);
        if (CurrentTableInfo == null)
        {
            return;
        }
        if (0 == Props.GetProperty_Int32(ENProperty.vocationid))
        {
            Props.SetProperty_Int32(ENProperty.vocationid, 1);
        }
        //显示玩家名称
        AddPlayerName();

        HP = CurrentTableInfo.FHPMax;
        MaxHP = CurrentTableInfo.FHPMax;
        MoveSpeed = CurrentTableInfo.MoveSpeed;
        MovebackSpeed = CurrentTableInfo.MovebackSpeed;
        AnimationSpeed = CurrentTableInfo.AnimationSpeed;
        AttackAnimSpeed = 1;
        Props.SetProperty_Int32(ENProperty.islive, 1);
        Props.SetProperty_Float(ENProperty.phyattack, (int)CurrentTableInfo.FPhyAttack);
        Props.SetProperty_Float(ENProperty.phydefend, (int)CurrentTableInfo.FPhyDefend);
        Props.SetProperty_Float(ENProperty.magattack, (int)CurrentTableInfo.FMagAttack);
        Props.SetProperty_Float(ENProperty.magdefend, (int)CurrentTableInfo.FMagDefend);
        Props.SetProperty_Float(ENProperty.avoid, (int)CurrentTableInfo.FAvoid);
        Props.SetProperty_Float(ENProperty.hit, (int)CurrentTableInfo.HitRate);
        Props.SetProperty_Float(ENProperty.crit, (int)CurrentTableInfo.CritRate);
        Props.SetProperty_Float(ENProperty.critParam, (int)CurrentTableInfo.CritParam);
        Props.SetProperty_Float(ENProperty.stamina, (float)CurrentTableInfo.StaminaMax);
        Props.SetProperty_Float(ENProperty.maxStamina, (float)CurrentTableInfo.StaminaMax);
        Props.SetProperty_Float(ENProperty.FResist, (float)CurrentTableInfo.FResist);
        Props.SetProperty_Float(ENProperty.AnitInterfere, (float)CurrentTableInfo.AnitInterfereRate);
        Props.SetProperty_Float(ENProperty.AnitInterrupt, (float)CurrentTableInfo.AnitInterruptRate);
        Props.SetProperty_Float(ENProperty.AnitRepel, (float)CurrentTableInfo.AnitRepelRate);
        Props.SetProperty_Float(ENProperty.AnitLauncher, (float)CurrentTableInfo.AnitLauncherRate);
        Props.SetProperty_Float(ENProperty.WoundParam, CurrentTableInfo.WoundParam);
        Props.SetProperty_Int32(ENProperty.WeaponID, CurrentTableInfo.WeaponId);
        Props.SetProperty_Float(ENProperty.ModelScale, CurrentTableInfo.ModelScale);

        CSItem card = CardBag.Singleton.GetCardByGuid(GUID);
        if (card == null)
        {
            if (Type == ActorType.enFollow)
            {
                card = Team.Singleton.Comrade;
            }
        }
        if (card != null)
        {
            MaxHP = card.GetHp();
            HP = MaxHP;
            Level = card.Level;
            Props.SetProperty_Float(ENProperty.phyattack, card.GetPhyAttack());
            Props.SetProperty_Float(ENProperty.magattack, card.GetMagAttack());
            Props.SetProperty_Float(ENProperty.phydefend, card.GetPhyDefend());
            Props.SetProperty_Float(ENProperty.magdefend, card.GetMagDefend());

            PropertyCustomValue value = Props.GetProperty_Custom(ENProperty.SkillIDList);
            if (value == null)
            {
                Debug.LogWarning("ENProperty.SkillIDList is null");
                return;
            }
            PropertyValueIntListView skillIDList = value as PropertyValueIntListView;
            if (skillIDList == null)
            {
                Debug.LogWarning("ENProperty.SkillIDList cast fail");
                return;
            }
            for (int i = 0; i < card.SkillItemInfoList.Length; ++i)
            {
                skillIDList.m_list[i] = card.SkillItemInfoList[i].m_skillID;
            }
            Props.SetProperty_Custom(ENProperty.SkillIDList, skillIDList);
        }
    }
    public void InitSwitchSkillUI()
    {
        SkillBag.Clear();
        SkillBag.Capacity = 1;
        CSItem card = CardBag.Singleton.GetCardByGuid(GUID);
        SkillInfo info = GameTable.SkillTableAsset.Lookup(card.SwitchSkillID);
        if (info != null)
        {
            SkillBag.Add(new ActorSkillInfo(info, card.SwitchSkillLevel));
        }
        foreach (var item in card.SkillItemInfoList)
        {
            SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(item.m_skillID);
            if (skillInfo == null) continue;
            if (skillInfo.SkillType == (int)ENSkillType.enPassive)
            {//被动技能
                IResult r = BattleFactory.Singleton.CreateResult(ENResult.AddBuff, ID, ID, 0, 0, skillInfo.BuffIDList.ToArray());
                if (r != null)
                {
                    r.ResultExpr(skillInfo.BuffIDList.ToArray());
                    BattleFactory.Singleton.DispatchResult(r);
                }
            }
        }
        //通知UISwitchSkill
        //NotifyChanged((int)ENPropertyChanged.enSwitchSkill, null);
    }
	GameObject mPlayerNameObj;
	public override void Update()
	{
		base.Update();
        if ((mPlayerNameObj != null) && (mPlayerNameObj.activeSelf))
        {
            Vector3 vpos = this.RealPos;
            vpos.y += 1.0f;
            vpos.x -= 0.0f;
            mPlayerNameObj.transform.position = vpos;
            MainGame.FaceToCamera(mPlayerNameObj);
        }
	}

    void AddPlayerName()
    {
        Actor player = ActorManager.Singleton.Lookup(ID);
        if (null == player)
        {
            Debug.Log("player is null!");
        }
        GameObject grp = GameObject.Find("PlayerNameGrp");
        if (grp == null)
        {
            grp = new GameObject("PlayerNameGrp");
            grp.transform.position = Vector3.zero;
            grp.transform.parent = MainGame.Singleton.MainObject.transform;
        }

        //float fsl = 0.005f;
        //Vector3 vpos = this.RealPos;
        //vpos.y = 1.0f;
        //mPlayerNameObj = GameObject.Instantiate(GameData.LoadUI<GameObject>("PlayerName")) as GameObject;
        //mPlayerNameObj.name = "Bar_" + Type.ToString() + "_" + ID.ToString();
        //mPlayerNameObj.transform.parent = grp.transform;
        //mPlayerNameObj.transform.position = vpos;
        //mPlayerNameObj.transform.localScale = new Vector3(fsl, fsl, fsl);
        //mPlayerNameObj.transform.FindChild("Name").GetComponent<UILabel>().text = player.Props.GetProperty_String(ENProperty.name);
    }

    public override void Destroy()
    {
		//string name = "Bar_" + Type.ToString() + "_" + ID.ToString();
		//GameObject grp = GameObject.Find(name);
		//if (null != grp)
		//{
		//	GameObject.Destroy(grp);
		//}
		if (null != mPlayerNameObj)
		{
			GameObject.Destroy(mPlayerNameObj);
			mPlayerNameObj = null;
		}
        base.Destroy();
    }


    public override bool CreateNeedModels()
    {
        if (MainObj == null)
        {
            Load("Model/Player", CurrentTableInfo.ModelId, CurrentTableInfo.ModelScale, CurrentTableInfo.WeaponId);
        }
        return true;
    }

	#region 行为
	public override void DefaultAction()
	{
        //StateControl.ChangeState<StandState>();
        ActionControl.AddAction(ActorAction.ENType.enStandAction);
	}

    public override void BeHited(Actor srcActor, bool isBack, bool isFly, float animGauge)
	{
        base.BeHited(srcActor, isBack, isFly, animGauge);
		//{
		//	SpasticityAction action = ActionControl.AddAction(ActorAction.ENType.enSpasticityAction) as SpasticityAction;
		//	action.LastTime = 0.5f;
		//}
	}
    #endregion
    //combo changed
    public override void OnComboChanged(int resultID, int targetID, float damageModify)
    {
        if (Combo != null)
        {
            Combo.ComboChanged(resultID, targetID, damageModify);
        }
    }
    public override void OnWeakenComboTime(float time)
    {
        if (Combo != null)
        {
            Combo.WeakenComboTime += time;
        }
    }
    public bool FireSwitchSkill()
    {//释放切入技
        if (BattleArena.Singleton.SwitchSkillCount < 1)
        {//切入技没有可用次数
            return false;
        }
        if (null != CurrentCmd)
        {//切入技角色当前有命令
            return false;
        }
        if (this.IsDead)
        {//角色死亡
            return false;
        }
        if (SkillBag.Count == 0)
        {//没有可用技能
            return false;
        }
        if (SkillBag[0].IsSilence)
        {
            return false;
        }
        int skillID = SkillBag[0].SkillTableInfo.ID;
        if (SkillControl.IsSkillCDRunning(skillID, this))
        {//cd中
            return false;
        }
        //Actor support = ActorManager.Singleton.Support;
        //if (support == null)
        {//将切入技角色加入列表
            ActorManager.Singleton.AddActor(ActorManager.Singleton.Support.ID, ActorManager.Singleton.Support);
        }
        //设置切入技角色目标
        ActorManager.Singleton.Support.CurrentTarget = ActorManager.Singleton.MainActor.CurrentTarget;

        CurrentCmd = new Player.Cmd(skillID);
        --BattleArena.Singleton.SwitchSkillCount;
        return true;
    }
    //获取种族类型
    public override int OnGetRaceType()
    {
        return CurrentTableInfo.Type;
    }

    public override void FixedUpdate()
    {
        if (!m_isLoaded)
        {
            return;
        }
        if (Type == ActorType.enFollow)
        {
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
        }
        base.FixedUpdate();
        if (null != WTF)
        {
            WTF.Tick();
        }
    }
    public override void OnAddWill(WillToFight.ENAddWillType type, float[] paramList)
    {
        base.OnAddWill(type, paramList);
        if (null != WTF)
        {
            WTF.AddWill(type, paramList);
        }
    }

    public override int GetActorInitModelID()
    {
        return CurrentTableInfo.ModelId;
    }
    public override int GetActorInitWeaponID()
    {
        return CurrentTableInfo.WeaponId;
    }
    //初始化同伴的位置
    public void InitPartnerPosition()
    {
        if (IsDead) return;
        Vector3 pos = ActorManager.Singleton.MainActor.MainPos + ActorManager.Singleton.MainActor.MainObj.transform.forward * -1;
        if (SM.RandomRoomLevel.Singleton.QuickFindPath(pos, ActorManager.Singleton.MainActor.MainPos))
        {
            ForceMoveToPosition(pos);
            ActionControl.AddAction(ActorAction.ENType.enStandAction);
        }
    }
    public override void OnInitSkillBag(bool isNotifyUI = false)
	{
		if (SkillBag == null)
		{
			SkillBag = new List<ActorSkillInfo>();
		}
		SkillBag.Clear();
		//normal skill
		foreach (var item in CurrentTableInfo.NormalSkillIDList)
		{
			SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(item);
			if (skillInfo == null) continue;
			if (null == SkillBag.Find(info => info.SkillTableInfo.ID == item))
			{
				SkillBag.Add(new ActorSkillInfo(skillInfo, 0));
			}
		}
		{//open box skill
			SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(CurrentTableInfo.OpenBoxSkillID);
			if (skillInfo != null)
			{
				if (null == SkillBag.Find(info => info.SkillTableInfo.ID == skillInfo.ID))
				{
					SkillBag.Add(new ActorSkillInfo(skillInfo, 0));
				}
			}
		}
        base.OnInitSkillBag(isNotifyUI);
    }

	public bool SwitchActorEnter(Vector3 pos, Vector3 forward)
    {
        ActorAction action = ActionControl.AddAction(ActorAction.ENType.enActorEnterAction);
        if (action != null)
        {
            action.IsSyncPosition = true;
            action.SyncPosition = pos;
		    //隐藏collider
		    EnableCollider(false);
		    //设置朝向
		    MainObj.transform.forward = forward;
            IsActorExit = false;
            return true;
        }
        return false;
	}
	public bool FireNormalSkill()
	{//释放普通技能
		if (IsDead)
		{
			return false;
		}
		if (NormalSkillList.Count == 0)
		{
			return false;
		}
		if (!this.CurrentTargetIsDead)
		{//当前目标没死
			if (this.CurrentTarget.Type == ActorType.enNPC)
			{
				NPC npc = this.CurrentTarget as NPC;
				if (npc.GetNpcType() == ENNpcType.enBoxNPC)
				{//目标为宝箱，不自动攻击
					return false;
				}
			}
		}
		int index = UnityEngine.Random.Range(0, NormalSkillList.Count);
		this.CurrentCmd = new Player.Cmd(NormalSkillList[index]);
		return true;
	}
}