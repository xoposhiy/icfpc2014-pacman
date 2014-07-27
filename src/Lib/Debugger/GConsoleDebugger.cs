using System;
using Lib.GMachine;
using Lib.Parsing;

namespace Lib.Debugger
{
	public static class GConsoleDebugger
	{
		[CanBeNull]
		public static Exception Run([NotNull] GMachine.GMachine m, [CanBeNull] Exception exception = null)
		{
			Console.Clear();
			var console = new FastConsole();
			ShowState(console, m, m.ParseResult, exception);
			while (true)
			{
				var cmd = Console.ReadKey(true);
				if (cmd.Key == ConsoleKey.Spacebar)
					console.Refresh();
				else if (cmd.Modifiers == 0 && (cmd.Key == ConsoleKey.F11 || cmd.Key == ConsoleKey.F10))
				{
					exception = StepSafe1(m.Step);
					ShowState(console, m, m.ParseResult, exception);
				}
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.F12)
				{
					exception = StepSafe2(m.StepBack);
					ShowState(console, m, m.ParseResult, exception);
				}
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.F5)
				{
					exception = StepSafe2(m.RunToEnd);
					ShowState(console, m, m.ParseResult, exception);
				}
				else if (cmd.Modifiers == ConsoleModifiers.Control && cmd.Key == ConsoleKey.R)
				{
					m.ResetState();
					exception = null;
					ShowState(console, m, m.ParseResult, exception);
				}
				else if (cmd.Modifiers == 0 && cmd.Key == ConsoleKey.Escape)
				{
					if (!m.State.Hlt)
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

		private static void ShowState([NotNull] FastConsole console, [NotNull] GMachine.GMachine m, [NotNull] ParseResult<GCmd> prog, [CanBeNull] Exception exception)
		{
			using (var view = new GConsoleDebuggerStateView(console, m, prog, exception))
			{
				view.ShowState(true);
			}
		}

		[CanBeNull]
		private static Exception StepSafe1([NotNull] Func<bool> step)
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

		[CanBeNull]
		private static Exception StepSafe2([NotNull] Action step)
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