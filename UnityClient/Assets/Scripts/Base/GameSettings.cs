using UnityEngine;

public class GameSettings : MonoBehaviour
{
	#region Singleton
	static public GameSettings Singleton { get; private set; }
    void Awake() 
    {
        if (null == Singleton)
        {
            Singleton = this;
        }
        else
        {
            Debug.LogWarning("GameSettings Recreated");
        }
    }
	void Start()
	{
		
	}
	#endregion
    [System.Serializable]
    public class CameraConfig
    {
        public float CameraFieldOfView = 35.0f;
        public Vector3 CameraRotate;
        public float CameraDistance;
        public float FogDensity = 0.06f;
        public bool IsOrtho = false;
    }
    public bool FingerOpen = false;
	public float CameraShackRange = 0.3f;
	public float CameraShackTime = 0.0f;
    public float CameraFieldOfView { get { return m_cameraConfigs[m_cameraCfgIndex].CameraFieldOfView; } }
        //=35.0f;
	public Vector3 CameraRotate{ get { return m_cameraConfigs[m_cameraCfgIndex].CameraRotate; } }
	public float CameraDistance{ get { return m_cameraConfigs[m_cameraCfgIndex].CameraDistance; } }
	public int NPCCount = 20;
    public string GameName = "GameA";
	public string ServerIP = "127.0.0.1";
	public string UserName = "test";
	public string PassWord = "1234";
	public string ResourceUrl = ""; // 游戏资源服务器主地址
    public bool NewPlayer = false;//新手标记
    public float MoveSpeed = 4.0f;
    public bool m_isSingle = true; // 是否单机
    public CameraConfig[] m_cameraConfigs;
    public int m_cameraCfgIndex=0;
    public bool m_refreshFov=false;
    public float m_floatGroundOffset = 0.2f;

    public bool m_playEffect    = true;    // 是否播放特效
    public bool m_playSound     = true;    // 是否播放音效
    public float m_bodyRotateSpeed = 0.5f;      //角色锁定目标身体旋转速度
    public float m_upperBodyRotateSpeed = 1.5f;   //上半身锁定目标旋转速度
    public float m_headRotateSpeed = 10f;
    public float m_standRotateSpeed = 3f;

    public float m_moveHeadRotateSpeed = 13f;   //点击无目标移动头部旋转速度
    public float m_moveUpperRotateSpeed = 8f;
    public float m_upperRotateDuration = 0.04f; //点击延时移动时长，用于旋转上半身
    

    public float m_cameraSmoothTimeForStart = 0.2f; // 摄像机开始时的到目标点的时间
    public float m_cameraSmoothTimeForStop = 0.5f;  // 摄像机停止时的到目标点的时间
    public float m_timeForCameraChangeTime = 0.3f; // 摄像机时间改变时所用的时间
    public float m_cameraStopDistance = 0.5f;       // actor距离目标点在m_cameraStopDistance范围内用m_cameraSmoothTimeForStop
    public float m_pickupTime   = 0.04f;
    public int m_dropGoldNum = 1;
    public float m_smoothMaxSpeed = 2.0f;
    public bool m_closeActorBlendAnim = false;
    public float m_moveAcceleration = 10f;
    public float m_stopMoveMaxRotationValS = 2.0f;      //旋转停止移动的最大值
    public float m_stopMoveMaxRotationValR = 1.0f;
    public float m_attackRotateTime = 0.1f;
    public float m_attackRotateSpeed = 10f;
    public float m_attackRotateMinAngle = 5f;//攻击旋转转身最小值
    public float m_fakeBlendWeightValue = 5f;
    public float m_fakeBlendFadeTime = 0.5f;
    public float m_roomPathfinderTileSize = 0.5f; //房间寻路块大小
    public bool m_isOpenFathfinderDebug = false;
    //测试改变shader
    public bool m_testShader = false;
    //是否隐藏切换角色和跟随角色
    public bool m_isHideChangeAndFollow = true;

