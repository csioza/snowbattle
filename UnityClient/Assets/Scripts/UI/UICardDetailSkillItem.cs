using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UICardDetailSkillItem : UIWindow
{
    GameObject m_lock   = null;
    UITexture m_frame   = null;
    GameObject m_button = null;
    Parma m_param       = null;

    public delegate void OnPressCallbacked();

    public delegate void OnClickCallbacked(object sender, EventArgs e);

    //OnPressCallbacked m_onPress = null;
    OnClickCallbacked m_onClick = null;

    static public UICardDetailSkillItem Create()
    {
        UICardDetailSkillItem self = UIManager.Singleton.LoadUI<UICardDetailSkillItem>("UI/UICardDetailSkillItem", UIManager.Anchor.Center);
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();

        m_lock      = FindChild("Lock");
        m_frame     = FindChildComponent<UITexture>("Frame");
        m_button    = FindChild("Button");
        m_param     = FindChildComponent<Parma>("Button");
    }

    public override void AttachEvent()
    {
        base.AttachEvent();

        AddMouseClickEvent(m_button, OnClick);
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (null == WindowRoot)
        {
            return;
        }
    }


    public override void OnShowWindow()
    {
        base.OnShowWindow();
    }

    public void SetPressCallbacked(EventDelegate.Callback callback)
    {
        EventDelegate.Add(m_button.GetComponent<UIEventTrigger>().onPress, callback);
    }


    public void SetClickCallbacked(OnClickCallbacked click)
    {
        m_onClick = click;
    }

    void OnClick(object sender, EventArgs e)
    {
        if ( m_onClick == null )
       {
           return;
       }

        m_onClick(sender, e);
    }


    public void Update(int skillId)
    {
        CSItem card = CardBag.Singleton.m_cardForDetail;
        if (card == null)
        {
            return;
        }

        SkillInfo skillInfo     = GameTable.SkillTableAsset.Lookup(skillId);
        if (null == skillInfo)
        {
            return;
        }
        IconInfomation iconInfo = GameTable.IconInfoTableAsset.Lookup(skillInfo.Icon);

        WindowRoot.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);

      
        // 身上是否有此技能

        m_lock.SetActive(!card.HaveSkill(skillId));


        int frameIconId = 0;
        // 是否是 切入技
        if (card.IsSwitchSkill(skillId))
        {
            frameIconId = (int)ENWorldParamIndex.enCardDetailSpecilSkillFrame;
        }
        else
        {
            frameIconId = (int)ENWorldParamIndex.enCardDetailSkillFrame;
        }

        WorldParamInfo worldInfo    = GameTable.WorldParamTableAsset.Lookup(frameIconId);
        if (worldInfo != null)
        {
            iconInfo                = GameTable.IconInfoTableAsset.Lookup(worldInfo.IntTypeValue);
            m_frame.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
        }

        m_param.m_id                = skillId;
    }

}
