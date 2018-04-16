using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


// and事件
public class LE_Event_And : LE_Event_Base
{
   public LE_Event_And()
       : base(EventManager.ENTriggerID.enAndEvent)
   {
     
   }

    // 事件列表
   [SerializeField]
   public List<GameObject> EventList;

   
}



