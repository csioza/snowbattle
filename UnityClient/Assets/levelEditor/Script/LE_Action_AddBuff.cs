using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class LE_Action_AddBuffInfo
{
    public enum Type
    {
        specifiedNPC = 0,           //0.指定的NPC
        Mainplayer,                 //1.主控角色
        TriggeringActor,            //2.触发该事件的角色
        CastingActor,               //3.施放技能的角色
        LastCreatedActor,           //4.上一个被创建出来的角色
        TargetActorOfSkill,         //5.技能的目标角色
        TargetActorOfTriggeringActor,//6.触发事件的角色的目标
        RebornActor,                //7.复活的角色
        AttackedActor,              //8.受到伤害的角色
        TransportingActor,          //9.传送的角色
        TriggerTrapActor,           //10.触发机关的角色
        CustomBlackboardStr,        //11.黑板中指定的角色（自定义字符串)
    }

    public enum ACTIONTYPE
    {
        addSpecifiedBuff = 0,//0.添加特定buff
        removeSpecifiedBuff,//1.移除特定buff
        removeAllBuff,      //2.移除所有buff
        removeAllNegativeBuff,//3.移除所有负面buff
        removeAllPositiveBuff,//4.移除所有正面buff
    }

   [HideInInspector]
    public GameObject targetObj;

    [HideInInspector]
   public Type actorType = Type.specifiedNPC;


    [HideInInspector]
    public ACTIONTYPE actionType = ACTIONTYPE.addSpecifiedBuff;

    [HideInInspector]
    public string buffBlackboardStr = "";

    [HideInInspector]
    public List<int> buffList; // buff列表


}

// 动作-添加/移除buff给角色

public class LE_Action_AddBuff : LE_Action_Base
{
    public LE_Action_AddBuff()
        : base(EResultManager.ENResultTypeID.enBuffResult)
    {

    }
    [HideInInspector]
    [SerializeField]
    public List<LE_Action_AddBuffInfo> List; // 列表
}


