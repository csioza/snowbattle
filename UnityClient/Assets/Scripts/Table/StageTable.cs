using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomNodeTree
{
    public int mID;
    public bool mIsLeaf = false;
    public List<RoomNodeTree> mNextNode = new List<RoomNodeTree>();
    public void BuildTree(ref string strWhole)
    {
        if ("" == strWhole)
        {
            return;
        }
        int nCurNodeStartPos = strWhole.IndexOf(mID.ToString());
        int nCurNodeEndPos = strWhole.IndexOf(",");
        string strCurNode;
        if (nCurNodeEndPos == -1)
        {
            strCurNode = strWhole.Substring(nCurNodeStartPos, strWhole.Length);
            strWhole = "";
        }
        else
        {
            strCurNode = strWhole.Substring(nCurNodeStartPos, nCurNodeEndPos);
            strWhole = strWhole.Substring(nCurNodeEndPos + 1);
        }
        int nCurNodeIdx = strCurNode.IndexOf("=");
        string strLeaf = strCurNode.Substring(nCurNodeIdx + 1);
        strLeaf = strLeaf.Substring(1);
        strLeaf = strLeaf.Substring(0, strLeaf.Length - 1);
        string[] param1 = strLeaf.Split(new char[1] { '|' });
        foreach (string c in param1)
        {
            if (!string.IsNullOrEmpty(c))
            {
                RoomNodeTree node = new RoomNodeTree();
                int nFlagIdx = c.IndexOf("A");
                if (nFlagIdx == -1)
                {
                    nFlagIdx = c.IndexOf("D");
                }
                string strID = c.Substring(0, nFlagIdx);
                node.mID = int.Parse(strID);
                string strFlag = c.Substring(nFlagIdx);
                if (strFlag == "D")
                {
                    node.mIsLeaf = true;
                }
                else
                {
                    node.mIsLeaf = false;
                    node.BuildTree(ref strWhole);
                }
                mNextNode.Add(node);
            }
        }
    }
    public void ComponentNode(ref string strWhole)
    {
        strWhole += mID.ToString();
        strWhole += "=[";
        for (int i = 0; i < mNextNode.Count; i++)
        {
            strWhole += mNextNode[i].mID.ToString();
            if (mNextNode[i].mIsLeaf)
            {
                strWhole += "D";
            }
            else
            {
                strWhole += "A";
            }
            if (i != mNextNode.Count - 1)
            {
                strWhole += "|";
            }
        }
        strWhole += "]";
        strWhole += ",";
        for (int i = 0; i < mNextNode.Count; i++)
        {
            if (!mNextNode[i].mIsLeaf)
            {
                mNextNode[i].ComponentNode(ref strWhole);
            }
        }
    }
}
public class StageLineInfo
{
    public RoomNodeTree mRoomNodeTree = new RoomNodeTree();
    public List<int> BossRoomList = new List<int>();
}
public class StageInfo
{
    public int ID { get; private set; }
    public int Level { get; private set; }
    public int nLayerId { get; private set; }
    public int nLineNum { get; private set; }
    public List<StageLineInfo> LineList { get;  set; }
    public void Load(BinaryHelper helper)
    {
        ID          = helper.ReadInt();
        Level       = helper.ReadInt();
        nLayerId    = helper.ReadInt();
        LineList = new List<StageLineInfo>();
        nLineNum = helper.ReadInt();
        for (int j = 0; j < nLineNum; j++)
        {
            string temp = helper.ReadString();//String
            StageLineInfo lineInfo = new StageLineInfo();
            string strNode = temp.Substring(0, 1);
            if (!string.IsNullOrEmpty(strNode))
            {
                lineInfo.mRoomNodeTree.mID = int.Parse(strNode);
                lineInfo.mRoomNodeTree.BuildTree(ref temp);
                if (!string.IsNullOrEmpty(temp))
                {
                    Debug.Log("node is error:" + temp);
                }
            }
            //!boss room list
            {
                string strBoss = helper.ReadString();//String
                if (!string.IsNullOrEmpty(strBoss))
                {
                    string[] param = strBoss.Split(new char[1] { ',' });
                    foreach (string i in param)
                    {
                        if (!string.IsNullOrEmpty(i))
                        {
                            int nID = int.Parse(i);
                            if (nID > 0)
                            {
                                lineInfo.BossRoomList.Add(nID);
                            }
                        }
                    }
                }
            }
            LineList.Add(lineInfo);
        }
    }
    #if UNITY_EDITOR
	public void Save(BinaryHelper helper)
	{
		helper.Write(ID);
        helper.Write(Level);
        helper.Write(nLayerId);
        helper.Write(nLineNum);
        for (int i = 0 ; i < LineList.Count; ++i)
        {
            string temp = "";
            LineList[i].mRoomNodeTree.ComponentNode(ref temp);
            temp = temp.Substring(0, temp.Length - 1);
            helper.Write(temp);
            //!boss room list
            string strBoss = "";
            for (int n = 0; n < LineList[i].BossRoomList.Count; n++)
            {
                strBoss += LineList[i].BossRoomList[n].ToString();
                if (i + 1 != LineList[i].BossRoomList.Count)
                    strBoss += ",";
            }
            helper.Write(strBoss);
        }
	}
#endif
}
public class StageTable
{
    public StageTable()
    {

    }
    public SortedList<int, StageInfo> StageInfoList { get; protected set; }
    public StageInfo Lookup(int id)
    {
        StageInfo info;
        StageInfoList.TryGetValue(id, out info);
        return info;
    }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        StageInfoList = new SortedList<int, StageInfo>(length);
        for (int index = 0; index < length; index++ )
        {
            StageInfo info = new StageInfo();
            info.Load(helper);
            StageInfoList.Add(info.ID, info);
        }

    }
}
