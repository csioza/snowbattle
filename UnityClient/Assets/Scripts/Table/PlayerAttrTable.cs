using System;
using System.Collections.Generic;

public class PlayerAttrInfo : IDataBase
{
    public int m_level { get; protected set; }
    public int m_needExp { get; protected set; }
    public int m_leaderShip { get; protected set; }
    public int m_stamina { get; protected set; } // 体力
    public int m_energy { get; protected set; }	//军资
    public int m_teamNum { get; protected set; } // 最大队伍数量
    public int m_unlockTeamLevel { get; protected set; } // 解锁队伍最大数量的 等级
}

public class PlayerAttrTable
{
    public Dictionary<int, PlayerAttrInfo> m_map { get; protected set; }
    public PlayerAttrInfo LookUp(int level)
    {
        PlayerAttrInfo info = null;
        if (m_map.TryGetValue(level, out info))
        {
            return info;
        }
        return null;
    }
    public void Load(byte[] bytes)
    {
        m_map = new Dictionary<int, PlayerAttrInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount      = helper.ReadInt();

        for (int index = 0; index < sceneCount; ++index)
        {
            PlayerAttrInfo info     = new PlayerAttrInfo();
            info.Load(helper);
            m_map.Add(info.m_level, info);
        }
    }
}