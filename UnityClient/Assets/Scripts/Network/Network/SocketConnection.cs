using System;
using System.Net;
using System.Net.Sockets;

#region disableWarning
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#endregion

namespace NGE.Network
{
	enum NetStateChange
	{
		State_Nothing = 0,
		State_Connect_Success = 1,
		State_Connect_Failed = 2,
		State_Connect_Lost = 4
	}
	/// <summary>
	/// 基于Socket的连接类
	/// </summary>
	public abstract class SocketConnection : /*UnManagedResource,*/ IConnection
	{
		private DateTime m_connectedTime;
		private long m_totalsent = 0;
		private long m_totalreceived = 0;
		protected bool m_secure_connection = false;
		//protected long m_sendatom  = 0;
		private int m_sendatom = 0;
		protected long m_recvtom = 0;
		protected bool m_userclose;
		protected bool m_catchedOnlost;

		protected Socket m_socket;
		protected IPEndPoint m_localip;
		protected IPEndPoint m_remoteip;
		private SendQueue m_SendQueue;

		private AsyncCallback m_OnSent;
		private AsyncCallback m_OnReceived;
		private AsyncCallback m_Connected;
		private ConnectionArgs m_receivedargs;
		private byte[] m_tempbyte = new byte[1];
		protected object m_connectcallbackState;
		protected object m_PacketHandlerCallbackArgument;

		public object PacketHandlerCallbackArgument
		{
			get
			{
				return m_PacketHandlerCallbackArgument;
			}
			set
			{
				m_PacketHandlerCallbackArgument = value;
			}
		}

		public object ConnectCallbackArgument
		{
			get
			{
				return m_connectcallbackState;
			}
			set
			{
				m_connectcallbackState = value;
			}
		}

		protected PacketReactor m_reactor;


		public PacketReactor Reactor
		{
			get
			{
				return m_reactor;
			}
			set
			{
				m_reactor = value;
			}
		}

		protected SocketListener m_listener;

		protected bool m_isAsyncNetCallBack = true;
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
		protected int m_netStateChange = 0;

		#region 统计信息
		/// <summary>
		/// 总共发送的数据大小(字节)
		/// </summary>
		public long TotalSent
		{
			get { return m_totalsent; }
		}
		/// <summary>
		/// 总共接受的数据大小(字节)
		/// </summary>
		public long TotalReceived
		{
			get { return m_totalreceived; }
		}
		/// <summary>
		/// 发送数据频率,//----总的平均速率
		/// </summary>
		public int SentRate
		{
			get
			{
				double seconds = (DateTime.Now - m_connectedTime).TotalSeconds;
				if (seconds > 0)
					return (int)((double)m_totalsent / seconds);
				else
					return 0;
			}
		}
		/// <summary>
		/// 接受数据频率 //----总的平均速率
		/// </summary>
		public int ReceivedRate
		{
			get
			{
				double seconds = (DateTime.Now - m_connectedTime).TotalSeconds;
				if (seconds > 0)
					return (int)((double)m_totalreceived / seconds);
				else
					return 0;
			}
		}
		#endregion

		/// <summary>
		/// 是否采用安全socket
		/// </summary>
		public bool SecureConnection
		{
			get { return m_secure_connection; }
		}


		/// <summary>
		/// 连接状态.
		/// </summary>
		public ConnectionState State
		{
			get
			{
				return m_connectionState;
			}
		}
		internal void SetState(ConnectionState state)
		{
			m_connectionState = state;
		}
		protected ConnectionState m_connectionState = ConnectionState.Uninitialised;

		// 接收到的数据缓存
		protected readonly byte[] m_RecvBuffer;
		// 接收到的数据大小
		int m_filled = 0;
		// 已经收到的，但还没有交给用户的数据的大小。它好像一个循环队列，一头进一头出。
		protected int m_iSegmentSize = 0;
		// 每次接收数据的最大长度
		private const int BUFFER_SIZE = Packet.MaxLength;

		// 连接的本地IP地址
		public IPEndPoint LocalIP { get { return m_localip; } }
		// 连接的远端IP地址
		public IPEndPoint RemoteIP { get { return m_remoteip; } }

		public bool IsSendOver { get { return m_sendatom > 0; } }

		public bool IsConnected { get { return m_socket != null && m_connectionState == ConnectionState.Connected; } }

		public DateTime ConnectedTime { get { return m_connectedTime; } }

