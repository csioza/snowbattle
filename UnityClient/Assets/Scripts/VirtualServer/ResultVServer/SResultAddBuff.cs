using System;
using System.Collections.Generic;
using NGE.Network;

public class SResultAddBuff : ResultAddBuff
{
    private int m_buffID = 4;

    public SResultAddBuff()
    {

    }
    public override void Serialize(PacketWriter stream)
    {
        base.Serialize(stream);
        stream.Write(m_buffID);
    }
    public override void ResultExpr(float[] param)
    {
        base.ResultExpr(param);
        m_buffID = (int)param[0];
    }
    public override void Exec(IResultControl control)
    {
        BuffTable buffTable = GameTable.BuffTableAsset;
        BuffInfo buffData = buffTable.Lookup(m_buffID);
        if (null == buffData)
            return;
        //Buff buff = new Buff(m_buffID);

    }
}
