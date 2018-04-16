using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ENBattleProperty
{
    static public int tableID = 1001;//对应表的id
    static public int isLive = 1002;//是否活着
    static public int positionX = 1003;//
    static public int positionY = 1004;//
    static public int skillIDList = 1005;//技能列表
    static public int camp = 1006;//阵营
    static public int level = 1007;//等级
    static public int hp = 1008;//血量
    static public int maxHP = 1009;//最大血量
    static public int name = 1010;//名字
    static public int runSpeed = 1011;//奔跑速度
    static public int backSpeed = 1012;//倒退速度
    static public int animationSpeed = 1013;//动作播放速度
    static public int ModelScal = 1014;//模型比例
    static public int phyAttack = 1015;//物理攻击力
    static public int phyDefend = 1016;//物理防御力
    static public int phyCrit = 1017;//物理暴击-->暂时没用
    static public int magAttack = 1018;//魔法攻击力
    static public int magDefend = 1019;//魔法防御力
    static public int magCrit = 1020;//魔法暴击-->暂时没用
    static public int hitRate = 1021;//命中率
    static public int avoidRate = 1022;//闪避率
    static public int CritRate = 1023;//暴击率
    static public int CritParam = 1024;//暴击系数
    static public int cdModifiedValue = 1025;//技能cd修正值
    static public int cdModifiedPercent = 1026;//技能cd修正百分比
    static public int Stamina = 1027;//强韧度
    static public int AnitInterfere = 1028;//扛普通受击
    static public int AnitInterrupt = 1029;//抗强受击
    static public int AnitRepel = 1030;//抗击退
    static public int AnitLauncher = 1031;//抗击飞
    static public int WoundParam = 1032;//受伤系数
    static public int FResist = 1033;//抗性
    static public int MaxStamina = 1034;//强韧度最大值
    static public int AttackAnimSpeed = 1035;//攻击animation播放速度
    static public int KillEnemy	= 1036 ;//杀敌数量
    static public int BeKilled	= 1037 ;//被杀数量
    static public int ReliveTime= 1038; //复活时间
    static public int IsActorExit = 1039;//角色是否退场
    static public int skillLevelList = 1040;//技能等级列表
    static public int Exp = 1041;//卡牌经验值
    static public int LevelUpPoint = 1042;//卡牌升级点数
    static public int YellowPointLevel= 1043;//黄点等级
}
public class ENProperty
{