		public bool IsSocketConnectedWithForceCheck
		{
			get
			{
				if (m_socket == null)
					return false;
				try
				{
					//需要在noblocking模式下
					m_socket.Send(m_tempbyte, 0, 0);//----这里发送1字节的数据会不会有问题？
					return true;
				}
				catch (SocketException e)
				{
					// 10035 == WSAEWOULDBLOCK
					if (e.NativeErrorCode.Equals(10035))
					{
						return true;
					}
					else
						return false;
				}
				catch
				{
					return false;
				}
			}
		}

		//通过accept方式创建自己的连接
		internal SocketListener Listener
		{
			get { return m_listener; }
			set { m_listener = value; }
		}

		/// <summary>
		/// (异步)成功连接到远端后回调事件.
		/// </summary>
		public event ConnectionCallback Connected;
		/// <summary>
		/// (异步)连接失败时回调事件
		/// </summary>
		public event ConnectionFailedCallback ConnectionFailed;
		/// <summary>
		/// (异步)连接断开的回调事件
		/// </summary>
		public event ConnectionFailedCallback ConnectionLost;

		public SocketConnection()
			: this(null, false)
		{

		}

		public SocketConnection(bool secure)
			: this(null, secure)
		{

		}

		internal SocketConnection(Socket socket, bool secure)
		{
			this.m_secure_connection = secure;
			this.m_socket = socket;
			m_RecvBuffer = new byte[BUFFER_SIZE + 30*1024/*8k,why?*/];
			m_OnSent = new AsyncCallback(OnEndSend);
			m_OnReceived = new AsyncCallback(OnEndReceive);
			m_Connected = new AsyncCallback(OnEndConnect);

			m_SendQueue = new SendQueue();
			if (socket != null)
			{
				m_socket.Blocking = false;
				if (IsSocketConnectedWithForceCheck)
				{
					m_connectedTime = DateTime.Now;
					if (m_secure_connection)
					{
						m_connectionState = ConnectionState.ConnectionSSLInit;
					}
					else
					{
						m_connectionState = ConnectionState.Connected;
					}
				}

				m_localip = (IPEndPoint)socket.LocalEndPoint;
				m_remoteip = (IPEndPoint)socket.RemoteEndPoint;
			}

			m_receivedargs = new ConnectionArgs(this, 0);

		}

		public virtual void Update(object state)
		{
			if (m_isAsyncNetCallBack)
				return;
			lock (this)
			{
				if (NetStateChange.State_Connect_Success
				== (NetStateChange)(m_netStateChange & (int)NetStateChange.State_Connect_Success) &&
				Connected != null)
				{
					OnConnectedCallback(m_connectcallbackState);
					m_netStateChange &= (~(int)NetStateChange.State_Connect_Success);
				}

				if (NetStateChange.State_Connect_Failed
					== (NetStateChange)(m_netStateChange & (int)NetStateChange.State_Connect_Failed) &&
					ConnectionFailed != null)
				{
					ConnectionFailed(new ConnectionArgs(this, 0), new ErrorArgs(ErrorType.ConnectingFailed, null), m_connectcallbackState);
					m_netStateChange &= (~(int)NetStateChange.State_Connect_Failed);
				}

				if (NetStateChange.State_Connect_Lost
					== (NetStateChange)(m_netStateChange & (int)NetStateChange.State_Connect_Lost) &&
					ConnectionLost != null)
				{
					ConnectionLost(new ConnectionArgs(this, 0), new ErrorArgs(ErrorType.ConnectionLost, null), m_connectcallbackState);
					m_netStateChange &= (~(int)NetStateChange.State_Connect_Lost);
				}
			}
		}

		/// <summary>
		/// 开始连接远端IP地址
		/// </summary>
		/// <param name="hostName">目的主机名(可以是ip地址和DNS名)</param>
		/// <param name="port">目的主机端口</param>
		/// <param name="secure">是否使用安全连接</param>
		/// <returns></returns>
		public int Connect(string hostName, int port, bool secure)
		{
			return Connect(hostName, port, secure, null);
		}
		/// <summary>
		/// 开始连接远端IP地址
		/// </summary>
		/// <param name="hostName">目的主机名(可以是ip地址和DNS名)</param>
		/// <param name="port">目的主机端口</param>
		/// <param name="secure">是否使用安全连接</param>
		/// <param name="state">传入的参数</param>
		/// <returns></returns>
		public int Connect(string hostName, int port, bool secure, object state)
		{
			if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
				throw new ArgumentOutOfRangeException("port");
            IPAddress ip = null;
            try
            {
                ip = IPAddress.Parse(hostName);
            }
            catch(Exception ex)
            {
            }
            if (ip == null)
            {
                IPAddress[] addresses = Dns.GetHostEntry(hostName).AddressList;
                if (addresses.Length > 0)
                {
                    ip = addresses[0];
                }
            }

			
			try
			{  //----这里有问题,这个Connect是异步的。
				return Connect(new IPEndPoint(ip, port), secure, state);
			}
			catch (Exception ex)
            {
                if (ConnectionFailed != null && !m_userclose)
                    ConnectionFailed(new ConnectionArgs(this, 0),
                        new ErrorArgs(ErrorType.ConnectingFailed, ex.Message), state);
			}
			
			return -1;
		}
		/// <summary>
		/// 开始连接远端IP地址
		/// </summary>
		/// <param name="hostName">目的主机名(可以是ip地址和DNS名)</param>
		/// <param name="port">目的主机端口</param>
		/// <returns></returns>
		public int Connect(string hostName, int port)
		{
			return Connect(hostName, port, false, null);
		}

