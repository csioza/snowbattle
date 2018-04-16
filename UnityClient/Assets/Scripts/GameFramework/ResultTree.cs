using UnityEngine;
using NGE.Network;
using System.Collections.Generic;
public class ResultTree 
{
    public List<int>m_nodeParentIndex = new List<int>();
    public List<IResult> m_results = new List<IResult>();
    public List<int> m_pushStack = new List<int>();
    BattleContext m_context;
    public ResultTree(BattleContext context)
    {
        m_context = context;
    }
    public void ReleaseAllResult()
    {
        for (int i = 0; i < m_results.Count;i++ )
        {
            IResult result = m_results[i];
            m_context.ReleaseResult(result);
        }

        m_results.Clear();
    }
    public void Serialize(PacketWriter stream)
    {
        stream.WriteCompressInt(m_results.Count);
        foreach (int index in m_nodeParentIndex)
        {
            stream.WriteCompressInt(m_nodeParentIndex[index]);
        }
        foreach (IResult result in m_results)
        {
            stream.WriteCompressInt((int)result.ClassID);
            result.Serialize(stream);
        }
    }
    public void Deserialize(PacketReader stream)
    {
        ReleaseAllResult();
        m_nodeParentIndex.Clear();
        m_results.Clear();

		int count = stream.ReadCompressInt();
        for (int i = 0; i < count; i++)
        {
			int idx = stream.ReadCompressInt();
            m_nodeParentIndex.Add(idx);
        }
        for (int i = 0; i < count; i++)
        {
			int classID = stream.ReadCompressInt();
            IResult result = m_context.CreateResult(classID);
            result.Deserialize(stream);
            m_results.Add(result);
        }
    }
    public void PushResult(IResult result)
    {
        int index = m_results.Count;
        m_results.Add(result);
        if (0 == m_pushStack.Count)
        {
            m_nodeParentIndex.Add(0);
        }
        else
        {
            m_nodeParentIndex.Add(m_pushStack[m_pushStack.Count - 1]);
        }
        m_pushStack.Add(index);
    }
    public void PopResult()
    {
        m_pushStack.RemoveAt(m_pushStack.Count - 1);
    }

    public void ExecAllResult(IResultControl control)
    {
        foreach (IResult result in m_results) 
        {
            result.Exec(control);
        }
    }

    public void ExecAllServerResult(IResultControl control)
    {
        foreach (IResult result in m_results)
        {
            result.ResultServerExec(control);
        }
    }
}