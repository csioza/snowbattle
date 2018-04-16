using System;
using System.Collections.Generic;

// 队伍单元
public class TeamItem   
{
    public CSItemGuid[] m_memberList = new CSItemGuid[4];

    public int m_index = 0;

    public TeamItem()
    {
        m_memberList.Initialize();
    }

    // 此队伍是否为空队伍
    public bool IsEmpty()
    {
        bool isEmpty = true;

        for (int i = 0; i < m_memberList.Length; i++)
        {
            if (!m_memberList[i].Equals(CSItemGuid.Zero))
            {
                isEmpty = false;
                break;
            }
        }
        return isEmpty;
    }
}
