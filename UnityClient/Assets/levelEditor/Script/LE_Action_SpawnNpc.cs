using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class SpawnNpcInfo
{
    public GameObject targetNpc;
    public RefreshMonsterResult.ENSwapPosType spawnPosition = RefreshMonsterResult.ENSwapPosType.AffectedGate;
    [HideInInspector]
    public Vector2 offset ;

    [HideInInspector]
    public GameObject targetObject;

    [HideInInspector]
    public string blackboardStr = "";
}

// 行为 刷怪
public class LE_Action_SpawnNpc : LE_Action_Base
{
    public LE_Action_SpawnNpc()
        : base(EResultManager.ENResultTypeID.enRefreshMonsterResult)
    {

    }
    [HideInInspector]
    [SerializeField]
    public List<SpawnNpcInfo> spawnNpcList; // 列表
}


