using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SandTableInfo : IDataBase
{
    public int id { get; protected set; }
    public float areaX1 { get; protected set; }
    public float areaY1 { get; protected set; }
    public float areaX2 { get; protected set; }
    public float areaY2 { get; protected set; }
    public float nextAreaX1 { get; protected set; }
    public float nextAreaY1 { get; protected set; }

}

public class SandTableInfoTable
{
    public List<SandTableInfo> SandTableInfoList { get; protected set; }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        SandTableInfoList = new List<SandTableInfo>(length);
        for (int index = 0; index < length; ++index)
        {
            SandTableInfo info = new SandTableInfo();
            info.Load(helper);
            SandTableInfoList.Add(info);
        }
    }

    public SandTableInfo Lookup(int id)
    {
        foreach (SandTableInfo info in SandTableInfoList)
        {
            if (info.id == id)
            {
                return info;
            }
        }
        return null;
    }
}