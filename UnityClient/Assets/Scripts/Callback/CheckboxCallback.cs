//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Callback
//	created:	2013-6-27
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;


public class CheckboxCallback : MonoBehaviour
{
	public Action<string, bool> CallbackAction;
	void OnActivate(bool isActive)
	{
		if (null != CallbackAction)
		{
			CallbackAction(name, isActive);
		}
	}
};