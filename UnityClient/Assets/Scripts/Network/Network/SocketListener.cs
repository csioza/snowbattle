using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace NGE.Network
{
	public abstract class SocketListener : /*UnManagedResource,*/ IListener
	{
		protected bool m_secure = false;
		protected Socket m_socket;

		protected AsyncCallback m_acceptcallback;
		protected object m_acceptcallbackArgument;

		public static int s_ListenerRefCout;
		public static object s_ListenerLock = new Object();
		public static List<SocketConnection> s_UncheckedConnList = new List<SocketConnection>();
		public static System.Threading.Thread s_ExecTimeOutThread;

		private bool m_isAsyncNetCallBack = true;
		public bool UseAsyncNetCallBack
		{
			get
			{
				return m_isAsyncNetCallBack;
			}
			set
			{
				m_isAsyncNetCallBack = value;
			}
		}
		private List<SocketConnection> m_acceptList = new List<SocketConnection>();
		private List<SocketConnection> m_delList = new List<SocketConnection>();


		/// <summary>
		/// 能够接受的最大连接数.
		/// </summary>
		public int MaxConnections
		{
			get
			{
				return m_maxConnections;
			}
			set
			{
				m_maxConnections = value;
			}
		}
		private int m_maxConnections = 1000;//----修改：郭锐。此参数直接影响侦听成功率。设置到足够大，充分利用系统资源。851在Windows2kPro/P4-2.4G/1G/100M中是一个比较接近特殊值的值。


		/// <summary>
		/// 接受一个新连接时的回调事件.
		/// </summary>
		public event ListenerCallback Accepted;

		public SocketListener()
			: this(false)
		{

		}

		public SocketListener(bool secure)
		{
			lock (s_ListenerLock)
			{
				if (s_ListenerRefCout == 0)
				{
					s_ExecTimeOutThread = new System.Threading.Thread(
					    new System.Threading.ThreadStart(ExecTimeOutThreadFunc));
					s_ExecTimeOutThread.Start();
				}
				s_ListenerRefCout++;
			}
			m_secure = secure;
		}


		/// <summary>
		/// 开始监听
		/// </summary>
		/// <param name="port">监听的端口.</param>
		public void Listen(int port, object state)
		{
			m_acceptcallbackArgument = state;
			IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
			if (m_socket == null)
				CreateSocket();
			bool isBindOk = false;
			while (false == isBindOk)
			{
				try
				{
					m_socket.Bind(endPoint);
					isBindOk = true;
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(e.StackTrace);
					Console.WriteLine("\r\nBind[" + port + "] Port Failed !Retry after 3000 ms\r\n");
					System.Threading.Thread.Sleep(3000);
				}
			}
			m_socket.Listen(m_maxConnections);//

			if (m_acceptcallback == null)
				m_acceptcallback = new AsyncCallback(OnAccept);
			m_socket.BeginAccept(m_acceptcallback, m_socket);
			NetworkPerformanceStatistics.Instance.nAsyncSocketAcceptBegin++;
		}
		public void Listen(int port)
		{
			Listen(port, null);
		}

		void OnAccept(IAsyncResult result)
		{
			try
			{
				if (m_socket == null || !object.ReferenceEquals(m_socket, result.AsyncState))
					return;
				Socket client = m_socket.EndAccept(result);
				NetworkPerformanceStatistics.Instance.nAsyncSocketAcceptEnd++;


				SocketConnection acceptedClient = null;
				if (client.ProtocolType == ProtocolType.Tcp)
					acceptedClient = new TcpConnection(client, m_secure);
				else if (client.ProtocolType == ProtocolType.Udp)
					acceptedClient = new UdpConnection(client, m_secure);

				if (!m_isAsyncNetCallBack && acceptedClient != null)
				{
					acceptedClient.UseAsyncNetCallBack = false;
					if (m_secure)
					{
						acceptedClient.Listener = this;

						lock (s_UncheckedConnList)
						{
							s_UncheckedConnList.Add(acceptedClient);
						}
					}
					else
					{
						lock (this)
						{
							m_acceptList.Add(acceptedClient);
						}
					}
				}
				else
				{
					if (acceptedClient != null && acceptedClient.IsSocketConnectedWithForceCheck)
					{
						if (Accepted != null && !m_secure)
						{
							acceptedClient.SetState(ConnectionState.Connected);
							Accepted(new ListenerArgs(this, 0, acceptedClient), m_acceptcallbackArgument);
						}
						if (acceptedClient != null)
						{
							if (m_secure)
							{
								acceptedClient.Listener = this;

								lock (s_UncheckedConnList)
									s_UncheckedConnList.Add(acceptedClient);
							}
						}
					}
					else
					{
						acceptedClient.Release();
						acceptedClient = null;
					}
				}
				//start receive data
				if (acceptedClient != null)
					acceptedClient.Receive();
			}
			catch (Exception)
			{

			}
			finally
			{
				// accept more...
				//if (m_socket != null) {
				//}
				System.Diagnostics.Debug.Assert(m_socket != null);
				m_socket.BeginAccept(m_acceptcallback, m_socket);//m_acceptcallback
				NetworkPerformanceStatistics.Instance.nAsyncSocketAcceptBegin++;
			}
		}

		public void Update(object state)
		{
			if (!object.ReferenceEquals(m_acceptcallbackArgument, state))
				m_acceptcallbackArgument = state;
			if (m_isAsyncNetCallBack || Accepted == null)
				return;
			lock (this)
			{
				foreach (SocketConnection conn in m_acceptList)
				{
					if (conn.IsSocketConnectedWithForceCheck)
					{
						if (Accepted != null)
							Accepted(new ListenerArgs(this, 0, conn), m_acceptcallbackArgument);
					}
					else
					{
						m_delList.Add(conn);
					}
				}
				m_acceptList.Clear();
				foreach (SocketConnection conn in m_delList)
				{
					conn.Release();
				}
				m_delList.Clear();
			}
		}

		private static void ExecTimeOutThreadFunc()
		{
			while (s_ListenerRefCout > 0)
			{
				lock (s_UncheckedConnList)
				{
					List<SocketConnection> deletelist = new List<SocketConnection>();
					for (int i = 0; i < s_UncheckedConnList.Count; i++)
					{
						SocketConnection conn = s_UncheckedConnList[i];
						if ((DateTime.Now - conn.ConnectedTime).TotalSeconds > 50)
						{
							conn.CloseConnection();
							deletelist.Add(conn);
						}
					}
					foreach (SocketConnection conn in deletelist)
					{
						s_UncheckedConnList.Remove(conn);
						conn.Release();
					}
					deletelist.Clear();
				}
				System.Threading.Thread.Sleep(100);
			}
			lock (s_UncheckedConnList)
			{
				List<SocketConnection> deletelist = new List<SocketConnection>();
				foreach (SocketConnection conn in s_UncheckedConnList)
				{
					conn.CloseConnection();
					deletelist.Add(conn);
				}
				foreach (SocketConnection conn in deletelist)
				{
					s_UncheckedConnList.Remove(conn);
					conn.Release();
				}
				deletelist.Clear();
			}
		}

		/// <summary>
		/// 从验证列表里删除一个连接
		/// </summary>
		/// <param name="client"></param>
		internal void RemoveConnection(IConnection client)
		{
			lock (s_UncheckedConnList)
			{
				s_UncheckedConnList.Remove((SocketConnection)client);
			}
		}

		internal void OnSecureAccepted(IConnection client)
		{
			lock (this)
			{
				if (Accepted != null)
				{
					if (m_isAsyncNetCallBack)
						Accepted(new ListenerArgs(this, 0, client), m_acceptcallbackArgument);
					else
					{
						m_acceptList.Add((TcpConnection)client);
					}
				}

			}

			RemoveConnection(client);
		}

		/// <summary>
		/// 关闭所有客户端连接
		/// </summary>
		public void CloseClients()
		{
			try
			{

			}
			catch
			{
			}
		}

		/// <summary>
		/// 停止侦听
		/// </summary>
		public void Stop()
		{
			CloseListenSocket();
		}
		/// <summary>
		/// 关闭网络侦听器
		/// </summary>
		public void Release()
		{
			lock (s_ListenerLock)
				s_ListenerRefCout--;
			Dispose(true);
		}

		protected /*override*/ void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			CloseListenSocket();
			m_acceptcallbackArgument = null;
			m_acceptcallback = null;
		}

		private void CloseListenSocket()
		{
			if (m_socket != null)
			{
				try { m_socket.Shutdown(SocketShutdown.Both); }
				catch { }
				try { m_socket.Close(); }
				catch { }
				m_socket = null;
			}
		}

		protected abstract void CreateSocket();
	}
}
