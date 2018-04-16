//////////////////////////////////
//仅用于heiz的测试关卡
//测试触发器效果
/////////////////////////////////


using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;


public class TestTriggerManager
{
	protected static GUIContent LABEL_SAMPLE_INT = new GUIContent("NPC ID", "");
	public NPCInfoTable NPCInfoTableAsset { get; private set; }
	public ModelInfoTable ModelInfoTableAsset { get; private set; }
	//public TriggerTable TriggerTableAsset { get; private set; }
	List<string> npcNameList = new List<string>();
	List<int> npcIDList = new List<int>();
	//List<int> triggerList = new List<int>();
	int m_dynamicIDSeed = 20000;
	static public TestTriggerManager Singleton { get; private set; }
	public  TestTriggerManager()
	{
		if (null == Singleton)
		{
			Singleton = this;
		}
		Init();
	}
	void Init()
	{
		TextAsset asset = GameData.LoadConfig<TextAsset>("NPCInfoTable");
		NPCInfoTableAsset = new NPCInfoTable();
		NPCInfoTableAsset.Load(asset.bytes);

		TextAsset obj = GameData.LoadConfig<TextAsset>("ModelInfoTable");
		ModelInfoTableAsset = new ModelInfoTable();
		ModelInfoTableAsset.Load(obj.bytes);
		foreach (var item in NPCInfoTableAsset.m_list.Values)
		{
			npcNameList.Add(item.StrName);
			npcIDList.Add(item.ID);
		}
	/*	TextAsset trigger = GameData.LoadConfig<TextAsset>("TriggerTable");
		TriggerTableAsset = new TriggerTable();
		TriggerTableAsset.Load(trigger.bytes);
		foreach (var item in TriggerTableAsset.m_map.Values)
		{
			triggerList.Add(item.m_HPreduce);
		}
		Debug.Log("Add NPC:name(" + npcNameList[1].ToString() + ") ");

		Debug.Log("triggerTable: "+triggerList[1].ToString());*/

	}
	public void testAddNpc(int NPCID,Vector3 position)
	{


		int npcStaticID = npcIDList[NPCID];
		Actor npcActor = ActorManager.Singleton.CreatePureActor(ActorType.enNPC, m_dynamicIDSeed++,CSItemGuid.Zero, npcStaticID);
		npcActor.IDInTable = npcStaticID;
		npcActor.Props.SetProperty_Int32(ENProperty.islive, 1); ;
		NPC npc = npcActor as NPC;
		NPCInfo info = NPCInfoTableAsset.Lookup(npcStaticID);
		ModelInfo modelinfo = ModelInfoTableAsset.Lookup(info.ModelId);
		if (info.Type == (int)ENNpcType.enBoxNPC)
		{
			GameObject tempBody = GameData.LoadActor<GameObject>(modelinfo.ModelFile);
			GameObject body = GameObject.Instantiate(tempBody) as GameObject;
			body.name = "body";
			body.transform.position = Vector3.zero;
			npc.SetBodyObject(body);
		}
		npcActor.CreateNeedModels();
		Vector3 pos = Vector3.zero;
		pos = position;
		npcActor.MainPos = pos;
		npcActor.ForceMoveToPosition(pos);
		npcActor.m_startAttackPos = pos;
		npcActor.UpdateHPBar();
	}
	public void createActorAtPosition(int number,int team,int NPCID,Vector3 position)
	{

		for (int actors =0; actors < number; actors++) 
		{
			/*Actor createdActor = ActorManager.Singleton.CreatePureActor(ActorType.enNPC, 20000+actors++,CSItemGuid.Zero, NPCID);
			createdActor.Props.SetProperty_Int32(ENProperty.NPC_IDInTable, NPCID);
			createdActor.Props.SetProperty_Int32(ENProperty.islive, 1);
			createdActor.CreateNeedModels();*/
			testAddNpc(NPCID-1,position);
		}
	}
 
	public  void testReduceHp()
	{
		//ActorManager.Singleton.MainActor.ReduceHp (triggerList[1]);
	}

}