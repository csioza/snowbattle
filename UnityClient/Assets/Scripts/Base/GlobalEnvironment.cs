using UnityEngine;
using System.Collections;

public class GlobalEnvironment
{
    static GlobalEnvironment m_singleton;
    static public GlobalEnvironment Singleton { get{
        if(m_singleton == null)
        {
            m_singleton = new GlobalEnvironment();
        }
        return m_singleton;
    }}
    //只有OnTriggerEnter要设置这个标志，最好是在OnTriggerEnter中不销毁和隐藏任何的gameobject
    public bool IsInCallbackOrTrigger = false;
}
