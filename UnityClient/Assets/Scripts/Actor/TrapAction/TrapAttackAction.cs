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
public class TrapAttackAction : TrapAction
{
    public override ENType GetActionType() { return ENType.enAttackAction; }
    public static ENType SGetActionType() { return ENType.enAttackAction; }
    public class AttackStepInfo
    {
        public string m_animName { get; private set; }
        public float m_animTime { get; private set; }

        public AttackStepInfo(string name, float time)
        {
            m_animName = name;
            m_animTime = time;
        }
    }
    //是否完成
    public bool IsFinished { get; set; }
    //技能ResultID
    public int mSkillResultID = -1;
    //m_animationNameList的key
    private int m_curAttackStep { get; set; }
    //当前攻击动作的步骤
    private Dictionary<int, AttackStepInfo> m_animationNameList { get; set; }

    Vector3 m_audioPos;
    //退出类型
    public enum ENExitType
    {
        enNone,
        enAction,
    }

    public List<string> mAnimNameList;
    public void Init(List<string> animNameList)
    {
        if (m_animationNameList == null)
        {
            m_animationNameList = new Dictionary<int, AttackStepInfo>();
        }
        else
        {
            m_animationNameList.Clear();
        }
        m_curAttackStep = 0;
        for (int i = 0; i < animNameList.Count; i++)
        {
            m_animationNameList.Add(m_animationNameList.Count, new AttackStepInfo(animNameList[i], 0.5f));
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
    public override string GetAnimationName()
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
    //float m_startTime = 0f;
    //bool m_isSwitchRotation = false;
    //float m_durationTime = 0f;
    //Vector3 m_targetForward = Vector3.zero;
    //float m_fRotateSpeed = 0f;
    public override void OnEnter()
    {//添加动画，设置初始数据
//         m_animationNameList.Add(m_animationNameList.Count, new AttackStepInfo("t-open-enter", 0.5f));
//         m_animationNameList.Add(m_animationNameList.Count, new AttackStepInfo("t-open-standby", 0.5f));
//         m_animationNameList.Add(m_animationNameList.Count, new AttackStepInfo("t-close-enter", 0.5f));
    }
    public override void OnInterupt(TrapAction.ENType newType)
    {

    }

    //private float m_animBeginTimer = 0;//动画开始的时间
    //private float m_animDuration = 0;//动画所用时间
    public override bool OnUpdate()
    {
        if (!IsFinished)
        {
            if (Time.time - AnimStartTime > AnimLength)
            {//当前动画播放完毕
                AttackStepInfo info = GetAttackStepInfo();
                if (info != null)
                {//当前有动画在播放
                    {
                        if (Time.time - AnimStartTime < info.m_animTime)
                        {//播放时间未到，继续播放
                            RefreshActionRef();
                            return false;
                        }
                        ++m_curAttackStep;
                        if (m_animationNameList.Count <= m_curAttackStep)
                        {//所有动作播放完毕
                            IsFinished = true;
                        }
                        else
                        {
//                            m_animBeginTimer = 0;
//                            m_animDuration = 0;
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
        return IsFinished;
    } 
    public override void OnExit()
    {
        
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
            Debug.LogWarning("OnTriggerEnter target is deaded");
            return;
        }
        //Debug.LogWarning("OnTriggerEnter target id is " +target.ID.ToString());
        SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(mSkillResultID);
        if (info == null)
        {
            return;
        }
        ActorProp selfProp = selfObj.transform.parent.GetComponent<ActorProp>();
        Trap self = selfProp.ActorLogicObj as Trap;

        if (!self.CheckActorAttackResult(target))
        {
            return;
        }

        if (ClientNet.Singleton.IsConnected)
        {
            IResult r = BattleFactory.Singleton.CreateResult(ENResult.Skill, self.ID, target.ID,
                mSkillResultID,  0);
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
                mSkillResultID, 0);
            if (r != null)
            {
                target.SetBlastPos(self.RealPos, self.GetBodyObject().transform.forward);
                r.ResultExpr(null);
                BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
            }
        }
//         if (!string.IsNullOrEmpty(info.SoundList))
//         {//播放击中声音
//             string[] param = info.SoundList.Split(new char[1] { ',' });
//             string sound = param[0];
//             if (!string.IsNullOrEmpty(sound))
//             {
//                 m_audioPos = selfObj.transform.position;
//                 GameResManager.Singleton.LoadResourceAsyncCallback(GameData.GetSoundPath(sound), Callback);
//                 //AudioClip clip = PoolManager.Singleton.CreateSoundObj(sound) as AudioClip;
//                 //AudioSource.PlayClipAtPoint(clip, selfObj.transform.position);
//             }
//             else
//             {
//                 Debug.LogWarning("sound string is null");
//             }
//         }
    }

    private List<GameObject> m_effectObjList = new List<GameObject>();
    public void AddEffectObj(GameObject obj)
    {
        m_effectObjList.Add(obj);
    }

    public override void Reset()
    {
        base.Reset();
        m_curAttackStep = -1;
        if (m_animationNameList != null)
        {
            m_animationNameList.Clear();
        }
//        m_animBeginTimer = 0;
//        m_animDuration = 0;
        IsFinished = false;
    }
};