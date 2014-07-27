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
				Repeat
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
					Call("RecursiveFindGoal_2",
						Call("InitQueueAndVisited",
							"world",
							World.LmLoc("world"),
							Call("InitVisited",
								Call("mapHeight", World.Map("world")),
								Call("mapWidth", World.Map("world")))))));


		public static SExpr RecursiveFindGoal_2 =
			Def("RecursiveFindGoal_2", ArgNames("queueAndVisitedAndWorld"),
				Call("RecursiveFindGoal",
					Get(0, "queueAndVisitedAndWorld"),
					Get(1, "queueAndVisitedAndWorld"),
					Get(2, "queueAndVisitedAndWorld")));

		public static SExpr RecursiveFindGoal =
			Def("RecursiveFindGoal", ArgNames("queue", "visited", "world"),
				If( Call("queue_isempty", "queue"), 
					(int)Direction.Down,
					If(	Call("IsGoodPoint", Car(Call("queue_peek", "queue")), "world"),
						Cdr(Call("queue_peek", "queue")),
						Call("RecursiveFindGoal_2",
							Call("AddNeighbours",
								Call("queue_dequeue", "queue"),
								Call("queue_peek", "queue"),
								"world",
								"visited")))));

		public static SExpr InitQueueAndVisited =
			Def("InitQueueAndVisited", ArgNames("world", "lmPoint", "visited"),
				Call("fold",
					List(Cons(0, 0), "visited", "world"),
					Fun("AddPointIntoQueue"),
					Call("NeighboursWithDirection", "lmPoint")));

		public static SExpr AddNeighbours =
			Def("AddNeighbours", ArgNames("queue", "pointAndDirection", "world", "visited"),
				Call("fold",
					List("queue", "visited", "world"),
					Fun("AddPointIntoQueue"),
					Call("Neighbours", Car("pointAndDirection"), Cdr("pointAndDirection"))));

		public static SExpr AddPointIntoQueue =
			Def("AddPointIntoQueue", ArgNames("queueAndVisitedAndWorld", "pointAndDirection"),
				If(Call("IsBadPoint", 
						Car("pointAndDirection"),
						Get(2, "queueAndVisitedAndWorld"),
						Get(1, "queueAndVisitedAndWorld")),
					"queueAndVisitedAndWorld",
					List(Call("queue_enqueue", Get(0, "queueAndVisitedAndWorld"), "pointAndDirection"),
						Call("setCell", Get(1, "queueAndVisitedAndWorld"), Car("pointAndDirection"), 1),
						Get(2, "queueAndVisitedAndWorld"))));

		public static SExpr IsBadPoint =
			Def("IsBadPoint", ArgNames("point", "world", "visited"),
				Call("IsBadCell", 
					World.GetCell(World.Map("world"), "point"),
					"world",
					"visited",
					"point"));

		public static SExpr IsBadCell =
			Def("IsBadCell", ArgNames("pointCell", "world", "visited", "point"), 
				Or(Ceq(World.GetCell("visited", "point"), 1),
					Call("pointIsWall", "pointCell"),
					Call("any_activeGhostAtPoint", World.GhStates("world"), "point")));

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
				List(Cons(Call("sum", "point", Cons(0, -1)), 0),
					Cons(Call("sum", "point", Cons(1, 0)), 1),
					Cons(Call("sum", "point", Cons(0, 1)), 2),
					Cons(Call("sum", "point", Cons(-1, 0)), 3)));

		public static SExpr Neighbours =
			Def("Neighbours", ArgNames("point", "direction"),
				List(Cons(Call("sum", "point", Cons(0, -1)), "direction"),
					Cons(Call("sum", "point", Cons(1, 0)), "direction"),
					Cons(Call("sum", "point", Cons(0, 1)), "direction"),
					Cons(Call("sum", "point", Cons(-1, 0)), "direction")));

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

		public static SExpr InitVisited =
			Def("InitVisited", ArgNames("mapHeight", "mapWidth"),
				Call("Repeat", "mapHeight", Call("Repeat", "mapWidth", 0)));

		public static SExpr Repeat =
			Def("Repeat", ArgNames("max", "value"),
				If("max",
					Cons("value", Call("Repeat", Sub("max", 1), "value")),
					0));
	}
}