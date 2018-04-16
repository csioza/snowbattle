//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Editor
//	created:	2013-5-9
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

#region SceneTeleportTableLoader
public class SceneTeleportTableLoader : SceneTeleportTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(TeleportInfoList.Count);
        foreach (SceneTeleportInfo info in TeleportInfoList.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        SceneTeleportTableLoader convertor = new SceneTeleportTableLoader();
        convertor.TeleportInfoList = new SortedList<int, SceneTeleportInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            SceneTeleportInfo info = ExcelTableTools.GenericObj<SceneTeleportInfo>(row);
            convertor.TeleportInfoList.Add(info.ID, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            SceneTeleportTable table = new SceneTeleportTable();
            table.Load(buff);
            foreach (SceneTeleportInfo info in table.TeleportInfoList.Values)
            {
                Debug.Log("ID=" + info.ID + " prefabName=" + info.prefabName);
            }
        }
    }
}
#endregion
#region SceneRoomTableLoader
public class SceneRoomTableLoader : SceneRoomTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(RoomInfoList.Count);
        foreach (SceneRoomInfo info in RoomInfoList.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        SceneRoomTableLoader convertor = new SceneRoomTableLoader();
        convertor.RoomInfoList = new SortedList<int, SceneRoomInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            SceneRoomInfo info = ExcelTableTools.GenericObj<SceneRoomInfo>(row);
            convertor.RoomInfoList.Add(info.ID, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            SceneRoomTable table = new SceneRoomTable();
            table.Load(buff);
            foreach (SceneRoomInfo info in table.RoomInfoList.Values)
            {
                Debug.Log("ID="+info.ID+" prefabName="+info.prefabName);
            }
        }
    }
}
#endregion
#region SceneGateTableLoader
public class SceneGateTableLoader : SceneGateTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(GateInfoList.Count);
        foreach (SceneGateInfo info in GateInfoList.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        SceneGateTableLoader convertor = new SceneGateTableLoader();
        convertor.GateInfoList = new SortedList<int, SceneGateInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            SceneGateInfo info = ExcelTableTools.GenericObj<SceneGateInfo>(row);
            convertor.GateInfoList.Add(info.ID, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            SceneGateTable table = new SceneGateTable();
            table.Load(buff);
            foreach (SceneGateInfo info in table.GateInfoList.Values)
            {
                Debug.Log("ID=" + info.ID + " prefabName=" + info.prefabName);
            }
        }
    }
}
#endregion
#region SceneBridgeTableLoader
public class SceneBridgeTableLoader : SceneBridgeTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(BridgeInfoList.Count);
        foreach (SceneBridgeInfo info in BridgeInfoList.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        SceneBridgeTableLoader convertor = new SceneBridgeTableLoader();
        convertor.BridgeInfoList = new SortedList<int, SceneBridgeInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            SceneBridgeInfo info = ExcelTableTools.GenericObj<SceneBridgeInfo>(row);
            convertor.BridgeInfoList.Add(info.ID, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            SceneBridgeTable table = new SceneBridgeTable();
            table.Load(buff);
            foreach (SceneBridgeInfo info in table.BridgeInfoList.Values)
            {
                Debug.Log("ID=" + info.ID + " bridgeHead=" + info.bridgeHeadPrefab + " bridgeBody=" + info.bridgeBodyPrefab + " bridgeTail=" + info.bridgeTailPrefab);
            }
        }
    }
}
#endregion
#region QTESequenceTableConverter
public class QTESequenceTableConverter : QTESequenceTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_list.Count);
        foreach (QTESequenceInfo info in m_list.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        QTESequenceTableConverter convertor = new QTESequenceTableConverter();
        convertor.m_list = new Dictionary<int, QTESequenceInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            QTESequenceInfo info = ExcelTableTools.GenericObj<QTESequenceInfo>(row);
            convertor.m_list.Add(info.ID, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            QTESequenceTable table = new QTESequenceTable();
            table.Load(buff);

            foreach (var info in table.m_list.Values)
            {
                Debug.Log("id=" + info.ID.ToString());
                Debug.Log("list=");
                foreach (var item in info.SequenceList)
                {
                    Debug.Log(item + " ");
                }
            }
        }
    }
}
#endregion
#region FlyingItemTableConverter
public class FlyingItemTableConverter : FlyingItemTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_list.Count);
        foreach (FlyingItemInfo info in m_list.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        FlyingItemTableConverter convertor = new FlyingItemTableConverter();
        convertor.m_list = new Dictionary<int, FlyingItemInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            FlyingItemInfo info = ExcelTableTools.GenericObj<FlyingItemInfo>(row);
            convertor.m_list.Add(info.ID, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            FlyingItemTable table = new FlyingItemTable();
            table.Load(buff);
            foreach (var key in table.m_list.Keys)
            {
                Debug.Log(key);
            }
        }
    }
}
#endregion

#region StageTableLoader
public class StageTableLoader : StageTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(StageInfoList.Count);
        foreach (StageInfo info in StageInfoList.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        StageTableLoader convertor = new StageTableLoader();
        convertor.StageInfoList = new SortedList<int, StageInfo>();
        PropertyInfo[] vInfo = typeof(StageInfo).GetProperties();
        foreach (IRow row in sheet)
        {
            int index = 0;
            ICell data = row.GetCell(index);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;
            StageInfo info = new StageInfo();
            ExcelTableTools.SetValue<StageInfo>(vInfo[index], info, data);                          //ID
            ++index; ExcelTableTools.SetValue<StageInfo>(vInfo[index], info, row.GetCell(index));    //关卡
            ++index; ExcelTableTools.SetValue<StageInfo>(vInfo[index], info, row.GetCell(index));    //当前层
            ++index; ExcelTableTools.SetValue<StageInfo>(vInfo[index], info, row.GetCell(index));    //线路数量
            //!线路
            info.LineList = new List<StageLineInfo>();
            for (int j = 0; j < info.nLineNum; j++)
            {
                ++index;
                data = row.GetCell(index);
                StageLineInfo lineInfo = new StageLineInfo();
                if (data != null)
                {
                    string temp = data.StringCellValue;
                    string strNode = temp.Substring(0, 1);
                    if (!string.IsNullOrEmpty(strNode))
                    {
                        lineInfo.mRoomNodeTree.mID = int.Parse(strNode);
                        lineInfo.mRoomNodeTree.BuildTree(ref temp);
                        if (!string.IsNullOrEmpty(temp))
                        {
                            Debug.Log("node is error:" + temp);
                        }
                    }
                }
                //!boss room list
                ++index;
                data = row.GetCell(index);
                if (data != null)
                {
                    string temp = data.StringCellValue;
                    if (!string.IsNullOrEmpty(temp))
                    {
                        string[] param = temp.Split(new char[1] { ',' });
                        foreach (string i in param)
                        {
                            if (!string.IsNullOrEmpty(i))
                            {
                                lineInfo.BossRoomList.Add(int.Parse(i));
                            }
                        }
                    }
                }
                info.LineList.Add(lineInfo);
            }
            convertor.StageInfoList.Add(info.ID, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            StageTable table = new StageTable();
            table.Load(buff);
            foreach (StageInfo info in table.StageInfoList.Values)
            {
                string temp = "";
                for (int i = 0; i < info.LineList.Count; i++ )
                {
                    info.LineList[i].mRoomNodeTree.ComponentNode(ref temp);
                    temp = temp.Substring(0, temp.Length - 1);
                    temp += "---------";
                }
                Debug.Log(temp);
            }
        }
    }
}
#endregion

#region RoomAttrTableLoader
public class RoomAttrTableLoader : RoomAttrTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(RoomAttrInfoList.Count);
        foreach (RoomAttrInfo info in RoomAttrInfoList.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        RoomAttrTableLoader convertor = new RoomAttrTableLoader();
        convertor.RoomAttrInfoList = new SortedList<int, RoomAttrInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            RoomAttrInfo info = ExcelTableTools.GenericObj<RoomAttrInfo>(row);
            convertor.RoomAttrInfoList.Add(info.ID, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            RoomAttrTable table = new RoomAttrTable();
            table.Load(buff);
            foreach (RoomAttrInfo info in table.RoomAttrInfoList.Values)
            {
                Debug.Log(info.ID);
            }
        }
    }
}
#endregion

#region ActionRelationTableLoader
public class ActionRelationTableLoader : ActionRelationTable
{
	public static void Convert(ISheet sheet, string targetPath)
	{
		ActionRelationTableLoader tableLoader = new ActionRelationTableLoader();
		tableLoader.m_relationArray = new byte[(int)ActorAction.ENType.Count * (int)ActorAction.ENType.Count];

		for (int index = 1; index < (int)ActorAction.ENType.Count + 1; ++index)
		{
			for (int innerIndex = 1; innerIndex < (int)ActorAction.ENType.Count + 1; ++innerIndex)
			{
				tableLoader.m_relationArray[(index - 1) * (int)ActorAction.ENType.Count + innerIndex - 1] = (byte)sheet.GetRow(index).GetCell(innerIndex).NumericCellValue;
			}
		}
		for (int index = 0; index < (int)ActorAction.ENType.Count; ++index)
		{
			string temp = "";
			for (int innerIndex = 0; innerIndex < (int)ActorAction.ENType.Count; ++innerIndex)
			{
				temp += tableLoader.m_relationArray[index * (int)ActorAction.ENType.Count + innerIndex] + ", ";
			}
			Debug.Log(temp);
		}
		if (File.Exists(targetPath))
		{
			File.Delete(targetPath);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
		{
			byte[] buff = tableLoader.Save();
			targetFile.Write(buff, 0, buff.Length);
		}
	}
	public byte[] Save()
	{
		BinaryHelper helper = new BinaryHelper();
		helper.Write(m_relationArray.Length);
		helper.InnerStream.Write(m_relationArray, 0, m_relationArray.Length);
		return helper.GetBytes();
	}
}
#endregion
//场景表
#region SceneInfoTableLoader
public class SceneInfoTableLoader : SceneInfoTable
{
	public void Convert(string filePath, string targetPath)
	{
		m_sceneMap = new Dictionary<int, SceneInfo>();

 		using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			HSSFWorkbook workbook = new HSSFWorkbook(file);
			ISheet sheet = workbook.GetSheetAt(0);
			foreach (IRow rowData in sheet)
			{
				if (null == rowData)
				{
					continue;
				}
				if (null == rowData.GetCell(0))
				{
					continue;
				}
				if (CellType.NUMERIC != rowData.GetCell(0).CellType)
				{
					continue;
				}
				SceneInfo sceneData = new SceneInfo();
				int cellIndex = 0;
				while (cellIndex < rowData.Cells.Count)
				{
					ICell data = rowData.GetCell(cellIndex++);
					sceneData.SceneId = null != data ? (int)data.NumericCellValue : 0;
					if (sceneData.SceneId == 0)
					{
						break;
					}
					data = rowData.GetCell(cellIndex++);
					sceneData.SceneName = null != data ? data.StringCellValue : "";

					data = rowData.GetCell(cellIndex++);
					sceneData.SceneFile = null != data ? data.StringCellValue : "";

					data = rowData.GetCell(cellIndex++);
					sceneData.SceneType = null != data ? (int)data.NumericCellValue : 0;

					data = rowData.GetCell(cellIndex++);
					sceneData.LevelMinLimit = null != data ? (int)data.NumericCellValue : 0;

					data = rowData.GetCell(cellIndex++);
					sceneData.LevelMaxLimit = null != data ? (int)data.NumericCellValue : 0;

					//中间几列先不读 todo
					cellIndex += 2;

                    data = rowData.GetCell(cellIndex++);
                    string borns = null != data ? data.StringCellValue : "";
                    string[] pos = borns.Split('|');
                    sceneData.BornPosList = new List<float>();
                    for (int index = 0; index < pos.Length; index++)
                    {
                        float value = 0;
                        try
                        {
                            value = float.Parse(pos[index]);
                        }
                        catch
                        {
                            continue;
                        }
                        if (value != 0.0f)
                        {
                            sceneData.BornPosList.Add(value);
                        }
                    }

                    cellIndex += 5;

					data = rowData.GetCell(cellIndex++);
					sceneData.ByteFile = null != data ? data.StringCellValue : "";

					data = rowData.GetCell(cellIndex++);
					sceneData.BeginArea = null != data ? data.StringCellValue : "";

                    data = rowData.GetCell(cellIndex++);
                    sceneData.Physical = null != data ? (int)data.NumericCellValue : 0;

					m_sceneMap.Add(sceneData.SceneId, sceneData);

					//后面的不读了
					break;
				}
			}
			//save
			if (m_sceneMap.Count > 0)
			{
				BinaryHelper streamHelper = new BinaryHelper();
				streamHelper.Write(m_sceneMap.Count);

				foreach (KeyValuePair<int, SceneInfo> item in m_sceneMap)
				{
					streamHelper.Write(item.Value.SceneId);
					streamHelper.Write(item.Value.SceneName);
					streamHelper.Write(item.Value.SceneFile);
					streamHelper.Write(item.Value.SceneType);
					streamHelper.Write(item.Value.LevelMinLimit);
					streamHelper.Write(item.Value.LevelMaxLimit);
                    streamHelper.Write(item.Value.BornPosList.Count);
                    for (int index = 0; index < item.Value.BornPosList.Count; ++index)
                    {
                        streamHelper.Write(item.Value.BornPosList[index]);
                    }
					streamHelper.Write(item.Value.ByteFile);
					streamHelper.Write(item.Value.BeginArea);
                    streamHelper.Write(item.Value.Physical);
				}

				if (File.Exists(targetPath))
				{
					File.Delete(targetPath);
				}
				FileStream writeFile = new FileStream(targetPath, FileMode.Create);
				byte[] buff = streamHelper.GetBytes();
				writeFile.Write(buff, 0, buff.Length);
				writeFile.Close();
				Debug.Log(sheet.SheetName + "转换完成");
			}
		}
	}
}
#endregion

