using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public enum EnMainHeadType
{
    enHpChanged,
    enSwitchActor,
    enInitActor,
    enSoulCharge,
    enActorDead,
    enActorRelive,
    enUpdataMainHeadLevel,//更新主头像等级
}
public class UIMainHead : UIWindow
{
    UIHPBar m_hpBar = null;
    UILabel m_switchCD = null;
	UIHPBar m_SwitchBarHP =null;
	UIHPBar m_SwitchBarHP_F =null;
	UIPanel m_headroot =null;
	UISlider m_SwitchCDbar =null;

    UITexture m_chiefSprite = null;
    //UIButton m_chiefButton = null;
    UITexture m_deputySprite = null;
    //UIButton m_deputyButton = null;
    UITexture m_deputySprite_F = null;
    GameObject m_deputyObj = null;
    GameObject m_cdObj = null;

	TweenColor tw_main =null;
	TweenColor tw_minor =null;
    //剑魂s
    //UIPanel m_swordEffect = null;
    //static int AllSoulsCount = 6;//一次性获得的最多数量剑魂值
    //static int AllWordCount = 9;//多少剑魂值升一级（剑身上有多少符文）
    //static int MaxSwordLevel = 5;
    //GameObject[] m_soulList = new GameObject[AllSoulsCount];//剑魂的集合
    //GameObject[] m_wordChiefList = new GameObject[AllWordCount];//符文的集合
    //GameObject[] m_wordDeputyList = new GameObject[AllWordCount];//符文的集合
    //Animation m_animSouls = null;
    //UISprite m_spriteBaseChief = null;
    //UISprite m_spriteBaseDeputy = null;
    //角色死亡时的遮罩
    GameObject m_chiefDeadMask = null;
    GameObject m_deputyDeadMask = null;
    GameObject m_cdMask = null;

    GameObject m_swordFrame = null;
    GameObject m_souls = null;
    GameObject m_swordframe_deputy = null;

    //等级
    UILabel m_levelLabel = null;
    static public UIMainHead GetInstance()
	{
        UIMainHead self = UIManager.Singleton.GetUIWithoutLoad<UIMainHead>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIMainHead>("UI/UIHeadPortrait", UIManager.Anchor.TopLeft);
		return self;
	}
    public UIMainHead()
    {
        IsAutoMapJoystick = false;
    }
	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enMainPlayer, OnPropertyChanged);
        m_hpBar = FindChildComponent<UIHPBar>("BarHP");
        m_switchCD = FindChildComponent<UILabel>("SwitchTime");
		m_headroot = FindChildComponent<UIPanel>("headroot");

		m_SwitchBarHP =FindChildComponent<UIHPBar>("SwitchBarHP");
		m_SwitchBarHP_F =FindChildComponent<UIHPBar>("SwitchBarHP_F");
		m_SwitchCDbar =FindChildComponent<UISlider>("SwitchCDbar");

        //角色死亡时的遮罩
        m_chiefDeadMask = FindChild("MainMask");
        m_deputyDeadMask = FindChild("MinorMask");
        m_cdMask = FindChild("CDMask");

        m_deputyObj = FindChild("heroMinor");
        m_cdObj = FindChild("CDFrame");
        m_chiefSprite = FindChildComponent<UITexture>("MainPortait");
//        m_chiefButton = FindChildComponent<UIButton>("MainPortait");
        m_deputySprite = FindChildComponent<UITexture>("MinorPortait");
//        m_deputyButton = FindChildComponent<UIButton>("MinorPortait");
        m_deputySprite_F = FindChildComponent<UITexture>("MinorPortait_F");

		tw_main = FindChildComponent<TweenColor> ("Foreground");
		tw_minor =FindChildComponent<TweenColor> ("Foregroundblue_m");
