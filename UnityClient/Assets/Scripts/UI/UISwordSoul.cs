using System;
using UnityEngine;

class UISwordSoul : UIWindow
{
    UIHPBar m_hpBar = null;

    static public UISwordSoul GetInstance()
    {
        UISwordSoul self = UIManager.Singleton.GetUIWithoutLoad<UISwordSoul>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UISwordSoul>("UI/UISwordSoul", UIManager.Anchor.TopLeft);
        }
        return self;
    }
    
    public UISwordSoul()
    {
        IsAutoMapJoystick = false;
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enBattlePropsManager, OnPropertyChanged);
        m_hpBar = FindChildComponent<UIHPBar>("BarControl");
    }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)BattleArena.ENPropertyChanged.enSwordSoul)
        {
            m_hpBar.HP = BattleArena.Singleton.SwordSoul.Percent;
            if (!ClientNet.Singleton.IsLongConnecting)
            {
                //ShowWindow();
            }
            
        }
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("pauseBtn", OnButtonFireSkill);
    }
    void OnButtonFireSkill(object sender, EventArgs e)
    {
        if (BattleArena.Singleton.SwordSoul.Percent != 1.0f)
        {
            return;
        }
        MainPlayer currentActor = ActorManager.Singleton.MainActor;
        int skillID = currentActor.CurrentTableInfo.SwordSoulSkillID;
        if (skillID == 0)
        {
            OccupationInfo info = GameTable.OccupationInfoAsset.LookUp(currentActor.CurrentTableInfo.Occupation);
            if (info == null)
            {
                Debug.LogError("current occupation is null,occupation:" + currentActor.CurrentTableInfo.Occupation);
                return;
            }
            if (info.m_swordSoulList.Count == 0 || info.m_swordSoulList[0] == 0)
            {
                Debug.LogError("current occupation is not have sword soul skill,occupation:" + currentActor.CurrentTableInfo.Occupation);
                return;
            }
            skillID = info.m_swordSoulList[0];
        }
        if (skillID == 0)
        {
            Debug.LogError("sword soul skill is null");
            return;
        }
        if (currentActor.OnFireSkill(skillID))
        {
            BattleArena.Singleton.SwordSoul.Clear();
        }
    }
}

