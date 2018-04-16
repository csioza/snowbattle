using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class RefreshMonsterResult : EResultBase
{
    ///////////////////////////////////////////////
    public enum ENSwapPosType
    {
        enLocationPosition,            // 0.默认位置，即怪物点在房间中摆放的位置
        triggerPositionOffset,            // 1.触发事件的角色的位置加上一个偏移量
        ObjectPositionOffset,            // 2.指定物体的位置加上一个偏移量
        PlayerPositionOffset,            // 3.主控角色的位置加上一个偏移量
        TargetActorOfTriggeringActor,            // 4.触发事件的角色的目标位置加上一个偏移量
                                    
        DeadActor,
        CastingActor,
        LastCreatedActor,
        TargetActorOfSkill,
        RebornActor,
        AttackedActor,
        TransportingActor,
        TriggerEventTrap,
        CustomBlackboardStr,
        AffectedGate,
        TriggeringGate,
        TriggeringTrap,
        TargetPositionOfSkillBeingCast,
        // 5.死亡的角色的位置加上一个偏移量
        // 6.施放技能的角色的位置加上一个偏移量
        // 7.上一个被创建出来的角色的位置加上一个偏移量
        // 8.技能的目标角色的位置加上一个偏移量
        // 9.复活的角色的位置加上一个偏移量
        // 10.受到伤害的角色的位置加上一个偏移量
        // 11.传送的角色的位置加上一个偏移量
        // 12.触发机关的角色的位置加上一个偏移量
        // 13.黑板中指定的角色（自定义字符串）的位置加上一个偏移量
        // 14.受影响的闸门的位置加上一个偏移量
        // 15.触发事件的闸门的位置加上一个偏移量
        // 16.触发事件的机关的位置加上一个偏移量
        // 17.技能释放的目标点的位置加上一个偏移量	

    }
    public enum ENTargetType
    {
        enMosnter,
        enScheme,
        enBox,
        enGate,
    }
    public struct RefreshMonsterData
    {
        public int mMonsterId;
        public ENSwapPosType mSwapPosType;
        public float mSwapPosX;
        public float mSwapPosZ;
        public int mSwapActorId;
        public string mBlackBoardActorStr;
        public ENTargetType mTargetType;
    }
    //int mConditionID = -1;
    List<RefreshMonsterData> mRefreshMosterDataList = new List<RefreshMonsterData>();
    public RefreshMonsterResult()
    {
        ResultTypeId = EResultManager.ENResultTypeID.enRefreshMonsterResult;
        IsTicked = true;
        IsEnabled = false;
    }
    public override void Tick()
    {
        base.Tick();

        if (IsEnabled)
        {

        }
    }
    public override bool Execute()
    {
//        if (IsEnabled)
        {
            //刷新怪物
            //SM.ActorRefresh.RefreshCondDict.Add(mConditionID, true);
            SM.SceneRoom room = SM.RandomRoomLevel.Singleton.LookupRoom(RoomGUID);
            foreach (var refreshData in mRefreshMosterDataList)
            {
                SM.ActorRefresh actorRefuresh = room.GetActorRefreshById(refreshData.mMonsterId);
                if (actorRefuresh == null)
                {
                    continue;
                }
                actorRefuresh.SpawnMe();

                Actor sourceActor = null;
                switch (refreshData.mSwapPosType)
                {
                    case ENSwapPosType.PlayerPositionOffset:
                        sourceActor = ActorManager.Singleton.MainActor;
                        break;
                    case ENSwapPosType.triggerPositionOffset:
                        sourceActor = BattleArena.Singleton.m_blackBoard.GetBlackBoardActor("CurActor");
                        break;
                    case ENSwapPosType.TargetActorOfTriggeringActor:
                        sourceActor = BattleArena.Singleton.m_blackBoard.GetBlackBoardActor("TargetActor");
                        break;
                    case ENSwapPosType.CustomBlackboardStr:
                        sourceActor = BattleArena.Singleton.m_blackBoard.GetBlackBoardActor(refreshData.mBlackBoardActorStr);
                        break;
                    case ENSwapPosType.ObjectPositionOffset:
                        sourceActor = room.GetMonsterByID(refreshData.mSwapActorId);
                        break;
                    default:
                        break;
                }
                //Vector3 basePos = sourceActor.RealPos;// GetSwapBasePos(refreshData.mSwapPosType, refreshData.mSwapActorId, refreshData.mBlackBoardActorStr);
                if (sourceActor != null)
                {
                    Vector3 movePos = Vector3.zero;
                    movePos = sourceActor.RealPos + new Vector3(refreshData.mSwapPosX, 0, refreshData.mSwapPosZ);
                    actorRefuresh.m_curMonsterObj.ForceMoveToPosition(movePos);
                }
            }

        }
        return base.Execute();
    }

    public Vector3 GetSwapBasePos(ENSwapPosType swapPosType, int swapActorId, string actorStr)
    {
        Actor sourceActor = null;
        Vector3 tmpVector3 = new Vector3();
        switch (swapPosType)
        {
            case ENSwapPosType.PlayerPositionOffset:
                sourceActor = ActorManager.Singleton.MainActor;
                break;
            case ENSwapPosType.triggerPositionOffset:
                sourceActor = BattleArena.Singleton.m_blackBoard.GetBlackBoardActor("CurActor");
                break;
            case ENSwapPosType.TargetActorOfTriggeringActor:
                sourceActor = BattleArena.Singleton.m_blackBoard.GetBlackBoardActor("TargetActor");
                break;
            case ENSwapPosType.CustomBlackboardStr:
                sourceActor = BattleArena.Singleton.m_blackBoard.GetBlackBoardActor(actorStr);
                break;
            case ENSwapPosType.ObjectPositionOffset:
                break;
            default:
                break;
        }
        if (sourceActor != null)
        {
            tmpVector3 = sourceActor.RealPos;
        }
        return tmpVector3;
    }


    public override void ParseJsonData(JsonData data)
    {
        base.ParseJsonData(data);
        LitJson.JsonData refreshMonsterDataList = data[EResultManager.sSwapMonsterDataList];
        for (int i = 0; i < refreshMonsterDataList.Count; i++ )
        {
            RefreshMonsterData tmpData = new RefreshMonsterData();
            tmpData.mMonsterId = int.Parse(refreshMonsterDataList[i][EResultManager.sSpawMonsterSerNo].ToString());
            tmpData.mSwapPosType = (ENSwapPosType)int.Parse(refreshMonsterDataList[i][EResultManager.sSpawnPosType].ToString());
            tmpData.mSwapPosX = float.Parse(refreshMonsterDataList[i][EResultManager.sSpawnMonsterOffsetx].ToString());
            tmpData.mSwapPosZ = float.Parse(refreshMonsterDataList[i][EResultManager.sSpawnMonsterOffsetz].ToString());
            tmpData.mSwapActorId = int.Parse(refreshMonsterDataList[i][EResultManager.sSpawnMonstertargetSerNo].ToString());
            tmpData.mBlackBoardActorStr = refreshMonsterDataList[i][EResultManager.sSpawnMonsterBlackBoardStr].ToString();
            mRefreshMosterDataList.Add(tmpData);
        }
    }
}