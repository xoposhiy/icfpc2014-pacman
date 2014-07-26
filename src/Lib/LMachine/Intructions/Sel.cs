namespace Lib.LMachine.Intructions
{
	public class Sel : Instruction
	{
		public Sel(uint trueAddress, uint falseAddress)
			: base(InstructionType.Sel)
		{
			TrueAddress = trueAddress;
			FalseAddress = falseAddress;
		}

		public uint TrueAddress { get; private set; }
		public uint FalseAddress { get; private set; }

		public override void Execute([NotNull] LMachineState state)
		{
			var x = state.DataStack.Pop().GetValue();
			state.ControlStack.Push(ControlStackItem.ForJoin(state.CurrentAddress + 1));
			state.CurrentAddress = x == 0 ? FalseAddress : TrueAddress;
		}
	}
}