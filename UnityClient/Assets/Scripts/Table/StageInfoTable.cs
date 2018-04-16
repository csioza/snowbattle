using System;
using System.Collections.Generic;

public class StageDetailInfo : IDataBase
{
    public int m_id { get; protected set; }
    public string m_name { get; protected set; }
    public int m_iconId { get; protected set; }
    public int m_fatherZoneId { get; protected set; }
    public List<int> m_floorList { get; protected set; }
    public int m_requireLevel { get; protected set; }
    public int m_backStageId { get; protected set; }
    public string m_tips { get; protected set; }
}

public class StageInfoTable
{
    public Dictionary<int, StageDetailInfo> m_map { get; protected set; }
    public StageDetailInfo LookUp(int id)
    {
        StageDetailInfo info = null;
        if (m_map.TryGetValue(id, out info))
        {
            return info;
        }

        return null;
    }

    public void Load(byte[] bytes)
    {
        m_map               = new Dictionary<int, StageDetailInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount      = helper.ReadInt();

        for (int index = 0; index < sceneCount; ++index)
        {
            StageDetailInfo info = new StageDetailInfo();
            
            info.Load(helper);

            m_map.Add(info.m_id, info);
        }
    }
}