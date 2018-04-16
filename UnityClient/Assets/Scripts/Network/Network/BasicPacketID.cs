using System;

namespace NGE.Network
{
	/// <summary>
	/// 消息id的定义，定义在这里方便大家查看,已经定义的消息id不要轻易改变!!!
	/// </summary>
	public struct BasicPacketID
	{
		//////////////////////////////////////////////////////////////////////////
		// 0~9 系统消息

		/// <summary>
		/// 测试消息
		/// </summary>
		public const int Simple_test = 0;
		/// <summary>
		/// 发送公钥
		/// </summary>
		public const int SendPublicKey = 1;
		/// <summary>
		/// 发送会话密钥
		/// </summary>
		public const int SendSessionKey = 2;
		/// <summary>
		/// 可执行的文本命令行
		/// </summary>
		public const int CommandLine = 3;
		/// <summary>
		/// 命令执行结果
		/// </summary>
		public const int CommandLineResult = 4;
		/// <summary>
		/// 服务器版本信息
		/// </summary>
		public const int CurrentProjectVersion = 5;
		/// <summary>
		/// 空消息
		/// </summary>
		public const int SimpleEmpty_test = 6;
		/// <summary>
		/// 传送文件的消息包
		/// </summary>
		public const int FileTransferPacket = 7;
		/// <summary>
		/// 传送文件的控制消息
		/// </summary>
		public const int FileTransferControlPacket = 8;
		/////////////////////////////////////////////////////////



		/////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 聊天消息(通过聊天服务器处理)
		/// </summary>
		public const int ChatPacket = 10;
		/// <summary>
		/// FrontEnd和ChatServer的控制消息
		/// </summary>
		public const int FrontEndChatServerControl = 12;
		/// <summary>
		/// FrontEnd和CenterServer的控制消息
		/// </summary>
		public const int FrontEndCenterServerControl = 13;
		/// <summary>
		/// FrontEnd和ZoneServer的控制消息
		/// </summary>
		public const int FrontEndZoneServerControl = 14;




		/////////////////////////////////////////////////////////
		//20~29 Log模块使用的消息
		/// <summary>
		/// Log消息
		/// </summary>
		public const int LogMessage = 20;
		/// <summary>
		/// Log消息的控制消息
		/// </summary>
		public const int LogMessageControl = 21;




		//////////////////////////////////////////////////////////////////////////
		//30~39 登陆模块使用的消息
		/// <summary>
		/// 登陆消息，登陆回应消息
		/// </summary>
		public const int AccountLogin = 30;

		/// <summary>
		/// 服务器登陆服务器消息(就是服务器id申请和分配机制)
		/// </summary>
		public const int ServerRegister = 31;

		/// <summary>
		/// Login发给客户端的服务器列表信息
		/// </summary>
		public const int ServerListInfo = 32;
		/// <summary>
		/// 查询帐号点数信息
		/// </summary>
		public const int QueryPoint = 33;
		/// <summary>
		/// 结算点数,扣点
		/// </summary>
		public const int BalancePoint = 34;
		/// <summary>
		/// 封锁帐号一段时间
		/// </summary>
		public const int LockAccount = 35;
		/// <summary>
		/// 服务器注销
		/// </summary>
		public const int ServerUnRegister = 36;
		/// <summary>
		/// 子服务器开始侦听FE
		/// </summary>
		public const int SubServerStartListen = 37;
		/// <summary>
		/// 账号数据库里保存的帐号信息
		/// </summary>
		public const int AccountInformation = 38;
		/// <summary>
		/// Login登陆验证码
		/// </summary>
		public const int enSELoginCheckSum_L2C = 39;



		//////////////////////////////////////////////////////////////////////////
		//40~49 其他服务器之间的控制消息

		/// <summary>
		/// account和login之间的控制消息
		/// </summary>
		public const int AccountLoginControl = 40;
		/// <summary>
		/// GameCenter和Login之间的控制消息
		/// </summary>
		public const int GameCenterLoginControl = 41;
		/// <summary>
		/// 更新游戏密码
		/// </summary>
		public const int UpdataGamePassword = 42;
		/// <summary>
		/// 金币操作
		/// </summary>
		public const int AccountCashOpertion = 43;
		/// <summary>
		/// 金币操作结果
		/// </summary>
		public const int AccountCashOpertionResult = 44;
		/// <summary>
		/// 玩家选择服务器
		/// </summary>
		public const int ClientSelectServer = 45;
		/// <summary>
		/// 登录服务器信息
		/// </summary>
		public const int ClientPassportInfo = 46;
		/// <summary>
		/// 踢人消息
		/// </summary>
		public const int KickSameUser = 47;
		
		/// <summary>
		/// login->client
		/// 更新服务器ip地址信息
		/// </summary>
		public const int AutoUpdateServerInfo = 50;

		/// <summary>
		/// center => account
		/// 查询奖品信息
		/// </summary>
		public const int SELookPrize_S2D = 425;

		/// <summary>
		/// account => center
		/// 回复查询奖品信息
		/// </summary>
		public const int SELookPrizeAck_D2S = 426;

		/// <summary>
		/// center => account
		/// 获得奖品
		/// </summary>
		public const int SEGetPrize_S2D = 427;

		/// <summary>
		/// account => center
		/// 回复获得奖品
		/// </summary>
		public const int SEGetPrizeAck_D2S = 428;

		/// <summary>
		/// center => account
		/// 查询交易魔币
		/// </summary>
		public const int SELookTradeMagicMoneyInfo_S2A = 431;

		/// <summary>
		/// center => account
		/// 查询交易魔币参数信息结果
		/// </summary>
		public const int SELookTradeMagicMoneyInfoAck_S2C = 432;

		/// <summary>
		/// account => center
		/// 查询交易魔币参数信息结果
		/// </summary>
		public const int SELookMagicMoneyBillAck_A2S2C = 434;
		
		/// <summary>
		///  center=>account
		/// 操作魔币挂单
		/// </summary>
		public const int SEOperateMagicMoneyBill_S2A = 436;
		/// <summary>
		///  center=>account
		/// 操作魔币挂单
		/// </summary>
		public const int SEOperateMagicMoneyBillAck_S2C = 437;
		/// <summary>
		///  center=>account
		/// 封号
		/// </summary>
		public const int SEGMForbid_CGS = 3014;
		/// <summary>
		///  center=>account
		/// 封号
		/// </summary>
		public const int SEGMForbid_C2A = 3038;
	}

	//奖品使用参数
	public struct PrizeParam
	{
		public const uint MaxPrizeDescripeLength = 256;
		public const uint MaxLookPrizeAckCount = 10;
		public const int MaxPlayerNameLength = 19;
		public const int MaxItemSocketCount = 3;
	}
}
