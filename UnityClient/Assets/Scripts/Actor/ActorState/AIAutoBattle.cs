using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AutoMoveRoom
{
    //托管战斗中不用再搜索的门方向标识
    public Dictionary<int, bool> m_gatePathEndList = new Dictionary<int, bool>();
    //当前房间是否打开所有机关
    public bool m_openAllTrap = false;
    public AutoMoveRoom()
    {
        //m_gatePathEndList = new bool[(int)SM.ENGateDirection.COUNT];
        m_gatePathEndList.Clear();
        m_openAllTrap = false;
    }
    public void Reset()
    {
        m_gatePathEndList.Clear();
        m_openAllTrap = false;
    }
    public bool NeedToSearch(int gateId)
    {
        if (!m_gatePathEndList.ContainsKey(gateId))
        {
            m_gatePathEndList[gateId] = false;
        }
        return m_gatePathEndList[gateId];
    }
    public void AcceptGatePathList(int gateId)
    {
        m_gatePathEndList[gateId] = true;
    }
    public void SetEnOpenTrap(bool val)
    {
        m_openAllTrap = val;
    }
}
public class AIAutoBattle : AIPlayer
{
    
    MainPlayer Self { get { return Owner as MainPlayer; } }
    #region AutoAttack
    //是否自动攻击
    //bool m_isAutoAttack = false;
    //自动攻击的间隔
    //float m_autoAttackInterval = 0;
    #endregion
    //最后一次位移动作的时间
    //float m_lastActionTime = 0;
    //是否可以反击
    //bool m_isCounterattack = false;
    //反击技能的攻击范围
    //float m_counterSkillRange = 0;
    //传送门对象
    //当前房间信息
    SM.SceneRoom m_curSceneRoom { get { return SM.RandomRoomLevel.Singleton.LookupRoom(SM.RandomRoomLevel.Singleton.LookupRoomGUID(Self.MainPos)); } }
    
    //寻路走过的房间List
    Dictionary<int, AutoMoveRoom> m_autoMoveRoomDic = new  Dictionary<int, AutoMoveRoom>();
    //托管寻路要移动到的坐标点
    //Vector3 m_moveToPos = Vector3.zero;
    //托管寻路状态标示符
    bool m_autoMove = false;
    //bool m_isDeadNotify = false;
    //标志走向传送门

