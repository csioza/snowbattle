using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region ComboProps
public class ComboProps
{

    enum ENComboOperateType
    {
        enAdd = 1,
        enClear = 2,
    }
    public class SkillConnectionInfo
    {
        //技能连接类型
        private ENSkillConnectionType m_skillConnType = ENSkillConnectionType.enNone;
        public ENSkillConnectionType SkillConnType { get { return m_skillConnType; } set { m_skillConnType = value; SkillConnTime = Time.time; } }
        //技能连接时间
        private float m_skillConnTime = 0;
        public float SkillConnTime { get { return m_skillConnTime; } private set { m_skillConnTime = value; } }
    }
    private float m_lastComboTime = 0;//最后一次连击的时间
    private float m_nextComboTimeModify = 0;//下一次的连击时间修正
    #region WeakonComboTime//受到伤害时的时间削减，连击之间的所有时间削减的和
    private float m_weakenComboTime = 0;
    public float WeakenComboTime
    {
        get { return m_weakenComboTime; }
        set { m_weakenComboTime = value; }
    }
    #endregion
    #region TotalComboNumber//总的连击数量
    private int m_totalComboNumber = 0;
    public int TotalComboNumber
    {
        get { return m_totalComboNumber; }
        set
        {
            m_totalComboNumber = value;
            MaxComboNumber = value > MaxComboNumber ? value : MaxComboNumber;
        }
    }
    #endregion
    public int MaxComboNumber = 0;//最大连击数，不能被外部改变
    //技能连接信息
    private SkillConnectionInfo m_skillConnInfo = new SkillConnectionInfo();
    #region SwitchActorTime//角色切换时，增加的combo时间
    private float m_switchActorTime = 0;
    public float SwitchActorTime { get { return m_switchActorTime; } set { m_switchActorTime = value; } }
    #endregion
    public Dictionary<int, float> DamageModifyList = new Dictionary<int, float>();

