using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;



// 关卡胜利或失败
public class LE_Action_VictorToLevel : LE_Action_Base
{
    public enum State
    {
        enFailed,
        enVictory,
    }
    public LE_Action_VictorToLevel()
        : base(EResultManager.ENResultTypeID.enVictorToLevel)
    {

    }

    public State victory = State.enVictory;
}


