using System;
using System.Collections.Generic;

public class ComboSwordSoulInfo : IDataBase
{
    public int m_comboValue { get; protected set; }
    public float m_normalResult { get; protected set; }
    public float m_breakResult { get; protected set; }
}
public class ComboSwordSoulTable
{
    public Dictionary<int, ComboSwordSoulInfo> m_map { get; protected set; }
    public ComboSwordSoulInfo LookUp(int id)
    {
        ComboSwordSoulInfo info = null;
        if (m_map.TryGetValue(id, out info))
        {
            return info;
        }

        return null;
    }
    public void Load(byte[] bytes)
    {
        m_map = new Dictionary<int, ComboSwordSoulInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount = helper.ReadInt();
        for (int index = 0; index < sceneCount; ++index)
        {
            ComboSwordSoulInfo info = new ComboSwordSoulInfo();
            info.Load(helper);
            m_map.Add(info.m_comboValue, info);
        }
    }
}