using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTrapEvent : EventBase
{
    public struct EventTrapData
    {
        public int mTrapId;                     //机关ID
        public Trap.TrapState mTrapState;        //机关状态
    }  
    //机关数据List
    public List<EventTrapData> mEventTrapDataList = new List<EventTrapData>();
    public TriggerTrapEvent()
    {
        triggerID = EventManager.ENTriggerID.enTriggerTrap;
    }
    public override void Tick()
    {
        base.Tick();
        SM.SceneRoom room = SM.RandomRoomLevel.Singleton.LookupRoom(RoomId);
        
        if (null != room)
        {
            foreach (var item in mEventTrapDataList)
            {
                Actor tmpActor = room.GetTrapByID(item.mTrapId);
                if (null != tmpActor)
                {
                    Trap trap = tmpActor as Trap;
                    if (trap.mTrapState == item.mTrapState)
                    {
                        IsEnabled = true;
                    }

                }
            }
            
        }
    }
    public override bool Execute()
    {
        return base.Execute();
    }
    public override void ParseJsonData(LitJson.JsonData data)
    {
        base.ParseJsonData(data);
        LitJson.JsonData eventTrapList = data[EventManager.sSchemeTrapList];
        for (int i = 0; i < eventTrapList.Count; i++)
        {
            EventTrapData tmpTrapData = new EventTrapData();
            tmpTrapData.mTrapId = int.Parse(eventTrapList[i][EventManager.sTrapSerNo].ToString());
            tmpTrapData.mTrapState = (Trap.TrapState)int.Parse(eventTrapList[i][EventManager.sTrapType].ToString());
            mEventTrapDataList.Add(tmpTrapData);
        }
        //Debug.Log("levelId=" + LevelId + " roomId=" + RoomId + " TriggerTrapId=" + TriggerTrapId);
    }
}