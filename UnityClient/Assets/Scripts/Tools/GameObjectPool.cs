//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Tools
//	created:	2013-6-20
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectPoolIdentifier : MonoBehaviour
{
	public GameObjectPool OwnPool { get; set; }
}

public sealed class GameObjectPool
{
	public GameObject CreateObj(bool isAutoActive = false)
	{
		GameObject obj;
		if (m_pool.Count > 0)
		{
			obj = m_pool.Last.Value;
			m_pool.RemoveLast();
         
 
			//obj.transform.position = m_source.transform.position;
			//obj.transform.rotation = m_source.transform.rotation;
		}
		else
		{
			obj = GameObject.Instantiate(m_source) as GameObject;
            GameObjectPoolIdentifier pool = obj.GetComponent<GameObjectPoolIdentifier>();
            if (pool == null)
            {
                pool = obj.AddComponent<GameObjectPoolIdentifier>();
            }
            pool.OwnPool = this;
		}
        obj.name = m_source.name;
        if (Application.isEditor)
        {
            obj.transform.parent = m_selfRoot.transform;
        }
        obj.SetActive(isAutoActive);
        return obj;
	}
	public void ReleaseObj(GameObject obj)
	{
		if (m_pool.Count < m_maxPoolCount)
		{
			obj.transform.parent = null;
            m_pool.AddLast(obj);
            ClearParticleSystem(obj.transform);
            obj.SetActive(false);
            if (Application.isEditor && m_selfRoot != null)
            {
                obj.transform.parent = m_selfRoot.transform;
            }
		}
		else
		{
			GameObject.Destroy(obj);
		}
	}
    void ClearParticleSystem(Transform t)
    {
        for (int i = 0; i < t.childCount; ++i)
        {
            Transform childT = t.GetChild(i);
            ClearParticleSystem(childT);
        }
        ParticleSystem partSys = t.GetComponent<ParticleSystem>();
        if (null != partSys)
        {
            partSys.Stop();
            partSys.Clear();
        }
    }
    public void ReleaseAll()
    {
        foreach (var item in m_pool)
        {
            ClearParticleSystem(item.transform);
            item.SetActive(false);
        }
    }
    public void DestroyAll()
    {
        foreach (var item in m_pool)
        {
            GameObject.Destroy(item);
        }
        //GameObject.Destroy(m_selfRoot);
        //m_selfRoot = null;
        m_pool.Clear();
    }

	public GameObjectPool(UnityEngine.Object source,GameObject editorRoot)
	{
        m_editorRoot = editorRoot;
		Init(source, 10);
	}
    public GameObjectPool(UnityEngine.Object source, int maxPoolCount, GameObject editorRoot)
    {
        m_editorRoot = editorRoot;
		Init(source, maxPoolCount);
	}
    private void Init(UnityEngine.Object source, int maxPoolCount)
	{
		m_source = source;
		m_maxPoolCount = maxPoolCount;
        if (Application.isEditor)
        {
            if (m_selfRoot == null)
            {
                m_selfRoot = new GameObject(source.name);
            }
            m_selfRoot.name = source.name;
            if (m_editorRoot != null)
            {
                m_selfRoot.transform.parent = m_editorRoot.transform;
            }
        }
	}
    public UnityEngine.Object SourceResource { get { return m_source; } }
	private LinkedList<GameObject> m_pool = new LinkedList<GameObject>();
	private int m_maxPoolCount = 10;
    private UnityEngine.Object m_source;
    private GameObject m_editorRoot;
    GameObject m_selfRoot;
};

