using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIQTESequence : UIWindow
{
    public enum EnEventType
    {
        enShow,
        enChange,
        enAllSucc,
        enHide,
    }
    static int AllBadgeCount = 6;
    GameObject[] m_badgeList = new GameObject[AllBadgeCount];
    UIQTEDoneorFailed[] m_uiChildList = new UIQTEDoneorFailed[AllBadgeCount];
    //ParticleSystem m_psAddSucc = null;
    //
    float m_hideStartTime = float.MaxValue;
    float m_hideDuration = 0;
    float HideDuration
    {
        get
        {
            if (m_hideDuration == 0)
            {
                m_hideDuration = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enQteSequenceHideTime).FloatTypeValue;
            }
            return m_hideDuration;
        }
    }
    static public UIQTESequence Singleton
    {
        get
        {
            UIQTESequence self = UIManager.Singleton.GetUIWithoutLoad<UIQTESequence>();
            if (self == null)
            {
                self = UIManager.Singleton.LoadUI<UIQTESequence>("UI/UIQTESequence", UIManager.Anchor.Left);
            }
            return self;
        }
    }

    static public UIQTESequence GetInstance()
    {
        UIQTESequence self = UIManager.Singleton.GetUIWithoutLoad<UIQTESequence>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UIQTESequence>("UI/UIQTESequence", UIManager.Anchor.Left);
        }
        return self;
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enBattlePropsManager, OnPropertyChanged);

        for (int i = 0; i < AllBadgeCount; ++i)
        {
            string str = "badge" + (i + 1).ToString();
            m_badgeList[i] = FindChild(str);

            UIQTEDoneorFailed child = UIQTEDoneorFailed.Create();
            child.Init(m_badgeList[i]);

            m_uiChildList[i] = child;
        }
//        m_psAddSucc = FindChildComponent<ParticleSystem>("ef-allDoneFlash");
    }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        //if (eventType == (int)BattleArena.ENPropertyChanged.enQteSequence)
        //{
        //    switch ((EnEventType)eventObj)
        //    {
        //        case EnEventType.enShow:
        //            {
        //                ShowWindow();
        //                m_hideStartTime = float.MaxValue;
        //                m_psAddSucc.gameObject.SetActive(false);
        //                for (int i = 0; i < AllBadgeCount; ++i)
        //                {
        //                    m_badgeList[i].SetActive(false);
        //                    List<QTEProps.QTEInfo> list = BattleArena.Singleton.QTE.CurrentQteList;
        //                    if (i >= list.Count)
        //                    {
        //                        continue;
        //                    }
        //                    UISprite sprite = m_badgeList[i].GetComponent<UISprite>();
        //                    switch (list[i].m_type)
        //                    {
        //                        case ENSkillConnectionType.enConnect:
        //                            {
        //                                sprite.spriteName = "connectBadge";
        //                            }
        //                            break;
        //                        case ENSkillConnectionType.enSpecial:
        //                            {
        //                                sprite.spriteName = "specialBadge";
        //                            }
        //                            break;
        //                        case ENSkillConnectionType.enFinal:
        //                            {
        //                                sprite.spriteName = "finalBadge";
        //                            }
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                    //child
        //                    m_uiChildList[i].Reset();
        //                    //fire
        //                    ParticleSystem psFire = FindChildComponent<ParticleSystem>("ef-onfire", m_badgeList[i]);
        //                    psFire.gameObject.SetActive(false);
        //                    if (list[i].m_state == QTEProps.QTEInfo.EnQteInfoState.enQteIng)
        //                    {//ing
        //                        psFire.Clear();
        //                        psFire.gameObject.SetActive(true);
        //                    }

        //                    m_badgeList[i].SetActive(true);
        //                    //show
        //                    //m_badgeList[i].animation.Play("ui-qte-show-00", PlayMode.StopAll);
        //                    Color c = sprite.color;
        //                    c.a = 1;
        //                    sprite.color = c;


        //                    ParticleSystem psCircle = FindChildComponent<ParticleSystem>("circle", m_badgeList[i]);
        //                    psCircle.gameObject.SetActive(false);
        //                    psCircle.Clear();
        //                    psCircle.gameObject.SetActive(true);
        //                }
        //            }
        //            break;
        //        case EnEventType.enChange:
        //            {
        //                for (int i = 0; i < AllBadgeCount; ++i)
        //                {
        //                    List<QTEProps.QTEInfo> list = BattleArena.Singleton.QTE.CurrentQteList;
        //                    if (i >= list.Count)
        //                    {
        //                        continue;
        //                    }
        //                    ParticleSystem psFire = FindChildComponent<ParticleSystem>("ef-onfire", m_badgeList[i]);
        //                    psFire.gameObject.SetActive(false);
        //                    switch (list[i].m_state)
        //                    {
        //                        case QTEProps.QTEInfo.EnQteInfoState.enQteInit:
        //                            {//init
        //                                m_uiChildList[i].Reset();
        //                            }
        //                            break;
        //                        case QTEProps.QTEInfo.EnQteInfoState.enQteIng:
        //                            {//ing
        //                                m_uiChildList[i].Reset();
        //                                psFire.Clear();
        //                                psFire.gameObject.SetActive(true);
        //                            }
        //                            break;
        //                        case QTEProps.QTEInfo.EnQteInfoState.enQteSucc:
        //                            {//succ
        //                                m_uiChildList[i].Reset(UIQTEDoneorFailed.EnShowType.enShowSucc);
        //                                //半透明
        //                                m_badgeList[i].animation.Play("ui-qte-off-00", PlayMode.StopAll);
        //                            }
        //                            break;
        //                        case QTEProps.QTEInfo.EnQteInfoState.enQteFail:
        //                            {//fail
        //                                m_uiChildList[i].Reset(UIQTEDoneorFailed.EnShowType.enShowFail);
        //                                //半透明
        //                                m_badgeList[i].animation.Play("ui-qte-off-00", PlayMode.StopAll);
        //                            }
        //                            break;
        //                        case QTEProps.QTEInfo.EnQteInfoState.enQteEnd:
        //                            break;
        //                    }
        //                }
        //            }
        //            break;
        //        case EnEventType.enHide:
        //            {
        //                m_hideStartTime = Time.time;
        //            }
        //            break;
        //        case EnEventType.enAllSucc:
        //            {//all succ
        //                m_psAddSucc.Clear();
        //                m_psAddSucc.gameObject.SetActive(true);
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        m_badgeList = null;
        m_uiChildList = null;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Time.time - m_hideStartTime > HideDuration)
        {
            if (IsVisiable())
            {
                m_hideStartTime = float.MaxValue;
                HideWindow();
            }
        }
    }
}
