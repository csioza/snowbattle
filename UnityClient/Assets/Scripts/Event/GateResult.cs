using System;
using System.Collections.Generic;
using UnityEngine;

public class GateResult : EResultBase
{
    ///////////////////////////////////////////////
    bool gateActive = false;
    int gateId = -1;
    public GateResult()
    {
        ResultTypeId = EResultManager.ENResultTypeID.enGateResult;
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
        if (IsEnabled)
        {
            SM.SceneRoom room = SM.RandomRoomLevel.Singleton.LookupRoom(RoomGUID);
            SM.Gate gate = room.GetGateByID(gateId);
            room.RoomOperateGate(gate, gate.gateDirectionIndex, gateActive);
        }
        return true;
    }
    public override void ParseJsonData(LitJson.JsonData data)
    {
        base.ParseJsonData(data);
        gateId = int.Parse(data[0].ToString());
        gateActive = bool.Parse(data[1].ToString());
    }
}