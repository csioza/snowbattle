//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Editor
//	created:	2013-5-9
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

public class EditorTableTools
{
	public static readonly string[] TableFolder = new string[] { "Assets/Editor/Table/" };
	public static readonly string[] TargetFolder = new string[] { "Assets/Resources/Config/" };
    [@MenuItem(@"Table Convert/QTE序列表(QTESequenceTable)")]
    public static void ConvertQTESequenceTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "QTESequenceTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                QTESequenceTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "QTESequenceTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/场景Teleport表(SceneTeleportTable)")]
    public static void ConvertSceneTeleportTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "SceneTeleportTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                SceneTeleportTableLoader.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "SceneTeleportTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/场景Room表(SceneRoomTable)")]
    public static void ConvertSceneRoomTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "SceneRoomTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                SceneRoomTableLoader.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "SceneRoomTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }
    [@MenuItem(@"Table Convert/场景Bridge表(SceneBridgeTable)")]
    public static void ConvertSceneBridgeTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "SceneBridgeTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                SceneBridgeTableLoader.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "SceneBridgeTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }
    [@MenuItem(@"Table Convert/场景Gate表(SceneGateTable)")]
    public static void ConvertSceneGateTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "SceneGateTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                SceneGateTableLoader.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "SceneGateTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }
    [@MenuItem(@"Table Convert/飞行道具表(FlyingItemTable)")]
    public static void ConvertFlyingItemTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "FlyingObjBehaviorTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                FlyingItemTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "FlyingObjBehaviorTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }
    [@MenuItem(@"Table Convert/stage属性表(StageTable)")]
    public static void ConvertStageTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
			using (FileStream file = new FileStream(TableFolder[index] + "StageTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                StageTableLoader.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "StageTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }
    [@MenuItem(@"Table Convert/Room属性表(RoomAttrTable)")]
    public static void ConvertRoomAttrTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "RoomAttrTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                RoomAttrTableLoader.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "RoomAttrTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }
	[@MenuItem(@"Table Convert/角色行为关系表(ActionRelation)")]
	public static void ConvertActionRelationTable()
	{
		for (int index = 0; index < TableFolder.Length; ++index)
		{
			using (FileStream file = new FileStream(TableFolder[index] + "ActionRelation.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				HSSFWorkbook workbook = new HSSFWorkbook(file);
				ActionRelationTableLoader.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "ActionRelation.bytes");
			}
        }
        AssetDatabase.Refresh();
	}

	[@MenuItem(@"Table Convert/场景表(SceneInfo)")]
	public static void ConvertSceneInfoTable()
	{
		for (int index = 0; index < TableFolder.Length; ++index)
		{
			SceneInfoTableLoader tableLoader = new SceneInfoTableLoader();
			tableLoader.Convert(TableFolder[index] + "SceneInfo.xls", TargetFolder[index] + "SceneInfoTable.bytes");
        }
        AssetDatabase.Refresh();
	}

    //[@MenuItem(@"Table Convert/副本表")]
    //public static void ConvertDungeonInfoTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        DungeonInfoTableConvertor tableLoader = new DungeonInfoTableConvertor();
    //        tableLoader.Convert(TableFolder[index] + "DungeonInfo.xls", TargetFolder[index] + "DungeonInfoTable.bytes");
    //    }
    //}


	[@MenuItem(@"Table Convert/技能表(SkillTable)")]
	public static void ConvertSkillTable()
	{
		for (int index = 0; index < TableFolder.Length; ++index)
		{
			using (FileStream file = new FileStream(TableFolder[index] + "SkillTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				HSSFWorkbook workbook = new HSSFWorkbook(file);
				SkillTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "SkillTable.bytes");
			}
		}
        AssetDatabase.Refresh();
	}

    [@MenuItem(@"Table Convert/任务(MissionTable)")]
    public static void ConvertMissionTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "Task.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                MissionTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "Task.bytes");
            }
        }
        AssetDatabase.Refresh();
    }
	[@MenuItem(@"Table Convert/职业表(VocationTable)")]
	public static void ConvertVocationTable()
	{
		for (int index = 0; index < TableFolder.Length; ++index)
		{
			using (FileStream file = new FileStream(TableFolder[index] + "VocationTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				HSSFWorkbook workbook = new HSSFWorkbook(file);
				VocationTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "VocationTable.bytes");
			}
        }
        AssetDatabase.Refresh();
	}
	[@MenuItem(@"Table Convert/NPC表(NPCInfoTable)")]
	public static void ConvertNPCInfoTable()
	{
		for (int index = 0; index < TableFolder.Length; ++index)
		{
			using (FileStream file = new FileStream(TableFolder[index] + "NPCInfoTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				HSSFWorkbook workbook = new HSSFWorkbook(file);
				NPCInfoTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "NPCInfoTable.bytes");
			}
        }
        AssetDatabase.Refresh();
	}
    [@MenuItem(@"Table Convert/Trap表(TrapInfoTable)")]
    public static void ConvertTrapInfoTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "TrapInfoTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                TrapInfoTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "TrapInfoTable.bytes");
            }
        }
		AssetDatabase.Refresh();
	}
    [@MenuItem(@"Table Convert/卡牌表(HeroInfoTable)")]
    public static void ConvertHeroInfoTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "HeroInfoTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                HeroInfoTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "HeroInfoTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }
    [@MenuItem(@"Table Convert/武器表(WeaponInfoTable)")]
    public static void ConvertWeaponInfoTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "WeaponTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                WeaponInfoTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "WeaponInfoTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }
    [@MenuItem(@"Table Convert/加载的UI资源(UILoadInfoTable)")]
    public static void ConvertUILoadInfoTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "UILoadInfoTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                UILoadInfoTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "UILoadInfoTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }
    //[@MenuItem("Table Convert/动画权重规则表(AnimWeight)")]
    //public static void ConvertAnimWeightTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        using (FileStream file = new FileStream(TableFolder[index] + "AnimWeight.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //        {
    //            HSSFWorkbook workbook = new HSSFWorkbook(file);
    //            AnimWeightTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "AnimWeight.bytes");
    //        }
    //    }
    //}
	[@MenuItem("Table Convert/技能效果表(SkillResultTable)")]
	public static void ConvertSkillResultTable()
	{
		for (int index = 0; index < TableFolder.Length; ++index)
		{
			using (FileStream file = new FileStream(TableFolder[index] + "SkillResultTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				HSSFWorkbook workbook = new HSSFWorkbook(file);
				SkillResultTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "SkillResultTable.bytes");
			}
        }
        AssetDatabase.Refresh();
	}

    //[@MenuItem("Table Convert/装备资质表(Aptitude)")]
    //public static void ConvertSkillAptitudeTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        using (FileStream file = new FileStream(TableFolder[index] + "Aptitude.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //        {
    //            HSSFWorkbook workbook = new HSSFWorkbook(file);
    //            SkillAptitudeTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "Aptitude.bytes");
    //        }
    //    }
    //}
    [@MenuItem(@"Table Convert/装备列表")]
    public static void ConverEquipBaseTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "EquipBase.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                EquipBaseTableLoader.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "EquipBaseData.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

	[@MenuItem(@"Table Convert/动画表(ActInfo)")]
	public static void ConverAnimationTable()
	{
		for (int index = 0; index < TableFolder.Length; ++index)
		{
			using (FileStream file = new FileStream(TableFolder[index] + "ActInfo.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				HSSFWorkbook workbook = new HSSFWorkbook(file);
				AnimationTableLoader.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "AnimationTown.bytes");
				AnimationTableLoader.Convert(workbook.GetSheetAt(1), TargetFolder[index] + "AnimationFight.bytes");
			}
        }
        AssetDatabase.Refresh();
	}
	[@MenuItem(@"Table Convert/Model表(Model)")]
	public static void ConvertModelInfoTable()
	{
		for (int index = 0; index < TableFolder.Length; ++index)
		{
			using (FileStream file = new FileStream(TableFolder[index] + "Model.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				HSSFWorkbook workbook = new HSSFWorkbook(file);
				ModelInfoTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "ModelInfoTable.bytes");
			}
        }
        AssetDatabase.Refresh();
	}
	[@MenuItem(@"Table Convert/图标图片表(IconTable)")]
	public static void ConvertIconTable()
	{
		for (int index = 0; index < TableFolder.Length; ++index)
		{
			using (FileStream file = new FileStream(TableFolder[index] + "IconTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				HSSFWorkbook workbook = new HSSFWorkbook(file);
				IconTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "IconTable.bytes");
			}
        }
        AssetDatabase.Refresh();
	}
    
	[@MenuItem(@"Table Convert/Buff表(Buff)")]
	public static void ConvertBuffTable()
	{
		for (int index = 0; index < TableFolder.Length; ++index)
		{
			using (FileStream file = new FileStream(TableFolder[index] + "Buff.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				HSSFWorkbook workbook = new HSSFWorkbook(file);
				BuffTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "Buff.bytes");
			}
        }
        AssetDatabase.Refresh();
	}

    [@MenuItem(@"Table Convert/Buff关系表(BuffReplaceRelation)")]
    public static void ConvertBuffRelationTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "BuffReplaceRelation.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                BuffRelationTableLoader.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "BuffReplaceRelation.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/Buff特效表(BuffEffect)")]
    public static void ConvertBuffEffectTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "BuffEffect.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                BuffEffectTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "BuffEffect.bytes");
            }
        }
        AssetDatabase.Refresh();
    }
    
    //[@MenuItem(@"Table Convert/Checkinfo表(Checkinfo)")]
    //public static void ConvertCheckinfoTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        using (FileStream file = new FileStream(TableFolder[index] + "Checkinfo.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //        {
    //            HSSFWorkbook workbook = new HSSFWorkbook(file);
    //            CheckInfoTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "Checkinfo.bytes");
    //        }
    //    }
    //}
        
    //[@MenuItem(@"Table Convert/NPC商店名表(NpcShop)")]
    //public static void ConvertNpcShopTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        using (FileStream file = new FileStream(TableFolder[index] + "NpcShop.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //        {
    //            HSSFWorkbook workbook = new HSSFWorkbook(file);
    //            NpcShopTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "NpcShop.bytes");
    //        }
    //    }
    //}

    [@MenuItem(@"Table Convert/装备资质(Aptitude)")]
    public static void ConvertAptitudeTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "Aptitude.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                AptitudeTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "Aptitude.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/字符串表(StringTable)")]
    public static void ConvertStringTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "StringTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                StringTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "StringTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/世界参数表(WorldParamTable)")]
    public static void ConvertWorldParamTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "WorldParamTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                WorldParamTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "WorldParamTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/CD表(CDTable)")]
    public static void ConvertCDTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "CDTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                CDTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "CDTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    //[@MenuItem(@"Table Convert/Server表(Servers)")]
    //public static void ConvertServersTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        using (FileStream file = new FileStream(TableFolder[index] + "Servers.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //        {
    //            HSSFWorkbook workbook = new HSSFWorkbook(file);
    //            ServersTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "Servers.bytes");
    //        }
    //    }
    //}

    //[@MenuItem(@"Table Convert/场景地图比例表(SceneMapNumericTable)")]
    //public static void ConvertSceneMapNumericTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        using (FileStream file = new FileStream(TableFolder[index] + "Coordinate.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //        {
    //            HSSFWorkbook workbook = new HSSFWorkbook(file);
    //            SceneMapNumericTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "Coordinate.bytes");
    //        }
    //    }
    //}

    //[@MenuItem(@"Table Convert/EquipExpMoney表")]
    //public static void ConvertEquipExpMoneyTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        using (FileStream file = new FileStream(TableFolder[index] + "EquipExpMoney.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //        {
    //            HSSFWorkbook workbook = new HSSFWorkbook(file);
    //            EquipExpMoneyTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "EquipExpMoney.bytes");
    //        }
    //    }
    //}

    //[@MenuItem(@"Table Convert/NpcSay表")]
    //public static void ConvertNpcSayTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        using (FileStream file = new FileStream(TableFolder[index] + "NpcSay.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //        {
    //            HSSFWorkbook workbook = new HSSFWorkbook(file);
    //            NpcSayTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "NpcSay.bytes");
    //        }
    //    }
    //}
    //[@MenuItem(@"Table Convert/摄像机表")]
    //public static void ConvertShakeTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        using (FileStream file = new FileStream(TableFolder[index] + "Shake.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //        {
    //            HSSFWorkbook workbook = new HSSFWorkbook(file);
    //            ShakeTableLoader.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "Shake.bytes");
    //        }
    //    }
    //}

    //[@MenuItem(@"Table Convert/新手引导表")]
    //public static void ConvertPlayerGuideTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        using (FileStream file = new FileStream(TableFolder[index] + "PlayerGuide.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //        {
    //            HSSFWorkbook workbook = new HSSFWorkbook(file);
    //            PlayerGuideTableLoader.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "PlayerGuide.bytes");
    //        }
    //    }
    //}
    //[@MenuItem(@"Table Convert/点亮表(IllumeTable)")]
    //public static void ConvertIllumeTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        using (FileStream file = new FileStream(TableFolder[index] + "Illume.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //        {
    //            HSSFWorkbook workbook = new HSSFWorkbook(file);
    //            IllumeTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "Illume.bytes");
    //        }
    //    }
    //}

    //[@MenuItem(@"Table Convert/Boss表")]
    //public static void ConvertBossTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        using (FileStream file = new FileStream(TableFolder[index] + "BOSS.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //        {
    //            HSSFWorkbook workbook = new HSSFWorkbook(file);
    //            BossTableLoader.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "BOSS.bytes");
    //        }
    //    }
    //}

    //[@MenuItem(@"Table Convert/沙盘地图表")]
    //public static void ConvertSandTableInfoTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        using (FileStream file = new FileStream(TableFolder[index] + "SandTableInfoTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //        {
    //            HSSFWorkbook workbook = new HSSFWorkbook(file);
    //            SandTableInfoTableLoader.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "SandTableInfoTable.bytes");
    //        }
    //    }
    //}

    [@MenuItem(@"Table Convert/LvUpExp表")]
    public static void ConvertLvUpExpTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "LvUpExp.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                LvUpExpTableLoader.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "LvUpExp.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    //[@MenuItem(@"Table Convert/DungeonEvent表")]
    //public static void ConvertDungeonEventTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        DungeonEventTableLoader tableLoader = new DungeonEventTableLoader();
    //        tableLoader.Convert(TableFolder[index] + "DungeonEvent.xls", TargetFolder[index] + "DungeonEvent.bytes");
    //    }
    //}

    //[@MenuItem(@"Table Convert/DungeonEventResult表")]
    //public static void ConvertDungeonEventResultTable()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        DungeonEventResultTableLoader tableLoader = new DungeonEventResultTableLoader();
    //        tableLoader.Convert(TableFolder[index] + "DungeonEventResult.xls", TargetFolder[index] + "DungeonEventResult.bytes");
    //    }
    //}

    //[@MenuItem(@"Table Convert/DungeonFiles表")]
    //public static void ConvertDungeonFiles()
    //{
    //    for (int index = 0; index < TableFolder.Length; ++index)
    //    {
    //        DungeonFilesLoader tableLoader = new DungeonFilesLoader();
    //        tableLoader.Convert(TableFolder[index] + "DungeonFiles.xls", TargetFolder[index] + "DungeonFiles.bytes");
    //    }
    //}

    [@MenuItem(@"Table Convert/RarityRelative表")]
    public static void ConvertRarityRelativeTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "RarityRelativeTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                RarityRelativeTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "RarityRelative.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/OccupationInfoTable表")]
    public static void ConverOccupationInfoTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "OccupationInfoTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                OccupationInfoTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "OccupationInfoTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }


    [@MenuItem(@"Table Convert/PlayerRandomName表")]
    public static void ConvertPlayerRandomNameTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "PlayerRandomNameTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                PlayerRandomNameTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "PlayerRandomName.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/ZoneInfo表")]
    public static void ConvertZoneInfoTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "ZoneInfo.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                ZoneInfoTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "ZoneInfo.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/StageInfo表")]
    public static void ConvertStageInfoable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "StageInfo.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                StageInfoTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "StageInfo.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/FloorInfo表")]
    public static void ConvertFloorInfoTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "FloorInfo.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                FloorInfoTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "FloorInfo.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/Item表")]
    public static void ConvertItemTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "Item.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                ItemInfoTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "Item.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/FloorRankTable表")]
    public static void ConvertFloorRankTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "FloorRankTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                FloorRankTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "FloorRankTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }


    [@MenuItem(@"Table Convert/ScoreParam表")]
    public static void ConvertScoreParamTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "ScoreParamTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                ScoreParamTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "ScoreParamTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/BagTable表")]
    public static void ConvertBagTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "BagTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                BagTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "BagTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/RaceInfoTable表")]
    public static void ConvertRaceInfoTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "RaceInfoTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                RaceInfoTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "RaceInfoTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/EquipmentTable表")]
    public static void ConverEqiupmentTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "Equipment.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                EquipmentTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "Equipment.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/PlayerAttr表")]
    public static void ConvertPlayerAttrTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "PlayerAttrTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                PlayerAttrTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "PlayerAttrTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/LoadingTips表")]
    public static void ConvertLoadingTipsTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "LoadingTips.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                LoadingTipsTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "LoadingTips.bytes");
            }
        }
        AssetDatabase.Refresh();
    }
    [@MenuItem(@"Table Convert/魔法石商店表(MagicStoneTable)")]
    public static void ConvertMagicStoneTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "MagicStonePrice.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                MagicStoneTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "MagicStonePrice.bytes");
            }
        }
        AssetDatabase.Refresh();
    }
    [@MenuItem(@"Table Convert/荣誉戒指兑换商店表(RingExchangeTable)")]
    public static void ConvertRingExchangeTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "RingExchangeTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                RingExchangeTableConvertor.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "RingExchangeTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

	[@MenuItem(@"Table Convert/服务器反馈CODE表(MessageRespondTable)")]
	public static void ConvertMessageRespondTable()
	{
		for (int index = 0; index < TableFolder.Length; ++index)
		{
			using (FileStream file = new FileStream(TableFolder[index] + "MessageRespond.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				HSSFWorkbook workbook = new HSSFWorkbook(file);
				MessageRespondTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "MessageRespondTable.bytes");
			}
        }
        AssetDatabase.Refresh();
	}

    [@MenuItem(@"Table Convert/RoomTagTable")]
    public static void ConvertRoomTagTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "RoomTagTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                MessageRespondTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "RoomTagTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/RoomEditInfoTable")]
    public static void ConvertRoomEditInfoTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "RoomInfoTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                RoomEdiInfoTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "RoomInfoTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }
    [@MenuItem(@"Table Convert/IconInfoTable")]
    public static void ConvertIconInfoTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "Icon.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                RoomEdiInfoTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "Icon.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/DungeonMonsterTable")]
    public static void ConvertDungeonMonsterTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "DungeonMonsterTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                DungeonMonsterConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "DungeonMonsterTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/AttrRatioTable")]
    public static void ConvertAttrRatioTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "AttrRatioTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                AttrRatioConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "AttrRatioTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/剑魂表(ComboSwordSoulTable)")]
    public static void ConvertComboSwordSoulTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "ComboSwordSoul.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                ComboSwordSoulTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "ComboSwordSoulTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/EventItemTable)")]
    public static void ConvertEventItemTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "EventItem.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                EventItemTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "EventItem.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/GradeUpRequireTable)")]
    public static void ConvertGradeUpRequireTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "GradeUpRequireTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                GradeUpRequireTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "GradeUpRequireTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/QualityRelativeTable)")]
    public static void ConvertQualityRelativeTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "QualityRelativeTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                QualityRelativeTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "QualityRelativeTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/CardTypeVariationTable)")]
    public static void ConvertCardTypeVariationTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "CardTypeVariationTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                CardTypeVariationTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "CardTypeVariationTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/CardLevelVariationTable)")]
    public static void ConvertCardLevelVariationTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "CardLevelupVariationTable.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                CardLevelVariationTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "CardLevelupVariationTable.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem(@"Table Convert/YellowPointParamTable)")]
    public static void ConvertYellowPointParamTable()
    {
        for (int index = 0; index < TableFolder.Length; ++index)
        {
            using (FileStream file = new FileStream(TableFolder[index] + "YellowPointParam.xls", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(file);
                YellowPointParamTableConverter.Convert(workbook.GetSheetAt(0), TargetFolder[index] + "YellowPointParam.bytes");
            }
        }
        AssetDatabase.Refresh();
    }

};