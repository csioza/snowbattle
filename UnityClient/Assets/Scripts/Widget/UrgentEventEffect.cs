using UnityEngine;
using System.Collections;

public class UrgentEventEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public bool m_isStart = false;
    private float m_fStartTime = 0.0f;
    public float m_fInterval = 0.0f;
    void FixedUpdate()
    {
        if (m_isStart == true)
        {
            float fDuration = Time.realtimeSinceStartup - m_fStartTime;
            if (fDuration >= m_fInterval)
            {
                gameObject.SetActive(false);
                m_isStart = false;
            }
        }
    }
    public void Begin()
    {
        gameObject.SetActive(true);
        m_isStart = true;
        m_fStartTime = Time.realtimeSinceStartup;
        WorldParamInfo worldParam = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enUrgentEventDurationParam);
        m_fInterval = worldParam.FloatTypeValue;
    }
}
