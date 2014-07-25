using System;
using System.IO;
using System.Linq;

namespace Lib
{
	public class MapUtils
	{
		public static MapCell[,] LoadFromKnownLocation(string filename)
		{
			return Load(File.ReadAllText(@"..\..\..\..\mazes\" + filename));
		}

		public static MapCell[,] Load(string text)
		{
			var lines = text.Split('\n').Select(line => line.Trim()).ToList();
			var h = lines.Count;
			var w = lines[0].Length;
			var res = new MapCell[h, w];
			for (int y = 0; y < h; y++)
				for (int x = 0; x < w; x++)
					res[y, x] = MapCellFromChar(lines[y][x]);
			return res;
		}

		public static MapCell MapCellFromChar(char c)
		{
			if (c == ' ') return MapCell.Empty;
			if (c == '#') return MapCell.Wall;
			if (c == '.') return MapCell.Pill;
			if (c == 'o') return MapCell.PowerPill;
			if (c == '%') return MapCell.Fruit;
			if (c == '\\') return MapCell.LManStartLoc;
			if (c == '=') return MapCell.GhostStartLoc;
			throw new Exception("Unknown MapCell " + c);
		}

		public static char CharFromMapCell(MapCell c)
		{
			if (c == MapCell.Empty) return ' ';
			if (c == MapCell.Wall) return '#';
			if (c == MapCell.Pill) return '.';
			if (c == MapCell.PowerPill) return 'o';
			if (c == MapCell.Fruit) return '%';
			if (c == MapCell.LManStartLoc) return '\\';
			if (c == MapCell.GhostStartLoc) return '=';
			throw new Exception("Unknown MapCell " + c);
		}
	}
}