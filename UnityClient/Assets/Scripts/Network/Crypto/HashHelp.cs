using System;
using System.Text;
using System.Security.Cryptography;

namespace NGE.Crypto
{
	/// <summary>
	/// Hash算法帮助类
	/// </summary>
	public sealed class HashHelp
	{
		private HashHelp(){}

		private static SHA1 m_sha1 = new SHA1CryptoServiceProvider();
        private static CRC32 m_crc32 = new CRC32();
		private static MD5 m_md5 = new MD5CryptoServiceProvider();
		
		public static SHA1 SHA1CSP{
			get{
				return m_sha1;
			}
		}

		public static MD5 MD5CSP{
			get{
				return m_md5;
			}
		}

        public static CRC32 CRC32CSP
        {
            get
            {
                return m_crc32;
            }
        }
		/// <summary>
		/// 计算一个字符串的SHA1字符串
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		public static string Shahash(string src)
		{
			byte[] array = m_sha1.ComputeHash(Encoding.Unicode.GetBytes(src));
			
			return Util.ArrayUtility.HexArrayToString(array);
		}

		/// <summary>
		/// 计算一个字符串的SHA1字符串
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		public static string MD5hash(string src)
		{
			byte[] array = m_md5.ComputeHash(Encoding.Unicode.GetBytes(src));
			
			return Util.ArrayUtility.HexArrayToString(array);
		}

        public static uint CRC32hash(byte[] buffer, int offset, int count)
        {
            m_crc32.Reset();
            m_crc32.ComputeCRC32(buffer, offset, count);
            return m_crc32.Value;
        }

        public static uint CRC32hash(byte[] buffer)
        {
            m_crc32.Reset();
            m_crc32.ComputeCRC32(buffer, 0, buffer.Length);
            return m_crc32.Value;
        }
	}
}
