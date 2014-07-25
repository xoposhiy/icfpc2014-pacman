using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib
{
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

		public override string ToString()
		{
			int h = Map.GetLength(0);
			int w = Map.GetLength(1);
			var sb = new StringBuilder();
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					var p = new Point(x, y);
					var mapCell = Map[y, x];
					var ch = MapUtils.CharFromMapCell(mapCell);
					if (mapCell == MapCell.LManStartLoc || mapCell == MapCell.GhostStartLoc || mapCell == MapCell.Fruit && FruitTicksRemaining == 0)
						ch = ' ';
					if (p.Equals(LMan.Location))
						ch = '\\';
					if (Ghosts.Any(g => g.Location.Equals(p)))
						ch = '=';
					sb.Append(ch);
				}
				sb.AppendLine();
			}
			sb.AppendFormat("Lives: {0}; Score: {1}", LMan.Lives, LMan.Score);
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
		public GhostState(GhostVitality vitality, Point location, Direction direction)
		{
			Vitality = vitality;
			Location = location;
			Direction = direction;
		}

		public readonly GhostVitality Vitality;
		public Point Location;
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

		public Point Location;

		public Direction Direction;

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