using System;
using System.Collections.Generic;
using UnityEngine;

public class HPEvent : EventBase
{
    //比较类型
    public enum ENCompareType
    {
        enNone = -1,
        enPercentLower = 0,
        enValueLower,
//         enValueGreater   ,
//         enPercentGreater,
    }
    public struct HPEventData
    {
        public int mNpcId;
        public ENActorType mNpcType;
        public ENCompareType mCompareType;
        public float mHPValue;
    }
    public List<HPEventData> mHPEventDataList = new List<HPEventData>();


    public ENCompareType CompareType = ENCompareType.enValueLower;
    public int CondParam = 0;
    public List<int> MonsterIdList = new  List<int>();
    public int HPValue = 0;
    public Actor mCurActor = null;
    public string mEventActorStr = "";
    public HPEvent()
    {
        triggerID   = EventManager.ENTriggerID.enMonsterHPX;
        CompareType = ENCompareType.enValueLower;
        CondParam   = 0;
    }
    public override void Tick()
    {
        base.Tick();
        SM.SceneRoom room = SM.RandomRoomLevel.Singleton.LookupRoom(RoomId);
        if (null != room)
        {
            bool tmpIsEnable = true;
            foreach (var hpData in mHPEventDataList)
            {
                mCurActor = GetEventActor(hpData.mNpcId, (ENActorType)hpData.mNpcType);
                float tmpCurActorHp = 0;
                float tmpCurMaxHp = 1000;
                if (mCurActor != null)
                {
                    tmpCurActorHp = mCurActor.HP;
                    tmpCurMaxHp = mCurActor.MaxHP;
                }
                else
                {
                    SM.ActorRefresh actorRefuresh = room.GetActorRefreshById(hpData.mNpcId);
                    if (actorRefuresh != null)
                    {
                        tmpIsEnable = false;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                switch (hpData.mCompareType)
                {
                    case ENCompareType.enValueLower:
                        tmpIsEnable = tmpCurActorHp < hpData.mHPValue;
                        break;
                    case ENCompareType.enPercentLower:
                        tmpIsEnable = (tmpCurActorHp / tmpCurMaxHp) * 100 < hpData.mHPValue;
                        break;
//                     case ENCompareType.enValueGreater:
//                         IsEnabled = tmpCurActorHp > hpData.mHPValue;
//                         break;

//                     case ENCompareType.enPercentGreater:
//                         IsEnabled = (tmpCurActorHp / mCurActor.MaxHP) > hpData.mHPValue;
//                         break;
                    default:
                        break;
                }
            }
            IsEnabled = tmpIsEnable;
            
        }
    }

    public Actor GetEventActor(int actorId, ENActorType actorType)
    {
        Actor tmpActor = null;
        switch (actorType)
        {
            case ENActorType.enMainPlayer:
                tmpActor = GetMainPlayer();
                break;
            case ENActorType.enSpicalNpc:
                tmpActor = GetSpecialNPC(actorId);
                break;
            default:
                break;
        }
        return tmpActor;
    }
    public Actor GetMainPlayer()
    {
        Actor sourceActor = ActorManager.Singleton.MainActor;
        if (sourceActor == null)
        {
            return null;
        }
        return sourceActor;
    }
    public Actor GetSpecialNPC(int actorId)
    {
        SM.SceneRoom curRoom = SM.RandomRoomLevel.Singleton.LookupRoom(RoomId);
        if (curRoom == null)
        {
            return null;
        }
        Actor sourceActor = curRoom.GetMonsterByID(actorId);
        if (sourceActor == null)
        {
            return null;
        }
        return sourceActor;
    }

    public override bool Execute()
    {
//         if (mEventActorStr != "")
//         {
//             BattleArena.Singleton.m_blackBoard.AddBlackBoardActor(mEventActorStr, mCurActor);
//         }
//         BattleArena.Singleton.m_blackBoard.AddBlackBoardActor("CurActor", mCurActor.CurrentTarget);
//         BattleArena.Singleton.m_blackBoard.AddBlackBoardActor("TargetActor", mCurActor.CurrentTarget);
        base.Execute();
        return true;
    }
    public override void ParseJsonData(LitJson.JsonData data)
    {
        base.ParseJsonData(data);
        LitJson.JsonData monsterDataList = data[EventManager.sHPMonsterList];
        for (int i = 0; i < monsterDataList.Count; i++)
        {
            HPEventData tmpData = new HPEventData();
            tmpData.mNpcId = int.Parse(monsterDataList[i][EventManager.sHPMonsterSerNo].ToString());
            tmpData.mNpcType = (ENActorType)int.Parse(monsterDataList[i][EventManager.sHPMonsterType].ToString());
            tmpData.mCompareType = (ENCompareType)int.Parse(monsterDataList[i][EventManager.sHPValType].ToString());
            tmpData.mHPValue = int.Parse(monsterDataList[i][EventManager.sHPValue].ToString());
            mHPEventDataList.Add(tmpData);
        }
        //mEventActorStr = data[EResultManager.sEventActorStr].ToString(); 
       
        

        //Debug.Log("levelId=" + LevelId + " roomId=" + RoomId + " MonsterId=" + MonsterId);
    }
}
