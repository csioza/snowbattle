using System;
using System.Collections.Generic;
using UnityEngine;

public class UISkillCD : UIWindow
{
    public enum EnShape
    {
        enNone,
        enCircle,
        enSquare,
        enSquare_Samall,
    }
    public int SkillID
    {
        get { return m_skillID; }
        set
        {
            m_skillID = value;
            SkillInfo info = GameTable.SkillTableAsset.Lookup(m_skillID);
            m_cdInfo = GameTable.CDTableAsset.Lookup(info.CoolDown);
        }
    }
    private int m_skillID = 0;
    CDInfo m_cdInfo = null;
    private GameObject m_remind = null;
    private UISprite m_sprite = null;

    UIButton m_btn = null;
    static public UISkillCD Create()
    {
        UISkillCD self = UIManager.Singleton.LoadUI<UISkillCD>("UI/UISkillCD", UIManager.Anchor.TopLeft);
        return self;
    }
    
    public UISkillCD()
    {
        IsAutoMapJoystick = false;
    }
    public void RegisterActor(Actor actor)
    {
        AddPropChangedNotify((int)MVCPropertyID.enActorStartID + actor.ID, OnPropertyChanged);
    }
    public void RemoveMainRegister()
    {
        RemovePropChangedNotify((int)MVCPropertyID.enMainPlayer, OnPropertyChanged);
    }
    EnShape m_shape = EnShape.enNone;
    public void Init(GameObject parent, UISkillCD.EnShape shape = EnShape.enCircle)
    {
        SetParent(parent);
        m_shape = shape;
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enMainPlayer, OnPropertyChanged);

        m_btn = FindChildComponent<UIButton>("SkillCD");
    }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == m_skillID)
        {
            Actor actor = obj as Actor;
            if (actor.Type == ActorType.enMain && actor.IsActorExit)
            {//离场角色的技能cd不刷新
                return;
            }
            if (!IsVisiable())
            {
                this.ShowWindow();
            }
            float fDuration = (float)(eventObj);
            m_sprite.fillAmount = fDuration;
            if (!m_btn.gameObject.activeSelf)
            {
                m_btn.gameObject.SetActive(true);
            }
            if (fDuration == 0)
            {
                //cd完毕，添加特效
                if (m_remind != null && m_cdInfo.CDTime != 0)
                {
                    m_remind.SetActive(true);
                    m_remind.GetComponent<Animation>().Play("ui-skillcdRemind-00", PlayMode.StopSameLayer);
                    m_animStartTime = Time.time;
                    m_tick = true;
                }
                //this.HideWindow();

                m_btn.gameObject.SetActive(false);
            }
        }
    }
    public void Reset()
    {
        if (m_btn == null) return;
        if (m_sprite == null)
        {
            WindowRoot.transform.localPosition = Vector3.zero;
            string objName = "", cdName = "";
            if (m_shape == EnShape.enCircle)
            {
                objName = "mask_circle";
                cdName = "CDRemind_circle";
            }
            else if (m_shape == EnShape.enSquare)
            {
                objName = "mask_rhombus";
                cdName = "CDRemind_rhombus";
            }
            else if (m_shape == EnShape.enSquare_Samall)
            {
                objName = "mask_square_small";
                cdName = "CDRemind_square_small";
            }
            else
            {
                Debug.LogWarning("objName is " + objName);
                return;
            }
            GameObject obj = FindChild(objName, WindowRoot);
            m_remind = FindChild(cdName, obj);
            m_sprite = obj.GetComponent<UISprite>();

            m_animLength = m_remind.GetComponent<Animation>().GetClip("ui-skillcdRemind-00").length;
            m_tick = true;
        }
        m_sprite.fillAmount = 0;

        m_btn.gameObject.SetActive(false);
    }
    public void ShowSkillCDBtn()
    {
        m_btn.gameObject.SetActive(true);
    }
    float m_animLength = 0;
    float m_animStartTime = 0;
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (m_animStartTime != 0 && Time.time - m_animStartTime > m_animLength)
        {
            m_animStartTime = 0;
            m_remind.SetActive(false);
        }
    }
    public override void OnShowWindow()
    {
        Reset();
        base.OnShowWindow();
    }
}
