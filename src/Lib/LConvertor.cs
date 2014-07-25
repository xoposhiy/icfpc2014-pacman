using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Lib
{
	public class LConvertor_Tests
	{
		[Test]
		public void ParseWorld()
		{
			string text = "(((0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, 0))))))))))))))))))))))), ((0, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (0, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (0, 0))))))))))))))))))))))), ((0, (2, (0, (0, (0, (2, (0, (0, (0, (0, (2, (0, (2, (0, (0, (0, (0, (2, (0, (0, (0, (2, (0, 0))))))))))))))))))))))), ((0, (3, (0, (0, (0, (2, (0, (0, (0, (0, (2, (0, (2, (0, (0, (0, (0, (2, (0, (0, (0, (3, (0, 0))))))))))))))))))))))), ((0, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (0, 0))))))))))))))))))))))), ((0, (2, (0, (0, (0, (2, (0, (2, (0, (0, (0, (0, (0, (0, (0, (2, (0, (2, (0, (0, (0, (2, (0, 0))))))))))))))))))))))), ((0, (2, (2, (2, (2, (2, (0, (2, (2, (2, (2, (0, (2, (2, (2, (2, (0, (2, (2, (2, (2, (2, (0, 0))))))))))))))))))))))), ((0, (0, (0, (0, (0, (2, (0, (0, (0, (0, (1, (0, (1, (0, (0, (0, (0, (2, (0, (0, (0, (0, (0, 0))))))))))))))))))))))), ((0, (1, (1, (1, (0, (2, (0, (1, (1, (1, (1, (6, (1, (1, (1, (1, (0, (2, (0, (1, (1, (1, (0, 0))))))))))))))))))))))), ((0, (0, (0, (0, (0, (2, (0, (1, (0, (0, (0, (1, (0, (0, (0, (1, (0, (2, (0, (0, (0, (0, (0, 0))))))))))))))))))))))), ((0, (1, (1, (1, (1, (2, (1, (1, (0, (1, (6, (6, (6, (1, (0, (1, (1, (2, (1, (1, (1, (1, (0, 0))))))))))))))))))))))), ((0, (0, (0, (0, (0, (2, (0, (1, (0, (0, (0, (0, (0, (0, (0, (1, (0, (2, (0, (0, (0, (0, (0, 0))))))))))))))))))))))), ((0, (1, (1, (1, (0, (2, (0, (1, (1, (1, (1, (4, (1, (1, (1, (1, (0, (2, (0, (1, (1, (1, (0, 0))))))))))))))))))))))), ((0, (0, (0, (0, (0, (2, (0, (1, (0, (0, (0, (0, (0, (0, (0, (1, (0, (2, (0, (0, (0, (0, (0, 0))))))))))))))))))))))), ((0, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (0, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (0, 0))))))))))))))))))))))), ((0, (2, (0, (0, (0, (2, (0, (0, (0, (0, (2, (0, (2, (0, (0, (0, (0, (2, (0, (0, (0, (2, (0, 0))))))))))))))))))))))), ((0, (3, (2, (2, (0, (2, (2, (2, (2, (2, (2, (5, (1, (2, (2, (2, (2, (2, (0, (2, (2, (3, (0, 0))))))))))))))))))))))), ((0, (0, (0, (2, (0, (2, (0, (2, (0, (0, (0, (0, (0, (0, (0, (2, (0, (2, (0, (2, (0, (0, (0, 0))))))))))))))))))))))), ((0, (2, (2, (2, (2, (2, (0, (2, (2, (2, (2, (0, (2, (2, (2, (2, (0, (2, (2, (2, (2, (2, (0, 0))))))))))))))))))))))), ((0, (2, (0, (0, (0, (0, (0, (0, (0, (0, (2, (0, (2, (0, (0, (0, (0, (0, (0, (0, (0, (2, (0, 0))))))))))))))))))))))), ((0, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (2, (0, 0))))))))))))))))))))))), ((0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, 0))))))))))))))))))))))), 0)))))))))))))))))))))), ((0, ((12, 16), (1, (3, 10)))), (((0, ((10, 8), 3)), ((0, ((9, 10), 3)), ((0, ((10, 10), 3)), ((0, ((11, 10), 3)), 0)))), 0)))";
			var value = LValue.Parse(text);
			var parserWorld = new LConvertor().Parse<World>(value);
			Assert.AreEqual(10, parserWorld.LMan.Score);
			Assert.AreEqual(3, parserWorld.LMan.Lives);
			Assert.AreEqual(4, parserWorld.Ghosts.Count);
		}
	}

	public class LConvertor
	{

		private static readonly IDictionary<Type, Func<LConvertor, LValue, object>> conv 
			= new Dictionary<Type, Func<LConvertor, LValue, object>>();

		static LConvertor()
		{
			Register(ReadWorld);
			Register(ReadMap);
			Register(ReadLManState);
			Register(ReadPoint);
			Register(ReadGhostState);
		}

		private static GhostState ReadGhostState(LConvertor c, LValue ghost)
		{
			var g = ghost.AsTuple(3);
			return new GhostState(
				(GhostVitality)g[0].Value,
				c.Parse<Point>(g[1]),
				(Direction)g[2].Value
				);
		}

		private static Point ReadPoint(LConvertor c, LValue point)
		{
			var p = point.AsTuple(2);
			return new Point(p[0].Value, p[1].Value);
		}

		private static LManState ReadLManState(LConvertor c, LValue lman)
		{
			var m = lman.AsTuple(5);
			return new LManState(
				m[0].Value, 
				c.Parse<Point>(m[1]),
				(Direction)m[2].Value,
				m[3].Value,
				m[4].Value
				);
		}

		private static World ReadWorld(LConvertor c, LValue world)
		{
			var w = world.AsTuple(4);
			return new World(
				ToArray(c.Parse<List<List<MapCell>>>(w[0])),
				c.Parse<LManState>(w[1]),
				w[2].AsList(c.Parse<GhostState>),
				w[3].Value
				);
		}

		private static T[,] ToArray<T>(List<List<T>> listOfLists)
		{
			int h = listOfLists.Count;
			int w = listOfLists[0].Count;
			var res = new T[h, w];
			for(int y=0; y<h;y++)
				for (int x = 0; x < w; x++)
					res[y, x] = listOfLists[y][x];
			return res;
		}

		private static List<List<MapCell>> ReadMap(LConvertor c, LValue map)
		{
			return map.AsList(v => v.AsList(cell => (MapCell)cell.Value));
		}

		static void Register<T>(Func<LConvertor, LValue, T> converter) where T : class
		{
			conv.Add(typeof(T), converter);
		}

		public T Parse<T>(LValue value)
		{
			return (T)conv[typeof (T)](this, value);
		}
	}
}