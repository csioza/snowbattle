using System;
using System.Collections.Generic;

public class LevelUpInfo : IDataBase
{
    public int lvl { get; protected set; }
    public int LvUp_need_exp { get; protected set; }
    public int Monster_Exp { get; protected set; }
    public float EachLvAttrIncreasePercent { get; protected set; }

}

public class LevelUpTable
{
    public Dictionary<int, LevelUpInfo> m_levelUpMap { get; protected set; }
    public LevelUpInfo LookUp(int level)
    {
        LevelUpInfo info = null;
        if (m_levelUpMap.TryGetValue(level, out info))
        {
            return info;
        }
        return null;
    }
    public void Load(byte[] bytes)
    {
        m_levelUpMap = new Dictionary<int, LevelUpInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount = helper.ReadInt();
        for (int index = 0; index < sceneCount; ++index)
        {
            LevelUpInfo info = new LevelUpInfo();
            info.Load(helper);
            m_levelUpMap.Add(info.lvl, info);
        }
    }
}