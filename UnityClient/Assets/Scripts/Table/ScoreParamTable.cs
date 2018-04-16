using System;
using System.Collections.Generic;

public class ScoreParamInfo : IDataBase
{
    public int m_id { get; protected set; }
    public float m_standardFloorTimePoint { get; protected set; }
    public float m_standardFloorTime { get; protected set; }
    public float m_fTimeParam { get; protected set; }
    public float m_eTimeParam { get; protected set; }
    public float m_dTimeParam { get; protected set; }
    public float m_cTimeParam { get; protected set; }
    public float m_bTimeParam { get; protected set; }
    public float m_aTimeParam { get; protected set; }
    public float m_sTimeParam { get; protected set; }
    public float m_ssTimeParam { get; protected set; }
    public float m_sssTimeParam { get; protected set; }

    public int m_standardComboPoint { get; protected set; }
    public int m_standardCombo { get; protected set; }

    public float m_fCountParam { get; protected set; }
    public float m_eCountParam { get; protected set; }
    public float m_dCountParam { get; protected set; }
    public float m_cCountParam { get; protected set; }
    public float m_bCountParam { get; protected set; }
    public float m_aCountParam { get; protected set; }
    public float m_sCountParam { get; protected set; }
    public float m_ssCountParam { get; protected set; }
    public float m_sssCountParam { get; protected set; }

    public int m_bossPoint { get; protected set; }

    public int m_standardBossKillCount { get; protected set; }

    public int m_fBossCount { get; protected set; }
    public int m_eBossCount { get; protected set; }
    public int m_dBossCount { get; protected set; }
    public int m_cBossCount { get; protected set; }
    public int m_bBossCount { get; protected set; }
    public int m_aBossCount { get; protected set; }
    public int m_sBossCount { get; protected set; }
    public int m_ssBossCount { get; protected set; }
    public int m_sssBossCount { get; protected set; }

    public int m_nonRespawnPoint { get; protected set; }
    public int m_standardRespawn { get; protected set; }

    public int m_fReliveCount { get; protected set; }
    public int m_eReliveCount { get; protected set; }
    public int m_dReliveCount { get; protected set; }
    public int m_cReliveCount { get; protected set; }
    public int m_bReliveCount { get; protected set; }
    public int m_aReliveCount { get; protected set; }
    public int m_sReliveCount { get; protected set; }
    public int m_ssReliveCount { get; protected set; }
    public int m_sssReliveCount { get; protected set; }
}

public class ScoreParamTable
{
    public Dictionary<int, ScoreParamInfo> m_map { get; protected set; }
    public ScoreParamInfo LookUp(int id)
    {
        ScoreParamInfo info = null;
        if (m_map.TryGetValue(id, out info))
        {
            return info;
        }

        return null;
    }

    public void Load(byte[] bytes)
    {
        m_map = new Dictionary<int, ScoreParamInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount      = helper.ReadInt();

        for (int index = 0; index < sceneCount; ++index)
        {
            ScoreParamInfo info = new ScoreParamInfo();
            
            info.Load(helper);

            m_map.Add(info.m_id, info);
        }
    }
}