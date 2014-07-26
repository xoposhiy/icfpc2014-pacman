using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.LMachine;

namespace Lib.Game
{
	public class World
	{
		public World(MapCell[,] map)
		{
			this.map = map;
			man = new LManState(
				0,
				map.GetLocationsOf(MapCell.LManStartLoc).Single(),
				Direction.Up,
				3,
				0);
			ghosts =
				map.GetLocationsOf(MapCell.GhostStartLoc)
				.Select((p, i) => new GhostState(GhostVitality.Standard, p, Direction.Up, i % 4))
				.ToList();
			fruitTicksRemaining = 0;
		}		
		
		public World(MapCell[,] map, LManState man, List<GhostState> ghosts, int fruitTicksRemaining)
		{
			this.map = map;
			this.man = man;
			this.ghosts = ghosts;
			this.fruitTicksRemaining = fruitTicksRemaining;
		}

		public readonly MapCell[,] map;
		public readonly LManState man;
		public readonly List<GhostState> ghosts;
		public int fruitTicksRemaining;

		public LValue ToLValue()
		{
			return
				LValue.FromTuple(
					LValue.FromList(GetCells(), row => LValue.FromList(row, cell => LValue.FromInt((int)cell))),
					man.ToLValue(),
					LValue.FromList(ghosts, g => g.ToLValue()),
					LValue.FromInt(fruitTicksRemaining)
					);
		}

		private IEnumerable<IEnumerable<MapCell>> GetCells()
		{
			int h = map.GetLength(0);
			int w = map.GetLength(1);
			return 
				Enumerable.Range(0, h)
				.Select(y => Enumerable.Range(0, w).Select(x => map[y, x]));
		} 

		public override string ToString()
		{
			int h = map.GetLength(0);
			int w = map.GetLength(1);
			var sb = new StringBuilder();
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					var p = new Point(x, y);
					var mapCell = map[y, x];
					var ch = mapCell.ToChar();
					if (mapCell == MapCell.LManStartLoc || mapCell == MapCell.GhostStartLoc || mapCell == MapCell.Fruit && fruitTicksRemaining == 0)
						ch = ' ';
					var ghost = ghosts.FirstOrDefault(g => g.location.Equals(p));
					if (ghost != null)
					{
						if (ghost.vitality == GhostVitality.Standard)
							ch = '=';
						if (ghost.vitality == GhostVitality.Invisible)
							ch = '-';
						if (ghost.vitality == GhostVitality.Fright)
							ch = '~';
					}
					if (p.Equals(man.location))
						ch = '\\';
					sb.Append(ch);
				}
				sb.AppendLine();
			}
			sb.AppendFormat("Lives: {0}; Score: {1}; Fright: {2}", man.lives, man.score, man.powerPillRemainingTicks > 0);
			return sb.ToString();
		}
	}

	public enum MapCell
	{
		Wall = 0,
		Empty,
		Pill,
		PowerPill,
		Fruit,
		LManStartLoc,
		GhostStartLoc
	}



	public class GhostState
	{
		public GhostState(GhostVitality vitality, Point location, Direction direction, int ghostIndex)
		{
			this.vitality = vitality;
			this.location = location;
			initialLocation = location;
			this.direction = direction;
			this.ghostIndex = ghostIndex;
		}

		public readonly Point initialLocation;

		public GhostVitality vitality;
		public Point location;
		public Direction direction;
		public readonly int ghostIndex;

		public LValue ToLValue()
		{
			return LValue.FromTuple(
				(int)vitality,
				location,
				(int)direction
				);
		}
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
			this.powerPillRemainingTicks = powerPillRemainingTicks;
			this.location = location;
			initialLocation = location;
			this.direction = direction;
			this.lives = lives;
			this.score = score;
		}

		///<summary>Lambda man vitality</summary>
		public int powerPillRemainingTicks;

		public Point location;

		public Direction direction;

		public int lives;

		public int score;

		public int ghostsKilledInThisFrightSession;

		public readonly Point initialLocation;

		public LValue ToLValue()
		{
			return LValue.FromTuple(
				powerPillRemainingTicks,
				location,
				(int)direction,
				lives,
				score
				);
		}
	}

	public enum Direction
	{
		Up = 0,
		Right,
		Down,
		Left
	}
}