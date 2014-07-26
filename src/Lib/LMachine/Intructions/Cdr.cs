namespace Lib.LMachine.Intructions
{
	public class Cdr : Instruction
	{
		public Cdr()
			: base(InstructionType.Cdr)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var x = state.DataStack.Pop().GetPair();
			state.DataStack.Push(x.Tail);
			state.CurrentAddress++;
		}
	}
}