//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Widget
//	created:	2013-4-9
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;
using System.Collections;


public class ReduceHP : IWidget
{
    bool m_isCrit = false;
	public void SetValue(int value, /*bool isScale, */bool isCrit)
    {
        m_isCrit = isCrit;
        if (isCrit)
        {
            m_obj = m_critObj;
            m_normalObj.SetActive(false);
        }
        else
        {
            m_obj = m_normalObj;
            m_critObj.SetActive(false);
        }
		UILabel label = m_obj.GetComponent<UILabel>();
		if (0 == value)
		{
			label.text = "Miss";
			//label.color = Color.white;
		}
		else
		{
			label.text = "-" + value.ToString();
            //label.color = Color.red;
            m_animTime = m_obj.GetComponent<Animation>().clip.length;
            //Debug.LogWarning("reduce hp time is "+m_animTime.ToString());
            m_obj.SetActive(true);
		}
        //if (isScale)
        //{
        //    m_obj.transform.LocalScaleX(0.4f);
        //    m_obj.transform.LocalScaleY(0.4f);
        //}
        //else
        //{
        //    m_obj.transform.LocalScaleX(0.2f);
        //    m_obj.transform.LocalScaleY(0.2f);
        //}
	}
	public void AttachObj(GameObject obj)
	{
		m_obj.transform.position = obj.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
	}
	public GameObject m_obj;
    private GameObject m_normalObj, m_critObj;
	private float m_startTime = 0.0f;
    private float m_animTime = 0.0f;
	public override void Update()
	{
		Transform cameraTrans = MainGame.Singleton.MainCamera.MainCamera.transform;
		Vector3 target = m_obj.transform.position + m_obj.transform.position - cameraTrans.position;
		m_obj.transform.LookAt(target, cameraTrans.rotation * Vector3.up);
		//m_obj.transform.Translate(new Vector3(0.01f, 0.01f, 0.0f));
	}

	public override void FixedUpdate()
	{
        if (Time.realtimeSinceStartup - m_startTime > m_animTime)
		{
			IsEnable = false;
		}
	}

	static private int m_count = 0;
	public override void Init()
	{
		IsEnable = true;
        m_startTime = Time.realtimeSinceStartup;
        MainGame.Singleton.StartCoroutine(Coroutine_LoadObj());
    }
    IEnumerator Coroutine_LoadObj()
    {
        if (null == m_normalObj)
		{
            GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
            IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetUIPath("UIReduceHp"), data, true);
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
                //GameObject prefabObj = GameData.LoadPrefab<GameObject>("UI/UIReduceHp");
                m_normalObj = GameObject.Instantiate(data.m_obj) as GameObject;//GameObject.Instantiate(prefabObj) as GameObject;
                m_normalObj.name = m_normalObj.name + "_" + (++m_count).ToString();
                m_normalObj.transform.parent = MainGame.Singleton.MainObject.transform;
            }
		}
        if (null == m_critObj)
        {
            GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
            IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetUIPath("UIReduceHp_C"), data, true);
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
                //GameObject prefabObj = GameData.LoadPrefab<GameObject>("UI/UIReduceHp_C");
                m_critObj = GameObject.Instantiate(data.m_obj) as GameObject; //GameObject.Instantiate(prefabObj) as GameObject;
                m_critObj.name = m_critObj.name + "_" + (++m_count).ToString();
                m_critObj.transform.parent = MainGame.Singleton.MainObject.transform;
            }
        }
	}

	public override void Release()
	{
		m_obj.SetActive(false);
		IPoolWidget<ReduceHP>.ReleaseObj(this);
	}

	public override void Destroy()
	{
        if (null != m_normalObj)
        {
            PoolManager.Singleton.ReleaseObj(m_normalObj);
            //GameObject.Destroy(m_normalObj);
            m_normalObj = null;
        }
        if (null != m_critObj)
        {
            PoolManager.Singleton.ReleaseObj(m_critObj);
            //GameObject.Destroy(m_critObj);
            m_critObj = null;
        }
	}

    public override void Notify()
    {
        //base.Notify();
        float duration = Time.realtimeSinceStartup - m_startTime;
        string name = "";
        if (duration <= 0.5)
        {//A+C
            name = "ui-reduceHP-02";
        }
        else if (duration > 0.5 && duration < 1)
        {//A+B
            name = "ui-reduceHP-01";
        }
        else
        {//不融合
            ;
        }
        if (!string.IsNullOrEmpty(name))
        {//融合
            m_obj.GetComponent<Animation>().Blend(name, 0.5f);
        }
        float speed = 1;
        if (1 / duration > 1)
        {
            speed = 1 / duration;
        }
        if (m_isCrit)
        {//暴击
            speed /= 1.5f;
        }
        {//改变动画播放的速度
            m_obj.GetComponent<Animation>()["ui-reduceHP-00"].speed = speed;
            if (!string.IsNullOrEmpty(name))
            {
                m_obj.GetComponent<Animation>()[name].speed = speed;
            }
        }
        //Debug.LogWarning("main player hp, speed is "+speed.ToString() + ", is Crit = " + m_isCrit.ToString() + ", "+name);
    }
};