using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class EResultManager
{
    public enum ENResultTypeID
    {
        enNone,
        //关卡胜利
        enVictorToLevel         = 301,
        //刷新怪物
        enRefreshMonsterResult  ,
        //门操作
        enGateOpResult          ,
        //开启/关闭机关
        enTrapResult,
        //开启/关闭闸门
        enGateResult,
        //添加/移除Buff
        enBuffResult,
        // 掉落物品
        enDropItem,
    }
    public static string sresults           = "results";
    public static string sresultTypeId      = "resultTypeId";
    public static string seventId           = "eventId";
    public static string sparams            = "params";
    public static string sSerNo             = "serNo";
    public static string sresultList        = "resultList";
    static string slevelId                  = "levelId";
    static string sroomId                   = "roomId";



    //////////////////Gate op result////////////////////////////////////////////
    //房门打开、关闭(1:打开,0:关闭)

    //房间位置
    public static string sGateDirection = "dir";
    ///////////////////////////////////////////////////////////////////////
    static public EResultManager Singleton { get; private set; }
    //事件结果列表
    List<EResultBase> EResultList = new List<EResultBase>();
    //////////////////////////////////////////////////////////////////////////////////////////

    public static string sSwapMonsterDataList           = "SpawnNpcList";                       //刷新怪物信息列表
    public static string sSpawMonsterSerNo              = "monsterSerNo";                       // 怪物点的 序列号
    public static string sSpawnPosType                  = "spawnPosition";                      // 刷怪行为的 刷怪位置
    public static string sSpawnMonsterOffsetx           = "spawnMonsterOffsetx";                // 刷怪行为的 刷怪位置偏移x
    public static string sSpawnMonsterOffsetz           = "spawnMonsterOffsetz";                // 刷怪行为的 刷怪位置偏移z
    public static string sSpawnMonstertargetSerNo       = "spawnMonsterTargetSerNo";            // 刷怪行为的 刷怪制定怪物点序列号
    public static string sSpawnMonsterBlackBoardStr     = "spawnMonsterBlackBoardStr";          // 刷怪行为的 刷怪制定怪物点序列号
    public static string sSpawnMonstertargetType        = "SpawnMonstertargetType";             // 指定物件的类型

    public static string svictory                   = "victory"; // 关卡胜利或失败的行为 状态

    //////////////////////BuffResult传入的参数///////////////////////////
    public static string sActorType = "actorType";
    public static string sBuffOptType = "buffOptType";
    public static string sBuffID = "buffID";
    public static string sActorID = "actorID";
    public static string sBuffBlackboardStr = "BuffBlackboardStr";
    public static string sBuffActorList = "BuffActorList";

    public static string sSchemeTrapList = "SchemeTrapList";
    public static string sTrapType = "TrapType";
    public static string sTrapSerNo = "SchemeSerNo";
    public static string sTrapBlackboardStr = "TrapBlackboardStr";
    public static string sTrapOptType = "ActionType";

    public static string sSchemeKeyList = "SchemeKeyList";
    public static string sKeyId = "KeyId";
    public static string sKeyOptType = "KeyOptType";

    public static string sGateList = "GateList";
    public static string sGateID = "GateID";
    public static string sActionType = "ActionType";
    public static string sGateType  = "GateType";

    public static string sItemList = "ItemList";

    public static string sGateBlackboardStr = "GateBlackboardStr";

    public static string sItemType  = "ItemType";
    public static string sKeyID     = "KeyID";
    public static string sItemCount = "ItemCount";


    /////////////////////RefreshMonsterResult/////////////////////////
//     public static string sSwapMonsterSerNo = "monsterSerNo";                    //刷出的怪物ID
//     public static string sSpawnPositionType = "spawnPositionType";          //刷出怪物位置类型
//     public static string sSpawnMonsterOffsetx = "spawnMonsterOffsetx";      //刷新怪物坐标x的偏移量
//     public static string sSpawnMonsterOffsetz = "spawnMonsterOffsetz";      //刷新坐标z的偏移量
//     public static string sSpawnMonsterTargetSerNo = "spawnMonsterTargetSerNo";      //刷新在的物体ID
//     public static string sSpawnMonsterBlackboardStr = "spawnMonsterBlackboardStr";  //黑板中Actor的索引字符串

    //事件中在黑板上写下的特殊字段
    public static string sEventActorStr = "eventActorStr";

    public EResultManager()
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
    public EResultBase LookupResult(int guid)
    {
        foreach (EResultBase eve in EResultList)
        {
            if (eve.GUID == guid)
            {
                return eve;
            }
        }
        return null;
    }
    EResultBase CreateEResult(ENResultTypeID typeId)
    {
        switch (typeId)
        {
            case ENResultTypeID.enVictorToLevel:
                return new LevelWinEResult();
            case ENResultTypeID.enRefreshMonsterResult:
                return new RefreshMonsterResult();
            case ENResultTypeID.enGateOpResult:
                return new GateOpResult();
            case ENResultTypeID.enTrapResult:
                return new TrapResult();
            case ENResultTypeID.enBuffResult:
                return new BuffResult();
            case ENResultTypeID.enDropItem:
                return new DropItemResult();
            default:
                break;
        }
        return null;
    }
    public void FixedUpdate()
    {
        foreach (EResultBase eresult in EResultList)
        {
            if (eresult.IsTicked)
            {
                eresult.Tick();
            }
        }
    }
    public void ParseJsonResults(JsonData data)
    {
        if (data.Keys.Contains(sresults))
        {
            JsonData jItem = data[sresults];
            if (!jItem.IsArray)
                return;
            EResultList.Clear();
            for (int i = 0; i < jItem.Count; i++)
            {
                if (!jItem[i].Keys.Contains(sresultList))
                {
                    continue;
                }
                JsonData jResultList = jItem[i][sresultList];
                if (!jResultList.IsArray)
                {
                    Debug.Log("!jResultList.IsArray");
                    return;
                }
                int levelId = int.Parse(jItem[i][slevelId].ToString());
                int roomId = int.Parse(jItem[i][sroomId].ToString());
                for (int n = 0; n < jResultList.Count; n++ )
                {
                    int resultId = int.Parse(jResultList[n][sSerNo].ToString());
                    ENResultTypeID resultTypeId = (ENResultTypeID)int.Parse(jResultList[n][sresultTypeId].ToString());
                    EResultBase result = CreateEResult(resultTypeId);
//                     if (jResultList[n].Keys.Contains(sresultTypeId))
//                     {
//                         int tmpResultTypeId = int.Parse(jResultList[n][sresultTypeId].ToString());
//                         result.EventList.Add(tmpResultTypeId);
//                     }
//                     else
//                     {
//                         result.IsTicked = false;
//                     }
                    result.GUID = roomId * 1000 + resultId;
                    result.RoomGUID = roomId;
                    result.LevelId = levelId;
                    result.ParseJsonData(jResultList[n][sparams]);
                    EResultList.Add(result);
                }
            }
        }
        
    }
}