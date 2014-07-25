using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Lib
{
	public class FruitInfo
	{
		public FruitInfo(int appearsTime, int expiresTime)
		{
			this.appearsTime = appearsTime;
			this.expiresTime = expiresTime;
		}

		public int appearsTime, expiresTime;
	}

	public class GameSettings
	{
		public int lambdaManPeriod = 127;
		public int lambdaManEatingPeriod = 137;
		public int[] ghostPeriod = { 130, 132, 134, 136 };
		public int[] ghostFrightPeriod = { 195, 198, 201, 204 };
		public FruitInfo[] fruits = { new FruitInfo(127 * 200, 127 * 280), new FruitInfo(127 * 400, 127 * 480), };
		public int frightModeDuration = 127 * 20;
		public int endOfLivesMapSizeMultiplier = 127 * 16;
	}

	public class GameSim
	{
		private class UpdateInfo : Tuple<int, int>
		{
			public UpdateInfo(int time, int ghostIndex)
				: base(time, ghostIndex)
			{
			}
			public int GhostIndex { get { return Item2; } }
			public int Time { get { return Item1; } }
			public bool LMan { get { return GhostIndex < 0; } }

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
			for (int i = 0; i < ghosts.Count; i++)
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
			while (time == updateInfo.Time)
			{
				var ghostIndex = updateInfo.GhostIndex;
				MoveCreatures(ghostIndex);
				updateQueue.Remove(updateInfo);
				updateInfo = updateQueue.Min();
			}
			DeactivateFrightMode();
			HandleFruit();
			Eat();
			MeetGhosts();
			HandleWin();
			HandleLose();
			time++;
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
			for (int iGhost = 0; iGhost < world.Ghosts.Count; iGhost++)
			{
				var ghost = world.Ghosts[iGhost];
				if (ghost.Vitality == GhostVitality.Standard)
					KillLambdaMan();
				if (ghost.Vitality == GhostVitality.Fright)
				{
					KillGhost(iGhost);
				}
			}
		}

		private void KillGhost(int iGhost)
		{
			//TODO
		}

		private void KillLambdaMan()
		{
			//TODO
		}

		private void Eat()
		{
			var man = world.LMan.Location;
			var mapCell = world.Map[man.Y, man.X];
			if (mapCell == MapCell.Pill)
			{
				world.LMan.Score += 10;
				world.Map[man.Y, man.X] = MapCell.Empty;
			}
			if (mapCell == MapCell.PowerPill)
			{
				world.LMan.Score += 50;
				world.Map[man.Y, man.X] = MapCell.Empty;
				world.LMan.PowerPillRemainingTicks = settings.frightModeDuration;
				foreach (var g in world.Ghosts)
					g.Vitality = GhostVitality.Fright;
			}
			if (mapCell == MapCell.Fruit && world.FruitTicksRemaining > 0)
			{
				world.LMan.Score += GetFruitCost();
				world.FruitTicksRemaining = 0;
			}
		}

		private int GetFruitCost()
		{
			var level = 1 + (map.GetLength(0) * map.GetLength(1) - 1) / 100;
			int[] costs = { 0, 100, 300, 500, 500, 700, 700, 1000, 1000, 2000, 2000, 3000, 3000, 5000 };
			return costs[Math.Min(13, level)];
		}

		private void HandleFruit()
		{
			var fruit = settings.fruits.FirstOrDefault(f => f.appearsTime == time);
			if (fruit != null)
				world.FruitTicksRemaining = fruit.expiresTime - time;
			else
				world.FruitTicksRemaining = Math.Max(0, world.FruitTicksRemaining - 1);
		}

		private void DeactivateFrightMode()
		{
			var man = world.LMan;
			man.PowerPillRemainingTicks--;
			if (man.PowerPillRemainingTicks == 0)
			{
				foreach (var g in world.Ghosts)
					g.Vitality = GhostVitality.Standard;
			}
		}

		private void MoveCreatures(int ghostIndex)
		{
			if (ghostIndex == -1) //LambdaMan
				MoveLMan();
			else
				MoveGhost(ghostIndex);
		}

		private void MoveGhost(int ghostIndex)
		{
			var ghost = world.Ghosts[ghostIndex];
			ghost.Location = TryMove(ghost.Location, ghost.Direction);
			var period = settings.ghostPeriod[ghostIndex]; // TODO Ghost index!!!
			updateQueue.Add(new UpdateInfo(time + period, ghostIndex));
		}

		private bool IsEatable(MapCell cell)
		{
			return cell == MapCell.Fruit || cell == MapCell.Pill || cell == MapCell.PowerPill;
		}

		private void MoveLMan()
		{
			var man = world.LMan;
			var res = step(state, world);
			state = res.Item1;
			man.Direction = res.Item2;
			man.Location = TryMove(man.Location, man.Direction);
			var eat = IsEatable(world.Map[man.Location.Y, man.Location.X]);
			var period = (eat ? settings.lambdaManEatingPeriod : settings.lambdaManPeriod);
			//TODO eaten ghost increases period too?
			updateQueue.Add(new UpdateInfo(time + period, -1));
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
