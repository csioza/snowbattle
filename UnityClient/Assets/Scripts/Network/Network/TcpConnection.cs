
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using NGE.Crypto;
using NGE.Util;
using UnityEngine;
#region disableWarning
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#endregion

namespace NGE.Network
{
	/// <summary>
	/// Tcp 连接类
	/// </summary>
	public sealed class TcpConnection : SocketConnection
	{
		short m_magicnumOfCurrentPacket;
		int m_lengthOfCurrentPacket;
		int m_packetIDOfCurrentPacket;
		int m_dispatcherIDOfCurrentPacket;

		byte[] m_lastPacketData;

		[ThreadStatic]
		static byte[] tls_sendSecureBuffer = new byte[Packet.MaxLength];
		[ThreadStatic]
		static byte[] tls_decompressbuffer = new byte[Packet.MaxLength];

		int m_lastRecvPacketSerialNumber = 0;
		int m_lastSendPacketSerialNumber = 0;
#if _NC_Compress
		TeaEncryption m_encrypt;
		TeaEncryption m_encryptfirst;
#else
        object m_encrypt;
        object m_encryptfirst;
#endif
		byte[] m_firstkey;
		object m_sendlock = new object();
		//bool m_delayHandlePacket = false;
		bool m_compressneedchecksum = true;


		Queue<PacketReader> m_recvqueue;

		public TcpConnection()
			: this(null, false)
		{
			if (m_socket == null)
				CreateSocket();
		}

		internal TcpConnection(Socket socket, bool secure)
			: base(socket, secure)
		{
			m_lastPacketData = new byte[Packet.MaxLength];
			m_recvqueue = new Queue<PacketReader>();
			if (m_socket != null)
				SetSocketDefaultOption();
		}


		/// <summary>
		/// 发送一个消息包
		/// </summary>
		/// <param name="packet"></param>
		/// <returns></returns>
		public void SendPacket(Packet packet)
		{
			SendPacket(packet, false);
		}

		/// <summary>
		/// 发送一个消息包
		/// </summary>
		/// <param name="packet"></param>
		/// <param name="needencrypt">是否加密</param>
		/// <returns></returns>
		public override void SendPacket(Packet packet, bool encrypt_if_need)
		{
            if (m_sendlock == null)
            {
                UnityEngine.Debug.Log("m_sendlock == null");
				return;
            }

			lock (this)
			{
				packet.SerialNumber = m_lastSendPacketSerialNumber;
				m_lastSendPacketSerialNumber++;

				byte[] data = packet.ToArray();
				int datalength = packet.Length;
				PacketFlag packetflag = packet.PacketFlag;
				//加密,压缩消息包
				if (m_secure_connection && m_connectionState == ConnectionState.Connected)
				{
					if (tls_sendSecureBuffer == null)
						tls_sendSecureBuffer = new byte[Packet.MaxLength];
					//压缩
					bool compressed = false;
#if _NC_Compress
					if (data.Length > Packet.NeedCompressMinLength)
					{
						int outlen = Compress.LZO.Compress(data, Packet.NoCryptHeaderLength, data.Length - Packet.NoCryptHeaderLength,
							tls_sendSecureBuffer, Packet.NoCryptHeaderLength);
						if (outlen <= (data.Length - 20))//压缩有实际效果
						{
							//Buffer.BlockCopy(m_sendSecureBuffer, 0, data, Packet.NoCryptHeaderLength, outlen);
							datalength = outlen + Packet.NoCryptHeaderLength;
							Buffer.BlockCopy(data, 0, tls_sendSecureBuffer, 0, Packet.NoCryptHeaderLength);
							if (m_compressneedchecksum)
							{
								uint crccheck = HashHelp.CRC32hash(tls_sendSecureBuffer, Packet.NoCryptHeaderLength, outlen);
								ArrayUtility.SetInt(tls_sendSecureBuffer, (int)crccheck, datalength);
								datalength += 4;
							}
							packetflag |= PacketFlag.Compressed;
							data = tls_sendSecureBuffer;
							compressed = true;
						}
					}
					if (encrypt_if_need && m_encrypt != null)
					{
						if (!compressed)
						{
							Buffer.BlockCopy(data, 0, tls_sendSecureBuffer, 0, datalength);
							data = tls_sendSecureBuffer;
						}
						m_encrypt.Encrypt(data, Packet.NoCryptHeaderLength, datalength - Packet.NoCryptHeaderLength);
						packetflag |= PacketFlag.Encrypted;
					}
#endif
				}

				//设置消息长度
                ArrayUtility.SetByte(data, (byte)packetflag, Packet.OffsetFlag);
				ArrayUtility.SetShort(data, (short)datalength, Packet.OffsetLength);
				this.Send(data, datalength);
			}
		}

