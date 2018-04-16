using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBase
{
    //!room 是否进入战斗
    public bool m_isBattleState = false;
    public RoomBase() { }
    virtual public bool IsBattleState()
    {
        return m_isBattleState;
    }
    virtual public void EnterBattleState()
    {

    }
    virtual public bool Tick()
    {
        return false;
    }
}
namespace SM
{
    // 场景中单一房间管理器
    public class SceneRoom : RoomBase
    {
        public static float blockWidth  = 1.25f;
        public static float blockHeight = 1.25f;
        //////////////////////////////////////////////////////////////////////////
        // room guid
        public int                  ID = 0;
        // cur room data
        public SceneRoomInfoTree    CurRoomInfo = null;
        // monster list
        public List<NPC>            mNpcList = new List<NPC>();
        //宝箱列表
        public List<NPC>            mTreasureList = new List<NPC>();
        //机关列表
        public List<Trap>           mTrapList = new List<Trap>();
        //该房间的怪物数据
        public Dictionary<int, MonsterRoomData> mMonsterDataDict = new Dictionary<int, MonsterRoomData>();
        //该房间内的宝箱数据
        public Dictionary<int, MonsterRoomData> mTreasureDataDict = new Dictionary<int, MonsterRoomData>();
        //房间内的机关数据
        public Dictionary<int, TrapData> mTrapDataDict = new Dictionary<int, TrapData>();
        //
        public Dictionary<int, int> mSatisfyIDMap = new Dictionary<int, int>();
        //is active
        public bool                 mIsAcitve = false;
        // 怪物刷新列表 
        List<ActorRefresh>          m_actorRefreshList = new List<ActorRefresh>();
        //宝箱刷新列表
        List<TreasureRefresh>       m_treasureRefreshList = new List<TreasureRefresh>();
        //机关刷新列表
        public List<TrapRefresh>           m_trapRefreshList = new List<TrapRefresh>();
        //当前怪物波数
        int                         m_curMonstersNum = 0;
        //
        public int CurMonstersNum { get { return m_curMonstersNum; } private set { m_curMonstersNum = value; } }
        //
        public GameObject                  m_parentObj = null;
        //上一房间
        public SceneRoom                   m_preRoom = null;
        //当前房间是否关闭状态
        public bool IsOpenState        = false;
        //是否杀死所有怪物
        public bool m_IsSkillAllMonster = false;
        //是否打开所有宝箱
        public bool m_IsOpenAllTreasure = false;
        //是否激活所有机关
        public bool m_IsActiveAllTrap = false;

