using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class LE_NpcDeadInfo
{

    public DeadEvent.ENDeadActorType npcType = DeadEvent.ENDeadActorType.enMainPlayer;

    [HideInInspector]
    [SerializeField]
    public GameObject targetNpc; // 指定NPC

    [HideInInspector]
    [SerializeField]
    public string blackboardActorName; // 黑板上的角色名称
}
// NPC死亡事件
public class LE_Event_NpcDead : LE_Event_Base
{
    public LE_Event_NpcDead()
        : base(EventManager.ENTriggerID.enMonsterDead)
    {

    }

    [HideInInspector]
    [SerializeField]
    public List<LE_NpcDeadInfo> npcList; // 列表

   
}


