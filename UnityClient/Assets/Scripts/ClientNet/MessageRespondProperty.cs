using UnityEngine;
using System.Collections;

public struct MessageRespond
{
	public short MessageID;		//消息ID
	public bool IsSuccess;		//操作是否成功
	public int RespondCode;		//回应代码
}

public class MessageRespondProperty : IPropertyObject
{
	#region Singleton
	static MessageRespondProperty m_singleton;
	static public MessageRespondProperty Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new MessageRespondProperty();
			}
			return m_singleton;
		}
	}
	#endregion


	public MessageRespondProperty()
    {
        SetPropertyObjectID((int)MVCPropertyID.enMessageRespond);     
    }

	public void OnMessageRespond(MessageRespond res)
	{
		NotifyChanged(0, res);
		switch ((ENMessageRespond)res.RespondCode)
		{
			case ENMessageRespond.enDungeonIDNotFound:
				{
					if (!(MainGame.Singleton.CurrentState is StateMainUI))
					{
						StateMainUI mainState = new StateMainUI();
						MainGame.Singleton.TranslateTo(mainState);
					}
				}
				break;
		}
	}
}
