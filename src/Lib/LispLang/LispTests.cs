using System;
using Lib.LMachine;
using Lib.LMachine.Intructions;
using Lib.Parsing.LParsing;
using NUnit.Framework;

namespace Lib.LispLang
{
	public class LambdaManAi : Lisp
	{
		public static string Compile()
		{
			return Api.CompileWithLibs(
				Def("main", ArgNames("world", "ghosts"),
					Cons(42, Fun("step"))
				),
				Def("step", ArgNames("ai", "world"),
					Cons(42, 3)
				)
			);
		}
	}

	[TestFixture]
	public class LambdaManAi_Test
	{
		[Test]
		public void Test()
		{
			//Console.Out.WriteLine(LambdaManAi.Compile());
			Console.Out.WriteLine(LParser.Parse(LambdaManAi.Compile()).Program.ToGcc());
		}
	}

	[TestFixture]
	public class ResultGen_Test : Lisp 
	{
		[Test]
		public void Test()
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
		}
	}

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
