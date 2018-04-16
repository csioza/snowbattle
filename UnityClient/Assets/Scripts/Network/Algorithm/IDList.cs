using System;

namespace NGE.Algorithm
{
	/// <summary>
	/// id列表,id从1开始到maxid,下一个id为空闲的最小id
    /// 0为无效
	/// </summary>
	public class IDList
	{
		private byte[] m_array;

		public IDList(int maxid)
		{
			m_array = new byte[maxid];
		}

		public int GetNext()
		{
			for(int i=0;i<m_array.Length; i++)
			{
				if(m_array[i] == 0)
				{
					m_array[i] = 1;
					return i+1;
				}
			}
			return 0;
		}

		public void Remove(int id)
		{
			if(id <= m_array.Length && id > 0)
				m_array[id -1] = 0;
		}
	}
}
