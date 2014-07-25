using JetBrains.Annotations;

namespace LMachine.Intructions
{
	public class Ld : Instruction
	{
		public Ld(uint frameIndex, uint valueIndex)
			: base(InstructionType.Ld)
		{
			FrameIndex = frameIndex;
			ValueIndex = valueIndex;
		}

		public uint FrameIndex { get; private set; }
		public uint ValueIndex { get; private set; }

		public override void Execute([NotNull] LMachineState state)
		{
			var fp = GetParentFrame(state.CurrentFrame, FrameIndex);
			var v = fp.GetValue(ValueIndex);
			state.DataStack.Push(v);
			state.CurrentAddress++;
		}
	}
}