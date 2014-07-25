using JetBrains.Annotations;
using Lib;

namespace LMachine.Intructions
{
	public class Cons : Instruction
	{
		public Cons()
			: base(InstructionType.Cons)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var y = state.DataStack.Pop();
			var x = state.DataStack.Pop();
			state.DataStack.Push(LValue.FromPair(x, y)); // todo !!! memmory heap managemnet / gc ?
			state.CurrentAddress++;
		}
	}
}