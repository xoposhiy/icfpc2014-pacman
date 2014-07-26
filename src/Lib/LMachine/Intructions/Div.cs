using System;

namespace Lib.LMachine.Intructions
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
				throw new InvalidOperationException("Division by zero");
			var z = div(x, y);
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}

		public static int div(int a, int b)
		{
			unchecked
			{
				var div = a / b;
				var rem = a % b;
				if (rem != 0 && (a ^ b) < 0)
					return div - 1;
				return div;
			}
		}
	}
}