using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.LMachine;
using Lib.LMachine.Intructions;
using Lib.LMachine.Parsing;
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
			var dataStack = LMachineInterpreter.Run(anySample).DataStack;
			Assert.AreEqual(0, dataStack.Pop().GetValue());
			Assert.AreEqual(1, dataStack.Pop().GetValue());
			Assert.AreEqual(1, dataStack.Pop().GetValue());
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
	}
}
