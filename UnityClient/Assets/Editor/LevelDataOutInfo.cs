
using System.Collections.Generic;
using System.IO;
using System;
using UnityEditor;
using UnityEngine;

//////////////////////////////////////////////////////////////////////////
// 基本信息 
// 基本信息父类
public class RME_BaseInfo
{

    public RME_BaseInfo(RMEBaseInfoEdit.Type type)
    {
        m_type = type;
    }
    public RMEBaseInfoEdit.Type m_type;

    // 子类需重载此方法
    virtual public void Write(BinaryHelper helper)
    {

    }

    virtual public void Read(BinaryHelper helper)
    {

    }
}
// 宝箱信息
public class BoxInfo : RME_BaseInfo
{
    public float percet = 0.0f; // 出现概率
    public int id = 0;    // 
    public int x = 0;    // 坐标X
    public int z = 0;    // 坐标Z
    public int SerializeNo = 0;
    public BoxInfo()
        : base(RMEBaseInfoEdit.Type.enBox)
    {

    }

    override public void Write(BinaryHelper helper)
    {
        helper.Write(id);
        helper.Write(x);
        helper.Write(z);
        helper.Write(percet);
        helper.Write(SerializeNo);
        Debug.Log("宝箱.x:" + x + "boxInfo.z:" + z + ",boxInfo.percet:" + percet + ",boxInfo.SerializeNo:" + SerializeNo);
    }

    override public void Read(BinaryHelper helper)
    {
        id = helper.ReadInt();
        x = helper.ReadInt();
        z = helper.ReadInt();
        float percent = helper.ReadFloat();
        int serNo = helper.ReadInt();
        Debug.Log("宝箱：" + ",x:" + x + ",z:" + z + ",percent:" + percent + ",id:" + id + ",serNo:" + serNo);
    }
}

// 传送门信息
public class TranspotInfo : RME_BaseInfo
{
    public int id = 0;    // 
    public int x = 0;    // 坐标X
    public int z = 0;    // 坐标Z

    public TranspotInfo()
        : base(RMEBaseInfoEdit.Type.enTranspotDoor)
    {

    }

    override public void Write(BinaryHelper helper)
    {
        helper.Write(id);
        helper.Write(x);
        helper.Write(z);
        Debug.Log("transpotInfo.x:" + x + "transpotInfo.z:" + z);
    }

    override public void Read(BinaryHelper helper)
    {
        int id = helper.ReadInt();
        int x = helper.ReadInt();
        int z = helper.ReadInt();
        Debug.Log("传送门：" + ",x:" + x + ",z:" + z + ",id:" + id);
    }
}
// 机关信息
public class SchemeInfo : RME_BaseInfo
{
    public int x = 0;    // 坐标X
    public int z = 0;    // 坐标Z
    public int npcId = 0;
    public float percent = 0.0f; // 出现概率
    public List<int> skillList;     // 技能列表
    public int SerializeNo = 0;
    public int beginEable = 1; // 初始可用
    public int beginState = 1; // 初始状态

    public float scaleX = 0.0f;
    public float scaleZ = 0.0f;
    public string desTranspotName = "";


    public SchemeInfo()
        : base(RMEBaseInfoEdit.Type.enScheme)
    {

    }
    override public void Write(BinaryHelper helper)
    {
        helper.Write(x);
        helper.Write(z);
        helper.Write(npcId);
        helper.Write(percent);
        helper.Write(SerializeNo);
        helper.Write(beginEable);
        helper.Write(beginState);
        helper.Write(desTranspotName);
        helper.Write(scaleX);
        helper.Write(scaleZ);
        helper.Write(skillList.Count);
        for (int i = 0; i < skillList.Count; i++)
        {
            helper.Write(skillList[i]);
        }
        Debug.Log("机关.x:" + x + "schemeInfo.z:" + z + ",schemeInfo.percet:" + percent + ",schemeInfo.npcId:" + npcId + ",schemeInfo.SerializeNo:" + SerializeNo);

    }
    override public void Read(BinaryHelper helper)
    {
        x = helper.ReadInt();
        z = helper.ReadInt();
        npcId = helper.ReadInt();
        float percent = helper.ReadFloat();
        int serNo = helper.ReadInt();
        int beginEable = helper.ReadInt();
        int beginState = helper.ReadInt();
        string name = helper.ReadString();
        float scalX = helper.ReadFloat();
        float scalZ = helper.ReadFloat();
     
        int skillCount = helper.ReadInt();

        Debug.Log("机关：" + ",x:" + x + ",z:" + z + ",percent:" + percent + ",npcId:" + npcId + ",skillCount:" + skillCount + ",serNo:" + serNo + ",beginEable:" + beginEable + ",beginState:" + beginState + ",name:" + name + ",scalX:" + scalX + ",scalZ:" + scalZ);

        for (int j = 0; j < skillCount; j++)
        {
            int skillId = helper.ReadInt();
            Debug.Log("技能ID：" + skillId);
        }
    }
}

