using System;
using UnityEngine;


public class EndTimeDisplay : IWidget
{
    private GameObject m_obj;
    private float m_startTime = 0.0f;
    public int EndTime { get; set; }
    WorldParamInfo WorldParamList;
    public void SetValue( int value)
    {
        WorldParamList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillbarSkillEND);
        UILabel label = m_obj.GetComponent<UILabel>();
        if (0 == EndTime)
        {
            label.text = WorldParamList.StringTypeValue;
            label.color = Color.white;
            IsEnable = false;
        }
        else
        {
            label.text = EndTime.ToString();
            label.color = Color.blue;
        }
        m_obj.transform.LocalScaleX(0.6f);
        m_obj.transform.LocalScaleY(0.6f);      
    }
    public void AttachObj(GameObject obj)
    {
        if (obj != null)
        {
            m_obj.transform.position = obj.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
        }
    }
    Actor actor = ActorManager.Singleton.MainActor;
    public override void Update()
    {
        if (Time.realtimeSinceStartup - m_startTime >= 1.0f)
        {
            EndTime = EndTime - 1;
            SetValue(EndTime);
            AttachObj(actor.CenterPart);
            m_startTime = Time.realtimeSinceStartup;
        }
        Transform cameraTrans = MainGame.Singleton.MainCamera.MainCamera.transform;
        Vector3 target = m_obj.transform.position + m_obj.transform.position - cameraTrans.position;
        m_obj.transform.LookAt(target, cameraTrans.rotation * Vector3.up);
        m_obj.transform.Translate(new Vector3(0.01f, 0.01f, 0.0f));      
    }
    private float m_countdowntime = 0.0f;
    public override void FixedUpdate()
    {
        if (Time.realtimeSinceStartup - m_startTime > m_countdowntime)
        {
            IsEnable = false;
        }
    }
    static private int m_count = 0;
    public override void Init()
    {
        IsEnable = true;
        m_startTime = Time.realtimeSinceStartup;
        WorldParamList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillbarSkillCountDown);
        m_countdowntime = WorldParamList.FloatTypeValue;
        
        if (null == m_obj)
        {
            PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetUIPath("EndTimeDisplay"), LoadObjCallback);
            //GameObject prefabObj = GameData.LoadPrefab<GameObject>("UI/EndTimeDisplay");
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
        IPoolWidget<EndTimeDisplay>.ReleaseObj(this);
    }

    public override void Destroy()
    {
        if (null != m_obj)
        {
            GameObject.Destroy(m_obj);
            m_obj = null;
        }
    }
}