#region DungeonInfoTableConvertor
/*public class DungeonInfoTableConvertor : DungeonInfoTable
{
	public void Convert(string filePath, string targetPath)
	{
		DungeonInfoMap = new SortedDictionary<DungeonInfoKey, DungeonInfo>();

		using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			HSSFWorkbook workbook = new HSSFWorkbook(file);
			ISheet sheet = workbook.GetSheetAt(0);
			foreach (IRow rowData in sheet)
			{
				if (null == rowData)
				{
					continue;
				}
				if (null == rowData.GetCell(0))
				{
					continue;
				}
				if (CellType.NUMERIC != rowData.GetCell(0).CellType)
				{
					continue;
				}
				DungeonInfo dungeonData = new DungeonInfo();
				int cellIndex = 0;
				while (cellIndex < rowData.Cells.Count)
				{
					ICell data = rowData.GetCell(cellIndex++);
					dungeonData.DungeonId = null != data ? (int)data.NumericCellValue : 0;
					if (dungeonData.DungeonId == 0)
					{
						break;
					}

                    data = rowData.GetCell(cellIndex++);
                    dungeonData.DungeonType = null != data ? (int)data.NumericCellValue : 0;

					data = rowData.GetCell(cellIndex++);
					dungeonData.DungeonGrade = null != data ? (int)data.NumericCellValue : 0;

					data = rowData.GetCell(cellIndex++);
					dungeonData.DungeonName = null != data ? data.StringCellValue : "";

					data = rowData.GetCell(cellIndex++);
					string sceneIds = null != data ? data.StringCellValue : "";
					string[] ids = sceneIds.Split('|');
					dungeonData.SceneIdList = new List<int>();
					for (int idsIndex = 0; idsIndex < ids.Length; idsIndex++ )
					{
						int intID = 0;
						try
						{
							intID = int.Parse(ids[idsIndex]);
						}
						catch 
						{
							continue;
						}
						if (intID != 0)
						{
							dungeonData.SceneIdList.Add(intID);
						}
					}

					data = rowData.GetCell(cellIndex++);
					dungeonData.LevelMin = null != data ? (int)data.NumericCellValue : 0;

					data = rowData.GetCell(cellIndex++);
					dungeonData.LevelMax = null != data ? (int)data.NumericCellValue : 0;

					data = rowData.GetCell(cellIndex++);
					dungeonData.QuestId = null != data ? (int)data.NumericCellValue : 0;

					data = rowData.GetCell(cellIndex++);
					dungeonData.NeedEnergy = null != data ? (int)data.NumericCellValue : 0;

					data = rowData.GetCell(cellIndex++);
					dungeonData.TeamVolume = null != data ? (int)data.NumericCellValue : 0;

					DungeonInfoKey newKey = new DungeonInfoKey();
					newKey.DungeonID = dungeonData.DungeonId;
					newKey.DungeonGrade = dungeonData.DungeonGrade;
					DungeonInfoMap.Add(newKey, dungeonData);
					break;
				}
			}
			//save
			if (DungeonInfoMap.Count > 0)
			{
				BinaryHelper streamHelper = new BinaryHelper();
				streamHelper.Write(DungeonInfoMap.Count);

				foreach (KeyValuePair<DungeonInfoKey, DungeonInfo> item in DungeonInfoMap)
				{
					streamHelper.Write(item.Value.DungeonId);
					streamHelper.Write(item.Value.DungeonGrade);
					streamHelper.Write(item.Value.DungeonName);
					streamHelper.Write(item.Value.SceneIdList.Count);
					for (int index = 0; index < item.Value.SceneIdList.Count; ++index )
					{
						streamHelper.Write(item.Value.SceneIdList[index]);
					}
					streamHelper.Write(item.Value.LevelMin);
					streamHelper.Write(item.Value.LevelMax);
					streamHelper.Write(item.Value.QuestId);
					streamHelper.Write(item.Value.NeedEnergy);
					streamHelper.Write(item.Value.TeamVolume);
				}
				if (File.Exists(targetPath))
				{
					File.Delete(targetPath);
				}
				FileStream writeFile = new FileStream(targetPath, FileMode.Create);
				byte[] buff = streamHelper.GetBytes();
				writeFile.Write(buff, 0, buff.Length);
				writeFile.Close();
				Debug.Log(sheet.SheetName + "转换完成");
			}
		}
	}
}*/
#endregion

#region SkillTableConverter
public class SkillTableConverter : SkillTable
{
	public byte[] Save()
	{
		BinaryHelper helper = new BinaryHelper();
		helper.Write(SkillInfoList.Count);
        foreach(var item in SkillInfoList.Values)
		{
			item.Save(helper);
		}
		return helper.GetBytes();
	}
	
	public static void Convert(ISheet sheet, string targetPath)
	{
        SkillTableConverter convertor = new SkillTableConverter();
        convertor.SkillInfoList = new Dictionary<int, SkillInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            SkillInfo info = ExcelTableTools.GenericObj<SkillInfo>(row);
            convertor.SkillInfoList.Add(info.ID, info);
        }
		if (File.Exists(targetPath))
		{
			File.Delete(targetPath);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
		{
			byte[] buff = convertor.Save();
			targetFile.Write(buff, 0, buff.Length);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			byte[] buff = new byte[targetFile.Length];
			targetFile.Read(buff, 0, (int)targetFile.Length);
			SkillTable table = new SkillTable();
			table.Load(buff);
            foreach (SkillInfo info in table.SkillInfoList.Values)
            {
                Debug.Log("ID is " + info.ID);
            }
		}
	}
}
#endregion

#region VocationTableConvertor
public class VocationTableConvertor : VocationTable
{
	public byte[] Save()
	{
		BinaryHelper helper = new BinaryHelper();
		helper.Write(VocationInfoList.Count);
		foreach (VocationInfo info in VocationInfoList)
		{
			info.Save(helper);
		}
		return helper.GetBytes();
	}
	public static void Convert(ISheet sheet, string targetPath)
	{
		VocationTableConvertor convertor = new VocationTableConvertor();
		convertor.VocationInfoList = new List<VocationInfo>();
		foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

			VocationInfo info = ExcelTableTools.GenericObj<VocationInfo>(row);
			convertor.VocationInfoList.Add(info);
		}
		if (File.Exists(targetPath))
		{
			File.Delete(targetPath);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
		{
			byte[] buff = convertor.Save();
			targetFile.Write(buff, 0, buff.Length);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			byte[] buff = new byte[targetFile.Length];
			targetFile.Read(buff, 0, (int)targetFile.Length);
			VocationTable table = new VocationTable();
			table.Load(buff);
			foreach (VocationInfo info in table.VocationInfoList)
			{
				Debug.Log(info.ID + " " + info.Name + " " + info.ModelID);
			}
		}
	}
}
#endregion

#region NPCInfoTableConvertor
public class NPCInfoTableConvertor : NPCInfoTable
{
	public byte[] Save()
	{
		BinaryHelper helper = new BinaryHelper();
        helper.Write(m_list.Count);
        foreach (NPCInfo info in m_list.Values)
		{
			info.Save(helper);
		}
		return helper.GetBytes();
	}
	public static void Convert(ISheet sheet, string targetPath)
	{
		NPCInfoTableConvertor convertor = new NPCInfoTableConvertor();
        convertor.m_list = new Dictionary<int, NPCInfo>();
        PropertyInfo[] vInfo = typeof(NPCInfo).GetProperties();
		foreach (IRow row in sheet)
        {
            int index = 0;
            ICell data = row.GetCell(index);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;
            NPCInfo info = new NPCInfo();
            ExcelTableTools.SetValue<NPCInfo>(vInfo[index],   info, data);                         //NPCID
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //名称
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //模型ID
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //模型缩放
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //武器类型
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //种族
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));     //是否可推动
			++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //AI类型
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //类型
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //BOSSNpc读取的XML文件名
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //BOSSNpc读取的XML文件中段落的名称
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //交互型子类型
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //NPC 聊天内容ID
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //等级
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //血量
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //血条段数
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //物理攻击
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //魔法攻击
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //物理防御
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //魔法防御
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //命中率
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //闪避
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //暴击率
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //暴击系数
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //抗性
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //受伤系数
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //抗普通受击概率
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //抗强受击概率
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //抗击退概率
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //抗击飞概率
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //移动转身速率
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //攻击旋转速率
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //移动速度
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //animation播放速度
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //攻击范围
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));    //呼叫同伴范围
			++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));     //警戒范围
			++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));     //取消警戒的范围
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));		//警戒时间
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));		//膨胀速率
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));		//冷却速率
			++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));		//追击最大间距
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));		//最大追击距离
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));		//后退间距
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));		//后退速度
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));		//后退距离
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));		//强韧上限
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));		//强韧恢复
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));		//强韧恢复间隔
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));		//强韧等于0后的事件
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));		//强韧释放的技能
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));		//强韧等于0后的恢复百分比
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));		//出生时携带的buffID列表
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));     //掉落
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));     //技能列表
            ++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));     //被动技能列表
			++index; ExcelTableTools.SetValue<NPCInfo>(vInfo[index], info, row.GetCell(index));     //功能个数
            // 功能列表
            int nfCount = 0;
            info.NpcFuncArgs = new List<NpcFuncArg>();
            while (nfCount < 5)
            {
                data = row.GetCell(++index);
                if (data != null)
                {
                    int nType = (int)data.NumericCellValue;
                    if (nType > 0)
                    {
                        int nPara = 0;
                        data = row.GetCell(++index);
                        if (data != null)
                        {
                            nPara = (int)data.NumericCellValue;
                        }
                        
                        NpcFuncArg vArg = new NpcFuncArg();
                        vArg.FuncType = nType;
                        vArg.FuncParam = nPara;
                        info.NpcFuncArgs.Add(vArg);
                    }
                }
                ++nfCount;
            }

            convertor.m_list.Add(info.ID, info);
		}
		if (File.Exists(targetPath))
		{
			File.Delete(targetPath);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
		{
			byte[] buff = convertor.Save();
			targetFile.Write(buff, 0, buff.Length);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			byte[] buff = new byte[targetFile.Length];
			targetFile.Read(buff, 0, (int)targetFile.Length);
			NPCInfoTable table = new NPCInfoTable();
			table.Load(buff);
            Debug.Log("COUNT = " + table.m_list.Count);
            foreach (NPCInfo info1 in table.m_list.Values)
			{
                int nCount = 0;
                List<NpcFuncArg> vLst = info1.NpcFuncArgs;
                if (vLst != null) nCount = vLst.Count;
                Debug.Log("ID = " + info1.ID + ", BossAIXmlName = " + info1.BossAIXmlName + ", Count = " + nCount + ", BossAIXmlSubName:" + info1.BossAIXmlSubName);
			}
		}
	}
}
#endregion

#region TrapInfoTable
public class TrapInfoTableConvertor :TrapInfoTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_list.Count);
        foreach (TrapInfo info in m_list.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        TrapInfoTableConvertor convertor = new TrapInfoTableConvertor();
        convertor.m_list = new Dictionary<int, TrapInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            TrapInfo info = ExcelTableTools.GenericObj<TrapInfo>(row);
            convertor.m_list.Add(info.ID, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            TrapInfoTable table = new TrapInfoTable();
            table.Load(buff);
            foreach (var info in table.m_list.Values)
            {
                Debug.Log(info.ID + " ModelID:" + info.ModelID + " CampType:" + info.CampType + " MinTouchTime:" + info.MinTouchTime);
            }
        }
        Debug.Log(targetPath);
    }
}
#endregion

#region HeroInfoTableConvertor
public class HeroInfoTableConvertor : HeroInfoTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_list.Count);
        foreach (HeroInfo info in m_list.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        HeroInfoTableConvertor convertor = new HeroInfoTableConvertor();
        convertor.m_list = new Dictionary<int, HeroInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            HeroInfo info = ExcelTableTools.GenericObj<HeroInfo>(row);
            convertor.m_list.Add(info.ID, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            HeroInfoTable table = new HeroInfoTable();
            table.Load(buff);

            foreach (var key in table.m_list.Keys)
            {
                Debug.Log(key);
            }
        }
    }
}
#endregion

#region AnimWeightTableConvertor
/*public class AnimWeightTableConvertor : AnimWeightTable
{
	public byte[] Save()
	{
		BinaryHelper helper = new BinaryHelper();
		helper.Write(m_currentList.Count);
		foreach (string item in m_currentList)
		{
			helper.Write(item);
		}
		helper.Write(m_newList.Count);
		foreach (string item in m_newList)
		{
			helper.Write(item);
		}
		helper.InnerStream.Write(m_byteArray, 0, m_byteArray.Length);
		return helper.GetBytes();
	}
	public static void Convert(ISheet sheet, string targetPath)
	{
		AnimWeightTableConvertor infoSave = new AnimWeightTableConvertor();
		infoSave.m_newList = new List<string>();
		infoSave.m_currentList = new List<string>();
		IRow firstRow = sheet.GetRow(0);
		for (int index = 1; index < firstRow.Cells.Count; ++index)
		{
			infoSave.m_newList.Add(firstRow.GetCell(index).StringCellValue);
		}
		infoSave.m_byteArray = new byte[infoSave.m_newList.Count * infoSave.m_newList.Count];
		for (int index = 1; null != sheet.GetRow(index); ++index)
		{
			IRow rowData = sheet.GetRow(index);
			infoSave.m_currentList.Add(rowData.GetCell(0).StringCellValue);
			for (int innerIndex = 1; innerIndex < rowData.Cells.Count; ++innerIndex)
			{
				infoSave.m_byteArray[(index - 1) * infoSave.m_newList.Count + innerIndex - 1] = (byte)rowData.GetCell(innerIndex).NumericCellValue;
			}
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
		{
			byte[] buff = infoSave.Save();
			targetFile.Write(buff, 0, buff.Length);
		}
	}
}*/
#endregion

