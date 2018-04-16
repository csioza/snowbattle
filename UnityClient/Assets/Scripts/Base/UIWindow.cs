using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public abstract class UIWindow
{
	//窗口所属的分组，可以以|分隔
	public string GroupFlag = "";
    public UIWindowCFG m_uiWindowCFG = null;
    public Boolean m_castRootAble = false;
	public GameObject WindowRoot { get; protected set; }
	public bool IsNamedWindow = false;
    //自动映射一个摇杆支持
    public bool IsAutoMapJoystick = true;
    //摇杆支持
    public JoystickAutoMapUI JoystickMap;

    [Flags]
    enum CachedStatus
    {
        None=0,
        Visible = 1,       //是否显示
        Parent = 2,        //是否改变了父亲
        Position=4,//设置了位置
        Scale=8,//设置了缩放
		Name =16, //设置了name
    }  
    Transform m_cacheParent;
    CachedStatus m_cacheStatus=CachedStatus.None;
    bool m_cacheVisiable = false;
    Vector3 m_cachePositon;
    Vector3 m_cacheScale;
	string m_cacheName;

   protected  bool m_tick = false;
    public bool IsWindowLoaded
    {
        get { return WindowRoot != null; }
    }
    public Vector3 LocalPosition
    {
        set
        {
            SetLocalPosition(value);
        }
    }
    public Vector3 LocalScale
    {
        set
        {
            SetLocalScale(value);
        }
    }
	public UIWindow ()
	{

	}

	public bool IsVisiable ()
	{
		return WindowRoot != null && WindowRoot.activeSelf;
	}

    public bool IsTick()
    {
        return m_tick;
    }
    public virtual void ShowWindow()
	{
        if (WindowRoot != null)
        {
            WindowRoot.SetActive(true);
            OnShowWindow();
            if (JoystickMap != null && WindowRoot.activeInHierarchy)
            {
                JoystickMap.OnEnable();
            }
        }
        else
        {
            m_cacheStatus |= CachedStatus.Visible;
            m_cacheVisiable = true;
        }
	}

    public virtual void HideWindow()
	{
        if (WindowRoot != null)
        {
            if (WindowRoot.activeSelf)
            {//当前窗口是显示状态
                if (GlobalEnvironment.Singleton.IsInCallbackOrTrigger)
                {
                    DelegateCoroutine.CallStartCoroutine(WindowRoot, HideWindowImpl());
                    return;
                }
                WindowRoot.SetActive(false);
            }
            OnHideWindow();
            if (JoystickMap != null)
            {
                JoystickMap.OnDestroy();
            }
        }
        else
        {
            m_cacheStatus |= CachedStatus.Visible;
            m_cacheVisiable = false;
        }
	}
    IEnumerator HideWindowImpl()
    {
        yield return new WaitForEndOfFrame();
        if (WindowRoot != null)
        {
            WindowRoot.SetActive(false);
            OnHideWindow();
        }
        else
        {
            m_cacheStatus |= CachedStatus.Visible;
            m_cacheVisiable = false;
        }
    }

    public void SetLocalPosition(Vector3 pos)
    {
        if (WindowRoot != null)
        {
            WindowRoot.transform.localPosition = pos;
        }
        else
        {
            m_cacheStatus |= CachedStatus.Position;
            m_cachePositon = pos;
        }
    }
    public void SetLocalScale(Vector3 scale)
    {
        if (WindowRoot != null)
        {
            WindowRoot.transform.localScale = scale;
        }
        else
        {
            m_cacheStatus |= CachedStatus.Scale;
            m_cacheScale = scale;
        }
    }
	public void SetRootName(string name)
	{
		 if (WindowRoot != null)
        {
            WindowRoot.transform.name = name;
        }
        else
        {
            m_cacheStatus |= CachedStatus.Name;
            m_cacheName = name;
        }
	}

    //public void Load (string prefabName)
    //{
    //    GameObject preObj = GameData.LoadPrefab<GameObject> (prefabName);
    //    if (preObj == null) {
    //        preObj = GameData.Load<GameObject> (prefabName);
    //    }
    //    WindowRoot = GameObject.Instantiate (preObj) as GameObject;
    //    WindowRoot.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
    //}
    //public bool TryLoadSync(string prefabName)
    //{
    //    GameObject preObj = GameData.LoadPrefab<GameObject>(prefabName);
    //    if (preObj == null)
    //    {
    //        preObj = GameData.Load<GameObject>(prefabName);
    //    }
    //    if (preObj != null)
    //    {
    //        WindowRoot = GameObject.Instantiate(preObj) as GameObject;
    //        WindowRoot.transform.localScale = Vector3.one;
    //        WindowRoot.transform.localPosition = Vector3.zero;
    //        OnLoaded();
    //        return true;
    //    }
    //    return false;
    //}
    //异步加载
    public void LoadAsync(string prefabName)
    {
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        MainGame.Singleton.StartCoroutine(Coroutine_Load(prefabName, data));
    }
    //public IEnumerator LoadAsync(string prefabName, ResPackge.AsyncLoadData data)
    public IEnumerator Coroutine_Load(string prefabName, GameResPackage.AsyncLoadObjectData data)
    {
        //GameResMng resMgr = GameResMng.GetResMng();
        //IEnumerator e = resMgr.LoadResourceAsync(GameData.GetPrefabPath(prefabName), data);
        IEnumerator e = GameResManager.Singleton.LoadResourceAsync(GameData.GetPrefabPath(prefabName), data);
        bool interrupted = false;
        //data.AsyncCount++;
        while (!interrupted)
        {
            e.MoveNext();
            //if (data.IsFinish)
            if (data.m_isFinish)
            {
                break;
            }
            yield return e.Current;
        }
        //data.FinishCount++;
        //if (!m_isDestroy && data.m_res != null)
        if (!m_isDestroy && data.m_obj != null)
        {
            WindowRoot = GameObject.Instantiate(data.m_obj) as GameObject;
            WindowRoot.transform.localScale = Vector3.one;
            //WindowRoot.transform.localPosition = Vector3.zero;
            OnLoaded();
        }
    }
    bool m_isDestroy=false;
	public void Destroy ()
    {
        if (JoystickMap != null)
        {
            JoystickMap.OnDestroy();
        }
        m_isDestroy = true;
		if (!IsNamedWindow) {
			UIManager.Singleton.RemoveUIFromList (this);
		} else {
			UIManager.Singleton.RemoveUIFromList (this);
            UIManager.Singleton.RemoveUIFromNamedList(this);
		}
		OnDestroy ();
        GameObject obj  = WindowRoot;
        WindowRoot      = null;
        if (obj != null)
        {

            UISprite[] sprites = obj.GetComponentsInChildren<UISprite>(true);
            for (int i = 0; i < sprites.Length; ++i)
            {

                UISprite tex = sprites[i];
                tex.atlas = null;
                sprites[i] = null;
            }


            UITexture[] textures = obj.GetComponentsInChildren<UITexture>();
            for (int i = 0; i < textures.Length; ++i)
            {
                UITexture tex = textures[i];
                if (tex.mainTexture != null)
                {
                    Resources.UnloadAsset(tex.mainTexture);
                }
                GameObject.DestroyImmediate(tex.gameObject);
                textures[i] = null;
            }
            GameObject.DestroyImmediate(obj);
        }
        Resources.UnloadUnusedAssets();
	}

    public void OnLoaded()
    {
        if ((m_cacheStatus &CachedStatus.Parent) != 0)
        {
            Vector3 pos = WindowRoot.transform.localPosition;
            SetParent(m_cacheParent);
            m_cacheParent = null;
            WindowRoot.transform.localScale = Vector3.one;
            WindowRoot.transform.localPosition = pos;
        }
        OnInit();
        AttachEvent();
        if ((m_cacheStatus & CachedStatus.Visible) != 0)
        {
            if (m_cacheVisiable)
            {
                ShowWindow();
            }
            else
            {
                HideWindow();
            }
        }
        if ((m_cacheStatus & CachedStatus.Position) != 0)
        {
            SetLocalPosition(m_cachePositon);
        }
        if ((m_cacheStatus & CachedStatus.Scale) != 0)
        {
            SetLocalScale(m_cacheScale);
        }
		if((m_cacheStatus & CachedStatus.Name) != 0)
		{
			SetRootName(m_cacheName);
		}
        m_cacheStatus = CachedStatus.None;
        if (IsAutoMapJoystick)
        {
            JoystickMap = new JoystickAutoMapUI();
            JoystickMap.Init(this);
        }
    }

    public void RefreshJoystickAutoMap()
    {
        if (JoystickMap != null)
        {
            JoystickMap.FindEventListener();
        }
    }

	public virtual void OnInit ()
	{
        m_uiWindowCFG = this.WindowRoot.GetComponent<UIWindowCFG>();
        if (null == m_uiWindowCFG)
        {
            //Debug.Log("------------------M_UIWindowCFG is NULL");
            //m_uiLevel = 1;// (int)m_uiWindowCFG.m_uiWindowLevel;
        }
	}

	public virtual void AttachEvent ()
	{

	}

	public virtual void OnShowWindow ()
	{

	}

    public virtual void OnHideWindow()
    {

    }

    public virtual Boolean OnLeave()
    {
        return true;
    }

	public virtual void OnDestroy ()
	{
        ClearPropChangedNotify();
	}
	struct PropCallbackData
	{
		public PropertyNotifyManager.OnPropertyChanged m_callback;
		public int m_objectID;
	}
	List<PropCallbackData> m_propCallback;

	public void AddPropChangedNotify (int objectID, PropertyNotifyManager.OnPropertyChanged callback)
	{
		if (m_propCallback == null) {
			m_propCallback = new List<PropCallbackData> ();
		}
		PropCallbackData d;
		d.m_callback = callback;
		d.m_objectID = objectID;
		m_propCallback.Add (d);
		PropertyNotifyManager.Singleton.RegisterReceiver (objectID, callback);
	}

    public void RemovePropChangedNotify(int objectID, PropertyNotifyManager.OnPropertyChanged callback)
    {
        if (m_propCallback != null)
        {
            m_propCallback.RemoveAll(item => item.m_objectID == objectID);
            PropertyNotifyManager.Singleton.RemoveReceiver(objectID, callback);
        }
    }

    public void ClearPropChangedNotify()
    {
        if (m_propCallback != null)
        {
            foreach (PropCallbackData d in m_propCallback)
            {
                PropertyNotifyManager.Singleton.RemoveReceiver(d.m_objectID, d.m_callback);
            }
            m_propCallback.Clear();
        }
    }

	public GameObject FindChild (string objName, GameObject fromParent=null)
	{
		GameObject parent = fromParent;
		if (parent == null) {
			parent = WindowRoot;
		}
		Transform child = FindChild (parent.transform, objName);
		if (null != child) {
			return child.gameObject;
		} else {
			return null;
		}
	}

	public T FindChildComponent<T> (string objName, GameObject fromParent = null) where T : Component
	{
		GameObject obj = FindChild (objName, fromParent);
		if (null != obj) {
			return obj.GetComponent<T> ();
		} else {
			return null;
		}
	}

	public T FindChildComponentByPath<T> (string objPath) where T : Component
	{
		Transform obj = WindowRoot.transform.Find (objPath);
		if (null != obj) {
			return obj.gameObject.GetComponent<T> ();
		} else {
			return null;
		}
	}
    
	//递归查找子控件
	private Transform FindChild (Transform parent, string objName)
	{
		if (parent.name == objName) {
			return parent;
		} else {
            Transform item;
            for (int i = 0; i < parent.childCount;i++ )
            {
                item = parent.GetChild(i);
                Transform child = FindChild(item, objName);
                if (null != child)
                {
                    return child;
                }
            }
			return null;
		}
	}

	public void AddChildMouseClickEvent (string objName, EventHandler<EventArgs> action)
	{
		AddMouseClickEvent (FindChild (objName), action);
	}

	public void AddChildMouseDoubleClickEvent (string objName, EventHandler<EventArgs> action)
	{
		AddMouseDoubleClickEvent (FindChild (objName), action);
	}

	public static void AddMouseClickEvent (GameObject obj, EventHandler<EventArgs> action)
	{
		if (obj != null) {
			AddEvent<UIMouseClick> (obj, action);
		}
	}
    public static void RemoveMouseClickEvent(GameObject obj)
    {
        if (obj != null)
        {
            RemoveEvent<UIMouseClick>(obj);
        }
    }
	public static void AddMouseDoubleClickEvent (GameObject obj, EventHandler<EventArgs> action)
	{
		if (obj != null) {
			AddEvent<UIMouseDoubleClick> (obj, action);
		}
	}
	public static void AddMouseDragEvent (GameObject obj, EventHandler<EventArgs> action)
	{
		if (obj != null) {
			AddEvent<UIMouseDrag> (obj, action);
		}
	}

    public void AddChildMouseClickEvent(string objName, UIEventListener.VoidDelegate fun)
    {
        UIEventListener.Get(FindChild(objName)).onClick = fun;
    }

	public static void AddEvent<T> (GameObject obj, EventHandler<EventArgs> action) where T : UIMouseEvent
	{
		T mouseEvent = obj.GetComponent<T> ();
		if (null != mouseEvent) {
			GameObject.DestroyImmediate (mouseEvent);
		}
		mouseEvent = obj.AddComponent<T> ();
		mouseEvent.MouseEvent = action;
	}
    public static void RemoveEvent<T>(GameObject obj) where T : UIMouseEvent
    {
        T mouseEvent = obj.GetComponent<T>();
        if (null != mouseEvent)
        {
            GameObject.DestroyImmediate(mouseEvent);
        }
    }
	public void AddChildMouseLongPressEvent(string objName, EventHandler<EventArgs> action)
	{
		AddMouseLongPressEvent (FindChild (objName), action);
	}
	
	public static void AddMouseLongPressEvent(GameObject obj, EventHandler<EventArgs> action)
	{
		if(obj != null){
			UIMouseLongPress mouseEvent = obj.GetComponent<UIMouseLongPress>();
			if(null != mouseEvent){
				GameObject.DestroyImmediate(mouseEvent);
			}
			mouseEvent = obj.AddComponent<UIMouseLongPress>();
			mouseEvent.trigger = UIMouseLongPress.Trigger.OnLongPress;
			mouseEvent.MouseEvent = action;
		}
	}
	
	public virtual void OnUpdate ()
	{

	}
	
	/// <summary>
	/// 修改Panel在Z轴方向坐标. Author: yulei
	/// </summary>
	/// <param name='z'>
	/// Z.
	/// </param>
	protected void SetWindowLayer (float z)
	{
		Vector3 vector = WindowRoot.transform.localPosition;
		WindowRoot.transform.localPosition = new Vector3 (vector.x, vector.y, z);
	}
    public void SetParent(Transform t)
    {
        if (WindowRoot != null)
        {
            WindowRoot.transform.parent = t;
        }
        else
        {
            m_cacheParent = t;
            m_cacheStatus |= CachedStatus.Parent;
        }
    }

    public void SetParent(GameObject parent)
    {
        SetParent(parent.transform);
    }

	//添加对服务器消息回应的处理
	public delegate void funcMsgResopond(MessageRespond respond);
	Dictionary<int, funcMsgResopond> m_respondListByRespondCode = new Dictionary<int, funcMsgResopond>();		//针对回应代码的回调函数
	Dictionary<int, funcMsgResopond> m_respondListByMessageID = new Dictionary<int, funcMsgResopond>();			//针对消息ID的回调函数
	public void RegisterRespondFuncByRespondCode(int respondCode,funcMsgResopond func)
	{
		if (m_respondListByRespondCode.Count == 0 )
		{
			AddPropChangedNotify((int)MVCPropertyID.enMessageRespond, OnMessageRespond);
			m_respondListByRespondCode.Add(respondCode, func);
		}
		else
		{
			if (m_respondListByRespondCode.ContainsKey(respondCode))
			{
				m_respondListByRespondCode[respondCode] = func;
			}
			else
			{
				m_respondListByRespondCode.Add(respondCode, func);
			}
			
		}
	}

	//添加对应消息的回馈调用
	public void RegisterRespondFuncByMessageID(int messageID, funcMsgResopond func)
	{
		if (m_respondListByMessageID.Count == 0)
		{
			AddPropChangedNotify((int)MVCPropertyID.enMessageRespond, OnMessageRespond);
			m_respondListByMessageID.Add(messageID, func);
		}
		else
		{
			if (m_respondListByMessageID.ContainsKey(messageID))
			{
				m_respondListByMessageID[messageID] = func;
			}
			else
			{
				m_respondListByMessageID.Add(messageID, func);
			}			
		}
	}

	public void OnMessageRespond(int objectID, int eventType, IPropertyObject obj, object eventObj)
	{
		if (!IsVisiable())
		{
			return;
		}
		MessageRespond respond = (MessageRespond)eventObj;
		int respondCode = respond.RespondCode;
		funcMsgResopond func = null;
		if (m_respondListByRespondCode.TryGetValue(respondCode, out func))
		{
			func(respond);
		}
		if (m_respondListByMessageID.TryGetValue(respond.MessageID, out func))
		{
			func(respond);
		}
	}

#region Coroutine
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        if (WindowRoot == null)
        {
            return null;
        }
        DelegateCoroutine mono = WindowRoot.GetComponent<DelegateCoroutine>();
        if (mono == null)
        {
            mono = WindowRoot.AddComponent<DelegateCoroutine>();
        }
        return mono.StartCoroutine(routine);
    }
    IEnumerator Invoke_Impl(Action callback, float timeSecond)
    {
        yield return new WaitForSeconds(timeSecond);
        callback();
    }
    public void Invoke(Action callback, float timeSecond)
    {
        StartCoroutine(Invoke_Impl(callback, timeSecond));
    }
#endregion
}

public class UINullWindow : UIWindow
{

}
