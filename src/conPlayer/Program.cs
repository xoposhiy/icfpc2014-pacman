using System;
using System.IO;
using Lib;
using Lib.AI;
using Lib.Game;
using Lib.GMachine;
using Lib.LispLang;
using Lib.LMachine;
using Lib.Parsing.GParsing;

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
			Console.WriteLine(LeftAi.code);
			LMMain main = new InterpretedLambdaMan(LeftAi.code).Main;
			//var randomGhostFactory = new RandomGhostFactory();
			var p = File.ReadAllText(KnownPlace.GccSamples + "sample.mghc");
			var ghostFactory = new GMachineFactory(GParser.Parse(p).Program);

			var sim = new GameSim(MapUtils.LoadFromKnownLocation("maze1.txt"), main, ghostFactory);
			var oldState = "";
			while (!sim.finished)
			{
				sim.Tick();
				var newState = sim.world.ToString();
				if (newState != oldState)
				{
					Console.Clear();
					Console.WriteLine(newState);
					Console.WriteLine("Use Cursor keys to control Lambda Man. Time: {0}", sim.time);
					oldState = newState;
					Console.ReadKey();
				}
			}
			Console.WriteLine("Game over");
			Console.WriteLine("Score " + sim.world.man.score);
			Console.WriteLine("Press any key to continue...");
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