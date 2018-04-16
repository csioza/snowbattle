using System;

namespace NGE.Network
{
	/// <summary>
	/// 侦听状态
	/// </summary>
	public enum ListenerState
	{
		/// <summary>
		/// 未初始化
		/// </summary>
		Uninitialised,
		/// <summary>
		/// 成功开始侦听.
		/// </summary>
		Listening,
		/// <summary>
		/// 侦听失败.
		/// </summary>
		ListeningFailed
	}

	/// <summary>
	/// 侦听事件参数.
	/// </summary>
	public class ListenerArgs
	{
		/// <summary>
		/// 客户端连接.
		/// </summary>
		public IConnection Client
		{
			get
			{
				return m_client;
			}
		}
		IConnection m_client;

		/// <summary>
		/// 操作序列号.
		/// </summary>
		public int Id
		{
			get
			{
				return m_id;
			}
		}
		int m_id;

		public IListener Server
		{
			get
			{
				return m_server;
			}
		}
		IListener m_server;

		internal ListenerArgs(IListener server, int id, IConnection client)
		{
			this.m_server = server;
			this.m_client = client;
			this.m_id = id;
		}

	}

	/// <summary>
	/// 侦听回调委托.
	/// </summary>
	public delegate void ListenerCallback(ListenerArgs args, Object state);
}
