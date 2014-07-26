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

		private static void AddNeigh(Queue_Functional queue, MapCell[,] map, Point point, int dir, int dirWas)
		{
			if (dir == 4)
				return;
			var point2 = point.MoveTo((Direction)dir);
			var status = map[point2.Y, point2.X];
			if (status != MapCell.Wall && status != MapCell.GhostStartLoc)
			{
				if (status == MapCell.Empty)
					UseCell(map, point2);
				int dir2 = (dirWas == -1 ? dir : dirWas);
				queue.Enqueue(LValue.FromPair(point2, dir2));
			}
			AddNeigh(queue, map, point, dir + 1, dirWas);
		}


		public static Tuple<LValue, Direction> Step(LValue currentAIState, World currentWorldState)
		{
			var map = new MapCell[currentWorldState.map.GetLength(0), currentWorldState.map.GetLength(1)];
			Array.Copy(currentWorldState.map, 0, map, 0, currentWorldState.map.Length);

			currentWorldState.ghosts.Select(g => UseCell(map, g.location));
			var lmPosition = currentWorldState.man.location;

			currentWorldState.ghosts.Select(g => UseCell(map, g.location));

			UseCell(map, lmPosition);
			var queue = new Queue_Functional();
			queue.Enqueue(LValue.FromPair(lmPosition, -1));
			var founded = currentWorldState.man.direction;

			while (!queue.IsEmpty())
			{
				var p = queue.Dequeue();
				var pointF = p.Pair.Head;
				var point = new Point(pointF.Pair.Head.Value.Value, pointF.Pair.Tail.Value.Value);
				var curCell = map[point.Y, point.X];

				if (curCell == MapCell.Pill ||
					curCell == MapCell.PowerPill) // TODO: fruits!
				{
					founded = (Direction)(p.Pair.Tail.Value.Value);
					break;
				}

				AddNeigh(queue, map, point, 0, p.Pair.Tail.Value.Value);
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
