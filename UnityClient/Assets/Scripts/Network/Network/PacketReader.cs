using System;
using System.Text;
using System.IO;
using NGE.Util;

namespace NGE.Network
{
	/// <summary>
	/// 消息包读取器,这个类的数据不是长久安全的，如果要保存数据，请自行复制
	/// </summary>
	public sealed class PacketReader
	{
		private byte[] m_Data;
		private int m_offset;
		private int m_Size;
		private int m_Index;
		private bool m_OwnBuffer;
		private IConnection m_clinet;

		public PacketReader(byte[] data, int packetlength, int startoffset, bool ownbuffer, IConnection clinet)
		{
			m_Data = data;
			m_Size = packetlength;
			m_offset = startoffset;
			m_Index = m_offset + Packet.HeaderSize;
			m_OwnBuffer = ownbuffer;
			m_clinet = clinet;
		}

		public void Reset()
		{
			m_Index = Packet.HeaderSize;////****m_offset?
			m_clinet = null;//****?
		}

		public void Reset(byte[] data, int startoffset, int packetlength, IConnection client)
		{
			m_Size = packetlength;
			m_offset = 0;
			m_Index = Packet.HeaderSize;
			m_clinet = client;
			Buffer.BlockCopy(data, startoffset, m_Data, 0, packetlength);
		}

		public void Release()
		{
			PacketReaderPool.Instance.Release(this);
		}


		public byte[] DataBuffer
		{
			get
			{
				if (m_OwnBuffer)
					return m_Data;
				byte[] buffer = new byte[m_Size];
				System.Buffer.BlockCopy(m_Data, m_offset, buffer, 0, m_Size);
				m_Data = buffer;
				m_OwnBuffer = true;
				m_Index -= m_offset;
				m_offset = 0;
				return buffer;
			}
		}

		public int Size
		{
			get
			{
				return m_Size;
			}
			set
			{
				m_Size = value;
			}
		}

		public int DispatcherID
		{
			get
			{
                return Util.ArrayUtility.GetInt(m_Data, m_offset + Packet.OffsetDispatcherID);
			}
			set
			{
                Util.ArrayUtility.SetInt(m_Data, value, m_offset + Packet.OffsetDispatcherID);
			}
		}
		public short PacketID
		{
			get
			{
                return Util.ArrayUtility.GetShort(m_Data, m_offset + Packet.OffsetPacketID);
			}
		}
		public bool WithSendRange
		{
			get
			{
                return (Util.ArrayUtility.GetByte(m_Data, m_offset + Packet.OffsetFlag) & (byte)PacketFlag.WithSendRange)
				    == (short)PacketFlag.WithSendRange;
			}
		}

		public bool OwnBuffer
		{
			get
			{
				return m_OwnBuffer;
			}
		}

		public IConnection Client
		{
			get
			{
				return m_clinet;
			}
		}

        public int Index
        {
            get
            {
                return m_Index;
            }
        }


		/// <summary>
		/// 重新构造为消息包
		/// </summary>
		/// <returns></returns>
		public Packet ToPacket()
		{
			Packet packet = new Packet(PacketID, DispatcherID);
			if (m_OwnBuffer)
			{
                packet.SetDataBuffer(m_Data, m_Size);
			}
			else if (Size > Packet.HeaderSize)
				packet.Writer.Write(m_Data, m_offset + Packet.HeaderSize, m_Size - Packet.HeaderSize);
			return packet;
		}

		public int Seek(int offset, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin: m_Index = m_offset + offset; break;
				case SeekOrigin.Current: m_Index += offset; break;
				case SeekOrigin.End: m_Index = m_Size + m_offset - offset; break;
			}

			return m_Index;
		}

		/// <summary>
		/// 读一个64位int值
		/// </summary>
		/// <returns></returns>
		public long ReadInt64()
		{
			if (m_Index + 8 > m_Size + m_offset)
				return 0;

			uint lowint = (uint)(m_Data[m_Index++]
				| m_Data[m_Index++] << 8
				| m_Data[m_Index++] << 16
				| m_Data[m_Index++] << 24);
			uint highint = (uint)(m_Data[m_Index++]
				| m_Data[m_Index++] << 8
				| m_Data[m_Index++] << 16
				| m_Data[m_Index++] << 24);

			return (long)((ulong)highint) << 32 | lowint;
		}

