using System;

namespace Lib.LMachine.Intructions
{
	public class DbgView : Instruction
	{
		public DbgView()
			: base(InstructionType.DbgView)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var info = state.DataStack.Pop();
			Console.WriteLine(info);
//			Console.ReadKey();
			state.DataStack.Push(info);
			state.CurrentAddress++;
		}
	}
}