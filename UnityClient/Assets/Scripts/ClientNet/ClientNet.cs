using UnityEngine;
using System.Collections;
using NGE.Network;

public class ClientNet
{
	#region Singleton
	static ClientNet m_singleton;
	static public ClientNet Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new ClientNet();
			}
			return m_singleton;
		}
	}
	#endregion

    //������
    #region ShortConnect
    private TcpConnection m_shortConnect;
    public TcpConnection ShortConnect
    {
        get
        {
            if (null == m_shortConnect)
            {
                m_shortConnect = new TcpConnection();
                m_shortConnect.Reactor = new PacketReactor();
                m_shortConnect.UseAsyncNetCallBack = false;
            }
            return m_shortConnect;
        } 
    }
    #endregion
    //������
    #region LongConnect
    private TcpConnection m_longConnect;
    public TcpConnection LongConnect
    {
        get
        {
            if (null == m_longConnect)
            {
                m_longConnect = new TcpConnection();
                m_longConnect.Reactor = new PacketReactor();
                m_longConnect.UseAsyncNetCallBack = false;
            }
            return m_longConnect;
        }
    }
    #endregion
    //��������
    public enum ENConnectionType
    {
        enShortConnect,
        enLongConnect,
    }
    public ENConnectionType m_connectionType = ENConnectionType.enShortConnect;
    public bool IsConnected { get { return ShortConnect.IsConnected; } }
    //�Ƿ�������
    public bool IsLongConnecting { get { return m_connectionType == ENConnectionType.enLongConnect; } }
    //�Ƿ��Ѿ���������
    public bool IsLongConnected { get { return LongConnect.IsConnected; } }

    public ClientNet()
    {
    }
	// Use this for initialization
	public void Start()
	{
        if (!GameSettings.Singleton.m_isSingle)
        {
            ShortConnect.Connect(GameSettings.Singleton.ServerIP, 10124, false);
        }
	}

	public void SendPacket(Packet p, ENConnectionType type = ENConnectionType.enShortConnect)
	{
        if (GameSettings.Singleton.m_isSingle)
        {
            VirtualServer.Singleton.ProcessMessage(p);
        }
        else
        {
            switch (type)
            {
                case ENConnectionType.enLongConnect:
                    LongConnect.SendPacket(p);
                    break;
                case ENConnectionType.enShortConnect:
                default:
                    ShortConnect.SendPacket(p);
                    break;
            }
            Debug.Log("Send Msg ,id = " + p.PacketID + " type:" + type);
        }
	}
	// Update is called once per frame
	public void Update()
	{
        if (!GameSettings.Singleton.m_isSingle)
        {
            ShortConnect.Update(this);
            LongConnect.Update(this);
        }
	}
    // ����ϢpacketID����reactor���б���
	public void RegisterHandler(int packetID, OnPacketReceive receive)
	{
        ShortConnect.Reactor.RegisterHandler(packetID, receive);
	}

    public void LCRegisterHandler(int packetID, OnPacketReceive receive)
    {
        LongConnect.Reactor.RegisterHandler(packetID, receive);
    }

    public void StartLongConnect()
    {
        Debug.LogWarning("bsIP:" + LCMsgSender.Singleton.BS_IP + ",bsPort:" + LCMsgSender.Singleton.BS_Port);
        LongConnect.Connect(LCMsgSender.Singleton.BS_IP, LCMsgSender.Singleton.BS_Port, false);
        LCMsgSender.Singleton.IsTryingConn = true;
    }
}
