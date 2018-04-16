using System;
using System.Collections.Generic;
using UnityEngine;

public class DeadEvent : EventBase
{
    public enum ENDeadActorType
    {
        enNone = -1,
        enMainPlayer,
        enSpicalNpc,
        enOther,
    }
    public struct DeadMonsterData
    {
        public int actorId;
        public ENDeadActorType actorType;
    }
    //怪物ID列表
    public List<DeadMonsterData> mDeadMonsterList = new List<DeadMonsterData>();
    //死亡顺序列表
    public List<int> mDeadOrderList = new List<int>();
    //最后一个死亡的Actor
    public SM.BlackBoardActorData mCurActor = null;
    public string mEventActorStr = "";
    public DeadEvent()
    {
        triggerID   = EventManager.ENTriggerID.enMonsterDead;
    }
    public override void Tick()
    {
        base.Tick();
        SM.SceneRoom room = SM.RandomRoomLevel.Singleton.LookupRoom(RoomId);
        if (null != room)
        {
            bool tmpIsEnabled = true;
            foreach (var mosterData in mDeadMonsterList)
            {
//                 if (!room.IsMonsterDead(mosterData.actorId))
//                 {
//                     return;
//                 }
                Actor tmpActor = null;// room.GetMonsterByID(mosterData.actorId);
                switch (mosterData.actorType)
                {
                    case ENDeadActorType.enMainPlayer:
                        tmpActor = ActorManager.Singleton.MainActor;
                        break;
                    case ENDeadActorType.enSpicalNpc:
                        tmpActor = room.GetMonster(mosterData.actorId);
                        break;
                    default:
                        break;
                }

                bool tmpActorIsDead = false;
                if (tmpActor == null)
                {
                    SM.ActorRefresh actorRefuresh = room.GetActorRefreshById(mosterData.actorId);
                    if (actorRefuresh == null)
                    {
                        tmpActorIsDead = true;
                    }
                }
                else if (tmpActor.IsRealDead)
                {
                    tmpActorIsDead = true;
                }
                if (tmpActorIsDead)
                {
                    if (!mDeadOrderList.Contains(mosterData.actorId))
                    {
                        mDeadOrderList.Add(mosterData.actorId);
                    }
                }
                else
                {
                    tmpIsEnabled = false;
                    break;
                }
            }
            if (tmpIsEnabled)
            {
                SM.BlackBoardActorData tmpActorData = new SM.BlackBoardActorData();
                tmpActorData.roomID = RoomId;
                tmpActorData.actorID = mDeadOrderList[mDeadOrderList.Count - 1];
                tmpActorData.mBlackActorType = LevelBlackboard.BlackActorType.enNPC;
                mCurActor = tmpActorData;// mDeadOrderList[mDeadOrderList.Count - 1];
                IsEnabled = true;
            }
        }
    }
    public override bool Execute()
    {
//         if (mEventActorStr != "")
//         {
// 
//         }
//         //触发事件
//         BattleArena.Singleton.m_blackBoard.mDefEvent = this;
//         //触发事件的角色
//         BattleArena.Singleton.m_blackBoard.AddBlackBoardActor("CurActor", mCurActor);
//         //死亡的角色
        BattleArena.Singleton.m_blackBoard.AddBlackBoardActor("DeadActor", mCurActor);
        BattleArena.Singleton.m_blackBoard.AddBlackBoardActor("DropItemActor", mCurActor);
//         //杀人的角色
//         BattleArena.Singleton.m_blackBoard.AddBlackBoardActor("KillerActor", mCurActor);
//         if (mEventActorStr.Length != 0)
//         {//特殊字段添加的人物
//             BattleArena.Singleton.m_blackBoard.AddBlackBoardActor(mEventActorStr, mCurActor);
//         }
        //EventBlackboard.mDicEventActor.Add("default", mCurActor);
        
        base.Execute();
        return true;
    }
    public override void ParseJsonData(LitJson.JsonData data)
    {
        base.ParseJsonData(data);
        LitJson.JsonData deadNpcList = data[EventManager.sdeadNpcList];
        for (int i = 0; i < deadNpcList.Count; i++)
        {
            deadNpcList[i].ToString();
            DeadMonsterData tmpData = new DeadMonsterData();
            tmpData.actorId = int.Parse(deadNpcList[i][EventManager.stargetNpcSerNo].ToString());
            tmpData.actorType = (ENDeadActorType)int.Parse(deadNpcList[i][EventManager.snpcDeadNpcType].ToString());
            mDeadMonsterList.Add(tmpData);
        }
        //mEventActorStr = data[EventManager.sEventActorStr].ToString(); 
        //Debug.Log("levelId=" + LevelId + " roomId=" + RoomId + " MonsterId=" + MonsterId);
    }
}