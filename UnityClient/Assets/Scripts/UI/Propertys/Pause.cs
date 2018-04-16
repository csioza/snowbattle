using System;
using System.Collections.Generic;

// 暂停
public class Pause : IPropertyObject  
{
    // 已得到的道具ID列表
    public List<int> m_itemList   = null ;


    #region Singleton
    static Pause m_singleton;
    static public Pause Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new Pause();
            }
            return m_singleton;
        }
    }
    #endregion

    public Pause()
    {
        SetPropertyObjectID((int)MVCPropertyID.enPause);

        m_itemList = new List<int>();
    }


    public string GetFloorName()
    {
        FloorInfo floorInfo = GameTable.FloorInfoTableAsset.LookUp(StageMenu.Singleton.m_curFloorId);
        if (null == floorInfo)
        {
            UnityEngine.Debug.Log("Pause GetFloorName FloorInfo floorInfo == null m_floorId:" + StageMenu.Singleton.m_curFloorId);
            return "";
        }

        return floorInfo.m_name;
    }

    public string GetFloorCondition()
    {
        FloorInfo floorInfo = GameTable.FloorInfoTableAsset.LookUp(StageMenu.Singleton.m_curFloorId);
        if (null == floorInfo)
        {
            UnityEngine.Debug.Log("Pause GetFloorName FloorInfo floorInfo == null m_floorId:" + StageMenu.Singleton.m_curFloorId);
            return "";
        }
        return floorInfo.m_conditions;
    }

    public string GetFloorProgress()
    {
        FloorInfo floorInfo = GameTable.FloorInfoTableAsset.LookUp(StageMenu.Singleton.m_curFloorId);
        if (null == floorInfo)
        {
            UnityEngine.Debug.Log("Pause GetFloorName FloorInfo floorInfo == null m_floorId:" + StageMenu.Singleton.m_curFloorId);
            return "";
        }

        StageInfo stageTable = GameTable.StageTableAsset.Lookup(1);
        if ( null == stageTable )
        {
            UnityEngine.Debug.Log("Pause GetFloorName StageInfo stageTable == null :" );
            return "";
        }
        return stageTable.nLayerId+"/"+floorInfo.m_layerNum;
    }
   
}
