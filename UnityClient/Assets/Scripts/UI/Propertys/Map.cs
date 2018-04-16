using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapPos
{
    public int m_x = 0;
    public int m_y = 0;
}

// 小地图
public class Map : IPropertyObject
{
 
    public float m_scale            = 0.0f; // 真正地图的 长与宽的比

    public float m_zoomInScale      = 0.1f;// 地图缩放比例

    public enum ENPropertyChanged
    {
        enNone,
        enSetMapSize,
        enAddPoint,
        enRemovePoint,
    }

    public Map()
    {
        SetPropertyObjectID((int)MVCPropertyID.enMap);

    }

    #region Singleton
    static Map m_singleton;
    static public Map Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new Map();
            }
            return m_singleton;
        }
    }
    #endregion

    public void SetMapSize()
    {
        Vector3 realLpos    = SM.RandomRoomLevel.Singleton.mLBPos;
        Vector3 realRpos    = SM.RandomRoomLevel.Singleton.mRTPos;

        float realMaxX      = realRpos.x - realLpos.x;
        float realMaxY      = realRpos.z - realLpos.z;

        // 用 长÷宽 算出来到底是 谁比较长 继而 用小地图中的 哪个作为依据 来重设
        m_scale             = realMaxX / realMaxY;

        //Debug.LogWarning("SetMapSize realMaxX:" + realMaxX + ",realMaxY:" + realMaxY + ",m_scale:" + m_scale + ",realLpos：" + realLpos + ",realRpos:" + realRpos);
        NotifyChanged((int)ENPropertyChanged.enSetMapSize, null);
    }

    public void AddPoint(Actor  actor)
    {
        NotifyChanged((int)ENPropertyChanged.enAddPoint, actor);
    }

    public void RomvePoint(Actor actor)
    {
        NotifyChanged((int)ENPropertyChanged.enRemovePoint, actor);
    }

}
