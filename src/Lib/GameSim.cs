using System;
using System.Collections.Generic;
using System.Drawing;

namespace Lib
{
    class GameSim
	{
		public MapCell[,] Map { get; set; }
		public LMStep Step { get; set; } //
		public LValue InitialState { get; set; }

		public GameSim(MapCell[,] map, LMStep step, LValue initialState)
		{
			Map = map;
			Step = step;
			InitialState = initialState;
		}

		public void Tick()
		{
			//ToDo
		}
	}

	public class World
	{
		public World(MapCell[,] map, LManState lMan, List<GhostState> ghosts, int fruitTicksRemaining)
		{
			Map = map;
			LMan = lMan;
			Ghosts = ghosts;
			FruitTicksRemaining = fruitTicksRemaining;
		}

		public readonly MapCell[,] Map;
		public readonly LManState LMan;
		public readonly List<GhostState> Ghosts;
		public readonly int FruitTicksRemaining;
	}
	
	public enum MapCell
	{
		Wall = '#',
		Empty = ' ',
		Pill = '.',
		PowerPill = 'o',
		Fruit = '%',
		LManStartLoc = '\\',
		GhostStartLoc = '=',
	}



	public class GhostState
	{
		public GhostState(GhostVitality vitality, Point location, Direction direction)
		{
			Vitality = vitality;
			Location = location;
			Direction = direction;
		}

		public readonly GhostVitality Vitality;
		public readonly Point Location;
		public readonly Direction Direction;
	}

	public enum GhostVitality
	{
		Standard = 0,
		Fright,
		Invisible
	}

	public class LManState
	{
		public LManState(int powerPillRemainingTicks, Point location, Direction direction, int lives, int score)
		{
			PowerPillRemainingTicks = powerPillRemainingTicks;
			Location = location;
			Direction = direction;
			Lives = lives;
			Score = score;
		}

		///<summary>Lambda man vitality</summary>
		public readonly int PowerPillRemainingTicks;

		public readonly Point Location;

		public readonly Direction Direction;

		public readonly int Lives;

		public readonly int Score;
	}

	public enum Direction
	{
		Up = 0,
		Right,
		Down,
		Left
	}
}
