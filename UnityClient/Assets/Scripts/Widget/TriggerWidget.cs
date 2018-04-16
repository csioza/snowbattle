//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Widget
//	created:	2013-6-17
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;


public class TriggerWidget : IWidget
{
	private GameObject m_obj;
	private float m_startTime = 0.0f;
	static private int m_count = 0;
	public override void Update()
	{
		Transform cameraTrans = MainGame.Singleton.MainCamera.MainCamera.transform;
		Vector3 target = m_obj.transform.position + m_obj.transform.position - cameraTrans.position;
		m_obj.transform.LookAt(target, cameraTrans.rotation * Vector3.up);
	}

	public override void FixedUpdate()
	{
		if (Time.realtimeSinceStartup - m_startTime > 0.5f)
		{
			IsEnable = false;
		}
	}

	public override void Init()
	{
		IsEnable = true;
		m_startTime = Time.realtimeSinceStartup;
		if (null == m_obj)
		{
            PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetUIPath("UIReduceHp"), LoadObjCallback);
            //GameObject prefabObj = GameData.LoadPrefab<GameObject>("UI/UIReduceHp");
            //m_obj = GameObject.Instantiate(prefabObj) as GameObject;
            //m_obj.name = m_obj.name + "_" + (++m_count).ToString();
            //m_obj.transform.parent = MainGame.Singleton.MainObject.transform;
		}
		else
		{
			m_obj.SetActive(true);
		}
	}

    void LoadObjCallback(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            m_obj = GameObject.Instantiate(obj) as GameObject;
            m_obj.name = m_obj.name + "_" + (++m_count).ToString();
            m_obj.transform.parent = MainGame.Singleton.MainObject.transform;
        }
    }

	public override void Release()
	{
		m_obj.SetActive(false);
		IPoolWidget<TriggerWidget>.ReleaseObj(this);
	}

	public override void Destroy()
	{
		if (null != m_obj)
		{
			GameObject.Destroy(m_obj);
			m_obj = null;
		}
	}
};