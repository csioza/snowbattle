using System;
using UnityEngine;
using System.Collections;


public class NpcReduceHP : IWidget
{
    bool m_isCrit = false;
    public void SetValue(int value, bool isCrit, float multiple, bool isHeal)
    {
        m_isCrit = isCrit;

        m_phyNormalObj.SetActive(false);
        m_phyCritObj.SetActive(false);
        m_magNormalObj.SetActive(false);
        m_magCritObj.SetActive(false);

        UILabel label = null;
        if (isHeal)
        {
            m_obj = m_magNormalObj;
            m_anim = m_magNormalAnim;
            label = m_magNormalLabel;
        }
        else
        {
            if (multiple <= 1.5f)
            {
                m_obj = m_phyNormalObj;
                m_anim = m_phyNormalAnim;
                label = m_phyNormalLabel;
            }
            else if (multiple > 2)
            {
                m_obj = m_magCritObj;
                m_anim = m_magCritAnim;
                label = m_magCritLabel;
            }
            else
            {
                m_obj = m_phyCritObj;
                m_anim = m_phyCritAnim;
                label = m_phyCritLabel;
            }
        }
        label.text = value.ToString();
        m_animTime = m_anim.clip.length;
        m_obj.SetActive(true);
	}
	public void AttachObj(GameObject obj)
	{
		m_obj.transform.position = obj.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
	}
	public GameObject m_obj;
    Animation m_anim = null;
    private GameObject m_phyNormalObj, m_phyCritObj, m_magNormalObj, m_magCritObj;
    UILabel m_phyNormalLabel, m_phyCritLabel, m_magNormalLabel, m_magCritLabel;
    Animation m_phyNormalAnim, m_phyCritAnim, m_magNormalAnim, m_magCritAnim;
	private float m_startTime = 0.0f;
    private float m_animTime = 0.0f;
	public override void Update()
	{
        if (null == m_obj)
        {
            return;
        }
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
        if (null == m_phyNormalObj)
		{
            GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
            IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetUIPath("UIPhysicalDamageNum"), data, true);
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
                //GameObject prefabObj = GameData.LoadPrefab<GameObject>("UI/UIPhysicalDamageNum");
                m_phyNormalObj = data.m_obj as GameObject;//GameObject.Instantiate(prefabObj) as GameObject;
                m_phyNormalObj.name = m_phyNormalObj.name + "_" + (++m_count).ToString();
                m_phyNormalObj.transform.parent = MainGame.Singleton.MainObject.transform;

                m_phyNormalLabel = m_phyNormalObj.GetComponentInChildren<UILabel>();
                m_phyNormalAnim = m_phyNormalObj.GetComponentInChildren<Animation>();
            }
        }
        if (null == m_phyCritObj)
        {
            GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
            IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetUIPath("UIPhysicalDamageNum_C"), data, true);
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
                //GameObject prefabObj = GameData.LoadPrefab<GameObject>("UI/UIPhysicalDamageNum_C");
                m_phyCritObj = data.m_obj as GameObject;//GameObject.Instantiate(prefabObj) as GameObject;
                m_phyCritObj.name = m_phyCritObj.name + "_" + (++m_count).ToString();
                m_phyCritObj.transform.parent = MainGame.Singleton.MainObject.transform;

                m_phyCritLabel = m_phyCritObj.GetComponentInChildren<UILabel>();
                m_phyCritAnim = m_phyCritObj.GetComponentInChildren<Animation>();
            }
        }
        if (null == m_magNormalObj)
        {
            GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
            IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetUIPath("UIHealNum"), data, true);
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
                //GameObject prefabObj = GameData.LoadPrefab<GameObject>("UI/UIHealNum");
                m_magNormalObj = data.m_obj as GameObject;//GameObject.Instantiate(prefabObj) as GameObject;
                m_magNormalObj.name = m_magNormalObj.name + "_" + (++m_count).ToString();
                m_magNormalObj.transform.parent = MainGame.Singleton.MainObject.transform;

                m_magNormalLabel = m_magNormalObj.GetComponentInChildren<UILabel>();
                m_magNormalAnim = m_magNormalObj.GetComponentInChildren<Animation>();
            }
        }
        if (null == m_magCritObj)
        {
            GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
            IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetUIPath("UIMagicDamageNum_C"), data, true);
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
                //GameObject prefabObj = GameData.LoadPrefab<GameObject>("UI/UIMagicDamageNum_C");
                m_magCritObj = data.m_obj as GameObject;//GameObject.Instantiate(prefabObj) as GameObject;
                m_magCritObj.name = m_magCritObj.name + "_" + (++m_count).ToString();
                m_magCritObj.transform.parent = MainGame.Singleton.MainObject.transform;

                m_magCritLabel = m_magCritObj.GetComponentInChildren<UILabel>();
                m_magCritAnim = m_magCritObj.GetComponentInChildren<Animation>();
            }
        }
	}

	public override void Release()
	{
        if (null != m_obj)
        {
            m_obj.SetActive(false);
        }
		IPoolWidget<NpcReduceHP>.ReleaseObj(this);
	}

	public override void Destroy()
	{
        if (null != m_phyNormalObj)
        {
            PoolManager.Singleton.ReleaseObj(m_phyNormalObj);
            //GameObject.Destroy(m_phyNormalObj);
            m_phyNormalObj = null;
        }
        if (null != m_phyCritObj)
        {
            PoolManager.Singleton.ReleaseObj(m_phyCritObj);
            //GameObject.Destroy(m_phyCritObj);
            m_phyCritObj = null;
        }
        if (null != m_magNormalObj)
        {
            PoolManager.Singleton.ReleaseObj(m_magNormalObj);
            //GameObject.Destroy(m_magNormalObj);
            m_magNormalObj = null;
        }
        if (null != m_magCritObj)
        {
            PoolManager.Singleton.ReleaseObj(m_magCritObj);
            //GameObject.Destroy(m_magCritObj);
            m_magCritObj = null;
        }
	}

    public override void Notify()
    {
        if (m_anim == null)
        {
            return;
        }
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
            m_anim.Blend(name, 0.5f);
        }
        float speed = 1;
        if (duration <0.5)
        {
            speed = 2f;
        }
		else if (duration <1&&duration >=0.5)
		{
			speed = 1 / duration;
		}
		if (m_isCrit)
		{//暴击
            speed /= 1.5f;
        }
        {//改变动画播放的速度
            m_anim["ui-reduceHP-00"].speed = speed;
            if (!string.IsNullOrEmpty(name))
            {
                m_anim[name].speed = speed;
            }
        }
    }
};