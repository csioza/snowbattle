using System;
using System.Collections.Generic;

public class EquipmentInfo : IDataBase
{
    public int m_id { get; protected set; }
    public int m_type { get; protected set; }
    public string m_equipmentType { get; protected set; }
    public int m_subType { get; protected set; }
    public int m_modelId { get; protected set; }
    public int m_iconId { get; protected set; }
    public string m_equipName { get; protected set; }
    public int m_strengthenMoney { get; protected set; }
    public string m_desciption { get; protected set; }
    public int m_iprofession { get; protected set; }
    public int m_salePrice { get; protected set; }
    public int m_tradePrice { get; protected set; }
    public int m_rank { get; protected set; }
    public int m_levelMax { get; protected set; }

    public float m_fAttackSpeed_E { get; protected set; }
    public int m_fHpMax_E { get; protected set; }
    public int m_fPhyAttack_E { get; protected set; }
    public int m_fMagAttack_E { get; protected set; }
    public int m_fPhyDefend_E { get; protected set; }
    public int m_fMagDefend_E { get; protected set; }
}

public class EquipmentTable
{
    public Dictionary<int, EquipmentInfo> m_map { get; protected set; }
    public EquipmentInfo LookUp(int id)
    {
        EquipmentInfo info = null;
        if (m_map.TryGetValue(id, out info))
        {
            return info;
        }

        return null;
    }

    public void Load(byte[] bytes)
    {
        m_map = new Dictionary<int, EquipmentInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);

        int sceneCount      = helper.ReadInt();

        for (int index = 0; index < sceneCount; ++index)
        {
            EquipmentInfo info = new EquipmentInfo();
            
            info.Load(helper);

            m_map.Add(info.m_id, info);
        }
    }
}