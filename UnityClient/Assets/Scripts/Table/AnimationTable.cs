//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Tools
//	created:	2013-5-2
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;
using System.Collections.Generic;

public class AnimInfo
{
	public string AnimType;
	public string[] AnimName;

	public virtual void Load(BinaryHelper helper)
	{
		AnimType = helper.ReadString();
		AnimName = new string[helper.ReadInt()];
		for (int innerIndex = 0; innerIndex < AnimName.Length; ++innerIndex)
		{
			AnimName[innerIndex] = helper.ReadString();
		}
	}

#if UNITY_EDITOR
	public virtual void Save(BinaryHelper helper)
	{
		helper.Write(AnimType);
		helper.Write(AnimName.Length);
		foreach (string name in AnimName)
		{
			helper.Write(name);
		}
	}
#endif
}

public class AnimationTable
{
	public Dictionary<string, AnimInfo> AnimInfoList { get; protected set; }

	public AnimInfo Lookup(string animType)
	{
        if (string.IsNullOrEmpty(animType))
        {
            return null;
        }
        AnimInfo info = null;
        AnimInfoList.TryGetValue(animType, out info);
        return info;
	}

	public void Load(byte[] bytes)
	{
		AnimInfoList = new Dictionary<string, AnimInfo>();
		BinaryHelper helper = new BinaryHelper(bytes);
		int listLength = helper.ReadInt();
		for (int index = 0; index < listLength; ++index)
		{
			AnimInfo info = new AnimInfo();
			info.Load(helper);
			AnimInfoList.Add(info.AnimType, info);
		}
	}
}

