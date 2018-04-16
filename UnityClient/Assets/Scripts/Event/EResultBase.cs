using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class EResultBase
{
    public enum ENActorType
    {
        enNone = -1,
        //主控角色
        enMainPlayer,
        //指定的NPC
        enSpecialNPC,
        //触发该事件的角色
        enActorOftriggerEvent,
        //触发事件的角色的目标
        enTargetActor,
        //指定String角色
        enSpecialActor,
    }
    public EResultManager.ENResultTypeID ResultTypeId = EResultManager.ENResultTypeID.enNone;
    //
    public int              GUID = 0;
    //房间ID
    public int              RoomGUID = -1;
    //
    public int              LevelId = 0;
    //是否生效
    public bool             IsEnabled = false;
    //是否走心跳
    public bool             IsTicked  = true;
    //触发事件列表 
    public List<int>        EventList = new List<int>();
    public EResultBase()
    {
        IsTicked = true;
    }
    public virtual void Tick()
    {
        foreach (int val in EventList)
        {
            EventBase eve = EventManager.Singleton.LookupEvent(val);
            if (null == eve || !eve.IsEnabled)
            {
                return;
            }
        }
        if (EventList.Count > 0)
        {
            IsEnabled = true;
            IsTicked = false;
        }
    }
    virtual public void ParseJsonData(JsonData data)
    {

    }
    virtual public bool Execute()
    {
        return true;
    }
}