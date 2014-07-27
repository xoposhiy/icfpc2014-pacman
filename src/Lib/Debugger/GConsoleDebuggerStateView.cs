using System;
using System.Collections.Generic;
using Lib.GMachine;
using Lib.Parsing;

namespace Lib.Debugger
{
	public class GConsoleDebuggerStateView : IDisposable
	{
		private readonly GMachine.GMachine m;
		private readonly ParseResult<GCmd> prog;
		private readonly Exception exception;
		private readonly FastConsole.FastConsoleWriter console;

		public GConsoleDebuggerStateView([NotNull] FastConsole console, [NotNull] GMachine.GMachine m, [NotNull] ParseResult<GCmd> prog, [CanBeNull] Exception exception)
		{
			this.m = m;
			this.prog = prog;
			this.exception = exception;
			this.console = console.BeginWrite();
		}

		public void ShowState(bool followCurrentAddress)
		{
			for (var i = 0; i < prog.CodeLines.Length; i++)
			{
				console.ForegroundColor = ConsoleColor.Green;
				console.Write(string.Format("{0,3} ", prog.CodeLines[i].ProgramLine));
				if (i == prog.SourceLines[m.State.Pc] - 1)
				{
					console.ForegroundColor = ConsoleColor.Black;
					console.BackgroundColor = ConsoleColor.Cyan;
					if (followCurrentAddress)
					{
						Console.SetCursorPosition(0, i);
						if (Console.WindowHeight - (i - Console.WindowTop) < 10)
							Console.WindowTop = 10 + i - Console.WindowHeight;
					}
				}
				else
					console.ResetColor();
				WriteCodeLine(prog.CodeLines[i], i == prog.SourceLines[m.State.Pc] - 1);
				console.ResetColor();
			}

			DumpRegisters();
			DumpDataMemory();

			if (m.State.Hlt)
			{
				console.ForegroundColor = ConsoleColor.White;
				console.BackgroundColor = ConsoleColor.Red;
				console.WriteLine("GMachine stopped. Press Escape to exit");
				console.ResetColor();
			}
			else if (exception != null)
			{
				console.ForegroundColor = ConsoleColor.White;
				console.BackgroundColor = ConsoleColor.Red;
				console.WriteLine("GMachine failed: " + exception);
				console.WriteLine("Press Escape to exit");
				console.WriteLine("Press Ctrl+R to restart");
				console.ResetColor();
			}
		}

		public void Dispose()
		{
			console.EndWrite();
		}

		private void WriteCodeLine([NotNull] CodeLine codeLine, bool active)
		{
			if (active)
			{
				console.ForegroundColor = ConsoleColor.Black;
				console.BackgroundColor = ConsoleColor.Cyan;
				console.WriteLine(codeLine);
				console.ResetColor();
			}
			else
			{
				console.ForegroundColor = ConsoleColor.White;
				console.Write(codeLine.Label);
				console.ForegroundColor = ConsoleColor.Yellow;
				console.Write(codeLine.Constant);
				console.ForegroundColor = ConsoleColor.Gray;
				console.Write(codeLine.Command);
				console.ForegroundColor = ConsoleColor.DarkYellow;
				console.Write(codeLine.Comment);
				console.ResetColor();
				console.WriteLine();
			}
		}
		
		private void DumpDataMemory()
		{
			console.WriteLine();
			console.ForegroundColor = ConsoleColor.Cyan;
			console.WriteLine("Data memory:");
			console.ResetColor();
			foreach (var value in m.State.DataMemory)
			{
				WriteValue(value);
				console.WriteLine();
			}
		}

		private void WriteValue(byte value)
		{
			console.Write(value);
			if (prog.AddressNames.Length >= value && !string.IsNullOrEmpty(prog.AddressNames[value]))
			{
				console.Write(" (");
				console.ForegroundColor = ConsoleColor.White;
				console.Write(prog.AddressNames[value]);
				console.ResetColor();
				console.Write(")");
			}
			List<string> constants;
			if (prog.Constants.TryGetValue(value, out constants))
			{
				console.Write(" (");
				console.ForegroundColor = ConsoleColor.Yellow;
				foreach (var constant in constants)
					console.Write(constant);
				console.ResetColor();
				console.Write(")");
			}
		}

		private void DumpRegisters()
		{
			console.WriteLine();
			console.ForegroundColor = ConsoleColor.Cyan;
			console.WriteLine("Registers:");
			console.ResetColor();
			for (int i = 0; i < m.State.Registers.Length; i++)
			{
				console.ForegroundColor = ConsoleColor.Magenta;
				console.Write((char)i + 'a');
				console.ResetColor();
				console.Write(" = ");
				WriteValue(m.State.Registers[i]);
			}
		}
	}
}