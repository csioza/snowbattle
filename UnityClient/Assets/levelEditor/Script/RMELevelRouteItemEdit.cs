using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class LevelRouteRoomInfo
{
    public int RoomId = 0;
    public int Roomweight = 0;
}


[System.Serializable]
public class BridgeInfo
{
   
    public int ObjID              = 0;
    public int Length             = 0;

    public GameObject nextRouteItem;
}


// 关卡路线原件 编辑
public class RMELevelRouteItemEdit : MonoBehaviour
{
    public enum Direction
    {
        enNorth = 0,
        enEast  ,
        enSouth,
        enWest,
    }
    public float showPercent    = 0.0f;


    [SerializeField]
    public List<LevelRouteRoomInfo> roomlist;

    public BridgeInfo NorthBridge;  // 北
    public BridgeInfo EastBridge;   // 东
    public BridgeInfo SouthBridge;  // 南
    public BridgeInfo WestBridge;   // 西

}




