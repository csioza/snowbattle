using System;
using NGE.Util;

namespace NGE.Network
{
	/// <summary>
	/// 消息包的标志，表示一个消息包是否压缩，加密
	/// </summary>
	public enum PacketFlag : short
	{
		Normal = 0,//不压缩，不加密，是缺省状态
		Compressed = 1,//压缩的
		Encrypted = 2,//加密的
		/// <summary>
		/// 带有发送范围的消息,有可能带有发送id列表的,广播消息时使用
		/// </summary>
		WithSendRange = 4,

        WithSession = 32,   //附带session的消息头
		Compress_Need_Checksum = 12345, //压缩的时候需要Checksum(crc32)34
	}

	/// <summary>
	/// 发送的目标范围,PacketIDList里使用
	/// </summary>
	public enum PacketSendRange : byte
	{
		SendRange_List = 0,//发送给消息后面带的id列表
		SendRange_World = 1,//发送给全部游戏世界里的玩家
		SendRange_All = 2//发送给所有的客户端(包括游戏世界里和外的)
	};


	/// <summary>
	/// 发到Gate去的消息，消息需要广播时附加的id列表数据，放到消息最后面
	/// Packet.Length += 1 + idlistcount*2 + 2;
	/// 比如要发送给1000个对象，那么消息增加的大小为1+1000*2 +2 = 2003字节
	/// 比如要发送给0个对象(SendRange != sendrange_list时)，那么消息增加的大小为1+0*2 +2 = 3字节
	/// </summary>
	public class PacketIDList
	{
		const short max_id_number_broadcast = 1024;//最多一个消息将广播的对象个数

		public PacketSendRange SendRange;//缺省为SendRange_List
		public short idlistcount;
		public byte[] data; //最后一个是个数大小

		public int Length
		{
			get
			{
				if (idlistcount == 0)
					return 0;

				return 1 + idlistcount * 2 + 2;
			}
		}

		public void AddId(short id)
		{
			if (data == null)
				data = new byte[max_id_number_broadcast * 2 + 3];
			ArrayUtility.SetShort(data, id, 1 + idlistcount * 2);
			idlistcount++;
		}

		public void Flush()
		{
			if (PacketSendRange.SendRange_List != SendRange)
			{
				data = new byte[3];
				idlistcount = 0;
			}
			else if (data == null)
				return;
			data[0] = (byte)SendRange;
			ArrayUtility.SetShort(data, idlistcount, 1 + idlistcount * 2);
		}
	};


	/// <summary>
	/// 消息包
	/// 内存布局
	/// {
	///     short  PacketFlag;//特殊的值
	///     short  PacketLength;//消息包的大小,消息不能大于32k
	///     int    PacketID;//消息包类型id；从这以后的数据可以加密
	///     int    SerialNumber;//消息包序列号,网络连接每发一个消息包，这个值+1
	///     int    DispatcherID;//发送者的id
	///                         //所以消息头的大小是2+2+4+4+4 = 16
	/// }
	/// </summary>
	public class Packet
	{
		public const int MaxLength = 32 << 10;//32000;修改郭锐
		public const short InvalidLength = -1;
		public const short NeedCompressMinLength = 100;
		public const short NoCryptHeaderLength = 4; // 2+2
        public const int OffsetLength = 0;
        public const int OffsetFlag = 8;
        public const int OffsetPacketID = 2;
        public const int OffsetDispatcherID = 4;

		//消息包标志
		public PacketFlag PacketFlag = PacketFlag.Normal;

		public short PacketID;

		/// <summary>
		/// 消息序列号
		/// </summary>
		public int SerialNumber;
		public int DispatcherID;//if id <0 ,is -serverid
		public PacketWriter m_writer;
		public static int m_packetIdentity = 0; //一个packet对象的唯一ID，用来进行重发识别。


        private byte[] m_databuffer; //已经有的数据的缓冲
        private int m_datalength; //已经有的数据的大小
        private bool m_useDataBufferLength=false;

        public Packet(int packetID, bool isLongConnect = false)
			: this(packetID, 0)
		{
            if (!isLongConnect)
            {//在这里附加session
                PacketFlag |= PacketFlag.WithSession;
                m_writer.Write(m_packetIdentity);
                m_writer.WriteAscii(MessageTransfer.Singleton.Session, 32);
            }
		}

        public void Reset(int packetID, bool isLongConnect)
        {
            PacketFlag = PacketFlag.Normal;
            PacketID = (short)packetID;
            DispatcherID = 0;
            m_writer.Clear();
            m_packetIdentity++;
            m_datalength = 0;
            m_useDataBufferLength = false;

            if (!isLongConnect)
            {//在这里附加session
                PacketFlag |= PacketFlag.WithSession;
                m_writer.Write(m_packetIdentity);
                m_writer.WriteAscii(MessageTransfer.Singleton.Session, 32);
            }
        }

		public Packet(int packetID, int dispatcherid)
		{
			PacketID = (short)packetID;
			DispatcherID = dispatcherid;
			m_writer = new PacketWriter();/*PacketWriter.CreateInstance()*/ ;//****
			m_packetIdentity++;
		}


		/// <summary>
		/// 消息总长度
		/// </summary>
		public int Length
		{
			get
			{
                if (m_useDataBufferLength == false)
                    return Packet.HeaderSize + m_writer.Length;
                else
                    return m_datalength + m_writer.Length;
			}
            //set
            //{
            //    if (m_databuffer == null)
            //        return;
            //    m_datalength = value;
            //}
		}

		internal byte[] DataBuffer
		{
			get
			{
				return m_databuffer;
			}
			set
			{
				m_databuffer = value;
				if (m_databuffer != null)
					m_datalength = m_databuffer.Length;
			}
		}

		public PacketWriter Writer
		{
			get
			{
				return m_writer;
			}
		}

        public void SetDataBuffer(byte[] buffer,int length)
        {
            m_databuffer = buffer;
            m_datalength = length;
            m_useDataBufferLength = true;
        }

		internal byte[] ToArray()
		{
			short packet_length = (short)this.Length;
			bool needcreatebuffer = false;
			if (DataBuffer == null || m_writer.Length > 0)
				needcreatebuffer = true;
			if (needcreatebuffer)
			{
				byte[] newbuffer = new byte[packet_length];
                if (m_databuffer != null && m_datalength > 0)
				{
					Buffer.BlockCopy(m_databuffer, 0, newbuffer, 0, m_datalength);
				}
				m_databuffer = newbuffer;
			}
			//写消息头
            short packetid = (short)this.PacketID;
            byte flag = (byte)PacketFlag;
            ArrayUtility.SetShort(m_databuffer, packet_length, Packet.OffsetLength);
            ArrayUtility.SetShort(m_databuffer, packetid, Packet.OffsetPacketID);
            ArrayUtility.SetInt(m_databuffer, this.DispatcherID, Packet.OffsetDispatcherID);
            ArrayUtility.SetByte(m_databuffer, flag, Packet.OffsetFlag);            

			if (needcreatebuffer && m_writer.Length > 0)
			{
				System.Buffer.BlockCopy(m_writer.ToArray(), 0, m_databuffer,
				    packet_length - m_writer.Length, m_writer.Length);
				m_writer.Length = 0;
                m_useDataBufferLength = true;
			}
			m_datalength = packet_length;

			return m_databuffer;
		}

		/// <summary>
		/// 消息头大小 2+2+4+4+4 = 16
		/// </summary>
		public static int HeaderSize
		{
			get
			{
				return 12;
			}
		}
	}
}
