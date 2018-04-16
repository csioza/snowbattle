using UnityEngine;
using System.Collections;

//触发区域
public enum ENTriggerOpt
{
	enEnter,
	enExit,
}
public enum ENTriggerEventType
{
    enNone,
    enTriggerArrowEvent,
    enTriggerBossWarningEvent,
    enTriggerPlayerEnter,
}
public class TriggerArea : MonoBehaviour
{
    public ENTriggerEventType mEventType = ENTriggerEventType.enNone;
	public void OnTriggerEnter(Collider other)
	{
		if (other.isTrigger)
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
		ActorProp prop = targetObj.parent.GetComponent<ActorProp>();
		if (null == prop)
		{
			return;
		}
// 		if (prop.Type != ActorType.enMain)
// 		{
// 			return;
// 		}
        if (ENTriggerEventType.enTriggerArrowEvent == mEventType)
        {
            ForwardDirectionArrow.Singleton.HideArrow(gameObject);
        }
        else if (ENTriggerEventType.enTriggerBossWarningEvent == mEventType)
        {
//             gameObject.GetComponent<Collider>().isTrigger = false;
//             gameObject.GetComponent<Collider>().enabled = false;
//             SceneManger.Singleton.NotifyChanged((int)SceneManger.ENPropertyChanged.enUIBossRoomWarning, null);
//             CursorEffectFunc.Singleton.Active();
            
        }
        else if (ENTriggerEventType.enTriggerPlayerEnter == mEventType)
        {
            
        }

	}
	void OnTriggerExit(Collider other) 
	{
// 		if (other.isTrigger)
// 		{
// 			return;
// 		}
// 		Transform targetObj = other.transform;
// 		while (null != targetObj && targetObj.name != "body")
// 		{
// 			targetObj = targetObj.parent;
// 		}
// 		if (null == targetObj)
// 		{
// 			return;
// 		}
// 		ActorProp prop = targetObj.parent.GetComponent<ActorProp>();
// 		if (null == prop)
// 		{
// 			return;
// 		}
// 		if (!ClientNet.Singleton.IsConnected || prop.Type != ActorType.enMain)
// 		{
// 			return;
// 		}
// 		if (prop.ID != ActorManager.Singleton.MainActor.ID)
// 		{
// 			return;
// 		}
// 
// 		SceneFuncPoint pointComp = gameObject.GetComponent<SceneFuncPoint>();
// 		if (pointComp == null)
// 		{
// 			return;
// 		}
// 		if (pointComp.m_pointType != ENScenePointType.triggerArea)
// 		{
// 			return;
// 		}
// 
// 		int triggerID = pointComp.m_triggerAreaInfo.m_triggerID;
// 		if (triggerID != 0)
// 		{
// 			//发送触发区域事件
//             //modify by luozj
// 			//ClientNet.Singleton.SendPacket(new AreaOnTriggerPacket(ENTriggerOpt.enExit, triggerID));
// 		}
// 		if (pointComp.m_triggerAreaInfo.m_triggerType == ENTriggerType.enOnceTrigger)
// 		{
// 			gameObject.collider.enabled = false;
// 		}
	}
	public void SetTimer(float timer)
	{
// 		m_countDown = true;
// 		m_timer = timer;
	}
	public void FixedUpdate()
	{
// 		if (!m_countDown)
// 		{
// 			return;
// 		}
// 		m_timer -= Time.deltaTime;
// 		if (m_timer <= 0)
// 		{
// 			m_countDown = false;
// 			//到时间了 隐藏
// 			gameObject.collider.enabled = false;
// 		}
	}
}
