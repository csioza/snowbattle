using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
public class CD
{
    public int CDID { get; set; }
    public bool IsNeedRemove { get; set; }
    public float m_cdTime { get; set; }
    private float m_startTime { get; set; }
    private int m_skillID = 0;
    public CD(int cdID, int skillID)
    {
        CDID = cdID;
        m_skillID = skillID;
    }
    public virtual void OnEnter(Actor actor)
    {
        int skillLevel = 0;
        float cdGrowParma = 0;
        Actor.ActorSkillInfo info = actor.SkillBag.Find(item => item.SkillTableInfo.ID == m_skillID);
        if (info != null)
        {//获得技能等级和cd成长
            skillLevel =  info.SkillLevel;
            cdGrowParma = info.SkillTableInfo.CoolDownParam;
        }
        CDInfo cdinfo = GameTable.CDTableAsset.Lookup(CDID);
        float timer = cdinfo.CDTime + skillLevel * cdGrowParma;
        m_cdTime = timer * (1 + actor.SkillCDModifyPercent) + actor.SkillCDModifyValue;
        m_startTime = Time.time;
    }
    public virtual void OnExit(Actor actor)
    {

    }
    public virtual void Tick(Actor actor)
    {
        if (Time.time - m_startTime >= m_cdTime)
        {
            IsNeedRemove = true;
        }
        if (m_cdTime <= 0)
        {//被修改
            IsNeedRemove = true;
            actor.NotifyChanged(m_skillID, 0.0f);
        }
        else
        {
            float fEclipse = 1 - ((Time.time - m_startTime) / m_cdTime);
            if (fEclipse < 0)
            {
                fEclipse = 0;
            }
            else if (fEclipse > 1)
            {
                fEclipse = 1;
            }
            actor.NotifyChanged(m_skillID, fEclipse);
        }
    }
    public void Remove(Actor actor = null)
    {
        m_cdTime = 0;
    }
    public float GetStartTime()
    {
        return m_startTime;
    }
    public void SetCDTime(float time)
    {
        m_cdTime = time;
    }
    public int GetCDSkillID()
    {
        return m_skillID;
    }
}
