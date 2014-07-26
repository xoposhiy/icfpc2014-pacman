using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Game;
using Lib.LMachine;
using NUnit.Framework;

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
			var stackInit = Directions
				.Select(d => Tuple.Create(lmPosition.Add(SizeByDirection[d]), d))
				.Where(dp => map.Get(dp.Item1) != MapCell.Wall);
			var visited = new bool[map.GetLength(0), map.GetLength(1)];
			visited.Set(lmPosition, true);

			var queue = new Queue<Tuple<Point, Direction>>(stackInit);
			var founded = currentWorldState.man.direction;
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
				{
					queue.Enqueue(Tuple.Create(newPoint, p.Item2));
				}
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

	public class Queue_Functional
	{
		private LValue twoStacks;

		public Queue_Functional()
		{
			twoStacks = LValue.FromPair(null, null);
		}

		public void Enqueue(int value)
		{
			twoStacks = LValue.FromPair(
				LValue.FromPair(LValue.FromInt(value), twoStacks.Pair.Head),
				twoStacks.Pair.Tail);
		}

		public int? Dequeue()
		{
			if (twoStacks.Pair.Tail.Tag != LTag.Pair)
				throwLeftStackToRight();
			if (twoStacks.Pair.Tail.Tag != LTag.Pair)
				return null;
			var result = twoStacks.Pair.Tail .Pair.Head;
			twoStacks = LValue.FromPair(
				twoStacks.Pair.Head,
				twoStacks.Pair.Tail .Pair.Head);
			return result.Value;
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
			twoStacks = LValue.FromPair(null, rightStack);
		}
	}


	public class TestQueueFunctional
	{
		private Queue_Functional func;
		private Queue<int> coll;
			
		[Test]
		public void TestQueue()
		{
			func = new Queue_Functional();
			coll = new Queue<int>();

			Push(1);
			Push(2);
			Push(3);
			Pop();
			Pop();
			Pop();
			Pop();
			Push(4);
			Push(5);
			Push(6);
			Pop();
			Pop();
			Push(7);
			Pop();
			Pop();
			Pop();
		}

		private void Push(int val)
		{
			func.Enqueue(val);
			coll.Enqueue(val);
		}

		private void Pop()
		{
			Assert.That(coll.Count == 0, Is.EqualTo(func.IsEmpty()));
			if (coll.Count > 0)
			{
				int val1 = coll.Dequeue();
				int? val2 = func.Dequeue();
				Assert.That(val2.HasValue);
				Assert.That(val1, Is.EqualTo(val2.Value));
			}
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