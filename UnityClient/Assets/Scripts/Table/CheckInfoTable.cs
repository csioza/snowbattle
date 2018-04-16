using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CheckInfo : IDataBase
{
    public int CheckID { get; private set; }		   // CheckID, NPC Info表里的 [ServiceType] 引用
    public string CheckType{ get; private set; }       // 操作类型
    public string CheckIcon{ get; private set; }       // 一条前面的图标
    public string CheckDesc{ get; private set; }       // 显示的文本内容
    public int EvnID { get; private set; }             // 对应事件ID
}



public class CheckInfoTable
{
	public List<CheckInfo> CheckInfoList { get; protected set; }
	public CheckInfo Lookup(int id)
	{
		return CheckInfoList.Find(item => item.CheckID == id);
	}
	public void Load(byte[] bytes)
	{
		BinaryHelper helper = new BinaryHelper(bytes);
		int length = helper.ReadInt();
		CheckInfoList = new List<CheckInfo>(length);
		for (int index = 0; index < length; ++index)
		{
			CheckInfo info = new CheckInfo();
			info.Load(helper);
			CheckInfoList.Add(info);
		}
	}
}
