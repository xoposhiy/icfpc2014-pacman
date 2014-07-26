using System;
using Lib.LMachine;
using NUnit.Framework;

namespace Lib.LispLang
{
	public class Samples : Api
	{
		public static string anySample = Compile(
			Call("any", List(1), Fun("gtzero")), // Fun дает понять, что gtzero не нужно искать в Env, а нужно просто найти функцию с таким именем 
			Call("any", List(0, 1), Fun("gtzero")),
			Call("any", List(0, 0), Fun("gtzero")),
			Return(),
			Def("gtzero", ArgNames("x"), IsGreater("x", 0)),
			Def("any", ArgNames("list", "f"),
				If(Atom("list"),
					0,
					If(CallFunRef("f", Car("list")), // CallFunRef дает понять, что f надо искать в Env
						1, 
						Call("any", Cdr("list"), "f")
					)))
			);

		[Test]
		public void Any()
		{
			Check(anySample, 0, 1, 1);
		}

		[Test]
		public void PassFunctionAsArg()
		{
			var code = Compile(
				Call("apply", Fun("inc"), 42),
				Return(),
				Def("inc", ArgNames("x"),
					Add("x", 1)),
				Def("apply", ArgNames("f", "x"),
					CallFunRef("f", "x"))
					);
			var dataStack = LMachineInterpreter.Run(code).DataStack;
			Assert.AreEqual(43, dataStack.Pop().GetValue());
		}

		public static readonly string max = Compile(
			Call("max", List(1)),
			Call("max", List(1, 2, 3)),
			Call("max", List(3, 2, 1)),
			Return(),
			Def("max", ArgNames("list"),
				Call("_max_iter", "list", int.MinValue),
				Return(),
				Def("_max_iter", ArgNames("list", "maxValue"),
					If(Atom("list"), 
						"maxValue",
						Call("_max_iter", Cdr("list"), Max(Car("list"), "maxValue"))
						)
				)
			)
			);

		[Test]
		public void TestMax()
		{
			Check(max, 3, 3, 1);
		}

		private void Check(string code, params int[] expectedStack)
		{
			Console.WriteLine(code);
			var state = LMachineInterpreter.Run(code);
			foreach (var expectedItem in expectedStack)
				Assert.AreEqual(expectedItem, state.DataStack.Pop().GetValue());
			Assert.IsTrue(state.DataStack.IsEmpty);
		}
	}
}
