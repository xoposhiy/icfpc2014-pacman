using System;
using System.Collections.Generic;
using System.Linq;
using Lib.LMachine;
using Lib.LMachine.Intructions;
using Lib.Parsing;

namespace Lib.Debugger
{
	public class ConsoleDebuggerStateView : IDisposable
	{
		private readonly LMachineInterpreter m;
		private readonly ParseResult<Instruction> prog;
		private readonly Exception exception;
		private readonly FastConsole.FastConsoleWriter console;

		public ConsoleDebuggerStateView([NotNull] FastConsole console, [NotNull] LMachineInterpreter m, [NotNull] ParseResult<Instruction> prog, [CanBeNull] Exception exception)
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
				console.Write(string.Format("{0,3} ", i + 1));
				if (i == prog.SourceLines[m.State.CurrentAddress] - 1)
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
				WriteCodeLine(prog.CodeLines[i], i == prog.SourceLines[m.State.CurrentAddress] - 1);
				console.ResetColor();
			}
			DumpDataStack();
			DumpFrames();
			DumpControlStack();
			if (m.State.Stopped)
			{
				console.ForegroundColor = ConsoleColor.White;
				console.BackgroundColor = ConsoleColor.Red;
				console.WriteLine("LMachine stopped. Press Escape to exit");
				console.ResetColor();
			}
			else if (exception != null)
			{
				console.ForegroundColor = ConsoleColor.White;
				console.BackgroundColor = ConsoleColor.Red;
				console.WriteLine("LMachine failed: " + exception);
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

		private void DumpControlStack()
		{
			if (!m.State.ControlStack.IsEmpty)
			{
				console.WriteLine();
				console.ForegroundColor = ConsoleColor.Cyan;
				console.WriteLine("Control Stack:");
				console.ResetColor();
				foreach (var item in m.State.ControlStack.Reverse())
				{
					switch (item.Tag)
					{
						case CTag.Join:
							console.Write("Join ");
							WriteAddress(item.Address);
							break;
						case CTag.Ret:
							console.Write("Ret ");
							WriteAddress(item.Address);
							break;
						case CTag.Frame:
							console.Write("Frame ");
							WriteFrameMeta(item.Frame);
							break;
						default:
							throw new InvalidOperationException(String.Format("Invalid tag of constrol stack item: {0}", item));
					}
					console.WriteLine();
				}
			}
		}

		private void DumpDataStack()
		{
			console.WriteLine();
			console.ForegroundColor = ConsoleColor.Cyan;
			console.WriteLine("Data stack:");
			console.ResetColor();
			foreach (var value in m.State.DataStack.Reverse())
			{
				WriteValue(value);
				console.WriteLine();
			}
		}

		private void WriteValue([NotNull] LValue value)
		{
			switch (value.Tag)
			{
				case LTag.Int:
					console.Write(value.GetValue());
					return;
				case LTag.Pair:
					WritePair(value.GetPair());
					return;
				case LTag.Closure:
					WriteClosure(value.GetClosure());
					return;
				default:
					throw new InvalidOperationException(String.Format("Invalid tag of value: {0}", value));
			}
		}

		private void WritePair([NotNull] Pair pair)
		{
			console.Write("(");
			WriteValue(pair.Head);
			console.Write(", ");
			WriteValue(pair.Tail);
			console.Write(")");
		}

		private void WriteClosure([NotNull] Closure closure)
		{
			console.Write("{");
			WriteAddress(closure.Address);
			console.Write(": ");
			WriteFrameMeta(closure.Frame);
			console.Write("}");
		}

		private void WriteAddress(uint address)
		{
			console.ForegroundColor = ConsoleColor.Green;
			console.Write(prog.SourceLines[address]);
			if (prog.AddressNames[address] != null)
			{
				console.ForegroundColor = ConsoleColor.White;
				console.Write(string.Format(" {0}", prog.AddressNames[address]));
			}
			console.ResetColor();
		}

		private void DumpFrames()
		{
			console.WriteLine();
			console.ForegroundColor = ConsoleColor.Cyan;
			console.Write("Current frame: ");
			console.ResetColor();
			WriteFrameMeta(m.State.CurrentFrame);
			if (m.State.CurrentFrame != null)
			{
				console.Write(" --> ");
				WriteFrameMeta(m.State.CurrentFrame.Parent);
			}
			console.WriteLine();
			WriteFrameValues(m.State.CurrentFrame);

			console.WriteLine();
			console.ForegroundColor = ConsoleColor.Cyan;
			console.Write("All frames: ");
			console.ResetColor();
			console.WriteLine();
			var first = true;
			foreach (var frame in GetFrames())
			{
				if (first)
				{
					console.WriteLine();
					first = false;
				}
				WriteFrameMeta(frame);
				console.Write(" --> ");
				WriteFrameMeta(frame.Parent);
				console.WriteLine();
				WriteFrameValues(frame);
			}
		}

		private void WriteFrameValues([CanBeNull] Frame frame)
		{
			if (frame != null && !frame.IsDum)
			{
				foreach (var value in frame.Values)
				{
					WriteValue(value);
					console.WriteLine();
				}
			}
		}

		[NotNull]
		private IEnumerable<Frame> GetFrames()
		{
			var frames = new HashSet<Frame>();
			if (m.State.CurrentFrame != null)
				frames.Add(m.State.CurrentFrame);
			foreach (var value in m.State.DataStack)
				GetFrames(value, frames);
			foreach (var item in m.State.ControlStack)
				if (item.Frame != null)
					frames.Add(item.Frame);
			var queue = new Queue<Frame>(frames);
			while (queue.Count > 0)
			{
				var frame = queue.Dequeue();
				if (frame.Parent != null && frames.Add(frame.Parent))
					queue.Enqueue(frame.Parent);
			}
			return frames.OrderBy(x => x.Id);
		}

		private static void GetFrames([NotNull] LValue value, [NotNull] HashSet<Frame> frames)
		{
			switch (value.Tag)
			{
				case LTag.Int:
					break;
				case LTag.Pair:
					GetFrames(value.GetPair().Head, frames);
					GetFrames(value.GetPair().Tail, frames);
					break;
				case LTag.Closure:
					var frame = value.GetClosure().Frame;
					if (frame != null)
						frames.Add(frame);
					break;
				default:
					throw new InvalidOperationException("TODO");
			}
		}

		private void WriteFrameMeta([CanBeNull] Frame frame)
		{
			if (frame == null)
			{
				console.ForegroundColor = ConsoleColor.Red;
				console.Write("no_frame");
				console.ResetColor();
			}
			else if (frame.IsDum)
			{
				console.ForegroundColor = ConsoleColor.Red;
				console.Write("DumFrame_" + frame.Id);
				console.ResetColor();
			}
			else
			{
				console.ForegroundColor = ConsoleColor.Magenta;
				console.Write("Frame_" + frame.Id);
				console.ResetColor();
			}
		}
	}
}