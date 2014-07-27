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
			while (true)
			{
				ShowState(console, m, prog, exception);
				var cmd = Console.ReadKey(true);
				if (cmd.Key == ConsoleKey.Spacebar)
					console.Refresh();
				else if (cmd.Modifiers == ConsoleModifiers.Shift && cmd.Key == ConsoleKey.F11)
					exception = StepSafe(m.StepOut);
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.F11)
					exception = StepSafe(m.Step);
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.F10)
					exception = StepSafe(m.StepOver);
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.F5)
					exception = StepSafe(m.RunUntilStop);
				else if (cmd.Modifiers == ConsoleModifiers.Control && cmd.Key == ConsoleKey.R)
				{
					m.Restart();
					exception = null;
				}
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.Escape)
				{
					if (!m.State.Stopped)
						return new DebuggerAbortedException(exception);
					return null;
				}
			}
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