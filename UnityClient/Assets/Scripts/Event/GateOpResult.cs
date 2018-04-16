using System;
using System.Collections.Generic;
using UnityEngine;

public class GateOpResult : EResultBase
{
    ///////////////////////////////////////////////

    //房门打开、关闭(true:打开,false:关闭)
    public enum ENGateType
    {
        specifiedGate = 0,          //0.指定的闸门
        TriggeringGate,             //1.触发事件的闸门
        AffectedGate,               //2.受影响的闸门
        CustomBlackboardStr,        //3.黑板中的指定闸门（自定义字符串）
    }
    public struct GateOpenData
    {
        public int mGateId;
        public bool mGateAction;
        public ENGateType mGateType;
        public string mGateBlackboardStr;
    }
    List<GateOpenData> mGateDataList = new List<GateOpenData>();
    public GateOpResult()
    {
        ResultTypeId    = EResultManager.ENResultTypeID.enGateOpResult;
        IsTicked        = true;
        IsEnabled       = false;
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
        if (base.Execute())
        {
            SM.SceneRoom curRoom = SM.RandomRoomLevel.Singleton.LookupRoom(RoomGUID);
            if (null == curRoom)
            {
                return false;
            }
            foreach (var item in mGateDataList)
            {
                switch (item.mGateType)
                {
                    case ENGateType.specifiedGate:
                        SM.Gate gate = curRoom.CurRoomInfo.GetGate(item.mGateId);
                        if (gate.isGateOpen)
                        {
                            if (gate.OutIsActive != item.mGateAction)
                            {
                                curRoom.RoomOperateGate(gate, gate.gateDirectionIndex, item.mGateAction);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            
            return true;
        }
        return false;
    }
    public override void ParseJsonData(LitJson.JsonData data)
    {
        base.ParseJsonData(data);
        LitJson.JsonData GateDataList = data[EResultManager.sGateList];
        for (int i = 0; i < GateDataList.Count; i++)
        {
            GateOpenData gateOpenData = new GateOpenData();
            gateOpenData.mGateId = int.Parse(GateDataList[i][EResultManager.sGateID].ToString());
            int op = int.Parse(GateDataList[i][EResultManager.sActionType].ToString());
            gateOpenData.mGateAction = (op == 0) ? true : false;
            gateOpenData.mGateType = (ENGateType)int.Parse(GateDataList[i][EResultManager.sGateType].ToString());
            gateOpenData.mGateBlackboardStr = GateDataList[i][EResultManager.sGateBlackboardStr].ToString();
            mGateDataList.Add(gateOpenData);
        }       
//         int op = int.Parse(data[EResultManager.sActionType].ToString());
//         mOpState = (op == 1) ? true : false;
        
    }
}