    public void Clear()
    {
        ComboClear();
        DamageModifyList.Clear();
    }
    public void ComboChanged(int resultID, int targetID, float damageModify)
    {
        float now = Time.time;
        if (TotalComboNumber == 0)
        {//当前连击数为0
            AddCombo(targetID, resultID);
        }
        else
        {
            if (now - m_lastComboTime <= m_nextComboTimeModify - WeakenComboTime + SwitchActorTime)
            {//连击成功
                AddCombo(targetID, resultID);

                DamageModifyList[TotalComboNumber] = damageModify;

                SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(resultID);
                if (null != info)
                {
                    for (int i = 0; i < info.ComboNum; i++)
                    {
                        BattleArena.Singleton.NotifyChanged((int)BattleArena.ENPropertyChanged.enComboCount, null);
                    }
                }



                {//评价
                    int appraiseIndex = 0;
                    if (IsAppraise((int)ENWorldParamIndex.enComboNice))
                    {
                        appraiseIndex = (int)ENWorldParamIndex.enComboNice;
                    }
                    else if (IsAppraise((int)ENWorldParamIndex.enComboCool))
                    {
                        appraiseIndex = (int)ENWorldParamIndex.enComboCool;
                    }
                    else if (IsAppraise((int)ENWorldParamIndex.enComboGreat))
                    {
                        appraiseIndex = (int)ENWorldParamIndex.enComboGreat;
                    }
                    else if (IsAppraise((int)ENWorldParamIndex.enComboBravo))
                    {
                        appraiseIndex = (int)ENWorldParamIndex.enComboBravo;
                    }
                    else if (IsAppraise((int)ENWorldParamIndex.enComboPerfect))
                    {
                        appraiseIndex = (int)ENWorldParamIndex.enComboPerfect;
                    }
                    if (appraiseIndex != 0)
                    {
                        BattleArena.Singleton.NotifyChanged((int)BattleArena.ENPropertyChanged.enComboAppraise, appraiseIndex);
                    }
                }
            }
            else
            {//连击失败
                ComboClear();
            }
        }
        SwitchActorTime = 0;
    }
    public void ComboClear()
    {
        BattleArena.Singleton.SwordSoul.Add(TotalComboNumber, SwordSoulProps.ENAddComboType.enBreak);
        TotalComboNumber = 0;

        // 如果是长连接
        if (ClientNet.Singleton.IsLongConnecting)
        {
            IMiniServer.Singleton.SendComboNum_C2BS((int)ENComboOperateType.enClear);
        }
    }
    void AddCombo(int targetID, int resultID)
    {

        SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(resultID);
        if (null == info)
        {
            return;
        }
        TotalComboNumber = TotalComboNumber + info.ComboNum;
        //++TotalComboNumber;
        m_lastComboTime = Time.time;
        WeakenComboTime = 0;
        ComboModify(resultID, targetID);

        BattleArena.Singleton.SwordSoul.Add(TotalComboNumber, SwordSoulProps.ENAddComboType.enNormal);

        // 如果是长连接
        if (ClientNet.Singleton.IsLongConnecting)
        {
            IMiniServer.Singleton.SendComboNum_C2BS((int)ENComboOperateType.enAdd, resultID);
        }
    }
    public void Tick()
    {
        float now = Time.time;
        if (TotalComboNumber != 0)
        {
            if (now - m_lastComboTime > m_nextComboTimeModify - WeakenComboTime + SwitchActorTime)
            {//连击失败
                ComboClear();
                SwitchActorTime = 0;
            }
        }
    }
    //一闪特效数量
    int m_connectEffectNumber = 0;
    public int ConnectEffectNumber { get { return m_connectEffectNumber; } private set { m_connectEffectNumber = value; } }
    public void ClearConnectEffect()
    {
        ConnectEffectNumber = 0;
    }
    public void SetConnetionInfo(int type)
    {
        m_skillConnInfo.SkillConnType = (ENSkillConnectionType)type;
    }
    private float m_skillConnectionDuration = 0;
    public bool IsPlayConnectEffect()
    {
        AttackAction action = ActorManager.Singleton.MainActor.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
        if (action != null)
        {
            if (m_skillConnectionDuration == 0)
            {
                m_skillConnectionDuration = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillConnectionDuration).FloatTypeValue;
            }
            if (action.AnimStartTime - m_skillConnInfo.SkillConnTime < m_skillConnectionDuration)
            {
                switch (m_skillConnInfo.SkillConnType)
                {
                    case ENSkillConnectionType.enNone:
                        break;
                    case ENSkillConnectionType.enNormal:
                        break;
                    case ENSkillConnectionType.enConnect:
                        if (action.m_skillInfo.SkillConnectionType == (int)ENSkillConnectionType.enConnect ||
                            action.m_skillInfo.SkillConnectionType == (int)ENSkillConnectionType.enSpecial ||
                            action.m_skillInfo.SkillConnectionType == (int)ENSkillConnectionType.enFinal)
                        {
                            ++ConnectEffectNumber;
                            return true;
                        }
                        break;
                    case ENSkillConnectionType.enSpecial:
                        if (action.m_skillInfo.SkillConnectionType == (int)ENSkillConnectionType.enSpecial ||
                            action.m_skillInfo.SkillConnectionType == (int)ENSkillConnectionType.enFinal)
                        {
                            ++ConnectEffectNumber;
                            return true;
                        }
                        break;
                    case ENSkillConnectionType.enFinal:
                        break;
                }
            }
        }
        return false;
    }
    private bool IsAppraise(int index)
    {
        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup(index);
        if (param != null && TotalComboNumber == param.IntTypeValue)
        {
            return true;
        }
        return false;
    }
    private void ComboModify(int resultID, int targetID)
    {
        SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(resultID);
        if (info == null)
        {
            return;
        }
        float staminaModify = 0;
        switch ((ENSkillComboType)info.ComboType)
        {
            case ENSkillComboType.enMore:
                {
                    //（取整(连击数/10)+1）*0.1
                    m_nextComboTimeModify = info.ComboTime + (float)((int)((TotalComboNumber / 10) + 1) - 1) * 0f;
                    //（取整(连击数/10)+1）*0.1+1
                    staminaModify = (float)((int)((TotalComboNumber / 10) + 1) - 1) * 0f + 1;
                }
                break;
            case ENSkillComboType.enSome:
                {
                    //（取整(连击数/10)+1）*0.12
                    m_nextComboTimeModify = info.ComboTime + (float)((int)((TotalComboNumber / 10) + 1) - 1) * 0f;
                    //（取整(连击数/10)+1）*0.2+1
                    staminaModify = (float)((int)((TotalComboNumber / 10) + 1) - 1) * 0f + 1;
                }
                break;
            case ENSkillComboType.enLittle:
                {
                    //（取整(连击数/10)+1）*0.15
                    m_nextComboTimeModify = info.ComboTime + (float)((int)((TotalComboNumber / 10) + 1) - 1) * 0f;
                    //（取整(连击数/10)+1）*0.3+1
                    staminaModify = (float)((int)((TotalComboNumber / 10) + 1) - 1) * 0f + 1;
                }
                break;
            default:
                break;
        }
        //修改目标的强韧度
        Actor target = ActorManager.Singleton.Lookup(targetID);
        if (target != null && !target.IsDead)
        {
            AttackAction action = target.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
            if (action != null)
            {
                if (action.m_skillInfo.IsStaminaFixed)
                {//锁定强韧度
                    return;
                }
            }
            staminaModify *= info.StaminaDamage;
            float curStamina = target.Props.GetProperty_Float(ENProperty.stamina);
            curStamina -= staminaModify;
            if (curStamina < 0)
            {
                curStamina = 0;
            }
            target.Props.SetProperty_Float(ENProperty.stamina, curStamina);
        }
    }
    public void ButtonAddComb(int count)
    {
        for (int i = 0; i < count; i++)
        {
            TotalComboNumber++;
            m_lastComboTime = Time.time;
            m_nextComboTimeModify = 2 + (float)((int)((TotalComboNumber / 10) + 1) - 1) * 0f;
            WeakenComboTime = 0;
            BattleArena.Singleton.NotifyChanged((int)BattleArena.ENPropertyChanged.enComboCount, null);
        }
    }
}
#endregion
#region SkillComboProps
public class SkillComboProps
{
    float m_connectionTime = 0;
    float ConnectionTime
    {
        get
        {
            if (m_connectionTime == 0)
            {
                m_connectionTime = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillConnectionTime).FloatTypeValue;
            }
            return m_connectionTime;
        }
    }
    float m_specialTime = 0;
    float SpecialTime
    {
        get
        {
            if (m_specialTime == 0)
            {
                m_specialTime = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillSpecialTime).FloatTypeValue;
            }
            return m_specialTime;
        }
    }
    float m_modifyTime = 0;
    float ModifyTime
    {
        get
        {
            if (m_modifyTime == 0)
            {
                m_modifyTime = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillComboAddTime).FloatTypeValue;
            }
            return m_modifyTime;
        }
    }
    //time
    float m_startTime = float.MaxValue;
    float m_duration = 0;
    //技能连接类型
    ENSkillConnectionType m_currentType = ENSkillConnectionType.enNone;

    //技能combo数量
    int m_skillComboNumber = 0;
    public int SkillComboNumber { get { return m_skillComboNumber; } private set { m_skillComboNumber = value; } }
    //清除数据
    public void ClearSkillCombo()
    {
        SkillComboNumber = 0;
        m_startTime = float.MaxValue;
        m_currentType = ENSkillConnectionType.enNone;
        //隐藏uiskillcombo
        BattleArena.Singleton.NotifyChanged((int)BattleArena.ENPropertyChanged.enSkillCombo, false);
    }
    //设置技能连接类型，并判断成功失败
    public void SetConnectionInfo(ENSkillConnectionType type)
    {
        if (m_currentType == ENSkillConnectionType.enNone)
        {
            Start(type);
            return;
        }
        switch (m_currentType)
        {
            case ENSkillConnectionType.enConnect:
                if (type == ENSkillConnectionType.enConnect ||
                    type == ENSkillConnectionType.enSpecial ||
                    type == ENSkillConnectionType.enFinal)
                {//succ
                    Succ(type);
                }
                else if (type == ENSkillConnectionType.enNormal)
                {
                }
                else
                {
                    Stop(type);
                }
                break;
            case ENSkillConnectionType.enSpecial:
                if (type == ENSkillConnectionType.enSpecial ||
                    type == ENSkillConnectionType.enFinal)
                {//succ
                    Succ(type);
                }
                else if (type == ENSkillConnectionType.enNormal)
                {
                }
                else
                {
                    Stop(type);
                }
                break;
            default:
                break;
        }
    }
    //开始
    void Start(ENSkillConnectionType type)
    {
        if (type == ENSkillConnectionType.enConnect ||
            type == ENSkillConnectionType.enSpecial)
        {
            m_currentType = type;
            m_startTime = Time.time;
            if (type == ENSkillConnectionType.enConnect)
            {
                m_duration = ConnectionTime;
            }
            else if (type == ENSkillConnectionType.enSpecial)
            {
                m_duration = SpecialTime;
            }
        }
    }
    //成功
    void Succ(ENSkillConnectionType type)
    {
        ++SkillComboNumber;
        //显示uiskillcombo
        BattleArena.Singleton.NotifyChanged((int)BattleArena.ENPropertyChanged.enSkillCombo, true);
        Start(type);
        m_duration += ModifyTime;
    }
    //停止
    void Stop(ENSkillConnectionType type)
    {
        //add soulcharge
        if (SkillComboNumber != 0)
        {
            //BattleArena.Singleton.SoulCharge.AddSoul(EnSoulType.enSkillCombo, SkillComboNumber + 1);
            //clear
            ClearSkillCombo();
            if (BattleArena.Singleton.Combo.ConnectEffectNumber != 0)
            {
                //BattleArena.Singleton.SoulCharge.AddSoul(EnSoulType.enConnectEffect, BattleArena.Singleton.Combo.ConnectEffectNumber);
                //clear connecteffectnumber
                BattleArena.Singleton.Combo.ClearConnectEffect();
            }
        }
        //BattleArena.Singleton.SoulCharge.StopResult();

        m_startTime = float.MaxValue;
        m_currentType = ENSkillConnectionType.enNone;
        if (type == ENSkillConnectionType.enConnect ||
            type == ENSkillConnectionType.enSpecial)
        {
            Start(type);
        }
        //隐藏uiskillcombo
        BattleArena.Singleton.NotifyChanged((int)BattleArena.ENPropertyChanged.enSkillCombo, false);
    }
    public void Tick()
    {
        if (Time.time - m_startTime > m_duration)
        {
            Stop(ENSkillConnectionType.enNone);
        }
    }
}
#endregion
#region QTEProps
public enum EnLevel
{
    enNone,
    enBasic,
    enMiddle,
    enHigh,
}

