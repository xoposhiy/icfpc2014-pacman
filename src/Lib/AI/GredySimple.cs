using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.Game;
using Lib.LMachine;

namespace Lib.AI
{
	public class GredySimple : LambdaMan
	{
		public Tuple<LValue, LMStep> Main(World initialWorld)
		{
			return Tuple.Create(
				LValue.FromInt(-1), // заглушка
				(LMStep)Step
				);
		}

		public static Tuple<LValue, Direction> Step(LValue currentAIState, World currentWorldState)
		{
			var map = new MapCell[currentWorldState.map.GetLength(0), currentWorldState.map.GetLength(1)];
			Array.Copy(currentWorldState.map, 0, map, 0, currentWorldState.map.Length);

			var lmPosition = currentWorldState.man.location;
			
			var queue = new Queue_Functional();

			currentWorldState.ghosts.Select(g => UseCell(map, g.location)).ToList();
			currentWorldState.ghosts.Select(g =>
			{
				queue.Enqueue(LValue.FromPair(g.location, -2));
				return true;
			}).ToList();

			UseCell(map, lmPosition);
			queue.Enqueue(LValue.FromPair(lmPosition, -1));

			var founded = currentWorldState.man.direction;

			while (!queue.IsEmpty())
			{
				var p = queue.Dequeue();
				var pointF = p.Pair.Head;
				var point = new Point(pointF.Pair.Head.Value.Value, pointF.Pair.Tail.Value.Value);
				var curCell = map[point.Y, point.X];
				var dirWas = p.Pair.Tail.Value.Value;


				for (int dir = 0; dir < 4; dir++)
				{
					var point2 = point.MoveTo((Direction)dir);
					var status = map[point2.Y, point2.X];
					if (status != MapCell.Wall)
					{
						if (status == MapCell.Empty || dirWas == -2)
							UseCell(map, point2);
						int dir2 = (dirWas == -1 ? dir : dirWas);
						queue.Enqueue(LValue.FromPair(point2, dir2));

						if (dirWas != -2) //not ghost
						{
							if (status == MapCell.Pill ||
								status == MapCell.PowerPill ||
								status == MapCell.Fruit)
							{
								founded = (Direction)dir2;
								return Tuple.Create(currentAIState, founded);								
							}
						}
					}
				}
			}
			return Tuple.Create(currentAIState, founded);
		}

		static bool UseCell(MapCell[,] map, Point p)
		{
			map[p.Y, p.X] = MapCell.Wall;
			return true;
		}
	}
}