// 怪物信息
public class MonsterInfo
{
    public int id;
    public int weight = 0; // 权重


    public int SerializeNo = 0;
}

// 单个阵型信息
public class FormationInfo
{
    public int id = 0;
    public int weight = 0; // 权重
    public int showBegin = 1;// 是否一开始就出现
    public Dictionary<int, List<MonsterInfo>> list;     // int 怪物点 索引Id，标志位上的怪物列表
    public Dictionary<int, int> SerNolist;     // int 怪物点 索引Id，怪物点唯一ID
	public Dictionary<int, int> showBeginList;     // int 怪物点 索引Id，是否一开始就出现
}

// 阵型点信息
public class ArrayInfo : RME_BaseInfo
{
    public int x = 0;    // 坐标X
    public int z = 0;    // 坐标Z
    public Dictionary<GameObject, FormationInfo> list;     // GameObject 阵型预设物件，FormationInfo 阵型详细信息
    public List<FormationInfo> dataList;     // GameObject 阵型预设物件，FormationInfo 阵型详细信息

    public ArrayInfo()
        : base(RMEBaseInfoEdit.Type.enArray)
    {

    }
    override public void Write(BinaryHelper helper)
    {
        helper.Write(x);
        helper.Write(z);
        helper.Write(list.Count);
        Debug.Log("阵型.x:" + x + "arrayInfo.z:" + z + ",arrayInfo.Count:" + list.Count);


        foreach (KeyValuePair<GameObject, FormationInfo> item in list)
        {
            helper.Write(item.Value.id);
            helper.Write(item.Value.weight);

            helper.Write(item.Value.list.Count);
			Debug.Log("FormationInfo ID:" + item.Value.id + "item.Value.weight:" + item.Value.weight + ",item.Value.list.Count:" + item.Value.list.Count);

            foreach (KeyValuePair<int, List<MonsterInfo>> formationItem in item.Value.list)
            {
                helper.Write(formationItem.Key);
                helper.Write(formationItem.Value.Count);

                for (int i = 0; i < formationItem.Value.Count; i++)
                {
                    MonsterInfo monsterInfo = formationItem.Value[i];
                    helper.Write(monsterInfo.id);
                    helper.Write(monsterInfo.weight);
                    
					if (item.Value.showBeginList.ContainsKey(formationItem.Key))
					{
						helper.Write(item.Value.showBeginList[formationItem.Key]);
					}
					// 无效数据
					else
					{
						helper.Write(0);
					}
					Debug.Log("monsterInfo.id:" + monsterInfo.id + ",monsterInfo.weight:" + monsterInfo.weight + ",item.Value.showBegin:" + item.Value.showBeginList[formationItem.Key]);
                }
                // 有效数据
                if (item.Value.SerNolist.ContainsKey(formationItem.Key))
                {
                    helper.Write(item.Value.SerNolist[formationItem.Key]);
                }
                // 无效数据
                else
                {
                    helper.Write(0);
                }
            }
        }
    }

