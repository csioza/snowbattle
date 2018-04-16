using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//战斗场地信息-包括基础数据信息统计、单机关卡信息、竞技场信息统计、MOBA信息统计等等组成
public class BattleArena : IPropertyObject
{
    int m_beKilledNum = 0;// 主角的被杀数量
    int m_killEnemyNum = 0;// 主角的杀敌数量
    public enum ENPropertyChanged
    {
        enNone,
        enComboCount = 1,
        enComboAppraise = 2,
        enUrgentEvent = 3,
        enMultiKillNPC = 4,
        enQteSequence = 5,
        enSkillCombo = 6,
        enSwordSoul = 7,
    }
    #region Singleton
    static BattleArena m_singleton;
    static public BattleArena Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new BattleArena();
            }
            return m_singleton;
        }
    }
    #endregion
    #region ComboProps//主控角色的ComboProps
    private ComboProps m_combo;
    public ComboProps Combo
    {
        get
        {
            if (m_combo == null)
            {
                m_combo = new ComboProps();
            }
            return m_combo;
        }
        set
        {
            m_combo = value;
        }
    }
    #endregion
    #region SkillComboProps//主控角色的SkillComboProps
    private SkillComboProps m_skillComboProps;
    public SkillComboProps SkillCombo
    {
        get
        {
            if (m_skillComboProps == null)
            {
                m_skillComboProps = new SkillComboProps();
            }
            return m_skillComboProps;
        }
        set
        {
            m_skillComboProps = value;
        }
    }
    #endregion
    /*#region QTEProps//主控角色的QTEProps
    private QTEProps m_qte;
    public QTEProps QTE
    {
        get
        {
            if (m_qte == null)
            {
                m_qte = new QTEProps();
            }
            return m_qte;
        }
        set
        {
            m_qte = value;
        }
    }
    #endregion
    */
    #region SwordSoul//主控角色的SwordSoulProps
    private SwordSoulProps m_swordSoulProps;
    public SwordSoulProps SwordSoul
    {
        get
        {
            if (m_swordSoulProps == null)
            {
                m_swordSoulProps = new SwordSoulProps();
            }
            return m_swordSoulProps;
        }
        set
        {
            m_swordSoulProps = value;
        }
    }
    #endregion
    #region ReliveCount//复活次数
    private int m_reliveCount;
    public int ReliveCount { get { return m_reliveCount; } set { m_reliveCount = value; } }
    #endregion
    #region KillBossCount//击杀boss数量
    private int m_killBossCount;
    public int KillBossCount { get { return m_killBossCount; } set { m_killBossCount = value; } }
    #endregion
    #region StageStartTime//关卡开始时间
    private float m_stageStartTime = 0;
    public float StageStartTime { get { return m_stageStartTime; } private set { m_stageStartTime = value; } }
    #endregion
    #region MaxComboNumber//combo最大值
    public int MaxComboNumber { get { return Combo.MaxComboNumber; } private set { Combo.MaxComboNumber = value; } }
    #endregion
    #region SwitchSkillCount//切入技次数
    private int m_switchSkillCount = 0;
    public int SwitchSkillCount { get { return m_switchSkillCount; } set { m_switchSkillCount = value; } }
    #endregion
    #region KillNpcCount//击杀NPC数量
    private float m_killNpcStartTime = 0;
    private float m_killNpcJudgeTime = 0;
    float KillNpcJudgeTime
    {
        get
        {
            if (m_killNpcJudgeTime == 0)
            {
                m_killNpcJudgeTime = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enKillNPC_JudgeTime).FloatTypeValue;
            }
            return m_killNpcJudgeTime;
        }
    }
    private int m_killNpcCount;
    public int KillNpcCount
    {
        get { return m_killNpcCount; }
        set
        {
            if (value == 1)
            {
                m_killNpcStartTime = Time.time;
            }
            m_killNpcCount = value;
        }
    }
    #endregion
    //原地复活次数
    public int m_origialCount = 1;
    //可以原地复活
    public bool m_canOrigialResurrection = true;
    public bool m_canRetLayerResurrection = true;

    public LevelBlackboard m_blackBoard = new LevelBlackboard();
    #region MainPlayer
    private Actor MainPlayer
    {
        get
        {
            return ActorManager.Singleton.MainActor;
        }
    }
    #endregion
    //切入技的步骤
    public ENSwitchStep SwitchStep = ENSwitchStep.enNone;
    //red flash
    private float m_redFlashTriggerPercent = 0;
    private bool m_isRedFlash = false;
    public bool m_roomIsRolling = true;
    //获得的掉落物品列表
    public List<int> DropCardsIdList = new List<int>();
    public BattleArena()
    {
        SetPropertyObjectID((int)MVCPropertyID.enBattlePropsManager);
        m_redFlashTriggerPercent = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enUrgentEventParam).IntTypeValue;
        m_isRedFlash = false;
    }
    public void FixedUpdate()
    {
        Combo.Tick();
        SkillCombo.Tick();
        //QTE.Tick();
        //SoulCharge.Tick();
        float fPercent = MainPlayer.HP * 100.0f / MainPlayer.MaxHP;
        if (fPercent <= m_redFlashTriggerPercent)
        {
            if (!m_isRedFlash)
            {
                m_isRedFlash = true;
                NotifyChanged((int)BattleArena.ENPropertyChanged.enUrgentEvent, true);
            }
        }
        else
        {
            if (m_isRedFlash)
            {
                m_isRedFlash = false;
                NotifyChanged((int)BattleArena.ENPropertyChanged.enUrgentEvent, false);
            }
        }
        {//Multi Kill NPC
            if (Time.time - m_killNpcStartTime > KillNpcJudgeTime)
            {
                if (KillNpcCount > 1)
                {
                    NotifyChanged((int)BattleArena.ENPropertyChanged.enMultiKillNPC, null);
                }
                KillNpcCount = 0;
            }
        }
        {//qte

        }
    }
    public void StageStart()
    {
        ReliveCount = 0;
        KillBossCount = 0;
        StageStartTime = Time.time;
        MaxComboNumber = 0;
        ResetSwitchSkillCount();
        //QTE.Init();
        SwordSoul.Clear();
        FloorInfo info = GameTable.FloorInfoTableAsset.LookUp(SM.RandomRoomLevel.Singleton.m_curFloorId);
        if (null != info)
        {
            m_origialCount = info.m_origialCount;// info.SceneManger.Singleton.m_curFloorId;
            m_canOrigialResurrection = info.m_canOrigialResurrection == 1 ? true : false;
            m_canRetLayerResurrection = true;
        }
        else
        {
            Debug.Log("OnChooseFloor FloorInfo info==null" + SM.RandomRoomLevel.Singleton.m_curFloorId);
        }
        DropCardsIdList.Clear();
    }
    public void StageStop(bool manual = false, bool victory = true)
    {
        NotifyChanged((int)BattleArena.ENPropertyChanged.enUrgentEvent, false);
        BattleSummary.Singleton.OnFinished(manual, victory);
        //QTE.Stop();
        SwordSoul.Clear();
        Combo.Clear();
        m_blackBoard.Delete();
    }
    private void ResetSwitchSkillCount()
    {
        if (ActorManager.Singleton.Support != null)
        {//切入技恢复
            SwitchSkillCount = ActorManager.Singleton.Support.CurrentTableInfo.SwitchSkillCount;
            SwitchStep = ENSwitchStep.enNone;
        }
    }
    public void Clear()
    {
        if (m_isRedFlash)
        {
            m_isRedFlash = false;
            NotifyChanged((int)BattleArena.ENPropertyChanged.enUrgentEvent, false);
        }
    }

    #region TeleportEffect
    string m_teleportHideEffect;
    public string TeleportHideEffect
    {
        get
        {
            if (string.IsNullOrEmpty(m_teleportHideEffect))
            {
                m_teleportHideEffect = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enTeleportHideEffect).StringTypeValue;
            }
            return m_teleportHideEffect;
        }
    }
    string m_teleportShowEffect;
    public string TeleportShowEffect
    {
        get
        {
            if (string.IsNullOrEmpty(m_teleportShowEffect))
            {
                m_teleportShowEffect = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enTeleportShowEffect).StringTypeValue;
            }
            return m_teleportShowEffect;
        }
    }
    #endregion

    #region KeyCount//获取要是个数
    public Dictionary<int, int> m_keyDic = new Dictionary<int, int>();
    //private int m_keyCount;
    public int KeyCount { get { if (!m_keyDic.ContainsKey(0)) { m_keyDic.Add(0, 0); } return m_keyDic[0]; } set { if (!m_keyDic.ContainsKey(0)) { m_keyDic.Add(0, 0); } m_keyDic[0] = value; } }
    #endregion
    public void SetKeyCount(int keyId, int count)
    {
        if (!m_keyDic.ContainsKey(keyId))
        {
            m_keyDic.Add(keyId, 0);
        }
        m_keyDic[keyId] += count;
    }
    public int GetKeyCountByID(int keyId)
    {
        if (!m_keyDic.ContainsKey(keyId))
        {
            m_keyDic.Add(keyId, 0);
        }
        return m_keyDic[keyId];
    }

    public void SetbeKilledNum(int num)
    {
        m_beKilledNum = num;
    }

    public void SetkillEnemyNum(int num)
    {
        m_killEnemyNum = num;
    }

    public int GetbeKilledNum()
    {
        return m_beKilledNum;
    }

    public int GetkillEnemyNum()
    {
        return m_killEnemyNum;
    }

    public void ClearKillInfo()
    {
        m_beKilledNum = 0;
        m_killEnemyNum = 0;
    }

    public void UpdateKillInfo(Actor actor)
    {
        if (actor.Type == ActorType.enMain)
        {
            SetkillEnemyNum(actor.KillEnemy);
            SetbeKilledNum(actor.BeKilled);
            SM.RandomRoomLevel.Singleton.m_sceneHeroReliveTime = actor.ReliveTime;
        }
    }



    public void PrepareEnterDungeons()
    {
        if (GameSettings.Singleton.m_isSingle)
        {
            //test
            //Debug.Log("begin random map");
            RandomMapGenerator rmg = new RandomMapGenerator();
            rmg.setup(1, 1, 4f, 9f, 8f, 5f, 5);
            //FileStream aFiler = File.OpenRead("testData.txt");

            //StreamReader sr = new StreamReader("E:\\testData.txt", Encoding.Default);
            string result = "";
            //                String line;
            //while ((line = sr.ReadLine()) != null)
            //{
            //   result += line;
            //}
            Debug.Log("finish random map");
            result = rmg.generate();
            Debug.Log(result);
            SM.RandomRoomLevel.Singleton.ParseDataBuildTree(result);
            SM.RandomRoomLevel.Singleton.NotifyChanged((int)SM.RandomRoomLevel.ENPropertyChanged.enEnterDungeon, null);
        }
        else
        {
            // 进入LOADING
            Loading.Singleton.SetLoadingTips((int)LOADINGTIPSENUM.enEnterCopy);


            // 发送当前选择的 战友
            int msgID = MiniServer.Singleton.SendSelectBattleHelper(StageMenu.Singleton.m_curHelperGuid);

            UIStageMenu.GetInstance().RegisterRespondFuncByMessageID(msgID, OnSelectHelperCallback);
        }
    }
    // 选择战友校验的回调
    public void OnSelectHelperCallback(MessageRespond respond)
    {
        Debug.Log("OnSelectHelperCallback:" + respond.IsSuccess);
        if (false == respond.IsSuccess)
        {
            UICommonMsgBoxCfg boxCfg = new UICommonMsgBoxCfg();
            boxCfg.mainTextPrefab = "UIComradeInfoErrorLabel";
            boxCfg.buttonNum = 1;
            UICommonMsgBox.GetInstance().ShowMsgBox(null, null, boxCfg);
            return;
        }
        // 校验成功 发送进入副本的消息
        // 发送服务器
        int myStamina = User.Singleton.UserProps.GetProperty_Int32(UserProperty.stamina);
        MiniServer.Singleton.SendEnterDungeon(StageMenu.Singleton.m_curFloorId, myStamina, StageMenu.Singleton.m_camp, StageMenu.Singleton.m_key, StageMenu.Singleton.m_isRolling);
    }

    public void ReceiveBattleArenaInfo(string result, int nDungeonID)
    {
        SM.RandomRoomLevel.Singleton.ParseDataBuildTree(result);
        SM.RandomRoomLevel.Singleton.m_curFloorId = nDungeonID;
        SM.RandomRoomLevel.Singleton.NotifyChanged((int)SM.RandomRoomLevel.ENPropertyChanged.enEnterDungeon, null);
    }
}
