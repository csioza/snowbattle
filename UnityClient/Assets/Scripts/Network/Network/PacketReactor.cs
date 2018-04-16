using System;
using System.Collections.Generic;

namespace NGE.Network
{
	/// <summary>
	/// 消息反应器
	/// </summary>
	public class PacketReactor
	{
		const int HandleLowArraySize = 500;
		PacketHandler[] m_handlesLow = new PacketHandler[HandleLowArraySize]; //id小于HandleLowArraySize的包
		Dictionary<int, PacketHandler> m_handlesHigh = new Dictionary<int, PacketHandler>();//id大于等于HandleLowArraySize的包

		PacketHandler m_defaultHandler;

		private static PacketReactor m_instance = new PacketReactor();
		public static PacketReactor Instance
		{
			get
			{
				return m_instance;
			}
		}

		public PacketReactor()
		{
		}

		public PacketHandler DefaultHandler
		{
			get
			{
				return m_defaultHandler;
			}
			set
			{
				m_defaultHandler = value;
			}
		}

		/// <summary>
		/// 注册一个消息处理器
		/// </summary>
		/// <param name="packetID">消息类型</param>
		/// <param name="length">消息长度</param>
		/// <param name="receive">接收处理委托</param>
		public void RegisterHandler(int packetID, OnPacketReceive receive)
		{
			if (packetID >= 0 && packetID < m_handlesLow.Length)
				m_handlesLow[packetID] = new PacketHandler(packetID, receive);
			else
				m_handlesHigh[packetID] = new PacketHandler(packetID, receive);
		}

		/// <summary>
		/// 注册缺省消息处理器,如果注册了这个处理器，所有未单独注册的消息包由这个处理器处理
		/// </summary>
		/// <param name="receive"></param>
		public void RegisterDefaultHandler(OnPacketReceive receive)
		{
			m_defaultHandler = new PacketHandler(0, receive);
		}

		/// <summary>
		/// 注销一个消息处理器
		/// </summary>
		/// <param name="packetID"></param>
		public void RemoveHandler(int packetID)
		{
			if (packetID >= 0 && packetID < m_handlesLow.Length)
				m_handlesLow[packetID] = null;
			else
				m_handlesHigh.Remove(packetID);
		}

		/// <summary>
		/// 根据消息类型得到消息处理器
		/// </summary>
		/// <param name="packetID">消息类型</param>
		/// <returns>消息处理器</returns>
		public PacketHandler GetHandler(int packetID)
		{
			PacketHandler handler;
			if (packetID >= 0 && packetID < m_handlesLow.Length)
				handler = m_handlesLow[packetID];
			else if (m_handlesHigh.ContainsKey(packetID))
				handler = m_handlesHigh[packetID];
			else
				return m_defaultHandler;

			if (handler == null)
				return m_defaultHandler;
			else
				return handler;
		}
	}
}