		public int Connect(string hostName, int port, object state)
		{
			return Connect(hostName, port, false, state);
		}


		/// <summary>
		/// 开始连接远端IP地址
		/// </summary>
		/// <param name="endPoint"></param>
		/// <param name="secure">是否使用安全连接</param>
		/// <returns></returns>
		public int Connect(IPEndPoint endPoint, bool secure, object state)
		{
			if (endPoint == null)
				throw new ArgumentNullException("endPoint");
			m_connectcallbackState = state;
			m_secure_connection = secure;
			m_remoteip = endPoint;
			CloseConnection();
			if (m_socket == null)
				CreateSocket();
			IAsyncResult result = m_socket.BeginConnect(endPoint, m_Connected, null);
            NetworkPerformanceStatistics.Instance.nAsyncSocketConnectBegin++;
            //等待几秒 by gpf
            bool isSuccess = result.AsyncWaitHandle.WaitOne(5000, true);
            if (!isSuccess)
            {
                NetworkErrorReporter.Instance.nSocketAsyncConnectFailed++;
                m_connectionState = ConnectionState.ConnectionFailed;
                CloseConnection();
                if (ConnectionFailed != null && !m_userclose && m_isAsyncNetCallBack)
                    ConnectionFailed(new ConnectionArgs(this, 0),
                        new ErrorArgs(ErrorType.ConnectingFailed, "connect time out"), m_connectcallbackState);
                else
                {
                    lock (this)
                        m_netStateChange |= (int)NetStateChange.State_Connect_Failed;
                }
            }
			return 0;
		}

		public int Connect(IPEndPoint endPoint, bool secure)
		{
			return Connect(endPoint, secure, null);
		}

		public int Connect(IPEndPoint endPoint)
		{
			return Connect(endPoint, false, null);
		}

		private void OnEndConnect(IAsyncResult rs)
		{
			try
			{
                UnityEngine.Debug.Log("OnEndConnect");
				if (m_socket == null)
					return;
				m_socket.EndConnect(rs);
				NetworkPerformanceStatistics.Instance.nAsyncSocketConnectEnd++;
				m_connectedTime = DateTime.Now;
				m_catchedOnlost = false;
				m_localip = (IPEndPoint)m_socket.LocalEndPoint;
				if (m_secure_connection)
				{
					lock (this)
						m_connectionState = ConnectionState.ConnectionSSLInit;
					StartSecureValidate();
				}
				else
				{
					m_connectionState = ConnectionState.Connected;
					if (m_isAsyncNetCallBack)
						OnConnectedCallback(m_connectcallbackState);
					else
					{
						lock (this)
							m_netStateChange |= (int)NetStateChange.State_Connect_Success;
					}
				}

				Receive();
                UnityEngine.Debug.Log("End OnEndConnect");
			}
			catch (SocketException ex)
			{
				NetworkErrorReporter.Instance.nSocketAsyncConnectFailed++;

				m_connectionState = ConnectionState.ConnectionFailed;
				CloseConnection();
				if (ConnectionFailed != null && !m_userclose && m_isAsyncNetCallBack)
					ConnectionFailed(new ConnectionArgs(this, 0),
						new ErrorArgs(ErrorType.ConnectingFailed, ex.Message), m_connectcallbackState);
				else
				{
					lock (this)
						m_netStateChange |= (int)NetStateChange.State_Connect_Failed;
				}
			}
			catch (Exception)
			{
			}
		}


