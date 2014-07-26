using System;

namespace Lib.LMachine.Intructions
{
	public class Trap : Instruction
	{
		public Trap(uint frameSize)
			: base(InstructionType.Trap)
		{
			FrameSize = frameSize;
		}

		public uint FrameSize { get; private set; }

		public override void Execute([NotNull] LMachineState state)
		{
			var f = state.DataStack.Pop().GetClosure();
			var fp = f.Frame;
			if (fp != state.CurrentFrame)
				throw new InvalidOperationException("TODO");
			if (fp == null || !fp.IsDum)
				throw new InvalidOperationException("TODO");
			var args = PopArgs(state, FrameSize);
			fp.SetValues(args);
			fp.IsDum = false;
			state.CurrentAddress = f.Address;
		}
	}
}