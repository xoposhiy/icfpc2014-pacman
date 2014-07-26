using System;

namespace Lib.LMachine.Intructions
{
	public class Join : Instruction
	{
		public Join()
			: base(InstructionType.Join)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var x = state.ControlStack.Pop();
			if (x.Tag != CTag.Join)
				throw new InvalidOperationException("TODO");
			state.CurrentAddress = x.Address;
		}
	}
}