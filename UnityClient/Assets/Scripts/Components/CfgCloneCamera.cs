using UnityEngine;
using System.Collections;

public class CfgCloneCamera : MonoBehaviour {
    public Camera m_cloneSource;
	// Use this for initialization
	void Start () {
	
	}
    void Awake()
    {
    }
	
	// Update is called once per frame
    void Update()
    {
        if (m_cloneSource != null)
        {
            int cullingMask = GetComponent<Camera>().cullingMask;
            CameraClearFlags clearFlags = GetComponent<Camera>().clearFlags;
            GetComponent<Camera>().CopyFrom(m_cloneSource);
            GetComponent<Camera>().cullingMask = cullingMask;
            GetComponent<Camera>().clearFlags = clearFlags;
        }
	}
}
