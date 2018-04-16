using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

//场景层寻路逻辑数据
public class FindPathLogicData
{
    static public int xMin = 0;
    static public int yMin = 0;
    static public int xMax = 0;
    static public int yMax = 0;
    /// <summary>
    /// ///////////////////////////////////////////////
    /// </summary>
    public bool Valid = false;
    public int GUID = -1;
    public int ID = 0;
    public Vector3 LBPos = new Vector3();
    public Vector3 RTPos = new Vector3();

    public struct LinkInfo
    {
        public FindPathLogicData m_linkRoom;
        public Vector3 m_linkPos;
    }
    public List<LinkInfo> m_links = new List<LinkInfo>();
    public int m_roomLogicX = 0;
    public int m_roomLogicY = 0;
    /// <summary>
    /// static function
    /// </summary>
    static public void CalcAABB(Vector3 lr, Vector3 bt)
    {
        float x1 = lr.x;
        float x2 = bt.x;
        float y1 = lr.y;
        float y2 = bt.y;
        float z1 = lr.z;
        float z2 = bt.z;
        lr.x = Mathf.Min(x1, x2);
        lr.y = Mathf.Min(y1, y2);
        lr.z = Mathf.Min(z1, z2);
        bt.x = Mathf.Max(x1, x2);
        bt.y = Mathf.Max(y1, y2);
        bt.z = Mathf.Max(z1, z2);

    }
    static public Vector3 CalcMin(Vector3 lr, Vector3 bt)
    {
        float x1 = lr.x;
        float x2 = bt.x;
        float y1 = lr.y;
        float y2 = bt.y;
        float z1 = lr.z;
        float z2 = bt.z;
        x1 = Mathf.Min(x1, x2);
        y1 = Mathf.Min(y1, y2);
        z1 = Mathf.Min(z1, z2);
        return new Vector3(x1, y1, z1);
    }
    static public Vector3 CalcMax(Vector3 lr, Vector3 bt)
    {
        float x1 = lr.x;
        float x2 = bt.x;
        float y1 = lr.y;
        float y2 = bt.y;
        float z1 = lr.z;
        float z2 = bt.z;
        x1 = Mathf.Max(x1, x2);
        y1 = Mathf.Max(y1, y2);
        z1 = Mathf.Max(z1, z2);
        return new Vector3(x1, y1, z1);
    }
    static public void CalcXY(SM.ENGateDirection dir, ref int x, ref int y)
    {
        if (dir == SM.ENGateDirection.enEAST)
        {
            x = x + 1;
        }
        else if (dir == SM.ENGateDirection.enWEST)
        {
            x = x - 1;
        }
        else if (dir == SM.ENGateDirection.enSOUTH)
        {
            y = y - 1;
        }
        else if (dir == SM.ENGateDirection.enNORTH)
        {
            y = y + 1;
        }
    }
    public void DrawLine()
    {
        Vector3 start1 = new Vector3(LBPos.x, 1f, LBPos.z);
        Vector3 end1 = new Vector3(LBPos.x, 1f, RTPos.z);
        UnityEngine.Debug.DrawLine(start1, end1, Color.green);
        end1.x = RTPos.x;
        end1.z = LBPos.z;
        UnityEngine.Debug.DrawLine(start1, end1, Color.green);
        start1.x = LBPos.x;
        start1.z = RTPos.z;
        end1.x = RTPos.x;
        end1.z = RTPos.z;
        UnityEngine.Debug.DrawLine(start1, end1, Color.green);
        start1.x = RTPos.x;
        start1.z = LBPos.z;
        UnityEngine.Debug.DrawLine(start1, end1, Color.green);
    }

}

public class ScenePathfinder
{
    static public ScenePathfinder Singleton { get; private set; }
    public ScenePathfinder()
    {
        if (null == Singleton)
        {
            Singleton = this;
        }
        else
        {
            Debug.LogWarning("SceneManger Recreated");
        }
    }
    public void Reset()
    {
        mFindPathDataList.Clear();
    }
    //Variables
    //房间之间的寻路数据
    public List<FindPathLogicData> mFindPathDataList = new List<FindPathLogicData>();
    private Node[,] Map = null;
    public bool MoveDiagonal = true;
    private FindPathLogicData[,] PathDataMap = null;
    int maxSearchRounds = 0;
    public float Tilesize = 1;
    public int HeuristicAggression;   



