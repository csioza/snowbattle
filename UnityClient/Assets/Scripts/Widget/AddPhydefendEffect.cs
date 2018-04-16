using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AddPhydefendEffect : IWidget
{
    private float m_startTime;
    public float m_endTime;
    GameObject mObj;
    Actor actor = ActorManager.Singleton.MainActor;
    public override void Update()
    {
        if (mObj != null)
        {
            Vector3 vpos = ActorManager.Singleton.MainActor.RealPos;
            mObj.transform.position = vpos;
        }
    }
    public override void FixedUpdate()
    {
        if (Time.realtimeSinceStartup - m_startTime > m_endTime)
        {
            IsEnable = false;
        }
    }
    public override void Init()
    {
        IsEnable = true;
        m_startTime = Time.realtimeSinceStartup;
        MainGame.Singleton.StartCoroutine(Coroutine_LoadEffectObj());
    }
    IEnumerator Coroutine_LoadEffectObj()
    {
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath("ef-all-diaoluo-buff-fangyu"), data);
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
            mObj = data.m_obj as GameObject;
            mObj.transform.position += actor.MainPos;
            mObj.SetActive(true);
        }
    }
    //释放回池
    public override void Release()
    {
        mObj.SetActive(false);
        IPoolWidget<AddPhydefendEffect>.ReleaseObj(this);
    }
    //彻底销毁所有资源
    public override void Destroy()
    {
        IsEnable = false;
        if (null != mObj)
        {
            //GameObject.DestroyObject(mObj);
            PoolManager.Singleton.ReleaseObj(mObj);
            mObj = null;
        }
    }
}

