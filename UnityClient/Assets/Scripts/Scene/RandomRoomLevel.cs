using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using LitJson;


#region SceneManager

public class SceneRoomInfoTree
{
    public static int m_nIndex = -2;
    public static int m_nID = -2;
    public static List<string> ModelNameList = new List<string>();
    public int m_roomID;
    public int m_GUID = -1;
    public string m_designRoomName;
    public GameObject m_designRoomObj;
    public string m_artRoomName;
    public SceneOneRoomConfig m_cfgForArtBuild;
    public SceneRoomInfoTree m_parent = null;
    public int m_branchCount = 0;
    public bool m_isBranch = false;
    public List<SceneRoomInfoTree> m_nextActiveRoom = new List<SceneRoomInfoTree>();
    public SceneRoomInfoTree()
    {
        m_isBranch = false;
        m_branchCount = 0;
        m_nIndex++;
        m_GUID = m_nIndex + 10;
    }
    public void BuildRoomInfo(int roomID)
    {
        m_roomID = roomID;
        RoomAttrInfo info = GameTable.RoomAttrTableAsset.Lookup(m_roomID);
        if (null != info)
        {
            int nIdx = UnityEngine.Random.Range(0, info.DesignResList.Count);
            m_designRoomName = info.DesignResList[nIdx];
            string prefabName = "Room/" + m_designRoomName;
            PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetPrefabPath(prefabName), LoadDesignObjCallback);
            //m_designRoomObj = GameObject.Instantiate(GameData.LoadPrefab<GameObject>(prefabName)) as GameObject;
            m_artRoomName = info.ArtRes;
        }
    }
    void LoadDesignObjCallback(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            m_designRoomObj = GameObject.Instantiate(obj) as GameObject;
        }
    }
    public void BuildTree(RoomNodeTree nodeTree, SceneRoomInfoTree parent)
    {
        m_parent = parent;
        RoomAttrInfo info = GameTable.RoomAttrTableAsset.Lookup(m_roomID);
        if (null != info)
        {
            if (info.DesignResList.Count > 0)
            {
                int nIdx = UnityEngine.Random.Range(0, info.DesignResList.Count);
                m_designRoomName = info.DesignResList[nIdx];
                string prefabName = "Room/" + m_designRoomName;
                PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetPrefabPath(prefabName), LoadDesignObjCallback);
                //m_designRoomObj = GameObject.Instantiate(GameData.LoadPrefab<GameObject>(prefabName)) as GameObject;
            }
            m_artRoomName = info.ArtRes;
            m_cfgForArtBuild = new SceneOneRoomConfig();
            if (info.BranchFlag > 0)
            {
                m_cfgForArtBuild.m_id = m_parent.m_branchCount;
                m_parent.m_branchCount++;
                m_isBranch = true;

            }
            else
            {
                m_nID++;
                m_cfgForArtBuild.m_id = m_nID;
                m_isBranch = false;
            }
        }
        m_nextActiveRoom.Clear();
        for (int i = 0; i < nodeTree.mNextNode.Count; i++)
        {
            SceneRoomInfoTree nextRoomTree = new SceneRoomInfoTree();
            nextRoomTree.m_roomID = nodeTree.mNextNode[i].mID;
            nextRoomTree.BuildTree(nodeTree.mNextNode[i], this);
            m_nextActiveRoom.Add(nextRoomTree);
        }
    }
    public void BuildSingleLine(List<SceneOneRoomConfig> cfgLine)
    {
        if (!m_isBranch)
        {
            m_cfgForArtBuild.RoofMini = new GameObject[m_branchCount];
            m_cfgForArtBuild.ChildObj = new GameObject[m_branchCount];
            cfgLine.Add(m_cfgForArtBuild);
        }
        for (int i = 0; i < m_nextActiveRoom.Count; i++)
        {
            m_nextActiveRoom[i].BuildSingleLine(cfgLine);
        }
    }
    //构建怪物信息，用于预加载
    public void BuildPreloadData()
    {
        GameObject obj = m_designRoomObj;//GameObject.Instantiate(GameData.LoadPrefab<GameObject>(prefabName)) as GameObject;
        if (m_isBranch)
        {
            if (m_parent != null)
            {
                obj.transform.position = m_parent.m_cfgForArtBuild.ChildObj[m_cfgForArtBuild.m_id].transform.position;
                obj.transform.rotation = m_parent.m_cfgForArtBuild.ChildObj[m_cfgForArtBuild.m_id].transform.rotation;
            }
        }
        else
        {
            if (m_cfgForArtBuild.m_object != null)
            {
                obj.transform.position = m_cfgForArtBuild.m_object.transform.position;
                obj.transform.rotation = m_cfgForArtBuild.m_object.transform.rotation;
            }
        }
        for (int n = 0; n < obj.transform.childCount; n++)
        {
            Transform child = obj.transform.GetChild(n);
            if (null != child)
            {
                RoomElement element = child.GetComponent<RoomElement>();
                if (null == element || !element.IsNpc || element.IsUsePrefabLoad)
                {
                    continue;
                }
                SpawnActor spawn = child.GetComponent<SpawnActor>();
                if (null == spawn)
                {
                    continue;
                }
                NPCInfo npcInfo = GameTable.NPCInfoTableAsset.Lookup(spawn.m_npcStaticID);
                ModelInfo info = GameTable.ModelInfoTableAsset.Lookup(npcInfo.ModelId);
                ModelNameList.Add(info.ModelFile);
            }
        }
        for (int i = 0; i < m_nextActiveRoom.Count; i++)
        {
            m_nextActiveRoom[i].BuildPreloadData();
        }
    }
    public void BuildSceneObj(SceneEffect sce)
    {
        MainGame.Singleton.StartCoroutine(Coroutine_LoadSceneObj(sce));
    }
    IEnumerator Coroutine_LoadSceneObj(SceneEffect sce)
    {
        string prefabName = "Scene/" + m_artRoomName;
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetPrefabPath(prefabName), data, false, false);
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
            GameObject sceneObj = data.m_obj as GameObject;
            if (m_cfgForArtBuild.m_id == -1)
            {
                sce.SceneObj.RoofFrist = sceneObj;
            }
            else if (m_cfgForArtBuild.m_id == sce.SceneObj.RoofNum)
            {
                sce.SceneObj.RoofEnd = sceneObj;
            }
            else
            {
                if (m_isBranch)
                {
                    m_parent.m_cfgForArtBuild.RoofMini[m_cfgForArtBuild.m_id] = sceneObj;
                    //sce.SceneObj.RoofMini[m_cfgForArtBuild.m_id] = sceneObj;
                }
                else
                {
                    sce.SceneObj.Roof[m_cfgForArtBuild.m_id] = sceneObj;
                }
            }
            for (int i = 0; i < m_nextActiveRoom.Count; i++)
            {
                m_nextActiveRoom[i].BuildSceneObj(sce);
            }
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
    public SceneRoomInfoTree LookupByRoomID(int roomID)
    {
        if (m_roomID == roomID)
        {
            return this;
        }
        foreach (SceneRoomInfoTree info in m_nextActiveRoom)
        {
            SceneRoomInfoTree r = info.LookupByRoomID(roomID);
            if (r != null)
            {
                return r;
            }
        }
        return null;
    }
}

