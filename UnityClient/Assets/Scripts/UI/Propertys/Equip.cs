using System;
using System.Collections.Generic;

// 装备
public class Equip : IPropertyObject  
{
    // 表中ID 
    public int m_id;
    // 格子ID
    public int m_slotId;
 
    public int m_level = 1;
    public int m_exp;
    public float m_gotTime;// 入手时间

    public Equip()
    {
 
    }

    
}
