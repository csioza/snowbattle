using System;
using System.Collections.Generic;

public class PlayerRandomNameInfo : IDataBase
{
    public int m_id { get; protected set; }
    public string m_firstName { get; protected set; }
    public string m_secondName { get; protected set; }
}

public class PlayerRandomNameTable
{
    public Dictionary<int, PlayerRandomNameInfo> m_map { get; protected set; }

    public PlayerRandomNameInfo LookUp(int id)
    {
        PlayerRandomNameInfo info = null;
        if (m_map.TryGetValue(id, out info))
        {
            return info;
        }

        return null;
    }
    public void Load(byte[] bytes)
    {
        m_map = new Dictionary<int, PlayerRandomNameInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int count = helper.ReadInt();
       
        for (int index = 0; index < count; ++index)
        {
            PlayerRandomNameInfo info = new PlayerRandomNameInfo();
            
            info.Load(helper);

            m_map.Add(info.m_id, info);
        }
    }

    public int GetCount()
    {
        return m_map.Count;
    }
}