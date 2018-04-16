using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIStageMenu : UIWindow
{
    enum CurType
    {
        enStageType = 1,
        enFloorType ,
        enBattleType,
        enHelperType,
    }

    UILabel m_level                         = null;
    UILabel m_name                          = null;
    //UIDragScrollView m_floorItem            = null;
    //UIDragScrollView m_stageItem            = null;
    List<UIStageMenuStageItem> m_itemList   = null;
    UILabel m_grid                          = null;

    //UILabel m_mainTips                      = null;
    //UILabel m_deputyTips                    = null;
    //UILabel m_supportTips                   = null;
    //UILabel m_comradeTips                   = null;

    UISprite m_battlePreparation            = null;

    UIButton m_battleBtn                    = null;
    //UILabel m_noMainTips                    = null;

    //Dictionary<CSItemGuid, GameObject> m_modelList = null; // int =  slotID  GameObject = 模型物件
    List<GameResPackage.AsyncLoadObjectData> m_modelDataList = new List<GameResPackage.AsyncLoadObjectData>();

    List<UIStageMenuFloorItem> m_floorItemList = null;

    int m_curType                           = 0;

    UILabel m_expText                       = null;
    UIProgressBar m_expBar                  = null;
    UILabel m_staminaText                   = null;
    UIProgressBar m_staminaBar              = null;
    UILabel m_staminaTimingText             = null;
    UILabel m_levelText                     = null;

    UIPanel m_helper                        = null;
    Dictionary<int, UIStageMenuTeamItem> m_teamList = new Dictionary<int, UIStageMenuTeamItem>();
    //UILabel m_teamItem                      = null;
    UIToggle m_helperItem                   = null;
    UIGrid m_helpGrid                       = null;
    UILabel m_helperCardName                = null;
    UILabel m_helperCardLevel               = null;
    UILabel m_helperCardRace                = null;
    UILabel m_helperCardOcc                 = null;
    UILabel m_helperPhyAttack               = null;
    UILabel m_helperMagAttack               = null;
    UILabel m_helperHP                      = null;
    UILabel m_helperModelFather             = null;
    bool m_isFirstChooseHelper              = true; // 是否是第一次点选 战友

    UIProgressBar m_heleperBar              = null;

    // 战友UI列表
    Dictionary<int, UIToggle> m_helperList          = new Dictionary<int,UIToggle>();

    // 战友模型
    //Dictionary<int, GameObject> m_helperModelList   = new Dictionary<int, GameObject>();
    GameResPackage.AsyncLoadObjectData m_helperModelData = new GameResPackage.AsyncLoadObjectData();

    float m_lastClickTime = 0f;// 上次点击返回按钮的时间

    UIInput m_inputKey = null;//进入副本的密钥
    UIInput m_inputCamp = null;//进入副本的阵营
    UIToggle m_isRolling = null;//是否允许翻滚

    static public UIStageMenu GetInstance()
	{
        UIStageMenu self = UIManager.Singleton.GetUIWithoutLoad<UIStageMenu>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIStageMenu>("UI/UIStageMenu", UIManager.Anchor.Center);
		return self;
	}

	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enStageMenu, OnPropertyChanged);
        //AddPropChangedNotify((int)MVCPropertyID.enSceneManager, OnPropertySceneChanged);

        m_inputCamp = FindChildComponent<UIInput>("IntCamp");
        m_inputKey = FindChildComponent<UIInput>("EnterCode");
        m_isRolling = FindChildComponent<UIToggle>("Toggle_effects");
        m_level     = FindChildComponent<UILabel>("Level");
        m_name      = FindChildComponent<UILabel>("ZoneName");
//        m_floorItem = FindChildComponent<UIDragScrollView>("FloorItem");
//        m_stageItem = FindChildComponent<UIDragScrollView>("StageItem");
        m_itemList      = new List<UIStageMenuStageItem>();
        m_floorItemList = new List<UIStageMenuFloorItem>();

        m_grid          = FindChildComponent<UILabel>("StageGrid");

//        m_mainTips      = FindChildComponent<UILabel>("MainTips");
//        m_deputyTips    = FindChildComponent<UILabel>("DeputyTips");
//        m_supportTips   = FindChildComponent<UILabel>("SupportTips");
//        m_comradeTips   = FindChildComponent<UILabel>("ComradeTips");

        m_battlePreparation = FindChildComponent<UISprite>("BattlePreparation");

        m_battleBtn     = FindChildComponent<UIButton>("Battle");

