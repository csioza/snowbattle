﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NGE.Network
{
	/// <summary>
	/// 内存块缓冲池
	/// </summary>
	internal sealed class BufferPool
	{
		private int m_BufferSize;
		private int m_InitialCapacity;
		private int m_Misses;
		private Queue<byte[]> m_FreeBuffers;
		private string m_Name;
		private static List<BufferPool> m_Pools;

		public static List<BufferPool> Pools
		{
			get
			{
				return m_Pools;
			}
			set
			{
				m_Pools = value;
			}
		}

		static BufferPool()
		{
			m_Pools = new List<BufferPool>();
		}

		public BufferPool(string name, int initialCapacity, int bufferSize)
		{
			m_Name = name;
			m_InitialCapacity = initialCapacity;
			m_BufferSize = bufferSize;
			m_FreeBuffers = new Queue<byte[]>(initialCapacity);

			for (int i = 0; i < initialCapacity; ++i)
				m_FreeBuffers.Enqueue(new byte[bufferSize]);

			lock (m_Pools)
				m_Pools.Add(this);
		}

		public byte[] Acquire()
		{
			byte[] buffer;

			lock (this)
			{
				if (m_FreeBuffers.Count > 0)
					return m_FreeBuffers.Dequeue();

				m_Misses++;

				for (int i = 0; i < m_InitialCapacity; i++)
					m_FreeBuffers.Enqueue(new byte[m_BufferSize]);

				buffer = m_FreeBuffers.Dequeue();
			}

			return buffer;
		}

		public void Free()
		{
			lock (m_Pools)
				m_Pools.Remove(this);
		}

		public void GetInfo(out string name, out int freeCount, out int initialCapacity, out int currentCapacity, out int bufferSize, out int misses)
		{
			lock (this)
			{
				name = m_Name;
				freeCount = m_FreeBuffers.Count;
				initialCapacity = m_InitialCapacity;
				currentCapacity = m_InitialCapacity * (1 + m_Misses);
				bufferSize = m_BufferSize;
				misses = m_Misses;
			}
		}

		//回收一个兼容的回收buffer.
		public void Reclaim(byte[] buffer)
		{
			if (buffer == null || buffer.Length != m_BufferSize)
				return;

			try
			{
				lock (this)
				{
					if (!m_FreeBuffers.Contains(buffer))//----同一个buffer不能加入两次，否则可能同一个buffer被两个使用者同时使用。
						m_FreeBuffers.Enqueue(buffer);
				}
			}
			catch
			{
				Console.WriteLine("release buffer error !");
			}

		}
	}
}
