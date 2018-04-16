using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public enum ENRelation
    {
        enUnable,	//新动作不许执行
        enBothCan,	//动作间可以共存
        enReplace,	//新动作替换旧动作
    }
    public enum ENBuffType
    {
        enBothCan,//共存
        enReplace,//替换
        Count,
    }
    public bool IsCalcProperty { get { return m_isCalcProperty; } set { m_isCalcProperty = value; } }
    public int BuffID { get { return m_buffID; } set { m_buffID = value; } }
    public bool IsNeedRemove { get { return m_markRemove; } set { m_markRemove = value; } }
    public bool IsNeedRemoveForDead { get { return m_markRemoveForDead; } set { m_markRemoveForDead = value; } }
    public GameObject EffectObj { get { return m_effectObj; } set { m_effectObj = value; } }
    public float BuffTimer { get { return m_timer; } set { m_timer = value; } }
    private List<IBuffEffect> m_effects = new List<IBuffEffect>();
    public List<IBuffEffect> EffectList { get { return m_effects; } }

    //是否结算属性
    bool m_isCalcProperty;
    //!tick interval,milli second
    int m_tickInterval;
    //!需要过滤处理
    bool m_isNeedResultPass;
    //!是否心跳
    bool m_isTickNeed;
    //!这些逻辑类型出现时,需要过滤处理
    int m_affectLogicTypeMask;
    //!buff id
    int m_buffID;
    //!标记是否移除
    bool m_markRemove;
    //标记死亡时是否移除
    bool m_markRemoveForDead;
    //!等级
    int m_level;

    //!计时器
    float m_timer;

    //周期生效时间
    bool m_isPeriodNeed;
    float m_period;//周期间隔
    float m_periodStartTime;//周期开始时间
    int m_periodWorkCount;//周期生效次数

    //是否永久生效
    bool m_permanent=false;

    GameObject m_effectObj = null;

    public Buff(int id, bool isRemoveForDead, float timer, bool isFirstWork, float period)
    {
        BuffID = id;
        IsNeedRemoveForDead = isRemoveForDead;
        m_timer = timer;
        IsNeedRemove = false;
        SetPeriod(isFirstWork, period);
    }

    public bool IsNeedTick()
    {
	    return m_isTickNeed || m_isPeriodNeed;
    }
    public int GetBuffLevel()
    {
	    return m_level;
    }

    public void SetBuffLevel(int lv)
    {
	    m_level=lv;
    }

    public void AddBuffEffect(IBuffEffect effect)
    {
	    m_effects.Add(effect);
    }

    public bool IsExsit(int buffEffectID)
    {
        return m_effects.Find(item => item.BuffEffectID == buffEffectID) != null;
    }

    public float GetPeriod()
    {
	    return m_period;
    }

    public void SetPeriod(bool isFirstWork, float period)
    {
        m_periodWorkCount = isFirstWork ? 1 : 0;
	    m_period = period;
        m_periodStartTime = Time.time - Time.deltaTime;
    }

    public void OnProduceResult(IResult result, IResultControl control)
    {
        foreach(IBuffEffect e in m_effects)
        {
            if(e.IsNeedResultPass)
            {
                e.OnProduceResult(result,control);
            }
        }
    }
    public void OnGetResult(IResult result, IResultControl control)
    {
	    foreach(IBuffEffect e in m_effects)
        {
            if(e.IsNeedResultPass)
            {
                e.OnGetResult(result,control);
            }
        }
    }
    public void MarkRemove(IResultControl control)
    {
        if (IsNeedRemove == false)
        {
            IsNeedRemove = true;
            foreach (IBuffEffect e in m_effects)
            {
                e.OnRemoved(control);
            }
        }
        if (EffectObj != null)
        {
            PoolManager.Singleton.ReleaseObj(EffectObj);
            EffectObj = null;
        }
    }
    public void IsRemoveBuff(float dt, IResultControl control)
    {
        if (!m_permanent)
        {
            m_timer -= dt;
            if (m_timer <= 0.0f)
            {
                MarkRemove(control);
            }
        }
    }
    public void Tick(IResultControl control, float dt)
    {
        if (IsNeedRemove)
	    {
		    return ;
	    }
	    if (m_isPeriodNeed)
	    {
            int count = (int)((Time.time - m_periodStartTime) / m_period);
            m_periodStartTime += m_period * count;
            m_periodWorkCount += count;
	    }
        
	    foreach(IBuffEffect e in m_effects)
        {
            if (IsNeedRemove)
		    {
			    e.OnRemoved(control);
		    }

		    if (e.IsNeedTick)
		    {
			    e.Tick(control,dt);
		    }

            for (int i = 0; i < m_periodWorkCount; ++i)
            {
                e.Exec(control);
            }
        }
        m_periodWorkCount = 0;
    }

    //!生成快速检查标志,IsNeedTick,NeedPassResult
    public void UpdateFastFilterFlag()
    {
	    m_permanent = m_timer <= 0;
	    m_isPeriodNeed = m_period > 0;

	    m_affectLogicTypeMask=0;
	    m_isNeedResultPass=false;
	    m_isTickNeed=false;
        foreach (IBuffEffect e in m_effects)
	    {
		    m_affectLogicTypeMask |= e.AffectLogicTypeMask;
		    m_isNeedResultPass|=e.IsNeedResultPass;
		    m_isTickNeed |= e.IsNeedTick;
	    }
    }
}