    //根据世界坐标查找房间寻路数据
    public FindPathLogicData FindRoomNode(Vector3 pos)
    {
        foreach (FindPathLogicData node in mFindPathDataList)
        {
            if ((pos.x >= node.LBPos.x && pos.z >= node.LBPos.z) &&
                (pos.x <= node.RTPos.x && pos.z <= node.RTPos.z))
            {
                return node;
            }
        }
        return null;
    }
    public FindPathLogicData FindRoomNode(Vector3 pos, int guid)
    {
        FindPathLogicData retNode = null;
        foreach (FindPathLogicData node in mFindPathDataList)
        {
            if ((pos.x >= node.LBPos.x && pos.z >= node.LBPos.z) &&
                (pos.x <= node.RTPos.x && pos.z <= node.RTPos.z))
            {
                retNode = node;
                if (guid != -1)
                {
                    if (retNode.GUID != -1)
                    {
                        return retNode;
                    }
                }
                else
                {
                    if (retNode.GUID == -1)
                    {
                        return retNode;
                    }
                }
            }
        }
        return retNode;
    }
    public void DrawAABBLine()
    {
        foreach (FindPathLogicData node in mFindPathDataList)
        {
            node.DrawLine();
        }
    }
    public FindPathLogicData FindRoomNode(int nGuid)
    {
        foreach (FindPathLogicData node in mFindPathDataList)
        {
            if (node.GUID == nGuid)
            {
                return node;
            }
        }
        return null;
    }
    public FindPathLogicData FindRoomNode(int x, int y)
    {
        return PathDataMap[x, y];
    }
    public void BuildRoomFindPathData(FindPathLogicData pathData)
    {
        int x = pathData.m_roomLogicX;
        int y = pathData.m_roomLogicY;
        if (FindPathLogicData.xMin > x)
        {
            FindPathLogicData.xMin = x;
        }
        if (FindPathLogicData.yMin > y)
        {
            FindPathLogicData.yMin = y;
        }
        if (FindPathLogicData.xMax < x)
        {
            FindPathLogicData.xMax = x;
        }
        if (FindPathLogicData.yMax < y)
        {
            FindPathLogicData.yMax = y;
        }
        pathData.Valid = true;
        mFindPathDataList.Add(pathData);
    }
    #region map
    //-------------------------------------------------INSTANIATE MAP-----------------------------------------------//
    public void CreateMap()
    {
        int width = (FindPathLogicData.xMax - FindPathLogicData.xMin) + 1;
        int height = (FindPathLogicData.yMax - FindPathLogicData.yMin) + 1;
        Debug.Log("xMax:" + FindPathLogicData.xMax + " xMin:" + FindPathLogicData.xMin + " yMax:" + FindPathLogicData.yMax + " yMin+" + FindPathLogicData.yMin);
        int nMax = Mathf.Max(width, height);
        width = nMax;
        height = nMax;
        PathDataMap = new FindPathLogicData[width, height];
        int size = width * height;
        SetListsSize(size);
        for (int i = 0; i < mFindPathDataList.Count; i++)
        {
            mFindPathDataList[i].m_roomLogicX -= FindPathLogicData.xMin;
            mFindPathDataList[i].m_roomLogicY -= FindPathLogicData.yMin;
            mFindPathDataList[i].ID = i;
            PathDataMap[mFindPathDataList[i].m_roomLogicX, mFindPathDataList[i].m_roomLogicY] = mFindPathDataList[i];
            Debug.Log("x:" + mFindPathDataList[i].m_roomLogicX + " y:" + mFindPathDataList[i].m_roomLogicY);
        }
        Debug.Log("width:" + width + " height:" + height);
        //Set map up
        Map = new Node[width, height];
        for (int x = 0; x < width; x++ )
        {
            for (int y = 0; y < height; y++)
            {
                bool walkable = false;
                if (y < PathDataMap.GetLength(1) && x < PathDataMap.GetLength(0) && null != PathDataMap[x, y])
                {
                    walkable = PathDataMap[x, y].Valid;
                }
                int ID = (x * width) + y; //ID we give to our Node!
                Map[x, y] = new Node(x, y, 0, ID, x, y, walkable);
            }
        }
    }
    #endregion //End map

    #region astar
    //---------------------------------------FIND PATH: A*------------------------------------------//

