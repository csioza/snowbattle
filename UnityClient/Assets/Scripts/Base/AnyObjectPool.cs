using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//!对象池
public class AnyObjectPool {
    Dictionary<object, List<object>> m_pool = new Dictionary<object, List<object>>();
    //从池中获取一个对象,如果池为空，则返回null
    public object GetObjectFromPool(object id)
    {
        List<object> l;
        if (m_pool.TryGetValue(id,out l))
        {
            if (l.Count > 0)
            {
                object o = l[l.Count - 1];
                l.RemoveAt(l.Count - 1);
                return o;
            }
        }
        return null;
    }

    public void ReleaseObject(object id, object obj)
    {
        List<object> l;
        if (!m_pool.TryGetValue(id, out l))
        {
            l = new List<object>();
            m_pool.Add(id, l);
        }
        l.Add(obj);
    }
    public void DestroyAll()
    {
        foreach (var it in m_pool)
        {
            List<object> l = it.Value;
            l.Clear();
        }
        m_pool.Clear();
    }
}

public class AnyObjectPoolMgr
{
    static AnyObjectPoolMgr m_singleton;
    static public AnyObjectPoolMgr Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new AnyObjectPoolMgr();
            }
            return m_singleton;
        }
    }

    
    List<AnyObjectPool> m_allPool = new List<AnyObjectPool>();
    public AnyObjectPool CreatePool()
    {
        AnyObjectPool pool = new AnyObjectPool();
        m_allPool.Add(pool);
        return pool;
    }
    public void DestroyAll()
    {
        for (int i = 0; i < m_allPool.Count;i++ )
        {
            AnyObjectPool pool = m_allPool[i];
            pool.DestroyAll();
        }
        System.GC.Collect();
    }
}
