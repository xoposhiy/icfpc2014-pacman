using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Lib.Game;
using Lib.LMachine;

namespace Lib.AI
{
	public class GreedyLambdaMen : ILambdaMan
	{
		public Tuple<LValue, LMStep> Main(World initialWorld)
		{
			return Tuple.Create(
				LValue.FromInt(-1), // заглушка
				(LMStep)LambdaMenGreedyStep
				);
		}

		public static Tuple<LValue, Direction> LambdaMenGreedyStep(LValue currentAIState, World currentWorldState)
		{
			var map = currentWorldState.map;
			var lmPosition = currentWorldState.man.location;
			var stackInit = directions
				.Select(d => LValue.FromPair(lmPosition.Add(SizeByDirection[d]), d))
				.Where(dp => map.Get(dp.Pair.Head) != MapCell.Wall)
				.ToArray();
			var queue = new Queue_Functional();

			var visited = new bool[map.GetLength(0), map.GetLength(1)];
			visited.Set(lmPosition, true);
			foreach (var tuple in stackInit)
			{
				queue.Enqueue(tuple);
				visited.Set(tuple.Pair.Head, true);
			}

			var founded = currentWorldState.man.direction;
			while (queue.IsEmpty())
			{
				var p = queue.Dequeue();
				var point = p.Pair.Head;
				if (map.Get(point) == MapCell.Pill ||
				    map.Get(point) == MapCell.PowerPill) // TODO: fruits!
				{
					founded = (Direction) (p.Pair.Tail.Value .Value);
					break;
				}
				foreach (var newPoint in GetNeighbours(point, map, visited))
				{
					queue.Enqueue(LValue.FromPair(newPoint, p.Pair.Tail));
					visited.Set(newPoint, true);
				}
			}
			return Tuple.Create(currentAIState, founded);
		}

		private const int Up = 0;
		private const int Left = 1;
		private const int Right = 2;
		private const int Down = 3;

		private readonly static List<int> directions = new List<int> { Up, Left, Right, Down };
//		private readonly static LValue directions = LValue.FromList(new List<int> { Up, Left, Right, Down }, i => i);

		public static Dictionary<int, Point> SizeByDirection =
			new Dictionary<int, Point>
			{
				{Up, new Point(0, -1)},
				{Down, new Point(0, 1)},
				{Left, new Point(-1, 0)},
				{Right, new Point(1, 0)}
			};

		public static IEnumerable<Point> GetNeighbours(LValue lValuePoint, MapCell[,] map, bool[,] visited)
		{
			return directions
				.Select(d => SizeByDirection[d])
				.Select(p => new Point(p.X + lValuePoint.Pair.Head.Value .Value, p.Y + lValuePoint.Pair.Tail.Value .Value))
				.Where(p => !visited.Get(p) && (map.Get(p) != MapCell.Wall));
		}
	}

	public class Queue_Functional
	{
		private LValue twoStacks;
		private static readonly LValue nil = LValue.FromInt(846534165);

		public Queue_Functional()
		{
			twoStacks = LValue.FromPair(nil, nil);
		}

		public bool IsEmpty()
		{
			return twoStacks.Pair.Tail.Tag == LTag.Pair
				|| twoStacks.Pair.Head.Tag == LTag.Pair;
		}

		public void Enqueue(LValue value)
		{
			twoStacks = LValue.FromPair(
				LValue.FromPair(value, twoStacks.Pair.Head),
				twoStacks.Pair.Tail);
		}

		public LValue Dequeue()
		{
			if (twoStacks.Pair.Tail.Tag != LTag.Pair)
				throwLeftStackToRight();
			if (twoStacks.Pair.Tail.Tag != LTag.Pair)
				return null;
			var result = twoStacks.Pair.Tail .Pair.Head;
			twoStacks = LValue.FromPair(
				twoStacks.Pair.Head,
				twoStacks.Pair.Tail .Pair.Head);
			return result;
		}

		private void throwLeftStackToRight()
		{
			LValue rightStack = null;
			while (twoStacks.Pair.Head.Tag == LTag.Pair)
			{
				var curr = twoStacks.Pair.Head.Pair.Head;
				rightStack = LValue.FromPair(curr, rightStack);
				twoStacks = LValue.FromPair(
					twoStacks.Pair.Head.Pair.Tail,
					twoStacks.Pair.Tail);
			}
			twoStacks = LValue.FromPair(nil, rightStack);
		}
	}

	public static class Extensions
	{
		public static T Get<T>(this T[,] map, LValue point)
		{
			CheckPoint(point);
			return map[point.Pair.Head.Value .Value, point.Pair.Tail.Value .Value];
		}

		public static T Get<T>(this T[,] map, Point point)
		{
			return map[point.Y, point.X];
		}

		public static void Set<T>(this T[,] map, LValue point, T value)
		{
			CheckPoint(point);
			map[point.Pair.Head.Value.Value, point.Pair.Tail.Value.Value] = value;
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