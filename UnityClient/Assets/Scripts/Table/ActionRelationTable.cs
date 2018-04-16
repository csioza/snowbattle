//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Table
//	created:	2013-5-9
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using UnityEngine;


public class ActionRelationTable
{
	protected byte[] m_relationArray;
	public ActorAction.ENRelation Lookup(ActorAction.ENType newType, ActorAction.ENType oldType)
	{
		return (ActorAction.ENRelation)m_relationArray[(int)oldType * (int)ActorAction.ENType.Count + (int)newType];
	}
	public void Load(byte[] bytes)
	{
		BinaryHelper helper = new BinaryHelper(bytes);
		m_relationArray = new byte[helper.ReadInt()];
		helper.InnerStream.Read(m_relationArray, 0, m_relationArray.Length);
	}
};

public class TrapActionRelationTable
{
    protected byte[] m_relationArray;
    public int Lookup(int newType, int oldType)
    {
        return (int)m_relationArray[oldType * (int)TrapAction.ENType.Count + newType];
    }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        m_relationArray = new byte[helper.ReadInt()];
        helper.InnerStream.Read(m_relationArray, 0, m_relationArray.Length);
    }
}