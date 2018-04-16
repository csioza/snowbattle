//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Editor
//	created:	2013-5-13
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using NPOI.SS.UserModel;
using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

public class ExcelTableTools
{
	public static T GenericObj<T>(IRow row) where T : new()
	{
		int index = 0;
		T obj = new T();
		PropertyInfo[] propInfo = typeof(T).GetProperties();
		foreach (PropertyInfo info in propInfo)
		{
			ICell cell = row.GetCell(index++);
			if (info.PropertyType == typeof(string))
			{
				if (cell != null && cell.CellType == CellType.STRING)
				{
					info.SetValue(obj, cell.StringCellValue, null);
				}
			}
			else if (info.PropertyType == typeof(int))
			{
                if (cell != null && cell.CellType == CellType.NUMERIC)
				{
					info.SetValue(obj, (int)cell.NumericCellValue, null);
				}
			}
			else if (info.PropertyType == typeof(float))
			{
                if (cell != null && cell.CellType == CellType.NUMERIC)
				{
					info.SetValue(obj, (float)cell.NumericCellValue, null);
				}
			}
			else if (info.PropertyType == typeof(bool))
			{
                if (cell != null && cell.CellType == CellType.NUMERIC)
				{
					info.SetValue(obj, cell.NumericCellValue != 0, null);
				}
			}
			else if (info.PropertyType == typeof(short))
			{
                if (cell != null && cell.CellType == CellType.NUMERIC)
				{
					info.SetValue(obj, (short)cell.NumericCellValue, null);
				}
			}
            else if (info.PropertyType == typeof(List<int>))
            {
                string s = "";
                if (cell != null && cell.CellType == CellType.STRING)
                {
                    s = cell.StringCellValue;
                }
                info.SetValue(obj, ConvertorTool.StringToList_Int(s), null);
            }
            else if (info.PropertyType == typeof(List<float>))
            {
                string s = "";
                if (cell != null && cell.CellType == CellType.STRING)
                {
                    s = cell.StringCellValue;
                }
                info.SetValue(obj, ConvertorTool.StringToList_Float(s), null);
            }
            else if (info.PropertyType == typeof(List<string>))
            {
                string s = "";
                if (cell != null && cell.CellType == CellType.STRING)
                {
                    s = cell.StringCellValue;
                }
                info.SetValue(obj, ConvertorTool.StringToList_String(s), null);
            }
            else
            {
                Debug.LogError("不支持的类型：" + info.PropertyType.ToString() + " " + info.Name);
            }
		}
		return obj;
	}

    public static bool SetValue<T>(PropertyInfo info, T obj, ICell cell)
    {
        if (info.PropertyType == typeof(string))
        {
            if ((cell != null) && (cell.CellType == CellType.STRING))
            {
                info.SetValue(obj, cell.StringCellValue, null);
            }
            else info.SetValue(obj, "", null);
        }
        else if (info.PropertyType == typeof(int))
        {
            if ((cell != null) && (cell.CellType == CellType.NUMERIC))
            {
                info.SetValue(obj, (int)cell.NumericCellValue, null);
            }
            else info.SetValue(obj, 0, null);
        }
        else if (info.PropertyType == typeof(float))
        {
            if ((cell != null) && (cell.CellType == CellType.NUMERIC))
            {
                info.SetValue(obj, (float)cell.NumericCellValue, null);
            }
            else info.SetValue(obj, 0.0f, null);
        }
        else if (info.PropertyType == typeof(bool))
        {
            if ((cell != null) && (cell.CellType == CellType.NUMERIC))
            {
                info.SetValue(obj, cell.NumericCellValue != 0, null);
            }
            else info.SetValue(obj, false, null);
        }
        else if (info.PropertyType == typeof(short))
        {
            if ((cell != null) && (cell.CellType == CellType.NUMERIC))
            {
                info.SetValue(obj, (short)cell.NumericCellValue, null);
            }
            else info.SetValue(obj, 0, null);
        }
        else if (info.PropertyType == typeof(List<int>))
        {
            string s = "";
            if (cell != null && cell.CellType == CellType.STRING)
            {
                s = cell.StringCellValue;
            }
            info.SetValue(obj, ConvertorTool.StringToList_Int(s), null);
        }
        else if (info.PropertyType == typeof(List<float>))
        {
            string s = "";
            if (cell != null && cell.CellType == CellType.STRING)
            {
                s = cell.StringCellValue;
            }
            info.SetValue(obj, ConvertorTool.StringToList_Float(s), null);
        }
        else if (info.PropertyType == typeof(List<string>))
        {
            string s = "";
            if (cell != null && cell.CellType == CellType.STRING)
            {
                s = cell.StringCellValue;
            }
            info.SetValue(obj, ConvertorTool.StringToList_String(s), null);
        }
        else
        {
            Debug.LogError("不支持的类型：" + info.PropertyType.ToString() + " " + info.Name);
            return false;
        }
        return true;
    }
};