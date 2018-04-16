using System;
using System.Collections.Generic;

public class QTESequenceInfo : IDataBase
{
    public int ID { get; protected set; }
    public List<int> SequenceList { get; protected set; }
}

public class QTESequenceTable
{
    public Dictionary<int, QTESequenceInfo> m_list { get; protected set; }
    public QTESequenceInfo LookUp(int id)
    {
        QTESequenceInfo info = null;
        m_list.TryGetValue(id, out info);
        return info;
    }

    public void Load(byte[] bytes)
    {
        m_list = new Dictionary<int, QTESequenceInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        for (int index = 0; index < length; ++index)
        {
            QTESequenceInfo info = new QTESequenceInfo();
            info.Load(helper);
            m_list.Add(info.ID, info);
        }
    }
}