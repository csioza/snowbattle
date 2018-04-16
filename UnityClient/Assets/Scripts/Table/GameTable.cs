//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Table
//	created:	2013-5-14
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;


public class GameTable
{
    static public QTESequenceTable QTESequenceTableAsset { get; private set; }
    static public FlyingItemTable FlyingItemTableAsset { get; private set; }
    static public StageTable StageTableAsset { get; private set; }
    static public RoomAttrTable RoomAttrTableAsset { get; private set; }
	static public SkillTable SkillTableAsset { get; private set; }
    static public VocationTable VocationTableAsset { get; private set; }
    static public NPCInfoTable NPCInfoTableAsset { get; private set; }
    static public TrapInfoTable TrapInfoTableAsset { get; private set; }
    static public HeroInfoTable HeroInfoTableAsset { get; private set; }
	//static public AnimWeightTable AnimWeightTableAsset { get; private set; }
	static public SceneInfoTable SceneInfoTableAsset { get; private set; }
	//static public DungeonInfoTable DungeonInfoTableAsset { get; private set; }
	static public SkillResultTable SkillResultTableAsset { get; private set; }
	static private AnimationTable AnimationTownAsset { get; set; }
	static private AnimationTable AnimationFightAsset { get; set; }
    static public AnimationTable AnimationTableAsset { get; private set; }
    static public ActionRelationTable ActionRelationTableAsset { get; private set; }
    static public TrapActionRelationTable trapActionRelationTableAsset { get; private set; }
    static public EquipTable EquipTableAsset { get; private set; }
    static public ModelInfoTable ModelInfoTableAsset { get; private set; }
    static public WeaponInfoTable WeaponInfoTableAsset { get; private set; }
    static public UILoadInfoTable UILoadInfoTableAsset { get; private set; }
    static public IconTable IconTableAsset { get; private set; }
    static public BuffTable BuffTableAsset { get; private set; }
    static public BuffRelationTable BuffRelationTableAsset { get; private set; }
    static public BuffEffectTable BuffEffectTableAsset { get; private set; }
    //static public CheckInfoTable CheckInfoTableAsset { get; private set; }
    //static public NpcSayTable NpcSayTableAsset { get; private set; }
    //static public ShopTable ShopTableAsset { get; private set; }

    static public MissionTable MissionTableAsset { get; private set; }
    static public AptitudeTable AptitudeTableAsset { get; private set; }
    static public StringTable StringTableAsset { get; private set; }
    static public WorldParamTable WorldParamTableAsset { get; private set; }
    static public CDTable CDTableAsset { get; private set; }
    //static public ServerTable ServerTableAsset { get; private set; }
    //static public SceneMapNumericTable SceneMapNumericTableAsset { get; private set; }
    //static public EquipExpMoneyTable EquipExpMoneyTableAsset { get; private set; }
    //static public ShakeTable ShakeTableAsset { get; private set; }
    //static public PlayerGuideTable PlayerGuideTableAsset { get; private set; }
    //static public IllumeTable IllumeTableAsset { get; private set; }
    //static public BossTable BossTableAsset { get; private set; }
    //static public SandTableInfoTable SandTableInfoTableAsset { get; private set; }

    //单机化相关
    static public LevelUpTable LevelUpTableAsset { get; private set; }
    //static public DungeonEventTable DungeonEventTableAsset { get; private set; }
    //static public DungeonEventResultTable DungeonEventResultTableAsset { get; private set; }
    //static public DungeonFiles DungeonFilesAsset { get; private set; }

 
    static public RarityRelativeTable RarityRelativeAsset { get; private set; }
    static public OccupationInfoTable OccupationInfoAsset { get; private set; }

    static public PlayerRandomNameTable PlayerRandomNameAsset { get; private set; }

    static public ZoneInfoTable ZoneInfoTableAsset { get; private set; }
    static public StageInfoTable StageInfoTableAsset { get; private set; }
    static public FloorInfoTable FloorInfoTableAsset { get; private set; }
    static public ItemTable ItemTableAsset { get; private set; }
    static public FloorRankTable floorRankTableAsset { get; private set; }

    static public ScoreParamTable ScoreParamTableAsset { get; private set; }

    static public BagTable BagTableAsset { get; private set; }
    static public RaceInfoTable RaceInfoTableAsset { get; private set; }
    static public EquipmentTable EquipmentTableAsset { get; private set; }

