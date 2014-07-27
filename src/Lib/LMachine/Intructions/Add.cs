namespace Lib.LMachine.Intructions
{
	public class Add : Instruction
	{
		public Add()
			: base(InstructionType.Add)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var y = state.DataStack.PopValue();
			var x = state.DataStack.PopValue();
			var z = unchecked (x + y);
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}
	}
}