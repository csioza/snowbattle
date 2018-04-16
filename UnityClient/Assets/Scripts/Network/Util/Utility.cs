using System;
using System.Text;
using System.Net;
using System.Diagnostics;

namespace NGE.Util
{
	/// <summary>
	/// 提供一些乱七八糟功能的帮助类
	/// </summary>
    public sealed class Utility
    {
		private Utility(){}

        private static Encoding m_UTF8 = new UTF8Encoding(false, false);
        private static Encoding m_UTF8WithEncoding = new UTF8Encoding(true, false);
        private static Random m_randomizer = new Random();
		private static int[] m_ArrInt = new int[]{7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2};
		private static char[] m_ArrCh = new char[]{'1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2'}; 

        public static Encoding UTF8{
            get{
                return m_UTF8;
            }
        }

        public static Encoding UTF8WithEncoding{
            get{
                return m_UTF8WithEncoding;
            }
        }

        public static Random Randomizer{
            get{
                return m_randomizer;
            }
        }

        public static int InsensitiveCompare(string first, string second)
        {
            return Insensitive.Compare(first, second);
        }

        public static bool InsensitiveStartsWith(string first, string second)
        {
            return Insensitive.StartsWith(first, second);
        }

		/// <summary>
		/// 15位的身份证号码转化成18位
		/// </summary>
		/// <param name="strTemp"></param>
		/// <returns></returns>
		public static string ID15To18(string strID15)
		{
			if(strID15 == null || strID15.Length != 15)
				return null;

			int  nTemp = 0;
			StringBuilder builder = new StringBuilder(18);
			builder.Append(strID15.Substring(0,6));
			builder.Append("19");
			builder.Append(strID15.Substring(6,strID15.Length-6));
			
			for(int i=0;i<17;i++)
			{
				nTemp += (builder[i] - '0') * m_ArrInt[i];
			}
			builder.Append(m_ArrCh[nTemp % 11]);

			return builder.ToString();

		}

		/// <summary>
		/// 32位的ip地址转换成字符串,xxx.xxx.xxx.xxx形式
		/// </summary>
		/// <param name="ipaddress"></param>
		/// <returns></returns>
		public static string UInt32IPToString(uint ipaddress)
		{
			return new IPAddress(ipaddress).ToString();
		}
        /// <summary>
        /// xxx.xxx.xxx.xxx字符串形式的ip地址转换32位uint值IPV4,
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <returns></returns>
        public static uint StringIPToUInt32(string ipaddress)
        {
            IPAddress address = IPAddress.Parse(ipaddress);
            byte[] ba = address.GetAddressBytes();
            return (uint)ArrayUtility.GetInt(ba, 0);
        }
        /// <summary>
        /// 从IPEndPoint得到IPV4的类型为32bit int的ip地址
        /// </summary>
        /// <param name="ep"></param>
        /// <returns></returns>
        public static uint GetIPV4FromIPEndPoint(IPEndPoint ep)
        {
            byte[] ba = ep.Address.GetAddressBytes();
            return (uint)ArrayUtility.GetInt(ba, 0);
        }

      

    }
}
