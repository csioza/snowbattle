using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class SkillAllMonsterEvent : EventBase
{
    public SkillAllMonsterEvent()
    {
        triggerID = EventManager.ENTriggerID.enSkillAllMonster;
    }
    public override void Tick()
    {
        base.Tick();
        SM.SceneRoom room = SM.RandomRoomLevel.Singleton.LookupRoom(RoomId);
        if (null != room)
        {
            if (room.SkillAllMonster())
            {
                IsEnabled = true;
            }
        }
    }
    public override void ParseJsonData(LitJson.JsonData data)
    {
        base.ParseJsonData(data);
        for (int i = 0; i < data.Count; i++)
        {
            //TriggerTrapId = int.Parse(data[i].ToString());
        }
        //Debug.Log("levelId=" + LevelId + " roomId=" + RoomId + " TriggerTrapId=" + TriggerTrapId);
    }
}
