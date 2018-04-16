using System;
using System.Collections.Generic;

public class FlyingItemInfo : IDataBase
{
    //反弹类型
    public enum ENBouncyType
    {
        enRandom,//随机
        enNotRepeat,//不重复选择目标
    }
    public int ID { get; protected set; }
    //飞行道具描述
    public string Item_Desc { get; protected set; }
    //生成飞行道具前是否有预警特效
    public bool IsWarningBeforeItem { get; protected set; }
    //预警特效名称
    public string WarningEffectName { get; protected set; }
    //预警特效循环播放时间
    public float WaringEffectDuration { get; protected set; }
    //飞行道具名称
    public string Item_Name { get; protected set; }
    //是否只对选中目标生效
    public bool IsOnlyForChosenTarget { get; protected set; }
    //飞行道具生成位置 不填写，目标点 其他，发射点名称
    public string Item_BoneName { get; protected set; }
    //飞行道具生成位置的偏移
    public string Item_BoneOffset { get; protected set; }
    //是否反弹
    public bool IsBouncy { get; protected set; }
    //反弹类型---ENBouncyType
    public int Item_BouncyType { get; protected set; }
    //反弹次数
    public int Item_BouncyCount { get; protected set; }
    //反弹距离
    public float Item_BouncyDistance { get; protected set; }
    //是否拉伸
    public bool IsStretch { get; protected set; }
    //拉伸速度
    public float Item_StretchSpeed { get; protected set; }
    //Z轴长度
    public float ZAxisLongth { get; protected set; }
    //是否延缓发射
    public bool IsLaunchSuspend { get; protected set; }
    //延缓飞行道具检测范围(半径）
    public float Item_MonitorArea { get; protected set; }
    //延缓飞行道具是否跟随创建者
    public bool IsFollowCreator { get; protected set; }
    //是否锁定飞行道具y轴
    public bool Item_IsLockY { get; protected set; }
    //飞行道具移动速度
    public float Item_MoveSpeed { get; protected set; }
    //飞行道具是否曲线移动
    public bool IsCurveMove { get; protected set; }
    //附加方向
    public string Item_AddtionalDirection { get; protected set; }
    //附加速率
    public float Item_AddtionalSpeed { get; protected set; }
    //附加方向和速率影响时间
    public float Item_AddtionalEffectTime { get; protected set; }
    //存在时间 0，无限 其他，相应时间
    public float Item_ExistTime { get; protected set; }
    //移动距离 0，无限 其他，相应距离
    public float Item_MoveDistance { get; protected set; }
    //道具是否在目标点销毁
    public bool Item_IsDestroyAtTargetPos { get; protected set; }
    //飞行道具技能Result
    public int Item_ResultID { get; protected set; }
    //生效后是否移除
    public bool IsRemoveAfterResult { get; protected set; }
    //是否可以对生效目标多次生效
    public bool IsMultiResult { get; protected set; }
    //是否追踪技能目标
    public bool IsTrack { get; protected set; }
    //移除后是否有效果
    public bool IsResultForRemove { get; protected set; }
    //移除后效果的resultid
    public int ResultIDForRemove { get; protected set; }
    //是否在移除飞行特效后播放特效
    public bool IsEffectForRemove { get; protected set; }
    //移除后特效名称
    public string EffectNameForRemove { get; protected set; }
    //移除后特效播放时间
    public float EffectTimeForRemove { get; protected set; }
    //是否改变camera焦点
    public bool IsChangeCamera { get; protected set; }
    //改变焦点所用的时间
    public float CC_ChangeTime { get; protected set; }
    //改变后的持续时间
    public float CC_Duration { get; protected set; }
    //飞行道具销毁后，持续的时间
    public float CC_DurationAfterDestroy { get; protected set; }
    //返回至主控角色时所用的时间
    public float CC_BackTime { get; protected set; }
}

public class FlyingItemTable
{
    public Dictionary<int, FlyingItemInfo> m_list { get; protected set; }
    public FlyingItemInfo LookUp(int id)
    {
        FlyingItemInfo info = null;
        m_list.TryGetValue(id, out info);
        return info;
    }

    public void Load(byte[] bytes)
    {
        m_list = new Dictionary<int, FlyingItemInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        for (int index = 0; index < length; ++index)
        {
            FlyingItemInfo info = new FlyingItemInfo();
            info.Load(helper);
            m_list.Add(info.ID, info);
        }
    }
}