    //下一次ai的间隔
    protected float m_nextAITime = 0.5f;
    public override void Update()
    {
        base.BaseUpdate();
        m_nextAITime -= Time.deltaTime;
        if (m_nextAITime > 0.0f)
        {
            return;
        }
        m_nextAITime = UnityEngine.Random.Range(0.1f, 0.5f);
        if (Self.IsActorExit) return;//当前角色离场
        
        if (Portals())//传送
        {

        }
        else if (PlayerCMD())//玩家操作
        {

        }
        else if (AutoBattle())//战斗
        {

        }
        else if (m_curSceneRoom == null)
        {
            bool tmpBool = false;
            if (Owner.ActionControl.IsActionRunning(ActorAction.ENType.enStandAction))
            {//检测是否在站立状态状态

                tmpBool = true;
            }
            else if (Self.ActionControl.IsActionRunning(ActorAction.ENType.enMoveAction))
            {//检测是否在移动状态
                MoveAction action = Self.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
                if (action.IsStopMove && m_autoMove)
                {//在移动状态停止移动
                    tmpBool = true;
                }
            }
            if (tmpBool)
            {
                FindPathLogicData roomFindPathData = SM.RandomRoomLevel.Singleton.FindRoomNode(Self.MainPos);
                if (roomFindPathData != null)
                {
                    Vector3 m_linkPos = roomFindPathData.m_links[1].m_linkPos;
                    ActionMoveTo(m_linkPos);
                }
            }
        }
    }
    //传送
    public bool Portals()
    {
        if (m_curSceneRoom != null && m_curSceneRoom.CurRoomInfo.m_teleportData != null)
        {//所在房间存在传送门，朝传送门移动
            ActionMoveTo(m_curSceneRoom.CurRoomInfo.m_teleportData.outTelepotrObj.transform.position);
            return true;
        }
        return false;
    }
    //玩家操作
    public bool PlayerCMD()
    {
        if (Self.CurrentCmd == null)
        {
            return false;
        }
        if (Self.CurrentCmd.m_type == Player.ENCmdType.enSkill)
        {
            int skillID = Self.CurrentCmd.m_skillID;
            Actor.ActorSkillInfo info = Self.SkillBag.Find(item => item.SkillTableInfo.ID == skillID);
            if (IsFireSkill(skillID))
            {
                GetRangeTargetList((ENTargetType)info.SkillTableInfo.TargetType, Self.CurrentTableInfo.AttackRange);
                if (Self.CurrentTarget != null)
                {
                    m_curTargetID = Self.CurrentTarget.ID;
                }
                if (ActionTryFireSkill(skillID))
                {
                    DelSkillHighlight();
                    Self.CurrentCmd = null;
                }
                else
                {
                    if (m_targetIDList.Count == 0)
                    {
                        DelSkillHighlight();
                        Self.CurrentCmd = null;
                    }
                }
            }
            else
            {
                GetRangeTargetList((ENTargetType)info.SkillTableInfo.TargetType, Self.CurrentTableInfo.AttackRange);
                if (m_targetIDList.Count != 0 && skillID != m_highlightSkillID)
                {//技能高亮的通知
                    AddSkillHighlight();
                    m_highlightSkillID = skillID;
                }
            }
            return true;
        }
        else if (Self.CurrentCmd.m_type == Player.ENCmdType.enSwitchActor)
        {
            if (ActorManager.Singleton.SwitchMainActor(false, false))
            {
                Self.CurrentCmd = null;
            }
            return true;
        }
        return false;
    }

    
    public bool AutoBattle()
    {
        if (BattleMonster())//战斗
        {
            return true;
        }
        else if (BattleTreasure())//宝箱
        {
            return true;
        }
        else if(BattleTrap())//机关
        {
            return true;
        }
        else
        {
            
            return AutoPathing();
            
        }

        //return false;
    }
    public bool BattleMonster()
    {
        if (m_curSceneRoom != null)
        {//在房间中
            //获取房间内的所有敌方
            SetRoomTargetListToEnemy(ENTargetType.enEnemy, m_curSceneRoom.mNpcList);
            if (m_targetIDList.Count > 0)
            {//敌方数量大于0
                if (m_curSceneRoom.mStarBattleActive)
                {
                    if (ActionTryFireSkill(0))
                    {//自动释放技能，并成功
                        Self.CurrentTarget = ActorManager.Singleton.Lookup(m_curTargetID);
                        return true;
                    }
                }
                else
                {
                    ActionMoveTo(m_minActor.MainPos);
                    return true;
                }
            }
        }
        return false;
    }

    public bool BattleTreasure()
    {
        if (m_curSceneRoom != null)
        {//在房间中
            SetRoomTargetListToBoxNpc(m_curSceneRoom.mTreasureList);
            if (m_targetIDList.Count > 0)
            {//宝箱数量大于0

                ActionTryFireSkill(0);//释放开宝箱技能
                return true;
            }
        }
        return false;
    }

    public bool BattleTrap()
    {
        if (m_curSceneRoom != null)
        {//在房间中
            SetRoomTrageListToTrap(m_curSceneRoom.mTrapList);
            if (m_targetIDList.Count > 0)
            {//机关数量大于0
                if (OpenTrapWithoutKey())
                {//打开不需要钥匙的机关
                    return true;
                }
                else if (OpenTrapWithKey())
                {//打开需要钥匙 且自身有钥匙的机关
                    return true;
                }
                //ActionTryFireSkill(0);//释放开机关技能
            }
        }
        return false;
    }
    bool OpenTrapWithoutKey()
    {
        bool openAllTrap = true;
        foreach (var item in m_targetIDList)
        {
            Actor tmpActor = ActorManager.Singleton.Lookup(item);
            Trap tmpTrap = tmpActor as Trap;
            if (!tmpTrap.m_IsNeedKey)
            {
                Owner.CurrentTarget = ActorManager.Singleton.Lookup(item);
                m_curTargetID = item;
                if (ActionTryFireSkill(0))//释放开宝箱技能
                {
                    m_curTargetID = 0;
                    Owner.CurrentTarget = null;
                }
            }
        }
        return openAllTrap;
    }

