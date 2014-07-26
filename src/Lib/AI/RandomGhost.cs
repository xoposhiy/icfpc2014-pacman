using Lib.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.AI
{
	public class RandomGhost : Ghost
	{
		public GStep Main(int ghostId, World initialWorld)
		{
			return new GStep((World) => Step(ghostId, World));
		}

		public Direction Step(int ghostId, World currentWorldState)
		{
			var currState = currentWorldState.ghosts[ghostId];
			var curr = currState.location;
			

			var map = currentWorldState.map;
			Func<Point, MapCell> getCell = (p) => map[p.Y, p.X];
			Func<Point, bool> isCorrect = (p) => !(p.X < 0 || p.X >= map.GetLength(1) || p.Y < 0 || p.Y >= map.GetLength(0) || getCell(p) == MapCell.Wall);
			Func<Point, Point, Point> sum = (p1, p2) => new Point(p1.X + p2.X, p1.Y + p2.Y);
			Func<Point, Point, bool> isReverseDirection = (p, p1) => sum(p, p1).Equals(new Point(0, 0));
			var dirs = new List<Point>() { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) };

			var possibleDirs = dirs.Select((d, i) => i).Where(i => !isReverseDirection(dirs[i], dirs[(int)currState.direction]) && isCorrect(sum(curr, dirs[i]))).ToArray();
			if (possibleDirs.Length > 0)
			{
				var rnd = new Random().Next(possibleDirs.Length);
				return (Direction)possibleDirs[rnd];
			}
			else
				return currState.direction;
		}
	}
}
