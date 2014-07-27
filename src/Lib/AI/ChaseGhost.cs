using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Game;

namespace Lib.AI
{
	public class ChaseGhost : IGMachine
	{
		private readonly IGhostInterruptService interruptService;

		public ChaseGhost(IGhostInterruptService interruptService)
		{
			this.interruptService = interruptService;
		}

		public void Run()
		{
			var currState = interruptService.TryGetGhostState(interruptService.GetThisGhostIndex());
			var curr = currState.location;

			Func<Point, MapCell> getCell = p => interruptService.GetMapState((byte)p.X, (byte)p.Y);
			Func<Point, Point, int> dist = (a,b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
			Func<Point, bool> isCorrect = p => getCell(p) != MapCell.Wall;
			Func<Point, Point, Point> sum = (p1, p2) => new Point(p1.X + p2.X, p1.Y + p2.Y);
			Func<Point, Point, bool> isReverseDirection = (p, p1) => sum(p, p1).Equals(new Point(0, 0));
			var dirs = new List<Point>() { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) };

			int[] possibleDirs = dirs.Select((d, i) => i).Where(i => !isReverseDirection(dirs[i], dirs[(int)currState.direction]) && isCorrect(sum(curr, dirs[i]))).ToArray();
			if (possibleDirs.Length > 0)
			{
				var lm = interruptService.GetLamdbaManCurrentLocation();
				var bestDir = possibleDirs.Cast<Direction>()
					.Select(d => Tuple.Create(dist(lm, curr.MoveTo(d)), d))
					.Min().Item2;
				interruptService.SetNewDirectionForThisGhost(bestDir);
			}
		}
	}
}