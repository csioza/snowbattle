//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Callback
//	created:	2013-5-17
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;


public class SkillResultCallback : MonoBehaviour
{
	public Action<GameObject, AnimationEvent> Callback { get; set; }

	void OnSkillResult(AnimationEvent animEvent)
	{
		if (null != Callback)
		{
			Callback(gameObject, animEvent);
		}
	}
};