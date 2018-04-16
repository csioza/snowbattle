using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


// 宝箱被开启事件
public class LE_Event_BoxOpen : LE_Event_Base
{
    public LE_Event_BoxOpen()
        : base(EventManager.ENTriggerID.enOpenBox)
    {

    }

    [SerializeField]
    public List<GameObject> boxList; // 列表

   
}