	#region Singleton
	static ENProperty m_singleton;
	static public ENProperty Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new ENProperty();
			}
			return m_singleton;
		}
	}
	public ENProperty()
	{
		InitPropertyDefine();
		InitPlayerDefines();
		InitNpcDefines();
        InitTrapDefines();
	}
	#endregion

	PropertySetDefine m_define = new PropertySetDefine();
	Dictionary<string, PropertySetClass> m_namedClass = new Dictionary<string, PropertySetClass>();
	public PropertySetDefine Define
	{
		get { return m_define; }
	}
	//添加人物基本属性(无基础属性)

	//职业
	public static int vocationid { get; private set; }
	//经验最大值
	public static int expmax { get; private set; }
	//经验
	public static int exp { get; private set; }
	//金钱（人民币）
	public static int money { get; private set; }
    //金钱（虚拟币）
    public static int bindMoney { get; private set; }
	//最大体力值
	public static int maxenergy { get; private set; }
	//体力值
	public static int energy { get; private set; }
	public static int maxmp { get; private set; }
	public static int mp { get; private set; }
	public static int maxsp { get; private set; }
	public static int sp { get; private set; }
	public static int stillattack { get; private set; }
	public static int stilldefend { get; private set; }
	public static int mprecoverspeed { get; private set; }
	public static int spabsorbspeed { get; private set; }
	public static int damangehumanex { get; private set; }
	public static int damangeanimalex { get; private set; }
	public static int damangemonsterex { get; private set; }
	public static int damangeundeadex { get; private set; }
	public static int damangedragonex { get; private set; }
	public static int equipView { get; private set; }
	public static int NPC_IDInMap { get; private set; }
	//地图场景ID
	public static int mapID { get; private set; }
	//NPC目标ID
    public static int NPC_TargetID { get; private set; }
    //携带组队卷 added by wangxin
    public static int teamVolume { get; private set; }
    public static int maxTeamVolume { get; private set; }
    //更新体力值时间
    public static int energyTime { get; private set; }
    //技能ID
    public static int SkillID { get; private set; }
    public static int equipArray { get; private set; }
    //任务
	public static int MissionInstList { get; private set; }
	public static int FinishedMissionList { get; private set; }
	public static int teamID { get; private set; }
    public static int ItemMaxCount { get; private set; }    // 道具携带数量上限
    public static int skillView { get; private set; }    // 技能信息
    //public static int attackDistance { get; private set; }    // 攻击距离
    public static int finishDungeonList { get; private set; }
    //新手引导
    public static int isNewPlayer { get; private set; }
    //武器ID
    public static int WeaponID { get; private set; }
    //怪物类型（普通怪，精英怪,boss）
    public static int NpcType { get; private set; }
    

    ////////////////////服务器序列化的属性/////////////////////////////
    public static int IDInTable { get; private set; } //在表中的id
    public static int islive { get; private set; } //是否活着
    public static int positionX { get; private set; } //位置x    
    public static int positionZ { get; private set; } //位置z    
    public static int SkillIDList { get; private set; } //技能ID列表
    public static int SkillLevelList { get; private set; }//技能等级列表
    public static int camp { get; private set; } //阵营
    public static int level { get; private set; }//等级
    public static int hp { get; private set; }//生命
    public static int maxhp { get; private set; }//最大生命
    public static int name { get; private set; }//名称
    public static int runSpeed { get; private set; }//移动速度
    public static int MovebackSpeed { get; private set; }//后退移动速度
    public static int AnimationSpeed { get; private set; }//animation播放速度
    public static int ModelScale { get; private set; }//怪物模型比例
    public static int phyattack { get; private set; }//物理攻击
    public static int phydefend { get; private set; }//物理防御
    public static int magattack { get; private set; }//魔法攻击
    public static int magdefend { get; private set; }//魔法防御
    public static int hit { get; private set; }//命中率
    public static int avoid { get; private set; }//闪避率
    public static int crit { get; private set; }//暴击率
    public static int critParam { get; private set; }//暴击系数
    public static int SkillCDModifyValue { get; private set; }//所有技能cd的修正值
    public static int SkillCDModifyPercent { get; private set; }//所有技能cd的修正百分比
    public static int stamina { get; private set; }//强韧度
    public static int AnitInterfere { get; private set; }//抗普通受击
    public static int AnitInterrupt { get; private set; }//抗强受击
    public static int AnitRepel { get; private set; }//抗击退
    public static int AnitLauncher { get; private set; }//抗击飞
    public static int WoundParam { get; private set; }//受伤系数
    public static int FResist { get; private set; }//抗性
    public static int maxStamina { get; private set; }//强韧度最大值
    public static int AttackAnimSpeed { get; private set; }//攻击animation播放速度
	static public int KillEnemy { get; private set; }//杀敌数量
    static public int BeKilled { get; private set; }//被杀数量
    static public int ReliveTime { get; private set; } //复活时间
    public static int IsActorExit { get; private set; }//角色是否退场
    public static int Exp { get; private set; }//卡牌经验
    public static int LevelUpPoint { get; private set; }//卡牌升级点数
    public static int YellowPointLevel { get; private set; }//黄点等级
	void InitPropertyDefine()
    {
        ENProperty.IDInTable = m_define.AddProperty_Int32("NPC_IDInTable", 0, ENBattleProperty.tableID);
        ENProperty.islive = m_define.AddProperty_Int32("islive", 1, ENBattleProperty.isLive);
        ENProperty.positionX = m_define.AddProperty_Float("positionX", 0.0f, ENBattleProperty.positionX);
        ENProperty.positionZ = m_define.AddProperty_Float("positionZ", 0.0f, ENBattleProperty.positionY);
        ENProperty.SkillIDList = m_define.AddProperty_Buffer("SkillIDList", PropertyValueIntListViewFactory.Singleton, 10 * 4, ENBattleProperty.skillIDList);
        ENProperty.SkillLevelList = m_define.AddProperty_Buffer("SkillLevelList", PropertyValueIntListViewFactory.Singleton, 10 * 4, ENBattleProperty.skillLevelList);
        ENProperty.camp = m_define.AddProperty_Int32("camp", 0, ENBattleProperty.camp);
        ENProperty.level = m_define.AddProperty_Int32("level", 1, ENBattleProperty.level);
		ENProperty.hp = m_define.AddProperty_Float("hp", 0.0f, ENBattleProperty.hp);
		ENProperty.maxhp = m_define.AddProperty_Float("maxhp", 0.0f, ENBattleProperty.maxHP);
        ENProperty.name = m_define.AddProperty_String("name", "empty", 16, ENBattleProperty.name);
        ENProperty.runSpeed = m_define.AddProperty_Float("runSpeed", 1, ENBattleProperty.runSpeed);
        ENProperty.MovebackSpeed = m_define.AddProperty_Float("MovebackSpeed", 1, ENBattleProperty.backSpeed);
        ENProperty.AnimationSpeed = m_define.AddProperty_Float("AnimationSpeed", 1, ENBattleProperty.animationSpeed);
        ENProperty.ModelScale = m_define.AddProperty_Float("ModelScale", 1.0f, ENBattleProperty.ModelScal);//更改模型比例
        ENProperty.phyattack = m_define.AddProperty_Float("phyattack", 0, ENBattleProperty.phyAttack);
        ENProperty.phydefend = m_define.AddProperty_Float("phydefend", 30, ENBattleProperty.phyDefend);
        ENProperty.magattack = m_define.AddProperty_Float("magattack", 60, ENBattleProperty.magAttack);
        ENProperty.magdefend = m_define.AddProperty_Float("magdefend", 28, ENBattleProperty.magDefend);
        ENProperty.hit = m_define.AddProperty_Float("hit", 1, ENBattleProperty.hitRate);
        ENProperty.avoid = m_define.AddProperty_Float("avoid", 1, ENBattleProperty.avoidRate);
        ENProperty.crit = m_define.AddProperty_Float("crit", 1, ENBattleProperty.CritRate);
        ENProperty.critParam = m_define.AddProperty_Float("critParam", 1, ENBattleProperty.CritParam);
        ENProperty.SkillCDModifyValue = m_define.AddProperty_Float("SkillCDModifyValue", 0, ENBattleProperty.cdModifiedValue);
        ENProperty.SkillCDModifyPercent = m_define.AddProperty_Float("SkillCDModifyPercent", 0, ENBattleProperty.cdModifiedPercent);
        ENProperty.stamina = m_define.AddProperty_Float("stamina", 100.0f, ENBattleProperty.Stamina);
        ENProperty.AnitInterfere = m_define.AddProperty_Float("AnitInterfere", 1.0f, ENBattleProperty.AnitInterfere);
        ENProperty.AnitInterrupt = m_define.AddProperty_Float("AnitInterrupt", 1.0f, ENBattleProperty.AnitInterrupt);
        ENProperty.AnitRepel = m_define.AddProperty_Float("AnitRepel", 1.0f, ENBattleProperty.AnitRepel);
        ENProperty.AnitLauncher = m_define.AddProperty_Float("AnitLauncher", 1.0f, ENBattleProperty.AnitLauncher);
        ENProperty.WoundParam = m_define.AddProperty_Float("WoundParam", 1.0f, ENBattleProperty.WoundParam);
        ENProperty.FResist = m_define.AddProperty_Float("FResist", 1.0f, ENBattleProperty.FResist);
        ENProperty.maxStamina = m_define.AddProperty_Float("maxStamina", 100.0f, ENBattleProperty.MaxStamina);
        ENProperty.AttackAnimSpeed = m_define.AddProperty_Float("AttackAnimSpeed", 1.0f, ENBattleProperty.AttackAnimSpeed);
        ENProperty.KillEnemy = m_define.AddProperty_Int32("KillEnemy", 0, ENBattleProperty.KillEnemy);
        ENProperty.BeKilled = m_define.AddProperty_Int32("BeKilled", 0, ENBattleProperty.BeKilled);
        ENProperty.ReliveTime = m_define.AddProperty_Float("ReliveTime", 0, ENBattleProperty.ReliveTime);
        ENProperty.IsActorExit = m_define.AddProperty_Int32("IsActorExit", 1, ENBattleProperty.IsActorExit);
        ENProperty.Exp = m_define.AddProperty_Int32("Exp", 1, ENBattleProperty.Exp);
        ENProperty.LevelUpPoint = m_define.AddProperty_Int32("LevelUpPoint", 0, ENBattleProperty.LevelUpPoint);
        ENProperty.YellowPointLevel = m_define.AddProperty_Int32("YellowPointLevel", 0, ENBattleProperty.YellowPointLevel);
        vocationid = m_define.AddProperty_Int32("vocationid", 1);
		expmax = m_define.AddProperty_Int32("expmax", 1);
		exp = m_define.AddProperty_Int32("exp", 1);
		money = m_define.AddProperty_Int32("money", 1);
        bindMoney = m_define.AddProperty_Int32("bindMoney", 1);	
		maxenergy = m_define.AddProperty_Int32("maxenergy", 1);
		energy = m_define.AddProperty_Int32("energy", 1);
		maxmp = m_define.AddProperty_Int32("maxmp", 180);
		mp = m_define.AddProperty_Int32("mp", 180);
		maxsp = m_define.AddProperty_Int32("maxsp", 0);
		sp = m_define.AddProperty_Int32("sp", 0);
		stillattack = m_define.AddProperty_Int32("stillattack", 100);
		stilldefend = m_define.AddProperty_Int32("stilldefend", 100);

		ENProperty.mprecoverspeed = m_define.AddProperty_Int32("mprecoverspeed", 100);//当前MP恢复速度
		ENProperty.spabsorbspeed = m_define.AddProperty_Int32("spabsorbspeed", 100);//当前气力值恢复速度
		ENProperty.damangehumanex = m_define.AddProperty_Int32("damangehumanex", 100);//当前人形伤害加成
		ENProperty.damangeanimalex = m_define.AddProperty_Int32("damangeanimalex", 100);//当前野兽伤害加成
		ENProperty.damangemonsterex = m_define.AddProperty_Int32("damangemonsterex", 100);//当前魔兽伤害加成
		ENProperty.damangeundeadex = m_define.AddProperty_Int32("damangeundeadex", 100);//当前不死伤害加成
		ENProperty.damangedragonex = m_define.AddProperty_Int32("damangedragonex", 100);//当前龙类伤害加成
		ENProperty.equipView = m_define.AddProperty_Buffer("equipView", PropertyValueEquipViewFactory.Singleton, 4 * 4);//人物身上装备可视化信息
		ENProperty.NPC_IDInMap = m_define.AddProperty_Int32("NPC_IDInMap", 0);//NPC属性,在地图中的ID号,与ActorPropConfig.IDInMap对应
		ENProperty.mapID = m_define.AddProperty_Int32("mapID", 0);//	
		ENProperty.NPC_TargetID = m_define.AddProperty_Int32("NPC_TargetID", 0);
        ENProperty.SkillID = m_define.AddProperty_Int32("SkillID", 0);
        ENProperty.teamVolume = m_define.AddProperty_Int32("teamVolume", 0);
        ENProperty.maxTeamVolume = m_define.AddProperty_Int32("maxTeamVolume", 0);
        ENProperty.energyTime = m_define.AddProperty_Int32("energyTime", 0);
        ENProperty.equipArray = m_define.AddProperty_Buffer("equipArray", PropertyValueEquipArrayFactory.Singleton, 36*40);
        ENProperty.MissionInstList = m_define.AddProperty_Buffer("MissionInstList", PropertyValueMissionListFactory.Singleton, 480);
        ENProperty.FinishedMissionList = m_define.AddProperty_Buffer("FinishedMissionList", PropertyValueFinishedMissionListFactory.Singleton, 512);
		ENProperty.teamID = m_define.AddProperty_Int32("teamID", 0);
        ENProperty.ItemMaxCount = m_define.AddProperty_Int32("ItemMaxCount", 60);   // 道具携带数量上限, 默认 60
        ENProperty.skillView = m_define.AddProperty_Buffer("SkillView", PropertyValueSkillViewFactory.Singleton, 12 * 30);   // 道具携带数量上限, 默认 60
        //ENProperty.attackDistance = m_define.AddProperty_Float("attackDistance", 1);//	
        ENProperty.finishDungeonList = m_define.AddProperty_Buffer("finishDungeonList", PropertyValueFinishDungeonListFactory.Singleton, 50);
        ENProperty.isNewPlayer = m_define.AddProperty_Int32("isNewPlayer", 1);//新手标记
        ENProperty.WeaponID = m_define.AddProperty_Int32("WeaponType", 0);
        ENProperty.NpcType = m_define.AddProperty_Int32("NpcType", 0);//NPC类型
        
    }

	void InitPlayerDefines()
	{
		//int saveAndViewable = (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave;
		PropertySetClass theClass = new PropertySetClass(m_define);
		m_namedClass["SOBPlayer"] = theClass;
		theClass.AddProperty(vocationid, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(expmax, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(exp, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(money, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(bindMoney, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(ENProperty.SkillID, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(ENProperty.equipView, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(ENProperty.equipArray, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(ENProperty.MissionInstList, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(ENProperty.FinishedMissionList, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(ENProperty.teamID, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(ENProperty.ItemMaxCount, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(ENProperty.skillView, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(ENProperty.isNewPlayer, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
       
		Init_AddCommonBattleProperty(theClass);
	}

	void Init_AddCommonBattleProperty(PropertySetClass theClass)
	{
        int saveAndViewable = (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave | (int)ENPropertyNotifyPipe.enNotifyPipeAround;
        theClass.AddProperty(ENProperty.IDInTable, saveAndViewable);
        theClass.AddProperty(ENProperty.islive, saveAndViewable);
        theClass.AddProperty(ENProperty.positionX, saveAndViewable);
        theClass.AddProperty(ENProperty.positionZ, saveAndViewable);
        theClass.AddProperty(ENProperty.SkillIDList, saveAndViewable);
        theClass.AddProperty(ENProperty.SkillLevelList, saveAndViewable);
        theClass.AddProperty(ENProperty.camp, saveAndViewable);
        theClass.AddProperty(ENProperty.level, saveAndViewable);
        theClass.AddProperty(ENProperty.hp, saveAndViewable);
        theClass.AddProperty(ENProperty.maxhp, saveAndViewable);
        theClass.AddProperty(ENProperty.name, saveAndViewable);
        theClass.AddProperty(ENProperty.runSpeed, saveAndViewable);
        theClass.AddProperty(ENProperty.MovebackSpeed, saveAndViewable);
        theClass.AddProperty(ENProperty.AnimationSpeed, saveAndViewable);
        theClass.AddProperty(ENProperty.ModelScale, saveAndViewable);
        theClass.AddProperty(ENProperty.phyattack, saveAndViewable);
        theClass.AddProperty(ENProperty.phydefend, saveAndViewable);
        theClass.AddProperty(ENProperty.magattack, saveAndViewable);
        theClass.AddProperty(ENProperty.magdefend, saveAndViewable);
        theClass.AddProperty(ENProperty.hit, saveAndViewable);
        theClass.AddProperty(ENProperty.avoid, saveAndViewable);
        theClass.AddProperty(ENProperty.crit, saveAndViewable);
        theClass.AddProperty(ENProperty.critParam, saveAndViewable);
        theClass.AddProperty(ENProperty.SkillCDModifyValue, saveAndViewable);
        theClass.AddProperty(ENProperty.SkillCDModifyPercent, saveAndViewable);
        theClass.AddProperty(ENProperty.stamina, saveAndViewable);
        theClass.AddProperty(ENProperty.AnitInterfere, saveAndViewable);
        theClass.AddProperty(ENProperty.AnitInterrupt, saveAndViewable);
        theClass.AddProperty(ENProperty.AnitRepel, saveAndViewable);
        theClass.AddProperty(ENProperty.AnitLauncher, saveAndViewable);
        theClass.AddProperty(ENProperty.WoundParam, saveAndViewable);
        theClass.AddProperty(ENProperty.FResist, saveAndViewable);
        theClass.AddProperty(ENProperty.maxStamina, saveAndViewable);
        theClass.AddProperty(ENProperty.AttackAnimSpeed, saveAndViewable);
        theClass.AddProperty(ENProperty.KillEnemy, saveAndViewable);
        theClass.AddProperty(ENProperty.BeKilled, saveAndViewable);
        theClass.AddProperty(ENProperty.ReliveTime, saveAndViewable);
        theClass.AddProperty(ENProperty.IsActorExit, saveAndViewable);
        theClass.AddProperty(ENProperty.Exp, saveAndViewable);
        theClass.AddProperty(ENProperty.LevelUpPoint, saveAndViewable);
        theClass.AddProperty(ENProperty.YellowPointLevel, saveAndViewable);
		theClass.AddProperty(maxenergy, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(energy, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(maxmp, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(mp, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(maxsp, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(sp, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(stillattack, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(stilldefend, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(ENProperty.mprecoverspeed, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(ENProperty.spabsorbspeed, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(ENProperty.damangehumanex, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(ENProperty.damangeanimalex, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(ENProperty.damangemonsterex, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(ENProperty.damangeundeadex, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(ENProperty.damangedragonex, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(ENProperty.mapID, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(ENProperty.teamVolume, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(ENProperty.maxTeamVolume, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(ENProperty.energyTime, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        //theClass.AddProperty(ENProperty.attackDistance, saveAndViewable);
        theClass.AddProperty(ENProperty.finishDungeonList, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(ENProperty.WeaponID, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(ENProperty.NpcType, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
	}

	void InitNpcDefines()
	{
		int saveAndViewable = (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave | (int)ENPropertyNotifyPipe.enNotifyPipeAround;
		PropertySetClass theClass = new PropertySetClass(m_define);
		m_namedClass["SOBNpc"] = theClass;
		Init_AddCommonBattleProperty(theClass);
		theClass.AddProperty(ENProperty.NPC_IDInMap, saveAndViewable);
		theClass.AddProperty(ENProperty.NPC_TargetID, saveAndViewable);
	}

    void InitTrapDefines()
    {
        //int saveAndViewable = (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave | (int)ENPropertyNotifyPipe.enNotifyPipeAround;
        PropertySetClass theClass = new PropertySetClass(m_define);
        m_namedClass["SOBTrap"] = theClass;
        //Init_AddCommonBattleProperty(theClass);
//         theClass.AddProperty(ENProperty.NPC_IDInMap, saveAndViewable);
//         theClass.AddProperty(ENProperty.NPC_TargetID, saveAndViewable);
        theClass.AddProperty(ENProperty.islive, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(ENProperty.phyattack, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(ENProperty.hit, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
    }

	public PropertySetClass LookupClass(string name)
	{
		if (m_namedClass.ContainsKey(name))
		{
			return m_namedClass[name];
		}
		return null;
	}
	public PropertySet CreatePropertySet(string name)
	{
		PropertySetClass theClass = LookupClass(name);
		if (theClass != null)
		{
			return theClass.CreateInstance();
		}
		return null;
	}
}
