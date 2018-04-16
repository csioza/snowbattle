using UnityEngine;
using System.Collections;

// LM: 控制升级特效的销毁
public class LvlUpEffTrigger : IWidget
{
    GameObject mObj   = null;
	float m_startTime = 0.0f;
        
	public override void Update()
	{
        if (mObj != null)
        {
            //Vector3 vpos = ActorManager.Singleton.MainActor.MainPos;
            Vector3 vpos = ActorManager.Singleton.MainActor.RealPos;
            mObj.transform.position = vpos;
        }
	}

	public override void FixedUpdate()
	{
		if (Time.realtimeSinceStartup - m_startTime > 2.0f)
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
        IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath("ef-all-w1-levelup-E01"), data);
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
            Vector3 vpos = ActorManager.Singleton.MainActor.MainPos;
            mObj.transform.position = vpos;
            mObj.transform.parent = MainGame.Singleton.MainObject.transform;
            mObj.name = "LvlUpEff";
        }
    }

	public override void Release()
	{
		mObj.SetActive(false);
		IPoolWidget<LvlUpEffTrigger>.ReleaseObj(this);
	}

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
