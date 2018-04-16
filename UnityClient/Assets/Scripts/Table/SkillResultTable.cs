//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Table
//	created:	2013-5-17
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResultParam
{
	//效果ID
	public int ID { get; private set; }
	//效果参数 5个
    public float[] Param { get; private set; }

	public ResultParam()
	{
        Param = new float[5];
	}

	public void Load(BinaryHelper helper)
	{
		ID = helper.ReadInt();
		for (int index = 0; index < Param.Length; ++index)
		{
			Param[index] = helper.ReadFloat();
		}
	}

	public void Save(BinaryHelper helper)
	{
		helper.Write(ID);
		foreach (float param in Param)
		{
			helper.Write(param);
		}
	}
}
//额外的combo效果
public class ExtraComboResultParam
{
    public int ComboJudgeCount { get; private set; }
    public string ComboTips { get; private set; }
    public int SkillResultID { get; private set; }

    public void Load(BinaryHelper helper)
    {
        ComboJudgeCount = helper.ReadInt();
        ComboTips = helper.ReadString();
        SkillResultID = helper.ReadInt();
    }
    public void Save(BinaryHelper helper)
    {
        helper.Write(ComboJudgeCount);
        helper.Write(ComboTips);
        helper.Write(SkillResultID);
    }
}

public enum ENSkillComboType
{
    enNone,
    enMore,//多段轻攻击
    enSome,//中段中攻击
    enLittle,//少段重攻击
}
public enum ENResultTargetType
{
    enNone,
    enEnemySingle,//敌人单体
    enEnemyAll,//敌人全体
    enFriendlySingle,//友方单体
    enFriendlyAll,//友方全体
    enEveryone,//所有人
    enSelf,//自己
    enFriendlyAllAndSelf,//友方全体和自己
    enFriendlySingleAndSelf,//友方单体和自己
}
public enum ENHpChangeType
{
    enNone,
    enPhyDamage,    //物理
    enMagDamage,    //魔法
    enHeal,         //治疗
}
public class SkillResultInfo
{
    public int ID { get; private set; }
    //说明
    public string Description { get; private set; }
    //立即生效的result 半径
    public float InstantRange { get; private set; }
    //目标类型
    public int ResultTargetType { get; private set; }//ENResultTargetType
	//命中率
    //public float HitPersent { get; private set; }
    //暴击率
    //public float CritPersent { get; private set; }
    //普通受击几率
    public float InterferePersent { get; private set; }
	//强受击几率
	public float BreakPersent { get; private set; }
	//击退几率
	public float HitBackPersent { get; private set; }
	//击飞几率
    public float HitFlyPersent { get; private set; }
    //combo类型
    public int ComboType { get; private set; }//ENSkillComboType
    //ComboNum
    public int ComboNum { get; private set; }
    //连击时间
    public float ComboTime { get; private set; }
    //强韧削减
    public float StaminaDamage { get; private set; }
    //非意志燃烧SkillResult增益
    public float SFightWillGet { get; private set; }
    //普通SkillResult增益
    public float NFightWillGet { get; private set; }
    //是否削减敌人的combo时间
    public bool IsWeakenComboTime { get; private set; }
    //削减敌人的combo时间
    public float WeakenComboTime { get; private set; }
    //伤害类型 ENHpChangeType
    public int HpChangeType { get; private set; }
    //动画力度参数
    public float AnimGauge { get; private set; }
    //生效后是否播放特效
    public bool IsPlayEffect { get; private set; }
    //特效名称
    public string EffectName { get; private set; }
    //特效播放位置
    public string EffectPos { get; private set; }
    //特效播放时间
    public float EffectDuration { get; private set; }
    //特效是否挂在骨骼下
    public bool IsAheadBone { get; private set; }
    //生效后是否改变目标shader
    public bool IsChangeShader { get; private set; }
    //shader名称
    //public string ShaderName { get; private set; }
    //shader参数
    //public float ShaderParam { get; private set; }
    //变化至该值所用时间
    //public float ShaderChangeTimer { get; private set; }
    //恢复所用时间
    //public float ShaderRestoreTimer { get; private set; }
    //生效后是否改变目标shader的颜色
    //public bool IsChangeShaderColor { get; private set; }
    //Shader颜色名称
    //public string ShaderColorName { get; private set; }
    //Shader颜色参数
    //public string ShaderColorParam { get; private set; }
    //Shader颜色持续时间
    //public float ShaderColorDuration { get; private set; }
    //改变Shader的animation名称
    public string ChangeShaderAnimName { get; private set; }
    //声音列表
    public string SoundList { get; private set; }
    //combo生效最小值
    public int MinComboCount { get; private set; }
    //物理攻击加成
    public float PhyAttackAdd { get; private set; }
    //魔法攻击加成
    public float MagAttackAdd { get; private set; }
    //攻击力倍数的修正
    public float AttackModify { get; private set; }
	//效果列表
    public ResultParam[] ParamList { get; private set; }
    //额外combo效果列表
    public ExtraComboResultParam[] ExtraParamList { get; private set; }

