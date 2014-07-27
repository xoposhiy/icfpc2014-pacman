using System;
using Lib.AI;
using Lib.Debugger;
using Lib.Game;
using Lib.GMachine;
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

			var enterLDebugger = false;
						var interpretedLambdaMan = new InterpretedLambdaMan(new LocallyGreedyCarefulLM_Lisp().Code, runUntilStopStep: x =>
			//var interpretedLambdaMan = new InterpretedLambdaMan(CarefulGreedyLambdaMan_Lisp.code, runUntilStopStep: x =>
			{
				if (enterLDebugger)
				{
						var ex = LConsoleDebugger.Run(x.Interpreter, x.ProgramParseResult);
					if (ex != null)
						throw new DebuggerAbortedException(ex);
					enterLDebugger = false;
				}
				else
					x.Interpreter.RunUntilStop();
			});

			var enterGDebugger = false;
			var ghostFactory = new GhostFactory(
				x =>
				{
					if (enterGDebugger)
					{
						var ex = GConsoleDebugger.Run(x);
						if (ex != null)
							throw new DebuggerAbortedException(ex);
						enterGDebugger = false;
					}
					else
						x.RunToEnd();
				},
				Ghost.ByProgram("chasing.mghc")
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
				catch (GException e)
				{
					exception = e;
					break;
				}
				catch (LException e)
				{
					exception = e;
					break;
				}
				catch (DebuggerAbortedException e)
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
							enterLDebugger = true;
						else if (pressed.Modifiers == 0 && pressed.Key == ConsoleKey.F3)
							enterGDebugger = true;
					}
					//Console.ReadKey();
				}
			}
			if (exception != null)
			{
				if (exception is GException)
					GConsoleDebugger.Run(((GException)exception).Machine, exception);
				if (exception is LException)
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