namespace Lib.LMachine.Intructions
{
	public class Tsel : Instruction
	{
		public Tsel(uint trueAddress, uint falseAddress)
			: base(InstructionType.Tsel)
		{
			TrueAddress = trueAddress;
			FalseAddress = falseAddress;
		}

		public uint TrueAddress { get; private set; }
		public uint FalseAddress { get; private set; }

		public override void Execute([NotNull] LMachineState state)
		{
			var x = state.DataStack.Pop().GetValue();
			state.CurrentAddress = x == 0 ? FalseAddress : TrueAddress;
		}
	}
}