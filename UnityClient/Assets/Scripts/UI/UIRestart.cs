using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIRestart : UIWindow
{
    static public UIRestart GetInstance()
	{
        UIRestart self = UIManager.Singleton.GetUIWithoutLoad<UIRestart>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIRestart>("UI/UIRestart", UIManager.Anchor.TopRight);
		return self;
	}
	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enMainPlayer, OnPropertyChanged);
	}
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)Actor.ENPropertyChanged.enRestart)
        {
            bool isShow = (bool)eventObj;
           if (isShow)
           {
               MainGame.Singleton.OnAppLogicPause(true, false);
               ShowWindow();
           }
           else
           {
               MainGame.Singleton.OnAppLogicPause(false, false);
               HideWindow();
           }
        }
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("restart", OnButtonYes);
    }
    public void OnButtonYes(object sender, EventArgs e)
    {
        //MainGame.Singleton.Reload();
    }
}
