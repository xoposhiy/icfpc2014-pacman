using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Game;
using Lib.LMachine;

namespace Lib.AI
{
	/// <summary>
	///     Очень простая реализация - на перекрестках идем преимущественно туда, где мы еще не были (где есть пилюли)
	///     Если выбор не очевиден - идем в случайную сторону.
	/// </summary>
	public class LocallyGreedyCarefulLambdaMan : LambdaMan
	{
		public Tuple<LValue, LMStep> Main(World initialWorldState)
		{
			var initState = LValue.FromPair(LValue.FromInt(-1), LValue.FromInt(-1));
			return new Tuple<LValue, LMStep>(initState, Step);
		}

		public Tuple<LValue, Direction> Step(LValue currentAiState, World currentWorldState)
		{
			// currentAIState содержит наши координаты на прошлом шаге

			var curr = currentWorldState.man.location;
			var currentPair = currentAiState.GetPair();
			var prev = new Point(currentPair.Head.GetValue(), currentPair.Tail.GetValue());

			var dirWeight = new List<int>() { 0, 0, 0, 0 };
			var dir = new List<Point>() { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) };
			Func<Point, Point, Point> sum = (p1, p2) => new Point(p1.X + p2.X, p1.Y + p2.Y);

			var map = currentWorldState.map;
			Func<Point, MapCell> getCell = (p) => map[p.Y, p.X];
			Func<Point, bool> isCorrect = (p) => !(p.X < 0 || p.X >= map.GetLength(1) || p.Y < 0 || p.Y >= map.GetLength(0) || getCell(p) == MapCell.Wall);

			Func<Point, Point, int, int> calculatePathScore = null;
			calculatePathScore = (currLoc, prevLoc, maxdepth) =>
				{
					int maxscore = int.MinValue;
					for (var nd = 0; nd < 4; ++nd)
					{
						var score = 0;
						var next = sum(currLoc, dir[nd]);
						if (!isCorrect(next))
							continue;
						var nextCell = getCell(next);
						if (currentWorldState.ghosts.Any(gost => gost.vitality == GhostVitality.Standard && gost.location.Equals(next)))
							score -= 100;

						if (next.Equals(prevLoc))
							score -= maxdepth + 1;

						if (nextCell == MapCell.Pill)
							score++;
						else if (nextCell == MapCell.PowerPill)
							score += 5;
						else if (nextCell == MapCell.Fruit && currentWorldState.fruitTicksRemaining > 0)
							score += 3;

						if (currentWorldState.ghosts.Any(gost => gost.vitality == GhostVitality.Fright && gost.location.Equals(next)))
							score ++;

						score = score * (maxdepth + 1) + (maxdepth > 0 ? calculatePathScore(next, currLoc, maxdepth - 1) : 0);
						if (score > maxscore)
							maxscore = score;
					}
					return maxscore;


				};
			int maxDepth = 8;
			for (var d = 0; d < 4; ++d)
			{
				var next = sum(curr, dir[d]);

				// cтена или конец карты
				if (!isCorrect(next))
				{
					dirWeight[d] = -1000;
					continue;
				}
				
				dirWeight[d] = calculatePathScore(next, curr, maxDepth);
				if (next.Equals(prev))
					dirWeight[d] -= maxDepth + 1;

				
			}
			var max = dirWeight.Max();
			var best = dirWeight.Select((d, i) => i).Where(i => dirWeight[i] == max).ToArray();
			var any = best.Length == 1 ? best[0] : best[new Random().Next(best.Length)];
			var direction = (Direction)any;

			var state = LValue.FromPair(LValue.FromInt(curr.X), LValue.FromInt(curr.Y));

			return new Tuple<LValue, Direction>(state, direction);
		}
	}
}