#endregion
///////////////////////////////////////////////////////////////////////////////////////////////////////
namespace SM
{
    public class RandomRoomLevel : IPropertyObject
    {
        static public RandomRoomLevel Singleton { get; private set; }

        public Dictionary<int, List<Vector3>> m_scenePathNodeDic = new Dictionary<int, List<Vector3>>();

        public Dictionary<int, Vector3> m_sceneCampReliveNode = new Dictionary<int, Vector3>();

        public float m_sceneHeroReliveTime = 0;

        public enum ENPropertyChanged
        {
            enUIBossRoomWarning,
            enEnterDungeon,
            enBeginBattle,
            enEndBattle,
            enDungeonEnd,
        }
        // 当前FLOORID
        public int m_curFloorId = 0;
        public RandomRoomLevel()
        {
            if (null == Singleton)
            {
                Singleton = this;
                SetPropertyObjectID((int)MVCPropertyID.enSceneManager);
            }
            else
            {
                Debug.LogWarning("RandomRoomLevel Recreated");
            }
        }
        public static string sfloor = "floor";
        public static string smonsters = "monsters";
        public static string slevels = "levels";
        public static string sstartingRoom = "startingRoom";
        public static string sbossRoom = "bossRoom";
        public static string slevelId = "levelId";
        //场景文理资源预设
        public GameObject m_sceneObject = null;
        // monsters data map
        Dictionary<int, MonsterData> MonstersDict = new Dictionary<int, MonsterData>();
        // scene room info struct data list
        Dictionary<int, SceneRoomInfoTree> SceneRoomTreeList = new Dictionary<int, SceneRoomInfoTree>();
        //boss 房间数据
        Dictionary<int, SceneRoomInfoTree> BossRoomDict = new Dictionary<int, SceneRoomInfoTree>();
        // room map
        List<SceneRoom> mActiveRoomList = new List<SceneRoom>();
        // current level id
        public int mCurLevelId = 0;
        //总层数
        public int mTotalLevels = 0;
        //
        public Vector3 mLBPos = Vector3.zero;
        public Vector3 mRTPos = Vector3.zero;
        /// <summary>
        /// 
        public FindPathLogicData FindRoomNode(Vector3 pos)
        {
            return ScenePathfinder.Singleton.FindRoomNode(pos);
        }
        public FindPathLogicData FindRoomNode(Vector3 pos, int guid)
        {
            return ScenePathfinder.Singleton.FindRoomNode(pos, guid);
        }
        public bool QuickFindPath(Vector3 startPos, Vector3 endPos, bool isSearchAround = true)
        {
            FindPathLogicData endPathData = FindRoomNode(endPos);
            if (endPathData == null)
            {
                return false;
            }
            FindPathLogicData startPathData = FindRoomNode(startPos, endPathData.GUID);
            if (startPathData == null)
            {
                return false;
            }
            if (startPathData.GUID == endPathData.GUID)
            {
                if (startPathData.GUID != -1)
                {
                    SceneRoom sceneRoom = RandomRoomLevel.Singleton.LookupRoom(startPathData.GUID);
                    if (null == sceneRoom)
                    {
                        return false;
                    }
                    Pathfinder pf = sceneRoom.CurRoomInfo.m_roomObj.GetComponent<Pathfinder>();
                    if (null == pf)
                    {
                        return false;
                    }
                    return pf.QuickFindPath(startPos, endPos, isSearchAround);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                //跨房间寻路
                Vector3 sPos = new Vector3(startPathData.m_roomLogicX, 0f, startPathData.m_roomLogicY);
                Vector3 ePos = new Vector3(endPathData.m_roomLogicX, 0f, endPathData.m_roomLogicY);
                List<Vector3> nodeList = ScenePathfinder.Singleton.FindPath(sPos, ePos);
                if (nodeList.Count <= 0)
                {
                    return false;
                }
                Vector3 vstart = startPos;
                Vector3 vMidPos = Vector3.zero;
                for (int i = 0; i < nodeList.Count; i++)
                {
                    int x = Mathf.CeilToInt(nodeList[i].x);
                    int y = Mathf.CeilToInt(nodeList[i].z);
                    FindPathLogicData pathData = ScenePathfinder.Singleton.FindRoomNode(x, y);
                    //Debug.Log("nod x:" + x + " y:" + y+" guid:"+pathData.GUID);
                    if (i < nodeList.Count - 1)
                    {
                        int x1 = Mathf.CeilToInt(nodeList[i + 1].x);
                        int y1 = Mathf.CeilToInt(nodeList[i + 1].z);
                        FindPathLogicData nextPathData = ScenePathfinder.Singleton.FindRoomNode(x1, y1);
                        foreach (FindPathLogicData.LinkInfo linkData in pathData.m_links)
                        {
                            if (linkData.m_linkRoom.ID == nextPathData.ID)
                            {
                                vMidPos = linkData.m_linkPos;
                                break;
                            }
                        }
                    }
                    else
                    {
                        vMidPos = endPos;
                    }
                    if (pathData.GUID != -1)
                    {
                        SceneRoom sceneRoom = RandomRoomLevel.Singleton.LookupRoom(pathData.GUID);
                        if (null == sceneRoom)
                        {
                            return false;
                        }
                        Pathfinder pf = sceneRoom.CurRoomInfo.m_roomObj.GetComponent<Pathfinder>();
                        if (null == pf)
                        {
                            return false;
                        }
                        if (!pf.QuickFindPath(vstart, vMidPos, isSearchAround))
                        {
                            return false;
                        }
                        vstart = vMidPos;
                    }
                    else
                    {
                        vstart = vMidPos;
                    }

                }
            }
            return true;
        }
        public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos, ref bool isPath)
        {
            List<Vector3> posNodeList = new List<Vector3>();
            FindPathLogicData endPathData = FindRoomNode(endPos);
            if (endPathData == null)
            {
                isPath = false;
                return posNodeList;
            }
            FindPathLogicData startPathData = FindRoomNode(startPos, endPathData.GUID);
            if (startPathData == null)
            {
                isPath = false;
                return posNodeList;
            }
            //Debug.Log("FindPath start guid:" + startPathData.GUID + " end guid:" + endPathData.GUID);

            if (startPathData.GUID == endPathData.GUID)
            {
                if (startPathData.GUID != -1)
                {
                    SceneRoom sceneRoom = RandomRoomLevel.Singleton.LookupRoom(startPathData.GUID);
                    if (null == sceneRoom)
                    {
                        return posNodeList;
                    }
                    Pathfinder pf = sceneRoom.CurRoomInfo.m_roomObj.GetComponent<Pathfinder>();
                    if (null == pf)
                    {
                        return posNodeList;
                    }
                    posNodeList = pf.FindPath(startPos, endPos);
                    if (posNodeList.Count <= 0)
                    {
                        isPath = false;
                    }
                }
                else
                {
                    posNodeList.Add(startPos);
                    posNodeList.Add(endPos);
                }
            }
            else
            {
                //跨房间寻路
                Vector3 sPos = new Vector3(startPathData.m_roomLogicX, 0f, startPathData.m_roomLogicY);
                Vector3 ePos = new Vector3(endPathData.m_roomLogicX, 0f, endPathData.m_roomLogicY);
                List<Vector3> nodeList = ScenePathfinder.Singleton.FindPath(sPos, ePos);
                Vector3 vstart = startPos;
                Vector3 vMidPos = Vector3.zero;
                for (int i = 0; i < nodeList.Count; i++)
                {
                    int x = Mathf.CeilToInt(nodeList[i].x);
                    int y = Mathf.CeilToInt(nodeList[i].z);
                    FindPathLogicData pathData = ScenePathfinder.Singleton.FindRoomNode(x, y);
                    //Debug.Log("nod x:" + x + " y:" + y+" guid:"+pathData.GUID);
                    if (i < nodeList.Count - 1)
                    {
                        int x1 = Mathf.CeilToInt(nodeList[i + 1].x);
                        int y1 = Mathf.CeilToInt(nodeList[i + 1].z);
                        FindPathLogicData nextPathData = ScenePathfinder.Singleton.FindRoomNode(x1, y1);
                        foreach (FindPathLogicData.LinkInfo linkData in pathData.m_links)
                        {
                            if (linkData.m_linkRoom.ID == nextPathData.ID)
                            {
                                vMidPos = linkData.m_linkPos;
                                break;
                            }
                        }
                    }
                    else
                    {
                        vMidPos = endPos;
                    }
                    if (pathData.GUID != -1)
                    {
                        SceneRoom sceneRoom = RandomRoomLevel.Singleton.LookupRoom(pathData.GUID);
                        if (null == sceneRoom)
                        {
                            return posNodeList;
                        }
                        Pathfinder pf = sceneRoom.CurRoomInfo.m_roomObj.GetComponent<Pathfinder>();
                        if (null == pf)
                        {
                            return posNodeList;
                        }
                        List<Vector3> tmpList = pf.FindPath(vstart, vMidPos);
                        if (tmpList.Count <= 0)
                        {
                            isPath = false;
                            return posNodeList;
                        }
                        posNodeList.AddRange(tmpList);
                        vstart = vMidPos;
                    }
                    else
                    {
                        posNodeList.Add(vMidPos);
                        vstart = vMidPos;
                    }

                }
            }
            return posNodeList;
        }

