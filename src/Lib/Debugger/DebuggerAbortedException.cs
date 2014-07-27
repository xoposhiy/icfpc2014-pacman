using System;

namespace Lib.Debugger
{
	public class DebuggerAbortedException : Exception
	{
		public DebuggerAbortedException(Exception exception)
			: base("Debugger aborted program", exception)
		{
		}
	}
}