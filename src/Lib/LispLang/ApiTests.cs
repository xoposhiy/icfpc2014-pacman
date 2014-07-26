using System;
using Lib.LMachine;
using NUnit.Framework;

namespace Lib.LispLang
{
	public class ApiTests : Api
	{
		[Test]
		public void TestMax()
		{
			Check(
				Compile(
					Call("max", List()),
					Call("max", List(1)),
					Call("max", List(0, 1)),
					Call("max", List(0, 1, 0)),
					Call("max", List(0, 5, 1, 6)),
					Return(),
					max),
				6, 1, 1, 1, int.MinValue);
		}

		[Test]
		public void TestMin()
		{
			Check(
				Compile(
					Call("min", List()),
					Call("min", List(1)),
					Call("min", List(0, 1)),
					Call("min", List(5, 1, 5)),
					Call("min", List(0, 5, 1, -6)),
					Return(),
					min),
				-6, 1, 0, 1, int.MaxValue);
		}

		[Test]
		public void TestAny()
		{
			Check(
				Compile(
					Call("any", List(1), Fun("gtzero")), // Fun дает понять, что gtzero не нужно искать в Env, а нужно просто найти функцию с таким именем 
					Call("any", List(0, 1), Fun("gtzero")),
					Call("any", List(0, 0), Fun("gtzero")),
					Return(),
					Def("gtzero", ArgNames("x"), IsGreater("x", 0)),
					Return(),
					any),
				0, 1, 1);
		}

		[Test]
		public void TestArgMax()
		{
			Check(
				Compile(
					Call("argmax", List()),
					Call("argmax", List(1)),
					Call("argmax", List(0, 1)),
					Call("argmax", List(0, 1, 0)),
					Call("argmax", List(0, 5, 1, 6)),
					Return(),
					argmax),
				3, 1, 1, 0, -1);
		}

		[Test]
		public void TestArgMin()
		{
			Check(
				Compile(
					Call("argmin", List()),
					Call("argmin", List(1)),
					Call("argmin", List(1, 0)),
					Call("argmin", List(0, 1)),
					Call("argmin", List(6, 1, 0)),
					Call("argmin", List(2, 5, 1, 6)),
					Return(),
					argmin),
				2, 2, 0, 1, 0, -1);
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