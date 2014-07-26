using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib;
using Lib.LMachine;
using Lib.LMachine.Parsing;

namespace LMRun
{
	class Program
	{
		static void Main(string[] args)
		{
			Run("get");
		}

		static void Run(string name)
		{
			var p = File.ReadAllText(KnownPlace.GccSamples + name + ".mgcc");
			var prog = LParser.Parse(p);
			var m = new LMachineInterpreter(prog.Program);
			while (true)
			{
				ShowState(m, prog);
				var cmd = Console.ReadLine();
				if (cmd == "")
					m.Step();
			}
		}

		private static void ShowState(LMachineInterpreter m, LParseResult prog)
		{
			var sourceLineIndex = prog.SourceLines[m.State.CurrentAddress];
			Console.WriteLine("Next Instruction: (Line " + sourceLineIndex + ") " + prog.CodeLines[sourceLineIndex-1]);
			Console.WriteLine("Data stack:");
			Console.WriteLine(m.State.DataStack.ToString());
			if (m.State.CurrentFrame == null)
				Console.WriteLine("No frame");
			else if (m.State.CurrentFrame.IsDum)
				Console.WriteLine("Current frame is dummy");
			else
			{
				Console.WriteLine("Current frame values:");
				Console.WriteLine(string.Join("\r\n", m.State.CurrentFrame.Values.Select(v => v.ToString())));
			}
		}
	}
}
