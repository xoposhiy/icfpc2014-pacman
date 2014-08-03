using System;
using Lib.LMachine;
using NUnit.Framework;

namespace Lib.NewLang
{
	[TestFixture]
	public class SE_Test
	{
		[Test]
		public void Test()
		{
			var gcc = SE.Compile(typeof(Queue));
			LMachineInterpreter.Run(gcc);
			Console.WriteLine(gcc);
		}
	}
}