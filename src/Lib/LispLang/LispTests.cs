using System;
using Lib.LMachine;
using Lib.LMachine.Parsing;
using NUnit.Framework;

namespace Lib.LispLang
{
	public class LispTests : Lisp
	{
		[Test]
		public void InnerFuncs()
		{
			var macro = Compile(
				Call("main", Args(1)),
				Cmd("RTN", new SExpr()),
				Def("main", ArgNames("x"), 
					new[]{
						Call("f", Call("f", 42)),
						Cmd("RTN"),
						Def("f", ArgNames("y"), Cmd("Add", "x", "y")),
					})
				);
			Console.WriteLine(macro);
			var parsed = LParser.Parse(macro);
			var m = new LMachineInterpreter(parsed.Program);
			m.RunUntilStop();
			Assert.AreEqual("44", m.State.DataStack.Pop().ToString());
		}
		[Test]
		public void CompileSample()
		{
			var macro = Compile(
				Call("get", Call("sampleList"), 3),
				Cmd("RTN", new SExpr()),
				Def("sampleList", ArgNames(), List(1, 2, 3, 4)),
				Def("get", ArgNames("list", "index"),
					If("index",
						Call("get", Cdr("list"), Sub("index", 1)),
						Car("list")))
				);
			Console.WriteLine(macro);
			var parsed = LParser.Parse(macro);
			var m = new LMachineInterpreter(parsed.Program);
			m.RunUntilStop();
			Assert.AreEqual("4", m.State.DataStack.Pop().ToString());
		}
	}
}
