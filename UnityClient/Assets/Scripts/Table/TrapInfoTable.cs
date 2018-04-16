//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Table
//	created:	2013-5-14
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEngine;

public class TrapInfo : IDataBase
{
    public int ID { get; private set; }
    public int ModelID { get; private set; }                //模型ID
    public float ModelScale { get; private set; }           //模型比例
    public int TrapType { get; private set; }               //机关类型
    public bool NeedKey { get; private set; }               //机关是否需要钥匙
    public int CampType { get; private set; }               //机关阵营
    public int MaxTouchCount { get; private set; }          //最大触发次数
    public float MinTouchTime { get; private set; }         //最小触发间隔
    public string AIXmlName { get; private set; }           //AIXML文件名字
    public string AIXmlSubName { get; private set; }        //AIXML字段名字
    public string Note { get; private set; }                //说明
    public List<string> AttackAnimList { get; private set; }        //攻击Action执行的动画列表
    public List<string> StandbyAnimList { get; private set; }       //待机Action执行的动画列表
    public List<string> ContinueDamageAnimList { get; private set; }        //持续伤害Action执行的动画列表
    public List<string> BackAnimList { get; private set; }          //返回Action执行的动画列表
    public string mOpenTrapAnim { get; private set; }               //开启动画名
    public string mCloseTrapAnim { get; private set; }              //关闭动画名
    public string mLockTrapAnim { get; private set; }               //锁定动画名
    public string mDisableTrapAnim { get; private set; }               //失效动画名
//     public string mAbleTrapAnim { get; private set; }               //激活动画
//     public string mUnableTrapAnim { get; private set; }             //未激活动画
}

public class TrapInfoTable
{
    public Dictionary<int, TrapInfo> m_list { get; protected set; }
    public TrapInfo Lookup(int id)
    {
        TrapInfo info = null;
        m_list.TryGetValue(id, out info);
        return info;
    }
    public void Load(byte[] bytes)
    {
        m_list = new Dictionary<int, TrapInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        for (int index = 0; index < length; ++index)
        {
            TrapInfo info = new TrapInfo();
            info.Load(helper);
            m_list.Add(info.ID, info);
            
        }
    }
};