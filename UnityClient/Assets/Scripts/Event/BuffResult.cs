using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffResult : EResultBase
{
    ///////////////////////////////////////////////
    public enum AddBuffType
    {
        specifiedNPC = 0,           //0.指定的NPC
        Mainplayer,                 //1.主控角色
        TriggeringActor,            //2.触发该事件的角色
        CastingActor,               //3.施放技能的角色
        LastCreatedActor,           //4.上一个被创建出来的角色
        TargetActorOfSkill,         //5.技能的目标角色
        TargetActorOfTriggeringActor,//6.触发事件的角色的目标
        RebornActor,                //7.复活的角色
        AttackedActor,              //8.受到伤害的角色
        TransportingActor,          //9.传送的角色
        TriggerTrapActor,           //10.触发机关的角色
        CustomBlackboardStr,        //11.黑板中指定的角色（自定义字符串)
    }
    public enum AddBuffACTIONTYPE
    {
        addSpecifiedBuff = 0,//0.添加特定buff
        removeSpecifiedBuff,//1.移除特定buff
        removeAllBuff,      //2.移除所有buff
        removeAllNegativeBuff,//3.移除所有负面buff
        removeAllPositiveBuff,//4.移除所有正面buff
    }
    public struct BuffActorData 
    {
        public AddBuffType mActorType;
        public AddBuffACTIONTYPE mBuffOptType;
        public int mActorId;
        public List<int> mBuffIdList;
        public string mActorStr;
    }
    List<BuffActorData> mBuffActorDataList = new List<BuffActorData>();
    
    public BuffResult()
    {
        ResultTypeId = EResultManager.ENResultTypeID.enBuffResult;
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
        //if (IsEnabled)
        {
            foreach (var item in mBuffActorDataList)
            {
                Actor sourceActor = GetCurActor(item.mActorId, item.mActorType, item.mActorStr);

                if (sourceActor == null)
                {
                    return false;
                }
//                IResult r = null;
                switch (item.mBuffOptType)
                {
                    case AddBuffACTIONTYPE.addSpecifiedBuff:
                        foreach (var tmpBuffId in item.mBuffIdList)
                        {
                            BattleFactory.Singleton.AddBuff(sourceActor.ID, sourceActor.ID, tmpBuffId);
                        }
                        break;
                    case AddBuffACTIONTYPE.removeSpecifiedBuff:
                        
                        foreach (var tmpBuffId in item.mBuffIdList)
                        {
                            BattleFactory.Singleton.RemoveBuff(sourceActor.ID, sourceActor.ID, tmpBuffId);
                        }
                        break;
                    case AddBuffACTIONTYPE.removeAllBuff:
                        break;
                    case AddBuffACTIONTYPE.removeAllNegativeBuff:
                        break;
                    case AddBuffACTIONTYPE.removeAllPositiveBuff:
                        break;
                    default:
                        break;
                }
            }
            }
           
        return true;
    }
    public Actor GetCurActor(int actorId, AddBuffType actorType, string actorStr)
    {
        Actor sourceActor = null;
        switch (actorType)
        {
            case AddBuffType.Mainplayer:
                sourceActor = GetMainPlayer();
                break;
            case AddBuffType.specifiedNPC:
                sourceActor = GetSpecialNPC(actorId);
                break;
            case AddBuffType.CustomBlackboardStr:
                sourceActor = GetActorOftrigger(actorStr);
                break;
//             case AddBuffType.enTargetActor:
//                 break;
//             case AddBuffType.enSpecialActor:
//                break;
            default:
                break;
        }
        return sourceActor;
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
    public Actor GetSpecialNPC(int actorID)
    {
        SM.SceneRoom curRoom = SM.RandomRoomLevel.Singleton.LookupRoom(RoomGUID);
        if (curRoom == null)
        {
            return null;
        }
        Actor sourceActor = curRoom.GetMonsterByID(actorID);
        if (sourceActor == null)
        {
            return null;
        }
        return sourceActor;
    }
    public Actor GetActorOftrigger(string blackBoardActorStr)
    {
        Actor sourceActor = BattleArena.Singleton.m_blackBoard.GetBlackBoardActor(blackBoardActorStr);
        if (sourceActor == null)
        {
            return null;
        }
        return sourceActor;
    }
    public Actor GetTargetActor(string blackBoardActorStr)
    {
        Actor sourceActor = BattleArena.Singleton.m_blackBoard.GetBlackBoardActor(blackBoardActorStr);
        if (sourceActor == null)
        {
            return null;
        }
        return sourceActor;
    }
    public Actor GetSpecialActor(string blackBoardActorSt)
    {
        Actor sourceActor = BattleArena.Singleton.m_blackBoard.GetBlackBoardActor(blackBoardActorSt);
        if (sourceActor == null)
        {
            return null;
        }
        return sourceActor;
    }
    public override void ParseJsonData(LitJson.JsonData data)
    {
        base.ParseJsonData(data);
        LitJson.JsonData buffActorDataList = data[EResultManager.sBuffActorList];
        for (int i = 0; i < buffActorDataList.Count; i++ )
        {
            BuffActorData buffActorData = new BuffActorData();
            buffActorData.mActorId = int.Parse(buffActorDataList[i][EResultManager.sActorID].ToString());
            buffActorData.mActorType = (AddBuffType)int.Parse(buffActorDataList[i][EResultManager.sActorType].ToString());
            buffActorData.mBuffOptType = (AddBuffACTIONTYPE)int.Parse(buffActorDataList[i][EResultManager.sBuffOptType].ToString());
            LitJson.JsonData jsonBuffIdList = buffActorDataList[i][EResultManager.sBuffID];
            buffActorData.mBuffIdList = new List<int>();
            for (int j = 0; j < jsonBuffIdList.Count; j++ )
            {
                buffActorData.mBuffIdList.Add(int.Parse(jsonBuffIdList[j].ToString()));
            }
            buffActorData.mActorStr = buffActorDataList[i][EResultManager.sActorID].ToString();
            mBuffActorDataList.Add(buffActorData);
        }
    }
}