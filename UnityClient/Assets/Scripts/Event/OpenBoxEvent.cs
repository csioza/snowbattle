using System;
using System.Collections.Generic;
using UnityEngine;

public class OpenBoxEvent : EventBase
{
    Actor mCurActor = null;
    public List<int> BoxIdList = new List<int>();
    ///////////////////////////////////////////////////////

    public OpenBoxEvent()
    {
        triggerID = EventManager.ENTriggerID.enOpenBox;
    }
    public override void Tick()
    {
        base.Tick();
        SM.SceneRoom room = SM.RandomRoomLevel.Singleton.LookupRoom(RoomId);
        if (null != room)
        {
            foreach (var boxId in BoxIdList)
            {
                Actor treasureActor = room.GetTreatureByID(boxId);
                mCurActor = treasureActor;
                if (treasureActor == null)
                {
                    IsEnabled = true;
                    return;
                }
                NPC tracsure = treasureActor as NPC;
                if (tracsure.IsDead)
                {
                    IsEnabled = true;
                }
            }
            
        }
    }
    public override bool Execute()
    {
        BattleArena.Singleton.m_blackBoard.AddBlackBoardActor("DropItemActor", mCurActor, LevelBlackboard.BlackActorType.enBox);
        return base.Execute();
    }
    public override void ParseJsonData(LitJson.JsonData data)
    {
        base.ParseJsonData(data);
        LitJson.JsonData openBoxDataList = data[EventManager.sBoxSerNoList];
        for (int i = 0; i < openBoxDataList.Count; i++)
        {
            BoxIdList.Add(int.Parse(openBoxDataList[i].ToString()));
        }
//         
//         Debug.Log("levelId=" + LevelId + " roomId=" + RoomId + " TriggerTrapId=" + BoxId);
    }
}