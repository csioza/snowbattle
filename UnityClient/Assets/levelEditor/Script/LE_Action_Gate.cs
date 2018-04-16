using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class LE_Action_GateInfo
{


    public enum ACTIONTYPE
    {
        enOpen= 0,
        enClose,
    }
   [HideInInspector]
    public GameObject targetGateObj;

    [HideInInspector]
   public GateOpResult.ENGateType gateType = GateOpResult.ENGateType.specifiedGate;


    [HideInInspector]
    public ACTIONTYPE actionType = ACTIONTYPE.enOpen;

    [HideInInspector]
    public string gateBlackboardStr = "";


}

// 动作-打开/关闭闸门

public class LE_Action_Gate : LE_Action_Base
{
    public LE_Action_Gate()
        : base(EResultManager.ENResultTypeID.enGateOpResult)
    {

    }
    [HideInInspector]
    [SerializeField]
    public List<LE_Action_GateInfo> List; // 列表
}


