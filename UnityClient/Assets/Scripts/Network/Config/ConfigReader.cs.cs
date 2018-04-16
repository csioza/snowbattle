using System;
using System.Xml;
using System.Collections.Generic;

namespace NGE.Config
{
	/// <summary>
	/// Summary description for ConfigReader.
	/// </summary>
    public sealed class ConfigReader
	{
        private Dictionary<string, string> m_nodelist = new Dictionary<string, string>();
		private XmlDocument m_doc = null;
		private XmlNode m_rootnode;

		public ConfigReader(string cfg)
		{
			m_doc = new XmlDocument();
			m_doc.Load(cfg);
			foreach(XmlNode node in m_doc.ChildNodes)
			{
				if(node.NodeType == XmlNodeType.Element)
				{
					m_rootnode = node;
					return;
				}
			}
		}

		public bool OpenNode(string node)
		{
			m_nodelist.Clear();
			XmlNode root = m_rootnode.SelectSingleNode(node);
			if(root == null)
				return false;
			if( root.HasChildNodes )
			{
				for(int n=0; n<root.ChildNodes.Count; n++)
				{
					m_nodelist.Add( root.ChildNodes[n].LocalName, root.ChildNodes[n].InnerText );
					//					Console.WriteLine( root.ChildNodes[n].LocalName, root.ChildNodes[n].InnerText );
				}
			}
			return true;
		}
		public void CloseNode()
		{
			m_nodelist.Clear();
		}

		public string ReadSingleNodeValue(string nodename)
		{
			XmlNode node = m_rootnode.SelectSingleNode(nodename);
			if(node == null)
				return null;
			return node.InnerText;
		}

		public string ReadChar(string item, string dflt)
		{
			if( m_nodelist.ContainsKey(item) )
			{
				string s_value = m_nodelist[item];
				return s_value;
			}
			else
			{
				return dflt;
			}
		}

		public int ReadInt(string item, int dflt)
		{
			if( m_nodelist.ContainsKey(item) )
			{
				int n_value = Convert.ToInt32(m_nodelist[item]);
				return n_value;
			}
			else
			{
				return dflt;
			}
		}
	}
}
