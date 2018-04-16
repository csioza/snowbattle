using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// 行为 
public class LE_Action_Base : MonoBehaviour
{

   [HideInInspector]
   [SerializeField]
    public EResultManager.ENResultTypeID m_type;

   [HideInInspector]
   [SerializeField]
   public int SerializeNo = 0;

   public LE_Action_Base(EResultManager.ENResultTypeID type)
   {
       m_type = type;
   }
}


