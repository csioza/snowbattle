using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class LE_Action_SchemeTrapInfo
{

   [HideInInspector]
    public GameObject schemeObj;

    [HideInInspector]
   public TrapResult.ENTrapOptType actionType  = TrapResult.ENTrapOptType.enActiveFalse;


    [HideInInspector]
    public TrapResult.ENTrapType trapType = TrapResult.ENTrapType.enSpecial;

    [HideInInspector]
    public string trapBlackboardStr = "";


}

// 行为 -使机关 锁定/失效/开启/关闭

public class LE_Action_SchemeTrap : LE_Action_Base
{
    public LE_Action_SchemeTrap()
        : base(EResultManager.ENResultTypeID.enTrapResult)
    {

    }
    [HideInInspector]
    [SerializeField]
    public List<LE_Action_SchemeTrapInfo> List; // 列表
}


