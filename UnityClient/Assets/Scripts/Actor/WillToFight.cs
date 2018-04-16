using System;
using UnityEngine;

//战斗意志
public class WillToFight
{
    //添加意志的来源枚举
    public enum ENAddWillType
    {
        enNone,
        enBeAttacked,//受击
        enSkillResult,//skillresult增益
    }
    #region WillValue//意志值
    float m_willValue = 0;
    public float WillValue
    {
        get
        {
            return m_willValue;
        }
        private set
        {
            m_willValue = value;
            if (m_willValue <= WillValueMin)
            {
                m_willValue = WillValueMin;
                IsWillBurning = false;
            }
            else if (m_willValue >= WillValueMax)
            {
                IsWillBurning = true;
            }
            //Debug.Log("will value:" + m_willValue);
        }
    }
    #endregion
    #region IsWillBurning//战斗意志是否燃烧
    bool m_isWillBurning = false;
    public bool IsWillBurning
    {
        get
        {
            return m_isWillBurning;
        }
        private set
        {
            if (m_isWillBurning == value)
            {
                return;
            }
            m_isWillBurning = value;
            if (m_isWillBurning)
            {//添加buff
                if (Owner != null && !Owner.IsDead)
                {//不死才能添加buff
                    BattleFactory.Singleton.AddBuff(Owner.ID, Owner.ID, FightWillBuffid);
                }
            }
            else
            {//删除buff
                BattleFactory.Singleton.RemoveBuff(Owner.ID, Owner.ID, FightWillBuffid);
            }
            //Debug.Log("will burning:" + m_isWillBurning);
        }
    }
    #endregion
    //持有者
    public Actor Owner { get; private set; }

    //timer
    float m_start = 0;

    #region WillValueMax//意志最大值和最小值
    float m_willValueMax = 0;
    float m_willValueMin = 0;
    float WillValueMax
    {
        get
        {
            if (m_willValueMax == 0)
            {
                WorldParamInfo info = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enWillValue);
                m_willValueMax = info.FloatTypeValue;
                m_willValueMin = info.IntTypeValue;
            }
            return m_willValueMax;
        }
    }
    float WillValueMin
    {
        get
        {
            if (m_willValueMin == 0)
            {
                WorldParamInfo info = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enWillValue);
                m_willValueMax = info.FloatTypeValue;
                m_willValueMin = info.IntTypeValue;
            }
            return m_willValueMin;
        }
    }
    #endregion
    #region TargetExistGet//注视增益值
    float m_targetExistGet = 0;
    float TargetExistGet
    {
        get
        {
            if (m_targetExistGet == 0)
            {
                m_targetExistGet = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enTargetExistGet).FloatTypeValue;
            }
            return m_targetExistGet;
        }
    }
    #endregion
    #region BeAttackedGet//受击增益值
    float m_beAttackedGetMax = 0;
    float m_beAttackedGetMin = 0;
    float BeAttackedGetMax
    {
        get
        {
            if (m_beAttackedGetMax == 0)
            {
                WorldParamInfo info = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enBeAttackedGet);
                m_beAttackedGetMax = info.FloatTypeValue;
                m_beAttackedGetMin = info.IntTypeValue;
            }
            return m_beAttackedGetMax;
        }
    }
    float BeAttackedGetMin
    {
        get
        {
            if (m_beAttackedGetMin == 0)
            {
                WorldParamInfo info = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enBeAttackedGet);
                m_beAttackedGetMax = info.FloatTypeValue;
                m_beAttackedGetMin = info.IntTypeValue;
            }
            return m_beAttackedGetMin;
        }
    }
    #endregion
    #region TimeflowGet//正常时间减益值
    float m_timeflowGet = 0;
    float TimeflowGet
    {
        get
        {
            if (m_timeflowGet == 0)
            {
                m_timeflowGet = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enTimeflowGet).FloatTypeValue;
            }
            return m_timeflowGet;
        }
    }
    #endregion
    #region FightWillLose//意志燃烧减益值
    float m_fightWillLose = 0;
    float FightWillLose
    {
        get
        {
            if (m_fightWillLose == 0)
            {
                m_fightWillLose = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enFightWillLose).FloatTypeValue;
            }
            return m_fightWillLose;
        }
    }
    #endregion
    #region FightWillBuffid//意志燃烧添加的buffid
    int m_fightWillBuffid = 0;
    int FightWillBuffid
    {
        get
        {
            if (m_fightWillBuffid == 0)
            {
                m_fightWillBuffid = (int)GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enFightWillBuffid).FloatTypeValue;
            }
            return m_fightWillBuffid;
        }
    }
    #endregion
    #region FightWillParam//y值
    float m_fightWillParam = 0;
    float FightWillParam
    {
        get
        {
            if (m_fightWillParam == 0)
            {
                m_fightWillParam = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enFightWillParam).FloatTypeValue;
            }
            return m_fightWillParam;
        }
    }
    #endregion

    public WillToFight(Actor actor)
    {
        Owner = actor;
        m_start = Time.time;
    }
    public void Tick()
    {
        if (Owner == null) return;
        if (Owner.IsDead)
        {
            if (WillValue != 0)
            {
                WillValue = 0;
            }
            return;
        }

        if (Time.time - m_start > 1f)
        {
            m_start += 1f;

            float value = 0;
            if (!Owner.CurrentTargetIsDead)
            {//注视增益
                value += TargetExistGet;
            }
            //正常时间减益
            value -= TimeflowGet;
            if (IsWillBurning)
            {//意志燃烧减益
                value -= FightWillLose;
            }

            WillValue += value;
        }
    }
    public void AddWill(ENAddWillType type, float[] paramList)
    {
        if (Owner == null) return;
        float value = 0;
        switch (type)
        {
            case ENAddWillType.enBeAttacked:
                {
                    if (paramList == null || paramList.Length < 1)
                    {
                        return;
                    }
                    value = paramList[0] / Owner.MaxHP * 100;
                    if (value > BeAttackedGetMax)
                    {
                        value = BeAttackedGetMax;
                    }
                    else if (value < BeAttackedGetMin)
                    {
                        value = BeAttackedGetMin;
                    }
                }
                break;
            case ENAddWillType.enSkillResult:
                {
                    if (paramList == null || paramList.Length < 2)
                    {
                        return;
                    }
                    if (!IsWillBurning)
                    {
                        value = paramList[0];
                    }
                    value += paramList[1];
                }
                break;
        }
        {//生命余值加成
            float percent = Owner.HP / Owner.MaxHP;
            value += (-Mathf.Log(percent) * FightWillParam + 1);
        }
        WillValue += value;
    }
}