﻿using System;
using System.Collections;

using System.Text;

namespace NGE.Util
{
    /// <summary>
    /// 大小写不敏感的字符串操作帮助类
    /// </summary>
    public sealed class Insensitive
    {
        private static IComparer m_Comparer = System.Collections.CaseInsensitiveComparer.Default;

        public static IComparer Comparer
        {
            get { return m_Comparer; }
        }

        private Insensitive()
        {
        }

        public static int Compare(string a, string b)
        {
            return m_Comparer.Compare(a, b);
        }

        public static bool Equals(string a, string b)
        {
            if (a == null && b == null)
                return true;
            else if (a == null || b == null || a.Length != b.Length)
                return false;

            return (m_Comparer.Compare(a, b) == 0);
        }

        public static bool StartsWith(string a, string b)
        {
            if (a == null || b == null || a.Length < b.Length)
                return false;

            return (m_Comparer.Compare(a.Substring(0, b.Length), b) == 0);
        }

        public static bool EndsWith(string a, string b)
        {
            if (a == null || b == null || a.Length < b.Length)
                return false;

            return (m_Comparer.Compare(a.Substring(a.Length - b.Length), b) == 0);
        }

        public static bool Contains(string a, string b)
        {
            if (a == null || b == null || a.Length < b.Length)
                return false;

            a = a.ToLower();
            b = b.ToLower();

            return (a.IndexOf(b) >= 0);
        }
    }
}