#region SkillResultTableConvertor
public class SkillResultTableConvertor : SkillResultTable
{
	public byte[] Save()
	{
		BinaryHelper helper = new BinaryHelper();
		helper.Write(SkillResultList.Count);
		foreach (var resultPair in SkillResultList)
		{
			resultPair.Value.Save(helper);
		}
		return helper.GetBytes();
	}
	public static void Convert(ISheet sheet, string targetPath)
	{
		SkillResultTableConvertor convertor = new SkillResultTableConvertor();
		convertor.SkillResultList = new Dictionary<int, SkillResultInfo>();

        foreach (IRow row in sheet)
        {
            SkillResultInfo result;
            {
                int index = 0;
                ICell cell = null;
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    int id = (int)cell.NumericCellValue;
                    if (0 == id)
                    {
                        continue;
                    }
                    result = new SkillResultInfo();
                    result.GetType().GetProperty("ID").SetValue(result, id, null);
                }
                else
                {
                    continue;
                }
                //Description
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.STRING)
                {
                    result.GetType().GetProperty("Description").SetValue(result, cell.StringCellValue, null);
                    Debug.Log(result.Description);
                }
                //InstantRange
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("InstantRange").SetValue(result, (int)cell.NumericCellValue, null);
                }
                //ResultTargetType
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("ResultTargetType").SetValue(result, (int)cell.NumericCellValue, null);
                }
                ////命中率
                //cell = row.GetCell(index++);
                //if (null != cell && cell.CellType == CellType.NUMERIC)
                //{
                //    result.GetType().GetProperty("HitPersent").SetValue(result, (float)cell.NumericCellValue, null);
                //}
                ////暴击率
                //cell = row.GetCell(index++);
                //if (null != cell && cell.CellType == CellType.NUMERIC)
                //{
                //    result.GetType().GetProperty("CritPersent").SetValue(result, (float)cell.NumericCellValue, null);
                //}
                //普通受击几率
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("InterferePersent").SetValue(result, (float)cell.NumericCellValue, null);
                }
                //强受击几率
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("BreakPersent").SetValue(result, (float)cell.NumericCellValue, null);
                }
                //击退几率
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("HitBackPersent").SetValue(result, (float)cell.NumericCellValue, null);
                }
                //击飞几率
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("HitFlyPersent").SetValue(result, (float)cell.NumericCellValue, null);
                }
                //combo type
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("ComboType").SetValue(result, (int)cell.NumericCellValue, null);
                }
                //iscombo
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("ComboNum").SetValue(result, (int)cell.NumericCellValue, null);
                }
                //combo时间
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("ComboTime").SetValue(result, (float)cell.NumericCellValue, null);
                }
                //强韧削减
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("StaminaDamage").SetValue(result, (float)cell.NumericCellValue, null);
                }
                //SFightWillGet
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("SFightWillGet").SetValue(result, (float)cell.NumericCellValue, null);
                }
                //NFightWillGet
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("NFightWillGet").SetValue(result, (float)cell.NumericCellValue, null);
                }
                //IsWeakenComboTime
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("IsWeakenComboTime").SetValue(result, cell.NumericCellValue != 0, null);
                }
                //WeakenComboTime
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("WeakenComboTime").SetValue(result, (float)cell.NumericCellValue, null);
                }
                //HpChangeType
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("HpChangeType").SetValue(result, (int)cell.NumericCellValue, null);
                }
                //AnimGauge
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("AnimGauge").SetValue(result, (float)cell.NumericCellValue, null);
                }
                //IsPlayEffect
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("IsPlayEffect").SetValue(result, cell.NumericCellValue != 0, null);
                }
                //EffectName
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.STRING)
                {
                    result.GetType().GetProperty("EffectName").SetValue(result, cell.StringCellValue, null);
                }
                //EffectPos
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.STRING)
                {
                    result.GetType().GetProperty("EffectPos").SetValue(result, cell.StringCellValue, null);
                }
                //EffectDuration
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("EffectDuration").SetValue(result, (float)cell.NumericCellValue, null);
                }
                //IsAheadBone
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("IsAheadBone").SetValue(result, cell.NumericCellValue != 0, null);
                }
                //IsChangeShader
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("IsChangeShader").SetValue(result, cell.NumericCellValue != 0, null);
                }
                ////ShaderName
                //cell = row.GetCell(index++);
                //if (null != cell && cell.CellType == CellType.STRING)
                //{
                //    result.GetType().GetProperty("ShaderName").SetValue(result, cell.StringCellValue, null);
                //}
                ////ShaderParam
                //cell = row.GetCell(index++);
                //if (null != cell && cell.CellType == CellType.NUMERIC)
                //{
                //    result.GetType().GetProperty("ShaderParam").SetValue(result, (float)cell.NumericCellValue, null);
                //}
                ////ShaderChangeTimer
                //cell = row.GetCell(index++);
                //if (null != cell && cell.CellType == CellType.NUMERIC)
                //{
                //    result.GetType().GetProperty("ShaderChangeTimer").SetValue(result, (float)cell.NumericCellValue, null);
                //}
                ////ShaderRestoreTimer
                //cell = row.GetCell(index++);
                //if (null != cell && cell.CellType == CellType.NUMERIC)
                //{
                //    result.GetType().GetProperty("ShaderRestoreTimer").SetValue(result, (float)cell.NumericCellValue, null);
                //}
                ////IsChangeShaderColor
                //cell = row.GetCell(index++);
                //if (null != cell && cell.CellType == CellType.NUMERIC)
                //{
                //    result.GetType().GetProperty("IsChangeShaderColor").SetValue(result, cell.NumericCellValue != 0, null);
                //}
                ////ShaderColorName
                //cell = row.GetCell(index++);
                //if (null != cell && cell.CellType == CellType.STRING)
                //{
                //    result.GetType().GetProperty("ShaderColorName").SetValue(result, cell.StringCellValue, null);
                //}
                ////ShaderColorParam
                //cell = row.GetCell(index++);
                //if (null != cell && cell.CellType == CellType.STRING)
                //{
                //    result.GetType().GetProperty("ShaderColorParam").SetValue(result, cell.StringCellValue, null);
                //}
                ////ShaderColorDuration
                //cell = row.GetCell(index++);
                //if (null != cell && cell.CellType == CellType.NUMERIC)
                //{
                //    result.GetType().GetProperty("ShaderColorDuration").SetValue(result, (float)cell.NumericCellValue, null);
                //}
                //ChangeShaderAnimName
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.STRING)
                {
                    result.GetType().GetProperty("ChangeShaderAnimName").SetValue(result, cell.StringCellValue, null);
                }
                //SoundList
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.STRING)
                {
                    result.GetType().GetProperty("SoundList").SetValue(result, cell.StringCellValue, null);
                }
                //MinComboCount
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("MinComboCount").SetValue(result, (int)cell.NumericCellValue, null);
                }
                //PhyAttackAdd
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("PhyAttackAdd").SetValue(result, (float)cell.NumericCellValue, null);
                }
                //MagAttackAdd
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("MagAttackAdd").SetValue(result, (float)cell.NumericCellValue, null);
                }
                //AttackModify
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    result.GetType().GetProperty("AttackModify").SetValue(result, (float)cell.NumericCellValue, null);
                }
                //效果列表
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    int count = (int)cell.NumericCellValue;
                    result.GetType().GetProperty("ParamList").SetValue(result, new ResultParam[count], null);
                }
                for (int innerIndex = 0; innerIndex < 5; ++innerIndex)
                {
                    if (innerIndex >= result.ParamList.Length)
                    {
                        index += 6;
                        break;
                    }
                    result.ParamList[innerIndex] = new ResultParam();
                    ResultParam param = result.ParamList[innerIndex];

                    cell = row.GetCell(index++);
                    if (null != cell && cell.CellType == CellType.NUMERIC)
                    {
                        param.GetType().GetProperty("ID").SetValue(param, (int)cell.NumericCellValue, null);
                    }
                    for (int paramIndex = 0; paramIndex < param.Param.Length; ++paramIndex)
                    {
                        cell = row.GetCell(index++);
                        if (null != cell && cell.CellType == CellType.NUMERIC)
                        {
                            param.Param[paramIndex] = (float)cell.NumericCellValue;
                        }
                    }
                }
                //额外combo效果数量
                int extraComboCount = 3;
                result.GetType().GetProperty("ExtraParamList").SetValue(result, new ExtraComboResultParam[extraComboCount], null);
                for (int i = 0; i < extraComboCount; ++i)
                {
                    result.ExtraParamList[i] = new ExtraComboResultParam();
                    ExtraComboResultParam param = result.ExtraParamList[i];

                    cell = row.GetCell(index++);
                    if (null != cell && cell.CellType == CellType.NUMERIC)
                    {
                        int count = (int)cell.NumericCellValue;
                        if (count == 0)
                        {
                            break;
                        }
                        param.GetType().GetProperty("ComboJudgeCount").SetValue(param, (int)cell.NumericCellValue, null);
                    }
                    cell = row.GetCell(index++);
                    if (null != cell && cell.CellType == CellType.STRING)
                    {
                        param.GetType().GetProperty("ComboTips").SetValue(param, cell.StringCellValue, null);
                    }
                    cell = row.GetCell(index++);
                    if (null != cell && cell.CellType == CellType.NUMERIC)
                    {
                        param.GetType().GetProperty("SkillResultID").SetValue(param, (int)cell.NumericCellValue, null);
                    }
                }
                //ComboJudgeList
                //cell = row.GetCell(index++);
                //if (null != cell && cell.CellType == CellType.STRING)
                //{
                //    result.ComboJudgeList = ConvertorTool.StringToArray_Int(cell.StringCellValue);
                //}
                //ComboResultIDList
                //cell = row.GetCell(index++);
                //if (null != cell && cell.CellType == CellType.STRING)
                //{
                //    result.ComboResultIDList = ConvertorTool.StringToArray_Int(cell.StringCellValue);
                //}
            }
            convertor.SkillResultList.Add(result.ID, result);
        }
		if (File.Exists(targetPath))
		{
			File.Delete(targetPath);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
		{
			byte[] buff = convertor.Save();
			targetFile.Write(buff, 0, buff.Length);
			Debug.Log(targetPath);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			byte[] buff = new byte[targetFile.Length];
			targetFile.Read(buff, 0, (int)targetFile.Length);
			SkillResultTable table = new SkillResultTable();
			table.Load(buff);
			foreach (var infoPair in table.SkillResultList)
			{
				Debug.Log(infoPair.Value.ComboNum);
			}
		}
	}
}
#endregion

#region BuffTableConvertor
public class BuffTableConvertor : BuffTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(BuffInfoList.Count);
        foreach (var BuffMsg in BuffInfoList)
        {
            BuffMsg.Value.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        BuffTableConvertor convertor = new BuffTableConvertor();
        convertor.BuffInfoList = new Dictionary<int, BuffInfo>();

        PropertyInfo[] vInfo = typeof(BuffInfo).GetProperties();
        foreach (IRow row in sheet)
        {
            int index = 0;
            ICell data = row.GetCell(index);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;
            BuffInfo info = new BuffInfo();
            ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, data);                       //ID
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//BuffName
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//BuffDesc
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//BuffIcon
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//BuffType
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//BuffEffectType
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//BuffPercent
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//BuffPercentParam
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//BuffDuration
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//BuffDurationParam
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//IsFirstWork
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//Period
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//IsNotRemoveForDead
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//Replaceable
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//BuffEffectID
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//MultiplyParam
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[index], info, row.GetCell(index));//AddParam
            for (int i = 0; i < BuffInfo.PropertyCount; ++i)
            {
                if (info.PropertyPercentList == null)
                {
                    info.PropertyPercentList = new float[BuffInfo.PropertyCount];
                }
                ++index;
                ICell c = row.GetCell(index);
                if (c != null)
                {
                    info.PropertyPercentList[i] = (float)c.NumericCellValue;
                }
            }
            for (int i = 0; i < BuffInfo.PropertyCount; ++i)
            {
                if (info.PropertyValueList == null)
                {
                    info.PropertyValueList = new float[BuffInfo.PropertyCount];
                }
                ++index;
                ICell c = row.GetCell(index);
                if (c != null)
                {
                    info.PropertyValueList[i] = (float)c.NumericCellValue;
                }
            }
            int infoIndex = index - BuffInfo.PropertyCount * 2 + 2 + 1;
            ++index; ExcelTableTools.SetValue<BuffInfo>(vInfo[infoIndex], info, row.GetCell(index));//效果列表
            info.BuffResultList = new List<BuffResultInfo>();
            for (int innerIndex = 0; innerIndex < info.BuffResultCount; ++innerIndex)
            {
                BuffResultInfo param = new BuffResultInfo();
                ++index;
                ICell cell = row.GetCell(index);
                if (cell == null)
                {
                    Debug.LogError("result count is error,ID:"+info.ID);
                    continue;
                }
                param.ID = (int)cell.NumericCellValue;

                for (int i = 0; i < param.ParamList.Length; ++i)
                {
                    ++index;
                    ICell c = row.GetCell(index);
                    if (c != null)
                    {
                        param.ParamList[i] = (float)c.NumericCellValue;
                    }
                }
                info.BuffResultList.Add(param);
            }
            convertor.BuffInfoList.Add(info.ID, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
            Debug.Log(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            BuffTable table = new BuffTable();
            table.Load(buff);
            foreach (var item in table.BuffInfoList.Values)
            {
                Debug.Log(item.ID);
                foreach (var index in item.BuffResultList)
                {
                    Debug.Log("param id is " + index.ID);
                    for (int i = 0; i < index.ParamList.Length; ++i)
                    {
                        Debug.Log("param index is " + i + ", " + index.ParamList[i]);
                    }
                }
            }
        }
    }
}

