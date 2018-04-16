using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;


public class RemoteAttackManager
{
    #region Singleton
    static RemoteAttackManager m_singleton;
    static public RemoteAttackManager Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new RemoteAttackManager();
            }
            return m_singleton;
        }
    }
    #endregion
	public class Info
	{
        //飞行道具的释放者
        public Actor m_srcActor;
        //飞行道具的目标
        public Actor m_dstActor;
        //源点
        public Vector3 m_srcPos = Vector3.zero;
        //目标点
        public Vector3 m_targetPos = Vector3.zero;
        //飞行道具的表信息
        public FlyingItemInfo m_itemInfo = null;
        //飞行道具的释放者的ActorType
        public ActorType m_actorType;
        //飞行道具的释放者的阵营
        public ENCamp m_actorCamp;
        //是否需要删除
        public bool m_isNeedRemove = false;
        //飞行道具的对象
        public GameObject m_itemObj = null;
        //飞行道具上的Rigidbody
        public Rigidbody m_rigidbody = null;
        //飞行的方向
        public Vector3 m_forward = Vector3.zero;
        //曲线移动的方向
        public Vector3 m_curveForward = Vector3.zero;
        //开始飞行的时间
        public float m_startTime = 0;
        //持续时间
        public float m_duration = 0;
        //开始飞行的位置
        public Vector3 m_startPos = Vector3.zero;
        //预警特效
        public GameObject m_objWarningEffect = null;
        //是否有预警特效
        public bool m_isHaveWarningEffect = false;
        //camera是否已经返回到原目标
        public bool m_cameraIsBack = false;
        //技能id
        public int m_skillID = 0;
        //是否立即发射
        public bool m_isLaunchSuspend = false;
        //生成或跟随时的transform
        public Transform m_boneT = null;
        //反弹次数
        public int m_bouncyCount = 0;
        //反弹列表
        public List<int> m_bouncyTargetIDList = null;

        public Info(FlyingItemInfo itemInfo)
        {
            m_itemInfo = itemInfo;
            m_duration = m_itemInfo.Item_ExistTime;
            m_isLaunchSuspend = m_itemInfo.IsLaunchSuspend;
            m_bouncyCount = m_itemInfo.Item_BouncyCount;
        }
	}
    //所有的远程攻击特效的列表
    private List<Info> m_infoList = new List<Info>();

	public void Add(RemoteAttackManager.Info info)
    {
        MainGame.Singleton.StartCoroutine(Coroutine_Add(info));
    }
    IEnumerator Coroutine_Add(RemoteAttackManager.Info info)
    {
        //飞行道具
        if (string.IsNullOrEmpty(info.m_itemInfo.Item_Name))
        {
            Debug.LogWarning("effect name is null");
        }
        else
        {
            GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
            IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath(info.m_itemInfo.Item_Name), data, true);
            while (true)
            {
                e.MoveNext();
                if (data.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
            if (data.m_obj == null)
            {
                Debug.LogWarning("load effect obj fail, effect name is " + info.m_itemInfo.Item_Name);
            }
            else
            {
                info.m_itemObj = data.m_obj as GameObject;
                //获得脚本
                FlyingItemCallback callback = info.m_itemObj.GetComponent<FlyingItemCallback>();
                if (callback == null)
                {
                    //从子中获得脚本
                    callback = info.m_itemObj.GetComponentInChildren<FlyingItemCallback>();
                    if (callback == null)
                    {//未获得脚本，添加脚本在collider上
                        //获得collider
                        Collider c = info.m_itemObj.GetComponent<Collider>();
                        if (c == null)
                        {
                            //从子中获得collider
                            c = info.m_itemObj.GetComponentInChildren<Collider>();
                        }
                        if (c != null)
                        {
                            callback = c.transform.gameObject.AddComponent<FlyingItemCallback>();
                        }
                        else
                        {
                            if (!info.m_itemInfo.IsStretch)
                            {//拉伸道具不使用collider
                                Debug.LogWarning("fly obj is not exist collider, fly obj id = " + info.m_itemInfo.ID);
                            }
                        }
                    }
                }
                if (callback != null)
                {
                    callback.Init(info);
                }
                //获得Rigidbody
                Rigidbody rBody = info.m_itemObj.GetComponent<Rigidbody>();
                if (rBody == null)
                {
                    rBody = info.m_itemObj.GetComponentInChildren<Rigidbody>();
                }
                info.m_rigidbody = rBody;

                if (!string.IsNullOrEmpty(info.m_itemInfo.Item_BoneName))
                {//起始bone
                    info.m_boneT = info.m_srcActor.LookupBone(info.m_srcActor.MainObj.transform, info.m_itemInfo.Item_BoneName);
                    if (info.m_boneT == null)
                    {//没有bone，则出生在CenterCollider
                        if (info.m_srcActor.CenterCollider != null)
                        {
                            info.m_boneT = info.m_srcActor.CenterCollider.transform;
                        }
                        info.m_itemObj.transform.position = info.m_srcActor.RealPos;
                        Debug.LogWarning("lookup bone fail, bone name is " + info.m_itemInfo.Item_BoneName);
                    }
                    else
                    {
                        info.m_itemObj.transform.position = info.m_boneT.position;
                    }
                }
                else
                {
                    if (info.m_targetPos != Vector3.zero)
                    {//没有起始bone，则出生在目标点
                        info.m_itemObj.transform.position = info.m_targetPos;
                    }
                    else
                    {//没有目标点，则出生在CenterCollider
                        info.m_itemObj.transform.position = info.m_srcActor.RealPos;
                    }
                    if (info.m_srcActor.CenterCollider != null)
                    {
                        info.m_boneT = info.m_srcActor.CenterCollider.transform;
                    }
                }
                if (!string.IsNullOrEmpty(info.m_itemInfo.Item_BoneOffset))
                {
                    Vector3 pos = info.m_itemObj.transform.position;
                    string[] param = info.m_itemInfo.Item_BoneOffset.Split(new char[1] { ',' });
                    Vector3 offset = new Vector3(float.Parse(param[0]), float.Parse(param[1]), float.Parse(param[2]));
                    pos = pos + info.m_boneT.rotation * offset;
                    info.m_itemObj.transform.position = pos;
                }
                if (info.m_srcPos != Vector3.zero)
                {
                    info.m_itemObj.transform.position = info.m_srcPos;
                }
                if (info.m_itemInfo.Item_MoveSpeed == 0)
                {
                    Vector3 currentForward = info.m_itemObj.transform.eulerAngles;
                    currentForward.y = info.m_srcActor.MainObj.transform.eulerAngles.y;
                    info.m_itemObj.transform.eulerAngles = currentForward;
                }
                info.m_startPos = info.m_itemObj.transform.position;
                info.m_actorType = info.m_srcActor.Type;
                if (info.m_actorType != info.m_srcActor.TempType)
                {//使用更改后的ActorType
                    info.m_actorType = info.m_srcActor.TempType;
                }
                info.m_actorCamp = (ENCamp)info.m_srcActor.Camp;
                if (info.m_actorCamp != info.m_srcActor.TempCamp)
                {
                    info.m_actorCamp = info.m_srcActor.TempCamp;
                }
                if (info.m_itemInfo.IsWarningBeforeItem)
                {
                    info.m_isHaveWarningEffect = true;
                    //方向
                    if (info.m_targetPos != Vector3.zero)
                    {//有目标，起始点为道具出生点
                        if (info.m_itemInfo.Item_IsLockY)
                        {
                            info.m_targetPos.y = info.m_startPos.y;
                        }
                        Vector3 direction = info.m_targetPos - info.m_startPos;
                        direction.Normalize();
                        info.m_forward = direction;
                    }
                    else
                    {//无目标
                        info.m_forward = info.m_srcActor.MainObj.transform.forward;
                    }
                    {//预警特效
                        GameResPackage.AsyncLoadObjectData data2 = new GameResPackage.AsyncLoadObjectData();
                        e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath(info.m_itemInfo.WarningEffectName), data2, true);
                        while (true)
                        {
                            e.MoveNext();
                            if (data2.m_isFinish)
                            {
                                break;
                            }
                            yield return e.Current;
                        }
                        if (data2.m_obj != null)
                        {
                            info.m_objWarningEffect = data2.m_obj as GameObject;
                            info.m_objWarningEffect.transform.localPosition = info.m_targetPos;
                        }
                    }
                    //info.m_objWarningEffect = PoolManager.Singleton.CreateEffectObj(info.m_itemInfo.WarningEffectName);
                    //info.m_objWarningEffect.transform.localPosition = info.m_targetPos;
                    info.m_startTime = Time.time;
                    info.m_itemObj.SetActive(false);
                }
                else
                {
                    //方向
                    if (info.m_targetPos != Vector3.zero)
                    {//有目标，起始点为collider
                        if (info.m_itemInfo.Item_IsLockY)
                        {
                            info.m_targetPos.y = info.m_startPos.y;
                        }
                        Vector3 direction = info.m_targetPos - info.m_startPos;
                        direction.Normalize();
                        info.m_forward = direction;
                    }
                    else
                    {//无目标
                        info.m_forward = info.m_srcActor.MainObj.transform.forward;
                    }
                }
                info.m_cameraIsBack = false;
                if (info.m_itemObj.activeSelf)
                {//没有预警特效，直接改变焦点
                    if (info.m_itemInfo.IsChangeCamera)
                    {
                        if (info.m_actorType == ActorType.enMain)
                        {
                            MainGame.Singleton.MainCamera.ChangeFollowTarget(info.m_itemObj, info.m_itemInfo.CC_ChangeTime, info.m_itemInfo.CC_BackTime, false);
                        }
                    }
                }
                if (info.m_itemInfo.IsCurveMove)
                {//曲线移动，设置曲线移动的方向
                    string[] array = info.m_itemInfo.Item_AddtionalDirection.Split(new char[1] { ',' });
                    int x = 0, y = 0, z = 0;
                    if (array.Length > 0)
                    {
                        x = int.Parse(array[0]);
                    }
                    if (array.Length > 1)
                    {
                        y = int.Parse(array[1]);
                    }
                    if (array.Length > 2)
                    {
                        z = int.Parse(array[2]);
                    }
                    info.m_curveForward = info.m_forward - new Vector3(x, y, z);
                    //Debug.LogError("old forward:"+info.m_forward.ToString()+",new forward:"+info.m_curveForward.ToString());
                }
                info.m_itemObj.transform.localScale = Vector3.one;
                if (info.m_itemInfo.IsStretch)
                {//拉伸-z轴
                    float d = 0;
                    if (info.m_dstActor != null)
                    {
                        d = ActorTargetManager.GetTargetDistance(info.m_srcActor.MainPos, info.m_dstActor.MainPos);
                    }
                    info.m_duration = d / info.m_itemInfo.ZAxisLongth / info.m_itemInfo.Item_StretchSpeed;
                    //Debug.LogError("remote attack,d:" + d + ",z:" + info.m_itemInfo.ZAxisLongth + ",speed:" + info.m_itemInfo.Item_StretchSpeed);
                    info.m_itemObj.transform.forward = info.m_forward;
                    info.m_itemObj.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                }
                info.m_itemObj.SetActive(false);
                m_infoList.Add(info);
            }
        }
    }
    public void Clear()
    {
        foreach (var item in m_infoList)
        {
            PoolManager.Singleton.ReleaseObj(item.m_objWarningEffect);
            PoolManager.Singleton.ReleaseObj(item.m_itemObj);
        }
        m_infoList.Clear();
    }

	public void Update()
	{
        for (int i = 0; i < m_infoList.Count; ++i)
        {
            Info info = m_infoList[i];
            //使用异步时，需要修改此代码 added by luozj
            if (info.m_itemInfo.IsWarningBeforeItem && info.m_isHaveWarningEffect)
            {//预警特效，并且没播放完毕
                if (Time.time - info.m_startTime > info.m_itemInfo.WaringEffectDuration)
                {
                    info.m_isHaveWarningEffect = false;
                    if (null != info.m_objWarningEffect)
                    {
                        PoolManager.Singleton.ReleaseObj(info.m_objWarningEffect);
                        info.m_objWarningEffect = null;
                    }
                    info.m_startTime = Time.time;
                    info.m_itemObj.SetActive(true);
                    //预警特效播放完毕之后，改变焦点
                    if (info.m_itemInfo.IsChangeCamera)
                    {
                        if (info.m_actorType == ActorType.enMain)
                        {
                            MainGame.Singleton.MainCamera.ChangeFollowTarget(info.m_itemObj, info.m_itemInfo.CC_ChangeTime, info.m_itemInfo.CC_BackTime, false);
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
            if (!info.m_itemObj.activeSelf)
            {//激活
                info.m_itemObj.SetActive(true);
            }
            if (info.m_rigidbody != null)
            {
                info.m_rigidbody.WakeUp();
            }
            if (info.m_startTime == 0)
            {//设置开始时间
                info.m_startTime = Time.time;
                if (info.m_itemInfo.IsStretch)
                {//拉伸，直接对目标进行一次伤害
                    if (info.m_dstActor != null)
                    {
                        IResult r = BattleFactory.Singleton.CreateResult(ENResult.Skill, info.m_srcActor.ID, info.m_dstActor.ID,
                        info.m_itemInfo.Item_ResultID, info.m_skillID);
                        if (r != null)
                        {
                            info.m_dstActor.SetBlastPos(info.m_itemObj.transform.position, info.m_itemObj.transform.forward);
                            r.ResultExpr(null);
                            BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
                        }
                    }
                }
            }
            if (info.m_duration != 0)
            {//时间
                if (Time.time - info.m_startTime > info.m_duration)
                {
                    info.m_isNeedRemove = true;
                    continue;
                }
            }
            if (info.m_itemInfo.Item_MoveDistance != 0)
            {//距离
                Vector3 moveDistance = info.m_itemObj.transform.position - info.m_startPos;
                if (moveDistance.magnitude > info.m_itemInfo.Item_MoveDistance)
                {
                    info.m_isNeedRemove = true;
                    continue;
                }
            }
            if (info.m_itemInfo.Item_IsDestroyAtTargetPos)
            {//飞行道具是否在目标位置上销毁
                Vector3 moveDistance = info.m_targetPos - info.m_itemObj.transform.position;
                if (Mathf.Abs(moveDistance.magnitude) <= 0.4f)
                {
                    info.m_itemObj.transform.position = info.m_targetPos;
                    info.m_isNeedRemove = true;
                    continue;
                }
            }
            if (info.m_isNeedRemove)
            {
                continue;
            }
            if (info.m_isLaunchSuspend)
            {//延缓发射
                continue;
            }
            if (info.m_itemInfo.IsChangeCamera)
            {//改变camera
                if (!info.m_cameraIsBack)
                {
                    //camera back
                    if (Time.time - info.m_startTime > info.m_itemInfo.CC_Duration + info.m_itemInfo.CC_ChangeTime)
                    {
                        MainGame.Singleton.MainCamera.BackToActor(info.m_itemObj, info.m_itemInfo.CC_BackTime);
                        info.m_cameraIsBack = true;
                    }
                }
            }
            if (info.m_itemInfo.IsCurveMove)
            {//曲线移动
                if (Time.time - info.m_startTime <= info.m_itemInfo.Item_AddtionalEffectTime)
                {//曲线移动时间内
                    if (info.m_itemInfo.Item_AddtionalSpeed != 0)
                    {//移动
                        info.m_itemObj.transform.forward = info.m_curveForward;
                        Vector3 forwardDistance = info.m_itemObj.transform.localPosition + info.m_curveForward * info.m_itemInfo.Item_AddtionalSpeed * Time.deltaTime;
                        info.m_itemObj.transform.localPosition = forwardDistance;
                        //计算朝向目标的方向
                        //Vector3 direction = info.m_targetPos - info.m_itemObj.transform.position;
                        //direction.Normalize();
                        //info.m_forward = direction;
                    }
                    continue;
                }
            }
            if (info.m_itemInfo.IsTrack)
            {//追踪目标
                if (!info.m_itemInfo.IsWarningBeforeItem && info.m_dstActor != null && !info.m_dstActor.IsDead)
                {//没有目标预警
                    info.m_targetPos = info.m_dstActor.RealPos;
                    if (info.m_itemInfo.Item_IsLockY)
                    {
                        info.m_targetPos.y = info.m_itemObj.transform.position.y;
                    }
                    Vector3 direction = info.m_targetPos - info.m_itemObj.transform.position;
                    direction.Normalize();
                    info.m_forward = direction;
                }
            }
            if (info.m_itemInfo.Item_MoveSpeed != 0)
            {//移动
                info.m_itemObj.transform.forward = info.m_forward;
                Vector3 forwardDistance = info.m_itemObj.transform.localPosition + info.m_forward * info.m_itemInfo.Item_MoveSpeed * Time.deltaTime;
                info.m_itemObj.transform.localPosition = forwardDistance;
            }
            else
            {
                if (info.m_itemInfo.IsStretch)
                {//拉伸-z轴
                    float z = info.m_itemObj.transform.localScale.z;
                    info.m_itemObj.transform.LocalScaleX(1);
                    info.m_itemObj.transform.LocalScaleY(1);
                    info.m_itemObj.transform.LocalScaleZ(z + info.m_itemInfo.Item_StretchSpeed*Time.deltaTime);
                }
            }
        }
        for (int j = 0; j < m_infoList.Count; ++j)
        {
            Info info = m_infoList[j];
            if (info.m_isNeedRemove)
            {
                if (info.m_dstActor != null)
                {
                    StartBouncy(info, info.m_dstActor.MainPos);
                }
                if (info.m_itemInfo.IsChangeCamera)
                {
                    if (!info.m_cameraIsBack)
                    {//camera back
                        if (info.m_itemInfo.CC_DurationAfterDestroy > 0)
                        {
                            MainGame.Singleton.MainCamera.ChangeFollowTarget(info.m_itemObj.transform.position, 0, info.m_itemInfo.CC_BackTime, false, info.m_itemInfo.CC_DurationAfterDestroy);
                        }
                        else
                        {
                            MainGame.Singleton.MainCamera.BackToActor(info.m_itemObj, info.m_itemInfo.CC_BackTime);
                        }
                        info.m_cameraIsBack = true;
                    }
                }
                if (info.m_itemInfo.IsResultForRemove)
                {//移除后是否有效果
                    SkillResultInfo srInfo = GameTable.SkillResultTableAsset.Lookup(info.m_itemInfo.ResultIDForRemove);
                    GetTargetList(info.m_itemObj.transform.localPosition, info.m_actorType, info.m_actorCamp, ENTargetType.enEnemy, srInfo.InstantRange);
                    for (int index = 0; index < m_targetIDList.Count; ++index)
                    {
                        //产生skillResult
                        IResult r = BattleFactory.Singleton.CreateResult(ENResult.Skill, info.m_srcActor.ID, m_targetIDList[index],
                            info.m_itemInfo.ResultIDForRemove, info.m_skillID);
                        if (r != null)
                        {
                            Actor targetActor = ActorManager.Singleton.Lookup(m_targetIDList[index]);
                            targetActor.SetBlastPos(info.m_itemObj.transform.position, info.m_itemObj.transform.forward);
                            r.ResultExpr(null);
                            BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
                        }
                    }
                }
                if (info.m_itemInfo.IsEffectForRemove)
                {//是否在移除飞行特效后播放特效
                    PlayEffect(info.m_itemInfo.EffectNameForRemove, info.m_itemInfo.EffectTimeForRemove, info.m_itemObj.transform.localPosition);
                }
                PoolManager.Singleton.ReleaseObj(info.m_itemObj);
            }
        }
        m_infoList.RemoveAll(item => item.m_isNeedRemove);
	}
    //搜寻目标的范围
    float m_searchTargetRange = 0;
    //搜寻目标的类型
    ENTargetType m_searchTargetType = ENTargetType.enNone;
    //ActorType
    ActorType m_selfActorType;
    //
    ENCamp m_selfCamp;
    //搜寻范围内的目标列表
    List<int> m_targetIDList = new List<int>();
    //自己的位置
    Vector3 m_selfPos = Vector3.zero;
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
        if (target.IsDead)
        {
            return;
        }
        switch (m_searchTargetType)
        {
            case ENTargetType.enEnemy:
                {
                    if (!ActorTargetManager.IsEnemy(m_selfCamp, m_selfActorType, target))
                    {
                        return;
                    }
                }
                break;
            case ENTargetType.enFriendly:
                {
                    if (!ActorTargetManager.IsFriendly(m_selfCamp, m_selfActorType, target))
                    {
                        return;
                    }
                }
                break;
            case ENTargetType.enSelf:
                {
                    if (m_selfActorType != target.Type)
                    {
                        return;
                    }
                }
                break;
            case ENTargetType.enNullTarget:
                break;
            case ENTargetType.enFriendlyAndSelf:
                {
                    if (!ActorTargetManager.IsFriendly(m_selfCamp, m_selfActorType, target) && m_selfActorType != target.Type)
                    {
                        return;
                    }
                }
                break;
            default:
                break;
        }
        Vector3 distance = target.RealPos - m_selfPos;
        distance.y = 0;
        if (m_searchTargetRange >= distance.magnitude)
        {
            m_targetIDList.Add(target.ID);
        }
    }
    //获得一定范围内的目标的列表
    void GetTargetList(Vector3 selfPos, ActorType actorType, ENCamp actorCamp, ENTargetType type, float range)
    {
        m_selfPos = selfPos;
        m_selfActorType = actorType;
        m_selfCamp = actorCamp;
        m_searchTargetType = type;
        m_searchTargetRange = range;
        m_targetIDList.Clear();
        ActorManager.Singleton.ForEach(CheckTarget);
    }
    void PlayEffect(string effectName, float effectTime, Vector3 pos)
    {
        if (false == GameSettings.Singleton.m_playEffect)
        {
            return;
        }

        //GameObject sceneEffect = PoolManager.Singleton.CreateEffectObj(effectName,false);
        //if (null == sceneEffect)
        //{
        //    return null;
        //}
        //sceneEffect.transform.position = pos;
        //sceneEffect.SetActive(true);
        //MainGame.Singleton.StartCoroutine(RemoveEffect2(sceneEffect, effectTime));
        //return sceneEffect;
        MainGame.Singleton.StartCoroutine(Coroutine_PlayEffect(effectName, effectTime, pos));
    }
    IEnumerator Coroutine_PlayEffect(string name, float duration, Vector3 pos)
    {
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath(name), data);
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
            GameObject obj = data.m_obj as GameObject;
            obj.transform.position = pos;
            obj.SetActive(true);
            yield return new WaitForSeconds(duration);
            if (null != obj)
            {
                PoolManager.Singleton.ReleaseObj(obj);
            }
        }
    }
    //private IEnumerator RemoveEffect2(GameObject obj, float duration)
    //{
    //    yield return new WaitForSeconds(duration);
    //    {
    //        if (null != obj)
    //        {
    //            PoolManager.Singleton.ReleaseObj(obj);
    //        }
    //    }
    //}
    //开始反弹
    public void StartBouncy(Info info, Vector3 startPos)
    {
        if (!info.m_itemInfo.IsBouncy)
        {//没有反弹
            return;
        }
        if (info.m_bouncyCount <= 0)
        {//没有反弹次数
            return;
        }
        //查找目标
        GetTargetList(startPos, info.m_actorType, info.m_actorCamp, ENTargetType.enEnemy, info.m_itemInfo.Item_BouncyDistance);
        if (info.m_bouncyTargetIDList == null)
        {
            info.m_bouncyTargetIDList = new List<int>();
            if ((FlyingItemInfo.ENBouncyType)info.m_itemInfo.Item_BouncyType == FlyingItemInfo.ENBouncyType.enNotRepeat)
            {
                info.m_bouncyTargetIDList.Add(info.m_dstActor.ID);
            }
        }
        for (int i = 0; i < m_targetIDList.Count; )
        {
            int id = m_targetIDList[i];
            if (info.m_dstActor != null && id == info.m_dstActor.ID)
            {
                m_targetIDList.RemoveAt(i);
                continue;
            }
            switch ((FlyingItemInfo.ENBouncyType)info.m_itemInfo.Item_BouncyType)
            {
                case FlyingItemInfo.ENBouncyType.enRandom:
                    break;
                case FlyingItemInfo.ENBouncyType.enNotRepeat:
                    {
                        if (info.m_bouncyTargetIDList.Contains(id))
                        {//已经反弹过该目标
                            m_targetIDList.RemoveAt(i);
                            continue;
                        }
                    }
                    break;
            }
            ++i;
        }
        if (m_targetIDList.Count != 0)
        {
            int index = UnityEngine.Random.Range(0, m_targetIDList.Count);
            Actor target = ActorManager.Singleton.Lookup(m_targetIDList[index]);
            RemoteAttackManager.Info newInfo = new RemoteAttackManager.Info(info.m_itemInfo);
            newInfo.m_srcActor = info.m_srcActor;
            newInfo.m_dstActor = target;
            startPos.y = info.m_itemObj.transform.position.y;
            newInfo.m_srcPos = startPos;
            newInfo.m_targetPos = target.MainPos;
            newInfo.m_skillID = info.m_skillID;
            newInfo.m_bouncyCount = --info.m_bouncyCount;
            newInfo.m_bouncyTargetIDList = new List<int>();
            newInfo.m_bouncyTargetIDList.AddRange(info.m_bouncyTargetIDList);
            if (!newInfo.m_bouncyTargetIDList.Contains(target.ID))
            {
                newInfo.m_bouncyTargetIDList.Add(target.ID);
            }
            RemoteAttackManager.Singleton.Add(newInfo);
        }
    }
}