    private Node[] openList;
    private Node[] closedList;
    private Node startNode;
    private Node endNode;
    private Node currentNode;
    //Use it with KEY: F-value, VALUE: ID. ID's might be looked up in open and closed list then
    private List<NodeSearch> sortedOpenList = new List<NodeSearch>();
// 
    private void SetListsSize(int size)
    {
        openList = new Node[size];
        closedList = new Node[size];
    }
    public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
    {
        //The list we returns when path is found
        List<Vector3> returnPath = new List<Vector3>();
        bool endPosValid = true;
        //Find start and end nodes, if we cant return null and stop!
        SetStartAndEndNode(startPos, endPos);

        if (startNode != null)
        {
            if (endNode == null)
            {
                endPosValid = false;
                FindEndNode(endPos);
                if (endNode == null)
                {
                    //still no end node - we leave and sends an empty list
                    maxSearchRounds = 0;
                    return new List<Vector3>();
                }
            }

            //Clear lists if they are filled
            Array.Clear(openList, 0, openList.Length);
            Array.Clear(closedList, 0, openList.Length);
            if (sortedOpenList.Count > 0) { sortedOpenList.Clear(); }

            //Insert start node
            if (startNode.ID >= openList.Length)
            {
                return new List<Vector3>();
            }
            openList[startNode.ID] = startNode;
            //sortedOpenList.Add(new NodeSearch(startNode.ID, startNode.F));
            BHInsertNode(new NodeSearch(startNode.ID, startNode.F));

            bool endLoop = false;

            while (!endLoop)
            {
                //If we have no nodes on the open list AND we are not at the end, then we got stucked! return empty list then.
                if (sortedOpenList.Count == 0)
                {
                    Debug.Log("Empty Openlist, closedList");
                    return new List<Vector3>();
                }

                //Get lowest node and insert it into the closed list
                int id = BHGetLowest();
                //sortedOpenList.Sort(sort);
                //int id = sortedOpenList[0].ID;
                currentNode = openList[id];
                closedList[currentNode.ID] = currentNode;
                openList[id] = null;
                //sortedOpenList.RemoveAt(0);

                if (currentNode.ID == endNode.ID)
                {
                    endLoop = true;
                    continue;
                }
                //Now look at neighbours, check for unwalkable tiles, bounderies, open and closed listed nodes.

                if (MoveDiagonal)
                {
                    NeighbourCheck();
                }
                else
                {
                    NonDiagonalNeighborCheck();
                }
            }


            while (currentNode.parent != null)
            {
                returnPath.Add(new Vector3(currentNode.xCoord, currentNode.yCoord, currentNode.zCoord));
                currentNode = currentNode.parent;
            }

            returnPath.Add(startPos);
            returnPath.Reverse();

            if (endPosValid)
            {
                //returnPath.Add(endPos);
            }

            if (returnPath.Count > 2 && endPosValid)
            {
                //Now make sure we do not go backwards or go to long
                if (Vector3.Distance(returnPath[returnPath.Count - 1], returnPath[returnPath.Count - 3]) < Vector3.Distance(returnPath[returnPath.Count - 3], returnPath[returnPath.Count - 2]))
                {
                    returnPath.RemoveAt(returnPath.Count - 2);
                }
                if (Vector3.Distance(returnPath[1], startPos) < Vector3.Distance(returnPath[0], returnPath[1]))
                {
                    returnPath.RemoveAt(0);
                }
            }
            maxSearchRounds = 0;
            return returnPath;

        }
        else
        {
            maxSearchRounds = 0;
        }
        return new List<Vector3>();
    }

    // Find start and end Node
    private void SetStartAndEndNode(Vector3 start, Vector3 end)
    {
        startNode = FindClosestNode(start);
        endNode = FindClosestNode(end);
    }

    private Node FindClosestNode(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int z = Mathf.FloorToInt(pos.z);
        if (x < 0 || z < 0 || x > Map.GetLength(0) || z > Map.GetLength(1))
            return null;

        Node n = Map[x, z];

        if (n.walkable)
        {
            return new Node(x, z, n.yCoord, n.ID, n.xCoord, n.zCoord, n.walkable);
        }
        else
        {
            //If we get a non walkable tile, then look around its neightbours
            for (int i = z - 1; i < z + 2; i++)
            {
                for (int j = x - 1; j < x + 2; j++)
                {
                    //Check they are within bounderies
                    if (i > -1 && i < Map.GetLength(1) && j > -1 && j < Map.GetLength(0))
                    {
                        if (Map[j, i].walkable)
                        {
                            return new Node(j, i, Map[j, i].yCoord, Map[j, i].ID, Map[j, i].xCoord, Map[j, i].zCoord, Map[j, i].walkable);
                        }
                    }
                }
            }
            return null;
        }
    }

