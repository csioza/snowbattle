using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ED_SimActorManager
{
    static ED_SimActorManager _Instance;
    public ED_SimActor MainActor
    {
        get
        {
            if (m_allActors.Count>0)
            {
                return m_allActors[0];
            }
            return null;
        }
    }
    public bool m_paused = false;
    public List<ED_SimActor> m_allActors = new List<ED_SimActor>();
    public static ED_SimActorManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new ED_SimActorManager();
                EditorApplication.update += StaticTick;
            }
            return _Instance;
        }
    }
    public static void StaticTick()
    {
        if (!Application.isPlaying)
        {
            ED_SimActorManager.Instance.TickAll();
        }
    }
    float m_lastUpdateTime = Time.realtimeSinceStartup;
    public void TickAll()
    {
        float deltaTime = Time.realtimeSinceStartup - m_lastUpdateTime;
        if (deltaTime > 0.1f)
        {
            deltaTime = 0.0f;
        }
        m_lastUpdateTime = Time.realtimeSinceStartup;
        deltaTime *= Time.timeScale;
        if (m_paused)
        {
            deltaTime = 0.0f;
        }
        Next(deltaTime);
    }
    public void Next(float deltaTime)
    {
        for (int i = 0; i < m_allActors.Count; i++)
        {
            m_allActors[i].Tick(deltaTime);
        }
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.Repaint();
        }
    }
    public void AddActor(GameObject obj)
    {
        ED_SimActor a = new ED_SimActor();
        a._object = obj;
        m_allActors.Add(a);
    }
    public void Clear()
    {
        m_allActors.Clear();
    }
}

public class ED_SimActor {

    public GameObject _object;
    public bool m_enablePlay = false;
    public string m_aniName;
    float m_lastAnimationTime;
    public void PlaySkill(string name)
    {
        m_aniName = name;
        m_enablePlay = true;
        Transform t = _object.transform.GetChild(0);
        if (t != null)
        {
            GameObject modelObj = t.gameObject;
            Animation modelAnim = modelObj.GetComponent<Animation>();
            modelAnim.Stop();
            modelAnim.Play(m_aniName);
            m_lastAnimationTime = 0.0f;
        }
    }
    public void Tick(float deltaTime)
    {
        if (_object != null && m_enablePlay)
        {
            string n = m_aniName;
            Transform t = _object.transform.GetChild(0);
            if (t != null)
            {
                GameObject modelObj = t.gameObject;
                Animation modelAnim = modelObj.GetComponent<Animation>();
                AnimationState st = modelAnim[n];
                if (st != null)
                {
                    st.time += deltaTime;
                    if (st.time > st.length)
                    {
                        st.time = 0;
                        m_enablePlay = false;
                    }
                    float backupLastAnimationTime = m_lastAnimationTime;
                    m_lastAnimationTime = st.time;
                    UpdateAnimationEvent(AnimationEventPanel.ObjEffect, st.clip, backupLastAnimationTime, st.time);
                }
                //t.gameObject.SampleAnimation(st.clip, st.time);
                modelAnim.Sample();
            }
        }
        AnimationEventPanel.ObjEffect.GetComponent<EDSkillEventTrigger>().Tick(deltaTime);
    }

    void UpdateAnimationEvent(GameObject obj, AnimationClip clip, float lastTime, float curTime)
    {
        AnimationEvent[] events = AnimationUtility.GetAnimationEvents(clip);
        for (int i = 0; i < events.Length; i++)
        {
            AnimationEvent ev = events[i];
            if (ev.time >= lastTime && ev.time < curTime)
            {
                obj.SendMessage(ev.functionName, ev, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
