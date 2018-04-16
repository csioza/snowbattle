using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NGE.Network;

class ResultAddExp : IResult
{
    public int m_expValue = 0;
    public bool m_isLevelUp = false;
    public int m_level = 0;
    public int m_curtExp = 0;
    public int m_levelUpPoint = 0;

    public ResultAddExp()
		:base((int)ENResult.AddExp)
	{
        m_expValue = 0;
        m_isLevelUp = false;
        m_level = 0;
        m_curtExp = 0;
        m_levelUpPoint = 0;
	}
    public static IResult CreateNew()
    {
        return new ResultAddExp();
    }
    public override void Deserialize(PacketReader stream)
    {
        base.Deserialize(stream);
        m_expValue = stream.ReadInt32();
        m_isLevelUp = (stream.ReadInt32() == 1) ? true : false;
        m_level = stream.ReadInt32();
        m_curtExp = stream.ReadInt32();
        m_levelUpPoint = stream.ReadInt32();
    }

    public override void ResultServerExec(IResultControl control)
    {
        base.ResultServerExec(control);
        Actor target = ActorManager.Singleton.Lookup(TargetID);
        if (null == target)
        {
            return;
        }
        target.Props.SetProperty_Int32(ENProperty.Exp, m_curtExp);
        target.Props.SetProperty_Int32(ENProperty.LevelUpPoint, m_levelUpPoint);
        if (m_isLevelUp)
        {
            target.Props.SetProperty_Int32(ENProperty.level, m_level);
        }
        MainPlayer mainPlayer = ActorManager.Singleton.MainActor;
        if (mainPlayer.ID == TargetID)
        {
            mainPlayer.UpDataSkillLevelUp();
            if (m_isLevelUp)
            {
                //播放升级特效
                mainPlayer.UpdataMainHeadLevel();
            }
        }
        //是否需要显示获得经验数字

    }
}
