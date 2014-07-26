using System;
using System.IO;
using Lib.LMachine.Intructions;
using Lib.Parsing.LParsing;

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

		public void StepOver()
		{
			var stackDepth = State.ControlStack.Count;
			do
			{
				Step();
			} while (!State.Stopped && State.ControlStack.Count > stackDepth);
		}

		public void StepOut()
		{
			var stackDepth = State.ControlStack.Count;
			do
			{
				Step();
			} while (!State.Stopped && State.ControlStack.Count >= stackDepth);
		}

		public static void Log(string s)
		{
//			File.AppendAllText("log.txt", s + "\r\n");
		}

		public void Step()
		{
			if (State.Stopped)
				throw new InvalidOperationException("TODO");
			if (State.CurrentAddress >= Program.Length)
				throw new InvalidOperationException("TODO");
			var instruction = Program[State.CurrentAddress];
			Log(instruction.SourceLineNo+ "\t" +instruction.ToString());
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