
using System.Collections.Generic;
using System.IO;
using System;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 关卡编辑数据导出
/// </summary>
public class LevelDataOut
{
    static List<RME_BaseInfo> m_infoList            = new List<RME_BaseInfo>();
    static List<RME_EventInfo> m_eventList          = new List<RME_EventInfo>();
    static List<RME_ActionInfo> m_actionList         = new List<RME_ActionInfo>();

    static RoomInfo m_roomInfo = null;
    static float m_height                       = 1.25f;
    static ActorBornPointInfo m_actorBornPointInfo = new ActorBornPointInfo();

    static int m_roomSerNo          = 1;

    static DungeonMonsterTable m_dungeonMonsterTable = null;
    static SM.RandomRoomLevel m_sceneManger = null;

    static string sFileDir          = "Assets/levelEditor/";

    static int m_floorId = 0;
    [@MenuItem("SceneEditor/DisableMesh Collider")]
    static void DisableMeshCollider()
    {
        EnableMeshCollider(false);  
    }

    [@MenuItem("SceneEditor/EnableMesh Collider")]
    static void EnableMeshCollider()
    {
        EnableMeshCollider(true);

    }

    static void EnableMeshCollider(bool enable)
    {
        // 获得当前选中物件
        Transform[] selection = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab | SelectionMode.Editable);

        if (selection.Length <= 0)
        {
            return;
        }

        if (null == selection[0])
        {
            return;
        }

        // 选择的物件
        GameObject obj = selection[0].gameObject;



        // 获取一个物件及其子物件上的所有的 脚本组件
        Component[] children = obj.GetComponentsInChildren(typeof(MeshCollider), true);

