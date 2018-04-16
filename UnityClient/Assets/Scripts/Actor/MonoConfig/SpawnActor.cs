using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnActor : MonoBehaviour {
    
    public int m_npcStaticID=2;
    //public int m_aliveNpcID = 0;
    public float m_initSpawnCD = -1;
    public float m_spawnCD = 0;
    public int m_spawnCount = 0;
    public int DyncID = -1;
    public List<GameObject> m_patrolList = new List<GameObject>();
}
