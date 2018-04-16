using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

public class DungeonInfo
{
	public int DungeonId { get; set; }
    public int DungeonType { get; set; }
	public int DungeonGrade { get; set; }
	public string DungeonName { get; set; }
	public List<int> SceneIdList { get; set; }
	public int LevelMin { get; set; }
	public int LevelMax { get; set; }
	public int QuestId { get; set; }
	public int NeedEnergy { get; set; }
	public int TeamVolume { get; set; }
}

public struct DungeonInfoKey:IComparable<DungeonInfoKey>
{
	public int DungeonID { get; set; }
	public int DungeonGrade { get; set; }
	public int CompareTo(DungeonInfoKey other)
	{
		if (DungeonID < other.DungeonID ||(DungeonID == other.DungeonID && DungeonGrade < other.DungeonGrade))
		{
			return -1;
		}
		else if (DungeonID == other.DungeonID && DungeonGrade == other.DungeonGrade)
		{
			return 0;
		}
		else 
		{ 
			return 1;
		}
	}
}

public class DungeonInfoTable
{
	public SortedDictionary<DungeonInfoKey,DungeonInfo> DungeonInfoMap { get; protected set; }

	public DungeonInfo Lookup(int dungeonId,int grade)
	{
		DungeonInfoKey key = new DungeonInfoKey();
		key.DungeonID = dungeonId;
		key.DungeonGrade = grade;
		DungeonInfo info = null;
		if (DungeonInfoMap.TryGetValue(key, out info))
		{
			return info;
		}
		return null;
	}

	public void Load(byte[] bytes)
	{
		DungeonInfoMap = new SortedDictionary<DungeonInfoKey, DungeonInfo>();
		BinaryHelper helper = new BinaryHelper(bytes);

		int dungeonCount = helper.ReadInt();
		for (int index = 0; index < dungeonCount; ++index)
		{
			DungeonInfo dungeonInfo = new DungeonInfo();
			dungeonInfo.DungeonId = helper.ReadInt();
			dungeonInfo.DungeonGrade = helper.ReadInt();
			dungeonInfo.DungeonName = helper.ReadString();
			int sceneCount = helper.ReadInt();
            dungeonInfo.SceneIdList = new List<int>(sceneCount);
			for (int sceneIndex = 0; sceneIndex < sceneCount ; sceneIndex++)
			{
				dungeonInfo.SceneIdList.Add(helper.ReadInt());
			}
			dungeonInfo.LevelMin = helper.ReadInt();
			dungeonInfo.LevelMax = helper.ReadInt();
			dungeonInfo.QuestId = helper.ReadInt();
			dungeonInfo.NeedEnergy = helper.ReadInt();
			dungeonInfo.TeamVolume = helper.ReadInt();

			DungeonInfoKey newKey = new DungeonInfoKey();
			newKey.DungeonID = dungeonInfo.DungeonId;
			newKey.DungeonGrade = dungeonInfo.DungeonGrade;
			DungeonInfoMap.Add(newKey, dungeonInfo);
		}
	}
}