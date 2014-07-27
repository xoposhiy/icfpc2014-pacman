using System;

namespace Lib.LMachine
{
	public class LException : Exception
	{
		public LException(Exception innerException)
			: base("L-program failed", innerException)
		{
		}
	}
}