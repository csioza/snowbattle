using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class LE_Event_HPInfo
{

     public DeadEvent.ENDeadActorType npcType = DeadEvent.ENDeadActorType.enMainPlayer;

    [HideInInspector]
    [SerializeField]
    public GameObject targetNpc; // 指定NPC

    [HideInInspector]
    [SerializeField]
    public string blackboardActorName; // 黑板上的角色名称 HPblackboardActorName

    [HideInInspector]
    [SerializeField]
    public HPEvent.ENCompareType hpValType = HPEvent.ENCompareType.enValueLower;

    [HideInInspector]
    [SerializeField]
    public int hpValue = 0;    //  如果是百分比 此值应该在 0~100之间
}
// 事件-角色的生命值低于某个值事件
public class LE_Event_HP : LE_Event_Base
{
    public LE_Event_HP()
        : base(EventManager.ENTriggerID.enMonsterHPX)
    {

    }

    [HideInInspector]
    [SerializeField]
    public List<LE_Event_HPInfo> npcList; // 列表

   
}


