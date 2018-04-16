using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIPauseBtn : UIWindow
{
	UIPanel m_pauseWindow   = null;
	UIPanel m_settingWindow = null;
	UIPanel m_sureWindow    = null;
	UISprite m_block        = null;
    MainPlayer player { get { return ActorManager.Singleton.MainActor as MainPlayer; } }

    UILabel m_name          = null;
    UILabel m_conditons     = null;
    UILabel m_progress      = null;

    UIToggle m_effect       = null;
    UIToggle m_sound        = null;

	UISprite m_pauseBack =null;
	UIButton m_pauseBtn=null;

    //显示boss硬直，测试用
    UILabel m_testBoss = null;
    int m_testBossID = 0;

    static public UIPauseBtn Singleton
    {
        get
        {
            UIPauseBtn self = UIManager.Singleton.GetUIWithoutLoad<UIPauseBtn>();
            if (self == null)
            {
                self = UIManager.Singleton.LoadUI<UIPauseBtn>("UI/UIPauseBtn", UIManager.Anchor.Center);
            }
            return self;
        }
    }

    static public UIPauseBtn GetInstance()
    {
        UIPauseBtn self = UIManager.Singleton.GetUIWithoutLoad<UIPauseBtn>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UIPauseBtn>("UI/UIPauseBtn", UIManager.Anchor.Center);
        }
        return self;
    }
    public UIPauseBtn()
    {
        IsAutoMapJoystick = false;
    }
    public void Register(int actorID)
    {
        AddPropChangedNotify((int)MVCPropertyID.enActorStartID + actorID, OnPropertyChanged);
        m_testBossID = actorID;
    }
    public override void OnInit()
    {
        base.OnInit();
		m_pauseWindow = FindChildComponent<UIPanel>("Panel_pauseWindow");
		m_settingWindow = FindChildComponent<UIPanel>("Panel_settingWindow");//heizTest 
		m_sureWindow = FindChildComponent<UIPanel>("Panel_sureWindow");//heizTest 
		m_block = FindChildComponent<UISprite>("Sprite_block");

        m_name      = FindChildComponent<UILabel>("Label_floorName");
        m_conditons = FindChildComponent<UILabel>("Label_conditons");
        m_progress  = FindChildComponent<UILabel>("Label_progress");

        m_effect    = FindChildComponent<UIToggle>("Toggle_effects");
        m_sound     = FindChildComponent<UIToggle>("Toggle_soundEffects");

		m_pauseBack =FindChildComponent<UISprite>("pauseBackground");
		m_pauseBtn  =FindChildComponent<UIButton>("pauseBtn");
        EventDelegate.Add(m_effect.onChange, OnChangeEffect);

        EventDelegate.Add(m_sound.onChange, OnChangeSound);

        m_testBoss = FindChildComponent<UILabel>("Label_TestBOSS");
	}
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)Actor.ENPropertyChanged.enPauseBtn)
        {
            Actor self = ActorManager.Singleton.Lookup(m_testBossID);
            if (self != null && !self.IsDead)
            {
                m_testBoss.text = self.Props.GetProperty_Float(ENProperty.stamina).ToString();
            }
        }
    }
	public override void AttachEvent()
	{
        base.AttachEvent();
		AddChildMouseClickEvent("pauseBtn", OnButtonPauseClicked);
		AddChildMouseClickEvent("settingBtn", OnButtonSettingClicked);//heizTest 
		AddChildMouseClickEvent("closeBtn", OnButtonCloseClicked);
		AddChildMouseClickEvent("backBtn", OnButtonBackClicked);//heizTest
		AddChildMouseClickEvent("quitBtn", OnButtonQuitClicked);//heizTest
		AddChildMouseClickEvent("cannelBtn", OnButtonCannelClicked);//heizTest
		AddChildMouseClickEvent("OKBtn", OnButtonOKClicked);//heizTest
		AddChildMouseClickEvent("testCloseSetCamera", OnButtonCloseSetCameraClicked);//heizTest 
		AddChildMouseClickEvent("testChangeUIBtn", OnButtonChangeUIClicked);//heizTest
	}
    public void OnButtonPauseClicked(GameObject obj)
    {
        MainGame.Singleton.OnAppLogicPause(true);
        m_block.gameObject.SetActive(true) ;
		m_pauseWindow.gameObject.SetActive(true) ;
		m_pauseBtn.normalSprite = "pause_btn_press";
		//m_pauseBack.spriteName = "pause_btn_press";

		StartCoroutine( m_pauseWindow.GetComponent<Animation>().Play("ui-showWindow-00", true, () => Debug.Log("onComplete")) );

        UpdateInfo();
	}
    public void OnButtonSettingClicked(GameObject obj)//heizTest 
	{
		m_pauseWindow.gameObject.SetActive(false) ;
		m_settingWindow.gameObject.SetActive(true) ;

		StartCoroutine( m_settingWindow.GetComponent<Animation>().Play("ui-showWindow-00", true, () => Debug.Log("onComplete")) );
	}
    public void OnButtonCloseClicked(GameObject obj)
    {
		m_pauseBtn.normalSprite = "pause_btn";
        MainGame.Singleton.OnAppLogicPause(false);
        m_block.gameObject.SetActive(false);
        m_pauseWindow.gameObject.SetActive(false);
		m_pauseBack.spriteName = "pause_btn";

	}
    public void OnButtonBackClicked(GameObject obj)//heizTest 
	{
		m_pauseWindow.gameObject.SetActive(true) ;
		m_settingWindow.gameObject.SetActive(false) ;
		
		StartCoroutine( m_pauseWindow.GetComponent<Animation>().Play("ui-showWindow-00", true, () => Debug.Log("onComplete")) );
	}
	public void OnButtonQuitClicked(GameObject obj )//heizTest 
	{
		m_sureWindow.gameObject.SetActive(true) ;

		StartCoroutine( m_sureWindow.GetComponent<Animation>().Play("ui-showWindow-00", true, () => Debug.Log("onComplete")) );   

	}
    public void OnButtonCannelClicked(GameObject obj)//heizTest 
	{
		m_sureWindow.gameObject.SetActive(false) ;
	}
	public void OnButtonOKClicked(GameObject obj)//heizTest 
	{
        OnButtonCloseClicked(obj);

		m_sureWindow.gameObject.SetActive(false) ;

//         StateMainUI mainState           = new StateMainUI();
//         mainState.m_bShowMain           = true;
//         MainGame.Singleton.TranslateTo(mainState);

        BattleArena.Singleton.StageStop(true,false);
	}
    public void OnButtonChangeUIClicked(GameObject obj)
	{
         TipMessageBox.Singleton.OnShowTipMessageBox();
         //player.ChangeBattleBtn();
	}
    public void OnButtonCloseSetCameraClicked(GameObject obj)//heizTest
	{
		player.CloseSetCamera();
	}

    void UpdateInfo()
    {
        m_name.text         = Pause.Singleton.GetFloorName();
        m_conditons.text    = Pause.Singleton.GetFloorCondition();
        m_progress.text     = Pause.Singleton.GetFloorProgress();
    }

    void OnChangeEffect()
    {
       // Debug.Log("UIToggle.current.value:" + UIToggle.current.value);

        GameSettings.Singleton.m_playEffect = UIToggle.current.value;
    }

    void OnChangeSound()
    {
        GameSettings.Singleton.m_playSound = UIToggle.current.value;
    }

   
}
