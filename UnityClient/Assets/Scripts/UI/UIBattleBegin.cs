using System;
using System.Collections.Generic;
using UnityEngine;

public class UIBattleBegin : UIWindow
{
    UIControlRealTimeEvent mEventScript = null;
    static public UIBattleBegin GetInstance()
    {
        UIBattleBegin self = UIManager.Singleton.GetUIWithoutLoad<UIBattleBegin>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIBattleBegin>("UI/UIBattleBegin", UIManager.Anchor.Center);
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
        if (eventType == (int)SM.RandomRoomLevel.ENPropertyChanged.enBeginBattle)
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
