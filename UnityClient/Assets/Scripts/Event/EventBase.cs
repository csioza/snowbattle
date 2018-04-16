using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class EventBase
{
    //指定的Actor类型
    public enum ENActorType
    {
        enNone = -1,
        enMainPlayer,
        enSpicalNpc,
        enOther,
    }
    public EventManager.ENTriggerID triggerID = EventManager.ENTriggerID.enNone;
    //是否生效
    public bool IsEnabled   = false;
    //是否移除
    public bool IsRemoved   = false;
    //
    public bool IsTicked    = true;
    //guid
    public int Guid         = -1;
    //levelid
    public int LevelId      = -1;
    //room id
    public int RoomId       = -1;
    //结果列表
    public List<int>        ResultList = new List<int>();
    public EventBase()
    {
        IsEnabled   = false;
        IsRemoved   = false;
        IsTicked    = true;
    }
    virtual public void Tick()
    {

    }
    virtual public void Notify()
    {

    }
    virtual public bool Execute()
    {
        foreach (var item in ResultList)
        {
           EResultBase itemResult = EResultManager.Singleton.LookupResult(item);
           if (itemResult != null)
           {
               itemResult.Execute();
           }
        }
        return true;
    }
    virtual public void ParseJsonData(JsonData data)
    {

    }
}
