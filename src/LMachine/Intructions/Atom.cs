using JetBrains.Annotations;
using Lib;

namespace LMachine.Intructions
{
	public class Atom : Instruction
	{
		public Atom()
			: base(InstructionType.Atom)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var x = state.DataStack.Pop();
			var z = x.Tag == LTag.Int ? 1 : 0;
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}
	}
}