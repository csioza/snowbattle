using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// 装备资质表
public class BossInfo : IDataBase
{
    public int ID { get; protected set; }            //ID
    public string path { get; protected set; }       //图片路径
    public string say { get; protected set; }       //NPC对话
    public float time { get; protected set; }       //显示时间
}

public class BossTable
{
    public List<BossInfo> BossInfoList { get; protected set; }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        BossInfoList = new List<BossInfo>(length);
        for (int index = 0; index < length; ++index)
        {
            BossInfo info = new BossInfo();
            info.Load(helper);
            BossInfoList.Add(info);
        }
    }

    public BossInfo Lookup(int id)
    {
        foreach (BossInfo info in BossInfoList)
        {
            if (info.ID == id)
            {
                return info;
            }
        }
        return null;
    }
}