using System;

namespace Lib.Game
{
	class Visualizer
	{
		public static void PrintMap(MapCell[,] map)
		{
			int n = map.GetLength(0);
			int m = map.GetLength(1);
			for (int x = 0; x < n; x++)
			{
				for (int y = 0; y < m; y++)
					Console.Out.Write(map[x, y].ToChar());
				Console.Out.Write("\n");
			}
		}
	}
}
