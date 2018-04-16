using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class LE_Action_DropItemInfo
{
    

    [HideInInspector]
    public DropItemResult.ID type = DropItemResult.ID.smallPill;

    [HideInInspector]
    public int keyId = 0;


}

// 动作-掉落物品

public class LE_Action_DropItem : LE_Action_Base
{
    public LE_Action_DropItem()
        : base(EResultManager.ENResultTypeID.enDropItem)
    {

    }
    [HideInInspector]
    [SerializeField]
    public List<LE_Action_DropItemInfo> List; // 列表
}


