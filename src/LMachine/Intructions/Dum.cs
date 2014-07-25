using JetBrains.Annotations;
using Lib;

namespace LMachine.Intructions
{
	public class Dum : Instruction
	{
		public Dum(uint frameSize)
			: base(InstructionType.Dum)
		{
			FrameSize = frameSize;
		}

		public uint FrameSize { get; private set; }

		public override void Execute([NotNull] LMachineState state)
		{
			var fp = Frame.Dum(state.CurrentFrame, FrameSize);
			state.CurrentFrame = fp;
			state.CurrentAddress++;
		}
	}
}