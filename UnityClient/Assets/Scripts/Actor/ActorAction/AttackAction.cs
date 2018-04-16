//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Actor\ActorAction
//	created:	2013-4-16
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//攻击
public class AttackAction : ActorAction
{
	public override ENType GetActionType() { return ENType.enAttackAction; }
    public static ENType SGetActionType() { return ENType.enAttackAction; }
    public class AttackStepInfo
    {
        public Actor.ENSkillStepType m_skillType { get; private set; }
        public string m_animName { get; private set; }
        public float m_animTime { get; private set; }

        public AttackStepInfo(Actor.ENSkillStepType type, string name, float time)
        {
            m_skillType = type;
            m_animName = name;
            m_animTime = time;
        }
    }
    //SkillResultID的索引（技能和技能result的关系为：n为技能id，技能resultID为(n-1)*10+1到(n-1)*10+10）
    public int m_skillResultIDIndex = 0;
    //SkillResultID
    public int SkillResultID
    {
        get
        {
            int index = m_skillResultIDIndex;
            if (m_skillResultIDIndex == 0)
            {
                index = 1;
            }
            if (m_skillID > 0)
            {
                return (m_skillID - 1) * 10 + index;
            }
            return -1;
        }
    }
    //需要trigger的SkillResultID
	//public int m_triggerSkillResultID { get; set; }
    //不需要trigger的SkillResultID
    //public int m_instantSkillResultID { get; set; }
    //技能id
    public int m_skillID { get; protected set; }
    //m_skillID对应的skillinfo
    public SkillInfo m_skillInfo { get; protected set; }
    //技能目标id
    public int m_skillTargetID { get; set; }
    //技能目标
    public Actor m_skillTarget { get { return ActorManager.Singleton.Lookup(m_skillTargetID); } }
    //技能目标id列表
    List<int> SkillTargetIDList { get; set; }
    //单体第一个目标
    public Actor m_firstTarget { get; set; }

    //是否完成
    public bool IsFinished { get; set; }

    //m_animationNameList的key
	private int m_curAttackStep { get; set; }
    //当前攻击动作的步骤
    private Dictionary<int, AttackStepInfo> m_animationNameList { get; set; }
    public virtual void Init(int skillID, int skillTargetID)
    {
        InitImpl(skillID, skillTargetID, false, Vector3.zero);
    }
    public virtual void InitImpl(int skillID, int skillTargetID, bool isSyncPosValidate, Vector3 syncPos)
	{
		m_skillID = skillID;
        m_skillTargetID = skillTargetID;
        m_firstTarget = null;
        if (m_animationNameList == null)
        {
            m_animationNameList = new Dictionary<int, AttackStepInfo>();
        }
        else
        {
            m_animationNameList.Clear();
        }
        
        m_skillInfo = GameTable.SkillTableAsset.Lookup(m_skillID);
        if (null != m_skillInfo && m_skillInfo.BuffIDList.Count != 0)
        {
            IResult r = BattleFactory.Singleton.CreateResult(ENResult.AddBuff, CurrentActor.ID, CurrentActor.ID, 0, 0, m_skillInfo.BuffIDList.ToArray());
            if (r != null)
            {
                r.ResultExpr(m_skillInfo.BuffIDList.ToArray());
                BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
            }
        }
        if (m_skillInfo.TargetNumber > 0)
        {
            if (SkillTargetIDList == null)
            {
                SkillTargetIDList = new List<int>();
            }
            else
            {
                SkillTargetIDList.Clear();
            }
            if (m_skillTarget != null && !m_skillTarget.IsDead)
            {
                SkillTargetIDList.Add(skillTargetID);
            }
        }
        if (CurrentActor.m_isDeposited)
        {//托管中，发送attack action的消息
            Vector3 pos = CurrentActor.MainPos;
            IMiniServer.Singleton.SendAction_Attack_C2BS(CurrentActor.ID, skillID, skillTargetID, pos.x, pos.z);
        }
	}
    float m_closestDistance = float.MaxValue;
    int m_closestTargetID = 0;
    void CheckTarget(Actor target)
    {
        if (SkillTargetIDList.Contains(target.ID))
        {
            return;
        }
        if (target.IsDead)
        {
            return;
        }
        float d = ActorTargetManager.GetTargetDistance(CurrentActor.RealPos, target.RealPos);
        if (m_skillInfo.AttackDistance >= d)
        {
            switch ((ENTargetType)m_skillInfo.TargetType)
            {
                case ENTargetType.enEnemy:
                    {
                        if (!ActorTargetManager.IsEnemy(CurrentActor, target))
                        {
                            return;
                        }
                    }
                    break;
                case ENTargetType.enFriendly:
                    {
                        if (!ActorTargetManager.IsFriendly(CurrentActor, target))
                        {
                            return;
                        }
                    }
                    break;
                case ENTargetType.enSelf:
                    {
                        if (CurrentActor != target)
                        {
                            return;
                        }
                    }
                    break;
                case ENTargetType.enNullTarget:
                    break;
                case ENTargetType.enInteraction:
                    {
                        if (!ActorTargetManager.IsInteractionally(CurrentActor, target))
                        {
                            return;
                        }
                    }
                    break;
                case ENTargetType.enFriendlyAndSelf:
                    {
                        if (!ActorTargetManager.IsFriendly(CurrentActor, target) && CurrentActor != target)
                        {
                            return;
                        }
                    }
                    break;
                default:
                    return;
            }
            if (d < m_closestDistance)
            {
                m_closestTargetID = target.ID;
            }
        }
    }
    
