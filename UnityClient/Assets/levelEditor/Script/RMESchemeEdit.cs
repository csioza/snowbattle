using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// 机关编辑
public class RMESchemeEdit : RMEBaseInfoEdit
{
    public enum BeginState
    {
        enClose =0,
        enOpen,
        enLock,
        enDisable,
    }
    [HideInInspector]
    public int SerializeNo    =0;

    public int npcId          = 0;          // 
    public float showPercent  = 0.0f;       // 出现的几率

    public int beginEable       = 1; // 初始可用
    public BeginState beginState = BeginState.enClose; // 初始状态

    public string desTranspotName = "";

    [SerializeField]public List<int> skillIdList; // 携带技能列表

    public RMESchemeEdit()
        : base(Type.enScheme)
    {

    }
}


