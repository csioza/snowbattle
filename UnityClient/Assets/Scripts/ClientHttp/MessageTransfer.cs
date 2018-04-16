using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NGE.Network;

public class MessageBlock
{
    //isLongConnect：是否是长连接
    static AnyObjectPool ms_pool = AnyObjectPoolMgr.Singleton.CreatePool();
    public static MessageBlock CreateMessage(short messageID, bool isLongConnect = false)
    {
        if (ServerSetting.Singleton.serverType == ServerType.enHttpServer)
        {
            return new HttpMessageBlock(messageID);
        }
        else if (ServerSetting.Singleton.serverType == ServerType.enCPPServer)
        {
            CppMessageBlock msg = null;
            // ms_pool.GetObjectFromPool(0) as CppMessageBlock;
            if (msg == null)
            {
                return new CppMessageBlock(messageID, isLongConnect);
            }
            msg.Reset(messageID, isLongConnect);
            return msg;
        }
        return null;
    }
    public static void ReleaseMessage(MessageBlock msg)
    {
        //ms_pool.ReleaseObject(0,msg);
    }
	public short MessageID
	{
		get;
		set;
	}

    public virtual void AddParam(string paramName, bool paramVal)
    {

    }
    public virtual void AddParam(string paramName, byte paramVal)
    {

    }
    public virtual void AddParam(string paramName, double paramVal)
    {

    }
    public virtual void AddParam(string paramName, float paramVal)
    {

    }
    public virtual void AddParam(string paramName, int paramVal)
    {

    }
    public virtual void AddParam(string paramName, long paramVal)
    {

    }   
    public virtual void AddParam(string paramName, short paramVal)
    {

    }
	public virtual void AddArray(string paramName, List<int> paramVal,int writeCount = 0)
	{

	}
    public virtual void AddString(string paramName, string paramVal)
    {

    }
    public virtual void AddString(string paramName, string paramVal, int size)
    {

    }

	public virtual void AddParam(string paramName, MailItem item)
	{

	}

}

public class CppMessageBlock : MessageBlock
{
    public Packet packet;
    //isLongConnect：是否是长连接
    public CppMessageBlock(short messageID, bool isLongConnect = false)
    {
        packet = new Packet(messageID, isLongConnect);
		MessageID = messageID;
    }
    public void Reset(short messageID, bool isLongConnect)
    {
        packet.Reset(messageID, isLongConnect);
        MessageID = messageID;
    }


    public override void AddParam(string paramName, bool paramVal)
    {
        packet.m_writer.Write(paramVal);
    }
    public override void AddParam(string paramName, byte paramVal)
    {
        packet.m_writer.Write(paramVal);
    }
    public override void AddParam(string paramName, double paramVal)
    {
        packet.m_writer.Write(paramVal);
    }
    public override void AddParam(string paramName, float paramVal)
    {
        packet.m_writer.Write(paramVal);
    }
    public override void AddParam(string paramName, int paramVal)
    {
        packet.m_writer.Write(paramVal);
    }
    public override void AddParam(string paramName, long paramVal)
    {
        packet.m_writer.Write(paramVal);
    }
    public override void AddParam(string paramName, short paramVal)
    {
        packet.m_writer.Write(paramVal);
    }
	public override void AddArray(string paramName, List<int> paramVal, int writeCount = 0)
	{
		int listSize = 0;
		if (paramVal != null)
			listSize = paramVal.Count;
		if (writeCount == 0)
			writeCount = listSize;
		for (int index = 0; index < writeCount; index++)
		{
			if (index >= listSize)
				packet.m_writer.Write(0);
			else
				packet.m_writer.Write(paramVal[index]);
		}
	}
    public override void AddString(string paramName, string paramVal)
    {
        packet.m_writer.WriteUTF8(paramVal);
    }
    public override void AddString(string paramName, string paramVal, int size)
    {
        packet.m_writer.WriteUTF8(paramVal, size);
    }
	public override void AddParam(string paramName, MailItem item)
	{
		item.Write(packet.m_writer);
	}
}

public class HttpMessageBlock : MessageBlock
{
    public HttpMessageBlock(short messageID)
    {
        MessageID = messageID;
    }

    private Dictionary<string, object> m_params = new Dictionary<string, object>();

    public void AddParamObj(string paramName, object paramVal)
    {
        m_params.Add(paramName, paramVal);
    }

