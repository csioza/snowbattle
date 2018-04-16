using System;
using System.Collections.Generic;
using NGE.Network;

public class SResultDamage : ResultDamage
{
    private int m_hp = 10;
    private int m_sp = 1;
    private bool m_isFly = false;

    public SResultDamage()
    {
    }
    public override void Serialize(PacketWriter stream)
    {
        base.Serialize(stream);
        stream.Write(m_sp);
        stream.Write(m_hp);
        //stream.Write(m_skillid);
        if (m_isHit)
            stream.Write(1);
        else
            stream.Write(0);
        if (m_isCrit)
            stream.Write(1);
        else
            stream.Write(0);
        if (m_isFly)
            stream.Write(1);
        else
            stream.Write(0);
    }
    public override void ResultExpr(float[] param)
    {
        base.ResultExpr(param);

        //Actor actorSrc = ActorManager.Singleton.Lookup(SourceID);
        //Actor actorTarget = ActorManager.Singleton.Lookup(TargetID);
        //SkillResultTable skillResultTable = GameTable.SkillResultTableAsset;
        //SkillResultInfo skillData = skillResultTable.Lookup(m_skillid);
        //int baseDamage = 0;
        //if (null != skillData)
        //{
        //    int level = actorSrc.Props.GetProperty_Int32(ENProperty.level);
        //    if (0 == level)
        //    {
        //        level = 1;
        //    }
        //    for (int index = 0; index < skillData.ParamList.Length; ++index)
        //    {
        //        if ((int)ENSkillType.enPhyAttack == skillData.SkillType)
        //        {
        //            int phyAttack = actorSrc.Props.GetProperty_Int32(ENProperty.phyattack);
        //            int phpDefend = actorTarget.Props.GetProperty_Int32(ENProperty.phydefend);
        //            //int npcID = actorTarget.Props.GetProperty_Int32(ENProperty.NPC_IDInTable);
        //            baseDamage = (int)((float)phyAttack * (1 - (float)phpDefend / ((float)level * 10 + (float)phpDefend)));
        //        }
        //        else if (skillData.SkillType == (int)ENSkillType.enMagAttack)
        //        {
        //            int magAttack = actorSrc.Props.GetProperty_Int32(ENProperty.magattack);
        //            int magDefend = actorTarget.Props.GetProperty_Int32(ENProperty.magdefend);
        //            baseDamage = (int)((float)magAttack * (1 - (float)magDefend / ((float)level * 10 + (float)magDefend)));
        //        }
        //        int skillLevel = 1;
        //        if ((int)ENDamageFormula.enSkillDamageID == (int)param[0])
        //        {
        //            m_hp = (int)((float)baseDamage * (param[1] + (skillLevel - 1) * param[2]) +
        //                param[3] + (skillLevel - 1) * param[4]);
        //        }
        //    }
        //}
    }
    public override void Exec(IResultControl control)
    {
        base.Exec(control);
        Actor actorTarget = ActorManager.Singleton.Lookup(TargetID);
		//Actor actorSrc = ActorManager.Singleton.Lookup(SourceID);
        int hpTarget = actorTarget.Props.GetProperty_Int32(ENProperty.hp);
        if (!m_isHit)
        {
            m_hp = 0;
        }
        if (m_isCrit)
        {
            m_hp = 2 * m_hp;
        }
        hpTarget = hpTarget - m_hp;
        if (0 >= hpTarget)
        {
            hpTarget = 0;
        }
        if (0 == hpTarget)
        {
            SResultDead deadResult = (SResultDead)control.GetContext().CreateResult((int)ENResult.Dead);
            deadResult.SourceID = this.SourceID;
            deadResult.TargetID = this.TargetID;
            control.DispatchResult(deadResult);
        }
        if (95 >= UnityEngine.Random.Range(0, 100))
        {
            m_isFly = true;
        }
    }
}
