using UnityEngine;
using System;

public class UIControlTimeEvent : MonoBehaviour
{
	public bool m_isStart = false;
    public bool m_isDestroy = false;
	private float m_fStartTime = 0.0f;
	public float m_fInterval = 2.0f;
	void FixedUpdate()
	{
		if (m_isStart == true) {
            float fDuration = Time.time - m_fStartTime;
			if (fDuration >= m_fInterval)
			{
				gameObject.SetActive (false);
                if (m_isDestroy)
                {
                    GameObject.Destroy(gameObject);
                }
				m_isStart = false;
			}
		}
	}
	public void Begin()
	{
		m_isStart = true;
		m_fStartTime = Time.time;
        Animation anim = gameObject.GetComponent<Animation>();
        if (anim != null && anim.clip != null)
        {
            m_fInterval = anim.clip.length;
        }
        gameObject.SetActive(true);
	}
}