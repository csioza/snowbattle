using System;
using System.Collections.Generic;
using NGE.Network;

public class SResultRemoveBuff : ResultRemoveBuff
{
    public int m_hp = 10;

    public SResultRemoveBuff()
    {

    }
    public override void Serialize(PacketWriter stream)
    {
        base.Serialize(stream);
        stream.Write(m_hp);
    }
    public override void Exec(IResultControl control)
    {
        base.Exec(control);
    }
}