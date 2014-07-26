namespace Lib.LMachine.Intructions
{
	public class Cgt : Instruction
	{
		public Cgt()
			: base(InstructionType.Cgt)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var y = state.DataStack.Pop().GetValue();
			var x = state.DataStack.Pop().GetValue();
			var z = x > y ? 1 : 0;
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}
	}
}