#endregion

#region BuffRelationTableLoader
public class BuffRelationTableLoader : BuffRelationTable
{
    public static void Convert(ISheet sheet, string targetPath)
    {
        BuffRelationTableLoader tableLoader = new BuffRelationTableLoader();
        int count = (int)Buff.ENBuffType.Count;
        tableLoader.m_relationArray = new byte[count * count];

        for (int index = 1; index < count + 1; ++index)
        {
            for (int innerIndex = 1; innerIndex < count + 1; ++innerIndex)
            {
                int i = (index - 1) * count + innerIndex - 1;
                tableLoader.m_relationArray[i] = (byte)sheet.GetRow(index).GetCell(innerIndex).NumericCellValue;
            }
        }
        for (int index = 1; index < count + 1; ++index)
        {
            string temp = "";
            for (int innerIndex = 1; innerIndex < count + 1; ++innerIndex)
            {
                int i = (index - 1) * count + innerIndex - 1;
                temp += tableLoader.m_relationArray[i] + ", ";
            }
            Debug.Log(temp);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = tableLoader.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
    }
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_relationArray.Length);
        helper.InnerStream.Write(m_relationArray, 0, m_relationArray.Length);
        return helper.GetBytes();
    }
}
#endregion
#region BuffEffectTableConvertor
public class BuffEffectTableConvertor : BuffEffectTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(BuffEffectInfoList.Count);
        foreach (var BuffMsg in BuffEffectInfoList)
        {
            BuffMsg.Value.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        BuffEffectTableConvertor convertor = new BuffEffectTableConvertor();
        convertor.BuffEffectInfoList = new Dictionary<int, BuffEffectInfo>();

        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            BuffEffectInfo buff = ExcelTableTools.GenericObj<BuffEffectInfo>(row);
            convertor.BuffEffectInfoList.Add(buff.ID, buff);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            BuffEffectTable table = new BuffEffectTable();
            table.Load(buff);
            foreach (var infoPair in table.BuffEffectInfoList)
            {
                Debug.Log(infoPair.Value.ID);
            }
        }
    }
}
#endregion

#region EquipBaseTableLoader
public class EquipBaseTableLoader : EquipTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(EquipInfoList.Count);
        foreach (EquipInfo info in EquipInfoList.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        EquipBaseTableLoader convertor = new EquipBaseTableLoader();
        convertor.EquipInfoList = new SortedList<int, EquipInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            EquipInfo equipInfo = ExcelTableTools.GenericObj<EquipInfo>(row);
            convertor.EquipInfoList.Add(equipInfo.EquipId, equipInfo);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            EquipBaseTableLoader table = new EquipBaseTableLoader();
            table.Load(buff);
            foreach (EquipInfo info in table.EquipInfoList.Values)
            {
                Debug.Log(info.EquipId + " " + info.EquipName);
            }
        }
    }
}
#endregion

#region AnimationTableLoader
public class AnimationTableLoader : AnimationTable
{
	public byte[] Save()
	{
		BinaryHelper helper = new BinaryHelper();
		helper.Write(AnimInfoList.Count);
		foreach (AnimInfo info in AnimInfoList.Values)
		{
			info.Save(helper);
		}
		return helper.GetBytes();
	}

	public static void Convert(ISheet sheet, string targetPath)
	{
		AnimationTableLoader convertor = new AnimationTableLoader();
		convertor.AnimInfoList = new Dictionary<string, AnimInfo>();
		foreach (IRow rowData in sheet)
		{
			if (null == rowData)
			{
				continue;
			}
			if (null == rowData.GetCell(0))
			{
				continue;
			}
			List<string> temp = new List<string>();
			for (int index = 2; index < rowData.Cells.Count; ++index)
			{
				if (null == rowData.GetCell(index))
				{
					continue;
				}
				string stringValue = rowData.GetCell(index).StringCellValue;
				if (string.IsNullOrEmpty(stringValue))
				{
					continue;
				}
				temp.Add(stringValue);
			}
			if (temp.Count <= 0)
			{
				continue;
			}
			AnimInfo info = new AnimInfo();
			info.AnimType = rowData.GetCell(0).StringCellValue;
			info.AnimName = temp.ToArray();
            convertor.AnimInfoList.Add(info.AnimType, info);
		}

		if (File.Exists(targetPath))
		{
			File.Delete(targetPath);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
		{
			byte[] buff = convertor.Save();
			targetFile.Write(buff, 0, buff.Length);
			Debug.Log(targetPath);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			byte[] buff = new byte[targetFile.Length];
			targetFile.Read(buff, 0, (int)targetFile.Length);
			AnimationTable table = new AnimationTable();
			table.Load(buff);
			foreach (var key in table.AnimInfoList.Keys)
			{
				Debug.Log(key);
			}
		}
	}
}
#endregion

#region ModelInfoTableConvertor
public class ModelInfoTableConvertor : ModelInfoTable
{
	public byte[] Save()
	{
		BinaryHelper helper = new BinaryHelper();
		helper.Write(m_list.Count);
        foreach (ModelInfo info in m_list.Values)
		{
			info.Save(helper);
		}
		return helper.GetBytes();
	}
	public static void Convert(ISheet sheet, string targetPath)
	{
		ModelInfoTableConvertor convertor = new ModelInfoTableConvertor();
        convertor.m_list = new Dictionary<int, ModelInfo>();
		foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

			ModelInfo info = ExcelTableTools.GenericObj<ModelInfo>(row);
            convertor.m_list.Add(info.ModelId, info);
		}
		if (File.Exists(targetPath))
		{
			File.Delete(targetPath);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
		{
			byte[] buff = convertor.Save();
			targetFile.Write(buff, 0, buff.Length);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			byte[] buff = new byte[targetFile.Length];
			targetFile.Read(buff, 0, (int)targetFile.Length);
			ModelInfoTable table = new ModelInfoTable();
			table.Load(buff);
            foreach (var info in table.m_list.Values)
			{
				Debug.Log(info.ModelId + " " + info.ModelFile);
			}
		}
		Debug.Log(targetPath);
	}
}
#endregion

#region WeaponInfoTableConvertor
public class WeaponInfoTableConvertor : WeaponInfoTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_list.Count);
        foreach (WeaponInfo info in m_list.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        WeaponInfoTableConvertor convertor = new WeaponInfoTableConvertor();
        convertor.m_list = new Dictionary<int, WeaponInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            WeaponInfo info = ExcelTableTools.GenericObj<WeaponInfo>(row);
            convertor.m_list.Add(info.WeaponID, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            WeaponInfoTable table = new WeaponInfoTable();
            table.Load(buff);
            foreach (var info in table.m_list.Values)
            {
                Debug.Log(info.WeaponID + " " + info.LeftModelID + info.RightModelID);
            }
        }
        Debug.Log(targetPath);
    }
}
#endregion

#region UILoadInfoTable
public class UILoadInfoTableConvertor : UILoadInfoTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_UILoadDic.Count);
        foreach (UILoadInfo info in m_UILoadDic.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        UILoadInfoTableConvertor convertor = new UILoadInfoTableConvertor();
        convertor.m_UILoadDic = new Dictionary<int, UILoadInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            UILoadInfo info = ExcelTableTools.GenericObj<UILoadInfo>(row);
            convertor.m_UILoadDic.Add(info.UIID, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            UILoadInfoTable table = new UILoadInfoTable();
            table.Load(buff);
            foreach (var info in table.m_UILoadDic.Values)
            {
                Debug.Log(info.UINameStr + " " + info.UIStateStr + " " + info.IsExitDestroy + " " + info.IsDynamicLoad);
            }
        }
        Debug.Log(targetPath);
    }
}
#endregion

#region IconTableConvertor
public class IconTableConvertor : IconTable
{
	public byte[] Save()
	{
		BinaryHelper helper = new BinaryHelper();
        helper.Write(m_list.Count);
        foreach (IconInfo info in m_list.Values)
		{
			info.Save(helper);
		}
		return helper.GetBytes();
	}
	public static void Convert(ISheet sheet, string targetPath)
	{
		IconTableConvertor convertor = new IconTableConvertor();
        convertor.m_list = new Dictionary<int, IconInfo>();
		foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

			IconInfo info = ExcelTableTools.GenericObj<IconInfo>(row);
            convertor.m_list.Add(info.ID, info);
		}
		if (File.Exists(targetPath))
		{
			File.Delete(targetPath);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
		{
			byte[] buff = convertor.Save();
			targetFile.Write(buff, 0, buff.Length);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			byte[] buff = new byte[targetFile.Length];
			targetFile.Read(buff, 0, (int)targetFile.Length);
			IconTable table = new IconTable();
			table.Load(buff);
            foreach (var info in table.m_list.Values)
			{
				Debug.Log(info.ID + " " + info.AtlasName + " " + info.SpriteName);
			}
		}
	}
}
#endregion

#region CheckInfoTableConvertor
/*public class CheckInfoTableConvertor : CheckInfoTable
{
	public byte[] Save()
	{
		BinaryHelper helper = new BinaryHelper();
		helper.Write(CheckInfoList.Count);
		foreach (CheckInfo info in CheckInfoList)
		{
			info.Save(helper);
		}
		return helper.GetBytes();
	}
	public static void Convert(ISheet sheet, string targetPath)
	{
		CheckInfoTableConvertor convertor = new CheckInfoTableConvertor();
		convertor.CheckInfoList = new List<CheckInfo>();
		foreach (IRow row in sheet)
		{
			CheckInfo info = ExcelTableTools.GenericObj<CheckInfo>(row);
			if (0 == info.CheckID)
			{
				continue;
			}
			convertor.CheckInfoList.Add(info);
		}
		if (File.Exists(targetPath))
		{
			File.Delete(targetPath);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
		{
			byte[] buff = convertor.Save();
			targetFile.Write(buff, 0, buff.Length);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			byte[] buff = new byte[targetFile.Length];
			targetFile.Read(buff, 0, (int)targetFile.Length);
			CheckInfoTable table = new CheckInfoTable();
			table.Load(buff);
			foreach (CheckInfo info in table.CheckInfoList)
			{
				Debug.Log(info.CheckID + " " + info.CheckType + " " + info.CheckIcon + " " + info.CheckDesc + " " + info.EvnID);
			}
		}
	}
}*/
#endregion

#region NpcSayTableConvertor
/*public class NpcSayTableConvertor : NpcSayTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(NpcSayList.Count);
        foreach (NpcSayInfo info in NpcSayList)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        NpcSayTableConvertor convertor = new NpcSayTableConvertor();
        convertor.NpcSayList = new List<NpcSayInfo>();
        foreach (IRow row in sheet)
        {
            NpcSayInfo npcSayInfo = ExcelTableTools.GenericObj<NpcSayInfo>(row);
            if (0 == npcSayInfo.id)
            {
                continue;
            }
            convertor.NpcSayList.Add(npcSayInfo);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            NpcSayTable table = new NpcSayTable();
            table.Load(buff);
            foreach (NpcSayInfo info in table.NpcSayList)
            {
                Debug.Log(info.id + " "+info.count + " " + info.say1);
            }
        }
    }
}*/
#endregion

#region NpcShopTableConvertor
/*public class NpcShopTableConvertor : ShopTable
{
	public byte[] Save()
	{
		BinaryHelper helper = new BinaryHelper();
		helper.Write(NpcShopList.Count);
		Save(helper);
		return helper.GetBytes();
	}
	public static void Convert(ISheet sheet, string targetPath)
	{
		NpcShopTableConvertor convertor = new NpcShopTableConvertor();
		convertor.NpcShopList = new Dictionary<int, NpcShop >();
		foreach (IRow row in sheet)
		{
            int index = 0;
            ICell cell = row.GetCell(index++);
            if ((cell == null) || (cell.CellType != CellType.NUMERIC)) continue;
			int id = (int)cell.NumericCellValue;
			if (0 == id)	continue;
            
            cell = row.GetCell(index++);

            NpcShop sayAry = new NpcShop();
            sayAry.ShopId = id;
            sayAry.ShopName = cell.StringCellValue;

            int nCount = 0;
            cell = row.GetCell(index++);
            if ((cell != null) && (cell.CellType == CellType.NUMERIC))
                nCount = (int)cell.NumericCellValue;
			

            sayAry.ShopItems = new List<int>();
            for (int i = 0; i < nCount; ++i)
            {
                cell = row.GetCell(index++);
                if(cell != null)
                    sayAry.ShopItems.Add((int)cell.NumericCellValue);
            }
            convertor.NpcShopList.Add(id, sayAry);
 
            sayAry = null;
		}
		if (File.Exists(targetPath))
		{
			File.Delete(targetPath);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
		{
			byte[] buff = convertor.Save();
			targetFile.Write(buff, 0, buff.Length);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			byte[] buff = new byte[targetFile.Length];
			targetFile.Read(buff, 0, (int)targetFile.Length);
			ShopTable table = new ShopTable();
			table.Load(buff);
			foreach (KeyValuePair<int, NpcShop > row in table.NpcShopList)
			{
                NpcShop info = row.Value;
				Debug.Log("ShopID = " + info.ShopId + ", ShopName = " + info.ShopName);
			}
		}
	}
}*/
#endregion

