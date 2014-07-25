using JetBrains.Annotations;
using Lib;

namespace LMachine.Intructions
{
	public class Ceq : Instruction
	{
		public Ceq()
			: base(InstructionType.Ceq)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var y = state.DataStack.Pop().GetValue();
			var x = state.DataStack.Pop().GetValue();
			var z = x == y ? 1 : 0;
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}
	}
}