    //animation文件转为bytes文件时，读取的时间频率
    public float m_animToBytesTimes = 0.033f;

    //自动攻击的间隔
    public float m_autoAttackInterval = 0.5f;
    //npc血条显示时间
    public float m_npcHPBloodDuration = 1f;
    //自动反击的时间
    public float m_autoCounterattack = 3;
    
    // 版本号
    public string m_version = "1.0";

    //同伴自己获得目标
    public bool m_partnerGetTargetFromSelf = false;

    //长连接tick间隔
    public float m_longConnectTickDuration = 0.5f;

    public void Update()
    {
        if (m_refreshFov)
        {
            m_cameraCfgIndex = (m_cameraCfgIndex + 1) % m_cameraConfigs.Length;
            if (m_cameraConfigs[m_cameraCfgIndex].IsOrtho)
            {
                MainGame.Singleton.MainCamera.MainCamera.GetComponent<Camera>().orthographic = true;
                MainGame.Singleton.MainCamera.MainCamera.GetComponent<Camera>().orthographicSize = 2.5f;
            }
            else
            {
                MainGame.Singleton.MainCamera.MainCamera.GetComponent<Camera>().orthographic = false;
                MainGame.Singleton.MainCamera.MainCamera.GetComponent<Camera>().fieldOfView = CameraFieldOfView;
                //MainGame.Singleton.MainCamera.MainCamera.camera.orthographic = false;
            }
            m_refreshFov = false;
        }
        if (m_testShader)
        {
            m_testShader = false;

            Actor target = ActorManager.Singleton.MainActor;
            AnimationShaderParamCallback callback = target.GetBodyParentObject().GetComponent<AnimationShaderParamCallback>();
            callback.ChangeShader("shader_beAttacked");
        }
    }
    // 以5度 为阶梯的 递减
    public void SetCameraRotateX()
    {
        
        //CameraRotate.x = CameraRotate.x - 5;

        //if (CameraRotate.x < 30)
        //{
        //    CameraRotate.x = 45;
        //}

    }


    // 
    public void SetCameraDistance()
    {

        //CameraDistance = CameraDistance + 0.5f;

        //if (CameraDistance > 17 )
        //{
        //    CameraDistance = 12f;
        //}
        m_cameraCfgIndex = (m_cameraCfgIndex + 1) % m_cameraConfigs.Length;
        if (m_cameraConfigs[m_cameraCfgIndex].IsOrtho)
        {
            MainGame.Singleton.MainCamera.MainCamera.GetComponent<Camera>().orthographic = true;
            MainGame.Singleton.MainCamera.MainCamera.GetComponent<Camera>().orthographicSize = 2.5f;
        }
        else
        {
            MainGame.Singleton.MainCamera.MainCamera.GetComponent<Camera>().orthographic = false;
            MainGame.Singleton.MainCamera.MainCamera.GetComponent<Camera>().fieldOfView = CameraFieldOfView;
        }
        RenderSettings.fogDensity = m_cameraConfigs[m_cameraCfgIndex].FogDensity;
    }

    public void SetCameraUp(bool isUp)
    {
        if (isUp)
        {
            m_cameraConfigs[m_cameraCfgIndex].CameraRotate.x = m_cameraConfigs[m_cameraCfgIndex].CameraRotate.x + 1;
        }
        else
        {
            m_cameraConfigs[m_cameraCfgIndex].CameraRotate.x = m_cameraConfigs[m_cameraCfgIndex].CameraRotate.x - 1;
        }
        //if (left)
        //{
        //    CameraRotate.y = CameraRotate.y + 10;
        //}
        //else
        //{
        //    CameraRotate.y = CameraRotate.y - 10;
        //}
       
    }
	public void SetCameraOverlook(bool isOver)
	{
		if(isOver)
		{
			m_cameraConfigs[m_cameraCfgIndex].CameraRotate.x = 35f;
		}
		else
		{
			m_cameraConfigs [m_cameraCfgIndex].CameraRotate.x =90f;
		}
	}
}
