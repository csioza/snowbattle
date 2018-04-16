using System;
using UnityEngine;
using System.Collections.Generic;
//飞行道具的脚本
public class FlyingItemCallback : MonoBehaviour
{
    //flying item info
    private RemoteAttackManager.Info m_info = null;
    //已经trigger到的目标列表
    private List<int> m_targetIDList = new List<int>();
    //偏移
    Vector3 m_offset = Vector3.zero;
    public void Init(RemoteAttackManager.Info info)
    {
        GetComponent<Rigidbody>().WakeUp();
        m_targetIDList.Clear();
        m_info = info;
        if (!string.IsNullOrEmpty(m_info.m_itemInfo.Item_BoneOffset))
        {
            string[] param = m_info.m_itemInfo.Item_BoneOffset.Split(new char[1] { ',' });
            m_offset = new Vector3(float.Parse(param[0]), float.Parse(param[1]), float.Parse(param[2]));
        }
    }
	void OnTriggerEnter(Collider other)
    {
        if (!ActorTargetManager.IsTrigger(other))
        {
            return;
        }
        try
        {
            if (m_info.m_isNeedRemove)
            {
                return;
            }
            Transform targetObj = other.transform;
            while (null != targetObj && targetObj.name != "body")
            {
                targetObj = targetObj.parent;
            }
            if (null == targetObj)
            {
                return;
            }
            ActorProp actorProp = targetObj.parent.GetComponent<ActorProp>();
            if (null == actorProp)
            {
                Debug.LogWarning("trigger return, actorProp get failed");
                return;
            }
            Actor target = actorProp.ActorLogicObj;
            if (target == null || target.IsDead)
            {
                return;
            }
            if (m_info.m_itemInfo.IsOnlyForChosenTarget)
            {//只对选中目标生效
                if (target != m_info.m_dstActor)
                {//不是选中目标
                    return;
                }
            }
            if (m_info.m_bouncyTargetIDList != null)
            {//反弹中
                if (target != m_info.m_dstActor)
                {//不是选中目标
                    return;
                }
            }
            SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(m_info.m_itemInfo.Item_ResultID);
            if (info == null)
            {
                //Debug.LogWarning("trigger return, SkillResultInfo is null, result id is " + m_info.m_itemInfo.Item_ResultID);
                return;
            }
            switch ((ENResultTargetType)info.ResultTargetType)
            {
                case ENResultTargetType.enEnemySingle:
                case ENResultTargetType.enEnemyAll:
                    {//作用于enemy
                        if (!ActorTargetManager.IsEnemy(m_info.m_actorCamp, m_info.m_actorType, target))
                        {
                            return;
                        }
                    }
                    break;
                case ENResultTargetType.enFriendlySingle:
                case ENResultTargetType.enFriendlyAll:
                    {//作用于friendly
                        if (!ActorTargetManager.IsFriendly(m_info.m_actorCamp, m_info.m_actorType, target))
                        {
                            return;
                        }
                    }
                    break;
                case ENResultTargetType.enEveryone:
                    break;
                case ENResultTargetType.enSelf:
                    if (m_info.m_actorType != target.Type)
                    {
                        return;
                    }
                    break;
                case ENResultTargetType.enFriendlyAllAndSelf:
                case ENResultTargetType.enFriendlySingleAndSelf:
                    {
                        if (!ActorTargetManager.IsFriendly(m_info.m_actorCamp, m_info.m_actorType, target) && m_info.m_actorType != target.Type)
                        {
                            return;
                        }
                    }
                    break;
                default:
                    return;
            }
            if (m_targetIDList.Contains(target.ID))
            {
                if (!m_info.m_itemInfo.IsMultiResult)
                {//不对目标多次生效
                    return;
                }
            }
            else
            {
                m_targetIDList.Add(target.ID);
            }

            GlobalEnvironment.Singleton.IsInCallbackOrTrigger = true;

            IResult r = BattleFactory.Singleton.CreateResult(ENResult.Skill, m_info.m_srcActor.ID, target.ID,
                m_info.m_itemInfo.Item_ResultID, m_info.m_skillID);
            if (r != null)
            {
                target.SetBlastPos(m_info.m_itemObj.transform.position, m_info.m_itemObj.transform.forward);
                r.ResultExpr(null);
                BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
            }
            if (m_info.m_itemInfo.IsRemoveAfterResult)
            {
                m_info.m_isNeedRemove = true;
            }

            //播放击中声音
            if (!string.IsNullOrEmpty(info.SoundList))
            {
                string[] param = info.SoundList.Split(new char[1] { ',' });
                string sound = param[0];
                if (!string.IsNullOrEmpty(sound))
                {
#if UNITY_IPHONE || UNITY_ANDRIOD
#else
                    AudioClip aClip = PoolManager.Singleton.LoadSound(sound);
                    if (aClip != null)
                    {
                        AudioSource.PlayClipAtPoint(aClip, gameObject.transform.position);
                    }
#endif
                }
                else
                {
                    Debug.LogWarning("sound string is null");
                }
            }
            RemoteAttackManager.Singleton.StartBouncy(m_info, target.MainPos);
        }
        catch (Exception e)
        {
            Debug.LogError("Error On OnTriggerEnter" + e.Message + ",,,,Stack:" + e.StackTrace.ToString());
        }
        GlobalEnvironment.Singleton.IsInCallbackOrTrigger = false;
	}
    void Update()
    {
        if (m_info != null)
        {
            if (m_info.m_isLaunchSuspend)
            {//延缓发射
                if (m_info.m_itemInfo.IsFollowCreator)
                {//跟随创建者
                    Vector3 targetPos = m_info.m_boneT.position;
                    targetPos = targetPos + m_info.m_boneT.rotation * m_offset;

                    m_info.m_itemObj.transform.position = targetPos;
                }
                //获取目标
                m_range = m_info.m_itemInfo.Item_MonitorArea;
                m_target = null;
                ActorManager.Singleton.ForEach(CheckTarget);
                if (m_target != null)
                {
                    m_info.m_dstActor = m_target;
                    m_info.m_targetPos = m_target.RealPos;
                    if (m_info.m_itemInfo.Item_IsLockY)
                    {
                        m_info.m_targetPos.y = m_info.m_itemObj.transform.position.y;
                    }
                    Vector3 direction = m_info.m_targetPos - m_info.m_itemObj.transform.position;
                    direction.Normalize();
                    m_info.m_forward = direction;
                    m_info.m_isLaunchSuspend = false;
                }
            }
        }
    }
    Actor m_target = null;
    float m_range = 0;
    void CheckTarget(Actor target)
    {
        if (m_target != null)
        {
            return;
        }
        if (target.IsDead)
        {
            return;
        }
        if (ActorTargetManager.IsEnemy(m_info.m_actorCamp, m_info.m_actorType, target))
        {
            float d = ActorTargetManager.GetTargetDistance(m_info.m_itemObj.transform.position, target.RealPos);
            if (d < m_range)
            {
                m_target = target;
            }
        }
    }
};