    override public void Read(BinaryHelper helper)
    {
        x = helper.ReadInt();
        z = helper.ReadInt();
        int arryListCount = helper.ReadInt();

        if (dataList == null)
        {
            dataList = new List<FormationInfo>();
        }

        dataList.Clear();

        Debug.Log("阵型.x:" + x + "z:" + z + ",arryListCount:" + arryListCount);

        for (int j = 0; j < arryListCount; j++)
        {
            
            FormationInfo info  = new FormationInfo();
            info.id             = helper.ReadInt();
            info.weight         = helper.ReadInt();

            info.list           = new Dictionary<int, List<MonsterInfo>>();

            int formationcount = helper.ReadInt();

            Debug.Log("阵型item id:" + info.id + "weight:" + info.weight + ",formationcount:" + formationcount);

            for (int m = 0; m < formationcount; m++)
            {
                List<MonsterInfo> item = new List<MonsterInfo>();
                int tag             = helper.ReadInt();
                int mosterCounts    = helper.ReadInt();

                Debug.Log("怪物 tag:" + tag + ",mosterCounts:" + mosterCounts);
                for (int n = 0; n < mosterCounts; n++)
                {
                    MonsterInfo monsterInfo = new MonsterInfo();
                    monsterInfo.id          = helper.ReadInt();
                    monsterInfo.weight      = helper.ReadInt();
                    int showBegin           = helper.ReadInt();
                    Debug.Log("monsterId.id:" + monsterInfo.id + ",monsterWeight:" + monsterInfo.weight + ",showBegin:" + showBegin);
                    item.Add(monsterInfo);
                }

                info.list.Add(tag, item);
                int monsterSerNo = helper.ReadInt();
                Debug.Log("monsterSerNo:" + monsterSerNo);
            }

            dataList.Add(info);
        }
    }
}

// 房间标签信息
public class RoomInfo
{
    public List<int> tagList;    //标签列表
    public string prefabName = "";
    public int terrainId = 0;    // 地形ID


    public void Reset()
    {
        prefabName = "";
        terrainId = 0;
        if (null != tagList)
        {
            tagList.Clear();
        }
    }
}

// 门信息
public class GateInfo : RME_BaseInfo
{
    public int postion = 0;
    public int objId = 0;    // 
    public int beginState = 0;    // 初始状态
    public int passType = 0;    // 通关条件类型
    public List<int> List;          // 通关条件 子类型的唯一序列ID列表
    public int serNo = 0;
    public GateInfo()
        : base(RMEBaseInfoEdit.Type.enGate)
    {

    }
    override public void Write(BinaryHelper helper)
    {
        helper.Write(postion);
        helper.Write(objId);
        helper.Write(beginState);
        helper.Write(passType);
        helper.Write(serNo);
        helper.Write(List.Count);
        for (int i = 0; i < List.Count; i++)
        {
            helper.Write(List[i]);
        }
    }
    override public void Read(BinaryHelper helper)
    {
        int postion = helper.ReadInt();
        int gateObjId = helper.ReadInt();
        int gateBeginState = helper.ReadInt();
        int gatePassType = helper.ReadInt();
        int gateserNo = helper.ReadInt();
        int gateListCount = helper.ReadInt();
        for (int j = 0; j < gateListCount; j++)
        {
            int no = helper.ReadInt();
            Debug.Log("no.id:" + no);
        }
        Debug.Log("postion:" + postion + "gate gateObjId:" + gateObjId + ",gateBeginState:" + gateBeginState +",gateserNo:"+gateserNo+ ",gatePassType:" + gatePassType + ",gateListCount:" + gateListCount);
    }
}

// 人物出生点信息
public class ActorBornPointInfo : RME_BaseInfo
{
    public int x = 0;    // 坐标X
    public int z = 0;    // 坐标Z
    public int haveData = 0;
    public ActorBornPointInfo()
        : base(RMEBaseInfoEdit.Type.enActorBorn)
    {

    }

    override public void Write(BinaryHelper helper)
    {
        helper.Write(x);
        helper.Write(z);
        Debug.Log("ActorBornPointInfo.x:" + x + "ActorBornPointInfo.z:" + z);
    }

    override public void Read(BinaryHelper helper)
    {
        int x = helper.ReadInt();
        int z = helper.ReadInt();
        Debug.Log("人物出生点信息：" + ",x:" + x + ",z:" + z );
    }
}
// 传送点信息
public class TranspotPointInfo : RME_BaseInfo
{
    public int x = 0;    // 坐标X
    public int z = 0;    // 坐标Z
    public int serNo = 0;

    public TranspotPointInfo()
        : base(RMEBaseInfoEdit.Type.enTranspotPoint)
    {

    }

