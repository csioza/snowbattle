using System;
using UnityEngine;

public class Skillbar : IWidget
{
    public float m_startTime;
    private static UISkillbar m_uiSkillBar;
    WorldParamInfo WorldParamList;
    private float m_SkillbarMaxTime;
    private float m_SkillbarFirstPartTime;
    public override void Update()
    {
    
    }
    public override void FixedUpdate() 
    {
        if (Time.realtimeSinceStartup - m_startTime > m_SkillbarMaxTime)
        {
            IsEnable = false;
        }
    }
    public override void Init() 
    {
        WorldParamList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enStartSkillbarTimeMax);
        m_SkillbarMaxTime = WorldParamList.FloatTypeValue;
        IsEnable = true;
        m_startTime = Time.realtimeSinceStartup;
        if (null == m_uiSkillBar)
        {
            m_uiSkillBar = UIManager.Singleton.LoadUI<UISkillbar>();
            m_uiSkillBar.OnInit();
        }
        else
        {
            m_uiSkillBar.ShowWindow();
            m_uiSkillBar.m_startTime = Time.realtimeSinceStartup;
        }
    }
    public override void Release()
    {     
        m_uiSkillBar.uiSkillBar1.value = 0.0f;
        m_uiSkillBar.uiSkillBar2.value = 0.0f;
        m_uiSkillBar.uiSkillBar3.value = 0.0f;
        m_uiSkillBar.obj1.SetActive(false);
        m_uiSkillBar.obj2.SetActive(false);
        m_uiSkillBar.obj3.SetActive(false);
        m_uiSkillBar.m_index = 1;
        m_uiSkillBar.HideWindow();
        IPoolWidget<Skillbar>.ReleaseObj(this);
    }
    public override void Destroy()
    {
        m_uiSkillBar.Destroy();
        //UIManager.Singleton.RemoveUI<UISkillbar>();
    }
}

public class UISkillbar : UIWindow
{
    public int m_index = 0;
    private float scaleTime = 0.0f;
    private float LastTime;
    public float m_startTime;   
    private float[] SkillbarTimeList = new float[3];
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;
    public UISlider uiSkillBar1;
    public UISlider uiSkillBar2;
    public UISlider uiSkillBar3;
    public override void OnInit()
    {
        Vector3 vpos = ActorManager.Singleton.MainActor.MainPos;
        vpos.y += 1.0f;
        vpos.x -= 0.35f;
        this.WindowRoot.transform.position += vpos;
        obj1 = FindChild("UISkillbar1");
        obj2 = FindChild("UISkillbar2");
        obj3 = FindChild("UISkillbar3");
        uiSkillBar1 = obj1.GetComponent<UISlider>();
        uiSkillBar2 = obj2.GetComponent<UISlider>();
        uiSkillBar3 = obj3.GetComponent<UISlider>();
        uiSkillBar1.value = 0.0f;
        uiSkillBar2.value = 0.0f;
        uiSkillBar3.value = 0.0f;
        float m_SkillbarTime1 = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillbarSkillFirstPartTime).FloatTypeValue;
        float m_SkillbarTime2 = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillbarSkillSecondPartTime).FloatTypeValue;
        float m_SkillbarTime3 = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillbarSkillThirdPartTime).FloatTypeValue;
        SkillbarTimeList[0] = m_SkillbarTime1;
        SkillbarTimeList[1] = m_SkillbarTime2 - m_SkillbarTime1;
        SkillbarTimeList[2] = m_SkillbarTime3 - m_SkillbarTime2;
        m_startTime = Time.realtimeSinceStartup;
        LastTime = SkillbarTimeList[0];
        m_index = 1;
    }

    public override void AttachEvent()
    {

    }
    public override void OnUpdate()
    {
        Transform cameraTrans = MainGame.Singleton.MainCamera.MainCamera.transform;
        Vector3 target = this.WindowRoot.transform.position + this.WindowRoot.transform.position - cameraTrans.position;
        this.WindowRoot.transform.LookAt(target, cameraTrans.rotation * Vector3.up);
        this.WindowRoot.transform.Translate(new Vector3(0.01f, 0.01f, 0.0f));
        Vector3 vpos = ActorManager.Singleton.MainActor.RealPos;
        vpos.y += 1.0f;
        vpos.x -= 0.35f;
        this.WindowRoot.transform.position = vpos;
        scaleTime = (Time.realtimeSinceStartup - m_startTime) / LastTime;
        if (scaleTime != 0 && scaleTime <= 1)
        {
            if (m_index == 1)
            {
                obj1.SetActive(true);
                uiSkillBar1.value = scaleTime;
            }
            else if (m_index == 2)
            {
                obj2.SetActive(true);
                uiSkillBar2.value = scaleTime;
            }
            else if (m_index == 3)
            {
                obj3.SetActive(true);
                uiSkillBar3.value = scaleTime;
            }
        }
        else
        {
            scaleTime = 0.0f;
            m_index = m_index + 1;
            if (m_index == 4)
            {
                obj1.SetActive(false);
                obj2.SetActive(false);
                obj3.SetActive(false);
                uiSkillBar1.value = 0;
                uiSkillBar2.value = 0;
                uiSkillBar3.value = 0;
                m_index = 1;
                scaleTime = 0.0f;
                return;
            }
            LastTime = SkillbarTimeList[m_index - 1];
            m_startTime = Time.realtimeSinceStartup;
        }
    }
}