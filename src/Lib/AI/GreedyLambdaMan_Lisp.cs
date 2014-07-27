using System.IO;
using Lib.Game;
using Lib.LispLang;
using Lib.LMachine.Intructions;
using Lib.Parsing.LParsing;

namespace Lib.AI
{
	public class GreedyLambdaMan_Lisp : Api
	{
		public static readonly string code;

		static GreedyLambdaMan_Lisp()
		{
			code = CompileWithLibs(
				main,
				GreedyStep,
				RecursiveFindGoal_2,
				RecursiveFindGoal,
				InitQueueAndVisited,
				AddNeighbours,
				AddPointIntoQueue,
				IsBadPoint,
				IsBadCell,
				IsGoodPoint,
				IsGoodCell,
				NeighboursWithDirection,
				Neighbours,
				pointIsWall,
				pointIsPill,
				pointIsPowerPill,
				pointIsFruit_Time,
				InitVisited,
				Repeat,
				ResultFromQueueElement,
				GetBestResult,
				AddNearestPointIntoQueue,
				IsBadNearestPoint,
				DisposeGhostsOnVisited,
				DisposeGhost
				);
			File.WriteAllText(KnownPlace.GccSamples + "GreedyLM" + ".mgcc", code);

			var gccCode = LParser.Parse(code).Program.ToGcc();
			File.WriteAllText(KnownPlace.GccSamples + "GreedyLM" + ".gcc", gccCode);
		}

		public static SExpr main =
			Def("main", ArgNames("world"),
				Cons(0, Fun("GreedyStep")));

		public static SExpr GreedyStep =
			Def("GreedyStep", ArgNames("state", "world"),
				Cons("state",
					GetResultDirection(
						Call("RecursiveFindGoal_2",
							Call("InitQueueAndVisited",
								"world",
								World.LmLoc("world"),
								Call("DisposeGhostsOnVisited",
									Call("InitVisited",
										Call("mapHeight", World.Map("world")),
										Call("mapWidth", World.Map("world"))),
									World.GhStates("world")))))));


		public static SExpr RecursiveFindGoal_2 =
			Def("RecursiveFindGoal_2", ArgNames("queueAndVisitedAndWorld"),
				Call("RecursiveFindGoal",
					Get(0, "queueAndVisitedAndWorld"),
					Get(1, "queueAndVisitedAndWorld"),
					Get(2, "queueAndVisitedAndWorld")));

		public static SExpr RecursiveFindGoal =
			Def("RecursiveFindGoal", ArgNames("queue", "visited", "world"),
				If( Call("queue_isempty", "queue"), 
					Result((int)Direction.Down, 0, 0),
					Call("GetBestResult", 
						Call("ResultFromQueueElement", Call("queue_peek", "queue"), 0),
						GetResultCandidate("queue", "world", "visited"))));

		public static SExpr GetResultCandidate(SExpr queue, SExpr world, SExpr visited)
		{
			return 
				If(Call("IsGoodPoint", QueueElementGetPoint(Call("queue_peek", queue)), world),
					Call("ResultFromQueueElement", Call("queue_peek", queue), 1),
					Call("RecursiveFindGoal_2",
						Call("AddNeighbours",
							Call("queue_dequeue", queue),
							Call("queue_peek", queue),
							world,
							visited)));
		}

		public static SExpr ResultFromQueueElement =
			Def("ResultFromQueueElement", ArgNames("queueElement", "isGood"),
				Result(QueueElementGetDirection("queueElement"), "isGood", QueueElementGetDepth("queueElement")));

		public static SExpr GetBestResult =
			Def("GetBestResult", ArgNames("currentResult", "resultCandidate"),
				If(GetResultIsGood("currentResult"), "currentResult",
					If(GetResultIsGood("resultCandidate"), "resultCandidate",
						If(Cgt(GetResultDepth("currentResult"), GetResultDepth("resultCandidate")), "currentResult",
							"resultCandidate"))));


		public static SExpr Result(SExpr direction, SExpr isGood, SExpr depth)
		{
			return Tuple(direction, isGood, depth);
		}