public class QTEProps
{
    public enum EnQteState
    {
        enStateStop,//停止
        enStateIng,//进行中
        enStateNext,//下一个
    }
    EnQteState m_qteState = EnQteState.enStateStop;

    public class QTEInfo
    {
        public enum EnQteInfoState
        {
            enQteInit,
            enQteIng,
            enQteSucc,
            enQteFail,
            enQteEnd,
        }
        public ENSkillConnectionType m_type = ENSkillConnectionType.enNone;
        public EnQteInfoState m_state = EnQteInfoState.enQteInit;
        public float m_cd = 0;
    }
    //qte所有的指令
    List<QTEInfo> m_qteList = new List<QTEInfo>();
    public List<QTEInfo> CurrentQteList { get { return m_qteList; } }
    //qte所有的指令
    int m_index = 0;
    public int CurrentIndex { get { return m_index; } private set { m_index = value; } }

    float m_cdStartTime = float.MinValue;//cd开始时间
    float m_cdDuration = 0;//cd间隔
    string m_qteIDList = "";//qteID的列表（世界参数表中读取）
    int m_qteSkillCount = 0;//qte所需的技能数（世界参数表中读取）
    EnLevel m_level = EnLevel.enNone;//当前角色的等级

    //战斗开始的时间
    private float m_stateFightStartTime = float.MaxValue;
    public float StateFightStartTime { get { return m_stateFightStartTime; } set { m_stateFightStartTime = value; } }
    float m_stateFightDuration = 0;//所需战斗状态开始之后的间隔（世界参数表中读取）
    //qte额外奖励
    int m_qteExtraReward = 0;
    public void Init()
    {
        m_qteState = EnQteState.enStateStop;
        m_cdStartTime = float.MinValue;
    }
    public void Stop()
    {
        if (m_qteState == EnQteState.enStateStop)
        {
            return;
        }
        m_qteList.Clear();
        //通知qte序列hide
        BattleArena.Singleton.NotifyChanged((int)BattleArena.ENPropertyChanged.enQteSequence, UIQTESequence.EnEventType.enHide);
        m_qteState = EnQteState.enStateStop;
    }
    public void Succ(int skillID, ENSkillConnectionType type)
    {
        if (m_qteState == EnQteState.enStateStop)
        {
            return;
        }
        if (CurrentIndex >= m_qteList.Count)
        {
            Stop();
            return;
        }
        if (type == m_qteList[CurrentIndex].m_type)
        {//succ
            m_qteList[CurrentIndex].m_state = QTEInfo.EnQteInfoState.enQteSucc;
            //通知qte序列
            BattleArena.Singleton.NotifyChanged((int)BattleArena.ENPropertyChanged.enQteSequence, UIQTESequence.EnEventType.enChange);
            //通知技能隐藏qte
            ActorManager.Singleton.MainActor.NotifyChanged((int)Actor.ENPropertyChanged.enBattleBtn_D, MainPlayer.ENBattleBtnNotifyType.enQteHide);

            m_qteList[CurrentIndex].m_state = QTEInfo.EnQteInfoState.enQteEnd;
            ++CurrentIndex;

            if (CurrentIndex == m_qteList.Count)
            {//所有成功
                //通知qte序列all succ
                BattleArena.Singleton.NotifyChanged((int)BattleArena.ENPropertyChanged.enQteSequence, UIQTESequence.EnEventType.enAllSucc);
                if (m_qteExtraReward == 0)
                {
                    m_qteExtraReward = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enQteExtraReward).IntTypeValue;
                }
                //add soulcharge
                if (BattleArena.Singleton.SkillCombo.SkillComboNumber != 0)
                {
                    //BattleArena.Singleton.SoulCharge.AddSoul(EnSoulType.enSkillCombo, BattleArena.Singleton.SkillCombo.SkillComboNumber);
                    //clear
                    BattleArena.Singleton.SkillCombo.ClearSkillCombo();
                    //connecteffect
                    if (BattleArena.Singleton.Combo.ConnectEffectNumber != 0)
                    {
                        //BattleArena.Singleton.SoulCharge.AddSoul(EnSoulType.enConnectEffect, BattleArena.Singleton.Combo.ConnectEffectNumber);
                        //clear connecteffectnumber
                        BattleArena.Singleton.Combo.ClearConnectEffect();
                    }
                }
                //BattleArena.Singleton.SoulCharge.AddSoul(EnSoulType.enQte, CurrentIndex);
                //BattleArena.Singleton.SoulCharge.AddSoul(EnSoulType.enExtra, m_qteExtraReward);

                //BattleArena.Singleton.SoulCharge.StopResult();
                Stop();
            }
            else
            {//通知下一个
                m_qteState = EnQteState.enStateNext;
                m_nextQteStartTime = Time.time;
                m_qteList[CurrentIndex].m_state = QTEInfo.EnQteInfoState.enQteIng;
                //通知qte序列
                BattleArena.Singleton.NotifyChanged((int)BattleArena.ENPropertyChanged.enQteSequence, UIQTESequence.EnEventType.enChange);
            }
        }
    }

    public void Fail()
    {
        if (CurrentIndex >= m_qteList.Count ||
            m_qteState == EnQteState.enStateStop)
        {
            return;
        }
        m_qteList[CurrentIndex].m_state = QTEInfo.EnQteInfoState.enQteFail;
        //通知qte序列
        BattleArena.Singleton.NotifyChanged((int)BattleArena.ENPropertyChanged.enQteSequence, UIQTESequence.EnEventType.enChange);
        //通知技能隐藏qte
        ActorManager.Singleton.MainActor.NotifyChanged((int)Actor.ENPropertyChanged.enBattleBtn_D, MainPlayer.ENBattleBtnNotifyType.enQteHide);

        m_qteList[CurrentIndex].m_state = QTEInfo.EnQteInfoState.enQteEnd;
        Stop();
    }
    //下一个qte指令的开始时间
    float m_nextQteStartTime = 0;
    //下一个qte指令的修正时间
    float m_nextQteModifyTime = 0;
    float NextQteModifyTime
    {
        get
        {
            if (m_nextQteModifyTime == 0)
            {
                m_nextQteModifyTime = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enNextQteModifyTime).FloatTypeValue;
            }
            return m_nextQteModifyTime;
        }
    }
    //qte指令计时开始的时间
    float m_qteSingleStartTime = 0;
    //qte指令的计时时间
    float m_qteSingleCD = 0;
    float QteSingleCD
    {
        get
        {
            if (m_qteSingleCD == 0)
            {
                m_qteSingleCD = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enQteSingleCDTime).FloatTypeValue;
            }
            return m_qteSingleCD;
        }
    }
    public void Tick()
    {
        //战斗开始的时间
        if (ActorManager.Singleton.MainActor.IsState_Fight)
        {
            if (StateFightStartTime == float.MaxValue)
            {
                StateFightStartTime = Time.time;
            }
        }
        else
        {
            StateFightStartTime = float.MaxValue;
        }
        //level
        EnLevel level = GetCurrentLevel();
        if (level != m_level)
        {
            m_level = level;
            switch (m_level)
            {
                case EnLevel.enBasic:
                    {
                        m_cdDuration = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enQTECDTime_Basic).FloatTypeValue;
                        m_qteIDList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enQTETableIDList_Basic).StringTypeValue;
                        m_qteSkillCount = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enQTESkillCount_Basic).IntTypeValue;
                        m_stateFightDuration = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enQTEDuration_NoSkill).FloatTypeValue;
                    }
                    break;
                case EnLevel.enMiddle:
                    {
                        m_cdDuration = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enQTECDTime_Middle).FloatTypeValue;
                        m_qteIDList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enQTETableIDList_Middle).StringTypeValue;
                        m_qteSkillCount = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enQTESkillCount_Middle).IntTypeValue;
                        m_stateFightDuration = 0;
                    }
                    break;
                case EnLevel.enHigh:
                    {
                        m_cdDuration = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enQTECDTime_High).FloatTypeValue;
                        m_qteIDList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enQTETableIDList_High).StringTypeValue;
                        m_qteSkillCount = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enQTESkillCount_High).IntTypeValue;
                        m_stateFightDuration = 0;
                    }
                    break;
                default:
                    return;
            }
        }
        float now = Time.time;
        switch (m_qteState)
        {
            case EnQteState.enStateStop:
                {
                    if (now - m_cdStartTime > m_cdDuration)
                    {
                        AIPlayer ai = ActorManager.Singleton.MainActor.SelfAI as AIPlayer;
                        bool isSateFight = ActorManager.Singleton.MainActor.IsState_Fight;

                        if (isSateFight && now - StateFightStartTime > m_stateFightDuration)
                        {//战斗状态
                            if (ai.FireSkillIDListWithoutNormal.Count >= m_qteSkillCount)
                            {//可释放技能
                                if (!string.IsNullOrEmpty(m_qteIDList))
                                {
                                    string[] array = m_qteIDList.Split(new char[1] { ',' });
                                    int r = UnityEngine.Random.Range(0, array.Length);
                                    int id = int.Parse(array[r]);

                                    QTESequenceInfo info = GameTable.QTESequenceTableAsset.LookUp(id);
                                    foreach (var item in info.SequenceList)
                                    {
                                        QTEInfo qteInfo = new QTEInfo();
                                        qteInfo.m_type = (ENSkillConnectionType)item;
                                        qteInfo.m_cd = QteSingleCD;
                                        qteInfo.m_state = QTEInfo.EnQteInfoState.enQteInit;

                                        m_qteList.Add(qteInfo);
                                    }
                                    if (m_qteList.Count != 0)
                                    {
                                        CurrentIndex = 0;
                                        m_qteList[CurrentIndex].m_state = QTEInfo.EnQteInfoState.enQteIng;
                                        //通知qte序列
                                        BattleArena.Singleton.NotifyChanged((int)BattleArena.ENPropertyChanged.enQteSequence, UIQTESequence.EnEventType.enShow);
                                        //通知技能显示qte
                                        ActorManager.Singleton.MainActor.NotifyChanged((int)Actor.ENPropertyChanged.enBattleBtn_D, MainPlayer.ENBattleBtnNotifyType.enQteShow);

                                        m_cdStartTime = now;
                                        m_qteState = EnQteState.enStateIng;
                                        m_qteSingleStartTime = now;
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            case EnQteState.enStateIng:
                {
                    if (now - m_qteSingleStartTime > QteSingleCD)
                    {//fail
                        Fail();
                    }
                }
                break;
            case EnQteState.enStateNext:
                {
                    if (now - m_nextQteStartTime > NextQteModifyTime)
                    {
                        //通知技能显示qte
                        ActorManager.Singleton.MainActor.NotifyChanged((int)Actor.ENPropertyChanged.enBattleBtn_D, MainPlayer.ENBattleBtnNotifyType.enQteShow);
                        m_qteState = EnQteState.enStateIng;
                        m_qteSingleStartTime = now;
                    }
                }
                break;
            default:
                return;
        }
    }
    int m_levelBasic = 0;
    int m_levelMiddle = 0;
    int m_levelHigh = 0;
    EnLevel GetCurrentLevel()
    {
        if (m_levelBasic == 0)
        {
            m_levelBasic = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enActorLevel_Basic).IntTypeValue;
        }
        if (m_levelMiddle == 0)
        {
            m_levelMiddle = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enActorLevel_Middle).IntTypeValue;
        }
        if (m_levelHigh == 0)
        {
            m_levelHigh = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enActorLevel_High).IntTypeValue;
        }
        int level = ActorManager.Singleton.MainActor.Level;
        if (level < m_levelBasic)
        {
            return EnLevel.enBasic;
        }
        else if (level < m_levelMiddle)
        {
            return EnLevel.enMiddle;
        }
        else if (level < m_levelHigh)
        {
            return EnLevel.enHigh;
        }
        return EnLevel.enNone;
    }
}
#endregion
#region SwordSoulProps//剑魂
public class SwordSoulProps
{
    public enum ENAddComboType
    {
        enNone,
        enNormal,
        enBreak,
    }
    #region//剑魂值
    float m_swordSoulValue = 0;
    public float SwordSoulValue
    {
        get
        {
            return m_swordSoulValue;
        }
        private set
        {
            m_swordSoulValue = value;
        }
    }
    #endregion
    #region//剑魂结算中normal的最小值
    float m_minNormal = 0;
    float MinNormal
    {
        get
        {
            if (m_minNormal == 0)
            {
                m_minNormal = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSwordSoulMinNormal).IntTypeValue;
            }
            return m_minNormal;
        }
    }
    #endregion
    #region//剑魂最大值
    float m_maxValue = 0;
    float MaxValue
    {
        get
        {
            if (m_maxValue == 0)
            {
                m_maxValue = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSwordSoulMaxValue).IntTypeValue;
            }
            return m_maxValue;
        }
    }
    #endregion
    #region//剑魂技的百分比
    public float Percent { get { return SwordSoulValue / MaxValue; } }
    #endregion

    public void Add(int combo, ENAddComboType type)
    {
        ComboSwordSoulInfo info = GameTable.ComboSwordSoulAsset.LookUp(combo);
        if (info == null)
        {
            if (combo != 1 && combo != 0)
            {
                Debug.LogWarning("combo soul charge error,combo:" + combo);
            }
            return;
        }
        float addValue = 0;
        switch (type)
        {
            case ENAddComboType.enNormal:
                {
                    if (info.m_normalResult >= MinNormal)
                    {
                        addValue += info.m_normalResult;
                    }
                }
                break;
            case ENAddComboType.enBreak:
                {
                    addValue += info.m_breakResult;
                }
                break;
        }
        if (addValue > 0)
        {
            SwordSoulValue += addValue;
            if (SwordSoulValue > MaxValue)
            {
                SwordSoulValue = MaxValue;
            }
            BattleArena.Singleton.NotifyChanged((int)BattleArena.ENPropertyChanged.enSwordSoul, null);
        }
    }
    public void Clear()
    {
        SwordSoulValue = 0;
        BattleArena.Singleton.NotifyChanged((int)BattleArena.ENPropertyChanged.enSwordSoul, null);
    }
}
/*
public enum EnSoulType
{
    enSkillCombo,//技能combo
    enConnectEffect,//一闪特效
    enQte,//qte
    enExtra,//额外奖励
}
public class SoulChargeProps
{
    enum EnNotifyType
    {
        enNone,
        enIng,
        enNext,
    }
    //剑魂结算信息
    public class SoulInfo
    {
        public List<EnSoulType> m_typeList = new List<EnSoulType>();
        public List<int> m_soulList = new List<int>();
        public int m_total = 0;
        public bool m_isIng = false;//是否在进行中
        public bool m_isRemove = false;
    }
    //主控角色和副角色的剑魂列表
    Dictionary<MainPlayer, List<SoulInfo>> m_actorSoulList = new Dictionary<MainPlayer, List<SoulInfo>>();
    //主控角色
    MainPlayer m_mainActor { get { return ActorManager.Singleton.MainActor; } }
    //通知ui的类型
    EnNotifyType m_notifyType = EnNotifyType.enNone;
    //通知ui的soulInfo
    public SoulInfo NotifySoulInfo = null;
    public void Clear()
    {
        m_actorSoulList.Clear();
        m_notifyType = EnNotifyType.enNone;
        NotifySoulInfo = null;
    }
    //结算
    public void AddSoul(EnSoulType type, int soul)
    {
        if (!m_actorSoulList.ContainsKey(m_mainActor))
        {
            m_actorSoulList.Add(m_mainActor, new List<SoulInfo>());
        }
        SoulInfo info = m_actorSoulList[m_mainActor].Find(item => item.m_isIng);
        if (info == null)
        {
            info = new SoulInfo();
            info.m_isIng = true;
            info.m_isRemove = false;
            m_actorSoulList[m_mainActor].Add(info);
        }
        info.m_typeList.Add(type);
        info.m_soulList.Add(soul);
        info.m_total += soul;

        m_mainActor.SoulCount += soul;
    }
    //所有结算结束（AddSoul）后调用
    public void StopResult()
    {
        if (!m_actorSoulList.ContainsKey(m_mainActor))
        {
            m_actorSoulList.Add(m_mainActor, new List<SoulInfo>());
        }
        SoulInfo info = m_actorSoulList[m_mainActor].Find(item => item.m_isIng);
        if (info != null)
        {
            info.m_isIng = false;
        }
        if (m_notifyType != EnNotifyType.enIng)
        {
            m_notifyType = EnNotifyType.enNext;
        }
    }
    //被ui(UIMainHead)通知，m_actorSoulList中的下一个
    public void Next()
    {
        m_notifyType = EnNotifyType.enNext;
        m_mainActor.NotifyChanged((int)Actor.ENPropertyChanged.enSoulCharge, UISoulCharge.EnEventType.enHide);
    }
    public void Tick()
    {
        switch (m_notifyType)
        {
            case EnNotifyType.enIng:
                break;
            case EnNotifyType.enNext:
                {
                    foreach (var item in m_actorSoulList)
                    {
                        item.Value.RemoveAll(info => info.m_isRemove);
                        NotifySoulInfo = item.Value.Find(info => !info.m_isIng);
                        if (NotifySoulInfo != null)
                        {
                            item.Key.NotifyChanged((int)Actor.ENPropertyChanged.enMainHead, EnMainHeadType.enSoulCharge);
                            item.Key.NotifyChanged((int)Actor.ENPropertyChanged.enSoulCharge, UISoulCharge.EnEventType.enShow);

                            NotifySoulInfo.m_isRemove = true;
                            m_notifyType = EnNotifyType.enIng;
                            return;
                        }
                    }
                    m_notifyType = EnNotifyType.enNone;
                }
                break;
        }
    }
}
*/
#endregion
//切入技步骤
public enum ENSwitchStep
{
    enNone,
    enJumpin,
    enAttack,
    enJumpout,
}
public class LevelBlackboard
{
    public enum BlackActorType
    {
        enNone = -1,
        enNPC,
        enBox,
    }
    public Dictionary<string, SM.BlackBoardActorData> mDicBlackActor = new Dictionary<string, SM.BlackBoardActorData>();
    public EventBase mDefEvent = null;

