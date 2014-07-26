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
				throw new InvalidOperationException("Division by zero");
			var z = floor_div2(x, y);
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}

		public static int floor_div2(int a, int b)
		{
			unchecked
			{
				var rem = a % b;
				var div = a / b;
				if (rem == 0)
					a = b;
				var sub = a ^ b;
				sub >>= 31;
				return div + sub;
			}
		}
	}
}