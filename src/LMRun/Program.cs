using System;
using Lib.Debugger;
using Lib.Game;
using Lib.LispLang;
using Lib.LMachine;
using Lib.LMachine.Intructions;
using Lib.Parsing.LParsing;

namespace LMRun
{
	public class Program
	{
		private static void Main(string[] args)
		{
			Run("local", "maze1.txt");
		}

		private static void Run(string programName, string worldName)
		{
			//var p = File.ReadAllText(KnownPlace.GccSamples + programName + ".mgcc");
			var p = LocallyGreedyCarefulLambdaManOnList.Code;
			var prog = LParser.Parse(p);
			var world = new World(MapUtils.LoadFromKnownLocation(worldName)).ToLValue();
			var m = new LMachineInterpreter(prog.Program);
			m.State.DataStack.Push(world);
			m.State.DataStack.Push(42);
			m.State.DataStack.Push(LValue.FromClosure(0, null));
			new Tap(2).Execute(m.State);
			ConsoleDebugger.Run(m, prog);
		}
	}
}