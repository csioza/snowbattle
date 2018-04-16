using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UISetCamera : UIWindow
{
    UILabel m_tips      = null;

    UILabel m_distance  = null;

	bool isOverlook     = false;
    GameObject m_ar     = null;
    GameObject m_aa     = null;
    GameObject m_ur     = null;
    GameObject m_ug     = null;
    GameObject m_sc     = null;

    UILabel m_aaText    = null;
    UILabel m_arText    = null;
    UILabel m_urText    = null;
    UILabel m_ugText    = null;

    UIInput m_fps       = null;

    UILabel m_fpsTips   = null;

    List<GameObject> m_gameObjList    = null;

    GameObject m_gold       = null;
    UILabel m_goldLabel     = null;

   public  float m_goldNum              = 0f; // 原来的金币的数目
   public float m_goldDesNum            = 0f; // 增长后的金币数目
   float svelocitx                  = 0.0f;

    bool m_goldGrow = false;

    static public UISetCamera GetInstance()
    {
        UISetCamera self = UIManager.Singleton.GetUIWithoutLoad<UISetCamera>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UISetCamera>("UI/UISetCamera", UIManager.Anchor.TopLeft);
        }
        return self;
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enMainPlayer, OnPropertyChanged);//heizTest

        m_tips          = FindChildComponent<UILabel>("Label");

        m_distance      = FindChildComponent<UILabel>("Label1");

        m_aaText        = FindChildComponent<UILabel>("AA");
        m_arText        = FindChildComponent<UILabel>("AR");
        m_urText        = FindChildComponent<UILabel>("UR");
        m_ugText        = FindChildComponent<UILabel>("UG");
        m_fps           = FindChildComponent<UIInput>("Fps");
        m_fpsTips       = FindChildComponent<UILabel>("FpsTips");

        m_gold          = FindChild("GoldSprite");

        m_goldLabel     = FindChildComponent<UILabel>("GoldLabel");

        m_tips.text     = "X:" + GameSettings.Singleton.CameraRotate.x + " Y:" + GameSettings.Singleton.CameraRotate.y + " Z:" + GameSettings.Singleton.CameraRotate.z;

        m_distance.text = "D=" + GameSettings.Singleton.CameraDistance + ",FOV=" + GameSettings.Singleton.CameraFieldOfView + ",R=" + GameSettings.Singleton.CameraRotate.x;

        m_gameObjList   = new List<GameObject>();

        PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetEffectPath("ef-AR"), ARCallback);
        PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetEffectPath("ef-AA"), AACallback);
        PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetEffectPath("ef-UR"), URCallback);
        PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetEffectPath("ef-UG"), UGCallback);
        PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetEffectPath("ef-SC"), SCCallback);

        m_fps.value = Application.targetFrameRate+"";
    }
    void ARCallback(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            m_ar = GameObject.Instantiate(obj) as GameObject;
            m_ar.SetActive(false);
        }
    }
    void AACallback(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            m_aa = GameObject.Instantiate(obj) as GameObject;
            m_aa.SetActive(false);
        }
    }
    void URCallback(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            m_ur = GameObject.Instantiate(obj) as GameObject;
            m_ur.SetActive(false);
        }
    }
    void UGCallback(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            m_ug = GameObject.Instantiate(obj) as GameObject;
            m_ug.SetActive(false);
        }
    }
    void SCCallback(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            m_sc = GameObject.Instantiate(obj) as GameObject;
            m_sc.SetActive(false);
        }
    }
	void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)//heizTest
    {
		if (eventType == (int)Actor.ENPropertyChanged.enSetCamera)
        {
            if (IsVisiable())
            {
                HideWindow();
            }
            else
            {
                ShowWindow();
            }
		}
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("Button", OnSet);
        AddChildMouseClickEvent("Button1", OnSetDistance);

        AddChildMouseClickEvent("Button2", OnSetYLeft);

        AddChildMouseClickEvent("Button3", OnSetYRight);

        AddChildMouseClickEvent("ButtonAA", OnShowAA);
        AddChildMouseClickEvent("ButtonAR", OnShowAR);
        AddChildMouseClickEvent("ButtonUG", OnShowUG);
        AddChildMouseClickEvent("ButtonUR", OnShowUR);

        AddChildMouseClickEvent("ButtonSC", OnShowSC);
        AddChildMouseClickEvent("ButtonEA", ShowNpcAttackRange);
        AddChildMouseClickEvent("ButtonEW", ShowNpcAlertRange);

        AddChildMouseClickEvent("FpsGo", OnChangeFps);

        AddChildMouseClickEvent("CreateObj", OnCreateModel);


		AddChildMouseClickEvent("CreateObjGrass", OnCreateModelGrass);
		AddChildMouseClickEvent("CreateObjStone", OnCreateModelStone);
        AddChildMouseClickEvent("ClearObj", OnClearModel);

        AddChildMouseClickEvent("ShowDebugLog", OnShowDebugLog);

    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        foreach ( GameObject item in m_gameObjList)
        {
            GameObject.Destroy(item);
        }
        m_gameObjList.Clear();

        if (m_ar != null)
        {
            GameObject.Destroy(m_ar);
            m_ar = null;
        }
        if (m_aa != null)
        {
            GameObject.Destroy(m_aa);
            m_aa = null;
        }
        if (m_ur != null)
        {
            GameObject.Destroy(m_ur);
            m_ur = null;
        }
        if (m_ug != null)
        {
            GameObject.Destroy(m_ug);
            m_ug = null;
        }
        if (m_sc != null)
        {
            GameObject.Destroy(m_sc);
            m_sc = null;
        }
    }

    public void OnSet(object sender, EventArgs e)
    {
       // GameSettings.Singleton.SetCameraRotateX();
		GameSettings.Singleton.SetCameraOverlook(isOverlook);
		if(isOverlook)
		{
			isOverlook =false;
		}
		else
		{
			isOverlook =true;
		}
        m_tips.text = "X:" + GameSettings.Singleton.CameraRotate.x + " Y:" + GameSettings.Singleton.CameraRotate.y + " Z:" + GameSettings.Singleton.CameraRotate.z;
    }

    public void OnSetDistance(object sender, EventArgs e)
    {
        GameSettings.Singleton.SetCameraDistance();

        m_distance.text = "D=" + GameSettings.Singleton.CameraDistance+",FOV="+GameSettings.Singleton.CameraFieldOfView+",R="+GameSettings.Singleton.CameraRotate.x;
    }

    public void OnSetYLeft(object sender, EventArgs e)
    {
        GameSettings.Singleton.SetCameraUp(true);
        m_distance.text = "D=" + GameSettings.Singleton.CameraDistance + ",FOV=" + GameSettings.Singleton.CameraFieldOfView + ",R=" + GameSettings.Singleton.CameraRotate.x;
//        m_tips.text = "X:" + GameSettings.Singleton.CameraRotate.x + " Y:" + GameSettings.Singleton.CameraRotate.y + " Z:" + GameSettings.Singleton.CameraRotate.z;
    }

    public void OnSetYRight(object sender, EventArgs e)
    {
        GameSettings.Singleton.SetCameraUp(false);
        m_distance.text = "D=" + GameSettings.Singleton.CameraDistance + ",FOV=" + GameSettings.Singleton.CameraFieldOfView + ",R=" + GameSettings.Singleton.CameraRotate.x;
        //m_tips.text = "X:" + GameSettings.Singleton.CameraRotate.x + " Y:" + GameSettings.Singleton.CameraRotate.y + " Z:" + GameSettings.Singleton.CameraRotate.z;
    }

    public void OnShowAR(object obj)
    {
        Player player                   = (Player)ActorManager.Singleton.MainActor;

        float AR                        = player.CurrentTableInfo.AttackRange*2;

        m_arText.text                   ="" + player.CurrentTableInfo.AttackRange;

        if (m_ar.activeSelf)
        {
            m_ar.SetActive(false);
        }
        else
        {
            m_ar.transform.parent           = player.MainObj.transform;
            m_ar.transform.localPosition    = new Vector3(0f, 0.1f, 0f);
            m_ar.transform.localScale       = new Vector3(AR , 0f, AR );

            m_ar.SetActive(true);
        }
       
        
    }

    public void OnShowAA(object obj)
    {
        Player player   = (Player)ActorManager.Singleton.MainActor;

        float AR        = player.CurrentTableInfo.AutoAttackRange * 2;

        m_aaText.text   = "" + player.CurrentTableInfo.AutoAttackRange;

        if (m_aa.activeSelf)
        {
            m_aa.SetActive(false);
        }
        else
        {
            m_aa.transform.parent           = player.MainObj.transform;
            m_aa.transform.localPosition    = new Vector3(0f, 0.1f, 0f);
            m_aa.transform.localScale       = new Vector3(AR , 0f, AR );

            m_aa.SetActive(true);
        }
    }

    public void OnShowUG(object obj)
    {
        Player player   = (Player)ActorManager.Singleton.MainActor;

        float AR        = player.CurrentTableInfo.UnlockGazingRange * 2;

        m_ugText.text   = "" + player.CurrentTableInfo.UnlockGazingRange;

        if (m_ug.activeSelf)
        {
            m_ug.SetActive(false);
        }
        else
        {
            m_ug.transform.parent           = player.MainObj.transform;
            m_ug.transform.localPosition    = new Vector3(0f, 0.1f, 0f);
            m_ug.transform.localScale       = new Vector3(AR , 0f, AR );

            m_ug.SetActive(true);
        }
    }

    public void OnShowUR(object obj)
    {
        Player player   = (Player)ActorManager.Singleton.MainActor;

        float AR        = player.CurrentTableInfo.UnlockRange * 2;

        m_urText.text   = "" + player.CurrentTableInfo.UnlockRange;

        if (m_ur.activeSelf)
        {
            m_ur.SetActive(false);
        }
        else
        {
            m_ur.transform.parent           = player.MainObj.transform;
            m_ur.transform.localPosition    = new Vector3(0f, 0.1f, 0f);
            m_ur.transform.localScale       = new Vector3(AR, 0f, AR );

            m_ur.SetActive(true);
        }
    }

    public void OnShowSC(object obj)
    {
        Player player   = (Player)ActorManager.Singleton.MainActor;

        float AR        = 8.5f;

        if (m_sc.activeSelf)
        {
            m_sc.SetActive(false);
        }
        else
        {
            m_sc.transform.parent           = player.MainObj.transform;
            m_sc.transform.localPosition    = new Vector3(0f, 0.1f, 0f);
            m_sc.transform.localScale       = new Vector3(AR, 0f, AR);

            m_sc.SetActive(true);
        }
    }
    public void ShowNpcAttackRange(object obj)
    {
        foreach (var actorPair in ActorManager.Singleton.m_actorMap)
        {
            if (actorPair.Value.Type != ActorType.enNPC)
            {
                continue;
            }

            NPC npc = actorPair.Value as NPC;
            npc.ShowAttackRange();
        }
    }

    public void ShowNpcAlertRange(object obj)
    {
        foreach (var actorPair in ActorManager.Singleton.m_actorMap)
        {
            if (actorPair.Value.Type != ActorType.enNPC)
            {
                continue;
            }

            NPC npc = actorPair.Value as NPC;
            npc.ShowAlertRange();
        }
    }

    public void OnChangeFps(object sender, EventArgs e)
    {
        Application.targetFrameRate = int.Parse(m_fps.value);

        m_fpsTips.text = Application.targetFrameRate + "";
    }
	
	public void OnCreateModelGrass( GameObject gameObj )
	{
        PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetPrefabPath("NPC/p2-n-grass"), LoadModelCallback);
	}
	public void OnCreateModelStone( GameObject gameObj )
	{
        PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetPrefabPath("NPC/p2-n-stone"), LoadModelCallback);
		
	}
    public void OnCreateModel( GameObject gameObj )
    {
        PoolManager.Singleton.LoadResourceAsyncCallback(GameData.GetPrefabPath("NPC/p2-n-sphere"), LoadModelCallback);
        //GameObject preObj       = GameData.LoadPrefab<GameObject>("NPC/p2-n-sphere");
    
        //GameObject obj          = GameObject.Instantiate(preObj) as GameObject;

        //m_gameObjList.Add(obj);

        //obj.transform.parent    =  MainGame.Singleton.MainObject.transform;

        //obj.LocalPositionX(ActorManager.Singleton.MainActor.MainObj.transform.position.x);
        //obj.LocalPositionY(ActorManager.Singleton.MainActor.MainObj.transform.position.y);
        //obj.LocalPositionZ(ActorManager.Singleton.MainActor.MainObj.transform.position.z);

    }
    void LoadModelCallback(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            GameObject model = GameObject.Instantiate(obj) as GameObject;

            m_gameObjList.Add(model);

            model.transform.parent = MainGame.Singleton.MainObject.transform;

            model.LocalPositionX(ActorManager.Singleton.MainActor.MainObj.transform.position.x);
            model.LocalPositionY(ActorManager.Singleton.MainActor.MainObj.transform.position.y);
            model.LocalPositionZ(ActorManager.Singleton.MainActor.MainObj.transform.position.z);
        }
    }

    public void OnClearModel(GameObject obj)
    {
        foreach (GameObject item in m_gameObjList)
        {
            GameObject.Destroy(item);
        }
        m_gameObjList.Clear();
    }

    public void OnShowDebugLog(GameObject obj)
    {
        DebugLog.Singleton.OnShowLog("");

    }

    public void PlayAnimation()
    {

        m_gold.GetComponent<Animation>().Play("ui-UIbattle-goldIncrease");

        m_goldGrow = true;
    }

    public Vector3 GetGoldPostion()
    {
        return m_gold.transform.position;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (m_goldGrow)
        {
            // 金币自动增长
            if ( m_goldDesNum - m_goldNum >  0.9 )
            {
               // m_goldNum = m_goldNum + 1;

                float golda = Mathf.SmoothDamp(m_goldNum, m_goldDesNum, ref svelocitx, 0.5f);

                m_goldNum   = golda;
                m_goldLabel.text = ((int)m_goldNum).ToString();
            }
            else
            {
                m_goldGrow = false;
            }
            //Debug.Log("m_goldNum:" + m_goldNum + ",m_goldDesNum:" + m_goldDesNum + ",m_goldGrow:" + m_goldGrow);
        }
    }
}