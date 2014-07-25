using System;
using JetBrains.Annotations;

namespace LMachine.Intructions
{
	public class Rap : Instruction
	{
		public Rap(uint frameSize)
			: base(InstructionType.Rap)
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
			state.ControlStack.Push(ControlStackItem.ForFrame(fp.Parent));
			state.ControlStack.Push(ControlStackItem.ForRet(state.CurrentAddress + 1));
			fp.IsDum = false;
			state.CurrentAddress = f.Address;
		}
	}
}