    public AttackStepInfo GetAttackStepInfo()
    {
        AttackStepInfo info = null;
        m_animationNameList.TryGetValue(m_curAttackStep, out info);
        if (info == null)
        {
            IsFinished = true;
        }
        return info;
    }
    
    public string GetAnimationName()
    {
        string animName = "";
        AttackStepInfo info = GetAttackStepInfo();
        if (null != info)
        {
            animName = info.m_animName;
        }
        return animName;
    }
    public bool m_isPlay = false;
//    float m_startTime = 0f;
    bool m_isSwitchRotation = false;
//    float m_durationTime = 0f;
    Vector3 m_targetForward = Vector3.zero;
    float m_fRotateSpeed = 0f;
	public override void OnEnter()
    {
        SyncPositionIfNeed();
        m_skillResultIDIndex = 0;
        if (null == m_skillInfo)
        {
            IsEnable = false;
            Debug.LogWarning("data error,skillID:" + m_skillID);
            return;
        }
        if (CurrentActor.Type == ActorType.enMain)
        {
            BattleArena.Singleton.SkillCombo.SetConnectionInfo((ENSkillConnectionType)m_skillInfo.SkillConnectionType);
            //if (m_skillInfo.SkillConnectionType != (int)ENSkillConnectionType.enNone &&
            //    m_skillInfo.SkillConnectionType != (int)ENSkillConnectionType.enNormal)
            //{
            //    BattleArena.Singleton.QTE.Succ(m_skillInfo.ID, (ENSkillConnectionType)m_skillInfo.SkillConnectionType);
            //}
        }
        if (m_skillInfo.TargetType == (int)ENTargetType.enNullTarget)
        {//无目标技能
            CurrentActor.StartCaution();
        }
        {//动作播放列表
            if (m_skillInfo.IsPrepareExist)
            {//有起手动作
                m_animationNameList.Add(m_animationNameList.Count, new AttackStepInfo(Actor.ENSkillStepType.enPrepare, m_skillInfo.PrepareMotion, 0));
            }
            if (m_skillInfo.IsSpellExist)
            {//有吟唱动作
                m_animationNameList.Add(m_animationNameList.Count, new AttackStepInfo(Actor.ENSkillStepType.enSpell, m_skillInfo.SpellMotion, m_skillInfo.SpellTime));
            }
            if (m_skillInfo.IsSlashExist)
            {//有冲刺动作
                m_animationNameList.Add(m_animationNameList.Count, new AttackStepInfo(Actor.ENSkillStepType.enSlash, m_skillInfo.SlashMotion, 0));
            }
            if (m_skillInfo.IsReleaseExist)
            {//有释放动作
                m_animationNameList.Add(m_animationNameList.Count, new AttackStepInfo(Actor.ENSkillStepType.enRelease, m_skillInfo.ReleaseMotion, 0));
            }
            if (m_skillInfo.IsConductExist)
            {//有引导动作
                m_animationNameList.Add(m_animationNameList.Count, new AttackStepInfo(Actor.ENSkillStepType.enConduct, m_skillInfo.ConductMotion, m_skillInfo.ConductTime));
            }
            if (m_skillInfo.IsEndConductExist)
            {//有引导结束动作
                m_animationNameList.Add(m_animationNameList.Count, new AttackStepInfo(Actor.ENSkillStepType.enEndConduct, m_skillInfo.EndConductMotion, 0));
            }
            m_curAttackStep = 0;
            RefreshActionRef();
            if (m_skillTarget != null && m_skillTarget != CurrentActor)
            {
                m_isSwitchRotation = true;
                if (CurrentActor.Type == ActorType.enMain)
                {
                    m_isSwitchRotation = true;
                }
                else
                {
                    m_isSwitchRotation = false;
                }
            }
            
        }
		{
            AttackActionCallback callback = CurrentActor.GetBodyParentObject().GetComponent<AttackActionCallback>();
			callback.enabled = true;
		}

		AnimationConfig animConfig = CurrentActor.MainAnim.GetComponent<AnimationConfig>();
		if (null != animConfig && null != animConfig.Trail)
		{
			animConfig.Trail.StartTrail(0.15f, 0.05f);
		}
        if (m_isSwitchRotation)
        {
            //m_startTime = Time.time;
            m_isPlay = false;
            m_targetForward = m_skillTarget.RealPos - CurrentActor.RealPos;
            m_targetForward.y = 0;
            m_targetForward.Normalize();
            float currotateSpeed = Vector3.Angle(CurrentActor.MainObj.transform.forward, m_targetForward);
            if (CurrentActor.Type == ActorType.enNPC)
            {
                NPC npc = CurrentActor as NPC;
                m_fRotateSpeed = npc.CurrentTableInfo.SwitchRotateSpeed;
            }
            else
            {
                m_fRotateSpeed = CurrentActor.PropConfig.SwitchRotateSpeed;
            }
            float fDuration = currotateSpeed / m_fRotateSpeed;
            if (fDuration > GameSettings.Singleton.m_attackRotateTime)
            {
                m_fRotateSpeed = GameSettings.Singleton.m_attackRotateSpeed;
            }
        }
        else
        {
            m_isPlay = true;
            if (m_skillTarget != null && m_skillTarget != CurrentActor)
            {//面向目标
                Vector3 direction = m_skillTarget.RealPos - CurrentActor.RealPos;
                direction.y = 0;
                direction.Normalize();
                CurrentActor.MoveRotation(Quaternion.LookRotation(direction.normalized) * Quaternion.LookRotation(Vector3.forward));
            }
        }
	}
	public override void OnInterupt()
    {
        CurrentActor.MainAnim.Stop();
        RemoveAllEffect();
        OnExit();
        if (CurrentActor.Type == ActorType.enMain)
        {
            //camera back
            MainGame.Singleton.MainCamera.BackToActor(Time.deltaTime);
            MainGame.Singleton.MainCamera.StopCameraCoverLightChange();
        }
        if (CurrentActor.m_isDeposited)
        {//托管中，发送action被打断的消息
            IMiniServer.Singleton.SendActionInterupt_C2BS(CurrentActor.ID, (int)GetActionType());
        }
	}

