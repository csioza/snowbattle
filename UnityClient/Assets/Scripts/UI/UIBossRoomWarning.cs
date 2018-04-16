using System;
using System.Collections.Generic;
using UnityEngine;

public class UIBossRoomWarning : UIWindow
{
    UIControlRealTimeEvent mEventScript = null;
    static public UIBossRoomWarning GetInstance()
    {
        UIBossRoomWarning self = UIManager.Singleton.GetUIWithoutLoad<UIBossRoomWarning>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIBossRoomWarning>("UI/UIBossRoomWarning", UIManager.Anchor.Center);
        return self;
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enSceneManager, OnPropertyChanged);
        mEventScript = WindowRoot.GetComponent("UIControlRealTimeEvent") as UIControlRealTimeEvent;
    }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)SM.RandomRoomLevel.ENPropertyChanged.enUIBossRoomWarning)
        {
            if (ActorManager.Singleton.MainActor.ActionControl.IsActionRunning(ActorAction.ENType.enMoveAction))
            {
                ActorManager.Singleton.MainActor.MainAnim.Stop();
                ActorManager.Singleton.MainActor.ActionControl.RemoveAction(ActorAction.ENType.enMoveAction);
            }
            mEventScript.Begin();
        }
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
    }
}