    private void FindEndNode(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int z = Mathf.FloorToInt(pos.z);
        if (x >= Map.GetLength(0) || z >= Map.GetLength(1))
        {
            return;
        }
        Node closestNode = Map[x, z];
        List<Node> walkableNodes = new List<Node>();

        int turns = 1;

        while (walkableNodes.Count < 1 && maxSearchRounds < (int)10 / Tilesize)
        {
            walkableNodes = EndNodeNeighbourCheck(x, z, turns);
            turns++;
            maxSearchRounds++;
        }

        if (walkableNodes.Count > 0) //If we found some walkable tiles we will then return the nearest
        {
            int lowestDist = 99999999;
            Node n = null;

            foreach (Node node in walkableNodes)
            {
                int i = GetHeuristics(closestNode, node);
                if (i < lowestDist)
                {
                    lowestDist = i;
                    n = node;
                }
            }
            endNode = new Node(n.x, n.y, n.yCoord, n.ID, n.xCoord, n.zCoord, n.walkable);
        }
    }

    private List<Node> EndNodeNeighbourCheck(int x, int z, int r)
    {
        List<Node> nodes = new List<Node>();

        for (int i = z - r; i < z + r + 1; i++)
        {
            for (int j = x - r; j < x + r + 1; j++)
            {
                //Check that we are within bounderis, and goes in ring around our end pos
                if (i > -1 && j > -1 && i < Map.GetLength(0) && j < Map.GetLength(1) && ((i < z - r + 1 || i > z + r - 1) || (j < x - r + 1 || j > x + r - 1)))
                {
                    //if it is walkable put it on the right list
                    if (Map[j, i].walkable)
                    {
                        nodes.Add(Map[j, i]);
                    }
                }
            }
        }

        return nodes;
    }

