using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace SM
{
    //
    //房间之间连接的起始点与结束点
//     public class RoomConnectData
//     {
//         public Vector3 StartPos = new Vector3();
//         public Vector3 EndPos = new Vector3();
//     }
    //与底层交互的房间数据
    public class SERoomDataTree
    {
        // 房间资源
        public GameObject           mRoomObj = null;
        //房间数据
        public GameObject           mRoomData = null;
        //
        public SceneRoomInfoTree    mRoomInfoTree = null;
        // 底层生成的房间gameobject用于上层逻辑使用
        public GameObject           mOutRoomObj = null;
        // 北东南西四个门数据
        public SEGateData[]         mGateData = new SEGateData[4];

    }
    // 底层需交互的门的数据
    public class SEGateData
    {
        // 是否有通路,false 则桥头、桥身、下个房间的数据全部为空
        public bool             mIsHaveTunnel = false;
        // 桥头资源
        public GameObject       mBridgeHeadObj = null;
        // 桥尾资源
        public GameObject       mBridgeTailObj = null;
        // 桥身资源
        public GameObject       mBridgeBodyObj = null;
        // 连接桥中心需要重覆多少次
        public int              mBridgeCenterLength = 0;
        // 连通下一个房间的数据
        public SERoomDataTree   mNextRoomNode = null;
    }
    public enum ENGateOpenCondType
    {
        enNone = 0,
        //消灭房内所有怪物
        enKillAllMonsters       = 1,
        //消灭特定怪物
        enKillSpecialMonster    = 2,
        //当机关或宝箱等条件ID(satisfyCondId)达成时开启, 所属条件ID参考appearCondParam
        enSatisfyCondID         = 3,
        //使用钥匙开通通道, 闸门所属的keyId参考appearCondParam
        enUseKey                = 4,

        enOpenBox,      // 打开 宝箱
        enOpenSheme,    // 打开特定机关
        enEvent,        // 特定触发事件
    }
    public enum ENGateDirection
    {
        enNone = -1,
        enNORTH = 0,
        enEAST,
        enSOUTH,
        enWEST,
        COUNT
    }
    public class Gate
    {
        static public string        OpenGateAnim = "unlock-00";
        static public string        LockGateAnim = "lock-00";
        public int gateId = -1;
        //房门的方向
        public int gateDirectionIndex = -1;
        //
        public int                  gateObjId = -1;
        // 这闸门会否有桥连接，即是否能开门
        public bool                 isGateOpen = false;
        // 通往此闸门的开放条件
        public ENGateOpenCondType   gateOpenCondId = ENGateOpenCondType.enNone;
        // 根据开放条件来记录不同的ID
        public List<int>            gateOpenCondParam = new List<int>();
        // 此参数仅应用于以条件ID为开门条件时使用，为各条年ID所需满足次数
        public List<int>            gateOpenCondCount = new List<int>();
        //
        // 桥数据
        public Bridge               BridgeData = null;
        //该门对应的房间对象
        public int                  GateToRoomObjID = -1;
        //门在运行期的开门延时时间
        public float                OpenGateTime = 0f;
        //输出参数用于Gate是否激活
        public bool                 OutIsActive = false;
        //是否需要重新激活
        public bool                 OutIsReactive = true;
        // 生成的gate oject
        public GameObject           OutGateObject = null;
		//设置当前门的开关及播放的动画
        public void GateOpenAnim(bool isOpen)
        {
            string strAnim = isOpen ? Gate.OpenGateAnim : Gate.LockGateAnim;
            OutGateObject.GetComponent<Animation>().Play(strAnim);
            OutIsActive = isOpen;
        }
        public void Reset()
        {
            OutIsActive = false;
            OutIsReactive = true;
        }
    }
    public class Bridge
    {
        public int                  bridgeObjId = -1;
        public int                  bridgeCenterLength = 0;
        public Vector3              startPos = new Vector3();
        public Vector3              endPos = new Vector3();
        public SceneRoomInfoTree    nextRoomNode = null;
    }
    public enum ENMonsterAppearType
    {
        //0. 自动出现
        enNone              = 1,
        //在上一波怪物被清除后出现, 怪物所属波数参考appearCondParam，第1波的怪物会在自动出现怪物被清除后出现
        enKillSomeMonsters  = 2,
        //当机关或宝箱或怪物等satisfyCondId达成时出现, 如跟怪物的appearCondParam相等，而且达成次数大于或等于appearCondCount时，怪物出现
        enSatisfyCondID     = 3,

    }
    [System.Serializable]
    public class MonsterRoomData
    {
        // 当前room 唯一ID
        public int                  monsterId = 0;
        // 对应MonsterData ID
        public int                  monsterObjId = 0;
        // 坐标
        public Vector3              position = Vector3.zero;
        // 怪物出现条件
        public ENMonsterAppearType  appearCondId = ENMonsterAppearType.enNone;
        // 怪物出现条件参数
        public int                  appearCondParam = 0;
        // 怪物出现条件参数量
        public int                  appearCondCount = 0;
        // 掉落物品列表
        public List<int>            dropList = new List<int>();
        // 如何才会令满足而令satisfyCondId满足
        public int                  satisfyCondTypeId = 0;
        // 上述方式的辅助参数
        public int                  satisfyCondTypeParam = 0;
        // 条件ID, 达成上述即满足其条件, 用作弹出相应怪物或开闸门
        public int                  satisfyCondId = 0;
        //
        public int                  outDyncId = -1;
        //是否刷新
        public bool                 isRefresh = false;
    }

	//房间内每一个机关的数据结构
    public class TrapData
    {
        // 当前room 唯一ID
        public int trapId = 0;
        // 对应TrapData ID
        public int trapObjId = 0;
        // 坐标
        public Vector3 position = Vector3.zero;
        // 机关出现条件
        public ENMonsterAppearType appearCondId = ENMonsterAppearType.enNone;
        // 机关出现条件参数
        public int appearCondParam = 0;
        // 机关出现条件参数量
        public int appearCondCount = 0;
        // 会掉落钥匙的id, 没这参数即不会掉落
        public int dropKeyId = -1;
        // 掉落物品种类
        public int dropTypeId = -1;
        // 如何才会令满足而令satisfyCondId满足
        public int satisfyCondTypeId = 0;
        // 上述方式的辅助参数
        public int satisfyCondTypeParam = 0;
        // 条件ID, 达成上述即满足其条件, 用作弹出相应怪物或开闸门
        public int satisfyCondId = 0;
        //
        public int outDyncId = -1;
        //起始状态（可否被触发）
        public bool beginState = false;
        //起始能力（是否激活）
        public bool beginEable = false;
        //public 
        public Trap.TrapState mTrapState = Trap.TrapState.enClose;
    }
    public class TeleportData
    {
		//传送门的ID
        public int          objId = 0;
		//传送门Json数据解析出来的坐标
        public Vector3      position = Vector3.zero;
        public string       target = "";
		//创建的传送门Object
        public GameObject 	outTelepotrObj = null;
    }
    public class SceneObstacleData
    {
        public struct ObData
        {
            public ENGateDirection enDirection;
            public Vector3 obstacleArray;
            public void Reset()
            {
                enDirection = ENGateDirection.enNone;
                obstacleArray = Vector3.zero;
            }
        }
        public int MaxSize = 0;
        public List<ObData> mData = new List<ObData>();
        public void Load(byte[] bytes)
        {
            BinaryHelper helper = new BinaryHelper(bytes);
            MaxSize = helper.ReadInt();
            int length = helper.ReadInt();
            for (int index = 0; index < length; index++)
            {
                ObData oData = new ObData();
                oData.enDirection = (ENGateDirection)helper.ReadInt();
                oData.obstacleArray.x = helper.ReadFloat();
                oData.obstacleArray.y = helper.ReadFloat();
                oData.obstacleArray.z = helper.ReadFloat();
                mData.Add(oData);
            }

        }
        public byte[] Save()
        {
            BinaryHelper helper = new BinaryHelper();
            helper.Write(MaxSize);
            helper.Write(mData.Count);
            for (int i = 0; i < mData.Count; i++)
            {
                helper.Write((int)(mData[i].enDirection));
                helper.Write(mData[i].obstacleArray.x);
                helper.Write(mData[i].obstacleArray.y);
                helper.Write(mData[i].obstacleArray.z);
            }
            return helper.GetBytes();
        }
    }
    public class SceneRoomInfoTree
    {
        #region SceneRoomInfoKeyString
        /// <summary>
        /// levels key string
        
        public static string sroomId = "roomId";
        public static string sroomObjId = "roomObjId";
        public static string sroonName = "roomName";
        public static string scharPosition = "charPosition";
        public static string steleportGate = "teleportGate";
        public static string starget = "target";
        public static string steleportObjId = "teleportGateObjId";
        public static string sgates = "gates";
        public static string sgateId = "serNo";
        public static string sgateObjId = "gateObjId";
        public static string sisGateOpen = "isGateOpen";
        public static string sgateOpenCondId = "gateOpenCondId";
        public static string sgateOpenCondParam = "gateOpenCondParam";
        public static string sgateOpenCondCount = "gateOpenCondCount";
        public static string sbridge = "bridge";
        public static string sbridgeObjId = "bridgeObjId";
        public static string sbridgeCenterLength = "bridgeCenterLength";
        public static string stargetRoom = "targetRoom";
        public static string smonsters = "monsters";
        public static string smonsterId = "monsterId";
        public static string smonsterObjId = "monsterObjId";
        public static string sposition = "position";
        public static string sposx = "x";
        public static string sposy = "y";
        public static string sappearCondId = "appearCondId";
        public static string sdropList = "dropList";
        public static string sbossRoom = "bossRoom";
        
        //房间内宝箱数据
        public static string sTreasure = "treasures";
        public static string sTreasureId = "treasureId";
        public static string sTreasureObjId = "treasureObjId";
        //房间内机关关键字
        public static string sTrap = "triggers";
        public static string sTrapId = "triggerId";
        public static string sTrapObjId = "triggerObjId";
        public static string sBeginState = "beginState";
        public static string sBeginEable = "beginEable";
        public static string sTrapState = "triggerState";
        /// </summary>
        #endregion

        //场景中所有怪物列表
        public static List<string> ModelNameList = new List<string>();
        /// increase index 
//        static int m_nIndex = -1;
        //parent node
        public SceneRoomInfoTree m_parent = null;
        //对应预设资源
        public int m_roomObjID = 0;
        //room guid
        public int m_GUID = -1;
        //房间预设名字
        public string m_roomName;
        //
        public string m_RoomPrefabName;
        //
        public SceneObstacleData  m_obstacleData = null;
        // 挂节点
        public GameObject m_locatorObj = null;
        //房间右上角挂节点
        public GameObject m_RTLocatorObj = null;
        // 房间gameobject
        public GameObject m_roomObj = null;
        //
        public GameObject m_roomData = null;
        //房门列表
        public List<Gate> m_gateList = new List<Gate>();
        //该房间怪物信息列表
        public List<MonsterRoomData> m_monsterDataList = new List<MonsterRoomData>();
        //该房间内宝箱信息列表
        public List<MonsterRoomData> m_treasureDataList = new List<MonsterRoomData>();
        //该房间内机关信息
        public List<TrapData> m_trapDataList = new List<TrapData>();
        //该房间可以通向的下一个房间列表
        public List<SceneRoomInfoTree> m_nextActiveRoom = new List<SceneRoomInfoTree>();
        //角色进入该房间时的坐标
        public Vector3 m_charPosition = Vector3.zero;//new Vector3(2f,0f,3f);
        //角色进入房间时的世界坐标
        public Vector3 CharPosition { get; set; }
        public Transform CharPosTransform = null;
        //传送门数据
        public TeleportData m_teleportData = null;
        //该层是否需要钥匙
        public bool mIsNeedKey = false;
        //是否是BOSS房间 
        public bool mIsBossRoom = false;
        //连接下一房间的坐标点
        //public Dictionary<int, RoomConnectData> ConnectNextRoomPosList = new Dictionary<int, RoomConnectData>();
        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        public SceneRoomInfoTree()
        {
//             m_nIndex++;
//             m_GUID = m_nIndex;
        }
        public void PrintRoomInfo()
        {
            Debug.Log("----------------------------begin-----------------------------------");
            Debug.Log("roomObjID =" + m_roomObjID);
            for (int i = 0; i < m_gateList.Count; i++)
            {
                int bridgeid = 0;
                int bridgelength = 0;
                if (null != m_gateList[i].BridgeData)
                {
                    bridgeid = m_gateList[i].BridgeData.bridgeObjId;
                    bridgelength = m_gateList[i].BridgeData.bridgeCenterLength;
                }
                Debug.Log("gateid=" + m_gateList[i].gateObjId + " isGateOpen=" + m_gateList[i].isGateOpen + " gateOpenCondId=" + m_gateList[i].gateOpenCondId + " bridgeid=" + bridgeid + " bridgelength=" + bridgelength);
            }
            for (int i = 0; i < m_monsterDataList.Count; i++)
            {
                Debug.Log("monsterid=" + m_monsterDataList[i].monsterId + " monsterObjId=" + m_monsterDataList[i].monsterObjId + " posX=" + m_monsterDataList[i].position.x + " posY=" + m_monsterDataList[i].position.y);
            }
            Debug.Log("--------------------------------------------------------------------");
            foreach (SceneRoomInfoTree roomNode in m_nextActiveRoom)
            {
                roomNode.PrintRoomInfo();
            }
        }
        // 根据服务器发送过来的字符串构建房间tree data
        public void BuildTree(JsonData data, SceneRoomInfoTree parent, string roomkey)
        {
            m_nextActiveRoom.Clear();
            m_parent = parent;
            if (!data.Keys.Contains(roomkey))
            {
                //return;
            }
            else
            {
                data = data[roomkey];
                // char position
                if (data.Keys.Contains(scharPosition))
                {
                    m_charPosition.x = int.Parse(data[scharPosition][sposx].ToString());
                    m_charPosition.z = int.Parse(data[scharPosition][sposy].ToString());
                }
                m_GUID = int.Parse(data[sroomId].ToString());
                m_roomObjID = int.Parse(data[sroomObjId].ToString());
                if (data.Keys.Contains(sroonName))
                {
                    m_roomName = data[sroonName].ToString();
                }

                SceneRoomInfo roomInfo = GameTable.SceneRoomTableAsset.Lookup(m_roomObjID);
                if (null != roomInfo)
                {
                    m_RoomPrefabName = roomInfo.prefabName;
                }
                //TextAsset asset = GameData.Load<TextAsset>(obstacleFileName);
                //m_obstacleData = new SceneObstacleData();
                //if (asset == null)
                //{
                //    Debug.Log("obstacleFileName=" + obstacleFileName);
                //}
                //m_obstacleData.Load(asset.bytes);
                // teleport data
                if (data.Keys.Contains(steleportGate))
                {
                    m_teleportData = new TeleportData();
                    m_teleportData.objId = int.Parse(data[steleportGate][steleportObjId].ToString());
                    m_teleportData.position.x = int.Parse(data[steleportGate][sposition][sposx].ToString());
                    m_teleportData.position.z = int.Parse(data[steleportGate][sposition][sposy].ToString());
                    m_teleportData.target = data[steleportGate][starget].ToString();
                }
                //gates info
                JsonData gatesItem = data[sgates];
                if (gatesItem.IsArray)
                {
                    for (int i = 0; i < gatesItem.Count; i++)
                    {
                        Gate gate = new Gate();
                        gate.gateId = int.Parse(gatesItem[i][sgateId].ToString());
                        gate.gateObjId = int.Parse(gatesItem[i][sgateObjId].ToString());
                        gate.isGateOpen = (int.Parse(gatesItem[i][sisGateOpen].ToString()) == 1);
                        gate.gateDirectionIndex = i;
                        if (gate.isGateOpen)
                        {
                            gate.gateOpenCondId = (ENGateOpenCondType)int.Parse(gatesItem[i][sgateOpenCondId].ToString());
                            // open gate condition param
                            if (gatesItem[i].Keys.Contains(sgateOpenCondParam))
                            {
                                if (gatesItem[i][sgateOpenCondParam].IsArray)
                                {
                                    for (int n = 0; n < gatesItem[i][sgateOpenCondParam].Count; n++)
                                    {
                                        gate.gateOpenCondParam.Add(int.Parse(gatesItem[i][sgateOpenCondParam][n].ToString()));
                                    }
                                }
                            }
                            if (gatesItem[i].Keys.Contains(sgateOpenCondCount))
                            {
                                if (gatesItem[i][sgateOpenCondCount].IsArray)
                                {
                                    for (int n = 0; n < gatesItem[i][sgateOpenCondCount].Count; n++)
                                    {
                                        gate.gateOpenCondCount.Add(int.Parse(gatesItem[i][sgateOpenCondCount][n].ToString()));
                                    }
                                }
                            }
                            //bridge info
                            if (gatesItem[i].Keys.Contains(sbridge))
                            {
                                JsonData bridgeItem = gatesItem[i][sbridge];
                                if (bridgeItem.IsObject)
                                {
                                    gate.BridgeData = new Bridge();
                                    gate.BridgeData.bridgeObjId = int.Parse(bridgeItem[sbridgeObjId].ToString());
                                    gate.BridgeData.bridgeCenterLength = int.Parse(bridgeItem[sbridgeCenterLength].ToString());
                                    SceneRoomInfoTree childInfo = new SceneRoomInfoTree();
                                    childInfo.BuildTree(bridgeItem, this, stargetRoom);
                                    gate.BridgeData.nextRoomNode = childInfo;
                                    m_nextActiveRoom.Add(childInfo);
                                }
                            }
                        }
                        m_gateList.Add(gate);
                    }
                }
                // monsters
                if (data.Keys.Contains(smonsters))
                {
                    JsonData monsterDataItem = data[smonsters];
                    if (monsterDataItem.IsArray)
                    {
                        for (int i = 0; i < monsterDataItem.Count; i++)
                        {
                            MonsterRoomData monsterData = new MonsterRoomData();
                            monsterData.monsterId = int.Parse(monsterDataItem[i][smonsterId].ToString());
                            monsterData.monsterObjId = int.Parse(monsterDataItem[i][smonsterObjId].ToString());
                            monsterData.position.x = int.Parse(monsterDataItem[i][sposition][sposx].ToString());
                            monsterData.position.z = int.Parse(monsterDataItem[i][sposition][sposy].ToString());
                            monsterData.appearCondId = (ENMonsterAppearType)int.Parse(monsterDataItem[i][sappearCondId].ToString());
                            if (monsterDataItem[i].Keys.Contains(sdropList))
                            {
                                for (int j = 0; j < monsterDataItem[i][sdropList].Count; j++)
                                {
                                    monsterData.dropList.Add(int.Parse(monsterDataItem[i][sdropList][j].ToString()));
                                }
                            }
                            m_monsterDataList.Add(monsterData);
                        }
                    }
                }

                //解析Json每个房间中的宝箱的数据
                if (data.Keys.Contains(sTreasure))
                {
                    JsonData treasureDataItem = data[sTreasure];
                    if (treasureDataItem.IsArray)
                    {
                        for (int i = 0; i < treasureDataItem.Count; i++)
                        {
                            MonsterRoomData treasureData = new MonsterRoomData();
                            treasureData.monsterId = int.Parse(treasureDataItem[i][sTreasureId].ToString());
                            treasureData.monsterObjId = int.Parse(treasureDataItem[i][sTreasureObjId].ToString());
                            treasureData.position.x = int.Parse(treasureDataItem[i][sposition][sposx].ToString());
                            treasureData.position.z = int.Parse(treasureDataItem[i][sposition][sposy].ToString());
                            //treasureData.appearCondId = (ENMonsterAppearType)int.Parse(treasureDataItem[i][sappearCondId].ToString());
                            m_treasureDataList.Add(treasureData);
                        }
                    }
                }
                {
//                     TrapData trapData = new TrapData();
//                     trapData.trapId = 201;
//                     trapData.trapObjId = 2;
//                     trapData.position.x = 5;
//                     trapData.position.z = 5;
//                     trapData.mTrapState = (RMESchemeEdit.BeginState)1;
//                     //treasureData.appearCondId = (ENMonsterAppearType)int.Parse(treasureDataItem[i][sappearCondId].ToString());
//                     m_trapDataList.Add(trapData);
//                     trapData = new TrapData();
//                     trapData.trapId = 202;
//                     trapData.trapObjId = 2;
//                     trapData.position.x = 7;
//                     trapData.position.z = 7;
//                     trapData.mTrapState = (RMESchemeEdit.BeginState)1;
//                     //treasureData.appearCondId = (ENMonsterAppearType)int.Parse(treasureDataItem[i][sappearCondId].ToString());
//                     m_trapDataList.Add(trapData);
                }
                //解析Json每个房间中的机关的数据
                if (data.Keys.Contains(sTrap))
                {
                    JsonData trapDataItem = data[sTrap];
                    if (trapDataItem.IsArray)
                    {
                        for (int i = 0; i < trapDataItem.Count; i++)
                        {
                            TrapData trapData = new TrapData();
                            trapData.trapId = int.Parse(trapDataItem[i][sTrapId].ToString());
                            trapData.trapObjId = int.Parse(trapDataItem[i][sTrapObjId].ToString());
                            trapData.position.x = int.Parse(trapDataItem[i][sposition][sposx].ToString());
                            trapData.position.z = int.Parse(trapDataItem[i][sposition][sposy].ToString());
                            trapData.beginState = int.Parse(trapDataItem[i][sBeginState].ToString()) == 1;
                            trapData.beginEable = int.Parse(trapDataItem[i][sBeginEable].ToString()) == 1;
                            trapData.mTrapState = (Trap.TrapState)int.Parse(trapDataItem[i][sBeginState].ToString());
                            //treasureData.appearCondId = (ENMonsterAppearType)int.Parse(treasureDataItem[i][sappearCondId].ToString());
                            m_trapDataList.Add(trapData);
                        }
                    }
                }
                //当前房间是否要用到钥匙
//                 if ()
//                 {
//                     mIsNeedKey = true;
//                 }
            }
        }
        public SceneRoomInfoTree Lookup(int guid)
        {
            if (m_GUID == guid)
            {
                return this;
            }
            foreach (SceneRoomInfoTree info in m_nextActiveRoom)
            {
                SceneRoomInfoTree r = info.Lookup(guid);
                if (r != null)
                {
                    return r;
                }
            }
            return null;
        }
        // 生成场景资源数据
        public IEnumerator BuildSceneResObj(SERoomDataTree sce, GameResPackage.AsyncLoadPackageData loadData, int gateIndex = -1)
        {
            string obstacleFileName = "Prefabs/" + m_RoomPrefabName + "-bytes.bytes";

            GameResPackage.AsyncLoadObjectData loadData2 = new GameResPackage.AsyncLoadObjectData();
            IEnumerator e = PoolManager.Singleton.Coroutine_Load(obstacleFileName, loadData2, false, false);
            while (true)
            {
                e.MoveNext();
                if (loadData2.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
            if (loadData2.m_obj != null)
            {
                TextAsset asset = loadData2.m_obj as TextAsset;
                if (asset == null)
                {
                    Debug.Log("obstacleFileName=" + obstacleFileName);
                }
                else
                {
                    m_obstacleData = new SceneObstacleData();
                    m_obstacleData.Load(asset.bytes);
                }
            }
            loadData2 = new GameResPackage.AsyncLoadObjectData();
            e = PoolManager.Singleton.Coroutine_Load(GameData.GetPrefabPath(m_RoomPrefabName), loadData2, false, false);
            while (true)
            {
                e.MoveNext();
                if (loadData2.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
            if (loadData2.m_obj != null)
            {
                sce.mRoomObj = loadData2.m_obj as GameObject;
            }
            string roomDataName = m_RoomPrefabName + "-data";
            loadData2 = new GameResPackage.AsyncLoadObjectData();
            e = PoolManager.Singleton.Coroutine_Load(GameData.GetPrefabPath(roomDataName), loadData2, false, false);
            while (true)
            {
                e.MoveNext();
                if (loadData2.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
            if (loadData2.m_obj != null)
            {
                sce.mRoomData = loadData2.m_obj as GameObject;
            }
            sce.mRoomInfoTree = this;
//             for (int i = 0; i < m_gateList.Count; i++)
//             {
//                 sce.mGateData[i] = new SEGateData();
//                 if (null != m_gateList[i].BridgeData)
//                 {
//                     SceneBridgeInfo bridgeInfo = GameTable.SceneBridgeTableAsset.Lookup(m_gateList[i].BridgeData.bridgeObjId);
//                     if (null != bridgeInfo)
//                     {
//                         loadData2 = new GameResPackage.AsyncLoadObjectData();
//                         e = PoolManager.Singleton.Coroutine_Load(GameData.GetPrefabPath(bridgeInfo.bridgeHeadPrefab), loadData2, false, false);
//                         while (true)
//                         {
//                             e.MoveNext();
//                             if (loadData2.m_isFinish)
//                             {
//                                 break;
//                             }
//                             yield return e.Current;
//                         }
//                         if (loadData2.m_obj != null)
//                         {
//                             sce.mGateData[i].mBridgeHeadObj = loadData2.m_obj as GameObject;
//                         }
//                         loadData2 = new GameResPackage.AsyncLoadObjectData();
//                         e = PoolManager.Singleton.Coroutine_Load(GameData.GetPrefabPath(bridgeInfo.bridgeBodyPrefab), loadData2, false, false);
//                         while (true)
//                         {
//                             e.MoveNext();
//                             if (loadData2.m_isFinish)
//                             {
//                                 break;
//                             }
//                             yield return e.Current;
//                         }
//                         if (loadData2.m_obj != null)
//                         {
//                             sce.mGateData[i].mBridgeBodyObj = loadData2.m_obj as GameObject;
//                         }
//                         loadData2 = new GameResPackage.AsyncLoadObjectData();
//                         e = PoolManager.Singleton.Coroutine_Load(GameData.GetPrefabPath(bridgeInfo.bridgeTailPrefab), loadData2, false, false);
//                         while (true)
//                         {
//                             e.MoveNext();
//                             if (loadData2.m_isFinish)
//                             {
//                                 break;
//                             }
//                             yield return e.Current;
//                         }
//                         if (loadData2.m_obj != null)
//                         {
//                             sce.mGateData[i].mBridgeTailObj = loadData2.m_obj as GameObject;
//                         }
//                         //sce.mGateData[i].mBridgeHeadObj = GameData.LoadPrefab<GameObject>(bridgeInfo.bridgeHeadPrefab);
//                         //sce.mGateData[i].mBridgeBodyObj = GameData.LoadPrefab<GameObject>(bridgeInfo.bridgeBodyPrefab);
//                     }
//                     sce.mGateData[i].mIsHaveTunnel = true;
//                     sce.mGateData[i].mBridgeCenterLength = m_gateList[i].BridgeData.bridgeCenterLength;
//                     sce.mGateData[i].mNextRoomNode = new SERoomDataTree();
//                     m_gateList[i].isGateOpen = true;
//                     GameResPackage.AsyncLoadPackageData loadData3 = new GameResPackage.AsyncLoadPackageData();
//                     IEnumerator e6 = m_gateList[i].BridgeData.nextRoomNode.BuildSceneResObj(sce.mGateData[i].mNextRoomNode, loadData3, (i + 2) % 4);//一共四个门0对应2,1对应3。0+2%4= 2,1+2%4=3,2+2%4=0,3+2%4=1。·
//                     while (true)
//                     {
//                         e6.MoveNext();
//                         if (loadData3.m_isFinish)
//                         {
//                             break;
//                         }
//                         yield return e6.Current;
//                     }
//                 }
//                 else
//                 {//当该房间的这个门是没有联通出去的路的，但是这个门是另外房间进来的门那这个门要有意义。否则无意义
//                     if (gateIndex == i)
//                     {
//                         m_gateList[i].isGateOpen = true;
//                         m_gateList[i].gateOpenCondId = ENGateOpenCondType.enNone;
//                     }
//                     else
//                     {
//                         m_gateList[i].isGateOpen = false;
//                     }
//                 }
//             }
            loadData.m_isFinish = true;
        }
        // 获得场景挂节点
        static public List<string> strGatesPrefix = new List<string>();
        public void BuildLocatorObj(SERoomDataTree dataTree)
        {
            m_roomObj           = dataTree.mOutRoomObj;
            m_roomData          = dataTree.mRoomData;
            RoomCfg roomCfg     = m_roomObj.AddComponent<RoomCfg>();
            roomCfg.GUID        = m_GUID;
            roomCfg.roomObjID   = m_roomObjID;
            m_roomObj.transform.localPosition = new Vector3(m_roomObj.transform.localPosition.x, -0.1f, m_roomObj.transform.localPosition.z);
            m_locatorObj        = m_roomObj.transform.Find("Locator").gameObject;
            m_RTLocatorObj      = m_roomObj.transform.Find("LocatorRT").gameObject;
//             for (int i = 0; i < m_gateList.Count; i++ )
//             {
//                 if (!m_gateList[i].isGateOpen)
//                 {
//                     Transform childTrans = m_roomObj.transform.Find("GateLocate_" + strGatesPrefix[i]);
//                     if (null != childTrans)
//                     {
//                         childTrans.gameObject.SetActive(false);
//                     }
//                     continue;
//                 }
//                 
//                 Transform childTrans1 = m_roomObj.transform.Find("RemovableGate_" + strGatesPrefix[i]);
//                 if (null != childTrans1)
//                 {
//                     childTrans1.gameObject.SetActive(false);
//                 }
//                 //重新生成障碍相关的二维表数据
//                 ConfigObstacleArray((ENGateDirection)i);
//                 //房间门位置相关信息生成
//                 childTrans1 = m_roomObj.transform.Find("GateLocate_" + strGatesPrefix[i]);
//                 if (null != childTrans1)
//                 {
//                     childTrans1.gameObject.SetActive(false);
//                     SceneGateInfo gateInfo = GameTable.SceneGateTableAsset.Lookup(m_gateList[i].gateObjId);
//                     if (null != gateInfo)
//                     {
//                         GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
//                         IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetPrefabPath(gateInfo.prefabName), data);
//                         while (true)
//                         {
//                             e.MoveNext();
//                             if (data.m_isFinish)
//                             {
//                                 break;
//                             }
//                             yield return e.Current;
//                         }
//                         if (data.m_obj != null)
//                         {
//                             GameObject obj = data.m_obj as GameObject;
//                             obj.transform.parent = childTrans1.parent;
//                             obj.transform.position = new Vector3(childTrans1.position.x, 0f, childTrans1.position.z);
//                             obj.transform.rotation = childTrans1.rotation;
//                             GateCfg gatecfg = obj.AddComponent<GateCfg>();
//                             gatecfg.OpenConditionType = m_gateList[i].gateOpenCondId;
//                             gatecfg.OpenCondParamList = m_gateList[i].gateOpenCondParam;
//                             gatecfg.OpenCondCountList = m_gateList[i].gateOpenCondCount;
//                             obj.SetActive(true);
//                             m_gateList[i].OutGateObject = obj;
//                         }
//                     }
//                 }
//                 if (null != m_gateList[i].BridgeData)
//                 {
//                     GameResPackage.AsyncLoadPackageData loadData2 = new GameResPackage.AsyncLoadPackageData();
//                     IEnumerator e2 = m_gateList[i].BridgeData.nextRoomNode.BuildLocatorObj(dataTree.mGateData[i].mNextRoomNode, loadData2);
//                     while (true)
//                     {
//                         e2.MoveNext();
//                         if (loadData2.m_isFinish)
//                         {
//                             break;
//                         }
//                         yield return e2.Current;
//                     }
//                     m_gateList[i].GateToRoomObjID = m_gateList[i].BridgeData.nextRoomNode.m_GUID;
//                     m_gateList[i].GateOpenAnim(false);
//                 }
//                 else
//                 {
//                     m_gateList[i].GateOpenAnim(true);
//                 }
//             }
//            loadData.m_isFinish = true;
        }
        //配置障碍二维表
        public void ConfigObstacleArray(ENGateDirection enDir, bool isHave = false)
        {
            if (enDir != ENGateDirection.enNone)
            {
                for (int k = 0; k < m_obstacleData.mData.Count; k++)
                {
                    if (m_obstacleData.mData[k].enDirection == enDir)
                    {
                        SM.SceneObstacleData.ObData d = m_obstacleData.mData[k];
                        d.obstacleArray.z = (isHave ? 1 : 0);
                        m_obstacleData.mData[k] = d;
                    }
                }
            }
        }
        public void RebuildFinderPathData(ENGateDirection enDir, bool isHave)
        {
            ConfigObstacleArray(enDir, isHave);
            Pathfinder finder = m_roomObj.GetComponent<Pathfinder>();
            finder.RebuildFinderPathData(m_obstacleData);
        }
        //房间配置寻路数据
        public IEnumerator ConfigPathfinderData(GameResPackage.AsyncLoadPackageData loadData)
        {
            //寻路组件
            Pathfinder finder = m_roomObj.AddComponent<Pathfinder>();
            finder.MapStartPosition = new Vector2(m_locatorObj.transform.position.x, m_locatorObj.transform.position.z);
            finder.MapEndPosition = new Vector2(m_RTLocatorObj.transform.position.x, m_RTLocatorObj.transform.position.z);
            finder.Tilesize = GameSettings.Singleton.m_roomPathfinderTileSize;
            finder.CreateMap(m_obstacleData);
            for (int i = 0; i < m_gateList.Count; i++)
            {
                if (!m_gateList[i].isGateOpen)
                {
                    continue;
                }
                if (null != m_gateList[i].BridgeData)
                {
                    GameResPackage.AsyncLoadPackageData loadData2 = new GameResPackage.AsyncLoadPackageData();
                    IEnumerator e = m_gateList[i].BridgeData.nextRoomNode.ConfigPathfinderData(loadData2);
                    while (true)
                    {
                        e.MoveNext();
                        if (loadData2.m_isFinish)
                        {
                            break;
                        }
                        yield return e.Current;
                    }
                }
            }
            loadData.m_isFinish = true;
        }

        //构建怪物信息，用于预加载
        public void BuildPreloadData()
        {
            for (int i = 0; i < m_monsterDataList.Count; i++)
            {
                MonsterData monsterDyncData = SM.RandomRoomLevel.Singleton.LookupMonsterData(m_monsterDataList[i].monsterObjId);
                if (null != monsterDyncData)
                {
                    NPCInfo npcInfo = GameTable.NPCInfoTableAsset.Lookup(monsterDyncData.MonsterSettingID);
                    if (null != npcInfo)
                    {
                        ModelInfo info = GameTable.ModelInfoTableAsset.Lookup(npcInfo.ModelId);
                        if (null != info)
                        {
                            ModelNameList.Add(info.ModelFile);
                        }
                        else
                        {
                            Debug.LogError("monsterDyncData.MonsterSettingID :" + monsterDyncData.MonsterSettingID + " npcInfo.ModelId:" + npcInfo.ModelId+" not exist!!");
                        }
                        
                    }
                    else
                    {
                        Debug.LogError("monsterDyncData.MonsterSettingID not exist!!! :" + monsterDyncData.MonsterSettingID);
                    }
                    
                }
            }
            for (int i = 0; i < m_gateList.Count; i++)
            {
                if (!m_gateList[i].isGateOpen)
                {
                    continue;
                }
                if (null != m_gateList[i].BridgeData)
                {
                    m_gateList[i].BridgeData.nextRoomNode.BuildPreloadData();
                }
            }
        }
        //构建房间之间的寻路数据
        public bool FindRoomPathData(List<int>nodeList, int nEndID)
        {
            if (m_GUID == nEndID)
            {
                nodeList.Add(nEndID);
                return true;
            }
            for (int i = 0; i < m_gateList.Count; i++)
            {
                if (!m_gateList[i].isGateOpen)
                {
                    continue;
                }
                if (null != m_gateList[i].BridgeData)
                {
                    if (m_gateList[i].BridgeData.nextRoomNode.FindRoomPathData(nodeList, nEndID))
                    {
                        nodeList.Add(m_GUID);
                        return true;
                    }
                }
            }
            return false;
        }
        public void Destory()
        {
            //ConnectNextRoomPosList.Clear();
            if (null != m_roomObj)
            {
                GameObject.Destroy(m_roomObj);
            }
            for (int i = 0; i < m_gateList.Count; i++)
            {
                if (!m_gateList[i].isGateOpen)
                {
                    continue;
                }
                if (m_gateList[i].OutGateObject)
                {
                    GameObject.Destroy(m_gateList[i].OutGateObject);
                }
                if (null != m_gateList[i].BridgeData)
                {
                    m_gateList[i].BridgeData.nextRoomNode.Destory();
                }
                m_gateList[i].Reset();
            }
        }
        public Gate GetGate(int GateId)
        {
            foreach (var gate in m_gateList)
            {
                if (gate.gateId == GateId)
                {
                    return gate;
                }
            }
            return null;
        }
    }
    // 服务器发送过来的关于怪物的有关属性
    public class SkillData
    {
        public int          SkillID = 0;
        public List<int>    SkillParamList = new List<int>();
    }
    public class MonsterData
    {
        //todo : monster data parsing key string
        static string smonsterObjId = "monsterObjId";
        static string smonsterSettingId = "monsterSettingId";
        static string shp = "hp";
        static string satk = "atk";
        static string sskills = "skills";
        static string sskillId = "skillId";
        static string sskillParam = "skillParam";
        //Actor的基本属性   mHp  mPhyAtk  mPhyDef  mMagAtk  mMagDef
        static string sHp = "mHp";
        static string sPhyAtk = "mPhyAtk";
        static string sPhyDef = "mPhyDef";
        static string sMagAtk = "mMagAtk";
        static string sMagDef = "mMagDef";
        static string sSkillList = "mSkillList";
        static string sPassiveSkillList = "mPassiveSkillList";
        static string sDropList = "mDropList";
        static string sMonsterType = "mMonsterType";
        static string sChangeScal = "changeScale";
        //////////////////////////////////////////////////////////////////////////
        public int MonsterObjID { get; set; }
        public int MonsterSettingID { get; set; }
        public int HP { get; set; }
        public int PhyAtk { get; set; }
        public int PhyDef { get; set; }
        public int MagAtk { get; set; }
        public int MagDef { get; set; }
        public List<int> SkillList = new List<int>();
        public List<int> PassiveSkillList = new List<int>();
        public List<int> DropList = new List<int>();
        public int MonsterType { get; set; }
        public float ChangeScale { get; set; }
        public List<SkillData> SkillDataList = new List<SkillData>();

        public void ParseData(JsonData data)
        {
            MonsterObjID = int.Parse(data[smonsterObjId].ToString());
            MonsterSettingID = int.Parse(data[smonsterSettingId].ToString());
//             HP = int.Parse(data[shp].ToString());
//             Atk = int.Parse(data[satk].ToString());
            if (data.Keys.Contains(sskills))
            {
                JsonData skillItems = data[sskills];
                if (skillItems.IsArray)
                {
                    for (int i = 0; i < skillItems.Count; i++)
                    {
                        SkillData skillData = new SkillData();
                        skillData.SkillID = int.Parse(skillItems[i][sskillId].ToString());
                        if (skillItems[i].Keys.Contains(sskillParam))
                        {
                            if (skillItems[i][sskillParam].IsArray)
                            {
                                for (int n = 0; n < skillItems[i][sskillParam].Count; n++)
                                {
                                    int nParam = int.Parse(skillItems[i][sskillParam][n].ToString());
                                    skillData.SkillParamList.Add(nParam);
                                }
                            }
                        }
                        SkillDataList.Add(skillData);
                    }
                }
            }
            //Monster的基本属性
            HP = GetPropValue(data, sHp);
            PhyAtk = GetPropValue(data, sPhyAtk);
            PhyDef = GetPropValue(data, sPhyDef);
            MagAtk = GetPropValue(data, sMagAtk);
            MagDef = GetPropValue(data, sMagDef);
            //Monster 技能列表
            if (data.Keys.Contains(sSkillList))
            {
                JsonData skillItems = data[sSkillList];
                if (skillItems.IsArray)
                {
                    for (int i = 0; i < skillItems.Count; i++)
                    {
                        SkillList.Add(int.Parse(skillItems[i].ToString()));
                    }
                }
            }
            //Monster 被动技能列表
            if (data.Keys.Contains(sPassiveSkillList))
            {
                JsonData skillItems = data[sPassiveSkillList];
                if (skillItems.IsArray)
                {
                    for (int i = 0; i < skillItems.Count; i++)
                    {
                        PassiveSkillList.Add(int.Parse(skillItems[i].ToString()));
                    }
                }
            }
            //Monster 类型
            MonsterType = GetPropValue(data, sMonsterType);
            //Monster 比例大小
            ChangeScale = GetPropValue_Float(data, sChangeScal);
        }
        public int GetPropValue(JsonData data, string propType)
        {
            if (data.Keys.Contains(propType))
            {
                return int.Parse(data[propType].ToString());
            }
            return -1;
        }
        public float GetPropValue_Float(JsonData data, string propType)
        {
            if (data.Keys.Contains(propType))
            {
                return float.Parse(data[propType].ToString());
            }
            return -1;
        }
    }

    public class BlackBoardActorData
    {
        public int roomID = -1;
        public int actorID = -1;
        public LevelBlackboard.BlackActorType mBlackActorType = LevelBlackboard.BlackActorType.enNone;
        public void Reset()
        {
            roomID = -1;
            actorID = -1;
            mBlackActorType = LevelBlackboard.BlackActorType.enNone;
        }
    }
}