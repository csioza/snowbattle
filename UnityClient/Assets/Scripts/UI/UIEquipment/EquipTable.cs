using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region disableWarning
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#endregion

public class ENEquipSlot
{
    public const string Weapon = "Weapon";
    public const string BodyArmor = "BodyArmor";
    public const string CloakArmor = "CloakArmor";
    public const string HeadArmor = "HeadArmor";
    public const string Gloves = "Gloves";
    public const string Decoration = "Decoration";
    public const string MAX = "MAX";
}

// public enum ENEquipSlot
// {
// 	//武器,衣服,裤子,帽子,手套,饰品
// 	Weapon = 0,
//     BodyArmor,
//     FootArmor,
//     HeadArmor,
// 	Gloves,
// 	Decoration,
// 	MAX,
// }

//资质
public enum ENEquipAptitude
{
    //锋利，贤者，勇者，热情，优雅，朴实，王者
}

// public enum ENEquipPropertys
// {
//     //攻击速度，生命值，魔法值，sp值，物攻，法功，物防，法防，命中，闪避，暴击，气魄
//     AttackSpeed = 0,
//     HP,
//     MP,
//     SP,
//     PhyAttack,
//     MagAttack,
//     PhyDefend,
//     MagDefend,
//     Hit,
//     Avoid,
//     CriAttack,
//     StillAttack,
//     StillDefend,
//     Effects_1,
//     Effects_2,
//     Effects_3,
//     Effects_4,
//     MAX,
// }

//装备Info.
public class EquipInfo:IDataBase
{
    public int EquipId { get; set; }
    public int EquipType { get; set; }
    public string EquipSlot { get; set; }
    public int SubType { get; set; }
    public int ModelRes { get; set; }                                    //对应表中的ModelRes列
    public int IconRes { get; set; }


    public string EquipName { get; set; }
    public int StrengthenMoney { get; set; }
    public string EquipDesc { get; set; }
    public int Profession { get; set; }                                     //职业
    public int SalePrice { get; set; }
    public int TradePrice { get; set; }
    public int EqRank { get; set; }                                         //装备品质（白，绿，蓝，红，紫）
    public int MaxLevel { get; set; }
    public float AttackSpeed { get; set; }
    public int HPMax { get; set; }
    public int MPMax { get; set; }
    public int SPMax { get; set; }
    public int PhyAttack { get; set; }
    public int MagAttack { get; set; }
    public int PhyDefend { get; set; }
    public int MagDefend { get; set; }
    public int Hit { get; set; }
    public int Avoid { get; set; }
    public int CriAttack { get; set; }                                      //暴击
    public int StillAttack { get; set; }
    public int StillDefend { get; set; }
    public int Effects_1 { get; set; }
    public int Effects_2 { get; set; }
    public int Effects_3 { get; set; }
    public int Effects_4 { get; set; }
    public int Count { get; set; }
    public int Buff1 { get; set; }
    public int Buff2 { get; set; }
    public int Buff3 { get; set; }
}

//装备Info表.   
public class EquipTable
{
    protected byte[] m_equipTable;
    public static int equipInfoId;
	public int m_EquipInfoId = 0;
	static List<string> prefabs = new List<string>();
    public SortedList<int,EquipInfo> EquipInfoList; // 保存所有INFO
    public EquipInfo[] EquipInfoArray;

	//Dictionary<int, EquipInfo> EquipInfoTable /*= new Dictionary<int, EquipInfo>()*/;
    //==================================================================================
    protected byte[] m_equipData;
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        EquipInfoList = new SortedList<int, EquipInfo>(length);
        for (int index = 0; index < length; ++index)
        {
            EquipInfo info = new EquipInfo();
            info.Load(helper);
            EquipInfoList.Add(info.EquipId, info);
        }
    }