    override public void Write(BinaryHelper helper)
    {
        helper.Write(serNo);
        helper.Write(x);
        helper.Write(z);
        Debug.Log("TranspotPointInfo 传送点 写信息.x:" + x + "ActorBornPointInfo.z:" + z + ",serNo:" + serNo);
    }

    override public void Read(BinaryHelper helper)
    {
        int serNo   = helper.ReadInt();
        int x       = helper.ReadInt();
        int z       = helper.ReadInt();
        Debug.Log("传送点 读取信息：" +serNo+ ",x:" + x + ",z:" + z);
    }
}

//////////////////////////////////////////////////////////////////////////
// 基本信息 完

//////////////////////////////////////////////////////////////////////////
// 事件 
// 事件父方法
public class RME_EventInfo
{
    public int eventId = 0;
    public int serNo = 0;
    public List<int> actionlist = new List<int>();// 行为 唯一ID列表
    // 子类需重载此方法
    virtual public void Write(BinaryHelper helper)
    {

    }

    // 子类需重载此方法
    virtual public void WriteStr(string strIn, out string str)
    {
        str = strIn;
    }

    virtual public void Read(BinaryHelper helper)
    {

    }
}

public class EventEventNpcDeadItemInfo 
{
    public int npcType          = 0;
    public int targetNpcSerNo   = 0;
    public string blackboardActorName = "\"\"";
}
// NPC死亡事件信息
public class EventNpcDeadInfo : RME_EventInfo
{

    public List<EventEventNpcDeadItemInfo> list = new List<EventEventNpcDeadItemInfo>();

   

    override public void WriteStr(string strIn, out string str)
    {
        str = strIn;
        str = str + "\"" + EventManager.sdeadNpcList + "\":[";
        for (int i = 0; i < list.Count;i++ )
        {
            str = str + "{\"" + EventManager.snpcDeadNpcType + "\":" + list[i].npcType;
            str = str + ",";
            str = str + "\"" + EventManager.stargetNpcSerNo + "\":" + list[i].targetNpcSerNo;
            str = str + ",";
            str = str + "\"" + EventManager.snpcDeadBlackboardActorName + "\":" + list[i].blackboardActorName;
            str = str + "}";
            if (i + 1 != list.Count)
            {
                str = str + ",";
            }
        }
        str = str + "]";
       
    }



}


// 触发事件 逻辑关系数据
public class EventsRelationship : RME_EventInfo
{
    public List<int> eventsList = new  List<int>();// 事件列表 
    //////////////////////////////////////////////////////////////////////////
    // 触发事件 逻辑关系事件的 读写数据 顺序为
    // 事件ID 也是事件关系
    // 关系事件列表的数量
    // 关系事件列表内循环内 此逻辑关系事件的唯一ID
    // 关系事件列表内循环内 各个事件的逻辑关系事件的数量
    // 关系事件列表内循环内 各个事件的逻辑关系事件列表循环内  事件唯一ID

    override public void Write(BinaryHelper helper)
    {
        helper.Write(serNo);
        helper.Write(eventsList.Count);

        Debug.Log("EventsRelationship 写数据serNo：" + serNo + "eventId:" + eventId + ",eventsList.Count:" + eventsList.Count);
        for (int i = 0; i < eventsList.Count;i++)
        {
            helper.Write(eventsList[i]);


            Debug.Log("EventsRelationship eventsList[i]:" + eventsList[i] );
        }   
    }

    override public void WriteStr(string strIn, out string str)
    {
        str = strIn;
        str = str + "\"" + EventManager.seventList + "\":[";

        for (int i = 0; i < eventsList.Count; i++)
        {
            str = str + eventsList[i];

            if (i + 1 != eventsList.Count)
            {
                str = str + ",";
            }
        }
        str = str + "]";
    }

    override public void Read(BinaryHelper helper)
    {
        int serNo           = helper.ReadInt();
        int eventCount      = helper.ReadInt();
        Debug.Log("EventsRelationship Read ,serNo:" + serNo+"eventCount:" + eventCount );
        for (int i = 0; i < eventCount; i++)
        {
            int eventSerNo = helper.ReadInt();
            
            Debug.Log("EventsRelationship Read eventSerNo:" + eventSerNo );

         
        }

        // 行为数量
        int actionCount = helper.ReadInt();
        for (int i = 0; i < actionCount; i++)
        {
            int actionId = helper.ReadInt();
            Debug.Log("EventsRelationship actionId:" + actionId);
        }
    }
}

