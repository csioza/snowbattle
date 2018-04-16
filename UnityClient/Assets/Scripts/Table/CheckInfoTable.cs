using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CheckInfo : IDataBase
{
    public int CheckID { get; private set; }		   // CheckID, NPC Info����� [ServiceType] ����
    public string CheckType{ get; private set; }       // ��������
    public string CheckIcon{ get; private set; }       // һ��ǰ���ͼ��
    public string CheckDesc{ get; private set; }       // ��ʾ���ı�����
    public int EvnID { get; private set; }             // ��Ӧ�¼�ID
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
