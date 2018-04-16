using System;
using System.Collections.Generic;

public class EventItemInfo : IDataBase
{
    public int itemId { get; protected set; }
    public string name { get; protected set; }
    public int iconId { get; protected set; }
    public int modelId { get; protected set; }
    public float scale { get; protected set; }
    public int trapID { get; protected set; }
    public int dropType { get; protected set; }
    public string describ { get; protected set; }
}

public class EventItemTable
{
    public Dictionary<int, EventItemInfo> m_map { get; protected set; }
    public EventItemInfo LookUp(int id)
    {
        EventItemInfo info = null;
        if (m_map.TryGetValue(id, out info))
        {
            return info;
        }
        return null;
    }
    public void Load(byte[] bytes)
    {
        m_map = new Dictionary<int, EventItemInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount = helper.ReadInt();
        for (int index = 0; index < sceneCount; ++index)
        {
            EventItemInfo info = new EventItemInfo();
            info.Load(helper);
            m_map.Add(info.itemId, info);
        }
    }
}