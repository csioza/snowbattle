using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffEffectInfo : IDataBase
{
	//ID
	public int ID { get; protected set; }
	//特效名字
    public string EffectName { get; protected set; }
    //特效播放位置
    public string PlayBoneT { get; protected set; }
    //是否特殊位置播放特效
    public bool IsSpecialPoint { get; protected set; }
    //播放特效的特殊位置
    public string SpecialPointBoneT { get; protected set; }
    //是否附着在特效播放位置下
    public bool IsAdhered { get; protected set; }
    //特效播放位置 偏移x
    public float OffsetX { get; protected set; }
    //特效播放位置 偏移y
    public float OffsetY { get; protected set; }
    //特效播放位置 偏移z
    public float OffsetZ { get; protected set; }
    //effec level
    public int EffectLevel { get; protected set; }
    //生效后是否改变目标Shader的color
    public bool IsChangeShaderColor { get; protected set; }
    //shader颜色的名称
    public string ShaderColorName { get; protected set; }
    //shader颜色的参数
    public string ShaderColorParam { get; protected set; }
}

public class BuffEffectTable
{
    public Dictionary<int, BuffEffectInfo> BuffEffectInfoList { get; protected set; }
    public BuffEffectInfo Lookup(int id)
	{
        BuffEffectInfo info = null;
        BuffEffectInfoList.TryGetValue(id, out info);
        return info;
	}
	public void Load(byte[] bytes)
	{
		BinaryHelper helper = new BinaryHelper(bytes);
		int length = helper.ReadInt();
        BuffEffectInfoList = new Dictionary<int, BuffEffectInfo>(length);
		for (int index = 0; index < length; ++index)
		{
            BuffEffectInfo info = new BuffEffectInfo();
			info.Load(helper);
            BuffEffectInfoList.Add(info.ID, info);
		}
	}
}
