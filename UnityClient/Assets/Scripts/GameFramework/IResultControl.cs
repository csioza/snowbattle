using UnityEngine;
using System.Collections;

public class IResultControl{
    BattleContext m_context;
    ResultTree m_tree;

	public void SetContext(BattleContext c){
		m_context = c;
        if (m_tree == null)
        {
            m_tree = new ResultTree(m_context);
        }
	}
	public BattleContext GetContext(){
		return m_context;
	}
    public IResultControl()
    {
    }
    public virtual void Release()
    {
    }
	public ResultTree GetResultTree(){
		return m_tree;
	}

    public virtual void DispatchResult(IResult result)
    {
        Actor obj = ActorManager.Singleton.Lookup(result.SourceID);
        if (obj != null)
        {
            obj.MyBuffControl.OnProduceResult(result, this);
        }
        if (result.IsEnable)
        {
            obj = ActorManager.Singleton.Lookup(result.TargetID);
            if (obj != null)
            {
                obj.MyBuffControl.OnGetResult(result, this);
            }
        }
        if (result.IsEnable)
        {
            m_tree.PushResult(result);
            result.Exec(this);
            m_tree.PopResult();
        }
    }
    public virtual void EndProcessBatch()
    {

    }
}
