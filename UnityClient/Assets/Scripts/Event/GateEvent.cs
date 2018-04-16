using System;
using System.Collections.Generic;
using UnityEngine;

public class GeteEvent : EventBase
{
    public enum GateActiveType
    {
        enNone = -1,
        enOpen,
        enClose,
    }
    //机关ID
    public int mGateId = 0;
    //机关状态（开启/关闭）         
    public GateActiveType mGateActiveType = GateActiveType.enOpen;
    public GeteEvent()
    {
        triggerID = EventManager.ENTriggerID.enTriggerTrap;
    }
    public override void Tick()
    {
        base.Tick();
        SM.SceneRoom room = SM.RandomRoomLevel.Singleton.LookupRoom(RoomId);
        if (null != room)
        {
            SM.Gate curGateData = room.GetGateByID(mGateId);
            if (null != curGateData)
            {
                switch (mGateActiveType)
                {
                    case GateActiveType.enOpen:
                        if (curGateData.OutIsActive)
                        {
                            IsEnabled = true;
                        }
                        break;
                    case GateActiveType.enClose:
                        if (!curGateData.OutIsActive)
                        {
                            IsEnabled = true;
                        }
                        break;
                    default:
                        break;
                }

            }
        }
    }
    public override void ParseJsonData(LitJson.JsonData data)
    {
        base.ParseJsonData(data);
        mGateId = int.Parse(data[0].ToString());
        mGateActiveType = (GateActiveType)int.Parse(data[1].ToString());
        for (int i = 0; i < data.Count; i++)
        {

        }
        Debug.Log("levelId=" + LevelId + " roomId=" + RoomId + " TriggerTrapId=" + mGateId);
    }
}