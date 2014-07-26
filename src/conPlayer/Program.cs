﻿using System;
using System.IO;
using Lib;
using Lib.AI;
using Lib.Game;
using Lib.LMachine;

namespace conPlayer
{
	class Program
	{
		static void Main(string[] args)
		{
//			LMMain main = w => Tuple.Create(LValue.FromInt(42), (LMStep)ConsoleStep);
			LMMain main = w => Tuple.Create(LValue.FromInt(42), (LMStep)GreedyLambdaMen.LambdaMenGreedyStep);
//			LMMain main = new LocallyGreedyCarefulLambdaMan().Main;
//			LMMain main = new InterpretedLambdaMan(File.ReadAllText(KnownPlace.GccSamples + "stupidBot.mgcc")).Main;
			GMain[] ghostProgs = new GMain[] { new RandomGhost().Main };
			
			var sim = new GameSim(MapUtils.LoadFromKnownLocation("small.txt"), main, ghostProgs);
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
