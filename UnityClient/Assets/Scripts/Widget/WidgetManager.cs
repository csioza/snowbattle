//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Widget
//	created:	2013-4-9
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEngine;


public class WidgetManager
{
//#region Singleton
//	public static WidgetManager Singleton { get; private set; }
//	public WidgetManager()
//	{
//		if (null == Singleton)
//		{
//			Singleton = this;
//		}
//		else
//		{
//			Debug.LogWarning("WidgetManager Recreated");
//		}
//	}
//#endregion
	public void Update()
	{
		m_widgetList.ForEach(delegate(IWidget item) { item.Update(); });
	}
	public void FixedUpdate()
	{
		m_widgetList.ForEach(delegate(IWidget item)
		{ 
			item.FixedUpdate();
			if (!item.IsEnable)
			{
				item.Release();
			}
		});
		m_widgetList.RemoveAll(delegate(IWidget item) { return !item.IsEnable; });
	}

	public void Clear()
	{
		foreach (var item in m_widgetList)
		{
			item.Release();
		}
		m_widgetList.Clear();
	}

	private List<IWidget> m_widgetList = new List<IWidget>();
	public T AddWidget<T>() where T : IWidget, new()
	{
		T obj = IPoolWidget<T>.CreateObj();
		m_widgetList.Add(obj);
		return obj;
	}
};