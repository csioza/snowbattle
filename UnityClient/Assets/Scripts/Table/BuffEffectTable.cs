using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffEffectInfo : IDataBase
{
	//ID
	public int ID { get; protected set; }
	//��Ч����
    public string EffectName { get; protected set; }
    //��Ч����λ��
    public string PlayBoneT { get; protected set; }
    //�Ƿ�����λ�ò�����Ч
    public bool IsSpecialPoint { get; protected set; }
    //������Ч������λ��
    public string SpecialPointBoneT { get; protected set; }
    //�Ƿ�������Ч����λ����
    public bool IsAdhered { get; protected set; }
    //��Ч����λ�� ƫ��x
    public float OffsetX { get; protected set; }
    //��Ч����λ�� ƫ��y
    public float OffsetY { get; protected set; }
    //��Ч����λ�� ƫ��z
    public float OffsetZ { get; protected set; }
    //effec level
    public int EffectLevel { get; protected set; }
    //��Ч���Ƿ�ı�Ŀ��Shader��color
    public bool IsChangeShaderColor { get; protected set; }
    //shader��ɫ������
    public string ShaderColorName { get; protected set; }
    //shader��ɫ�Ĳ���
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
