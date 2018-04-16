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

public enum NpcFuncType
{
    NPC_SHOP    = 1,    // 商店
    NPC_MISSION = 2,    // NPC任务
    NPC_EQUIP   = 3,    // 装备强化
}

public enum ENNpcType
{
    enNone = 0,
    enCommonNPC = 1,//普通怪物NPC
    enFunctionNPC = 2,//功能NPC
    enBOSSNPC = 3,//BOSS
    enBoxNPC = 4,//木箱类NPC（互动物件）
	enEliteNPC = 5,//精英NPC
    enBlockNPC = 6,//障碍NPC
    enMobaNpc = 7,//moba小兵
    enMobaTower = 8,//moba箭塔
    Count,
}

public enum ENNpcInterSubType
{
    enNone = 0,
    enTreasureBox = 1,// 宝箱
    Count,
}
public enum ENNPCAIType
{
    enAITypeNone,
    enAITypeNormal,
    enAITypeBlock,
}
public enum ENStaminaEventType
{
    enNone,
    enBeHitFly,//被击飞
    enFireSkill,//释放技能
}


// NPC 功能描述:类型, 参数
public class NpcFuncArg
{
    public int FuncType { get;  set; }
    public int FuncParam { get;  set; }

	public void Load(BinaryHelper helper)
	{
		FuncType  = helper.ReadInt();
        FuncParam = helper.ReadInt();
	}

#if UNITY_EDITOR
	public void Save(BinaryHelper helper)
	{
		helper.Write(FuncType);
		helper.Write(FuncParam);
	}
#endif
}

public class NPCInfo
{
	public int		    ID			    { get; private set; }   //NPCID
	public string		StrName		    { get; private set; }   //名称
	public int		    ModelId		    { get; private set; }   //模型ID
	public float		ModelScale		{ get; private set; }	//模型缩放
    public int          WeaponID        { get; private set; }   //武器ID
    public int          RaceType        { get; private set; }   //种族，ENRaceType
    public bool         IsPush          { get; private set; }   //是否可以推动 0：不可推动 1：可以推动
    public int          AIType          { get; private set; }   //AI类型  ENNPCAIType
    public int          Type            { get; private set; }   //类型    ENNpcType
    public string       BossAIXmlName   { get; private set; }     //BOSSNpc读取的XML文件名
    public string       BossAIXmlSubName { get; private set; }     //BOSSNpc读取的XML文件中段落的名称
    public int          InterSubType    { get; private set; }   //交互型细类   ENNpcInterSubType
    public int          NpcSay          { get; private set; }   // NPC 聊天内容ID
	public int		    Level			{ get; private set; }	//等级
	public float		HPMax		    { get; private set; }   //血量
    public int          HPCount         { get; private set; }   //血条段数（只用于boss）
	public float		PhyAttack	    { get; private set; }   //物理攻击
	public float		PhyDefend	    { get; private set; }   //物理防御
	public float		MagAttack	    { get; private set; }   //魔法攻击
	public float		MagDefend	    { get; private set; }   //魔法防御
    public float        HitRate         { get; private set; }   //命中率
	public float		Avoid		    { get; private set; }   //闪避
    public float        CritRate        { get; private set; }   //暴击率
    public float        CritParam       { get; private set; }   //暴击系数
    public float        Resist          { get; private set; }   //抗性
    public float        WoundParam      { get; private set; }   //受伤系数
    public float        AnitInterfereRate{get; private set; }   //抗普通受击概率
    public float        AnitInterruptRate{get; private set; }   //抗强受击概率
    public float        AnitRepelRate   { get; private set; }   //抗击退概率
    public float        AnitLauncherRate{ get; private set; }   //抗击飞概率
    public float        RotateSpeed     { get; private set; }   //移动转身速率
    public float        SwitchRotateSpeed { get; private set; } //攻击旋转速率
	public float		MoveSpeed	    { get; private set; }   //移动速度
	public float		AnimationSpeed  { get; private set; }   //animation播放速度
	public float		AttackRange	    { get; private set; }   //攻击范围
	public float		CallNpcRange    { get; private set; }   //呼叫同伴范围
	public float 		AlertRange		{ get; private set; }	//警戒范围
	public float 		CancelAlertRange{ get; private set; }	//取消警戒的范围
	public float		AlertPeriod		{ get; private set; } 	//警戒时间
	public float		LargeRate		{ get; private set; } 	//膨胀速率
	public float		SmallRate		{ get; private set; } 	//冷却速率
	public float 		VisionRange		{ get; private set; } 	//追击最大间距
	public float 		MaxChaseRange	{ get; private set; } 	//最大追击距离
    public float        RetreatDistance { get; private set; }   //后退间距
    public float        RetreatSpeed    { get; private set; }   //后退速度
    public float        RetreatTime     { get; private set; }   //后退时间
    public int          StaminaMax      { get; private set; }   //强韧上限
    public float        StaminaRestoreValue { get; private set; } 	//强韧恢复值
    public float        StaminaRestorePeriod { get; private set; } 	//强韧恢复间隔
    public int          StaminaEvent    { get; private set; }   //强韧度等于0后的事件 ENStaminaEventType
    public int          StaminaSkillID  { get; private set; }   //强韧事件的技能id
    public float        StaminaReset    { get; private set; }   //强韧度等于0后的恢复百分比
    public List<float>  GiftBuffIDList  { get; private set; }           //出生时携带的buffID列表
	public int		    DropID	        { get; private set; }   //掉落
    public List<int>    SkillList       { get; private set; }           //技能列表
    public List<int>    PassiveSkillList{ get; private set; }           //被动技能列表
    public int functionNum { get; private set; }   //功能个数
    public List<NpcFuncArg> NpcFuncArgs { get; set; }           //NPC 功能列表
    public NpcFuncArg Lookup(int nType)
	{
        if(NpcFuncArgs != null)
		    return NpcFuncArgs.Find(item => item.FuncType == nType);
        return null;
	}

