namespace Lib.LMachine.Intructions
{
	public class Ldc : Instruction
	{
		public Ldc(int value)
			: base(InstructionType.Ldc)
		{
			Value = value;
		}

		public int Value { get; private set; }

		public override void Execute([NotNull] LMachineState state)
		{
			state.DataStack.Push(LValue.FromInt(Value));
			state.CurrentAddress++;
		}
	}
}