    private void NeighbourCheck()
    {
        int x = currentNode.x;
        int y = currentNode.y;

        for (int i = y - 1; i < y + 2; i++)
        {
            for (int j = x - 1; j < x + 2; j++)
            {
                //Check it is within the bounderies
                if (i > -1 && i < Map.GetLength(1) && j > -1 && j < Map.GetLength(0))
                {
                    //Dont check for the current node.
                    if (i != y || j != x)
                    {
                        //Check the node is walkable
                        if (Map[j, i].walkable)
                        {
                            //We do not recheck anything on the closed list
                            if (!OnClosedList(Map[j, i].ID))
                            {
                                //Check if we can move up or jump down!
                                //if ((Map[j, i].yCoord - currentNode.yCoord < ClimbLimit && Map[j, i].yCoord - currentNode.yCoord >= 0) || (currentNode.yCoord - Map[j, i].yCoord < MaxFalldownHeight && currentNode.yCoord >= Map[j, i].yCoord))
                                {
                                    //If it is not on the open list then add it to
                                    if (!OnOpenList(Map[j, i].ID))
                                    {
                                        Node addNode = new Node(Map[j, i].x, Map[j, i].y, Map[j, i].yCoord, Map[j, i].ID, Map[j, i].xCoord, Map[j, i].zCoord, Map[j, i].walkable, currentNode);
                                        addNode.H = GetHeuristics(Map[j, i].x, Map[j, i].y);
                                        addNode.G = GetMovementCost(x, y, j, i) + currentNode.G;
                                        addNode.F = addNode.H + addNode.G;
                                        //Insert on open list
                                        openList[addNode.ID] = addNode;
                                        //Insert on sorted list
                                        BHInsertNode(new NodeSearch(addNode.ID, addNode.F));
                                        //sortedOpenList.Add(new NodeSearch(addNode.ID, addNode.F));
                                    }
                                    else
                                    {
                                        ///If it is on openlist then check if the new paths movement cost is lower
                                        Node n = GetNodeFromOpenList(Map[j, i].ID);
                                        if (currentNode.G + GetMovementCost(x, y, j, i) < openList[Map[j, i].ID].G)
                                        {
                                            n.parent = currentNode;
                                            n.G = currentNode.G + GetMovementCost(x, y, j, i);
                                            n.F = n.G + n.H;
                                            BHSortNode(n.ID, n.F);
                                            //ChangeFValue(n.ID, n.F);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void NonDiagonalNeighborCheck()
    {
        int x = currentNode.x;
        int y = currentNode.y;

        for (int i = y - 1; i < y + 2; i++)
        {
            for (int j = x - 1; j < x + 2; j++)
            {
                //Check it is within the bounderies
                if (i > -1 && i < Map.GetLength(1) && j > -1 && j < Map.GetLength(0))
                {
                    //Dont check for the current node.
                    if (i != y || j != x)
                    {
                        //Check that we are not moving diagonal
                        if (GetMovementCost(x, y, j, i) < 14)
                        {
                            //Check the node is walkable
                            if (Map[j, i].walkable)
                            {
                                //We do not recheck anything on the closed list
                                if (!OnClosedList(Map[j, i].ID))
                                {
                                    //Check if we can move up or jump down!
                                    //if ((Map[j, i].yCoord - currentNode.yCoord < ClimbLimit && Map[j, i].yCoord - currentNode.yCoord >= 0) || (currentNode.yCoord - Map[j, i].yCoord < MaxFalldownHeight && currentNode.yCoord >= Map[j, i].yCoord))
                                    {
                                        //If it is not on the open list then add it to
                                        if (!OnOpenList(Map[j, i].ID))
                                        {
                                            Node addNode = new Node(Map[j, i].x, Map[j, i].y, Map[j, i].yCoord, Map[j, i].ID, Map[j, i].xCoord, Map[j, i].zCoord, Map[j, i].walkable, currentNode);
                                            addNode.H = GetHeuristics(Map[j, i].x, Map[j, i].y);
                                            addNode.G = GetMovementCost(x, y, j, i) + currentNode.G;
                                            addNode.F = addNode.H + addNode.G;
                                            //Insert on open list
                                            openList[addNode.ID] = addNode;
                                            //Insert on sorted list
                                            BHInsertNode(new NodeSearch(addNode.ID, addNode.F));
                                            //sortedOpenList.Add(new NodeSearch(addNode.ID, addNode.F));
                                        }
                                        else
                                        {
                                            ///If it is on openlist then check if the new paths movement cost is lower
                                            Node n = GetNodeFromOpenList(Map[j, i].ID);
                                            if (currentNode.G + GetMovementCost(x, y, j, i) < openList[Map[j, i].ID].G)
                                            {
                                                n.parent = currentNode;
                                                n.G = currentNode.G + GetMovementCost(x, y, j, i);
                                                n.F = n.G + n.H;
                                                BHSortNode(n.ID, n.F);
                                                //ChangeFValue(n.ID, n.F);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void ChangeFValue(int id, int F)
    {
        foreach (NodeSearch ns in sortedOpenList)
        {
            if (ns.ID == id)
            {
                ns.F = F;
            }
        }
    }

    //Check if a Node is already on the openList
    private bool OnOpenList(int id)
    {
        return (openList[id] != null) ? true : false;
    }

    //Check if a Node is already on the closedList
    private bool OnClosedList(int id)
    {
        return (closedList[id] != null) ? true : false;
    }

    private int GetHeuristics(int x, int y)
    {
        //Make sure heuristic aggression is not less then 0!
        int HA = (HeuristicAggression < 0) ? 0 : HeuristicAggression;
        return (int)(Mathf.Abs(x - endNode.x) * (10F + (10F * HA))) + (int)(Mathf.Abs(y - endNode.y) * (10F + (10F * HA)));
    }

    private int GetHeuristics(Node a, Node b)
    {
        //Make sure heuristic aggression is not less then 0!
        int HA = (HeuristicAggression < 0) ? 0 : HeuristicAggression;
        return (int)(Mathf.Abs(a.x - b.x) * (10F + (10F * HA))) + (int)(Mathf.Abs(a.y - b.y) * (10F + (10F * HA)));
    }

    private int GetMovementCost(int x, int y, int j, int i)
    {
        //Moving straight or diagonal?
        return (x != j && y != i) ? 14 : 10;
    }

    private Node GetNodeFromOpenList(int id)
    {
        return (openList[id] != null) ? openList[id] : null;
    }

    #region Binary_Heap (min)

    private void BHInsertNode(NodeSearch ns)
    {
        //We use index 0 as the root!
        if (sortedOpenList.Count == 0)
        {
            sortedOpenList.Add(ns);
            openList[ns.ID].sortedIndex = 0;
            return;
        }

        sortedOpenList.Add(ns);
        bool canMoveFurther = true;
        int index = sortedOpenList.Count - 1;
        openList[ns.ID].sortedIndex = index;

        while (canMoveFurther)
        {
            int parent = Mathf.FloorToInt((index - 1) / 2);

            if (index == 0) //We are the root
            {
                canMoveFurther = false;
                openList[sortedOpenList[index].ID].sortedIndex = 0;
            }
            else
            {
                if (sortedOpenList[index].F < sortedOpenList[parent].F)
                {
                    NodeSearch s = sortedOpenList[parent];
                    sortedOpenList[parent] = sortedOpenList[index];
                    sortedOpenList[index] = s;

                    //Save sortedlist index's for faster look up
                    openList[sortedOpenList[index].ID].sortedIndex = index;
                    openList[sortedOpenList[parent].ID].sortedIndex = parent;

                    //Reset index to parent ID
                    index = parent;
                }
                else
                {
                    canMoveFurther = false;
                }
            }
        }
    }

    private void BHSortNode(int id, int F)
    {
        bool canMoveFurther = true;
        int index = openList[id].sortedIndex;
        sortedOpenList[index].F = F;

        while (canMoveFurther)
        {
            int parent = Mathf.FloorToInt((index - 1) / 2);

            if (index == 0) //We are the root
            {
                canMoveFurther = false;
                openList[sortedOpenList[index].ID].sortedIndex = 0;
            }
            else
            {
                if (sortedOpenList[index].F < sortedOpenList[parent].F)
                {
                    NodeSearch s = sortedOpenList[parent];
                    sortedOpenList[parent] = sortedOpenList[index];
                    sortedOpenList[index] = s;

                    //Save sortedlist index's for faster look up
                    openList[sortedOpenList[index].ID].sortedIndex = index;
                    openList[sortedOpenList[parent].ID].sortedIndex = parent;

                    //Reset index to parent ID
                    index = parent;
                }
                else
                {
                    canMoveFurther = false;
                }
            }
        }
    }

    private int BHGetLowest()
    {

        if (sortedOpenList.Count == 1) //Remember 0 is our root
        {
            int ID = sortedOpenList[0].ID;
            sortedOpenList.RemoveAt(0);
            return ID;
        }
        else if (sortedOpenList.Count > 1)
        {
            //save lowest not, take our leaf as root, and remove it! Then switch through children to find right place.
            int ID = sortedOpenList[0].ID;
            sortedOpenList[0] = sortedOpenList[sortedOpenList.Count - 1];
            sortedOpenList.RemoveAt(sortedOpenList.Count - 1);
            openList[sortedOpenList[0].ID].sortedIndex = 0;

            int index = 0;
            bool canMoveFurther = true;
            //Sort the list before returning the ID
            while (canMoveFurther)
            {
                int child1 = (index * 2) + 1;
                int child2 = (index * 2) + 2;
                int switchIndex = -1;

                if (child1 < sortedOpenList.Count)
                {
                    switchIndex = child1;
                }
                else
                {
                    break;
                }
                if (child2 < sortedOpenList.Count)
                {
                    if (sortedOpenList[child2].F < sortedOpenList[child1].F)
                    {
                        switchIndex = child2;
                    }
                }
                if (sortedOpenList[index].F > sortedOpenList[switchIndex].F)
                {
                    NodeSearch ns = sortedOpenList[index];
                    sortedOpenList[index] = sortedOpenList[switchIndex];
                    sortedOpenList[switchIndex] = ns;

                    //Save sortedlist index's for faster look up
                    openList[sortedOpenList[index].ID].sortedIndex = index;
                    openList[sortedOpenList[switchIndex].ID].sortedIndex = switchIndex;

                    //Switch around idnex
                    index = switchIndex;
                }
                else
                {
                    break;
                }
            }
            return ID;

        }
        else
        {
            return -1;
        }
    }

    #endregion
 
    //     #endregion //End astar region!
    #endregion
}