    public bool HasFunc(int nType)
    {
        return (null != Lookup(nType));
    }

	public void Load(BinaryHelper helper)
	{
		ID = helper.ReadInt();
        StrName = helper.ReadString();
        ModelId = helper.ReadInt();
		ModelScale = helper.ReadFloat();
        WeaponID = helper.ReadInt();
        RaceType = helper.ReadInt();
        IsPush = helper.ReadBool();
        AIType = helper.ReadInt();
	    Type = helper.ReadInt();
        BossAIXmlName = helper.ReadString();
        BossAIXmlSubName = helper.ReadString();
        InterSubType = helper.ReadInt();
        NpcSay = helper.ReadInt();
	    Level = helper.ReadInt();
	    HPMax = helper.ReadFloat();
        HPCount = helper.ReadInt();
	    PhyAttack = helper.ReadFloat();
	    MagAttack = helper.ReadFloat();
	    PhyDefend = helper.ReadFloat();
	    MagDefend = helper.ReadFloat();
        HitRate = helper.ReadFloat();
	    Avoid = helper.ReadFloat();
        CritRate = helper.ReadFloat();
        CritParam = helper.ReadFloat();
        Resist = helper.ReadFloat();
        WoundParam = helper.ReadFloat();
        AnitInterfereRate = helper.ReadFloat();
        AnitInterruptRate = helper.ReadFloat();
        AnitRepelRate = helper.ReadFloat();
        AnitLauncherRate = helper.ReadFloat();
        RotateSpeed = helper.ReadFloat();
        SwitchRotateSpeed = helper.ReadFloat();
	    MoveSpeed = helper.ReadFloat();
        AnimationSpeed = helper.ReadFloat();
        AttackRange = helper.ReadFloat();
        CallNpcRange = helper.ReadFloat();
		AlertRange = helper.ReadFloat ();
        CancelAlertRange = helper.ReadFloat();
		AlertPeriod = helper.ReadFloat ();
        LargeRate = helper.ReadFloat();
        SmallRate = helper.ReadFloat();
		VisionRange = helper.ReadFloat ();
		MaxChaseRange = helper.ReadFloat ();
        RetreatDistance = helper.ReadFloat();
        RetreatSpeed = helper.ReadFloat();
        RetreatTime = helper.ReadFloat();
        StaminaMax = helper.ReadInt();
        StaminaRestoreValue = helper.ReadFloat();
        StaminaRestorePeriod = helper.ReadFloat();
        StaminaEvent = helper.ReadInt();
        StaminaSkillID = helper.ReadInt();
        StaminaReset = helper.ReadFloat();
        GiftBuffIDList = ConvertorTool.StringToList_Float(helper.ReadString());
        DropID = helper.ReadInt();
        SkillList = ConvertorTool.StringToList_Int(helper.ReadString());
        PassiveSkillList = ConvertorTool.StringToList_Int(helper.ReadString());
        functionNum = helper.ReadInt();      // NpcFuncArgs 的数量
        if (functionNum > 0)
        {
            NpcFuncArgs = new List<NpcFuncArg>();
            for (int i = 0; i < functionNum; ++i)
            {
                NpcFuncArg val = new NpcFuncArg();
                val.Load(helper);
                NpcFuncArgs.Add(val);
            }
        }
        
	}

#if UNITY_EDITOR
	public void Save(BinaryHelper helper)
	{
		helper.Write(ID);
        helper.Write(StrName);
        helper.Write(ModelId);
		helper.Write(ModelScale);
        helper.Write(WeaponID);
        helper.Write(RaceType);
        helper.Write(IsPush);
        helper.Write(AIType);
	    helper.Write(Type);
        helper.Write(BossAIXmlName);
        helper.Write(BossAIXmlSubName);
        helper.Write(InterSubType);
        helper.Write(NpcSay);
	    helper.Write(Level);
	    helper.Write(HPMax);
        helper.Write(HPCount);
	    helper.Write(PhyAttack);
	    helper.Write(MagAttack);
	    helper.Write(PhyDefend);
	    helper.Write(MagDefend);
        helper.Write(HitRate);
	    helper.Write(Avoid);
        helper.Write(CritRate);
        helper.Write(CritParam);
        helper.Write(Resist);
        helper.Write(WoundParam);
        helper.Write(AnitInterfereRate);
        helper.Write(AnitInterruptRate);
        helper.Write(AnitRepelRate);
        helper.Write(AnitLauncherRate);
        helper.Write(RotateSpeed);
        helper.Write(SwitchRotateSpeed);
	    helper.Write(MoveSpeed);
        helper.Write(AnimationSpeed);
        helper.Write(AttackRange);
        helper.Write(CallNpcRange);
		helper.Write(AlertRange);
        helper.Write(CancelAlertRange);
		helper.Write(AlertPeriod);
        helper.Write(LargeRate);
        helper.Write(SmallRate);
		helper.Write(VisionRange);
		helper.Write(MaxChaseRange);
        helper.Write(RetreatDistance);
        helper.Write(RetreatSpeed);
        helper.Write(RetreatTime);
        helper.Write(StaminaMax);
        helper.Write(StaminaRestoreValue);
        helper.Write(StaminaRestorePeriod);
        helper.Write(StaminaEvent);
        helper.Write(StaminaSkillID);
        helper.Write(StaminaReset);
        helper.Write(ConvertorTool.ListToString(GiftBuffIDList));
        helper.Write(DropID);
        helper.Write(ConvertorTool.ListToString(SkillList));
        helper.Write(ConvertorTool.ListToString(PassiveSkillList));
        helper.Write(functionNum);
        if(NpcFuncArgs != null)
			functionNum = NpcFuncArgs.Count;
        if (functionNum > 0)
        {
            foreach (NpcFuncArg val in NpcFuncArgs)
            {
                val.Save(helper);
            }
        }
	}
#endif
}

public class NPCInfoTable
{
	public Dictionary<int, NPCInfo> m_list { get; protected set; }
	public NPCInfo Lookup(int id)
	{
        NPCInfo info = null;
        m_list.TryGetValue(id, out info);
        return info;
	}
    public bool NpcHasFunc(int id, int nType)   // true: 挂有 nType 对应的功能类型
    {
        NPCInfo info = Lookup(id);
        if (info != null)
        {
            return info.HasFunc(nType);
        }
        return false;
    }
	public void Load(byte[] bytes)
	{
        m_list = new Dictionary<int, NPCInfo>();
		BinaryHelper helper = new BinaryHelper(bytes);
		int length = helper.ReadInt();
		for (int index = 0; index < length; ++index)
		{
			NPCInfo info = new NPCInfo();
			info.Load(helper);
            m_list.Add(info.ID, info);
		}
	}
};