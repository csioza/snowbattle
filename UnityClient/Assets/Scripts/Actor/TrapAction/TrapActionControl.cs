////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Actor\TrapAction
//	created:	2013-4-15
//	author:		Mingzhen Zhang

////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class TrapAction 
{
    #region ENTypePart
    public enum ENType
    {
        enNone,	//无
        enAttackAction,
        enStandAction,	//站立
        enBackAction,//收起
        enContinueDamageAction,
        enLockAction,
        Count,
    }
    public static ENType Type2Type(Type t)
    {
        MethodInfo miCreateQuery = t.GetMethod("SGetActionType", BindingFlags.Static | BindingFlags.Public);
        ENType value;
        if (miCreateQuery != null)
        {
            System.Object o = miCreateQuery.Invoke(null, null);
            value = (ENType)o;
            return value;
        }
        return ENType.enNone;
    }
    public static ENType ActionType<T>() where T : TrapAction, new()
    {
        MethodInfo miCreateQuery = typeof(T).GetMethod("SGetActionType", BindingFlags.Static | BindingFlags.Public);
        ENType value;
        if (miCreateQuery != null)
        {
            System.Object o = miCreateQuery.Invoke(null, null);
            value = (ENType)o;
            //Debug.Log("aaaaa=" + o.ToString());
            return value;
        }
        else
        {
            T obj = null;
            try
            {
                obj = new T();
            }
            catch (Exception e)
            {
                Debug.LogError("Error On ActionType " + typeof(T).ToString() + ", msg:" + e.Message + ",Stack:" + e.StackTrace.ToString());
                return ENType.enNone;
            }
            return obj.GetActionType();
        }
    }
    #endregion

    #region ENRelationPart
    public enum ENRelation
    {
        enUnable,	//新动作不许执行
        enBothCan,	//动作间可以共存
        enReplace,	//新动作替换旧动作
    }
    #endregion

    public bool IsEnable = true;
    public bool IsInited = false;
    public Trap CurrentActor { get; set; }
    //animation的播放时间长度，初始时长为float.MaxValue（先播放动画，再计算时间）
    public float AnimLength = float.MaxValue;
    //animation的播放开始时间
    public float AnimStartTime = 0;
    //animation的名字
    public string AnimFullName = "";
    public virtual ENType GetActionType() { return ENType.enNone; }
    public virtual void OnEnter() { }
    public virtual void OnInterupt(TrapAction.ENType newType) { }
    public virtual void OnExit() { }
    //返回true表示执行完毕
    public virtual bool OnUpdate() { return true; }
    public virtual bool OnLateUpdate() { return true; }

    public virtual string GetAnimationName()
    {
        return AnimFullName;
    }
    public virtual void Reset()
    {
        IsEnable = true;
        IsInited = false;
        CurrentActor = null;
        AnimLength = float.MaxValue;//重置为float.MaxValue（先播放动画，再计算时间）
        AnimStartTime = 0;
        AnimFullName = "";
    }
    public void RefreshActionRef()
    {
        uint refCount = 0;
        refCount++;
        if (CurrentActor.mActionRefCountDict[(int)GetActionType()] > 0)
        {
            refCount = CurrentActor.mActionRefCountDict[(int)GetActionType()];
            refCount++;
        }
        CurrentActor.mActionRefCountDict[(int)GetActionType()] = refCount;
    }
}

