using System;
using NUnit.Framework;

namespace Lib.GMachine
{
	[TestFixture]
	public class GMachine_Test
	{
		[Test]
		public void Test()
		{
			var x = byte.MaxValue;
			Console.Out.WriteLine(++x);
		}
	}
}