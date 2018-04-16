using UnityEngine;

public class UIMultiKillNPC : UIWindow
{
    UISprite m_sprite = null;
    Animation m_anim = null;
    static public UIMultiKillNPC Singleton
    {
        get
        {
            UIMultiKillNPC self = UIManager.Singleton.GetUIWithoutLoad<UIMultiKillNPC>();
            if (self == null)
            {
                self = UIManager.Singleton.LoadUI<UIMultiKillNPC>("UI/UIMultiKill", UIManager.Anchor.TopRight);
            }
            return self;
        }
    }

    static public UIMultiKillNPC GetInstance()
    {
        UIMultiKillNPC self = UIManager.Singleton.GetUIWithoutLoad<UIMultiKillNPC>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UIMultiKillNPC>("UI/UIMultiKill", UIManager.Anchor.TopRight);
        }
        return self;
    }

	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enBattlePropsManager, OnPropertyChanged);
        m_sprite = FindChildComponent<UISprite>("MultiKillWord");
        m_anim = WindowRoot.GetComponent<Animation>();
    }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)BattleArena.ENPropertyChanged.enMultiKillNPC)
        {
            int count = BattleArena.Singleton.KillNpcCount;
            string name;
            if (count == GetWorldParam(ENWorldParamIndex.enKillNPC_DoubleKill, out name))
            {
            }
            else if (count == GetWorldParam(ENWorldParamIndex.enKillNPC_TripleKill, out name))
            {
            }
            else if (count == GetWorldParam(ENWorldParamIndex.enKillNPC_QuadraKill, out name))
            {
            }
            else if (count == GetWorldParam(ENWorldParamIndex.enKillNPC_KillingSpree, out name))
            {
            }
            else if (count == GetWorldParam(ENWorldParamIndex.enKillNPC_Rampage, out name))
            {
            }
            else if (count >= GetWorldParam(ENWorldParamIndex.enKillNPC_Unstoppable, out name))
            {
            }
            if (!string.IsNullOrEmpty(name))
            {
                ShowWindow();
                m_anim.Stop();
                m_sprite.spriteName = name;
                m_anim.Play(m_anim.clip.name, PlayMode.StopAll);
                m_showStartTime = Time.time;
                m_tick = true;
            }
        }
    }
    float m_showStartTime = 0;
    float m_showDuration = 0;
    float ShowDuration
    {
        get
        {
            if (m_showDuration == 0)
            {
                m_showDuration = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enKillNPC_ShowDuration).FloatTypeValue;
            }
            return m_showDuration;
        }
    }
    override public void OnUpdate()
    {
        if (IsVisiable())
        {
            if (Time.time - m_showStartTime > ShowDuration)
            {
                HideWindow();
                m_tick = false;
            }
        }
    }
    int GetWorldParam(ENWorldParamIndex index, out string name)
    {
        WorldParamInfo info = GameTable.WorldParamTableAsset.Lookup((int)index);
        name = info.StringTypeValue;
        return info.IntTypeValue;
    }
}