    static public PlayerAttrTable playerAttrTableAsset { get; private set; }

    static public LoadingTipsTable loadingTipsAsset { get; private set; }
    static public SceneRoomTable SceneRoomTableAsset { get; private set; }
    static public SceneBridgeTable SceneBridgeTableAsset { get; private set; }
    static public SceneGateTable SceneGateTableAsset { get; private set; }
    static public SceneTeleportTable SceneTeleportTableAsset { get; private set; }

    static public MagicStoneTable MagicStoneTableAsset { get; private set; }

    static public RingExchangeTable RingExchangeTableAsset { get; private set; }

	static public MessageRespondTable MessageRespondTableAsset { get; private set; }

    static public IconInfoTable IconInfoTableAsset { get; private set; }

    static public AttrRatioTable attrRatioTableAsset { get; private set; }
    static public ComboSwordSoulTable ComboSwordSoulAsset { get; private set; }

    static public EventItemTable eventItemAsset { get; private set; }

    static public GradeUpRequireTable gradeUpRequireAsset { get; private set; }
    
    static public QualityRelativeTable qualityRelativeAsset { get; private set; }

    static public CardTypeVariationTable cardTypeVariationAsset { get; private set; }

    static public CardLevelVariationTable cardLevelVariationAsset { get; private set; }

    static public YellowPointParamTable yellowPointParamAsset { get; private set; }


