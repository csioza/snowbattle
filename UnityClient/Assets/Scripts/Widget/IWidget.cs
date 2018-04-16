//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Widget
//	created:	2013-4-9
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class IWidget
{
	public bool IsEnable { get; protected set; }

	public virtual void Update() { }
	public virtual void FixedUpdate() { }

	public virtual void Init() { }
	//释放回池
	public virtual void Release() { }
	//彻底销毁所有资源
    public virtual void Destroy() { }
    //通知
    public virtual void Notify() { }
};

public sealed class IPoolWidget<T> where T : IWidget, new()
{
	static public T CreateObj()
	{
		T obj;
		if (m_pool.Count > 0)
		{
			obj = m_pool.Last.Value;
			m_pool.RemoveLast();
		}
		else
		{
			obj = new T();
		}

		obj.Init();
		return obj;
	}
	static public void ReleaseObj(T obj)
	{
		if (m_pool.Count < m_maxPoolCount)
		{
			m_pool.AddLast(obj);
		}
		else
		{
			obj.Destroy();
		}
	}
	static private LinkedList<T> m_pool = new LinkedList<T>();
	static private int m_maxPoolCount = 10;
}