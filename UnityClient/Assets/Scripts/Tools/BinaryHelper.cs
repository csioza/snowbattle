//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Tools
//	created:	2013-5-7
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using UnityEngine;


public class BinaryHelper
{
	private MemoryStream m_stream = null;
	public MemoryStream InnerStream { get { return m_stream; } }
	public BinaryHelper()
	{
		m_stream = new MemoryStream();
	}
	public BinaryHelper(byte[] buff)
	{
		m_stream = new MemoryStream(buff);
	}

	public byte[] GetBytes()
	{
		return m_stream.ToArray();
	}

	public void Write(int value)
	{
		byte[] temp = BitConverter.GetBytes(value);
		m_stream.Write(temp, 0, temp.Length);
	}
	public void Write(float value)
	{
		byte[] temp = BitConverter.GetBytes(value);
		m_stream.Write(temp, 0, temp.Length);
	}
	public void Write(double value)
	{
		byte[] temp = BitConverter.GetBytes(value);
		m_stream.Write(temp, 0, temp.Length);
	}
	public void Write(bool value)
	{
		byte[] temp = BitConverter.GetBytes(value);
		m_stream.Write(temp, 0, temp.Length);
	}
	public void Write(string value)
	{
		if (null == value)
		{
			Write(0);
		}
		else
		{
			byte[] temp = new byte[value.Length * sizeof(char)];
			Write(temp.Length);	//先写入字符串长度
			System.Buffer.BlockCopy(value.ToCharArray(), 0, temp, 0, temp.Length);
			m_stream.Write(temp, 0, temp.Length);
		}
	}

	public void Write(short value)
	{
		byte[] temp = BitConverter.GetBytes((int)value);
		m_stream.Write(temp, 0, temp.Length);
	}
	public void Write(long value)
	{
		byte[] temp = BitConverter.GetBytes(value);
		m_stream.Write(temp, 0, temp.Length);
	}


	public int ReadInt()
	{
		int length = sizeof(int);
		byte[] temp = new byte[length];
		m_stream.Read(temp, 0, length);
		return BitConverter.ToInt32(temp, 0);
	}
	public float ReadFloat()
	{
		int length = sizeof(float);
		byte[] temp = new byte[length];
		m_stream.Read(temp, 0, length);
		return BitConverter.ToSingle(temp, 0);
	}
	public bool ReadBool()
	{
		int length = sizeof(bool);
		byte[] temp = new byte[length];
		m_stream.Read(temp, 0, length);
		return BitConverter.ToBoolean(temp, 0);
	}

    public byte ReadByte()
    {
        int length = sizeof(byte);
        byte[] temp = new byte[length];
        m_stream.Read(temp, 0, length);
        return temp[0];
    }
	public string ReadString()
	{
		int length = ReadInt();
		if (0 == length)
		{
			return null;
		}
		byte[] temp = new byte[length];
		m_stream.Read(temp, 0, length);
		char[] value = new char[length / sizeof(char)];
		System.Buffer.BlockCopy(temp, 0, value, 0, temp.Length);
		return new string(value);
	}

	public short ReadShort()
	{
		short length = sizeof(short);
		byte[] temp = new byte[length];
		m_stream.Read(temp, 0, length);
		return BitConverter.ToInt16(temp, 0);
	}

	public long ReadLong()
	{
		int length = sizeof(long);
		byte[] temp = new byte[length];
		m_stream.Read(temp, 0, length);
		return BitConverter.ToInt64(temp, 0);
	}

	public uint ReadUInt()
	{
		int length = sizeof(uint);
		byte[] temp = new byte[length];
		m_stream.Read(temp, 0, length);
		return BitConverter.ToUInt32(temp, 0);
	}
};