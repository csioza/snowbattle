using UnityEngine;
using System.Collections;

public class IBuffEffect 
{
	//!需要过滤处理
	bool m_isNeedResultPass;
	//!是否心跳
	bool m_isTickNeed;
	//!这些逻辑类型出现时,需要过滤处理
	int	m_affectLogicTypeMask;
    //BuffEffect的类型
    ENBuff m_classID;
    //目标id(自己)
    int m_targetID;
    //来源id（给自己加buff的actor）
    int m_sourceID;
    //buffID BuffTable
    int m_buffID;
    //buffEffectID
    int m_buffEffectID;

    public IBuffEffect(ENBuff type)
    {
        ClassID = type;
    }
    public bool IsNeedResultPass { get { return m_isNeedResultPass; } set { m_isNeedResultPass = value; } }
    public bool IsNeedTick { get { return m_isTickNeed; } set { m_isTickNeed = value; } }
    public int AffectLogicTypeMask { get { return m_affectLogicTypeMask; } set { m_affectLogicTypeMask = value; } }

    public int TargetID { get { return m_targetID; } set { m_targetID = value; } }
    public int SourceID { get { return m_sourceID; } set { m_sourceID = value; } }
    public int BuffID { get { return m_buffID; } set { m_buffID = value; } }
    public int BuffEffectID { get { return m_buffEffectID; } set { m_buffEffectID = value; } }
    public ENBuff ClassID { get { return m_classID; } protected set { m_classID = value; } }
    public virtual void Reset()
    {

    }
    //result创建时通知自己
    public virtual void OnProduceResult(IResult result, IResultControl control) { }
    //result创建时通知目标
    public virtual void OnGetResult(IResult result, IResultControl control) { }
    //buff创建时调用
    public virtual void OnGetBuffEffect() { }
    public virtual void Tick(IResultControl control, float dt) { }
    public virtual void Exec(IResultControl control) { }
    public virtual void OnRemoved(IResultControl control) { }
    //移除自身所在的buff
    public void RemoveSelf()
    {
        IResult r = BattleFactory.Singleton.CreateResult(ENResult.RemoveBuff, TargetID, TargetID, 0, 0, new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffID, BuffID });
        if (r != null)
        {
            r.ResultExpr(new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffID, BuffID });
            BattleFactory.Singleton.DispatchResult(r);
        }
    }
}