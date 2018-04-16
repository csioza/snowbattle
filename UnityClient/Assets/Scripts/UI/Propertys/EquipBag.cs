using System;
using System.Collections.Generic;

// 装备背包
public class EquipBag : IPropertyObject  
{
    // 装备背包容量
    public int m_equipCapacity          = 0;

    // 材料背包容量
    public int m_materialCapacity       = 0;

    // 装备背包信息
    public  BagInfo m_equipBagInfo      = null;

    // 材料背包信息
    public  BagInfo m_materialBagInfo   = null;

    // 装备背包的一行
    public int m_equipRow               = 0;

    // 材料背包的一行
    public int m_materialRow            = 0;

    public int m_type                   = (int)enType.enEquipType;

    // 背包中实际装备列表 slotID,Equip
    public Dictionary<int, Equip> m_equipList = null;

    // 背包中材料列表slotID,itemID
    public Dictionary<int, int> m_materialList  = null;

    public Dictionary<int, Equip> m_sortedEquipList = null;

    public enum ENPropertyChanged
    {
      enShowEquipBag = 1,
      enUpdate       = 2,
    }



    public enum enType
    {
        enEquipType,
        enMaterialType,
    }

    public EquipBag()
    {
        SetPropertyObjectID((int)MVCPropertyID.enEquipBag);

        m_equipList         = new Dictionary<int, Equip>();
        m_materialList      = new Dictionary<int, int>();

        m_sortedEquipList   = new Dictionary<int, Equip>();

        WorldParamInfo worldParamInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enEquipBagID);
        if (null == worldParamInfo)
        {
            return;
        }

        BagInfo bagInfo = GameTable.BagTableAsset.LookUp(worldParamInfo.IntTypeValue);

        if (null == bagInfo)
        {
            return;
        }

        m_equipBagInfo  = bagInfo;

        m_equipCapacity = bagInfo.m_initalSize;

        worldParamInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enMaterialBagID);
        if (null == worldParamInfo)
        {
            return;
        }

        bagInfo = GameTable.BagTableAsset.LookUp(worldParamInfo.IntTypeValue);

        if (null == bagInfo)
        {
            return;
        }

        m_materialBagInfo   = bagInfo;

        m_materialCapacity  = bagInfo.m_initalSize;
       
    }

    #region Singleton
    static EquipBag m_singleton;
    static public EquipBag Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new EquipBag();
            }
            return m_singleton;
        }
    }
    #endregion

    public void OnShowEquipBag()
    {

        NotifyChanged((int)ENPropertyChanged.enShowEquipBag, null);
    }

    public void RandomAddCard()
    {
        int itemCounts  = 0;
        int capacity    = 0;

        int index       = 0;
        int itemID      = 0;

        if (m_type == (int)enType.enEquipType)
        {
            capacity    = m_equipCapacity;
            itemCounts  = m_equipList.Count;

            index = UnityEngine.Random.Range(0, GameTable.EquipmentTableAsset.m_map.Count);
            int temp = 0;
            foreach (int key in GameTable.EquipmentTableAsset.m_map.Keys)
            {
                if (temp == index)
                {
                    itemID = key;
                }
                temp++; 
            }
        }
        else if (m_type == (int)enType.enMaterialType)
        {
            capacity    = m_materialCapacity;
            itemCounts  = m_materialList.Count;

            index = UnityEngine.Random.Range(0, GameTable.ItemTableAsset.m_map.Count);
            int temp = 0;
            foreach (int key in GameTable.ItemTableAsset.m_map.Keys)
            {
                if (index == temp)
                {
                    itemID = key;
                }
                temp++; 
            }
        }

        if (itemCounts >= capacity)
        {
            return;
        }

        UnityEngine.Debug.Log("RandomAddCard itemID:" + itemID);


        if (m_type == (int)enType.enEquipType)
        {
            AddEquip(m_equipList.Count + 1, itemID);
            
        }
        else if (m_type == (int)enType.enMaterialType)
        {
            AddMaterial(m_materialList.Count + 1, itemID);
        }


        NotifyChanged((int)ENPropertyChanged.enUpdate, null);
    }


    public void AddEquip(int slotId,int equipId)
    {

        if (m_equipList.ContainsKey(slotId))
        {
            return;
        }

        Equip equip     = new Equip();
        equip.m_id      = equipId;
        equip.m_slotId  = slotId;
        equip.m_gotTime = UnityEngine.Time.time;

        m_equipList.Add(slotId, equip);

        // 更新排序列表
        m_sortedEquipList.Clear();

        foreach (KeyValuePair<int, Equip> item in m_equipList)
        {
            m_sortedEquipList.Add(item.Key, item.Value);
        }

    }

    public void RemoveEquip(int slotId)
    {
        if (false == m_equipList.ContainsKey(slotId))
        {
            return;
        }
        m_equipList.Remove(slotId);
    }

    public void AddMaterial(int slotId, int itemId)
    {
        if (m_materialList.ContainsKey(slotId))
        {
            return;
        }
        m_materialList.Add(slotId, itemId);
    }

    public void RemoveMaterial(int slotId)
    {
        if (false == m_materialList.ContainsKey(slotId))
        {
            return;
        }
        m_materialList.Remove(slotId);
    }


    // 根据生命值 来排序 down 是否降序
    public void SortByHp(bool down)
    {
        // int = HP
        List<int> temp = new List<int>();

        // int = HP
        Dictionary<int, List<Equip>> dic = new Dictionary<int, List<Equip>>();

        foreach (KeyValuePair<int, Equip> item in m_equipList)
        {
            EquipmentInfo equipmentInfo = GameTable.EquipmentTableAsset.LookUp(item.Value.m_id);

            // 筛选条件

            int hp = equipmentInfo.m_fHpMax_E;
            if (dic.ContainsKey(hp))
            {
                dic[hp].Add(item.Value);
            }
            else
            {
                List<Equip> list = new List<Equip>();
                list.Add(item.Value);
                dic.Add(hp, list);
                temp.Add(hp);
            }

        }

        temp.Sort();

        // 降序
        if (down)
        {
            temp.Reverse();
        }

        m_sortedEquipList.Clear();

        foreach (int hp in temp)
        {
            dic[hp].Sort(new EquipIdCompare());

            foreach (Equip item in dic[hp])
            {
                m_sortedEquipList.Add(item.m_slotId, item);
            }

        }

        NotifyChanged((int)ENPropertyChanged.enUpdate, null);
    }

    // 根据部位 来排序 down 是否降序
    public void SortBySubType(bool down)
    {
        // int = 部位ID
        List<int> temp = new List<int>();

        // int = 部位ID
        Dictionary<int, List<Equip>> dic = new Dictionary<int, List<Equip>>();

        foreach (KeyValuePair<int, Equip> item in m_equipList)
        {
            EquipmentInfo equipmentInfo = GameTable.EquipmentTableAsset.LookUp(item.Value.m_id);

            // 筛选条件

            int hp = equipmentInfo.m_subType;
            if (dic.ContainsKey(hp))
            {
                dic[hp].Add(item.Value);
            }
            else
            {
                List<Equip> list = new List<Equip>();
                list.Add(item.Value);
                dic.Add(hp, list);
                temp.Add(hp);
            }

        }

        temp.Sort();

        // 降序
        if (down)
        {
            temp.Reverse();
        }

        m_sortedEquipList.Clear();

        foreach (int hp in temp)
        {
            dic[hp].Sort(new EquipIdCompare());

            foreach (Equip item in dic[hp])
            {
                m_sortedEquipList.Add(item.m_slotId, item);
            }

        }

        NotifyChanged((int)ENPropertyChanged.enUpdate, null);
    }

    // 根据等级 来排序 down 是否降序
    public void SortByLevel(bool down)
    {
        // int = 等级
        List<int> temp = new List<int>();

        // int = 等级
        Dictionary<int, List<Equip>> dic = new Dictionary<int, List<Equip>>();

        foreach (KeyValuePair<int, Equip> item in m_equipList)
        {
            EquipmentInfo equipmentInfo = GameTable.EquipmentTableAsset.LookUp(item.Value.m_id);

            // 筛选条件

            int hp = equipmentInfo.m_levelMax;
            if (dic.ContainsKey(hp))
            {
                dic[hp].Add(item.Value);
            }
            else
            {
                List<Equip> list = new List<Equip>();
                list.Add(item.Value);
                dic.Add(hp, list);
                temp.Add(hp);
            }

        }

        temp.Sort();

        // 降序
        if (down)
        {
            temp.Reverse();
        }

        m_sortedEquipList.Clear();

        foreach (int hp in temp)
        {
            dic[hp].Sort(new EquipIdCompare());

            foreach (Equip item in dic[hp])
            {
                m_sortedEquipList.Add(item.m_slotId, item);
            }

        }

        NotifyChanged((int)ENPropertyChanged.enUpdate, null);
    }

    // 根据稀有度 来排序 down 是否降序
    public void SortByRank(bool down)
    {
        // int = 稀有度
        List<int> temp = new List<int>();

        // int = 稀有度
        Dictionary<int, List<Equip>> dic = new Dictionary<int, List<Equip>>();

        foreach (KeyValuePair<int, Equip> item in m_equipList)
        {
            EquipmentInfo equipmentInfo = GameTable.EquipmentTableAsset.LookUp(item.Value.m_id);

            // 筛选条件

            int hp = equipmentInfo.m_rank;
            if (dic.ContainsKey(hp))
            {
                dic[hp].Add(item.Value);
            }
            else
            {
                List<Equip> list = new List<Equip>();
                list.Add(item.Value);
                dic.Add(hp, list);
                temp.Add(hp);
            }

        }

        temp.Sort();

        // 降序
        if (down)
        {
            temp.Reverse();
        }

        m_sortedEquipList.Clear();

        foreach (int hp in temp)
        {
            dic[hp].Sort(new EquipIdCompare());

            foreach (Equip item in dic[hp])
            {
                m_sortedEquipList.Add(item.m_slotId, item);
            }

        }

        NotifyChanged((int)ENPropertyChanged.enUpdate, null);
    }

    // 根据物理攻击力 来排序 down 是否降序
    public void SortByPhyAttack(bool down)
    {
        // int = 物理攻击力
        List<int> temp = new List<int>();

        // int = 物理攻击力
        Dictionary<int, List<Equip>> dic = new Dictionary<int, List<Equip>>();

        foreach (KeyValuePair<int, Equip> item in m_equipList)
        {
            EquipmentInfo equipmentInfo = GameTable.EquipmentTableAsset.LookUp(item.Value.m_id);

            // 筛选条件

            int hp = equipmentInfo.m_fPhyAttack_E;
            if (dic.ContainsKey(hp))
            {
                dic[hp].Add(item.Value);
            }
            else
            {
                List<Equip> list = new List<Equip>();
                list.Add(item.Value);
                dic.Add(hp, list);
                temp.Add(hp);
            }

        }

        temp.Sort();

        // 降序
        if (down)
        {
            temp.Reverse();
        }

        m_sortedEquipList.Clear();

        foreach (int hp in temp)
        {
            dic[hp].Sort(new EquipIdCompare());

            foreach (Equip item in dic[hp])
            {
                m_sortedEquipList.Add(item.m_slotId, item);
            }

        }

        NotifyChanged((int)ENPropertyChanged.enUpdate, null);
    }

    // 根据魔法攻击力 来排序 down 是否降序
    public void SortByMagAttack(bool down)
    {
        // int = 魔法攻击力
        List<int> temp = new List<int>();

        // int = 魔法攻击力
        Dictionary<int, List<Equip>> dic = new Dictionary<int, List<Equip>>();

        foreach (KeyValuePair<int, Equip> item in m_equipList)
        {
            EquipmentInfo equipmentInfo = GameTable.EquipmentTableAsset.LookUp(item.Value.m_id);

            // 筛选条件

            int hp = equipmentInfo.m_fMagAttack_E;
            if (dic.ContainsKey(hp))
            {
                dic[hp].Add(item.Value);
            }
            else
            {
                List<Equip> list = new List<Equip>();
                list.Add(item.Value);
                dic.Add(hp, list);
                temp.Add(hp);
            }

        }

        temp.Sort();

        // 降序
        if (down)
        {
            temp.Reverse();
        }

        m_sortedEquipList.Clear();

        foreach (int hp in temp)
        {
            dic[hp].Sort(new EquipIdCompare());

            foreach (Equip item in dic[hp])
            {
                m_sortedEquipList.Add(item.m_slotId, item);
            }

        }

        NotifyChanged((int)ENPropertyChanged.enUpdate, null);
    }

    // 根据攻击速度 来排序 down 是否降序
    public void SortByAttackSpeed(bool down)
    {
        // float = 攻击速度
        List<float> temp = new List<float>();

        // float = 攻击速度
        Dictionary<float, List<Equip>> dic = new Dictionary<float, List<Equip>>();

        foreach (KeyValuePair<int, Equip> item in m_equipList)
        {
            EquipmentInfo equipmentInfo = GameTable.EquipmentTableAsset.LookUp(item.Value.m_id);

            // 筛选条件

            float hp = equipmentInfo.m_fAttackSpeed_E;
            if (dic.ContainsKey(hp))
            {
                dic[hp].Add(item.Value);
            }
            else
            {
                List<Equip> list = new List<Equip>();
                list.Add(item.Value);
                dic.Add(hp, list);
                temp.Add(hp);
            }

        }

        temp.Sort();

        // 降序
        if (down)
        {
            temp.Reverse();
        }

        m_sortedEquipList.Clear();

        foreach (float hp in temp)
        {
            dic[hp].Sort(new EquipIdCompare());

            foreach (Equip item in dic[hp])
            {
                m_sortedEquipList.Add(item.m_slotId, item);
            }

        }

        NotifyChanged((int)ENPropertyChanged.enUpdate, null);
    }

    // 根据物理防御力 来排序 down 是否降序
    public void SortByPhyDefence(bool down)
    {
        // int = 物理防御力 
        List<int> temp = new List<int>();

        // int = 物理防御力 
        Dictionary<int, List<Equip>> dic = new Dictionary<int, List<Equip>>();

        foreach (KeyValuePair<int, Equip> item in m_equipList)
        {
            EquipmentInfo equipmentInfo = GameTable.EquipmentTableAsset.LookUp(item.Value.m_id);

            // 筛选条件

            int hp = equipmentInfo.m_fPhyDefend_E;
            if (dic.ContainsKey(hp))
            {
                dic[hp].Add(item.Value);
            }
            else
            {
                List<Equip> list = new List<Equip>();
                list.Add(item.Value);
                dic.Add(hp, list);
                temp.Add(hp);
            }

        }

        temp.Sort();

        // 降序
        if (down)
        {
            temp.Reverse();
        }

        m_sortedEquipList.Clear();

        foreach (int hp in temp)
        {
            dic[hp].Sort(new EquipIdCompare());

            foreach (Equip item in dic[hp])
            {
                m_sortedEquipList.Add(item.m_slotId, item);
            }

        }

        NotifyChanged((int)ENPropertyChanged.enUpdate, null);
    }

    // 根据魔法防御力 来排序 down 是否降序
    public void SortByMagDefence(bool down)
    {
        // int = 魔法防御力 
        List<int> temp = new List<int>();

        // int = 魔法防御力 
        Dictionary<int, List<Equip>> dic = new Dictionary<int, List<Equip>>();

        foreach (KeyValuePair<int, Equip> item in m_equipList)
        {
            EquipmentInfo equipmentInfo = GameTable.EquipmentTableAsset.LookUp(item.Value.m_id);

            // 筛选条件

            int hp = equipmentInfo.m_fMagDefend_E;
            if (dic.ContainsKey(hp))
            {
                dic[hp].Add(item.Value);
            }
            else
            {
                List<Equip> list = new List<Equip>();
                list.Add(item.Value);
                dic.Add(hp, list);
                temp.Add(hp);
            }

        }

        temp.Sort();

        // 降序
        if (down)
        {
            temp.Reverse();
        }

        m_sortedEquipList.Clear();

        foreach (int hp in temp)
        {
            dic[hp].Sort(new EquipIdCompare());

            foreach (Equip item in dic[hp])
            {
                m_sortedEquipList.Add(item.m_slotId, item);
            }

        }

        NotifyChanged((int)ENPropertyChanged.enUpdate, null);
    }


    // 根据入手时间 来排序 down 是否降序
    public void SortByGotTime(bool down)
    {
        // float = 入手时间
        List<float> temp = new List<float>();

        // float = 入手时间
        Dictionary<float, List<Equip>> dic = new Dictionary<float, List<Equip>>();

        foreach (KeyValuePair<int, Equip> item in m_equipList)
        {
//            EquipmentInfo equipmentInfo = GameTable.EquipmentTableAsset.LookUp(item.Value.m_id);

            // 筛选条件

            float time = item.Value.m_gotTime;
            if (dic.ContainsKey(time))
            {
                dic[time].Add(item.Value);
            }
            else
            {
                List<Equip> cardList = new List<Equip>();
                cardList.Add(item.Value);
                dic.Add(time, cardList);
                temp.Add(time);
            }

        }

        temp.Sort();

        // 降序
        if (down)
        {
            temp.Reverse();
        }
        m_sortedEquipList.Clear();

        foreach (float time in temp)
        {
            dic[time].Sort(new EquipIdCompare());

            foreach (Equip item in dic[time])
            {
                m_sortedEquipList.Add(item.m_slotId, item);
            }

        }

        NotifyChanged((int)ENPropertyChanged.enUpdate, null);
    }
}
public class EquipIdCompare : IComparer<Equip>
{
    //按ID排序
    public int Compare(Equip x, Equip y)
    {
        return x.m_id.CompareTo(y.m_id);
    }
}