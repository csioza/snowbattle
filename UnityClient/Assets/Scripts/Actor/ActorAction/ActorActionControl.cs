//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Actor\ActorAction
//	created:	2013-4-15
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class ActorAction
{
	#region ENTypePart
	public enum ENType
	{
		enNone,	//无
		enStandAction,	//站立
		enMoveAction,	//移动
		enAttackAction,	//攻击
		enSpasticityAction,	//被动僵直
		enBeAttackAction,	//受击
		enUndownAction,	//霸体
		enDeadAction,	//死亡
		enReliveAction,	//复活
		enPlayEffectAction,	//播放特效
		enSearchEnemyAction,	//搜索敌人
		enTeleportAction,	//瞬移
        enControlMoveAction,//不能移动
		enHoldDownAction,//按下状态
        enSelfSpasticityAction,	//主动僵直
        enAlertAction,      //警戒
        enJumpinAction,  //入场
        enJumpoutAction,   //退场
        enRollAction, //翻滚
        enFakeBeAttackAction, //假受击
        enActorExitAction, //主控角色退场
        enActorEnterAction, //主控角色入场
        enControlAttackAction,//不能攻击
        enControlBeAttackAction,//不能受击
        enAttackingMoveAction,//攻击时移动
        enDragMoveAction,   //拖拽
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
    //public static ENType ActionType<T>() where T : ActorAction, new()
    //{
    //    MethodInfo miCreateQuery = typeof(T).GetMethod("SGetActionType", BindingFlags.Static | BindingFlags.Public);
    //    ENType value;
    //    if (miCreateQuery != null)
    //    {
    //        System.Object o = miCreateQuery.Invoke(null, null);
    //        value = (ENType)o;
    //        //Debug.Log("aaaaa=" + o.ToString());
    //        return value;
    //    }
    //    else
    //    {
    //        T obj = null;
    //        try
    //        {
    //            obj = new T();
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError("Error On ActionType " + typeof(T).ToString() + ", msg:" + e.Message + ",Stack:" + e.StackTrace.ToString());
    //            return ENType.enNone;
    //        }
    //        return obj.GetActionType();
    //    }
    //}
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
	public Actor CurrentActor { get; set; }
    //animation的播放时间长度，初始时长为float.MaxValue（先播放动画，再计算时间）
    public float AnimLength = float.MaxValue;
    //animation的播放开始时间
    public float AnimStartTime = 0;
    //animation的名字
    public string AnimFullName = "";
    //是否同步位置
    public bool IsSyncPosition = false;
    public Vector3 SyncPosition { get; set; }
	public virtual ENType GetActionType() { return ENType.enNone; }
	public virtual void OnEnter() { }
	public virtual void OnInterupt() { }
	public virtual void OnExit() { }
    //返回true表示执行完毕
    public virtual bool OnUpdate() { return true; }
    public virtual bool OnLateUpdate() { return true; }
    //int id_;
	public virtual void Reset()
	{
        //if (CurrentActor != null)
        //{
        //    id_ = CurrentActor.ID;
        //}
		IsEnable = true;
		IsInited = false;
		CurrentActor = null;
        //AnimLength = float.MaxValue;//重置为float.MaxValue（先播放动画，再计算时间）
        AnimLength = 3.33f;
        AnimStartTime = 0;
        AnimFullName = "";
        IsSyncPosition = false;
	}
    public void SyncPositionIfNeed()
    {
        if (IsSyncPosition)
        {
            IsSyncPosition = false;
            Vector3 v = CurrentActor.MainPos - SyncPosition;
            if (v.sqrMagnitude > 2.5f)
            {
                CurrentActor.MoveTo(SyncPosition);
            }
        }
    }
    public void RefreshActionRef()
    {
        uint refCount = 0;
        refCount++;
        if (CurrentActor == null)
        {
            Debug.LogError("ActorAction RefreshActionRef The Actor is null!!!!" + GetActionType());
            return;
        }
        if (CurrentActor.mActionRefCountDict[(int)GetActionType()] > 0)
        {
            refCount = CurrentActor.mActionRefCountDict[(int)GetActionType()];
            refCount++;
        }
        CurrentActor.mActionRefCountDict[(int)GetActionType()] = refCount;
//         if (CurrentActor.mActionRefCountDict.TryGetValue(GetActionType(), out refCount))
//         {
//             refCount++;
//             CurrentActor.mActionRefCountDict[GetActionType()] = refCount;
//         }
//         else
//         {
//             refCount++;
//             CurrentActor.mActionRefCountDict.Add(GetActionType(), refCount);
//         }
    }
}

public class ActorActionControl
{
	public ActorActionControl(Actor actor)
	{
		CurrentActor = actor;
	}
    public Actor CurrentActor { get; private set; }
    private List<ActorAction> m_actionList = new List<ActorAction>();
    public List<ActorAction> ActionList { get { return m_actionList; } }
    public int[] ActioningArray = new int[(int)ActorAction.ENType.Count];
    public ActorAction[] ActioningObjArray = new ActorAction[(int)ActorAction.ENType.Count];
    //当前是否没有任何action
    public bool IsActionEmpty() { return m_actionList.Count == 0; }
	public bool IsActionRunning(ActorAction.ENType acType)
	{
        return ActioningArray[(int)acType] > 0;
	}
	public ActorAction LookupAction(ActorAction.ENType type)
	{
        return ActioningObjArray[(int)type];
		//return m_actionList.Find(action => action.GetActionType() == type);
	}
	ActorAction.ENType m_curRemoveType_Temp;
	private bool Temp_Remove_Action(ActorAction act)
	{
		if (act.GetActionType () == m_curRemoveType_Temp) 
		{
			ReleaseObj (act);
			return true;
		}
		return false;
	}
	public void RemoveAction(ActorAction.ENType type)
	{
		foreach (var action in m_actionList)
		{
			if (action.GetActionType() == type)
			{
                action.OnInterupt();
				RemoveDisableCount(type);
			}
		}
		m_curRemoveType_Temp = type;
		m_actionList.RemoveAll(Temp_Remove_Action);
        ActioningObjArray[(int)type] = null;
        ActioningArray[(int)type] = 0;
	}
    public void RemoveAction(ActorAction action)
    {
        if (action == null)
        {
            return;
        }
        action.OnInterupt();
        RemoveDisableCount(action.GetActionType());
        m_actionList.Remove(action);
		ReleaseObj (action);
    }
    public void RemoveAll()
    {
        foreach (var action in m_actionList)
        {
            action.OnExit();
            RemoveDisableCount(action.GetActionType());
        }
        m_actionList.Clear();
        for (int i = 0; i < ActioningArray.Length;i++ )
        {
            ActioningArray[i] = 0;
        }
        for (int i = 0; i < ActioningObjArray.Length; i++)
        {
            ActioningObjArray[i] = null;
        }
    }
	public void ReleaseAll()
	{
		for (int i=0;i<m_actionList.Count;i++)
		{
			ActorAction action = m_actionList[i];
			ReleaseObj(action);
		}
		m_actionList.Clear();
	}
	public ActorAction AddAction(ActorAction.ENType newType)
	{
		if (!IsDisable(newType))
		{
            bool isValidateActionState = false;
			for (int index = m_actionList.Count - 1; index >= 0; --index)
			{
				ActorAction item = m_actionList[index];
				if (!item.IsEnable)
				{
					continue;
				}
                if (ActorAction.ENRelation.enReplace == CheckRelation(newType, item.GetActionType()))
				{
                    m_actionList.RemoveAt(index);
                    isValidateActionState = true;

					item.OnInterupt();
					RemoveDisableCount(item.GetActionType());
					item.IsEnable = false;
					ReleaseObj(item);
				}
			}
            ActorAction obj = CreateObj(newType);
			obj.CurrentActor = CurrentActor;
            if (null != CurrentActor.mActorBlendAnim)
            {
                CurrentActor.mActorBlendAnim.CurActionType = newType;
            }
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
                for (int i = 0; i < m_actionList.Count;i++)
                {
                    ActorAction ac = m_actionList[i];
                    ActioningObjArray[(int)ac.GetActionType()] = ac;
                    ActioningArray[(int)ac.GetActionType()]++;
                }
            }
			return obj;
		}
		else
		{
            string currentActions = CurrentActor.GetType() + ":" + newType.ToString() + " Current Actions:";
            foreach (ActorAction action in m_actionList)
            {
                currentActions += action.GetActionType().ToString() + ", ";
            }
            //Debug.LogWarning("Add action failed:" + newType.ToString()+" actor type:"+CurrentActor.Type.ToString());
			return null;
		}
	}
    static private AnyObjectPool m_pool = AnyObjectPoolMgr.Singleton.CreatePool();
	private ActorAction CreateObj(ActorAction.ENType newType )
    {
        ActorAction action = m_pool.GetObjectFromPool(newType) as ActorAction;
        if (action != null)
		{
            return action;
		}
        switch (newType)
        {
        case ActorAction.ENType.enStandAction:
			action = new StandAction();
        	break;	//站立
		case ActorAction.ENType.enMoveAction:
            action = new MoveAction();
			break;	//移动
		case ActorAction.ENType.enAttackAction:
            action = new AttackAction();
			break;	//攻击
		case ActorAction.ENType.enSpasticityAction:
            action = new SpasticityAction();
			break;	//被动僵直
		case ActorAction.ENType.enBeAttackAction:
            action = new BeAttackAction();
			break;	//受击
		case ActorAction.ENType.enUndownAction:
            action = new UndownAction();
			break;	//霸体
		case ActorAction.ENType.enDeadAction:
            action = new DeadAction();
			break;	//死亡
		case ActorAction.ENType.enReliveAction:
            action = new ReliveAction();
			break;	//复活
		case ActorAction.ENType.enPlayEffectAction:
            action = new PlayEffectAction();
			break;	//播放特效
		case ActorAction.ENType.enSearchEnemyAction:
            //action = new SearchEnemyAction();
			break;	//搜索敌人
		case ActorAction.ENType.enTeleportAction:
            action = new TeleportAction();
			break;	//瞬移
        case ActorAction.ENType.enControlMoveAction:
            action = new ControlMoveAction();
        	break;//控制技能定身
		case ActorAction.ENType.enHoldDownAction:
            action = new HoldDownAction();
			break;//按下状态
        case ActorAction.ENType.enSelfSpasticityAction:
            action = new SelfSpasticityAction();
        	break;	//主动僵直
        case ActorAction.ENType.enAlertAction:
            action = new AlertAction();
        	break;      //警戒
        case ActorAction.ENType.enJumpinAction:
            action = new JumpinAction();
        	break;  //入场
        case ActorAction.ENType.enJumpoutAction:
            action = new JumpoutAction();
        	break;   //退场
        case ActorAction.ENType.enRollAction:
            action = new RollAction();
        	break; //翻滚
        case ActorAction.ENType.enFakeBeAttackAction:
            action = new FakeBeAttackAction();
        	break; //假受击
        case ActorAction.ENType.enActorExitAction:
            action = new ActorExitAction();
            break; //主控角色退场
        case ActorAction.ENType.enActorEnterAction:
            action = new ActorEnterAction();
            break; //主控角色入场
        case ActorAction.ENType.enControlAttackAction:
            action = new ControlAttackAction();
            break; //不能攻击
        case ActorAction.ENType.enControlBeAttackAction:
            action = new ControlBeAttackAction();
            break; //不能受击
        case  ActorAction.ENType.enAttackingMoveAction:
            action = new AttackingMoveAction();
            break;//攻击时移动
        case  ActorAction.ENType.enDragMoveAction:
            action = new DragMoveAction();
            break;//拖拽
        default:
            throw new Exception("Miss Action Create for" + newType.ToString());
        }
        return action;
	}
	private void ReleaseObj(ActorAction obj)
	{
		obj.Reset();

        m_pool.ReleaseObject(obj.GetActionType(), obj);
	}
    public void LateUpdate()
    {
//         for (int index = m_actionList.Count - 1; index >= 0; --index)
//         {
//             ActorAction action = m_actionList[index];
//             if (!action.IsInited)
//             {
//                 continue;
//             }
//             if (!action.IsEnable)
// 			{
// 				continue;
// 			}
//             if (action.OnLateUpdate())
//             {
//                 if ((action.GetActionType() == ActorAction.ENType.enMoveAction || action.GetActionType() != ActorAction.ENType.enStandAction))
//                 {
//                     action.OnExit();
//                     RemoveDisableCount(action.GetActionType());
//                     action.IsEnable = false;
// 
//                     m_actionList.RemoveAt(index);
//                     ReleaseObj(action);
//                 }
//             }
//         }
//         if (m_actionList.Count == 0)
//         {
//             AddAction(ActorAction.ENType.enStandAction) as StandAction;
//         }
    }
	public void FixedUpdate()
	{
		for (int index = m_actionList.Count - 1; index >= 0; --index)
		{
			ActorAction action = m_actionList[index];
			if (!action.IsInited)
			{
                action.AnimStartTime = Time.time;
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
        if (m_actionList.Count == 0)
        {
            AddAction(ActorAction.ENType.enStandAction);
        }
	}

	#region DisableCount

	private ActorAction.ENRelation CheckRelation(ActorAction.ENType newAction, ActorAction.ENType oldAction)
	{
		return GameTable.ActionRelationTableAsset.Lookup(newAction, oldAction);
	}

	private int[] m_disableCount = new int[(int)ActorAction.ENType.Count];

	public void AddDisableCount(ActorAction.ENType type)
    {
        ActioningArray[(int)type]++;
		for (int index = 0; index < m_disableCount.Length; ++index)
		{
			if (ActorAction.ENRelation.enUnable == CheckRelation((ActorAction.ENType)index, type))
			{
				++m_disableCount[index];
			}
        }
	}
	private void RemoveDisableCount(ActorAction.ENType type)
    {
        ActioningObjArray[(int)type] = null;
        ActioningArray[(int)type]--;
        if (ActioningArray[(int)type] < 0)
        {
            ActioningArray[(int)type] = 0;
        }
		for (int index = 0; index < m_disableCount.Length; ++index)
		{
			if (ActorAction.ENRelation.enUnable == CheckRelation((ActorAction.ENType)index, type))
			{
				--m_disableCount[index];
			}
		}
	}
	public bool IsDisable(ActorAction.ENType type)
	{
		return m_disableCount[(int)type] > 0;
	}
	#endregion

	public bool InterruptAction(ActorAction.ENType actionType)
	{
		ActorAction item = LookupAction(actionType);
		if (item != null)
		{
            item.OnInterupt();
			item.IsEnable = false;
            return true;
		}
        return false;
	}
};