using UnityEngine;
using System.Collections;

public class PlayerData
{
	public PlayerData()
	{
	}

	public void SaveString(string key, string info)
	{
		PlayerPrefs.SetString(key, info);
		PlayerPrefs.Save();
	}
	public string LoadString(string key)
	{
		if (!PlayerPrefs.HasKey(key))
		{
			return null;
		}
		return PlayerPrefs.GetString(key);
	}

	public void Save<T>(string key, T obj)
	{
		string text = BaseXML<T>.SaveToString(obj);
		Debug.Log(text);
		PlayerPrefs.SetString(key, text);
		PlayerPrefs.Save();
	}
	public T Load<T>(string key)
	{
		if (!PlayerPrefs.HasKey(key))
		{
			return default(T);
		}
		string text = PlayerPrefs.GetString(key);
		Debug.Log(text);
		return BaseXML<T>.LoadFromText(text);
	}
}