#region MissionTableConvertor
public class MissionTableConvertor : MissionTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(MissionInfoList.Count);
        foreach (var MissionInfo in MissionInfoList)
        {
            MissionInfo.Value.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        MissionTableConvertor convertor = new MissionTableConvertor();
        convertor.MissionInfoList = new Dictionary<int,MissionInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            MissionInfo info = ExcelTableTools.GenericObj<MissionInfo>(row);
            convertor.MissionInfoList.Add(info.MissionID,info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            MissionTable table = new MissionTable();
            table.Load(buff);
            foreach (var mission in table.MissionInfoList)
            {
                Debug.Log(mission.Value.MissionID + " " + mission.Value.MissionName);
            }
        }
    }
//     public byte[] Save()
//     {
//         BinaryHelper helper = new BinaryHelper();
//         helper.Write(MissionInfoList.Count);
//         foreach (var resultPair in MissionInfoList)
//         {
//             resultPair.Value.Save(helper);
//         }
//         return helper.GetBytes();
//     }
//     public static void Convert(ISheet sheet, string targetPath)
//     {
//         MissionTableConvertor convertor = new MissionTableConvertor();
//         convertor.MissionInfoList = new Dictionary<int,MissionInfo>();
// 
//         foreach (IRow row in sheet)
//         {
//             MissionInfo result;
//             {
//                 int index = 0;
//                 ICell cell = null;
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     int id = (int)cell.NumericCellValue;
//                     if (0 == id)
//                     {
//                         continue;
//                     }
//                     result = new MissionInfo();
//                     result.GetType().GetProperty("TaskId").SetValue(result, id, null);
//                 }
//                 else
//                 {
//                     continue;
//                 }
//                 //任务类型
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("Taskmold").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //任务名称
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("Taskname").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //接任务NPC
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("AcceptNPC").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //交任务NPC
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("PayNPC").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //任务描述
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("Bewrite").SetValue(result, cell.StringCellValue, null);
//                 }
//                 //接受任务对话
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("AcceptDialogue").SetValue(result, cell.StringCellValue, null);
//                 }
//                 //交任务对话
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("PayDialogue").SetValue(result, cell.StringCellValue, null);
//                 }
//                 //前置任务
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("Prepose").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //职业限制
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("Restrict").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //接受任务等级
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("TaskLv").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //接受任务最大等级
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("TaskLvMax").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //奖励经验
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("AwardEXP").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //奖励游戏币
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("Money").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //奖励RMB
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("RMBMoney").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //奖励物品1
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("Article1").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //奖励物品2
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("Article2").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //奖励物品3
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("Article3").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //奖励物品4
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("Article4").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //奖励组队券
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("Team").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //任务完成条件1
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("Term").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //任务完成条件2
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("Term2").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
//                 //任务完成条件3
//                 cell = row.GetCell(index++);
//                 if (null != cell && cell.CellType == CellType.NUMERIC)
//                 {
//                     result.GetType().GetProperty("Term3").SetValue(result, (int)cell.NumericCellValue, null);
//                 }
// //                 //任务完成条件4
// //                 cell = row.GetCell(index++);
// //                 if (null != cell && cell.CellType == CellType.NUMERIC)
// //                 {
// //                     result.GetType().GetProperty("Term4").SetValue(result, (int)cell.NumericCellValue, null);
// //                 }
//             }
//             convertor.MissionInfoList.Add(result.MissionID,result);
//         }
//         if (File.Exists(targetPath))
//         {
//             File.Delete(targetPath);
//         }
//         using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
//         {
//             byte[] buff = convertor.Save();
//             targetFile.Write(buff, 0, buff.Length);
//             Debug.Log(targetPath);
//         }
//         using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
//         {
//             byte[] buff = new byte[targetFile.Length];
//             targetFile.Read(buff, 0, (int)targetFile.Length);
//             MissionTable table = new MissionTable();
//             table.Load(buff);
//             foreach (var infoPair in table.MissionInfoList)
//             {
//                 Debug.Log(infoPair.Value.MissionID);
//             }
//         }
//     }
}
#endregion

#region AptitudeTableConvertor
public class AptitudeTableConvertor : AptitudeTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(AptitudeInfoList.Count);
        foreach (var AptitudeInfo in AptitudeInfoList)
        {
            AptitudeInfo.Value.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        AptitudeTableConvertor convertor = new AptitudeTableConvertor();
        convertor.AptitudeInfoList = new Dictionary<int, AptitudeInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            AptitudeInfo info = ExcelTableTools.GenericObj<AptitudeInfo>(row);
            convertor.AptitudeInfoList.Add(info.AptitudeID, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            AptitudeTable table = new AptitudeTable();
            table.Load(buff);
            foreach (var aptitude in table.AptitudeInfoList)
            {
                Debug.Log(aptitude.Value.AptitudeID + " " + aptitude.Value.AptitudeName);
            }
        }
    }
}
#endregion

#region StringTableConvertor
public class StringTableConvertor : StringTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_stringInfoList.Length);
        for (int index = 1; index < m_stringInfoList.Length;index++ )
        {
            m_stringInfoList[index].Save(helper);
        }
//         foreach (var AptitudeInfo in StringInfoList)
//         {
//             AptitudeInfo.Value.Save(helper);
//         }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        Debug.Log("StringTable Convert");
        StringTableConvertor convertor = new StringTableConvertor();
        convertor.m_stringInfoList = new StringInfo[(int)ENStringIndex.MAX];
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            StringInfo info = ExcelTableTools.GenericObj<StringInfo>(row);
            if (info.ID >= (int)ENStringIndex.MAX)
            {
                continue;
            }
            convertor.m_stringInfoList[info.ID] = info;
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            StringTable table = new StringTable();
            table.Load(buff);
            for (int index = 1; index < table.m_stringInfoList.Length; index++)
            {
                Debug.Log(table.m_stringInfoList[index].ID + " " + table.m_stringInfoList[index].Name);
            }
        }
    }
}
#endregion

#region WorldParamTableConverter
public class WorldParamTableConverter : WorldParamTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(WorldParamInfoList.Count);
        foreach (WorldParamInfo info in WorldParamInfoList.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        WorldParamTableConverter convertor = new WorldParamTableConverter();
        convertor.WorldParamInfoList = new SortedList<int, WorldParamInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            WorldParamInfo worldParamInfo = ExcelTableTools.GenericObj<WorldParamInfo>(row);
            convertor.WorldParamInfoList.Add(worldParamInfo.ID, worldParamInfo);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            WorldParamTable table = new WorldParamTable();
            table.Load(buff);
            foreach (WorldParamInfo info in table.WorldParamInfoList.Values)
            {
                Debug.Log(info.ID + " " + info.Describe);
            }
        }
    }
}
#endregion

#region CDTableConverter
public class CDTableConverter : CDTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(CDInfoList.Count);
        foreach (CDInfo info in CDInfoList.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        CDTableConverter convertor = new CDTableConverter();
        convertor.CDInfoList = new SortedList<int, CDInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            CDInfo CDInfo = ExcelTableTools.GenericObj<CDInfo>(row);
            convertor.CDInfoList.Add(CDInfo.ID, CDInfo);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            CDTable table = new CDTable();
            table.Load(buff);
            foreach (CDInfo info in table.CDInfoList.Values)
            {
                Debug.Log(info.ID + " " + info.CDTime);
            }
        }
    }
}
#endregion

#region ServersTableConverter
/*public class ServersTableConverter : ServerTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_serverList.Count);
        foreach (ServerInfo info in m_serverList)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        ServersTableConverter convertor = new ServersTableConverter();
        convertor.m_serverList = new List<ServerInfo>();
        foreach (IRow row in sheet)
        {
            ServerInfo serverInfo = ExcelTableTools.GenericObj<ServerInfo>(row);
            if ("Name" == serverInfo.m_name)
            {
                continue;
            }
            convertor.m_serverList.Add(serverInfo);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            ServerTable table = new ServerTable();
            table.Load(buff);
            foreach (ServerInfo info in table.m_serverList)
            {
                Debug.Log(info.m_name + " " + info.m_IP);
            }
        }
    }
}*/
#endregion

#region SceneMapNumericTableConverter
/*public class SceneMapNumericTableConverter : SceneMapNumericTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(SceneMapNumericInfoList.Count);
        foreach (var BuffMsg in SceneMapNumericInfoList)
        {
            BuffMsg.Value.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        SceneMapNumericTableConverter convertor = new SceneMapNumericTableConverter();
        convertor.SceneMapNumericInfoList = new Dictionary<int, SceneMapNumericInfo>();

        foreach (IRow row in sheet)
        {
            SceneMapNumericInfo buff;
            {
                int index = 0;
                ICell cell = null;
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    int id = (int)cell.NumericCellValue;
                    if (0 == id)
                    {
                        continue;
                    }
                    buff = new SceneMapNumericInfo();
                    buff.GetType().GetProperty("ID").SetValue(buff, id, null);
                }
                else
                {
                    continue;
                }
                //场景横坐标
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    buff.GetType().GetProperty("SceneMapX").SetValue(buff, (float)cell.NumericCellValue, null);
                }
                //场景纵坐标
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    buff.GetType().GetProperty("SceneMapY").SetValue(buff, (float)cell.NumericCellValue, null);
                }
                //小地图X
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    buff.GetType().GetProperty("IconMapX").SetValue(buff, (float)cell.NumericCellValue, null);
                }
                //小地图Y
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    buff.GetType().GetProperty("IconMapY").SetValue(buff, (float)cell.NumericCellValue, null);
                }
                //偏移量X
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    buff.GetType().GetProperty("DeviationX").SetValue(buff, (float)cell.NumericCellValue, null);
                }
                //偏移量Y
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    buff.GetType().GetProperty("DeviationY").SetValue(buff, (float)cell.NumericCellValue, null);
                }
                //小地图路径Y
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.STRING)
                {
                    buff.GetType().GetProperty("IconPath").SetValue(buff, (string)cell.StringCellValue, null);
                }
                //传送点坐标
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    int count = (int)cell.NumericCellValue;
                    buff.GetType().GetProperty("TeleporterList").SetValue(buff, new TeleportInfo[count], null);
                }
                for (int innerIndex = 0; innerIndex < buff.TeleporterList.Length; ++innerIndex)
                {
                    buff.TeleporterList[innerIndex] = new TeleportInfo();
                    TeleportInfo param = buff.TeleporterList[innerIndex];
                    for (int paramIndex = 0; paramIndex < param.ParamList.Length; ++paramIndex)
                    {
                        cell = row.GetCell(index++);
                        if (null != cell && cell.CellType == CellType.NUMERIC)
                        {
                            param.ParamList[paramIndex] = (float)cell.NumericCellValue;
                        }
                    }
                }
            }
            convertor.SceneMapNumericInfoList.Add(buff.ID, buff);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
            Debug.Log(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            SceneMapNumericTable table = new SceneMapNumericTable();
            table.Load(buff);
            foreach (var infoPair in table.SceneMapNumericInfoList)
            {
                Debug.Log(infoPair.Value.ID);
            }
        }
    }
}*/
#endregion

#region EquipExpMoneyTableConverter
/*public class EquipExpMoneyTableConverter : EquipExpMoneyTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_equipExpMoneyInfoList.Count);
        foreach (EquipExpMoneyInfo info in m_equipExpMoneyInfoList)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        EquipExpMoneyTableConverter convertor = new EquipExpMoneyTableConverter();
        convertor.m_equipExpMoneyInfoList = new List<EquipExpMoneyInfo>();
        foreach (IRow row in sheet)
        {
            EquipExpMoneyInfo info = ExcelTableTools.GenericObj<EquipExpMoneyInfo>(row);
            if (0 == info.level)
            {
                continue;
            }
            convertor.m_equipExpMoneyInfoList.Add(info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            EquipExpMoneyTable table = new EquipExpMoneyTable();
            table.Load(buff);
            foreach (EquipExpMoneyInfo info in table.m_equipExpMoneyInfoList)
            {
                Debug.Log(info.level + " " + info.maxExp + " " + info.needMoney);
            }
        }
    }
}*/
#endregion

#region ShakeTableConverter
/*public class ShakeTableLoader : ShakeTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(ShakeParamInfoList.Count);
        foreach (ShakeParamInfo info in ShakeParamInfoList.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        ShakeTableLoader convertor = new ShakeTableLoader();
        convertor.ShakeParamInfoList = new SortedList<int, ShakeParamInfo>();
        foreach (IRow row in sheet)
        {
            ShakeParamInfo shakeInfo = ExcelTableTools.GenericObj<ShakeParamInfo>(row);
            if (0 == shakeInfo.ID)
            {
                continue;
            }
            convertor.ShakeParamInfoList.Add(shakeInfo.ID, shakeInfo);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            ShakeTableLoader table = new ShakeTableLoader();
            table.Load(buff);
            foreach (ShakeParamInfo info in table.ShakeParamInfoList.Values)
            {
                Debug.Log(info.ID + " ");
            }
        }
    }
}*/
#endregion

#region PlayerGuideTableConverter
/*public class PlayerGuideTableLoader : PlayerGuideTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(PlayerGuideList.Count);
        foreach (PlayerGuideInfo info in PlayerGuideList)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        PlayerGuideTableLoader convertor = new PlayerGuideTableLoader();
        convertor.PlayerGuideList = new List<PlayerGuideInfo>();
        foreach (IRow row in sheet)
        {
            PlayerGuideInfo playerGuideInfo = ExcelTableTools.GenericObj<PlayerGuideInfo>(row);
            if (0 == playerGuideInfo.id)
            {
                continue;
            }
            convertor.PlayerGuideList.Add(playerGuideInfo);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            PlayerGuideTableLoader table = new PlayerGuideTableLoader();
            table.Load(buff);
            foreach (PlayerGuideInfo info in table.PlayerGuideList)
            {
                Debug.Log(info.id + " " + info.icon + " " +info.groupID);
            }
        }
    }
}*/
#endregion

