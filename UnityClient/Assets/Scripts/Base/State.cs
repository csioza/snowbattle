using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class State
{
	public CameraController MainCamera { get; protected set; }
    public WidgetManager Widgets { get; protected set; }
    public bool mIsPuase = false;
    //是否切换到下一个状态
    public bool SwitchNexted { get { return mIsSwitchNexted; } set { mIsSwitchNexted = value; } }
    bool mIsSwitchNexted = false;
	public virtual void OnEnter()
	{
        SwitchNexted = false;
	}
	public virtual void OnExit()
	{
		MainCamera = null;
		Widgets = null;
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
	}
    public bool IsGamePause()
    {
        return mIsPuase;
    }
    public virtual void OnLogicPause()
    {
        mIsPuase = true;
    }
    public virtual void OnLogicResume()
    {
        mIsPuase = false;
    }
	public virtual void OnUpdate()
	{
		
	}
	public virtual void OnLateUpdate()
	{

	}
	public virtual void OnFixedUpdate()
	{

	}
	public virtual void OnPause()
	{
#if UNITY_EDITOR
#else
		//ClientNet.Singleton.MainConnect.CloseConnection();
#endif
	}
	public virtual void OnResume()
	{
#if UNITY_EDITOR
#else
		//UIDoubleCheck1 uiChat = UIManager.Singleton.GetUI<UIDoubleCheck1>();
		//string message = GameTable.StringTableAsset.GetString(ENStringIndex.UIConnectClosed);
		//uiChat.SetShowWindowInfo(message, delegate() { MainGame.Singleton.TranslateTo(new StateLogin()); });
		//uiChat.ShowWindow();
#endif
	}

    public virtual void OnGUI()
    {

    }

	public virtual void OnTap(Vector2 fingerPos) { }
    public virtual void OnLongPress(Vector2 fingerPos) { }
	public virtual void OnDoubleTap(Vector2 fingerPos) { }
	public virtual void OnDragMove(Vector2 fingerPos, Vector2 delta, Vector2 TotalMove) { }
	public virtual void OnDragMoveEnd(Vector2 fingerPos, Vector2 delta, Vector2 TotalMove) { }
	public virtual void OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta) { }
	public virtual void OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta) { }
	public virtual void OnTwoFingerDragMoveEnd(Vector2 fingerPos) { }
	public virtual void OnDrawBegin(Vector2 fingerPos, Vector2 startPos) { }
	public virtual void OnDrawEnd(Vector2 fingerPos) { }
	public virtual void OnPressUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown) { }
	public virtual void OnPressDown(int fingerIndex, Vector2 fingerPos) { }
	public virtual void OnRotate(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta) { }
	public virtual void OnRotateEnd(Vector2 fingerPos1, Vector2 fingerPos2, float totalRotationAngle) { }
    public virtual void OnBackKey() { }
    public virtual void OnDrawGraph(string graphName) { }
    public virtual void OnSwipe(Vector2 total,FingerGestures.SwipeDirection direction) { }
}