using System;
using System.Collections;
using UnityEngine;

class BattleFactory
{
    #region Singleton
    static BattleFactory m_singleton;
    static public BattleFactory Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new BattleFactory();
                m_singleton.Init();
            }
            return m_singleton;
        }
    }
    #endregion
    private BattleContext m_battleContext = null;
    public BattleContext GetBattleContext() { return m_battleContext; }
    public BattleFactory()
    {
        m_battleContext = new BattleContext();
    }
    void Init()
    {
        InitResult();
        InitBuff();
    }
    void InitResult()
    {
        // register all result
        m_battleContext.RegisterResult((int)ENResult.Skill, ResultSkill.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.Damage, ResultDamage.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.AddBuff, ResultAddBuff.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.RemoveBuff, ResultRemoveBuff.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.Dead, ResultDead.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.StaminaChanged, ResultStaminaChanged.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.Relive, ResultRelive.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.OpenBox, OpenBoxResult.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.Health, ResultHealth.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.InstantDeath, ResultInstantDeath.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.SkillCD, ResultSkillCD.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.Taut, ResultTaut.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.Drag, ResultDrag.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.RefreshCD, ResultRefreshCD.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.ChainDamage, ResultChainDamage.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.Call, ResultCall.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.Vampire, ResultVampire.CreateNew);
        m_battleContext.RegisterResult((int)ENResult.AddExp, ResultAddExp.CreateNew);
        
    }
    void InitBuff()
    {
        // register all buff
        m_battleContext.RegisterBuffEffect((int)ENBuff.ControlMove, BuffEffectControlMove.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.AddUndown, BuffEffectAddUndown.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.NoDeadAddBuff, BuffEffectNoDeadAddBuff.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.StaminaChanged, BuffEffectStaminaChanged.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.Vampire, BuffEffectVampire.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.ContinuedReduceHP, BuffEffectContinuedReduceHP.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.ChangeDamage, BuffEffectChangeDamage.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.ChangeRestore, BuffEffectChangeRestore.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.ContinuedRestoreHP, BuffEffectContinuedRestoreHP.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.SkillSilence, BuffEffectSkillSilence.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.OffsetDamage, BuffEffectOffsetDamage.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.ControlAttack, BuffEffectControlAttack.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.ControlBeAttack, BuffEffectControlBeAttack.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.ReturnDamage, BuffEffectReturnDamage.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.DecreaseDamage, BuffEffectDecreaseDamage.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.ChangeModel, BuffEffectChangeMode.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.Halo, BuffEffectHalo.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.Chaos, BuffEffectChaos.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.ZeroCD, BuffEffectZeroCD.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.OutlawDeath, BuffEffectOutlawDeath.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.Fear, BuffEffectFear.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.IgnoreDebuff, BuffEffectIgnoreDebuff.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.BreakWithDamage, BuffEffectBreakWithDamage.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.Charm, BuffEffectCharm.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.Sneak, BuffEffectSneak.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.FindSneak, BuffEffectFindSneak.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.Teleport, BuffEffectTeleport.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.Plague, BuffEffectPlague.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.ChangeBuff, BuffEffectChangeBuff.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.DamageSuperimposition, BuffEffectDamageSuperimposition.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.KillSuperimposition, BuffEffectKillSuperimposition.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.HealthSuperimposition, BuffEffectHealthSuperimposition.CreateNew);
        m_battleContext.RegisterBuffEffect((int)ENBuff.DamageRacial, BuffEffectDamageRacial.CreateNew);
    }
    #region result
    public IResult CreateResult(ENResult classID, int sourceID, int targetID, int skillResultID = 0, int skillID = 0, float[] param = null)
    {

        // 如果是长连接
        if (ClientNet.Singleton.IsLongConnecting)
        {
            Actor actor = ActorManager.Singleton.Lookup(sourceID);
            if ( null != actor )
            {
                // 如果托管则 发送服务器 创建result的消息
                if (actor.m_isDeposited)
                {
                    MiniServer.Singleton.SendCreateResult_C2BS((int)classID, sourceID, targetID, skillResultID, skillID, param); 
                }
                
            }
            return null;
        }


        IResult result = BattleFactory.Singleton.GetBattleContext().CreateResult((int)classID);
        if (result == null)
        {
            Debug.LogWarning("result create fail, classID is " + classID.ToString());
        }
        else
        {
            result.SourceID = sourceID;
            result.TargetID = targetID;
            result.SkillResultID = skillResultID;
            result.SkillID = skillID;
        }

        return result;
    }
    public void DispatchResult(IResult r)
    {
        GetBattleContext().CreateResultControl().DispatchResult(r);
    }
    #region SkillResult
    //通过skillResultID产生技能result
    public bool CreateSkillResult(int sourceID, int skillResultID, int targetID = 0, int skillID = 0, float[] param = null)
    {
        m_self = ActorManager.Singleton.Lookup(sourceID);
        if (m_self == null)
        {
            Debug.LogWarning("self is null, id is " + sourceID);
            return false;
        }
        Actor target = ActorManager.Singleton.Lookup(targetID);
        SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(skillResultID);
        if (info == null)
        {
            if (skillResultID != 0)
            {
                Debug.LogWarning("result id is error, id is " + skillResultID);
            }
            return false;
        }
        float range = float.MinValue;
        if (info.InstantRange != 0)
        {
            range = info.InstantRange;
        }
        //优先判断当前目标和自己
        switch ((ENResultTargetType)info.ResultTargetType)
        {
            case ENResultTargetType.enEnemySingle:
                {
                    if (target != null && !target.IsDead)
                    {
                        if (ActorTargetManager.IsEnemy(m_self, target))
                        {
                            float d = ActorTargetManager.GetTargetDistance(m_self.RealPos, target.RealPos);
                            if (d <= range)
                            {//给当前技能目标加skillresult
                                CreateResult_Skill(info.ID, target, skillID, param);
                                return true;
                            }
                        }
                    }
                }
                break;
            case ENResultTargetType.enFriendlySingle:
                {
                    if (target != null && !target.IsDead)
                    {
                        if (ActorTargetManager.IsFriendly(m_self, target))
                        {
                            float d = ActorTargetManager.GetTargetDistance(m_self.RealPos, target.RealPos);
                            if (d <= range)
                            {//给当前技能目标加skillresult
                                CreateResult_Skill(info.ID, target, skillID, param);
                                return true;
                            }
                        }
                    }
                }
                break;
            case ENResultTargetType.enSelf:
            case ENResultTargetType.enFriendlySingleAndSelf:
                {//给自己加skillresult
                    CreateResult_Skill(info.ID, m_self, skillID, param);
                    return true;
                }
                //break;
        }
        ActorManager.Singleton.ForEach_result(CheckTarget, new float[4] { info.ID, range, info.ResultTargetType, skillID });
        return true;
    }
    Actor m_self = null;
    bool isReturn = false;
    //paramList
    //0:resultID
    //1:resultRange
    //2:resultTargetType
    //3:skillID
    void CheckTarget(Actor target, float[] paramList)
    {
        if (isReturn)
        {
            return;
        }
        if (target.IsDead)
        {
            return;
        }
        float range = paramList[1];
        float d = ActorTargetManager.GetTargetDistance(m_self.RealPos, target.RealPos);
        if (d > range)
        {
            return;
        }
        ENResultTargetType type = (ENResultTargetType)paramList[2];
        switch ((ENResultTargetType)type)
        {
            case ENResultTargetType.enEnemySingle:
                {
                    if (ActorTargetManager.IsEnemy(m_self, target))
                    {
                        CreateResult_Skill((int)paramList[0], target, (int)paramList[3], paramList);
                        isReturn = true;
                    }
                }
                break;
            case ENResultTargetType.enEnemyAll:
                {
                    if (ActorTargetManager.IsEnemy(m_self, target))
                    {
                        CreateResult_Skill((int)paramList[0], target, (int)paramList[3], paramList);
                    }
                }
                break;
            case ENResultTargetType.enFriendlySingle:
                {
                    if (ActorTargetManager.IsFriendly(m_self, target))
                    {
                        CreateResult_Skill((int)paramList[0], target, (int)paramList[3], paramList);
                        isReturn = true;
                    }
                }
                break;
            case ENResultTargetType.enFriendlyAll:
                {
                    if (ActorTargetManager.IsFriendly(m_self, target))
                    {
                        CreateResult_Skill((int)paramList[0], target, (int)paramList[3], paramList);
                        isReturn = true;
                    }
                }
                break;
            case ENResultTargetType.enEveryone:
                {
                    CreateResult_Skill((int)paramList[0], target, (int)paramList[3], paramList);
                }
                break;
            case ENResultTargetType.enFriendlyAllAndSelf:
                {
                    if (ActorTargetManager.IsFriendly(m_self, target) || m_self == target)
                    {
                        CreateResult_Skill((int)paramList[0], target, (int)paramList[3], paramList);
                    }
                }
                break;
        }
    }
    void CreateResult_Skill(int resultID, Actor target, int skillID, float[] param = null)
    {
        IResult r = CreateResult(ENResult.Skill, m_self.ID, target.ID, resultID, skillID, param);
        if (r != null)
        {
            DispatchResult(r);
        }
        //设置爆破点
        target.SetBlastPos(m_self.RealPos, m_self.GetBodyObject().transform.forward);

        //MiniServer.Singleton.SendCreateResult_C2BS((int)ENResult.Skill, 0, m_self.ID, target.ID, resultID, skillID);
    }
    #endregion
    #region BuffResult
    public bool AddBuff(int sourceID, int targetID, int buffID)
    {
        IResult r = CreateResult(ENResult.AddBuff, sourceID, targetID,0,0, new float[1] { (float)buffID });
        if (r != null)
        {
            r.ResultExpr(new float[1] { (float)buffID });
            DispatchResult(r);
            return true;
        }
        return false;
    }
    public bool RemoveBuff(int sourceID, int targetID, int buffID)
    {
        IResult r = CreateResult(ENResult.RemoveBuff, sourceID, targetID, 0, 0, new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffID, (float)buffID });
        if (r != null)
        {
            r.ResultExpr(new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffID, (float)buffID });
            DispatchResult(r);
            return true;
        }
        return false;
    }
    public bool RemoveBuff(int sourceID, int targetID, BuffInfo.ENBuffType buffType)
    {
        IResult r = CreateResult(ENResult.RemoveBuff, sourceID, targetID,0, 0, new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffType, (float)buffType });
        if (r != null)
        {
            r.ResultExpr(new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffType, (float)buffType });
            DispatchResult(r);
            return true;
        }
        return false;
    }
    #endregion
    #endregion
}