#region IllumeTableConverter
/*public class IllumeTableConverter : IllumeTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(IllumeInfoList.Count);
        foreach (var BuffMsg in IllumeInfoList)
        {
            BuffMsg.Value.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        IllumeTableConverter convertor = new IllumeTableConverter();
        convertor.IllumeInfoList = new Dictionary<int, IllumeInfo>();

        foreach (IRow row in sheet)
        {
            IllumeInfo buff;
            {
                int index = 0;
                ICell cell = null;
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    int id = (int)cell.NumericCellValue;
                    if (0 == id)
                    {
                        continue;
                    }
                    buff = new IllumeInfo();
                    buff.GetType().GetProperty("ID").SetValue(buff, id, null);
                }
                else
                {
                    continue;
                }
                //副本ID
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    buff.GetType().GetProperty("DungeonID").SetValue(buff, (int)cell.NumericCellValue, null);
                }
                //副本群ID
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    buff.GetType().GetProperty("DungeonGroupID").SetValue(buff, (int)cell.NumericCellValue, null);
                }
                //完成条件
                cell = row.GetCell(index++);
                if (null != cell && cell.CellType == CellType.NUMERIC)
                {
                    int count = (int)cell.NumericCellValue;
                    buff.GetType().GetProperty("ConditionList").SetValue(buff, new ConditionInfo[count], null);
                }
                for (int innerIndex = 0; innerIndex < buff.ConditionList.Length; ++innerIndex)
                {
                    buff.ConditionList[innerIndex] = new ConditionInfo();
                    ConditionInfo param = buff.ConditionList[innerIndex];
                    for (int paramIndex = 0; paramIndex < param.ParamList.Length; ++paramIndex)
                    {
                        cell = row.GetCell(index++);
                        if (null != cell && cell.CellType == CellType.NUMERIC)
                        {
                            param.ParamList[paramIndex] = (int)cell.NumericCellValue;
                        }
                    }
                }
            }
            convertor.IllumeInfoList.Add(buff.ID, buff);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
            Debug.Log(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            IllumeTable table = new IllumeTable();
            table.Load(buff);
            foreach (var infoPair in table.IllumeInfoList)
            {
                //Debug.Log(infoPair.Value.DungeonID);
                foreach (ConditionInfo condinfo in infoPair.Value.ConditionList)
                {
                    Debug.Log(infoPair.Value.ID);
                    Debug.Log("condinfo.ParamList[0]~~" + condinfo.ParamList[0]);
                }
            }
        }
    }
}*/
#endregion

#region BossTableConverter
/*public class BossTableLoader : BossTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(BossInfoList.Count);
        foreach (BossInfo info in BossInfoList)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        BossTableLoader convertor = new BossTableLoader();
        convertor.BossInfoList = new List<BossInfo>();
        foreach (IRow row in sheet)
        {
            BossInfo bossInfo = ExcelTableTools.GenericObj<BossInfo>(row);
            if (0 == bossInfo.ID)
            {
                continue;
            }
            convertor.BossInfoList.Add(bossInfo);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            BossTableLoader table = new BossTableLoader();
            table.Load(buff);
            foreach (BossInfo info in table.BossInfoList)
            {
                Debug.Log(info.ID + " " + info.path + " " + info.say);
            }
        }
    }
}*/
#endregion

#region SandTableInfoTableConverter
/*public class SandTableInfoTableLoader : SandTableInfoTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(SandTableInfoList.Count);
        foreach (SandTableInfo info in SandTableInfoList)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        SandTableInfoTableLoader convertor = new SandTableInfoTableLoader();
        convertor.SandTableInfoList = new List<SandTableInfo>();
        foreach (IRow row in sheet)
        {
            SandTableInfo sandTableInfo = ExcelTableTools.GenericObj<SandTableInfo>(row);
            if (0 == sandTableInfo.id)
            {
                continue;
            }
            convertor.SandTableInfoList.Add(sandTableInfo);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            SandTableInfoTableLoader table = new SandTableInfoTableLoader();
            table.Load(buff);
            foreach (SandTableInfo info in table.SandTableInfoList)
            {
                Debug.Log(info.id + " " + info.nextAreaX1 + " " + info.nextAreaY1);
            }
        }
    }
}*/
#endregion

#region LvUpExpTableConverter
public class LvUpExpTableLoader : LevelUpTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_levelUpMap.Count);
        foreach (KeyValuePair<int ,LevelUpInfo> item in m_levelUpMap)
        {
            item.Value.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        LvUpExpTableLoader convertor = new LvUpExpTableLoader();
        convertor.m_levelUpMap = new Dictionary<int, LevelUpInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            LevelUpInfo info = ExcelTableTools.GenericObj<LevelUpInfo>(row);
            convertor.m_levelUpMap.Add(info.lvl, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            LvUpExpTableLoader table = new LvUpExpTableLoader();
            table.Load(buff);
            foreach (KeyValuePair<int, LevelUpInfo> item in table.m_levelUpMap)
            {
                Debug.Log(item.Value.lvl + " " + item.Value.LvUp_need_exp + " " + item.Value.Monster_Exp);
            }
        }
    }
}
#endregion

#region DungeonEventTableConvertor
/*public class DungeonEventTableLoader : DungeonEventTable
{
    public void Convert(string filePath, string targetPath)
    {
        m_dungeonEventMap = new Dictionary<int, DungeonEventInfo>();
        using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            HSSFWorkbook workbook = new HSSFWorkbook(file);
            ISheet sheet = workbook.GetSheetAt(0);
            foreach (IRow rowData in sheet)
            {
                if (null == rowData)
                {
                    continue;
                }
                if (null == rowData.GetCell(0))
                {
                    continue;
                }
                if (CellType.NUMERIC != rowData.GetCell(0).CellType)
                {
                    continue;
                }
                DungeonEventInfo info = new DungeonEventInfo();
                int cellIndex = 0;
                while (cellIndex < rowData.Cells.Count)
                {
                    ICell data = rowData.GetCell(cellIndex++);
                    info.EventID = null != data ? (int)data.NumericCellValue : 0;
                    if (0 == info.EventID)
                    {
                        break;
                    }

                    data = rowData.GetCell(cellIndex++);
                    if (null == data)
                    {
                        info.EventType = -1;
                    }
                    else if ("KillCount" == data.StringCellValue)
                    {
                        info.EventType = 0;
                    }
                    else if ("OnEnterTrigger" == data.StringCellValue)
                    {
                        info.EventType = 1;
                    }
                    else if ("OnExitTrigger" == data.StringCellValue)
                    {
                        info.EventType = 2;
                    }

                    data = rowData.GetCell(cellIndex++);
                    info.TargetID = null != data ? (int)data.NumericCellValue : 0;

                    data = rowData.GetCell(cellIndex++);
                    info.TargetCount = null != data ? (int)data.NumericCellValue : 0;

                    m_dungeonEventMap.Add(info.EventID, info);
                    break;
                }
            }
            //save
            if (m_dungeonEventMap.Count > 0)
            {
                BinaryHelper streamHelper = new BinaryHelper();
                streamHelper.Write(m_dungeonEventMap.Count);

                foreach (KeyValuePair<int, DungeonEventInfo> item in m_dungeonEventMap)
                {
                    streamHelper.Write(item.Value.EventID);
                    streamHelper.Write(item.Value.EventType);
                    streamHelper.Write(item.Value.TargetID);
                    streamHelper.Write(item.Value.TargetCount);
                }
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                FileStream writeFile = new FileStream(targetPath, FileMode.Create);
                byte[] buff = streamHelper.GetBytes();
                writeFile.Write(buff, 0, buff.Length);
                writeFile.Close();
                Debug.Log(sheet.SheetName + "转换完成");
            }
        }
    }
}*/
#endregion

#region DungeonEventResultTableConvertor
/*public class DungeonEventResultTableLoader : DungeonEventResultTable
{
    public void Convert(string filePath, string targetPath)
    {
        m_dungeonEventResultMap = new Dictionary<int, DungeonEventResultInfo>();
        using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            HSSFWorkbook workbook = new HSSFWorkbook(file);
            ISheet sheet = workbook.GetSheetAt(0);
            foreach (IRow rowData in sheet)
            {
                if (null == rowData)
                {
                    continue;
                }
                if (null == rowData.GetCell(0))
                {
                    continue;
                }
                if (CellType.NUMERIC != rowData.GetCell(0).CellType)
                {
                    continue;
                }
                DungeonEventResultInfo info = new DungeonEventResultInfo();
                int cellIndex = 0;
                while (cellIndex < rowData.Cells.Count)
                {
                    ICell data = rowData.GetCell(cellIndex++);
                    info.ResultID = null != data ? (int)data.NumericCellValue : 0;
                    if (0 == info.ResultID)
                    {
                        break;
                    }

                    data = rowData.GetCell(cellIndex++);
                    info.ResultName = null != data ? data.StringCellValue : "";

                    for (int index = 0; index < info.ResultParamList.Length; index++)
                    {
                        data = rowData.GetCell(cellIndex++);
                        if (null == data)
                        {
                            info.ResultParamList[index].paramInt = 0;
                            info.ResultParamList[index].paramString = "";
                        }
                        else if (CellType.NUMERIC == data.CellType)
                        {
                            info.ResultParamList[index].paramInt = (int)data.NumericCellValue;
                            info.ResultParamList[index].paramString = "";
                        }
                        else if (CellType.STRING == data.CellType)
                        {
                            info.ResultParamList[index].paramInt = 0;
                            info.ResultParamList[index].paramString = data.StringCellValue;
                        }
                    }
                    Debug.Log(info.ResultID + " " + info.ResultName + " " + info.ResultParamList[0].paramInt + " " + info.ResultParamList[0].paramString);
                    m_dungeonEventResultMap.Add(info.ResultID, info);
                    break;
                }
            }
            //save
            if (m_dungeonEventResultMap.Count > 0)
            {
                BinaryHelper streamHelper = new BinaryHelper();
                streamHelper.Write(m_dungeonEventResultMap.Count);

                foreach (KeyValuePair<int, DungeonEventResultInfo> item in m_dungeonEventResultMap)
                {
                    streamHelper.Write(item.Value.ResultID);
                    streamHelper.Write(item.Value.ResultName);
                    for (int index = 0; index < item.Value.ResultParamList.Length; index++)
                    {
                        streamHelper.Write(item.Value.ResultParamList[index].paramInt);
                        streamHelper.Write(item.Value.ResultParamList[index].paramString);
                    }
                }
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                FileStream writeFile = new FileStream(targetPath, FileMode.Create);
                byte[] buff = streamHelper.GetBytes();
                writeFile.Write(buff, 0, buff.Length);
                writeFile.Close();
                Debug.Log(sheet.SheetName + "转换完成");
            }
        }
    }
}*/
#endregion

#region DungeonFilesConverter
/*public class DungeonFilesLoader : DungeonFiles
{
    public void Convert(string filePath, string targetPath)
    {
        m_dungeonFilesList = new List<DungeonFilesInfo>();
        using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            HSSFWorkbook workbook = new HSSFWorkbook(file);
            ISheet sheet = workbook.GetSheetAt(0);
            foreach (IRow rowData in sheet)
            {
                if (null == rowData)
                {
                    continue;
                }
                if (null == rowData.GetCell(0))
                {
                    continue;
                }
                if (CellType.NUMERIC != rowData.GetCell(0).CellType)
                {
                    continue;
                }
                DungeonFilesInfo info = new DungeonFilesInfo();
                int cellIndex = 0;
                while (cellIndex < rowData.Cells.Count)
                {
                    ICell data = rowData.GetCell(cellIndex++);
                    info.DungeonID = null != data ? (int)data.NumericCellValue : 0;
                    if (0 == info.DungeonID)
                    {
                        break;
                    }

                    data = rowData.GetCell(cellIndex++);
                    info.GradeID = null != data ? (int)data.NumericCellValue : 0;

                    data = rowData.GetCell(cellIndex++);
                    info.SceneID = null != data ? (int)data.NumericCellValue : 0;

                    data = rowData.GetCell(cellIndex++);
                    info.Area = null != data ? data.StringCellValue : "";

                    data = rowData.GetCell(cellIndex++);
                    info.EventType = null != data ? (int)data.NumericCellValue : 0;

                    data = rowData.GetCell(cellIndex++);
                    info.ERCount = null != data ? (int)data.NumericCellValue : 0;

                    info.ERSet = new EventToResult[info.ERCount];
                    for (int index = 0; index < info.ERCount; index++)
                    {
                        data = rowData.GetCell(cellIndex++);
                        info.ERSet[index].EventCount = null != data ? (int)data.NumericCellValue : 0;

                        info.ERSet[index].Event = new int[info.ERSet[index].EventCount];
                        for (int i = 0; i < info.ERSet[index].EventCount; i++)
                        {
                            data = rowData.GetCell(cellIndex++);
                            info.ERSet[index].Event[i] = null != data ? (int)data.NumericCellValue : 0;
                        }

                        data = rowData.GetCell(cellIndex++);
                        info.ERSet[index].ResultCount = null != data ? (int)data.NumericCellValue : 0;

                        info.ERSet[index].Result = new int[info.ERSet[index].ResultCount];
                        for (int j = 0; j < info.ERSet[index].ResultCount; j++)
                        {
                            data = rowData.GetCell(cellIndex++);
                            info.ERSet[index].Result[j] = null != data ? (int)data.NumericCellValue : 0;
                        }
                    }
                    m_dungeonFilesList.Add(info);
                    break;
                }
            }
            //save
            if (m_dungeonFilesList.Count > 0)
            {
                BinaryHelper streamHelper = new BinaryHelper();
                streamHelper.Write(m_dungeonFilesList.Count);

                foreach (var item in m_dungeonFilesList)
                {
                    streamHelper.Write(item.DungeonID);
                    streamHelper.Write(item.GradeID);
                    streamHelper.Write(item.SceneID);
                    streamHelper.Write(item.Area);
                    streamHelper.Write(item.EventType);
                    streamHelper.Write(item.ERCount);
                    for (int index = 0; index < item.ERCount; index++)
                    {
                        streamHelper.Write(item.ERSet[index].EventCount);
                        for (int i = 0; i < item.ERSet[index].EventCount; i++)
                        {
                            streamHelper.Write(item.ERSet[index].Event[i]);
                            Debug.Log("Event = " + item.ERSet[index].Event[i]);
                        }
                        streamHelper.Write(item.ERSet[index].ResultCount);
                        for (int j = 0; j < item.ERSet[index].ResultCount; j++)
                        {
                            streamHelper.Write(item.ERSet[index].Result[j]);
                        }
                    }
                }
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                FileStream writeFile = new FileStream(targetPath, FileMode.Create);
                byte[] buff = streamHelper.GetBytes();
                writeFile.Write(buff, 0, buff.Length);
                writeFile.Close();
                Debug.Log(sheet.SheetName + "转换完成");
            }
        }
    }
}*/
#endregion


