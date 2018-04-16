using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class UICreateSceneAreas : EditorWindow
{
	GameObject tarObj;

	[MenuItem("EditorTools/保存场景区域NPC")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		EditorWindow.GetWindow(typeof(UICreateSceneAreas));
	}
	void OnGUI()
	{
		EditorGUILayout.LabelField("保存的物体:", GUILayout.Width(100f));
		GameObject sel = EditorGUILayout.ObjectField(tarObj, typeof(GameObject), true, GUILayout.Width(140f)) as GameObject;
		if (sel != null)
		{
			tarObj = sel;
		}
		EditorGUILayout.LabelField("注意:物体的名字将作为二进制文件的名字", GUILayout.Width(250f));
		if (GUILayout.Button("确认", GUILayout.Width(76f)))
		{
			EditorSaveSceneAreas.SaveSceneAreas(sel);
		}
	}
}


public class EditorSaveSceneAreas
{
	public static void SaveSceneAreas(GameObject monsterNode)
	{
		if (monsterNode == null)
		{
			return;
		}
		if (monsterNode.name == "")
		{
			Debug.Log("必须给物体起名,该名字将作为保存的二进制文件的名字");
		}
		int areaCount = monsterNode.transform.childCount;
		SceneAreas sceneAreas = new SceneAreas();
		for (int areaIndex = 0; areaIndex < areaCount; areaIndex++)
		{
			Transform tarAreaTrans = monsterNode.transform.GetChild(areaIndex);
			//一个区域
			SceneArea newArea = new SceneArea();
			newArea.m_areaName = tarAreaTrans.name;
			int funcPointCount = tarAreaTrans.childCount;
			for (int funcPointIndex = 0; funcPointIndex < funcPointCount; funcPointIndex++)
			{
				Transform tarPoint = tarAreaTrans.GetChild(funcPointIndex);
				SceneFuncPoint funcPoint = tarPoint.GetComponent<SceneFuncPoint>();
				if (funcPoint == null || funcPoint.m_pointType == ENScenePointType.none)
				{
					Debug.Log(tarPoint.name + " has no component 'SceneFuncPoint' or it is 'none' ");
					return;
				}
				switch (funcPoint.m_pointType)
				{
					case ENScenePointType.normalNPC:
						FuncNPCInfo normalNpc =funcPoint.m_npcInfo;
						normalNpc.m_pos = tarPoint.localPosition;
						newArea.m_normalNpcList.Add(normalNpc);
						break;
					case ENScenePointType.blockNPC:
						FuncNPCInfo blockNpc = funcPoint.m_npcInfo;
						blockNpc.m_pos = tarPoint.localPosition;
						newArea.m_blockNpcList.Add(blockNpc);
						break;
					case ENScenePointType.triggerArea:
						TriggerAreaInfo triggerAreaInfo = funcPoint.m_triggerAreaInfo;
						triggerAreaInfo.m_pos = tarPoint.localPosition;
						newArea.m_triggerAreaList.Add(triggerAreaInfo);
						break;
					case ENScenePointType.spawnPoint:
						SpawnPointInfo spawnPointInfo = funcPoint.m_spawnPointInfo;
						spawnPointInfo.m_pos = tarPoint.localPosition;
						spawnPointInfo.m_areaScale = tarPoint.localScale;
						for (int index = 0; index < funcPoint.m_spawnPointInfo.m_spawnPointsId.Count; index++)
						{
							spawnPointInfo.m_spawnPointsId.Add(funcPoint.m_spawnPointInfo.m_spawnPointsId[index]);
						}
						newArea.m_spawnPointList.Add(spawnPointInfo);
						break;
				}				
			}
			sceneAreas.m_areaList.Add(newArea);			
		}
		using (FileStream targetFile = new FileStream("Assets/Resources/Prefabs/ScenePrefabs/" + monsterNode.name + ".bytes", FileMode.Create))
		{
			BinaryHelper helper = new BinaryHelper();
			sceneAreas.Save(helper);
			byte[] saveBytes = helper.GetBytes();
			targetFile.Write(saveBytes, 0, saveBytes.Length);
			Debug.Log(monsterNode.name + ".bytes Success Saved");
		}
	}
}
