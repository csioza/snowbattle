using UnityEngine;
using System;
using System.Collections.Generic;

#region disableWarning
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#endregion

public class ShowFPS : MonoBehaviour
{
	private GameObject m_fpsObj;
    private GameObject m_light;
    private Light m_sceneLight;
	//private UILabel m_lable;
    private GameObject m_mainCamera;
	void Start()
	{
        //GameObject fpsRes = GameData.LoadPrefab<GameObject>("Base/FPS") as GameObject;
        m_mainCamera = GameObject.Find("MainGame").transform.Find("Main Camera").gameObject;
        //m_fpsObj = NGUITools.AddChild(GameObject.Find("BottomLeftAnchor"), fpsRes);
        //m_fpsObj.transform.localPosition = new Vector3(10, 600, 0);
        //m_fpsObj.transform.localScale = new Vector3(32, 32, 0);
		//m_lable = m_fpsObj.GetComponent<UILabel>();
	}

	string m_updateStr;
	float m_lastUpdateTime;
    float m_minFpsForSecond;
    List<float> m_minFps = new List<float>();

	float m_lastUpdateFpsTime;
	float m_curFps;
    float m_curFramesOneSecond;
    List<int> m_totalFrameCount = new List<int>();
    int m_loopSecond;
	void Update()
	{
        UpdateFpsWithOneSecond();
	}
    void UpdateFpsWithOneSecond()
    {
        float curTime = Time.realtimeSinceStartup;
        if (curTime > m_lastUpdateFpsTime+1.0f)
        {
            m_curFps = m_curFramesOneSecond / (curTime - m_lastUpdateFpsTime);
            m_totalFrameCount.Add((int)m_curFramesOneSecond);
                
            m_lastUpdateFpsTime = curTime;
            m_curFramesOneSecond = 0;
            if (m_totalFrameCount.Count>3)
            {
                m_totalFrameCount.RemoveAt(0);
            }
            int framecount = 0;
            for (int i = 0; i < m_totalFrameCount.Count;i++ )
            {
                framecount += m_totalFrameCount[i];
            }
            m_minFps.Add(m_curFps);
            CalcMinFpsForOneSecond();
            m_updateStr = m_curFps.ToString("f2") + "/" + m_minFpsForSecond.ToString("f2") + "(" + framecount + ")";
        }

        m_curFramesOneSecond += 1;
    }
    void CalcMinFpsForOneSecond()
    {
        if (m_minFps.Count > 3)
        {
            m_minFps.RemoveRange(0,1);
        }
        m_minFpsForSecond = 10000;
        for (int i = 0; i < m_minFps.Count; i++)
        {
            float f = m_minFps[i];
            if (f < m_minFpsForSecond)
            {
                m_minFpsForSecond = f;
            }
        }
    }
	void UpdateFpsWithOneFrame()
	{
        float currentTime = Time.realtimeSinceStartup;
        float fps = (1.0f / (currentTime - m_lastUpdateTime));
        m_updateStr = fps.ToString("f2") + "/" + m_minFpsForSecond.ToString("f2");
        m_lastUpdateTime = currentTime;

        m_minFps.Add(fps);
        CalcMinFps();
	}
    void CalcMinFps()
    {
        if (m_minFps.Count > 90)
        {
            m_minFps.RemoveRange(0,30);
        }
        m_minFpsForSecond = 10000;
        for (int i = 0; i < m_minFps.Count;i++ )
        {
            float f = m_minFps[i];
            if (f<m_minFpsForSecond)
            {
                m_minFpsForSecond = f;
            }
        }
    }
	string m_fixedStr;
	float m_lastFixedTime;
	void FixedUpdate()
	{
		float currentTime = Time.realtimeSinceStartup;
		m_fixedStr = (1.0f / (currentTime - m_lastFixedTime)).ToString("f2");
		m_lastFixedTime = currentTime;
	}
    string m_cameraRotateX;
    string m_cameraRotateY;
    string m_cameraRotateZ;
    string m_cameraView;
    string m_lightStr;
	string m_guiStr;
	float m_lastGUITime;
	void OnGUI()
	{
        m_light = GameObject.Find("Directional light");
		if (m_light != null)
		{
			m_sceneLight = m_light.GetComponent<Light>();
			m_lightStr = (m_sceneLight.intensity).ToString("f2");
		}
		else
		{
			//Debug.Log("light is null");
		}
		float currentTime = Time.realtimeSinceStartup;
        if (m_mainCamera != null)
        {
            m_cameraRotateX = (m_mainCamera.transform.localEulerAngles.x).ToString("f2");
            m_cameraRotateY = (m_mainCamera.transform.localEulerAngles.y).ToString("f2");
            m_cameraRotateZ = (m_mainCamera.transform.localEulerAngles.z).ToString("f2");
            m_cameraView = (m_mainCamera.GetComponent<Camera>().fieldOfView).ToString("f2");
        }
		m_guiStr = (1.0f / (currentTime - m_lastGUITime)).ToString("f2");
		m_lastGUITime = currentTime;
//      m_lable.text = string.Format("update:{0}	fixed:{1}   gui:{2}  \nX:{3}	Y:{4}   Z:{5}   View:{6}    Light{7}", m_updateStr, m_fixedStr, m_guiStr, m_cameraRotateX, m_cameraRotateY, m_cameraRotateZ, m_cameraView,m_lightStr);
        //if (m_lable != null)
        {
            //m_lable.text = string.Format("{0}", m_updateStr);
        }
        GUI.Label(new Rect(0, 0, 300, 30), m_updateStr);
	}
}