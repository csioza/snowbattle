using System;

namespace NGE.Network
{
	/// <summary>
	/// 网络连接状态
	/// </summary>
	public enum ConnectionState
	{
		/// <summary>
		/// 未连接.
		/// </summary>
		Uninitialised,
		/// <summary>
		/// 已经连接成功.
		/// </summary>
		Connected,
		/// <summary>
		/// 连接失败.
		/// </summary>
		ConnectionFailed,
		/// <summary>
		/// 安全连接握手
		/// </summary>
		ConnectionSSLInit,
	}

	/// <summary>
	/// 连接事件参数.
	/// </summary>
	public class ConnectionArgs
	{
		/// <summary>
		/// 发生事件的网络连接.
		/// </summary>
		public IConnection Connection
		{
			get
			{
				return m_connection;
			}
		}
		IConnection m_connection;

		public int Id
		{
			get
			{
				return m_id;
			}
		}
		int m_id;

		internal ConnectionArgs(IConnection connection, int id)
		{
			this.m_connection = connection;
			this.m_id = id;
		}
	}

	/// <summary>
	/// 所有连接错误枚举.
	/// </summary>
	public enum ErrorType
	{
		/// <summary>
		/// 无法连接服务器.
		/// </summary>
		ConnectingFailed,
		/// <summary>
		/// 发送数据时出错.
		/// </summary>
		SendFailed,
		/// <summary>
		/// 接受数据时出错.
		/// </summary>
		ReceiveFailed,
		/// <summary>
		/// 连接断开.
		/// </summary>
		ConnectionLost
	}

	/// <summary>
	/// 错误事件参数
	/// </summary>
	public class ErrorArgs
	{
		/// <summary>
		/// 错误类型.
		/// </summary>
		public ErrorType ErrorType
		{
			get
			{
				return m_errorType;
			}
		}
		ErrorType m_errorType;

		/// <summary>
		/// 错误的描述.
		/// </summary>
		public string Description
		{
			get
			{
				return m_description;
			}
		}
		string m_description;

		internal ErrorArgs(ErrorType type, string description)
		{
			this.m_errorType = type;
			this.m_description = description;
		}
	}

	/// <summary>
	/// Connected 事件(success)发生时的回调委托.
	/// </summary>
	public delegate void ConnectionCallback(ConnectionArgs args, Object state);

	/// <summary>
	/// 连接出错时的回调委托.
	/// </summary>
	public delegate void ConnectionFailedCallback(ConnectionArgs args, ErrorArgs error, Object state);
}
