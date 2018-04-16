using System;

namespace NGE
{
	/// <summary>
	/// Summary description for ConfigException.
	/// </summary>
	public class ConfigException : Exception
	{
		public ConfigException(string message)
			:base(message)
		{
		}
	}
}
