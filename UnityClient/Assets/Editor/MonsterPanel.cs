using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MonsterPanel : EditorWindow
{
    int m_index = 0;            // 当前列表索引
    static int m_dynamicIDSeed = 10000;
    protected static GUIContent LABEL_SAMPLE_INT = new GUIContent("NPC ID", "");
    public NPCInfoTable NPCInfoTableAsset { get; private set; }
    public ModelInfoTable ModelInfoTableAsset { get; private set; }
    List<string> npcNameList = new List<string>();
    List<int> npcIDList = new List<int>();
    [MenuItem("EditorTools/MonsterPanel")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(MonsterPanel));
        
    }
    void OnGUI()
    {
        this.Repaint();
        if (null == NPCInfoTableAsset)
        {
            TextAsset asset = GameData.LoadConfig<TextAsset>("NPCInfoTable");
            NPCInfoTableAsset = new NPCInfoTable();
            NPCInfoTableAsset.Load(asset.bytes);
            foreach (var item in NPCInfoTableAsset.m_list.Values)
            {
                npcNameList.Add(item.StrName);
                npcIDList.Add(item.ID);
            }
        }
        if (null == ModelInfoTableAsset)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("ModelInfoTable");
            ModelInfoTableAsset = new ModelInfoTable();
            ModelInfoTableAsset.Load(obj.bytes);
        }
        EditorGUILayout.LabelField("NPC列表:", GUILayout.Width(100f));
        m_index = EditorGUILayout.Popup("NPC名称", m_index, npcNameList.ToArray());
        EditorGUILayout.FloatField(LABEL_SAMPLE_INT, npcIDList[m_index]);
        Rect rct = new Rect(155, 60, 40, 30);
        if (GUI.Button(rct, @"Add"))
        {
            int npcStaticID = npcIDList[m_index];
            if (Application.isPlaying)
            {
                Debug.Log("Add NPC:name(" + npcNameList[m_index].ToString() + ") npcID(" + npcStaticID.ToString() + ")");
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
                pos = new Vector3(ActorManager.Singleton.MainActor.MainObj.transform.position.x, 0.0f, ActorManager.Singleton.MainActor.MainObj.transform.position.z);
                npcActor.MainPos = pos;
                npcActor.m_startAttackPos = pos;
                npcActor.UpdateHPBar();
            }
            else
            {
                NPCInfo info = NPCInfoTableAsset.Lookup(npcStaticID);
                ModelInfo modelinfo = ModelInfoTableAsset.Lookup(info.ModelId);
                GameObject tempBody = GameData.LoadActor<GameObject>(modelinfo.ModelFile);
                if (null != tempBody)
                {
                    GameObject body = GameObject.Instantiate(tempBody) as GameObject;
                    body.name = "body";
                }
            }
        }
    }
}
