using System;
using System.Collections;
using Lib.LMachine.Intructions;
using Lib.LMachine.Parsing;

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

		public void RunUntilStop()
		{
			while (!State.Stopped)
				Step();
		}

		public static LMachineState Run(string code)
		{
			var m = new LMachineInterpreter(LParser.Parse(code).Program);
			m.RunUntilStop();
			return m.State;
		}
	}
}