//        m_teamItem      = FindChildComponent<UILabel>("Item");
//        m_noMainTips    = FindChildComponent<UILabel>("NoMainTips");
        m_expText       = FindChildComponent<UILabel>("ExpText");

        m_expBar        = FindChildComponent<UIProgressBar>("ExpBar");
        m_staminaText   = FindChildComponent<UILabel>("StaminaText");
        m_staminaBar    = FindChildComponent<UIProgressBar>("StaminaBar");
        m_staminaTimingText = FindChildComponent<UILabel>("TiminglText");
        m_levelText     = FindChildComponent<UILabel>("LevelText");
        m_helper        = FindChildComponent<UIPanel>("Helper");
        m_helperItem    = FindChildComponent<UIToggle>("HelperItem");
        m_helpGrid      = FindChildComponent<UIGrid>("HelperGrid");
        m_helperCardName = FindChildComponent<UILabel>("HelperCardName");
        m_helperCardLevel = FindChildComponent<UILabel>("HelperLevel");
        m_helperCardRace = FindChildComponent<UILabel>("HelperRace");
        m_helperCardOcc = FindChildComponent<UILabel>("HelperOcc");
        m_helperPhyAttack = FindChildComponent<UILabel>("HelperPhyAttack");
        m_helperMagAttack = FindChildComponent<UILabel>("HelperMagAttack");
        m_helperHP        = FindChildComponent<UILabel>("HelperHP");
        m_helperModelFather = FindChildComponent<UILabel>("HelpModel");
        m_heleperBar        = FindChildComponent<UIProgressBar>("HelperBar");

        UIStageMenuTeamItem item        = UIStageMenuTeamItem.Create();
        item.SetParent(m_battlePreparation.transform);
        m_teamList.Add(0, item);
	}

    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("Return", OnReturn);

        AddChildMouseClickEvent("Battle", OnBattle);

        AddChildMouseClickEvent("HelperConfirm", OnHelperConfirm);

    }
    public override void OnDestroy()
    {
      
        base.OnDestroy();

        foreach (UIStageMenuStageItem item in m_itemList)
        {
            item.Destroy();
        }

        HideAllModel();


        foreach (UIStageMenuFloorItem item in m_floorItemList)
        {
            item.Destroy();
        }

        foreach (KeyValuePair<int, UIStageMenuTeamItem> item in m_teamList)
        {
            item.Value.Destroy();
        }
        
        foreach (KeyValuePair<int, UIToggle> item in m_helperList)
        {
            GameObject.Destroy(item.Value);
        }

        UIManager.Singleton.HideModel(m_helperModelData.m_obj as GameObject);
    }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
       if (eventType == (int)StageMenu.ENPropertyChanged.enChangeTeam)
       {
            if (false == WindowRoot.activeSelf)
            {
                return;
            }
           bool up = (bool)eventObj;
           ChangeTeam(up);
           UpdateBattlePreparation();
       }
        else if (eventType == (int)StageMenu.ENPropertyChanged.enHide)
        {
            HideFloor();
            HideWindow();
        }
        else if (eventType == (int)StageMenu.ENPropertyChanged.enSetCurHelperGuid)
        {
            FocusOnCurHelperGuid();
        }
      
    }


    void OnPropertySceneChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        // 进入副本
        if ( eventType == (int)SM.RandomRoomLevel.ENPropertyChanged.enEnterDungeon )
        {
            //隐藏LOADING
            Loading.Singleton.Hide();

            // 进入副本
            OnEnterBattle();
        }
    }
    void OnReturn(GameObject obj)
    {
       // 避免快速点击时出现 问题
        if (Time.time - m_lastClickTime < 0.5f)
        {
            return;
        }
        m_lastClickTime = Time.time;

        if (m_curType == (int)CurType.enFloorType)
        {
            HideFloor();
            UpdateStageInfo();
        }
        else if (m_curType == (int)CurType.enStageType)
        {
            MainMenu.Singleton.ShowMainMenu();
            MainUIManager.Singleton.HideCurWindow();
        }
        else if (m_curType == (int)CurType.enBattleType)
        {
            HideBattlePreparation();
            UpdateHelper();
           // UpdateFloorInfo(StageMenu.Singleton.m_curStageId);
        }
        else if (m_curType == (int)CurType.enHelperType)
        {
            HideHelper();
            UpdateFloorInfo(StageMenu.Singleton.m_curStageId);
        }  
    }

    // 准备战斗
    void OnBattle(object sender)
    {
        if (null == Team.Singleton.Chief)
        {
            return;
        }
        if (m_inputCamp != null)
        {
            StageMenu.Singleton.m_camp = int.Parse(m_inputCamp.value);
        }
        if (m_inputKey != null)
        {
            StageMenu.Singleton.m_key = m_inputKey.value;
        }
        if (null != m_isRolling)
        {
            StageMenu.Singleton.m_isRolling = m_isRolling.value;
        }
   
        BattleArena.Singleton.PrepareEnterDungeons();
 
    }
    
     // 服务器回调 进入游戏
    void OnEnterBattle()
    {
        MainGame.Singleton.CurrentState.SwitchNexted    = true;
        StageMenu.Singleton.m_isReqHelpData             = true;
    }

    // 进入战斗准备界面
    void OnHelperConfirm(object sender)
    {
        HideHelper();

        CSItem item     = null;

        Helper helper   = User.Singleton.HelperList.LookupHelper(StageMenu.Singleton.m_curHelperGuid);
        if (null != helper)
        {
            item            = new CSItem();
            item.m_guid     = helper.m_cardGuid;
            item.IDInTable  = (short)helper.m_cardId;
            item.Level      = helper.m_cardLevel;
            item.BreakCounts= helper.m_cardBreakCounts;

            item.Init();

            // 测试用
            HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(helper.m_cardId);
            if (null != heroInfo)
            {
                List<int> skillIDList = heroInfo.GetAllSkillIDList();
                foreach (int skillId in skillIDList)
                {
                    HeroCardSkill info = new HeroCardSkill();
                    info.m_skillID = skillId;

                    item.AddSkill(info);
                }
                foreach (var item1 in heroInfo.PassiveSkillIDList)
                {//被动技能
                    HeroCardSkill info = new HeroCardSkill();
                    info.m_skillID = item1;

                    item.AddSkill(info);
                }
            }
        }
       
        Team.Singleton.Comrade = item;
        BattleArena.Singleton.PrepareEnterDungeons();
        //UpdateBattlePreparation();
    }

    public override void OnHideWindow()
    {
        base.OnHideWindow();
        HidePrepare();
    }
    public override void OnShowWindow()
    {
        base.OnShowWindow();
        Test();
//         UpdateStageInfo();
//         if (m_inputCamp != null)
//         {
//             m_inputCamp.value = StageMenu.Singleton.m_camp.ToString();
//         }
//         if (m_inputKey != null)
//         {
//             m_inputKey.value = StageMenu.Singleton.m_key;
//         }
    }
    // 用来更新 体力恢复倒计时
    override public void OnUpdate()
    {
        int recoverTime = User.Singleton.GetStaminaRecoverTime();
        // 一下点体力恢复时间
        if ( recoverTime == 0 )
        {
            m_staminaTimingText.text = "";

            // 更新 体力
            m_staminaText.text = User.Singleton.GetStamina() + "/" + User.Singleton.GetMaxStamina();

            // 体力进度条
            m_staminaBar.value = (float)User.Singleton.GetStamina() / (float)User.Singleton.GetMaxStamina();
        }
        else
        {
            int nMinite     = recoverTime / 60;
            int nSec        = recoverTime - nMinite * 60;
            m_staminaTimingText.text = nMinite + ":" + string.Format("00", nSec);
        }

        if (m_inputCamp != null && !string.IsNullOrEmpty(m_inputCamp.value))
        {
            int value = int.Parse(m_inputCamp.value);
            if (value != StageMenu.Singleton.m_camp)
            {
                StageMenu.Singleton.m_camp = value;
            }
        }
        if (m_inputKey != null && !string.IsNullOrEmpty(m_inputKey.value))
        {
            string value = m_inputKey.value;
            if (value != StageMenu.Singleton.m_key)
            {
                StageMenu.Singleton.m_key = value;
            }
        }
    }

    //  更新stage相关
    void UpdateStageInfo()
    {

        int expPercent      = User.Singleton.GetExp()*100/User.Singleton.GetMaxExp();
        // 经验
        m_expText.text      = expPercent + "%";

        //经验进度条
        m_expBar.value      = (float)expPercent / 100f;

        // 体力
        m_staminaText.text  = User.Singleton.GetStamina()+"/"+User.Singleton.GetMaxStamina();

        // 体力进度条
        m_staminaBar.value  = (float)User.Singleton.GetStamina() / (float)User.Singleton.GetMaxStamina();

        m_levelText.text    = Localization.Get("CardLevel") + User.Singleton.GetLevel();

        ZoneTableInfo zoneInfo  = GameTable.ZoneInfoTableAsset.LookUp(StageMenu.Singleton.m_fatherZoneId);
        if ( null == zoneInfo )
        {
            return;
        }

        m_name.text             = zoneInfo.m_name;

        List<int> stageList     = new List<int>();

        foreach ( int stageId in zoneInfo.m_stageList )
        {
            stageList.Add(stageId);
        }

        foreach (UIStageMenuStageItem item in m_itemList)
        {
            item.Destroy();
        }

        m_itemList.Clear();

        int pos    = 0;
        for (int i = stageList.Count - 1; i >= 0; i--)
        {
            int stageId                 = stageList[i];

            UIStageMenuStageItem item = UIStageMenuStageItem.Create();

            // 设置 父物体
            item.SetParent(m_grid.transform);

            // 设置名称
            item.WindowRoot.transform.name = stageList[i].ToString();

            item.Update(stageId);

            UIEventListener.Get(item.WindowRoot).onClick = OnChooseStage;

            m_itemList.Add(item);
            item.WindowRoot.transform.LocalPositionX(600f);
            item.WindowRoot.transform.LocalPositionY(pos * (-66.3f));
            item.WindowRoot.transform.LocalPositionZ(0f);

            pos++;
        }

        int index = 0;

        foreach (UIStageMenuStageItem item in m_itemList)
        {
            TweenPosition temp = item.WindowRoot.transform.GetComponent<TweenPosition>();
            if (null == temp)
            {
                continue;
            }
            temp.delay = 0.03f * index;
            index++;

            Vector3 posV = item.WindowRoot.transform.localPosition;
            posV.x = 0;

            TweenPosition.Begin(item.WindowRoot, 0.5f, posV).method = UITweener.Method.EaseIn;
        }

        m_curType = (int)CurType.enStageType;
        RefreshJoystickAutoMap();
    }

    // 选择stage响应
    void OnChooseStage( GameObject button )
    {
        int stageId = int.Parse(button.name);

        HideStage();

        UpdateFloorInfo(stageId);
        
    }

    //  选择floor响应
    void OnChooseFloor(GameObject sender)
    {
        // 进入floor前 如果背包 获得卡牌 大于背包容量 则 跳转
        if (CardBag.Singleton.IsPopSizeMax())
        {
            PopWnd.Singleton.Show();
            return;
        }

       int floorId      = int.Parse(sender.name);
       FloorInfo info   = GameTable.FloorInfoTableAsset.LookUp(floorId);
       if (null == info)
       {
           Debug.Log("OnChooseFloor FloorInfo info==null" + floorId);
           return;
       }
     
       SM.RandomRoomLevel.Singleton.m_curFloorId = floorId;

        //
       if (StageMenu.Singleton.m_isReqHelpData)
       {
           // 请求数据前 清空下数据
           User.Singleton.HelperList.Clear();
           // 发送服务器 请求战友列表
           //int msgID = MiniServer.Singleton.req_battleHelperList();
 
           //RegisterRespondFuncByMessageID(msgID, OnHelperCallback);  

           HideFloor();
           UpdateHelper();
       }
       else
       { 
           HideFloor();
           UpdateHelper();
       }
        
    }
    public void Test()
    {
        int floorId = 20101;
        FloorInfo info = GameTable.FloorInfoTableAsset.LookUp(floorId);
        if (null == info)
        {
            Debug.Log("OnChooseFloor FloorInfo info==null" + floorId);
            return;
        }

        SM.RandomRoomLevel.Singleton.m_curFloorId = floorId;

        //
        if (StageMenu.Singleton.m_isReqHelpData)
        {
            // 请求数据前 清空下数据
            User.Singleton.HelperList.Clear();
            // 发送服务器 请求战友列表
            //int msgID = MiniServer.Singleton.req_battleHelperList();

            //RegisterRespondFuncByMessageID(msgID, OnHelperCallback);  

            HideFloor();
            UpdateHelper();
        }
        else
        {
            HideFloor();
            UpdateHelper();
        }
    }
    // 隐藏stage
    void HideStage()
    {
        int i = 0;
        foreach (UIStageMenuStageItem item in m_itemList)
        {
            TweenPosition temp      = item.WindowRoot.transform.GetComponent<TweenPosition>();
            if ( null == temp )
            {
                continue;
            }

            temp.delay              = 0.03f * i;
            i++;
            Vector3 pos             = item.WindowRoot.transform.localPosition;
            pos.x                   = 600;
            TweenPosition.Begin(item.WindowRoot, 0.5f, pos).method = UITweener.Method.EaseOut;
        }
    }

   // 隐藏FLOOR
    void HideFloor()
    {
        int index = 0;

        foreach (UIStageMenuFloorItem item in m_floorItemList)
        {
            TweenPosition teamp = item.WindowRoot.transform.GetComponent<TweenPosition>();
            if (null == teamp)
            {
                continue;
            }
            teamp.delay = 0.03f * index;
            index++;

            Vector3 posV = item.WindowRoot.transform.localPosition;
            posV.x = 600f;

            TweenPosition.Begin(item.WindowRoot, 0.5f, posV).method = UITweener.Method.EaseIn;
        }
    }

    // 隐藏战斗准备界面
    void HidePrepare()
    {
//         TweenPosition temp = m_battlePreparation.gameObject.transform.GetComponent<TweenPosition>();
//         if (null == temp)
//         {
//             return;
//         }
// 
//         Vector3 posV    = m_battlePreparation.gameObject.transform.localPosition;
//         posV.x          = -1200;
// 
//         m_battlePreparation.gameObject.transform.LocalPositionX(-1200f);
//         temp            = m_battleBtn.gameObject.transform.GetComponent<TweenPosition>();
//         if (null == temp)
//         {
//             return;
//         }
// 
//         posV            = m_battleBtn.gameObject.transform.localPosition;
//         posV.x          = 900;
//         m_battleBtn.gameObject.transform.LocalPositionX(900f);

        foreach (KeyValuePair<int, UIStageMenuTeamItem> item in m_teamList)
        {
            item.Value.HideWindow();
        }


        m_battleBtn.gameObject.SetActive(false);
    }

    // 显示floor
    void UpdateFloorInfo( int stageId )
    {
        m_heleperBar.value = 0;

        StageMenu.Singleton.m_curStageId                    = stageId;

        StageDetailInfo stageDetailInfo = GameTable.StageInfoTableAsset.LookUp(stageId);
        if (null == stageDetailInfo)
        {
            return;
        }

        m_name.text         = stageDetailInfo.m_name;

        List<int> floorList = new List<int>();

        foreach (int floorId in stageDetailInfo.m_floorList)
        {

            floorList.Add(floorId);
            
        }

        foreach (UIStageMenuFloorItem item in m_floorItemList)
        {
            item.Destroy();
        }

        m_floorItemList.Clear();

        int pos     = 0;
        for (int i = floorList.Count - 1; i >= 0; i--)
        {
            int floorId         = floorList[i];

            UIStageMenuFloorItem floorItem = UIStageMenuFloorItem.Create();

            // 设置 父物体
            floorItem.SetParent(m_grid.transform);
            // 设置名称
            floorItem.WindowRoot.transform.name = floorList[i].ToString();

            floorItem.Update(floorId);

            UIEventListener.Get(floorItem.WindowRoot.gameObject).onClick = OnChooseFloor;

            m_floorItemList.Add(floorItem);

            floorItem.WindowRoot.transform.LocalPositionX(600f);
            floorItem.WindowRoot.transform.LocalPositionY(pos * (-66.3f));
            floorItem.WindowRoot.transform.LocalPositionZ(0f);

            TweenPosition temp = floorItem.WindowRoot.transform.GetComponent<TweenPosition>();
            if (null == temp)
            {
                continue;
            }
            temp.delay = 0.03f * pos;

            Vector3 posV = floorItem.WindowRoot.transform.localPosition;
            posV.x = 0;

            TweenPosition.Begin(floorItem.WindowRoot, 0.5f, posV).method = UITweener.Method.EaseIn;

            pos++;
         }

        m_curType = (int)CurType.enFloorType;

        RefreshJoystickAutoMap();
    }

    void HideAllModel()
    {
        foreach (var item in m_modelDataList)
        {
            UIManager.Singleton.HideModel(item.m_obj as GameObject);
        }
        m_modelDataList.Clear();
    }

    // 显示更新 战斗准备
    void UpdateBattlePreparation()
    {

        for (int i = 0; i < Team.Singleton.TeamNum; ++i)
        {
            if ( false == m_teamList.ContainsKey(i) )
            {
                UIStageMenuTeamItem item = UIStageMenuTeamItem.Create();
                item.SetParent(m_battlePreparation.transform);
                m_teamList.Add(i, item);
            }
        }

        int curTeamIndex = Team.Singleton.m_curTeamIndex;

        foreach (KeyValuePair<int, UIStageMenuTeamItem> item in m_teamList)
        {
            if (item.Key == curTeamIndex)
            {
                item.Value.ShowWindow();
            }
            else
            {
                item.Value.HideWindow();
            }
        }

        HideAllModel();
       
        if (!m_teamList.ContainsKey(curTeamIndex))
        {
            Team.Singleton.m_curTeamIndex   = 0;
            curTeamIndex                    = 0;
        }

        UIStageMenuTeamItem teamItem        = m_teamList[curTeamIndex];
        teamItem.ShowWindow();

        AddModel(Team.Singleton.GetCard(curTeamIndex, Team.EDITTYPE.enMain)     , teamItem.m_mainModelObj);
        AddModel(Team.Singleton.GetCard(curTeamIndex, Team.EDITTYPE.enDeputy)   , teamItem.m_deputyModelObj);
        AddModel(Team.Singleton.GetCard(curTeamIndex, Team.EDITTYPE.enSupport)  , teamItem.m_supportModelObj);
        AddModel(Team.Singleton.Comrade, teamItem.m_comradeModelObj);

        UILabel mainTips    = teamItem.m_mainTips;
        UILabel deputyTips  = teamItem.m_deputyTips;
        UILabel supportTips = teamItem.m_supportTips;
        UILabel comradeTips = teamItem.m_comradeTips;

        HeroInfo info       = null;
        CSItem csItem       = null;
      
        csItem              = Team.Singleton.Chief;
        // 主角色
        if (null != csItem)
        {
            m_battleBtn.isEnabled = true;

            info = GameTable.HeroInfoTableAsset.Lookup(csItem.IDInTable);
            if (null != info)
            {
                OccupationInfo occupationInfo = GameTable.OccupationInfoAsset.LookUp(info.Occupation);
                if (null != occupationInfo)
                {
                    mainTips.text = Localization.Get("CardLevel") + csItem.Level + "\n";
                    mainTips.text += Localization.Get("Occ：")    + occupationInfo.m_name + "\n";
                    mainTips.text += Localization.Get("Hp：")     + csItem.GetHp() + "\n";
                    mainTips.text += Localization.Get("PhyAtk：") + csItem.GetPhyAttack() + "\n";
                    mainTips.text += Localization.Get("MagAtk：") + csItem.GetMagAttack() + "\n";
                    mainTips.text += Localization.Get("Def：") + csItem.GetPhyDefend();
                }
            }

            teamItem.m_noMainTipsLab.SetActive(false);
        }
        // 没有主角色 不可以进行战斗
        else
        {
            m_battleBtn.isEnabled = false;
            teamItem.m_noMainTipsLab.SetActive(true);

        }

        csItem          = Team.Singleton.Deputy;
        // 副角色
        if (null != csItem)
        {
            info = GameTable.HeroInfoTableAsset.Lookup(csItem.IDInTable);
            if (null != info)
            {
                OccupationInfo occupationInfo = GameTable.OccupationInfoAsset.LookUp(info.Occupation);
                if (null != occupationInfo)
                {
                    deputyTips.text = Localization.Get("CardLevel")  + csItem.Level + "\n";
                    deputyTips.text += Localization.Get("Occ：")     + occupationInfo.m_name + "\n";
                    deputyTips.text += Localization.Get("Hp：")      + csItem.GetHp() + "\n";
                    deputyTips.text += Localization.Get("PhyAtk：")  + csItem.GetPhyAttack() + "\n";
                    deputyTips.text += Localization.Get("MagAtk：")  + csItem.GetMagAttack() + "\n";
                    deputyTips.text += Localization.Get("Def：") + csItem.GetPhyDefend();
                }
            }
        }

        csItem          = Team.Singleton.Support;
        // 支援角色
        if (null != csItem)
        {
            info = GameTable.HeroInfoTableAsset.Lookup(csItem.IDInTable);
            if (null != info)
            {
                OccupationInfo occupationInfo = GameTable.OccupationInfoAsset.LookUp(info.Occupation);
                if (null != occupationInfo)
                {
                    supportTips.text = Localization.Get("CardLevel") + csItem.Level + "\n";
                    supportTips.text += Localization.Get("Occ：")    + occupationInfo.m_name + "\n";
                    supportTips.text += Localization.Get("Hp：")     + csItem.GetHp() + "\n";
                    supportTips.text += Localization.Get("PhyAtk：") + csItem.GetPhyAttack() + "\n";
                    supportTips.text += Localization.Get("MagAtk：") + csItem.GetMagAttack() + "\n";
                    supportTips.text += Localization.Get("Def：") + csItem.GetPhyDefend();
                }
            }
        }

        csItem          = Team.Singleton.Comrade;
        // 战友
        if (null != csItem)
        {
            info = GameTable.HeroInfoTableAsset.Lookup(csItem.IDInTable);
            if (null != info)
            {
                OccupationInfo occupationInfo = GameTable.OccupationInfoAsset.LookUp(info.Occupation);
                if (null != occupationInfo)
                {
                    comradeTips.text = Localization.Get("CardLevel") + csItem.Level + "\n";
                    comradeTips.text += Localization.Get("Occ：")    + occupationInfo.m_name + "\n";
                    comradeTips.text += Localization.Get("Hp：")     + csItem.GetHp() + "\n";
                    comradeTips.text += Localization.Get("PhyAtk：") + csItem.GetPhyAttack() + "\n";
                    comradeTips.text += Localization.Get("MagAtk：") + csItem.GetMagAttack() + "\n";
                    comradeTips.text += Localization.Get("Def：") +  csItem.GetPhyDefend();
                }
            }
        }
       

   //      TweenPosition temp = m_battlePreparation.gameObject.transform.GetComponent<TweenPosition>();
//         if (null == temp)
//         {
//             return;
//         }
// 
//         Vector3 posV = m_battlePreparation.gameObject.transform.localPosition;
//         posV.x = 0;
// 
//         TweenPosition.Begin(m_battlePreparation.gameObject, 0.5f, posV).method = UITweener.Method.EaseIn;

       

//         temp = m_battleBtn.gameObject.transform.GetComponent<TweenPosition>();
//         if (null == temp)
//         {
//             return;
//         }
// 
//         posV = m_battleBtn.gameObject.transform.localPosition;
//         posV.x = 330;
// 
//         TweenPosition.Begin(m_battleBtn.gameObject, 0.5f, posV).method = UITweener.Method.EaseIn;

        //m_battlePreparation.gameObject.SetActive(true);
        m_battleBtn.gameObject.SetActive(true);
        m_level.gameObject.SetActive(false);

        TweenPosition  temp = m_name.gameObject.transform.GetComponent<TweenPosition>();
        if (null == temp)
        {
            return;
        }

        Vector3 posV = m_name.gameObject.transform.localPosition;
        posV.x = -700;

        TweenPosition.Begin(m_name.gameObject, 0.5f, posV).method = UITweener.Method.EaseIn;

        m_curType = (int)CurType.enBattleType;
    }

    void AddModel( CSItem card ,GameObject parent )
    {
        if (null == card || parent == null)
        {
            return;
        }
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        m_modelDataList.Add(data);
        UIManager.Singleton.AddModel(card.IDInTable, parent, data);
    }

    // 添加 战友模型
    void AddHelperModel( int cardId )
    {
        UIManager.Singleton.HideModel(m_helperModelData.m_obj as GameObject);
        m_helperModelData.m_isFinish = false;
        UIManager.Singleton.AddModel(cardId, m_helperModelFather.gameObject, m_helperModelData);
    }
    // 隐藏战斗准备界面
    void HideBattlePreparation()
    {
//         TweenPosition temp = m_battlePreparation.gameObject.transform.GetComponent<TweenPosition>();
//         if (null == temp)
//         {
//             return;
//         }
// 
//         Vector3 posV = m_battlePreparation.gameObject.transform.localPosition;
//         posV.x = -1200;
// 
//         TweenPosition.Begin(m_battlePreparation.gameObject, 0.5f, posV).method = UITweener.Method.EaseIn;
// 
// 
//         temp = m_battleBtn.gameObject.transform.GetComponent<TweenPosition>();
//         if (null == temp)
//         {
//             return;
//         }
// 
//         posV = m_battleBtn.gameObject.transform.localPosition;
//         posV.x = 900;
// 
//         TweenPosition.Begin(m_battleBtn.gameObject, 0.5f, posV).method = UITweener.Method.EaseIn;

       
        foreach (KeyValuePair<int, UIStageMenuTeamItem> item in m_teamList)
        {
            item.Value.HideWindow();
        }


        m_battleBtn.gameObject.SetActive(false);

    }

    // 更换队伍
    void ChangeTeam(bool up)
    {
       
      
    }

    // 更新显示战友列表
    void UpdateHelper()
    {
       
        m_helper.gameObject.SetActive(true);

        // 等级和关卡名称显示出来
        m_level.gameObject.SetActive(true);

        TweenPosition temp = m_name.gameObject.transform.GetComponent<TweenPosition>();
        if (null == temp)
        {
            return;
        }

       Vector3 posV = m_name.gameObject.transform.localPosition;
        posV.x = -322.2004f;

        TweenPosition.Begin(m_name.gameObject, 0.5f, posV).method = UITweener.Method.EaseIn;


        // 列表索引
        int index       = 0;
        GameObject obj  = null;

        // 冒险者
        foreach (Helper item in User.Singleton.HelperList.BattleHelpers)
        {
            if (item.m_type != (int)HelpType.enAdventurers)
            {
                continue;
            }

            obj = AddHelpList(index, item);
            if ( null == obj )
            {
                continue;
            }

            // 战友ID
            obj.GetComponent<Parma>().m_id = item.m_userGuid;

            obj.GetComponent<UIButton>().normalSprite                                   = "i-interface-selectfriends-adventurer-E-1";
            // 背景图片不一样
            //obj.transform.FindChild("Background").GetComponent<UISprite>().spriteName   = "i-interface-selectfriends-adventurer-E-1";

            // 点选图片不一样
            obj.transform.Find("Checkmark").GetComponent<UISprite>().spriteName    = "i-interface-selectfriends-adventurer-E-2";

            // 玩家名称
            obj.transform.Find("HelperName").GetComponent<UILabel>().text          = item.m_userName;

            // 冒险者
            obj.transform.Find("HelperType").GetComponent<UILabel>().text          = Localization.Get("friendtype1");
           
            // 已被选用次数
            UILabel tempLabel   = obj.transform.Find("ChosenNum").GetComponent<UILabel>();
            tempLabel.text      = "";

            // 友情点数
            int point           = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enAdventurerFriendPoint).IntTypeValue;
            tempLabel           = obj.transform.Find("FriendPoint").GetComponent<UILabel>();
            tempLabel.text      = "";
            tempLabel.text      = string.Format(Localization.Get("getfriendpt"), point);
                 
           

            // 玩家代表卡牌的头像
            HeroInfo info       = GameTable.HeroInfoTableAsset.Lookup(item.m_cardId);
            if ( null == info )
            {
                continue;
            }

            IconInfomation iconInfo   = GameTable.IconInfoTableAsset.Lookup(info.headImageId);
            if (null == iconInfo)
            {
                continue;
            }
            obj.transform.Find("HelperPic").GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);

            index++;
        }

        // 好友
        foreach (Helper item in User.Singleton.HelperList.BattleHelpers)
        {
            if (item.m_type == (int)HelpType.enAdventurers)
            {
                continue;
            }
                
            obj             = AddHelpList(index, item);
            if (null == obj)
            {
                continue;
            }

            // 战友ID
            obj.GetComponent<Parma>().m_id = item.m_userGuid;

            // 玩家名称
            obj.transform.Find("HelperName").GetComponent<UILabel>().text = item.m_userName;

            string str      = "";
            // 战友类型
            if (item.m_type == (int)HelpType.enFriend)
            {
                str         = Localization.Get("friendtype2");
            }
            else if (item.m_type == (int)HelpType.enGroupMember)
            {
                str         = Localization.Get("friendtype3");
            }

            obj.transform.Find("HelperType").GetComponent<UILabel>().text = str;

            // 已被选用次数
            UILabel tempLabel   = obj.transform.Find("ChosenNum").GetComponent<UILabel>();
            tempLabel.text      = "";
            tempLabel.text      = string.Format(Localization.Get("selecttime"), item.m_chosenNum);

            // 友情点数
            int point           = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enFriendPoint).IntTypeValue;
            tempLabel           = obj.transform.Find("FriendPoint").GetComponent<UILabel>();
            tempLabel.text      = "";
            tempLabel.text      = string.Format(Localization.Get("getfriendpt"), point);

            // 玩家代表卡牌的头像
            HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(item.m_cardId);
            if (null == info)
            {
                continue;
            }

            IconInfomation iconInfo = GameTable.IconInfoTableAsset.Lookup(info.headImageId);
            if (null == iconInfo)
            {
                continue;
            }

            obj.transform.Find("HelperPic").GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);

            index++;
        }

        // 隐藏剩下的
        for (; index < m_helperList.Count;index++ )
        {
            if (m_helperList.ContainsKey(index))
            {
                m_helperList[index].gameObject.SetActive(false);
            }
        }

        // 先默认 选中第一个
        FocusOnFirstHelper();

        // 更新战友模型相关
        UpdateHelperModel();
       
       
        m_helpGrid.Reposition();

        m_curType = (int)CurType.enHelperType;

    }

    // 隐藏战友相关
    void HideHelper()
    {
        m_helper.gameObject.SetActive(false);

        int index = 0;

        foreach (KeyValuePair<int, UIToggle> item in m_helperList)
        {
            TweenPosition pos = item.Value.transform.GetComponent<TweenPosition>();
            if (null == pos)
            {
                continue;
            }
            pos.delay = 0.03f * index;
            index++;

            Vector3 posV = item.Value.gameObject.transform.localPosition;
            posV.x = 600f;

            TweenPosition.Begin(item.Value.gameObject, 0.5f, posV).method = UITweener.Method.EaseIn;
        }
         
    }

    // 添加更新 战友Item
    GameObject AddHelpList(int index,Helper helper)
    {
        UIToggle copy                   = null;

        // 如果存在 则更新
        if (m_helperList.ContainsKey(index))
        {
            copy                        = m_helperList[index];
        }
        // 不存在 则添加
        else
        {
            copy                        = GameObject.Instantiate(m_helperItem) as UIToggle;

            // 设置 父物体
            copy.transform.parent       = m_helpGrid.transform;

            // 设置大小
            copy.transform.localScale   = m_helperItem.transform.localScale;

            // 设置名称
            copy.transform.name         = index.ToString("00000");
            
            m_helperList.Add(index, copy);

            UIEventListener.Get(copy.gameObject).onClick = OnChooseHelper;

            AddMouseClickEvent(copy.transform.Find("HelperPic").gameObject, OnClickHelpPic);
        }

        copy.gameObject.SetActive(true);

        return copy.gameObject;
    }

    
    //  选择helper响应
    void OnChooseHelper(GameObject sender)
    {
        Parma parma     = sender.GetComponent<Parma>();

        Helper helper   = User.Singleton.HelperList.LookupHelper(parma.m_id);

        if (null == helper)
        {
            return;
        }

        StageMenu.Singleton.m_curHelperGuid = helper.m_userGuid;

        // 更新模型相关信息
        UpdateHelperModel();
   
    }

    // 点击战友头像
    void OnClickHelpPic(object obj, EventArgs e)
    {
        GameObject gameObj  = (GameObject)obj;
        Parma param         = gameObj.transform.parent.GetComponent<Parma>();
        UIRepresentativeCard.GetInstance().ShowGUID(param.m_id, UIRepresentativeCard.ENOptType.enRepreType);
    }

    // 把点选放在第一个上
    void FocusOnFirstHelper()
    {
        // 只有在第一次的时候才这样做 
       if ( false == m_isFirstChooseHelper)
       {
           return;
       }

        m_isFirstChooseHelper = false;

        int index = 0;

        foreach (KeyValuePair<int, UIToggle> item in m_helperList)
        {

            if ( index == 0 )
            {
                item.Value.value                    = true;
                StageMenu.Singleton.m_curHelperGuid = item.Value.GetComponent<Parma>().m_id;
            }
            else
            {
                item.Value.value = false;
            }

            index++;
        }
    }

    // 把焦点放在 当前选择的 战友
    void FocusOnCurHelperGuid()
    {
        foreach (KeyValuePair<int, UIToggle> item in m_helperList)
        {
            item.Value.value = item.Value.GetComponent<Parma>().m_id == StageMenu.Singleton.m_curHelperGuid;
        }
        UpdateHelperModel();
    }


    // 更新战友模型相关信息
    void UpdateHelperModel()
    {
        Helper helper               = User.Singleton.HelperList.LookupHelper(StageMenu.Singleton.m_curHelperGuid);
        if ( null != helper )
        {
            AddHelperModel(helper.m_cardId);

            HeroInfo heroInfo       = GameTable.HeroInfoTableAsset.Lookup(helper.m_cardId);
            if ( null != heroInfo )
            {
                m_helperCardName.text   = heroInfo.StrName;
                m_helperCardLevel.text  = Localization.Get("CardLevel") + helper.m_cardLevel;
                RaceInfo raceInfo       = GameTable.RaceInfoTableAsset.LookUp(heroInfo.Type);
                m_helperCardRace.text   = raceInfo.m_name;
                OccupationInfo occInfo  = GameTable.OccupationInfoAsset.LookUp(heroInfo.Occupation);
                m_helperCardOcc.text    = occInfo.m_name;

                AttrRatioInfo rarityInfo = GameTable.attrRatioTableAsset.LookUp(heroInfo.Occupation, heroInfo.Rarity);
                // 计算生命值
                int hp = (int)((heroInfo.FHPMax + rarityInfo.m_hPMaxAdd * (helper.m_cardLevel-1)) * rarityInfo.m_hpMutiply);

                m_helperHP.text         = hp.ToString();
                

                // 计算物理攻击力
                int attack  = BattleFormula.GetPhyAttack(helper.m_cardId, 1);
                m_helperPhyAttack.text = attack.ToString();

                // 魔法攻击力
                attack      = BattleFormula.GetMagAttack(helper.m_cardId, 1);
                m_helperMagAttack.text = attack.ToString();
               

               
            }
        }
       
    }

    // 请求战友数据的回调
    public void OnHelperCallback(MessageRespond respond)
    {
        Debug.Log("OnHelperCallback:" + respond.IsSuccess);
        if (false == respond.IsSuccess)
        {
            return;
        }

        StageMenu.Singleton.m_isReqHelpData = false;

        HideFloor();
        UpdateHelper();
    }
}
