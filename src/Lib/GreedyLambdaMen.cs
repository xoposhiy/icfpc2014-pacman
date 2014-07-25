using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib
{
	public class GreedyLambdaMen
	{
		public static Tuple<LValue, Direction> LambdaMenGreedyStep(LValue currentAIState, World currentWorldState)
		{
			var map = currentWorldState.Map;
			var lmPosition = currentWorldState.LMan.Location;
			var stackInit = Directions
				.Select(d => Tuple.Create<Point, Direction>(lmPosition.Add(SizeByDirection[d]), d))
				.Where(dp => map.Get(dp.Item1) != MapCell.Wall);
			var visited = new bool[map.GetLength(0), map.GetLength(1)];
			visited.Set(lmPosition, true);

			var queue = new Queue<Tuple<Point, Direction>>(stackInit);
			var founded = currentWorldState.LMan.Direction;
			while (queue.Count > 0)
			{
				var p = queue.Dequeue();
				var point = p.Item1;
				visited.Set(point, true);
				if (map.Get(point) == MapCell.Pill ||
				    map.Get(point) == MapCell.PowerPill) // TODO: fruits!
				{
					founded = p.Item2;
					break;
				}
				foreach (var newPoint in GetNeighbours(point, map, visited))
					queue.Enqueue(Tuple.Create(newPoint, p.Item2));
			}
			return Tuple.Create(currentAIState, founded);
		}

		public static readonly Direction[] Directions = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

		public static Dictionary<Direction, Point> SizeByDirection =
			new Dictionary<Direction, Point>
			{
				{Direction.Up, new Point(0, -1)},
				{Direction.Down, new Point(0, 1)},
				{Direction.Left, new Point(-1, 0)},
				{Direction.Right, new Point(1, 0)}
			};

		public static IEnumerable<Point> GetNeighbours(Point point, MapCell[,] map, bool[,] visited)
		{
			return Directions
				.Select(d => point.Add(SizeByDirection[d]))
				.Where(p => !visited.Get(p) && (map.Get(p) != MapCell.Wall));
		}
	}

	public static class Extensions
	{
		public static T Get<T>(this T[,] map, Point point)
		{
			return map[point.Y, point.X];
		}

		public static void Set<T>(this T[,] map, Point point, T value)
		{
			map[point.Y, point.X] = value;
		}

		public static Point Add(this Point p, Point s)
		{
			return new Point(p.X + s.X, p.Y + s.Y);
		}
	}
}