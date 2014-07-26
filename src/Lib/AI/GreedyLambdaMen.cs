using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Lib.Game;
using Lib.LMachine;
using NUnit.Framework;

namespace Lib.AI
{
	public class GreedyLambdaMen : LambdaMan
	{
		public Tuple<LValue, LMStep> Main(World initialWorld)
		{
			return Tuple.Create(
				LValue.FromInt(-1), // заглушка
				(LMStep)LambdaMenGreedyStep
				);
		}

		public static Tuple<LValue, Direction> LambdaMenGreedyStep(LValue currentAIState, World world)
		{
			var map = world.map;
			var lmPosition = world.man.location;
			var standardGhost = world.ghosts.Where(g => g.vitality == GhostVitality.Standard).ToArray();
			var stackInit = directions
				.Select(d => LValue.FromPair(lmPosition.Add(SizeByDirection[d]), (int)d))
				.Where(dp => map.Get(dp.Pair.Head) != MapCell.Wall)
				.Where(pts =>
				{
					var point = pts.Pair.Head;
					var position = new Point(point.Pair.Head.Value.Value, point.Pair.Tail.Value.Value);
					return standardGhost.All(g => !g.location.Equals(position) && GetFourNeighbours(g.location).All(gn => !position.Equals(gn)));
				})
				.ToArray();
			var queue = new Queue_Functional();

			var visited = new bool[map.GetLength(0), map.GetLength(1)];
			visited.Set(lmPosition, true);
			foreach (var tuple in stackInit)
			{
				queue.Enqueue(tuple);
				visited.Set(tuple.Pair.Head, true);
			}

			var founded = world.man.direction;
			while (!queue.IsEmpty())
			{
				var p = queue.Dequeue();
				var point = p.Pair.Head;
				if (map.Get(point) == MapCell.Pill ||
				    map.Get(point) == MapCell.PowerPill ||
					(map.Get(point) == MapCell.Fruit && world.fruitTicksRemaining > 126) ||
					(world.ghosts.Any(g => g.vitality == GhostVitality.Fright && g.location.Equals(new Point(point.Pair.Head.Value.Value, point.Pair.Tail.Value.Value)))))
				{
					founded = (Direction) (p.Pair.Tail.Value .Value);
					break;
				}
				foreach (var newPoint in GetNeighbours(point, world, visited))
				{
					queue.Enqueue(LValue.FromPair(newPoint, p.Pair.Tail));
					visited.Set(newPoint, true);
				}
			}
			return Tuple.Create(currentAIState, founded);
		}

		private readonly static List<Direction> directions = new List<Direction> { Direction.Up, Direction.Left, Direction.Right, Direction.Down };

		public static Dictionary<Direction, Point> SizeByDirection =
			new Dictionary<Direction, Point>
			{
				{Direction.Up, new Point(0, -1)},
				{Direction.Down, new Point(0, 1)},
				{Direction.Left, new Point(-1, 0)},
				{Direction.Right, new Point(1, 0)}
			};

		public static IEnumerable<Point> GetNeighbours(LValue point, World world, bool[,] visited)
		{
			return GetFourNeighbours(point)
				.Where(p =>
					!visited.Get(p) &&
					(world.map.Get(p) != MapCell.Wall));
		}

		private static IEnumerable<Point> GetFourNeighbours(LValue point)
		{
			return GetFourNeighbours(new Point(point.Pair.Head.Value.Value, point.Pair.Tail.Value.Value));
		}

		private static IEnumerable<Point> GetFourNeighbours(Point point)
		{
			return directions
				.Select(d => SizeByDirection[d])
				.Select(p => new Point(p.X + point.X, p.Y + point.Y));
		}
	}

	public static class Extensions
	{
		public static T Get<T>(this T[,] map, LValue point)
		{
			CheckPoint(point);
			return map[point.Pair.Tail.Value.Value, point.Pair.Head.Value.Value];
		}

		public static T Get<T>(this T[,] map, Point point)
		{
			return map[point.Y, point.X];
		}

		public static void Set<T>(this T[,] map, LValue point, T value)
		{
			CheckPoint(point);
			map[point.Pair.Tail.Value.Value, point.Pair.Head.Value.Value] = value;
		}

		public static void Set<T>(this T[,] map, Point point, T value)
		{
			map[point.Y, point.X] = value;
		}

		private static void CheckPoint(LValue point)
		{
			if (point.Tag != LTag.Pair || point.Pair.Head.Tag != LTag.Int || point.Pair.Tail.Tag != LTag.Int || point.Pair.Head.Value == null || point.Pair.Tail.Value == null)
				throw new Exception("Bad point in ");
		}

		public static Point Add(this Point p, Point s)
		{
			return new Point(p.X + s.X, p.Y + s.Y);
		}
	}
}