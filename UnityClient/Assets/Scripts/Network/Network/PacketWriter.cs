using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NGE.Network
{
	/// <summary>
	/// 消息包书写器
	/// </summary>
	public sealed class PacketWriter
	{
		/// <summary>
		/// 内存池
		/// </summary>
		private static Stack<PacketWriter> m_Pool = new Stack<PacketWriter>();

		public static PacketWriter CreateInstance()
		{
			return CreateInstance(64);
		}

		public static PacketWriter CreateInstance(int capacity)
		{
			PacketWriter pw = null;

			lock (m_Pool)
			{
				if (m_Pool.Count > 0)
				{
					pw = m_Pool.Pop();

					if (pw != null)
					{
						pw.m_Capacity = capacity;
						pw.m_Stream.SetLength(0);
					}
				}
			}

			if (pw == null)
				pw = new PacketWriter(capacity);

			return pw;
		}

		public static void ReleaseInstance(PacketWriter pw)
		{
			lock (m_Pool)
			{
				if (!m_Pool.Contains(pw))
				{
					m_Pool.Push(pw);
				}
				else
				{
					try
					{
						using (StreamWriter op = new StreamWriter("neterr.log"))
						{
							op.WriteLine("{0}\tInstance pool contains writer", DateTime.Now);
						}
					}
					catch
					{
						Console.WriteLine("net error");
					}
				}
			}
		}

		private MemoryStream m_Stream;

		private int m_Capacity;

		/// <summary>
		/// Internal format buffer.
		/// </summary>
		private byte[] m_Buffer = new byte[8];


		public PacketWriter()
			: this(256)
		{
		}

		/// <summary>
		/// 指定容量的构造器.
		/// </summary>
		/// <param name="capacity">初始容量大小.</param>
		public PacketWriter(int capacity)
		{
			m_Stream = new MemoryStream(capacity);
			m_Capacity = capacity;
		}

		/// <summary>
		/// 写一个1字节的bool值 (false 为 0, true 为 1).
		/// </summary>
		public void Write(bool value)
		{
			m_Stream.WriteByte((byte)(value ? 1 : 0));
		}

		/// <summary>
		/// 写一个无符号字节
		/// </summary>
		public void Write(byte value)
		{
			m_Stream.WriteByte(value);
		}

		/// <summary>
		/// 写一个有符号字节
		/// </summary>
		public void Write(sbyte value)
		{
			m_Stream.WriteByte((byte)value);
		}

		/// <summary>
		/// 写一个short.
		/// </summary>
		public void Write(short value)
		{
			m_Buffer[1] = (byte)(value >> 8);
			m_Buffer[0] = (byte)value;

			m_Stream.Write(m_Buffer, 0, 2);
		}

		/// <summary>
		/// 写一个无符号short.
		/// </summary>
		public void Write(ushort value)
		{
			m_Buffer[1] = (byte)(value >> 8);
			m_Buffer[0] = (byte)value;

			m_Stream.Write(m_Buffer, 0, 2);
		}

		/// <summary>
		/// 写一个32位int.
		/// </summary>
		public void Write(int value)
		{
			m_Buffer[3] = (byte)(value >> 24);
			m_Buffer[2] = (byte)(value >> 16);
			m_Buffer[1] = (byte)(value >> 8);
			m_Buffer[0] = (byte)value;

			m_Stream.Write(m_Buffer, 0, 4);
		}

		/// <summary>
		/// 写一个无符号32位int.
		/// </summary> 
		public void Write(uint value)
		{
			m_Buffer[3] = (byte)(value >> 24);
			m_Buffer[2] = (byte)(value >> 16);
			m_Buffer[1] = (byte)(value >> 8);
			m_Buffer[0] = (byte)value;

			m_Stream.Write(m_Buffer, 0, 4);
		}

		/// <summary>
		/// 写一个64位的int
		/// </summary>
		/// <param name="value"></param>
		public void Write(long value)
		{
			m_Buffer[7] = (byte)(value >> 56);
			m_Buffer[6] = (byte)(value >> 48);
			m_Buffer[5] = (byte)(value >> 40);
			m_Buffer[4] = (byte)(value >> 32);
			m_Buffer[3] = (byte)(value >> 24);
			m_Buffer[2] = (byte)(value >> 16);
			m_Buffer[1] = (byte)(value >> 8);
			m_Buffer[0] = (byte)value;

			m_Stream.Write(m_Buffer, 0, 8);
		}

		/// <summary>
		/// 写一个无符号64位int
		/// </summary>
		/// <param name="value"></param>
		public void Write(ulong value)
		{
			m_Buffer[7] = (byte)(value >> 56);
			m_Buffer[6] = (byte)(value >> 48);
			m_Buffer[5] = (byte)(value >> 40);
			m_Buffer[4] = (byte)(value >> 32);
			m_Buffer[3] = (byte)(value >> 24);
			m_Buffer[2] = (byte)(value >> 16);
			m_Buffer[1] = (byte)(value >> 8);
			m_Buffer[0] = (byte)value;

			m_Stream.Write(m_Buffer, 0, 8);
		}

        public void WriteCompressInt(int value)
        {
            if (value < 255)
            {
                Write((byte)value);
            }
            else
            {
                Write(value);
            }
        }

		/// <summary>
		/// 写浮点数，gtw
		/// </summary> 
		public void Write(float value)
		{
			m_Stream.Write(BitConverter.GetBytes(value), 0, 4);
		}


		/// <summary>
		/// 写浮点数，gtw
		/// </summary> 
		public void Write(double value)
		{
			m_Stream.Write(BitConverter.GetBytes(value), 0, 8);
		}


		/// <summary>
		/// 写一个数组
		/// </summary>
		public void Write(byte[] buffer, int offset, int size)
		{
			if (buffer == null || size <= 0)
				return;
			m_Stream.Write(buffer, offset, size);
		}

		/// <summary>
		/// 写一个数组
		/// </summary>
		public void Write(byte[] buffer, int size)
		{
			if (buffer == null || size <= 0)
				return;
			m_Stream.Write(buffer, 0, size);
		}

		/// <summary>
		/// 写一个数组
		/// </summary>
		public void Write(byte[] buffer)
		{
			if (buffer == null)
				return;
			m_Stream.Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// 写一个小字节序(little-endian) unicode 字符串,以null结尾.
		/// </summary>
		public void WriteString(string value)
		{
			if (value == null)//write empty string ""
			{
				value = String.Empty;
			}
            byte[] buffer = new byte[value.Length * 2];
            for (int i = 0; i < value.Length; i++)
            {
                ushort c = (ushort)value[i];
                buffer[i * 2] = (byte)(c & 0x00ff);
                buffer[i * 2 + 1] = (byte)((c >> 8) & 0x00ff);
            }
            //由于Encoding.Unicode.GetBytes会判断字符串的具体内容来改变返回的数组的值,所以不再使用,例如gb2312的'地'在Encoding.Unicode.GetBytes调用后,变成了乱码
            //byte[] buffer = Encoding.Unicode.GetBytes(value);

			m_Stream.Write(buffer, 0, buffer.Length);

			m_Buffer[0] = 0;
			m_Buffer[1] = 0;
			m_Stream.Write(m_Buffer, 0, 2);
		}

		/// <summary>
		/// 写一个指定长度 小字节序(little-endian) unicode 字符串,以null结尾.
		/// </summary>
		public void WriteString(string value, int size)
		{
			if (value == null)
			{
				value = String.Empty;
			}

			size *= 2;

			byte[] buffer = Encoding.Unicode.GetBytes(value);

			if (buffer.Length >= size)
			{
				m_Stream.Write(buffer, 0, size);
			}
			else
			{
				m_Stream.Write(buffer, 0, buffer.Length);
				Fill(size - buffer.Length);
			}
		}

		/// <summary>
		/// 写一个Ascii字符串.
		/// </summary>
		public void WriteAscii(string value)
		{
			if (value == null)
			{
				value = String.Empty;
			}

			byte[] buffer = Encoding.ASCII.GetBytes(value);

			m_Stream.Write(buffer, 0, buffer.Length);
			m_Stream.WriteByte(0);
		}

		/// <summary>
		/// 写一个指定长度的Ascii字符串.
		/// </summary>
		public void WriteAscii(string value, int size)
		{
			if (value == null)
			{
				value = String.Empty;
			}

			byte[] buffer = Encoding.ASCII.GetBytes(value);

			if (buffer.Length >= size)
			{
				m_Stream.Write(buffer, 0, size);
			}
			else
			{
				m_Stream.Write(buffer, 0, buffer.Length);
				Fill(size - buffer.Length);
			}
		}

        /// <summary>
        /// 写一个Ascii字符串.
        /// </summary>
        public void WriteUTF8(string value)
        {
            if (value == null)
            {
                value = String.Empty;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(value);

            m_Stream.Write(buffer, 0, buffer.Length);
            m_Stream.WriteByte(0);
        }

        /// <summary>
        /// 写一个指定长度的Ascii字符串.
        /// </summary>
        public void WriteUTF8(string value, int size)
        {
            if (value == null)
            {
                value = String.Empty;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(value);

            if (buffer.Length >= size)
            {
                m_Stream.Write(buffer, 0, size);
            }
            else
            {
                m_Stream.Write(buffer, 0, buffer.Length);
                Fill(size - buffer.Length);
            }
        }

		/// <summary>
		/// 返回流的长度.
		/// </summary>
		public int Length
		{
			get
			{
				return (int)m_Stream.Length;
			}
			set
			{
				m_Stream.SetLength(value);
			}
		}

		/// <summary>
		/// 取得或者设置流的当前位置.
		/// </summary>
		public int Position
		{
			get
			{
				return (int)m_Stream.Position;
			}
			set
			{
				m_Stream.Position = value;
			}
		}

		/// <summary>
		/// 用0填充
		/// </summary>
		public void Fill()
		{
			Fill((int)(m_Capacity - m_Stream.Length));
		}

		/// <summary>
		/// 用0填充.
		/// </summary>
		public void Fill(int length)
		{
			if (m_Stream.Position == m_Stream.Length)
			{
				m_Stream.SetLength(m_Stream.Length + length);
				m_Stream.Seek(0, SeekOrigin.End);
			}
			else
			{
				m_Stream.Write(new byte[length], 0, length);
			}
		}

		public long Seek(long offset, SeekOrigin origin)
		{
			return m_Stream.Seek(offset, origin);
		}

		/// <summary>
		/// 取得整个流的字节数组.
		/// </summary>
		public byte[] ToArray()
		{
			return m_Stream.ToArray();
		}

		/// <summary>
		/// 清空数据
		/// </summary>
		public void Clear()
		{
			m_Stream.SetLength(0);
		}
	}

}
