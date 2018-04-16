//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Callback
//	created:	2013-4-22
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;


public class AnimationSoundCallback : MonoBehaviour
{
	public Action<GameObject, AnimationEvent> Callback { get; set; }
	void Sound(AnimationEvent animEvent)
	{
        if (false == GameSettings.Singleton.m_playSound)
        {
            return;
        }
		if (null != Callback)
		{
			Callback(gameObject, animEvent);
		}
		else
		{
			if (!string.IsNullOrEmpty(animEvent.stringParameter))
			{
				Debug.LogError("Can not use string!");
			}
			AudioClip clip = animEvent.objectReferenceParameter as AudioClip;
			if (null == clip)
			{
				return;
			}
			AudioSource.PlayClipAtPoint(clip, gameObject.transform.position);
		}
	}
};