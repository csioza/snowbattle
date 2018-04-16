using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

public enum LOADINGTIPSENUM
{
    enLogin =1,// 登陆游戏
    enJumpToTeam,   //  跳转至队伍界面
    enJumpToBag,    //  跳转至背包界面
    enJumpToStore,  //  跳转至商店界面
    enJumpToSocial, //  跳转至社群界面
    enChooseFloor,  //  选择关卡
    enEnterCopy,    //  进入副本

}
public class LoadingTipsInfo : IDataBase
{
    public int ID { get; protected set; }

    public List<string> DescribeList { get; protected set; }
}

public class LoadingTipsTable
{
    public LoadingTipsInfo Lookup(int id)
    {
        LoadingTipsInfo info;
        InfoList.TryGetValue(id, out info);
        return info;
    }

    // 获得相关ID的随机提示
    public string GetTips(int id)
    {
        LoadingTipsInfo info = Lookup(id);

        if (null == info || info.DescribeList.Count == 0)
        {
            return "";
        }
        int index = UnityEngine.Random.Range(0, info.DescribeList.Count);
        return info.DescribeList[index];
    }

    public Dictionary<int, LoadingTipsInfo> InfoList { get; protected set; }
    public void Load(byte[] bytes)
    {
        InfoList        = new Dictionary<int, LoadingTipsInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        for (int index = 0; index < length; ++index)
        {
            LoadingTipsInfo info = new LoadingTipsInfo();
            info.Load(helper);
            InfoList.Add(info.ID, info);
        }
    }
}