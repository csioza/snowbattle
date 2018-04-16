using System;

namespace NGE.Algorithm
{
	/// <summary>
	/// 自动对象列表,为了对象的快速遍历
	/// </summary>
	public class AutoObjectList
	{
		private object[] m_array;
		private int m_maxid;
		private int m_size;

		public int MaxID{
			get{
				return m_maxid;
			}
		}

		public int Size{
			get{
				return m_size;
			}
		}
		public AutoObjectList(int capacity)
		{
			m_array = new object[capacity];
			m_maxid = capacity;
		}

		public void Add(object obj)
		{
			m_array[m_size++] = obj;
		}

		public void Remove(object obj)
		{
			for(int i=0; i< m_size; i++)
			{
				if(object.ReferenceEquals(obj,m_array[i]))
				{
					m_array[i] = null;
					if(i < m_size -1)
					{
						m_array[i] = m_array[m_size -1];
					}
					m_size --;
					return;

				}
			}
		}

		public void RemoveAt(int index)
		{
			if(index == m_size -1)
			{
				m_array[m_size -1 ] = null;
				m_size --;
			}
			else if(index < m_size -1)
			{
				m_array[index] = m_array[m_size -1];
				m_array[m_size - 1] = null;
				m_size--;
			}
		}

		public void Clear()
		{
			m_size = 0;
			for(int i=0; i< m_size; i++)
				m_array[i] = null;
		}
	}
}
