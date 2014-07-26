using System;
using Lib.LMachine.Intructions;
using NUnit.Framework;

namespace Lib.LMachine
{
	[TestFixture]
	public class LMachineInterpreter_Test
	{
		[TestCase(0, 0, 0)]
		[TestCase(-2, 1, -1)]
		[TestCase(int.MaxValue, 1, int.MinValue)]
		[TestCase(int.MinValue, -1, int.MaxValue)]
		[TestCase(int.MinValue, int.MaxValue, -1)]
		public void Add(int x, int y, int expectedResult)
		{
			var program = new Instruction[]
			{
				new Ldc(x),
				new Ldc(y),
				new Add(),
				new Rtn(),
			};
			Assert.That(RunProgram(program).GetValue(), Is.EqualTo(expectedResult));
		}

		[TestCase(0, 0, 0)]
		[TestCase(-2, 1, -3)]
		[TestCase(int.MaxValue, -1, int.MinValue)]
		[TestCase(int.MinValue, 1, int.MaxValue)]
		[TestCase(int.MinValue, int.MaxValue, 1)]
		public void Sub(int x, int y, int expectedResult)
		{
			var program = new Instruction[]
			{
				new Ldc(x),
				new Ldc(y),
				new Sub(),
				new Rtn(),
			};
			Assert.That(RunProgram(program).GetValue(), Is.EqualTo(expectedResult));
		}

		[TestCase(0, 0, 0)]
		[TestCase(0, 1, 0)]
		[TestCase(-2, 1, -2)]
		[TestCase(int.MaxValue, -1, -int.MaxValue)]
		[TestCase(int.MinValue, 1, int.MinValue)]
		[TestCase(int.MinValue, int.MaxValue, -2147483648)]
		public void Mul(int x, int y, int expectedResult)
		{
			var program = new Instruction[]
			{
				new Ldc(x),
				new Ldc(y),
				new Mul(),
				new Rtn(),
			};
			Assert.That(RunProgram(program).GetValue(), Is.EqualTo(expectedResult));
		}

		[TestCase(0, 1, 0)]
		[TestCase(1, 2, 0)]
		[TestCase(-2, 1, -2)]
		[TestCase(-3, 2, -2)]
		[TestCase(3, -2, -2)]
		[TestCase(3, 2, 1)]
		[TestCase(-3, -2, 1)]
		[TestCase(int.MaxValue, -1, -int.MaxValue)]
		[TestCase(int.MinValue, 1, int.MinValue)]
		[TestCase(int.MinValue, int.MaxValue, -2)]
		public void Div(int x, int y, int expectedResult)
		{
			var program = new Instruction[]
			{
				new Ldc(x),
				new Ldc(y),
				new Div(),
				new Rtn(),
			};
			Assert.That(RunProgram(program).GetValue(), Is.EqualTo(expectedResult));
		}

		[CanBeNull]
		private static LValue RunProgram([NotNull] Instruction[] program)
		{
			var lm = new LMachineInterpreter(program);
			while (!lm.State.Stopped)
				lm.Step();
			Assert.That(lm.State.ControlStack.IsEmpty);
			Assert.That(lm.State.CurrentFrame, Is.Null);
			var data = lm.State.DataStack;
			if (data.IsEmpty)
				return null;
			var result = data.Pop();
			Console.Out.WriteLine(result);
			Assert.That(data.IsEmpty);
			return result;
		}
	}
}