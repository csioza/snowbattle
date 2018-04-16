using System;
using System.Collections.Generic;


public class DungeonMonsterInfo : IDataBase
{
    public int ID { get; protected set; }
    public int m_floorId { get; protected set; }
    public int m_monsterId { get; protected set; }
    public int m_npcId { get; protected set; }
}

public class DungeonMonsterItemInfo
{
    public int m_monsterId { get;  set; }
    public int m_npcId { get;  set; }
}
public class DungeonMonsterTable
{

    public Dictionary<int, DungeonMonsterInfo> m_list { get; protected set; }

    public Dictionary<int, List<DungeonMonsterItemInfo>> m_dataList { get; protected set; }
    public int LookUp(int floorId,int monsterId )
    {
        int npcId = 0;
        List<DungeonMonsterItemInfo> list = null;
        if (m_dataList.TryGetValue(floorId, out list))
        {
            foreach (DungeonMonsterItemInfo item in list )
           {
               if (item.m_monsterId == monsterId)
                {
                    return item.m_npcId;
                }
           }
        }

        return npcId;
    }

    public void Load(byte[] bytes)
    {
        m_list = new Dictionary<int, DungeonMonsterInfo>();
        m_dataList = new Dictionary<int, List<DungeonMonsterItemInfo>>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount = helper.ReadInt();

        for (int index = 0; index < sceneCount; ++index)
        {
            DungeonMonsterInfo info = new DungeonMonsterInfo();

            info.Load(helper);

            m_list.Add(info.ID, info);
            DungeonMonsterItemInfo itemInfo = new DungeonMonsterItemInfo();
            itemInfo.m_monsterId            = info.m_monsterId;
            itemInfo.m_npcId                = info.m_npcId;
            if (m_dataList.ContainsKey(info.m_floorId))
            {
                m_dataList[info.m_floorId].Add(itemInfo);
            }
            else
            {
                 List<DungeonMonsterItemInfo> list = new List<DungeonMonsterItemInfo>();
                 list.Add(itemInfo);
                 m_dataList.Add(info.m_floorId, list);
            }
        }
    }
}