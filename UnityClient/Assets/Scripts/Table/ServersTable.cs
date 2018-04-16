using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

public class ServerTable
{
    public List<ServerInfo> m_serverList { get; protected set; }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        m_serverList = new List<ServerInfo>(length);
        for (int index = 0; index < length; ++index)
        {
            ServerInfo info = new ServerInfo();
            info.Load(helper);
            m_serverList.Add(info);
        }
    }
}

public class ServerInfo : IDataBase
{
    public string m_name { get; protected set; }
    public string m_IP { get; protected set; }
    public bool m_hot { get; protected set; }
}