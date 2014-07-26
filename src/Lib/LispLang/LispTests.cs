using System;
using Lib.LMachine;
using Lib.LMachine.Parsing;
using NUnit.Framework;

namespace Lib.LispLang
{
	public class LispTests : Lisp
	{
		[Test]
		public void CompileSample()
		{
			var macro = Compile(
				Call("get", Call("sampleList"), 3),
				Cmd("RTN", new SExpr()),
				Def("sampleList", ArgNames(), L(1, 2, 3, 4, 0)),
				Def("get", ArgNames("list", "index"), 
					If("index", 
						Call("get", Args(Cdr("list"), Sub("index", 1))),
						Car("list")))
				);
			Console.WriteLine(macro);
			var parsed = LParser.Parse(macro);
			var m = new LMachineInterpreter(parsed.Program);
			m.RunUntilStop();
			Assert.AreEqual(4, m.State.DataStack.Pop().GetValue());
		}

	}
}
