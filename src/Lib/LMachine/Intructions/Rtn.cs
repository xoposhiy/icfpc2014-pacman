using System;

namespace Lib.LMachine.Intructions
{
	public class Rtn : Instruction
	{
		public Rtn()
			: base(InstructionType.Rtn)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			if (state.ControlStack.IsEmpty)
			{
				state.Stopped = true;
				return;
			}
			var x = state.ControlStack.Pop();
			if (x.Tag != CTag.Ret)
				throw new InvalidOperationException("TODO");
			var fp = state.ControlStack.Pop();
			if (fp.Tag != CTag.Frame)
				throw new InvalidOperationException("TODO");
			state.CurrentFrame = fp.Frame;
			state.CurrentAddress = x.Address;
		}
	}
}