    bool OpenTrapWithKey()
    {
        bool openAllTrap = true;
        foreach (var item in m_targetIDList)
        {
            Actor tmpActor = ActorManager.Singleton.Lookup(item);
            Trap tmpTrap = tmpActor as Trap;
            if (tmpTrap.m_IsNeedKey)
            {
                if (BattleArena.Singleton.GetKeyCountByID(tmpTrap.m_needKeyId) > 0)
                {
                    Owner.CurrentTarget = ActorManager.Singleton.Lookup(item);
                    m_curTargetID = item;
                    if (ActionTryFireSkill(0))//释放开宝箱技能
                    {
                        m_curTargetID = 0;
                        Owner.CurrentTarget = null;
                    }
                }
                else
                {
                    openAllTrap = false;
                }
            }
        }
        //当前房间机关是否全部开启
        AutoMoveRoom tmpAutoMoveRoomData = GetAutoMoveRoomData(m_curSceneRoom.ID);
        tmpAutoMoveRoomData.SetEnOpenTrap(openAllTrap);
        return true;
    }

    public bool AutoPathing()
    {
        if (m_curSceneRoom == null)
        {//监测当前所站立位置死否有房间
            return false;
        }
        if (!m_curSceneRoom.IsOpenState)
        {//监测当前房间是否开门
            return false;
        }
        if (Self.ActionControl.IsActionRunning(ActorAction.ENType.enStandAction) && m_autoMove)
        {//监测当前是否在站立状态
            m_autoMove = false;
        }
        else if (Self.ActionControl.IsActionRunning(ActorAction.ENType.enMoveAction))
        {//检测是否在移动状态
            MoveAction action = Self.ActionControl.LookupAction(ActorAction.ENType.enMoveAction) as MoveAction;
            if (action.IsStopMove && m_autoMove)
            {//在移动状态停止移动
                m_autoMove = false;
            }
        }
        if (m_autoMove)
        {
            return false;
        }
        SM.SceneRoom curSceneRoom = m_curSceneRoom;
        AutoMoveRoom curRoomMoveData = GetAutoMoveRoomData(curSceneRoom.ID);
        int moveToRoomID = -1;
        bool noRoomCanTo = true;
        for (int i = 0; i < curSceneRoom.CurRoomInfo.m_gateList.Count; i++)
        {//遍历当前房间闸门
            SM.Gate curGate = curSceneRoom.CurRoomInfo.m_gateList[i];
            if (curGate.GateToRoomObjID == -1)
            {
                continue;
            }
            if (!curGate.OutIsActive)
            {//检测当前房间房间门是否开放
                noRoomCanTo = false;
                continue;
            }
            if (curRoomMoveData.NeedToSearch(curGate.gateId))
            {//检测当前闸门是否还用再去探索
                continue;
            }
            moveToRoomID = curGate.GateToRoomObjID;
            break;
        }

        if (moveToRoomID == -1)
        {//闸门没有可去的那么返回上一个房间
            SM.SceneRoom preRoom = m_curSceneRoom.m_preRoom;
            if (preRoom == null)
            {
                return false;
            }
            moveToRoomID = preRoom.ID;
            if (noRoomCanTo)
            {
                for (int i = 0; i < preRoom.CurRoomInfo.m_gateList.Count; i++)
                {
                    SM.Gate gate = preRoom.CurRoomInfo.m_gateList[i];
                    if (gate.GateToRoomObjID == curSceneRoom.ID)
                    {//将上一个房间通向这个房间的闸门标记为不再探索
                        GetAutoMoveRoomData(moveToRoomID).AcceptGatePathList(gate.gateId);
                    }
                }
            }
        }
        FindPathLogicData logicData = ScenePathfinder.Singleton.FindRoomNode(m_curSceneRoom.CurRoomInfo.m_GUID);
        foreach (FindPathLogicData.LinkInfo linkData in logicData.m_links)
        {
            foreach (FindPathLogicData.LinkInfo linkData1 in linkData.m_linkRoom.m_links)
            {
                if (linkData1.m_linkRoom.GUID == moveToRoomID)
                {
                    ActionMoveTo(linkData1.m_linkPos);
                    m_autoMove = true;
                    return true;
                }
            }
        }
        return false;
    }
    public AutoMoveRoom GetAutoMoveRoomData(int roomId)
    {
         if (!m_autoMoveRoomDic.ContainsKey(roomId))
         {//检测要去的房间是否走过并设置要去房间进入房间的房门和来时房间的ID
             AutoMoveRoom tmpMoveRoom = new AutoMoveRoom();
             m_autoMoveRoomDic.Add(roomId, tmpMoveRoom);
         }
         return m_autoMoveRoomDic[roomId];
    }
    public bool CheckCurRoomPathEnd()
    {
        bool tmpBool = false;
        for (int i = 0; i < m_curSceneRoom.CurRoomInfo.m_gateList.Count; i++)
        {
            SM.Gate tmpGate = m_curSceneRoom.CurRoomInfo.m_gateList[i];
            if (!tmpGate.isGateOpen)
            {//检测当前房间房间门是否开放
                continue;
            }
            if (GetAutoMoveRoomData(m_curSceneRoom.ID).m_gatePathEndList[i])
            {//检测当前闸门是否还用再去探索
                continue;
            }
            if (!tmpGate.OutIsActive)
            {
                AutoMoveRoom curRoomMoveData = GetAutoMoveRoomData(m_curSceneRoom.ID);
                if (curRoomMoveData.m_openAllTrap)
                {
                    tmpBool = true;
                }
            }
        }
        return tmpBool;
    }

