
using System;
using UnityEngine;


public class PickUpCallback : MonoBehaviour
{
    public enum enDropItem
    {
        enGold  =   0,// 金币
        enCard  =   1,// 卡牌
        enHp    =   2,// 血瓶
        enKey   ,     // 钥匙
    }


    private GameObject m_targetObj      = null;
    float yVelocity                     = 0f;
    float zVelocity                     = 0f;
    float xVelocity                     = 0f;

    float svelocitx = 0f;
    float svelocity = 0f;
    float svelocitz = 0f;

    bool m_destory                      = false;

    public enDropItem m_tag = enDropItem.enGold;

    double m_lastTime   ;
    bool m_enter        = true;

    public int m_index = 0;// 用来删除对象的索引

    Vector3 m_desPos;

    void Start()
    {

    }

	void OnTriggerEnter(Collider other)
	{
        //Debug.Log("OnTriggerEnter");
        if ( m_enter == false )
        {
            return;
        }

        GlobalEnvironment.Singleton.IsInCallbackOrTrigger = true;
        try
        {
            Transform targetObj = other.transform;
            while (null != targetObj && targetObj.name != "body")
            {
                targetObj = targetObj.parent;
            }
            if (null != targetObj)
            {


                ActorProp actorProp = targetObj.parent.GetComponent<ActorProp>();
                if (null != actorProp && actorProp.Type == ActorType.enMain)
                {

                    m_targetObj = actorProp.ActorLogicObj.CenterPart;
                   
                    Rigidbody body = transform.gameObject.GetComponent<Rigidbody>();
                    body.useGravity = false;
                    body.isKinematic = true;

                    

                    m_lastTime = TimeUtil.GetCurrentTimeMillis();
                   // Debug.Log("碰到金币了 m_lastTime:" + m_lastTime);
                    m_enter = false;
                }  
            }
          
        }
        catch (Exception e)
        {
            Debug.LogError("Error On PickUpCallback  OnTriggerEnter" + e.Message + ",,,,Stack:" + e.StackTrace.ToString());
        }
        GlobalEnvironment.Singleton.IsInCallbackOrTrigger = false;

        svelocitx = 0f;
        svelocity = 0f;
        svelocitz = 0f;

       
	}

    public void Clear()
    {
        m_enter = true; ;
    }

    void FixedUpdate()
    {
        if ( m_targetObj == null )
        {
            return;
        }

        // 如果距离满足条件 则销毁本身
        if (m_destory)
        {
            m_destory = false;

            switch(m_tag)
            {
                     // 金币行为
                case enDropItem.enGold:
                    {
                        // 播放UI动画
                        UIReward.GetInstance().PlayGoldAnimation();

                        transform.Find("ef-tail").gameObject.SetActive(false);
                        break;
                    }
                    // 契约书行为
                case enDropItem.enCard:
                    {
                        UIReward.GetInstance().PlayCardAnimation();
                        break;
                    }
                // 钥匙
                case enDropItem.enKey:
                    {
                        // 激活 显示获得钥匙
                        Reward.Singleton.EnableKey();
                        break;
                    }
            }
           // Debug.Log("pickupcallback 移除：" + transform.name);
            DropItemPerformance.Singleton.RemoveDropItem(m_index);
            return;
        }

        // 设置位置
        Vector3 targerPos = Vector3.zero  ;

        switch (m_tag)
        {
            // 金币行为
            case enDropItem.enGold:
                {
                    // 获得屏幕某UI对应屏幕像素坐标
                    Vector3 vec         = UICamera.currentCamera.WorldToScreenPoint(UIReward.GetInstance().GetGoldPostion());
                    Ray ray             = Camera.main.ScreenPointToRay(vec);

                    Vector3 desPostion  = ray.GetPoint(5);

                    // 飞到屏幕某点
                    targerPos           = desPostion;
                    break;
                }
            // 契约书行为
            case enDropItem.enCard:
                {
                    Vector3 rage        = new Vector3(0.0f, 0.1f, 0);

                    m_desPos            = new Vector3(m_targetObj.transform.position.x, m_targetObj.transform.position.y, m_targetObj.transform.position.z);

                    transform.RotateAround(m_desPos, rage, 9);

                    float desRotation   = 0.5f;
                    float xx            = Mathf.SmoothDamp(transform.parent.localScale.x, desRotation, ref svelocitx, GameSettings.Singleton.m_pickupTime);
                    float yy            = Mathf.SmoothDamp(transform.parent.localScale.y, 0.5f, ref svelocity, GameSettings.Singleton.m_pickupTime);
                    float zz            = Mathf.SmoothDamp(transform.parent.localScale.z, 0.5f, ref svelocitz, GameSettings.Singleton.m_pickupTime);

                    transform.parent.LocalScaleX(xx);
                    transform.parent.LocalScaleY(yy);
                    transform.parent.LocalScaleZ(zz);

                    // 先绕圈飞一段时间 然后再飞进身体
                    if (TimeUtil.GetCurrentTimeMillis() - m_lastTime < 900)
                    {
                        return;
                    }

                    // 飞到主角身上
                    targerPos           = new Vector3(m_targetObj.transform.position.x, m_targetObj.transform.position.y, m_targetObj.transform.position.z);

                    break;
                }
            // 
            case enDropItem.enHp:
            case enDropItem.enKey:
                {

                    // 飞到主角身上
                    targerPos           = new Vector3(m_targetObj.transform.position.x, m_targetObj.transform.position.y, m_targetObj.transform.position.z);

                    break;
                }
        }
        

        Vector3 selfPos         = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 direction       = targerPos - selfPos;
        direction.y             = 0;

        // 距离够了 则消失
        if (direction.magnitude < 0.2)
        {
            m_destory           = true;
            transform.position  = targerPos;
            return;
        }

        direction.Normalize();

        transform.forward                   = direction;
        //Vector3 forwardDistance             = selfPos + direction * GameSettings.Singleton.m_pickupTime * Time.deltaTime;


        float newPositionY = Mathf.SmoothDamp(transform.position.y, targerPos.y, ref yVelocity, GameSettings.Singleton.m_pickupTime);
        float newPositionZ = Mathf.SmoothDamp(transform.position.z, targerPos.z, ref zVelocity, GameSettings.Singleton.m_pickupTime);
        float newPositionX = Mathf.SmoothDamp(transform.position.x, targerPos.x, ref xVelocity, GameSettings.Singleton.m_pickupTime);

        transform.position = new Vector3(newPositionX, newPositionY, newPositionZ);


        // 如果是金币 则有变小的效果
        if (m_tag == enDropItem.enGold)
        {
            float desRotation = 0.1f;// GameSettings.Singleton.m_dropOffsetX;
            newPositionX = Mathf.SmoothDamp(transform.parent.localScale.x, desRotation, ref svelocitx, GameSettings.Singleton.m_pickupTime);
            newPositionY = Mathf.SmoothDamp(transform.parent.localScale.y, 0.2f/*GameSettings.Singleton.m_dropOffsetY*/, ref svelocity, GameSettings.Singleton.m_pickupTime);
            newPositionZ = Mathf.SmoothDamp(transform.parent.localScale.z, 0.1f/*GameSettings.Singleton.m_dropOffsetz*/, ref svelocitz, GameSettings.Singleton.m_pickupTime);

            transform.parent.LocalScaleX(newPositionX);
            transform.parent.LocalScaleY(newPositionY);
            transform.parent.LocalScaleZ(newPositionZ);
        }
    }

};