    public void Delete()
    {
        mDicBlackActor.Clear();
        mDefEvent = null;
    }

    //向黑板上写有关Actor的数据
    public void AddBlackBoardActor(string actorStr, Actor actor, BlackActorType type = BlackActorType.enNPC)
    {
        if (actor == null)
        {
            return;
        }
        SM.BlackBoardActorData tmpActorData = new SM.BlackBoardActorData();
        tmpActorData.roomID = actor.mCurRoomGUID;
        tmpActorData.actorID = actor.roomElement.MonsterData.monsterId;
        tmpActorData.mBlackActorType = type;
        AddBlackBoardActor(actorStr, tmpActorData);
    }
    public void AddBlackBoardActor(string actorStr, SM.BlackBoardActorData actor)
    {
        if (actor == null)
        {
            return;
        }
        if (mDicBlackActor.ContainsKey(actorStr))
        {
            mDicBlackActor[actorStr] = actor;
        }
        else
        {
            mDicBlackActor.Add(actorStr, actor);
        }

    }
    //从黑板上Actor数据的字典中获取Actor数据
    public Actor GetBlackBoardActor(string actorStr)
    {
        SM.BlackBoardActorData blackBoardActorData = mDicBlackActor[actorStr];
        SM.SceneRoom room = SM.RandomRoomLevel.Singleton.LookupRoom(blackBoardActorData.roomID);
        if (null != room)
        {
            if (blackBoardActorData.mBlackActorType == BlackActorType.enNPC)
            {
                return room.GetMonster(blackBoardActorData.actorID);
            }
            else if (blackBoardActorData.mBlackActorType == BlackActorType.enBox)
            {
                return room.GetTreatureByID(blackBoardActorData.actorID);
            }
        }
        return null;
    }



}
