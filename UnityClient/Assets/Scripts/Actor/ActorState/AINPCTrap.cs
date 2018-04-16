using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AINpcTrap : AIBase
{
    protected Trap Self { get { return Owner as Trap; } }
    public AINode m_baseAI = null;
    //下一次ai的间隔
    protected float m_nextAITime = 0.5f;
    //上次机关触发时间
    public float tmp_preAttackTime = 0f;
    //机关被触发次数
    public int tmp_attackCount = 0;
    public AINpcTrap(string fileName, string bossStr)
    {
        m_baseAI = AINodeManager.CreateBossAI(fileName, bossStr);

//         AIBaseData tmpAIBaseData = GetBaseData("PreAttackTime", AIBaseData.DataType.enTime);
//         tmpAIBaseData = GetBaseData("AttackCount", AIBaseData.DataType.enInt);
    }
    public override void Update()
    {
//         m_nextAITime -= Time.deltaTime;
//         if (m_nextAITime > 0.0f)
//         {
//             return;
//         }
//         m_nextAITime = UnityEngine.Random.Range(0.1f, 0.5f);
        if (Self.IsDead)
        {//死亡
            return;
        }
        if (Self.mTrapState ==Trap.TrapState.enDisable)
        {
            return;
        }

        //m_baseAI.Exec(Owner);
//         if (false)
//         {//如果有命令执行命令函数
// 
//         }
//         else if (CheckAttackAction())
//         {//执行攻击Action
// 
//         }
//         else if (OnContinueDamageAction())
//         {//执行持续伤害Action
// 
//         }
//         else
//         {
//             OnStandByAction();
//         }
    }
    public void Reset()
    {
        tmp_preAttackTime = Time.time;
        tmp_attackCount = 0;
    }
//     public void PushActorIDToList(int actorId)
//     {
//         mEnterActorIDList.Add(actorId);
//     }
// 
//     public void PopActorIDToList(int actorId)
//     {
//         mEnterActorIDList.Remove(actorId);
//     }

    public bool CheckAttackAction()
    {
//         if (Self.mEnterActorIDList.Count <= 0)
//         {
//             return false;
//         }
        AIBaseData curAttackCount = GetBaseData("AttackCount", AIBaseData.DataType.enInt);
        if (Self.mMaxAttackCount > 0)
        {
            if (curAttackCount.GetValue_Float() > Self.mMaxAttackCount)
            {
                return false;
            }
        }
        AIBaseData preAttackTime = GetBaseData("PreAttackTime", AIBaseData.DataType.enTime);
        if (Time.time - preAttackTime.GetValue_Float() < Self.mMinAttackTime)
        {
            return false;
        }
        Self.TrapActive = true;
        if (OnAttackAction())
        {
            preAttackTime.ResetValue() ;
            curAttackCount.SetValue(curAttackCount.GetValue_Float() + 1);
        }
        return false;
    }
    public bool OnAttackAction()
    {
        TrapAttackAction attackAction = Self.mActionControl.AddAction(TrapAction.ENType.enAttackAction) as TrapAttackAction;
        if (attackAction != null)
        {
            attackAction.Init(Self.mAttackActionAnimList);
            return true;
        }
        return false;
    }
    public bool OnContinueDamageAction()
    {
        return false;
    }

    public bool OnStandByAction()
    {
        TrapStandAction attackAction = Self.mActionControl.AddAction(TrapAction.ENType.enStandAction) as TrapStandAction;
        if (attackAction != null)
        {
            attackAction.Init(Self.mStandByAnimList[0]);
            return true;
        }
        return false;
    }
    public bool OnLockAction()
    {
        return false;
    }
}
