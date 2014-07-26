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
				Def("sampleList", ArgNames(), List(1, 2, 3, 4)),
				Def("get", ArgNames("list", "index"), 
					If("index", 
						Call("get", Args(Cdr("list"), Sub("index", 1))),
						Car("list")))
				);
			Console.WriteLine(macro);
			var parsed = LParser.Parse(macro);
			var m = new LMachineInterpreter(parsed.Program);
			m.RunUntilStop();
			Assert.AreEqual("4", m.State.DataStack.Pop().ToString());
		}

		[Test]
		public void TestEnqueue()
		{
			var macro = Compile(
				Call("enqueue", Call("sampleQueue"), 3),
				Cmd("RTN", new SExpr()),
				Def("sampleQueue", ArgNames(), Cons(List(), List())),
				Enqueue());

			Console.WriteLine(macro);
			var parsed = LParser.Parse(macro);
			var m = new LMachineInterpreter(parsed.Program);
			m.RunUntilStop();
			Assert.AreEqual("((3, 0), 0)", m.State.DataStack.Pop().ToString());
		}

		private SExpr Enqueue()
		{
			return Def("enqueue", ArgNames("queue", "value"),
				Cons(Cons("value", Car("queue")), Cdr("queue")));
		}
	}
}
