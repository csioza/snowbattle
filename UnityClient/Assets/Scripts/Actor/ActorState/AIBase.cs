using UnityEngine;
using System.Collections.Generic;
using System;

public class AIBase
{
    public Actor Owner;
    public virtual void Update()
    {
        if (Owner.IsDead)
        {//死亡
            DeadAction action = Owner.ActionControl.LookupAction(ActorAction.ENType.enDeadAction) as DeadAction;
            if (action == null)
            {
                action = Owner.ActionControl.AddAction(ActorAction.ENType.enDeadAction) as DeadAction;
                if (action != null)
                {
                    action.Init(!Owner.IsRealDead);
                }
            }
        }
    }
    //是否被同伴呼叫
    public bool IsCalled { get { return m_isCalled; } set { m_isCalled = value; } }
    private bool m_isCalled = false;
    //搜寻目标的范围
    float m_searchTargetRange = 0;
    //搜寻目标的类型
    ENTargetType m_searchTargetType = ENTargetType.enNone;
    //可攻击范围内的目标列表
    public List<int> m_targetIDList = new List<int>();
    public float m_minDistance = float.MaxValue;
    public Actor m_minActor = null;
    public int m_curTargetID = 0;
    //优先选择的目标类型
    public ActorType m_firstTargetType = ActorType.enNone;
    //是否加载过预CD
    protected bool m_isLoadPreCD = false;
    void CheckTarget(Actor target)
    {
        if (target.Type == ActorType.enNPC)
        {
            NPC npc = target as NPC;
            if (npc.GetNpcType() == ENNpcType.enBlockNPC ||
                npc.GetNpcType() == ENNpcType.enFunctionNPC)
            {
                return;
            }
        }
        else if (target.Type == ActorType.enMain)
        {
            if (target.IsActorExit)
            {
                return;
            }
        }
        if (target.IsDead)
        {
            return;
        }
        switch (m_searchTargetType)
        {
            case ENTargetType.enEnemy:
                {
                    if (!ActorTargetManager.IsEnemy(Owner, target))
                    {
                        return;
                    }
                }
                break;
            case ENTargetType.enFriendly:
                {
                    if (!ActorTargetManager.IsFriendly(Owner, target))
                    {
                        return;
                    }
                }
                break;
            case ENTargetType.enSelf:
                {
                    if (Owner != target)
                    {
                        return;
                    }
                }
                break;
            case ENTargetType.enNullTarget:
                break;
            case ENTargetType.enFriendlyAndSelf:
                {
                    if (!ActorTargetManager.IsFriendly(Owner, target) && Owner != target)
                    {
                        return;
                    }
                }
                break;
            default:
                break;
        }
        float range = 0;
        if (Owner.Type == ActorType.enMain || Owner.Type == ActorType.enSwitch || Owner.Type == ActorType.enFollow)
        {
            Player self = Owner as Player;
            range = self.CurrentTableInfo.AttackRange;
            if (range <= 0)
            {
                return;
            }
            range = m_searchTargetRange != 0 ? m_searchTargetRange : range;
        }
        else if (Owner.Type == ActorType.enNPC)
        {
            NPC npc = Owner as NPC;
            if (npc.AttackRange <= 0)
            {
                return;
            }
            range = m_searchTargetRange != 0 ? m_searchTargetRange : npc.AttackRange;
        }
        float distance = ActorTargetManager.GetTargetDistance(Owner.RealPos, target.RealPos);
        if (range >= distance)
        {
            if (distance < m_minDistance)
            {
                m_minDistance = distance;
                m_minActor = target;
            }
            m_targetIDList.Add(target.ID);
        }
    }
    public void GetRangeTargetList(ENTargetType type, float range = 0, ActorType firstTargetType = ActorType.enNone)
    {
        m_searchTargetType = type;
        m_searchTargetRange = range;
        m_minDistance = float.MaxValue;
        m_minActor = null;
        m_targetIDList.Clear();
        m_firstTargetType = firstTargetType;
        ActorManager.Singleton.ForEach(CheckTarget);
    }
//     //战斗托管时组建targetList
//     void CheckAutoBattleTarget(ENTargetType type, Actor target)
//     {
//         if (target.Type == ActorType.enNPC)
//         {
//             NPC npc = target as NPC;
//             if (npc.GetNpcType() == ENNpcType.enBlockNPC ||
//                 npc.GetNpcType() == ENNpcType.enFunctionNPC)
//             {
//                 return;
//             }
//         }
//         else if (target.Type == ActorType.enMain)
//         {
//             MainPlayer player = target as MainPlayer;
//             if (player.IsActorExit)
//             {
//                 return;
//             }
//         }
//         if (target.IsDead)
//         {
//             return;
//         }
//         switch (m_searchTargetType)
//         {
//             case ENTargetType.enEnemy:
//                 {
//                     if (!ActorTargetManager.IsEnemy(Owner, target))
//                     {
//                         return;
//                     }
//                 }
//                 break;
//             case ENTargetType.enFriendly:
//                 {
//                     if (!ActorTargetManager.IsFriendly(Owner, target))
//                     {
//                         return;
//                     }
//                 }
//                 break;
//             case ENTargetType.enSelf:
//                 {
//                     if (Owner != target)
//                     {
//                         return;
//                     }
//                 }
//                 break;
//             case ENTargetType.enNullTarget:
//                 break;
//             case ENTargetType.enFriendlyAndSelf:
//                 {
//                     if (!ActorTargetManager.IsFriendly(Owner, target) && Owner != target)
//                     {
//                         return;
//                     }
//                 }
//                 break;
//             default:
//                 break;
//         }
//         float distance = ActorTargetManager.GetTargetDistance(Owner.RealPos, target.RealPos);
//         if (distance < m_minDistance)
//         {
//             m_minDistance = distance;
//             m_minActor = target;
//         }
//         m_targetIDList.Add(target.ID);
//     }
//     //获得一定范围内的目标的列表(range=0时为攻击范围)

//     public void SetRoomTargetList(ENTargetType type, List<NPC> npcList)
//     {
//         m_targetIDList.Clear();
//         setTargetIDlist(type, npcList);
//     }
//     public void setTargetIDlist(ENTargetType type, List<NPC> npcList)
//     {
//         foreach (NPC item in npcList)
//         {
//             CheckAutoBattleTarget(type,item);
//         }
//     }
    public bool ActionMoveTo(Vector3 targetPos)
    {
        return ActionMoveTo(targetPos, true, false, false, Vector3.zero);
    }
    public bool ActionMoveToNotAStar(Vector3 targetPos)
    {
        return ActionMoveTo(targetPos, false, false, false, Vector3.zero);
    }
    //朝着targetPos，以speed的速度移动(isBack为true时，播放动作为后退)
    public bool ActionMoveTo(Vector3 targetPos, bool isBack)
    {
        return ActionMoveTo(targetPos, false, true, false, Vector3.zero);
    }
    public bool ActionMoveTo(Vector3 targetPos,bool astar,bool isback,bool isSyncPosition,Vector3 syncPosition)
    {
        if (Owner.ActionControl.IsDisable(ActorAction.ENType.enMoveAction))
        {
            return false;
        }
        MoveAction action = Owner.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
        if (action != null)
        {
            if (action.m_realTargetPos == targetPos)
            {
                action.m_isBack = false;
                return true;
            }
        }
        if (action == null)
        {
            action = Owner.ActionControl.AddAction(ActorAction.ENType.enMoveAction) as MoveAction;
        }
        action.IsSyncPosition = isSyncPosition;
        action.SyncPosition = syncPosition;
        action.m_isNotAStar = !astar;
        action.m_isBack = isback;
        action.Retarget(targetPos);
        return true;
    }
    public bool ActionForwardTo(Vector3 targetPos)
    {
        StandAction action = Owner.ActionControl.LookupAction(ActorAction.ENType.enStandAction) as StandAction;
        if (action != null)
        {
            action.ForwardTarget(targetPos);
        }
        return true;
    }
    public bool ActionForwardTo(int targetID)
    {
        Actor curTarget = ActorManager.Singleton.Lookup(targetID);
        if (curTarget != null && !curTarget.IsDead)
        {//面向目标
            Vector3 curForward = Owner.MainObj.transform.forward;
            Vector3 targetForward = curTarget.RealPos - Owner.RealPos;
            targetForward.y = 0;
            Vector3 v = curForward - targetForward;
            if (v.magnitude > 0.01f)
            {
                if (!Owner.ActionControl.IsActionRunning(ActorAction.ENType.enAttackAction) && !Owner.ActionControl.IsActionRunning(ActorAction.ENType.enMoveAction))
                {
                    return ActionForwardTo(curTarget.RealPos);
                }
            }
        }
        return false;
    }
    #region ActionTryFireSkill
    public bool ActionTryAttack(int skillID, Actor skillTarget)
    {
        return ActionTryAttack(skillID, skillTarget, false, Vector3.zero);
    }
    public bool ActionTryAttack(int skillID, Actor skillTarget, bool isSyncPosValidate, Vector3 syncPos)
    {
        AttackAction action = Owner.ActionControl.AddAction(ActorAction.ENType.enAttackAction) as AttackAction;
        if (null == action)
        {
            return false;
        }
        Owner.SkillControl.AddSkill(skillID, Owner);
        int targetID = 0;
        if (skillTarget != null && !skillTarget.IsDead)
        {
            targetID = skillTarget.ID;
        }
        action.InitImpl(skillID, targetID, isSyncPosValidate,syncPos);
        //添加怒气值
        Actor.ActorSkillInfo info = Owner.SkillBag.Find(item => item.SkillTableInfo.ID == skillID);
        if (info != null)
        {
            Owner.CurrentRage += info.SkillTableInfo.GenerateRagePoint;
            Owner.CurrentRage -= info.SkillTableInfo.CostRagePoint;
        }
        return true;
    }
    bool LookupTarget(SkillInfo info, out Actor skillTarget, out float closestDistance)
    {
        skillTarget = null;
        closestDistance = float.MaxValue;
        //if (info.SkillTableInfo.SkillEffectType == (int)ENSkillEffectType.enRestore)
        //{
        //    return false;
        //}
        if (info.TargetType == (int)ENTargetType.enNullTarget)
        {//不需要目标就能释放的技能
            return true;
        }
        //首先查看是否可对当前目标释放
        Actor curTarget = ActorManager.Singleton.Lookup(m_curTargetID);
        if (curTarget != null && !curTarget.IsDead)
        {
            float distance = ActorTargetManager.GetTargetDistance(Owner.MainPos, curTarget.MainPos);
            if (distance >= info.LeastAttackDistance)
            {//最小攻击距离外
                switch ((ENTargetType)info.TargetType)
                {
                    case ENTargetType.enEnemy:
                        {
                            if (ActorTargetManager.IsEnemy(Owner, curTarget))
                            {
                                skillTarget = curTarget;
                                return true;
                            }
                        }
                        break;
                    case ENTargetType.enFriendly:
                        {
                            if (ActorTargetManager.IsFriendly(Owner, curTarget))
                            {
                                skillTarget = curTarget;
                                return true;
                            }
                        }
                        break;
                    case ENTargetType.enSelf:
                        {
                            if (Owner == curTarget)
                            {
                                skillTarget = curTarget;
                                return true;
                            }
                        }
                        break;
                    case ENTargetType.enNullTarget:
                        break;
                    case ENTargetType.enFriendlyAndSelf:
                        {
                            if (ActorTargetManager.IsFriendly(Owner, curTarget) && Owner != curTarget)
                            {
                                skillTarget = curTarget;
                                return true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        //不可以对当前目标释放，查找距离自己最近的

        for (int i = 0; i < m_targetIDList.Count; ++i)
        {
            Actor target = ActorManager.Singleton.Lookup(m_targetIDList[i]);
            if (target == null)
            {
                continue;
            }
            if (target.IsDead)
            {
                continue;
            }
            float distance = ActorTargetManager.GetTargetDistance(Owner.MainPos, target.MainPos);
            if (distance < info.LeastAttackDistance)
            {//最小攻击距离内
                continue;
            }
            switch ((ENTargetType)info.TargetType)
            {
                case ENTargetType.enEnemy:
                    {
                        if (!ActorTargetManager.IsEnemy(Owner, target))
                        {
                            continue;
                        }
                    }
                    break;
                case ENTargetType.enFriendly:
                    {
                        if (!ActorTargetManager.IsFriendly(Owner, target))
                        {
                            continue;
                        }
                    }
                    break;
                case ENTargetType.enSelf:
                    {
                        if (Owner != target)
                        {
                            continue;
                        }
                    }
                    break;
                case ENTargetType.enNullTarget:
                    break;
                case ENTargetType.enFriendlyAndSelf:
                    {
                        if (!ActorTargetManager.IsFriendly(Owner, target) && Owner != target)
                        {
                            continue;
                        }
                    }
                    break;
                default:
                    break;
            }
            if (m_firstTargetType != ActorType.enNone)
            {//优先选择目标条件成立
                if (target.Type == m_firstTargetType)
                {//是优先选择的目标类型
                    if (skillTarget == null)
                    {
                        skillTarget = target;
                        closestDistance = distance;
                    }
                    else
                    {
                        if (skillTarget.Type == m_firstTargetType)
                        {//已有优先目标，进行比较，取近的
                            if (distance < closestDistance)
                            {
                                skillTarget = target;
                                closestDistance = distance;
                            }
                        }
                        else
                        {
                            skillTarget = target;
                            closestDistance = distance;
                        }
                    }
                }
                else
                {
                    if (skillTarget == null)
                    {
                        skillTarget = target;
                        closestDistance = distance;
                    }
                    else
                    {
                        if (skillTarget.Type != m_firstTargetType)
                        {
                            if (distance < closestDistance)
                            {
                                skillTarget = target;
                                closestDistance = distance;
                            }
                        }
                    }
                }
            }
            else
            {
                if (distance < info.AttackDistance)
                {
                    skillTarget = target;
                    break;
                }
                else
                {
                    if (distance < closestDistance)
                    {
                        skillTarget = target;
                        closestDistance = distance;
                    }
                }
            }
        }
        return skillTarget != null;
    }
    bool LookupSkill(int skillID, out int skillIDForTarget, out Actor skillTarget)
    {
        skillIDForTarget = 0;
        skillTarget = null;

        float closestDistance = float.MaxValue;
        Actor closestActor = null;
        int closestSkillID = 0;
        foreach (var item in Owner.SkillBag)
        {
            SkillInfo info = item.SkillTableInfo;
            if (skillID != 0 && skillID != info.ID)
            {
                continue;
            }
            if (!item.IsCanFire(Owner, skillID != 0))
            {
                continue;
            }
            Actor temp = null;
            float distance = 0;
            if (LookupTarget(info, out temp, out distance))
            {//找到此技能的最近释放目标
                if (distance <= closestDistance)
                {//跟其它技能的最近的目标作比对
                    closestDistance = distance;
                    closestActor = temp;
                    closestSkillID = info.ID;
                }
                break;
            }
        }
        if (closestDistance <= float.MaxValue && closestSkillID != 0)
        {
            if (closestActor == null)
            {//目标为空，判断是否为无目标技能
                Actor.ActorSkillInfo info = Owner.SkillBag.Find(item => item.SkillTableInfo.ID == closestSkillID);
                if (info == null)
                {
                    Debug.LogWarning("skill id is not exist! id is " + closestSkillID.ToString() + " self id is " + Owner.ID.ToString());
                    return false;
                }
                if (info.SkillTableInfo.TargetType != (int)ENTargetType.enNullTarget)
                {
                    Debug.LogWarning("target is null, skill target is not null target, TargetType is " + info.SkillTableInfo.TargetType);
                    return false;
                }
            }
            skillTarget = closestActor;
            skillIDForTarget = closestSkillID;
        }
        return skillIDForTarget != 0;
    }
    public bool ActionTryFireSkill(int skillID)
    {
        if (skillID == 0)
        {//自动查找技能
            if (Owner.ActionControl.IsActionRunning(AttackAction.SGetActionType()))
            {//正在攻击，暂时返回
                return false;
            }
        }
        int skillIDForTarget = 0;
        Actor skillTarget = null;
        if (!LookupSkill(skillID, out skillIDForTarget, out skillTarget))
        {
            //Debug.LogWarning("LookupSkill failed! id is " + skillIDForTarget.ToString() + " self id is "+Owner.ID.ToString());
            return false;
        }
        Actor.ActorSkillInfo info = Owner.SkillBag.Find(item => item.SkillTableInfo.ID == skillIDForTarget);
        if (info == null)
        {
            Debug.LogWarning("skill id is not exist! id is " + skillIDForTarget.ToString() + " self id is " + Owner.ID.ToString());
            return false;
        }
        if (skillID == 0 && Owner.ActionControl.IsActionRunning(ActorAction.ENType.enAttackAction))
        {//攻击中并且不是技能命令， 不释放
            return false;
        }
        if (skillTarget == null)
        {//无目标技能
            //释放技能
            return ActionTryAttack(skillIDForTarget, skillTarget);
        }
        else
        {
            Vector3 distance = Owner.RealPos - skillTarget.RealPos;
            distance.y = 0;
            if (distance.magnitude > info.SkillTableInfo.AttackDistance)
            {//不在攻击范围内
                //向技能目标移动
                ActionMoveTo(skillTarget.RealPos);
            }
            else
            {//在攻击范围内
                //释放技能
                if (ActionTryAttack(skillIDForTarget, skillTarget))
                {
                    m_curTargetID = skillTarget.ID;
                    return true;
                }
            }
        }
        return false;
    }
    #endregion
    #region health
    //是否治疗友方技能
    protected bool IsHealthFriendly()
    {
        if (null != Owner.SkillBag.Find(item => (item.SkillTableInfo.SkillEffectType == (int)ENSkillEffectType.enRestore && item.SkillTableInfo.TargetType == (int)ENTargetType.enFriendlyAndSelf)))
        {//有治疗友方的技能
            GetRangeTargetList(ENTargetType.enFriendlyAndSelf);
            Actor leastTarget = null;
            float leastPercent = float.MaxValue;
            foreach (var targetID in m_targetIDList)
            {
                Actor target = ActorManager.Singleton.Lookup(targetID);
                if (target == null || target.IsDead)
                {
                    continue;
                }
                float percent = target.HP / target.MaxHP;
                if (percent < leastPercent && percent < 1)
                {
                    leastTarget = target;
                    leastPercent = percent;
                }
            }
            if (leastTarget != null)
            {//治疗血量百分比最小的目标
                foreach (var info in Owner.SkillBag)
                {
                    if (!info.IsCanFire(Owner))
                    {
                        continue;
                    }
                    if (info.SkillTableInfo.SkillEffectType == (int)ENSkillEffectType.enRestore)
                    {//治疗技能
                        if (info.SkillTableInfo.TargetType == (int)ENTargetType.enFriendlyAndSelf)
                        {//目标类型为友方和自己
                            if (ActionTryAttack(info.SkillTableInfo.ID, leastTarget))
                            {//治疗友方
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
    //是否治疗自己
    protected bool IsHealthSelf()
    {
        foreach (var info in Owner.SkillBag)
        {
            if (!info.IsCanFire(Owner))
            {
                continue;
            }
            if (info.SkillTableInfo.SkillEffectType == (int)ENSkillEffectType.enRestore)
            {//治疗技能
                if (info.SkillTableInfo.TargetType == (int)ENTargetType.enSelf)
                {//目标类型为自己
                    if (ActionTryAttack(info.SkillTableInfo.ID, Owner))
                    {//治疗自己
                        return true;
                    }
                }
            }
        }
        return false;
    }
    #endregion

    #region AIChild中不同的变量
    public bool mIsInitBaseData = false;
    public Dictionary<string, AIBaseData> m_baseDataDic = new Dictionary<string, AIBaseData>();

    public AIBaseData GetBaseData(string valueName)
    {
        if (m_baseDataDic.ContainsKey(valueName))
        {
            return m_baseDataDic[valueName];
        }
        return null;
    }
    public AIBaseData GetBaseData(string valueName, AIBaseData.DataType valueType)
    {
        if (m_baseDataDic.ContainsKey(valueName))
        {
            return m_baseDataDic[valueName];
        }
        else
        {
            AIBaseData tmpAIBaseData = AIBaseData.CreateAIBaseData(valueType);
            m_baseDataDic.Add(valueName, tmpAIBaseData);
            return tmpAIBaseData;
        }
    }
    #endregion
}
public class AIBaseData
{
    //数据的类型枚举
    public enum DataType
    {
        enNone = -1,
        enFloat,
        enTime,
        enInt,
        enBool,
        enString,
    }
    //比较的类型枚举
    public enum CompareType
    {
        enNone,
        enHigh,
        enLow,
        enEqual,
    }
    //设置数据的类型枚举
    public enum SetType
    {
        enNone,
        enAdd,
        enSub,
        enReset,
    }
    //数据的字符串名称
    public string mDataName = "";
    //数据的Int类型数值
    public int mDataIntValue = 0;
    //数据的Float类型数值
    public float mDataFloatValue = 0f;
    //数据的Time类型数值
    public float mDataTimeValue = 0f;
    //数据的String类型数值
    public string mDataStrValue = "";
    //数据的Bool类型数值
    public bool mDataBoolValue = false;
    //数据的类型
    public DataType mDataType = DataType.enFloat;

    public AIBaseData()
    {
        ResetValue();
    }
    public AIBaseData(DataType type)
    {
        mDataName = "TmpData";
        mDataType = type;
        ResetValue();
    }
    public AIBaseData(string name, DataType type)
    {
        mDataName = name;
        mDataType = type;
        ResetValue();
    }
    public AIBaseData(string name, string typeStr = "Float")
    {
        mDataName = name;
        mDataType = ChangeStrToDataType(typeStr);
        ResetValue();
    }
    public static AIBaseData CreateAIBaseData(DataType type = DataType.enFloat)
    {
        switch (type)
        {
            case DataType.enFloat:
                return new AIBaseDataFloat(type);
            case DataType.enTime:
                return new AIBaseDataTime(type);
            case DataType.enBool:
                return new AIBaseDataBool(type);
            case DataType.enInt:
                return new AIBaseDataInt(type);
        }
        return null;
    }
    //将字符串改变为数据类型
    public static DataType ChangeStrToDataType(string str)
    {
        switch (str)
        {
            case "Int":
                return DataType.enInt;
            case "Float":
                return DataType.enFloat;
            case "Time":
                return DataType.enTime;
            case "String":
                return DataType.enString;
            case "Bool":
                return DataType.enBool;
        }
        return DataType.enNone;
    }
    //将字符串改变为比较类型
    public static CompareType ChangeStrToCompareType(string str)
    {
        switch (str)
        {
            case "ValueHigh":
                return CompareType.enHigh;
            case "ValueLow":
                return CompareType.enLow;
            case "Equal":
                return CompareType.enEqual;
        }
        return CompareType.enNone;
    }
    //将字符串改变为设置类型
    public static SetType ChangeStrToSetType(string str)
    {
        switch (str)
        {
            case "Add":
                return SetType.enAdd;
            case "Sub":
                return SetType.enSub;
            case "Reset":
                return SetType.enReset;
        }
        return SetType.enNone;
    }
    //数值比较
    public virtual bool CompareValue(CompareType comType, AIBaseData compData)
    {
        return false;
    }
    //按照设置类型设值
    public virtual void SetValue(SetType setType, AIBaseData val)
    {

    }
    //直接赋值
    public virtual void SetValue(AIBaseData val)
    {

    }
    //不同类型赋不同的值
    public void SetIntValue(int val)
    {
        mDataIntValue = val;
    }
    public void SetFloatValue(float val)
    {
        mDataFloatValue = val;
    }
    public void SetTimeValue(float val)
    {
        mDataTimeValue = val;
    }
    public void SetBoolValue(bool val)
    {
        mDataBoolValue = val;
    }
    public void SetStringValue(string val)
    {
        mDataStrValue = val;
    }

    //按照参数为不同类型的值赋值
    public void SetValue(string typeStr, string valueStr)
    {
        switch (ChangeStrToDataType(typeStr))
        {
            case DataType.enFloat:
            case DataType.enTime:
                SetValue(float.Parse(valueStr));
                break;
            case DataType.enString:
                SetValue(valueStr);
                break;
            case DataType.enBool:
                SetValue(valueStr == "True" ? true : false);
                break;
        }
    }
    public void SetValue(float value)
    {
        if (mDataType == DataType.enFloat)
        {
            mDataFloatValue = value;
        }
        else if (mDataType == DataType.enTime)
        {
            mDataTimeValue = value;
        }
        
    }
    public void SetValue(bool tmpBool)
    {
        mDataBoolValue = tmpBool;
    }
    public void SetValue(int val)
    {
        mDataIntValue = val;
    }
    //按照本身数据类型解析字符串赋值
    public void SetValue(string valueStr)
    {
        switch (mDataType)
        {
            case DataType.enFloat:
                SetValue(float.Parse(valueStr));
                break;
            case DataType.enTime:
                SetValue(float.Parse(valueStr));
                break;
            case DataType.enString:
                SetValue(valueStr);
                break;
            case DataType.enBool:
                SetValue(valueStr == "True" ? true : false);
                break;
            case DataType.enInt:
                SetValue(int.Parse(valueStr));
                break;
        }
    }
    //重置数据
    public void ResetValue()
    {
        mDataIntValue = 0;
        mDataFloatValue = 0f;
        mDataTimeValue = (float)Math.Ceiling(Time.time);
        mDataStrValue = "";
        mDataBoolValue = false;
    }

    //按照不同类型获取数值
    public int GetValue_Int()
    {
        return mDataIntValue;
    }
    public float GetValue_Float()
    {
        return mDataFloatValue;
    }
    public float GetValue_Time()
    {
        return mDataTimeValue;
    }
    public bool GetValue_Bool()
    {
        return mDataBoolValue;
    }
    public string GetValue_String()
    {
        return mDataStrValue;
    }
}

public class AIBaseDataFloat :AIBaseData
{
    public AIBaseDataFloat(DataType type)
        : base(type)
    {
        mDataType = type;
        ResetValue();
    }
    public override bool CompareValue(CompareType comType, AIBaseData compData)
    {
        switch (comType)
        {
            case CompareType.enHigh:
                return mDataFloatValue > compData.mDataFloatValue;
            case CompareType.enLow:
                return mDataFloatValue < compData.mDataFloatValue;
            case CompareType.enEqual:
                return mDataFloatValue == compData.mDataFloatValue;
        }
        return false;
    }
    public override void SetValue(SetType setType, AIBaseData val)
    {
        switch (setType)
        {
            case SetType.enAdd:
                base.SetFloatValue(val.mDataFloatValue + mDataFloatValue);
                break;
            case SetType.enSub:
                base.SetFloatValue(val.mDataFloatValue - mDataFloatValue);
                break;
            case SetType.enReset:
                base.ResetValue();
                break;
        }
    }
    public override void SetValue(AIBaseData val)
    {
        base.SetFloatValue(val.mDataFloatValue);
    }
}

public class AIBaseDataTime : AIBaseData
{
    public AIBaseDataTime(DataType type)
        : base(type)
    {
        mDataType = type;
        ResetValue();
    }

    public override bool CompareValue(CompareType comType, AIBaseData compData)
    {
        switch (comType)
        {
            case CompareType.enHigh:
                return Time.time - mDataTimeValue > compData.mDataTimeValue;
            case CompareType.enLow:
                return Time.time - mDataTimeValue < compData.mDataTimeValue;
            case CompareType.enEqual:
                return Time.time - mDataTimeValue == compData.mDataTimeValue;
        }
        return false;
    }
    public override void SetValue(SetType setType, AIBaseData val)
    {
        switch (setType)
        {
            case SetType.enAdd:
                base.SetTimeValue(val.mDataTimeValue + mDataTimeValue);
                break;
            case SetType.enSub:
                base.SetTimeValue(val.mDataTimeValue - mDataTimeValue);
                break;
            case SetType.enReset:
                base.ResetValue();
                break;
        }
    }
    public override void SetValue(AIBaseData val)
    {
        base.SetTimeValue(val.mDataTimeValue);
    }
}

public class AIBaseDataInt : AIBaseData
{
    public AIBaseDataInt(DataType type)
        : base(type)
    {
        mDataType = type;
        ResetValue();
    }

    public override bool CompareValue(CompareType comType, AIBaseData compData)
    {
        switch (comType)
        {
            case CompareType.enHigh:
                return mDataIntValue > compData.mDataIntValue;
            case CompareType.enLow:
                return mDataIntValue < compData.mDataIntValue;
            case CompareType.enEqual:
                return mDataIntValue == compData.mDataIntValue;
        }
        return false;
    }
    public override void SetValue(SetType setType, AIBaseData val)
    {
        switch (setType)
        {
            case SetType.enAdd:
                base.SetIntValue(val.mDataIntValue + mDataIntValue);
                break;
            case SetType.enSub:
                base.SetIntValue(val.mDataIntValue - mDataIntValue);
                break;
            case SetType.enReset:
                base.ResetValue();
                break;
        }
    }
    public override void SetValue(AIBaseData val)
    {
        base.SetIntValue(val.mDataIntValue);
    }
}

public class AIBaseDataBool : AIBaseData
{
    public AIBaseDataBool( DataType type)
        : base(type)
    {
        mDataType = type;
        ResetValue();
    }

    public override bool CompareValue(CompareType comType, AIBaseData compData)
    {
        return mDataBoolValue == compData.mDataBoolValue;
    }

    public override void SetValue(AIBaseData val)
    {
        base.SetBoolValue(val.mDataBoolValue);
    }
}