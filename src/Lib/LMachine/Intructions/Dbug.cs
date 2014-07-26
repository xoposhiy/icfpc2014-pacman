using System;

namespace Lib.LMachine.Intructions
{
	public class Dbug : Instruction
	{
		public Dbug()
			: base(InstructionType.Dbug)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var info = state.DataStack.Pop();
			Console.WriteLine(info);
			state.CurrentAddress++;
		}
	}
}