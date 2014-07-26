using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lib;
using Lib.LispLang;
using Lib.LMachine;
using Lib.LMachine.Parsing;

namespace LMRun
{
	class Program
	{
		static void Main(string[] args)
		{
			Run("generated");
		}

		static void Run(string name)
		{
//			var p = File.ReadAllText(KnownPlace.GccSamples + name + ".mgcc");
			var p = Samples.anySample;
			var prog = LParser.Parse(p);
			var m = new LMachineInterpreter(prog.Program);
			while (!m.State.Stopped)
			{
				ShowState(m, prog);
				var cmd = Console.ReadKey();
				if (cmd.Key == ConsoleKey.F10)
					m.Step();
				else if (cmd.Key == ConsoleKey.F5)
					m.RunUntilStop();
			}
			ShowState(m, prog);
			while (Console.ReadKey().Key != ConsoleKey.Escape)
			{
			}
		}

		private static void ShowState(LMachineInterpreter m, LParseResult prog)
		{
			var left = Console.CursorLeft;
			var top = Console.CursorTop;
			var windowLeft = Console.WindowLeft;
			var windowTop = Console.WindowTop;
			Console.Clear();
			for (int i = 0; i < prog.CodeLines.Length; i++)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("{0,3} ", i + 1);
				if (i == prog.SourceLines[m.State.CurrentAddress] - 1)
				{
					Console.ForegroundColor = ConsoleColor.Black;
					Console.BackgroundColor = ConsoleColor.Cyan;
				}
				else
					Console.ResetColor();
				WriteCodeLine(prog.CodeLines[i], i == prog.SourceLines[m.State.CurrentAddress] - 1);
				Console.ResetColor();
			}
			DumpDataStack(prog, m);
			DumpFrames(prog, m);
			DumpControlStack(prog, m);
			if (m.State.Stopped)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.Red;
				Console.WriteLine("LMachine stopped. Press Escape to exit");
				Console.ResetColor();
			}
			Console.SetCursorPosition(left, top);
			Console.SetWindowPosition(windowLeft, windowTop);
		}

		private static void WriteCodeLine([NotNull] CodeLine codeLine, bool active)
		{
			if (active)
			{
				Console.ForegroundColor = ConsoleColor.Black;
				Console.BackgroundColor = ConsoleColor.Cyan;
				Console.WriteLine(codeLine);
				Console.ResetColor();
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(codeLine.Label);
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.Write(codeLine.Constant);
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write(codeLine.Command);
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.Write(codeLine.Comment);
				Console.ResetColor();
				Console.WriteLine();
			}
		}

		private static void DumpControlStack([NotNull] LParseResult prog, [NotNull] LMachineInterpreter m)
		{
			if (!m.State.ControlStack.IsEmpty)
			{
				Console.WriteLine();
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine("Control Stack:");
				Console.ResetColor();
				foreach (var item in m.State.ControlStack.Reverse())
				{
					switch (item.Tag)
					{
						case CTag.Join:
							Console.Write("Join ");
							WriteAddress(prog, item.Address);
							break;
						case CTag.Ret:
							Console.Write("Ret ");
							WriteAddress(prog, item.Address);
							break;
						case CTag.Frame:
							Console.Write("Frame ");
							WriteFrameMeta(item.Frame);
							break;
						default:
							throw new InvalidOperationException(string.Format("Invalid tag of constrol stack item: {0}", item));
					}
					Console.WriteLine();
				}
			}
		}

		private static void DumpDataStack([NotNull] LParseResult prog, [NotNull] LMachineInterpreter m)
		{
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("Data stack:");
			Console.ResetColor();
			foreach (var value in m.State.DataStack.Reverse())
			{
				WriteValue(prog, value);
				Console.WriteLine();
			}
		}

		private static void WriteValue([NotNull] LParseResult prog, [NotNull] LValue value)
		{
			switch (value.Tag)
			{
				case LTag.Int:
					Console.Write(value.GetValue());
					return;
				case LTag.Pair:
					WritePair(prog, value.GetPair());
					return;
				case LTag.Closure:
					WriteClosure(prog, value.GetClosure());
					return;
				default:
					throw new InvalidOperationException(string.Format("Invalid tag of value: {0}", value));
			}
		}

		private static void WritePair([NotNull] LParseResult prog, [NotNull] Pair pair)
		{
			Console.Write("(");
			WriteValue(prog, pair.Head);
			Console.Write(", ");
			WriteValue(prog, pair.Tail);
			Console.Write(")");
		}

		private static void WriteClosure([NotNull] LParseResult prog, [NotNull] Closure closure)
		{
			Console.Write("{");
			WriteAddress(prog, closure.Address);
			Console.Write(": ");
			WriteFrameMeta(closure.Frame);
			Console.Write("}");
		}

		private static void WriteAddress([NotNull] LParseResult prog, uint address)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(prog.SourceLines[address]);
			if (prog.AddressNames[address] != null)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(" {0}", prog.AddressNames[address]);
			}
			Console.ResetColor();
		}

		private static void DumpFrames([NotNull] LParseResult prog, [NotNull] LMachineInterpreter m)
		{
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("Current frame: ");
			Console.ResetColor();
			WriteFrameMeta(m.State.CurrentFrame);
			if (m.State.CurrentFrame != null)
			{
				Console.Write(" --> ");
				WriteFrameMeta(m.State.CurrentFrame.Parent);
			}
			Console.WriteLine();
			WriteFrameValues(prog, m, m.State.CurrentFrame);
			
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("All frames: ");
			Console.ResetColor();
			Console.WriteLine();
			bool first = true;
			foreach (var frame in GetFrames(m))
			{
				if (first)
				{
					Console.WriteLine();
					first = false;
				}
				WriteFrameMeta(frame);
				Console.Write(" --> ");
				WriteFrameMeta(frame.Parent);
				Console.WriteLine();
				WriteFrameValues(prog, m, frame);
			}
		}

		private static void WriteFrameValues([NotNull] LParseResult prog, [NotNull] LMachineInterpreter m, [CanBeNull] Frame frame)
		{
			if (frame != null && !frame.IsDum)
			{
				foreach (var value in frame.Values)
				{
					WriteValue(prog, value);
					Console.WriteLine();
				}
			}
		}

		[NotNull]
		private static IEnumerable<Frame> GetFrames([NotNull] LMachineInterpreter m)
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

		private static void WriteFrameMeta([CanBeNull] Frame frame)
		{
			if (frame == null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("no_frame");
				Console.ResetColor();
			}
			else if (frame.IsDum)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("DumFrame_" + frame.Id);
				Console.ResetColor();
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.Write("Frame_" + frame.Id);
				Console.ResetColor();
			}
		}
	}
}
