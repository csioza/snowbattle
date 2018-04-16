using System;
using UnityEngine;


public class AttackActionCallback : MonoBehaviour
{
	void ChangeResultID(AnimationEvent animEvent)
	{
		ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
		if (null == selfProp)
        {
			return;
		}
		AttackAction action = selfProp.ActorLogicObj.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
        if (null != action)
        {
            //action.m_triggerSkillResultID = animEvent.intParameter;
            ++action.m_skillResultIDIndex;
            action.m_firstTarget = null;
        }
        else
        {
            AttackingMoveAction amAction = selfProp.ActorLogicObj.ActionControl.LookupAction(AttackingMoveAction.SGetActionType()) as AttackingMoveAction;
            if (amAction != null)
            {
                //amAction.m_triggerSkillResultID = animEvent.intParameter;
                ++amAction.m_skillResultIDIndex;
                amAction.m_firstTarget = null;
            }
        }
	}
	void TriggerSelfSpasticity(AnimationEvent animEvent)
	{
		ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
		if (null == selfProp)
		{
			return;
		}
		SelfSpasticityAction spAction = selfProp.ActorLogicObj.ActionControl.LookupAction (ActorAction.ENType.enSelfSpasticityAction) as SelfSpasticityAction;
		if (null == spAction)
        {
			spAction = selfProp.ActorLogicObj.ActionControl.AddAction(ActorAction.ENType.enSelfSpasticityAction) as SelfSpasticityAction;
		}
        if (null != spAction)
        {
            spAction.ChangeDurationTime(animEvent.floatParameter);
        }
	}
    Transform RecursionQueryBone(Transform child, string boneName)
    {
        if (child.name == boneName)
        {
            return child;
        }
        for (int i = 0; i < child.childCount; i++ )
        {
            Transform boneTransform = RecursionQueryBone(child.GetChild(i), boneName);
            if (null != boneTransform)
            {
                return boneTransform;
            }
        }
        return null;
    }
    void TriggerBoneStartPosition(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp)
        {
            return;
        }
        string boneName = animEvent.stringParameter;
        Transform trans = RecursionQueryBone(selfProp.ActorLogicObj.MainObj.transform, boneName);
        if (trans != null)
        {
            selfProp.ActorLogicObj.mCurAttackBoneStartPos = trans.position;
        }
    }
    void TriggerBoneEndPosition(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp)
        {
            return;
        }
        string boneName = animEvent.stringParameter;
        Transform trans = RecursionQueryBone(selfProp.ActorLogicObj.MainObj.transform, boneName);
        if (trans != null)
        {
            selfProp.ActorLogicObj.mCurAttackBoneEndPos = trans.position;
        }
    }
	/*void OnTriggerEnter(Collider other)
	{
        if (!ActorTargetManager.IsTrigger(other))
        {
            return;
        }
        //Debug.LogWarning(gameObject.name + " trigger with " + other);
		ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
		if (null == selfProp)
		{
            Debug.LogWarning("trigger return, actorProp get failed");
			return;
		}
        GlobalEnvironment.Singleton.IsInCallbackOrTrigger = true;
        try
        {
            Actor self = selfProp.ActorLogicObj;
            if (null != self && !self.IsDead)
            {
                AttackAction action = self.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
                if (null != action)
                {
                    action.OnTriggerEnter(gameObject, other);
                }
                else
                {
                    AttackingMoveAction amAction = self.ActionControl.LookupAction(AttackingMoveAction.SGetActionType()) as AttackingMoveAction;
                    if (amAction != null)
                    {
                        amAction.OnTriggerEnter(gameObject, other);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error On OnTriggerEnter" + e.Message + ",,,,Stack:" + e.StackTrace.ToString());
            DebugLog.Singleton.OnShowLog("[AttackActionCallback Error On OnTriggerEnter] " + e.Message + " " + e.StackTrace.ToString());
        }
        GlobalEnvironment.Singleton.IsInCallbackOrTrigger = false;
	}*/
    void ChangeInstantResultID(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp)
        {
            return;
        }
        Actor self = selfProp.ActorLogicObj;
        if (null != self && !self.IsDead)
        {
            AttackAction action = self.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
            if (null != action)
            {
                ++action.m_skillResultIDIndex;
                action.CreateSkillResult();
            }
            else
            {
                AttackingMoveAction amAction = self.ActionControl.LookupAction(AttackingMoveAction.SGetActionType()) as AttackingMoveAction;
                if (amAction != null)
                {
                    ++amAction.m_skillResultIDIndex;
                    amAction.CreateSkillResult();
                }
            }
        }
    }
    //停止动作(技能目标死亡或被击飞时，停止AttackAction)
    void StopAction(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp)
        {
            Debug.LogWarning("StopAction return, actorProp get failed");
            return;
        }
        Actor self = selfProp.ActorLogicObj;
        if (null != self && !self.IsDead)
        {
            AttackAction action = self.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
            if (null != action)
            {
                if (action.m_skillTarget != null)
                {
                    if (action.m_skillTarget.IsDead)
                    {//技能目标死亡
                        action.IsFinished = true;
                    }
                    else
                    {
                        BeAttackAction baAction = action.m_skillTarget.ActionControl.LookupAction(ActorAction.ENType.enBeAttackAction) as BeAttackAction;
                        if (baAction != null && baAction.m_isFly)
                        {//技能目标被击飞
                            action.IsFinished = true;
                        }
                    }
                }
            }
        }
    }
    //播放武器动作(int：0为左手，其它为右手，string：动画名称)
    void PlayWeaponsAnimation(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp)
        {
            Debug.LogWarning("StopAction return, actorProp get failed");
            return;
        }
        Actor self = selfProp.ActorLogicObj;
        if (null != self && !self.IsDead)
        {
            self.PlayWeaponsAnimation(animEvent.intParameter != 0, animEvent.stringParameter);
        }
    }
    //停止无效的攻击动作，在每个trigger的前后各有一个
    void StopInvalidAttack(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp)
        {
            Debug.LogWarning("StopAction return, actorProp get failed");
            return;
        }
        Actor self = selfProp.ActorLogicObj;
        if (null != self && !self.IsDead)
        {
            AttackAction action = self.ActionControl.LookupAction(AttackAction.SGetActionType()) as AttackAction;
            if (action != null)
            {
                action.StopInvalidAttack();
            }
        }
    }
    //攻击中，开始旋转
    //void StartRotate(AnimationEvent animEvent)
    //{
    //    ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
    //    if (null == selfProp)
    //    {
    //        Debug.LogWarning("StopAction return, actorProp get failed");
    //        return;
    //    }
    //    Actor self = selfProp.ActorLogicObj;
    //    if (null != self && !self.IsDead)
    //    {
    //        AttackAction action = self.ActionControl.LookupAction(AttackAction.SGetActionType()) as AttackAction;
    //        if (action != null)
    //        {
    //            action.StartRotate(animEvent.floatParameter);
    //        }
    //    }
    //}
    //立即结束攻击动作
    void TerminateAttackAction(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp)
        {
            Debug.LogWarning("StopAction return, actorProp get failed");
            return;
        }
        Actor self = selfProp.ActorLogicObj;
        if (null != self && !self.IsDead)
        {
            AttackAction action = self.ActionControl.LookupAction(AttackAction.SGetActionType()) as AttackAction;
            if (action != null)
            {
                action.IsFinished = true;
            }
        }
    }
    //显示武器model
    void ShowWeapon(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp)
        {
            Debug.LogWarning("StopAction return, actorProp get failed");
            return;
        }
        Actor self = selfProp.ActorLogicObj;
        if (null != self && !self.IsDead)
        {
            self.ShowWeaponModelWithTable(animEvent.intParameter == 0);
        }
    }
};