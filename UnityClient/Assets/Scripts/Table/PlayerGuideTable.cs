using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerGuideInfo : IDataBase
{
    public int id { get; protected set; }
    public string icon { get; protected set; }
    public int groupID { get; protected set; }

}

public class PlayerGuideTable
{
    public List<PlayerGuideInfo> PlayerGuideList { get; protected set; }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        PlayerGuideList = new List<PlayerGuideInfo>(length);
        for (int index = 0; index < length; ++index)
        {
            PlayerGuideInfo info = new PlayerGuideInfo();
            info.Load(helper);
            PlayerGuideList.Add(info);
        }
    }

    public PlayerGuideInfo Lookup(int id)
    {
        foreach (PlayerGuideInfo info in PlayerGuideList)
        {
            if (info.id == id)
            {
                return info;
            }
        }
        return null;
    }
}
