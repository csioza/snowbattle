using System;
using System.Text;

namespace NGE.Network
{
	//接收消息处理委托
	public delegate void OnPacketReceive(PacketReader preader, object state);

	/// <summary>
	/// 消息处理
	/// </summary>
	public sealed class PacketHandler
	{
		private int m_packetID;
		private OnPacketReceive m_onReceive;

		public int PacketID
		{
			get
			{
				return m_packetID;
			}
		}

		public OnPacketReceive OnReceive
		{
			get { return m_onReceive; }
		}

		public PacketHandler(int packetID, OnPacketReceive onReceive)
		{
			m_packetID = packetID;
			m_onReceive = onReceive;
		}
	}
}
