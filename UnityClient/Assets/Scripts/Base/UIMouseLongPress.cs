//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------
using UnityEngine;
using System;

/// <summary>
/// Sends a message to the remote object when something happens.
/// </summary>

//[AddComponentMenu("NGUI/Interaction/Button Message")]
public class UIMouseLongPress : MonoBehaviour
{
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnLongPress,
		OnRelease,
		OnDoubleClick,
	}

	public GameObject target;
	public string functionName;
	public Trigger trigger = Trigger.OnClick;
	public bool includeChildren = false;
	bool mStarted = false;
	bool mHighlighted = false;

	void Start ()
	{
		mStarted = true;
	}

	void OnEnable ()
	{
		if (mStarted && mHighlighted)
			OnHover (UICamera.IsHighlighted (gameObject));
	}

	void OnHover (bool isOver)
	{
		if (enabled) {
			if (((isOver && trigger == Trigger.OnMouseOver) ||
				(!isOver && trigger == Trigger.OnMouseOut)))
				Send ();
			mHighlighted = isOver;
		}
	}
	
	bool isTouching;
	float touchTime;
	bool isLongPress;

	void OnPress (bool isPressed)
	{
		if (enabled) {
			if (isPressed) {
				isTouching = true;
				touchTime = Time.time;
			} else {
				isTouching = false;
			}
			if (((isPressed && trigger == Trigger.OnPress) ||
				(!isPressed && trigger == Trigger.OnRelease)))
				Send ();
		}
	}
	
	void OnClick ()
	{
		if (isLongPress) {
			isLongPress = false;
			return;
		}
		if (enabled && trigger == Trigger.OnClick) {
			Send ();
		}
	}

	void OnDoubleClick ()
	{
		if (enabled && trigger == Trigger.OnDoubleClick) {
			Send ();
		}
	}
	
	void Send ()
	{
		if (string.IsNullOrEmpty (functionName))
			return;
		if (target == null)
			target = gameObject;

		if (includeChildren) {
			Transform[] transforms = target.GetComponentsInChildren<Transform> ();

			for (int i = 0, imax = transforms.Length; i < imax; ++i) {
				Transform t = transforms [i];
				t.gameObject.SendMessage (functionName, gameObject, SendMessageOptions.DontRequireReceiver);
			}
		} else {
			target.SendMessage (functionName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	private const float TIME_BORDER_LONGPRESS = 0.5f;
	private const string FUN_LONGPRESS = "OnLongPress";
	
	void Update ()
	{
		if (!isLongPress && isTouching && Time.time - touchTime > TIME_BORDER_LONGPRESS && trigger == Trigger.OnLongPress) {
			isLongPress = true;
			
//			Send ();
			//现有架构下，不在采用Unity官方通用做法来相应事件，而是采用回调方式 Author: by yulei.
			InvokeRepeating (FUN_LONGPRESS, 0.5f, 0.3f);
		} 
		
		if (IsInvoking (FUN_LONGPRESS) && !isTouching) {
			CancelInvoke (FUN_LONGPRESS);
		}
	}
	
	public EventHandler<EventArgs> MouseEvent { get; set; }
	
	void OnLongPress ()
	{
		if (null != MouseEvent) 
        {
            try
            {
                MouseEvent(gameObject, null);
            }
            catch (Exception e)
            {
                DebugLog.Singleton.OnShowLog("[UIMouseLongPress OnLongPress ] " + e.Message + " " + e.StackTrace.ToString());
            }
			
		}
	}
	
}