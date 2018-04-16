using UnityEngine;
using System.Collections;

public class ClientNetProperty : IPropertyObject 
{
	public enum ENPropertyChanged
	{
		enConnected = 1,
		enConnectionFailed,	
	}

	#region Singleton
	static ClientNetProperty m_singleton;
	static public ClientNetProperty Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new ClientNetProperty();
			}
			return m_singleton;
		}
	}
	#endregion


	public ClientNetProperty()
    {
		SetPropertyObjectID((int)MVCPropertyID.enClientNet);     
    }


}
