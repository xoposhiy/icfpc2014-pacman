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
		public bool finished;
		private int pillsCount;

		//TODO add ghost AIs
		public GameSim(MapCell[,] map, LMMain main)
		{
			this.map = map;
			var lmState = new LManState(
				0,
				GetLocationsOf(map, MapCell.LManStartLoc).Single(),
				Direction.Up,
				3,
				0);
			List<GhostState> ghosts =
				GetLocationsOf(map, MapCell.GhostStartLoc)
				.Select((p, i) => new GhostState(GhostVitality.Standard, p, Direction.Up, i%4))
				.ToList();
			world = new World(map,
				lmState,
				ghosts,
				0);
			var res = main(world);
			state = res.Item1;
			step = res.Item2;
			time = 1;
			pillsCount = GetLocationsOf(map, MapCell.Pill).Count();
			updateQueue.Add(new UpdateInfo(time + settings.lambdaManPeriod, -1));
			for (int i = 0; i < ghosts.Count; i++)
				updateQueue.Add(new UpdateInfo(time + settings.ghostPeriod[i], i));
			finished = false;
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
			if (world.man.lives == 0 || time >= 127 * map.GetLength(0) * map.GetLength(1) * 16)
				finished = true;
		}

		private void HandleWin()
		{
			if (pillsCount == 0)
			{
				world.man.score *= (world.man.lives + 1);
				finished = true;
			}
		}

		private void MeetGhosts()
		{
			foreach (var ghost in world.ghosts.Where(g => g.location.Equals(world.man.location)))
			{
				if (ghost.vitality == GhostVitality.Standard)
					KillLambdaMan();
				if (ghost.vitality == GhostVitality.Fright)
					KillGhost(ghost);
			}
		}

		private void KillGhost(GhostState ghost)
		{
			world.man.ghostsKilledInThisFrightSession++;
			world.man.score += 100 * (1 << Math.Min(4, world.man.ghostsKilledInThisFrightSession));
			ghost.vitality = GhostVitality.Invisible;
			ghost.location = ghost.initialLocation;
		}

		private void KillLambdaMan()
		{
			world.man.lives--;
			world.man.location = world.man.initialLocation;
			foreach (var ghost in world.ghosts)
			{
				ghost.vitality = GhostVitality.Standard;
				ghost.location = ghost.initialLocation;
			}
		}

		private void Eat()
		{
			var man = world.man.location;
			var mapCell = world.map[man.Y, man.X];
			if (mapCell == MapCell.Pill)
			{
				world.man.score += 10;
				world.map[man.Y, man.X] = MapCell.Empty;
				pillsCount--;
			}
			if (mapCell == MapCell.PowerPill)
			{
				world.man.score += 50;
				world.map[man.Y, man.X] = MapCell.Empty;
				world.man.powerPillRemainingTicks = settings.frightModeDuration;
				foreach (var g in world.ghosts)
					g.vitality = GhostVitality.Fright;
			}
			if (mapCell == MapCell.Fruit && world.fruitTicksRemaining > 0)
			{
				world.man.score += GetFruitCost();
				world.fruitTicksRemaining = 0;
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
				world.fruitTicksRemaining = fruit.expiresTime - time;
			else
				world.fruitTicksRemaining = Math.Max(0, world.fruitTicksRemaining - 1);
		}

		private void DeactivateFrightMode()
		{
			var man = world.man;
			man.powerPillRemainingTicks--;
			if (man.powerPillRemainingTicks == 0)
			{
				foreach (var g in world.ghosts)
					g.vitality = GhostVitality.Standard;
				man.ghostsKilledInThisFrightSession = 0;
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
			var ghost = world.ghosts[ghostIndex];
			ghost.location = TryMove(ghost.location, ghost.direction);
			var period = settings.ghostPeriod[ghost.ghostIndex];
			updateQueue.Add(new UpdateInfo(time + period, ghostIndex));
		}

		private bool IsEatable(MapCell cell)
		{
			return cell == MapCell.Fruit || cell == MapCell.Pill || cell == MapCell.PowerPill;
		}

		private void MoveLMan()
		{
			var man = world.man;
			var res = step(state, world);
			state = res.Item1;
			man.direction = res.Item2;
			man.location = TryMove(man.location, man.direction);
			var eat = IsEatable(world.map[man.location.Y, man.location.X]);
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
			var sim = new GameSim(map, Main);
			var w1 = sim.world.ToString();
			sim.Tick();
			var w2 = sim.world.ToString();
			Assert.AreEqual(w1, w2); // do not go to wall

		}

		private Tuple<LValue, LMStep> Main(World initialworld)
		{
			return Tuple.Create<LValue, LMStep>(LValue.FromInt(42), Step);
		}

		[Test]
		public void DoNotGoToWall()
		{
			var map = MapUtils.Load(
@"#####
#.\.#
#####");
			var sim = new GameSim(map, Main);
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
			var sim = new GameSim(map, Main);
			sim.Tick();
			Assert.AreEqual(128, sim.time);
			Assert.AreEqual(new Point(2, 3), sim.world.man.location);

		}

		private Tuple<LValue, Direction> Step(LValue currentaistate, World currentworldstate)
		{
			return Tuple.Create(currentaistate, Direction.Down);
		}
	}
}
