using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NGE.Network;

public class IResult
{
    public IResult(int type) 
    {
		ClassID = type;
        IsEnable = true;
    }
    public virtual void Reset()
    {
        
    }
    public virtual void Deserialize(PacketReader stream)
	{
		stream.ReadInt32();//空读一下，对应stream->BeginChunk();
        int src = stream.ReadInt32();
		int target = stream.ReadInt32();
        int multiTargetCount = stream.ReadCompressInt();
        m_multiTarget.Clear();
        for (int i = 0; i < multiTargetCount; i++)
        {
            int targetID = stream.ReadCompressInt();
            m_multiTarget.Add(targetID);
        }
		int logicType = stream.ReadCompressInt();
	    LogicType=logicType;
		SourceID = src;
		TargetID = target;
    }
    public virtual void Serialize(PacketWriter stream)
    {
        stream.Write(0);
        stream.Write(SourceID);
        stream.Write(TargetID);
        stream.WriteCompressInt(m_multiTarget.Count);
        for (int i = 0; i < m_multiTarget.Count; i++)
        {
            stream.WriteCompressInt(m_multiTarget.IndexOf(i));
        }
        stream.WriteCompressInt(LogicType);
    }
 	public int ClassID { get; set; }
    public int LogicType { get; set; }
    public int SourceID { get; set; }
    public int TargetID { get; set; }
    public int SkillResultID { get; set; }
    public int SkillID { get; set; }
    public bool IsEnable { get; set; }

    public bool m_isHit = true;
    public bool m_isCrit = false;

    public List<int> m_multiTarget=new List<int>();
    public virtual void Exec(IResultControl control)
    {

    }
    //自身结算
    public virtual void ResultExpr(float[] param)
    {

    }

    //根据服务器发回的数据进行结算
    public virtual void ResultServerExec(IResultControl control)
    {

    }
}