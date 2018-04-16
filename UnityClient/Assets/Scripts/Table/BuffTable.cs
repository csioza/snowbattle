using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffResultInfo
{
    public int ID { get; set; }
    public float[] ParamList { get; set; }

    public BuffResultInfo()
    {
        ParamList = new float[6];
    }
}

public class BuffInfo
{
    public enum ENBuffType
    {
        Harmful,//�к�
        Useful,//����
    }
    public static int PropertyCount = 20;
	//ͼ��ID
	public int ID { get; protected set; }
	//
	public string BuffName { get; protected set; }
    //buff����
    public string BuffDesc { get; protected set; }
    //buffͼ��
    public int BuffIcon { get; protected set; }
    //buff����  ENBuffType
    public int BuffType { get; protected set; }
    //buffЧ������
    public int BuffEffectType { get; protected set; }
    //�ɹ�����
    public float BuffPercent { get; protected set; }
    //�ɹ����ʳɳ�����
    public float BuffPercentParam { get; protected set; }
    //Ч������ʱ��
    public float BuffDuration { get; protected set; }
    //Ч������ʱ��ɳ�����
    public float BuffDurationParam { get; protected set; }
    //�Ƿ��һ����Ч
    public bool IsFirstWork { get; protected set; }
    //��ЧƵ��
    public float Period { get; protected set; }
    //����ʱ�Ƿ��Ƴ�
    public bool IsNotRemoveForDead { get; protected set; }
    //�滻�򹲴�
    public int Replaceable { get; protected set; }
    //buff��Чid
    public int BuffEffectID { get; protected set; }
    //���Ա����ɳ�����
    public float MultiplyParam { get; protected set; }
    //���Ըı�ɳ�����
    public float AddParam { get; protected set; }
    //���԰ٷֱȸı�
    public float[] PropertyPercentList { get; set; }
    //������ֵ�ı�
    public float[] PropertyValueList { get; set; }

    public int BuffResultCount { get; private set; }
    public List<BuffResultInfo> BuffResultList { get; set; }

    public virtual void Load(BinaryHelper helper)
    {
        ID = helper.ReadInt();
        BuffName = helper.ReadString();
        BuffDesc = helper.ReadString();
        BuffIcon = helper.ReadInt();
        BuffType = helper.ReadInt();
        BuffEffectType = helper.ReadInt();
        BuffPercent = helper.ReadFloat();
        BuffPercentParam = helper.ReadFloat();
        BuffDuration = helper.ReadFloat();
        BuffDurationParam = helper.ReadFloat();
        IsFirstWork = helper.ReadBool();
        Period = helper.ReadFloat();
        IsNotRemoveForDead = helper.ReadBool();
        Replaceable = helper.ReadInt();
        BuffEffectID = helper.ReadInt();
        MultiplyParam = helper.ReadFloat();
        AddParam = helper.ReadFloat();
        for (int i = 0; i < PropertyCount; ++i)
        {
            if (PropertyPercentList == null)
            {
                PropertyPercentList = new float[PropertyCount];
            }
            PropertyPercentList[i] = helper.ReadFloat();
        }
        for (int j = 0; j < PropertyCount; ++j)
        {
            if (PropertyValueList == null)
            {
                PropertyValueList = new float[PropertyCount];
            }
            PropertyValueList[j] = helper.ReadFloat();
        }
        BuffResultCount = helper.ReadInt();
        BuffResultList = new List<BuffResultInfo>(BuffResultCount);
        for (int index = 0; index < BuffResultCount; ++index)
        {
            BuffResultInfo info = new BuffResultInfo();
            info.ID = helper.ReadInt();
            for (int innerIndex = 0; innerIndex < info.ParamList.Length; ++innerIndex)
            {
                info.ParamList[innerIndex] = helper.ReadFloat();
            }
            BuffResultList.Add(info);
        }
    }

#if UNITY_EDITOR
    public virtual void Save(BinaryHelper helper)
    {
        helper.Write(ID);
        helper.Write(BuffName);
        helper.Write(BuffDesc);
        helper.Write(BuffIcon);
        helper.Write(BuffType);
        helper.Write(BuffEffectType);
        helper.Write(BuffPercent);
        helper.Write(BuffPercentParam);
        helper.Write(BuffDuration);
        helper.Write(BuffDurationParam);
        helper.Write(IsFirstWork);
        helper.Write(Period);
        helper.Write(IsNotRemoveForDead);
        helper.Write(Replaceable);
        helper.Write(BuffEffectID);
        helper.Write(MultiplyParam);
        helper.Write(AddParam);
        foreach (var item1 in PropertyPercentList)
        {
            helper.Write(item1);
        }
        foreach (var item2 in PropertyValueList)
        {
            helper.Write(item2);
        }
        if (BuffResultList != null)
        {
            BuffResultCount = BuffResultList.Count;
        }
        helper.Write(BuffResultCount);
        if (BuffResultCount > 0)
        {
            foreach (var info in BuffResultList)
            {
                helper.Write(info.ID);
                foreach (var innerInfo in info.ParamList)
                {
                    helper.Write(innerInfo);
                }
            }
        }
    }
#endif
}

public class BuffTable
{
	public Dictionary<int, BuffInfo> BuffInfoList { get; protected set; }
	public BuffInfo Lookup(int id)
	{
        BuffInfo info = null;
        BuffInfoList.TryGetValue(id, out info);
        return info;
	}
	public void Load(byte[] bytes)
	{
		BinaryHelper helper = new BinaryHelper(bytes);
		int length = helper.ReadInt();
        BuffInfoList = new Dictionary<int, BuffInfo>(length);
		for (int index = 0; index < length; ++index)
		{
			BuffInfo info = new BuffInfo();
			info.Load(helper);
			BuffInfoList.Add(info.ID, info);
		}
	}
}
