using System;
using System.Net;
using System.Net.Sockets;

namespace NGE.Network
{
	public sealed class UdpListener : SocketListener
	{
		public UdpListener()
			: base()
		{
			CreateSocket();
		}
		public UdpListener(bool secure)
			: base(secure)
		{
			CreateSocket();
		}

		protected override void CreateSocket()
		{
			m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		}
	}
}
