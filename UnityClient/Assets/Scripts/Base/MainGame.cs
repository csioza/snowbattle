using UnityEngine;
using System;
using System.Collections;

public class MainGame
{
	static public MainGame Singleton { get; private set; }
	public string ResourcePath { get; private set; }
	public MainGame()
	{
		if (null == Singleton)
		{
			Singleton = this;
		}
		else
		{
			Debug.LogWarning("MainGame Recreated");
		}
	}
	public GameObject MainObject { get; private set; }
	public Main MainScript { get; private set; }
    public GameObject CameraMaskRedFlash { get; private set; }
	public CameraController MainCamera
	{
		get
		{
			return m_camera;
		}
	}
	public WidgetManager CurrentWidgets { get { return m_widgetManager; } }
	private WidgetManager m_widgetManager = new WidgetManager();
    private CameraController m_camera;

    // 面向Camera
    public static void FaceToCamera(GameObject obj)
    {
        if (obj != null)
        {
            Transform cameraTrans = MainGame.Singleton.MainCamera.MainCamera.transform;
            Vector3 target = obj.transform.position + obj.transform.position - cameraTrans.position;
            obj.transform.LookAt(target, cameraTrans.rotation * Vector3.up);
        }
    }
    //朝向camera的方向，但是y不变
    public static void FaceToCameraWithoutY(GameObject obj)
    {
        if (obj != null)
        {
            Vector3 cameraPos = MainGame.Singleton.MainCamera.MainCamera.transform.position;
            Vector3 selfPos = obj.transform.position;
            obj.transform.forward = new Vector3(cameraPos.x - selfPos.x, 0, cameraPos.z - selfPos.z);
        }
    }
    public GameObject mSelectBoxObj = null;
	public void OnStart(GameObject mainObj, Main main, string resPath)
	{
		MainObject = mainObj;
		MainScript = main;
		ResourcePath = resPath;

		//GameTable.LoadTable();

		new UIManager();
        UIManager.Singleton.EnableJoystickAutoMap();
        //场景相关
        new ScenePathfinder();
        new SM.RandomRoomLevel();
        new EventManager();
        new EResultManager();
        ///////////////////////////////////////////////////////////////
		//new ActorManager();
		m_camera = new CameraController();
		m_camera.Init(GameObject.Find("Main Camera"));
        mSelectBoxObj = GameObject.Find("SelectBox");
//         CameraMaskRedFlash = GameObject.Find("CameraMaskRedFlash");
//         CameraMaskRedFlash.AddComponent("UrgentEventEffect");
//         CameraMaskRedFlash.SetActive(false);
        //m_nextState = new StateInitApp();
        //m_nextState = new StateLogin();
        GameResManager.Singleton.SetEntryAs(main);
        m_nextState = new StateStartup();
	}

	// 每帧渲染时调用
	public void Update()
	{
		try
		{
            OnUpdate();
            UIManager.Singleton.OnUpdate();
		}
		catch (Exception e)
		{
			ExceptionProcessor(e);
		}
	}

	public void LateUpdate()
	{
		try
		{
			OnLateUpdate();
		}
		catch (Exception e)
		{
			LateExceptionProcessor(e);
		}
	}

	//固定时间调用，与图像渲染分离
	public void FixedUpdate()
	{
		try
		{
			OnFixedUpdate();
		}
		catch (Exception e)
		{
			FixedExceptionProcessor(e);
		}
	}

	public void OnGUI()
	{
		try
		{


            if (null != m_currentState)
            {
                m_currentState.OnGUI();
            }
		}
		catch (Exception e)
		{
			GUIExceptionProcessor(e);
		}
	}