        //房间进入可战斗状态
        public bool mStarBattleActive = false;
  /// <summary>
  /// ////////////////////////////////////////////////////////////////////////////////
  /// </summary>
        public SceneRoom(SceneRoomInfoTree curRoomInfo, SceneRoom preRoom)
        {
            CurRoomInfo         = curRoomInfo;
            ID                  = CurRoomInfo.m_GUID;
            m_curMonstersNum    = 0;
            m_preRoom           = preRoom;
        }
        public void Destroy()
        {
            for (int i = 0; i < mNpcList.Count; i++ )
            {
                if (null != mNpcList[i])
                {
                    ActorManager.Singleton.ReleaseActor(mNpcList[i].ID);
                }
            }
            mStarBattleActive = false;
            mNpcList.Clear();
            m_actorRefreshList.Clear();
            mMonsterDataDict.Clear();
            GameObject.Destroy(m_parentObj);
        }
        public override void EnterBattleState()
        {
            m_isBattleState = true;
            Actor target = ActorManager.Singleton.MainActor.TargetManager.CurrentTarget;
            if (null != target && target.Type == ActorType.enNPC)
            {
                NPC npc = target as NPC;
                if (npc.GetNpcType() == ENNpcType.enBoxNPC)
                {
                    ActorManager.Singleton.MainActor.TargetManager.CurrentTarget = null;
                }
            }
        }
        void CheckAllMonsterDead()
        {
            foreach (KeyValuePair<int, MonsterRoomData> val in mMonsterDataDict)
            {
                if (val.Value.isRefresh)
                {
                    Actor actor = ActorManager.Singleton.Lookup(val.Value.outDyncId);
                    if (null != actor && !actor.IsRealDead)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            SM.RandomRoomLevel.Singleton.NotifyChanged((int)SM.RandomRoomLevel.ENPropertyChanged.enEndBattle, null);
            mIsAcitve = false;
            m_isBattleState = false;
        }
        public Actor GetMonster(int monsterID)
        {
            MonsterRoomData data = null;
            Actor actor = null;
            if (mMonsterDataDict.TryGetValue(monsterID, out data))
            {
                actor = ActorManager.Singleton.Lookup(data.outDyncId);
            }
            return actor;
        }
        public Actor GetMonsterByID(int monsterId)
        {
            MonsterRoomData data = null;
            Actor actor = null;
            if (mMonsterDataDict.TryGetValue(monsterId, out data))
            {
                actor = ActorManager.Singleton.Lookup(data.outDyncId);
                if (null == actor || actor.IsRealDead)
                {
                    return null;
                }
            }
            return actor;
        }

        public Actor GetTreatureByID(int treasureId)
        {
            MonsterRoomData data = null;
            Actor actor = null;
            if (mTreasureDataDict.TryGetValue(treasureId, out data))
            {
                actor = ActorManager.Singleton.Lookup(data.outDyncId);
//                 if (null == actor || actor.IsRealDead)
//                 {
//                     return null;
//                 }
            }
            return actor;
        }
        public Actor GetTrapByID(int trapId)
        {
            TrapData data = null;
            Actor actor = null;
            if (mTrapDataDict.TryGetValue(trapId, out data))
            {
                actor = ActorManager.Singleton.Lookup(data.outDyncId);
                if (null == actor)
                {
                    return null;
                }
            }
            return actor;
        }
        public Gate GetGateByID(int GateId)
        {
            Gate data = null;
            data = CurRoomInfo.m_gateList[GateId];
            return data;
        }
        public ActorRefresh GetActorRefreshById(int monsterId)
        {
            foreach (var item in m_actorRefreshList)
            {
                if (item.IsMonsterDataId(monsterId))
                {
                    return item;
                }
            }
            return null;
        }
        public bool IsMonsterDead(int monsterId)
        {
            MonsterRoomData data = null;
            if (mMonsterDataDict.TryGetValue(monsterId, out data))
            {
                if (data.isRefresh)
                {
                    Actor actor = ActorManager.Singleton.Lookup(data.outDyncId);
                    if (null == actor || actor.IsRealDead)
                    {
                        return true;
                    }
                }
               
            }
            return false;
        }
        public void BuildRoomData(int levelId)
        {
            // generate monster data
            m_parentObj = GameObject.Instantiate(CurRoomInfo.m_roomData) as GameObject;
            m_parentObj.transform.localPosition = Vector3.zero;
            m_parentObj.transform.position      = CurRoomInfo.m_roomObj.transform.position;
            RoomDataCfg dataCfg = m_parentObj.GetComponent<RoomDataCfg>();
            for (int i = 1; i <= dataCfg.MonsterCount; i++)
            {
                string str;
                if (i<10)
                {
                    str = string.Format("monster0{0}", i);
                }
                else
                {
                    str = string.Format("monster{0}", i);
                }
                GameObject obj = m_parentObj.transform.Find(str).gameObject;
                RoomElement ele = obj.GetComponent<RoomElement>();
                ele.MonsterData = new SM.MonsterRoomData();
                ele.MonsterData.monsterId = i;
                ele.MonsterData.monsterObjId = ele.ObjSettingID;
                if (ele._PatrolList.Count <= 0)
                {
                    ele._PatrolList = dataCfg._GlobalPatrolList;
                }
                ActorRefresh actor = new ActorRefresh(obj.transform, this);
                m_actorRefreshList.Add(actor);
            }
            //出生点
            if (null == CurRoomInfo.m_parent)
            {
                Transform posTrans = m_parentObj.transform.Find("charPosition");
                if (null == posTrans)
                {
                    string objName = "charposition";
                    GameObject childObj = new GameObject(objName);
                    childObj.transform.parent = m_parentObj.transform;
                    childObj.transform.localPosition = new Vector3(CurRoomInfo.m_charPosition.x * blockWidth, 0f, CurRoomInfo.m_charPosition.z * blockHeight);
                    CurRoomInfo.CharPosition = new Vector3(childObj.transform.position.x, 0.15f, childObj.transform.position.z); ;
                    CurRoomInfo.CharPosTransform = childObj.transform;
                }
                else
                {
                    CurRoomInfo.CharPosition = new Vector3(posTrans.position.x, 0.15f, posTrans.position.z); ;
                    CurRoomInfo.CharPosTransform = posTrans;
                }

            }
            //激活检测开始战斗的刚体
            if (CurRoomInfo.m_monsterDataList.Count > 0)
            {
                Transform starBattleColliderTans = CurRoomInfo.m_roomObj.transform.Find("activeArea");
                if (starBattleColliderTans != null)
                {
                    for (int i = 0; i < starBattleColliderTans.transform.childCount; i++)
                    {
                        Transform child = starBattleColliderTans.transform.GetChild(i);
                        TriggerCallback triggerCallback = child.gameObject.AddComponent<TriggerCallback>();
                        triggerCallback.EnterCallback = OnCheckEnterCallBack;
                    }
                }
            }
        }

        IEnumerator Coroutine_LoadTeleport(int levelId)
        {
            SceneTeleportInfo teleInfo = GameTable.SceneTeleportTableAsset.Lookup(CurRoomInfo.m_teleportData.objId);
            GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
            IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetPrefabPath(teleInfo.prefabName), data);
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
                GameObject teleportObj = data.m_obj as GameObject;
                teleportObj.transform.parent = m_parentObj.transform;
                LevelTeleportDoor teleport = teleportObj.AddComponent<LevelTeleportDoor>();
                teleport.LevelId = levelId;
                teleport.Target = CurRoomInfo.m_teleportData.target;
                teleportObj.transform.localPosition = new Vector3(CurRoomInfo.m_teleportData.position.x * blockHeight, 0f, CurRoomInfo.m_teleportData.position.z * blockHeight);
                CurRoomInfo.m_teleportData.outTelepotrObj = teleportObj;
                teleportObj.SetActive(true);
            }
        }

