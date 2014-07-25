using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Lib
{
	class MapGenerator
	{
		private readonly Random random = new Random();
		private readonly int[] dxs = {0, 0, 1, -1};
		private readonly int[] dys = {1, -1, 0, 0};
		private bool man;

		private bool CheckMapWalls(MapCell[,] map)
		{
			int n = map.GetLength(0);
			int m = map.GetLength(1);
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

		private bool TryFree(MapCell[,] map, int x, int y)
		{
			if (x <= 0 || y <= 0 || x >= map.GetLength(0) - 1 || y >= map.GetLength(1) - 1)
				return false;
			if (map[x, y] != MapCell.Wall)
				return false;

			map[x, y] = MapCell.Empty;
			if (CheckMapWalls(map))
				return true;
			map[x, y] = MapCell.Wall;
			return false;
		}

		private void GenerateAlley(MapCell[,] map, int x, int y, int count)
		{
			var free = new List<Point>();
			if (!TryFree(map, x, y))
				return;
			if (!man)
				map[x, y] = MapCell.LManStartLoc;

			free.Add(new Point(x, y));
			for (int i = 0; i < count; i++)
			{
				bool done = false;
				for (int j = 0; j < 100 && !done; j++)
				{
					var toGrow = free[random.Next()%free.Count];
					for (int k = 0; k < 4; k++)
					{
						int d = random.Next()%4;
						if (TryFree(map, toGrow.X + dxs[d], toGrow.Y + dys[d]))
						{
							done = true;
							free.Add(new Point(toGrow.X + dxs[d], toGrow.Y + dys[d]));
							break;
						}
					}
				}

				if (!done)
					break;
			}

			if (!man)
			{
				foreach (var point in free)
				{
					if (map[point.X, point.Y] == MapCell.Empty && random.Next() % 30 == 0)
						map[point.X, point.Y] = MapCell.Fruit;
					if (map[point.X, point.Y] == MapCell.Empty && random.Next()%10 == 0)
						map[point.X, point.Y] = MapCell.PowerPill;
					if (map[point.X, point.Y] == MapCell.Empty && random.Next() % 5 == 0)
						map[point.X, point.Y] = MapCell.Pill;
				}
				man = true;
			}
		}

		///<summary>return width*height array</summary>
		public MapCell[,] GenerateRandomMap(int height, int width, int ghosts)
		{
			var map = new MapCell[width,height];

			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
				{
					map[x, y] = MapCell.Wall;
				}

			GenerateAlley(map, 1 + random.Next() % (width - 2), 1 + random.Next() % (height - 2), height * 5);

			while (ghosts > 0)
			{
				int x = random.Next()%width;
				int y = random.Next()%height;
				if (map[x, y] == MapCell.Empty)
				{
					map[x, y] = MapCell.GhostStartLoc;
					ghosts--;
				}
			}

			return map;
		}

		[Test]
		public void TestGenerateMap()
		{
			MapCell[,] map = GenerateRandomMap(40, 40, 5);
			Visualizer.PrintMap(map);
		}
	}
}
