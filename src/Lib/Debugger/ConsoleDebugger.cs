using System;
using Lib.LMachine;
using Lib.LMachine.Intructions;
using Lib.Parsing;

namespace Lib.Debugger
{
	public static class ConsoleDebugger
	{
		[CanBeNull]
		public static Exception Run([NotNull] LMachineInterpreter m, [NotNull] ParseResult<Instruction> prog, [CanBeNull] Exception exception = null)
		{
			Console.Clear();
			var console = new FastConsole();
			while (!m.State.Stopped && exception == null)
			{
				ShowState(console, m, prog, exception);
				var cmd = Console.ReadKey(true);
				if (cmd.Key == ConsoleKey.Spacebar)
					console.Refresh();
				else if (cmd.Modifiers == ConsoleModifiers.Shift && cmd.Key == ConsoleKey.F11)
					exception = StepSafe(m.StepOut);
				else if (cmd.Key == ConsoleKey.F11)
					exception = StepSafe(m.Step);
				else if (cmd.Key == ConsoleKey.F10)
					exception = StepSafe(m.StepOver);
				else if (cmd.Key == ConsoleKey.F5)
					exception = StepSafe(m.RunUntilStop);
			}
			ShowState(console, m, prog, exception);
			return exception;
		}

		private static void ShowState([NotNull] FastConsole console, [NotNull] LMachineInterpreter m, [NotNull] ParseResult<Instruction> prog, [CanBeNull] Exception exception)
		{
			using (var view = new ConsoleDebuggerStateView(console, m, prog, exception))
			{
				view.ShowState(true);
			}
		}

		[CanBeNull]
		private static Exception StepSafe([NotNull] Action step)
		{
			try
			{
				step();
				return null;
			}
			catch (Exception e)
			{
				return e;
			}
		}
	}
}