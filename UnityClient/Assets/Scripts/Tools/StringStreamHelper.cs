//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Tools
//	created:	2013-10-23
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using System.IO;


public class StringStreamReader
{
	private StringReader m_stream;
	public StringStreamReader(string str)
	{
		m_stream = new StringReader(str);
	}

	public int ReadInt()
	{
		return Int32.Parse(m_stream.ReadLine());
	}
	public float ReadFloat()
	{
		return float.Parse(m_stream.ReadLine());
	}
	public bool ReadBool()
	{
		return bool.Parse(m_stream.ReadLine());
	}
	public short ReadShort()
	{
		return short.Parse(m_stream.ReadLine());
	}
	public byte ReadByte()
	{
		return byte.Parse(m_stream.ReadLine());
	}

	public string ReadString()
	{
		int length = ReadInt();
		char[] buff = new char[length];
		m_stream.Read(buff, 0, length);
		return new string(buff);
	}
};

public class StringStreamWriter
{
	private StringWriter m_stream = new StringWriter();

	public override string ToString()
	{
		return m_stream.ToString();
	}

	public void Write(int value)
	{
		m_stream.WriteLine(value);
	}

	public void Write(float value)
	{
		m_stream.WriteLine(value);
	}

	public void Write(bool value)
	{
		m_stream.WriteLine(value);
	}

	public void Write(short value)
	{
		m_stream.WriteLine(value);
	}

	public void Write(byte value)
	{
		m_stream.WriteLine(value);
	}

	public void Write(string value)
	{
		Write(value.Length);
		m_stream.Write(value);
	}
};