        /// /////////////////////////////////////////////////////////////////////////////
        /// parse json data
        public void ParseDataBuildTree(string msgData)
        {
            //Debug.Log("msgData =" + msgData);
            JsonData data = JsonMapper.ToObject(msgData);
            BuildMonstersData(data);
            BuildSceneLevelsData(data);
//             if (data.Keys.Contains(sfloor))
//             {
//                 if (null != EventManager.Singleton)
//                 {
//                     EventManager.Singleton.ParseJsonEvents(data[sfloor]);
//                 }
//                 if (null != EResultManager.Singleton)
//                 {
//                     EResultManager.Singleton.ParseJsonResults(data[sfloor]);
//                 }
//             }
        }
        //解析当前Floor中拥有的怪物集合
        void BuildMonstersData(JsonData data)
        {
            MonstersDict.Clear();
            if (!data[sfloor].Keys.Contains(smonsters))
            {
                return;
            }
            JsonData jItem = data[sfloor][smonsters];
            for (int i = 0; i < jItem.Count; i++)
            {
                MonsterData monsterdata = new MonsterData();
                monsterdata.ParseData(jItem[i]);
                MonstersDict.Add(monsterdata.MonsterObjID, monsterdata);
            }
            // TODO: test monster data
            //             Debug.Log("monsters count =" + MonstersDict.Count);
            //             foreach (KeyValuePair<int, MonsterData> val in MonstersDict)
            //             {
            //                 Debug.Log("monsterObjid =" + val.Value.MonsterObjID + " settingid =" + val.Value.MonsterSettingID + " hp=" + val.Value.HP + " atk=" + val.Value.Atk);
            //             }
            ///////////////////////////////////////////////////////////////
        }
        // 构建层数据
        void BuildSceneLevelsData(JsonData data)
        {
            SceneRoomTreeList.Clear();
            BossRoomDict.Clear();
            if (!data[sfloor].Keys.Contains(slevels))
            {
                //return;
            }
            else
            {
                JsonData jItem = data[sfloor][slevels];
                int nLevelID = 0;
                mTotalLevels = jItem.Count;
                for (int i = 0; i < jItem.Count; i++)
                {
                    nLevelID = int.Parse(jItem[i][slevelId].ToString());
                    SceneRoomInfoTree infoTree = new SceneRoomInfoTree();
                    infoTree.BuildTree(jItem[i], null, sstartingRoom);
                    SceneRoomTreeList.Add(nLevelID, infoTree);
                    if (jItem[i].Keys.Contains(sbossRoom))
                    {
                        SceneRoomInfoTree bossInfo = new SceneRoomInfoTree();
                        bossInfo.mIsBossRoom = true;
                        bossInfo.BuildTree(jItem[i], null, sbossRoom);
                        BossRoomDict.Add(nLevelID, bossInfo);
                    }
                }
            }
            // TODO : test
            //             Debug.Log("levels count =" + SceneRoomTreeList.Count);
            //             foreach (KeyValuePair<int, SceneRoomInfoTree> val in SceneRoomTreeList)
            //             {
            //                 val.Value.PrintRoomInfo();
            //             }
            //             Debug.Log("boss room:");
            //             foreach (KeyValuePair<int, SceneRoomInfoTree> val in BossRoomDict)
            //             {
            //                 val.Value.PrintRoomInfo();
            //             }
        }
        #region Editor_Mode_ViewSceneOnEditor
        public void ViewSceneOnEditor(string resMsg, MonoBehaviour mono = null)
        {
            MonstersDict.Clear();
            SceneRoomTreeList.Clear();
            BossRoomDict.Clear();
            mActiveRoomList.Clear();

            RandomRoomLevel.Singleton.ParseDataBuildTree(resMsg);
            SceneRoomInfoTree.strGatesPrefix.Add("N");
            SceneRoomInfoTree.strGatesPrefix.Add("E");
            SceneRoomInfoTree.strGatesPrefix.Add("S");
            SceneRoomInfoTree.strGatesPrefix.Add("W");
            mCurLevelId = 1;
            SceneRoomInfoTree levelInfoTree = null;
            if (!SceneRoomTreeList.TryGetValue(1, out levelInfoTree))
            {
                return;
            }
            GameResPackage.AsyncLoadPackageData loadData = new GameResPackage.AsyncLoadPackageData();
            MonoBehaviour coroutineMono = null;
            if (mono != null)
            {
                coroutineMono = mono;
            }
            else
            {
                coroutineMono = MainGame.Singleton.MainScript;
            }
            coroutineMono.StartCoroutine(Coroutine_BuildRes(levelInfoTree, new Vector3(0f, 0f, 0f), loadData));
        }
        IEnumerator Coroutine_BuildRes(SceneRoomInfoTree levelInfoTree, Vector3 pos, GameResPackage.AsyncLoadPackageData loadData)
        {
            if (m_sceneObject == null)
            {
                GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
                IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetPrefabPath("Room/RandomMap"), data);
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
                    m_sceneObject = data.m_obj as GameObject;
                    m_sceneObject.SetActive(true);
                }
            }
            SERoomDataTree dataTree = new SERoomDataTree();
            //构建场景相关预设资源
            GameResPackage.AsyncLoadPackageData data2 = new GameResPackage.AsyncLoadPackageData();
            IEnumerator e2 = levelInfoTree.BuildSceneResObj(dataTree, data2);
            while (true)
            {
                e2.MoveNext();
                if (data2.m_isFinish)
                {
                    break;
                }
                yield return e2.Current;
            }
            //dataTree 传入底层生成场景
            InitRandMap(dataTree, pos, ENGateDirection.enNone);
            Debug.Log("LBPos:" + mLBPos + " RTPos:" + mRTPos);
            //生成房间门位置相关信息，配置房间数据
            levelInfoTree.BuildLocatorObj(dataTree);
//            GameResPackage.AsyncLoadPackageData data3 = new GameResPackage.AsyncLoadPackageData();
//            IEnumerator e3 = levelInfoTree.BuildLocatorObj(dataTree, data3);
//             while (true)
//             {
//                 e3.MoveNext();
//                 if (data3.m_isFinish)
//                 {
//                     break;
//                 }
//                 yield return e3.Current;
//             }
             loadData.m_isFinish = true;
        }
        #endregion

        public MonsterData LookupMonsterData(int objId)
        {
            MonsterData data = new MonsterData();
            if (MonstersDict.TryGetValue(objId, out data))
            {
                return data;
            }
            return null;
        }
        public SceneRoomInfoTree LookupBossRoomInfo(int levelId)
        {
            SceneRoomInfoTree bossInfo = null;
            if (BossRoomDict.TryGetValue(levelId, out bossInfo))
            {
                return bossInfo;
            }
            return null;
        }
        public SceneRoomInfoTree LookupLevelRoomInfo(int levelId)
        {
            SceneRoomInfoTree info = null;
            if (SceneRoomTreeList.TryGetValue(levelId, out info))
            {
                return info;
            }
            return null;
        }
        public SceneRoom LookupRoom(int guid)
        {
            foreach (SceneRoom room in mActiveRoomList)
            {
                if (guid == room.ID)
                {
                    return room;
                }
            }
            return null;
        }
        public SceneRoom LookupRoom(Vector3 vPos)
        {
            foreach (SceneRoom room in mActiveRoomList)
            {
                Vector3 startPos = room.CurRoomInfo.m_locatorObj.transform.position;
                Vector3 endPos = room.CurRoomInfo.m_RTLocatorObj.transform.position;
                if ((vPos.x > startPos.x && vPos.z > startPos.z) && (vPos.x < endPos.x && vPos.z < endPos.z))
                {
                    //room.CurRoomInfo.m_roomObj
                    return room;
                }
            }
            return null;
        }
        public int LookupRoomGUID(Vector3 vPos)
        {
            foreach (SceneRoom room in mActiveRoomList)
            {
                Vector3 startPos = room.CurRoomInfo.m_locatorObj.transform.position;
                Vector3 endPos = room.CurRoomInfo.m_RTLocatorObj.transform.position;
                if ((vPos.x > startPos.x && vPos.z > startPos.z) && (vPos.x < endPos.x && vPos.z < endPos.z))
                {
                    return room.ID;
                }
            }
            return -1;
        }
        //loading state 调用
        public void PrebuildSceneData()
        {
            foreach (KeyValuePair<int, SceneRoomInfoTree> val in SceneRoomTreeList)
            {
                val.Value.BuildPreloadData();
            }
        }
        //加载场景资源
        public void LoadNeedResource()
        {
            SceneRoomInfoTree.strGatesPrefix.Add("N");
            SceneRoomInfoTree.strGatesPrefix.Add("E");
            SceneRoomInfoTree.strGatesPrefix.Add("S");
            SceneRoomInfoTree.strGatesPrefix.Add("W");
            OnEnterlevel(1);
        }
        public void OnQuit()
        {
            foreach (KeyValuePair<int, SceneRoomInfoTree> val in SceneRoomTreeList)
            {
                val.Value.Destory();
            }
            foreach (KeyValuePair<int, SceneRoomInfoTree> val in BossRoomDict)
            {
                val.Value.Destory();
            }
            foreach (SceneRoom room in mActiveRoomList)
            {
                room.Destroy();
            }
            MonstersDict.Clear();
            SceneRoomTreeList.Clear();
            BossRoomDict.Clear();
            mActiveRoomList.Clear();
            PoolManager.Singleton.ReleaseObj(m_sceneObject);
            m_sceneObject = null;
        }
        public void OnEnterlevel(int levelId)
        {
            MainGame.Singleton.StartCoroutine(Coroutine_EnterLevel(levelId));
        }
        IEnumerator Coroutine_EnterLevel(int levelId)
        {
            //ScenePathfinder.Singleton.Reset();
            //////////////////////////////////////////
            mCurLevelId = levelId;
            SceneRoomInfoTree levelInfoTree = null;
            if (!SceneRoomTreeList.TryGetValue(levelId, out levelInfoTree))
            {
                //return;
            }
            else
            {
                GameResPackage.AsyncLoadPackageData loadData = new GameResPackage.AsyncLoadPackageData();
                IEnumerator e = Coroutine_BuildRes(levelInfoTree, new Vector3(0f, -0.1f, 0f), loadData);
                while (true)
                {
                    e.MoveNext();
                    if (loadData.m_isFinish)
                    {
                        break;
                    }
                    yield return e.Current;
                }
                //Debug.Log("xMin:" + RoomFindPathData.xMin + " yMin:" + RoomFindPathData.yMin);
                //生成场景层寻路数据
                //ScenePathfinder.Singleton.CreateMap();
                //寻路数据生成
                GameResPackage.AsyncLoadPackageData loadData2 = new GameResPackage.AsyncLoadPackageData();
                IEnumerator e2 = levelInfoTree.ConfigPathfinderData(loadData2);
                while (true)
                {
                    e2.MoveNext();
                    if (loadData2.m_isFinish)
                    {
                        break;
                    }
                    yield return e2.Current;
                }
                //激活第一个房间
                ActiveSceneRoom(levelInfoTree);
                //active boss
//                 SceneRoomInfoTree bossInfo = null;
//                 if (BossRoomDict.TryGetValue(levelId, out bossInfo))
//                 {
//                     ActiveBossRoom(bossInfo);
//                 }
                ///////////////////////////////////////////////////////////////////////////////
                while (true)
                {
                    if (ActorManager.Singleton.MainActor.MainObj != null && ActorManager.Singleton.MainActor.MainObj.transform != null)
                    {
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                }
                if (!ClientNet.Singleton.IsLongConnecting)
                {
                    ActorManager.Singleton.MainActor.ForceMoveToPosition(levelInfoTree.CharPosition);
                    if (ActorManager.Singleton.Comrade != null)
                    {
                        ActorManager.Singleton.Comrade.InitPartnerPosition();
                    }
                }
                MainGame.Singleton.MainCamera.MoveAtOnce(ActorManager.Singleton.MainActor);

                // 每进入一层 则 重置一下钥匙显示
                Reward.Singleton.NeedKey(levelInfoTree.mIsNeedKey);
            }
        }
        public void OnLeaveLevel(int levelId = 0)
        {
            if (levelId == 0)
            {
                levelId = mCurLevelId;
            }
            SceneRoomInfoTree levelInfoTree = null;
            if (!SceneRoomTreeList.TryGetValue(levelId, out levelInfoTree))
            {
                return;
            }
            levelInfoTree.Destory();
            foreach (SceneRoom room in mActiveRoomList)
            {
                room.Destroy();
            }
            mActiveRoomList.Clear();
        }
        public void ActiveBossRoom(SceneRoomInfoTree roomInfoTree)
        {
            MainGame.Singleton.StartCoroutine(Coroutine_ActiveRoom(roomInfoTree, new Vector3(264f, -0.1f, -256f)));
            //SERoomDataTree dataTree = new SERoomDataTree();
            //roomInfoTree.BuildSceneResObj(dataTree);
            ////dataTree 传入底层生成场景
            //InitRandMap(dataTree, new Vector3(264f, -0.1f, -256f), ENGateDirection.enNone);
            ////生成房间门位置相关信息，配置房间数据
            //roomInfoTree.BuildLocatorObj(dataTree);
            ////寻路数据生成
            //roomInfoTree.ConfigPathfinderData();
            //ActiveSceneRoom(roomInfoTree);
        }
        IEnumerator Coroutine_ActiveRoom(SceneRoomInfoTree roomInfoTree, Vector3 pos)
        {
            GameResPackage.AsyncLoadPackageData loadData = new GameResPackage.AsyncLoadPackageData();
            IEnumerator e = Coroutine_BuildRes(roomInfoTree, pos, loadData);
            while (true)
            {
                e.MoveNext();
                if (loadData.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
            //寻路数据生成
            GameResPackage.AsyncLoadPackageData loadData2 = new GameResPackage.AsyncLoadPackageData();
            IEnumerator e2 = roomInfoTree.ConfigPathfinderData(loadData2);
            while (true)
            {
                e2.MoveNext();
                if (loadData2.m_isFinish)
                {
                    break;
                }
                yield return e2.Current;
            }
            ActiveSceneRoom(roomInfoTree);
        }
        //active room
        public void ActiveSceneRoom(SceneRoomInfoTree roomInfoTree, SceneRoom preRoom = null)
        {
            SceneRoom room = new SceneRoom(roomInfoTree, preRoom);
            mActiveRoomList.Add(room);
            room.BuildRoomData(mCurLevelId);
        }
        public void FixedUpdate()
        {
            for (int i = 0; i < mActiveRoomList.Count; i++)
            {
                SceneRoom curRoom = mActiveRoomList[i];
                curRoom.Tick();
            }
            if (GameSettings.Singleton.m_isOpenFathfinderDebug)
            {
                ScenePathfinder.Singleton.DrawAABBLine();
            }
        }
        /// <summary>
        /// 随机地图生成逻辑
        /// </summary>
        /// <param name="dataTree"></param>
        /// <param name="roomPos"></param>
        /// <param name="preDirection"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="prePathData"></param>
        void InitRandMap(SERoomDataTree dataTree, Vector3 roomPos, ENGateDirection preDirection, int x = 0, int y = 0, FindPathLogicData preBridgePathData = null)
        {
            dataTree.mOutRoomObj = GameObject.Instantiate(dataTree.mRoomObj) as GameObject;
            //dataTree.mOutRoomObj.name = dataTree.mRoomInfoTree.m_roomName;
            dataTree.mOutRoomObj.AddComponent<SceneObjAttrib>();
            dataTree.mOutRoomObj.transform.position = roomPos;
            //ENGateDirection enDir = ENGateDirection.enNone;
            //Vector3 linkGatePos = Vector3.zero;
//             if (preDirection == ENGateDirection.enEAST)
//             {
//                 Transform gateTrans = dataTree.mOutRoomObj.transform.Find("GateLocate_W");
//                 Vector3 offset = dataTree.mOutRoomObj.transform.position - gateTrans.position;
//                 dataTree.mOutRoomObj.transform.position += offset;
//                 linkGatePos = gateTrans.position;
//                 gateTrans.gameObject.SetActive(false);
//                 Transform childTrans1 = dataTree.mOutRoomObj.transform.Find("RemovableGate_W");
//                 childTrans1.gameObject.SetActive(false);
//                 enDir = ENGateDirection.enWEST;
//             }
//             else if (preDirection == ENGateDirection.enWEST)
//             {
//                 Transform gateTrans = dataTree.mOutRoomObj.transform.Find("GateLocate_E");
//                 Vector3 offset = dataTree.mOutRoomObj.transform.position - gateTrans.position;
//                 dataTree.mOutRoomObj.transform.position += offset;
//                 linkGatePos = gateTrans.position;
//                 gateTrans.gameObject.SetActive(false);
//                 Transform childTrans1 = dataTree.mOutRoomObj.transform.Find("RemovableGate_E");
//                 childTrans1.gameObject.SetActive(false);
//                 enDir = ENGateDirection.enEAST;
//             }
//             else if (preDirection == ENGateDirection.enNORTH)
//             {
//                 Transform gateTrans = dataTree.mOutRoomObj.transform.Find("GateLocate_S");
//                 Vector3 offset = dataTree.mOutRoomObj.transform.position - gateTrans.position;
//                 dataTree.mOutRoomObj.transform.position += offset;
//                 linkGatePos = gateTrans.position;
//                 gateTrans.gameObject.SetActive(false);
//                 Transform childTrans1 = dataTree.mOutRoomObj.transform.Find("RemovableGate_S");
//                 childTrans1.gameObject.SetActive(false);
//                 enDir = ENGateDirection.enSOUTH;
//             }
//             else if (preDirection == ENGateDirection.enSOUTH)
//             {
//                 Transform gateTrans = dataTree.mOutRoomObj.transform.Find("GateLocate_N");
//                 Vector3 offset = dataTree.mOutRoomObj.transform.position - gateTrans.position;
//                 dataTree.mOutRoomObj.transform.position += offset;
//                 linkGatePos = gateTrans.position;
//                 gateTrans.gameObject.SetActive(false);
//                 Transform childTrans1 = dataTree.mOutRoomObj.transform.Find("RemovableGate_N");
//                 childTrans1.gameObject.SetActive(false);
//                 enDir = ENGateDirection.enNORTH;
//             }
            //当前房间的寻路逻辑数据
//             FindPathLogicData roomPathData = new FindPathLogicData();
//             roomPathData.GUID = dataTree.mRoomInfoTree.m_GUID;
//             roomPathData.m_roomLogicX = x;
//             roomPathData.m_roomLogicY = y;
//             Vector3 LBPos = dataTree.mOutRoomObj.transform.Find("Locator").position;
//             Vector3 RTPos = dataTree.mOutRoomObj.transform.Find("LocatorRT").position;
//             roomPathData.LBPos = FindPathLogicData.CalcMin(LBPos, RTPos);
//             roomPathData.RTPos = FindPathLogicData.CalcMax(LBPos, RTPos);
//             if (!dataTree.mRoomInfoTree.mIsBossRoom)
//             {
//                 mLBPos = FindPathLogicData.CalcMin(mLBPos, roomPathData.LBPos);
//                 mRTPos = FindPathLogicData.CalcMax(mRTPos, roomPathData.RTPos);
//             }
//             // Debug.Log("roomPathData.LBPos:" + roomPathData.LBPos + " roomPathData.RTPos:" + roomPathData.RTPos);
//             int X1 = x;
//             int Y1 = y;
            //构建连接信息
//             if (null != preBridgePathData)
//             {
//                 if (preDirection == ENGateDirection.enNORTH)
//                 {
//                     preBridgePathData.RTPos.z = roomPathData.LBPos.z;
//                     linkGatePos.z = roomPathData.LBPos.z;
//                 }
//                 else if (preDirection == ENGateDirection.enSOUTH)
//                 {
//                     preBridgePathData.LBPos.z = roomPathData.RTPos.z;
//                     linkGatePos.z = preBridgePathData.LBPos.z;
//                 }
//                 else if (preDirection == ENGateDirection.enWEST)
//                 {
//                     preBridgePathData.LBPos.x = roomPathData.RTPos.x;
//                     linkGatePos.x = preBridgePathData.LBPos.x;
//                 }
//                 else
//                 {
//                     preBridgePathData.RTPos.x = roomPathData.LBPos.x;
//                     linkGatePos.x = preBridgePathData.RTPos.x;
//                 }
//                 //房间与桥的连接信息
//                 FindPathLogicData.LinkInfo linkInfo = new FindPathLogicData.LinkInfo();
//                 linkInfo.m_linkRoom = preBridgePathData;
//                 linkInfo.m_linkPos = linkGatePos;
//                 roomPathData.m_links.Add(linkInfo);
//                 //桥与房间的连接信息
//                 FindPathLogicData.LinkInfo roomlinkInfo = new FindPathLogicData.LinkInfo();
//                 roomlinkInfo.m_linkRoom = roomPathData;
//                 roomlinkInfo.m_linkPos = linkGatePos;
//                 preBridgePathData.m_links.Add(roomlinkInfo);
            }
//             if (null != ScenePathfinder.Singleton)
//             {
//                 ScenePathfinder.Singleton.BuildRoomFindPathData(roomPathData);
//             }
//             else
//             {
//                 Debug.Log("ScenePathfinder.Singleton null !!!!");
//             }
            //配置障碍数据
//            dataTree.mRoomInfoTree.ConfigObstacleArray(enDir);
            //
//             for (int i = 0; i < dataTree.mGateData.Length; i++)
//             {
//                 if (!dataTree.mGateData[i].mIsHaveTunnel)
//                 {
//                     continue;
//                 }
//                 GameObject bridgeHeadObj = GameObject.Instantiate(dataTree.mGateData[i].mBridgeHeadObj) as GameObject;
//                 GameObject gateObj = dataTree.mOutRoomObj.transform.Find("GateLocate_" + SceneRoomInfoTree.strGatesPrefix[i]).gameObject;
//                 Transform localA = bridgeHeadObj.transform.Find("locator_A");
//                 bridgeHeadObj.transform.rotation = gateObj.transform.rotation;
//                 bridgeHeadObj.transform.parent = gateObj.transform.parent;
//                 bridgeHeadObj.transform.position = gateObj.transform.position;
//                 bridgeHeadObj.transform.position += (bridgeHeadObj.transform.position - localA.position);
//                 //bridgeHeadObj.transform.position = new Vector3(bridgeHeadObj.transform.position.x, 0f, bridgeHeadObj.transform.position.z);
//                 localA.gameObject.SetActive(false);
//                 //设置桥的起始坐标，相对从门开始的
//                 dataTree.mRoomInfoTree.m_gateList[i].BridgeData.startPos = localA.position;
//                 //房间连接数据,当前房间连接下一房间
//                 //                 RoomConnectData connData = new RoomConnectData();
//                 //                 connData.StartPos = gateObj.transform.position;
//                 //                 //下一房间连接当前房间
//                 //                 RoomConnectData preConnData = new RoomConnectData();
//                 //                 preConnData.EndPos = gateObj.transform.position;
//                 //计算AABB
//                 Vector3 AAPos = bridgeHeadObj.transform.Find("locator").position;
//                 Vector3 BBPos = bridgeHeadObj.transform.Find("locatorRT").position;
//                 Vector3 lb0 = FindPathLogicData.CalcMin(AAPos, BBPos);
//                 Vector3 rt0 = FindPathLogicData.CalcMax(AAPos, BBPos);
//                 GameObject CurObj = bridgeHeadObj;
//                 //3段桥身
//                 int nLength = dataTree.mGateData[i].mBridgeCenterLength == 0 ? 1 : dataTree.mGateData[i].mBridgeCenterLength;
//                 //Debug.Log("nLength:" + nLength);
//                 for (int j = 0; j < nLength; j++)
//                 {
//                     GameObject bridgeBodyObj = GameObject.Instantiate(dataTree.mGateData[i].mBridgeBodyObj) as GameObject;
//                     bridgeBodyObj.transform.rotation = CurObj.transform.rotation;
//                     bridgeBodyObj.transform.parent = CurObj.transform.parent;
// 
//                     Transform localB = CurObj.transform.Find("locator_B");
//                     Vector3 pos = localB.position;
//                     localB.gameObject.SetActive(false);
//                     localA = bridgeBodyObj.transform.Find("locator_A");
//                     bridgeBodyObj.transform.position = pos;
//                     bridgeBodyObj.transform.position += (bridgeBodyObj.transform.position - localA.position);
//                     //bridgeBodyObj.transform.position = new Vector3(bridgeBodyObj.transform.position.x, 0f, bridgeBodyObj.transform.position.z);
//                     Vector3 AAPos1 = bridgeBodyObj.transform.Find("locator").position;
//                     Vector3 BBPos1 = bridgeBodyObj.transform.Find("locatorRT").position;
//                     Vector3 lb1 = FindPathLogicData.CalcMin(AAPos1, BBPos1);
//                     Vector3 rt1 = FindPathLogicData.CalcMax(AAPos1, BBPos1);
//                     lb0 = FindPathLogicData.CalcMin(lb0, lb1);
//                     rt0 = FindPathLogicData.CalcMax(rt0, rt1);
//                     localA.gameObject.SetActive(false);
//                     CurObj = bridgeBodyObj;
//                 }
// 
//                 //桥尾
//                 GameObject bridgeTailObj = GameObject.Instantiate(dataTree.mGateData[i].mBridgeTailObj) as GameObject;
//                 //                 float fYRoteAngle = bridgeHeadObj.transform.eulerAngles.y;
//                 //                 fYRoteAngle = Mathf.Abs(fYRoteAngle) - 180f;
//                 //                 Vector3 eulerAngle = bridgeTailObj.transform.eulerAngles;
//                 //                 bridgeTailObj.transform.eulerAngles = new Vector3(eulerAngle.x, fYRoteAngle, eulerAngle.z);
//                 bridgeTailObj.transform.parent = CurObj.transform.parent;
//                 bridgeTailObj.transform.rotation = CurObj.transform.rotation;
// 
//                 localA = bridgeTailObj.transform.Find("locator_A");
//                 bridgeTailObj.transform.position = CurObj.transform.Find("locator_B").position;
//                 CurObj.transform.Find("locator_B").gameObject.SetActive(false);
//                 bridgeTailObj.transform.position += (bridgeTailObj.transform.position - localA.position);
//                 //bridgeTailObj.transform.position = new Vector3(bridgeTailObj.transform.position.x, 0f, bridgeTailObj.transform.position.z);
//                 localA.gameObject.SetActive(false);
//                 Vector3 targetPos = bridgeTailObj.transform.Find("locator_B").position;
//                 bridgeTailObj.transform.Find("locator_B").gameObject.SetActive(false);
//                 //设置桥的结束坐标，相对从门开始的
//                 dataTree.mRoomInfoTree.m_gateList[i].BridgeData.endPos = targetPos;
// 
//                 //计算AABB
//                 Vector3 AAPos2 = bridgeTailObj.transform.Find("locator").position;
//                 Vector3 BBPos2 = bridgeTailObj.transform.Find("locatorRT").position;
//                 Vector3 lb = FindPathLogicData.CalcMin(AAPos2, BBPos2);
//                 Vector3 rt = FindPathLogicData.CalcMax(AAPos2, BBPos2);
//                 AAPos = FindPathLogicData.CalcMin(lb0, lb);
//                 BBPos = FindPathLogicData.CalcMax(rt0, rt);
//                 //Debug.Log("x:" + x + " y:" + y);
//                 //创建寻路数据
//                 FindPathLogicData.CalcXY((ENGateDirection)i, ref x, ref y);
//                 FindPathLogicData bridgePathData = new FindPathLogicData();
//                 bridgePathData.m_roomLogicX = x;
//                 bridgePathData.m_roomLogicY = y;
//                 if ((ENGateDirection)i == ENGateDirection.enNORTH)
//                 {
//                     AAPos.z = roomPathData.RTPos.z;
//                     dataTree.mRoomInfoTree.m_gateList[i].BridgeData.startPos.z = AAPos.z;
//                 }
//                 else if ((ENGateDirection)i == ENGateDirection.enSOUTH)
//                 {
//                     BBPos.z = roomPathData.LBPos.z;
//                     dataTree.mRoomInfoTree.m_gateList[i].BridgeData.startPos.z = BBPos.z;
//                 }
//                 else if ((ENGateDirection)i == ENGateDirection.enWEST)
//                 {
//                     BBPos.x = roomPathData.LBPos.x;
//                     dataTree.mRoomInfoTree.m_gateList[i].BridgeData.startPos.x = AAPos.x;
//                 }
//                 else
//                 {
//                     AAPos.x = roomPathData.RTPos.x;
//                     dataTree.mRoomInfoTree.m_gateList[i].BridgeData.startPos.x = AAPos.x;
//                 }
//                 bridgePathData.LBPos = AAPos;
//                 bridgePathData.RTPos = BBPos;
//                 Debug.Log("bridgePathData.LBPos:" + AAPos + " bridgePathData.RTPos:" + BBPos);
//                 //当前房间所对应桥的连接信息
//                 FindPathLogicData.LinkInfo linkInfo = new FindPathLogicData.LinkInfo();
//                 linkInfo.m_linkRoom = bridgePathData;
//                 linkInfo.m_linkPos = dataTree.mRoomInfoTree.m_gateList[i].BridgeData.startPos;
//                 roomPathData.m_links.Add(linkInfo);
//                 //桥连接信息
//                 FindPathLogicData.LinkInfo gatelinkInfo = new FindPathLogicData.LinkInfo();
//                 gatelinkInfo.m_linkRoom = roomPathData;
//                 gatelinkInfo.m_linkPos = dataTree.mRoomInfoTree.m_gateList[i].BridgeData.startPos;
//                 bridgePathData.m_links.Add(gatelinkInfo);
//                 if (null != ScenePathfinder.Singleton)
//                 {
//                     ScenePathfinder.Singleton.BuildRoomFindPathData(bridgePathData);
//                 }
//                 else
//                 {
//                     Debug.Log("ScenePathfinder.Singleton null gatepathData!!!!");
//                 }
//                 //Debug.Log("x:" + x + " y:" + y);
//                 FindPathLogicData.CalcXY((ENGateDirection)i, ref x, ref y);
//                 //连接下一房间数据
//                 //connData.EndPos = targetPos;
//                 //dataTree.mRoomInfoTree.ConnectNextRoomPosList.Add(dataTree.mGateData[i].mNextRoomNode.mRoomInfoTree.m_GUID, connData);
//                 //连接上一房间数据
//                 //preConnData.StartPos = targetPos;
//                 //dataTree.mGateData[i].mNextRoomNode.mRoomInfoTree.ConnectNextRoomPosList.Add(dataTree.mRoomInfoTree.m_GUID, preConnData);
//                 //
//                 InitRandMap(dataTree.mGateData[i].mNextRoomNode, targetPos, (ENGateDirection)i, x, y, bridgePathData);
//                 x = X1;
//                 y = Y1;
//             }
//         }
    }

}