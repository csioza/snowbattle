using System;
using System.Net;
using System.Net.Sockets;

namespace NGE.Network
{
	/// <summary>
	/// Udp连接类
	/// </summary>
	public sealed class UdpConnection : SocketConnection
	{
		public UdpConnection()
			: base()
		{
			InitSocket();
		}

		internal UdpConnection(Socket socket, bool secure)
			: base(socket, secure)
		{
		}

		public override void SendPacket(Packet packet, bool encrypt_if_need)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		private void InitSocket()
		{
			Release();
			m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

		}

		protected override void CreateSocket()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		protected override void StartSecureValidate()
		{
			throw new Exception("The method or operation is not implemented.");
		}
		protected override void OnReceivedDataCallBack(byte[] data, int length)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
