namespace Lib.LMachine.Intructions
{
	public class Sub : Instruction
	{
		public Sub()
			: base(InstructionType.Sub)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var y = state.DataStack.Pop().GetValue();
			var x = state.DataStack.Pop().GetValue();
			var z = unchecked(x - y);
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}
	}
}