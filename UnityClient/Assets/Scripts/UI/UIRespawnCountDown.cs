using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIRespawnCountDown : UIWindow
{
	float m_curCountDownTime;
	float m_starTime;
	UILabel m_countDownLabel;
	static public UIRespawnCountDown GetInstance()
	{
		UIRespawnCountDown self = UIManager.Singleton.GetUIWithoutLoad<UIRespawnCountDown>();
		if (self != null)
		{
			return self;
		}
		self = UIManager.Singleton.LoadUI<UIRespawnCountDown>("UI/UIRespawnCountDown", UIManager.Anchor.Center);
		return self;
	}
	public override void OnInit()
	{
		base.OnInit();
		AddPropChangedNotify((int)MVCPropertyID.enMainPlayer, OnPropertyChanged);
		m_countDownLabel = FindChildComponent<UILabel>("CountDown");
	}
	void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
	{
		if (eventType == (int)Actor.ENPropertyChanged.enCountDown)
		{
			m_curCountDownTime = SM.RandomRoomLevel.Singleton.m_sceneHeroReliveTime;
			m_starTime = Time.time;
			m_countDownLabel.text = Math.Floor(m_curCountDownTime).ToString();
			ShowWindow();
		}
	}
	public override void AttachEvent()
	{
		base.AttachEvent();
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		m_countDownLabel.text = Math.Ceiling(m_curCountDownTime - (Time.time - m_starTime)).ToString();
		if (Math.Ceiling(m_curCountDownTime - (Time.time - m_starTime)) < 0)
		{
			HideWindow();
		}
	}
}