		public void HandleAllReceivedPacket(object state)
		{
			try
			{
				if (!m_isAsyncNetCallBack && m_reactor != null)
				{
					while (m_recvqueue.Count > 0)
					{
						PacketReader reader;
						lock (m_recvqueue)
							reader = m_recvqueue.Dequeue();
						PacketHandler handler = m_reactor.GetHandler(reader.PacketID);
						if (handler == null)
						{
							return;
						}
						handler.OnReceive(reader, state);
					}
				}
			}
			catch (Exception e)
			{
				//Debug.TraceException(e);
                UnityEngine.Debug.LogError(e.Message);
                UnityEngine.Debug.LogError(e.StackTrace);
                DebugLog.Singleton.OnShowLog("HandleAllReceivedPacket exception: " + e.Message + " " + e.StackTrace);
			}
		}

		//把底层传来的数据分多次提交给使用者。
		//每次提交的数据称为包，其大小记录在包头，所有的包顺序排列。
		//如果现存的数据不足包长，则等待后续数据。
		protected override void OnReceivedDataCallBack(byte[] data/*整个包的起始地址，已在下层进行拼接*/, int length)
		{
			if (m_connectionState == ConnectionState.Uninitialised || length <= 0)
				return;
			m_iSegmentSize += length;//合并收到的包
			int startOffset = 0;//amount for the submits to user.
			int rawpacketlength = 0;//user packet size

			while (m_iSegmentSize >= Packet.HeaderSize)
			{
				//从消息头中读出包长度
				m_lengthOfCurrentPacket = Util.ArrayUtility.GetShort(data, startOffset+Packet.OffsetLength);
				//包长错误，忽略此包
				if (m_lengthOfCurrentPacket < Packet.HeaderSize || m_lengthOfCurrentPacket > Packet.MaxLength)
				{
					m_iSegmentSize = 0;
					return;
				}
				//数据不够组成一个消息包，等待后续数据
				if (m_iSegmentSize < m_lengthOfCurrentPacket)
					break;
				rawpacketlength = m_lengthOfCurrentPacket;

                //按顺序读出包ID,DispatcherID,和标记位
                m_packetIDOfCurrentPacket = ArrayUtility.GetShort(data, startOffset + Packet.OffsetPacketID);
                m_dispatcherIDOfCurrentPacket = ArrayUtility.GetInt(data, Packet.OffsetDispatcherID + startOffset);
                m_magicnumOfCurrentPacket = Util.ArrayUtility.GetByte(data, startOffset + Packet.OffsetFlag);

				//HandleOneRawPacket();
				//处理明文数据（不加密也不压缩)
				if ((m_magicnumOfCurrentPacket & (short)PacketFlag.Encrypted) == 0 &&
					(m_magicnumOfCurrentPacket & (short)PacketFlag.Compressed) == 0)
				{
					//int packetserialnum = ArrayUtility.GetInt(data, 8 + startOffset);
					//包的序列号错误
                    //if (packetserialnum != m_lastRecvPacketSerialNumber)
                    //{
                    //    packetserialnum = m_lastRecvPacketSerialNumber;//why?
                    //    if (m_encrypt != null)//非加密连接，出错就不管了,警告一下
                    //    {
                    //        CloseConnection();
                    //        return;
                    //    }
                    //    Console.WriteLine("Error:TcpConnection packetserialnum != m_lastRecvPacketSerialNumber!");
                    //}
					m_lastRecvPacketSerialNumber++;
					ProcessPacket(m_packetIDOfCurrentPacket, data, m_lengthOfCurrentPacket, startOffset);
				}
				else//解密解压消息
				{
					int securedatalength = m_lengthOfCurrentPacket - Packet.NoCryptHeaderLength;
					if (securedatalength <= 0 || m_encrypt == null)
					{
						CloseConnection();
						return;
					}
					System.Buffer.BlockCopy(data, startOffset,
                        m_lastPacketData, 0, m_lengthOfCurrentPacket);
#if _NC_Compress
					//解密
					if ((m_magicnumOfCurrentPacket & (short)PacketFlag.Encrypted) == (short)PacketFlag.Encrypted)
					{
						m_encrypt.Decrypt(m_lastPacketData, Packet.NoCryptHeaderLength, securedatalength);
						m_magicnumOfCurrentPacket &= ~(short)PacketFlag.Encrypted;
                    }
					//解压
					if ((m_magicnumOfCurrentPacket & (short)PacketFlag.Compressed) == (short)PacketFlag.Compressed)
					{
						if (m_compressneedchecksum)
						{//check src32
							uint crccheck = (uint)ArrayUtility.GetInt(m_lastPacketData, securedatalength);
							securedatalength -= 4;
							uint crccomp = HashHelp.CRC32hash(m_lastPacketData, Packet.NoCryptHeaderLength, securedatalength);
							if (crccheck != crccomp)
							{
								CloseConnection();
								return;
							}
						}
						if (tls_decompressbuffer == null)
							tls_decompressbuffer = new byte[Packet.MaxLength];
						int outlen = Compress.LZO.Decompress(m_lastPacketData, Packet.NoCryptHeaderLength, securedatalength, tls_decompressbuffer, 0);
						if (outlen <= Packet.HeaderSize - Packet.NoCryptHeaderLength || outlen >= Packet.MaxLength - Packet.NoCryptHeaderLength)//解压失败
						{
							CloseConnection();
							return;
						}
						m_magicnumOfCurrentPacket &= ~(short)PacketFlag.Compressed;
						Buffer.BlockCopy(tls_decompressbuffer, 0, m_lastPacketData, Packet.NoCryptHeaderLength, outlen);
						m_lengthOfCurrentPacket = (short)(Packet.NoCryptHeaderLength + outlen);
					}
#endif
                    m_packetIDOfCurrentPacket = ArrayUtility.GetInt(m_lastPacketData, 4);
					int packetserialnum = ArrayUtility.GetInt(m_lastPacketData, 8);
					if (packetserialnum != m_lastRecvPacketSerialNumber)
					{
						packetserialnum = m_lastRecvPacketSerialNumber;//why?
						CloseConnection();
						return;
					}
					m_lastRecvPacketSerialNumber++;
					ArrayUtility.SetShort(m_lastPacketData, m_magicnumOfCurrentPacket, 0);
					m_dispatcherIDOfCurrentPacket = ArrayUtility.GetInt(m_lastPacketData, 12);
					ProcessPacket(m_packetIDOfCurrentPacket, m_lastPacketData, m_lengthOfCurrentPacket, 0);
				}

				if (m_connectionState == ConnectionState.Uninitialised)
					return;

				m_iSegmentSize -= rawpacketlength;
				startOffset += rawpacketlength;

			}
			if (startOffset > 0 && m_iSegmentSize > 0)
				System.Buffer.BlockCopy(data, startOffset, m_RecvBuffer, 0, m_iSegmentSize);
		}

