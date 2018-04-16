using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class UIPoint : UIWindow
{
    UILabel m_name      = null;
    UISprite m_point    = null;
    int m_sobId         = 0;  
   
    static public UIPoint Create()
    {
        UIPoint self  = UIManager.Singleton.LoadUI<UIPoint>("UI/UIPoint", UIManager.Anchor.Center);
        return self;
    }
    
    public UIPoint()
    {
        IsAutoMapJoystick = false;
    }
    public override void OnInit()
    {
        base.OnInit();

        AddPropChangedNotify((int)MVCPropertyID.enMap, OnPropertyChanged);

        m_name  = FindChildComponent<UILabel>("Name");
        m_point = FindChildComponent<UISprite>("point");

    }

    public override void AttachEvent()
    {
        base.AttachEvent();

    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (null == WindowRoot)
        {
            return;
        }
    }

    public void SetAnchors(Transform transform)
    {
       UILabel pointLable               = WindowRoot.GetComponent<UILabel>();
       pointLable.rightAnchor.target    = transform;
       pointLable.leftAnchor.target     = transform;
       pointLable.bottomAnchor.target   = transform;
       pointLable.topAnchor.target      = transform;

       WindowRoot.transform.parent      = transform;
    }

    public void SetData(Actor actor)
    {
        ShowWindow();

        m_name.text = actor.Name+"<"+actor.Camp+">";

        // 暂时这样写 因为阵营没定 
        if (actor.Camp == 1)
        {
            m_point.spriteName = "number_bgn";
        }
        else if (actor.Camp == 2)
        {
            m_point.spriteName = "button";
        }
        else if (actor.Camp == 3)
        {
            m_point.spriteName = "Screening_cancel_icon";
        }

        m_sobId = actor.ID;

    }


    public override void OnUpdate()
    {
        Actor actor = ActorManager.Singleton.Lookup(m_sobId);

        if (actor == null || actor.CenterPart == null)
        {
            HideWindow();
            return;
        }

        float zoomInScale   = Map.Singleton.m_zoomInScale;
        float LBPosx        = SM.RandomRoomLevel.Singleton.mLBPos.x;
        float LBPosz        = SM.RandomRoomLevel.Singleton.mLBPos.z;

        WindowRoot.GetComponent<UILabel>().leftAnchor.absolute      = (int)((actor.CenterPart.transform.position.x - LBPosx) * zoomInScale) * 2;
        WindowRoot.GetComponent<UILabel>().bottomAnchor.absolute    = (int)((actor.CenterPart.transform.position.z - LBPosz) * zoomInScale) * 2;
    }


}
public class UIMap : UIWindow
{

    UISprite m_map          = null;
    UISprite m_bg           = null;
    UIGrid m_grid           = null;

    Dictionary<int, UIPoint> m_list = new Dictionary<int, UIPoint>();// int 为SOBID

    static public UIMap GetInstance()
    {
        UIMap self = UIManager.Singleton.GetUIWithoutLoad<UIMap>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIMap>("UI/UIMap", UIManager.Anchor.Center);
        return self;
    }
    
    public UIMap()
    {
        IsAutoMapJoystick = false;
    }
    public override void OnInit()
    {
        base.OnInit();

        AddPropChangedNotify((int)MVCPropertyID.enMap, OnPropertyChanged);

        m_map           = FindChildComponent<UISprite>("Map");

        m_bg            = FindChildComponent<UISprite>("Head_bgn");

        m_grid          = FindChildComponent<UIGrid>("Map");
    }

    public override void AttachEvent()
    {
        base.AttachEvent();

    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        foreach (KeyValuePair<int, UIPoint> item in m_list)
        {
            if (item.Value!= null)
            {
                item.Value.Destroy();
            }
           
        }
    }


    public void AddPoint(Actor actor)
    {
        if ( actor == null )
        {
            return;
        }

        if ( m_list.ContainsKey(actor.ID) )
        {
            m_list[actor.ID].SetData(actor);
        }
        else
        {
            UIPoint point = UIPoint.Create();
            point.SetData(actor);
            point.SetAnchors(m_map.transform);
            m_list.Add(actor.ID, point);
        }
        
    }

    public void RemovePoint(Actor actor)
    {
        if (actor == null)
        {
            return;
        }

        if (m_list.ContainsKey(actor.ID))
        {
           m_list[actor.ID].Destroy();
           m_list.Remove(actor.ID);
        }
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (null == WindowRoot)
        {
            return;
        }

        if (eventType == (int)Map.ENPropertyChanged.enSetMapSize)
        {
            SetSmallMapSize();

            m_grid.Reposition();
        }
        else if (eventType == (int)Map.ENPropertyChanged.enAddPoint)
        {
            Actor actor = (Actor)eventObj;
            AddPoint(actor);
        }
        else if (eventType == (int)Map.ENPropertyChanged.enRemovePoint)
        {
            Actor actor = (Actor)eventObj;
            RemovePoint(actor);
        }
    }

    // 设置小地图的 大小
    void SetSmallMapSize()
    {
        float scale = Map.Singleton.m_scale;
        // Y大于X 则以m_bg的高为基准 变化m_map的长
        if ( scale < 1.0f )
        {
            float y         = m_bg.localSize.y;
            float x         = y * Map.Singleton.m_scale;
           
            m_map.width     = (int)x;
            m_map.height    = (int)y;
            //Debug.LogWarning("1SetSmallMapSize x:" + x + ",y:" + y);
        }
        // X大于Y 则以m_bg的长为基准 变化m_map的高
        else
        {
            float x         = m_bg.localSize.x;
            float y         = x / Map.Singleton.m_scale;
            m_map.width     = (int)x;
            m_map.height    = (int)y;
           // Debug.LogWarning("2SetSmallMapSize x:" + x + ",y:" + y);

        }

        SetZoomInScale();
    }

    // 设置 缩放比例
    void SetZoomInScale()
    {
        Vector3 realLpos    = SM.RandomRoomLevel.Singleton.mLBPos;
        Vector3 realRpos    = SM.RandomRoomLevel.Singleton.mRTPos;

        float realMaxX      = realRpos.x - realLpos.x;

        Map.Singleton.m_zoomInScale = m_map.width / realMaxX;
    }
}
