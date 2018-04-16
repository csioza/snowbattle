using System;
using System.Collections.Generic;
using UnityEngine;

public class UsedSkillEvent : EventBase
{
    //使用的技能
    public int MonsterId = 0;
    public int SkillId      = 0;
    //技能使用的次数
    public int UsedCount    = 0;
    /// //////////////////////////////////////////////////////////////////////////////////
    public UsedSkillEvent()
    {
        triggerID = EventManager.ENTriggerID.enMonsterUseSkillXCount;
    }
    public override void Tick()
    {
        base.Tick();
         SM.SceneRoom room = SM.RandomRoomLevel.Singleton.LookupRoom(RoomId);
         if (null != room)
         {
//             Actor tmpActor = room.GetMonsterByID(MonsterId);
             //int monsterUsedCount = tmpActor.Get
         }
    }
    public override void ParseJsonData(LitJson.JsonData data)
    {
        base.ParseJsonData(data);
        MonsterId = int.Parse(data[0].ToString());
        SkillId = int.Parse(data[1].ToString());
        UsedCount = int.Parse(data[2].ToString());
        for (int i = 0; i < data.Count; i++)
        {

        }
         Debug.Log("levelId=" + LevelId + " roomId=" + RoomId + " MonsterId=" + MonsterId);
    }
}