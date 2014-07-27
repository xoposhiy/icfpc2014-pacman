using System;

namespace Lib.GMachine
{
	public class GException : Exception
	{
		public GException(Exception innerException)
			: base("G-Program failed", innerException)
		{
		}
	}
}