        // 遍历物件上的所有脚本
        foreach (Component child in children)
        {
            Type t = child.GetType();
            if (null == t)
            {
                continue;
            }

      
            MeshCollider mesh = child as MeshCollider;
            mesh.enabled = enable;
           
        }
    }
    [@MenuItem("SceneEditor/LevelDataWrite")]
    static void Out()
    {

        // 获得当前选中物件
        Transform[] selection = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab | SelectionMode.Editable);

        if (selection.Length <= 0)
        {
            return;
        }

        if (null == selection[0])
        {
            return;
        }

        // 选择的物件
        GameObject obj          = selection[0].gameObject;

        // 函数名称列表
        //List<string> list       = new List<string>();

        // 重置序列号
        RMEBaseInfo.ResetIndex();

        // 获取一个物件及其子物件上的所有的 脚本组件
        Component[] children    = obj.GetComponentsInChildren(typeof(MonoBehaviour),true);

        m_infoList.Clear();

        m_eventList.Clear();

        m_actionList.Clear();

        if (null != m_roomInfo)
        {
            m_roomInfo.Reset();
        }

        //房间标签信息
        RoomEditData(obj);

        // 先 生成阵型及怪物 点的 唯一ID
        // 遍历物件上的所有脚本
        foreach (Component child in children)
        {
            if (null == child)
            {
                continue;
            }
            Type t = child.GetType();
            if (null == t)
            {
                continue;
            }

            if (child is RMEBaseInfoEdit)
            {
                switch (((RMEBaseInfoEdit)child).m_type)
                {
                    // 阵型
                    case RMEBaseInfoEdit.Type.enArray:
                        {
                            ArrayEditData(child.gameObject);
                            break;
                        }
                    // 适配点 距离
                    case RMEBaseInfoEdit.Type.enAdjustPos:
                        {
                            PosEditData(child.gameObject);
                            break;
                        }
                }
            }
        }

         // 遍历物件上的所有脚本 基础信息在这里
        foreach (Component child in children)
        {
            if (null == child)
            {
                continue;
            }

            Type t = child.GetType();
            if (null == t)
            {
                continue;
            }
            #region 基础信息
            // 基础信息
            if (child is RMEBaseInfoEdit)
            {          
                switch (((RMEBaseInfoEdit)child).m_type)
                {
                    //  宝箱
                    case RMEBaseInfoEdit.Type.enBox:
                        {
                            BoxEditData(child.gameObject);
                            break;
                        }
                    // 机关
                    case RMEBaseInfoEdit.Type.enScheme:
                        {
                            SchemeEditData(child.gameObject); 
                            break;
                        }
                    //  传送门
                    case RMEBaseInfoEdit.Type.enTranspotDoor:
                        {
                            TranspotEditData(child.gameObject);
                            break;
                        }
                    // 门 包括过关条件
                    case RMEBaseInfoEdit.Type.enGate:
                        {
                            GateEditData(child.gameObject);
                            break;
                        }
                    // 人物出生点
                    case RMEBaseInfoEdit.Type.enActorBorn:
                        {
                            ActorBornEditData(child.gameObject);
                            break;
                        }
                    // 人物传送点
                    case RMEBaseInfoEdit.Type.enTranspotPoint:
                        {
                            TranspotPointEditData(child.gameObject);
                            break;
                        }
                }
            }

            #endregion
        }

        // 遍历物件上的所有脚本 行为信息在这里
        foreach (Component child in children)
        {
            if (null == child)
            {
                continue;
            }

            Type t = child.GetType();
            if (null == t)
            {
                continue;
            }

            #region 行为信息
            // 行为信息
            if (child is LE_Action_Base)
            {

                switch (((LE_Action_Base)child).m_type)
                {
                    // 刷怪行为
                    case EResultManager.ENResultTypeID.enRefreshMonsterResult:
                        {
                            ActionSpawnNpcInfoData(child.gameObject);
                            break;
                        }
                    // 关卡胜利或失败行为
                    case EResultManager.ENResultTypeID.enVictorToLevel:
                        {
                            ActionVictorToLevelInfoData(child.gameObject);
                            break;
                        }
                    // 行为 -使机关 锁定/失效/开启/关闭
                    case EResultManager.ENResultTypeID.enTrapResult:
                        {
                            ActionSchemeTrapData(child.gameObject);
                            break;
                        }
                    // 行为 -使机关 锁定/失效/开启/关闭
                    case EResultManager.ENResultTypeID.enGateOpResult:
                        {
                            ActionGateOpData(child.gameObject);
                            break;
                        }
                    // 行为-掉落物品
                    case EResultManager.ENResultTypeID.enDropItem:
                        {
                            ActionDropItemData(child.gameObject);
                            break;
                        }
                    // 行为-添加/移除 buff给角色
                    case EResultManager.ENResultTypeID.enBuffResult:
                        {
                            ActionAddBuffData(child.gameObject);
                            break;
                        }


                }
            }
            #endregion

        }

        // 遍历物件上的所有脚本 事件在这里
        foreach (Component child in children)
        {
            if (null == child)
            {
                continue;
            }

            Type t = child.GetType();
            if (null == t)
            {
                continue;
            }


            #region 事件信息
            // 事件信息
            if (child is LE_Event_Base)
            {
                switch (((LE_Event_Base)child).m_type)
                {
                    // NPC死亡事件
                    case EventManager.ENTriggerID.enMonsterDead:
                        {
                            NpcDeadData(child.gameObject);
                            break;
                        }

                    // and事件 序列号清零
                    case EventManager.ENTriggerID.enAndEvent:
                        {
                            EventAndDataClearSerNo(child.gameObject);
                            break;
                        }
                    // OR事件 序列号清零
                    case EventManager.ENTriggerID.enOrEvent:
                        {
                            EventORdDataClearSerNo(child.gameObject);
                            break;
                        }
                    // 机关触发
                    case EventManager.ENTriggerID.enTriggerTrap:
                        {
                            Event_SchemeActionData(child.gameObject);
                            break;
                        }

                    // 宝箱被打开事件
                    case EventManager.ENTriggerID.enOpenBox:
                        {
                            EventBoxOpenData(child.gameObject);
                            break;
                        }

                    // //怪物生命值低于 [参数1] 的百分比触发
                    case EventManager.ENTriggerID.enMonsterHPX:
                        {
                            Event_HpData(child.gameObject);
                            break;
                        }
                }
            }
            #endregion

        }
       
         // 最后生成 AND OR 等 触发事件逻辑关系相关数据 因为需要 其他事件与行为 先生成序列号
         // 遍历物件上的所有脚本
        foreach (Component child in children)
        {
            if (null == child)
            {
                continue;
            }
            Type t = child.GetType();
            if (null == t)
            {
                continue;
            }

            if (child is LE_Event_Base)
            {
               // Debug.Log("t.FullName:" + t.FullName + ",((LE_Action_Base)child ).m_type:" + ((LE_Event_Base)child).m_type);
                switch (((LE_Event_Base)child).m_type )
                {
                    case EventManager.ENTriggerID.enAndEvent:
                        {
                            EventAndData(child.gameObject);
                            break;
                        }
                    case EventManager.ENTriggerID.enOrEvent:
                        {
                            EventOrData(child.gameObject);
                            break;
                        }
                }
            }
        }

        // 检查排错
        foreach (Component child in children)
        {
            if (null == child)
            {
                continue;
            }
            Type t = child.GetType();
            if (null == t)
            {
                continue;
            }
            #region 排查基本信息
            if (child is RMEBaseInfoEdit)
            {
                switch (((RMEBaseInfoEdit)child).m_type)
                {
                    // 怪物点
                    case RMEBaseInfoEdit.Type.enMonster:
                        {
                            MonsterDataCheck(child.gameObject);
                            
                            break;
                        }
                    // 宝箱
                    case RMEBaseInfoEdit.Type.enBox:
                        {
                            BoxDataCheck(child.gameObject);
                            break;
                        }
                    // 机关
                    case RMEBaseInfoEdit.Type.enScheme:
                        {
                            SchemeDataCheck(child.gameObject);
                            break;
                        }
                    // 房间
                    case RMEBaseInfoEdit.Type.enRoom:
                        {
                            RoomDataCheck(child.gameObject);
                            break;
                        }
                }
            }
            #endregion

            #region 排查事件信息
            if (child is LE_Event_Base)
            {
                switch (((LE_Event_Base)child).m_type)
                {
                        // 怪物死亡事件
                    case EventManager.ENTriggerID.enMonsterDead:
                        {
                            MonsterDeadEventDataCheck(child.gameObject);
                            break;
                        }
                    // 宝箱开启事件
                    case EventManager.ENTriggerID.enOpenBox:
                        {
                            BoxOpenEventDataCheck(child.gameObject);
                            break;
                        }
                     // 生命值事件
                    case EventManager.ENTriggerID.enMonsterHPX:
                        {
                            HPEventDataCheck(child.gameObject);
                            break;
                        }
                    // 机关事件
                    case EventManager.ENTriggerID.enTriggerTrap:
                        {
                            SchemeEventDataCheck(child.gameObject);
                            break;
                        }
                    // and事件
                    case EventManager.ENTriggerID.enAndEvent:
                        {
                            AndEventDataCheck(child.gameObject);
                            break;
                        }
                    // OR事件
                    case EventManager.ENTriggerID.enOrEvent:
                        {
                            OREventDataCheck(child.gameObject);
                            break;
                        }
                }
            }
            #endregion

            #region 排查动作信息
            if (child is LE_Action_Base)
            {
                switch (((LE_Action_Base)child).m_type)
                {

                        // 添加BUFF动作
                    case EResultManager.ENResultTypeID.enBuffResult:
                        {
                            AddBuffActionDataCheck(child.gameObject);
                            break;
                        }

                    // 掉落物品动作
                    case EResultManager.ENResultTypeID.enDropItem:
                        {
                            DropItemActionDataCheck(child.gameObject);
                            break;
                        }
                    // 闸门动作
                    case EResultManager.ENResultTypeID.enGateOpResult:
                        {
                            GateOpActionDataCheck(child.gameObject);
                            break;
                        }

                    // 机关动作
                    case EResultManager.ENResultTypeID.enTrapResult:
                        {
                            SchemeActionDataCheck(child.gameObject);
                            break;
                        }
                    // 刷怪动作
                    case EResultManager.ENResultTypeID.enRefreshMonsterResult:
                        {
                            SpawnNpcActionDataCheck(child.gameObject);
                            break;
                        }
                }
            }
            #endregion
        }

        string fileName = sFileDir + obj.name;

         using (FileStream targetFile = new FileStream(fileName, FileMode.Create))
         {
             byte[] buff = Save();
 
             targetFile.Write(buff, 0, buff.Length);
         }

        m_actorBornPointInfo.haveData = 0;
    }

    static public byte[] Save()
    {
       
        BinaryHelper helper = new BinaryHelper();

        // 房间标签信息

        helper.Write(m_roomInfo.prefabName); //  房间预设名称
        helper.Write(m_roomInfo.terrainId);  //  地形ID

        // 标签数量
        helper.Write(m_roomInfo.tagList.Count);
        // 标签ID
        foreach (int id in m_roomInfo.tagList)
        {
            helper.Write(id);
        }

        helper.Write(m_actorBornPointInfo.haveData);
        // 如果此房间 有人物出生点则记录数据
        if ( m_actorBornPointInfo.haveData == 1 )
        {
            helper.Write(m_actorBornPointInfo.x);
            helper.Write(m_actorBornPointInfo.z);

            Debug.Log("此房间的人物出生点 写数据 m_actorBornPointInfo.x：" + m_actorBornPointInfo.x + ",m_actorBornPointInfo.z:" + m_actorBornPointInfo.z);
        }
        Debug.Log("roomInfo.tag:" + m_roomInfo.tagList + "roomInfo.prefabName:" + m_roomInfo.prefabName + ",roomInfo.terrainId:" + m_roomInfo.terrainId);
        

        int total   = m_infoList.Count;
        // 基础信息数量
        helper.Write(total);

        Debug.Log("total:" + total);

         foreach (RME_BaseInfo baseInfo in m_infoList)
         {
             helper.Write((int)baseInfo.m_type);
             baseInfo.Write(helper);
         }

        // 以下格式为 JASON数据格式

         string actionStr   = "";
         // 行为
         actionStr          = "[";
         int count = 0;
         foreach (RME_ActionInfo actionInfo in m_actionList)
         {
             count++;
             actionStr = actionStr + "{";
             actionStr = actionStr + "\"" + EResultManager.sresultTypeId + "\":" + actionInfo.actionId;
             actionStr = actionStr + ",";
             actionStr = actionStr + "\"" + EResultManager.sSerNo + "\":" + actionInfo.serNo;
             actionStr = actionStr + ",";

             actionStr = actionStr + "\"" + EResultManager .sparams+ "\":{";
             actionInfo.WriteStr(actionStr,out actionStr);
             actionStr = actionStr + "}";


             actionStr = actionStr + "}";
             if (count != m_actionList.Count)
             {
                  actionStr = actionStr + ",";
             }
         }
         actionStr = actionStr + "]";

         Debug.Log("actionStr:" + actionStr);

         helper.Write(actionStr);

        // 事件数量
        int eventCount  = 0 ;
        string eventStr = "";
        // 事件
        eventStr        = "[";
        foreach (RME_EventInfo eventInfo in  m_eventList )
        {
            eventCount++;
            eventStr = eventStr + "{";
            eventStr = eventStr + "\"" + EventManager.striggerTypeId + "\":" + eventInfo.eventId;
            eventStr = eventStr + ",";
            eventStr = eventStr + "\"" + EventManager.seventId + "\":" + eventInfo.serNo;
            eventStr = eventStr + ",";
            Debug.Log("RME_EventInfo actionList.Count:" + eventInfo.actionlist.Count);

            eventStr = eventStr + "\"" + EventManager.sactionList + "\":[";

            for (int i = 0; i < eventInfo.actionlist.Count; i++)
            {
                eventStr = eventStr + eventInfo.actionlist[i];
                if (i + 1 != eventInfo.actionlist.Count)
                 {
                     eventStr = eventStr + ",";
                 }
            }
            eventStr    = eventStr + "],";

            eventStr = eventStr + "\"" + EventManager .sparams+ "\":{";
            eventInfo.WriteStr(eventStr, out eventStr);
            eventStr    = eventStr + "}";


            eventStr    = eventStr + "}";
            if (eventCount != m_eventList.Count)
            {
                eventStr= eventStr + ",";
            }

        }
        eventStr = eventStr + "]";

        helper.Write(eventStr);
        Debug.Log("eventStr:" + eventStr);
        return helper.GetBytes();
    }

    // 宝箱数据检查
    static public void BoxDataCheck(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }
        RMEBoxEdit editInfo = obj.GetComponent<RMEBoxEdit>();
        if (null == editInfo)
        {
            return;
        }
        if (editInfo.showPecent > -0.001 && editInfo.showPecent<0.001)
        {
            Debug.LogError(obj.name + " 怪物点 属性的Show Pecent 为0！请检查");
        }

        if (editInfo.id ==0 )
        {
            Debug.LogError(obj.name + "怪物点 属性的Id 为0！请检查");
        }
       
    }

    // 刷怪动作数据检查
    static public void SpawnNpcActionDataCheck(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Action_SpawnNpc monsterEditInfo = obj.GetComponent<LE_Action_SpawnNpc>();
        if (null == monsterEditInfo)
        {
            return;
        }

        if (monsterEditInfo.spawnNpcList.Count == 0)
        {
            Debug.LogError(obj.name + "刷怪动作  spawnNpcList 为0个！请检查");
        }

        for (int i = 0; i < monsterEditInfo.spawnNpcList.Count; i++)
        {
            if (monsterEditInfo.spawnNpcList[i].targetNpc == null)
            {

                Debug.LogError(obj.name + "刷怪动作 spawnNpcList[" + i + "]属性的targetNpc 为空！请检查");
                continue;
            }

            if (monsterEditInfo.spawnNpcList[i].targetNpc.GetComponent<RMEMonsterEdit>() == null)
            {
                Debug.LogError(obj.name + "刷怪动作 List[" + i + "]属性的targetNpc上 无RMEMonsterEdit脚本 ！请检查");
            }
        }
    }

    // 机关动作数据检查
    static public void SchemeActionDataCheck(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Action_SchemeTrap monsterEditInfo = obj.GetComponent<LE_Action_SchemeTrap>();
        if (null == monsterEditInfo)
        {
            return;
        }

        if (monsterEditInfo.List.Count == 0)
        {
            Debug.LogError(obj.name + "机关动作  List 为0个！请检查");
        }

        for (int i = 0; i < monsterEditInfo.List.Count; i++)
        {
            if (monsterEditInfo.List[i].trapType == TrapResult.ENTrapType.enSpecial)
            {
                if (monsterEditInfo.List[i].schemeObj == null)
                {
                    Debug.LogError(obj.name + "机关动作 List[" + i + "]属性的schemeObj 为空！请检查");
                    continue;
                }

                if (monsterEditInfo.List[i].schemeObj.GetComponent<RMESchemeEdit>() == null)
                {
                    Debug.LogError(obj.name + "机关动作 List[" + i + "]属性的schemeObj上 无RMESchemeEdit脚本 ！请检查");
                }
            }
        }
    }

    // 闸门动作数据检查
    static public void GateOpActionDataCheck(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Action_Gate monsterEditInfo = obj.GetComponent<LE_Action_Gate>();
        if (null == monsterEditInfo)
        {
            return;
        }

        if (monsterEditInfo.List.Count == 0)
        {
            Debug.LogError(obj.name + "闸门动作  List 为0个！请检查");
        }

        for (int i = 0; i < monsterEditInfo.List.Count; i++ )
        {
            if (monsterEditInfo.List[i].gateType == GateOpResult.ENGateType.specifiedGate)
            {
                if ( monsterEditInfo.List[i].targetGateObj == null)
                {
                    Debug.LogError(obj.name + "闸门动作 List["+i+"]属性的targetGateObj 为空！请检查");
                    continue;
                }

                if (monsterEditInfo.List[i].targetGateObj.GetComponent<RMEGateEdit>() == null)
                {
                    Debug.LogError(obj.name + "闸门动作 List[" + i + "]属性的targetGateObj上 无RMEGateEdit脚本 ！请检查");
                }
            }
        }
    }

    // 掉落物品动作数据检查
    static public void DropItemActionDataCheck(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Action_DropItem monsterEditInfo = obj.GetComponent<LE_Action_DropItem>();
        if (null == monsterEditInfo)
        {
            return;
        }

        if (monsterEditInfo.List.Count == 0)
        {
            Debug.LogError(obj.name + "掉落物品动作  List 为0个！请检查");
        }
    }

    // 添加BUFF动作数据检查
    static public void AddBuffActionDataCheck(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Action_AddBuff monsterEditInfo = obj.GetComponent<LE_Action_AddBuff>();
        if (null == monsterEditInfo)
        {
            return;
        }

        if (monsterEditInfo.List.Count == 0 )
        {
            Debug.LogError(obj.name + "添加BUFF动作  List 为0个！请检查");
        }

        for (int i = 0; i < monsterEditInfo.List.Count; i++)
        {

            if (monsterEditInfo.List[i].actorType == LE_Action_AddBuffInfo.Type.specifiedNPC)
            {
                if (monsterEditInfo.List[i].targetObj == null )
                {
                    Debug.LogError(obj.name + "添加BUFF动作 索引" + i + "的targetObj为空 请检查");
                    continue;
                }

                if (monsterEditInfo.List[i].targetObj.GetComponent<RMEMonsterEdit>() == null)
                {
                    Debug.LogError(obj.name + "添加BUFF动作 索引" + i + "的targetObj上无 RMEMonsterEdit脚本 请检查");
                }
            }

            if (monsterEditInfo.List[i].actionType == LE_Action_AddBuffInfo.ACTIONTYPE.addSpecifiedBuff ||
                monsterEditInfo.List[i].actionType == LE_Action_AddBuffInfo.ACTIONTYPE.removeSpecifiedBuff)
           {

               if (monsterEditInfo.List[i].buffList.Count == 0)
                {
                    Debug.LogError(obj.name + "添加BUFF动作 索引" + i + "的buffList 数量为0  请检查");
                }

               for (int j = 0; j < monsterEditInfo.List[i].buffList.Count;j++ )
               {
                   if (monsterEditInfo.List[i].buffList[j] == 0 )
                   {
                       Debug.LogError(obj.name + "添加BUFF动作 索引" + i + "的buffList["+j+"] 为0  请检查");
                   }
               }
           }
        }
    }

    // or事件数据检查
    static public void OREventDataCheck(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Event_Or monsterEditInfo = obj.GetComponent<LE_Event_Or>();
        if (null == monsterEditInfo)
        {
            return;
        }

        if (monsterEditInfo.EventList.Count < 2)
        {
            Debug.LogError(obj.name + "OR事件 EventList少于2个！请检查");
        }

        for (int i = 0; i < monsterEditInfo.EventList.Count; i++)
        {

            if (monsterEditInfo.EventList[i] == null)
            {
                Debug.LogError(obj.name + "OR事件EventList索引" + i + "的target为空 请检查");
                continue;
            }

            Component[] children = monsterEditInfo.EventList[i].GetComponents(typeof(LE_Event_Base));

            if (children.Length == 0)
            {
                Debug.LogError(obj.name + "OR事件EventList 索引" + i + "的 物件 无事件脚本 请检查");
            }
        }

        if (monsterEditInfo.ActionList.Count == 0)
        {
            Debug.LogError(obj.name + "OR事件 ACTIONlIST为空！请检查");
        }

        for (int i = 0; i < monsterEditInfo.ActionList.Count; i++)
        {
            if (monsterEditInfo.ActionList[i] == null)
            {
                Debug.LogError(obj.name + "OR事件ACTIONlIST 索引" + i + "的ACTION 为空 请检查");
                continue;
            }

            Component[] children = monsterEditInfo.ActionList[i].GetComponents(typeof(LE_Action_Base));

            if (children.Length == 0)
            {
                Debug.LogError(obj.name + "OR事件ACTIONlIST 索引" + i + "的ACTION 物件 无ACTION脚本 请检查");
            }
        }
    }
    // and事件数据检查
    static public void AndEventDataCheck(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Event_And monsterEditInfo = obj.GetComponent<LE_Event_And>();
        if (null == monsterEditInfo)
        {
            return;
        }

        if (monsterEditInfo.EventList.Count < 2 )
        {
            Debug.LogError(obj.name + "and事件 EventList少于2个！请检查");
        }

        for (int i = 0; i < monsterEditInfo.EventList.Count; i++)
        {

            if (monsterEditInfo.EventList[i] == null)
            {
                Debug.LogError(obj.name + "and事件EventList索引" + i + "的target为空 请检查");
                continue;
            }

            Component[] children = monsterEditInfo.EventList[i].GetComponents(typeof(LE_Event_Base));

            if (children.Length == 0)
            {
                Debug.LogError(obj.name + "and事件EventList 索引" + i + "的 物件 无事件脚本 请检查");
            }
        }

        if (monsterEditInfo.ActionList.Count == 0)
        {
            Debug.LogError(obj.name + "and事件 ACTIONlIST为空！请检查");
        }

        for (int i = 0; i < monsterEditInfo.ActionList.Count; i++)
        {
            if (monsterEditInfo.ActionList[i] == null)
            {
                Debug.LogError(obj.name + "and事件ACTIONlIST 索引" + i + "的ACTION 为空 请检查");
                continue;
            }

            Component[] children = monsterEditInfo.ActionList[i].GetComponents(typeof(LE_Action_Base));

            if (children.Length == 0)
            {
                Debug.LogError(obj.name + "and事件ACTIONlIST 索引" + i + "的ACTION 物件 无ACTION脚本 请检查");
            }
        }
    }

    // 机关事件数据检查
    static public void SchemeEventDataCheck(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Event_SchemeTrap monsterEditInfo = obj.GetComponent<LE_Event_SchemeTrap>();
        if (null == monsterEditInfo)
        {
            return;
        }

        if (monsterEditInfo.schemeList.Count == 0)
        {
            Debug.LogError(obj.name + "机关事件 schemeList为空！请检查");
        }

        for (int i = 0; i < monsterEditInfo.schemeList.Count; i++)
        {

            if (monsterEditInfo.schemeList[i].target == null)
            {
                Debug.LogError(obj.name + "机关事件索引" + i + "的target为空 请检查");
                continue;
            }


            if (monsterEditInfo.schemeList[i].target.GetComponent<RMESchemeEdit>() == null)
            {
                Debug.LogError(obj.name + "机关事件索引 " + i + "的 target无RMESchemeEdit脚本  请检查");
            }
          

        }

        if (monsterEditInfo.ActionList.Count == 0)
        {
            Debug.LogError(obj.name + "机关事件 ACTIONlIST为空！请检查");
        }

        for (int i = 0; i < monsterEditInfo.ActionList.Count; i++)
        {
            if (monsterEditInfo.ActionList[i] == null)
            {
                Debug.LogError(obj.name + "机关事件ACTIONlIST 索引" + i + "的ACTION 为空 请检查");
                continue;
            }

            Component[] children = monsterEditInfo.ActionList[i].GetComponents(typeof(LE_Action_Base));

            if (children.Length == 0)
            {
                Debug.LogError(obj.name + "机关事件ACTIONlIST 索引" + i + "的ACTION 物件 无ACTION脚本 请检查");
            }
        }
    }

    // 生命值事件数据检查
    static public void HPEventDataCheck(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Event_HP monsterEditInfo = obj.GetComponent<LE_Event_HP>();
        if (null == monsterEditInfo)
        {
            return;
        }

        if (monsterEditInfo.npcList.Count == 0)
        {
            Debug.LogError(obj.name + "生命值事件 npcList为空！请检查");
        }

        for (int i = 0; i < monsterEditInfo.npcList.Count; i++)
        {

            if (monsterEditInfo.npcList[i].hpValue == 0)
            {
                Debug.LogError(obj.name + "生命值事件索引" + i + "的生命值为0 请检查");
                continue;
            }

            if (monsterEditInfo.npcList[i].npcType == DeadEvent.ENDeadActorType.enSpicalNpc)
            {
                if ( monsterEditInfo.npcList[i].targetNpc == null)
                {
                    Debug.LogError(obj.name + "生命值事件 索引" + i + "的 targetNpc为空  请检查");
                    continue;
                }

                if (monsterEditInfo.npcList[i].targetNpc.GetComponent<RMEMonsterEdit>() == null)
                {
                    Debug.LogError(obj.name + "生命值事件 索引" + i + "的 targetNpc无怪物RMEMonsterEdit脚本  请检查");
                }
            }

        }

        if (monsterEditInfo.ActionList.Count == 0)
        {
            Debug.LogError(obj.name + "生命值事件 ACTIONlIST为空！请检查");
        }

        for (int i = 0; i < monsterEditInfo.ActionList.Count; i++)
        {
            if (monsterEditInfo.ActionList[i] == null)
            {
                Debug.LogError(obj.name + "生命值事件ACTIONlIST 索引" + i + "的ACTION 为空 请检查");
                continue;
            }

            Component[] children = monsterEditInfo.ActionList[i].GetComponents(typeof(LE_Action_Base));

            if (children.Length == 0)
            {
                Debug.LogError(obj.name + "生命值事件ACTIONlIST 索引" + i + "的ACTION 物件 无ACTION脚本 请检查");
            }
        }
    }

    // 宝箱开启事件数据检查
    static public void BoxOpenEventDataCheck(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Event_BoxOpen monsterEditInfo = obj.GetComponent<LE_Event_BoxOpen>();
        if (null == monsterEditInfo)
        {
            return;
        }

        if (monsterEditInfo.boxList.Count == 0)
        {
            Debug.LogError(obj.name + "宝箱开启事件 boxList为空！请检查");
        }

        for (int i = 0; i < monsterEditInfo.boxList.Count; i++)
        {

            if (monsterEditInfo.boxList[i] == null)
            {
                Debug.LogError(obj.name + "宝箱开启事件" + i + " 为空 请检查");
                continue;
            }

            if (monsterEditInfo.boxList[i].GetComponent<RMEBoxEdit>() == null)
            {
                Debug.LogError(obj.name + "宝箱开启事件" + i + "  没有 RMEBoxEdit脚本 请检查");
                continue;
            }
            
        }

        if (monsterEditInfo.ActionList.Count == 0)
        {
            Debug.LogError(obj.name + "宝箱开启事件 ACTIONlIST为空！请检查");
        }

        for (int i = 0; i < monsterEditInfo.ActionList.Count; i++)
        {
            if (monsterEditInfo.ActionList[i] == null)
            {
                Debug.LogError(obj.name + "宝箱开启事件ACTIONlIST 索引" + i + "的ACTION 为空 请检查");
                continue;
            }

            Component[] children = monsterEditInfo.ActionList[i].GetComponents(typeof(LE_Action_Base));

            if (children.Length == 0)
            {
                Debug.LogError(obj.name + "宝箱开启事件ACTIONlIST 索引" + i + "的ACTION 物件 无ACTION脚本 请检查");
            }
        }
    }

    // NPC死亡事件数据检查
    static public void MonsterDeadEventDataCheck(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Event_NpcDead monsterEditInfo = obj.GetComponent<LE_Event_NpcDead>();
        if (null == monsterEditInfo)
        {
            return;
        }

        if (monsterEditInfo.npcList.Count == 0)
        {
            Debug.LogError(obj.name + "怪物死亡事件 NPClist为空！请检查");
        }

        for (int i = 0; i < monsterEditInfo.npcList.Count; i++)
        {
            if (monsterEditInfo.npcList[i].npcType == DeadEvent.ENDeadActorType.enSpicalNpc)
            {
                if (monsterEditInfo.npcList[i].targetNpc == null)
                {
                    Debug.LogError(obj.name + "怪物死亡事件" + i + "的targetNpc 为空 请检查");
                    continue;
                }

                if (monsterEditInfo.npcList[i].targetNpc.GetComponent<RMEMonsterEdit>() == null)
                {
                    Debug.LogError(obj.name + "怪物死亡事件" + i + "的targetNpc 不是怪物点 没有 RMEMonsterEdit脚本 请检查");
                    continue;
                }
            }
        }

        if (monsterEditInfo.ActionList.Count == 0)
        {
            Debug.LogError(obj.name + "怪物死亡事件 ACTIONlIST为空！请检查");
        }

        for (int i = 0; i < monsterEditInfo.ActionList.Count; i++)
        {
            if (monsterEditInfo.ActionList[i] == null)
            {
                Debug.LogError(obj.name + "怪物死亡事件ACTIONlIST 索引" + i + "的ACTION 为空 请检查");
                continue;
            }

            Component[] children = monsterEditInfo.ActionList[i].GetComponents(typeof(LE_Action_Base));

            if (children.Length == 0 )
            {
                Debug.LogError(obj.name + "怪物死亡事件ACTIONlIST 索引" + i + "的ACTION 物件 无ACTION脚本 请检查");
            }
        }
    }

    // 怪物点数据检查
    static public void MonsterDataCheck(GameObject obj)
    {
        if ( null == obj )
        {
            return;
        }

        RMEMonsterEdit monsterEditInfo = obj.GetComponent<RMEMonsterEdit>();
        if (null == monsterEditInfo)
        {
            return;
        }

        if (monsterEditInfo.monsterList.Count == 0)
        {
            Debug.LogError(obj.name + "怪物列表为空！请检查");
        }

        for (int i = 0; i < monsterEditInfo.monsterList.Count; i++)
        {
            int id = monsterEditInfo.monsterList[i].id;
            int weight = monsterEditInfo.monsterList[i].weight;

            if (id == 0)
            {
                Debug.LogError(obj.name + "怪物索引为" + i + "的ID 为0请检查");
            }

            if (weight == 0)
            {
                Debug.LogError(obj.name + "怪物索引为" + i + "的weight 为0请检查");
            }
        }
    }

    // 机关数据检查
    static public void SchemeDataCheck(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }
        RMESchemeEdit editInfo = obj.GetComponent<RMESchemeEdit>();
        if (null == editInfo)
        {
            return;
        }
        if (editInfo.showPercent > -0.001 && editInfo.showPercent < 0.001)
        {
            Debug.LogError(obj.name + " 机关 属性的Show Pecent 为0！请检查");
        }

        if (editInfo.npcId == 0)
        {
            Debug.LogError(obj.name + "机关 属性的npcId 为0！请检查");
        }

    }

    // 房间数据检查
    static public void RoomDataCheck(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }
        RMERoomEdit editInfo = obj.GetComponent<RMERoomEdit>();
        if (null == editInfo)
        {
            return;
        }
      
        if (editInfo.terrainId == 0)
        {
            Debug.LogError(obj.name + "房间 属性的terrainId 为0！请检查");
        }

    }


    // 宝箱存储数据
    static public void BoxEditData( GameObject obj )
    {
        if ( null == obj )
        {
            return;
        }
        RMEBoxEdit editInfo =  obj.GetComponent<RMEBoxEdit>();
        if (null == editInfo)
        {
            return;
        }

        BoxInfo info    = new BoxInfo();
        
        info.percet     =   editInfo.showPecent;
        info.x          = (int)(obj.transform.localPosition.x / m_height);
        info.z          = (int)(obj.transform.localPosition.z / m_height);
        info.id         = editInfo.id;
        info.SerializeNo =++RMEBaseInfo.Index;
        editInfo.SerializeNo=info.SerializeNo;

        Debug.Log("BoxEditData info.percet:" + info.percet + ", info.x :" + info.x + ", info.z ：" + info.z);
        m_infoList.Add(info);
    }
     // 传送门存储数据
    static public void  TranspotEditData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }
        RMETranspotEdit editInfo   = obj.GetComponent<RMETranspotEdit>();
        if (null == editInfo)
        {
            return;
        }

        TranspotInfo info       = new TranspotInfo();

        info.x = (int)(obj.transform.localPosition.x / m_height);
        info.z = (int)(obj.transform.localPosition.z / m_height);
        info.id = editInfo.id;

        Debug.Log("TranspotEditData, info.x :" + info.x + ", info.z ：" + info.z);
        m_infoList.Add(info);
    }

    // 人物出生点存储数据
    static public void ActorBornEditData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }
        RMEActorBornPointEdit editInfo = obj.GetComponent<RMEActorBornPointEdit>();
        if (null == editInfo)
        {
            return;
        }
        if (  m_actorBornPointInfo.haveData == 1)
        {
            Debug.LogError("人物出生点 有多个 请检查。。。。。。重要");
        }


        m_actorBornPointInfo.x = (int)(obj.transform.localPosition.x / m_height);
        m_actorBornPointInfo.z = (int)(obj.transform.localPosition.z / m_height);
        m_actorBornPointInfo.haveData = 1;
        Debug.Log("ActorBornEditData, info.x :" + m_actorBornPointInfo.x + ", info.z ：" + m_actorBornPointInfo.z);
    }

    // 传送点存储数据
    static public void TranspotPointEditData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }
        RMETranspotPointEdit editInfo = obj.GetComponent<RMETranspotPointEdit>();
        if (null == editInfo)
        {
            return;
        }
        //editInfo.serNo = ++RMEBaseInfo.Index;
        TranspotPointInfo info = new TranspotPointInfo();

        info.x = (int)(obj.transform.localPosition.x / m_height);
        info.z = (int)(obj.transform.localPosition.z / m_height);
        info.serNo = editInfo.serNo;
        Debug.Log("ActorBornEditData, info.x :" + info.x + ", info.z ：" + info.z);
        m_infoList.Add(info);
    }

    // 机关存储数据
    static public void SchemeEditData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }
        RMESchemeEdit editInfo = obj.GetComponent<RMESchemeEdit>();
        if (null == editInfo)
        {
            return;
        }

        SchemeInfo info     = new SchemeInfo();

        info.x              = (int)(obj.transform.localPosition.x / m_height);
        info.z              = (int)(obj.transform.localPosition.z / m_height);

        info.npcId          = editInfo.npcId;
        info.percent        = editInfo.showPercent;
        info.SerializeNo    = ++RMEBaseInfo.Index;
        editInfo.SerializeNo = info.SerializeNo;
        info.beginEable     = editInfo.beginEable;
        info.beginState     = (int)editInfo.beginState;
        info.skillList      = new List<int>();
        info.desTranspotName = editInfo.desTranspotName;
        info.scaleX          = obj.transform.localScale.x;
        info.scaleZ         = obj.transform.localScale.z;
        foreach (int skillId in editInfo.skillIdList)
        {
            info.skillList.Add(skillId);
            Debug.Log("skillId:" + skillId);
        }
        Debug.Log("SchemeEditData, info.x :" + info.x + ", info.z ：" + info.z + ",info.npcId:" + info.npcId + ",info.percent:" + info.percent + ",info.beginEable:" + info.beginEable + ",info.beginState:" + info.beginState + ",info.desTranspotName:" + info.desTranspotName + ",info.scaleX:" + info.scaleX + ",info.scaleZ:" + info.scaleZ);
        
        m_infoList.Add(info);
    }

    // 适配点
    static public void PosEditData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }
        RMEAdjustPosInScene editInfo = obj.GetComponent<RMEAdjustPosInScene>();
        if (null == editInfo)
        {
            return;
        }

        m_height = editInfo.height;
    }

    // 阵型存储数据
    static public void ArrayEditData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }
        RMEArrayEdit editInfo  = obj.GetComponent<RMEArrayEdit>();
        if (null == editInfo)
        {
            return;
        }

        ArrayInfo info      = new ArrayInfo();

        info.x              = (int)(obj.transform.localPosition.x / m_height);
        info.z              = (int)(obj.transform.localPosition.z / m_height);

        info.list           = new Dictionary<GameObject, FormationInfo>(); 

        foreach (ArrayEditInfo item in editInfo.list)
        {
            if ( item.obj == null )
            {
                continue;
            }

            string objName =  item.obj.name;

            objName         = objName.Replace("LE-formation_", "");
            
            FormationInfo formationInfo = new FormationInfo();

            formationInfo.weight        = item.weight;
            formationInfo.list          = new Dictionary<int, List<MonsterInfo>>();
            formationInfo.SerNolist     = new Dictionary<int, int>();
			formationInfo.showBeginList = new Dictionary<int, int>();
            formationInfo.id            = int.Parse(objName);

            Debug.Log("formationInfo.id:" + formationInfo.id);
            // 遍历物件上的所有脚本
            foreach (Component comp in item.obj.GetComponentsInChildren(typeof(MonoBehaviour), true))
            {
                Type t = comp.GetType();
                if (null == t)
                {
                    continue;
                }

                // 怪物点
                if (t.FullName == "RMEMonsterEdit")
                {
                   RMEMonsterEdit monsterEditInfo = comp.gameObject.GetComponent<RMEMonsterEdit>();
                   if ( null == monsterEditInfo )
                   {
                        continue;
                   }

                   if (!formationInfo.list.ContainsKey(monsterEditInfo.tag))
                    {
                       List<MonsterInfo> temList    = new List<MonsterInfo>();

                      foreach(MonsterEditInfo tempInfo in  monsterEditInfo.monsterList)
                      {
                          MonsterInfo monsterInfo   = new MonsterInfo();
                          monsterInfo.id            = tempInfo.id;
                          monsterInfo.weight        = tempInfo.weight;
                          
                          temList.Add(monsterInfo);

                          Debug.Log("怪物点：" + monsterEditInfo.tag + ",monsterInfo.id :" + monsterInfo.id );
                      }
                      formationInfo.list.Add(monsterEditInfo.tag, temList);
                      monsterEditInfo.SerializeNo =  ++RMEBaseInfo.Index;
                      formationInfo.SerNolist.Add(monsterEditInfo.tag, monsterEditInfo.SerializeNo);
                      
						formationInfo.showBeginList.Add(monsterEditInfo.tag,monsterEditInfo.showBegin );


						Debug.Log("-------------------item.obj,:" + item.obj.name + ",formationInfo:" + formationInfo.id+","+formationInfo.showBegin);

                    }

                   Debug.Log("MonsterEdit:" + comp.gameObject.name + ",monsterEditInfo.SerializeNo:" + monsterEditInfo.SerializeNo);
                }

            }  
			info.list.Add(item.obj, formationInfo);
        }
        
        //Debug.Log("SchemeEditData, info.x :" + info.x + ", info.z ：" + info.z + ",info.npcId:" + info.npcId + ",info.percent:" + info.percent);
        m_infoList.Add(info);
    }

    // 房间标签存储数据
    static public void RoomEditData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }
        RMERoomEdit editInfo = obj.GetComponent<RMERoomEdit>();
        if (null == editInfo)
        {
            return;
        }
        if (m_roomInfo == null)
        {
            m_roomInfo = new RoomInfo();
            m_roomInfo.tagList = new List<int>();
        }

        m_roomInfo.tagList.Clear();

        foreach (int id in editInfo.idList)
        {
            m_roomInfo.tagList.Add(id);
            Debug.Log("RoomEditData info.tagID :" + id);
        }

        m_roomInfo.prefabName   = obj.name;
        m_roomInfo.terrainId    =  editInfo.terrainId;

        Debug.Log("RoomEditData info.tag :" + m_roomInfo.tagList + ", info.prefabName :" + m_roomInfo.prefabName + ", info.terrainName ：" + m_roomInfo.terrainId);

    }


    // 门存储数据
    static public void GateEditData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }
        RMEGateEdit editInfo = obj.GetComponent<RMEGateEdit>();
        if (null == editInfo)
        {
            return;
        }
        editInfo.m_serNo = ++RMEBaseInfo.Index;
        GateInfo info   = new GateInfo();

        info.objId      = editInfo.objId;
        info.beginState = editInfo.beginState;
        info.passType   = (int)editInfo.passtype;
        info.serNo      = editInfo.m_serNo;
        info.List       = new List<int>();
        switch (obj.name)
        {
            case "GateLocate_E":
                info.postion = (int)SM.ENGateDirection.enEAST;
                Debug.Log("GateLocate_E");
                break;
            case "GateLocate_N":
                info.postion = (int)SM.ENGateDirection.enNORTH;
                Debug.Log("GateLocate_N");
                break;
            case "GateLocate_S":
                info.postion = (int)SM.ENGateDirection.enSOUTH;
                Debug.Log("GateLocate_S");
                break;
            case "GateLocate_W":
                info.postion = (int)SM.ENGateDirection.enWEST;
                Debug.Log("GateLocate_W");
                break;
        }
        switch (editInfo.passtype)
        {
            case SM.ENGateOpenCondType.enKillSpecialMonster:
                {
                    for (int i = 0; i < editInfo.monsterList.Count;i++ )
                    {
                        GameObject objTemp         = editInfo.monsterList[i];
                        if (null != objTemp)
                        {
                            RMEMonsterEdit monsterInfo = objTemp.GetComponent<RMEMonsterEdit>();
                            if (null != monsterInfo)
                            {
                                info.List.Add(monsterInfo.SerializeNo);
                                Debug.Log("monsterInfo.SerializeNo:" + monsterInfo.SerializeNo);
                            }
                        }
                        
                    }
                    break;
                }
            case SM.ENGateOpenCondType.enOpenBox:
                {
                    for (int i = 0; i < editInfo.boxList.Count; i++)
                    {
                        GameObject objTemp          = editInfo.boxList[i];
                        if (null != objTemp)
                        {
                            RMEBoxEdit infoTemp = objTemp.GetComponent<RMEBoxEdit>();
                            if (null != infoTemp)
                            {
                                info.List.Add(infoTemp.SerializeNo);
                                Debug.Log("enOpenBox.SerializeNo:" + infoTemp.SerializeNo);
                            }
                        }
                        
                    }
                    break;
                }
            case SM.ENGateOpenCondType.enOpenSheme:
                {
                    for (int i = 0; i < editInfo.shemeList.Count; i++)
                    {
                        GameObject objTemp      = editInfo.shemeList[i];
                        if (null != objTemp)
                        {
                            RMESchemeEdit infoTemp = objTemp.GetComponent<RMESchemeEdit>();
                            if (null != infoTemp)
                            {
                                info.List.Add(infoTemp.SerializeNo);
                                Debug.Log("enOpenSheme.SerializeNo:" + infoTemp.SerializeNo);
                            }
                        }
                        
                    }
                    break;
                }
        }

        Debug.Log("GateEditData, info.objId :" + info.objId + ", info.beginState ：" + info.beginState);
        m_infoList.Add(info);
    }

    
     // npc死亡事件数据
    static public void NpcDeadData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }
        LE_Event_NpcDead editInfo = obj.GetComponent<LE_Event_NpcDead>();
        if (null == editInfo)
        {
            return;
        }
        editInfo.SerializeNo = ++RMEBaseInfo.Index;

       
        EventNpcDeadInfo info   = new EventNpcDeadInfo();
        info.eventId            = (int)editInfo.m_type;
        info.serNo              = editInfo.SerializeNo;

        for (int i = 0; i < editInfo.npcList.Count;i++ )
        {
            EventEventNpcDeadItemInfo itemInfo  = new EventEventNpcDeadItemInfo();
            itemInfo.npcType                    = (int)editInfo.npcList[i].npcType;
            
            if (!string.IsNullOrEmpty(editInfo.npcList[i].blackboardActorName))
            {
                itemInfo.blackboardActorName = editInfo.npcList[i].blackboardActorName;
            }

            Debug.Log("editInfo.npcList[i].blackboardActorName:" + editInfo.npcList[i].blackboardActorName);
            if (editInfo.npcList[i].npcType == DeadEvent.ENDeadActorType.enSpicalNpc)
            {
                if (null != editInfo.npcList[i].targetNpc)
                {
                    RMEMonsterEdit monsterInfo = editInfo.npcList[i].targetNpc.GetComponent<RMEMonsterEdit>();
                    if (null != monsterInfo)
                    {
                        itemInfo.targetNpcSerNo = monsterInfo.SerializeNo;
                    }
                    else
                    {
                        Debug.Log("obj:" + obj.name + ",预设无RMEMonsterEdit脚本，请检查 重要***********************");
                    }
                }
                else
                {
                    Debug.Log("obj:" + obj.name + ",预设无editInfo.targetNpc，请检查 重要***********************");
                }
            }

            info.list.Add(itemInfo);
        }
      
        

        // 行为列表
        foreach (GameObject item in editInfo.ActionList)
        {
            if ( null == item )
            {
                continue;
            }

             Component[] children    = item.GetComponents(typeof(LE_Action_Base));


             foreach (Component child in children)
             {
                 LE_Action_Base actionComp = (LE_Action_Base)child;

                 info.actionlist.Add(actionComp.SerializeNo);
             }
        }

        m_eventList.Add(info);
    }

     // 机关触发事件数据
    static public void Event_SchemeActionData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }
        LE_Event_SchemeTrap editInfo = obj.GetComponent<LE_Event_SchemeTrap>();
        if (null == editInfo)
        {
            return;
        }
        editInfo.SerializeNo = ++RMEBaseInfo.Index;

       
        EventSchemeTrapInfo info   = new EventSchemeTrapInfo();
        info.eventId            = (int)editInfo.m_type;
        info.serNo              = editInfo.SerializeNo;

        for (int i = 0; i < editInfo.schemeList.Count; i++)
        {
            EventSchemeActionItemInfo itemInfo = new EventSchemeActionItemInfo();
            itemInfo.actionType = (int)editInfo.schemeList[i].type;

             if (null != editInfo.schemeList[i].target)
              {
                    RMESchemeEdit schemeInfo = editInfo.schemeList[i].target.GetComponent<RMESchemeEdit>();
                    if (null != schemeInfo)
                    {
                        itemInfo.serNo = schemeInfo.SerializeNo;
                    } 
             }

            info.list.Add(itemInfo);
        }
        

        // 行为列表
        foreach (GameObject item in editInfo.ActionList)
        {
            if (null == item)
            {
                continue;
            }

            Component[] children = item.GetComponents(typeof(LE_Action_Base));


            foreach (Component child in children)
            {
                LE_Action_Base actionComp = (LE_Action_Base)child;

                info.actionlist.Add(actionComp.SerializeNo);
            }
        }

        m_eventList.Add(info);
    }

    // 怪物生命值低于 [参数1] 的百分比触发 事件数据
    static public void Event_HpData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }
        LE_Event_HP editInfo = obj.GetComponent<LE_Event_HP>();
        if (null == editInfo)
        {
            return;
        }
        editInfo.SerializeNo = ++RMEBaseInfo.Index;


        EventHpInfo info = new EventHpInfo();
        info.eventId    = (int)editInfo.m_type;
        info.serNo      = editInfo.SerializeNo;

        for (int i = 0; i < editInfo.npcList.Count; i++)
        {
            EventHpItemInfo itemInfo = new EventHpItemInfo();
            itemInfo.npcType = (int)editInfo.npcList[i].npcType;

            if (null != editInfo.npcList[i].targetNpc)
            {
                RMEMonsterEdit schemeInfo = editInfo.npcList[i].targetNpc.GetComponent<RMEMonsterEdit>();
                if (null != schemeInfo)

                {
                    itemInfo.targetNpcSerNo = schemeInfo.SerializeNo;
                }
            }
            itemInfo.hpValType  = (int)editInfo.npcList[i].hpValType;
            itemInfo.hpValue    = (int)editInfo.npcList[i].hpValue;
            if (!string.IsNullOrEmpty(editInfo.npcList[i].blackboardActorName) )
            {
                itemInfo.blackboardActorName = editInfo.npcList[i].blackboardActorName;
            }
            

            info.list.Add(itemInfo);
        }


        // 行为列表
        foreach (GameObject item in editInfo.ActionList)
        {
            if (null == item)
            {
                continue;
            }

            Component[] children = item.GetComponents(typeof(LE_Action_Base));


            foreach (Component child in children)
            {
                LE_Action_Base actionComp = (LE_Action_Base)child;

                info.actionlist.Add(actionComp.SerializeNo);
            }
        }

        m_eventList.Add(info);
    }

    // 关卡胜利失败行为数据
    static void ActionVictorToLevelInfoData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Action_VictorToLevel editInfo = obj.GetComponent<LE_Action_VictorToLevel>();
        if (null == editInfo)
        {
            return;
        }

        VictorToLevelInfo info  = new VictorToLevelInfo();

        int serNo               = ++RMEBaseInfo.Index;
        editInfo.SerializeNo    = serNo;

        info.actionId           = (int)editInfo.m_type;
        info.serNo              = serNo;
        info.victory            = (int)editInfo.victory;

        m_actionList.Add(info);
    }

    // 动作-使机关 锁定/失效/开启/关闭行为数据
    static void ActionSchemeTrapData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Action_SchemeTrap editInfo = obj.GetComponent<LE_Action_SchemeTrap>();
        if (null == editInfo)
        {
            return;
        }

        RMESchemeTrapActionInfo info = new RMESchemeTrapActionInfo();

        int serNo                   = ++RMEBaseInfo.Index;
        editInfo.SerializeNo = serNo;

        info.actionId = (int)editInfo.m_type;
        info.serNo = serNo;

        foreach (LE_Action_SchemeTrapInfo item in editInfo.List)
        {
            RMESchemeTrapActionItemInfo itemInfo    = new RMESchemeTrapActionItemInfo();
            itemInfo.actionType                     = (int)item.actionType;
            if (!string.IsNullOrEmpty(item.trapBlackboardStr))
            {
                itemInfo.trapBlackboardStr = item.trapBlackboardStr;
            }
           
            itemInfo.trapType                       = (int)item.trapType;

            if (item.schemeObj != null)
            {
                RMESchemeEdit schemeEditInfo        = item.schemeObj.GetComponent<RMESchemeEdit>();

                if ( null != schemeEditInfo )
                {
                    itemInfo.targetTrapSerNo        = schemeEditInfo.SerializeNo;
                }
            }

            info.list.Add(itemInfo);
        }

        m_actionList.Add(info);
    }

    // 动作-打开/关闭 闸门 行为数据
    static void ActionGateOpData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Action_Gate editInfo = obj.GetComponent<LE_Action_Gate>();
        if (null == editInfo)
        {
            return;
        }

        RMEGateOpActionInfo info = new RMEGateOpActionInfo();

        int serNo = ++RMEBaseInfo.Index;
        editInfo.SerializeNo = serNo;

        info.actionId = (int)editInfo.m_type;
        info.serNo = serNo;

        foreach (LE_Action_GateInfo item in editInfo.List)
        {
            RMEGateOpActionInfoItemInfo itemInfo = new RMEGateOpActionInfoItemInfo();
            itemInfo.actionType = (int)item.actionType;

            if (!string.IsNullOrEmpty(item.gateBlackboardStr))
            {
                itemInfo.gateBlackboardStr = item.gateBlackboardStr;
            }

            itemInfo.gateType = (int)item.gateType;

            if (item.targetGateObj != null)
            {
                RMEGateEdit gateEditInfo = item.targetGateObj.GetComponent<RMEGateEdit>();

                if (null != gateEditInfo)
                {
                    itemInfo.targetGateSerNo = gateEditInfo.m_serNo;
                }
            }

            info.list.Add(itemInfo);
        }

        m_actionList.Add(info);
    }

    // 动作-z掉落
    static void ActionDropItemData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Action_DropItem editInfo = obj.GetComponent<LE_Action_DropItem>();
        if (null == editInfo)
        {
            return;
        }

        RMEDropItemActionInfo info = new RMEDropItemActionInfo();

        int serNo               = ++RMEBaseInfo.Index;
        editInfo.SerializeNo    = serNo;

        info.actionId           = (int)editInfo.m_type;
        info.serNo              = serNo;

        foreach (LE_Action_DropItemInfo item in editInfo.List)
        {
            RMEDropItemActionInfoItemInfo itemInfo = new RMEDropItemActionInfoItemInfo();

            itemInfo.itemType   = (int)item.type;
            itemInfo.keyID      = item.keyId;

            info.list.Add(itemInfo);
        }

        m_actionList.Add(info);
    }

    // 动作-添加/移除 buff给角色
    static void ActionAddBuffData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Action_AddBuff editInfo = obj.GetComponent<LE_Action_AddBuff>();
        if (null == editInfo)
        {
            return;
        }

        RMEAddBuffActionInfo info = new RMEAddBuffActionInfo();

        int serNo = ++RMEBaseInfo.Index;
        editInfo.SerializeNo = serNo;

        info.actionId = (int)editInfo.m_type;
        info.serNo = serNo;

        foreach (LE_Action_AddBuffInfo item in editInfo.List)
        {
            RMEAddBuffActionInfoItemInfo itemInfo = new RMEAddBuffActionInfoItemInfo();

            itemInfo.actorType= (int)item.actorType;
            if (item.targetObj != null)
            {
                RMEMonsterEdit monsterEditInfo = item.targetObj.GetComponent<RMEMonsterEdit>();

                if (null != monsterEditInfo)
                {
                    itemInfo.actorID = monsterEditInfo.m_serNo;
                }
            }

            if ( !string.IsNullOrEmpty(item.buffBlackboardStr))
            {
                itemInfo.buffBlackboardStr = item.buffBlackboardStr;
            }

            itemInfo.buffOptType = (int)item.actionType;

            for (int i =0;i<item.buffList.Count;i++)
            {
                itemInfo.buffIDList.Add(item.buffList[i]);
            }
           
            info.list.Add(itemInfo);
        }

        m_actionList.Add(info);
    }
    // 刷怪行为数据
    static void ActionSpawnNpcInfoData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Action_SpawnNpc editInfo = obj.GetComponent<LE_Action_SpawnNpc>();
        if (null == editInfo)
        {
            return;
        }

        ActionSpawnNpcInfo spawnNpcInfo = new ActionSpawnNpcInfo();

        int serNo               = ++RMEBaseInfo.Index;
        editInfo.SerializeNo    = serNo;

        spawnNpcInfo.actionId   = (int)editInfo.m_type;
        spawnNpcInfo.serNo      = serNo;


        Debug.Log("ActionSpawnNpcInfoData:" + serNo + ",editorInfo.spawnNpcList.Count:" + editInfo.spawnNpcList.Count);
        for (int i = 0; i < editInfo.spawnNpcList.Count; i++)
        {
            SpawnNpcInfo itemInfo = editInfo.spawnNpcList[i];
            if (null != itemInfo.targetNpc)
            {
                ActionSpawnNpcItemInfo spawnNpcItemInfo = new ActionSpawnNpcItemInfo();
                RMEMonsterEdit monsterEditInfo = itemInfo.targetNpc.GetComponent<RMEMonsterEdit>();
                if (null != monsterEditInfo)
                {
                    spawnNpcItemInfo.npcSerNo = monsterEditInfo.SerializeNo;
                    Debug.Log("ActionSpawnNpcWriteData:" + monsterEditInfo.SerializeNo);
                }
                if (!string.IsNullOrEmpty(itemInfo.blackboardStr))
                {
                    spawnNpcItemInfo.blackboardStr = itemInfo.blackboardStr;
                }
               
                spawnNpcItemInfo.spawnPosition = (int)itemInfo.spawnPosition;

                switch (itemInfo.spawnPosition)
                {
                    case RefreshMonsterResult.ENSwapPosType.ObjectPositionOffset:
                        {
                            if (itemInfo.targetObject != null)
                            {
                                monsterEditInfo = itemInfo.targetObject.GetComponent<RMEMonsterEdit>();
                                if (null != monsterEditInfo)
                                {
                                    spawnNpcItemInfo.targetSerNo    = monsterEditInfo.SerializeNo;
                                    spawnNpcItemInfo.targetType     = (int)RefreshMonsterResult.ENTargetType.enMosnter;
                                }
//                                 
                                RMEBoxEdit boxEdit = itemInfo.targetObject.GetComponent<RMEBoxEdit>();
                                if (null != boxEdit)
                                {
                                    spawnNpcItemInfo.targetSerNo    = boxEdit.SerializeNo;
                                    spawnNpcItemInfo.targetType = (int)RefreshMonsterResult.ENTargetType.enBox;
                                }

                                RMESchemeEdit schemeEdit = itemInfo.targetObject.GetComponent<RMESchemeEdit>();
                                if (null != schemeEdit)
                                {
                                    spawnNpcItemInfo.targetSerNo    = schemeEdit.SerializeNo;
                                    spawnNpcItemInfo.targetType = (int)RefreshMonsterResult.ENTargetType.enScheme;
                                }

                                RMEGateEdit gateEdit = itemInfo.targetObject.GetComponent<RMEGateEdit>();
                                if (null != boxEdit)
                                {
                                    spawnNpcItemInfo.targetSerNo    = gateEdit.m_serNo;
                                    spawnNpcItemInfo.targetType = (int)RefreshMonsterResult.ENTargetType.enGate;
                                }
                            }
                            else
                            {
                                Debug.LogError("刷怪 到指定物件位置的 指定物件为空 请检查****************** 预设名称为" + itemInfo.targetNpc.name);
                            }

                            spawnNpcItemInfo.offset.x = itemInfo.offset.x;
                            spawnNpcItemInfo.offset.y = itemInfo.offset.y;
                            break;
                        }
                    case RefreshMonsterResult.ENSwapPosType.PlayerPositionOffset:
                    case RefreshMonsterResult.ENSwapPosType.CustomBlackboardStr:
                    case RefreshMonsterResult.ENSwapPosType.AffectedGate:
                    case RefreshMonsterResult.ENSwapPosType.TriggeringGate:
                    case RefreshMonsterResult.ENSwapPosType.TriggeringTrap:
                    case RefreshMonsterResult.ENSwapPosType.TargetPositionOfSkillBeingCast:
                    case RefreshMonsterResult.ENSwapPosType.TargetActorOfTriggeringActor:
                    case RefreshMonsterResult.ENSwapPosType.DeadActor:
                    case RefreshMonsterResult.ENSwapPosType.CastingActor:
                    case RefreshMonsterResult.ENSwapPosType.LastCreatedActor:
                    case RefreshMonsterResult.ENSwapPosType.TargetActorOfSkill:
                    case RefreshMonsterResult.ENSwapPosType.RebornActor:
                    case RefreshMonsterResult.ENSwapPosType.AttackedActor:
                    case RefreshMonsterResult.ENSwapPosType.TransportingActor:
                    case RefreshMonsterResult.ENSwapPosType.TriggerEventTrap:
                    case RefreshMonsterResult.ENSwapPosType.triggerPositionOffset:
                        {
                            spawnNpcItemInfo.offset.x = itemInfo.offset.x;
                            spawnNpcItemInfo.offset.y = itemInfo.offset.y;
                            break;
                        }
                }

                spawnNpcInfo.NpcList.Add(spawnNpcItemInfo);
            }
        }

        m_actionList.Add(spawnNpcInfo);
    }

    // and事件 序列号清0
    static public void EventAndDataClearSerNo(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Event_And editInfo = obj.GetComponent<LE_Event_And>();
        if (editInfo == null)
        {
            return;
        }

        // 序列号清零
        editInfo.SerializeNo = 0;
    }
    // OR事件 序列号清0
    static public void EventORdDataClearSerNo(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Event_Or editInfo = obj.GetComponent<LE_Event_Or>();
        if (editInfo == null)
        {
            return;
        }

        // 序列号清零
        editInfo.SerializeNo = 0;
    }

    // and事件信息数据
    static public void EventAndData(GameObject obj)
    {
        if (null == obj)
        {
            return ;
        }
     
        LE_Event_And editInfo   = obj.GetComponent<LE_Event_And>();
        if ( editInfo == null )
        {
            return;
        }

        // 如果没有生成序列号
        if ( editInfo.SerializeNo == 0  )
        {
            editInfo.SerializeNo = ++RMEBaseInfo.Index;// 生成序列号
        }

        EventsRelationship info = new EventsRelationship();
        info.eventId            = (int)EventManager.ENTriggerID.enAndEvent;
        info.serNo              = editInfo.SerializeNo;


        // 事件列表
        foreach (GameObject item in editInfo.EventList)
        {
            if (null == item)
            {
                continue;
            }
            Debug.Log("item:" + item.name);
            Component[] children = item.GetComponents(typeof(LE_Event_Base));
           
 
            // 遍历物件上的所有脚本
            foreach (Component child in children)
            {
                Debug.Log("child:" + child.name);
                LE_Event_Base leInfo = (LE_Event_Base)child;
                // 如果没有序列号则生成一个
                if ( leInfo.SerializeNo ==0 )
                {
                    leInfo.SerializeNo = ++RMEBaseInfo.Index;// 生成序列号
                }
               
                info.eventsList.Add(leInfo.SerializeNo);             
            }
    
        }
        


        // 行为列表
        foreach (GameObject item in editInfo.ActionList)
        {
            if (null == item)
            {
                continue;
            }

            Component[] children = item.GetComponents(typeof(LE_Action_Base));


            foreach (Component child in children)
            {
                LE_Action_Base actionComp = (LE_Action_Base)child;

                info.actionlist.Add(actionComp.SerializeNo);
            }
        }


        m_eventList.Add(info);
    }

    // Or事件信息数据
    static public void EventOrData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Event_Or editInfo = obj.GetComponent<LE_Event_Or>();
        if (editInfo == null)
        {
            return;
        }
        // 如果没有生成序列号
        if (editInfo.SerializeNo == 0)
        {
            editInfo.SerializeNo = ++RMEBaseInfo.Index;// 生成序列号
        }
        EventsRelationship info = new EventsRelationship();
        info.eventId            = (int)EventManager.ENTriggerID.enOrEvent;
        info.serNo              = editInfo.SerializeNo;

        // 事件列表
        foreach (GameObject item in editInfo.EventList)
        {
            if (null == item)
            {
                continue;
            }

            Component[] children = item.GetComponents(typeof(LE_Event_Base));


            // 遍历物件上的所有脚本
            foreach (Component child in children)
            {

                LE_Event_Base leInfo = (LE_Event_Base)child;

                // 如果没有序列号则生成一个
                if (leInfo.SerializeNo == 0)
                {
                    leInfo.SerializeNo = ++RMEBaseInfo.Index;// 生成序列号
                }
               
                info.eventsList.Add(leInfo.SerializeNo);
            }

        }
        // 行为列表
        foreach (GameObject item in editInfo.ActionList)
        {
            if (null == item)
            {
                continue;
            }

            Component[] children = item.GetComponents(typeof(LE_Action_Base));


            foreach (Component child in children)
            {
                LE_Action_Base actionComp = (LE_Action_Base)child;

                info.actionlist.Add(actionComp.SerializeNo);
            }
        }

        m_eventList.Add(info);

    }
    
      // 宝箱开启事件信息数据
    static public void EventBoxOpenData(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }

        LE_Event_BoxOpen editInfo = obj.GetComponent<LE_Event_BoxOpen>();
        if (editInfo == null)
        {
            return;
        }

        editInfo.SerializeNo    = ++RMEBaseInfo.Index;

        EventBoxOpenInfo info   = new EventBoxOpenInfo();
        info.eventId            = (int)EventManager.ENTriggerID.enOpenBox;
        info.serNo              = editInfo.SerializeNo;


        for (int i = 0; i < editInfo.boxList.Count;i++ )
        {
            if (null == editInfo.boxList[i])
            {
                continue;
            }

            RMEBoxEdit boxInfo = editInfo.boxList[i].GetComponent<RMEBoxEdit>();
            if (null == boxInfo)
            {
                continue;
            }

            info.list.Add(boxInfo.SerializeNo);   
        }


        // 行为列表
        foreach (GameObject item in editInfo.ActionList)
        {
            if (null == item)
            {
                continue;
            }

            Component[] children = item.GetComponents(typeof(LE_Action_Base));


            foreach (Component child in children)
            {
                LE_Action_Base actionComp = (LE_Action_Base)child;

                info.actionlist.Add(actionComp.SerializeNo);
            }
        }

        m_eventList.Add(info);

    }
    [@MenuItem("SceneEditor/LevelDataRead")]
    static void In()
    {
        Transform[] selection = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab | SelectionMode.Editable);

        if (selection.Length <= 0)
        {
            return;
        }

        if (null == selection[0])
        {
            return;
        }

        // 选择的物件
        GameObject obj = selection[0].gameObject;

        string fileName = "Assets/levleEditor/" + obj.name;
       using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
       {

           byte[] buff          = new byte[file.Length];

           file.Read(buff, 0, (int)file.Length);

           BinaryHelper helper  = new BinaryHelper(buff);

//            // 读取 数据
           Read(helper);
          
       }
    }

    static void Read(BinaryHelper helper)
    {
        string prefabName = helper.ReadString(); //  房间预设名称
        int terrainID = helper.ReadInt();  //  地形ID

        // 标签数量
        int roomTagCount = helper.ReadInt();
        // 标签ID
        for (int i = 0; i < roomTagCount; i++)
        {
            int id = helper.ReadInt();
            Debug.Log("roomInfo.tagId:" + id);

        }

        int haveData = helper.ReadInt();
        // 如果此房间 有人物出生点
        if (haveData == 1)
        {
            int actorBornPointInfox = helper.ReadInt();
            int actorBornPointInfoy = helper.ReadInt();

            Debug.Log("此房间的 人物出生点 actorBornPointInfox:" + actorBornPointInfox + ",actorBornPointInfoy：" + actorBornPointInfoy );
        }

        Debug.Log("roomInfo.prefabName:" + prefabName + ",roomInfo.terrainId:" + terrainID + ",roomTagCount:" + roomTagCount);
        
        // 数量
        int count           = helper.ReadInt();

        for (int i = 0; i < count; i++)
        {
            int type = helper.ReadInt();

            // 传送门
            if (type == (int)RMEBaseInfoEdit.Type.enTranspotDoor)
            {
                TranspotInfo transpotInfo = new TranspotInfo();
                transpotInfo.Read(helper);
            }

            // 宝箱
            else if (type == (int)RMEBaseInfoEdit.Type.enBox)
            {
                BoxInfo boxInfo = new BoxInfo();
                boxInfo.Read(helper);    
            }

             // 机关
            else if (type == (int)RMEBaseInfoEdit.Type.enScheme)
            {
                SchemeInfo schemeInfo = new SchemeInfo();
                schemeInfo.Read(helper);
            }

            // 阵型
            else if (type == (int)RMEBaseInfoEdit.Type.enArray)
            {
                ArrayInfo arrayInfo = new ArrayInfo();
                arrayInfo.Read(helper);
                
            }
            // 门 过关条件
            else if (type == (int)RMEBaseInfoEdit.Type.enGate)
            {
                GateInfo gateInfo = new GateInfo();
                gateInfo.Read(helper);
            }
            // 传送点
            else if (type == (int)RMEBaseInfoEdit.Type.enTranspotPoint)
            {
                TranspotPointInfo info = new TranspotPointInfo();
                info.Read(helper);
            }    
        }

        string temp = helper.ReadString();

        Debug.Log("行为信息为：" + temp);

        temp        = helper.ReadString();

        Debug.Log("事件信息为：" + temp);

        

    }

    [@MenuItem("SceneEditor/LevelRouteDataWrite")]
    public static void LevelRouteDataOut()
    {
        // 获得当前选中物件
        Transform[] selection = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab | SelectionMode.Editable);

        if (selection.Length <= 0)
        {
            return;
        }

        if (null == selection[0])
        {
            return;
        }

        // 选择的物件
        GameObject obj = selection[0].gameObject;

        RMELevelRouteHead head = obj.GetComponent<RMELevelRouteHead>();

        if (null == head)
        {
            return;
        }
        string name = obj.name;
        string idName =  name.Replace("LE-levelRoute_", "");

        if ( !name.Contains("LE-levelRoute_") )
        {
            Debug.LogError(obj.name + "命名 不符命名规则 未含有LE-levelRoute_");
        }
        else
        {
            if (head.id != int.Parse(idName))
            {
                Debug.LogError(obj.name + "中ID 与头房间 名称中的ID 不符");
            }
        }
       
       

        string fileName = "Assets/levleEditor/" + obj.name;

        using (FileStream targetFile = new FileStream(fileName, FileMode.Create))
        {
            BinaryHelper helper = new BinaryHelper();

            helper.Write(head.id);

            WriteLevelRouteData(head.head, helper);

            byte[] buff = helper.GetBytes();

            targetFile.Write(buff, 0, buff.Length);
        }
    }

    static void WriteLevelRouteData(GameObject obj, BinaryHelper helper)
    {
        RMELevelRouteItemEdit edit = obj.GetComponent<RMELevelRouteItemEdit>();
        if (null == edit)
        {
            Debug.Log("obj:" + obj.name + ",此预设无LevelRouteItemEdit 脚本 请查看是否是正确*********************");
            return;
        }

        if (edit.showPercent == 0)
        {
            Debug.LogError(obj.name + "中showPercent 为0  请检查");
        }

        if (edit.roomlist.Count == 0)
        {
            Debug.LogError(obj.name + "中roomlist.Count 为0  请检查");
        }

        helper.Write(edit.showPercent);
        helper.Write(edit.roomlist.Count);

        int i = 0;
        foreach (LevelRouteRoomInfo info in edit.roomlist)
        {
            helper.Write(info.RoomId);
            helper.Write(info.Roomweight);
            Debug.Log("info.RoomId:" + info.RoomId + ",info.Roomweight:" + info.Roomweight);

            if (info.RoomId == 0)
            {
                Debug.LogError(obj.name + "中roomlist[" + i + "] 的RoomId为0  请检查");
            }

            if (info.Roomweight == 0)
            {
                Debug.LogError(obj.name + "中roomlist[" + i + "] 的Roomweight为0  请检查");
            }
            i++;
        }

        // 北
        helper.Write((int)RMELevelRouteItemEdit.Direction.enNorth);
        if (edit.NorthBridge.nextRouteItem !=null)
        {
            // 有数据
            helper.Write(1);
            helper.Write(edit.NorthBridge.ObjID);
            helper.Write(edit.NorthBridge.Length);

            WriteLevelRouteData(edit.NorthBridge.nextRouteItem,helper);

            if (edit.NorthBridge.ObjID == 0 )
            {
                Debug.LogError(obj.name + "中NorthBridge 的ObjID为0  请检查");
            }
            if (edit.NorthBridge.Length == 0)
            {
                Debug.LogError(obj.name + "中NorthBridge 的Length为0  请检查");
            }
        }
        else
        {
            // 无数据
            helper.Write(0);
        }

        // 东
        helper.Write((int)RMELevelRouteItemEdit.Direction.enEast);
        if (edit.EastBridge.nextRouteItem != null)
        {
            // 有数据
            helper.Write(1);
            helper.Write(edit.EastBridge.ObjID);
            helper.Write(edit.EastBridge.Length);

            WriteLevelRouteData(edit.EastBridge.nextRouteItem, helper);

            if (edit.EastBridge.ObjID == 0)
            {
                Debug.LogError(obj.name + "中NorthBridge 的ObjID为0  请检查");
            }
            if (edit.EastBridge.Length == 0)
            {
                Debug.LogError(obj.name + "中NorthBridge 的Length为0  请检查");
            }
        }
        else
        {
            // 无数据
            helper.Write(0);
        }

        // 南
        helper.Write((int)RMELevelRouteItemEdit.Direction.enSouth);
        if (edit.SouthBridge.nextRouteItem != null)
        {
            // 有数据
            helper.Write(1);
            helper.Write(edit.SouthBridge.ObjID);
            helper.Write(edit.SouthBridge.Length);

            WriteLevelRouteData(edit.SouthBridge.nextRouteItem, helper);

            if (edit.SouthBridge.ObjID == 0)
            {
                Debug.LogError(obj.name + "中NorthBridge 的ObjID为0  请检查");
            }
            if (edit.SouthBridge.Length == 0)
            {
                Debug.LogError(obj.name + "中NorthBridge 的Length为0  请检查");
            }
        }
        else
        {
            // 无数据
            helper.Write(0);
        }

        // 西
        helper.Write((int)RMELevelRouteItemEdit.Direction.enWest);
        if (edit.WestBridge.nextRouteItem != null)
        {
            // 有数据
            helper.Write(1);
            helper.Write(edit.WestBridge.ObjID);
            helper.Write(edit.WestBridge.Length);

            WriteLevelRouteData(edit.WestBridge.nextRouteItem, helper);

            if (edit.WestBridge.ObjID == 0)
            {
                Debug.LogError(obj.name + "中NorthBridge 的ObjID为0  请检查");
            }
            if (edit.WestBridge.Length == 0)
            {
                Debug.LogError(obj.name + "中NorthBridge 的Length为0  请检查");
            }
        }
        else
        {
            // 无数据
            helper.Write(0);
        }

   
    }

    [@MenuItem("SceneEditor/LevelRouteDataRead")]
    static void LevelRouteDataRead()
    {
        Transform[] selection = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab | SelectionMode.Editable);

        if (selection.Length <= 0)
        {
            return;
        }

        if (null == selection[0])
        {
            return;
        }

        // 选择的物件
        GameObject obj = selection[0].gameObject;

        string fileName = "Assets/levleEditor/" + obj.name;
        using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {

            byte[] buff = new byte[file.Length];

            file.Read(buff, 0, (int)file.Length);

            BinaryHelper helper = new BinaryHelper(buff);

            int id = helper.ReadInt();
            Debug.Log("路线ID：" + id);
            //            // 读取 数据
            ReadRoute(helper);

        }
    }

    static void ReadRoute(BinaryHelper helper)
    {
        float percent   = helper.ReadFloat();
        int roomCount   = helper.ReadInt();

        Debug.Log(",percent:" + percent + ",roomCount:" + roomCount);

        for (int i = 0; i < roomCount;i++ )
        {
            int roomId          = helper.ReadInt();
            int roomWeight      = helper.ReadInt();
            Debug.Log("roomId:" + roomId + ",roomWeight:" + roomWeight);
        }

        // 四个方向
        ReadDirection(helper);
        ReadDirection(helper);
        ReadDirection(helper);
        ReadDirection(helper);
    }

    static void ReadDirection(BinaryHelper helper)
    {
        int direction   = helper.ReadInt();
        int haveData    = helper.ReadInt();
        string temp = "";
        switch (direction)
        {
            case (int)RMELevelRouteItemEdit.Direction.enNorth:
                {
                    temp = "enNorth";
                    break;
                }
            case (int)RMELevelRouteItemEdit.Direction.enSouth:
                {
                    temp = "enSouth";
                    break;
                }
            case (int)RMELevelRouteItemEdit.Direction.enEast:
                {
                    temp = "enEast";
                    break;
                }
            case (int)RMELevelRouteItemEdit.Direction.enWest:
                {
                    temp = "enWest";
                    break;
                }
        }
        Debug.Log("ReadDirection direction:" + temp + ",haveData:" + haveData);

        // 如果有数据
        if (haveData == 1)
        {
            int ObjID           = helper.ReadInt();
            int Length          = helper.ReadInt();
            Debug.Log("ReadDirection ObjID:" + ObjID + ",Length:" + Length );
            ReadRoute(helper);
        }
    }

    public static void BuildLevelRouteData(GameObject obj, MonoBehaviour mono)
    {
        if ( obj == null )
        {
            Debug.LogError("此路先无起始房间 请检查。。。。。");
            return;
        }
        RMELevelRouteHead headMono  = mono as RMELevelRouteHead;
        m_floorId                   = headMono.viewId;

        Debug.Log("...........m_floorId:" + m_floorId);
        string temp = "{";
        temp = temp + "\"" + SM.RandomRoomLevel.sfloor + "\":{";
        temp = temp + "\"" + SM.RandomRoomLevel.smonsters + "\":[],";
        temp = temp + "\"" + SM.RandomRoomLevel.slevels + "\":[{";
        temp = temp + "\"" + SM.RandomRoomLevel.slevelId + "\":1,";

        temp = temp + "\"" + SM.RandomRoomLevel.sstartingRoom + "\":";
        BuildLevelRouteRoomData(obj, temp, out temp);
        temp = temp + "}";
        temp = temp + "]";
        temp = temp + "}";
        temp = temp + "}";

        Debug.Log(temp);

        if (null == m_sceneManger)
        {
            m_sceneManger = new SM.RandomRoomLevel();
        }
        LoadRandMapTableData();

        m_sceneManger.ViewSceneOnEditor(temp, mono);

        //SetMonsters();
    }

    // 在预览地图上种怪物
    public static void SetMonsters(GameObject obj)
    {
        RMELevelRouteHead head  = obj.GetComponent<RMELevelRouteHead>();
        m_floorId               = head.viewId;
       
        if (m_dungeonMonsterTable == null)
        {
            m_dungeonMonsterTable = new DungeonMonsterTable();
        }
        TextAsset objText = GameData.LoadConfig<TextAsset>("DungeonMonsterTable");

        m_dungeonMonsterTable.Load(objText.bytes);

         if (null == m_sceneManger)
        {
            m_sceneManger = new SM.RandomRoomLevel();
        }
        SM.SceneRoomInfoTree sceneRoomInfoTree = m_sceneManger.LookupLevelRoomInfo(1);

        ForeachSceneRoomInfoTree(sceneRoomInfoTree); 
    }

    static void ForeachSceneRoomInfoTree(SM.SceneRoomInfoTree sceneRoomInfoTree)
    {
        if (null == sceneRoomInfoTree)
         {
             Debug.Log("ForeachSceneRoomInfoTree  null == sceneRoomInfoTree ");
             return;
         }
        string fileName = sFileDir + sceneRoomInfoTree.m_roomName;

        Debug.Log("sceneRoomInfoTree.m_RoomPrefabName...................:" + fileName + ",m_roomObjID:" + sceneRoomInfoTree.m_roomObjID + ",m_RoomPrefabName:" + sceneRoomInfoTree.m_RoomPrefabName + ",m_roomObj:" + sceneRoomInfoTree.m_roomObj.name);
        List<ArrayInfo> arrayList = new List<ArrayInfo>();
        using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {

            byte[] buff = new byte[file.Length];

            file.Read(buff, 0, (int)file.Length);

            BinaryHelper helper = new BinaryHelper(buff);

            //            // 读取 数据
            string prefabName = helper.ReadString(); //  房间预设名称
            int terrainID = helper.ReadInt();  //  地形ID
            Debug.Log("prefabName:" + prefabName + ",terrainID:" + terrainID);

            // 标签数量
            int roomTagCount = helper.ReadInt();
            // 标签ID
            for (int j = 0; j < roomTagCount; j++)
            {
                int id = helper.ReadInt();
                Debug.Log("roomInfo.tagId:" + id);

            }

            int haveData = helper.ReadInt();
            // 如果此房间 有人物出生点
            if (haveData == 1)
            {
                int actorBornPointInfox = helper.ReadInt();
                int actorBornPointInfoy = helper.ReadInt();

                Debug.Log("此房间的 人物出生点 actorBornPointInfox:" + actorBornPointInfox + ",actorBornPointInfoy：" + actorBornPointInfoy);
            }

            Debug.Log("roomInfo.prefabName:" + prefabName + ",roomInfo.terrainId:" + terrainID + ",roomTagCount:" + roomTagCount);

            // 数量
            int count = helper.ReadInt();

            for (int j = 0; j < count; j++)
            {
                int type = helper.ReadInt();

                // 传送门
                if (type == (int)RMEBaseInfoEdit.Type.enTranspotDoor)
                {
                    TranspotInfo transpotInfo = new TranspotInfo();
                    transpotInfo.Read(helper);
                }

                // 宝箱
                else if (type == (int)RMEBaseInfoEdit.Type.enBox)
                {
                    BoxInfo boxInfo = new BoxInfo();
                    boxInfo.Read(helper);

                    // 宝箱的预览
                    GameObject obj = AssetDatabase.LoadMainAssetAtPath("Assets/levleEditor/Resource/Mark/LE-treasureMark.prefab") as GameObject;
                    GameObject effect = GameObject.Instantiate(obj) as GameObject;
                    Debug.Log("obj:" + effect.name);

                    effect.transform.parent = sceneRoomInfoTree.m_locatorObj.transform;
                    effect.transform.LocalPositionX(boxInfo.x * m_height);
                    effect.transform.LocalPositionZ(boxInfo.z * m_height);

                    int npcId = boxInfo.id;

                    Debug.Log("预览的 宝箱npcId：" + npcId);
                }

                 // 机关
                else if (type == (int)RMEBaseInfoEdit.Type.enScheme)
                {
                    SchemeInfo schemeInfo = new SchemeInfo();
                    schemeInfo.Read(helper);
                }

                // 阵型
                else if (type == (int)RMEBaseInfoEdit.Type.enArray)
                {
                    ArrayInfo arrayInfo = new ArrayInfo();
                    arrayInfo.Read(helper);

                    arrayList.Add(arrayInfo);

                }
                // 门 过关条件
                else if (type == (int)RMEBaseInfoEdit.Type.enGate)
                {

                    int postion = helper.ReadInt();
                    int gateObjId = helper.ReadInt();
                    int gateBeginState = helper.ReadInt();
                    int gatePassType = helper.ReadInt();
                    int gateserNo = helper.ReadInt();
                    int gateListCount = helper.ReadInt();
                    for (int m = 0; m < gateListCount; m++)
                    {
                        int no = helper.ReadInt();
                        Debug.Log("no.id:" + no);
                    }

                }
                // 传送点
                else if (type == (int)RMEBaseInfoEdit.Type.enTranspotPoint)
                {
                    TranspotPointInfo info = new TranspotPointInfo();
                    info.Read(helper);
                }
            }
        }



        foreach (ArrayInfo itemInfo in arrayList)
        {

            int index = UnityEngine.Random.Range(1, arrayList.Count);
            int i = 0;

            // 先随机阵型
            foreach (FormationInfo item in itemInfo.dataList)
            {
                Debug.Log("随机的阵型ID 为 formationId：" + item.id);
                i++;
                if (index == i)
                {
                    int formationId = item.id;

                    Debug.Log("随机的阵型ID 为 formationId：" + formationId );//LE-formation_1
                    Dictionary<int, int> mosterList = new Dictionary<int, int>(); // 阵型中的 点索引 、怪物ID
                    // 再随机怪物
                    foreach (KeyValuePair<int, List<MonsterInfo>> monsterItemInfo in item.list)
                    {
                        i = 0;
                        index = UnityEngine.Random.Range(1, monsterItemInfo.Value.Count);

                        
                        for (int j = 0; j < monsterItemInfo.Value.Count; j++)
                        {
                            i++;
                            if (index == i)
                            {
                                mosterList.Add(monsterItemInfo.Key, monsterItemInfo.Value[j].id);

                                Debug.Log("随机的怪物ID为：" + monsterItemInfo.Value[j].id + ",其怪物点索引为：" + monsterItemInfo.Key);
                            }
                        }
                    }


                    // 根据 信息 生成怪物
                   // Resources.Load("levleEditor/Resource/formaiton/" + "LE - formation_1");
                    GameObject obj = AssetDatabase.LoadMainAssetAtPath("Assets/levleEditor/Resource/formaiton/" + "LE-formation_"+formationId+".prefab") as GameObject;
                    GameObject effect = GameObject.Instantiate(obj) as GameObject;
                    Debug.Log("obj:" + effect.name);

                    effect.transform.parent     = sceneRoomInfoTree.m_locatorObj.transform;
                    effect.transform.LocalPositionX(itemInfo.x * m_height);
                    effect.transform.LocalPositionZ(itemInfo.z * m_height);

                    // 获取一个物件及其子物件上的所有的 脚本组件
                    Component[] children = effect.GetComponentsInChildren(typeof(RMEMonsterEdit), true);

                    foreach (Component child in children)
                    {
                        RMEMonsterEdit actionComp = (RMEMonsterEdit)child;

                        if (mosterList.ContainsKey(actionComp.tag))
                        {
                            int  monsterId  = mosterList[actionComp.tag];
                           
                            int npcId       = m_dungeonMonsterTable.LookUp(m_floorId, monsterId);

                            Debug.Log("怪物点：" + actionComp.tag + ",m_floorId:" + m_floorId + ",monsterId:" + monsterId + ",npcId:" + npcId + ",formationId:" + formationId);

                            NPCInfo npcInfo = GameTable.NPCInfoTableAsset.Lookup(npcId);
                            if (npcInfo == null )
                            {
                                Debug.LogError("npcInfo为空npcId：" + npcId);
                                continue;
                            }

                            ModelInfo modelInfo = GameTable.ModelInfoTableAsset.Lookup(npcInfo.ModelId);
                            if (null == modelInfo)
                            {
                                Debug.LogError("modelInfo为空npcId：" + npcId + ",npcInfo.ModelId:" + npcInfo.ModelId);
                                continue;
                            }
                            Debug.Log("modelInfo.ModelName...............：" + modelInfo.ModelFile);
                            GameObject modelObj = AssetDatabase.LoadMainAssetAtPath("Assets/Resources/Prefabs/" + modelInfo.ModelFile + ".prefab") as GameObject;
                            GameObject model                = GameObject.Instantiate(modelObj) as GameObject;
                            model.transform.parent          = child.gameObject.transform;
                            model.transform.localPosition   = Vector3.zero;
                            model.SetActive(true);

                            Transform poly = child.gameObject.transform.Find("polySurface3");
                            if (null != poly)
                            {
                                poly.gameObject.SetActive(false);
                            }
                        }
                    }
                    
                    Debug.Log("itemInfo.x:" + itemInfo.x + ",itemInfo.z" + itemInfo.z);
                    break;
                }
            }

        }
        foreach (SM.SceneRoomInfoTree itemInfo in sceneRoomInfoTree.m_nextActiveRoom)
        {
            ForeachSceneRoomInfoTree(itemInfo);
        }
    }

    // 加载 生成地图 中用到的表数据
    static public void LoadRandMapTableData()
    {
        GameTable.LoadRandMapTableData();

        if (null != GameTable.SceneRoomTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("SceneRoomTable");

            GameTable.SceneRoomTableAsset.Load(obj.bytes);
        }

        if (null != GameTable.SceneBridgeTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("SceneBridgeTable");

            GameTable.SceneBridgeTableAsset.Load(obj.bytes);
        }
        if (null != GameTable.SceneGateTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("SceneGateTable");
            GameTable.SceneGateTableAsset.Load(obj.bytes);
        }
        if (null != GameTable.SceneTeleportTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("SceneTeleportTable");
            GameTable.SceneTeleportTableAsset.Load(obj.bytes);
        }

        if (null != GameTable.NPCInfoTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("NPCInfoTable");
            GameTable.NPCInfoTableAsset.Load(obj.bytes);
        }
  

        if (null != GameTable.ModelInfoTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("ModelInfoTable");
            GameTable.ModelInfoTableAsset.Load(obj.bytes);
        }

       
    }
    static void BuildLevelRouteRoomData(GameObject obj,string src,out string des)
    {
        des = src ;
        RMELevelRouteItemEdit edit = obj.GetComponent<RMELevelRouteItemEdit>();
        if (null == edit)
        {
            Debug.Log("obj:" + obj.name + ",此预设无LevelRouteItemEdit 脚本 请查看是否是正确*********************");
            des = src + "\"\"";
            return;
        }

        TextAsset asset = GameData.LoadConfig<TextAsset>("RoomInfoTable");
        RoomEditInfoTable roomInfoTable = new RoomEditInfoTable();
        roomInfoTable.Load(asset.bytes);


         int index  = UnityEngine.Random.Range(1, edit.roomlist.Count);
         int i      = 0;
         int roomId = 0;
        

         foreach (LevelRouteRoomInfo info in edit.roomlist)
         {
             i++;
             if (i == index)
             {
                 roomId = info.RoomId;
             }
             
         }

        RoomEditInfo roomEditInfo =  roomInfoTable.Lookup(roomId);

        if ( null == roomEditInfo )
        {
            des = src + "\"\"";
            Debug.LogError("随机房间 roomId：" + roomId + ",RoomEditInfo表中无此数据，请检查 roomInfoTable");
            return;
        }

        int terrainID       = 0;
        string fileName     = sFileDir + roomEditInfo.name;
        int southSeroNo     = 0;
        int eastSeroNo      = 0;
        int northSeroNo     = 0;
        int westSeroNo      = 0;

        Debug.Log("支持数据：" + fileName);
        int southGateObjId = 0;
        int eastGateObjId = 0;
        int northGateObjId = 0;
        int westGateObjId = 0;
        using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {

            byte[] buff = new byte[file.Length];

            file.Read(buff, 0, (int)file.Length);

            BinaryHelper helper = new BinaryHelper(buff);

            //            // 读取 数据
            string prefabName   = helper.ReadString(); //  房间预设名称
            terrainID           = helper.ReadInt();  //  地形ID
            Debug.Log("prefabName:" + prefabName + ",terrainID:" + terrainID);

            // 标签数量
            int roomTagCount = helper.ReadInt();
            // 标签ID
            for (int j = 0; j < roomTagCount; j++)
            {
                int id = helper.ReadInt();
                Debug.Log("roomInfo.tagId:" + id);

            }

            int haveData = helper.ReadInt();
            // 如果此房间 有人物出生点
            if (haveData == 1)
            {
                int actorBornPointInfox = helper.ReadInt();
                int actorBornPointInfoy = helper.ReadInt();

                Debug.Log("此房间的 人物出生点 actorBornPointInfox:" + actorBornPointInfox + ",actorBornPointInfoy：" + actorBornPointInfoy);
            }

            Debug.Log("roomInfo.prefabName:" + prefabName + ",roomInfo.terrainId:" + terrainID + ",roomTagCount:" + roomTagCount);

            // 数量
            int count = helper.ReadInt();

            for (int j = 0; j < count; j++)
            {
                int type = helper.ReadInt();

                // 传送门
                if (type == (int)RMEBaseInfoEdit.Type.enTranspotDoor)
                {
                    TranspotInfo transpotInfo = new TranspotInfo();
                    transpotInfo.Read(helper);
                }

                // 宝箱
                else if (type == (int)RMEBaseInfoEdit.Type.enBox)
                {
                    BoxInfo boxInfo = new BoxInfo();
                    boxInfo.Read(helper);
                }

                 // 机关
                else if (type == (int)RMEBaseInfoEdit.Type.enScheme)
                {
                    SchemeInfo schemeInfo = new SchemeInfo();
                    schemeInfo.Read(helper);
                }

                // 阵型
                else if (type == (int)RMEBaseInfoEdit.Type.enArray)
                {
                    ArrayInfo arrayInfo = new ArrayInfo();
                    arrayInfo.Read(helper);

                }
                // 门 过关条件
                else if (type == (int)RMEBaseInfoEdit.Type.enGate)
                {
        
                    int postion         = helper.ReadInt();
                    int gateObjId       = helper.ReadInt();
                    int gateBeginState  = helper.ReadInt();
                    int gatePassType    = helper.ReadInt();
                    int gateserNo       = helper.ReadInt();
                    int gateListCount   = helper.ReadInt();
                    for (int m = 0; m < gateListCount; m++)
                    {
                        int no = helper.ReadInt();
                        Debug.Log("no.id:" + no);
                    }
                    switch (postion)
                    {
                        case (int)SM.ENGateDirection.enEAST:
                            eastSeroNo = gateserNo;
                            eastGateObjId = gateObjId;
                           
                            break;
                        case (int)SM.ENGateDirection.enNORTH:
                            northGateObjId = gateObjId;
                            northSeroNo = gateserNo;
                            break;
                        case (int)SM.ENGateDirection.enSOUTH:
                            southSeroNo = gateserNo;
                            southGateObjId = gateObjId;
                            break;
                        case (int)SM.ENGateDirection.enWEST:
                            westSeroNo = gateserNo;
                            westGateObjId = gateObjId;
                            break;
                    }
                }
                // 传送点
                else if (type == (int)RMEBaseInfoEdit.Type.enTranspotPoint)
                {
                    TranspotPointInfo info = new TranspotPointInfo();
                    info.Read(helper);
                }
            }
        }

        des = des + "{";

        des = des + "\"" + SM.SceneRoomInfoTree.sroomObjId + "\":" + terrainID;

        des = des + ",";

        des = des + "\"" + SM.SceneRoomInfoTree.sroomId+ "\":" + (++m_roomSerNo);

        des = des + ",";
        des = des + "\"" + SM.SceneRoomInfoTree.sroonName + "\":"  + "\""+roomEditInfo.name+"\"";

        des = des + ",";

        des = des + "\"" + SM.SceneRoomInfoTree.sgates + "\":[";

        //////////////////////////////////////////////////////////////////////////
        // 北
        des = des + "{";

        des = des + "\"" + SM.SceneRoomInfoTree.sisGateOpen + "\":1,";

        des = des + "\"" + SM.SceneRoomInfoTree.sgateObjId + "\":"+northGateObjId+",";

        des = des + "\"" + SM.SceneRoomInfoTree.sgateId + "\":" + northSeroNo;

        des = des + ",";
        
        if (edit.NorthBridge.nextRouteItem != null)
        {

            des = des + "\"" + SM.SceneRoomInfoTree.sbridge + "\":{";

            des = des + "\"" + SM.SceneRoomInfoTree.sbridgeObjId + "\":" + edit.NorthBridge.ObjID;

            des = des + ",";

            des = des + "\"" + SM.SceneRoomInfoTree.sbridgeCenterLength + "\":" + edit.NorthBridge.Length;

            des = des + ",";

            des = des + "\"" + SM.SceneRoomInfoTree.stargetRoom + "\":";

            BuildLevelRouteRoomData(edit.NorthBridge.nextRouteItem, des, out des);

            des = des + "},";
           
        }

        des = des + "\"" + SM.SceneRoomInfoTree.sgateOpenCondId + "\":1";

        des = des + "}";

        des = des + ",";

        //////////////////////////////////////////////////////////////////////////
        // 东
        des = des + "{";

        des = des + "\"" + SM.SceneRoomInfoTree.sisGateOpen + "\":1,";

        des = des + "\"" + SM.SceneRoomInfoTree.sgateObjId + "\":" + eastGateObjId + ",";

        des = des + "\"" + SM.SceneRoomInfoTree.sgateId + "\":" + eastSeroNo;

        des = des + ",";

        if (edit.EastBridge.nextRouteItem != null)
        {
            des = des + "\"" + SM.SceneRoomInfoTree.sbridge + "\":{";

            des = des + "\"" + SM.SceneRoomInfoTree.sbridgeObjId + "\":" + edit.EastBridge.ObjID;

            des = des + ",";

            des = des + "\"" + SM.SceneRoomInfoTree.sbridgeCenterLength + "\":" + edit.EastBridge.Length;

            des = des + ",";

            des = des + "\"" + SM.SceneRoomInfoTree.stargetRoom + "\":";


            BuildLevelRouteRoomData(edit.EastBridge.nextRouteItem, des, out des);

            des = des + "},";

        }

        des = des + "\"" + SM.SceneRoomInfoTree.sgateOpenCondId + "\":1";

        des = des + "}";

        des = des + ",";
        //////////////////////////////////////////////////////////////////////////
        // 南
        des = des + "{";

        des = des + "\"" + SM.SceneRoomInfoTree.sisGateOpen + "\":1,";

        des = des + "\"" + SM.SceneRoomInfoTree.sgateObjId + "\":" + southGateObjId + ",";

        des = des + "\"" + SM.SceneRoomInfoTree.sgateId + "\":" + southSeroNo;

        des = des + ",";
      

        if (edit.SouthBridge.nextRouteItem != null)
        {
            des = des + "\"" + SM.SceneRoomInfoTree.sbridge + "\":{";

            des = des + "\"" + SM.SceneRoomInfoTree.sbridgeObjId + "\":" + edit.SouthBridge.ObjID;

            des = des + ",";

            des = des + "\"" + SM.SceneRoomInfoTree.sbridgeCenterLength + "\":" + edit.SouthBridge.Length;

            des = des + ",";

            des = des + "\"" + SM.SceneRoomInfoTree.stargetRoom + "\":";

            BuildLevelRouteRoomData(edit.SouthBridge.nextRouteItem, des, out des);

            des = des + "},";

        }

        des = des + "\"" + SM.SceneRoomInfoTree.sgateOpenCondId + "\":1";

        des = des + "}";

        des = des + ",";

        //////////////////////////////////////////////////////////////////////////
        // 西
        des = des + "{";

        des = des + "\"" + SM.SceneRoomInfoTree.sisGateOpen + "\":1,";

        des = des + "\"" + SM.SceneRoomInfoTree.sgateObjId + "\":" + westGateObjId + ",";

        des = des + "\"" + SM.SceneRoomInfoTree.sgateId + "\":" + westSeroNo;

        des = des + ",";

        if (edit.WestBridge.nextRouteItem != null)
        {
            des = des + "\"" + SM.SceneRoomInfoTree.sbridge + "\":{";

            des = des + "\"" + SM.SceneRoomInfoTree.sbridgeObjId + "\":" + edit.WestBridge.ObjID;

            des = des + ",";

            des = des + "\"" + SM.SceneRoomInfoTree.sbridgeCenterLength + "\":" + edit.WestBridge.Length;

            des = des + ",";

            des = des + "\"" + SM.SceneRoomInfoTree.stargetRoom + "\":";


            BuildLevelRouteRoomData(edit.WestBridge.nextRouteItem, des, out des);

            des = des + "},";

        }

        des = des + "\"" + SM.SceneRoomInfoTree.sgateOpenCondId + "\":1";

        des = des + "}";


        des = des + "]";

        des = des + "}";
    }
}