using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class EnterLevelEvent : EventBase
{
    ///////////////////////////////////////////////////
    public EnterLevelEvent()
    {
        triggerID = EventManager.ENTriggerID.enActorEnterLevel;
    }
    public override void Tick()
    {
        base.Tick();
    }

}