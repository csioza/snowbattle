using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class TimeUtil
{
	static long timeDiff;//服务器与客户端之间时间差(s)
	public static void SetServerTimeStamp(uint serverTimeStamp)//s
	{

		long clientNow = GetCurrentTimeStamp();
		timeDiff = serverTimeStamp - clientNow;
	}

	/// <summary>
	/// 返回1970-1-1 0:0:0 以来的s数
	/// </summary>
	/// <returns></returns>
	public static long GetCurrentTimeStamp()
	{
		TimeSpan span= DateTime.UtcNow.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
		return (long)span.TotalSeconds;
	}
	
	/// <summary>
	/// 返回1970-1-1 0:0:0 以来的毫秒数
	/// </summary>
	/// <returns></returns>
	public static double GetCurrentTimeMillis()
	{
		TimeSpan span= DateTime.UtcNow.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
		return span.TotalMilliseconds;
	}
	
	//s
	public static long GetServerTimeStampNow()
	{
		long clientNow = GetCurrentTimeStamp();
		return clientNow + timeDiff;
	}
}