    public override void OnExit()
    {
        AnimationConfig animConfig = CurrentActor.MainAnim.GetComponent<AnimationConfig>();
        if (null != animConfig && null != animConfig.Trail)
        {
            animConfig.Trail.ClearTrail();
        }
        {
            AttackActionCallback callback = CurrentActor.GetBodyParentObject().GetComponent<AttackActionCallback>();
            callback.enabled = false;
        }
        if (m_skillTarget != null && m_skillTarget != CurrentActor)
        {//面向目标
            Vector3 direction = m_skillTarget.RealPos - CurrentActor.RealPos;
            direction.y = 0;
            // 由于这里修改旋转，会导致Actor的RealPos不正确，所以这里不能乱转 [8/19/2014 tgame]
            //CurrentActor.MoveRotation(Quaternion.LookRotation(direction.normalized) * Quaternion.LookRotation(Vector3.forward));
        }
        m_effectObjList.Clear();
        if (m_skillInfo.IsSlashExist)
        {//显示BoxCollider
            if (CurrentActor.CenterCollider != null)
            {
                CurrentActor.CenterCollider.gameObject.layer = LayerMask.NameToLayer("Actor");
            }
        }
        if (null != CurrentActor.Combo)
        {//设置连接技的信息
            CurrentActor.Combo.SetConnetionInfo(m_skillInfo.SkillConnectionType);
        }

        StopPlaySkillEventAnimation();
        //隐藏body下的所有istrigger状态的collider
        CurrentActor.HideTriggerCollider();

        //停止播放所有武器动作
        CurrentActor.StopPlayWeaponsAnimation();
        //隐藏武器
        CurrentActor.ShowWeaponModelWithTable(false);
        //回馈位移动画
        CurrentActor.ApplyAnimationOffset();
	}

