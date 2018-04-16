using System;
using UnityEngine;

//攻击时移动
public class AttackingMoveAction : AttackAction
{
    public override ENType GetActionType() { return ENType.enAttackingMoveAction; }
    public static ENType SGetActionType() { return ENType.enAttackingMoveAction; }

    public string AnimName { get; private set; }
    float ActionLength { get; set; }
    public override void Init(int skillID, int skillTargetID)
    {
        InitImpl(skillID, skillTargetID, false, Vector3.zero);
    }
    public override void InitImpl(int skillID, int targetID, bool isSyncPosition, Vector3 syncPos)
    {
        IsSyncPosition = isSyncPosition;
        SyncPosition = syncPos;
        AnimStartTime = Time.time;

        m_skillID = skillID;
        m_skillInfo = GameTable.SkillTableAsset.Lookup(m_skillID);
        if (m_skillInfo != null)
        {
            AnimName = m_skillInfo.ConductMotion;
            ActionLength = m_skillInfo.ConductTime;
        }
        m_skillTargetID = targetID;

        if (CurrentActor.m_isDeposited)
        {//托管中，发送attackingmove action的消息
            Vector3 pos = CurrentActor.MainPos;
            IMiniServer.Singleton.SendAction_AttackingMove_C2BS(CurrentActor.ID, skillID, targetID, pos.x, pos.z);
        }
    }
    public override void OnEnter()
    {
        SyncPositionIfNeed();
        RefreshActionRef();
        //继承于AttackAction，所以不调用base
        //base.OnEnter();
        m_skillResultIDIndex = 0;
        CurrentActor.StopPlaySkillEventAnimation();
        CurrentActor.PlaySkillEventAnimation(m_skillInfo.ID, (int)Actor.ENSkillStepType.enConduct, AnimName, Actor.ENAnimType.enPlay);
    }
    public override void OnInterupt()
    {
        //继承于AttackAction，所以不调用base
        //base.OnInterupt(newType);
    }
    public override void OnExit()
    {
        //继承于AttackAction，所以不调用base
        //base.OnExit();
        //Debug.LogWarning("AttackingMoveAction OnExit");
        CurrentActor.StopPlaySkillEventAnimation();
    }
    public override bool OnUpdate()
    {
        if (Time.time - AnimStartTime > ActionLength)
        {
            //Debug.LogWarning("ActionLength " + ActionLength + ", AnimStartTime " + AnimStartTime);
            return true;
        }
        PlayMissingSound();
        return false;
    }
    public override void Reset()
    {
        base.Reset();
    }
}