#region RarityRelativeTableConverter
public class RarityRelativeTableConverter : RarityRelativeTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_rarityRelativeMap.Count);
        foreach (RarityRelativeInfo info in m_rarityRelativeMap.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        RarityRelativeTableConverter convertor = new RarityRelativeTableConverter();
        convertor.m_rarityRelativeMap = new Dictionary<int, RarityRelativeInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            RarityRelativeInfo info = ExcelTableTools.GenericObj<RarityRelativeInfo>(row);
            convertor.m_rarityRelativeMap.Add(info.m_rarity, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            RarityRelativeTable table = new RarityRelativeTable();
            table.Load(buff);
            foreach (RarityRelativeInfo info in table.m_rarityRelativeMap.Values)
            {
                Debug.Log(info.m_rarity + " " + info.m_dropItemModelID);
            }
        }
    }
}
#endregion

#region OccupationInfoTableConverter
public class OccupationInfoTableConverter : OccupationInfoTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (OccupationInfo info in m_map.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        OccupationInfoTableConverter convertor = new OccupationInfoTableConverter();
        convertor.m_map = new Dictionary<int, OccupationInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            OccupationInfo info = ExcelTableTools.GenericObj<OccupationInfo>(row);
            convertor.m_map.Add(info.m_id, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            OccupationInfoTable table = new OccupationInfoTable();
            table.Load(buff);
            foreach (OccupationInfo info in table.m_map.Values)
            {
                Debug.Log(info.m_id + " " + info.m_describe);
            }
        }
    }
}
#endregion

#region PlayerRandomNameTableConverter
public class PlayerRandomNameTableConverter : PlayerRandomNameTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (PlayerRandomNameInfo info in m_map.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        PlayerRandomNameTableConverter convertor = new PlayerRandomNameTableConverter();
        convertor.m_map = new Dictionary<int, PlayerRandomNameInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            PlayerRandomNameInfo info = ExcelTableTools.GenericObj<PlayerRandomNameInfo>(row);
            convertor.m_map.Add(info.m_id, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            PlayerRandomNameTable table = new PlayerRandomNameTable();
            table.Load(buff);
            foreach (PlayerRandomNameInfo info in table.m_map.Values)
            {
                Debug.Log(info.m_id + " " + info.m_firstName);
            }
        }
    }
}
#endregion

#region ZoneInfoTableConverter
public class ZoneInfoTableConverter : ZoneInfoTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (ZoneTableInfo info in m_map.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        ZoneInfoTableConverter convertor = new ZoneInfoTableConverter();
        convertor.m_map = new Dictionary<int, ZoneTableInfo>();

        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            ZoneTableInfo info = ExcelTableTools.GenericObj<ZoneTableInfo>(row);
            convertor.m_map.Add(info.m_id, info);
        }


        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            ZoneInfoTable table = new ZoneInfoTable();
            table.Load(buff);
            foreach (ZoneTableInfo info in table.m_map.Values)
            {
                Debug.Log(info.m_id + " " + info.m_name);
            }
        }
    }
}
#endregion

#region StageInfoTableConverter
public class StageInfoTableConverter : StageInfoTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (StageDetailInfo info in m_map.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        StageInfoTableConverter convertor   = new StageInfoTableConverter();
        convertor.m_map                     = new Dictionary<int, StageDetailInfo>();

        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            StageDetailInfo info = ExcelTableTools.GenericObj<StageDetailInfo>(row);
            convertor.m_map.Add(info.m_id, info);
        }

        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            StageInfoTable table = new StageInfoTable();
            table.Load(buff);
            foreach (StageDetailInfo info in table.m_map.Values)
            {
                Debug.Log(info.m_id + " ," + info.m_name + " ,m_tips" + info.m_tips + " " + info.m_requireLevel + "," + info.m_backStageId);
            }
        }
    }
}
#endregion

#region MagicStoneTableConvertor
public class MagicStoneTableConvertor : MagicStoneTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_magicStoneTableInfoList.Count);
        foreach (MagicStoneTableInfo info in m_magicStoneTableInfoList)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        MagicStoneTableConvertor convertor = new MagicStoneTableConvertor();
        convertor.m_magicStoneTableInfoList = new List<MagicStoneTableInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            MagicStoneTableInfo info = ExcelTableTools.GenericObj<MagicStoneTableInfo>(row);
            convertor.m_magicStoneTableInfoList.Add(info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            MagicStoneTable table = new MagicStoneTable();
            table.Load(buff);
            foreach (var info in table.m_magicStoneTableInfoList)
            {
                Debug.Log(info.id + " " + info.magciStoneNumber + " " + info.price + " " + info.discountPrice);
            }
        }
    }
}
#endregion


#region RingExchangeTableConvertor
public class RingExchangeTableConvertor : RingExchangeTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_ringExchangeTableInfoList.Count);
        foreach (RingExchangeTableInfo info in m_ringExchangeTableInfoList)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }
    public static void Convert(ISheet sheet, string targetPath)
    {
        RingExchangeTableConvertor convertor = new RingExchangeTableConvertor();
        convertor.m_ringExchangeTableInfoList = new List<RingExchangeTableInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            RingExchangeTableInfo info = ExcelTableTools.GenericObj<RingExchangeTableInfo>(row);
            convertor.m_ringExchangeTableInfoList.Add(info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            RingExchangeTable table = new RingExchangeTable();
            table.Load(buff);
            foreach (var info in table.m_ringExchangeTableInfoList)
            {
                Debug.Log(info.id + " " + info.startDate );
            }
        }
    }
}
#endregion



#region FloorInfoTableConverter
public class FloorInfoTableConverter : FloorInfoTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (FloorInfo info in m_map.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        FloorInfoTableConverter convertor = new FloorInfoTableConverter();
        convertor.m_map = new Dictionary<int, FloorInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            FloorInfo info = ExcelTableTools.GenericObj<FloorInfo>(row);
            convertor.m_map.Add(info.m_id, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            FloorInfoTable table = new FloorInfoTable();
            table.Load(buff);
            foreach (FloorInfo info in table.m_map.Values)
            {
                Debug.Log(info.m_id +"," + info.m_conditions+","+info.m_layerNum);
            }
        }
    }
}
#endregion

#region ItemInfoTableConverter
public class ItemInfoTableConverter : ItemTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (ItemInfo info in m_map.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        ItemInfoTableConverter convertor = new ItemInfoTableConverter();
        convertor.m_map = new Dictionary<int, ItemInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            ItemInfo info = ExcelTableTools.GenericObj<ItemInfo>(row);
            convertor.m_map.Add(info.m_id, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            ItemTable table = new ItemTable();
            table.Load(buff);
            foreach (ItemInfo info in table.m_map.Values)
            {
                Debug.Log(info.m_id + " " );
            }
        }
    }
}
#endregion

#region FloorRankTableConverter
public class FloorRankTableConverter : FloorRankTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (FloorRankInfo info in m_map.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        FloorRankTableConverter convertor = new FloorRankTableConverter();
        convertor.m_map = new Dictionary<string, FloorRankInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            FloorRankInfo info = ExcelTableTools.GenericObj<FloorRankInfo>(row);
            if ("" == info.m_rank)
            {
                continue;
            }
            convertor.m_map.Add(info.m_rank, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            FloorRankTable table = new FloorRankTable();
            table.Load(buff);
            foreach (FloorRankInfo info in table.m_map.Values)
            {
                Debug.Log(info.m_rank + " " + info.m_expParam + "," + info.m_coinParam);
            }
        }
    }
}
#endregion

#region ScoreParamTableConverter
public class ScoreParamTableConverter : ScoreParamTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (ScoreParamInfo info in m_map.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        ScoreParamTableConverter convertor = new ScoreParamTableConverter();
        convertor.m_map = new Dictionary<int, ScoreParamInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            ScoreParamInfo info = ExcelTableTools.GenericObj<ScoreParamInfo>(row);
            convertor.m_map.Add(info.m_id, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            ScoreParamTable table = new ScoreParamTable();
            table.Load(buff);
            foreach (ScoreParamInfo info in table.m_map.Values)
            {
                Debug.Log(info.m_id + " " + info.m_standardCombo + "," + info.m_nonRespawnPoint+","+ info.m_standardBossKillCount);
            }
        }
    }
}
#endregion

#region BagTableConverter
public class BagTableConverter : BagTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (BagInfo info in m_map.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        BagTableConverter convertor = new BagTableConverter();
        convertor.m_map = new Dictionary<int, BagInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            BagInfo info = ExcelTableTools.GenericObj<BagInfo>(row);
            convertor.m_map.Add(info.m_id, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff         = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            BagTable table      = new BagTable();
            table.Load(buff);
            foreach (BagInfo info in table.m_map.Values)
            {
               Debug.Log(info.m_id + " " + info.m_bagType + "," + info.m_expandSize1 );
            }
        }
    }
}
#endregion

#region RaceInfoTableConverter
public class RaceInfoTableConverter : RaceInfoTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (RaceInfo info in m_map.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        RaceInfoTableConverter convertor = new RaceInfoTableConverter();
        convertor.m_map = new Dictionary<int, RaceInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            RaceInfo info = ExcelTableTools.GenericObj<RaceInfo>(row);
            convertor.m_map.Add(info.m_id, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            RaceInfoTable table = new RaceInfoTable();
            table.Load(buff);
            foreach (RaceInfo info in table.m_map.Values)
            {
                Debug.Log(info.m_id + " " + info.m_describe + "," + info.m_name);
            }
        }
    }
}
#endregion

#region EquipmentTableConverter
public class EquipmentTableConverter : EquipmentTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (EquipmentInfo info in m_map.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        EquipmentTableConverter convertor = new EquipmentTableConverter();
        convertor.m_map = new Dictionary<int, EquipmentInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            EquipmentInfo info = ExcelTableTools.GenericObj<EquipmentInfo>(row);
            convertor.m_map.Add(info.m_id, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            EquipmentTable table = new EquipmentTable();
            table.Load(buff);
            foreach (EquipmentInfo info in table.m_map.Values)
            {
                Debug.Log(info.m_id + " " + info.m_equipName + "," + info.m_fMagAttack_E);
            }
        }
    }
}
#endregion

#region PlayerAttrTableConverter
public class PlayerAttrTableConverter : PlayerAttrTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (KeyValuePair<int, PlayerAttrInfo> item in m_map)
        {
            item.Value.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        PlayerAttrTableConverter convertor = new PlayerAttrTableConverter();
        convertor.m_map = new Dictionary<int, PlayerAttrInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            PlayerAttrInfo info = ExcelTableTools.GenericObj<PlayerAttrInfo>(row);
            convertor.m_map.Add(info.m_level, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            PlayerAttrTableConverter table = new PlayerAttrTableConverter();
            table.Load(buff);
            foreach (KeyValuePair<int, PlayerAttrInfo> item in table.m_map)
            {
                Debug.Log(item.Value.m_level + " " + item.Value.m_needExp + " " + item.Value.m_leaderShip);
            }
        }
    }
}
#endregion

#region LoadingTipsTableConverter
public class LoadingTipsTableConverter : LoadingTipsTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(InfoList.Count);
        foreach (KeyValuePair<int, LoadingTipsInfo> item in InfoList)
        {
            item.Value.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        LoadingTipsTableConverter convertor = new LoadingTipsTableConverter();
        convertor.InfoList = new Dictionary<int, LoadingTipsInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            LoadingTipsInfo info = ExcelTableTools.GenericObj<LoadingTipsInfo>(row);
            convertor.InfoList.Add(info.ID, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            LoadingTipsTableConverter table = new LoadingTipsTableConverter();
            table.Load(buff);
            foreach (KeyValuePair<int, LoadingTipsInfo> item in table.InfoList)
            {
                Debug.Log(item.Value.ID + " " + item.Value.DescribeList );
            }
        }
    }
}
#endregion


#region MessageRespondTableConverter
public class MessageRespondTableConverter : MessageRespondTable
{
	public byte[] Save()
	{
		BinaryHelper helper = new BinaryHelper();
		helper.Write(MessageRespondInfoList.Count);
		foreach (MessageRespondInfo info in MessageRespondInfoList.Values)
		{
			info.Save(helper);
		}
		return helper.GetBytes();
	}

