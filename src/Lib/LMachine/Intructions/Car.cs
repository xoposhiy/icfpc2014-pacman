namespace Lib.LMachine.Intructions
{
	public class Car : Instruction
	{
		public Car()
			: base(InstructionType.Car)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var x = state.DataStack.Pop().GetPair();
			state.DataStack.Push(x.Head);
			state.CurrentAddress++;
		}
	}
}