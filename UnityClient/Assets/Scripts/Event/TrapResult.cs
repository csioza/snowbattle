using System;
using System.Collections.Generic;
using UnityEngine;

public class TrapResult : EResultBase
{
    ///////////////////////////////////////////////
    public enum ENTrapOptType
    {
        enActiveTrue,           //激活机关
        enActiveFalse,          //关闭机关
        enStateTrue,            //机关可激活
        enStateFlase,           //机关不可激活
    }
    public enum ENTrapType
    {
        enNone,
        enSpecial,
        TriggeringTrap,     //触发事件的机关（从黑板中读取）
        CustomBlackboardStr,  //黑板中的指定机关（自定义字符串)
    }
    public struct TrapData
    {
        public int mTrapId;
        public ENTrapType mTrapType;
        public Trap.TrapState mTrapState;
        public string mBlackBoardStr;
    }
    List<TrapData> mTrapDataList = new List<TrapData>();
    public TrapResult()
    {
        ResultTypeId = EResultManager.ENResultTypeID.enTrapResult;
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
            SM.SceneRoom room = SM.RandomRoomLevel.Singleton.LookupRoom(RoomGUID);
            foreach (var trapData in mTrapDataList)
            {
                Actor trapActor = null;// GetTrapActor(trapData.mTrapType);//room.GetTrapByID(trapData.mTrapId);
                switch (trapData.mTrapType)
                {
                    case ENTrapType.enSpecial:
                        trapActor = room.GetTrapByID(trapData.mTrapId);
                        break;
                    default:
                        break;
                }
                if (trapActor == null)
                {
                    return false;
                }
                Trap trap = trapActor as Trap;
                trap.SetTrapState(trapData.mTrapState);
            }
           
            
        }
        return true;
    }

    public override void ParseJsonData(LitJson.JsonData data)
    {
        base.ParseJsonData(data);
        LitJson.JsonData trapDataList = data[EResultManager.sSchemeTrapList];
        for (int i = 0; i < trapDataList.Count; i++ )
        {
            TrapData tmpData = new TrapData();
            tmpData.mTrapId = int.Parse(trapDataList[i][EResultManager.sTrapSerNo].ToString());
            tmpData.mTrapType = (ENTrapType)int.Parse(trapDataList[i][EResultManager.sTrapType].ToString());
            tmpData.mTrapState = (Trap.TrapState)int.Parse(trapDataList[i][EResultManager.sTrapOptType].ToString());
            tmpData.mBlackBoardStr = trapDataList[i][EResultManager.sTrapBlackboardStr].ToString();
            mTrapDataList.Add(tmpData);
        }
    }
}