public sealed class PoolManager
{
	#region Singleton
	static PoolManager m_singleton;
	static public PoolManager Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new PoolManager();
			}
			return m_singleton;
		}
	}
	#endregion
    GameObject m_poolRoot;
    public PoolManager()
    {
        if (Application.isEditor)
        {
            m_poolRoot = new GameObject("PoolManager");
        }
    }
    public GameObject CreateObj(GameObject obj, bool isAutoActive = true)
	{
		if (null == obj)
		{
			return null;
		}
		GameObjectPool pool = null;
		if (m_poolDict.TryGetValue(obj.name, out pool))
		{
            return pool.CreateObj(isAutoActive);
		}
		else
		{
            pool = new GameObjectPool(obj, m_poolRoot);
			m_poolDict.Add(obj.name, pool);
            return pool.CreateObj(isAutoActive);
		}
	}
    //加载Object
    public T LoadWithoutInstantiate<T>(string resName) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(resName))
        {
            return null;
        }
        GameObjectPool pool = null;
        if (m_poolDict.TryGetValue(resName, out pool))
        {
            return pool.SourceResource as T;
        }
        else
        {
            UnityEngine.Object obj = GameResManager.Singleton.LoadResource(resName);
            if (obj == null)
            {
                return null;
            }
            pool = new GameObjectPool(obj, m_poolRoot);
            m_poolDict.Add(resName, pool);
            return pool.SourceResource as T;
        }
    }
    //加载icon
    public T LoadIcon<T>(string resName) where T : UnityEngine.Object
    {
        string path = GameData.GetIconPath(resName);
        return LoadWithoutInstantiate<T>(path);
    }
    //加载sound
    public AudioClip LoadSound(string resName)
    {
        string path = GameData.GetSoundPath(resName);
        return LoadWithoutInstantiate<AudioClip>(path);
    }
    //加载GameObject
    public GameObject Load(string resName, bool isAutoActive = false)
    {
        if (string.IsNullOrEmpty(resName))
        {
            return null;
        }
        GameObjectPool pool = null;
        if (m_poolDict.TryGetValue(resName, out pool))
        {
            return pool.CreateObj(isAutoActive);
        }
        else
        {
            GameObject obj = GameData.Load(resName) as GameObject;
            if (obj == null)
            {
                return null;
            }
            pool = new GameObjectPool(obj, m_poolRoot);
            m_poolDict.Add(resName, pool);
            return pool.CreateObj(isAutoActive);
        }
    }

	public void ReleaseObj(GameObject obj)
	{
        if (obj == null) return;
		GameObjectPoolIdentifier poolIdentifier = obj.GetComponent<GameObjectPoolIdentifier>();
        if (null != poolIdentifier)
		{
            if (poolIdentifier.OwnPool == null)
            {
                Debug.LogError("OwnPool is null, obj name:" + obj.name);
            }
            else
            {
                poolIdentifier.OwnPool.ReleaseObj(obj);
            }
		}
		else
		{
			GameObject.Destroy(obj);
		}
	}

    public void ReleaseAll()
    {
        foreach (var item in m_poolDict)
        {
            item.Value.ReleaseAll();
        }
    }

    public void DestroyAll()
    {
        foreach (var item in m_poolDict)
        {
            item.Value.DestroyAll();
        }
        m_poolDict.Clear();
    }

	public void EraseAll()
	{
		m_poolDict.Clear();
	}

	private Dictionary<string, GameObjectPool> m_poolDict = new Dictionary<string, GameObjectPool>();

    //协程-异步加载资源
    //resName：资源名称、isInst：加载完的资源是否实例化、isAutoActive：是否显示
    public IEnumerator Coroutine_Load(string resName, GameResPackage.AsyncLoadObjectData data, bool isAutoActive = false, bool isInst = true)
    {
        if (!m_poolDict.ContainsKey(resName))
        {
            GameResPackage.AsyncLoadObjectData asyncData = new GameResPackage.AsyncLoadObjectData();
            IEnumerator e = GameResManager.Singleton.LoadResourceAsync(resName, asyncData);
            while (true)
            {
                e.MoveNext();
                if (asyncData.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
            if (asyncData.m_obj != null)
            {
                GameObjectPool pool = null;
                if (!m_poolDict.ContainsKey(resName))
                {
                    pool = new GameObjectPool(asyncData.m_obj, m_poolRoot);
                    m_poolDict.Add(resName, pool);
                }
                else
                {
                    pool = m_poolDict[resName];
                }
                if (isInst)
                {
                    data.m_obj = pool.CreateObj(isAutoActive);
                }
                else
                {
                    data.m_obj = pool.SourceResource;
                }
            }
        }
        else
        {
            GameObjectPool pool = m_poolDict[resName];
            if (isInst)
            {
                data.m_obj = pool.CreateObj(isAutoActive);
            }
            else
            {
                data.m_obj = pool.SourceResource;
            }
        }
        data.m_isFinish = true;
    }
    #region 加载资源并回调
    //加载资源并且回调
    //resName：资源名称、callback：回调函数、isInst：加载完的资源是否实例化、isAutoActive：是否显示
    public void LoadResourceAsyncCallback(string resName, GameResManager.LoadResourceCallback callback, bool isAutoActive = false, bool isInst = true)
    {
        if (m_poolDict.ContainsKey(resName))
        {
            if (isInst)
            {
                callback(m_poolDict[resName].CreateObj(isAutoActive));
            }
            else
            {
                callback(m_poolDict[resName].SourceResource);
            }
        }
        else
        {
            MainGame.Singleton.StartCoroutine(Coroutine_LoadCallback(resName, callback, isAutoActive, isInst));
        }
    }
    //协程-加载资源并且回调
    IEnumerator Coroutine_LoadCallback(string resName, GameResManager.LoadResourceCallback callback, bool isAutoActive = false, bool isInst = true)
    {
        UnityEngine.Object obj = null;
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        IEnumerator e = GameResManager.Singleton.LoadResourceAsync(resName, data);
        while (true)
        {
            e.MoveNext();
            if (data.m_isFinish)
            {
                break;
            }
            yield return e.Current;
        }
        if (data.m_obj != null)
        {
            GameObjectPool pool = null;
            if (m_poolDict.ContainsKey(resName))
            {
                pool = m_poolDict[resName];
            }
            else
            {
                pool = new GameObjectPool(data.m_obj, m_poolRoot);
                m_poolDict.Add(resName, pool);
            }

            if (isInst)
            {
                obj = pool.CreateObj(isAutoActive);
            }
            else
            {
                obj = pool.SourceResource;
            }
        }
        callback(obj);
    }
    #endregion
}