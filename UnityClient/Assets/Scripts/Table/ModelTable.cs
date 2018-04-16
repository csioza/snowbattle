//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEngine;


public class ModelInfo : IDataBase
{
	public int ModelId { get; private set; }		//模型ID
	public string ModelFile { get; private set; }	//模型文件
	public string ModelName { get; private set; }
	public float Scale { get; private set; }		//suofang 
	public string IconRes { get; private set; }		//图标
	public string HeadRes { get; private set; }		//头像
    public int PortraitRes { get; private set; }	//半身像
    public int DropIcon { get; private set; }		// 掉落时显示的图标
    public List<string> AnimationList { get; private set; } //位移动作列表
    public int modelType { get; private set; } //模型类型
}

public class ModelInfoTable
{
	public Dictionary<int, ModelInfo> m_list { get; protected set; }
	public ModelInfo Lookup(int id)
	{
        ModelInfo info = null;
        m_list.TryGetValue(id, out info);
        return info;
	}
	public void Load(byte[] bytes)
	{
        m_list = new Dictionary<int, ModelInfo>();
		BinaryHelper helper = new BinaryHelper(bytes);
		int length = helper.ReadInt();
		for (int index = 0; index < length; ++index)
		{
			ModelInfo info = new ModelInfo();
            info.Load(helper);
			m_list.Add(info.ModelId, info);
		}
	}
};