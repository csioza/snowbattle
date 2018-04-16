//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Table
//	created:	2013-5-13
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

#region disableWarning
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#endregion

public enum ENSkillType
{
    enSkill = 0,            //技能
    enSkillNormalType = 1,  //普通攻击技能
    enOpenBox,              //开宝箱技能
    enDodge,                //闪避技
    enPassive,//被动技能
    enSwordSoul,//剑魂技
    enBreakStamina,//破韧技
    
}
public enum ENSkillSpecialType
{
    enNone,
    enShift,//位移
    enControl,//控制
}
public enum ENSkillEffectType
{
    enNone,
    enDamage,
    enRestore,
    enBuff,
}
//技能等级类型
public enum ENSkillLevelType
{
    enNone,
    enBase,//初级
    enMiddle,//中级
    enHigh,//高级
}
//技能距离类型
public enum ENSkillDistanceType
{
    enNone,
    enNear,//近战
    enMiddle,//中等
    enFar,//远程
}
public enum ENTargetType
{
    enNone,
    enEnemy,//敌人
    enFriendly,//友方
    enSelf,//自身
    enNullTarget,//无目标
    enInteraction,//互动物件，主控角色拥有，目标是npc类型的互动物件
    enFriendlyAndSelf,//友方和自己
}
public enum ENSkillConnectionType
{
    enNone,
    enNormal,//普通
    enConnect,//连接
    enSpecial,//特殊
    enFinal,//终结
}
//技能主数据
public class SkillInfo : IDataBase
{
    //技能ID
    public int ID { get; protected set; }
    //技能名称
    public string Name { get; protected set; }
    //技能说明
    public string Description { get; protected set; }
    //技能resultID列表
    public List<string> SkillResultIDList { get; protected set; }
    // 技能最大等级
    public int MaxLevel { get; protected set; }
    // 技能经验类型
    public int SkillExpType { get; protected set; }
    //技能图标
    public int Icon { get; protected set; }
    //技能类型
    public int SkillType { get; protected set; } //ENSkillType
    //技能特殊类型
    public int SkillSpecialType { get; protected set; }//ENSkillSpecialType
    //技能效果类型
    public int SkillEffectType { get; protected set; } //ENSkillEffectType
    //技能级别类型
    public int SkillLevelType { get; protected set; }//ENSkillLevelType
    //技能距离类型
    public int SkillDistanceType { get; protected set; }//ENSkillDistanceType
    //技能连接类型
    public int SkillConnectionType { get; protected set; }//ENSkillConnectionType
    //目标类型
    public int TargetType { get; protected set; }//ENTargetType
    //后续技能
    public int FollowUpSkill { get; protected set; }
    //预CD
    public int PreCoolDown { get; protected set; }
    //冷却时间ID
    public int CoolDown { get; protected set; }
    //冷却时间成长参数
    public float CoolDownParam { get; protected set; }
    //最小combo需求
    public int MinComboRequir { get; protected set; }
    //最小combo需求成长参数
    public float ComboRequirementParam { get; protected set; }
    //需要卡牌的等级
    public int NeedLevel { get; protected set; }
    //学习消耗
    public int StudyMoney { get; protected set; }
    //目标数量
    public int TargetNumber { get; protected set; }
    //攻击距离
    public float AttackDistance { get; protected set; }
    //最小攻击距离
    public float LeastAttackDistance { get; protected set; }
    //技能释放之前添加的buff列表
    public List<float> BuffIDList { get; protected set; }
    //目标死亡时，是否停止攻击
    public bool IsStopAttck { get; protected set; }
    //是否锁定强韧值
    public bool IsStaminaFixed { get; protected set; }
    //产生的怒气值
    public float GenerateRagePoint { get; protected set; }
    //消耗的怒气值
    public float CostRagePoint { get; protected set; }
    //是否可以移动
    public bool IsCanMove { get; protected set; }
    //是否有起手动作
    public bool IsPrepareExist { get; protected set; }
    //起手动作名称
    public string PrepareMotion { get; protected set; }
    //起手动作被打断时需移除的特效列表
    public List<string> PrepareDestroyEffectList { get; protected set; }
    //是否有吟唱动作
    public bool IsSpellExist { get; protected set; }
    //吟唱动作名称
    public string SpellMotion { get; protected set; }
    //吟唱动作播放时间
    public float SpellTime { get; protected set; }
    //吟唱动作的特效列表
    public List<string> SpellEffectList { get; protected set; }
    //吟唱动作的特效位置列表
    public List<string> SpellEffectLocationList { get; protected set; }
    //吟唱动作被打断时需移除的特效列表
    public List<string> SpellDestroyEffectList { get; protected set; }
    //是否有冲刺动作
    public bool IsSlashExist { get; protected set; }
    //冲刺动作名称
    public string SlashMotion { get; protected set; }
    //冲刺动作位移距离
    public float SlashMotionDistance { get; protected set; }
    //冲刺动作-目标位置偏移量
    public float SlashTargetPosOffset { get; protected set; }
    //冲刺动作-混合动作名称
    public string SlashBlendMotionName { get; protected set; }
    //冲刺动作移动速度
    //public float SlashMotionSpeed { get; protected set; }
    //冲刺动作特效名称
    //public List<string> SlashEffectList { get; set; }
    //冲刺动作特效位置
    //public List<string> SlashEffectLocationList { get; set; }
    //冲刺动作被打断时需移除的特效列表
    public List<string> SlashDestroyEffectList { get; protected set; }
    //是否有释放动作
    public bool IsReleaseExist { get; protected set; }
    //释放动作名称
    public string ReleaseMotion { get; protected set; }
    //释放动作位移距离
    public float ReleaseMotionDistance { get; protected set; }
    //目标位置偏移量
    public float ReleaseTargetPosOffset { get; protected set; }
    //混合动作名称
    public string ReleaseBlendMotionName { get; protected set; }
    //释放动作被打断时需移除的特效列表
    public List<string> ReleaseDestroyEffectList { get; protected set; }
    //是否有引导动作
    public bool IsConductExist { get; protected set; }
    //引导动作名称
    public string ConductMotion { get; protected set; }
    //引导动作播放时间
    public float ConductTime { get; protected set; }
    //引导动作特效名称
    public List<string> ConductEffectList { get; protected set; }
    //引导动作特效位置
    public List<string> ConductEffectLocationList { get; protected set; }
    //引导动作被打断时需移除的特效列表
    public List<string> ConductDestroyEffectList { get; protected set; }
    //是否有引导结束动作
    public bool IsEndConductExist { get; protected set; }
    //引导结束动作
    public string EndConductMotion { get; protected set; }
    //引导结束动作被打断时需移除的特效列表
    public List<string> EndConductDestroyEffectList { get; protected set; }
    //固定伤害成长参数
    public float FixedDamageParam { get; protected set; }
    //物理倍数成长参数
    public float PhyDamageParam { get; protected set; }
    //魔法倍数成长参数
    public float MagDamageParam { get; protected set; }
    //百分比伤害成长参数
    public float PercentDamageParam { get; protected set; }
    //物理破甲伤害成长参数
    public float PhyPicerceDamageParam { get; protected set; }
    //魔法破甲伤害成长参数
    public float MagPicerceDamageParam { get; protected set; }
    //距离伤害成长参数
    public float PhyRangeDamageParam { get; protected set; }
    //魔法距离伤害成长参数
    public float MagRangeDamageParam { get; protected set; }
    //固定恢复成长参数
    public float FixedRecoverParam { get; protected set; }
    //百分比恢复成长参数
    public float PercentRecoverParam { get; protected set; }
    //物理攻击恢复成长参数 
    public float PhyRecoverParam { get; protected set; }
    //魔法攻击恢复成长参数 
    public float MagRecoverParam { get; protected set; }
    //斩杀血量百分比成长参数
    public float InstantKillHPPercentParam { get; protected set; }
    //斩杀几率成长参数
    public float InstantKillProbParam { get; protected set; }
    //吸血成长参数
    public float VampireParam { get; protected set; }
}




public class SkillTable
{
    public SkillInfo Lookup(int id)
    {
        SkillInfo info = null;
        SkillInfoList.TryGetValue(id, out info);
        return info;
    }
    public Dictionary<int,SkillInfo> SkillInfoList { get; protected set; }
    public void Load(byte[] bytes)
    {
        SkillInfoList = new Dictionary<int, SkillInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        for (int index = 0; index < length; ++index)
        {
            SkillInfo info = new SkillInfo();
            info.Load(helper);
            SkillInfoList.Add(info.ID, info);
        }
    }
}