		public void Send(byte[] data, int count)
		{
			System.Diagnostics.Debug.Assert(count <= data.Length);
			if (NetworkPerformanceStatistics.Instance.nSendPacketSizeMax < count)
				NetworkPerformanceStatistics.Instance.nSendPacketSizeMax = count;
			if (NetworkPerformanceStatistics.Instance.nSendPacketSizeMin > count)
				NetworkPerformanceStatistics.Instance.nSendPacketSizeMin = count;

            if (m_socket == null || data == null || count <= 0)
            {
                return;
            }
			try
			{
				bool sendnow = false;
				lock (m_SendQueue)
				{
					if (m_sendatom == 0)//IsSendOver
					{
						m_sendatom++;
						sendnow = true;
					}
					else//sending
						m_SendQueue.Enqueue(data, count);

				}

				//
                if (sendnow)
                {
                    UnityEngine.Debug.Log("BeginSend");
                    IAsyncResult result = m_socket.BeginSend(data, 0, count, SocketFlags.None, m_OnSent, m_socket);
                    bool success = result.AsyncWaitHandle.WaitOne(5000, true);
                    if (!success)
                    {
                        UnityEngine.Debug.Log("BeginSend failed!!!");
                    }
                }

			}
			catch (SocketException e)
			{
				NetworkErrorReporter.Instance.nAsyncSendFailure_SocketException++;

				m_sendatom = 0;
				CloseConnection();
                CatchConnectionLost(ErrorType.SendFailed, e.Message);
				return;
			}
			catch (ObjectDisposedException e)
			{
				m_sendatom = 0;
				CloseConnection();
                CatchConnectionLost(ErrorType.SendFailed, e.Message);
				return;
			}
			catch (Exception e)
			{
				Debug.TraceException(e);
				CloseConnection();
			}
		}
		private void OnEndSend(IAsyncResult rs)
		{
			try
			{
                UnityEngine.Debug.Log("begin OnEndSend");
				if (m_socket == null)
				{
					return;
				}
				int sendbyte = m_socket.EndSend(rs);
				m_totalsent += sendbyte;

				NetworkPerformanceStatistics.Instance.nAsyncSendEnd++;

				int length;
				byte[] data;
				lock (m_SendQueue)
				{
					if (!m_SendQueue.Dequeue(out data, out length))
					{
						m_sendatom--;
						return;
					}
				}

				//
				m_socket.BeginSend(data, 0, length, SocketFlags.None, m_OnSent, m_socket);
                UnityEngine.Debug.Log("end OnEndSend");

			}
			catch (SocketException e)
			{
				//socket be closed .. 
                CatchConnectionLost(ErrorType.SendFailed, e.Message);
			}
			catch (ObjectDisposedException e)
			{
				CloseConnection();
                CatchConnectionLost(ErrorType.SendFailed, e.Message);
			}
			catch (Exception e)
			{
				Debug.TraceException(e);
				return;
			}
		}

		private void BeginSend(byte[] buf, int count)
		{
			NetworkPerformanceStatistics.Instance.nAsyncSendBegin++;
			m_sendatom++;
			m_socket.BeginSend(buf, 0, count, SocketFlags.None, m_OnSent, null);
		}
		public void Send(byte[] data)
		{
			Send(data, data.Length);
		}

		/// <summary>
		/// Starts receiving data.
		/// </summary>
		internal int Receive()
		{
			return Receive(m_RecvBuffer, 0, BUFFER_SIZE);
		}

		private int Receive(byte[] data, int offset, int count)
		{
			try
			{
				if (m_socket != null)
				{
					NetworkPerformanceStatistics.Instance.nAsyncRecvBegin++;
#if UNITY_IPHONE
					m_socket.Blocking=true;
#endif
                    UnityEngine.Debug.Log("begin Receive");
					IAsyncResult result = m_socket.BeginReceive(data, offset, count, SocketFlags.None, m_OnReceived, null);
				}
			}
			catch (SocketException e)
			{
				CloseConnection();
                CatchConnectionLost(ErrorType.ReceiveFailed, e.Message);
                UnityEngine.Debug.Log("Receive SocketException");
			}
			catch (Exception e)
			{
                Debug.TraceException(e);
                CatchConnectionLost(ErrorType.ReceiveFailed, e.Message);
                UnityEngine.Debug.Log("Receive Exception");
			}
			return 0;
		}


