using UnityEngine;
using System.Collections;
using NGE.Network;

public class ResultRefreshCD : IResult
{
    enum RefreshSkillType
    {
        enNone,
        enAllSkill,
        enSkill,
    }
    RefreshSkillType m_type = RefreshSkillType.enNone;
    int mSkillID = -1;
    public bool mRefreshResult = false;
    public ResultRefreshCD()
        : base((int)ENResult.RefreshCD)
    {
    }
    public static IResult CreateNew()
    {
        return new ResultRefreshCD();
    }

    public override void Deserialize(PacketReader stream)
    {
        base.Deserialize(stream);
        mRefreshResult  = (stream.ReadInt32() ==1) ?true:false;
        mSkillID        = stream.ReadInt32();

    }



    public override void ResultExpr(float[] param)
    {
        //传入参数，1，移除几率。2、技能（自身全部技能，当前释放技能）
        base.ResultExpr(param);
        if (param == null)
        {
            return;
        }
        float randPercent = UnityEngine.Random.Range(0f, 1f);
        if (randPercent > param[0])
        {
            return;
        }
        if (param[1] == 0)
        {
            m_type = RefreshSkillType.enAllSkill;
        }
        else
        {
            m_type = RefreshSkillType.enSkill;
            mSkillID = (int)param[1];
        }
        mRefreshResult = true;
    }
    public override void Exec(IResultControl control)
    {
        if (mRefreshResult)
        {
            Actor targetActor = ActorManager.Singleton.Lookup(SourceID);
            if (targetActor == null)
            {
                return;
            }
            switch (m_type)
            {
                case RefreshSkillType.enAllSkill:
                    foreach (var tmpCD in targetActor.CDControl.CDList)
                    {
                        tmpCD.Remove();
                    }
                    break;
                case RefreshSkillType.enSkill:
                    foreach (var tmpCD in targetActor.CDControl.CDList)
                    {
                        if (mSkillID == tmpCD.GetCDSkillID())
                        {
                            tmpCD.Remove();
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public override void ResultServerExec(IResultControl control)
    {
        if (mRefreshResult)
        {
            Actor targetActor = ActorManager.Singleton.Lookup(SourceID);
            if (targetActor == null)
            {
                return;
            }
            
            // 全部技能
            if (mSkillID == 0 )
            {
                foreach (var tmpCD in targetActor.CDControl.CDList)
                {
                    tmpCD.Remove();
                }
            }
            // 某个技能
            else
            {
                foreach (var tmpCD in targetActor.CDControl.CDList)
                {
                    if (mSkillID == tmpCD.GetCDSkillID())
                    {
                        tmpCD.Remove();
                        break;
                    }
                }
            }
        }

    }
}
