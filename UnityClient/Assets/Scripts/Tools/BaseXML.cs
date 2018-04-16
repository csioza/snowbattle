using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
public class BaseXML<T>
{
	public static void Save(T obj, string path)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		using(FileStream stream = new FileStream(path, FileMode.Create))
		{
			TextWriter writer = new StreamWriter(stream, new UTF8Encoding());
			serializer.Serialize(writer, obj);
		}
	}
	
	public  static T LoadPath(string path)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		using(FileStream stream = new FileStream(path, FileMode.Open))
		{
			return (T)serializer.Deserialize(stream);
		}
	}

	//Loads the xml directly from the given string. Useful in combination with www.text.
	public  static T LoadFromText(string text) 
	{
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		return (T)serializer.Deserialize(new StringReader(text));
	}

    public static void SaveList(List<T> obj, string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            TextWriter writer = new StreamWriter(stream, new UTF8Encoding());
            serializer.Serialize(writer, obj);
        }
    }

    public static List<T> LoadListPath(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            return (List<T>)serializer.Deserialize(stream);
        }
    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static List<T> LoadListFromText(string text)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
        return (List<T>)serializer.Deserialize(new StringReader(text));
    }

    /// <summary>
    /// 序列化对象并返回对象生成的字符串.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    public static string SaveToString(T data)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        MemoryStream ms = new MemoryStream();
        serializer.Serialize(ms, data);
        ms.Position = 0;
        StreamReader sr = new StreamReader(ms);
        return sr.ReadToEnd();
        
    }
}