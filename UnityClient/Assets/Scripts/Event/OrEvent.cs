using System;
using System.Collections.Generic;
using UnityEngine;

public class OrEvent : EventBase
{
    List<int> eventIdList = new List<int>();
    public OrEvent()
    {
        triggerID = EventManager.ENTriggerID.enAndEvent;
    }
    public override void Tick()
    {
        base.Tick();
        bool tmpIsEnabled = false;
        foreach (int eventId in eventIdList)
        {
            EventBase tmpEventBase = EventManager.Singleton.LookupEvent(eventId);
            if (null != tmpEventBase)
            {
                if (tmpEventBase.IsEnabled)
                {
                    tmpIsEnabled = true;
                    break;
                }
                //tmpIsEnabled = tmpIsEnabled || tmpEventBase.IsEnabled;
            }
        }
        IsEnabled = tmpIsEnabled;
    }
    public override bool Execute()
    {
        return base.Execute();
    }
    public override void ParseJsonData(LitJson.JsonData data)
    {
        base.ParseJsonData(data);
        LitJson.JsonData addEventDataList = data[EventManager.sOrEventDataList];
        for (int i = 0; i < addEventDataList.Count; i++)
        {
            eventIdList[i] = int.Parse(addEventDataList[EventManager.sOrEventSerNo].ToString());
        }
        int eventId = int.Parse(data[0].ToString());
        eventIdList.Add(eventId);
        //Debug.Log("levelId=" + LevelId + " roomId=" + RoomId + " MonsterId=" + MonsterId);
    }
}
