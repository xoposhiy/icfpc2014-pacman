using Lib.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib.AI
{
	public class RandomGhost : IGMachine
	{
		private readonly IGhostInterruptService interruptService;
		private static readonly Random random = new Random(314);

		public RandomGhost(IGhostInterruptService interruptService)
		{
			this.interruptService = interruptService;
		}

		public void Run()
		{
			var currState = interruptService.TryGetGhostState(interruptService.GetThisGhostIndex());
			var curr = currState.location;

			Func<Point, MapCell> getCell = p => interruptService.GetMapState((byte)p.X, (byte)p.Y);
			Func<Point, bool> isCorrect = p => getCell(p) != MapCell.Wall;
			Func<Point, Point, Point> sum = (p1, p2) => new Point(p1.X + p2.X, p1.Y + p2.Y);
			Func<Point, Point, bool> isReverseDirection = (p, p1) => sum(p, p1).Equals(new Point(0, 0));
			var dirs = new List<Point>{ new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) };

			var possibleDirs = dirs.Select((d, i) => i).Where(i => !isReverseDirection(dirs[i], dirs[(int)currState.direction]) && isCorrect(sum(curr, dirs[i]))).ToArray();
			if (possibleDirs.Length > 0)
			{
				var rnd = random.Next(possibleDirs.Length);
				interruptService.SetNewDirectionForThisGhost((Direction)possibleDirs[rnd]);
			}
		}

		public int GhostIndex
		{
			get { return interruptService.GetThisGhostIndex(); }
		}
	}
}
