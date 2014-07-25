using System;
using JetBrains.Annotations;
using Lib;

namespace LMachine.Intructions
{
	public class Div : Instruction
	{
		public Div()
			: base(InstructionType.Div)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var y = state.DataStack.Pop().GetValue();
			var x = state.DataStack.Pop().GetValue();
			if (y == 0)
				throw new InvalidOperationException("TODO");
			var z = unchecked(x / y); // todo !!! -3 / 2 = -2
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}
	}
}