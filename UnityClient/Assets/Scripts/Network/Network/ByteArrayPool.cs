using System;
using System.Collections.Generic;
using System.Text;

namespace NGE.Network
{

	internal sealed class ByteArrayPool
	{
		static Stack<byte[]> s_free_object_array1 = new Stack<byte[]>();//Packet.MaxLength
		static Stack<byte[]> s_free_object_array2 = new Stack<byte[]>();//4k
		static Stack<byte[]> s_free_object_array3 = new Stack<byte[]>();//1k
		const int size_3 = Packet.MaxLength;
		const int size_2 = 4096;
		const int size_1 = 1024;
		static ByteArrayPool m_instance = new ByteArrayPool();
		public static ByteArrayPool Instance
		{
			get
			{
				return m_instance;
			}
		}

		static ByteArrayPool()
		{
			for (int i = 0; i < 50; i++)// init 100 packet memory to free object queue
			{
				s_free_object_array1.Push(new byte[size_1]);
				s_free_object_array2.Push(new byte[size_2]);
				s_free_object_array3.Push(new byte[size_3]);
			}
		}

		public byte[] MallocArray(int size)
		{
			int obj_size = 0;
			Stack<byte[]> array;
			array = FindStackBySize(size, ref obj_size);
			if (array == null)
				return null;
			lock (array)
			{
				if (array.Count == 0)
				{
					for (int i = 0; i < 50; i++)// add 100 packet memory to free object queue
					{
						array.Push(new byte[obj_size]);
					}
				}
				return array.Pop();
			}
		}

		public void Release(byte[] obj)
		{
			if (obj == null)
				return;
			int obj_size = 0;
			Stack<byte[]> array;
			array = FindStackBySize(obj.Length, ref obj_size);
			if (array == null)
				return;

			lock (array)
				array.Push(obj);
		}

		private Stack<byte[]> FindStackBySize(int size, ref int obj_size)
		{
			obj_size = 0;
			Stack<byte[]> array;
			if (size <= size_1)
			{
				obj_size = size_1;
				array = s_free_object_array1;
			}
			else if (size <= size_2)
			{
				obj_size = size_2;
				array = s_free_object_array2;
			}
			else if (size <= size_3)
			{
				obj_size = size_3;
				array = s_free_object_array3;
			}
			else
				return null;
			return array;
		}
	}

	internal sealed class PacketReaderPool
	{
		static Stack<PacketReader> s_free_object_array1 = new Stack<PacketReader>();
		static Stack<PacketReader> s_free_object_array2 = new Stack<PacketReader>();//4k
		static Stack<PacketReader> s_free_object_array3 = new Stack<PacketReader>();//1k
		const int size_3 = Packet.MaxLength;
		const int size_2 = 4096;
		const int size_1 = 1024;
		static PacketReaderPool m_instance = new PacketReaderPool();
		public static PacketReaderPool Instance
		{
			get
			{
				return m_instance;
			}
		}

		static PacketReaderPool()
		{
			for (int i = 0; i < 50; i++)
			{
				s_free_object_array1.Push(new PacketReader(new byte[size_1], 0, 0, true, null));
				s_free_object_array2.Push(new PacketReader(new byte[size_2], 0, 0, true, null));
				s_free_object_array3.Push(new PacketReader(new byte[size_3], 0, 0, true, null));
			}
		}

		public PacketReader CreatePacketReader(byte[] data, int startoffset, int packetlength, IConnection client)
		{
			int obj_size = 0;
			Stack<PacketReader> array;
			PacketReader reader;
			array = FindStackBySize(packetlength, ref obj_size);
			if (array == null)
				return null;
			lock (array)
			{
				if (array.Count == 0)
				{
					for (int i = 0; i < 50; i++)// add 100 packet memory to free object queue
					{
						array.Push(new PacketReader(new byte[obj_size], 0, 0, true, null));
					}
				}
				reader = array.Pop();
			}
			reader.Reset(data, startoffset, packetlength, client);
			return reader;
		}

		public void Release(PacketReader reader)
		{
			reader.Reset();
			int obj_size = 0;
			Stack<PacketReader> array;
			array = FindStackBySize(reader.DataBuffer.Length, ref obj_size);
			if (array == null)
				return;

			lock (array)
				array.Push(reader);
		}

		private Stack<PacketReader> FindStackBySize(int size, ref int obj_size)
		{
			obj_size = 0;
			Stack<PacketReader> array;
			if (size <= size_1)
			{
				obj_size = size_1;
				array = s_free_object_array1;
			}
			else if (size <= size_2)
			{
				obj_size = size_2;
				array = s_free_object_array2;
			}
			else if (size <= size_3)
			{
				obj_size = size_3;
				array = s_free_object_array3;
			}
			else
				return null;
			return array;
		}
	};
}
