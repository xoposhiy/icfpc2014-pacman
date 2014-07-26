namespace Lib.LMachine.Intructions
{
	public class Ap : Instruction
	{
		public Ap(uint frameSize)
			: base(InstructionType.Ap)
		{
			FrameSize = frameSize;
		}

		public uint FrameSize { get; set; }

		public override void Execute([NotNull] LMachineState state)
		{
			var f = state.DataStack.Pop().GetClosure();
			var args = PopArgs(state, FrameSize);
			var fp = Frame.ForFunctionCall(f.Frame, args);
			state.ControlStack.Push(ControlStackItem.ForFrame(state.CurrentFrame));
			state.ControlStack.Push(ControlStackItem.ForRet(state.CurrentAddress + 1));
			state.CurrentFrame = fp;
			state.CurrentAddress = f.Address;
		}
	}
}