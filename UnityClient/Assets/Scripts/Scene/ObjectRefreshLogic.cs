using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SM
{
    public class IRefresh
    {
        public enum ENRefreshType
        {
            enActorRefresh      = 0,
            enTreasureRefresh   ,
            enTrapRefresh       ,
        }
        public static int m_dynamicIDSeed = 994;
        public SceneRoom m_room = null;
        public RoomElement m_ele = null;
        public MonsterRoomData m_monsterData = null;
        public ENRefreshType m_refreshType = ENRefreshType.enActorRefresh;
        public IRefresh()
        {

        }
        virtual public void FixedUpdate()
        {

        }
    }
    public class ActorRefresh : IRefresh
    {
        public static Dictionary<int, bool> RefreshCondDict = new Dictionary<int, bool>();
        int             m_curSpawnCount = 0;
        public Actor m_curMonsterObj = null;
        public ActorRefresh(Transform trans, SceneRoom room)
        {
            m_refreshType       = ENRefreshType.enActorRefresh;
            m_room              = room;
            m_ele               = trans.gameObject.GetComponent<RoomElement>();
            m_monsterData       = m_ele.MonsterData;
        }
        override public void FixedUpdate()
        {
            if (m_curSpawnCount == 0)
            {
                if (m_monsterData.appearCondId == ENMonsterAppearType.enNone)
                {
                    SpawnMe();
                    m_curSpawnCount++;
                }
                return;
            }
        }
        public void SpawnMe()
        {
            Actor npcActor = ActorManager.Singleton.CreatePureActor(ActorType.enNPC, m_dynamicIDSeed++, CSItemGuid.Zero, m_ele.ObjSettingID, m_room.mStarBattleActive);
            m_curMonsterObj = npcActor;
            int npcStaticID = m_ele.ObjSettingID;
            npcActor.IDInTable = npcStaticID;
            npcActor.Props.SetProperty_Int32(ENProperty.islive, 1);

            NPC npc = npcActor as NPC;
            npc.roomElement = m_ele;
            npc.CurRoom = m_room;
            npc.SetBodyObject(m_ele.gameObject);
            npc.roomElement.MonsterData.outDyncId = npcActor.ID;
            npc.roomElement.MonsterData.isRefresh = true;
            npc.roomElement.CampID = m_ele.CampID;
            npc.TempCamp = m_ele.CampID;
            npc.PatrolList = m_ele._PatrolList;
            npc.OnInitProps();
//            NPCInfo info = GameTable.NPCInfoTableAsset.Lookup(npcStaticID);
            Vector3 pos = Vector3.zero;
            pos = new Vector3(m_ele.gameObject.transform.position.x, 0.15f, m_ele.gameObject.transform.position.z);
            Quaternion quater = m_ele.gameObject.transform.rotation;//new Quaternion(actorData.gameObject.transform.rotation.x, actorData.gameObject.transform.rotation.y, actorData.gameObject.transform.rotation.)
            npcActor.CreateNeedModels();
            npcActor.ForceMoveToPosition(pos, quater);
            //npcActor.MainObj.transform.rotation = quater;
            npcActor.m_startAttackPos = pos;

            m_room.mNpcList.Add(npc);
            m_room.mMonsterDataDict.Add(npc.roomElement.MonsterData.monsterId, npc.roomElement.MonsterData);
            m_room.mIsAcitve = true;
        }
        public bool IsMonsterDataId(int id)
        {

            if (m_monsterData.monsterId == id)
            {
                return true;
            }
            return false;
        }
    }

    public class TreasureRefresh : IRefresh
    {
        public static Dictionary<int, bool> RefreshCondDict = new Dictionary<int, bool>();
        //static int      m_dynamicIDSeed = 5994;
        //int             m_curSpawnCount = 0;
        bool m_isSpawnEnd = false;
//         SceneRoom       m_room = null;
//         RoomElement     m_ele = null;
//         MonsterRoomData m_monsterData = null;
        public TreasureRefresh(Transform trans, SceneRoom room)
        {
            m_refreshType       = ENRefreshType.enTreasureRefresh;
            m_room              = room;
            m_ele               = trans.gameObject.GetComponent<RoomElement>();
            m_monsterData       = m_ele.MonsterData;
        }
        override public void FixedUpdate()
        {
            if (!m_isSpawnEnd)
            {
				m_isSpawnEnd = true;
                SpawnMe();
            }
        }
        void SpawnMe()
        {
            Actor npcActor = ActorManager.Singleton.CreatePureActor(ActorType.enNPC, m_dynamicIDSeed++, CSItemGuid.Zero, m_ele.ObjSettingID);
            int npcStaticID = m_ele.ObjSettingID;
            npcActor.IDInTable = npcStaticID;
            npcActor.Props.SetProperty_Int32(ENProperty.islive, 1);

            NPC npc = npcActor as NPC;
            npc.roomElement = m_ele;
            npc.CurRoom = m_room;
            npc.roomElement.MonsterData.outDyncId = npcActor.ID;
            npc.roomElement.MonsterData.isRefresh = true;

//            NPCInfo info = GameTable.NPCInfoTableAsset.Lookup(npcStaticID);
            Vector3 pos = Vector3.zero;
            pos = new Vector3(m_ele.gameObject.transform.position.x, 0.15f, m_ele.gameObject.transform.position.z);
            Quaternion quater = m_ele.gameObject.transform.rotation;//new Quaternion(actorData.gameObject.transform.rotation.x, actorData.gameObject.transform.rotation.y, actorData.gameObject.transform.rotation.)
            npcActor.CreateNeedModels();
            npcActor.ForceMoveToPosition(pos, quater);
            //npcActor.MainObj.transform.rotation = quater;
            npcActor.m_startAttackPos = pos;
            //npc.SetBodyObject(m_ele.gameObject);
            
            m_room.mTreasureList.Add(npc);
            m_room.mTreasureDataDict.Add(npc.roomElement.MonsterData.monsterId, npc.roomElement.MonsterData);
            m_room.mIsAcitve = true;
            
        }
    }
    public class TrapRefresh : IRefresh
    {
        public static Dictionary<int, bool> RefreshCondDict = new Dictionary<int, bool>();
        //static int m_dynamicIDSeed = 6994;
        //int m_curSpawnCount = 0;
        bool m_isSpawnEnd = false;
        //SceneRoom m_room = null;
        TrapData m_trapData;
        Transform m_ObjectPred = null;
        //RoomElement m_ele = null;
        //MonsterRoomData m_monsterData = null;
        public TrapRefresh(Transform trans, SceneRoom room, TrapData trapData)
        {
            m_refreshType = ENRefreshType.enTrapRefresh;
            m_room = room;
            m_ObjectPred = trans;
            m_trapData = trapData;
        }
        override public void FixedUpdate()
        {
            if (!m_isSpawnEnd)
            {
                SpawnMe();
                m_isSpawnEnd = true;
            }
        }
        public static void SpawnMe(SceneRoom room, TrapData trapData)
        {
            string objName = "trap_" + trapData.trapId + "_" + trapData.trapObjId;
            GameObject childObj = new GameObject(objName);
            childObj.transform.parent = room.m_parentObj.transform;
            childObj.transform.localPosition = new Vector3(trapData.position.x * SM.SceneRoom.blockWidth, 0f, trapData.position.z * SM.SceneRoom.blockHeight);
            TrapRefresh trap = new TrapRefresh(childObj.transform, room, trapData);
            room.m_trapRefreshList.Add(trap);
        }
        void SpawnMe()
        {
            Actor trapActor = ActorManager.Singleton.CreatePureActor(ActorType.enNPCTrap, m_dynamicIDSeed++, CSItemGuid.Zero, m_trapData.trapObjId);
            trapActor.Props.SetProperty_Int32(ENProperty.islive, 1);
            Vector3 pos = Vector3.zero;
            Trap tmpTrapActor = trapActor as Trap;
            tmpTrapActor.CurRoom = m_room;
            tmpTrapActor.SetTrapState(m_trapData.mTrapState);
            tmpTrapActor.SetTrapAble(m_trapData.beginState);
            tmpTrapActor.SetTrapActive(m_trapData.beginEable);
            tmpTrapActor.InitProps();
            m_trapData.outDyncId = trapActor.ID;

            m_room.mTrapList.Add(tmpTrapActor);
            m_room.mTrapDataDict.Add(m_trapData.trapId, m_trapData);
            pos = new Vector3(m_ObjectPred.gameObject.transform.position.x, 0.0f, m_ObjectPred.gameObject.transform.position.z);
            trapActor.CreateNeedModels();
            trapActor.ForceMoveToPosition(pos);
            

        }
    }
}


