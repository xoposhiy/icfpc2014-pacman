using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Lib
{
	class GameSettings
	{
		public int lambdaManPeriod = 127;
		public int lambdaManEatingPeriod = 137;
		public int[] ghostPeriod = {130, 132, 134, 136};
		public int[] ghostFrightPeriod = {195, 198, 201, 204};
	}

	class GameSim
	{
		private class UpdateInfo : Tuple<int, int>
		{
			public UpdateInfo(int time, int ghostIndex): base(time, ghostIndex)
			{
			}
			public int GhostIndex { get { return Item2; }}
			public int Time { get { return Item1; }}
			public bool LMan { get { return GhostIndex < 0; }}

		}
		private readonly List<UpdateInfo> updateQueue = new List<UpdateInfo>();
		public World world;
		private MapCell[,] map;
		private LMStep step;
		private LValue state;
		public GameSettings settings = new GameSettings();
		public int time;

		public GameSim(MapCell[,] map, LMStep step, LValue state)
		{
			this.map = map;
			this.step = step;
			this.state = state;
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
			time = 1;
			updateQueue.Add(new UpdateInfo(time + settings.lambdaManPeriod, -1));
			for (int i=0; i<ghosts.Count; i++)
				updateQueue.Add(new UpdateInfo(time + settings.ghostPeriod[i], i));
		}

		private static IEnumerable<Point> GetLocationsOf(MapCell[,] map, MapCell mapCell)
		{
			for (int y = 0; y < map.GetLength(0); y++)
				for (int x = 0; x < map.GetLength(1); x++)
					if (map[y, x] == mapCell) yield return new Point(x, y);
		}

		public void Tick()
		{
			var updateInfo = updateQueue.Min();
			var ghostIndex = updateInfo.GhostIndex;
			MoveCreatures(ghostIndex);
			DeactivateFrightMode();
			HandleFruit();
			Eat();
			MeetGhosts();
			HandleWin();
			HandleLose();
			time = updateInfo.Time;
			updateQueue.Remove(updateInfo);
		}

		private void HandleLose()
		{
//			throw new NotImplementedException();
		}

		private void HandleWin()
		{
//			throw new NotImplementedException();
		}

		private void MeetGhosts()
		{
//			throw new NotImplementedException();
		}

		private void Eat()
		{
//			throw new NotImplementedException();
		}

		private void HandleFruit()
		{
//			throw new NotImplementedException();
		}

		private void DeactivateFrightMode()
		{
//			throw new NotImplementedException();
		}

		private void MoveCreatures(int ghostIndex)
		{
			if (ghostIndex == -1) //LambdaMan
				MoveLMan();
			else
				MoveGhost(world.Ghosts[ghostIndex]);
		}

		private void MoveGhost(GhostState ghost)
		{
			ghost.Location = TryMove(ghost.Location, ghost.Direction);
		}

		private void MoveLMan()
		{
			var man = world.LMan;
			var res = step(state, world);
			state = res.Item1;
			man.Direction = res.Item2;
			man.Location = TryMove(man.Location, man.Direction);
		}

		private Point TryMove(Point location, Direction direction)
		{
			var newLoc = location.MoveTo(direction);
			return map[newLoc.Y, newLoc.X] == MapCell.Wall ? location : newLoc;
		}
	}

	class GameSimTests
	{
		[Test]
		public void Create()
		{
			var map = MapUtils.Load(File.ReadAllText(@"..\..\..\..\mazes\maze1.txt"));
			var sim = new GameSim(map, Step, LValue.FromInt(42));
			var w1 = sim.world.ToString();
			sim.Tick();
			var w2 = sim.world.ToString();
			Assert.AreEqual(w1, w2); // do not go to wall

		}
		[Test]
		public void DoNotGoToWall()
		{
			var map = MapUtils.Load(
@"#####
#.\.#
#####");
			var sim = new GameSim(map, Step, LValue.FromInt(42));
			var w1 = sim.world.ToString();
			sim.Tick();
			var w2 = sim.world.ToString();
			Assert.AreEqual(w1, w2); // do not go to wall

		}
		[Test]
		public void CanMoveToPills()
		{
			var map = MapUtils.Load(
@"#####
#...#
#.\.#
#...#
#####");
			var sim = new GameSim(map, Step, LValue.FromInt(42));
			sim.Tick();
			Assert.AreEqual(128, sim.time);
			Assert.AreEqual(new Point(2, 3), sim.world.LMan.Location);

		}

		private Tuple<LValue, Direction> Step(LValue currentaistate, World currentworldstate)
		{
			return Tuple.Create(currentaistate, Direction.Down);
		}
	}
}
