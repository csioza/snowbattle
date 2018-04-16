using System;
using System.Collections.Generic;
using UnityEngine;

public enum ENRaceType
{
    None,
    Human,//人族
    Elves,//精灵
    Demon,//魔族
    Orcs,//兽族
    Dragon,//龙族
}
public class HeroInfo : IDataBase
{
    public enum ENOccupation
    {
        None,
        Soldier,//战士
        Assassin,//刺客
        Master,//法师
        Pastor,//牧师
    }
	public int		    ID			    { get; private set; }   //ID
	public string		StrName		    { get; private set; }   //名称
	public int		    ModelId		    { get; private set; }   //模型ID   
    public int          ImageId         { get; private set; }   //图片ID
    public int          headImageId     { get; private set; }   //头像ID
    public int          WeaponId        { get; private set; }   //武器模型ID
	public float		ModelScale		{ get; private set; }	//模型缩放
    public int          Type            { get; private set; }   //种族 ENRaceType
    public int          Occupation      { get; private set; }   //职业 ENOccupation
    public int          Cost            { get; private set; }   //领导力消耗
    public int          Quality         { get; private set; }   // 品质
    public int          Rarity          { get; private set; }   //星级
    public int          MaxRarity       { get; private set; }   //星级最大值
    public int          MaxLevel        { get; private set; }   //等级最大值
    public int          EvolveChangeID  { get; private set; }   //进化后对应卡牌id
	public float		FHPMax		    { get; private set; }   //血量上限
	public float		FPhyAttack	    { get; private set; }   //物理攻击
	public float		FPhyDefend	    { get; private set; }   //物理防御
	public float		FMagAttack	    { get; private set; }   //魔法攻击
	public float		FMagDefend	    { get; private set; }   //魔法防御
    public float        HitRate         { get; private set; }   //命中率
	public float		FAvoid		    { get; private set; }   //闪避
    public float        CritRate        { get; private set; }   //暴击率
    public float        CritParam       { get; private set; }   //暴击系数
    public float        AttrTypeID      { get; private set; }   //卡牌属性类型ID
    public float        levelTypeID     { get; private set; }   //卡牌等级成长类型ID
    public float        FResist         { get; private set; }   //抗性
    public float        WoundParam      { get; private set; }   //受伤系数
    public float        AnitInterfereRate{ get; private set; }  //抗普通受击概率
    public float        AnitInterruptRate{ get; private set; }  //抗强受击概率
    public float        AnitRepelRate   { get; private set; }   //抗击退概率
    public float        AnitLauncherRate{ get; private set; }   //抗击飞概率
	public float		AttackRange	    { get; private set; }   //搜索距离
	public float		AutoAttackRange { get; private set; }   //自动攻击范围
    public float        UnlockRange     { get; private set; }   //清队目标的距离
    public float        UnlockGazingRange  {get; private set;}  //解除注视范围
	public float		MoveSpeed	    { get; private set; }   //移动速度
	public float		MovebackSpeed   { get; private set; }   //后退移动速度
	public float		AnimationSpeed  { get; private set; }   //animation播放速度
    public int          StaminaMax      { get; private set; }   //强韧上限
    public float        StaminaRestoreValue     { get; private set; } 	//强韧恢复值
    public float        StaminaRestorePeriod    { get; private set; } 	//强韧恢复间隔
    public int          SwitchCD                { get; private set; }   //切换角色的cd
    public List<float>  SwitchBuffIDList        { get; private set; }   //角色离场时添加的buff列表
    public string       BreakThroughCardList    { get; private set; }   //突破需要卡牌列表
    public int          BreakThroughCount       { get; private set; }   //突破次数
    public int          BreakThroughLevel       { get; private set; }   //突破等级/次
    public List<int>    EvolveCardList          { get; private set; }   //进化需要卡牌列表 <卡牌ID，数量>
    public int          EvolveNeedLevel         { get; private set; }   //进化需要等级
    public int          EvolveSetLevel          { get; private set; }   //进化后等级
    public int          SwitchSkillID           { get; private set; }   //切入技
    public int          SwitchSkillCount        { get; private set; }   //切入技使用次数
    public int          SwordSoulSkillID        { get; private set; }   //剑魂技
    public List<int>    NormalSkillIDList       { get; private set; }   //普通攻击技能id列表
    public int          OpenBoxSkillID          { get; private set; }   //开宝箱技能
    public List<int>    AllSkillIDList          { get; private set; }   //所有的技能列表
    public List<int>    UnlockSkillLevelList    { get; private set; }   //主动技能解锁等级
    public List<int>    PassiveSkillIDList      { get; private set; }   //被动技能列表
    public List<int>    UnlockPassvieSkillLevelList { get; private set; }   //被动技能解锁等级
    public bool         IsTradable  { get; private set; }   //是否可以被出售 0:不可以
    public int          Price       { get; private set; }   //售价
    public int          Ring        { get; private set; }   //包含戒指
    public bool         IsEatable   { get; private set; }   //是否可以被吞噬 0:不可以
    public int          ExpSupply   { get; private set; }   //吞噬提供的经验
    public string       Description { get; private set; }   //背景描述
    public int          CardId      { get; private set; }   //卡牌ID用于图鉴

    //获得所有技能id列表
    public List<int> GetAllSkillIDList()
    {
        List<int> skillIDList = new List<int>();
        skillIDList.AddRange(AllSkillIDList);
        if (!skillIDList.Contains(OpenBoxSkillID))
        {
            skillIDList.Add(OpenBoxSkillID);
        }
        foreach (var item in NormalSkillIDList)
        {
            if (!skillIDList.Contains(item))
            {
                skillIDList.Add(item);
            }
        }
        return skillIDList;
    }
    // 获得普通技能解锁所需等级
    public int GetSkillNeedLevel(int skillId)
    {

        SkillInfo info = GameTable.SkillTableAsset.Lookup(skillId);
        if (null == info)
        {
            return 0;
        }

        List<int> list          = null;
        List<int> unlockList    = null;
        if (info.SkillType == (int)ENSkillType.enSkill)
        {
            list        = AllSkillIDList;
            unlockList  = UnlockSkillLevelList;
        }
        else if (info.SkillType == (int)ENSkillType.enPassive)
        {
            list        = PassiveSkillIDList;
            unlockList  = UnlockPassvieSkillLevelList;
        }

        if (list == null )
        {
            return 0;
        }

        int i = 0;
        foreach (int item in list)
        {

            if (item == skillId)
            {
                break;
            }
            i++;
        }
        // 如果没有找到技能ID 则 返回0
        if (i >= list.Count - 1)
        {
            return 0;
        }

        if (i < unlockList.Count)
        {
            return unlockList[i];
        }

        return 0;
    }
}
public class HeroInfoTable
{
	public Dictionary<int, HeroInfo> m_list { get; protected set; }
    public HeroInfo Lookup(int id)
	{
        HeroInfo info = null;
        m_list.TryGetValue(id, out info);
        return info;
	}
	public void Load(byte[] bytes)
	{
        m_list = new Dictionary<int, HeroInfo>();
		BinaryHelper helper = new BinaryHelper(bytes);
		int length = helper.ReadInt();
		for (int index = 0; index < length; ++index)
		{
            HeroInfo info = new HeroInfo();
			info.Load(helper);
            m_list.Add(info.ID, info);
		}
	}
};