using System;
using Lib.LMachine;
using Lib.LMachine.Intructions;
using Lib.Parsing;

namespace Lib.Debugger
{
	public static class LConsoleDebugger
	{
		[CanBeNull]
		public static Exception Run([NotNull] LMachineInterpreter m, [NotNull] ParseResult<Instruction> prog, [CanBeNull] Exception exception = null)
		{
			Console.Clear();
			var console = new FastConsole();
			ShowState(console, m, prog, exception);
			while (true)
			{
				var cmd = Console.ReadKey(true);
				if (cmd.Key == ConsoleKey.Spacebar)
					console.Refresh();
				else if (cmd.Modifiers == ConsoleModifiers.Shift && cmd.Key == ConsoleKey.F11)
				{
					exception = StepSafe(m.StepOut);
					ShowState(console, m, prog, exception);
				}
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.F11)
				{
					exception = StepSafe(m.Step);
					ShowState(console, m, prog, exception);
				}
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.F12)
				{
					exception = StepSafe(m.StepBack);
					ShowState(console, m, prog, exception);
				}
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.F10)
				{
					exception = StepSafe(m.StepOver);
					ShowState(console, m, prog, exception);
				}
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.F5)
				{
					exception = StepSafe(m.RunUntilStop);
					ShowState(console, m, prog, exception);
				}
				else if (cmd.Modifiers == ConsoleModifiers.Control && cmd.Key == ConsoleKey.R)
				{
					m.Restart();
					exception = null;
					ShowState(console, m, prog, exception);
				}
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.Escape)
				{
					if (!m.State.Stopped)
						return new DebuggerAbortedException(exception);
					return null;
				}
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.PageDown)
					NavigateConsole(console, Console.WindowTop + Console.WindowHeight);
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.PageUp)
					NavigateConsole(console, Console.WindowTop - Console.WindowHeight);
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.DownArrow)
					NavigateConsole(console, Console.WindowTop + 1);
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.UpArrow)
					NavigateConsole(console, Console.WindowTop - 1);
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.End)
					NavigateConsole(console, console.Height - Console.WindowHeight);
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.Home)
					NavigateConsole(console, 0);
			}
		}

		private static void NavigateConsole(FastConsole console, int newtop)
		{
			if (newtop > console.Height - Console.WindowHeight)
				newtop = console.Height - Console.WindowHeight;
			if (newtop < 0)
				newtop = 0;
			Console.WindowTop = newtop;
		}

		private static void ShowState([NotNull] FastConsole console, [NotNull] LMachineInterpreter m, [NotNull] ParseResult<Instruction> prog, [CanBeNull] Exception exception)
		{
			using (var view = new LConsoleDebuggerStateView(console, m, prog, exception))
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