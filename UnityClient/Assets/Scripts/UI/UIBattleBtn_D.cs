using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIBattleBtn_D : UIWindow
{
    class SameInfo
    {
        public GameObject m_objSkill = null;
        public UISkillCD m_uiSkillCD = null;
        //public UIQTETimeLeft m_uiQteTimeLeft = null;
    }
    UISprite m_markAutoAttack = null;
    GameObject m_markAutoSkill = null;

    Material m_normalMaterial = null; // 正常技能的材质
    Material m_diableMaterial = null; // disable技能材质

    //skill object列表
    static int AllSkillCount = 6;
    static int MaxSkillNumber = AllSkillCount - 1;
    static int SwitchSkillIndex = AllSkillCount - 1;
    SameInfo[] m_sameInfoList = new SameInfo[AllSkillCount];
    //atlas列表
    //List<UIAtlas> m_atlasList = new List<UIAtlas>();
    UISprite m_spriteFrame = null;
    UIGrid m_grid = null;
    MainPlayer m_mainPlayer { get { return ActorManager.Singleton.MainActor; } }
    int m_openBoxSkillID = 0;
    //float m_connectionTime = 0;//连接技时间
    //float m_specialTime = 0;//特殊技时间

	GameObject m_skillInfoTips = null;
	UILabel m_tipsSkillName = null;
	UILabel m_skillGetTips = null;
	UILabel m_tipsSkillDes = null;
	UILabel m_tipsSkillCD = null;

	UILabel m_tipsSkillLevel = null;

	//int m_tipsDesHeightBase = 32;
	//int m_tipsSwitchSkillBasePos = 120;

	GameObject m_switchSkillTips = null;
	GameObject m_comboItem = null;
	List<GameObject> m_comboList = new List<GameObject>();
	GameObject m_tipsSkillType = null;

    GameObject m_autoAttack = null;

    UIButton m_addActorLevel = null;
    UILabel m_yellowPointLabel = null;//黄点显示等级
    static public UIBattleBtn_D Singleton
    {
        get
        {
            UIBattleBtn_D self = UIManager.Singleton.GetUIWithoutLoad<UIBattleBtn_D>();
            if (self == null)
            {
                self = UIManager.Singleton.LoadUI<UIBattleBtn_D>("UI/UIBattleBtn_D", UIManager.Anchor.Center);
            }
            return self;
        }
    }

    static public UIBattleBtn_D GetInstance()
    {
        UIBattleBtn_D self = UIManager.Singleton.GetUIWithoutLoad<UIBattleBtn_D>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UIBattleBtn_D>("UI/UIBattleBtn_D", UIManager.Anchor.Center);
        }
        return self;
    }
    public UIBattleBtn_D()
    {
        IsAutoMapJoystick = false;
    }
    public override void OnInit()
    {
        base.OnInit();
        string temp                     = "UI/Interface/BattleBtn/battleBtn.mat";
        WorldParamInfo worldParamInfo   = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enbattleBtnNormal);
        if ( null != worldParamInfo )
        {
            temp = worldParamInfo.StringTypeValue;
        }
        m_normalMaterial = PoolManager.Singleton.LoadWithoutInstantiate<Material>(temp);
        temp                = "UI/Interface/BattleBtn/battleBtn_disable.mat";
        worldParamInfo      = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enbattleBtnDisable);
        if (null != worldParamInfo)
        {
            temp = worldParamInfo.StringTypeValue;
        }
        m_diableMaterial = PoolManager.Singleton.LoadWithoutInstantiate<Material>(temp);


        AddPropChangedNotify((int)MVCPropertyID.enMainPlayer, OnPropertyChanged);
        m_markAutoAttack = FindChildComponent<UISprite>("MarkAutoAttack");
        m_markAutoSkill = FindChild("MarkAutoUseSkill");
        m_spriteFrame = FindChildComponent<UISprite>("skillFramework");
        m_grid = FindChildComponent<UIGrid>("SkillGrid");

        m_addActorLevel = FindChildComponent<UIButton>("ActorLevelButton");
        m_yellowPointLabel = FindChildComponent<UILabel>("yelloPointLabel");
        m_skillInfoTips = FindChild("SkillInfoTips");
		m_tipsSkillName = FindChildComponent<UILabel>("SkillName");
		m_skillGetTips = FindChildComponent<UILabel>("SkillGetTips");
		m_tipsSkillDes = FindChildComponent<UILabel>("SkillDes");
		m_tipsSkillCD = FindChildComponent<UILabel>("SkillCD");
		m_tipsSkillLevel = FindChildComponent<UILabel>("SkillLevel");
		m_switchSkillTips = FindChild("SwitchSkillTips");
		m_comboItem = FindChild("Combo");
		m_tipsSkillType = FindChild("SkillType");

        m_autoAttack = FindChild("AutoAttack");

        for (int i = 0; i < AllSkillCount; ++i)
        {
            string str = "Skill" + (i+1).ToString();
            GameObject skill = FindChild(str, WindowRoot);
            GameObject bk = FindChild("Background", skill);

            UISkillCD uiCd = UISkillCD.Create();
            uiCd.Init(bk, UISkillCD.EnShape.enSquare_Samall);
            uiCd.HideWindow();

            //UIQTETimeLeft uiQteTimeLeft = UIQTETimeLeft.Create();
            //uiQteTimeLeft.Init(bk);
            //uiQteTimeLeft.HideWindow();

            SameInfo info = new SameInfo();
            info.m_objSkill = skill;
            info.m_uiSkillCD = uiCd;
            //info.m_uiQteTimeLeft = uiQteTimeLeft;
            m_sameInfoList[i] = info;
        }
        GameObject lastSkill = m_sameInfoList[SwitchSkillIndex].m_objSkill;
        if (ActorManager.Singleton.Support != null && ActorManager.Singleton.Support.SkillBag.Count != 0)
        {
            lastSkill.SetActive(true);

            SkillInfo skillInfo = ActorManager.Singleton.Support.SkillBag[0].SkillTableInfo;
            IconInfomation info = GameTable.IconInfoTableAsset.Lookup(skillInfo.Icon);

            FindChildComponent<UITexture>("skill", lastSkill).mainTexture = PoolManager.Singleton.LoadIcon<Texture>(info.dirName);
            UISkillCD cd = m_sameInfoList[SwitchSkillIndex].m_uiSkillCD;
            cd.SkillID = skillInfo.ID;
            cd.RemoveMainRegister();
            cd.RegisterActor(ActorManager.Singleton.Support);

            SwitchSkillChanged();
        }
        else
        {
            lastSkill.SetActive(false);
        }

        //m_connectionTime = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillConnectionTime).FloatTypeValue;
        //m_specialTime = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillSpecialTime).FloatTypeValue;

        PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetUIPath("UISkillHighlight"), Coroutine_LoadHightlightObjCallback);
    }
    void Coroutine_LoadHightlightObjCallback(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            m_objHightlight = GameObject.Instantiate(obj) as GameObject;
        }
    }
    public void UpdateAddSkillLevel()
    {
        int levelUpPoint = m_mainPlayer.Props.GetProperty_Int32(ENProperty.LevelUpPoint);
        //判断黄点是否达到满级
        MainPlayer mainPlayer = ActorManager.Singleton.MainActor;
        int yellowPoint = mainPlayer.Props.GetProperty_Int32(ENProperty.YellowPointLevel);
        YellowPointInfo nextYellowInfo = GameTable.yellowPointParamAsset.LookUp((yellowPoint+1));
        //如果为空则达到最大黄点等级
        if (null == nextYellowInfo)
        {
            if (null != m_yellowPointLabel)
            {
                m_yellowPointLabel.text = "LV.MAX";
            }
        }
        else 
        {
            if (null != m_yellowPointLabel)
            {
                m_yellowPointLabel.text = "LV." + yellowPoint;
            }
        }
        if (levelUpPoint > 0 && null != nextYellowInfo)
        {
            m_addActorLevel.gameObject.SetActive(true);
        }
        else
        {
            m_addActorLevel.gameObject.SetActive(false);
        }
        

        int skillLeveLIndex = 1;
        for (int i = 0; i < MaxSkillNumber; ++i)
        {
            GameObject skill = m_sameInfoList[i].m_objSkill;
            if (!skill.activeSelf)
            {
                continue;
            }
            UISkillCD uiSkillCD = m_sameInfoList[i].m_uiSkillCD;
            Actor.ActorSkillInfo info = m_mainPlayer.SkillBag.Find(item => item.SkillTableInfo.ID == uiSkillCD.SkillID);
            if (info == null)
            {
                Debug.LogWarning("skill table is null, index:" + i);
                continue;
            }

            UILabel skillLabel = FindChildComponent<UILabel>("SkillLevel", skill);
            if (null != skillLabel)
            {
                if (info.SkillLevel >= info.SkillTableInfo.MaxLevel)
                {
                    skillLabel.text = "LV.MAX";
                }
                else 
                {
                    skillLabel.text = "LV." + info.SkillLevel;
                }
                
            }
            //技能升级按钮
            UIButton addLevel = FindChildComponent<UIButton>("AddLevel" + skillLeveLIndex, skill);
            if (null != addLevel)
            {
                if (levelUpPoint > 0 && info.SkillLevel < info.SkillTableInfo.MaxLevel)
                {
                    addLevel.gameObject.SetActive(true);
                }
                else
                {
                    addLevel.gameObject.SetActive(false);
                }
            }
            ++skillLeveLIndex;
        }
    }

    void SkillChange()
    {
        int index = 0;
        bool isNormaled = false;

        foreach (var item in m_mainPlayer.SkillBag)
        {
            SkillInfo skillInfo = item.SkillTableInfo;
            if (skillInfo.SkillType != (int)ENSkillType.enSkill &&
                skillInfo.SkillType != (int)ENSkillType.enSkillNormalType &&
                skillInfo.SkillType != (int)ENSkillType.enDodge)
            {
                if (skillInfo.SkillType == (int)ENSkillType.enOpenBox)
                {//开宝箱技能
                    m_openBoxSkillID = skillInfo.ID;
                }
                continue;
            }
            if (skillInfo.SkillType == (int)ENSkillType.enSkillNormalType)
            {
                continue;
            }
            if (skillInfo.SkillType == (int)ENSkillType.enDodge)
            {
                if (isNormaled)
                {
                    continue;
                }
                isNormaled = true;
            }

            GameObject skill = m_sameInfoList[index].m_objSkill;
            skill.SetActive(true);
            UITexture sprite = FindChildComponent<UITexture>("skill", skill);

            //设置技能等级
            UILabel skillLabel = FindChildComponent<UILabel>("SkillLevel", skill);
            if (null != skillLabel)
            {
                skillLabel.text = "LV." + item.SkillLevel;
            }

            //icon

            IconInfomation info = GameTable.IconInfoTableAsset.Lookup(skillInfo.Icon);
            if (info == null)
            {
                Debug.LogWarning("load icon fail,id:" + skillInfo.Icon);
            }
            else
            {
                sprite.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(info.dirName);
            }

            sprite.material     = m_normalMaterial;
            //cd
            UISkillCD cd = m_sameInfoList[index].m_uiSkillCD;
            cd.Reset();
            cd.SkillID = skillInfo.ID;
            if (m_mainPlayer.SkillControl.IsSkillCDRunning(skillInfo.ID, m_mainPlayer))
            {
                cd.ShowSkillCDBtn();
            }

            //uiQteTimeLeft
            //UIQTETimeLeft qteCD = m_sameInfoList[index].m_uiQteTimeLeft;
            //qteCD.Reset();

            ++index;
        }
        for (int i = index; i < MaxSkillNumber; ++i)
        {
            m_sameInfoList[i].m_objSkill.SetActive(false);
        }
        //设置frame的width
        m_spriteFrame.width = index * (int)m_grid.cellWidth;
        if (ActorManager.Singleton.Support != null)
        {
            m_spriteFrame.width += (int)m_grid.cellWidth;
        }
        //设置grid
        m_grid.repositionNow = true;
        //qte
        /*if (m_isQteVisible)
        {
            SetQteVisible(m_isQteVisible);
        }*/
        //更新是否需要显示加点按钮
        UpdateAddSkillLevel();
        DelHighlight();
        SkillEnabledChanged();
    }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)Actor.ENPropertyChanged.enBattleBtn_D)
        {
            switch ((MainPlayer.ENBattleBtnNotifyType)eventObj)
            {
                case MainPlayer.ENBattleBtnNotifyType.enShowChange:
                    {//显示改变
                        if (IsVisiable())
                        {
                            HideWindow();
                            DelHighlight();
                        }
                        else
                        {
                            if (m_mainPlayer.m_curBattleBtn == (int)Actor.ENPropertyChanged.enBattleBtn_D)
                            {
                                AddHighlight(m_mainPlayer.m_curHighlightIndex);
                                if (m_objHightlightChild != null)
                                {
                                    m_objHightlightChild.SetActive(true);
                                }
                                ShowWindow();
                            }
                        }
                    }
                    break;
                case MainPlayer.ENBattleBtnNotifyType.enSkillChange:
                    {//技能改变
                        SkillChange();
                    }
                    break;
                case MainPlayer.ENBattleBtnNotifyType.enAddSkillHighlight:
                    {//技能高亮
                        if (m_objHightlightChild != null)
                        {
                            m_objHightlightChild.SetActive(true);
                        }
                    }
                    break;
                case MainPlayer.ENBattleBtnNotifyType.enDelSkillHighlight:
                    {//删除技能高亮
                        DelHighlight();
                    }
                    break;
                case MainPlayer.ENBattleBtnNotifyType.enTargetChanged:
                    {//目标改变
                        SkillEnabledChanged();
                        SwitchSkillChanged();
                    }
                    break;
                case MainPlayer.ENBattleBtnNotifyType.enSkillEnabled:
                    {//可释放技能
                        SkillEnabledChanged();
                    }
                    break;
                case MainPlayer.ENBattleBtnNotifyType.enUpdataSkillLevelUp:
                    {
                        //更新技能升级UI
                        UpdateAddSkillLevel();
                    }
                    break;
                /*case MainPlayer.ENBattleBtnNotifyType.enQteShow:
                    {//qte显示
                        m_qteStartTime = Time.time;
                        SetQteVisible(true);
                    }
                    break;
                case MainPlayer.ENBattleBtnNotifyType.enQteHide:
                    {//qte显示
                        SetQteVisible(false);
                    }
                    break;
                 * */
            }
        }
    }
    void SkillEnabledChanged()
    {
        bool targetIsBox = false;
        bool targetIsTrap = false;
        if (!m_mainPlayer.CurrentTargetIsDead)
        {
            if (m_mainPlayer.CurrentTarget.Type == ActorType.enNPC)
            {
                NPC npc = m_mainPlayer.CurrentTarget as NPC;
                if (npc.GetNpcType() == ENNpcType.enBoxNPC)
                {
                    targetIsBox = true;
                }
            }
            if (m_mainPlayer.CurrentTarget.Type == ActorType.enNPCTrap)
            {
                targetIsTrap = true;
            }
        }
        for (int i = 0; i < MaxSkillNumber; ++i)
        {
            GameObject skill = m_sameInfoList[i].m_objSkill;
            if (!skill.activeSelf)
            {
                continue;
            }
            GameObject enabled = FindChild("Enabled", skill);
            UITexture sprite = FindChildComponent<UITexture>("skill", skill);

            UISkillCD uiSkillCD = m_sameInfoList[i].m_uiSkillCD;
            Actor.ActorSkillInfo info = m_mainPlayer.SkillBag.Find(item => item.SkillTableInfo.ID == uiSkillCD.SkillID);
            if (info == null)
            {
                Debug.LogWarning("skill table is null, index:"+i);
                continue;
            }
            if (info.SkillTableInfo.SkillType == (int)ENSkillType.enSkillNormalType ||
                info.SkillTableInfo.SkillType == (int)ENSkillType.enOpenBox)
            {//普通技能
                if (targetIsBox || targetIsTrap)
                {
                    if (uiSkillCD.SkillID != m_openBoxSkillID)
                    {
                        uiSkillCD.Reset();
                        uiSkillCD.SkillID = m_openBoxSkillID;
                        info = m_mainPlayer.SkillBag.Find(item => item.SkillTableInfo.ID == uiSkillCD.SkillID);
                    }
                }
                else
                {
                    if (uiSkillCD.SkillID == m_openBoxSkillID)
                    {
                        uiSkillCD.SkillID = m_mainPlayer.NormalSkillList[0];
                        info = m_mainPlayer.SkillBag.Find(item => item.SkillTableInfo.ID == uiSkillCD.SkillID);
                    }
                }
                continue;
            }
            IconInfomation iconInfo = GameTable.IconInfoTableAsset.Lookup(info.SkillTableInfo.Icon);
            if (iconInfo == null)
            {
                Debug.LogWarning("icon is null, id:" + info.SkillTableInfo.Icon);
                return;
            }
            else
            {
                sprite.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
            }
 
            
             Material curMaterial = sprite.material;

//              List<int> list = (m_mainPlayer.SelfAI as AIPlayer).SkillIDListForFire;
//              if (!info.IsSilence && list.Contains(info.SkillTableInfo.ID))
             {
                 enabled.SetActive(false);
                 curMaterial = m_normalMaterial;
             }
//              else
//              {
//                  enabled.SetActive(true);
//                 curMaterial = m_diableMaterial;
//              }
 
             //if (curMaterial != sprite.material)
             {
                sprite.material = curMaterial;
             }     
        }
    }
    //切入技改变
    void SwitchSkillChanged()
    {
        if (ActorManager.Singleton.Support == null || ActorManager.Singleton.Support.SkillBag.Count == 0)
        {
            return;
        }
        GameObject skill = m_sameInfoList[SwitchSkillIndex].m_objSkill;
        GameObject enabled = FindChild("Enabled", skill);
        UITexture sprite = FindChildComponent<UITexture>("skill", skill);

        Actor.ActorSkillInfo info = ActorManager.Singleton.Support.SkillBag[0];
        IconInfomation iconInfo = GameTable.IconInfoTableAsset.Lookup(info.SkillTableInfo.Icon);

        sprite.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);

        Material curMaterial = sprite.material;

        if (info.IsSilence)
        {//技能被沉默
            enabled.SetActive(true);
            curMaterial = m_diableMaterial;
        }
        else
        {
            if (m_mainPlayer.CurrentTargetIsDead)
            {//无目标
                enabled.SetActive(true);
                curMaterial = m_diableMaterial;
            }
            else
            {
                if (!ActorTargetManager.IsEnemy(m_mainPlayer, m_mainPlayer.CurrentTarget))
                {//当前目标不是敌人
                    enabled.SetActive(true);
                    curMaterial = m_diableMaterial;
                }
                else
                {
                    enabled.SetActive(false);
                    curMaterial = m_normalMaterial;
                }
            }
        }
        //if (sprite.material != curMaterial)
        {
            sprite.material = curMaterial;
        }   
    }
	public override void AttachEvent ()
	{
        base.AttachEvent();
        AddChildMouseClickEvent("Skill1", OnButtonSkill1Clicked);
		AddChildMouseClickEvent("Skill2", OnButtonSkill2Clicked);
		AddChildMouseClickEvent("Skill3", OnButtonSkill3Clicked);
        AddChildMouseClickEvent("Skill4", OnButtonSkill4Clicked);
        AddChildMouseClickEvent("Skill5", OnButtonSkill5Clicked);
        AddChildMouseClickEvent("Skill6", OnButtonSkill6Clicked);
        AddChildMouseClickEvent("AddLevel2", OnButtonAddLevel2Clicked);
        AddChildMouseClickEvent("AddLevel3", OnButtonAddLevel3Clicked);
        AddChildMouseClickEvent("AddLevel4", OnButtonAddLevel4Clicked);
        AddChildMouseClickEvent("AddLevel5", OnButtonAddLevel5Clicked);
        AddChildMouseClickEvent("AddLevel6", OnButtonAddLevel6Clicked);
        AddChildMouseClickEvent("ActorLevelButton", OnButtOnAddActorLevel);

		AddChildMouseLongPressEvent("Skill1", OnButtonLongSkill1Clicked);
		AddChildMouseLongPressEvent("Skill2", OnButtonLongSkill2Clicked);
		AddChildMouseLongPressEvent("Skill3", OnButtonLongSkill3Clicked);
		AddChildMouseLongPressEvent("Skill4", OnButtonLongSkill4Clicked);
		AddChildMouseLongPressEvent("Skill5", OnButtonLongSkill5Clicked);
		AddChildMouseLongPressEvent("Skill6", OnButtonLongSkill6Clicked);
		
        AddChildMouseClickEvent("AutoAttack", OnButtonMarkAutoAttackClicked);
		AddChildMouseClickEvent("changeTarget", OnButtonChangeTargetClicked);
		AddChildMouseClickEvent("Return", OnButtonChangeSkillClicked);
		AddChildMouseClickEvent("AddComboBtn", OnButtonAddComboClicked);
        UpdateMarkState();

        JoystickWrap joystick = WindowRoot.GetComponent<JoystickWrap>();
        if (joystick != null)
        {
            if (joystick.m_axisMapping != null && joystick.m_axisMapping.Count > 0)
            {
                joystick.m_axisMapping[0].m_axisEvent += OnMoveClicked;
            }
            if (joystick.m_mapping != null)
            {
                for (int i = 0; i < joystick.m_mapping.Count;i++ )
                {
                    if (joystick.m_mapping[i].m_name == "Scroll")
                    {
                        joystick.m_mapping[i].m_keyEvent += OnScrollClicked;
                    }
                    if (joystick.m_mapping[i].m_name == "SwitchActor")
                    {
                        joystick.m_mapping[i].m_keyEvent += SwitchActor;
                    }
                }
            }
        }
	}
    void UpdateMarkState()
    {
        if (m_mainPlayer.m_autoUseSkill)
        {
            //自动攻击
            m_markAutoAttack.spriteName = "i-interface-mu-D-0";
            //MarkAutoUseSkill
            m_markAutoSkill.SetActive(true);
        }
        else if (m_mainPlayer.m_autoAttackAll)
        {
            //自动攻击
            m_markAutoAttack.spriteName = "i-interface-mu-D-0";
            //MarkAutoUseSkill
            m_markAutoSkill.SetActive(false);
        }
        else
        {
            //正常控制
            m_markAutoAttack.spriteName = "i-interface-kong-D-0";
            m_markAutoSkill.SetActive(false);
        }
    }
    GameObject m_objHightlight = null;
    GameObject m_objHightlightChild = null;
    private void DelHighlight()
    {
        if (m_objHightlight != null)
        {
            FindChild("_circle", m_objHightlight).SetActive(false);
            FindChild("_square", m_objHightlight).SetActive(false);
        }
        m_objHightlightChild = null;
        if (IsVisiable())
        {
            m_mainPlayer.m_curHighlightIndex = 0;
        }
    }
    private void AddHighlight(int index)
    {
        if (index <= 1) return;
        m_mainPlayer.m_curHighlightIndex = index;
        if (null == m_objHightlight)
        {
            return;
        }
        string str = "Skill" + index.ToString();
        GameObject skill = FindChild(str, WindowRoot);
        GameObject bk = FindChild("Background", skill);
        m_objHightlight.transform.position = bk.transform.position;
        m_objHightlight.transform.parent = bk.transform;
        m_objHightlight.transform.localScale = Vector3.one;
        m_objHightlightChild = FindChild("_square", m_objHightlight);
    }
    private void FireUISKill(int index)
    {
		if (m_skillInfoTips.activeSelf)
		{
			m_skillInfoTips.SetActive(false);
			return;
		}
        int skillID = m_sameInfoList[index].m_uiSkillCD.SkillID;
        if (!m_mainPlayer.SkillControl.IsSkillCDRunning(skillID, m_mainPlayer))
        {
            AddHighlight(index + 1);
        }
        m_mainPlayer.OnFireSkill(skillID);
    }
	public string GetTimeString(float time)
	{
		string str = "";
		float min = time / 60;
		float sec = time % 60;
		if (min >= 1)
		{
			str = string.Format(Localization.Get("RevertTime"), min, sec);
		}
		else
		{
			str = string.Format(Localization.Get("RevertTime"), 0, sec);
		}
		return str;
	}
	Dictionary<int, string> GetComboList(int skillId)
	{
		Dictionary<int, string> list = new Dictionary<int, string>();

		int startIndex = (skillId - 1) * 10 + 1;
		int endIndex = skillId * 10;

		for (int i = startIndex; i <= endIndex; i++)
		{
			SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(i);

			if (null == info)
			{
				continue;
			}

			foreach (var item in info.ExtraParamList)
			{
				if (item.ComboJudgeCount <= 0)
				{
					continue;
				}
				if (!list.ContainsKey(item.ComboJudgeCount))
				{
					list.Add(item.ComboJudgeCount, item.ComboTips);
				}
			}
		}
		return list;
	}
	private void OnShowSkillTips(SkillInfo skillInfo, int lvl)
	{
		int skillId = skillInfo.ID;

		SkillInfo info = GameTable.SkillTableAsset.Lookup(skillId);
		if (null == info)
		{
			return;
		}

		CDInfo cdInfo = GameTable.CDTableAsset.Lookup(info.CoolDown);

		if (cdInfo == null)
		{
			m_tipsSkillCD.text = GetTimeString(0f);
		}
		else
		{
			m_tipsSkillCD.text = GetTimeString(cdInfo.CDTime);
		}


		//        Debug.Log("m_tipsSkillDes:" + m_tipsSkillDes.height);

//		int needLevel = 0;
		// tips 位置
		int offset = 50;

		// 主动技能类型
		if (info.SkillType == (int)ENSkillType.enSkill)
		{
			offset = 100;
		}
		else
		{
			offset = 200;
		}

			m_skillGetTips.gameObject.SetActive(false);



		// 设置内容
		m_tipsSkillName.text = info.Name;
		m_tipsSkillDes.text = info.Description;
		m_tipsSkillLevel.text = lvl.ToString();



		int height = 100;
		height = height + m_tipsSkillDes.height;

		int switchTipsHeight = 100;

		switchTipsHeight = switchTipsHeight + m_tipsSkillDes.height;


		m_comboItem.SetActive(false);

		for (int i = 0; i < m_comboList.Count; i++)
		{
			if (null != m_comboList[i])
			{
				GameObject.Destroy(m_comboList[i]);
			}
		}

		string strSkillType = "";

		// 终结技 
		if (info.SkillConnectionType == (int)ENSkillConnectionType.enFinal)
		{
			strSkillType = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enFinalSkill).StringTypeValue;

			m_comboItem.SetActive(true);
            Actor.ActorSkillInfo actroSkillInfo = this.m_mainPlayer.SkillBag.Find(item => item.SkillTableInfo.ID == info.ID);
            m_comboItem.GetComponent<UILabel>().text = actroSkillInfo.MinComboRequir.ToString();
			WorldParamInfo worldInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enActiveSkill);
			m_comboItem.transform.Find("ComboDes").GetComponent<UILabel>().text = Localization.Get(worldInfo.StringTypeValue);
			height = height + 60;


			Dictionary<int, string> list = GetComboList(skillId);
			int comboMax = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enMaxCombo).IntTypeValue;

			if (list != null)
			{
				int i = 1;
				foreach (KeyValuePair<int, string> item in list)
				{
					if (i >= comboMax)
					{
						break;
					}
					GameObject copy = GameObject.Instantiate(m_comboItem) as GameObject;

					copy.SetActive(true);
					copy.transform.parent = m_switchSkillTips.transform;
					copy.name = copy.name + i;
					copy.transform.localScale = Vector3.one;

					copy.transform.localPosition = Vector3.zero;

					copy.GetComponent<UILabel>().text = item.Key.ToString();
					copy.transform.Find("ComboDes").GetComponent<UILabel>().text = item.Value;
					height = height + 60;
					i++;

					m_comboList.Add(copy);
				}
			}
		}
		else if (info.SkillConnectionType == (int)ENSkillConnectionType.enConnect)
		{
			strSkillType = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enConectSkill).StringTypeValue;
		}
		else if (info.SkillConnectionType == (int)ENSkillConnectionType.enSpecial)
		{
			strSkillType = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSpecialSkill).StringTypeValue;
		}


		m_tipsSkillType.GetComponent<UILabel>().text = Localization.Get(strSkillType);

		// 设置tips的 大小 主要是高度
		m_skillInfoTips.GetComponent<UISprite>().height = height;

		// 设置位置
		m_skillInfoTips.GetComponent<UISprite>().topAnchor.absolute = -offset;

		m_switchSkillTips.GetComponent<UILabel>().topAnchor.absolute = -switchTipsHeight;

		m_switchSkillTips.GetComponent<UIGrid>().Reposition();
	}
	private void LongPressButton(int index)
	{
		int skillID = m_sameInfoList[index].m_uiSkillCD.SkillID;
		SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(skillID);
		//m_skillInfoTips.transform
		OnShowSkillTips(skillInfo,1);
		m_skillInfoTips.SetActive(true);
	}

	public void OnButtonLongSkill1Clicked(object sender, EventArgs e)
	{
		LongPressButton(0);
	}
	public void OnButtonLongSkill2Clicked(object sender, EventArgs e)
	{
		LongPressButton(1);
	}
	public void OnButtonLongSkill3Clicked(object sender, EventArgs e)
	{
		LongPressButton(2);
	}
	public void OnButtonLongSkill4Clicked(object sender, EventArgs e)
	{
		LongPressButton(3);
	}
	public void OnButtonLongSkill5Clicked(object sender, EventArgs e)
	{
		LongPressButton(4);
	}
	public void OnButtonLongSkill6Clicked(object sender, EventArgs e)
	{
		LongPressButton(5);
	}

    public void OnButtonSkill1Clicked(object sender, EventArgs e)
    {
        DelHighlight();
        if (!m_mainPlayer.CurrentTargetIsDead)
        {
            if (m_mainPlayer.CurrentTarget.Type == ActorType.enNPC)
            {
                NPC npc = m_mainPlayer.CurrentTarget as NPC;
                if (npc.GetNpcType() == ENNpcType.enBoxNPC)
                {
                    m_mainPlayer.OnFireSkill(m_openBoxSkillID);
                    return;
                }
            }
        }
        FireUISKill(0);
    }

	public void OnButtonSkill2Clicked(object sender, EventArgs e)
    {
        FireUISKill(1);
	}
	
	public void OnButtonSkill3Clicked(object sender, EventArgs e)
    {
        FireUISKill(2);
	}
	
	public void OnButtonSkill4Clicked(object sender, EventArgs e)
    {
        FireUISKill(3);
	}


    public void OnButtonSkill5Clicked(object sender, EventArgs e)
    {
        FireUISKill(4);
    }
    public void OnButtonSkill6Clicked(object sender, EventArgs e)
    {
        if (ActorManager.Singleton.Support == null)
        {
            return;
        }
        ActorManager.Singleton.Support.FireSwitchSkill();
    }

    public int GetSkillIndexBySkillId(int SkillId) 
    {
        int index = 0;
        
        PropertyValueIntListView skillIDList = m_mainPlayer.Props.GetProperty_Custom(ENProperty.SkillIDList) as PropertyValueIntListView;
        foreach (var item in skillIDList.m_list) 
        {
            if (item == SkillId)
            {
                return index;
            }
            ++index;
        }

        return index;
    }

    public void OnButtonAddLevel2Clicked(object sender, EventArgs e)
    {
        int skillID = m_sameInfoList[1].m_uiSkillCD.SkillID;
        MiniServer.Singleton.SendLevelUpSkill_C2BS(m_mainPlayer.ID, skillID, true);
    }
    public void OnButtonAddLevel3Clicked(object sender, EventArgs e)
    {
        int skillID = m_sameInfoList[2].m_uiSkillCD.SkillID;
        MiniServer.Singleton.SendLevelUpSkill_C2BS(m_mainPlayer.ID, skillID, true);
    }
    public void OnButtonAddLevel4Clicked(object sender, EventArgs e)
    {
        int skillID = m_sameInfoList[3].m_uiSkillCD.SkillID;
        MiniServer.Singleton.SendLevelUpSkill_C2BS(m_mainPlayer.ID, skillID, true);
    }
    public void OnButtonAddLevel5Clicked(object sender, EventArgs e)
    {
        int skillID = m_sameInfoList[4].m_uiSkillCD.SkillID;
        MiniServer.Singleton.SendLevelUpSkill_C2BS(m_mainPlayer.ID, skillID, true);
    }
    public void OnButtonAddLevel6Clicked(object sender, EventArgs e)
    {
        int skillID = m_sameInfoList[5].m_uiSkillCD.SkillID;
        MiniServer.Singleton.SendLevelUpSkill_C2BS(m_mainPlayer.ID, skillID,true);
    }
    public void OnButtOnAddActorLevel(object sender, EventArgs e) 
    {
        MiniServer.Singleton.SendLevelUpSkill_C2BS(m_mainPlayer.ID, 0,false);
    }
    // 自动翻滚 [8/3/2015 tgame]
    public void OnScrollClicked(JoystickWrap.JoystickKeyMapping k)
    {
        Vector3 f = m_mainPlayer.MainObj.transform.forward;
        JoystickWrap joystick = WindowRoot.GetComponent<JoystickWrap>();
        if (joystick.m_axisMapping != null && joystick.m_axisMapping.Count > 0)
        {
            JoystickWrap.JoystickAxisMapping axis = joystick.m_axisMapping[0];
            if (axis.GetVector3D().sqrMagnitude > 0.01f)
            {
                f = axis.GetVector3D() * 5.0f;
            }
        }

        f = m_mainPlayer.MainPos + f*3.0f;
        m_mainPlayer.CurrentCmd = new MainPlayer.Cmd(f, Player.ENCmdType.enRoll);
    }
    //移动 [8/3/2015 tgame]
    public void OnMoveClicked(JoystickWrap.JoystickAxisMapping axis)
    {
        Vector3 f = axis.GetVector3D();
        if (f.sqrMagnitude<0.01f)
        {
            MoveAction ac = m_mainPlayer.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
            if (ac != null)
            {
                m_mainPlayer.CurrentCmd = new MainPlayer.Cmd(f, Player.ENCmdType.enStopMove);
            }
            return;
        }
        f = m_mainPlayer.MainPos + f * 4.0f;
        m_mainPlayer.CurrentCmd = new MainPlayer.Cmd(f, Player.ENCmdType.enMove);
        m_mainPlayer.CurrentCmd.m_isMoveByNoAStar = true;
    }
    //切换角色
    void SwitchActor(JoystickWrap.JoystickKeyMapping k)
    {
        bool isAttacking = false;
        MainPlayer mainActor = ActorManager.Singleton.MainActor;
        if (mainActor.CurrentCmd != null)
        {
            if (mainActor.CurrentCmd.m_type == Player.ENCmdType.enLoopNormalAttack || mainActor.CurrentCmd.m_type == Player.ENCmdType.enSkill)
            {
                isAttacking = true;
            }
        }
        (mainActor.SelfAI as AIPlayer).m_isAttacking = isAttacking;
        mainActor.CurrentCmd = new Player.Cmd(Player.ENCmdType.enSwitchActor);
    }
	public void OnButtonMarkAutoAttackClicked(object sender, EventArgs e)
    {
        /*if (m_mainPlayer.m_autoUseSkill)
        {
            m_mainPlayer.m_autoAttackAll = false;
            m_mainPlayer.m_autoUseSkill = false;
        }
        else if (m_mainPlayer.m_autoAttackAll)
        {
            m_mainPlayer.m_autoUseSkill = true;
        }
        else
        {
            m_mainPlayer.m_autoAttackAll = true;
            if (m_mainPlayer.CurrentCmd == null)
            {
                m_mainPlayer.FireNormalSkill();
            }
        }
        UpdateMarkState();*/
        //m_mainPlayer.SwitchAI();
        UISprite autoSprite = null;
        GameObject autoIcon = FindChild("AutoAttack");
        if (autoIcon != null)
        {
            Transform tImage = autoIcon.transform.Find("Image");
            if (tImage != null)
            {
                autoSprite = tImage.GetComponent<UISprite>();
            }
        }
        if (ActorManager.Singleton.Chief.SwitchAI())
        {
            autoSprite.spriteName = "autoAttacking";
        }
        else
        {
            autoSprite.spriteName = "autoAttack";
            if (ActorManager.Singleton.Chief.ActionControl.IsActionRunning(ActorAction.ENType.enMoveAction))
            {
                ActorManager.Singleton.Chief.ActionControl.RemoveAction(ActorAction.ENType.enMoveAction);
            }
        }
        if (ActorManager.Singleton.Deputy != null)
        {
            ActorManager.Singleton.Deputy.SwitchAI();
        }
    }
	public void OnButtonChangeTargetClicked(object sender, EventArgs e)
    {
        if (m_mainPlayer.ChangeTarget())
        {
            DelHighlight();
        }		
	}

	//置换SkillButton
	public void OnButtonChangeSkillClicked(object sender, EventArgs e)
	{
        Dictionary<int, int> changeSkillList = new Dictionary<int, int>();
        Transform changeSkillGrid = FindChild("ChangeSkillGrid").transform;
		for (int i = 1; i < 5; i++)
		{
			Transform skillIndex = changeSkillGrid.Find("Skill" + i);
			string skillStr = skillIndex.Find("SkillId").GetComponent<UILabel>().text;
			if (string.IsNullOrEmpty(skillStr))
			{
				continue;
			}
			int changeSkillId = int.Parse(skillStr);
			SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(changeSkillId);
			if (skillInfo == null)
			{
				Debug.LogError("skill table lookup,result is null,id:" + changeSkillId);
				continue;
			}
            changeSkillList.Add(i-1, changeSkillId);
		}
		if (changeSkillList.Count == 0)
		{
			return;
		}
        m_mainPlayer.ChangeSkill(changeSkillList);
	}
	public void OnButtonAddComboClicked(object sender, EventArgs e)
	{
		m_mainPlayer.Combo.ButtonAddComb(10);
	}
    public override void OnDestroy()
    {
        if (m_objHightlight != null)
        {
            GameObject.Destroy(m_objHightlight);
            m_objHightlight = null;
        }
        m_objHightlightChild = null;
        m_markAutoAttack = null;
        m_markAutoSkill = null;
        foreach (var item in m_sameInfoList)
        {
            item.m_uiSkillCD.Destroy();
            //item.m_uiQteTimeLeft.Destroy();
        }
        m_sameInfoList = null;
        //m_atlasList.Clear();
        //m_atlasList = null;
        base.OnDestroy();
    }
    public override void OnShowWindow()
    {
        m_mainPlayer.OnInitSkillBag(true);
        //SkillChange();
        base.OnShowWindow();
    }

    public void HideAutoAttack()
    {
        m_autoAttack.SetActive(false);
    }
}
