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
	public class LocallyGreedyCarefulLambdaMan : ILambdaMan
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
			Func<Point, bool> isCorrect = (p) => !(p.X < 0 || p.X >= map.GetLength(0) || p.Y < 0 || p.Y >= map.GetLength(1) || map[p.X, p.Y] == MapCell.Wall);
			for (var d = 0; d < 4; ++d)
			{
				var next = sum(curr, dir[d]);

				// cтена или конец карты
				if (!isCorrect(next))
				{
					dirWeight[d] = -1000;
					continue;
				}
				var nextCell = map[next.X, next.Y];

				//есть или нет пилюля? +1/0   это стена? -100
				if (nextCell == MapCell.Pill)
					dirWeight[d] += 2;
				else if (nextCell == MapCell.PowerPill)
					dirWeight[d] += 10;

				//откуда мы пришли?
				if (next.Equals(prev))
					dirWeight[d] -= 5;

				// есть ли госты поблизости = -100
				// есть ли еще пилюли поблизости?
				for (var nd = 0; nd < 4; ++nd)
				{
					var nnext = sum(next, dir[d]);
					if (!isCorrect(nnext) && !nnext.Equals(curr))
						continue;
					var nnextCell = map[nnext.X, nnext.Y];
					if (currentWorldState.ghosts.Any(gost => gost.vitality == GhostVitality.Standard && gost.location.Equals(nnext)))
						dirWeight[d] -= 100;
					if (nnextCell == MapCell.Pill)
						dirWeight[d]++;
					else if (nnextCell == MapCell.PowerPill)
						dirWeight[d] += 5;
				}
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