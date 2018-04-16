using System;
using System.Collections.Generic;
using UnityEngine;

public class CloseToGateEvent : EventBase
{
    ///////////////////////////////////////////////////////
    public CloseToGateEvent()
    {
        triggerID = EventManager.ENTriggerID.enActorCloseToGate;
    }
    public override void Tick()
    {
        base.Tick();
    }
}