	public void OnApplicationQuit()
	{

	}
    public bool IsAppLogicPause()
    {
        if (null != m_currentState)
        {
            return m_currentState.IsGamePause();
        }
        return false;
    }
    float m_timeScale = 0;
    public void OnAppLogicPause(bool pause, bool isPauseTime = true)
    {
        if (pause)
        {
            if (null != m_currentState)
            {
                m_currentState.OnLogicPause();
                if (isPauseTime)
                {
                    m_timeScale = Time.timeScale;
                    Time.timeScale = 0.0f;
                }
            }
        }
        else
        {
            if (null != m_currentState)
            {
                m_currentState.OnLogicResume();
                if (isPauseTime)
                {
                    Time.timeScale = m_timeScale;
                }
            }
        }
    }
	public void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			if (null != m_currentState)
			{
				m_currentState.OnPause();
			}
		}
		else
		{
			if (null != m_currentState)
			{
				m_currentState.OnResume();
			}
		}
	}

	protected virtual void ExceptionProcessor(Exception e)
	{
		Debug.LogError("[Application Update] " + e.Message + " " + e.StackTrace);
        DebugLog.Singleton.OnShowLog("[Application Update] " + e.Message + " " + e.StackTrace);
	}
	protected virtual void LateExceptionProcessor(Exception e)
	{
		Debug.LogError("[Application LateUpdate] " + e.Message + " " + e.StackTrace);
        DebugLog.Singleton.OnShowLog("[Application LateUpdate] " + e.Message + " " + e.StackTrace);
	}
	protected virtual void FixedExceptionProcessor(Exception e)
	{
		Debug.LogError("[Application FixedUpdate] " + e.Message + " " + e.StackTrace);
        DebugLog.Singleton.OnShowLog("[Application FixedUpdate] " + e.Message + " " + e.StackTrace);
	}
	protected virtual void GUIExceptionProcessor(Exception e)
	{
		Debug.LogError("[Application OnGUI] " + e.Message + " " + e.StackTrace);
        DebugLog.Singleton.OnShowLog("[Application OnGUI] " + e.Message + " " + e.StackTrace);
	}

	protected void OnUpdate()
	{
		m_widgetManager.Update();
		if (null != m_currentState)
		{
			m_currentState.OnUpdate();
		}
	}

	protected void OnLateUpdate()
	{
		if (null != m_currentState)
		{
			m_currentState.OnLateUpdate();
		}
	}

	protected void OnFixedUpdate()
	{
        if (m_currentState == null)
        {
            if (GameResManager.Singleton.m_isPreLoadFinish)
            {//第一个state要等到PreLoad完成
                User.Singleton.OnInit();
                m_nextState = new StateStartup();
            }
        }
		if (null != m_currentState)
		{
			m_currentState.OnFixedUpdate();
			m_widgetManager.FixedUpdate();
		}

		if (null != m_nextState)
		{
			if (null != m_currentState)
			{
				m_currentState.OnExit();
				m_widgetManager.Clear();
                AnyObjectPoolMgr.Singleton.DestroyAll();
				Resources.UnloadUnusedAssets();
                for (int i = 0; i < 100;i++ )
                {
                    GC.Collect();
                }
                //GC.WaitForPendingFinalizers();
			}
			m_currentState = m_nextState;
			m_nextState = null;
			m_currentState.OnEnter();
		}
        GameResManager.Singleton.FixedUpdate();
	}

	private State m_currentState = null;
	public State CurrentState { get { return m_currentState; } }
	private State m_nextState = null;

	public void TranslateTo(State nextState)
	{
		m_nextState = nextState;
	}

	public void OnTap(Vector2 fingerPos)
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnTap(fingerPos);
	}
	public void OnLongPress(Vector2 fingerPos)
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnLongPress(fingerPos);
	}
	public void OnDoubleTap(Vector2 fingerPos)
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnDoubleTap(fingerPos);
	}
	public void OnDragMove(Vector2 fingerPos, Vector2 delta, Vector2 TotalMove)
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnDragMove(fingerPos, delta, TotalMove);
	}

	public void OnDragMoveEnd(Vector2 fingerPos, Vector2 delta, Vector2 TotalMove)
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnDragMoveEnd(fingerPos, delta, TotalMove);
	}
	public void OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta)
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnPinchMove(fingerPos1, fingerPos2, delta);
	}
	public void OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta)
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnTwoFingerDragMove(fingerPos, delta);
	}
	public void OnTwoFingerDragMoveEnd(Vector2 fingerPos)
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnTwoFingerDragMoveEnd(fingerPos);
	}
	public void OnDrawBegin(Vector2 fingerPos, Vector2 startPos)
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnDrawBegin(fingerPos, startPos);
	}
	public void OnDrawEnd(Vector2 fingerPos)
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnDrawEnd(fingerPos);
	}
	public void OnPressUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown)
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnPressUp(fingerIndex, fingerPos, timeHeldDown);
	}
	public void OnPressDown(int fingerIndex, Vector2 fingerPos)
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnPressDown(fingerIndex, fingerPos);
	}
	public void OnRotate(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta)
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnRotate(fingerPos1, fingerPos2, rotationAngleDelta);
	}
	public void OnRotateEnd(Vector2 fingerPos1, Vector2 fingerPos2, float totalRotationAngle)
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnRotateEnd(fingerPos1, fingerPos2, totalRotationAngle);
	}
	public void OnBackKey()
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnBackKey();
	}
	public void OnDrawGraph(string graphName)
	{
		if (null == m_currentState)
		{
			return;
		}
		m_currentState.OnDrawGraph(graphName);
	}
    public void OnSwipe(Vector2 total,FingerGestures.SwipeDirection direction)
    {
        if (null == m_currentState)
        {
            return;
        }
        m_currentState.OnSwipe(total,direction);
    }

	public Coroutine StartCoroutine(IEnumerator routine)
	{
		return MainScript.StartCoroutine(routine);
	}
	public Coroutine StartCoroutine(string methodName)
	{
		return MainScript.StartCoroutine(methodName);
	}
	public Coroutine StartCoroutine(string methodName, object value)
	{
		return MainScript.StartCoroutine(methodName, value);
	}
	public Coroutine StartCoroutine_Auto(IEnumerator routine)
	{
		return MainScript.StartCoroutine(routine);
	}
	public void StopAllCoroutines()
	{
		MainScript.StopAllCoroutines();
	}
	public void StopCoroutine(string methodName)
	{
		MainScript.StopCoroutine(methodName);
	}
}
