using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public enum ENRelation
    {
        enUnable,	//�¶�������ִ��
        enBothCan,	//��������Թ���
        enReplace,	//�¶����滻�ɶ���
    }
    public enum ENBuffType
    {
        enBothCan,//����
        enReplace,//�滻
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

    //�Ƿ��������
    bool m_isCalcProperty;
    //!tick interval,milli second
    int m_tickInterval;
    //!��Ҫ���˴���
    bool m_isNeedResultPass;
    //!�Ƿ�����
    bool m_isTickNeed;
    //!��Щ�߼����ͳ���ʱ,��Ҫ���˴���
    int m_affectLogicTypeMask;
    //!buff id
    int m_buffID;
    //!����Ƿ��Ƴ�
    bool m_markRemove;
    //�������ʱ�Ƿ��Ƴ�
    bool m_markRemoveForDead;
    //!�ȼ�
    int m_level;

    //!��ʱ��
    float m_timer;

    //������Чʱ��
    bool m_isPeriodNeed;
    float m_period;//���ڼ��
    float m_periodStartTime;//���ڿ�ʼʱ��
    int m_periodWorkCount;//������Ч����

    //�Ƿ�������Ч
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

    //!���ɿ��ټ���־,IsNeedTick,NeedPassResult
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