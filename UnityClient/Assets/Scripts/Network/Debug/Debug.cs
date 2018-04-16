using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace NGE
{
    /// <summary>
    /// µ˜ ‘£¨æØ∏Ê£¨’Ô∂œ∞Ô÷˙¿‡
    /// </summary>
    public static class Debug
    {
        [Conditional("DEBUG")]
        public static void TraceException(Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            UnityEngine.Debug.LogError(e.Message);
            UnityEngine.Debug.LogError(e.StackTrace);
        }

        public static void TraceExceptionAlways(Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            UnityEngine.Debug.LogError(e.Message);
            UnityEngine.Debug.LogError(e.StackTrace);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(string str)
        {
            Console.WriteLine(str);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(string str, params object[] objs)
        {
            Console.WriteLine(str, objs);
        }
    }
}