public class TrapActionControl
{
    public TrapActionControl(Trap actor)
    {
        CurrentActor = actor;
    }
    public Trap CurrentActor { get; private set; }
    private List<TrapAction> m_actionList = new List<TrapAction>();
    public List<TrapAction> ActionList { get { return m_actionList; } }
    public int[] ActioningArray = new int[(int)TrapAction.ENType.Count];
    public TrapAction[] ActioningObjArray = new TrapAction[(int)TrapAction.ENType.Count];
    //当前是否没有任何action
    public bool IsActionEmpty() { return m_actionList.Count == 0; }
    public bool IsActionRunning(TrapAction.ENType acType)
    {
        return ActioningArray[(int)acType] > 0;
    }
    public TrapAction LookupAction(TrapAction.ENType type)
    {
        return ActioningObjArray[(int)type];
        //return m_actionList.Find(action => action.GetActionType() == type);
    }
    public void RemoveAction(TrapAction.ENType type)
    {
        foreach (var action in m_actionList)
        {
            if (action.GetActionType() == type)
            {
                action.OnExit();
                RemoveDisableCount(type);
            }
        }
        m_actionList.RemoveAll(action => action.GetActionType() == type);
        ActioningObjArray[(int)type] = null;
        ActioningArray[(int)type] = 0;
    }
    public void RemoveAction(TrapAction action)
    {
        if (action == null)
        {
            return;
        }
        action.OnExit();
        RemoveDisableCount(action.GetActionType());
        m_actionList.Remove(action);
    }
    public void RemoveAll()
    {
        foreach (var action in m_actionList)
        {
            action.OnExit();
            RemoveDisableCount(action.GetActionType());
        }
        m_actionList.Clear();
        for (int i = 0; i < ActioningArray.Length; i++)
        {
            ActioningArray[i] = 0;
        }
        for (int i = 0; i < ActioningObjArray.Length; i++)
        {
            ActioningObjArray[i] = null;
        }
    }
    public TrapAction AddAction(TrapAction.ENType newType)
    {
        if (!IsDisable(newType))
        {
            bool isValidateActionState = false;
            for (int index = m_actionList.Count - 1; index >= 0; --index)
            {
                TrapAction item = m_actionList[index];
                if (!item.IsEnable)
                {
                    continue;
                }
                if (TrapAction.ENRelation.enReplace == CheckRelation(newType, item.GetActionType()))
                {
                    m_actionList.RemoveAt(index);
                    isValidateActionState = true;

                    item.OnInterupt(newType);
                    RemoveDisableCount(item.GetActionType());
                    item.IsEnable = false;
                    ReleaseObj(item);
                }
            }
            TrapAction obj = CreateObj(newType);
            obj.CurrentActor = CurrentActor;

            ActioningObjArray[(int)newType] = obj;
            m_actionList.Add(obj);
            AddDisableCount(obj.GetActionType());
            if (isValidateActionState)
            {
                for (int i = 0; i < ActioningArray.Length; i++)
                {
                    ActioningArray[i] = 0;
                    ActioningObjArray[i] = null;
                }
                for (int i = 0; i < m_actionList.Count; i++)
                {
                    TrapAction ac = m_actionList[i];
                    ActioningObjArray[(int)ac.GetActionType()] = ac;
                    ActioningArray[(int)ac.GetActionType()]++;
                }
            }
            return obj;
        }
        else
        {
            string currentActions = CurrentActor.GetType() + ":" + newType.ToString() + " Current Actions:";
            foreach (TrapAction action in m_actionList)
            {
                currentActions += action.GetActionType().ToString() + ", ";
            }
            //Debug.LogWarning("Add action failed:" + newType.ToString()+" actor type:"+CurrentActor.Type.ToString());
            return null;
        }
    }
    private Dictionary<TrapAction.ENType, LinkedList<TrapAction>> m_pool = new Dictionary<TrapAction.ENType, LinkedList<TrapAction>>();
    private TrapAction CreateObj(TrapAction.ENType newType)
    {
        LinkedList<TrapAction> outList;
        if (m_pool.TryGetValue(newType, out outList))
        {
            if (outList.Count > 0)
            {
                TrapAction obj = outList.Last.Value as TrapAction;
                outList.RemoveLast();
                return obj;
            }
        }
        switch (newType)
        {
            case TrapAction.ENType.enAttackAction:
                return new TrapAttackAction();
//                break;	//站立
            case TrapAction.ENType.enBackAction:
                return new TrapBackAction();
//                break;
            case TrapAction.ENType.enStandAction:
                return new TrapStandAction();
//                break;
        }

        throw new Exception("Miss Action Create for" + newType.ToString());
    }
    private void ReleaseObj(TrapAction obj)
    {
        obj.Reset();
        LinkedList<TrapAction> outList;
        if (m_pool.TryGetValue(obj.GetActionType(), out outList))
        {
            outList.AddLast(obj);
        }
        else
        {
            outList = new LinkedList<TrapAction>();
            outList.AddLast(obj);
            m_pool.Add(obj.GetActionType(), outList);
        }
    }
    public void LateUpdate()
    {

    }
    public void FixedUpdate()
    {
        for (int index = m_actionList.Count - 1; index >= 0; --index)
        {
            TrapAction action = m_actionList[index];
            if (!action.IsInited)
            {
                action.OnEnter();
                action.IsInited = true;
            }
            if (!action.IsEnable)
            {
                RemoveDisableCount(action.GetActionType());
                m_actionList.RemoveAt(index);

                ReleaseObj(action);
                continue;
            }
            if (action.OnUpdate())
            {
                action.OnExit();
                RemoveDisableCount(action.GetActionType());
                action.IsEnable = false;

                m_actionList.RemoveAt(index);
                ReleaseObj(action);
            }
        }
//         if (m_actionList.Count == 0)
//         {
//             AddAction(TrapAction.ENType.enStandAction);
//         }
    }

    #region DisableCount

    private TrapAction.ENRelation CheckRelation(TrapAction.ENType newAction, TrapAction.ENType oldAction)
    {
        if (newAction == oldAction)
        {
            return TrapAction.ENRelation.enUnable;
        }
        if (oldAction == TrapAction.ENType.enAttackAction)
        {
            switch (newAction)
            {
                case TrapAction.ENType.enContinueDamageAction:
                    return TrapAction.ENRelation.enReplace;
                default:
                    return TrapAction.ENRelation.enUnable;
            }
        }
        if (oldAction == TrapAction.ENType.enStandAction)
        {
            switch (newAction)
            {
                case TrapAction.ENType.enAttackAction:
                    return TrapAction.ENRelation.enReplace;
                default:
                    return TrapAction.ENRelation.enReplace;
            }
        }
        return TrapAction.ENRelation.enUnable;
        //return (TrapAction.ENRelation)GameTable.trapActionRelationTableAsset.Lookup((int)newAction, (int)oldAction);
    }

    private int[] m_disableCount = new int[(int)TrapAction.ENType.Count];

    public void AddDisableCount(TrapAction.ENType type)
    {
        ActioningArray[(int)type]++;
        for (int index = 0; index < m_disableCount.Length; ++index)
        {
            if (TrapAction.ENRelation.enUnable == CheckRelation((TrapAction.ENType)index, type))
            {
                ++m_disableCount[index];
            }
        }
    }
    private void RemoveDisableCount(TrapAction.ENType type)
    {
        ActioningObjArray[(int)type] = null;
        ActioningArray[(int)type]--;
        if (ActioningArray[(int)type] < 0)
        {
            ActioningArray[(int)type] = 0;
        }
        for (int index = 0; index < m_disableCount.Length; ++index)
        {
            if (TrapAction.ENRelation.enUnable == CheckRelation((TrapAction.ENType)index, type))
            {
                --m_disableCount[index];
            }
        }
    }
    public bool IsDisable(TrapAction.ENType type)
    {
        return m_disableCount[(int)type] > 0;
    }
    #endregion
};