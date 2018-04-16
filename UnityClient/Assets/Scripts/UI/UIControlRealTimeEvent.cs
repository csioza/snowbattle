using UnityEngine;
using System.Collections;

public class UIControlRealTimeEvent : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public bool m_isStart = false;
    public bool m_isDestroy = false;
    public bool m_isLoop = false;
    private float m_fStartTime = 0.0f;
    public float m_fInterval = 2.0f;
    public bool m_isPauseGame = false;
    public bool m_isPauseTime = false;
    void FixedUpdate()
    {
        if (m_isStart)
        {
            if (m_isLoop)
            {
                return;
            }
            float fDuration = Time.realtimeSinceStartup - m_fStartTime;
            if (fDuration >= m_fInterval)
            {
                if (m_isPauseGame)
                {
                    MainGame.Singleton.OnAppLogicPause(false, m_isPauseTime);
                }
                gameObject.SetActive(false);
                m_isStart = false;
                if (m_isDestroy)
                {
                    GameObject.Destroy(gameObject);
                }
            }
        }
    }
    public void Begin()
    {
        if (m_isPauseGame)
        {
            MainGame.Singleton.OnAppLogicPause(true, m_isPauseTime);
        }
        Animation anim = gameObject.GetComponent<Animation>();
        if (anim != null && anim.clip != null)
        {
            m_fInterval = anim.clip.length;
        }
        gameObject.SetActive(true);
        m_isStart = true;
        m_fStartTime = Time.realtimeSinceStartup;
        
    }
    public void Loop()
    {
        if (m_isPauseGame)
        {
            MainGame.Singleton.OnAppLogicPause(true, m_isPauseTime);
        }
        m_isLoop = true;
        m_isStart = true;
        gameObject.SetActive(true);
    }
    public void Stop()
    {
        m_isStart = false;
        gameObject.SetActive(false);
        if (m_isDestroy)
        {
            GameObject.Destroy(gameObject);
        }
        if (m_isPauseGame)
        {
            MainGame.Singleton.OnAppLogicPause(false, m_isPauseTime);
        }
    }
}
