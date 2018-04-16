using UnityEngine;
using System.Collections;

public class TestBossAttack : MonoBehaviour {
    public Animation m_ani;
    public bool isSleep;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        m_ani.Play("attack-01");
        gameObject.transform.position = gameObject.transform.position;
        isSleep = gameObject.GetComponent<Rigidbody>().IsSleeping();
        gameObject.GetComponent<Rigidbody>().WakeUp();
	}
}
