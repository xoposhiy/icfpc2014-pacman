﻿using System;
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

		public static Tuple<LValue, Direction> LambdaMenGreedyStep(LValue currentAIState, World currentWorldState)
		{
			var map = currentWorldState.map;
			var lmPosition = currentWorldState.man.location;
			var stackInit = directions
				.Select(d => LValue.FromPair(lmPosition.Add(SizeByDirection[d]), (int)d))
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
			while (!queue.IsEmpty())
			{
				var p = queue.Dequeue();
				var point = p.Pair.Head;
				if (map.Get(point) == MapCell.Pill ||
				    map.Get(point) == MapCell.PowerPill) // TODO: fruits!
				{
					founded = (Direction) (p.Pair.Tail.Value .Value);
					break;
				}
				foreach (var newPoint in GetNeighbours(point, currentWorldState, visited))
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

		public static IEnumerable<Point> GetNeighbours(LValue lValuePoint, World world, bool[,] visited)
		{
			return directions
				.Select(d => SizeByDirection[d])
				.Select(p => new Point(p.X + lValuePoint.Pair.Head.Value.Value, p.Y + lValuePoint.Pair.Tail.Value.Value))
				.Where(p => !visited.Get(p) && (world.map.Get(p) != MapCell.Wall) && world.ghosts.All(g => !p.Equals(g.location)));
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