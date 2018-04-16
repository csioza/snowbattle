using NGE.Network;
using System;
using System.Collections.Generic;
using UnityEngine;


public class VirtualServer
{
	#region Singleton
	static VirtualServer m_singleton;
	static public VirtualServer Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new VirtualServer();
			}
			return m_singleton;
		}
	}
	#endregion

	private Dictionary<short, Func<PacketReader, bool>> m_msgFuncList = new Dictionary<short, Func<PacketReader, bool>>();
	public VirtualServer()
    {
        m_msgFuncList[GamePacketID.msgLogin_C2S] = MsgLogin;
        m_msgFuncList[GamePacketID.msgPlayerCreate_C2S] = MsgPlayerCreate;
        m_msgFuncList[GamePacketID.msgSceneSwitch_C2S] = MsgSceneSwitch;
        m_msgFuncList[GamePacketID.msgFriendAdd_C2S] = MsgFriendAdd;
        m_msgFuncList[GamePacketID.msgFriendAdd_Agree_C2S] = MsgFriendAdd_Agree;
        m_msgFuncList[GamePacketID.msgFriendAdd_Refuse_C2S] = MsgFriendAdd_Refuse;
        m_msgFuncList[GamePacketID.msgFriendDel_C2S] = MsgFriendDel;
        m_msgFuncList[GamePacketID.msgFriendChat_C2S] = MsgFriendChat;
	}

	public void SendToClient(Packet p)
	{
        if (GameSettings.Singleton.m_isSingle)
        {
            PacketHandler handler = ClientNet.Singleton.ShortConnect.Reactor.GetHandler(p.PacketID);
            if (null != handler)
            {
                PacketReader reader = new PacketReader(p.ToArray(), p.Length, 0, false, null);
                handler.OnReceive(reader, null);
            }
        }
        else
        {
            //发送给客户端
        }
	}

	public bool ProcessMessage(Packet p)
	{
		Func<PacketReader, bool> func = null;
		if (m_msgFuncList.TryGetValue(p.PacketID, out func))
		{
			PacketReader reader = new PacketReader(p.ToArray(), p.Length, 0, false, null);
			return func(reader);
		}
		else
		{
            Debug.LogWarning("packet error,id:" + p.PacketID);
			return false;
		}
	}
    //登录
    private bool MsgLogin(PacketReader p)
    {
        string user = p.ReadAscii(64);
        string pass = p.ReadAscii(32);

        // 通知客户端
        Packet msgPacket = new Packet(GamePacketID.msgLogin_S2C);
        SendToClient(msgPacket);
        return true;
    }
    //创建角色
    private bool MsgPlayerCreate(PacketReader p)
    {
        string name = p.ReadUTF8(64);

        // 通知客户端
        Packet msgPacket = new Packet(GamePacketID.msgPlayerCreate_S2C);
        SendToClient(msgPacket);
        return true;
    }
    //场景切换
    private bool MsgSceneSwitch(PacketReader p)
    {
        int oldID = p.ReadInt32();
        int newID = p.ReadInt32();
        // 做验证

        // 通知客户端
        Packet msgPacket = new Packet(GamePacketID.msgSceneSwitch_S2C);
        msgPacket.Writer.Write(newID);
        SendToClient(msgPacket);
        return true;
    }
    //朋友相关
    private bool MsgFriendAdd(PacketReader p)
    {
        string myName = p.ReadUTF8(64);
        string friendName = p.ReadUTF8(64);
        return true;
    }
    private bool MsgFriendAdd_Agree(PacketReader p)
    {
        string myName = p.ReadUTF8(64);
        string friendName = p.ReadUTF8(64);
        return true;
    }
    private bool MsgFriendAdd_Refuse(PacketReader p)
    {
        string myName = p.ReadUTF8(64);
        string friendName = p.ReadUTF8(64);
        string reason = p.ReadUTF8(256);
        return true;
    }
    private bool MsgFriendDel(PacketReader p)
    {
        string myName = p.ReadUTF8(64);
        string friendName = p.ReadUTF8(64);
        return true;
    }
    private bool MsgFriendChat(PacketReader p)
    {
        string myName = p.ReadUTF8(64);
        string friendName = p.ReadUTF8(64);
        string info = p.ReadUTF8(256);
        return true;
    }
};