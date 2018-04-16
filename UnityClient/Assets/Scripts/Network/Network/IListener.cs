using System;

namespace NGE.Network
{
	/// <summary>
	/// 网络侦听器接口
	/// </summary>
	public interface IListener
	{
		/// <summary>
		/// 接受客户端连接时回调事件
		/// </summary>
		event ListenerCallback Accepted;

		/// <summary>
		/// 是否采用异步事件调用
		/// </summary>
		bool UseAsyncNetCallBack { get;set;}

		/// <summary>
		/// 开始侦听
		/// </summary>
		/// <param name="port">端口</param>
		/// <param name="state">回调参数</param>
		void Listen(int port, object state);
		/// <summary>
		/// 同步更新函数
		/// </summary>
		/// <param name="state"></param>
		void Update(object state);
		/// <summary>
		/// 停止侦听
		/// </summary>
		void Stop();
		/// <summary>
		/// 关闭侦听器，这将关闭所有客户端连接
		/// </summary>
		void Release();
	}
}