    public WWWForm SerializeToForm()
    {
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<string, object> item in m_params)
        {
            form.AddField(item.Key, item.Value.ToString());
        }
        return form;
    }


    public override void AddParam(string paramName, bool paramVal)
    {
        AddParamObj(paramName, paramVal);
    }
    public override void AddParam(string paramName, byte paramVal)
    {
        AddParamObj(paramName, paramVal);
    }
    public override void AddParam(string paramName, double paramVal)
    {
        AddParamObj(paramName, paramVal);
    }
    public override void AddParam(string paramName, float paramVal)
    {
        AddParamObj(paramName, paramVal);
    }
    public override void AddParam(string paramName, int paramVal)
    {
        AddParamObj(paramName, paramVal);
    }
    public override void AddParam(string paramName, long paramVal)
    {
        AddParamObj(paramName, paramVal);
    }
    public override void AddParam(string paramName, short paramVal)
    {
        AddParamObj(paramName, paramVal);
    }
    public override void AddString(string paramName, string paramVal)
    {
        AddParamObj(paramName, paramVal);
    }
    public override void AddString(string paramName, string paramVal, int size)
    {
        AddParamObj(paramName, paramVal);
    }
	public override void AddParam(string paramName, MailItem item)
	{

	}
}

public class MessageTransfer
{
	#region singleton
	private static MessageTransfer m_transfer;
	public static MessageTransfer Singleton
	{
		get
		{
			if (m_transfer == null)
			{
				m_transfer = new MessageTransfer();
                //todo Keith 先放这里
                MessageProcess.Singleton.Init();
			}
			return m_transfer;
		}
	}

	#endregion
    private string m_session = "abc0abc1abc2abc3abc4abc5abc6abc7";
    public bool GotSession = false;
	public string Session
	{
		get { return m_session; }
        set { m_session = value; GotSession = true; }
	}
	private int m_uid;
	public int UID
	{
		get { return m_uid; }
		set { m_uid = value; }
	}

	bool m_isTryingConn = false;
	public bool IsTryingConn
	{
		get { return m_isTryingConn; }
		set { m_isTryingConn = value; }
	}	

    //List<MessageBlock> m_cacheMessage = new List<MessageBlock>();
	MessageBlock m_cacheMessage = null;

    public MessageTransfer()
    {
        //注册，用来接收消息回应
        PropertyNotifyManager.Singleton.RegisterReceiver((int)MVCPropertyID.enMessageRespond, OnPropertyChanged);
    }

    public void PostHttpMessage(HttpMessageBlock msg)
	{
		//todo
		if (MessageTransfer.Singleton.UID != 0)
		{
			msg.AddParam("uid", MessageTransfer.Singleton.UID);
		}

		if (MessageTransfer.Singleton.Session != "")
		{
			msg.AddString("session", MessageTransfer.Singleton.Session);
		}

        WWWHelper.Singleton.WWWPost(msg.MessageID, msg);
	}

    //依据设置发送PHP或者C++对应的消息
    public void SendMsg(MessageBlock msg)
    {
		if (GameSettings.Singleton.m_isSingle)
		{
			return;
		}
        if (ServerSetting.Singleton.serverType == ServerType.enHttpServer)
        {
            SendPHPMsg(msg);
        }
        else if (ServerSetting.Singleton.serverType == ServerType.enCPPServer)
        {
            SendCppMsg(msg);
        }
    }

   
    public void SendCppMsg(MessageBlock msg)
    {
        //if (!ClientNet.Singleton.IsLongConnecting)
        {//没有长连接中
            UILoading.GetInstance().ShowWindow();
            UILoading.GetInstance().SetWaitMessageID(msg.MessageID);
        }
		m_cacheMessage = msg;
        if(ClientNet.Singleton.IsConnected)
        {
            CppMessageBlock cppMsg = (CppMessageBlock)msg;
            ClientNet.Singleton.SendPacket(cppMsg.packet);
        }
		else if (MessageTransfer.Singleton.IsTryingConn)
		{
			//m_cacheMessage.Add(msg);
		}
        else
        {
            ClientNet.Singleton.Start();
            //m_cacheMessage.Add(msg);
			MessageTransfer.Singleton.IsTryingConn = true;
        }
    }
    public void SendPHPMsg(MessageBlock msg)
    {
        HttpMessageBlock httpMsg = (HttpMessageBlock)msg;
        PostHttpMessage(httpMsg);
    }

    public void SendCacheCachedMessage()
    {
		if (m_cacheMessage == null)
		{ 
			return;
		}
		CppMessageBlock cppMsg = (CppMessageBlock)m_cacheMessage;
		SendCppMsg(cppMsg);	
	}

    //消息回应
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (m_cacheMessage == null)
        {
            return;
        }

        MessageRespond respond = (MessageRespond)eventObj;
        if (respond.MessageID == m_cacheMessage.MessageID)
        {//清除缓存消息
            m_cacheMessage = null;
        }
    }
}
