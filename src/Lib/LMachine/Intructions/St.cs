namespace Lib.LMachine.Intructions
{
	public class St : Instruction
	{
		public St(uint frameIndex, uint valueIndex)
			: base(InstructionType.St)
		{
			FrameIndex = frameIndex;
			ValueIndex = valueIndex;
		}

		public uint FrameIndex { get; private set; }
		public uint ValueIndex { get; private set; }

		public override void Execute([NotNull] LMachineState state)
		{
			var fp = GetParentFrame(state.CurrentFrame, FrameIndex);
			var v = state.DataStack.Pop();
			fp.SetValue(ValueIndex, v);
			state.CurrentAddress++;
		}
	}
}