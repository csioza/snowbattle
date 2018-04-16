using UnityEngine;
using System.Collections;
using NGE.Network;

public class ResultAddBuff : IResult
{
    public float[] BuffIDList = new float[]{};
    //buff修正时间
    public float[] BuffModifyTimeList = null;
    //是否不允许执行
    public bool[] BuffIDIsRun = null; 
    public ResultAddBuff()
        : base((int)ENResult.AddBuff)
    {

    }
    public static IResult CreateNew()
    {
        return new ResultAddBuff();
    }

    public override void Deserialize(PacketReader stream)
    {
        base.Deserialize(stream);

        int size    = stream.ReadInt32();

        float[] param  = new float[size] ;
        float[] modityTime = new float[size];
        for (int i = 0; i < size;i++ )
        {
            param[i] = stream.ReadInt32();
            modityTime[i] = stream.ReadFloat();
        }
        BuffIDList = param;
        BuffModifyTimeList = modityTime;
    }

    public override void ResultExpr(float[] param)
    {
        base.ResultExpr(param);
        if (param != null)
        {
            BuffIDList = param;
            BuffModifyTimeList = new float[BuffIDList.Length];
            BuffIDIsRun = new bool[BuffIDList.Length];
        }
    }
    public override void Exec(IResultControl control)
    {
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            return;
        }
        for (int i = 0; i < BuffIDList.Length; ++i)
        {
            if (BuffIDIsRun[i])
            {
                continue;
            }
            BuffInfo info = GameTable.BuffTableAsset.Lookup((int)BuffIDList[i]);
            if (null == info)
            {
                continue;
            }
            if (info.BuffType == (int)BuffInfo.ENBuffType.Harmful)
            {//有害buff
                float r = UnityEngine.Random.Range(0.0f, 1.0f);
                float curPercent = info.BuffPercent - actor.Props.GetProperty_Float(ENProperty.FResist);
                //Debug.LogWarning("add buff id==" + m_buffID.ToString() + "  buff value=" + curPercent.ToString() + "target id = " + actor.ID.ToString());
                if (curPercent <= 0.0f)
                {//被抗性抵消，buff没有添加
                    continue;
                }
                if (r > curPercent)
                {//buff没有添加
                    continue;
                }
                if (actor.Type == ActorType.enMain)
                {
                    MainPlayer player = actor as MainPlayer;
                    player.Damgaed(ActorManager.Singleton.Lookup(SourceID));
                }
            }
            actor.MyBuffControl.RemoveBuff(info.ID, control);
            actor.MyBuffControl.AddBuff(info.ID, SourceID, BuffModifyTimeList[i],false);
        }
    }

      //根据服务器发回的数据进行结算
    public override void ResultServerExec(IResultControl control)
    {

        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            return;
        }
        for (int i = 0; i < BuffIDList.Length; ++i)
        {
           
            BuffInfo info = GameTable.BuffTableAsset.Lookup((int)BuffIDList[i]);
            if (null == info)
            {
                continue;
            }
           
            actor.MyBuffControl.RemoveBuff(info.ID, control);
            actor.MyBuffControl.AddBuff(info.ID, SourceID, BuffModifyTimeList[i],true);
        }

    }
}