public class EventSchemeActionItemInfo
{
    public int serNo            = 0;
    public int actionType       = 0;
}

// 机关触发事件信息
public class EventSchemeTrapInfo : RME_EventInfo
{

    public List<EventSchemeActionItemInfo> list = new List<EventSchemeActionItemInfo>();


    override public void WriteStr(string strIn, out string str)
    {
        str = strIn;
        str = str + "\"" + EventManager.sSchemeTrapList + "\":[";
        for (int i = 0; i < list.Count; i++)
        {
            str = str + "{\"" + EventManager.sTrapSerNo + "\":" + list[i].serNo;
            str = str + ",";
            str = str + "\"" + EventManager.sTrapType + "\":" + list[i].actionType;
            str = str + "}";
            if (i + 1 != list.Count)
            {
                str = str + ",";
            }
        }
        str = str + "]";

    }
}

// 宝箱开启事件信息
public class EventBoxOpenInfo : RME_EventInfo
{

    public List<int> list = new List<int>();


    override public void WriteStr(string strIn, out string str)
    {
        str = strIn;
        str = str + "\"" + EventManager.sBoxSerNoList + "\":[";
        for (int i = 0; i < list.Count; i++)
        {
            str = str + list[i];
            if (i + 1 != list.Count)
            {
                str = str + ",";
            }
        }
        str = str + "]";

    }



}



public class EventHpItemInfo
{
    public int npcType                  = 0;
    public int targetNpcSerNo           = 0;
    public string blackboardActorName = "\"\"";
    public int hpValType = 0;
    public int hpValue   = 0;
}

// 怪物生命值低于 [参数1] 的百分比触发 事件 信息
public class EventHpInfo : RME_EventInfo
{

    public List<EventHpItemInfo> list = new List<EventHpItemInfo>();


    override public void WriteStr(string strIn, out string str)
    {
        str = strIn;
        str = str + "\"" + EventManager.sHPMonsterList + "\":[";
        for (int i = 0; i < list.Count; i++)
        {
            str = str + "{\"" + EventManager.sHPMonsterType + "\":" + list[i].npcType;
            str = str + ",";
            str = str + "\"" + EventManager.sHPMonsterSerNo + "\":" + list[i].targetNpcSerNo;
            str = str + ",";
            str = str + "\"" + EventManager.sHPblackboardActorName + "\":" + list[i].blackboardActorName;
            str = str + ",";
            str = str + "\"" + EventManager.sHPValType + "\":" + list[i].hpValType;
            str = str + ",";
            str = str + "\"" + EventManager.sHPValue + "\":" + list[i].hpValue;
            str = str + "}";

            if (i + 1 != list.Count)
            {
                str = str + ",";
            }
        }
        str = str + "]";

    }



}

//////////////////////////////////////////////////////////////////////////
// 事件 完

//////////////////////////////////////////////////////////////////////////
// 行为
// 行为父方法
public class RME_ActionInfo
{
    public int actionId = 0;
    public int serNo = 0;// 唯一ID

    // 子类需重载此方法
    virtual public void Write(BinaryHelper helper)
    {

    }

    // 子类需重载此方法
    virtual public void WriteStr(string strIn ,out string str)
    {
        str = strIn;
    }

    virtual public void Read(BinaryHelper helper)
    {

    }
}

public class ActionSpawnNpcItemInfo
{
    public int npcSerNo = 0;
    public int spawnPosition = 0;
    public Vector2 offset;
    public int targetSerNo = 0;
    public string blackboardStr = "\"\"";
    public int targetType = 0;


}
// 刷怪行为信息
public class ActionSpawnNpcInfo : RME_ActionInfo
{
    public List<ActionSpawnNpcItemInfo> NpcList = new List<ActionSpawnNpcItemInfo>();

