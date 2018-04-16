using System;
using System.Net;

namespace NGE.Network
{
	/// <summary>
	/// 网络连接抽象接口
	/// </summary>
	public interface IConnection
	{
		/// <summary>
		/// (异步)成功连接到远端后回调事件
		/// </summary>
		event ConnectionCallback Connected;
		/// <summary>
		/// (异步)连接失败时回调事件
		/// </summary>
		event ConnectionFailedCallback ConnectionFailed;
		/// <summary>
		/// (异步)连接断开的回调事件
		/// </summary>
		event ConnectionFailedCallback ConnectionLost;
		/// <summary>
		/// 消息反应器
		/// </summary>
		PacketReactor Reactor { get;set;}
		/// <summary>
		/// 是否采用异步事件调用
		/// </summary>
		bool UseAsyncNetCallBack { get;set;}

		/// <summary>
		/// 连接状态.
		/// </summary>
		ConnectionState State { get; }
		/// <summary>
		/// 连接时间
		/// </summary>
		DateTime ConnectedTime { get;}

		/// <summary>
		/// 总共发送的数据大小(字节)
		/// </summary>
		long TotalSent { get;}

		/// <summary>
		/// 总共接受的数据大小(字节)
		/// </summary>
		long TotalReceived { get;}

		/// <summary>
		/// 发送数据频率
		/// </summary>
		int SentRate { get;}

		/// <summary>
		/// 接受数据频率
		/// </summary>
		int ReceivedRate { get;}
		/// <summary>
		/// 是否发送完毕
		/// </summary>
		bool IsSendOver { get;}
		/// <summary>
		/// 是否连接正常
		/// </summary>
		bool IsConnected { get;}

		/// <summary>
		/// 本地IP地址.
		/// </summary>
		IPEndPoint LocalIP { get; }

		/// <summary>
		/// 远端IP地址.
		/// </summary>
		IPEndPoint RemoteIP { get; }


		/// <summary>
		/// 开始连接远端IP地址
		/// </summary>
		/// <param name="hostName">目的主机名(可以是ip地址和DNS名)</param>
		/// <param name="port">目的主机端口</param>
		/// <param name="secure">是否采用安全连接</param>
		/// <param name="state">回调参数</param>
		/// <returns></returns>
		int Connect(string hostName, int port, bool secure, object state);

		int Connect(IPEndPoint endPoint, bool secure, object state);

		/// <summary>
		/// 发送消息包
		/// </summary>
		/// <param name="packet"></param>
		/// <param name="encrypt_if_need"></param>
		void SendPacket(Packet packet, bool encrypt_if_need);
		/// <summary>
		/// 同步更新函数
		/// </summary>
		/// <param name="state"></param>
		void Update(object state);

		/// <summary>
		/// 关闭网络连接并且释放所有资源
		/// </summary>
		void Release();
		/// <summary>
		/// 关闭网络连接
		/// </summary>
		void CloseConnection();
	}
}

