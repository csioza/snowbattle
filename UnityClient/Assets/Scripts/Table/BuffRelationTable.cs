using System;
using System.IO;
using UnityEngine;

public class BuffRelationTable
{
    protected byte[] m_relationArray;
    public Buff.ENRelation Lookup(int newType, int oldType)
    {
        return (Buff.ENRelation)m_relationArray[oldType * (int)Buff.ENBuffType.Count + newType];
    }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        m_relationArray = new byte[helper.ReadInt()];
        helper.InnerStream.Read(m_relationArray, 0, m_relationArray.Length);
    }
}
