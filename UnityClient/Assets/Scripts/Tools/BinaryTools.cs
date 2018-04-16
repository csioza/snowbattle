using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

class BinaryTools
{
	public static object ReadFile(string filePath)
	{
		FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		BinaryFormatter b = new BinaryFormatter();
		object obj = b.Deserialize(fileStream);
		fileStream.Close();
		return obj;
	}
	public static void WriteFile(string filePath, object obj)
	{
		FileStream file = new FileStream(filePath, FileMode.OpenOrCreate);
		BinaryFormatter formatter = new BinaryFormatter();
		formatter.Serialize(file, obj);
		file.Close();
	}
	public static object ReadBytes(byte[] bytes)
	{
		MemoryStream memStream = new MemoryStream(bytes);
		BinaryFormatter b = new BinaryFormatter();
		object obj = b.Deserialize(memStream);
		memStream.Close();
		return obj;
	}
}