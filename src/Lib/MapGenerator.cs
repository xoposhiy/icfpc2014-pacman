using System;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
	class MapGenerator
	{
		private static Random random = new Random();
		private static int[] dx = {0, 0, 1, -1};
		private static int[] dy = {1, -1, 0, 0};

		private static bool CheckMapWalls(MapCell[,] map)
		{
			int n = map.Length;
			int m = map.GetLength(0);
			for (int x = 0; x + 1 < n; x++)
				for (int y = 0; y + 1 < m; y++)
				{
					bool ok = false;
					for (int dx = 0; dx < 2; dx++)
						for (int dy = 0; dy < 2; dy++)
							if (map[x + dx, y + dy] == MapCell.Wall)
								ok = true;
					if (!ok)
						return false;
					if (x == 0 || x + 1 == n || y == 0 || y + 1 == m)
						if (map[x, y] != MapCell.Wall)
							return false;
				}

			return true;
		}

		private static bool TryFree(MapCell[,] map, int x, int y)
		{
			if (map[x, y] != MapCell.Wall)
				return false;

			map[x, y] = MapCell.Empty;
			if (CheckMapWalls(map))
				return true;
			map[x, y] = MapCell.Wall;
			return false;
		}

		private static void GenerateAlley(MapCell[,] map, int x, int y, int count)
		{
			var free = new List<Point>();
			if (!TryFree(map, x, y))
				return;

			free.Add(new Point(x, y));
			for (int i = 0; i < count; i++)
			{
				bool done = false;
				for (int j = 0; j < 100 && !done; j++)
				{
					var toGrow = free[random.Next()%free.Count];
					for (int d = 0; d < 4; d++)
						if (TryFree(map, toGrow.X + dx[d], toGrow.Y + dy[d]))
						{
							done = true;
							break;
						}
				}

				if (!done)
					break;
			}
		}

		///<summary>return wight*height array</summary>
		public static MapCell[,] GenerateRandomMap(int height, int wight, int ghosts)
		{
			var map = new MapCell[wight,height];
			
			for (int x = 0; x < wight; x++)
				for (int y = 0; y < height; y++)
				{
					map[x, y] = MapCell.Wall;
				}

			GenerateAlley(map, 4, 4, 10);

			return map;
		}

		[Test]
		public void TestGenerateMap()
		{
			MapCell[,] map = GenerateRandomMap(20, 20, 0);
		}
	}
}
