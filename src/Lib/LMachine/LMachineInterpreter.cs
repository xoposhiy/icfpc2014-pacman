using System;
using Lib.LMachine.Intructions;

namespace Lib.LMachine
{
	public class LMachineInterpreter
	{
		public LMachineInterpreter([NotNull] Instruction[] program)
		{
			Program = program;
			State = new LMachineState();
		}

		[NotNull]
		public Instruction[] Program { get; private set; }

		[NotNull]
		public LMachineState State { get; private set; }

		public void Step()
		{
			if (State.Stopped)
				throw new InvalidOperationException("TODO");
			if (State.CurrentAddress >= Program.Length)
				throw new InvalidOperationException("TODO");
			var instruction = Program[State.CurrentAddress];
			instruction.Execute(State);
		}
	}
}