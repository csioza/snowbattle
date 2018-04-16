/********************************************************************
	created:	2013/05/13 16:25
	filename: 	SceneInfoTable.cs
	author:		LiChaobin
	
	purpose:	场景表 
*********************************************************************/
using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class SceneInfo
{
	public int SceneId { get; set; }
	public string SceneName;
	public string SceneFile { get; set; }
	public int SceneType { get; set; }
	public int LevelMinLimit { get; set; }
    public int LevelMaxLimit { get; set; }
    public List<float> BornPosList { get; set; }
	public string ByteFile { get; set; }
	public string BeginArea { get; set; }
    public int Physical { get; set; }
}

public class SceneInfoTable
{
	public Dictionary<int, SceneInfo> m_sceneMap { get; protected set; }
	public SceneInfo LookUpSceneById(int sceneId)
	{
		SceneInfo info = null;
		if (m_sceneMap.TryGetValue(sceneId,out info))
		{
			return info;
		}
		return null;		
	}
	public void Load(byte[] bytes)
	{
		m_sceneMap = new Dictionary<int, SceneInfo>();
		BinaryHelper helper = new BinaryHelper(bytes);

		int sceneCount = helper.ReadInt();
		for (int index = 0; index < sceneCount; ++index)
		{
			SceneInfo sceneInfo = new SceneInfo();
			sceneInfo.SceneId = helper.ReadInt();
			sceneInfo.SceneName = helper.ReadString();
			sceneInfo.SceneFile = helper.ReadString();
			sceneInfo.SceneType = helper.ReadInt();
			sceneInfo.LevelMinLimit = helper.ReadInt();
            sceneInfo.LevelMaxLimit = helper.ReadInt();
            int count = helper.ReadInt();
            sceneInfo.BornPosList = new List<float>();
            for (int i = 0; i < count; i++)
            {
                sceneInfo.BornPosList.Add(helper.ReadFloat());
            }
			sceneInfo.ByteFile = helper.ReadString();
			sceneInfo.BeginArea = helper.ReadString();
            sceneInfo.Physical = helper.ReadInt();
			m_sceneMap.Add(sceneInfo.SceneId, sceneInfo);
		}
	}

}

