using System;
using System.Linq;
using Lib.LMachine.Intructions;
using Lib.Parsing.LParsing;

namespace Lib.LMachine
{
	public class LMachineInterpreter
	{
		public LMachineInterpreter([NotNull] Instruction[] program, LValue entryPoint, params LValue[] entryPointArguments)
			: this(program)
		{
			foreach (var argument in entryPointArguments)
				State.DataStack.Push(argument);
			State.DataStack.Push(entryPoint);
			new Tap((uint)entryPointArguments.Length).Execute(State);
		}

		public LMachineInterpreter([NotNull] Instruction[] program)
		{
			Program = program;
			State = new LMachineState();
		}

		private LMachineState startupState;

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

		public void Restart()
		{
			if (startupState != null)
			{
				State = startupState;
				startupState = null;
			}
		}

		public void StepBack()
		{
			var stepsCount = State.StepsCount;
			Restart();
			while (!State.Stopped && State.StepsCount < stepsCount - 1)
				Step();
		}

		public void Step()
		{
			if (startupState == null)
			{
				startupState = new LMachineState();
				startupState.StepsCount = State.StepsCount;
				startupState.CurrentAddress = State.CurrentAddress;
				foreach (var item in State.DataStack.Reverse())
					startupState.DataStack.Push(item);
				startupState.CurrentFrame = State.CurrentFrame;
				foreach (var item in State.ControlStack.Reverse())
					startupState.ControlStack.Push(item);
				startupState.Stopped = State.Stopped;
			}
			if (State.Stopped)
				return;
			if (State.CurrentAddress >= Program.Length)
				throw new InvalidOperationException(string.Format("Invalid CurrentAddress: {0}", State.CurrentAddress));
			var instruction = Program[State.CurrentAddress];
			Log(instruction.SourceLineNo+ "\t" +instruction);
			try
			{
				instruction.Execute(State);
			}
			catch (Exception e)
			{
				throw new LException(e);
			}
			State.StepsCount++;
			if (State.StepsCount > 3072 * 1000)
				throw new LMTimeoutException();
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