	public static void Convert(ISheet sheet, string targetPath)
	{
		MessageRespondTableConverter convertor = new MessageRespondTableConverter();
		convertor.MessageRespondInfoList = new SortedList<int, MessageRespondInfo>();
		foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

			MessageRespondInfo messageRespondInfo = ExcelTableTools.GenericObj<MessageRespondInfo>(row);
			convertor.MessageRespondInfoList.Add(messageRespondInfo.ID, messageRespondInfo);
		}
		if (File.Exists(targetPath))
		{
			File.Delete(targetPath);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
		{
			byte[] buff = convertor.Save();
			targetFile.Write(buff, 0, buff.Length);
		}
		using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			byte[] buff = new byte[targetFile.Length];
			targetFile.Read(buff, 0, (int)targetFile.Length);
			MessageRespondTable table = new MessageRespondTable();
			table.Load(buff);
			foreach (MessageRespondInfo info in table.MessageRespondInfoList.Values)
			{
				Debug.Log(info.ID + " " + info.Describe);
			}
		}
	}
}
#endregion

#region RoomTagTableConverter
public class RoomTagTableConverter : RoomTagTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(list.Count);
        foreach (RoomTagInfo info in list.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        RoomTagTableConverter convertor = new RoomTagTableConverter();
        convertor.list = new SortedList<int, RoomTagInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            RoomTagInfo messageRespondInfo = ExcelTableTools.GenericObj<RoomTagInfo>(row);
            convertor.list.Add(messageRespondInfo.ID, messageRespondInfo);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            RoomTagTable table = new RoomTagTable();
            table.Load(buff);
            foreach (RoomTagInfo info in table.list.Values)
            {
                Debug.Log(info.ID + " " + info.name);
            }
        }
    }
}
#endregion

#region RoomEditInfoTableConverter
public class RoomEdiInfoTableConverter : RoomEditInfoTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_list.Count);
        foreach (RoomEditInfo info in m_list.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        RoomEdiInfoTableConverter convertor = new RoomEdiInfoTableConverter();
        convertor.m_list = new Dictionary<int, RoomEditInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            RoomEditInfo messageRespondInfo = ExcelTableTools.GenericObj<RoomEditInfo>(row);
            convertor.m_list.Add(messageRespondInfo.id, messageRespondInfo);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            RoomEditInfoTable table = new RoomEditInfoTable();
            table.Load(buff);
            foreach (RoomEditInfo info in table.m_list.Values)
            {
                Debug.Log(info.id + " " + info.name);
            }
        }
    }
}
#endregion

#region IconInfoTableConverter
public class IconInfoTableConverter : IconInfoTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_list.Count);
        foreach (IconInfomation info in m_list.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        IconInfoTableConverter convertor = new IconInfoTableConverter();
        convertor.m_list = new Dictionary<int, IconInfomation>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            IconInfomation messageRespondInfo = ExcelTableTools.GenericObj<IconInfomation>(row);
            convertor.m_list.Add(messageRespondInfo.ID, messageRespondInfo);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            IconInfoTable table = new IconInfoTable();
            table.Load(buff);
            foreach (IconInfomation info in table.m_list.Values)
            {
                Debug.Log(info.ID +  ","+ info.dirName);
            }
        }
    }
}
#endregion

#region DungeonMonsterConverter
public class DungeonMonsterConverter : DungeonMonsterTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_list.Count);
        foreach (DungeonMonsterInfo info in m_list.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        DungeonMonsterConverter convertor = new DungeonMonsterConverter();
        convertor.m_list = new Dictionary<int, DungeonMonsterInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            DungeonMonsterInfo messageRespondInfo = ExcelTableTools.GenericObj<DungeonMonsterInfo>(row);
            convertor.m_list.Add(messageRespondInfo.ID, messageRespondInfo);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            DungeonMonsterTable table = new DungeonMonsterTable();
            table.Load(buff);
            foreach (DungeonMonsterInfo info in table.m_list.Values)
            {
                Debug.Log(info.ID + ",..........." + info.m_floorId+","+info.m_npcId);
            }
        }
    }
}
#endregion

#region AttrRatioConverter
public class AttrRatioConverter : AttrRatioTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (AttrRatioInfo info in m_map.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        AttrRatioConverter convertor = new AttrRatioConverter();
        convertor.m_map = new Dictionary<int, AttrRatioInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            AttrRatioInfo messageRespondInfo = ExcelTableTools.GenericObj<AttrRatioInfo>(row);
            convertor.m_map.Add(messageRespondInfo.m_id, messageRespondInfo);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            AttrRatioTable table = new AttrRatioTable();
            table.Load(buff);
            foreach (AttrRatioInfo info in table.m_map.Values)
            {
                Debug.Log(info.m_id + ",..........." + info.m_magDefendAdd + "," + info.m_rarity);
            }
        }
    }
}
#endregion

#region ComboSwordSoulTableConverter
public class ComboSwordSoulTableConverter : ComboSwordSoulTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (ComboSwordSoulInfo info in m_map.Values)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        ComboSwordSoulTableConverter convertor = new ComboSwordSoulTableConverter();
        convertor.m_map = new Dictionary<int, ComboSwordSoulInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            ComboSwordSoulInfo info = ExcelTableTools.GenericObj<ComboSwordSoulInfo>(row);
            convertor.m_map.Add(info.m_comboValue, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            ComboSwordSoulTable table = new ComboSwordSoulTable();
            table.Load(buff);
            foreach (ComboSwordSoulInfo info in table.m_map.Values)
            {
                Debug.Log(info.m_comboValue);
            }
        }
    }
}
#endregion

#region EventItemTableConverter
public class EventItemTableConverter : EventItemTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (KeyValuePair<int, EventItemInfo> item in m_map)
        {
            item.Value.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        EventItemTableConverter convertor = new EventItemTableConverter();
        convertor.m_map = new Dictionary<int, EventItemInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            EventItemInfo info = ExcelTableTools.GenericObj<EventItemInfo>(row);
            convertor.m_map.Add(info.itemId, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            EventItemTable table = new EventItemTable();
            table.Load(buff);
            foreach (KeyValuePair<int, EventItemInfo> item in table.m_map)
            {
                Debug.Log(item.Value.itemId + " " + item.Value.modelId );
            }
        }
    }
}
#endregion

#region GradeUpRequireTableConverter
public class GradeUpRequireTableConverter : GradeUpRequireTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(GradeUpInfoList.Count);
        foreach (GradeUpRequireInfo info in GradeUpInfoList)
        {
            info.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        GradeUpRequireTableConverter convertor = new GradeUpRequireTableConverter();
        convertor.GradeUpInfoList = new List<GradeUpRequireInfo>();
        PropertyInfo[] vInfo = typeof(GradeUpRequireInfo).GetProperties();
        foreach (IRow row in sheet)
        {
            int index = 0;
            ICell data = row.GetCell(index);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;
            GradeUpRequireInfo info = new GradeUpRequireInfo();
            ExcelTableTools.SetValue<GradeUpRequireInfo>(vInfo[index], info, data);                          //ID
            ++index; ExcelTableTools.SetValue<GradeUpRequireInfo>(vInfo[index], info, row.GetCell(index));    //升段次数
            ++index; ExcelTableTools.SetValue<GradeUpRequireInfo>(vInfo[index], info, row.GetCell(index));    //每次升段提升等级上限
            ++index; ExcelTableTools.SetValue<GradeUpRequireInfo>(vInfo[index], info, row.GetCell(index));    //每次升段提升生命值上限
            ++index; ExcelTableTools.SetValue<GradeUpRequireInfo>(vInfo[index], info, row.GetCell(index));    //每次升段提升攻击力上限
            ++index; ExcelTableTools.SetValue<GradeUpRequireInfo>(vInfo[index], info, row.GetCell(index));    //每次升段提升魔导力上限
            ++index; ExcelTableTools.SetValue<GradeUpRequireInfo>(vInfo[index], info, row.GetCell(index));    //升段公式一消耗金币数量
            ++index; ExcelTableTools.SetValue<GradeUpRequireInfo>(vInfo[index], info, row.GetCell(index));    //升段公式数量

            for (int i = 0; i < (info.FormulaNum - 1); i++ )
            {
                //FormulaInfo
                PropertyInfo[] vInfo1 = typeof(FormulaInfo).GetProperties();
                FormulaInfo info1 = new FormulaInfo();
                int index1 = 0;
                ++index; ExcelTableTools.SetValue<FormulaInfo>(vInfo1[index1], info1, row.GetCell(index));//公式消耗金币
                index1++;
                ++index; ExcelTableTools.SetValue<FormulaInfo>(vInfo1[index1], info1, row.GetCell(index));//公式参数个数
                index1++;

                for (int j = 1; j <= 5; j++ )
                {
                    //FormulaParam
                    PropertyInfo[] vInfo3 = typeof(FormulaParam).GetProperties();
                    FormulaParam info3 = new FormulaParam();
                    int index3 = 0;
                    ++index; ExcelTableTools.SetValue<FormulaParam>(vInfo3[index3], info3, row.GetCell(index));//参数类型
                    index3++;
                    ++index; ExcelTableTools.SetValue<FormulaParam>(vInfo3[index3], info3, row.GetCell(index));//指定ID
                    index3++;
                    ++index; ExcelTableTools.SetValue<FormulaParam>(vInfo3[index3], info3, row.GetCell(index));//需要的卡牌等级
                    index3++;
                    ++index; ExcelTableTools.SetValue<FormulaParam>(vInfo3[index3], info3, row.GetCell(index));//需要的卡牌星级
                    index3++;
                    ++index; ExcelTableTools.SetValue<FormulaParam>(vInfo3[index3], info3, row.GetCell(index));//需要的卡牌职业
                    index3++;
                    ++index; ExcelTableTools.SetValue<FormulaParam>(vInfo3[index3], info3, row.GetCell(index));//需要的荣誉戒指数量
                    info1.ParamList.Add(info3);
                }
                info.FormulaList.Add(info1);
            }
            convertor.GradeUpInfoList.Add(info);
        }

        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            GradeUpRequireTable table = new GradeUpRequireTable();
            table.Load(buff);
            foreach (var info in table.GradeUpInfoList)
            {
                Debug.Log(info.ID);
            }
        }
    }
}
#endregion


#region QualityRelativeTableConverter
public class QualityRelativeTableConverter : QualityRelativeTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (KeyValuePair<int, QualityInfo> item in m_map)
        {
            item.Value.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        QualityRelativeTableConverter convertor = new QualityRelativeTableConverter();
        convertor.m_map = new Dictionary<int, QualityInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            QualityInfo info = ExcelTableTools.GenericObj<QualityInfo>(row);
            convertor.m_map.Add(info.m_quality, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            QualityRelativeTable table = new QualityRelativeTable();
            table.Load(buff);
            foreach (KeyValuePair<int, QualityInfo> item in table.m_map)
            {
                Debug.Log(item.Value.m_quality + " " + item.Value.m_mainAttrMutiply);
            }
        }
    }
}
#endregion

#region CardTypeVariationTableConverter
public class CardTypeVariationTableConverter : CardTypeVariationTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (KeyValuePair<int, CardTypeInfo> item in m_map)
        {
            item.Value.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        CardTypeVariationTableConverter convertor = new CardTypeVariationTableConverter();
        convertor.m_map = new Dictionary<int, CardTypeInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            CardTypeInfo info = ExcelTableTools.GenericObj<CardTypeInfo>(row);
            convertor.m_map.Add(info.m_id, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            CardTypeVariationTable table = new CardTypeVariationTable();
            table.Load(buff);
            foreach (KeyValuePair<int, CardTypeInfo> item in table.m_map)
            {
                Debug.Log(item.Value.m_id + " " + item.Value.m_hp + "," + item.Value.m_magDef);
            }
        }
    }
}
#endregion

#region CardLevelVariationTableConverter
public class CardLevelVariationTableConverter : CardLevelVariationTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (KeyValuePair<int, CardLevelInfo> item in m_map)
        {
            item.Value.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        CardLevelVariationTableConverter convertor = new CardLevelVariationTableConverter();
        convertor.m_map = new Dictionary<int, CardLevelInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            CardLevelInfo info = ExcelTableTools.GenericObj<CardLevelInfo>(row);
            convertor.m_map.Add(info.m_id, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            CardLevelVariationTable table = new CardLevelVariationTable();
            table.Load(buff);
            foreach (KeyValuePair<int, CardLevelInfo> item in table.m_map)
            {
                Debug.Log(item.Value.m_id + " " + item.Value.m_hpParam + "," + item.Value.m_magAttParam);
            }
        }
    }
}
#endregion

#region YellowPointParamTableConverter
public class YellowPointParamTableConverter : YellowPointParamTable
{
    public byte[] Save()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.Write(m_map.Count);
        foreach (KeyValuePair<int, YellowPointInfo> item in m_map)
        {
            item.Value.Save(helper);
        }
        return helper.GetBytes();
    }

    public static void Convert(ISheet sheet, string targetPath)
    {
        YellowPointParamTableConverter convertor = new YellowPointParamTableConverter();
        convertor.m_map = new Dictionary<int, YellowPointInfo>();
        foreach (IRow row in sheet)
        {
            ICell data = row.GetCell(0);
            if ((data == null) || (data.CellType != CellType.NUMERIC)) continue;
            if (((int)data.NumericCellValue) <= 0) continue;

            YellowPointInfo info = ExcelTableTools.GenericObj<YellowPointInfo>(row);
            convertor.m_map.Add(info.m_id, info);
        }
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
        {
            byte[] buff = convertor.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        using (FileStream targetFile = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] buff = new byte[targetFile.Length];
            targetFile.Read(buff, 0, (int)targetFile.Length);
            YellowPointParamTable table = new YellowPointParamTable();
            table.Load(buff);
            foreach (KeyValuePair<int, YellowPointInfo> item in table.m_map)
            {
                Debug.Log(item.Value.m_id + " " + item.Value.m_hpParam + "," + item.Value.m_magAttParam);
            }
        }
    }
}
#endregion