    override public void WriteStr(string strIn ,out string str)
    {
        str = strIn;
        str = str + "\"" + EResultManager.sSwapMonsterDataList + "\":" + "[";
        int index = 0;
        foreach (ActionSpawnNpcItemInfo item in NpcList)
        {
            index++;
            str = str + "{";
            str = str + "\"" + EResultManager.sSpawMonsterSerNo + "\":" + item.npcSerNo;
            str = str + ",";
            str = str + "\"" + EResultManager.sSpawnPosType + "\":" + item.spawnPosition;
            str = str + ",";
            str = str + "\"" + EResultManager.sSpawnMonsterOffsetx + "\":" + item.offset.x;
            str = str + ",";
            str = str + "\""+EResultManager.sSpawnMonsterOffsetz+"\":"+item.offset.y;
            str = str + ",";
            str = str + "\"" + EResultManager.sSpawnMonstertargetSerNo + "\":" + item.targetSerNo;
            str = str + ",";
            str = str + "\"" + EResultManager.sSpawnMonstertargetType + "\":" + item.targetType;
            str = str + ",";
            str = str + "\"" + EResultManager.sSpawnMonsterBlackBoardStr + "\":" + item.blackboardStr;
            str = str + "}";

            if (index != NpcList.Count)
            {
                str = str + ",";
            }
            Debug.Log("刷怪行为信息 写数据:npcSerNo " + item.npcSerNo + ",spawnPosition:" + item.spawnPosition + ",offsetx:" + item.offset.x + ",offsety:" + item.offset.y + ",targetSerNo:" + item.targetSerNo);
        }

        str = str + "]";
        Debug.Log("刷怪行为信息 写数据:" + actionId + ",serNo:" + serNo + ",npcCount:" + NpcList.Count);
    }
    override public void Read(BinaryHelper helper)
    {
        int serNo = helper.ReadInt();
        int npcCount = helper.ReadInt();

        for (int i = 0; i < npcCount; i++)
        {
            int npcSerNo = helper.ReadInt();
            int spawnPosition = helper.ReadInt();
            float offsetx = helper.ReadFloat();
            float offsety = helper.ReadFloat();
            int targetSerNo = helper.ReadInt();

            Debug.Log("刷怪行为信息 读数据:npcSerNo " + npcSerNo + ",spawnPosition:" + spawnPosition + ",offsetx:" + offsetx + ",offsety:" + offsety + ",targetSerNo:" + targetSerNo);
        }
        Debug.Log("刷怪行为信息 读数据:" + ",serNo:" + serNo + ",npcCount:" + npcCount);
    }
}

// 关卡胜利或失败行为信息
public class VictorToLevelInfo : RME_ActionInfo
{
   public  int victory = 0;

    override public void WriteStr(string strIn, out string str)
    {
        str = strIn;

        str = str + "\"" + EResultManager.svictory + "\":" + victory;

        Debug.Log("关卡胜利或失败行为信息 写数据:" + actionId + ",serNo:" + serNo + ",victory:" + victory);
    }
}

public class RMESchemeTrapActionItemInfo
{
    public int trapType             = 0;//机关类型
    public int targetTrapSerNo      = 0;//目标机关序列号
    public string trapBlackboardStr = "\"\"";//黑板中的自定义字符
    public int actionType           = 0;//动作类型
}

// 行为 -使机关 锁定/失效/开启/关闭 行为信息
public class RMESchemeTrapActionInfo : RME_ActionInfo
{
    public List<RMESchemeTrapActionItemInfo> list = new List<RMESchemeTrapActionItemInfo>();

    override public void WriteStr(string strIn, out string str)
    {
        str = strIn;
        str = str + "\"" + EResultManager.sSchemeTrapList + "\":";
        str = str + "[";

        int index = 0;
        foreach (RMESchemeTrapActionItemInfo item in list)
        {
            index++;
            str = str + "{";
            str = str + "\"" + EResultManager.sTrapType + "\":" + item.trapType;
            str = str + ",";
            str = str + "\"" + EResultManager.sTrapSerNo + "\":" + item.targetTrapSerNo;
            str = str + ",";
            str = str + "\"" + EResultManager.sTrapBlackboardStr + "\":" + item.trapBlackboardStr;
            str = str + ",";
            str = str + "\"" + EResultManager.sTrapOptType + "\":" + item.actionType;

            str = str + "}";

            if (index != list.Count)
            {
                str = str + ",";
            }
        }
        str = str + "]";
    }
}

