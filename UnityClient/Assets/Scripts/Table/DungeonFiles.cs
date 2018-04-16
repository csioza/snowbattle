using System;
using System.Collections.Generic;

public struct EventToResult
{
    public int EventCount { get; set; }
    public int[] Event { get; set; }
    public int ResultCount { get; set; }
    public int[] Result { get; set; }
}

public class DungeonFilesInfo
{
    public int DungeonID { get; set; }
    public int GradeID { get; set; }
    public int SceneID { get; set; }
    public string Area { get; set; }
    public int EventType { get; set; }
    public int ERCount { get; set; }
    public EventToResult[] ERSet { get; set; }
}

public class DungeonFiles
{
    public List<DungeonFilesInfo> m_dungeonFilesList { get; set; }

    public List<DungeonFilesInfo> LookUp(int dungeonID, int gradeID, int sceneID)
    {
        List<DungeonFilesInfo> tmpList = new List<DungeonFilesInfo>();
        foreach (var item in m_dungeonFilesList)
        {
            if (item.DungeonID == dungeonID && item.GradeID == gradeID && item.SceneID == sceneID)
            {
                tmpList.Add(item);
            }
        }
        return tmpList;
    }

    public DungeonFilesInfo LookUp(int dungeonID, int gradeID, int sceneID, string area)
    {
        foreach (var item in m_dungeonFilesList)
        {
            if (item.DungeonID == dungeonID && item.GradeID == gradeID && item.SceneID == sceneID && item.Area == area)
            {
                return item;
            }
        }
        return null;
    }

    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int count = helper.ReadInt();
        m_dungeonFilesList = new List<DungeonFilesInfo>(count);
        for (int index = 0; index < count; ++index)
        {
            DungeonFilesInfo info = new DungeonFilesInfo();
            info.DungeonID = helper.ReadInt();
            info.GradeID = helper.ReadInt();
            info.SceneID = helper.ReadInt();
            info.Area = helper.ReadString();
            info.EventType = helper.ReadInt();
            info.ERCount = helper.ReadInt();
            info.ERSet = new EventToResult[info.ERCount];
            for (int i = 0; i < info.ERCount; i++)
            {
                info.ERSet[i].EventCount = helper.ReadInt();
                info.ERSet[i].Event = new int[info.ERSet[i].EventCount];
                for (int j = 0; j < info.ERSet[i].EventCount; j++)
                {
                    info.ERSet[i].Event[j] = helper.ReadInt();
                }
                info.ERSet[i].ResultCount = helper.ReadInt();
                info.ERSet[i].Result = new int[info.ERSet[i].ResultCount];
                for (int j = 0; j < info.ERSet[i].ResultCount; j++)
                {
                    info.ERSet[i].Result[j] = helper.ReadInt();
                }
            }
            m_dungeonFilesList.Add(info);
        }
    }
}
