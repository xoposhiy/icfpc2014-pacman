using JetBrains.Annotations;
using Lib;

namespace LMachine.Intructions
{
	public class Add : Instruction
	{
		public Add()
			: base(InstructionType.Add)
		{
		}

		public override void Execute([NotNull] LMachineState state)
		{
			var y = state.DataStack.Pop().GetValue();
			var x = state.DataStack.Pop().GetValue();
			var z = unchecked (x + y);
			state.DataStack.Push(LValue.FromInt(z));
			state.CurrentAddress++;
		}
	}
}