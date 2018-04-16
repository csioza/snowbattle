using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ServerType
{
    enHttpServer = 1,
    enCPPServer = 2,
}

public class ServerSetting
{
    #region singleton
    private static ServerSetting m_singleton;
    public static ServerSetting Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new ServerSetting();
            }
            return m_singleton;
        }
    }
    #endregion

    public ServerType serverType = ServerType.enCPPServer;
	public int ProtocolVersion = 6;
}

//所有服务器的总设置
public class GlobalServerSetting
{
	#region singleton
	private static GlobalServerSetting m_singleton;
	public static GlobalServerSetting Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new GlobalServerSetting();
			}
			return m_singleton;
		}
	}
	#endregion

	public List<ZoneInfo> ZoneList;

	public int GotSetting = 0;		//0 未获取.  1获取成功, 2获取失败.
}

//本区域服务器设置.
public class ZoneServerSetting
{	
	#region singleton
	private static ZoneServerSetting m_singleton;
	public static ZoneServerSetting Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new ZoneServerSetting();
			}
			return m_singleton;
		}
	}
	#endregion

	public List<int> initHerocards;
}