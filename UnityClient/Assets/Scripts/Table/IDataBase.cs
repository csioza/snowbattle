//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Table
//	created:	2013-5-13
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

public abstract class IDataBase
{
	public virtual void Load(BinaryHelper helper)
	{
		PropertyInfo[] propInfo = GetType().GetProperties();
		foreach (PropertyInfo info in propInfo)
		{
			if (info.PropertyType == typeof(string))
			{
				info.SetValue(this, helper.ReadString(), null);
			}
			else if (info.PropertyType == typeof(int))
			{
				info.SetValue(this, helper.ReadInt(), null);
			}
			else if (info.PropertyType == typeof(float))
			{
				info.SetValue(this, helper.ReadFloat(), null);
			}
			else if (info.PropertyType == typeof(bool))
			{
				info.SetValue(this, helper.ReadBool(), null);
			}
			else if (info.PropertyType == typeof(short))
			{
				info.SetValue(this, (short)helper.ReadInt(), null);
			}
            else if (info.PropertyType == typeof(List<int>))
            {
                info.SetValue(this, ConvertorTool.StringToList_Int(helper.ReadString()), null);
            }
            else if (info.PropertyType == typeof(List<float>))
            {
                info.SetValue(this, ConvertorTool.StringToList_Float(helper.ReadString()), null);
            }
            else if (info.PropertyType == typeof(List<string>))
            {
                info.SetValue(this, ConvertorTool.StringToList_String(helper.ReadString()), null);
            }
            else
            {
                Debug.Log("不支持的类型：" + info.PropertyType.ToString() + " " + info.Name);
            }

		}
	}

#if UNITY_EDITOR
	public virtual void Save(BinaryHelper helper)
	{
		PropertyInfo[] propInfo = GetType().GetProperties();
		foreach (PropertyInfo info in propInfo)
		{
			if (info.PropertyType == typeof(string))
			{
				helper.Write(info.GetValue(this, null) as string);
			}
			else if (info.PropertyType == typeof(int))
			{
				helper.Write((int)info.GetValue(this, null));
			}
			else if (info.PropertyType == typeof(float))
			{
				helper.Write((float)info.GetValue(this, null));
			}
			else if (info.PropertyType == typeof(bool))
			{
				helper.Write((bool)info.GetValue(this, null));
			}
			else if (info.PropertyType == typeof(short))
			{
				helper.Write((short)info.GetValue(this, null));
			}
            else if (info.PropertyType == typeof(List<int>))
            {
                helper.Write(ConvertorTool.ListToString(info.GetValue(this, null) as List<int>));
            }
            else if (info.PropertyType == typeof(List<float>))
            {
                helper.Write(ConvertorTool.ListToString(info.GetValue(this, null) as List<float>));
            }
            else if (info.PropertyType == typeof(List<string>))
            {
                helper.Write(ConvertorTool.ListToString(info.GetValue(this, null) as List<string>));
            }
            else
            {
                Debug.Log("不支持的类型：" + info.PropertyType.ToString() + " " + info.Name);
            }
		}
	}
#endif
};