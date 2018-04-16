using System;
using System.Collections.Generic;
using System.Text;

namespace NGE.Algorithm
{
    /// <summary>
    /// 循环id列表,id从1开始到maxid,下一个id为maxid++
    /// 0为无效
    /// </summary>
    public class CycIDList
    {
        Queue<int> m_freeIDqueue = new Queue<int>();
        int[] m_array;

        public CycIDList(int maxid)
		{
            m_array = new int[maxid + 1];
            for (int i = 1; i <= maxid; i++)
                m_freeIDqueue.Enqueue(i);
		}

		public int GetNext()
		{
            if (m_freeIDqueue.Count == 0)
                return 0;
            int id = m_freeIDqueue.Dequeue();
            m_array[id] = 1;//1表示使用了
            return id;
		}

		public void Remove(int id)
		{
            if (m_array[id] == 0)
                return;
            m_array[id] = 0;
            m_freeIDqueue.Enqueue(id);
		}
    }
}
