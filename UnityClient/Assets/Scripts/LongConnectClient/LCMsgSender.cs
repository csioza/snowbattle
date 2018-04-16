//长连接消息发送器 added by luozj
using System;
using UnityEngine;

public class LCMsgSender
{
    #region Singleton
    private static LCMsgSender m_singleton;
    public static LCMsgSender Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new LCMsgSender();
            }
            return m_singleton;
        }
    }
    #endregion

    bool m_isTryingConn = false;
    public bool IsTryingConn
    {
        get { return m_isTryingConn; }
        set { m_isTryingConn = value; }
    }
    //缓存消息
    MessageBlock m_cacheMsg = null;
    //长连接服务器（BattleServer）的ip和port
    public string BS_IP { get; private set; }
    public int BS_Port { get; private set; }

    //长连接消息接收器
    LCMsgReceiver m_lcMsgReceiver = null;

    public LCMsgSender()
    {
        //注册，用来接收消息回应
        PropertyNotifyManager.Singleton.RegisterReceiver((int)MVCPropertyID.enMessageRespond, OnPropertyChanged);
    }

    public void Init(string ip, int port)
    {
        BS_IP = ip;
        BS_Port = port;

        m_lcMsgReceiver = new LCMsgReceiver();
        m_lcMsgReceiver.Init();
    }

    public void ReConnect()
    {
        ClientNet.Singleton.StartLongConnect();
    }

    //发送消息
    public void SendMsg(MessageBlock msg)
    {
        if (GameSettings.Singleton.m_isSingle) return;

        Send(msg);
    }

    //发送缓存消息
    public void SendCacheMsg()
    {
        Send(m_cacheMsg);
    }
    //发送消息的内部实现
    void Send(MessageBlock msg)
    {
        if (msg == null) return;

        m_cacheMsg = msg;

        if (ClientNet.Singleton.IsLongConnected)
        {
            CppMessageBlock cppMsg = (CppMessageBlock)msg;
            ClientNet.Singleton.SendPacket(cppMsg.packet, ClientNet.ENConnectionType.enLongConnect);
        }
        else if (IsTryingConn)
        {
        }
        else
        {
            ClientNet.Singleton.StartLongConnect();
        }
    }

    //消息回应
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (m_cacheMsg == null)
        {
            return;
        }

        MessageRespond respond = (MessageRespond)eventObj;
        if (respond.MessageID == m_cacheMsg.MessageID)
        {//清除缓存消息
            m_cacheMsg = null;
        }
    }
}