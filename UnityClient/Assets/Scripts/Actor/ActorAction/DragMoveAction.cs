using System;
using UnityEngine;
//被拖拽

public class DragMoveAction : ActorAction
{
    Vector3 m_moveForward = Vector3.zero;          //移动方向
    float mSpeed = 0f;                            //移动速度  
    float mActionLength = 0f;                    //Action执行的时长

    public override ENType GetActionType() { return ENType.enDragMoveAction; }
    public static ENType SGetActionType() { return ENType.enDragMoveAction; }

    public void Init(Vector3 targetPos, float offset, float speed)
    {
        mSpeed = speed;

        Vector3 tmpTargetPos = targetPos - CurrentActor.MainPos;
        tmpTargetPos.y = 0f;
        if (tmpTargetPos.magnitude <= offset)
        {
            IsInited = true;
            IsEnable = false;
            return;
        }
        mActionLength = (tmpTargetPos.magnitude + offset) / mSpeed;

        m_moveForward = tmpTargetPos;
        m_moveForward.Normalize();          //从被拖拽对象到拖拽目标点的向量

        AnimStartTime = Time.time;
        if (CurrentActor.CenterCollider != null)
        {
            CurrentActor.CenterCollider.gameObject.layer = LayerMask.NameToLayer("DisableCollider");
        }
    }
    public override void OnEnter()
    {
        RefreshActionRef();
    }

    public override void OnExit()
    {
        if (CurrentActor.CenterCollider != null)
        {
            CurrentActor.CenterCollider.gameObject.layer = LayerMask.NameToLayer("Actor");
        }
    }

    public override bool OnUpdate()
    {
        if(mActionLength > Time.time - AnimStartTime)
        {
            CurrentActor.MainPos = CurrentActor.MainPos + m_moveForward * mSpeed * Time.deltaTime;
            return false;
        }
        return true;
    }

    public override void Reset()
    {
        base.Reset();
        m_moveForward = Vector3.zero;          //移动方向
        mSpeed = 0f;                            //移动速度  
        mActionLength = 0f;                    //Action执行的时长
    }
}
