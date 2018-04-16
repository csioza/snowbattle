using System;
using System.Net;
using System.Net.Sockets;

namespace NGE.Network
{
	/// <summary>
	/// 侦听TCP 网络客户端连接
	/// </summary>
	public sealed class TcpListener : SocketListener
	{
		public TcpListener()
			: base()
		{
			CreateSocket();
		}

		public TcpListener(bool secure)
			: base(secure)
		{
			CreateSocket();
		}


		protected override void CreateSocket()
		{
			m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}
	}
}
