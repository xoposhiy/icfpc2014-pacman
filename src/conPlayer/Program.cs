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
//			var interpretedLambdaMan = new InterpretedLambdaMan(LocallyGreedyCarefulLambdaManOnList.code);

			var enterDebugger = false;
//			var interpretedLambdaMan = new InterpretedLambdaMan(LocallyGreedyCarefulLambdaManOnList.Code, runUntilStopStep: x =>
			var interpretedLambdaMan = new InterpretedLambdaMan(new LocallyCarefulLM_Lisp().Code, runUntilStopStep: x =>
			{
				if (enterDebugger)
				{
						var ex = LConsoleDebugger.Run(x.Interpreter, x.ProgramParseResult);
					if (ex != null)
						throw new DebuggerAbortedException(ex);
					enterDebugger = false;
				}
				else
					x.Interpreter.RunUntilStop();
			});

			var ghostFactory = new GhostFactory(
/*
				Ghost.ByProgram("flickle.ghc"),
				Ghost.ByProgram("flipper.ghc"),
				Ghost.ByType<ChaseGhost>(),
*/
				Ghost.ByProgram("chasing.ghc")
			);

			LMMain main = interpretedLambdaMan.Main;
			var sim = new GameSim(MapUtils.LoadFromKnownLocation("maze1.txt"), main, ghostFactory);
			var oldState = "";
			Exception exception = null;
			var runStepByStep = true;
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

					if (runStepByStep || Console.KeyAvailable)
					{
						var pressed = Console.ReadKey();
						if (pressed.Modifiers == 0 && pressed.Key == ConsoleKey.F5)
							runStepByStep = false;
						else if (pressed.Modifiers == 0 && pressed.Key == ConsoleKey.F6)
							runStepByStep = true;
						else if (pressed.Modifiers == 0 && pressed.Key == ConsoleKey.F2)
							enterDebugger = true;
					}
					//Console.ReadKey();
				}
			}
			if (exception != null)
			{
				if (!(exception is DebuggerAbortedException))
					LConsoleDebugger.Run(interpretedLambdaMan.Interpreter, interpretedLambdaMan.ProgramParseResult, exception);
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
}