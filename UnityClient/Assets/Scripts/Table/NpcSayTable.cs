using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcSayInfo : IDataBase
{
    public int id { get; protected set; }
    public int count { get; protected set; }
    public string say1 { get; protected set; }
    public string say2 { get; protected set; }
    public string say3 { get; protected set; }
}

public class NpcSayTable
{
    public List<NpcSayInfo> NpcSayList { get; protected set; }
    public NpcSayInfo sayInfoLookup(int id)
    {
        for (int index = 0; index < NpcSayList.Count;index ++ )
        {
            if ( NpcSayList[index].id == id )
            {
                return NpcSayList[index];
            }
        }
        return null;
    }
// 	public List<string> Lookup(int id)
// 	{
//         List<string> vList = null;
//         if (NpcSayList.TryGetValue(id, out vList))
//             return vList;
// 		return null;
// 	}

    public string GetRandSay(int nID)
    {
        string szVal = "??";
        NpcSayInfo sayInfo = sayInfoLookup(nID);
        if (null != sayInfo)
        {
            int nIndx = UnityEngine.Random.Range(0, sayInfo.count);
            Debug.Log("nIndx:" + nIndx);
            switch (nIndx)
            {
                case 0:
                    szVal = sayInfo.say1;
                    break;
                case 1:
                    szVal = sayInfo.say2;
                    break;
                case 2:
                    szVal = sayInfo.say3;
                    break;
            }
        }
// 
//         if ((null != vList) && (vList.Count > 0))
//         {
//             int nIndx = UnityEngine.Random.Range(0, vList.Count);
//             nIndx = Mathf.Min(nIndx, vList.Count - 1);
//             szVal = vList[nIndx];
//         }
        return szVal;
    }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        NpcSayList = new List<NpcSayInfo>(length);
        for (int index = 0; index < length; ++index)
        {
            NpcSayInfo info = new NpcSayInfo();
            info.Load(helper);
            NpcSayList.Add(info);
        }
    }

}
