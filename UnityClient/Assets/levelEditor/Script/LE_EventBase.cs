using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LE_Event_Base : MonoBehaviour
{
 
  

   [HideInInspector]
   [SerializeField]
    public EventManager.ENTriggerID m_type;

   [HideInInspector]
   [SerializeField]
   public int SerializeNo = 0; // 在生成数据的时候 需手动生成 也就是++RMEBaseInfo.Index即可
   
   [HideInInspector]
   [SerializeField]
   public List<GameObject> ActionList; // 行为列表

   public LE_Event_Base(EventManager.ENTriggerID type)
   {
       m_type       = type;
       SerializeNo  = 0; 
   }
}