//     public void Load(byte[] bytes)
//     {
//         BinaryHelper helper = new BinaryHelper(bytes);
//         EquipInfoTable = new Dictionary<int, EquipInfo>();
//         int equipInfoCount = helper.ReadInt();
//         for (int index = 0; index < equipInfoCount ; index++)
//         {
//         	EquipInfo  equipInfo = new EquipInfo();
//             equipInfo.EquipId = helper.ReadInt();
//             int equipInfoListCount = helper.ReadInt();
//             for (int index2 = 0; index2 < equipInfoListCount;index2++ )
//             {
//                 //装备ID
//                 equipInfo.EquipId = helper.ReadInt();
//                 //装备类型
//                 equipInfo.EquipSlot = (ENEquipSlot)helper.ReadInt();
//                 //
//                 equipInfo.SubType = helper.ReadInt();
//                 
//                 equipInfo.ModelRes =  helper.ReadInt();
//                 equipInfo.IconRes = helper.ReadString();
//                 //装备名称
//                 equipInfo.EquipName = helper.ReadString();
//                 equipInfo.StrengthenMoney = helper.ReadInt();
//                 equipInfo.EquipDesc = helper.ReadString();
//                 //职业
//                 equipInfo.Profession = helper.ReadInt();
//                 //出售价格
//                 equipInfo.SalePrice = helper.ReadInt();
//                 //交易价格
//                 equipInfo.TradePrice = helper.ReadInt();
//                 //
//                 equipInfo.EqRank = helper.ReadInt();
//                 //最大等级
//                 equipInfo.MaxLevel = helper.ReadInt();
//                 //攻击速度
//                 equipInfo.AttackSpeed = helper.ReadFloat();
//                 //HP
//                 equipInfo.HPMax = helper.ReadInt();
//                 //MP
//                 equipInfo.MPMax = helper.ReadInt();
//                 //SP
//                 equipInfo.SPMax = helper.ReadInt();
//                 //物理攻击
//                 equipInfo.PhyAttack = helper.ReadInt();
//                 //魔法攻击
//                 equipInfo.MagAttack = helper.ReadInt();
//                 //物理防御
//                 equipInfo.PhyDefend = helper.ReadInt();
//                 //魔法防御
//                 equipInfo.MagDefend = helper.ReadInt();
//                 //命中
//                 equipInfo.Hit = helper.ReadInt();
//                 //闪避
//                 equipInfo.Avoid = helper.ReadInt();
//                 //暴击
//                 equipInfo.CriAttack = helper.ReadInt();
//                 //
//                 equipInfo.StillAttack = helper.ReadInt();
//                 //
//                 equipInfo.StillDefend = helper.ReadInt();
//                 //特效1
//                 equipInfo.Effects_1 = helper.ReadInt();
//                 //特效2
//                 equipInfo.Effects_2 = helper.ReadInt();
//                 //特效3
//                 equipInfo.Effects_3 = helper.ReadInt();
//                 //特效4
//                 equipInfo.Effects_4 = helper.ReadInt();
//                 //BUFF数量
//                 equipInfo.Count = helper.ReadInt();
//                 equipInfo.Buff1 = helper.ReadInt();
//                 equipInfo.Buff2 = helper.ReadInt();
//                 equipInfo.Buff3 = helper.ReadInt();
//             }
//             EquipInfoTable.Add(equipInfo.EquipId, equipInfo);
//         }
//     }
    //==================================================================================

    //测试用..给table添加一些数据
    public void initEquipInfo(int equipCount)
    {
        prefabs.Add("Orc Bracers");
        prefabs.Add("Orc Shoulders");
/*        GameTable.GetEquipTable();*/
//         for (int index = 101000; index < 101000+equipCount; index++)
//         {
//             ENEquipSlot part = (ENEquipSlot)Random.Range(0, (int)ENEquipSlot.MAX);
//             int prefabIndex = Random.Range(0, prefabs.Count);
// 
//             EquipInfo equip = new EquipInfo();
//             equip.EquipId = equipInfoId++;
// 
//             equip.EquipSlot = (ENEquipSlot)part;
//             equip.EquipName = ((ENEquipSlot)part).ToString();
//             equip.EquipPrefabName = prefabs[prefabIndex];
// 
//             EquipInfoTable.Add(equipInfoId, equip);
//         }
    }

	public EquipInfo LookUpEquipInfoById(int equipId)
	{
        EquipInfo info;
        EquipInfoList.TryGetValue(equipId, out info);
        return info;
	}

	//public EquipInfo getRandomEquipInfo()
	//{

	//	int rand = Random.Range(0, 4);

	//	if (rand == 0)
	//	{
	//		int randVal = Random.Range(402101, 402103);
	//		if (EquipInfoTable.ContainsKey(randVal))
	//		{
	//			return EquipInfoTable[randVal];
	//		}
	//	}


	//	else if (rand == 1)
	//	{
	//		int randVal1 = Random.Range(302101, 302103);
	//		if (EquipInfoTable.ContainsKey(randVal1))
	//		{
	//			return EquipInfoTable[randVal1];
	//		}
	//	}
	//	else if (rand == 2)
	//	{
	//		int randVal2 = Random.Range(202101, 202103);
	//		if (EquipInfoTable.ContainsKey(randVal2))
	//		{
	//			return EquipInfoTable[randVal2];
	//		}
	//	}
	//	else if (rand == 3)
	//	{
	//		int randVal3 = Random.Range(103101, 103103);
	//		if (EquipInfoTable.ContainsKey(randVal3))
	//		{
	//			return EquipInfoTable[randVal3];
	//		}
	//	}
	//	return null;
	//}
}
