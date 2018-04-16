using UnityEngine;
using System.Collections;

//新手指导触发区域
public class PlayerGuideTriggerArea : MonoBehaviour
{
    bool m_countDown = false;//是否在倒计时中
    float m_timer;
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
        if (!ClientNet.Singleton.IsConnected || prop.Type != ActorType.enMain)
        {
            return;
        }
        if (prop.ID != ActorManager.Singleton.MainActor.ID)
        {
            return;
        }

        SceneFuncPoint pointComp = gameObject.GetComponent<SceneFuncPoint>();
        if (pointComp == null)
        {
            return;
        }
        if (pointComp.m_pointType != ENScenePointType.triggerArea)
        {
            return;
        }

        //int triggerID = pointComp.m_triggerAreaInfo.m_triggerID;
        //UIPlayerGuide playerGuide = UIManager.Singleton.GetUI<UIPlayerGuide>();
        //playerGuide.ShowWindow();
        //playerGuide.SetTriggerID(triggerID);
        //if (triggerID != 0)
        //{
        //    //发送触发区域事件
        //    ClientNet.Singleton.SendPacket(new AreaOnTriggerPacket(ENTriggerOpt.enEnter, triggerID));
        //}
    }
    void OnTriggerExit(Collider other)
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
        if (!ClientNet.Singleton.IsConnected || prop.Type != ActorType.enMain)
        {
            return;
        }
        if (prop.ID != ActorManager.Singleton.MainActor.ID)
        {
            return;
        }

        SceneFuncPoint pointComp = gameObject.GetComponent<SceneFuncPoint>();
        if (pointComp == null)
        {
            return;
        }
        if (pointComp.m_pointType != ENScenePointType.triggerArea)
        {
            return;
        }

        int triggerID = pointComp.m_triggerAreaInfo.m_triggerID;
        if (triggerID != 0)
        {
            //发送触发区域事件 modify by luozj
            //ClientNet.Singleton.SendPacket(new AreaOnTriggerPacket(ENTriggerOpt.enExit, triggerID));
        }
        if (pointComp.m_triggerAreaInfo.m_triggerType == ENTriggerType.enOnceTrigger)
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
    public void SetTimer(float timer)
    {
        m_countDown = true;
        m_timer = timer;
    }
    public void FixedUpdate()
    {
        if (!m_countDown)
        {
            return;
        }
        m_timer -= Time.deltaTime;
        if (m_timer <= 0)
        {
            m_countDown = false;
            //到时间了 隐藏
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}
