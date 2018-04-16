using System;
using System.Collections.Generic;
using UnityEngine;

public class UIUrgent : UIWindow
{
    UIControlRealTimeEvent mEventScript = null;
    static public UIUrgent GetInstance()
    {
        UIUrgent self = UIManager.Singleton.GetUIWithoutLoad<UIUrgent>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIUrgent>("UI/UIUrgentEvent", UIManager.Anchor.Center);
        return self;
    }
    public override void OnInit()
    {
        base.OnInit();

        AddPropChangedNotify((int)MVCPropertyID.enBattlePropsManager, OnPropertyChanged);
        mEventScript = WindowRoot.GetComponent("UIControlRealTimeEvent") as UIControlRealTimeEvent;
        UIRoot root = GameObject.FindObjectOfType<UIRoot>();
        if (null != root)
        {
            float s = (float)root.activeHeight / Screen.height;
            int height = Mathf.CeilToInt(Screen.height * s);
            int width = Mathf.CeilToInt(Screen.width * s);
            UIWidget widget = WindowRoot.GetComponent<UIWidget>();
            UISprite sprite = widget as UISprite;
            sprite.width = width;
            sprite.height = height;
        }
        
    }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)BattleArena.ENPropertyChanged.enUrgentEvent)
        {
            bool isShow = (bool)eventObj;
            if (isShow)
            {
               mEventScript.Begin();
            }
            else
            {
                mEventScript.Stop();
            }
        }
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
    }
}
