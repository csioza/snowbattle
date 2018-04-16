//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEngine;


public class WeaponInfo : IDataBase
{
    public int WeaponID { get; private set; }		//武器ID
    public string ModelName { get; private set; }   //武器名称
    public float WeaponType { get; private set; }		//武器类型
    public int LeftModelID { get; private set; }		//左手武器模型ID
    public int RightModelID { get; private set; }		//右手武器模型ID
    public bool IsHideLeftModel { get; protected set; } //是否隐藏左手武器
    public bool IsHideRightModel { get; protected set; } //是否隐藏右手武器
    public string LeftPoint	{ get; private set; }		//左手武器挂载骨骼名称
    public string RightPoint { get; private set; }		//右手武器挂载骨骼名称
}

public class WeaponInfoTable
{
    public Dictionary<int, WeaponInfo> m_list { get; protected set; }
    public WeaponInfo Lookup(int id)
    {
        WeaponInfo info = null;
        m_list.TryGetValue(id, out info);
        return info;
    }
    public void Load(byte[] bytes)
    {
        m_list = new Dictionary<int, WeaponInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        for (int index = 0; index < length; ++index)
        {
            WeaponInfo info = new WeaponInfo();
            info.Load(helper);
            m_list.Add(info.WeaponID, info);
        }
    }
};