using System;

namespace Lib.GMachine
{
	public class GException : Exception
	{
		public GMachine Machine { get; protected set; }

		public GException(GMachine machine, Exception innerException)
			: base("G-Program failed", innerException)
		{
			Machine = machine;
		}
	}
}