        public void CheckMonster()
        {
            for (int n = 0; n < mNpcList.Count; n++)
            {
                NPC actor = mNpcList[n];
                if (actor != null && !actor.IsRealDead)
                {
                    if (m_curMonstersNum == 0)
                    {
                        if (actor.roomElement.MonsterData.appearCondId == ENMonsterAppearType.enNone)
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (actor.roomElement.MonsterData.appearCondId == ENMonsterAppearType.enKillSomeMonsters)
                        {
                            if (actor.roomElement.MonsterData.appearCondParam == m_curMonstersNum)
                            {
                                return;
                            }
                        }
                    }
                }
            }
            m_curMonstersNum++;
        }
        public override bool Tick()
        {
            //刷新怪物
            for (int n = 0; n < m_actorRefreshList.Count; n++)
            {
                m_actorRefreshList[n].FixedUpdate();
            }
            //刷新宝箱
            for (int i = 0; i < m_treasureRefreshList.Count; i++)
            {
                m_treasureRefreshList[i].FixedUpdate();
            }
            //刷新机关
            for (int i = 0; i < m_trapRefreshList.Count; i++)
            {
                m_trapRefreshList[i].FixedUpdate();
            }
            if (!mStarBattleActive)
            {
                foreach (var item in mNpcList)
                {
                    item.mAnimControl.Tick();
                }
                return true;
            }
            if (!mIsAcitve)
            {
                return true;
            }
            //
            CheckAllMonsterDead();
            return true;
        }
        void RoomCloseGates()
        {
            if ( ActorManager.Singleton.MainActor.mCurRoomGUID == ID)
            {
                for (int i = 0; i < CurRoomInfo.m_gateList.Count; i++)
                {
                    if (!CurRoomInfo.m_gateList[i].isGateOpen)
                    {
                        continue;
                    }
                    if (!CurRoomInfo.m_gateList[i].OutIsActive)
                    {
                        continue;
                    }
                    //关闭闸门
                    Gate gate = CurRoomInfo.m_gateList[i];
                    ENGateOpenCondType condType = gate.gateOpenCondId;
                    if (condType == ENGateOpenCondType.enNone)
                    {
                        gate.gateOpenCondId = ENGateOpenCondType.enKillAllMonsters;
                    }
                    RoomOperateGate(gate, i, false);
                }
                IsOpenState = false;
            }
        }
        public void RoomOperateGate(Gate gate, int gateIndex, bool isOpen)
        {
            gate.GateOpenAnim(isOpen);
            CurRoomInfo.RebuildFinderPathData((ENGateDirection)(gateIndex), isOpen?false:true);
            if (gate.BridgeData != null && gate.OutIsReactive)
            {
                RandomRoomLevel.Singleton.ActiveSceneRoom(gate.BridgeData.nextRoomNode, this);
                gate.OutIsReactive = false;
            }
        }
        void RoomOpenGates()
        {
            IsOpenState = true;
            for (int i = 0; i < CurRoomInfo.m_gateList.Count; i++)
            {
                if (!CurRoomInfo.m_gateList[i].isGateOpen)
                {
                    continue;
                }
                if (CurRoomInfo.m_gateList[i].OutIsActive)
                {
                    continue;
                }
                Gate gate = CurRoomInfo.m_gateList[i];
                ENGateOpenCondType condType = gate.gateOpenCondId;

                if (CheckOpenGatesCondition(condType, gate.gateOpenCondParam, gate.gateOpenCondCount))
                {
                    if (gate.OpenGateTime < 1)
                    {
                        gate.OpenGateTime = Time.time + 0.5f;
                    }
                    if (Time.time > gate.OpenGateTime)
                    {
                        RoomOperateGate(gate, i, true);
                        if (CurRoomInfo.m_gateList[i].BridgeData != null && gate.OutIsReactive)
                        {
                            RandomRoomLevel.Singleton.ActiveSceneRoom(CurRoomInfo.m_gateList[i].BridgeData.nextRoomNode, this);
                            gate.OutIsReactive = false;
                        }
                    }
                    else
                    {
                        IsOpenState = false;
                    }
                }
                else
                {
                    gate.OpenGateTime = 0;
                }

            }
        }
        bool CheckOpenGatesCondition(ENGateOpenCondType condType, List<int> condParam, List<int> paramCount)
        {
            // direct open gate
            if (condType == ENGateOpenCondType.enNone)
            {
                return true;
            }
            else if (condType == ENGateOpenCondType.enKillAllMonsters)
            {
                for (int n = 0; n < mNpcList.Count; n++)
                {
                    Actor actor = mNpcList[n];
                    if (actor != null && !actor.IsRealDead)
                    {
                        return false;
                    }
                }
            }
            else if (condType == ENGateOpenCondType.enKillSpecialMonster)
            {
                for (int n = 0; n < mNpcList.Count; n++)
                {
                    NPC actor = mNpcList[n];
                    if (actor != null && !actor.IsRealDead)
                    {
                        for (int i = 0; i < condParam.Count; i++ )
                        {
                            if (actor.roomElement.MonsterData.monsterId == condParam[i])
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            else if (condType == ENGateOpenCondType.enSatisfyCondID)
            {
                for (int n = 0; n < condParam.Count; n++ )
                {
                    int count = 0;
                    if (mSatisfyIDMap.TryGetValue(condParam[n], out count))
                    {
                        if (count < paramCount[n])
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (condType == ENGateOpenCondType.enUseKey)
            {
                return false;
            }
            return true;
        }
        public bool CheckSceneRoomState()
        {
            bool tmpBool = true;
            foreach (var item in mNpcList)
            {
                if (!item.IsDead)
                {
                    tmpBool = false;
                    break;
                }
            }
            return tmpBool;
        }
        public bool SkillAllMonster()
        {
            
//             if (m_IsSkillAllMonster)
//             {
//                 return true ;
//             }
            bool tmpBool = true;
            foreach (var item in mNpcList)
            {
                if (!item.IsDead)
                {
                    tmpBool = false;
                    break;
                }
            }
            if (tmpBool)
            {
                m_IsSkillAllMonster = true;
            }
            return tmpBool;
        }
        public bool OpenAllTreasure()
        {
            if (m_IsOpenAllTreasure)
            {
                return true;
            }
            bool tmpBool = true;
            foreach (var item in mTreasureList)
            {
                if (!item.IsDead)
                {
                    tmpBool = false;
                    break;
                }
            }
            if (tmpBool)
            {
                m_IsOpenAllTreasure = true;
            }
            return tmpBool;
        }
        public bool ActiveAllTrap()
        {
            if (m_IsActiveAllTrap)
            {
                return true;
            }
            bool tmpBool = true;
            foreach (var item in mTrapList)
            {
                if (!item.IsActive)
                {
                    tmpBool = false;
                    break;
                }
            }
            if (tmpBool)
            {
                m_IsActiveAllTrap = true;
            }
            return tmpBool;
        }

        void OnCheckEnterCallBack(GameObject gameObject, Collider other)
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
            if (targetObj == null)
            {
                return;
            }

            ActorProp prop = targetObj.parent.GetComponent<ActorProp>();
            Actor targetActor = prop.ActorLogicObj;
            if (targetActor.Type == ActorType.enMain)
            {
                if (!mStarBattleActive)
                {
                    mStarBattleActive = true;
                    SM.RandomRoomLevel.Singleton.NotifyChanged((int)SM.RandomRoomLevel.ENPropertyChanged.enBeginBattle, null);
                    //RoomCloseGates();
                    foreach (var item in mNpcList)
                    {
                        ActorManager.Singleton.m_actorMap.Add(item.ID, item);
                        Actor delayActor;
                        ActorManager.Singleton.m_delayReleaseActorDict.TryGetValue(item.ID, out delayActor);
                        if (null != delayActor)
                        {
                            ActorManager.Singleton.m_delayReleaseActorDict.Remove(item.ID);
                        }
                    }
                }
            }
        }
    }
}