    private float m_animBeginTimer = 0;//动画开始的时间
    private float m_animDuration = 0;//动画所用时间
    //private Vector3 m_slashVelocity = Vector3.zero;//冲刺的速度
    //private void ActionToSlash()
    //{
    //    if (m_animBeginTimer == 0)
    //    {//添加特效
    //        m_animBeginTimer = Time.time;
    //        AddEffect(m_skillInfo.SlashEffectList, m_skillInfo.SlashEffectLocationList);
    //        //隐藏BoxCollider
    //        CurrentActor.CenterCollider.gameObject.layer = LayerMask.NameToLayer("DisableCollider");
    //        //朝目标移动
    //        Vector3 direction = m_skillTarget.RealPos - CurrentActor.MainPos;
    //        direction.y = 0;
    //        m_animDuration = (direction.magnitude + 1) / m_skillInfo.SlashMotionSpeed;
    //        direction.Normalize();
    //        m_slashVelocity = direction * m_skillInfo.SlashMotionSpeed * Time.deltaTime;
    //        CurrentActor.MainPos += m_slashVelocity;
    //    }
    //    else
    //    {
    //        if (Time.time - m_animBeginTimer >= m_animDuration)
    //        {
    //            RemoveEffect(m_skillInfo.SlashDestroyEffectList);
    //            //显示BoxCollider
    //            CurrentActor.CenterCollider.gameObject.layer = LayerMask.NameToLayer("Actor");
    //            ++m_curAttackStep;
    //            if (m_animationNameList.Count <= m_curAttackStep)
    //            {//所有动作播放完毕
    //                IsFinished = true;
    //            }
    //            else
    //            {
    //                m_animBeginTimer = 0;
    //                m_animDuration = 0;
    //                RefreshActionRef();
    //            }
    //        }
    //        else
    //        {
    //            CurrentActor.MainPos += m_slashVelocity;
    //        }
    //    }
    //}
    private void ActionToSpell()
    {
        if (m_animBeginTimer == 0)
        {//添加特效
            AddEffect(m_skillInfo.SpellEffectList, m_skillInfo.SpellEffectLocationList);
            m_animBeginTimer = Time.time;
            m_animDuration = m_skillInfo.SpellTime;
        }
        else
        {
            if (Time.time - m_animBeginTimer >= m_animDuration)
            {
                RemoveEffect(m_skillInfo.SpellDestroyEffectList);
                ++m_curAttackStep;
                if (m_animationNameList.Count <= m_curAttackStep)
                {//所有动作播放完毕
                    IsFinished = true;
                }
                else
                {
                    m_animBeginTimer = 0;
                    m_animDuration = 0;
                    RefreshActionRef();
                }
            }
        }
    }
    private void ActionToConduct()
    {
        if (m_animBeginTimer == 0)
        {//添加特效
            AddEffect(m_skillInfo.ConductEffectList, m_skillInfo.ConductEffectLocationList);
            m_animBeginTimer = Time.time;
            m_animDuration = m_skillInfo.ConductTime;
        }
        else
        {
            if (Time.time - m_animBeginTimer >= m_animDuration)
            {
                RemoveEffect(m_skillInfo.ConductDestroyEffectList);
                ++m_curAttackStep;
                if (m_animationNameList.Count <= m_curAttackStep)
                {//所有动作播放完毕
                    IsFinished = true;
                }
                else
                {
                    m_animBeginTimer = 0;
                    m_animDuration = 0;
                    RefreshActionRef();
                }
            }
        }
    }
    float RotateForwardTarget()
    {
        Vector3 vForward = new Vector3(CurrentActor.MainObj.transform.forward.x, 0f, CurrentActor.MainObj.transform.forward.z);
        float fCurForwardAngle = ActorBlendAnim.HorizontalAngle(vForward);
        float fTargetAngle = ActorBlendAnim.HorizontalAngle(m_targetForward);

        float fLerp = Mathf.LerpAngle(fCurForwardAngle, fTargetAngle, Time.deltaTime * m_fRotateSpeed);
        float fInterval = fLerp - fCurForwardAngle;
        Quaternion quater = Quaternion.Euler(0f, fInterval, 0f);
        CurrentActor.MainObj.transform.rotation = quater * CurrentActor.MainObj.transform.rotation;
        return fInterval;
    }
	public override bool OnUpdate()
	{
        if (m_isSwitchRotation)
        {
            float fInterval = RotateForwardTarget();
            if (Mathf.Abs(fInterval) <= GameSettings.Singleton.m_attackRotateMinAngle)
            {
                m_isPlay = true;
                m_isSwitchRotation = false;
                AnimStartTime = Time.time;
                AnimLength = float.MaxValue;
                RefreshActionRef();
            }
            else
            {
                return false;
            }
        }
        if (!IsFinished)
        {
            PlaySkillEventAnimation();
            PlayMissingSound();
            //if (m_skillInfo.IsStopAttck)
            //{//如果目标死亡，且不在主动僵直中，则停止攻击
            //    if (m_skillTarget != null && m_skillTarget.IsDead)
            //    {//目标死亡
            //        if (!CurrentActor.ActionControl.IsActionRunning(ActorAction.ENType.enSelfSpasticityAction))
            //        {//不在主动僵直中
            //            return true;
            //        }
            //    }
            //}
            if (m_skillInfo.IsCanMove)
            {//移动攻击技能，改为移动攻击action
                AttackStepInfo info = GetAttackStepInfo();
                if (info != null)
                {
                    if (m_skillInfo.IsConductExist && info.m_animName == m_skillInfo.ConductMotion)
                    {//引导动作
                        if (CurrentActor.ActionControl.IsActionRunning(AttackingMoveAction.SGetActionType()))
                        {
                            return false;
                        }
                        int skillID = m_skillID, targetID = m_skillTargetID;
                        AttackingMoveAction action = CurrentActor.ActionControl.AddAction(AttackingMoveAction.SGetActionType()) as AttackingMoveAction;
                        if (action != null)
                        {
                            action.Init(skillID, targetID);
                            return false;
                        }
                    }
                }
            }
            if (Time.time - AnimStartTime > AnimLength)
            {//当前动画播放完毕
                //回馈位移动画
                CurrentActor.ApplyAnimationOffset();
                AttackStepInfo info = GetAttackStepInfo();
                if (info != null)
                {//当前有动画在播放
                    if (m_skillInfo.IsSpellExist && info.m_animName == m_skillInfo.SpellMotion)
                    {//吟唱动作
                        ActionToSpell();
                    }
                    //else if (m_skillInfo.IsSlashExist && info.m_animName == m_skillInfo.SlashMotion)
                    //{//冲刺动作
                    //    //ActionToSlash();
                    //}
                    else if (m_skillInfo.IsConductExist && info.m_animName == m_skillInfo.ConductMotion)
                    {//引导动作
                        if (m_skillInfo.IsCanMove)
                        {//移动攻击技能，改为移动攻击action
                            int skillID = m_skillID, targetID = m_skillTargetID;
                            AttackingMoveAction action = CurrentActor.ActionControl.AddAction(AttackingMoveAction.SGetActionType()) as AttackingMoveAction;
                            if (action != null)
                            {
                                action.Init(skillID, targetID);
                                return false;
                            }
                        }
                        else
                        {
                            ActionToConduct();
                        }
                    }
                    else
                    {
                        if (Time.time - AnimStartTime < info.m_animTime)
                        {//播放时间未到，继续播放
                            RefreshActionRef();
                            return false;
                        }
                        if (info.m_animName == m_skillInfo.ReleaseMotion)
                        {//释放动作
                            if (m_skillInfo.TargetNumber > 0)
                            {//对多个目标进行释放动作
                                if (m_skillInfo.TargetNumber > SkillTargetIDList.Count)
                                {
                                    m_closestDistance = float.MaxValue;
                                    m_closestTargetID = 0;
                                    ActorManager.Singleton.ForEach(CheckTarget);
                                    if (m_closestTargetID != 0)
                                    {//查找到最近的目标
                                        m_skillTargetID = m_closestTargetID;
                                        CurrentActor.CurrentTarget = m_skillTarget;
                                        RefreshActionRef();
                                        //因为要播放同一技能步骤下的动作，所以把m_currentType重置
                                        m_currentType = Actor.ENSkillStepType.enNone;
                                        //加入到技能目标id列表中
                                        SkillTargetIDList.Add(m_skillTargetID);
                                        return false;
                                    }
                                }
                            }
                        }
                        ++m_curAttackStep;
                        if (m_animationNameList.Count <= m_curAttackStep)
                        {//所有动作播放完毕
                            IsFinished = true;
                        }
                        else
                        {
                            m_animBeginTimer = 0;
                            m_animDuration = 0;
                            RefreshActionRef();
                        }
                    }
                }
                else
                {
                    IsFinished = true;
                }
            }
        }
        if (!CurrentActor.MainRigidBody.isKinematic)
        {
            CurrentActor.MainRigidBody.velocity = Vector3.zero;
        }
        return IsFinished;
	}
    Dictionary<string, GameObject> m_effectList = new Dictionary<string, GameObject>();
    private void AddEffect(List<string> effectNameList, List<string> boneList)
    {
        for (int i = 0; i < effectNameList.Count; ++i)
        {
            MainGame.Singleton.StartCoroutine(Coroutine_Load(effectNameList[i], 0, boneList[i], true, Vector3.zero));
            //GameObject effectObj = CurrentActor.PlayEffect(effectNameList[i], 0, boneList[i], true, Vector3.zero);
            //if (effectObj == null)
            //{
            //    continue;
            //}
            //GameObject obj = null;
            //if (!m_effectList.TryGetValue(effectNameList[i], out obj))
            //{
            //    m_effectList.Add(effectNameList[i], effectObj);
            //}
        }
    }
    IEnumerator Coroutine_Load(string effectName, float effectTime, string posByBone, bool isAdhered, Vector3 offset)
    {
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath(effectName), data);
        while (true)
        {
            e.MoveNext();
            if (data.m_isFinish)
            {
                break;
            }
            yield return e.Current;
        }
        if (data.m_obj != null)
        {
            CurrentActor.PlayEffect(data.m_obj as GameObject, effectTime, posByBone, isAdhered, offset);
            if (!m_effectList.ContainsKey(effectName))
            {
                m_effectList.Add(effectName, data.m_obj as GameObject);
            }
        }
    }
    private void RemoveEffect(List<string> effectNameList)
    {
        for (int i = 0; i < effectNameList.Count; ++i)
        {
            GameObject obj = null;
            if (m_effectList.TryGetValue(effectNameList[i], out obj))
            {
                obj.SetActive(false);
                //PoolManager.Singleton.ReleaseObj(obj);
            }
            else
            {
                Transform boneT = CurrentActor.LookupBone(CurrentActor.MainObj.transform, effectNameList[i]);
                if (boneT)
                {
                    boneT.gameObject.SetActive(false);
                    //PoolManager.Singleton.ReleaseObj(boneT.gameObject);
                }
            }
        }
    }
    private void RemoveAllEffect()
    {
        RemoveEffect(m_skillInfo.PrepareDestroyEffectList);
        RemoveEffect(m_skillInfo.SpellDestroyEffectList);
        RemoveEffect(m_skillInfo.SlashDestroyEffectList);
        RemoveEffect(m_skillInfo.ReleaseDestroyEffectList);
        RemoveEffect(m_skillInfo.ConductDestroyEffectList);
        RemoveEffect(m_skillInfo.EndConductDestroyEffectList);
        for (int i = 0; i < m_effectObjList.Count; ++i)
        {
            if (m_effectObjList[i] != null)
            {
                m_effectObjList[i].SetActive(false);
            }
        }
    }

	public override void Reset()
	{
		base.Reset();
		m_skillID = 0;
		//m_triggerSkillResultID = 0;
        //m_instantSkillResultID = 0;
        m_curAttackStep = -1;
        if (m_animationNameList != null)
        {
            m_animationNameList.Clear();
        }
        m_animBeginTimer = 0;
        m_animDuration = 0;
        //m_slashVelocity = Vector3.zero;
        IsFinished = false;
        m_isSwitchRotation = false;
        m_isMissing = true;
        m_currentType = Actor.ENSkillStepType.enNone;
        m_attackTriggerState = ENAttackTriggerState.enNone;
        m_isTriggerSucced = false;
	}
	public void OnTriggerEnter(GameObject selfObj, Collider other)
	{
		if (other.isTrigger)
		{
            //Debug.LogWarning("OnTriggerEnter other.isTrigger is true");
			return;
		}
        Actor target = null;
        Transform targetObj = other.transform;
        while (null != targetObj && targetObj.name != "body")
        {
            targetObj = targetObj.parent;
        }
        if (null == targetObj)
        {
            //Debug.LogWarning("OnTriggerEnter target obj is null");
            return;
        }
        ActorProp prop = targetObj.parent.GetComponent<ActorProp>();
        target = prop.ActorLogicObj;
        if (null == target)
        {
            Debug.LogWarning("OnTriggerEnter target is null");
            return;
        }
        if (target.IsDead)
        {
            //Debug.LogWarning("OnTriggerEnter target is deaded");
            return;
        }
        //Debug.LogWarning("OnTriggerEnter target id is " +target.ID.ToString());
        SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(SkillResultID);
        if (info == null)
        {
            Debug.LogError("SkillResultID is null, actor ID:" + CurrentActor.ID + "SkillResultID:,"  + SkillResultID + ",skillID:" + m_skillID + ",index:" + m_skillResultIDIndex);
            DebugLog.Singleton.OnShowLog("SkillResultID is null, actor ID:" + CurrentActor.ID +  "SkillResultID:" + SkillResultID + ",skillID:" + m_skillID + ",index:" + m_skillResultIDIndex);
            return;
        }
		ActorProp selfProp = selfObj.transform.parent.GetComponent<ActorProp>();
        Actor self = selfProp.ActorLogicObj;
        switch ((ENResultTargetType)info.ResultTargetType)
        {
            case ENResultTargetType.enEnemySingle:
                {//只作用于第一个enemy
                    if (m_skillTarget != null)
                    {
                        if (m_skillTarget != target)
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (m_firstTarget != null)
                        {
                            return;
                        }
                        if (!ActorTargetManager.IsEnemy(self, target))
                        {
                            return;
                        }
                        m_firstTarget = target;
                    }
                }
                break;
            case ENResultTargetType.enEnemyAll:
                {//作用于所有enemy
                    if (!ActorTargetManager.IsEnemy(self, target))
                    {
                        return;
                    }
                }
                break;
            case ENResultTargetType.enFriendlySingle:
                {//只作用于第一个friendly
                    if (m_skillTarget != null)
                    {
                        if (m_skillTarget != target)
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (m_firstTarget != null)
                        {
                            return;
                        }
                        if (!ActorTargetManager.IsFriendly(self, target))
                        {
                            return;
                        }
                        m_firstTarget = target;
                    }
                }
                break;
            case ENResultTargetType.enFriendlyAll:
                {//作用于所有friendly
                    if (!ActorTargetManager.IsFriendly(self, target))
                    {
                        return;
                    }
                }
                break;
            case ENResultTargetType.enEveryone:
                break;
            case ENResultTargetType.enSelf:
                {
                    if (self != target)
                    {
                        return;
                    }
                }
                break;
            case ENResultTargetType.enFriendlyAllAndSelf:
                {
                    if (!ActorTargetManager.IsFriendly(self, target) && self != target)
                    {
                        return;
                    }
                }
                break;
            case ENResultTargetType.enFriendlySingleAndSelf:
                {
                    if (!ActorTargetManager.IsFriendly(self, target) && self != target)
                    {
                        return;
                    }
                    if (m_firstTarget != null)
                    {
                        return;
                    }
                    m_firstTarget = target;
                }
                break;
            default:
                return;
        }
        
		if (ClientNet.Singleton.IsConnected)
		{
            IResult r = BattleFactory.Singleton.CreateResult(ENResult.Skill, self.ID, target.ID,
                SkillResultID, m_skillInfo != null ? m_skillInfo.ID : 0);
            if (r != null)
            {
                target.SetBlastPos(self.RealPos, self.GetBodyObject().transform.forward);
                r.ResultExpr(null);
                BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
            }
		}
        else
        {
            IResult r = BattleFactory.Singleton.CreateResult(ENResult.Skill, self.ID, target.ID,
                SkillResultID, m_skillInfo != null ? m_skillInfo.ID : 0);
            if (r != null)
            {
                target.SetBlastPos(self.RealPos, self.GetBodyObject().transform.forward);
                r.ResultExpr(null);
                BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
            }
		}
        if (!string.IsNullOrEmpty(info.SoundList))
        {//播放击中声音
            string[] param = info.SoundList.Split(new char[1] { ',' });
            string sound = param[0];
            if (!string.IsNullOrEmpty(sound))
            {
#if UNITY_IPHONE || UNITY_ANDRIOD
#else
                AudioClip aClip = PoolManager.Singleton.LoadSound(sound);
                if (aClip != null)
                {
                    AudioSource.PlayClipAtPoint(aClip, selfObj.transform.position);
                }
#endif
            }
            else
            {
                Debug.LogWarning("sound string is null");
            }
        }
        m_isTriggerSucced = true;
        m_isMissing = false;
	}
    public bool CreateSkillResult()
    {
        SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(SkillResultID);
        if (info == null)
        {
            Debug.LogError("SkillResultID is null, SkillResultID:" + SkillResultID + ",skillID:" + m_skillID + ",index:" + m_skillResultIDIndex);
            return false;
        }

        CurrentActor.SelfAI.GetRangeTargetList(ENTargetType.enNone, info.InstantRange);
        List<int> resultActorIDList = new List<int>();
        float minDistance = float.MaxValue;
        int tempActorID = 0;
        foreach (var targetID in CurrentActor.SelfAI.m_targetIDList)
        {
            Actor target = ActorManager.Singleton.Lookup(targetID);
            if (target == null || target.IsDead)
            {
                continue;
            }
            switch ((ENResultTargetType)info.ResultTargetType)
            {
                case ENResultTargetType.enEnemySingle:
                    {//作用于单个enemy
                        if (m_skillTarget != null && ActorTargetManager.IsEnemy(CurrentActor, m_skillTarget))
                        {//有技能目标,技能目标是enemy
                            if (m_skillTarget == target)
                            {
                                tempActorID = m_skillTarget.ID;
                            }
                        }
                        else
                        {
                            if (ActorTargetManager.IsEnemy(CurrentActor, target))
                            {
                                Vector3 temp = target.RealPos - CurrentActor.MainPos;
                                temp.y = 0;
                                if (temp.magnitude < minDistance)
                                {
                                    minDistance = temp.magnitude;
                                    tempActorID = target.ID;
                                }
                            }
                        }
                    }
                    break;
                case ENResultTargetType.enEnemyAll:
                    {//作用于所有enemy
                        if (ActorTargetManager.IsEnemy(CurrentActor, target))
                        {
                            resultActorIDList.Add(target.ID);
                        }
                    }
                    break;
                case ENResultTargetType.enFriendlySingle:
                    {//作用于单个friendly
                        if (m_skillTarget != null && ActorTargetManager.IsFriendly(CurrentActor, m_skillTarget))
                        {//有技能目标,技能目标是friend
                            if (m_skillTarget == target)
                            {
                                tempActorID = m_skillTarget.ID;
                            }
                        }
                        else
                        {
                            if (ActorTargetManager.IsFriendly(CurrentActor, target))
                            {
                                Vector3 temp = target.RealPos - CurrentActor.MainPos;
                                temp.y = 0;
                                if (temp.magnitude < minDistance)
                                {
                                    minDistance = temp.magnitude;
                                    tempActorID = target.ID;
                                }
                            }
                        }
                    }
                    break;
                case ENResultTargetType.enFriendlyAll:
                    {//作用于所有friendly
                        if (ActorTargetManager.IsFriendly(CurrentActor, target))
                        {
                            resultActorIDList.Add(target.ID);
                        }
                    }
                    break;
                case ENResultTargetType.enEveryone:
                    {
                        resultActorIDList.Add(target.ID);
                    }
                    break;
                case ENResultTargetType.enSelf:
                    {
                        if (CurrentActor == target)
                        {
                            tempActorID = target.ID;
                        }
                    }
                    break;
                case ENResultTargetType.enFriendlyAllAndSelf:
                    {
                        if (ActorTargetManager.IsFriendly(CurrentActor, target) || CurrentActor == target)
                        {
                            resultActorIDList.Add(target.ID);
                        }
                    }
                    break;
                case ENResultTargetType.enFriendlySingleAndSelf:
                    {
                        if (m_skillTarget != null &&
                            (ActorTargetManager.IsFriendly(CurrentActor, m_skillTarget) || m_skillTarget == CurrentActor))
                        {
                            tempActorID = m_skillTarget.ID;
                        }
                        else
                        {//自己
                            if (CurrentActor == target)
                            {
                                tempActorID = target.ID;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        if (tempActorID != 0)
        {
            resultActorIDList.Add(tempActorID);
        }
        //if (ClientNet.Singleton.IsConnected)
        //{
        //}
        //else
        {
            foreach (var targetID in resultActorIDList)
            {
                IResult r = BattleFactory.Singleton.CreateResult(ENResult.Skill, CurrentActor.ID, targetID,
                    SkillResultID, m_skillInfo != null ? m_skillInfo.ID : 0);
                if (r != null)
                {
                    Actor targetActor = ActorManager.Singleton.Lookup(targetID);
                    targetActor.SetBlastPos(CurrentActor.RealPos, CurrentActor.GetBodyObject().transform.forward);
                    r.ResultExpr(null);
                    BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
                }
            }
        }
        return true;
    }
    private List<GameObject> m_effectObjList = new List<GameObject>();
    public void AddEffectObj(GameObject obj)
    {
        m_effectObjList.Add(obj);
    }

    public bool IsNormalAttack()
    {
        if (m_skillInfo != null)
        {
            return m_skillInfo.SkillType == (int)ENSkillType.enSkillNormalType;
        }
        return false;
    }
    //攻击是否落空
    bool m_isMissing = true;
    //攻击盒子开启
    bool m_isStartTrigger = false;
    //播放攻击落空声音
    protected void PlayMissingSound()
    {
        Collider c =  null;
        foreach (var item in CurrentActor.ColliderArray)
        {
            if (item.gameObject.activeSelf)
            {
                c = item;
                break;
            }
        }
        if (c != null)
        {
            m_isStartTrigger = true;
        }
        else
        {
            if (m_isStartTrigger)
            {
                m_isStartTrigger = false;
                if (m_isMissing)
                {//播放落空声音
                    SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(SkillResultID);
                    if (info == null)
                    {
                        Debug.LogWarning("skill result id is error,id:" + SkillResultID);
                        return;
                    }
                    if (!string.IsNullOrEmpty(info.SoundList))
                    {
                        string[] param = info.SoundList.Split(new char[1] { ',' });
                        if (param.Length < 2)
                        {
                            Debug.LogWarning("sound list is error, length is " + param.Length);
                            return;
                        }
                        string sound = param[1];
                        if (!string.IsNullOrEmpty(sound))
                        {
#if UNITY_IPHONE || UNITY_ANDRIOD
#else
                            AudioClip aClip = PoolManager.Singleton.LoadSound(sound);
                            if (aClip != null)
                            {
                                AudioSource.PlayClipAtPoint(aClip, CurrentActor.MainPos);
                            }
#endif
                        }
                        else
                        {
                            Debug.LogWarning("sound string is null");
                        }
                    }
                }
                m_isMissing = false;
            }
        }
    }
    Actor.ENSkillStepType m_currentType = Actor.ENSkillStepType.enNone;
    void PlaySkillEventAnimation()
    {
        AttackStepInfo info = GetAttackStepInfo();
        if (info == null || info.m_skillType == m_currentType)
        {
            return;
        }
        m_currentType = info.m_skillType;
        StopPlaySkillEventAnimation();
        if (info.m_skillType == Actor.ENSkillStepType.enSlash)
        {//冲锋有动作融合
            //计算冲刺的权重
            if (m_skillInfo.SlashMotionDistance > 0 && m_skillTarget != null)
            {
                Vector3 direction = m_skillTarget.RealPos - CurrentActor.RealPos;
                direction.y = 0f;
                float targetWeight = (direction.magnitude + m_skillInfo.SlashTargetPosOffset) / m_skillInfo.SlashMotionDistance;
                string blendAnimName = m_skillInfo.SlashBlendMotionName;
                if (targetWeight < 0)
                {
                    targetWeight = 0;
                }
                CurrentActor.PlaySkillEventAnimation(m_skillInfo.ID, (int)info.m_skillType, info.m_animName, Actor.ENAnimType.enBlend,
                    new float[] { (int)AnimationBlendMode.Blend, targetWeight, 0 });
                CurrentActor.PlaySkillEventAnimation(m_skillInfo.ID, (int)info.m_skillType + 1, blendAnimName, Actor.ENAnimType.enBlend,
                    new float[] { (int)AnimationBlendMode.Blend, 1 - targetWeight, 0 });
                return;
            }
        }
        else if (info.m_skillType == Actor.ENSkillStepType.enRelease)
        {//释放有动作融合
            if (m_skillInfo.ReleaseMotionDistance > 0 && m_skillTarget != null)
            {
                Vector3 direction = m_skillTarget.RealPos - CurrentActor.RealPos;
                direction.y = 0f;
                float targetWeight = (direction.magnitude + m_skillInfo.ReleaseTargetPosOffset) / m_skillInfo.ReleaseMotionDistance;
                string blendAnimName = m_skillInfo.ReleaseBlendMotionName;
                if (!string.IsNullOrEmpty(blendAnimName))
                {
                    if (targetWeight < 0)
                    {
                        targetWeight = 0;
                    }
                    CurrentActor.PlaySkillEventAnimation(m_skillInfo.ID, (int)info.m_skillType, info.m_animName, Actor.ENAnimType.enBlend,
                        new float[] { (int)AnimationBlendMode.Blend, targetWeight, 0 });
                    CurrentActor.PlaySkillEventAnimation(m_skillInfo.ID, (int)info.m_skillType + 1, blendAnimName, Actor.ENAnimType.enBlend,
                        new float[] { (int)AnimationBlendMode.Blend, 1 - targetWeight, 0 });
                    return;
                }
            }
        }
        CurrentActor.PlaySkillEventAnimation(m_skillInfo.ID, (int)info.m_skillType, info.m_animName, Actor.ENAnimType.enPlay);
    }
    void StopPlaySkillEventAnimation()
    {
        CurrentActor.StopPlaySkillEventAnimation();
    }
    enum ENAttackTriggerState
    {
        enNone,
        enBegin,
    }
    //攻击盒子的状态
    ENAttackTriggerState m_attackTriggerState = ENAttackTriggerState.enNone;
    bool m_isTriggerSucced = false;
    //停止无效的攻击动作
    public void StopInvalidAttack()
    {
        if (ENAttackTriggerState.enNone == m_attackTriggerState)
        {
            m_attackTriggerState = ENAttackTriggerState.enBegin;
        }
        else if (ENAttackTriggerState.enBegin == m_attackTriggerState)
        {
            //检测trigger是否生效
            if (!m_isTriggerSucced)
            {//trigger失败，停止AttackAction
                IsFinished = true;
            }
            m_isTriggerSucced = false;
            m_attackTriggerState = ENAttackTriggerState.enNone;
        }
    }
};