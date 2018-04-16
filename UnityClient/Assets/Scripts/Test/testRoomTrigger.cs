//////////////////////////////////
//仅用于heiz的测试关卡
//测试触发器效果
/////////////////////////////////

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[System.Serializable]
public class triggerData
{	public enum ENEventType
	{
		npcSpawn,
		npcDead,
	}
	[SerializeField] public GameObject m_npc = null;
	public ENEventType m_eventType =ENEventType.npcSpawn;
	public int m_SpawnNumber =1;
	public int m_team =1;
	public int m_NPCID =1;
	public GameObject m_SpawnPosition = null;
	public bool m_isSpawnAtLocal =false;

	[HideInInspector]public Vector3 m_position = Vector3.zero;
	[HideInInspector]public NPC m_targetNPC =null;
	[HideInInspector]public bool haveGetNpc =false;
	[HideInInspector]public bool NpcDead =false;
}


public class testRoomTrigger : MonoBehaviour {

	TestTriggerManager m_trigger{get{return TestTriggerManager.Singleton;}}
	public List<triggerData> EventData = new List<triggerData>();


	void Start () 
	{
		for(int numberofEvent=0;numberofEvent<EventData.Count;numberofEvent++)
		{
			//EventData[numberofEvent].m_position = EventData[numberofEvent].m_SpawnPosition.transform.position;
			EventData[numberofEvent].m_position = new Vector3(EventData[numberofEvent].m_SpawnPosition.transform.position.x, 0.0f, EventData[numberofEvent].m_SpawnPosition.transform.position.z);
		} 
	}
	void Update () 
	{
		for(int numberofEvent=0;numberofEvent<EventData.Count;numberofEvent++)
		{
		if (EventData[numberofEvent].haveGetNpc) 
		{
			if (EventData[numberofEvent].m_eventType == triggerData.ENEventType.npcSpawn)
			{
				m_trigger.createActorAtPosition (EventData[numberofEvent].m_SpawnNumber, EventData[numberofEvent].m_team, EventData[numberofEvent].m_NPCID, EventData[numberofEvent].m_position);
			}
			EventData[numberofEvent].haveGetNpc = false;
			int m_targetNPCID = EventData[numberofEvent].m_targetNPC.ID;
				Debug.Log ("npc name is" + m_targetNPCID+" index is"+numberofEvent);
		}
		if (EventData[numberofEvent].m_eventType ==triggerData.ENEventType.npcDead&&null != EventData[numberofEvent].m_targetNPC)
		{	
			if (EventData[numberofEvent].m_targetNPC.IsDead)
			{	
				if(!EventData[numberofEvent].NpcDead)
				{
					if(EventData[numberofEvent].m_isSpawnAtLocal)
					{
						EventData[numberofEvent].m_position = EventData[numberofEvent].m_targetNPC.MainPos;
					}
					m_trigger.createActorAtPosition(EventData[numberofEvent].m_SpawnNumber,EventData[numberofEvent].m_team,EventData[numberofEvent].m_NPCID,EventData[numberofEvent].m_position);
					EventData[numberofEvent].NpcDead =true;
				}
			}
		}
		}
	}
	public void getNPC(NPC npc,int index)
	{
		EventData[index].m_targetNPC = npc;
		EventData[index].haveGetNpc = true;
	}
}
