using JetBrains.Annotations;
using Lib;

namespace LMachine.Intructions
{
	public class Tap : Instruction
	{
		public Tap(uint frameSize)
			: base(InstructionType.Tap)
		{
			FrameSize = frameSize;
		}

		public uint FrameSize { get; private set; }

		public override void Execute([NotNull] LMachineState state)
		{
			var f = state.DataStack.Pop().GetClosure();
			var args = PopArgs(state, FrameSize);
			var fp = Frame.ForFunctionCall(f.Frame, args); // todo !!! use current frame if there are no usages
			state.CurrentFrame = fp;
			state.CurrentAddress = f.Address;
		}
	}
}