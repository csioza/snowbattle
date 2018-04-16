using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



//装备实例.
// public class EquipInst:CSOldItem
// {
//     public EquipInfo EquipInfo { get; set; }    //equipInfo指针
//     public CSItemGuid EquipInstID { get; set; }        //装备实例ID
// /*    public CSItemGuid EquipInstItemID { get; set; }*/
//     //==============
//     public int exp;
//     public int Level;                           //装备等级
//     public int m_aptitude;                   //装备资质ID
//     public float m_aptitudePer;                 //资质随即百分比
//     public bool m_isBind;
//     public int m_addPropData1;
//     public int m_addPropData2;
//     public int m_addPropData3;
//     public int m_addPropData4;
// 
// }
//装备属性(显示结构)
public class EquipInstProperty
{
    public string propertyName;
    public float value;
}

public enum ENEquipSlotToPlayerPart
{
    Weapen = 0,
    BodyArmor = 1,
    FootArmor = 2,
    HeadArmor = 3,
    Gloves = 4,
    Decoration = 5,
}
public class MyEquips
{
    #region Singleton
    static MyEquips m_singleton;
    static public MyEquips Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new MyEquips();
            }
            return m_singleton;
        }
    }
    #endregion
    public MyEquips()
    {
        EquipInstId.m_highPart = 1000;
        EquipInstId.m_lowPart = 1000;
    }
    static CSItemGuid EquipInstId;
    static Dictionary<CSItemGuid, CSOldItem> m_equips = new Dictionary<CSItemGuid, CSOldItem>();
    List<CSOldItem> m_showEquipListNow = new List<CSOldItem>();     //当前显示的列表
    public CSItemGuid[] dress = new CSItemGuid[5];
    static public int GetEquipCount() { return m_equips.Count; }
    //装备排序用
    static int CompareBySubUp(CSOldItem a, CSOldItem b)
    {
        EquipInfo infoA = GameTable.EquipTableAsset.LookUpEquipInfoById(a.m_id);
        EquipInfo infoB = GameTable.EquipTableAsset.LookUpEquipInfoById(b.m_id);
        if (infoA.SubType < infoB.SubType) return -1;
        if (infoA.SubType > infoB.SubType) return 1;
        return 0;
    }
    static int CompareBySubDown(CSOldItem a, CSOldItem b)
    {
        EquipInfo infoA = GameTable.EquipTableAsset.LookUpEquipInfoById(a.m_id);
        EquipInfo infoB = GameTable.EquipTableAsset.LookUpEquipInfoById(b.m_id);
        if (infoA.SubType < infoB.SubType) return 1;
        if (infoA.SubType > infoB.SubType) return -1;
        return 0;
    }
    static int CompareByLevelUp(CSOldItem a, CSOldItem b)
    {
        if (a.Level < b.Level) return -1;
        if (a.Level > b.Level) return 1;
        return 0;
    }

    static int CompareByLevelDown(CSOldItem a, CSOldItem b)
    {
        if (a.Level < b.Level) return 1;
        if (a.Level > b.Level) return -1;
        return 0;
    }
    public Dictionary<CSItemGuid, CSOldItem> myEquips
    {
        get
        {
            return m_equips;
        }
    }

    public CSOldItem getEquipByInstId(CSItemGuid id)
    {
        CSOldItem obj = null;
        if (m_equips.TryGetValue(id, out obj))
        {
            return obj;
        }
        else
        {
            return null;
        }
    }

    public void SetNewLevel(CSItemGuid id, int level, int exp, bool isBind)
    {
        foreach (KeyValuePair<CSItemGuid, CSOldItem> pair in m_equips)
        {
            if (pair.Value.m_guid == id)
            {
                pair.Value.Level = level;
                pair.Value.exp = exp;
                pair.Value.m_isBind = isBind;
            }
        }
    }

    public bool IsOnBody(CSItemGuid guid)
    {
        for (int index = 0; index < dress.Count();index++ )
        {
            if (guid == dress[index])
            {
                return true;
            }
        }
        return false;
    }
    //脱装备
    public void GetOff(CSItemGuid guid)
    {
        for (int index = 0; index < dress.Count(); index++)
        {
            if (guid == dress[index])
            {
                CSItemGuid zero;
                zero.m_lowPart = 0;
                zero.m_highPart = 0;
                dress[index] = zero;
            }
        }
    }
    //身上装备替换
    public void ChangePartEquip(CSItemGuid guid,ENEquipSlotToPlayerPart part)
    {
        int index = (int)part;
        dress[index] = guid;
    }

//	public CSOldItem addNewEquip()
//	{
//		EquipInfo equipInfo = GameTable.EquipTableAsset.getRandomEquipInfo();
//		if (equipInfo == null)
//		{
//			return null;
//		}
//		CSOldItem newEquip = new CSOldItem();
////         newEquip.EquipInfo = equipInfo;
//		CSItemGuid newID;
//		newID.m_highPart = EquipInstId.m_highPart;
//		newID.m_lowPart = EquipInstId.m_lowPart++;
//		newEquip.m_guid = newID;
//		newEquip.Level = UnityEngine.Random.Range(1, 10);
//		newEquip.m_aptitude = UnityEngine.Random.Range(1, 10);
//		newEquip.m_aptitudePer = UnityEngine.Random.Range(1, 100) / 100;
//		//添加到  m_equips
//		m_equips.Add(newEquip.m_guid, newEquip);

