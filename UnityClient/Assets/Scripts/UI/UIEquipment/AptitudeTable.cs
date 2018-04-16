using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 装备资质表
public class AptitudeInfo : IDataBase
{
    public int AptitudeID { get; protected set; }            //资质ID
    public string AptitudeName { get; protected set; }       //名称
    public int Probability { get; protected set; }           //几率
    public int RrateUp { get; protected set; }               //攻速
    public int TargetUp { get; protected set; }              //命中
    public int AaccuracyUp { get; protected set; }           //回避
    public int MagicUp { get; protected set; }               //魔攻
    public int AgainstUp { get; protected set; }             //魔防
    public int PhysicalUp { get; protected set; }            //物攻
    public int CritUp { get; protected set; }                //暴击
    public int TechnologyUp { get; protected set; }          //物防
    public int HpDown { get; protected set; }                //降低HP
}

public class AptitudeTable
{
    public Dictionary<int, AptitudeInfo> AptitudeInfoList { get; protected set; }

    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        AptitudeInfoList = new Dictionary<int, AptitudeInfo>();
        for (int index = 0; index < length; ++index)
        {
            AptitudeInfo info = new AptitudeInfo();
            info.Load(helper);
            AptitudeInfoList.Add(info.AptitudeID, info);
        }
    }
}