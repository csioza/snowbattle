using System;
using System.Collections.Generic;
using UnityEngine;

public class AndEvent : EventBase
{
    public enum ENCompareType
    {
        enTypeLower = 0,
        enTypeGreater,
    }
/*    public ENCompareType CompareType = ENCompareType.enTypeLower;*/
    public int CondParam = 0;
    public int MonsterId = 0;
    public int HPValue = 0;
    List<int> eventIdList = new List<int>();
    public AndEvent()
    {
        triggerID = EventManager.ENTriggerID.enAndEvent;
    }
    public override void Tick()
    {
        base.Tick();
        bool tmpIsEnabled = true;
        foreach (int eventId in eventIdList)
        {
            EventBase tmpEventBase = EventManager.Singleton.LookupEvent(eventId);
            if (null != tmpEventBase)
            {
                if (!tmpEventBase.IsEnabled)
                {
                    tmpIsEnabled = false;
                    break;
                }
                //tmpIsEnabled = tmpIsEnabled && tmpEventBase.IsEnabled;
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
        LitJson.JsonData addEventDataList = data[EventManager.sAddEventDataList];
        for (int i = 0; i < addEventDataList.Count; i++)
        {
            eventIdList[i] = int.Parse(addEventDataList[EventManager.sAddEventSerNo].ToString());
        }
        int eventId = int.Parse(data[0].ToString());
        eventIdList.Add(eventId);
        //Debug.Log("levelId=" + LevelId + " roomId=" + RoomId + " MonsterId=" + MonsterId);
    }
}