    static public void LoadTable()
	{
        if (null == QTESequenceTableAsset)
        {
            TextAsset asset = GameData.LoadConfig<TextAsset>("QTESequenceTable");
            QTESequenceTableAsset = new QTESequenceTable();
            QTESequenceTableAsset.Load(asset.bytes);
        }
        if (null == FlyingItemTableAsset)
        {
            TextAsset asset = GameData.LoadConfig<TextAsset>("FlyingObjBehaviorTable");
            FlyingItemTableAsset = new FlyingItemTable();
            FlyingItemTableAsset.Load(asset.bytes);
        }
        if (null == StageTableAsset)
        {
            TextAsset asset = GameData.LoadConfig<TextAsset>("StageTable");
            StageTableAsset = new StageTable();
            StageTableAsset.Load(asset.bytes);
        }
        if (null == RoomAttrTableAsset)
        {
            TextAsset asset = GameData.LoadConfig<TextAsset>("RoomAttrTable");
            RoomAttrTableAsset = new RoomAttrTable();
            RoomAttrTableAsset.Load(asset.bytes);
        }
		if (null == SkillTableAsset)
		{
            TextAsset asset = GameData.LoadConfig<TextAsset>("SkillTable");
			SkillTableAsset = new SkillTable();
			SkillTableAsset.Load(asset.bytes);
		}
		if (null == VocationTableAsset)
		{
            TextAsset asset = GameData.LoadConfig<TextAsset>("VocationTable");
			VocationTableAsset = new VocationTable();
			VocationTableAsset.Load(asset.bytes);
		}
		if (null == NPCInfoTableAsset)
		{
            TextAsset asset = GameData.LoadConfig<TextAsset>("NPCInfoTable");
			NPCInfoTableAsset = new NPCInfoTable();
			NPCInfoTableAsset.Load(asset.bytes);
        }
        if (null == TrapInfoTableAsset)
		{
            TextAsset asset = GameData.LoadConfig<TextAsset>("TrapInfoTable");
            TrapInfoTableAsset = new TrapInfoTable();
            TrapInfoTableAsset.Load(asset.bytes);
        }
        
        if (null == HeroInfoTableAsset)
        {
            TextAsset asset = GameData.LoadConfig<TextAsset>("HeroInfoTable");
            HeroInfoTableAsset = new HeroInfoTable();
            HeroInfoTableAsset.Load(asset.bytes);
        }
        //if (null == AnimWeightTableAsset)
        //{
        //    TextAsset asset = GameData.LoadConfig<TextAsset>("AnimWeight");
        //    AnimWeightTableAsset = new AnimWeightTable();
        //    AnimWeightTableAsset.Load(asset.bytes);
        //}
		if (null == SceneInfoTableAsset)
		{
            TextAsset asset = GameData.LoadConfig<TextAsset>("SceneInfoTable");
			SceneInfoTableAsset = new SceneInfoTable();
			SceneInfoTableAsset.Load(asset.bytes);
		}
        //if (null == DungeonInfoTableAsset)
        //{
        //    TextAsset asset = GameData.LoadConfig<TextAsset>("DungeonInfoTable");
        //    DungeonInfoTableAsset = new DungeonInfoTable();
        //    DungeonInfoTableAsset.Load(asset.bytes);
        //}
		if (null == SkillResultTableAsset)
		{
            TextAsset asset = GameData.LoadConfig<TextAsset>("SkillResultTable");
			SkillResultTableAsset = new SkillResultTable();
			SkillResultTableAsset.Load(asset.bytes);
		}
		if (null == AnimationTownAsset)
		{
			TextAsset asset = GameData.LoadConfig<TextAsset>("AnimationTown");
			AnimationTownAsset = new AnimationTable();
			AnimationTownAsset.Load(asset.bytes);
		}
		if (null == AnimationFightAsset)
		{
			TextAsset asset = GameData.LoadConfig<TextAsset>("AnimationFight");
			AnimationFightAsset = new AnimationTable();
			AnimationFightAsset.Load(asset.bytes);
		}
		AnimationTableAsset = AnimationFightAsset;
		if (null == ActionRelationTableAsset)
		{
			TextAsset asset = GameData.LoadConfig<TextAsset>("ActionRelation");
			ActionRelationTableAsset = new ActionRelationTable();
			ActionRelationTableAsset.Load(asset.bytes);
		}
        if (null == EquipTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("EquipBaseData");
            EquipTableAsset = new EquipTable();
            EquipTableAsset.Load(obj.bytes);
        }
		if (null == ModelInfoTableAsset)
		{
			TextAsset obj = GameData.LoadConfig<TextAsset>("ModelInfoTable");
			ModelInfoTableAsset = new ModelInfoTable();
			ModelInfoTableAsset.Load(obj.bytes);
		}
        if (null == WeaponInfoTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("WeaponInfoTable");
            WeaponInfoTableAsset = new WeaponInfoTable();
            WeaponInfoTableAsset.Load(obj.bytes);
        }
        if (null == UILoadInfoTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("UILoadInfoTable");
            UILoadInfoTableAsset = new UILoadInfoTable();
            UILoadInfoTableAsset.Load(obj.bytes);
        }
		if (null == IconTableAsset)
		{
			TextAsset obj = GameData.LoadConfig<TextAsset>("IconTable");
			IconTableAsset = new IconTable();
			IconTableAsset.Load(obj.bytes);
		}
		if (null == BuffTableAsset)
		{
			TextAsset obj = GameData.LoadConfig<TextAsset>("Buff");
			BuffTableAsset = new BuffTable();
			BuffTableAsset.Load(obj.bytes);
        }
        if (null == BuffRelationTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("BuffReplaceRelation");
            BuffRelationTableAsset = new BuffRelationTable();
            BuffRelationTableAsset.Load(obj.bytes);
        }
        if (null == BuffEffectTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("BuffEffect");
            BuffEffectTableAsset = new BuffEffectTable();
            BuffEffectTableAsset.Load(obj.bytes);
        }
        //if (null == CheckInfoTableAsset)
        //{
        //    TextAsset obj = GameData.LoadConfig<TextAsset>("Checkinfo");
        //    CheckInfoTableAsset = new CheckInfoTable();
        //    CheckInfoTableAsset.Load(obj.bytes);
        //}
        //if (null == NpcSayTableAsset)
        //{
        //    TextAsset obj = GameData.LoadConfig<TextAsset>("NpcSay");
        //    NpcSayTableAsset = new NpcSayTable();
        //    NpcSayTableAsset.Load(obj.bytes);
        //}

        //if (null == ShopTableAsset)
        //{
        //    TextAsset obj = GameData.LoadConfig<TextAsset>("NpcShop");
        //    ShopTableAsset = new ShopTable();
        //    ShopTableAsset.Load(obj.bytes);
        //}

        if (null == MissionTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("Task");
            MissionTableAsset = new MissionTable();
            MissionTableAsset.Load(obj.bytes);
        }

        if (null == AptitudeTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("Aptitude");
            AptitudeTableAsset = new AptitudeTable();
            AptitudeTableAsset.Load(obj.bytes);
        }

        if (null == StringTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("StringTable");
            StringTableAsset = new StringTable();
            StringTableAsset.Load(obj.bytes);
        }
        

        if (null == WorldParamTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("WorldParamTable");
            WorldParamTableAsset = new WorldParamTable();
            WorldParamTableAsset.Load(obj.bytes);
        }
        if (null == CDTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("CDTable");
            CDTableAsset = new CDTable();
            CDTableAsset.Load(obj.bytes);
        }
        //if (null == ServerTableAsset)
        //{
        //    TextAsset obj = GameData.LoadConfig<TextAsset>("Servers");
        //    ServerTableAsset = new ServerTable();
        //    ServerTableAsset.Load(obj.bytes);
        //}
        //if (null == SceneMapNumericTableAsset)
        //{
        //    TextAsset obj = GameData.LoadConfig<TextAsset>("Coordinate");
        //    SceneMapNumericTableAsset = new SceneMapNumericTable();
        //    SceneMapNumericTableAsset.Load(obj.bytes);
        //}
        //if (null == EquipExpMoneyTableAsset)
        //{
        //    TextAsset obj = GameData.LoadConfig<TextAsset>("EquipExpMoney");
        //    EquipExpMoneyTableAsset = new EquipExpMoneyTable();
        //    EquipExpMoneyTableAsset.Load(obj.bytes);
        //}
        //if (null == ShakeTableAsset)
        //{
        //    TextAsset obj = GameData.LoadConfig<TextAsset>("Shake");
        //    ShakeTableAsset = new ShakeTable();
        //    ShakeTableAsset.Load(obj.bytes);
        //}
        //if (null == PlayerGuideTableAsset)
        //{
        //    TextAsset obj = GameData.LoadConfig<TextAsset>("PlayerGuide");
        //    PlayerGuideTableAsset = new PlayerGuideTable();
        //    PlayerGuideTableAsset.Load(obj.bytes);
        //}
        //if (null == IllumeTableAsset)
        //{
        //     TextAsset obj = GameData.LoadConfig<TextAsset>("Illume");
        //     IllumeTableAsset = new IllumeTable();
        //     IllumeTableAsset.Load(obj.bytes);
        //}
        //if (null == BossTableAsset)
        //{
        //     TextAsset obj = GameData.LoadConfig<TextAsset>("BOSS");
        //     BossTableAsset = new BossTable();
        //     BossTableAsset.Load(obj.bytes);
        //}
        //if (null == SandTableInfoTableAsset)
        //{
        //    TextAsset obj = GameData.LoadConfig<TextAsset>("SandTableInfoTable");
        //     SandTableInfoTableAsset = new SandTableInfoTable();
        //     SandTableInfoTableAsset.Load(obj.bytes);
        //}
        
        if (null == LevelUpTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("LvUpExp");
            LevelUpTableAsset = new LevelUpTable();
            LevelUpTableAsset.Load(obj.bytes);
        }

        //if (null == DungeonEventTableAsset)
        //{
        //    TextAsset obj = GameData.LoadConfig<TextAsset>("DungeonEvent");
        //    DungeonEventTableAsset = new DungeonEventTable();
        //    DungeonEventTableAsset.Load(obj.bytes);
        //}

        //if (null == DungeonEventResultTableAsset)
        //{
        //    TextAsset obj = GameData.LoadConfig<TextAsset>("DungeonEventResult");
        //    DungeonEventResultTableAsset = new DungeonEventResultTable();
        //    DungeonEventResultTableAsset.Load(obj.bytes);
        //}

        //if (null == DungeonFilesAsset)
        //{
        //    TextAsset obj = GameData.LoadConfig<TextAsset>("DungeonFiles");
        //    DungeonFilesAsset = new DungeonFiles();
        //    DungeonFilesAsset.Load(obj.bytes);
        //}
        if (null == RarityRelativeAsset)
        {

            TextAsset obj = GameData.LoadConfig<TextAsset>("RarityRelative");
            RarityRelativeAsset = new RarityRelativeTable();
            RarityRelativeAsset.Load(obj.bytes);

        }
        if (null == OccupationInfoAsset)
        {

            TextAsset obj = GameData.LoadConfig<TextAsset>("OccupationInfoTable");
            OccupationInfoAsset = new OccupationInfoTable();
            OccupationInfoAsset.Load(obj.bytes);

        }
       
        if (null == PlayerRandomNameAsset)
        {

            TextAsset obj           = GameData.LoadConfig<TextAsset>("PlayerRandomName");
            PlayerRandomNameAsset= new PlayerRandomNameTable();
            PlayerRandomNameAsset.Load(obj.bytes);

        }

        if (null == ZoneInfoTableAsset)
        {

            TextAsset obj = GameData.LoadConfig<TextAsset>("ZoneInfo");
            ZoneInfoTableAsset = new ZoneInfoTable();
            ZoneInfoTableAsset.Load(obj.bytes);

        }

        if (null == StageInfoTableAsset)
        {

            TextAsset obj = GameData.LoadConfig<TextAsset>("StageInfo");
            StageInfoTableAsset = new StageInfoTable();
            StageInfoTableAsset.Load(obj.bytes);

        }

        if (null == FloorInfoTableAsset)
        {

            TextAsset obj = GameData.LoadConfig<TextAsset>("FloorInfo");
            FloorInfoTableAsset = new FloorInfoTable();
            FloorInfoTableAsset.Load(obj.bytes);

        }

        if (null == ItemTableAsset)
        {

            TextAsset obj = GameData.LoadConfig<TextAsset>("Item");
            ItemTableAsset = new ItemTable();
            ItemTableAsset.Load(obj.bytes);

        }

        if (null == floorRankTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("FloorRankTable");
            floorRankTableAsset = new FloorRankTable();
            floorRankTableAsset.Load(obj.bytes);
        }

        if (null == ScoreParamTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("ScoreParamTable");
            ScoreParamTableAsset = new ScoreParamTable();
            ScoreParamTableAsset.Load(obj.bytes);
        }

        if ( null == BagTableAsset )
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("BagTable");
            BagTableAsset = new BagTable();
            BagTableAsset.Load(obj.bytes);
        }