		private void OnEndReceive(IAsyncResult rs)
		{
			try
			{
				if (m_socket == null)
					return;

				m_filled = m_socket.EndReceive(rs);

				if (m_filled > 0)
				{
					m_totalreceived += m_filled;
					NetworkPerformanceStatistics.Instance.nAsyncRecvEnd++;
					if (NetworkPerformanceStatistics.Instance.nRecvPacketSizeMax < m_filled)
						NetworkPerformanceStatistics.Instance.nRecvPacketSizeMax = m_filled;
					if (NetworkPerformanceStatistics.Instance.nRecvPacketSizeMin > m_filled)
						NetworkPerformanceStatistics.Instance.nRecvPacketSizeMin = m_filled;

					OnReceivedDataCallBack(m_RecvBuffer, m_filled);

					if (m_connectionState == ConnectionState.Uninitialised)
						return;
					//System.Diagnostics.Debug.Assert( m_iSegmentSize== 0);//----这里有问题，之所以出错不表现，是因为m_iSegmentSize一般为0.
					int leftlength = m_RecvBuffer.Length - m_iSegmentSize;
					int nextrecvlength = Math.Min(leftlength, BUFFER_SIZE);
                    UnityEngine.Debug.Log("begin OnEndReceive");
					Receive(m_RecvBuffer, m_iSegmentSize, nextrecvlength);
				}
				else
				{
					if (m_filled == 0)// the remote host Shutdown()ed, and all available data has been received,
					{
						CatchConnectionLost(ErrorType.ReceiveFailed, "connection closed.");
						return;
					}
				}
			}
			catch (SocketException e)
			{
				//int wsaecode = e.NativeErrorCode;
				CatchConnectionLost(ErrorType.ReceiveFailed, e.Message);
			}
			catch (ObjectDisposedException e)
			{
				CatchConnectionLost(ErrorType.ReceiveFailed, e.Message);
			}
			catch (Exception e)
			{
				Debug.TraceException(e);
				CatchConnectionLost(ErrorType.ReceiveFailed, e.Message);
			}
		}

		/// <summary>
		/// 关闭网络连接和连接对象
		/// </summary>
		public void Release()
		{
			m_userclose = true;
			Dispose(true);
		}

		public virtual void CloseConnection()
		{
			if (m_socket != null)
			{
				try { m_socket.Shutdown(SocketShutdown.Both); }
				catch { };
				try { m_socket.Close(); }
				catch { };
				m_socket = null;
			}
			m_iSegmentSize = 0;
			m_totalsent = 0;
			m_totalreceived = 0;
			m_connectionState = ConnectionState.Uninitialised;
			if (m_listener != null)
			{
				m_listener.RemoveConnection(this);
				m_listener = null;
			}
			if (m_SendQueue != null)
				m_SendQueue.Clear();

			m_sendatom = 0;
		}

		protected virtual void Dispose(bool disposing)
		{
			CloseConnection();
			if (m_socket != null)
			{
				m_socket.Close();
				m_socket = null;
			}
			m_connectcallbackState = null;
			m_PacketHandlerCallbackArgument = null;
			m_OnSent = null;
			m_OnReceived = null;
			m_Connected = null;
			m_receivedargs = null;
			m_reactor = null;
			m_SendQueue = null;
			//m_RecvBuffer = null;
		}


		private void CatchConnectionLost(ErrorType etype, string message)
		{
			ConnectionFailedCallback conLost = null;
			object callbackState = null;
			lock (this)
			{
				CloseConnection();
				if (ConnectionLost != null
					&& !m_userclose
					&& !m_catchedOnlost
					&& m_isAsyncNetCallBack)
				{
					conLost=ConnectionLost;
					callbackState = m_connectcallbackState;
				}
			}

			//此处不能对自己加锁，因为回调函数要求该对象的父对象加锁，造成死锁，gtw
			if (conLost!=null)
			{
				conLost(new ConnectionArgs(this, 0), new ErrorArgs(etype, message), callbackState);
			}

			lock(this)
			{
				if (conLost==null)
				{
					if (NetStateChange.State_Connect_Success ==
						(NetStateChange)(m_netStateChange & (int)NetStateChange.State_Connect_Success))
					{
						m_netStateChange &= (~(int)NetStateChange.State_Connect_Success);
						m_netStateChange |= (int)NetStateChange.State_Connect_Failed;
					}
					else
					{
						m_netStateChange |= (int)NetStateChange.State_Connect_Lost;
					}
				}

				m_catchedOnlost = true;
			}
		}



		internal void OnConnectedCallback(Object state)
		{
			if (Connected != null)
			{
				Connected(new ConnectionArgs(this, 0), state);
			}
		}

		protected void SetSocketDefaultOption()
		{
			m_socket.Blocking = false;
			m_socket.NoDelay = true;
			m_socket.LingerState = new LingerOption(true, 0);
            //unity不支持此选项
			//m_socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.KeepAlive, 1);
            m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 32768);
            m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 0);
			m_socket.ReceiveBufferSize = 32768;
			m_socket.SendBufferSize = 32768;
		}

		protected abstract void CreateSocket();
		protected abstract void StartSecureValidate();
		protected abstract void OnReceivedDataCallBack(byte[] data, int length);

		public abstract void SendPacket(Packet packet, bool encrypt_if_need);
	}
}
