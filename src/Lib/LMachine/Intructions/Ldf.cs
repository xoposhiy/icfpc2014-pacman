namespace Lib.LMachine.Intructions
{
	public class Ldf : Instruction
	{
		public Ldf(uint address)
			: base(InstructionType.Ldf)
		{
			Address = address;
		}

		public uint Address { get; private set; }

		public override void Execute([NotNull] LMachineState state)
		{
			state.DataStack.Push(LValue.FromClosure(Address, state.CurrentFrame)); // todo !!! memmory heap managemnet / gc ?
			state.CurrentAddress++;
		}
	}
}