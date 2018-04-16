using System;
using System.Text;

namespace NGE.Util
{
    /// <summary>
    /// 数组操作帮助类
    /// </summary>
    public sealed class ArrayUtility
    {
        private ArrayUtility() { }

        public static int GetInt(byte[] data, int offset)
        {
            return data[offset + 3] << 24 |
                   data[offset + 2] << 16 |
                   data[offset + 1] << 8 |
                   data[offset];
        }

        public static void SetInt(byte[] data, int val, int offset)
        {
            data[offset] = (byte)(val & 0x000000FF);
            data[offset + 1] = (byte)((val >> 8) & 0x000000FF);
            data[offset + 2] = (byte)((val >> 16) & 0x000000FF);
            data[offset + 3] = (byte)((val >> 24) & 0x000000FF);
        }

        public static short GetShort(byte[] data, int offset)
        {
            return (short)(data[offset + 1] << 8 | data[offset]);
        }
        public static byte GetByte(byte[] data, int offset)
        {
            return data[offset];
        }
        public static void SetByte(byte[] data, byte val, int offset)
        {
            data[offset] = val;
        }

        public static void SetShort(byte[] data, short val, int offset)
        {
            data[offset] = (byte)(val & 0x00ff);
            data[offset + 1] = (byte)((val >> 8) & 0x00ff);
        }

		/// <summary>
		/// 二进制数组转化成字符串
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		public static string HexArrayToString(byte[] buffer)
		{
			if(buffer == null)
				return null;
			char[] chars = new char[buffer.Length*2];
			char char1,char2;
			int int1,int2;
			for(int i=0;i<buffer.Length;i++)
			{
				int1 = buffer[i]>>4;
				int2 = buffer[i]&0x0f;
				char1 = (char)(int1 > 9 ? 55+int1 : 48+int1);
				char2 = (char)(int2 > 9 ? 55+int2 : 48+int2);
				chars[i*2] = char1;
				chars[i*2+1] = char2;
			}
			return new string(chars);
		}

		/// <summary>
		/// 字符串转化成二进制数组
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		public static byte[] StringToHexArray(string src)
		{
			if(src == null || src.Length < 2 || src.Length%2 != 0)
				return null;
			byte[] result = new byte[src.Length/2];
			src.ToUpper();
			char char1,char2;
			int int1,int2;
			for(int i=0;i<result.Length;i++)
			{
				char1 = src[i*2];
				char2 = src[i*2 +1];
				//高4位
				if(char1 > 64 && char1 < 91)
					int1 = char1 -55;
				else if(char1 > 47 && char1 < 58)
					int1 = char1 - 48;
				else 
					return null;
				//低4位
				if(char2 > 64 && char2 < 91)
					int2 = char2 -55;
				else if(char2 > 47 && char2 < 58)
					int2 = char2 - 48;
				else 
					return null;

				result[i] = (byte)(int1<<4 | int2);
			}
			return result;
		}
    }
}
