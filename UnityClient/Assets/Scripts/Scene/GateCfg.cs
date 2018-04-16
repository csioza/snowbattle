using System;
using System.Collections.Generic;
using UnityEngine;



public class GateCfg : MonoBehaviour
{
    public SM.ENGateOpenCondType    OpenConditionType = SM.ENGateOpenCondType.enNone;
    public List<int>                OpenCondParamList = new List<int>();
    public List<int>                OpenCondCountList = new List<int>();
}

