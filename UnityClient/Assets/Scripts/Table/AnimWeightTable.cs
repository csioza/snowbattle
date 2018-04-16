using System.Collections.Generic;
public class AnimWeightTable
{
	public byte Lookup(string newAnim, string oldAnim)
	{
		if (string.IsNullOrEmpty(oldAnim))
		{
			return 1;
		}
		if (string.IsNullOrEmpty(newAnim))
		{
			return 0;
		}
		int newIndex = m_newList.IndexOf(newAnim);
		int oldIndex = m_currentList.IndexOf(oldAnim);
		//Debug.Log(newIndex + " " + oldIndex);
		return m_byteArray[m_newList.Count * oldIndex + newIndex];
	}

	public List<string> m_currentList;
	public List<string> m_newList;
	public byte[] m_byteArray;

	public void Load(byte[] bytes)
	{
		BinaryHelper helper = new BinaryHelper(bytes);

		m_currentList = new List<string>();
		int currentLength = helper.ReadInt();
		for (int index = 0; index < currentLength; ++index)
		{
			m_currentList.Add(helper.ReadString());
		}
		m_newList = new List<string>();
		int newLength = helper.ReadInt();
		for (int index = 0; index < newLength; ++index)
		{
			m_newList.Add(helper.ReadString());
		}
		m_byteArray = new byte[m_newList.Count * m_newList.Count];
		helper.InnerStream.Read(m_byteArray, 0, m_newList.Count * m_newList.Count);
	}
}