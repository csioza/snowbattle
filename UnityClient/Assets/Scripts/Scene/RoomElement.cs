using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ENNPCActiveCondition
{
    enNone,
    enNeverActive,
    enKillAllMonster,
}
public class RoomElement : MonoBehaviour
{
    public bool IsNpc = true;
    public bool IsUsePrefabLoad = false;
    public int  ObjSettingID = 0;
    public ENCamp CampID = ENCamp.enNone;
    public ENNPCActiveCondition m_ActiveCondition = ENNPCActiveCondition.enNone;
    public SM.MonsterRoomData MonsterData;
    public List<Vector3> _PatrolList = new List<Vector3>(); 
}
