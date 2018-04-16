//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Callback
//	created:	2013-4-16
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;


public class AnimationEffectCallback : MonoBehaviour
{
    public Action<GameObject, AnimationEvent> Callback { get; set; }
    public Action<GameObject, AnimationEvent> ConnectEffectCallback { get; set; }
    public Action<GameObject, AnimationEvent> ComboEffectCallback { get; set; }
	void Effect(AnimationEvent animEvent)
	{
		if (null != Callback)
		{
			Callback(gameObject, animEvent);
		}
	}
    //连接技特效
    void ConnectEffect(AnimationEvent animEvent)
    {
        if (null != ConnectEffectCallback)
        {
            ConnectEffectCallback(gameObject, animEvent);
        }
    }
    //终结技特效
    void ComboEffect(AnimationEvent animEvent)
    {
        if (null != ComboEffectCallback)
        {
            ComboEffectCallback(gameObject, animEvent);
        }
    }
};