		private void ProcessPacket(int packetID, byte[] data, int packetlength, int startoffset)
		{
            //UnityEngine.Debug.Log("Packet id = " + packetID);
			if (m_secure_connection &&
				m_connectionState != ConnectionState.Connected &&
				(packetID != 1 && packetID != 2))//没有成功安全连接就收到别的消息!
			{
				CloseConnection();
				return;
			}
			if (packetID == BasicPacketID.SendPublicKey)
			{
				OnReceivePublicKey(new PacketReader(data, packetlength, startoffset, false, null));
			}
			else if (packetID == BasicPacketID.SendSessionKey)
			{
				OnReceiveSessionKey(new PacketReader(data, packetlength, startoffset, false, null));
			}
			else //if(m_reactor != null)
			{
				if (!m_isAsyncNetCallBack)
				{
					byte[] newdata = new byte[packetlength];
					Buffer.BlockCopy(data, startoffset, newdata, 0, packetlength);
					lock (m_recvqueue)
						m_recvqueue.Enqueue(new PacketReader(newdata, packetlength, 0, true, this));
				}
				else
				{
					if (m_reactor == null)
						return;
					// 接收队列非空，处理 [10/17/2006]
					HandleAllReceivedPacket(m_PacketHandlerCallbackArgument);
					PacketHandler handler = m_reactor.GetHandler(packetID);
					if (handler == null)
					{
						return;
					}
					handler.OnReceive(new PacketReader(data, packetlength, startoffset, false, this), m_PacketHandlerCallbackArgument);
				}
			}
		}

