using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]

// 门编辑
public class RMEGateEdit : RMEBaseInfoEdit
{
    public int objId            = 0;
    public int beginState       = 0; // 初始状态

    [HideInInspector]
    
    public List<GameObject> monsterList;
    [HideInInspector]

    public List<GameObject> boxList;
    [HideInInspector]

    public List<GameObject> shemeList;

    public SM.ENGateOpenCondType passtype = SM.ENGateOpenCondType.enKillAllMonsters;

    public RMEGateEdit()
        : base(Type.enGate)
    {

    }

}




