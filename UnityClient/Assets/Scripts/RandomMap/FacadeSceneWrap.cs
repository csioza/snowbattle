using UnityEngine;
using System.Collections;

public class FacadeSceneWrap : MonoBehaviour {

    public int m_TestSetEnv = 30;
    public Color FogColor = new Color(1, 1, 1);
    public Color AmbiColor = new Color(1, 1, 1);
    public Color LightColor = new Color(1, 1, 1);

    public float FogDesity = 0.5f;

    public Light SceneLight;
    public static FacadeSceneWrap Instance;
	// Use this for initialization
	void Start () {
	    if (Instance == null)
	    {
            Instance = this;
        }
        else
        {
            gameObject.SetActive(false);
        }
	}
    void Awake()
    {
        SetEnvironment();
    }
	void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    public void SetEnvironment()
    {
        Camera.main.backgroundColor = FogColor;
        RenderSettings.fogColor = FogColor;
        RenderSettings.ambientLight = AmbiColor;
        RenderSettings.fogEndDistance = 300.0f * (FogDesity * 2);
        RenderSettings.fogStartDistance = 300.0f * FogDesity;
        if (SceneLight!=null)
        {
            SceneLight.color = LightColor;
        }
    }
    void Update()
    {
        //if (m_TestSetEnv>0)
        //{
            //m_TestSetEnv--;
            SetEnvironment();
        //}
    }
}
