using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum ENMissionState
{
    CanReceive,
    CannotReceive,
    UNFinished,
    Finished
}

public enum ENMissionFinishType 
{
    enKillEnemy = 1,
    enLevelUP   = 2,
    enFinishDounge = 3
}

public class MyMissions
{
    #region Singleton
    static MyMissions m_singleton;
    static public MyMissions Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new MyMissions();
            }
            return m_singleton;
        }
    }
    #endregion
    public MissionInst[] m_myMissions = new MissionInst[30];
//     public MissionInst[] MyMission 
//     {
//         get
//         {
//             return m_myMissions; 
//         }
//         private set;
//     }
    public short[] m_finish = new short[256];
    //public List<MissionInst> m_myMissionList = new List<MissionInst>();

    public void AddMission(MissionInst inst)
    {
        for (int index = 0; index < m_myMissions.Count(); index++)
        {
            if (null == m_myMissions[index] || m_myMissions[index].m_id == 0)
            {
                m_myMissions[index] = inst;
                return;
            }
        }
        return;
    }

    public void Clear()
    {
        for (int index = 0; index < m_myMissions.Count();index++ )
        {
            m_myMissions[index] = null;
        }
    }

    public void RemoveMission(int id)
    {
        for (int index = 0; index < m_myMissions.Count(); index++)
        {
            if (m_myMissions[index] == null)
            {
                continue;
            }
            if (m_myMissions[index].m_id == id)
            {
                m_myMissions[index] = null;
            }
        }
    }

    public MissionInst Lookup(int id)
    {
        for (int index = 0; index < m_myMissions.Count(); index++)
        {
            if (m_myMissions[index] == null)
            {
                continue;
            }
            if (m_myMissions[index].m_id == id)
            {
                return m_myMissions[index];
            }
        }
        return null;
    }

    public MissionInst FindMission(int id)
    {
        for (int index = 0; index < m_myMissions.Count(); index ++ )
        {
            if (m_myMissions[index] == null)
            {
                continue;
            }
            if (m_myMissions[index].m_id == id)
            {
                return m_myMissions[index];
            }
        }
        return null;
    }
    public void AddFinishedMission(int id)
    {
        Debug.Log(" AddFinishedMission" + id);
        short m_id = (short)id;
        if (FindFinishedByID(id) == false)
        {
            if (m_finish[id] == 0)
            {
                m_finish[id] = m_id;
            }
            //             for (int index = 0; index < 256; index++)
            //             {
            //                 if (m_finish[id] == 0)
            //                 {
            //                     m_finish[index] = m_id;
            //                 }
            //             }
        }
    }
    public bool FindFinishedByID(int id)
    {
		//short m_id = (short)id;
        if (m_finish[id] != 0)
        {
            return true;
        }
//         for (int index = 0; index < 256;index++ )
//         {
//             if (m_finish[index] == m_id)
//            {
//                 return true;
//            }
//         }
        return false;
    }

    public void AcceptMissionMsg(int id)
    {
        //AcceptMission packet = new AcceptMission((short)id);
        //ClientNet.Singleton.SendPacket(packet);
        Debug.Log("Accept");
    }

    //设置任务完成状态
    public void SetFinishMission(int id)
    {
        MissionInst inst = FindMission(id);
        if (inst != null)
        {
            inst.m_isFinished = true;
        }
    }

    public void RenounceMissionCallback(int id)
    {
        //删除任务
        RemoveMission(id);
        /*        myMission.m_myMissionList.Add(inst);*/
        Debug.Log("Renounce Mission " + id);
    }

    public void AcceptMissionCallback(int id, bool finished)
    {
        MissionInfo info = GameTable.MissionTableAsset.Lookup(id);
        MissionInst inst = new MissionInst();
        inst.m_id = id;
        inst.m_isFinished = finished;
        inst.m_questRequire1 = info.condition1;
        inst.m_questRequire2 = info.condition2;
        inst.m_questRequire3 = 0;
        MyMissions myMission = MyMissions.Singleton;
        myMission.AddMission(inst);
        if (finished == true)
        {
            AddFinishedMission(id);
        }
        /*        myMission.m_myMissionList.Add(inst);*/
        Debug.Log("Add Mission " + id);
    }

    public void FinishMissionMsg(int id)
    {
        //FinishedMission packet = new FinishedMission((short)id);
        //ClientNet.Singleton.SendPacket(packet);
        Debug.Log("Finished");
    }

    public void FinishMissionCallback(int id)
    {
        RemoveMission(id);
        AddFinishedMission(id);
    }
}

public class MissionInst
{
    public int m_id;
    public bool m_isFinished;
    public short m_questRequire1;
    public short m_questRequire2;
    public short m_questRequire3;
    public short m_questRequire4;
}
