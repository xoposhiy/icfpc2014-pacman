using System;
using Lib.AI;
using Lib.Debugger;
using Lib.Game;
using Lib.LispLang;
using Lib.LMachine;

namespace conPlayer
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			//			LMMain main = w => Tuple.Create(LValue.FromInt(42), (LMStep)ConsoleStep);
			//			LMMain main = w => Tuple.Create(LValue.FromInt(42), (LMStep)GredySimple.Step);
			//			LMMain main = w => Tuple.Create(LValue.FromInt(42), (LMStep)GreedyLambdaMen.LambdaMenGreedyStep);
			//			LMMain main = new LocallyGreedyCarefulLambdaMan().Main;
			var interpretedLambdaMan = new InterpretedLambdaMan(LocallyGreedyCarefulLambdaManOnList.code);
//				, runUntilStopStep: x =>
//			{
//				var ex = ConsoleDebugger.Run(x.Interpreter, x.ProgramParseResult);
//				if (ex != null)
//					throw new DebuggerAbortedException(ex);
//			});
			LMMain main = interpretedLambdaMan.Main;
			var ghostFactory = new RandomGhostFactory();
			//var p = File.ReadAllText(KnownPlace.GccSamples + "sample2.mghc");
			//var ghostFactory = new GMachineFactory(GParser.Parse(p).Program);

			var sim = new GameSim(MapUtils.LoadFromKnownLocation("maze1.txt"), main, ghostFactory);
			var oldState = "";
			Exception exception = null;
			while (!sim.finished)
			{
				try
				{
					sim.Tick();
				}
				catch (Exception e)
				{
					exception = e;
					break;
				}
				var newState = sim.world.ToString();
				if (newState != oldState)
				{
					Console.Clear();
					Console.WriteLine(newState);
					Console.WriteLine("Use Cursor keys to control Lambda Man. Time: {0}", sim.time);
					oldState = newState;
//					Console.ReadKey();
				}
			}
			if (exception != null)
			{
				if (!(exception is DebuggerAbortedException))
					ConsoleDebugger.Run(interpretedLambdaMan.Interpreter, interpretedLambdaMan.ProgramParseResult, exception);
			}
			else
			{
				Console.WriteLine("Game over");
				Console.WriteLine("Score " + sim.world.man.score);
				Console.WriteLine("Press any key to continue...");
			}
			var key = Console.ReadKey();
			while (key.Key != ConsoleKey.Enter)
				key = Console.ReadKey();
		}

		private static Tuple<LValue, Direction> ConsoleStep(LValue currentaistate, World currentworldstate)
		{
			var k = Console.ReadKey();
			if (k.Key == ConsoleKey.LeftArrow)
				return Tuple.Create(LValue.FromInt((int)Direction.Left), Direction.Left);
			if (k.Key == ConsoleKey.RightArrow)
				return Tuple.Create(LValue.FromInt((int)Direction.Right), Direction.Right);
			if (k.Key == ConsoleKey.UpArrow)
				return Tuple.Create(LValue.FromInt((int)Direction.Up), Direction.Up);
			if (k.Key == ConsoleKey.DownArrow)
				return Tuple.Create(LValue.FromInt((int)Direction.Down), Direction.Down);
			return Tuple.Create(currentaistate, (Direction)currentaistate.Value);
		}
	}

	public class DebuggerAbortedException : Exception
	{
		public DebuggerAbortedException(Exception exception) : base("Debugger aborted program", exception)
		{
		}
	}
}