//        m_spriteBaseChief = FindChildComponent<UISprite>("Sprite_swordBase", FindChildComponent<UIPanel>("swordFrame").gameObject);
//        m_spriteBaseDeputy = FindChildComponent<UISprite>("Sprite_swordBase", FindChildComponent<UIPanel>("swordFrame_Deputy").gameObject);
        //m_swordEffect = FindChildComponent<UIPanel>("Panel_swordEffect");
        //for (int i = 0; i < AllSoulsCount; i++)
        //{
        //    string str = "Sprite_soul" + (i + 1).ToString();
        //    m_soulList[i] = FindChild(str);
        //}
        //for (int j = 0; j < AllWordCount; j++)
        //{
        //    string str1 = "word" + (j + 1).ToString();
        //    m_wordChiefList[j] = FindChild(str1, m_spriteBaseChief.gameObject);
        //    m_wordDeputyList[j] = FindChild(str1, m_spriteBaseDeputy.gameObject);
        //}
        //m_animSouls = FindChildComponent<Animation>("Souls");
        //m_soulTimeLength = m_animSouls.GetClip("UIsword-souls-fly-01").length;

        GameObject obj = FindChild("LevelFrame");
        m_levelLabel = FindChildComponent<UILabel>("Label-level", obj);

        m_swordFrame = FindChild("swordFrame");
        m_souls = FindChild("Souls");
        m_swordframe_deputy = FindChild("swordFrame_Deputy");
	}
    public void UpdatMainHeadLevel()
    {
        MainPlayer MainPlayer = ActorManager.Singleton.MainActor;
        int level = MainPlayer.Props.GetProperty_Int32(ENProperty.level);
        m_levelLabel.text = "LV" + level;
    }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)Actor.ENPropertyChanged.enMainHead)
        {
            switch ((EnMainHeadType)eventObj)
            {
                case EnMainHeadType.enHpChanged:
                    {
                        Actor actor = obj as Actor;
                        float scaleHP = actor.HP / actor.MaxHP;

                        if (actor == ActorManager.Singleton.Chief)//主角色使用主血条
                        {
                            m_hpBar.HP = scaleHP;
                        }
                        else//副角色使用副血条
                        {
                            if (m_SwitchBarHP.gameObject.activeSelf)//在非切换动画播放的状态下时
                            {
                                m_SwitchBarHP.HP = scaleHP;
                            }
                            else//在切换动画播放时
                            {
                                m_SwitchBarHP_F.HP = scaleHP;
                            }
                        }
                    }
                    break;
                case EnMainHeadType.enSwitchActor:
                    {//切换角色
                        m_SwitchCDbar.value = m_isDead ? 1 : 0;

                        m_switchCD.text = m_cdTotal.ToString();
                        m_switchCD.gameObject.SetActive(m_isDead ? false : true);
                        m_cdValue = m_cdTotal;
                        m_cdBarTime = Time.time;

                        if (ActorManager.Singleton.Deputy != null)
                        {
                            Actor actor = obj as Actor;
                            if (actor == ActorManager.Singleton.Chief)
                            {
                                m_headroot.GetComponent<Animation>().Play("ui-switchportrait-01", PlayMode.StopSameLayer);
                                tw_main.PlayReverse();
                                tw_minor.duration = 0.01f;
                                tw_minor.PlayReverse();
                            }
                            else
                            {
                                m_headroot.GetComponent<Animation>().Play("ui-switchportrait-00", PlayMode.StopSameLayer);
                                tw_main.PlayForward();
                                tw_minor.duration = 0.3f;
                                tw_minor.PlayForward();
                            }
                        }
                        m_levelLabel.text = "lv" + ActorManager.Singleton.MainActor.Level.ToString();
                    }
                    break;
                case EnMainHeadType.enInitActor:
                    {
                        InitActor();
                    }
                    break;
                case EnMainHeadType.enUpdataMainHeadLevel:
                    {
                        //更新头像等级
                        UpdatMainHeadLevel();
                    }
                    break;
                case EnMainHeadType.enSoulCharge:
                    {
                        //if ((MainPlayer)obj == ActorManager.Singleton.Chief)
                        //{
                        //    m_spriteSoulCurrent = m_spriteBaseChief;
                        //}
                        //else
                        //{
                        //    m_spriteSoulCurrent = m_spriteBaseDeputy;
                        //}
                        //m_soulIndex = 0;
                        //ShowSoul();
                    }
                    break;
                case EnMainHeadType.enActorDead:
                    {
                        Actor actor = obj as Actor;
                        if (actor.ID == ActorManager.Singleton.Chief.ID)
                        {
                            m_chiefDeadMask.SetActive(true);
                        }
                        else
                        {
                            m_deputyDeadMask.SetActive(true);
                        }
                        m_cdMask.SetActive(true);
                    }
                    break;
                case EnMainHeadType.enActorRelive:
                    {
                        m_chiefDeadMask.SetActive(false);
                        m_deputyDeadMask.SetActive(false);
                        m_cdMask.SetActive(false);
                    }
                    break;
            }
        }
    }
    void InitActor()
    {
        int chiefIconID = GameTable.HeroInfoTableAsset.Lookup(ActorManager.Singleton.Chief.IDInTable).headImageId;
        IconInfomation chiefIconInfo = GameTable.IconInfoTableAsset.Lookup(chiefIconID);

        m_chiefSprite.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(chiefIconInfo.dirName);
       // m_chiefButton.normalSprite = chiefIconInfo.SpriteName;
        if (ActorManager.Singleton.Deputy == null)
        {//没有副角色，隐藏界面
            m_deputyObj.SetActive(false);
            m_cdObj.SetActive(false);
        }
        else
        {
            m_deputyObj.SetActive(true);
            m_cdObj.SetActive(true);
            int deputyIconID = GameTable.HeroInfoTableAsset.Lookup(ActorManager.Singleton.Deputy.IDInTable).headImageId;
   
            IconInfomation deputyIconInfo = GameTable.IconInfoTableAsset.Lookup(deputyIconID);
            m_deputySprite.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(deputyIconInfo.dirName);
           
         //   m_deputyButton.normalSprite = deputyIconInfo.SpriteName;
            m_deputySprite_F.mainTexture = m_deputySprite.mainTexture;
        }
        if (ActorManager.Singleton.Chief != null && ActorManager.Singleton.Chief.IsActorExit)
        {
            //初始化血条
            float scaleHP = ActorManager.Singleton.Chief.HP / ActorManager.Singleton.Chief.MaxHP;
            m_hpBar.HP = scaleHP;
            if (ActorManager.Singleton.Deputy != null)
            {
                m_headroot.GetComponent<Animation>().Play("ui-switchportrait-00", PlayMode.StopSameLayer);
                tw_main.PlayForward();
                tw_minor.duration = 0.3f;
                tw_minor.PlayForward();
                //初始化血条
                scaleHP = ActorManager.Singleton.Deputy.HP / ActorManager.Singleton.Deputy.MaxHP;
                m_SwitchBarHP.HP = scaleHP;
            }
        }
        m_levelLabel.text = "lv" + ActorManager.Singleton.MainActor.Level.ToString();
    }
    //soul
    /*int m_soulIndex = 0;
    UISprite m_spriteSoulCurrent = null;
    void ShowSoul()
    {
        if (m_soulIndex >= BattleArena.Singleton.SoulCharge.NotifySoulInfo.m_typeList.Count)
        {
            m_soulStartTime = float.MaxValue;
            BattleArena.Singleton.SoulCharge.Next();
            return;
        }
        EnSoulType type = BattleArena.Singleton.SoulCharge.NotifySoulInfo.m_typeList[m_soulIndex];
        int number = BattleArena.Singleton.SoulCharge.NotifySoulInfo.m_soulList[m_soulIndex];

        string spriteName = "";
        switch (type)
        {
            case EnSoulType.enSkillCombo:
                spriteName = "QTE_soul_S";
                break;
            case EnSoulType.enConnectEffect:
                spriteName = "QTE_soul_SC";
                break;
            case EnSoulType.enQte:
                spriteName = "QTE_soul_Q";
                break;
            case EnSoulType.enExtra:
                spriteName = "QTE_soul_QB";
                break;
        }
        for (int i = 0; i < AllSoulsCount; ++i)
        {
            m_soulList[i].SetActive(false);
            if (i >= number)
            {
                continue;
            }
            if (!string.IsNullOrEmpty(spriteName))
            {
                m_soulList[i].GetComponent<UISprite>().spriteName = spriteName;
            }
            ParticleSystem psFlash = FindChildComponent<ParticleSystem>("ef_soulFlash", m_soulList[i]);
            ParticleSystem psStar = FindChildComponent<ParticleSystem>("ef_soulStar", m_soulList[i]);
            ParticleSystem psAir = FindChildComponent<ParticleSystem>("ef_soulAir", m_soulList[i]);
            psFlash.gameObject.SetActive(false);
            psStar.gameObject.SetActive(false);
            psAir.gameObject.SetActive(false);
            psFlash.Clear();
            psStar.Clear();
            psAir.Clear();
            psFlash.gameObject.SetActive(true);
            psStar.gameObject.SetActive(true);
            psAir.gameObject.SetActive(true);

            m_soulList[i].SetActive(true);
        }
        m_animSouls.Play("UIsword-souls-fly-01", PlayMode.StopSameLayer);
        m_soulStartTime = Time.time;
    }
    */
    public override void AttachEvent()
    {
        base.AttachEvent();
		AddChildMouseClickEvent("MinorPortait", OnButtonSwitchActor);
		AddChildMouseClickEvent("MainPortait", OnButtonSwitchActortoMain);
    }
    public void OnButtonSwitchActor(object sender, EventArgs e)
    {
        SwitchActor();
    }
	public void OnButtonSwitchActortoMain(object sender, EventArgs e)
    {
        SwitchActor();
	}
    void SwitchActor()
    {
        bool isAttacking = false;
        MainPlayer mainActor = ActorManager.Singleton.MainActor;
        if (mainActor.CurrentCmd != null)
        {
            if (mainActor.CurrentCmd.m_type == Player.ENCmdType.enLoopNormalAttack || mainActor.CurrentCmd.m_type == Player.ENCmdType.enSkill)
            {
                isAttacking = true;
            }
        }
        (mainActor.SelfAI as AIPlayer).m_isAttacking = isAttacking;
        mainActor.CurrentCmd = new Player.Cmd(Player.ENCmdType.enSwitchActor);
    }
    float m_cdTimer { get { return ActorManager.Singleton.m_switchCDTimer; } set { ActorManager.Singleton.m_switchCDTimer = value; } }
    int m_cdTotal { get { return ActorManager.Singleton.m_switchCDTotal; } set { ActorManager.Singleton.m_switchCDTotal = value; } }
    bool m_isDead { get { return ActorManager.Singleton.m_isSwitchForDead; } set { ; } }
	float m_cdValue = 0;//等于切换CD的总时长
    float m_cdBarTime = 0;
    //
    //float m_soulStartTime = float.MaxValue;
    //float m_soulTimeLength = 0;
    public override void OnUpdate()
    {
        base.OnUpdate();
        float now = Time.time;
        if (m_cdTotal > 0)
        {
            if (now - m_cdTimer > 1)
            {
                m_cdTimer = now;
                --m_cdTotal;
                m_switchCD.text = m_cdTotal.ToString();
            }
            float d = now - m_cdBarTime;
            m_cdBarTime = now;
            m_SwitchCDbar.value += 1.0f / m_cdValue * d;
        }
        else if (m_cdTotal == 0)
        {
            m_switchCD.gameObject.SetActive(false);
            m_SwitchCDbar.value = 1;
        }
        //if (now - m_soulStartTime > m_soulTimeLength)
        //{
        //    if (BattleArena.Singleton.SoulCharge.NotifySoulInfo != null)
        //    {
        //        addSwordSoulValue(BattleArena.Singleton.SoulCharge.NotifySoulInfo.m_soulList[m_soulIndex]);
        //        AddSoulBuff();
        //        lightSwordWord();
        //        ++m_soulIndex;
        //        ShowSoul();
        //    }
        //}
    }
    //void AddSoulBuff()
    //{
    //    if (m_soulLevel > 0 && m_soulLevel <= MaxSwordLevel)
    //    {
    //        MainPlayer mp = null;
    //        if (m_spriteBaseChief == m_spriteSoulCurrent)
    //        {//chief
    //            mp = ActorManager.Singleton.Chief;
    //        }
    //        else
    //        {//deputy
    //            mp = ActorManager.Singleton.Deputy;
    //        }
    //        OccupationInfo info = GameTable.OccupationInfoAsset.LookUp(mp.CurrentTableInfo.Occupation);
    //        if (info != null)
    //        {
    //            if (m_soulLevel <= info.m_soulChargeList.Count)
    //            {
    //                int buffID = info.m_soulChargeList[m_soulLevel - 1];
    //                if (mp.MyBuffControl.BuffList.Find(item => item.BuffID == buffID) == null)
    //                {//buff只上一次
    //                    ResultAddBuff r = BattleFactory.Singleton.CreateResult(ENResult.AddBuff, mp.ID, mp.ID) as ResultAddBuff;
    //                    if (r != null)
    //                    {
    //                        r.ResultExpr(new float[1] { buffID });
    //                        BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
    //int[] m_soulTotal = new int[2];
    //int m_soulLevel = 0;
    //public void addSwordSoulValue(int increase)
    //{
    //    int level = 0, total = 0;
    //    if (m_spriteBaseChief == m_spriteSoulCurrent)
    //    {
    //        m_soulTotal[0] += increase;
    //        total = m_soulTotal[0];
    //        if (total > AllWordCount * MaxSwordLevel)
    //        {
    //            total = AllWordCount * MaxSwordLevel;
    //            m_soulTotal[0] = total;
    //        }
    //    }
    //    else
    //    {
    //        m_soulTotal[1] += increase;
    //        total = m_soulTotal[1];
    //        if (total > AllWordCount * MaxSwordLevel)
    //        {
    //            total = AllWordCount * MaxSwordLevel;
    //            m_soulTotal[1] = total;
    //        }
    //    }
    //    if (total % AllWordCount != 0)
    //    {
    //        level = total / AllWordCount;
    //    }
    //    else
    //    {
    //        if (total < AllWordCount)
    //        {
    //            level = 0;
    //        }
    //        else
    //        {
    //            level = total / AllWordCount;
    //            if (level >= MaxSwordLevel)
    //            {
    //                level = MaxSwordLevel;
    //            }
    //            else
    //            {
    //                level -= 1;
    //            }
    //        }
    //    }
    //    //Debug.LogWarning("total is " + total.ToString() + ", level is " + level.ToString());
    //    if (m_soulLevel != level)
    //    {
    //        m_soulLevel = level;
    //        if (level != 0)
    //        {
    //            string anim = "Swordlv" + m_soulLevel.ToString();
    //            m_spriteSoulCurrent.active = false;
    //            m_spriteSoulCurrent.active = true;
    //            m_spriteSoulCurrent.animation.Stop();
    //            m_spriteSoulCurrent.animation.Play(anim, PlayMode.StopAll);
    //        }
    //    }
    //}
    //public void lightSwordWord()
    //{
    //    int value = 0;
    //    if (m_spriteBaseChief == m_spriteSoulCurrent)
    //    {
    //        value = m_soulTotal[0] % AllWordCount;
    //        if (value == 0)
    //        {
    //            if (m_soulTotal[0] >= AllWordCount)
    //            {
    //                value = AllWordCount;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        value = m_soulTotal[1] % AllWordCount;
    //        if (value == 0)
    //        {
    //            if (m_soulTotal[1] >= AllWordCount)
    //            {
    //                value = AllWordCount;
    //            }
    //        }
    //    }
    //    for (int i = 0; i < AllWordCount; i++)
    //    {
    //        if (m_spriteBaseChief == m_spriteSoulCurrent)
    //        {
    //            m_wordChiefList[i].SetActive(false);
    //            if (i >= value)
    //            {
    //                continue;
    //            }
    //            m_wordChiefList[i].SetActive(true);
    //        }
    //        else
    //        {
    //            m_wordDeputyList[i].SetActive(false);
    //            if (i >= value)
    //            {
    //                continue;
    //            }
    //            m_wordDeputyList[i].SetActive(true);
    //        }
    //    }
    //    if (m_soulLevel == MaxSwordLevel)
    //    {
    //        m_swordEffect.gameObject.SetActive(true);
    //    }
    //}
    public override void OnShowWindow()
    {
        base.OnShowWindow();
        InitActor();
    }

    public void HideSwordSouls()
    {
        m_souls.SetActive(false);
        m_swordFrame.SetActive(false);
        m_swordframe_deputy.SetActive(false);
    }
}
