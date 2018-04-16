using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;


public class EventManager
{
    public enum ENTriggerID
    {
        enNone                  = -1,
        //怪物死亡
        enMonsterDead           = 1,
        //怪物生命值低于 [参数1] 的百分比 
        enMonsterHPX            = 2,
        //怪物使用了技能 [参数1] 达 [参数2] 次数
        enMonsterUseSkillXCount = 3,
        //宝箱被打开
        enOpenBox               ,
        //机关被触发
        enTriggerTrap           ,
        //玩家接近闸门
        enActorCloseToGate      ,
        //玩家进入楼层
        enActorEnterLevel       ,
        //事件的结合
        enAndEvent              ,
        //杀死房间内所有怪物     
        enSkillAllMonster       ,
        //事件的或
        enOrEvent,
    }
    /// //////////////////////////////////////////////////////////
    public static string sevent             = "events";
    public static string seventId           = "eventId";
    public static string striggerTypeId     = "triggerTypeId";
    static string slevelId                  = "levelId";
    static string sroomId                   = "roomId";
    public static string sparams            = "params";
    public static string seventList         = "eventList"; // 事件列表
    public static string sactionList        = "actionList"; // 行为列表
    public static string sdeadNpcList       = "deadNpcList";// 死亡NPC列表
    public static string stargetNpcSerNo    = "targetNpcSerNo"; // 死亡NPC事件目标NPC序列号
    public static string snpcDeadNpcType    = "npcDeadNpcType"; //NPC死亡事件的 NPC制定类型
    public static string snpcDeadBlackboardActorName = "npcDeadBlackboardActorName"; //NPC死亡事件的 黑板中 所出现的角色名称

    public static string sSchemeTrapList    = "SchemeTrapList";// 机关触发列表
    public static string sTrapSerNo       = "SchemeSerNo";          // 机关 序列号
    public static string sTrapType          = "TrapType"; // 机关触发类型

    public static string sBoxSerNoList      = "BoxSerNoList";//宝箱开启列表
    public static string sboxSerNo          = "boxSerNo";          // 宝箱 序列号

    public static string sHPMonsterList = "HPMonsterList";  //角色血量事件角色列表
    public static string sHPMonsterSerNo = "HPMonsterSerNo";    //角色ID
    public static string sHPMonsterType = "HPMonsterType";    //角色类型
    public static string sHPblackboardActorName = "HPblackboardActorName";    // 事件 中的 黑板中 所出现的角色名称
    public static string sHPValType = "HPValType";    //数值类型
    public static string sHPValue = "HPValue";    //数值

    public static string sAddEventDataList = "AddEventDataList";        //add事件事件ID列表
    public static string sAddEventSerNo = "AddEventSerNo";                    //事件ID 标示符


    public static string sOrEventDataList = "OrEventDataList";        //or事件事件ID列表
    public static string sOrEventSerNo = "OrEventSerNo";                    //事件ID 标示符

    /// //////////////////////////////////////////////////////////
    static public EventManager Singleton { get; private set; }
    
    List<EventBase> EventList = new List<EventBase>();
    /////////////////////////////////////////////////////////////////
    public EventManager()
    {
        if (null == Singleton)
        {
            Singleton = this;
        }
        else
        {
            Debug.LogWarning("EventManager Recreated");
        }
    }
    static public EventBase CreateEvent(EventManager.ENTriggerID triggerId)
    {
        switch (triggerId)
        {
            case ENTriggerID.enMonsterDead:
                return new DeadEvent();
            case ENTriggerID.enMonsterHPX:
                return new HPEvent();
            case ENTriggerID.enMonsterUseSkillXCount:
                return new UsedSkillEvent();
            case ENTriggerID.enOpenBox:
                return new OpenBoxEvent();
            case ENTriggerID.enTriggerTrap:
                return new TriggerTrapEvent();
            case ENTriggerID.enActorCloseToGate:
                return new CloseToGateEvent();
            case ENTriggerID.enActorEnterLevel:
                return new EnterLevelEvent();
            case ENTriggerID.enAndEvent:
                return new AndEvent();
            case ENTriggerID.enSkillAllMonster:
                return new SkillAllMonsterEvent();
            default:
                break;
        }
        return null;
    }

    public void FixedUpdate()
    {
        foreach (EventBase eve in EventList)
        {
            if (eve.IsTicked)
            {
                eve.Tick();
                if (eve.IsEnabled)
                {
                    eve.Execute();
                    eve.IsTicked = false;
                }
            }
        }
    }
    public EventBase LookupEvent(int guid)
    {
        foreach (EventBase eve in EventList)
        {
            if (eve.Guid == guid)
            {
                return eve;
            }
        }
        return null;
    }
    public void ParseJsonEvents(JsonData data)
    {
        if (!data.Keys.Contains(sevent))
        {
            return;
        }
        JsonData jItem = data[sevent];
        if (!jItem.IsArray)
            return;
        EventList.Clear();
        for (int i = 0; i < jItem.Count; i++)
        {
            if (!jItem[i].Keys.Contains(seventList))
            {
                continue;
            }
            JsonData jEventList = jItem[i][seventList];
            if (!jEventList.IsArray)
            {
                Debug.Log("!jEventList.IsArray");
                return;
            }
            int levelId = int.Parse(jItem[i][slevelId].ToString());
            int roomId = int.Parse(jItem[i][sroomId].ToString());
            for (int n = 0; n < jEventList.Count; n++ )
            {
                int eventId = int.Parse(jEventList[n][seventId].ToString());
                ENTriggerID triggerId = (ENTriggerID)int.Parse(jEventList[n][striggerTypeId].ToString());
                EventBase eventBase = CreateEvent(triggerId);
                JsonData jActionIdList = jEventList[n][sactionList];
                if (jActionIdList.IsArray)
                {
                    for (int j = 0; j < jActionIdList.Count; j++)
                    {
                        int resultID = (roomId * 1000) + int.Parse(jActionIdList[j].ToString());
                        eventBase.ResultList.Add(resultID);
                    }
                }
                eventBase.Guid = (roomId * 1000) + eventId;
                eventBase.LevelId = levelId;
                eventBase.RoomId = roomId;
                eventBase.ParseJsonData(jEventList[n][sparams]);
                EventList.Add(eventBase);
            }
        }
     }
}
