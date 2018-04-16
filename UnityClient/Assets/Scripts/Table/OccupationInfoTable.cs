using System;
using System.Collections.Generic;

public class OccupationInfo : IDataBase
{
    public int m_id { get; protected set; }
    public string m_name { get; protected set; }
    public string m_describe { get; protected set; }
    public int m_iconId { get; protected set; }
    public List<int> m_swordSoulList { get; protected set; }//剑魂技列表
}

public class OccupationInfoTable
{
    public Dictionary<int, OccupationInfo> m_map { get; protected set; }
    public OccupationInfo LookUp(int id)
    {
        OccupationInfo info = null;
        if (m_map.TryGetValue(id, out info))
        {
            return info;
        }

        return null;
    }
    public void Load(byte[] bytes)
    {
        m_map = new Dictionary<int, OccupationInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount = helper.ReadInt();
        for (int index = 0; index < sceneCount; ++index)
        {
            OccupationInfo info = new OccupationInfo();
            
            info.Load(helper);

            m_map.Add(info.m_id, info);
        }
    }
}