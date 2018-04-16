using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NGE.Network;

public enum ENMailContentType
{
	enMailContentMoney		= 0x1,
	enMailContentBMoney		= 0x1<<1,
	enMailContentRing		= 0x1<<2,
	enMailContentItemByID	= 0x1<<3,
	enMailContentItemGuid	= 0x1<<4,
	enMailContentText		= 0x1<<5,
}

public class MailItem
{
	public int m_mailID;
	public int m_mailType;
	public int m_senderID;
	public int m_receiverID;
	public int m_contentFlag;
	public int m_contentSize;
	public int m_sendTime;
	public int m_mailStatus;
	public int m_removeTime;

	public int m_money;
	public int m_bMoney;
	public int m_ring;
    public List<int> m_itemIDList = new List<int>();
    public List<CSItemGuid> m_itemGuidList = new List<CSItemGuid>();
	public string m_contentText;

	public void Write(PacketWriter writer)
	{
		writer.Write(m_receiverID);
		WriteContent(writer);
	}

	public void Read(PacketReader reader)
	{
		ReadContent(reader);
	}

	public void WriteContent(PacketWriter writer)
	{
		int pos1 = writer.Position;
		writer.Write((int)ENMailContentType.enMailContentMoney);
		if (m_money > 0 )
		{
			m_contentFlag |= (int)ENMailContentType.enMailContentMoney;
			writer.Write(m_money);
		}
		if (m_bMoney > 0)
		{
			m_contentFlag |= (int)ENMailContentType.enMailContentBMoney;
			writer.Write(m_bMoney);
		}
		if (m_ring > 0)
		{
			m_contentFlag |= (int)ENMailContentType.enMailContentRing;
			writer.Write(m_ring);
		}
		if (m_itemIDList.Count > 0)
		{
			m_contentFlag |= (int)ENMailContentType.enMailContentItemByID;
			writer.Write(m_itemIDList.Count);
			for(int index = 0 ; index < m_itemIDList.Count; index++)
			{
				writer.Write(m_itemIDList[index]);
			}
		}
		if (m_itemGuidList.Count > 0)
		{
			m_contentFlag |= (int)ENMailContentType.enMailContentItemGuid;
			writer.Write(m_itemGuidList.Count);
			for (int index = 0; index < m_itemGuidList.Count; index++)
			{
				writer.Write(m_itemGuidList[index].m_lowPart);
				writer.Write(m_itemGuidList[index].m_highPart);
			}
		}
		if(m_contentText.Length > 0)
		{
			m_contentFlag |= (int)ENMailContentType.enMailContentText;
			writer.Write(m_contentText.Length);
			writer.WriteUTF8(m_contentText, m_contentText.Length);
		}
 		int pos2 = writer.Position;
 		writer.Seek(pos1, System.IO.SeekOrigin.Begin);
 		writer.Write(m_contentFlag);
 		writer.Seek(pos2, System.IO.SeekOrigin.Begin);
	}
	public void ReadContent(PacketReader reader)
	{
		m_contentFlag = reader.ReadInt32();
		if ((m_contentFlag & (int)ENMailContentType.enMailContentMoney)>0)
		{
			m_money = reader.ReadInt32();
		}
		if ((m_contentFlag & (int)ENMailContentType.enMailContentBMoney) > 0)
		{
			m_bMoney = reader.ReadInt32();
		}
		if ((m_contentFlag & (int)ENMailContentType.enMailContentRing) > 0)
		{
			m_ring = reader.ReadInt32();
		}
		if ((m_contentFlag & (int)ENMailContentType.enMailContentItemByID) > 0)
		{
			int itemCount = reader.ReadInt32();
			for(int index = 0;index<itemCount;index++)
			{
				int itemID = reader.ReadInt32();
			}
		}
		if ((m_contentFlag & (int)ENMailContentType.enMailContentItemGuid) > 0)
		{
			int itemCount = reader.ReadInt32();
			for (int index = 0; index < itemCount; index++)
			{
				int lowPart = reader.ReadInt32();
				int highPart = reader.ReadInt32();
			}
		}
		if ((m_contentFlag & (int)ENMailContentType.enMailContentText) > 0)
		{
			int stringSize = reader.ReadInt32();
			string text = reader.ReadUTF8(stringSize);
		}
	}

	public void AddToMessage(MessageBlock msg)
	{

	}
}

public class CSMailBag
{

#region Singleton
	static CSMailBag m_singleton;
	static public CSMailBag Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new CSMailBag();
			}
			return m_singleton;
		}
	}
#endregion
	Dictionary<int,MailItem> m_mailList;

	
}