        if ( null == RaceInfoTableAsset )
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("RaceInfoTable");
            RaceInfoTableAsset = new RaceInfoTable();
            RaceInfoTableAsset.Load(obj.bytes);
        }

        if ( null == EquipmentTableAsset )
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("Equipment");
            EquipmentTableAsset = new EquipmentTable();
            EquipmentTableAsset.Load(obj.bytes);
        }

        if (null == playerAttrTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("PlayerAttrTable");
            playerAttrTableAsset = new PlayerAttrTable();
            playerAttrTableAsset.Load(obj.bytes);
        }
        if ( null == loadingTipsAsset )
        {
            TextAsset obj       = GameData.LoadConfig<TextAsset>("LoadingTips");
            loadingTipsAsset    = new LoadingTipsTable();
            loadingTipsAsset.Load(obj.bytes);
        }

        if (null == MagicStoneTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("MagicStonePrice");
            MagicStoneTableAsset = new MagicStoneTable();
            MagicStoneTableAsset.Load(obj.bytes);
        }

        if (null == RingExchangeTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("RingExchangeTable");
            RingExchangeTableAsset = new RingExchangeTable();
            RingExchangeTableAsset.Load(obj.bytes);
        }

//         if (null == SceneRoomTableAsset)
//         {
//             TextAsset obj = GameData.LoadConfig<TextAsset>("SceneRoomTable");
//             SceneRoomTableAsset = new SceneRoomTable();
//             SceneRoomTableAsset.Load(obj.bytes);
//         }
//         if (null == SceneBridgeTableAsset)
//         {
//             TextAsset obj = GameData.LoadConfig<TextAsset>("SceneBridgeTable");
//             SceneBridgeTableAsset = new SceneBridgeTable();
//             SceneBridgeTableAsset.Load(obj.bytes);
//         }
//         if (null == SceneGateTableAsset)
//         {
//             TextAsset obj = GameData.LoadConfig<TextAsset>("SceneGateTable");
//             SceneGateTableAsset = new SceneGateTable();
//             SceneGateTableAsset.Load(obj.bytes);
//         }
//         if (null == SceneTeleportTableAsset)
//         {
//             TextAsset obj = GameData.LoadConfig<TextAsset>("SceneTeleportTable");
//             SceneTeleportTableAsset = new SceneTeleportTable();
//             SceneTeleportTableAsset.Load(obj.bytes);
//         }
		if (null == MessageRespondTableAsset)
		{
			TextAsset obj = GameData.LoadConfig<TextAsset>("MessageRespondTable");
			MessageRespondTableAsset = new MessageRespondTable();
			MessageRespondTableAsset.Load(obj.bytes);
		}

        if (null == IconInfoTableAsset)
        {
            TextAsset obj       = GameData.LoadConfig<TextAsset>("Icon");
            IconInfoTableAsset  = new IconInfoTable();
            IconInfoTableAsset.Load(obj.bytes);
        }

        if (null == attrRatioTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("AttrRatioTable");
            attrRatioTableAsset = new AttrRatioTable();
            attrRatioTableAsset.Load(obj.bytes);
        }

        if (null == ComboSwordSoulAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("ComboSwordSoulTable");
            ComboSwordSoulAsset = new ComboSwordSoulTable();
            ComboSwordSoulAsset.Load(obj.bytes);
        }

        if (null == eventItemAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("EventItem");
            eventItemAsset = new EventItemTable();
            eventItemAsset.Load(obj.bytes);
        }

        if (null == gradeUpRequireAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("GradeUpRequireTable");
            gradeUpRequireAsset = new GradeUpRequireTable();
            gradeUpRequireAsset.Load(obj.bytes);
        }

        if (null == qualityRelativeAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("QualityRelativeTable");
            qualityRelativeAsset = new QualityRelativeTable();
            qualityRelativeAsset.Load(obj.bytes);
        }


        if (null == cardTypeVariationAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("CardTypeVariationTable");
            cardTypeVariationAsset = new CardTypeVariationTable();
            cardTypeVariationAsset.Load(obj.bytes);
        }

        if (null == cardLevelVariationAsset )
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("CardLevelupVariationTable");
            cardLevelVariationAsset = new CardLevelVariationTable();
            cardLevelVariationAsset.Load(obj.bytes);
        }

        if (null == yellowPointParamAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("YellowPointParam");
            yellowPointParamAsset = new YellowPointParamTable();
            yellowPointParamAsset.Load(obj.bytes);
        }
        //加载随机地图相关表数据
        LoadRandMapTableData();

        int id = 70;
        int level = 12;
        int yellow = 3;
        int hp = BattleFormula.GetHp(id, level, yellow);
        int phyAttack = BattleFormula.GetPhyAttack(id, level, yellow);
        int magAttack = BattleFormula.GetMagAttack(id, level, yellow);
        int magDefend = BattleFormula.GetMagDefend(id, level, yellow);
        int phyDEFEND = BattleFormula.GetPhyDefend(id, level, yellow);

        Debug.Log("magDefend:" + magDefend);
        Debug.Log("magAttack:" + magAttack);
        Debug.Log("phyDEFEND:" + phyDEFEND);
        Debug.Log("phyAttack:" + phyAttack);
        Debug.Log("hp:" + hp);

    }
    static public void LoadRandMapTableData()
    {
        if (null == SceneRoomTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("SceneRoomTable");
            SceneRoomTableAsset = new SceneRoomTable();
            SceneRoomTableAsset.Load(obj.bytes);
        }
        if (null == SceneBridgeTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("SceneBridgeTable");
            SceneBridgeTableAsset = new SceneBridgeTable();
            SceneBridgeTableAsset.Load(obj.bytes);
        }
        if (null == SceneGateTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("SceneGateTable");
            SceneGateTableAsset = new SceneGateTable();
            SceneGateTableAsset.Load(obj.bytes);
        }
        if (null == SceneTeleportTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("SceneTeleportTable");
            SceneTeleportTableAsset = new SceneTeleportTable();
            SceneTeleportTableAsset.Load(obj.bytes);
        }

        if (null == NPCInfoTableAsset)
        {
            TextAsset asset = GameData.LoadConfig<TextAsset>("NPCInfoTable");
            NPCInfoTableAsset = new NPCInfoTable();
            NPCInfoTableAsset.Load(asset.bytes);
        }
        if (null == TrapInfoTableAsset)
        {
            TextAsset asset = GameData.LoadConfig<TextAsset>("TrapInfoTable");
            TrapInfoTableAsset = new TrapInfoTable();
            TrapInfoTableAsset.Load(asset.bytes);
        }
        if (null == ModelInfoTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("ModelInfoTable");
            ModelInfoTableAsset = new ModelInfoTable();
            ModelInfoTableAsset.Load(obj.bytes);
        }

    }
	static public void SwitchAnimationTable(string tableName)
	{
		switch (tableName)
		{
			case "AnimationTown":
				AnimationTableAsset = AnimationTownAsset;
				break;
			case "AnimationFight":
				AnimationTableAsset = AnimationFightAsset;
				break;
            //新的沙盘表
            case "AnimationWorld":
                //新表
                AnimationTableAsset = AnimationTownAsset;
                break;
			default:
				break;
		}
		
	}
};