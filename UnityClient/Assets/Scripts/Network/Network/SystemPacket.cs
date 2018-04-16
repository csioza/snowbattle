using System;
using System.Text;

namespace NGE.Network
{
	public class SimplePacket : Packet//id为0的消息包
	{
		public SimplePacket()
			: base(BasicPacketID.Simple_test)
		{
			m_writer.Write(12345678);
			m_writer.Write(true);
			m_writer.Write((short)20000);
			m_writer.WriteString("this is a simple packet,fun ^_^.这是简体中文，這是繁體中文");
		}

		public SimplePacket(int loopcount)
			: base(BasicPacketID.Simple_test)
		{
			m_writer.Write(12345678);
			m_writer.Write(true);
			m_writer.Write((short)20000);
			for (int i = 0; i < loopcount; i++)
			{
				m_writer.WriteString("this is a simple packet,fun ^_^.这是简体中文，這是繁體中文");
			}
		}
	}

	public class SimpleEmptyPacket : Packet//空消息
	{
		public SimpleEmptyPacket()
			: base(BasicPacketID.SimpleEmpty_test)
		{

		}
	}

	public class SendPublicKeyPacket : Packet//id =1
	{
		public SendPublicKeyPacket(int encryptflag, byte[] key)
			: base(BasicPacketID.SendPublicKey)
		{
			m_writer.Write(encryptflag);
			m_writer.Write(key, 0, key.Length);
		}
	}

	public class SendSessionKeyPacket : Packet//id=2
	{
		public SendSessionKeyPacket(byte[] key)
			: base(BasicPacketID.SendSessionKey)
		{
			m_writer.Write(key, 0, key.Length);
		}
	}

	/// <summary>
	/// 命令行消息
	/// </summary>
	public class CommandLinePacket : Packet//id=3
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="execid">执行流水号,返回消息将返回这个值,方便发送方判断</param>
		/// <param name="commandtext">命令文本</param>
		public CommandLinePacket(int execid, string commandtext)
			: base(BasicPacketID.CommandLine)
		{
			m_writer.Write(execid);
			m_writer.WriteString(commandtext);
		}
	}

	/// <summary>
	/// 命令行执行结果消息
	/// </summary>
	public class CommandLineResultPacket : Packet//id=4
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="execid">执行流水号 = 发送消息的流水号</param>
		/// <param name="resultvalue">返回值</param>
		/// <param name="resulttext">返回文本</param>
		public CommandLineResultPacket(int execid, int resultvalue, string resulttext)
			: base(BasicPacketID.CommandLineResult)
		{
			m_writer.Write(execid);
			m_writer.Write(resultvalue);
			m_writer.WriteString(resulttext);
		}
	}
}
