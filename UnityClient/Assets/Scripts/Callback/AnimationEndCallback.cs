using System;
using UnityEngine;

public class AnimationEndCallback : MonoBehaviour
{
	//GameObject	当前对象
	//AnimationEvent	当前事件
	public Action<GameObject> Callback { get; set; }
	void OnAnimationEnd()
	{
		if (null != Callback)
		{
			Callback(gameObject);
			Callback = null;
		}
	}
}