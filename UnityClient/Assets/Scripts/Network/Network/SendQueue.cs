using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NGE.Network {
	/// <summary>
	/// 发送消息队列,内部使用
	/// </summary>
	public sealed class SendQueue {
		public SendQueue() {
			m_Queue = new Queue<Entry>();
		}
		
		//一个Buffer和它的使用情况
		private class Entry {
			public byte[] Buffer;
			public int BufferSize { get { return Buffer.Length; } }//缓存总大小 
			public int Costed;//已用 
			public int Untapped { get { return Buffer.Length - Costed; } }//未用

			public Entry(byte[] buffer, int length) {
				Buffer = buffer;
				Costed = length;
			}
 		}

		//缓冲块池的大小
		private static int m_CoalesceBufferSize = 32<<10 ;
		//一个内存块的池，每个内存块大小为32<<10,此池供所有的SendQueue使用。
		private static BufferPool s_BufferPool = new BufferPool("SendQueue", 2, m_CoalesceBufferSize);

		public static int CoalesceBufferSize {
			get { return m_CoalesceBufferSize; }
			set {
				if (m_CoalesceBufferSize == value)
					return;

				if (s_BufferPool != null)
					s_BufferPool.Free();

				m_CoalesceBufferSize = value;
				s_BufferPool = new BufferPool("SendQueue", 2, m_CoalesceBufferSize);
			}
		}
  		private Queue<Entry> m_Queue;
		private Entry m_FirstEntry;
		private bool m_hasPeek = false;


		public void Clear() {
			lock (this) {
				if (m_FirstEntry != null) {
					s_BufferPool.Reclaim(m_FirstEntry.Buffer);
					m_FirstEntry = null;
				}
				while (m_Queue.Count > 0) {
					s_BufferPool.Reclaim(m_Queue.Dequeue().Buffer);
				}
			}
		}


		//每次取出一个片段。
		public bool Dequeue(out byte[] data, out int length) {
			lock (this) {
				if (m_hasPeek) {//----清除上一次使用的Buffer
 					s_BufferPool.Reclaim(m_Queue.Dequeue().Buffer);
					m_hasPeek = false;
				}
				if (m_Queue.Count > 0) {//返回m_Queue中的数据
 					Entry entry = m_Queue.Peek();
					m_hasPeek = true;//设置已使用Buffer的标记，以待下一次根据它把这个Buffer清除，
					data = entry.Buffer;
					length = entry.Costed;
					NetworkPerformanceStatistics.Instance.nPacketCachePoped++;
					NetworkPerformanceStatistics.Instance.nCacheBlock--;
					return true;
				} else if (m_FirstEntry != null) {//返回m_Buffered中的数据。//消息大小还没有达到1个块大小
 					length = m_FirstEntry.Costed;
					data = m_FirstEntry.Buffer;
					m_FirstEntry = null;
					NetworkPerformanceStatistics.Instance.nPacketCachePoped++;
					return true;
				} else {//没有数据可以返回
					System.Diagnostics.Debug.Assert(m_FirstEntry == null);
					data = null;
					length = 0;
					return false;
				}
			}
		}

		/// <summary>
		/// 把一段数据放入m_Queue，
		/// 如果这段数据太长，则将其折成几段依次放入m_Queue。
		/// </summary>
		/// <returns></returns>
		public bool Enqueue(byte[] userData, int userDataLen) {
			System.Diagnostics.Debug.Assert(userData.Length >= userDataLen, "Too large userDataLen[" + userDataLen + "] for buffer[size=" + userData.Length + "]");
			NetworkPerformanceStatistics.Instance.nPacketCachePushed++;
			lock (this) {
				int userDataCopied = 0;
				while (userDataCopied < userDataLen) {
					if (m_FirstEntry == null)
						m_FirstEntry = new Entry(s_BufferPool.Acquire(), 0); //Entry.Pool();
					int copySize = Math.Min(m_FirstEntry.Untapped, userDataLen - userDataCopied);
					Buffer.BlockCopy(userData, userDataCopied, m_FirstEntry.Buffer, m_FirstEntry.Costed, copySize);
					m_FirstEntry.Costed += copySize;
					userDataCopied += copySize;
					if (m_FirstEntry.Untapped == 0){//full
 						m_Queue.Enqueue(m_FirstEntry);
						m_FirstEntry = null;
						NetworkPerformanceStatistics.Instance.nCacheBlock++;
					}
				}
				return m_Queue.Count == 0;
			}
 		}
	}
}