		public static SExpr GetResultDirection(SExpr result)
		{
			return GetTuple3(0, result);
		}

		public static SExpr GetResultIsGood(SExpr result)
		{
			return GetTuple3(1, result);
		}

		public static SExpr GetResultDepth(SExpr result)
		{
			return GetTuple3(2, result);
		}

		public static SExpr InitQueueAndVisited =
			Def("InitQueueAndVisited", ArgNames("world", "lmPoint", "visited"),
				Call("fold",
					List(Cons(0, 0), "visited", "world"),
					Fun("AddNearestPointIntoQueue"),
					Call("NeighboursWithDirection", "lmPoint")));

		public static SExpr AddNearestPointIntoQueue =
			Def("AddNearestPointIntoQueue", ArgNames("queueAndVisitedAndWorld", "pointAndDirection"),
				If(Call("IsBadNearestPoint",
						QueueElementGetPoint("pointAndDirection"),
						Get(2, "queueAndVisitedAndWorld"),
						Get(1, "queueAndVisitedAndWorld")),
					"queueAndVisitedAndWorld",
					List(Call("queue_enqueue", Get(0, "queueAndVisitedAndWorld"), "pointAndDirection"),
						Call("setCell", Get(1, "queueAndVisitedAndWorld"), QueueElementGetPoint("pointAndDirection"), 1),
						Get(2, "queueAndVisitedAndWorld"))));

		public static SExpr IsBadNearestPoint =
			Def("IsBadNearestPoint", ArgNames("point", "world", "visited"),
				Or(Ceq(World.GetCell("visited", "point"), 1),
					Call("pointIsWall", World.GetCell(World.Map("world"), "point")),
					Call("any_activeGhostAtPoint", World.GhStates("world"), "point"),
					Call("any_activeGhostAtPoint", World.GhStates("world"), Call("sum", "point", Cons(0, -1))),
					Call("any_activeGhostAtPoint", World.GhStates("world"), Call("sum", "point", Cons(1, 0))),
					Call("any_activeGhostAtPoint", World.GhStates("world"), Call("sum", "point", Cons(0, 1))),
					Call("any_activeGhostAtPoint", World.GhStates("world"), Call("sum", "point", Cons(-1, 0)))
					));

		public static SExpr AddNeighbours =
			Def("AddNeighbours", ArgNames("queue", "pointAndDirection", "world", "visited"),
				Call("fold",
					List("queue", "visited", "world"),
					Fun("AddPointIntoQueue"),
					CallNeighbours( 
						QueueElementGetPoint("pointAndDirection"), 
						QueueElementGetDirection("pointAndDirection"),
						QueueElementGetDepth("pointAndDirection"))));

		public static SExpr AddPointIntoQueue =
			Def("AddPointIntoQueue", ArgNames("queueAndVisitedAndWorld", "pointAndDirection"),
				If(CallIsBadPoint(
						QueueElementGetPoint("pointAndDirection"),
						Get(2, "queueAndVisitedAndWorld"),
						Get(1, "queueAndVisitedAndWorld"),
						QueueElementGetDepth("pointAndDirection")),
					"queueAndVisitedAndWorld",
					List(Call("queue_enqueue", Get(0, "queueAndVisitedAndWorld"), "pointAndDirection"),
						Call("setCell", Get(1, "queueAndVisitedAndWorld"), QueueElementGetPoint("pointAndDirection"), 1),
						Get(2, "queueAndVisitedAndWorld"))));

		public static SExpr IsBadPoint =
			Def("IsBadPoint", ArgNames("point", "world", "visited", "depth"),
				Or(Cgt("depth", maxDepth),
					Call("IsBadCell", 
						World.GetCell(World.Map("world"), "point"),
						"world",
						"visited",
						"point",
						"depth")));

		private const int maxDepth = 20;
		private const int visibleGhostDepth = 4;
		public static SExpr CallIsBadPoint(SExpr point, SExpr world, SExpr visited, SExpr depth)
		{
			return Call("IsBadPoint", point, world, visited, depth);
		}

