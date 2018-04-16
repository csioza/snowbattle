using UnityEngine;
using System.Collections;

public class IBuffEffect 
{
	//!��Ҫ���˴���
	bool m_isNeedResultPass;
	//!�Ƿ�����
	bool m_isTickNeed;
	//!��Щ�߼����ͳ���ʱ,��Ҫ���˴���
	int	m_affectLogicTypeMask;
    //BuffEffect������
    ENBuff m_classID;
    //Ŀ��id(�Լ�)
    int m_targetID;
    //��Դid�����Լ���buff��actor��
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
    //result����ʱ֪ͨ�Լ�
    public virtual void OnProduceResult(IResult result, IResultControl control) { }
    //result����ʱ֪ͨĿ��
    public virtual void OnGetResult(IResult result, IResultControl control) { }
    //buff����ʱ����
    public virtual void OnGetBuffEffect() { }
    public virtual void Tick(IResultControl control, float dt) { }
    public virtual void Exec(IResultControl control) { }
    public virtual void OnRemoved(IResultControl control) { }
    //�Ƴ��������ڵ�buff
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