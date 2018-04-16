using UnityEngine;
using System.Collections;

public class TestActorMove : MonoBehaviour {
    public Rigidbody m_moveRigid;
    public bool m_moving = false;
    public bool isSleep;
    public bool m_manualWakeUp = false;
    float m_moveChangeTime;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time > m_moveChangeTime && m_moving)
        {
            m_moveChangeTime = Time.time + 5;
            Vector3 dir = new Vector3(UnityEngine.Random.Range(-3.0f,3.0f),0,UnityEngine.Random.Range(-3.0f,3.0f));
            dir = dir - m_moveRigid.transform.localPosition;
            dir.Normalize();
            m_moveRigid.velocity = dir * 3.0f;
        }
        if (!m_moving)
        {
            m_moveRigid.velocity = Vector3.zero;
        }
        isSleep = m_moveRigid.IsSleeping();
        if (m_manualWakeUp)
        {
            m_manualWakeUp = false;
            m_moveRigid.WakeUp();
        }
	}
}