	public void Load(BinaryHelper helper)
	{
		ID = helper.ReadInt();
        Description = helper.ReadString();
        InstantRange = helper.ReadFloat();
        ResultTargetType = helper.ReadInt();
        //HitPersent = helper.ReadFloat();
        //CritPersent = helper.ReadFloat();
        InterferePersent = helper.ReadFloat();
		BreakPersent = helper.ReadFloat();
		HitBackPersent = helper.ReadFloat();
		HitFlyPersent = helper.ReadFloat();
        ComboType = helper.ReadInt();
        ComboNum    = helper.ReadInt();
        ComboTime = helper.ReadFloat();
        StaminaDamage = helper.ReadFloat();
        SFightWillGet = helper.ReadFloat();
        NFightWillGet = helper.ReadFloat();
        IsWeakenComboTime = helper.ReadBool();
        WeakenComboTime = helper.ReadFloat();
        HpChangeType = helper.ReadInt();
        AnimGauge = helper.ReadFloat();
        IsPlayEffect = helper.ReadBool();
        EffectName = helper.ReadString();
        EffectPos = helper.ReadString();
        EffectDuration = helper.ReadFloat();
        IsAheadBone = helper.ReadBool();
        IsChangeShader = helper.ReadBool();
        //ShaderName = helper.ReadString();
        //ShaderParam = helper.ReadFloat();
        //ShaderChangeTimer = helper.ReadFloat();
        //ShaderRestoreTimer = helper.ReadFloat();
        //IsChangeShaderColor = helper.ReadBool();
        //ShaderColorName = helper.ReadString();
        //ShaderColorParam = helper.ReadString();
        //ShaderColorDuration = helper.ReadFloat();
        ChangeShaderAnimName = helper.ReadString();
        SoundList = helper.ReadString();
        MinComboCount = helper.ReadInt();
        PhyAttackAdd = helper.ReadFloat();
        MagAttackAdd = helper.ReadFloat();
        AttackModify = helper.ReadFloat();
		int count = helper.ReadInt();
		ParamList = new ResultParam[count];
		for (int index = 0; index < count; ++index)
		{
			ParamList[index] = new ResultParam();
			ParamList[index].Load(helper);
		}
        count = helper.ReadInt();
        ExtraParamList = new ExtraComboResultParam[count];
        for (int i = 0; i < count; ++i)
        {
            ExtraParamList[i] = new ExtraComboResultParam();
            ExtraParamList[i].Load(helper);
        }
        //ComboJudgeList = ConvertorTool.StringToArray_Int(helper.ReadString());
        //ComboResultIDList = ConvertorTool.StringToArray_Int(helper.ReadString());
	}

	public void Save(BinaryHelper helper)
	{
        helper.Write(ID);
        helper.Write(Description);
        helper.Write(InstantRange);
        helper.Write(ResultTargetType);
        //helper.Write(HitPersent);
        //helper.Write(CritPersent);
        helper.Write(InterferePersent);
		helper.Write(BreakPersent);
		helper.Write(HitBackPersent);
		helper.Write(HitFlyPersent);
        helper.Write(ComboType);
        helper.Write(ComboNum);
        helper.Write(ComboTime);
        helper.Write(StaminaDamage);
        helper.Write(SFightWillGet);
        helper.Write(NFightWillGet);
        helper.Write(IsWeakenComboTime);
        helper.Write(WeakenComboTime);
        helper.Write(HpChangeType);
        helper.Write(AnimGauge);
        helper.Write(IsPlayEffect);
        helper.Write(EffectName);
        helper.Write(EffectPos);
        helper.Write(EffectDuration);
        helper.Write(IsAheadBone);
        helper.Write(IsChangeShader);
        //helper.Write(ShaderName);
        //helper.Write(ShaderParam);
        //helper.Write(ShaderChangeTimer);
        //helper.Write(ShaderRestoreTimer);
        //helper.Write(IsChangeShaderColor);
        //helper.Write(ShaderColorName);
        //helper.Write(ShaderColorParam);
        //helper.Write(ShaderColorDuration);
        helper.Write(ChangeShaderAnimName);
        helper.Write(SoundList);
        helper.Write(MinComboCount);
        helper.Write(PhyAttackAdd);
        helper.Write(MagAttackAdd);
        helper.Write(AttackModify);
		helper.Write(ParamList.Length);
		foreach (ResultParam param in ParamList)
		{
			param.Save(helper);
        }
        helper.Write(ExtraParamList.Length);
        foreach (var item in ExtraParamList)
        {
            item.Save(helper);
        }
        //helper.Write(ConvertorTool.ArrayToString(ComboJudgeList));
        //helper.Write(ConvertorTool.ArrayToString(ComboResultIDList));
	}
}

public class SkillResultTable
{
	public SkillResultInfo Lookup(int id)
	{
		SkillResultInfo info;
		SkillResultList.TryGetValue(id, out info);
		return info;
	}

	public Dictionary<int, SkillResultInfo> SkillResultList { get; protected set; }
	public void Load(byte[] bytes)
	{
		BinaryHelper helper = new BinaryHelper(bytes);
		int length = helper.ReadInt();
		SkillResultList = new Dictionary<int, SkillResultInfo>(length);
		for (int index = 0; index < length; ++index)
		{
			SkillResultInfo info = new SkillResultInfo();
			info.Load(helper);
			SkillResultList.Add(info.ID, info);
		}
	}
};