		/// <summary>
		/// 读一个无符号64位int值
		/// </summary>
		/// <returns></returns>
		public ulong ReadUInt64()
		{
			if (m_Index + 8 > m_Size + m_offset)
				return 0;

			uint lowint = (uint)(m_Data[m_Index++]
				| m_Data[m_Index++] << 8
				| m_Data[m_Index++] << 16
				| m_Data[m_Index++] << 24);
			uint highint = (uint)(m_Data[m_Index++]
				| m_Data[m_Index++] << 8
				| m_Data[m_Index++] << 16
				| m_Data[m_Index++] << 24);

			return ((ulong)highint) << 32 | lowint;
		}

		/// <summary>
		/// 读一个32位的int值
		/// </summary>
		/// <returns></returns>
		public int ReadInt32()
		{
			if (m_Index + 4 > m_Size + m_offset)
				return 0;

			return m_Data[m_Index++]
				 | m_Data[m_Index++] << 8
				 | m_Data[m_Index++] << 16
				 | m_Data[m_Index++] << 24;
		}
		/// <summary>
		/// 读一个16位的int值
		/// </summary>
		/// <returns></returns>
		public short ReadInt16()
		{
			if (m_Index + 2 > m_Size + m_offset)
				return 0;

			return (short)(m_Data[m_Index++] | m_Data[m_Index++] << 8);
		}
		/// <summary>
		/// 读一个无符号字节
		/// </summary>
		/// <returns></returns>
		public byte ReadByte()
		{
			if (m_Index + 1 > m_Size + m_offset)
				return 0;

			return m_Data[m_Index++];
		}
		/// <summary>
		/// 读一个无符号的32位int值
		/// </summary>
		/// <returns></returns>
		public uint ReadUInt32()
		{
			if (m_Index + 4 > m_Size + m_offset)
				return 0;

			return (uint)(m_Data[m_Index++] | m_Data[m_Index++] << 8 | m_Data[m_Index++] << 16 | m_Data[m_Index++] << 24);
		}
		/// <summary>
		/// 读一个无符号的16位int值
		/// </summary>
		/// <returns></returns>
		public ushort ReadUInt16()
		{
			if (m_Index + 2 > m_Size + m_offset)
				return 0;

			return (ushort)(m_Data[m_Index++] | m_Data[m_Index++] << 8);
		}
		/// <summary>
		/// 读一个有符号的字节
		/// </summary>
		/// <returns></returns>
		public sbyte ReadSByte()
		{
			if (m_Index + 1 > m_Size + m_offset)
				return 0;

			return (sbyte)m_Data[m_Index++];
		}
		/// <summary>
		/// 读一个bool值
		/// </summary>
		/// <returns></returns>
		public bool ReadBoolean()
		{
			if (m_Index + 1 > m_Size + m_offset)
				return false;

			return (m_Data[m_Index++] != 0);
		}
		/// <summary>
		/// 读一个字节数组
		/// </summary>
		/// <param name="length">数组的长度</param>
		/// <returns></returns>
		public byte[] ReadBuffer(int length)
		{
			if (m_Index + length > m_Size + m_offset || length <= 0)
				return null;

			byte[] buffer = new byte[length];
			System.Buffer.BlockCopy(m_Data, m_Index, buffer, 0, length);
			m_Index += length;

			return buffer;
		}
		/// <summary>
		/// 读取压缩整数
		/// </summary>
		/// <returns></returns>
		public int ReadCompressInt()
		{
			int val = 0;
			if (m_Index < m_Size + m_offset)
			{
				val = m_Data[m_Index++];
			}

			if (val == 255)
			{
				val = ReadInt32();
			}

			return val;
		}

		/// <summary>
		/// 读一个C++字符串std::string
		/// </summary>
		/// <returns></returns>
		public string ReadCString()
		{
			int length = ReadCompressInt();
			if (length <= 0)
			{
				return "";
			}

			int end = m_Index + length;
			if (end > m_Size + m_offset)
			{
				end = m_Size + m_offset;
			}

			string val = System.Text.Encoding.GetEncoding("GB2312").GetString(m_Data, m_Index, length);
			m_Index = end;
			return val;
		}

		/// <summary>
		/// 读一个unicode字符串
		/// </summary>
		/// <returns></returns>
		public string ReadString()//unicode
		{
			StringBuilder sb = new StringBuilder(96);

			int c;

			while ((m_Index + 1) < m_Size + m_offset && (c = (m_Data[m_Index++] | (m_Data[m_Index++] << 8))) != 0)
			{
				if (IsSafeChar(c))
					sb.Append((char)c);

			}

			return sb.ToString();
		}

