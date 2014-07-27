using System.IO;
using Lib.LispLang;
using Lib.LMachine.Intructions;
using Lib.Parsing.LParsing;

namespace Lib.AI
{
	public class CarefulGreedyLambdaMan_Lisp : GreedyLambdaMan_Lisp
	{
		public static readonly string code;

		static CarefulGreedyLambdaMan_Lisp()
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
				pointIsFruit,
				pointIsFruit_Time,
				InitVisited,
				Repeat,

				AddNearestPointIntoQueue,
				IsBadNearestPoint
				);
			File.WriteAllText(KnownPlace.GccSamples + "CarefulGreedyLM" + ".mgcc", code);

			var gccCode = LParser.Parse(code).Program.ToGcc();
			File.WriteAllText(KnownPlace.GccSamples + "CarefulGreedyLM" + ".gcc", gccCode);
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
						Car("pointAndDirection"),
						Get(2, "queueAndVisitedAndWorld"),
						Get(1, "queueAndVisitedAndWorld")),
					"queueAndVisitedAndWorld",
					List(Call("queue_enqueue", Get(0, "queueAndVisitedAndWorld"), "pointAndDirection"),
						Call("setCell", Get(1, "queueAndVisitedAndWorld"), Car("pointAndDirection"), 1),
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

		public static SExpr IsBadCell =
			Def("IsBadCell", ArgNames("pointCell", "world", "visited", "point"),
				Or(Ceq(World.GetCell("visited", "point"), 1),
					Call("pointIsWall", "pointCell")));
	}
}