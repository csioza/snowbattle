using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class RMEBaseInfo
{
    public static int Index = 100;
    public static void ResetIndex()
    {
        Index = 100;
    }
}
[System.Serializable]
public class MonsterEditInfo
{
    
    [SerializeField]
    public int id       = 0;// 怪物ID
    [SerializeField]
    public int weight   = 1;// 出现的权重
}

[System.Serializable]
// 怪物点编辑
public class RMEMonsterEdit : RMEBaseInfoEdit
{
    [HideInInspector]public int SerializeNo = 0;
    public int tag = 0; //  怪物点标志位

    public int showBegin = 1;// 是否一开始就出现

    [SerializeField]
    public List<MonsterEditInfo> monsterList; // 怪物列表

    public RMEMonsterEdit()
        : base(Type.enMonster)
    {

    }

}