		//收到公钥数据
		private void OnReceivePublicKey(PacketReader reader)
		{
#if _NC_Compress
			if (m_secure_connection == false || reader.Size != 148)
			{
				this.CloseConnection();
				return;
			}
			int secflag = reader.ReadInt32();
			m_compressneedchecksum = (secflag == (int)PacketFlag.Compress_Need_Checksum);
			byte[] firstkey = reader.ReadBuffer(16);
			byte[] checksum = reader.ReadBuffer(16);

			if (m_encryptfirst == null)
				m_encryptfirst = new TeaEncryption();
			m_encryptfirst.Key = firstkey;
			m_encryptfirst.Decrypt(checksum, 0, checksum.Length);
			for (int i = 0; i < 16; i++)
			{
				if (firstkey[i] != checksum[i])
				{
					CloseConnection();
					return;
				}
			}
			if (m_encrypt == null)
			{
				m_encrypt = new TeaEncryption();
				m_encrypt.GenericKey();
			}
			byte[] pairkey = new byte[16];
			Array.Copy(m_encrypt.Key, pairkey, 16);
			m_encryptfirst.Encrypt(pairkey, 0, 16);
			byte[] sendkey = new byte[128];
			Utility.Randomizer.NextBytes(sendkey);
			Buffer.BlockCopy(pairkey, 0, sendkey, 0, 16);
			m_encryptfirst.Decrypt(pairkey, 0, 16);
			SendSessionKeyPacket packet = new SendSessionKeyPacket(sendkey);
			SendPacket(packet, false);
			firstkey = null;//clear temp key
			m_connectionState = ConnectionState.Connected;
			//callback
			if (m_listener != null)
				m_listener.OnSecureAccepted(this);
#endif
		}

		//收到会话密钥数据
		private void OnReceiveSessionKey(PacketReader reader)
		{
#if _NC_Compress
			if (m_secure_connection == false)
			{
				CloseConnection();
				return;
			}
			byte[] sessionkey = reader.ReadBuffer(16);
			if (reader.Size != 144 || sessionkey == null)
			{
				CloseConnection();
				return;
			}
			if (m_encryptfirst == null)
			{
				m_encryptfirst = new TeaEncryption();
				m_encryptfirst.Key = m_firstkey;
			}

			m_encryptfirst.Decrypt(sessionkey, 0, 16);
			if (m_encrypt == null)
				m_encrypt = new TeaEncryption();

			m_encrypt.Key = sessionkey;
			m_connectionState = ConnectionState.Connected;

			if (m_isAsyncNetCallBack)
			{
				OnConnectedCallback(null);
			}
			else
			{
				lock (this)
				{
					m_netStateChange |= (int)NetStateChange.State_Connect_Success;
				}
			}
#endif
		}

		//发送公钥数据
		protected override void StartSecureValidate()
		{
#if _NC_Compress
			m_firstkey = new byte[16];
			byte[] keydata = new byte[128];
			Utility.Randomizer.NextBytes(keydata);
			Utility.Randomizer.NextBytes(m_firstkey);
			Buffer.BlockCopy(m_firstkey, 0, keydata, 0, m_firstkey.Length);
			Buffer.BlockCopy(m_firstkey, 0, keydata, m_firstkey.Length, m_firstkey.Length);
			if (m_encryptfirst == null)
			{
				m_encryptfirst = new TeaEncryption();
			}
			m_encryptfirst.Key = m_firstkey;
			m_encryptfirst.Encrypt(keydata, m_firstkey.Length, m_firstkey.Length);
			SendPublicKeyPacket packet = new SendPublicKeyPacket((int)PacketFlag.Compress_Need_Checksum, keydata);
			this.SendPacket(packet, false);
#endif
		}

		protected override void CreateSocket()
		{
			if (m_socket != null)
			{
				try { m_socket.Shutdown(SocketShutdown.Both); }
				catch { }
				try { m_socket.Close(); }
				catch { }
			}
			m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			//m_socket.Blocking = false;
            SetSocketDefaultOption();
		}

		public override void Update(object state)
		{
			if (!object.ReferenceEquals(m_PacketHandlerCallbackArgument, state))
				m_PacketHandlerCallbackArgument = state;
			if (m_isAsyncNetCallBack)
				return;
			base.Update(state);
			if (m_recvqueue == null)
				return;
			HandleAllReceivedPacket(m_PacketHandlerCallbackArgument);
		}

		public override void CloseConnection()
		{
			base.CloseConnection();
			m_lastRecvPacketSerialNumber = 0;
			m_lastSendPacketSerialNumber = 0;
			m_magicnumOfCurrentPacket = 0;
			m_lengthOfCurrentPacket = 0;
			m_packetIDOfCurrentPacket = 0;
			m_dispatcherIDOfCurrentPacket = 0;
		}

		protected override void Dispose(bool disposing)
		{
			m_lastPacketData = null;
			tls_sendSecureBuffer = null;
			m_encrypt = null;
			m_encryptfirst = null;
			m_firstkey = null;
			if (m_recvqueue != null)
				m_recvqueue.Clear();
			m_recvqueue = null;
			m_reactor = null;
			base.Dispose(disposing);
		}
	}
}