public class RMEGateOpActionInfoItemInfo
{
    public int gateType             = 0;//闸门类型
    public int targetGateSerNo      = 0;//目标闸门序列号
    public string gateBlackboardStr = "\"\"";//黑板中的自定义字符
    public int actionType           = 0;//动作类型
}

// 行为-打开/关闭 闸门行为信息
public class RMEGateOpActionInfo : RME_ActionInfo
{
    public List<RMEGateOpActionInfoItemInfo> list = new List<RMEGateOpActionInfoItemInfo>();

    override public void WriteStr(string strIn, out string str)
    {
        str = strIn;
        str = str + "\"" + EResultManager.sGateList + "\":" + "[";
      

        int index = 0;
        foreach (RMEGateOpActionInfoItemInfo item in list)
        {
            index++;
            str = str + "{";
            str = str + "\"" + EResultManager.sGateType + "\":" + item.gateType;
            str = str + ",";
            str = str + "\"" + EResultManager.sGateID + "\":" + item.targetGateSerNo;
            str = str + ",";
            str = str + "\"" + EResultManager.sGateBlackboardStr + "\":" + item.gateBlackboardStr;
            str = str + ",";
            str = str + "\"" + EResultManager.sTrapOptType + "\":" + item.actionType;

            str = str + "}";

            if (index != list.Count)
            {
                str = str + ",";
            }
        }
        str = str + "]";
    }
}

public class RMEDropItemActionInfoItemInfo
{
    public int itemType = 0;
    public int keyID = 0;
 
}

// 行为-掉落
public class RMEDropItemActionInfo : RME_ActionInfo
{
    public List<RMEDropItemActionInfoItemInfo> list = new List<RMEDropItemActionInfoItemInfo>();

    override public void WriteStr(string strIn, out string str)
    {
        str = strIn;
        str = str + "\"" + EResultManager.sItemList + "\":" + "[";
   

        int index = 0;
        foreach (RMEDropItemActionInfoItemInfo item in list)
        {
            index++;
            str = str + "{";
            str = str + "\"" + EResultManager.sItemType + "\":" + item.itemType;
            str = str + ",";
            str = str + "\"" + EResultManager.sKeyID + "\":" + item.keyID;
          
            str = str + "}";

            if (index != list.Count)
            {
                str = str + ",";
            }
        }
        str = str + "]";
    }
}

public class RMEAddBuffActionInfoItemInfo
{
    public int actorType = 0;
    public int actorID = 0;
    public string buffBlackboardStr = "\"\"";//黑板中的自定义字符  json默认数据格式 字符串空为 "";
    public int buffOptType = 0;
    public List<int> buffIDList = new List<int>();

}

// 动作-添加/移除 buff给角色
public class RMEAddBuffActionInfo : RME_ActionInfo
{
    public List<RMEAddBuffActionInfoItemInfo> list = new List<RMEAddBuffActionInfoItemInfo>();

    override public void WriteStr(string strIn, out string str)
    {
        str = strIn;
        str = str + "\"" + EResultManager.sBuffActorList + "\":" + "[";

        int index = 0;
        foreach (RMEAddBuffActionInfoItemInfo item in list)
        {
            index++;
            str = str + "{";
            str = str + "\"" + EResultManager.sActorType + "\":" + item.actorType;
            str = str + ",";
            str = str + "\"" + EResultManager.sActorID + "\":" + item.actorID;
            str = str + ",";
            str = str + "\"" + EResultManager.sBuffBlackboardStr + "\":" + item.buffBlackboardStr;
            str = str + ",";
            str = str + "\"" + EResultManager.sBuffOptType + "\":" + item.buffOptType;
            str = str + ",";
            str = str + "\"" + EResultManager.sBuffID + "\":";

            str = str + "[";

            int temp = 0;
            for ( int i = 0; i < item.buffIDList.Count;i++ )
            {
                temp++;

                str = str + item.buffIDList[i];

                if (temp != item.buffIDList.Count)
                {
                    str = str + ",";
                }
            }
            str = str + "]";

            str = str + "}";

            if (index != list.Count)
            {
                str = str + ",";
            }
        }
        str = str + "]";
    }
}

//////////////////////////////////////////////////////////////////////////
// 行为 完