//		return newEquip;
//	}

    public bool DelEquipById(CSItemGuid instId)
    {
        return m_equips.Remove(instId);
    }


    public void getEquipsBySlot(string slot, List<CSOldItem> list)
    {
        Debug.Log("getEquipsBySlot");
        list.Clear();
        m_showEquipListNow.Clear();
        if (slot == ENEquipSlot.MAX)
        {
            foreach (KeyValuePair<CSItemGuid, CSOldItem> pair in m_equips)
            {
                list.Add(pair.Value);
                m_showEquipListNow.Add(pair.Value);
            }
        }
        else
        {
            //m_equips.Where(delegate(KeyValuePair<int, EquipInst> pair) { return pair.Value.EquipInfo.EquipSlot == slot; });
            foreach (KeyValuePair<CSItemGuid, CSOldItem> pair in m_equips)
            {
                EquipInfo info = GameTable.EquipTableAsset.LookUpEquipInfoById(pair.Value.m_id);
                if (info.EquipSlot == slot)
                {
                    list.Add(pair.Value);
                    m_showEquipListNow.Add(pair.Value);
                }
            }
        }
    }
    //装备强化用
    public void getEquipsBySlotInEquipStrengthen(string slot, List<CSOldItem> list,CSOldItem item)
    {
        Debug.Log("getEquipsBySlot");
        list.Clear();
        m_showEquipListNow.Clear();
        if (slot == ENEquipSlot.MAX)
        {
            foreach (KeyValuePair<CSItemGuid, CSOldItem> pair in m_equips)
            {
                if (pair.Value.m_guid == item.m_guid)
                {
                    continue;
                }
                list.Add(pair.Value);
                m_showEquipListNow.Add(pair.Value);
            }
        }
        else
        {
            //m_equips.Where(delegate(KeyValuePair<int, EquipInst> pair) { return pair.Value.EquipInfo.EquipSlot == slot; });
            foreach (KeyValuePair<CSItemGuid, CSOldItem> pair in m_equips)
            {
                EquipInfo info = GameTable.EquipTableAsset.LookUpEquipInfoById(pair.Value.m_id);
                if (pair.Value.m_guid == item.m_guid)
                {
                    continue;
                }
                if (info.EquipSlot == slot)
                {
                    list.Add(pair.Value);
                    m_showEquipListNow.Add(pair.Value);
                }
            }
        }
    }
    //新排序(品质)
    public void getEquipsByQuality(int index, List<CSOldItem> list)
    {
        list.Clear();

        if (index == 0)
        {
            if (m_showEquipListNow.Count == 0)
            {
                foreach (KeyValuePair<CSItemGuid, CSOldItem> pair in m_equips)
                {
                    list.Add(pair.Value);
                    list.Sort(CompareBySubUp);
                }
            }
            else
            {
                foreach (CSOldItem inst in m_showEquipListNow)
                {
                    list.Add(inst);
                    list.Sort(CompareBySubUp);
                }
            }
        }
        else
        {
            if (m_showEquipListNow.Count == 0)
            {
                foreach (KeyValuePair<CSItemGuid, CSOldItem> pair in m_equips)
                {
                    list.Add(pair.Value);
                    list.Sort(CompareBySubDown);
                }
            }
            else
            {
                foreach (CSOldItem inst in m_showEquipListNow)
                {
                    list.Add(inst);
                    list.Sort(CompareBySubDown);
                }
            }
        }
    }

    //新排序(等级)
    public void getEquipsByLevel(int index, List<CSOldItem> list)
    {
        list.Clear();

        if (index == 0)
        {
            if (m_showEquipListNow.Count == 0)
            {
                foreach (KeyValuePair<CSItemGuid, CSOldItem> pair in m_equips)
                {
                    list.Add(pair.Value);
                    list.Sort(CompareByLevelUp);
                }
            }
            else
            {
                foreach (CSOldItem inst in m_showEquipListNow)
                {
                    list.Add(inst);
                    list.Sort(CompareByLevelUp);
                }
            }
        }
        else
        {
            if (m_showEquipListNow.Count == 0)
            {
                foreach (KeyValuePair<CSItemGuid, CSOldItem> pair in m_equips)
                {
                    list.Add(pair.Value);
                    list.Sort(CompareByLevelDown);
                }
            }
            else
            {
                foreach (CSOldItem inst in m_showEquipListNow)
                {
                    list.Add(inst);
                    list.Sort(CompareByLevelDown);
                }
            }
        }
    }

    public void SendEquipChangeMsg(CSOldItem inst,int changeType)
    {
//        short equipSlotIndex = 0;
        //EquipInfo info = GameTable.EquipTableAsset.LookUpEquipInfoById(inst.m_id);
        //switch (info.EquipSlot)
        //{
        //    case ENEquipSlot.Weapon:
        //        equipSlotIndex = 0;
        //        break;
        //    case ENEquipSlot.BodyArmor:
        //        equipSlotIndex = 1;
        //        break;
        //    case ENEquipSlot.CloakArmor:
        //        equipSlotIndex = 2;
        //        break;
        //    case ENEquipSlot.HeadArmor:
        //        equipSlotIndex = 3;
        //        break;
        //    case ENEquipSlot.Gloves:
        //        equipSlotIndex = 4;
        //        break;
        //    case ENEquipSlot.Decoration:
        //        equipSlotIndex = 5;
        //        break;
        //}
        //向服务器请求换装
        //EquipChangePacket packet = new EquipChangePacket(equipSlotIndex, inst.m_guid, changeType);
        //ClientNet.Singleton.SendPacket(packet);
    }

}