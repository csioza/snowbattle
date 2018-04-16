using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//!Buff,BuffEffect,Result的工厂方法,负责查找GameObject,负责创建对应的ResultControl
public class BattleContext {
    public delegate IResult IResultFactory();
    public delegate IBuffEffect IBuffEffectFactory();
	BattleContext	m_parent;
	Dictionary<int,IResultFactory>	m_resultFactory = new Dictionary<int,IResultFactory>();
    Dictionary<int, IBuffEffectFactory> m_effectFactory = new Dictionary<int, IBuffEffectFactory>();

    IResultControl m_resultControl = null;
    AnyObjectPool m_resultPool;
    AnyObjectPool m_effectPool;
    public BattleContext(BattleContext parent)
    {
        m_resultPool = AnyObjectPoolMgr.Singleton.CreatePool();
        m_effectPool = AnyObjectPoolMgr.Singleton.CreatePool();
        m_parent = parent;
    }
    public BattleContext()
	{
		m_resultPool = AnyObjectPoolMgr.Singleton.CreatePool();
		m_effectPool = AnyObjectPoolMgr.Singleton.CreatePool();
    }

    public void ReleaseResult(IResult result)
    {
        result.Reset();
        m_resultPool.ReleaseObject(result.ClassID, result);
    }

    public void ReleaseBuffEffect(IBuffEffect eff)
    {
        eff.Reset();
        m_effectPool.ReleaseObject(eff.ClassID, eff);
    }
	public virtual IResult CreateResult(int classID)
    {
        if (m_parent != null)
        {
            return m_parent.CreateResult(classID);
        }
        IResult r = m_resultPool.GetObjectFromPool(classID) as IResult;
        if (r != null)
        {
            return r;
        }
        
        IResultFactory f = null;
        if(m_resultFactory.TryGetValue(classID,out f))
        {
            return f();
        }
        return null;
    }

	public virtual IBuffEffect CreateBuffEffect(int effectID)
    {
        if (m_parent != null)
        {
            return m_parent.CreateBuffEffect(effectID);
        }
        IBuffEffect eff = m_resultPool.GetObjectFromPool(effectID) as IBuffEffect;
        if (eff != null)
        {
            return eff;
        }
        IBuffEffectFactory f = null;
        if(m_effectFactory.TryGetValue(effectID,out f))
        {
            return f();
        }
        return null;
    }

	public virtual void RegisterResult(int classID,IResultFactory f)
    {
        m_resultFactory[classID]=f;
    }
	public virtual void RegisterBuffEffect(int effectID,IBuffEffectFactory f)
    {
        m_effectFactory[effectID]=f;
    }
	

	public virtual IResultControl CreateResultControl()
    {
        if (null == m_resultControl)
        {
            m_resultControl = new IResultControl();
            m_resultControl.SetContext(this);
        }
        return m_resultControl;
    }
};