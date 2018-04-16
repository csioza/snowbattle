using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]

// 基本信息编辑
public class RMEBaseInfoEdit : MonoBehaviour
{
    public enum Type
    {
        enBox,
        enArray,
        enTranspotDoor,
        enMonster,
        enScheme,
        enRoom,
        enGate,
        enActorBorn,
        enTranspotPoint,
        enAdjustPos,

    }
    public RMEBaseInfoEdit(Type type)
    {
        m_type = type;
    }
    [HideInInspector]
    public Type m_type;

    [HideInInspector]
    public int m_serNo;

}




