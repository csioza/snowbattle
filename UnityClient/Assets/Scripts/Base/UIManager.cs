using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class UIManager
{
	#region Singleton
	static public UIManager Singleton { get; private set; }
	public UIManager()
	{
		if (null == Singleton)
		{
			Singleton = this;
		}
		else
		{
			Debug.LogWarning("UIManager Recreated");
		}
	}
	#endregion

#region WindowWidth
	public Vector2 m_windowSize = Vector2.zero;
	public Vector2 WindowSize
	{
		get
		{
			if (m_windowSize == Vector2.zero)
			{
				GameObject obj = GameObject.Find("UI Root (2D)");
				if (obj)
				{
					UIRoot root = obj.GetComponent<UIRoot>();
					m_windowSize = new Vector2(root.manualHeight * Screen.width / Screen.height, root.manualHeight);
					return m_windowSize;
				}
			}
			return m_windowSize;
		}
	}

#endregion
    JoystickAutoMapUIRoot m_joystickAutoMapUIRoot;

    public void EnableJoystickAutoMap()
    {
        if (m_joystickAutoMapUIRoot == null)
        {
            m_joystickAutoMapUIRoot = new JoystickAutoMapUIRoot();
        }
        m_joystickAutoMapUIRoot.m_markObject = GameObject.Find("JoystickMark");
    }
    public JoystickAutoMapUIRoot JoystickAutoMap
    {
        get
        {
            return m_joystickAutoMapUIRoot;
        }
    }

	public void OnUpdate()
	{
		int UIIndex = 0;
		while (UIIndex < m_UIList.Count)
		{
            UIWindow ui = m_UIList[UIIndex];
			if (ui == null )
            {//|| m_UIList[UIIndex].WindowRoot == null
                // 异步加载，所以移除了判断空窗框 [9/4/2013 tgame]
				m_UIList.RemoveAt(UIIndex);
			}
			else
			{
                if (ui.IsVisiable() || ui.IsTick())
                {
                    ui.OnUpdate();
                }
				UIIndex++;
			}
		}
        m_joystickAutoMapUIRoot.Update();
	}

	public enum Anchor
	{
		Center = 0,
		TopLeft,
		Top,
		TopRight,
		Left,
		Right,
		BottomLeft,
		Bottom,
		BottomRight,
	}
    //加载UI，预设名字和T相同，位置为Center
	public T LoadUI<T>(bool needManage = true) where T : UIWindow, new()
	{
		return LoadUI<T>(typeof(T).ToString(), Anchor.Center, needManage);
	}
    //加载UI，预设名字和T相同
    public T LoadUI<T>(Anchor anchor, bool needManage = true) where T : UIWindow, new()
	{
		return LoadUI<T>(typeof(T).ToString(), anchor, needManage);
	}

    //CoroutineReturn TestYield()
    //{
        //yield return new TestYieldReturn();
    //}
    //加载UI
    public T LoadUI<T>(string prefabName, Anchor anchor, bool needManage = true) where T : UIWindow, new()
    {
        T ui = new T();
        GameObject anchorObj = GameObject.Find(anchor.ToString() + "Anchor");
        ui.SetParent(anchorObj.transform);
        if (needManage)
        {
            m_UIList.Add(ui);
        }
        //if (!ui.TryLoadSync(prefabName))
        {
            MainGame.Singleton.StartCoroutine(Coroutine_LoadUI(ui, prefabName));
        }
        //if (ui.IsNamedWindow)
        {
            if (!m_namedUIList.ContainsKey(ui.ToString()))
            {
                m_namedUIList.Add(ui.ToString(), ui);
            }
            else
            {
                m_namedUIList[ui.ToString()] = ui;
            }
        }
        //MainGame.Singleton.StartCoroutine(RadicalRoutine.Run(TestYield()));
        return ui;
    }
    //同步加载UI
    //public T LoadUISync<T>(string prefabName, Anchor anchor, bool needManage = true) where T : UIWindow, new()
    //{
    //    T ui = new T();
    //    ui.Load(prefabName);
    //    if (null == ui.WindowRoot)
    //    {
    //        Debug.LogError("Load" + prefabName + "error");
    //        return null;
    //    }
    //    GameObject anchorObj = GameObject.Find(anchor.ToString() + "Anchor");
    //    Vector3 pos = ui.WindowRoot.transform.localPosition;
    //    ui.WindowRoot.transform.parent = anchorObj.transform;
    //    ui.WindowRoot.transform.localScale = Vector3.one;
    //    ui.WindowRoot.transform.localPosition = pos;


    //    ui.OnInit();
    //    ui.AttachEvent();
    //    if (needManage)
    //    {
    //        m_UIList.Add(ui);
    //    }
    //    return ui;
    //}
    //异步加载UI
    IEnumerator Coroutine_LoadUI<T>(T win, string prefabName) where T : UIWindow, new()
    {
        //ResPackge.AsyncLoadData data = new ResPackge.AsyncLoadData();
        ///IEnumerator e = win.LoadAsync(prefabName, data);
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        IEnumerator e = win.Coroutine_Load(prefabName, data);
        bool interrupted = false;
        while(!interrupted)
        {
            e.MoveNext();
            //if (data.IsFinish)
            if (data.m_isFinish)
            {
                break;
            }
            yield return e.Current;
        }
    }
    
    public GameObject GetAnchor(Anchor anchor)
    {
        return GameObject.Find(anchor.ToString() + "Anchor");
    }

	//添加一个UI列表
	List<UIWindow> m_UIList = new List<UIWindow>();
    Dictionary<string, UIWindow> m_namedUIList = new Dictionary<string, UIWindow>();

	List<UIWindow> m_SavedUIStatus = new List<UIWindow>();

    public void RemoveUIFromList(UIWindow win)
    {
        if (m_UIList.Contains(win))
        {
            m_UIList.Remove(win);
        }
        else
        {
            Debug.LogWarning("RemoveUIFramList Failed:" + win.GetType().Name);
        }
    }
    //暂不使用
    //public bool RemoveUI<T>() where T : UIWindow, new()
    //{
    //    T window = GetUIWithoutLoad<T>();
    //    if (window != null)
    //    {
    //        window.Destroy();
    //        return true;
    //    }
    //    return false;
    //}

	public T GetUI<T>() where T : UIWindow, new()
	{
		T window = GetUIWithoutLoad<T>();
		if (null == window)
		{
			return LoadUI<T>();
		}
		else
		{
			return window;
		}
	}
	public T GetUIWithoutLoad<T>() where T : UIWindow, new()
	{
		for (int index = 0; index < m_UIList.Count; index++)
		{
			T tarUI = m_UIList[index] as T;
			if (tarUI != null)
			{
				return tarUI;
			}
		}
		return null;
	}
    public void RemoveUIByName(string winName)
    {
        UIWindow win = null;
        if (m_namedUIList.TryGetValue(winName, out win))
        {
            m_namedUIList.Remove(winName);
            win.Destroy();
        }
    }
    
    public void RemoveUIFromNamedList(UIWindow win)
    {
        foreach (KeyValuePair<string, UIWindow> p in m_namedUIList)
        {
            if (p.Value == win)
            {
                m_namedUIList.Remove(p.Key);
                return;
            }
        }
    }
    public T GetNamedUI<T>(string name, Anchor anchor) where T : UIWindow, new()
    {
        T obj = null;
        if (m_namedUIList.ContainsKey(name))
        {
            obj = (T)m_namedUIList[name];
            return obj;
        }
        obj = (T)LoadUI<T>(name, anchor);
        obj.IsNamedWindow = true;
        m_namedUIList[name] = obj;
        return obj;
    }
    public T LookupNamedUI<T>(string name) where T : UIWindow
    {
        T obj = null;
        if (m_namedUIList.ContainsKey(name))
        {
            obj = (T)m_namedUIList[name];
            return obj;
        }
        return null;
    }
    public void VisibleGroupedWindow(string groupName,bool visible)
    {
        foreach (UIWindow win in m_UIList)
        {
            if (win.GroupFlag.Contains(groupName))
            {
                if (visible)
                {
                    win.ShowWindow();
                }
                else
                {
                    win.HideWindow();
                }
            }
        }
    }
    public void HideAllWindow()
    {
        foreach (UIWindow win in m_UIList)
        {
            win.HideWindow();
        }
    }
	//保存窗口状态,待恢复
	public void SaveStatusAndHideAllWindow()
	{
		foreach (UIWindow win in m_UIList)
		{
			if (win.WindowRoot != null && win.WindowRoot.activeSelf == true)
			{
				win.WindowRoot.SetActive(false);
				win.OnHideWindow();
				m_SavedUIStatus.Add(win);
			}
		}
	}
	public void RecoverSavedWindow()
	{
		foreach (UIWindow win in m_SavedUIStatus)
		{
			if (win.WindowRoot != null && win.WindowRoot.activeSelf == false)
			{
				win.WindowRoot.SetActive(true);
				win.OnShowWindow();				
			}
		}
		m_SavedUIStatus.Clear();
    }
    //创建模型，并播放standby动画，cardID：HeroTableInfo表的id，parentObj：创建的obj的parent
    public void AddModel(int cardID, GameObject parentObj, GameResPackage.AsyncLoadObjectData data, bool isAutoActive = true)
    {
        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(cardID);
        if (null == info)
        {
            Debug.LogWarning("HeroInfo is null, id:" + cardID);
            return;
        }
        ModelInfo modelInfo = GameTable.ModelInfoTableAsset.Lookup(info.ModelId);
        if (null == modelInfo)
        {
            Debug.LogWarning("ModelInfo is null, id:" + info.ModelId);
            return;
        }
        MainGame.Singleton.StartCoroutine(Coroutine_LoadModel(GameData.GetPrefabPath(modelInfo.ModelFile), info.ModelId, info.WeaponId, parentObj, data, isAutoActive));
    }
    public void HideModel(GameObject obj)
    {
        PoolManager.Singleton.ReleaseObj(obj);
    }
    //协程-加载模型，并播放standby动画
    IEnumerator Coroutine_LoadModel(string prefabName, int modelID, int weaponID, GameObject parentObj, GameResPackage.AsyncLoadObjectData data, bool isAutoActive = true)
    {
        IEnumerator e = PoolManager.Singleton.Coroutine_Load(prefabName, data, true);
        while (true)
        {
            e.MoveNext();
            if (data.m_isFinish)
            {
                break;
            }
            yield return e.Current;
        }
        if (null != data.m_obj)
        {
            GameObject obj = data.m_obj as GameObject;
            obj.layer = GameObject.Find("UI Root (2D)").layer;
            obj.SetActive(true);
            foreach (Transform t in obj.transform.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = obj.layer;
            }
            PreAnimationLoad.Singleton.LoadAnimation(obj, modelID, weaponID);
            Animation MainAnim = obj.GetComponent<Animation>();

            if (null != MainAnim)
            {
                string mAnimName = "";
                ModelInfo info = GameTable.ModelInfoTableAsset.Lookup(modelID);
                WeaponInfo wInfo = GameTable.WeaponInfoTableAsset.Lookup(weaponID);
                if (info != null && wInfo != null)
                {
                    mAnimName = "a-" + info.modelType.ToString() + "-w" + ((int)wInfo.WeaponType).ToString() + "-";
                }
                AnimInfo tempAnimData = GameTable.AnimationTableAsset.Lookup("standby");
                if (null != tempAnimData)
                {
                    mAnimName = mAnimName + tempAnimData.AnimName[UnityEngine.Random.Range(0, tempAnimData.AnimName.Length)];
                }
                AnimationClip clip = MainAnim.GetClip(mAnimName);
                if (clip != null)
                {
                    MainAnim.CrossFade(clip.name);
                }
                else
                {
                    Debug.LogWarning("play model animation fail,prefabName:" + prefabName + ",animationName:" + mAnimName);
                }
            }

            obj.transform.parent = parentObj.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.Euler(0, 0, 0);

            if (weaponID != 0)
            {
                GameResPackage.AsyncLoadPackageData data2 = new GameResPackage.AsyncLoadPackageData();
                IEnumerator e2 = Coroutine_LoadWeapon(obj, weaponID, data2);
                while (true)
                {
                    e2.MoveNext();
                    if (data2.m_isFinish)
                    {
                        break;
                    }
                    yield return e2.Current;
                }
            }

            obj.SetActive(isAutoActive);
        }
    }
    //协程-加载武器
    public IEnumerator Coroutine_LoadWeapon(GameObject parentObj, int weaponID, GameResPackage.AsyncLoadPackageData loadData)
    {
        Transform t = Actor.S_LookupBone2(parentObj.transform, "LeftArmWeapon");
        if (t != null)
        {
            PoolManager.Singleton.ReleaseObj(t.gameObject);
        }
        t =  Actor.S_LookupBone2(parentObj.transform, "RightArmWeapon");
        if (t != null)
        {
            PoolManager.Singleton.ReleaseObj(t.gameObject);
        }
        WeaponInfo weaponInfo = GameTable.WeaponInfoTableAsset.Lookup(weaponID);
        if (weaponInfo == null)
        {
            Debug.Log("WeaponInfo load error,id:" + weaponID);
        }
        else
        {
            if (weaponInfo.LeftModelID != 0)
            {
                Transform leftArmParentTrans = Actor.S_LookupBone2(parentObj.transform, weaponInfo.LeftPoint);
                GameResPackage.AsyncLoadPackageData data = new GameResPackage.AsyncLoadPackageData();
                IEnumerator e = Coroutine_ArmLoad(leftArmParentTrans, "LeftArmWeapon", weaponInfo.LeftModelID, weaponInfo.IsHideLeftModel, data);
                while (true)
                {
                    e.MoveNext();
                    if (data.m_isFinish)
                    {
                        break;
                    }
                    yield return e.Current;
                }
            }
            if (weaponInfo.RightModelID != 0)
            {
                Transform rightArmParentTrans = Actor.S_LookupBone2(parentObj.transform, weaponInfo.RightPoint);
                GameResPackage.AsyncLoadPackageData data = new GameResPackage.AsyncLoadPackageData();
                IEnumerator e = Coroutine_ArmLoad(rightArmParentTrans, "RightArmWeapon", weaponInfo.RightModelID, weaponInfo.IsHideRightModel, data);
                while (true)
                {
                    e.MoveNext();
                    if (data.m_isFinish)
                    {
                        break;
                    }
                    yield return e.Current;
                }
            }
        }
        loadData.m_isFinish = true;
    }
    //协程-加载武器
    IEnumerator Coroutine_ArmLoad(Transform parentTrans, string armWeaponName, int armModelID, bool isHideModel, GameResPackage.AsyncLoadPackageData loadData)
    {
        ModelInfo modelInfo = GameTable.ModelInfoTableAsset.Lookup(armModelID);
        if (modelInfo == null)
        {
            Debug.Log("ModelInfo load fail, id:" + armModelID);
        }
        else
        {
            GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
            IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetPrefabPath(modelInfo.ModelFile), data, true);
            while (true)
            {
                e.MoveNext();
                if (data.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
            if (data.m_obj != null)
            {
                GameObject armObj = data.m_obj as GameObject;
                if (armObj != null)
                {
                    armObj.layer = parentTrans.gameObject.layer;

                    foreach (Transform t in armObj.transform.GetComponentsInChildren<Transform>())
                    {
                        t.gameObject.layer = armObj.layer;
                    }

                    armObj.transform.parent = parentTrans;
                    armObj.transform.LocalPositionY(0);
                    armObj.transform.LocalPositionX(0);
                    armObj.transform.LocalPositionZ(0);
                    armObj.transform.LocalScaleX(1);
                    armObj.transform.LocalScaleY(1);
                    armObj.transform.LocalScaleZ(1);
                    armObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    armObj.name = armWeaponName;

                    armObj.SetActive(!isHideModel);
                }
            }
            else
            {
                Debug.LogError("load obj fail, name:" + modelInfo.ModelFile);
            }
        }
        loadData.m_isFinish = true;
    }
}