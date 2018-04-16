using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
// 宝箱编辑
public class RMEBoxEdit : RMEBaseInfoEdit
{
    [HideInInspector]
    public int SerializeNo = 0;
    public float showPecent = 0.0f;// 出现几率
    public int id = 0;

    public RMEBoxEdit()
        : base(Type.enBox)
    {

    }

}




