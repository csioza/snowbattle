using UnityEngine;

public enum ENFearType
{
    enNone,
    enEscape,//逃跑
}
//npc的恐惧ai
public class AINpc_Fear : AINpc
{
    //恐惧类型
    ENFearType m_fearType = ENFearType.enNone;
    //恐惧的目标ID
    int m_targetID = 0;
    //逃跑距离
    float m_escapeDistance = 0;
    public AINpc_Fear(ENFearType fearType, float escapeDistance, int targetID)
    {
        m_fearType = fearType;
        m_targetID = targetID;
        m_escapeDistance = escapeDistance;
    }
    public override void Update()
    {
        base.BaseUpdate();
        if (Self.IsDead)
        {//死亡
            return;
        }
        m_nextAITime -= Time.deltaTime;
        if (m_nextAITime > 0.0f)
        {
            return;
        }
        m_nextAITime = UnityEngine.Random.Range(0.1f, 0.5f);
        FindSneak();
        switch (m_fearType)
        {
            case ENFearType.enEscape:
                {
                    Actor target = ActorManager.Singleton.Lookup(m_targetID);
                    if (target != null)
                    {
                        float distance = ActorTargetManager.GetTargetDistance(Owner.MainPos, target.MainPos);
                        if (distance < m_escapeDistance)
                        {//恐惧逃跑
                            Vector3 direction = Owner.MainPos - target.MainPos;
                            direction.y = 0;
                            direction.Normalize();

                            Vector3 targetPos = Owner.MainPos + direction * (m_escapeDistance - distance);
                            MoveAction action = Owner.ActionControl.AddAction(MoveAction.SGetActionType()) as MoveAction;
                            if (action != null)
                            {
                                action.Retarget(targetPos);
                            }
                            return;
                        }
                    }
                    //恐惧站立
                    Owner.ActionControl.RemoveAction(MoveAction.SGetActionType());
                    StandAction standAction = Owner.ActionControl.LookupAction(StandAction.SGetActionType()) as StandAction;
                    if (standAction == null)
                    {
                        standAction = Owner.ActionControl.AddAction(StandAction.SGetActionType()) as StandAction;
                    }
                    if (standAction != null)
                    {
                        standAction.AnimName = "standby";
                    }

                }
                break;
        }
    }
}