using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Lib
{
	class GameSim
	{
		private World world;
		private MapCell[,] map;
		private LMStep step;
		private LValue initialState;

		public GameSim(MapCell[,] map, LMStep step, LValue initialState)
		{
			this.map = map;
			this.step = step;
			this.initialState = initialState;
			var lmState = new LManState(
				0,
				GetLocationsOf(map, MapCell.LManStartLoc).Single(),
				Direction.Up,
				3,
				0);
			List<GhostState> ghosts =
				GetLocationsOf(map, MapCell.GhostStartLoc)
				.Select(p => new GhostState(GhostVitality.Standard, p, Direction.Up))
				.ToList();
			world = new World(map,
				lmState,
				ghosts,
				0);
		}

		private IEnumerable<Point> GetLocationsOf(MapCell[,] map, MapCell mapCell)
		{
			for (int y = 0; y < map.GetLength(0); y++)
				for (int x = 0; x < map.GetLength(1); x++)
					if (map[y, x] == mapCell) yield return new Point(x, y);
		}

		public void Tick()
		{
			//ToDo
		}
	}

	class GameSim_Tests
	{
		[Test]
		public void Create()
		{
			var map = MapUtils.Load(File.ReadAllText(@"..\..\..\..\mazes\maze1.txt"));
			var sim = new GameSim(map, Step, LValue.FromInt(42));
			sim.Tick();
		}

		private Tuple<LValue, Direction> Step(LValue currentaistate, World currentworldstate)
		{
			return Tuple.Create(currentaistate, Direction.Left);
		}
	}
}