    //战斗托管时组建targetList
    void CheckAutoBattleTarget(ENTargetType type, Actor target)
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
        switch (type)
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
        float distance = ActorTargetManager.GetTargetDistance(Owner.RealPos, target.RealPos);
        if (distance < m_minDistance)
        {
            m_minDistance = distance;
            m_minActor = target;
        }
        m_targetIDList.Add(target.ID);
    }
    public void SetRoomTargetListToEnemy(ENTargetType type, List<NPC> npcList)
    {
        m_targetIDList.Clear();
        setTargetIDlist(type, npcList);
    }
    public void setTargetIDlist(ENTargetType type, List<NPC> npcList)
    {
        m_minDistance = float.MaxValue;
        foreach (NPC item in npcList)
        {
            CheckAutoBattleTarget(type, item);
        }
    }
    public void SetRoomTargetListToBoxNpc(List<NPC> treasureList)
    {
        m_minDistance = float.MaxValue;
        m_targetIDList.Clear();
        foreach (NPC item in treasureList)
        {
            if (item.GetNpcType() == ENNpcType.enBoxNPC && !item.IsDead)
            {
                float distance = ActorTargetManager.GetTargetDistance(Owner.RealPos, item.RealPos);
                if (distance < m_minDistance)
                {
                    m_minDistance = distance;
                    m_minActor = item;
                }
                m_targetIDList.Add(item.ID);
            }
        }
    }
    public void SetRoomTrageListToTrap(List<Trap> trapList)
    {
        m_minDistance = float.MaxValue;
        m_targetIDList.Clear();
        Dictionary<float, List<int>> tmpTrapDic = new Dictionary<float, List<int>>();
        List<float> tmpDistList = new List<float>();
        foreach (Trap item in trapList)
        {
            if (item.mTrapType == TrapType.enAuto || item.mTrapType == TrapType.enTouchType)
            {
                continue;
            }
            if (!item.m_trapActivate)
            {
                float distance = ActorTargetManager.GetTargetDistance(Owner.RealPos, item.RealPos);
                if (tmpTrapDic.ContainsKey(distance))
                {
                    tmpTrapDic[distance].Add(item.ID);
                }
                else
                {
                    List<int> tmpList =  new List<int> ();
                    tmpList.Add(item.ID);
                    tmpTrapDic.Add(distance, tmpList);
                    tmpDistList.Add(distance);
                }
            }
        }
        tmpDistList.Sort();
        foreach(var item in tmpDistList)
        {
            foreach (var id in tmpTrapDic[item])
            {
                m_targetIDList.Add(id);
            }
        }
    }

    public bool CheckAutoBattle()
    {
        //战斗逻辑


        //开宝箱

        //开机关
        if (AutoBattle())
        {
            return true;
        }

        //寻路

        AutoPathing();


        if (CheckRoomState())
        {
            //寻路逻辑
            if (m_curSceneRoom == null)
            {
                return false;
            }
            AutoPathing();
        }
        else
        {
            //战斗逻辑
            m_autoMove = false;
            AutoBattle();
        }

        return true;
    }

    public bool CheckRoomState()
    {
        if (m_curSceneRoom == null)
        {
            return true;
        }
        return m_curSceneRoom.CheckSceneRoomState();
    }
}