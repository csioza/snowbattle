using UnityEngine;
using System;
using System.Collections;

#region disableWarning
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#endregion

public class Main : MonoBehaviour
{
	public string ResourcePath = "Resources/";
	void Awake()
    {
        //NGUITools.IsForceCallDestroy = true;
		Application.targetFrameRate = 30;
		QualitySettings.vSyncCount = 0;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //ResPath.ServerResourceURL = ResPath.WLocalURLPrefix + Application.dataPath + "/server/";
        //GameResManager.Singleton.CheckUpdate(Callback);
        //GameResMng.CreateResMng(this);
		//Debug.Log("SystemInfo.deviceUniqueIdentifier " + SystemInfo.deviceUniqueIdentifier);
	}
    //void Callback(bool isUpdate)
    //{
    //    Debug.LogWarning("isUpdate:" + isUpdate);
    //    if (isUpdate)
    //    {
    //        GameResManager.Singleton.DownloadUpdatePackage();
    //    }
    //}
    bool ShouldProcessTouch(int fingerIndex, Vector2 position)
    {
        return !UICamera.Raycast(position);
        //return true;
        //Ray ray = UICamera.currentCamera.ScreenPointToRay(position);
        //RaycastHit hitInfo;
        //bool touchUI = Physics.Raycast(ray, out hitInfo ,float.PositiveInfinity,LayerMask.NameToLayer("UIRoot"));
        //if (touchUI)
        //{
        //    Debug.LogWarning("Touch UI:" + hitInfo.collider.gameObject.name);
        //}
        
        //return !touchUI;
    }  
    void OnDestroy()
    {
        //NGUITools.IsForceCallDestroy = false;
        GameResManager.Singleton.CleanAll();
        Debug.Log("main OnDestroy");
    }

	void Start()
	{
#if Facebook
		CallFBInit();
#endif
		//GameResMng.CreateResMng(this);
        FingerGestures.GlobalTouchFilter = ShouldProcessTouch;
		if(MainGame.Singleton == null)
		{
			new MainGame();
		}
		MainGame.Singleton.OnStart(gameObject, this, ResourcePath);

		GameObject launchObj = GameObject.Find("Launch");
		if (null != launchObj)
		{
			launchObj.SetActive(false);
		}
	}


#region Facebook
#if fixme
	private bool isInit = false;

	private void CallFBInit()
	{
		FB.Init(OnInitComplete, OnHideUnity);
	}

	private void OnInitComplete()
	{
		Debug.Log("FB.Init completed");
		isInit = true;
		CallGetAuthResponse();
		CallFBPublishInstall();
	}

	private void OnHideUnity(bool isGameShown)
	{
		Debug.Log("Is game showing? " + isGameShown);
	}

	private void CallGetAuthResponse()
	{
		FB.GetAuthResponse(Callback);
	}

	void Callback(FBResult result)
	{
		if (string.IsNullOrEmpty(result.Error))
		{
			Debug.Log(result.Text);
		}
		else
		{
			Debug.LogError(result.Error);
		}
	}

	private void CallFBPublishInstall()
	{
		Debug.Log("CallFBPublishInstall");
		FB.PublishInstall(PublishComplete);
	}

	private void PublishComplete(FBResult result)
	{
		Debug.Log("publish response: " + result.Text);
	}
#endif
#endregion


	void Update()
	{
		MainGame.Singleton.Update();
	}

	void LateUpdate()
	{
		MainGame.Singleton.LateUpdate();
	}

	void FixedUpdate()
	{
		MainGame.Singleton.FixedUpdate();
	}

	void OnGUI()
	{
		MainGame.Singleton.OnGUI();
	}

	void OnApplicationQuit()
	{
		if (null == MainGame.Singleton)
		{
			return;
		}
        //NGUITools.IsForceCallDestroy = false;
		MainGame.Singleton.OnApplicationQuit();
	}

	void OnApplicationPause(bool pause)
	{
		if (null == MainGame.Singleton)
		{
			return;
		}
		MainGame.Singleton.OnApplicationPause(pause);
	}

	void OnTap(TapGesture gesture)
	{
		if (UICamera.touchCount > 0)
		{
			return;
		}
		/*
        if (UICamera.lastHit.collider != null)
        {
            return;
        }
        */
		
		//Debug.LogWarning("OnTap");
		if (!m_ignoreNextTap)
		{
			MainGame.Singleton.OnTap(gesture.Position);
		}
		m_ignoreNextTap = false;
	}
    void OnLongPress(LongPressGesture gesture) 
    {
        /* your code here */
		if (UICamera.touchCount > 0)
		{
			return;
		}
		/*
		if (UICamera.lastHit.collider != null)
		{
			return;
		}
		*/
		MainGame.Singleton.OnLongPress(gesture.Position);
    }
	private bool m_ignoreNextTap = false;
	void OnDoubleTap(TapGesture gesture)
	{
		m_ignoreNextTap = true;
		if (UICamera.touchCount > 0)
		{
			return;
        }
		/*
        if (UICamera.lastHit.collider != null)
        {
            return;
        }
        */
		//Debug.Log("OnDoubleTap");
		MainGame.Singleton.OnDoubleTap(gesture.Position);
	}
	void OnDrag(DragGesture gesture)
	{
		if (UICamera.touchCount > 0)
		{
			return;
		}
		//Debug.Log("OnDrag");
		switch (gesture.Phase)
		{
			case ContinuousGesturePhase.Updated:
				MainGame.Singleton.OnDragMove(gesture.StartPosition, gesture.DeltaMove, gesture.TotalMove);
				break;
			case ContinuousGesturePhase.Ended:
				MainGame.Singleton.OnDragMoveEnd(gesture.StartPosition, gesture.DeltaMove, gesture.TotalMove);
				break;
		}
	}
	void OnTwoFingerDrag(DragGesture gesture)
	{
		if (UICamera.touchCount > 0)
		{
			return;
		}
		//Debug.Log("OnTwoFingerDrag");
		MainGame.Singleton.OnTwoFingerDragMove(gesture.StartPosition, gesture.DeltaMove);
	}
	void OnPinch(PinchGesture gesture)
	{
		if (UICamera.touchCount > 0)
		{
			return;
		}
		//Debug.Log("OnPinch");
		MainGame.Singleton.OnPinchMove(gesture.Fingers[0].Position, gesture.Fingers[1].Position, gesture.Delta);
	}

	void OnDrawGraph(PointCloudGesture gesture)
	{
		if (UICamera.touchCount > 0)
		{
			return;
		}
		//Debug.Log("OnDrawGraph " + gesture.RecognizedTemplate.name);
		MainGame.Singleton.OnDrawGraph(gesture.RecognizedTemplate.name);
	}

    void OnPressUp(FingerEvent fingerEvent)
    {       
        FingerUpEvent upEvent = fingerEvent as FingerUpEvent;
        MainGame.Singleton.OnPressUp(fingerEvent.Finger.Index, fingerEvent.Position, upEvent.TimeHeldDown);
    }

    void OnPressDown(FingerEvent fingerEvent)
    {
        MainGame.Singleton.OnPressDown(fingerEvent.Finger.Index, fingerEvent.Position);
    }
    //手指滑动
    void OnSwipe(SwipeGesture getsture)
    {
        MainGame.Singleton.OnSwipe(getsture.Move, getsture.Direction);
    }
}