		/// <summary>
		/// 以确定长度读一个unicode字符串
		/// </summary>
		/// <param name="fixedLength"></param>
		/// <returns></returns>
		public string ReadString(int fixedLength)//unicode
		{
			int bound = m_Index + (fixedLength << 1);

			if (bound > m_Size + m_offset)
				bound = m_Size + m_offset;
			int end = bound;


			StringBuilder sb = new StringBuilder(96);

			int c;

			while ((m_Index + 1) < bound && (c = (m_Data[m_Index++] | (m_Data[m_Index++] << 8))) != 0)
			{
				if (IsSafeChar(c))
					sb.Append((char)c);
			}

			m_Index = end;

			return sb.ToString();
		}

		/// <summary>
		/// 读一个Ascii编码的字符串
		/// </summary>
		/// <returns></returns>
		public string ReadAscii()
		{
			StringBuilder sb = new StringBuilder(96);

			int c;

			while (m_Index < m_Size + m_offset && (c = m_Data[m_Index++]) != 0)
			{
				if (IsSafeChar(c))
					sb.Append((char)c);
			}

			return sb.ToString();
		}

		/// <summary>
		/// 以确定长度读一个Ascii编码的字符串
		/// </summary>
		/// <param name="fixedLength"></param>
		/// <returns></returns>
		public string ReadAscii(int fixedLength)
		{
			int bound = m_Index + fixedLength;

			if (bound > m_Size + m_offset)
				bound = m_Size + m_offset;
			int end = bound;


			StringBuilder sb = new StringBuilder(96);

			int c;

			while (m_Index < bound && (c = m_Data[m_Index++]) != 0)
			{
				if (IsSafeChar(c))
					sb.Append((char)c);
			}

			m_Index = end;

			return sb.ToString();
		}
		/// <summary>
		/// 读一个utf8编码的字符串
		/// </summary>
		/// <returns></returns>
		public string ReadUTF8()
		{
			if (m_Index >= m_Size + m_offset)
				return String.Empty;

			int count = 0;
			int index = m_Index;

			while (index < m_Size + m_offset && m_Data[index++] != 0)
				++count;

			index = 0;

			byte[] buffer = new byte[count];
			int value = 0;

			while (m_Index < m_Size + m_offset && (value = m_Data[m_Index++]) != 0)
				buffer[index++] = (byte)value;

			string s = Utility.UTF8.GetString(buffer);

			bool isSafe = true;

			for (int i = 0; isSafe && i < s.Length; ++i)
				isSafe = IsSafeChar((int)s[i]);

			if (isSafe)
				return s;

			StringBuilder sb = new StringBuilder(s.Length);

			for (int i = 0; i < s.Length; ++i)
			{
				if (IsSafeChar((int)s[i]))
					sb.Append(s[i]);
			}

			return sb.ToString();
		}
		/// <summary>
		/// 以确定长度读取一个utf8编码的字符串
		/// </summary>
		/// <param name="fixedLength"></param>
		/// <returns></returns>
		public string ReadUTF8(int fixedLength)
		{
			if (m_Index >= m_Size + m_offset)
			{
				m_Index = m_Size + m_offset;
				return String.Empty;
			}

			int bound = m_Index + fixedLength;

			if (bound > m_Size + m_offset)
				bound = m_Size + m_offset;

			int count = 0;
			int index = m_Index;
			int start = m_Index;

			while (index < bound && m_Data[index++] != 0)
				++count;

			index = 0;

			byte[] buffer = new byte[count];
			int value = 0;

			while (m_Index < bound && (value = m_Data[m_Index++]) != 0)
				buffer[index++] = (byte)value;

			string s = Utility.UTF8.GetString(buffer);

			bool isSafe = true;

			for (int i = 0; isSafe && i < s.Length; ++i)
				isSafe = IsSafeChar((int)s[i]);

			m_Index = start + fixedLength;

			if (isSafe)
				return s;

			StringBuilder sb = new StringBuilder(s.Length);

			for (int i = 0; i < s.Length; ++i)
				if (IsSafeChar((int)s[i]))
					sb.Append(s[i]);

			return sb.ToString();
		}
		/// <summary>
		/// 判断是否是合法的字符串
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public static bool IsSafeChar(int c)
		{
			return (c >= 0x20 && c < 0xFFFE);
		}

		//反序列化float，gtw
		public float ReadFloat()
		{
			if (m_Index + 4 > m_Size + m_offset)
				return 0;

			float f = BitConverter.ToSingle(m_Data, m_Index);
			m_Index += 4;

			return f;
		}

		//反序列化Double，gtw
		public double ReadDouble()
		{
			if (m_Index + 8 > m_Size + m_offset)
				return 0.0;

			double db = BitConverter.ToDouble(m_Data, m_Index);
			m_Index += 8;

			return db;
		}
	}
}
