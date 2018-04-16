using System;
using System.Collections.Generic;
using UnityEngine;


// 战斗结算
public class BattleSummary : IPropertyObject  
{
    #region Singleton
    static BattleSummary m_singleton;
    static public BattleSummary Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new BattleSummary();
            }
            return m_singleton;
        }
    }
    #endregion

    // 升级前的 等级
    public int m_oldLevel               = 0;

    // 之前的金钱
    public  int m_oldMoney              = 0;

    // 之前的 经验值 用于经验条的变化
    public int m_oldExp                 = 0;

    // 之前的 经验值 用于数值变化
    public int m_oldExpNum              = 0;

    // 是否 要进行 经验数值变化 
    public bool m_expChange             = false;

    // 是否 要进行 经验条 的变化
    public bool m_expBarChange          = false;

    // 是否 要进行 金钱的数值变化
    public bool m_moneyChange           = false;

    // 是否是手动退出
    public bool m_manualQuit            = false;

    public string m_curRank             = "F";

    // 每帧所改变的值
    public int m_expChangePerFPS        = 0;

    // 改变 完成一共所需的帧数
    int m_expChangeTotalFPS             = 0;

    // 每帧所改变的值
    public int m_moneyChangePerFPS      = 0;

    // 改变 完成一共所需的帧数
    int m_moneyChangeTotalFPS           = 0;

    // 要涨到的目标经验值
    public int m_desExp                 = 0;

    // 要涨到的目标金钱
    public int m_desMoney               = 0;

    public bool m_fastPrizeCard         = false; // 是否快速显示 奖励卡牌

    public enum ENPropertyChanged
    {
        enShow = 1,
        enTick,
        enFail,
    }
    public List<int> m_battleRewardItemList = null;

  
    //通关时间
    public int m_passStageTime = 0;
    //最大combo
    public int m_maxComboCount  = 0;
    //复活次数
    public int m_reliveCount    =0;
    //击杀boss数量
    public int m_killBossCount  = 0;

    Dictionary<int, int> m_chiefSkillList   = new Dictionary<int, int>(); // 主角色的技能列表 skillID ,Level
    Dictionary<int, int> m_deputySkillList  = new Dictionary<int, int>(); // 副角色的技能列表 skillID ,Level
    Dictionary<int, int> m_supportSkillList = new Dictionary<int, int>(); //  切入角色的技能列表 skillID ,Level

    public Dictionary<int, int> m_newCardList      = new Dictionary<int, int>();// 结算时 获得的是新卡的列表

    public BattleSummary()
    {
        SetPropertyObjectID((int)MVCPropertyID.enBattelSummary);

        m_battleRewardItemList          = new List<int>();
    }

    // 战斗结束 完成结算 退出关卡 manual 是否是手动退出 victory 关卡是否胜利
    public void OnFinished(bool manual = false,bool victory = true)
    {
        MainGame.Singleton.StartCoroutine(CoroutineOnFinished(manual,victory));

        // 把掉落资源清除
        DropItemPerformance.Singleton.ClearDropItem();
    }


    private System.Collections.IEnumerator CoroutineOnFinished(bool manual, bool victory )
    {
        m_manualQuit    = manual;

        // 如果不是手动退出
        if ( false == m_manualQuit)
        {
            float time = 2f;

            WorldParamInfo worldParamInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enBattleFinishTime);
            if (null != worldParamInfo)
            {
                time = worldParamInfo.FloatTypeValue;

            }
            yield return new WaitForSeconds(time);
        }
        Debug.Log(" 退出战斗 CoroutineOnFinished：");

         // 关卡胜利
        if (victory)
        {
            m_passStageTime = (int)(Time.time - BattleArena.Singleton.StageStartTime);
            m_maxComboCount = BattleArena.Singleton.MaxComboNumber;
            m_reliveCount = BattleArena.Singleton.ReliveCount;
            m_killBossCount = BattleArena.Singleton.KillBossCount;
            List<int> DropList = BattleArena.Singleton.DropCardsIdList;
            // 如果是单机
            if (GameSettings.Singleton.m_isSingle)
            {
                // 如果手动退出
                if (m_manualQuit)
                {
                    StateMainUI mainState = new StateMainUI();
                    MainGame.Singleton.TranslateTo(mainState);
                }
                else
                {
                    OnShowBattleSummary();
                }
            }
            // 联网
            else
            {
                m_curRank = GetRank();
                // 显示 战斗结算界面
                m_desExp = (int)(GetBaseExp() * GetExpParam());

                m_desMoney = (int)(GetBaseMoney() * GetMoneyParam());

                Debug.Log("OnFinished:m_desExp " + m_desExp + ",m_desMoney:" + m_desMoney);

                // 显示LOADING 
                Loading.Singleton.SetLoadingTips(1);

                // 发送服务器 战斗结束了
                //int curRankID = 0;
                //FloorRankInfo curRankInfo = GameTable.floorRankTableAsset.LookUp(m_curRank);
                //if (null != curRankInfo)
                //{
                //    curRankID = curRankInfo.m_rankID;
                //}

                int totalScore = GetKilledBossScore() + GetElapsedTimeScore() + GetMaxComboScore() + GetReliveScore();
                //todo
                IMiniServer.Singleton.SendDungeonSettlement(StageMenu.Singleton.m_curFloorId, m_passStageTime, m_maxComboCount, m_killBossCount, m_reliveCount, totalScore, m_desExp, m_desMoney, 1, DropList);
                //IMiniServer.Singleton.SendDungeonSettlement(curRankID, StageMenu.Singleton.m_curFloorId, m_desExp, m_desMoney,1);

                Debug.Log(" 退出战斗 战斗结算：" + StageMenu.Singleton.m_curFloorId + "," + m_passStageTime + "," + m_maxComboCount + "," + m_killBossCount + "," + m_reliveCount + "," + totalScore + "," + m_desExp + "," + m_desMoney + "," + DropList);
            }
        }

        // 关卡失败
        else
        {
             // 如果是单机
            if (GameSettings.Singleton.m_isSingle)
            {
                // 如果手动退出
                if (m_manualQuit)
                {
                    StateMainUI mainState = new StateMainUI();
                    MainGame.Singleton.TranslateTo(mainState);
                }
                else
                {
                    StateMainUI mainState = new StateMainUI();
                    MainGame.Singleton.TranslateTo(mainState);
                }
            }
            // 联网
            else
            {
                IMiniServer.Singleton.SendExitDungeon(StageMenu.Singleton.m_curFloorId);
            }
            Debug.Log(" 关卡失败 退出战斗 战斗结算：");
        }
        
    }


    public string  GetRank()
    {
        int killedBossScore     = GetKilledBossScore();
        int elapsedTimeScore    = GetElapsedTimeScore();
        int maxComboScore       = GetMaxComboScore();
        int reliveCountScore    = GetReliveScore();
        int totalScore          = killedBossScore+elapsedTimeScore+maxComboScore+reliveCountScore;

        return GetRank(totalScore, StageMenu.Singleton.m_curFloorId);
    }

    public void OnShowBattleSummary()
    {
        NotifyChanged((int)ENPropertyChanged.enShow, null);
    }

    // 一进入场景 时记录一些数据 已便在战斗结束时 做一些计算
    public void RecordData()
    {
        // 进入时的等级
         m_oldLevel          = User.Singleton.GetLevel();

        // 进入时的金钱
        m_oldMoney           = User.Singleton.GetMoney();

        // 进入时的经验值
        m_oldExp             = User.Singleton.GetExp();

        m_oldExpNum          = m_oldExp;

        RecordTeamSkillData();
    }

    // 记下 队伍每个人的技能等级
    void RecordTeamSkillData()
    {
       
        m_chiefSkillList.Clear();
        m_deputySkillList.Clear();
        m_supportSkillList.Clear();

        if (null != Team.Singleton.Chief)
        {
            foreach (var item in Team.Singleton.Chief.SkillItemInfoList)
            {
                SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(item.m_skillID);
                if (skillInfo == null) continue;

                // 如果是解锁技能
                if (Team.Singleton.Chief.HaveSkill(item.m_skillID))
                {
                    m_chiefSkillList.Add(item.m_skillID, item.m_skillLevel);
                    Debug.Log("chief SkillID :" + item.m_skillID + ",item.m_skillLevel:" + item.m_skillLevel);
                }
            }
        }

        if (null != Team.Singleton.Deputy)
        {
            foreach (var item in Team.Singleton.Deputy.SkillItemInfoList)
            {
                SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(item.m_skillID);
                if (skillInfo == null) continue;

                // 如果是解锁技能
                if (Team.Singleton.Deputy.HaveSkill(item.m_skillID))
                {
                    if (!m_deputySkillList.ContainsKey(item.m_skillID))
                    {
                        m_deputySkillList.Add(item.m_skillID, item.m_skillLevel);
                    }
                    Debug.Log("Deputy SkillID :" + item.m_skillID + ",item.m_skillLevel:" + item.m_skillLevel);
                }
            }
        }


        if (null != Team.Singleton.Support)
        {
            foreach (var item in Team.Singleton.Support.SkillItemInfoList)
            {
                SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(item.m_skillID);
                if (skillInfo == null) continue;

                // 如果是解锁技能
                if (Team.Singleton.Support.HaveSkill(item.m_skillID))
                {
                    m_supportSkillList.Add(item.m_skillID, item.m_skillLevel);
                    Debug.Log("Support SkillID :" + item.m_skillID + ",item.m_skillLevel:" + item.m_skillLevel);
                }
            }
        }
        
    }

    // 获得金钱的基数
    public float GetBaseMoney()
    {

        FloorInfo floorInfo = GameTable.FloorInfoTableAsset.LookUp(StageMenu.Singleton.m_curFloorId);
        if ( null == floorInfo )
        {
            UnityEngine.Debug.Log("BattleSummary GetMoney FloorInfo floorInfo == null m_floorId=" + StageMenu.Singleton.m_curFloorId);
            return 0;
        }

        Debug.Log("GetBaseMoney:" + floorInfo.m_coinBase);
        return floorInfo.m_coinBase;
    }

    // 获得金钱的倍率
    public float GetMoneyParam()
    {
        FloorRankInfo rankInfo = GameTable.floorRankTableAsset.LookUp(m_curRank);
        if (null == rankInfo)
        {
            UnityEngine.Debug.Log("BattleSummary GetMoney FloorRankInfo rankInfo == null m_curRank=" + m_curRank);
            return 0;
        }
        return rankInfo.m_coinParam;
    }

    // 获得经验 的基数
    public float GetBaseExp()
    {
        FloorInfo floorInfo = GameTable.FloorInfoTableAsset.LookUp(StageMenu.Singleton.m_curFloorId);
        if (null == floorInfo)
        {
            UnityEngine.Debug.Log("BattleSummary GetExp FloorInfo floorInfo == null m_floorId=" + StageMenu.Singleton.m_curFloorId);
            return 0;
        }
        Debug.Log("GetBaseExp:" + floorInfo.m_expBase);
        return  floorInfo.m_expBase;
    }

    // 获得 经验的 倍率
    public float GetExpParam()
    {
        FloorRankInfo rankInfo = GameTable.floorRankTableAsset.LookUp(m_curRank);
        if (null == rankInfo)
        {
            UnityEngine.Debug.Log("BattleSummary GetExp FloorRankInfo rankInfo == null m_curRank=" + m_curRank);
            return 0;
        }
        return rankInfo.m_expParam ;
    }

    public string GetPassStageTime()
    {
        int nMinite = m_passStageTime / 60;
        int nSec = m_passStageTime - nMinite * 60;
        return string.Format(Localization.Get("RevertTime"), nMinite, nSec);
    }

    public int GetTotalScore()
    {
        return GetMaxComboScore() + GetReliveScore() + GetElapsedTimeScore() + GetKilledBossScore();
    }

    public int GetMaxComboScore()
    {
        FloorInfo floorInfo = GameTable.FloorInfoTableAsset.LookUp(StageMenu.Singleton.m_curFloorId);
        if (null == floorInfo)
        {
            UnityEngine.Debug.Log("GetMaxComboScore FloorInfo floorInfo == null m_floorId:" + StageMenu.Singleton.m_curFloorId);
            return 0;
        }

        ScoreParamInfo scoreInfo = GameTable.ScoreParamTableAsset.LookUp(floorInfo.m_scoreId);
        if (null == scoreInfo)
        {
            UnityEngine.Debug.Log("GetMaxComboScore scoreInfo floorInfo == null m_scoreId:" + floorInfo.m_scoreId);
            return 0;
        }

        //UnityEngine.Debug.Log("GetMaxComboScore player.m_maxComboCount:" + player.m_maxComboCount + ",scoreInfo.m_standardCombo:" + scoreInfo.m_standardCombo + ",scoreInfo.m_standardComboPoint:" + scoreInfo.m_standardComboPoint);
        return (int)(scoreInfo.m_standardComboPoint * ((float)m_maxComboCount / (float)scoreInfo.m_standardCombo));

    }

    public int GetReliveScore()
    {
        FloorInfo floorInfo = GameTable.FloorInfoTableAsset.LookUp(StageMenu.Singleton.m_curFloorId);
        if (null == floorInfo)
        {
            UnityEngine.Debug.Log("GetReliveScore FloorInfo floorInfo == null m_floorId:" + StageMenu.Singleton.m_curFloorId);
            return 0;
        }

        ScoreParamInfo scoreInfo = GameTable.ScoreParamTableAsset.LookUp(floorInfo.m_scoreId);
        if (null == scoreInfo)
        {
            UnityEngine.Debug.Log("GetReliveScore scoreInfo floorInfo == null m_scoreId:" + floorInfo.m_scoreId);
            return 0;
        }
       
        return scoreInfo.m_nonRespawnPoint - (m_reliveCount - scoreInfo.m_standardRespawn) * (scoreInfo.m_nonRespawnPoint / 3);
    }

    public int GetElapsedTimeScore()
    {
        FloorInfo floorInfo = GameTable.FloorInfoTableAsset.LookUp(StageMenu.Singleton.m_curFloorId);
        if (null == floorInfo)
        {
            UnityEngine.Debug.Log("GetElapsedTimeScore FloorInfo floorInfo == null m_floorId:" + StageMenu.Singleton.m_curFloorId);
            return 0;
        }

        ScoreParamInfo scoreInfo = GameTable.ScoreParamTableAsset.LookUp(floorInfo.m_scoreId);
        if (null == scoreInfo)
        {
            UnityEngine.Debug.Log("GetElapsedTimeScore scoreInfo floorInfo == null m_scoreId:" + floorInfo.m_scoreId);
            return 0;
        }

       // UnityEngine.Debug.Log("scoreInfo.m_standardFloorTimePoint:" + scoreInfo.m_standardFloorTimePoint + ",m_passStageTime:" + m_passStageTime + ",scoreInfo.m_standardFloorTime:" + scoreInfo.m_standardFloorTime);

        return (int)(scoreInfo.m_standardFloorTimePoint * (scoreInfo.m_standardFloorTime / m_passStageTime));
    }

    public int GetKilledBossScore()
    {
        FloorInfo floorInfo = GameTable.FloorInfoTableAsset.LookUp(StageMenu.Singleton.m_curFloorId);
        if (null == floorInfo)
        {
            UnityEngine.Debug.Log("GetKilledBossScore FloorInfo floorInfo == null m_floorId:" + StageMenu.Singleton.m_curFloorId);
            return 0;
        }

        ScoreParamInfo scoreInfo = GameTable.ScoreParamTableAsset.LookUp(floorInfo.m_scoreId);
        if (null == scoreInfo)
        {
            UnityEngine.Debug.Log("GetKilledBossScore scoreInfo floorInfo == null m_scoreId:" + floorInfo.m_scoreId);
            return 0;
        }
      
        //UnityEngine.Debug.Log("player.m_killBossCount:" + player.m_killBossCount + ",scoreInfo.m_bossPoint:" + scoreInfo.m_bossPoint + ",scoreInfo.m_standardBossKillCount:" + scoreInfo.m_standardBossKillCount);


        return m_killBossCount * scoreInfo.m_bossPoint / scoreInfo.m_standardBossKillCount;
    }

    public string GetRank(int score,int floorId)
    {
        FloorInfo floorInfo = GameTable.FloorInfoTableAsset.LookUp(floorId);
        if (null == floorInfo)
        {
            UnityEngine.Debug.Log(" GetRank FloorInfo floorInfo == null m_floorId:" + floorId);
            return "";
        }

        ScoreParamInfo scoreInfo = GameTable.ScoreParamTableAsset.LookUp(floorInfo.m_scoreId);
        if (null == scoreInfo)
        {
            UnityEngine.Debug.Log("GetRank scoreInfo floorInfo == null m_scoreId:" + floorInfo.m_scoreId);
            return "";
        }

      float fTimeScore      = (scoreInfo.m_standardFloorTimePoint * scoreInfo.m_fTimeParam);
      float eTimeScore      = (scoreInfo.m_standardFloorTimePoint * scoreInfo.m_eTimeParam);
      float dTimeScore      = (scoreInfo.m_standardFloorTimePoint * scoreInfo.m_dTimeParam);
      float cTimeScore      = (scoreInfo.m_standardFloorTimePoint * scoreInfo.m_cTimeParam);
      float bTimeScore      = (scoreInfo.m_standardFloorTimePoint * scoreInfo.m_bTimeParam);
      float aTimeScore      = (scoreInfo.m_standardFloorTimePoint * scoreInfo.m_aTimeParam);
      float sTimeScore      = (scoreInfo.m_standardFloorTimePoint * scoreInfo.m_sTimeParam);
      float ssTimeScore     = (scoreInfo.m_standardFloorTimePoint * scoreInfo.m_ssTimeParam);
      float sssTimeScore    = (scoreInfo.m_standardFloorTimePoint * scoreInfo.m_sssTimeParam);
      
      float fCountScore     = (scoreInfo.m_standardComboPoint * scoreInfo.m_fCountParam);
      float eCountScore     = (scoreInfo.m_standardComboPoint * scoreInfo.m_eCountParam);
      float dCountScore     = (scoreInfo.m_standardComboPoint * scoreInfo.m_dCountParam);
      float cCountScore     = (scoreInfo.m_standardComboPoint * scoreInfo.m_cCountParam);
      float bCountScore     = (scoreInfo.m_standardComboPoint * scoreInfo.m_bCountParam);
      float aCountScore     = (scoreInfo.m_standardComboPoint * scoreInfo.m_aCountParam);
      float sCountScore     = (scoreInfo.m_standardComboPoint * scoreInfo.m_sCountParam);
      float ssCountScore    = (scoreInfo.m_standardComboPoint * scoreInfo.m_ssCountParam);
      float sssCountScore   = (scoreInfo.m_standardComboPoint * scoreInfo.m_sssCountParam);

      float fBossScore      = (scoreInfo.m_fBossCount / scoreInfo.m_standardBossKillCount * scoreInfo.m_bossPoint);
      float eBossScore      = (scoreInfo.m_eBossCount / scoreInfo.m_standardBossKillCount * scoreInfo.m_bossPoint);
      float dBossScore      = (scoreInfo.m_dBossCount / scoreInfo.m_standardBossKillCount * scoreInfo.m_bossPoint);
      float cBossScore      = (scoreInfo.m_cBossCount / scoreInfo.m_standardBossKillCount * scoreInfo.m_bossPoint);
      float bBossScore      = (scoreInfo.m_bBossCount / scoreInfo.m_standardBossKillCount * scoreInfo.m_bossPoint);
      float aBossScore      = (scoreInfo.m_aBossCount / scoreInfo.m_standardBossKillCount * scoreInfo.m_bossPoint);
      float sBossScore      = (scoreInfo.m_sBossCount / scoreInfo.m_standardBossKillCount * scoreInfo.m_bossPoint);
      float ssBossScore     = (scoreInfo.m_ssBossCount / scoreInfo.m_standardBossKillCount * scoreInfo.m_bossPoint);
      float sssBossScore    = (scoreInfo.m_sssBossCount / scoreInfo.m_standardBossKillCount * scoreInfo.m_bossPoint);


      float fReliveScore    = (scoreInfo.m_nonRespawnPoint - (scoreInfo.m_fReliveCount - scoreInfo.m_standardRespawn) * (scoreInfo.m_nonRespawnPoint / 3));
      float eReliveScore    = (scoreInfo.m_nonRespawnPoint - (scoreInfo.m_eReliveCount - scoreInfo.m_standardRespawn) * (scoreInfo.m_nonRespawnPoint / 3));
      float dReliveScore    = (scoreInfo.m_nonRespawnPoint - (scoreInfo.m_dReliveCount - scoreInfo.m_standardRespawn) * (scoreInfo.m_nonRespawnPoint / 3));
      float cReliveScore    = (scoreInfo.m_nonRespawnPoint - (scoreInfo.m_cReliveCount - scoreInfo.m_standardRespawn) * (scoreInfo.m_nonRespawnPoint / 3));
      float bReliveScore    = (scoreInfo.m_nonRespawnPoint - (scoreInfo.m_bReliveCount - scoreInfo.m_standardRespawn) * (scoreInfo.m_nonRespawnPoint / 3));
      float aReliveScore    = (scoreInfo.m_nonRespawnPoint - (scoreInfo.m_aReliveCount - scoreInfo.m_standardRespawn) * (scoreInfo.m_nonRespawnPoint / 3));
      float sReliveScore    = (scoreInfo.m_nonRespawnPoint - (scoreInfo.m_sReliveCount - scoreInfo.m_standardRespawn) * (scoreInfo.m_nonRespawnPoint / 3));
      float ssReliveScore   = (scoreInfo.m_nonRespawnPoint - (scoreInfo.m_ssReliveCount - scoreInfo.m_standardRespawn) * (scoreInfo.m_nonRespawnPoint / 3));
      float sssReliveScore  = (scoreInfo.m_nonRespawnPoint - (scoreInfo.m_sssReliveCount - scoreInfo.m_standardRespawn) * (scoreInfo.m_nonRespawnPoint / 3));


      float fTotalScore     = fTimeScore + fCountScore + fReliveScore + fBossScore;
      float eTotalScore     = eTimeScore + eCountScore + eReliveScore + eBossScore;
      float dTotalScore     = dTimeScore + dCountScore + dReliveScore + dBossScore;
      float cTotalScore     = cTimeScore + cCountScore + cReliveScore + cBossScore;
      float bTotalScore     = bTimeScore + bCountScore + bReliveScore + bBossScore;
      float aTotalScore     = aTimeScore + aCountScore + aReliveScore + aBossScore;
      float sTotalScore     = sTimeScore + sCountScore + sReliveScore + sBossScore;
      float ssTotalScore    = ssTimeScore + ssCountScore + ssReliveScore + ssBossScore;
      float sssTotalScore   = sssTimeScore + sssCountScore + sssReliveScore + sssBossScore;

        UnityEngine.Debug.Log("score:" + score + ",fTotalScore:" + fTotalScore + ",eTotalScore:" + eTotalScore + ",dTotalScore:" + dTotalScore + ",cTotalScore:" + cTotalScore + ",bTotalScore:" + bTotalScore + ",aTotalScore:" + aTotalScore + ",sTotalScore:" + sTotalScore + ",ssTotalScore:" + ssTotalScore + ",sssTotalScore:" + sssTotalScore);
        if ( score <=  eTotalScore )
        {
            return "E";
        }
        else if (score > eTotalScore && score <= dTotalScore)
        {
            return "E";
        }
        else if (score > dTotalScore && score <= cTotalScore)
        {
            return "D";
        }
        else if (score > cTotalScore && score <= bTotalScore)
        {
            return "C";
        }
        else if (score > bTotalScore && score <= aTotalScore)
        {
            return "B";
        }
        else if (score > aTotalScore && score <= sTotalScore)
        {
            return "A";
        }
        else if (score > sTotalScore && score <= ssTotalScore)
        {
            return "S";
        }
        else if (score > ssTotalScore && score <= sssTotalScore)
        {
            return "SS";
        }
        else if (score > sssTotalScore)
        {
            return "SSS";
        }
        return "F";
    }

    // 经验值开始变化
    public void ExpChange(int changeTotalFps)
    {
        m_expChange         = true;

        m_expChangeTotalFPS = changeTotalFps;

       // m_oldExpNum         = (int)GetBaseExp();

       // m_desExp            = (int)(GetBaseExp() * GetExpParam());

        m_expChangePerFPS   = (m_desExp - m_oldExpNum) / m_expChangeTotalFPS;

        if (m_expChangePerFPS<= 0 )
        {
            m_expChangePerFPS = 1;
        }

        NotifyChanged((int)ENPropertyChanged.enTick, null);
    }

    // 金钱值开始变化
    public void MoneyChange(int changeTotalFps)
    {
        m_moneyChange         = true;

        m_moneyChangeTotalFPS = changeTotalFps;

       // m_desMoney            =(int) (GetBaseMoney() * GetMoneyParam());

        m_oldMoney            = (int)GetBaseMoney();

        m_moneyChangePerFPS   = (m_desMoney - m_oldMoney) / m_moneyChangeTotalFPS;

        if (m_moneyChangePerFPS <= 0 )
        {
            m_moneyChangePerFPS = 1;
        }

        NotifyChanged((int)ENPropertyChanged.enTick, null);
    }

    // 经验进度条开始变化
    public void ExpBarChange()
    {
        m_expBarChange = true;

        NotifyChanged((int)ENPropertyChanged.enTick, null);
    }

    // 战斗结算 服务器的回调
    public void BattleSummaryDungeonEnd(bool bVictory)
    {
        // 隐藏LOADING
        Loading.Singleton.Hide();

        // 关卡胜利
        if (bVictory)
        {
            // 计算卡牌技能升级情况
            TeamCardSkillLevelUpSummary();

            // 如果是手动退出
            if (m_manualQuit)
            {
                StateMainUI mainState = new StateMainUI();
                MainGame.Singleton.TranslateTo(mainState);
            }
            else
            {
                NotifyChanged((int)ENPropertyChanged.enShow, null);
            }


        }
        // 关卡失败
        else
        {
            ShowFail();
        }

    }

    public void ShowFail()
    {
        NotifyChanged((int)ENPropertyChanged.enFail, null);
    }

    // 计算卡牌技能升级情况
    void TeamCardSkillLevelUpSummary()
    {

        LevelUp.Singleton.ClearSkillList();

        LevelUp.Singleton.m_curState = LevelUp.LevelState.enSummary;

        if (null != Team.Singleton.Chief)
        {
            foreach (var item in Team.Singleton.Chief.SkillItemInfoList)
            {
                SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(item.m_skillID);
                if (skillInfo == null) continue;

                // 如果是解锁技能
                if (Team.Singleton.Chief.HaveSkill(item.m_skillID))
                {
                    // 如果记录中的 等级和 最新等级不一样则 为升级
                    if (m_chiefSkillList.ContainsKey(item.m_skillID))
                    {
                        if (item.m_skillLevel > m_chiefSkillList[item.m_skillID])
                        {
                            SkillLevelUpInfo info   = new SkillLevelUpInfo();
                            info.m_skillId          = item.m_skillID;
                            info.m_skillLevel       = item.m_skillLevel;

                            LevelUp.Singleton.m_skillList.Add(info);
                            Debug.Log("chief SkillID :" + item.m_skillID + "Level up " + item.m_skillLevel);
                        }
                    }  
                }
            }
        }

        if (null != Team.Singleton.Deputy)
        {
            foreach (var item in Team.Singleton.Deputy.SkillItemInfoList)
            {
                SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(item.m_skillID);
                if (skillInfo == null) continue;

                // 如果是解锁技能
                if (Team.Singleton.Deputy.HaveSkill(item.m_skillID))
                {
                    // 如果记录中的 等级和 最新等级不一样则 为升级
                    if (m_deputySkillList.ContainsKey(item.m_skillID))
                    {
                        if (item.m_skillLevel > m_deputySkillList[item.m_skillID])
                        {
                            SkillLevelUpInfo info   = new SkillLevelUpInfo();
                            info.m_skillId          = item.m_skillID;
                            info.m_skillLevel       = item.m_skillLevel;

                            LevelUp.Singleton.m_skillList.Add(info);
                            Debug.Log("deputy SkillID :" + item.m_skillID + "Level up " + item.m_skillLevel);
                        }
                    }  
                }
            }
        }


        if (null != Team.Singleton.Support)
        {
            foreach (var item in Team.Singleton.Support.SkillItemInfoList)
            {
                SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(item.m_skillID);
                if (skillInfo == null) continue;

                // 如果是解锁技能
                if (Team.Singleton.Support.HaveSkill(item.m_skillID))
                {
                    // 如果记录中的 等级和 最新等级不一样则 为升级
                    if (m_supportSkillList.ContainsKey(item.m_skillID))
                    {
                        if (item.m_skillLevel > m_supportSkillList[item.m_skillID])
                        {
                            SkillLevelUpInfo info   = new SkillLevelUpInfo();
                            info.m_skillId          = item.m_skillID;
                            info.m_skillLevel       = item.m_skillLevel;

                            LevelUp.Singleton.m_skillList.Add(info);
                            Debug.Log("support SkillID :" + item.m_skillID + "Level up " + item.m_skillLevel);
                        }
                    }  
                }
            }
        }
    }

    // 结算时获得的新卡 列表
    public void SummaryGetNewCard(int cardId)
    {
        ENGainLevel level = Illustrate.Singleton.GetIllustratedGainLv(cardId);

        if (level == ENGainLevel.enNone || level == ENGainLevel.enSee)
        {
            if (!m_newCardList.ContainsKey(cardId))
           {
               m_newCardList.Add(cardId, 0);
           }
        }
    }

    // 获得是否是新获得的卡
    public bool IsNewCard(int cardId)
    {
        if (m_newCardList.ContainsKey(cardId))
        {
            // 为了 计算 此新卡 是否 是第一次看到
            m_newCardList[cardId] = m_newCardList[cardId] + 1;

            //Debug.Log("m_newCardList[cardId]:" + m_newCardList[cardId] + ",cardId:" + cardId);
            // 第一次  看到这个卡
            if (m_newCardList[cardId] == 1)
            {
                return true;
            }
            return false;
        }
        else
        {
            ENGainLevel gainLevel = Illustrate.Singleton.GetIllustratedGainLv(cardId);

            if (gainLevel == ENGainLevel.enNone || gainLevel == ENGainLevel.enSee)
            {
                return true;
            }
        }
        return false;
    }
}
