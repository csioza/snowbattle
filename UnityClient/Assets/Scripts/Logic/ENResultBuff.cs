using UnityEngine;
using System.Collections;

public enum ENResult{
    None,
    Skill = 1,
    Damage = 2,
    AddBuff = 3,
    RemoveBuff = 4,
    Dead = 5,
    StaminaChanged = 6,//强韧度改变
    Relive = 7,
    OpenBox = 8,
    Health = 9,
    InstantDeath = 10,  //立即死亡
    SkillCD = 11,       //技能CDResult
    Taut = 12,//嘲讽
    Drag = 13,
    RefreshCD = 14,
    ChainDamage = 15,//连锁伤害
    Call = 16,//召唤
    Vampire = 17,//吸血
    AddExp = 18,//增加经验
}
public enum ENBuff {
    None,
    ControlMove = 1,//控制移动
    AddUndown = 2,//增加霸体
    NoDeadAddBuff = 3,//禁止死亡并且添加一个buff
    StaminaChanged = 4,//强韧度改变(触怒)
    Vampire = 5,//吸血
    ContinuedReduceHP = 6,//持续减血
    ChangeDamage = 7,//减少伤害
    ChangeRestore = 8,//减少恢复
    ContinuedRestoreHP = 9,//持续回血
    SkillSilence = 10,//技能沉默
    OffsetDamage = 11,//抵消伤害
    ControlAttack = 12,//控制攻击
    ControlBeAttack = 13,//控制受击
    ReturnDamage = 14,//反弹伤害
    DecreaseDamage = 15,//递减伤害
    ChangeModel = 16,//更改外观
    Halo = 17,//光环
    Chaos = 18,//混乱   
    ZeroCD = 19,    //技能CD跳过
    OutlawDeath = 20, //禁止死亡
    Fear = 21,//恐惧
    IgnoreDebuff = 22,//忽略debuff
    BreakWithDamage = 23,//受击打断
    Charm = 24,//魅惑
    Sneak = 25,//潜行
    FindSneak = 26,//反隐形（破除潜行）
    Teleport = 27,//瞬移
    Plague = 28,//瘟疫
    ChangeBuff = 29,//改变buff
    DamageSuperimposition = 30,//伤害叠加
    KillSuperimposition = 31,//击杀叠加
    HealthSuperimposition = 32,//给予叠加
    DamageRacial = 33,//种族伤害
}