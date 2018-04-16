using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum ENScenePointType
{
	none,
	normalNPC,
	blockNPC,
	triggerArea,
	spawnPoint,
}


public enum ENTriggerType
{
	enOnceTrigger,
	enRepeatTrigger,
}

[System.Serializable]
public class FuncNPCInfo
{
	public int m_IDInMap;
	public int m_IDInTable;
	[HideInInspector]
	public Vector3 m_pos;

	public void Save(BinaryHelper helper)
	{
		helper.Write(m_IDInMap);
		helper.Write(m_IDInTable);
		helper.Write(m_pos.x);
		helper.Write(m_pos.y);
		helper.Write(m_pos.z);
	}
	public void Read(BinaryHelper helper)
	{
		m_IDInMap = helper.ReadInt();
		m_IDInTable = helper.ReadInt();
		m_pos.x = helper.ReadFloat();
		m_pos.y = helper.ReadFloat();
		m_pos.z = helper.ReadFloat();
	}
}

[System.Serializable]
public class TriggerAreaInfo
{
	public int m_triggerID;

	[HideInInspector]public Vector3 m_pos;

	public ENTriggerType m_triggerType = ENTriggerType.enOnceTrigger;

	public void Save(BinaryHelper helper)
	{
		helper.Write(m_triggerID);
		helper.Write((int)m_triggerType);
		helper.Write(m_pos.x);
		helper.Write(m_pos.y);
		helper.Write(m_pos.z);

	}
	public void Read(BinaryHelper helper)
	{
		m_triggerID = helper.ReadInt();
		m_triggerType = (ENTriggerType)helper.ReadInt();
		m_pos.x = helper.ReadFloat();
		m_pos.y = helper.ReadFloat();
		m_pos.z = helper.ReadFloat();

	}
}

[System.Serializable]
public class SpawnPointInfo
{
	public int m_pointId;
	[HideInInspector]public Vector3 m_pos;
	[HideInInspector]public Vector3 m_areaScale;
	public List<int> m_spawnPointsId = new List<int>();

	public void Save(BinaryHelper helper)
	{
		helper.Write(m_pointId);
		helper.Write(m_pos.x);
		helper.Write(m_pos.y);
		helper.Write(m_pos.z);

		helper.Write(m_areaScale.x);
		helper.Write(m_areaScale.y);
		helper.Write(m_areaScale.z);

		helper.Write(m_spawnPointsId.Count);
		for (int index = 0; index < m_spawnPointsId.Count; index++)
		{
			helper.Write(m_spawnPointsId[index]);
		}
	}
	public void Read(BinaryHelper helper)
	{
		m_pointId = helper.ReadInt();
		m_pos.x = helper.ReadFloat();
		m_pos.y = helper.ReadFloat();
		m_pos.z = helper.ReadFloat();

		m_areaScale.x = helper.ReadFloat();
		m_areaScale.y = helper.ReadFloat();
		m_areaScale.z = helper.ReadFloat();

		int pointCount = helper.ReadInt();
		for (int index = 0; index < pointCount; index++)
		{
			m_spawnPointsId.Add(helper.ReadInt());
		}
	}
}


public class SceneFuncPoint : MonoBehaviour
{
	public ENScenePointType m_pointType;
	public FuncNPCInfo m_npcInfo;
	public TriggerAreaInfo m_triggerAreaInfo;
	public SpawnPointInfo m_spawnPointInfo;
}

public class SceneArea
{
	public int m_virsion = 1; //1,添加了传送门
	public string m_areaName;
	public List<FuncNPCInfo> m_normalNpcList = new List<FuncNPCInfo>();			//普通
	public List<FuncNPCInfo> m_blockNpcList = new List<FuncNPCInfo>();			//阻挡NPC
	public List<TriggerAreaInfo> m_triggerAreaList = new List<TriggerAreaInfo>();	//触发区域
	public List<SpawnPointInfo> m_spawnPointList = new List<SpawnPointInfo>();		//刷怪点

	public void Save(BinaryHelper helper)
	{
		helper.Write(m_virsion);
		helper.Write(m_areaName);
		helper.Write(m_normalNpcList.Count);
		for (int index = 0; index < m_normalNpcList.Count; index++)
		{
			m_normalNpcList[index].Save(helper);
		}

		helper.Write(m_blockNpcList.Count);
		for (int index = 0; index < m_blockNpcList.Count; index++)
		{
			m_blockNpcList[index].Save(helper);
		}

		helper.Write(m_triggerAreaList.Count);
		for (int index = 0; index < m_triggerAreaList.Count; index++)
		{
			m_triggerAreaList[index].Save(helper);
		}

		helper.Write(m_spawnPointList.Count);
		for (int index = 0; index < m_spawnPointList.Count; index++)
		{
			m_spawnPointList[index].Save(helper);
		}
	}

	public void Load(BinaryHelper helper)
	{
		m_virsion = helper.ReadInt();
		m_areaName = helper.ReadString();
		int normalNpcCount = helper.ReadInt();
		for (int index = 0; index < normalNpcCount; index++)
		{
			FuncNPCInfo normalNpc = new FuncNPCInfo();
			normalNpc.Read(helper);
		}

		int blockNpcCount = helper.ReadInt();
		for (int index = 0; index < blockNpcCount; index++)
		{
			FuncNPCInfo blockNpc = new FuncNPCInfo();
			blockNpc.Read(helper);
		}

		int triggerAreaCount = helper.ReadInt();
		for (int index = 0; index < triggerAreaCount; index++)
		{
			TriggerAreaInfo triggerAreaInfo = new TriggerAreaInfo();
			triggerAreaInfo.Read(helper);
		}
		int spawnPointCount = helper.ReadInt();
		for (int index = 0; index < spawnPointCount; index++)
		{
			SpawnPointInfo spawnPointInfo = new SpawnPointInfo();
			spawnPointInfo.Read(helper);
		}
	}
}

public class SceneAreas
{
	public List<SceneArea> m_areaList = new List<SceneArea>();
	public void Save(BinaryHelper helper)
	{
		helper.Write(m_areaList.Count);
		for (int index = 0; index < m_areaList.Count; index++)
		{
			m_areaList[index].Save(helper);
		}
	}
	public void Read(byte [] bytes)
	{
		BinaryHelper helper = new BinaryHelper(bytes);
		int areaCount = helper.ReadInt();
		for (int index = 0; index < areaCount; index++)
		{
			SceneArea area = new SceneArea();
			area.Load(helper);
		}
	}
}