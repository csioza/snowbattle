using System;
using System.Collections.Generic;

public class StageMenu : IPropertyObject  
{
    #region Singleton
    static StageMenu m_singleton;
    static public StageMenu Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new StageMenu();
            }
            return m_singleton;
        }
    }
    #endregion
    public enum ENPropertyChanged
    {
        enShowStage = 1,
        enChangeTeam,
        enHide,
        enSetCurHelperGuid,
    }

    // 父ZONEID
    public int m_fatherZoneId   = 0;


    // 当前floorID
    public int m_curFloorId
    {
        get
        {
            return SM.RandomRoomLevel.Singleton.m_curFloorId;
        }
    }

    // 当前stageID
    public int m_curStageId     = 0;

    // 当前所选择的战友GUID
    public int m_curHelperGuid  = 0;

    public bool m_isReqHelpData = true;// 是否请求战友数据

    public int m_camp { get; set; }//阵营
    public string m_key { get; set; }//密钥
    public bool m_isRolling { get; set; }//是否允许翻滚

    public StageMenu()
    {
        SetPropertyObjectID((int)MVCPropertyID.enStageMenu);
    }

    public void ShowMenu( int zoneId )
    {
        m_fatherZoneId  = zoneId;

        //NotifyChanged((int)ENPropertyChanged.enShowStage, null); 
    }

    public void ChangeTeam(bool up)
    {
        NotifyChanged((int)ENPropertyChanged.enChangeTeam, up);
    }

    public void OnHide()
    {
        NotifyChanged((int)ENPropertyChanged.enHide,null);
    }

    // 设置当前 选择的战友ID
    public void SetCurHelperGuid(int curHelperGuid)
    {
        m_curHelperGuid = curHelperGuid;
        NotifyChanged((int)ENPropertyChanged.enSetCurHelperGuid, null);
    }
}
