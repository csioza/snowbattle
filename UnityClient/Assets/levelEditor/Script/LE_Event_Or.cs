using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


// or事件
public class LE_Event_Or : LE_Event_Base
{
    public LE_Event_Or()
       : base(EventManager.ENTriggerID.enOrEvent)
   {
     
   }

    // 事件列表
   [SerializeField]
   public List<GameObject> EventList;

}



