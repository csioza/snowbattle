using System;
using System.Collections.Generic;

public class QualityInfo : IDataBase
{
    public int m_quality { get; protected set; }
    public float m_mainAttrMutiply { get; protected set; }

}

public class QualityRelativeTable
{
    public Dictionary<int, QualityInfo> m_map { get; protected set; }
    public QualityInfo LookUp(int quality)
    {
        QualityInfo info = null;
        if (m_map.TryGetValue(quality, out info))
        {
            return info;
        }
        return null;
    }
    public void Load(byte[] bytes)
    {
        m_map = new Dictionary<int, QualityInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount = helper.ReadInt();
        for (int index = 0; index < sceneCount; ++index)
        {
            QualityInfo info = new QualityInfo();
            info.Load(helper);
            m_map.Add(info.m_quality, info);
        }
    }
}