using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class LE_SchemeTrapInfo
{

    public RMESchemeEdit.BeginState type = RMESchemeEdit.BeginState.enOpen;

    [HideInInspector]
    [SerializeField]
    public GameObject target; // 指定机关
}
// 机关开启/关闭事件
public class LE_Event_SchemeTrap : LE_Event_Base
{
    public LE_Event_SchemeTrap()
        : base(EventManager.ENTriggerID.enTriggerTrap)
    {

    }

    [HideInInspector]
    [SerializeField]
    public List<LE_SchemeTrapInfo> schemeList; // 列表



   
}


