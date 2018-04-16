using UnityEngine;
using System.Collections;
using NGE.Network;

public class ResultSkillCD : IResult
{
    float[] m_buffIDList = new float[] { };
    private float m_cdTime = 0;
    public float CDTime { get { return m_cdTime; } set { m_cdTime = value; } }
    CD m_curCD = null;

    int m_cdId      = 0;
    int m_skillId   = 0;
    public ResultSkillCD()
        : base((int)ENResult.SkillCD)
    {

    }
    public static IResult CreateNew()
    {
        return new ResultSkillCD();
    }

    public override void Deserialize(PacketReader stream)
    {
        base.Deserialize(stream);
        m_cdId      = stream.ReadInt32();
        m_skillId   = stream.ReadInt32();
    }



    public override void ResultExpr(float[] param)
    {
        base.ResultExpr(param);
        if (param != null)
        {
            m_buffIDList = param;
        }

        Actor targetActor = ActorManager.Singleton.Lookup(TargetID);
        m_curCD = new CD((int)m_buffIDList[0], (int)m_buffIDList[1]);
        m_curCD.OnEnter(targetActor);
        m_cdTime = m_curCD.m_cdTime;
    }
    public override void Exec(IResultControl control)
    {
        Actor targetActor = ActorManager.Singleton.Lookup(TargetID);
        m_curCD.m_cdTime = m_cdTime;
        targetActor.CDControl.CDListAdd(m_curCD);
    }

    public override void ResultServerExec(IResultControl control)
    {
        Actor targetActor = ActorManager.Singleton.Lookup(TargetID);
        if (targetActor == null)
        {
            return;
        }
        if (m_cdId == 0 || m_skillId == 0)
        {
            return;
        }

        m_curCD = new CD(m_cdId, m_skillId);
        m_curCD.OnEnter(targetActor);
        targetActor.CDControl.CDListAdd(m_curCD);
    }
}
