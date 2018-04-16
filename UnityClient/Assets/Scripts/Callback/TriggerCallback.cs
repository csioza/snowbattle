using System;
using UnityEngine;

//攻击collider的回调脚本，跟攻击collider在同一gameobject上
class TriggerCallback : MonoBehaviour
{
    public Action<GameObject, Collider> EnterCallback { get; set; }
    public Action<GameObject, Collider> ExitCallback { get; set; }

    void OnTriggerEnter(Collider other)
    {
        GlobalEnvironment.Singleton.IsInCallbackOrTrigger = true;
        if (EnterCallback != null)
        {
            EnterCallback(gameObject, other);
        }
        GlobalEnvironment.Singleton.IsInCallbackOrTrigger = false;
    }
    void OnTriggerExit(Collider other)
    {
        GlobalEnvironment.Singleton.IsInCallbackOrTrigger = true;
        if (ExitCallback != null)
        {
            ExitCallback(gameObject, other);
        }
        GlobalEnvironment.Singleton.IsInCallbackOrTrigger = false;
    }
}