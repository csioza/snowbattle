using System;
using System.Collections.Generic;

public class FloorInfo : IDataBase
{
    public int m_id { get; protected set; }
    public string m_name { get; protected set; }
    public int m_iconId { get; protected set; }
    public int m_fatherStageId { get; protected set; }
    public int m_requireLevel { get; protected set; }
    public int m_frontFloorId { get; protected set; }
    public int m_backFloorId { get; protected set; }
    public int m_cost { get; protected set; }
    public string m_difficulities { get; protected set; }
    public int m_expBase { get; protected set; }
    public int m_coinBase { get; protected set; }
    public int m_goldOutput { get; protected set; }
    public int m_skillExpBase { get; protected set; }
    public int m_scoreId { get; protected set; }
    public int m_treasureDropList { get; protected set; }
    public string m_conditions { get; protected set; }
    public int m_layerNum { get; protected set; }
    public int m_randomMapID { get; protected set; }
    public int m_canOrigialResurrection { get; protected set; }
    public int m_origialCount { get; protected set; }
	public int m_longConnect { get; protected set; }
}

public class FloorInfoTable
{
    public Dictionary<int, FloorInfo> m_map;
    public FloorInfo LookUp(int id)
    {
        FloorInfo info = null;
        if (m_map.TryGetValue(id, out info))
        {
            return info;
        }

        return null;
    }

    public void Load(byte[] bytes)
    {
        m_map               = new Dictionary<int, FloorInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount      = helper.ReadInt();

        for (int index = 0; index < sceneCount; ++index)
        {
            FloorInfo info  = new FloorInfo();
            
            info.Load(helper);

            m_map.Add(info.m_id, info);
        }
    }
}