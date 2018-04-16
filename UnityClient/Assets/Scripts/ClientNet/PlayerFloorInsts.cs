using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NGE.Network;

public class FloorInst
{
	public int floorID;
	public FloorInstData instData = new FloorInstData();
};

public class FloorInstData
{
	public void Read(BinaryHelper helper)
	{
		m_passTime	= helper.ReadInt();
		m_combo		= helper.ReadInt();
		m_killBoss	= helper.ReadInt();
		m_reSpawn	= helper.ReadInt();
		m_score		= helper.ReadInt();
		m_rankID	= helper.ReadInt();
	}
	public void Read(PacketReader reader)
	{
		m_passTime = reader.ReadInt32();
		m_combo = reader.ReadInt32();
		m_killBoss = reader.ReadInt32();
		m_reSpawn = reader.ReadInt32();
		m_score = reader.ReadInt32();
		m_rankID = reader.ReadInt32();
	}
	public int m_passTime;         //通过时间
	public int m_combo;            //最大combo数
	public int m_killBoss;         //击杀boss数
	public int m_reSpawn;          //复活数
	public int m_score;            //分数
	public int m_rankID;           //等级
}

public class PlayerFloorInsts 
{
	Dictionary<int, FloorInst> m_floorInsts = new Dictionary<int,FloorInst>();


	public void Deserialize(PacketReader stream)
	{
		int mask = stream.ReadInt32();
		if ((mask & (int)ENSerializeMask.enSerializeDirty) > 0)
		{
			Debug.Log("Sync FloorInsts error, mask & enSerializeDirty > 0 ");
			return;
		}
		byte version = stream.ReadByte();
		int count = stream.ReadInt32();
		m_floorInsts.Clear();
		for(int index = 0 ; index < count; index++)
		{
			FloorInst inst = new FloorInst();
			inst.floorID = stream.ReadInt32();
			byte[] tmp = stream.ReadBuffer(64);
			BinaryHelper instDataHelper = new BinaryHelper(tmp);
			inst.instData.Read(instDataHelper);
			m_floorInsts.Add(inst.floorID, inst);
		}
	}

	public FloorInst GetFloorInst(int floorID)
	{
		FloorInst inst = null;
		if (m_floorInsts.TryGetValue(floorID,out inst))
		{
			return inst;
		}
		return null;
	}

	void AddFloorInst(int floorID,FloorInstData data)
	{
		FloorInst inst = new FloorInst();
		inst.floorID = floorID;
		inst.instData = data;
		m_floorInsts.Add(inst.floorID, inst);
	}

	public void ReplaceFloorInstData(int floorID,FloorInstData data)
	{
		FloorInst inst = GetFloorInst(floorID);
		if(inst != null)
		{
			inst.instData = data;
		}
		else 
		{
			AddFloorInst(floorID, data);
		}
	}
}