		public static SExpr IsBadCell =
			Def("IsBadCell", ArgNames("pointCell", "world", "visited", "point", "depth"),
				Or(Ceq(World.GetCell("visited", "point"), 1),
					Call("pointIsWall", "pointCell"),
					And(Cgt(visibleGhostDepth, "depth"),
						Ceq(World.GetCell("visited", "point"), 2))));

		public static SExpr IsGoodPoint =
			Def("IsGoodPoint", ArgNames("point", "world"),
				Call("IsGoodCell",
					World.GetCell(World.Map("world"), "point"),
					"world",
					"point"));

		public static SExpr IsGoodCell =
			Def("IsGoodCell", ArgNames("pointCell", "world", "point"),
				Or(Call("pointIsPill", "pointCell"),
					Call("pointIsPowerPill", "pointCell"),
					Call("pointIsFruit_Time", "pointCell", "world"),
					Call("any_frightGhostAtPoint", World.GhStates("world"), "point")));

		public static SExpr NeighboursWithDirection =
			Def("NeighboursWithDirection", ArgNames("point"),
				List(QueueElement(Call("sum", "point", Cons(0, -1)), 0, 1),
					QueueElement(Call("sum", "point", Cons(1, 0)), 1, 1),
					QueueElement(Call("sum", "point", Cons(0, 1)), 2, 1),
					QueueElement(Call("sum", "point", Cons(-1, 0)), 3, 1)));

		public static SExpr Neighbours =
			Def("Neighbours", ArgNames("point", "direction", "depth"),
				List(QueueElement(Call("sum", "point", Cons(0, -1)), "direction", Add("depth", 1)),
					QueueElement(Call("sum", "point", Cons(1, 0)), "direction", Add("depth", 1)),
					QueueElement(Call("sum", "point", Cons(0, 1)), "direction", Add("depth", 1)),
					QueueElement(Call("sum", "point", Cons(-1, 0)), "direction", Add("depth", 1))));

		public static SExpr pointIsWall =
			Def("pointIsWall", ArgNames("pointCell"),
				Ceq("pointCell", (int)(MapCell.Wall)));

		public static SExpr pointIsPill =
			Def("pointIsPill", ArgNames("pointCell"),
				Ceq("pointCell", (int)(MapCell.Pill)));

		public static SExpr pointIsPowerPill =
			Def("pointIsPowerPill", ArgNames("pointCell"),
				Ceq("pointCell", (int)(MapCell.PowerPill)));

		public static SExpr pointIsFruit_Time =
			Def("pointIsFruit_Time", ArgNames("pointCell", "world"),
				And(Ceq("pointCell", (int)(MapCell.Fruit)),
					Cgt(World.FruitExpired("world"), 126)));

		// 0 - not visited, 1 - visited, 2 - ghost, 
		public static SExpr DisposeGhostsOnVisited =
			Def("DisposeGhostsOnVisited", ArgNames("visited", "ghosts"),
				Call("fold", "visited", Fun("DisposeGhost"), "ghosts"));

		public static SExpr DisposeGhost =
			Def("DisposeGhost", ArgNames("visited", "ghost"),
				Call("setCell", "visited", World.GhLoc("ghost"), 2));

		public static SExpr InitVisited =
			Def("InitVisited", ArgNames("mapHeight", "mapWidth"),
				Call("Repeat", "mapHeight", Call("Repeat", "mapWidth", 0)));

		public static SExpr Repeat =
			Def("Repeat", ArgNames("max", "value"),
				If("max",
					Cons("value", Call("Repeat", Sub("max", 1), "value")),
					0));

		public static SExpr CallNeighbours(SExpr point, SExpr direction, SExpr depth)
		{
			return Call("Neighbours", point, direction, depth);
		}

		public static SExpr QueueElement(SExpr point, SExpr direction, SExpr depth)
		{
			return Tuple(point, direction, depth);
		}

		public static SExpr QueueElementGetPoint(SExpr queueElement)
		{
			return GetTuple3(0, queueElement);
		}

		public static SExpr QueueElementGetDirection(SExpr queueElement)
		{
			return GetTuple3(1, queueElement);
		}

		public static SExpr QueueElementGetDepth(SExpr queueElement)
		{
			return GetTuple3(2, queueElement);
		}
	}
}