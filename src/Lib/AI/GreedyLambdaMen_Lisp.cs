using System.IO;
using Lib.Game;
using Lib.LispLang;
using Lib.LMachine.Intructions;
using Lib.Parsing.LParsing;

namespace Lib.AI
{
	public class GreedyLambdaMen_Lisp : Api
	{
		public static readonly string code;

		static GreedyLambdaMen_Lisp()
		{
			code = CompileWithLibs(
				main,
				GreedyStep,
				RecursiveFindGoal_2,
				RecursiveFindGoal,
				InitQueueAndVisited,
				AddNeighbours,
				AddPointIntoQueue,
				IsBadCell,
				IsGoodCell,
				NeighboursWithDirection,
				Neighbours,
				pointIsWall,
				pointIsPill,
				pointIsPowerPill,
				pointIsFruit,
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
				If(Call("IsGoodCell", Car(Call("queue_peek", "queue")), "world"),
					Cdr(Call("queue_peek", "queue")),
					Call("RecursiveFindGoal_2",
						Call("AddNeighbours",
							Call("queue_dequeue", "queue"),
							Call("queue_peek", "queue"),
							"world",
							"visited"))));

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
				If(Call("IsBadCell", 
						Car("pointAndDirection"),
						Get(2, "queueAndVisitedAndWorld"),
						Get(1, "queueAndVisitedAndWorld")),
					"queueAndVisitedAndWorld",
					List(Call("queue_enqueue", Get(0, "queueAndVisitedAndWorld"), "pointAndDirection"),
						Call("setCell", Get(1, "queueAndVisitedAndWorld"), Car("pointAndDirection"), 1),
						Get(2, "queueAndVisitedAndWorld"))));

		public static SExpr IsBadCell =
			Def("IsBadCell", ArgNames("point", "world", "visited"), 
				Or(Ceq(World.GetCell("visited", "point"), 1),
					Call("pointIsWall", "point", World.Map("world")),
					Call("any_activeGhostAtPoint", World.GhStates("world"), "point")));

		public static SExpr IsGoodCell =
			Def("IsGoodCell", ArgNames("point", "world"),
				Or(Call("pointIsPill", "point", World.Map("world")),
					Call("pointIsPowerPill", "point", World.Map("world")),
					Call("pointIsFruit_Time", "point", "world"),
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
			Def("pointIsWall", ArgNames("point", "map"),
				Ceq(World.GetCell("map", "point"),
					(int)(MapCell.Wall)));

		public static SExpr pointIsPill =
			Def("pointIsPill", ArgNames("point", "map"),
				Ceq(World.GetCell("map", "point"),
					(int)(MapCell.Pill)));

		public static SExpr pointIsPowerPill =
			Def("pointIsPowerPill", ArgNames("point", "map"),
				Ceq(World.GetCell("map", "point"),
					(int)(MapCell.PowerPill)));

		public static SExpr pointIsFruit =
			Def("pointIsFruit", ArgNames("point", "world"),
				Ceq(World.GetCell(World.Map("world"), "point"),
					(int)(MapCell.Fruit)));

		public static SExpr pointIsFruit_Time =
			Def("pointIsFruit_Time", ArgNames("point", "world"),
				And(Ceq(World.GetCell(World.Map("world"), "point"),
					(int)(MapCell.Fruit)),
					Cgt(World.FruitExpired("world"),
						126)));

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